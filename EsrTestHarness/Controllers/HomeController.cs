using EsrTestHarness.Hubs;
using EsrTestHarness.Model;
using EsrTestHarness.Repository;
using EsrTestHarness.ViewModels;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsrTestHarness.Controllers
{
    [Controller]
    public class HomeController : BaseController
    {
        private readonly IHubContext<MessageHub> _hubcontext;
        private readonly ITestDatabase _database;

        public HomeController(IHubContext<MessageHub> hubcontext, ITestDatabase database)
        {
            _hubcontext = hubcontext;
            _database = database;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Username = model.Username;
            if (await _database.GetUserAsync(model.Username) == null)
            {
                await _database.InsertUserAsync(new User { Email = model.Username });
            }

            return RedirectToAction("List");
        }

        public IActionResult List()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                return RedirectToAction("Index");
            }

            return View(new ListVM
            {
                SessionId = Username,
                LmsHost = new Uri(UriHelper.GetDisplayUrl(Request)).GetLeftPart(UriPartial.Authority)
            });
        }

        [HttpPost("/SignalRHubConnection")]
        public async Task<IActionResult> SignalRHubConnection([FromBody] SignalRHubConnectionVM model)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                return RedirectToAction("Index");
            }

            await _database.UpdateUserAsync(Username, model.ConnectionId);

            return Ok();
        }

        [HttpPost("/lms-content/{*path}")]
        public async Task<IActionResult> LmsContent()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            if (!body.StartsWith("scormd="))
            {
                return BadRequest();
            }

            var decoded = Decode(body.Remove(0, "scormd=".Length));

            var splits = decoded.Split("~olmota~", StringSplitOptions.RemoveEmptyEntries);

            try
            {
                List<string> responseItems = new();

                var contents = splits.Select(t => t.Split("=", 2)).ToDictionary(t => t[0], t => t[1]);

                var actionValue = contents.TryGetValue("action", out var action) ? action : "no-action";

                switch (actionValue)
                {
                    case "GetAttemptState":
                        responseItems.Add("initialized=false");
                        responseItems.Add("finished=false");
                        responseItems.Add("autoCommit=false");
                        responseItems.Add("error=0");
                        break;
                    case "Initialize":
                        responseItems.Add("initialized=true");
                        responseItems.Add("error=0");
                        break;
                    case "GetParam":
                        //responseItems.Add("cmi.suspend_data=");
                        //responseItems.Add("cmi.launch_data=");
                        //responseItems.Add("cmi.comments=");

                        responseItems.Add("cmi.core.student_id=1");
                        responseItems.Add("cmi.core.student_name=test, user");
                        //responseItems.Add("cmi.core.lesson_location=uk");

                        responseItems.Add("cmi.core.credit=credit"); //credit, no-credit

                        responseItems.Add("cmi.core.lesson_status=incomplete"); // passed, failed, completed, incomplete, browsed, not attempted

                        responseItems.Add("cmi.core.entry=resume"); // ab-initio, resume
                        responseItems.Add("cmi.core.exit=logout"); //time-out, suspend, logout

                        responseItems.Add("cmi.core.total_time=0000:00:00.00");
                        responseItems.Add("cmi.core.session_time=0000:00:00.00");

                        responseItems.Add("cmi.core.lesson_mode=review"); //browse, normal, review

                        responseItems.Add("cmi.student_preference.language=en");
                        responseItems.Add("cmi.student_data.mastery_score=90");

                        //responseItems.Add("cmi.core.score.raw=0");
                        //responseItems.Add("oracle.ila.test=1000");
                        //responseItems.Add("oracle.ila.user.=");

                        responseItems.Add("error=0");
                        break;
                    case "PutParam":
                    case "ExitAU":
                        responseItems.Add("error=0");
                        break;
                    default:
                        responseItems.Add("error=0");
                        break;
                }

                if (contents.TryGetValue("oracle.apps.ota.private.sessionId", out var sessionId))
                {
                    await SendLmsTrafficFeedback(sessionId, splits, responseItems.ToArray());
                }

                return Ok(string.Join("\r\n", responseItems));
            }
            catch (Exception ex)
            {
                await _hubcontext.Clients.All
                             .SendAsync("ReceiveLmsTraffic", splits, ex.ToString().Replace(Environment.NewLine, "<br/>"));

                return Ok("error=0");
            }
        }

        private static string Decode(string payload)
        {
            string str1 = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            int num1 = 0;
            string str7 = "";
            long num2 = 0;
            for (int index = payload.Length - 1; index >= 0; --index)
            {
                num2 += (long)str1.IndexOf(payload[index]) * (long)Math.Pow(36.0, (double)num1);
                ++num1;
                if (Math.IEEERemainder((double)num1, 3.0) == 0.0)
                {
                    str7 = ((char)num2).ToString() + str7;
                    num1 = 0;
                    num2 = 0L;
                }
            }

            return str7;
        }

        private async Task SendLmsTrafficFeedback(string sessionId, string[] request, string[] response)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(sessionId))
                {
                    var email = Encoding.UTF8.GetString(Convert.FromBase64String(sessionId));

                    var user = await _database.GetUserAsync(email);

                    if (user?.SignalRHubConnectionId != null)
                    {
                        await _hubcontext.Clients.Client(user.SignalRHubConnectionId)
                            .SendAsync("ReceiveLmsTraffic", string.Join("<br/>", request.OrderBy(a => a)), string.Join("<br/>", response));
                    }
                }
            }
            catch { }
        }

        private static void Log(string sessionId, string[] request, string[] response)
        {
            List<string> logs = new();
            logs.Add($"{DateTime.Now:HH:mm:ss:fff} Request - session: {sessionId}");
            foreach (var line in request) logs.Add($"\t{line}");
            logs.Add($"{DateTime.Now:HH:mm:ss:fff} Response:");
            foreach (var line in response) logs.Add($"\t{line}");
            logs.Add("");

            System.IO.File.AppendAllLines($"{Environment.CurrentDirectory}\\logs.log", logs);
        }
    }
}
using System.ComponentModel.DataAnnotations;

namespace EsrTestHarness.ViewModels
{
    public class LoginVM
    {
        [Required, EmailAddress]
        public string Username { get; set; }
    }
}
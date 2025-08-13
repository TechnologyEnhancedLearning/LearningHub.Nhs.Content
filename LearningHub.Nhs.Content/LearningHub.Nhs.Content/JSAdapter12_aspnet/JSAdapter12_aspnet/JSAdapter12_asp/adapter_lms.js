<!-- $Header: adapter_lms.js 120.0.12010000.6 2021/10/26 10:13:25 smahanka noship $ -->

var apiTimer = null;

function refreshOutline()
{
   // Just eat the refresh outline request
}

function navigate(loId)
{  
   parent.location.href = parent.getLMSHost() + "/OA_HTML/OA.jsp?page=/oracle/apps/ota/player/webui/PlayerNavigationRN&addBreadCrumb=S&loId=" + escape(loId);
}

function doOnLoad()
{
   if(API != null && API != undefined)
   {
      if(apiTimer != null)
      {
         clearTimeout(apiTimer);
         apiTimer = null;
      }
      parent.apiLoaded();
   }else
   {
      apiTimer = setTimeout("doOnLoad()", 3000);
   }
}

function getSessionId()
{
   return parent.sessionId;
}
<!-- $Header: adapter_lms.js 120.0.12010000.3 2016/07/28 09:12:58 smahanka noship $ -->

function refreshOutline() {
   // Just eat the refresh outline request
}

function navigate(loId) {  
   parent.location.href = parent.getLMSHost() + "/OA_HTML/OA.jsp?page=/oracle/apps/ota/player/webui/PlayerNavigationRN&addBreadCrumb=S&loId=" + escape(loId);
}

function doOnLoad(){
   parent.apiLoaded();
   /*if(parent.shouldDebug()){
     openLog();
   }*/
}

function getSessionId(){
   return parent.sessionId;
}

function openLog(){
   API.openLog();
}

function closeLog(){
   API.closeLog();
}
<!-- $Header: adapter_js.js 120.0.12010000.6 2016/07/28 09:11:31 smahanka noship $ -->
var queryParameters=null;
var sessionId=null;
function getLMSUrl()
{
	var lmsUrl = getQueryParameter("LMS_URL");
	lmsUrl = lmsUrl + "&sessionid=" + getQueryParameter("sessionid");
	return lmsUrl;
}

function shouldDebug() {
   return (getQueryParameter("lms_debug") != null);
}

function shouldLogDebug() {
   return (getQueryParameter("lms_log_debug") != null);
}

function getKey1(){
	return getQueryParameter("key1");
}

function getKey2(){
	return getQueryParameter("key2");
}

function getKey3(){
	return getQueryParameter("key3");
}

function getLMSHost()
{
	var lmsUrl=getLMSUrl();
	if(lmsUrl!=null){
		var slashIndex=0;
		slashIndex=lmsUrl.indexOf("/",slashIndex);
		slashIndex=lmsUrl.indexOf("/",slashIndex+1);
		slashIndex=lmsUrl.indexOf("/",slashIndex+1);
		if(slashIndex==-1){
			return lmsUrl;
		}else{
			return lmsUrl.substring(0,slashIndex);
		}
	}else{
		return null;
	}
}

function addLMSUrl(url){
	if(url.indexOf("LMS_URL=")==-1){
		var lmsUrl=getLMSUrl();
		if(lmsUrl!=null&&lmsUrl!=""){
			lmsUrl=escape(lmsUrl);
			if(url.indexOf("?")!=-1){
				url=url+"&LMS_URL="+lmsUrl;
			}else{
				url=url+"?LMS_URL="+lmsUrl;
			}
		}
	}
	return url;
}

function queryParameter(name,value){
	this.name=name;
	this.value=value;
}

function getQueryParameter(paramName){
	var params=getQueryParameters();
	var upperParamName=paramName.toUpperCase();
	for(var i=0;i<params.length;i++){
		if(params[i].name.toUpperCase()==upperParamName){
			return unescape(params[i].value);
		}
	}
	return null;
}

function getQueryParameters(){
	if(queryParameters==null){
		queryParameters=new Array();
		var queryString=location.search;
		if(queryString!=null&&queryString!=""){
			queryString=queryString.slice(1);
			queryParameters=queryString.split("&");
			for(var i=0;i<queryParameters.length;i++){
				var pair=queryParameters[i].split("=");
				if(pair.length==1){
					queryParameters[i]=new queryParameter(pair[0],"");
				}else{
					queryParameters[i]=new queryParameter(pair[0],unescape(pair.slice(1).join("=")));
				}
			}
		}
	}
	return queryParameters;
}

function doLoad(){
	sessionId=getQueryParameter("sessionid");
	var startingUrl=getQueryParameter("starting_url");
	if(startingUrl==null){
		rco_frame.location="adapter_no_url.html";
	}else{
		lms_frame.location=getLMSPage();
	}
}

function apiLoaded(){
	initAPI();
	launchRco();
}

function launchRco(){
	var startingUrl=getQueryParameter("starting_url");
	var params=getQueryParameters();
	var queryString="";
	for(var i=0;i<params.length;i++){
		var upperParamName=params[i].name.toUpperCase();
		if(upperParamName!="STARTING_URL"&&upperParamName!="SESSIONID"&&upperParamName!="LMS_URL"&&upperParamName!="LMS_DEBUG"&&upperParamName!="LMS_SIGNED"&&upperParamName!="LMS_LOG_DEBUG"){
			if(queryString.length==0&&startingUrl.indexOf("?")==-1){
				queryString+="?";
			}else{
				queryString+="&";
			}
			queryString+=params[i].name+"="+params[i].value;
		}
	}
	var escapedUrl=startingUrl+queryString;
	while(escapedUrl.indexOf('+')!=-1){
		escapedUrl=escapedUrl.replace('+',' ');
	}
	rco_frame.location=escapedUrl;
}

function hideLog(){
	document.getElementById("fset").rows="0,*,0";
}
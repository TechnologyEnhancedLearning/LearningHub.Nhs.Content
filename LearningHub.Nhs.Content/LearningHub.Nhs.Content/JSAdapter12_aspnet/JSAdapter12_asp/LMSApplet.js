<!-- $Header: LMSApplet.js 120.0.12010000.8 2017/09/21 06:47:22 smahanka noship $ -->
function refreshOutline(){
	alert("Call refreshOutline() here");
}

var queryParameters=null;

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

function getXmlHttpRequest(){
	var xmlhttp;
	try{
		xmlhttp=new ActiveXObject("Msxml2.XMLHTTP");
	}catch(e){
	try{
		xmlhttp=new ActiveXObject("Microsoft.XMLHTTP");
	}catch(E){
		xmlhttp=false;
	}}
	if(!xmlhttp&&typeof XMLHttpRequest!='undefined'){
		xmlhttp=new XMLHttpRequest();
	}
	return xmlhttp;
}
function LMSApplet(lmsUrl,attemptId,respId,respApplId,secGroupId){
	if(typeof(arguments[0])=='undefined'){
		alert("lmsUrl is a mandatory parameter");
	}
	if(typeof(arguments[1])=='undefined'){
		alert("attemptId is a mandatory parameter");
	}
	this._initialize(lmsUrl,attemptId,respId,respApplId,secGroupId);
}

LMSApplet.prototype._initialize=function(lmsUrl,attemptId,respId,respApplId,secGroupId){
	this._elements=null;
	this._dirtyElements=null;
	this._lastError=null;
	this._haveState=false;
	this._autoCommit=true;
	this._initialized=false;
	this._finished=false;
	this._lmsUrl=lmsUrl;
	this._attemptId=attemptId;
	this._respId=respId;
	this._respApplId=respApplId;
	this._secGroupId=secGroupId;
	this.logWin=null;
	this._logUrl="log.html";
	this._jumpId=null;
	this.validatedURIs=new Hashtable();
	this.exceptions=null;
	this._scormVersion="subclass should define";
	};
NO_ERROR="0";
AICC_INVALID_COMMAND="1";
AICC_INVALID_AU_PASSWORD="2";
AICC_INVALID_SESSION_ID="3";
GENERAL_EXCEPTION="101";
TIMEOUT_EXCEPTION="102";
NO_STATUS_CALCULATOR="103";
OBJECTIVE_NOT_FOUND="104";
COMMUNICATION_IO_EXCEPTION="201";
COMMUNICATION_SECURITY_EXCEPTION="202";
COMMUNICATION_GENERAL_EXCEPTION="203";
COMMUNICATION_NULL_ACTION_EXCEPTION="204";
COMMUNICATION_INVALID_ACTION_EXCEPTION="204";
NETSCAPE_FORBIDDEN_TARGET="205";
BAD_LMS_URL="206";
ATTEMPT_ALREADY_FINISHED="301";
ATTEMPT_ALREADY_INITIALIZED="302";
ATTEMPT_NOT_INITIALIZED="303";
FINISH_BEFORE_INITIALIZE="304";
FINISH_AFTER_FINISH="305";
GET_VALUE_BEFORE_INITIALIZE="306";
GET_VALUE_AFTER_FINISH="307";
SET_VALUE_BEFORE_INITIALIZE="308";
SET_VALUE_AFTER_FINISH="309";
COMMIT_BEFORE_INITIALIZE="310";
COMMIT_AFTER_FINISH="311";
ELEMENT_NOT_INITIALIZED="401";
INVALID_BOOLEAN_VALUE="402";
INVALID_TIME_INTERVAL_FORMAT="403";
INVALID_TIME_FORMAT="404";
INVALID_VOCABULARY_CODE="405";
INVALID_VOCABULARY_WORD="406";
INCORRECT_DATA_TYPE="407";
ELEMENT_IS_READ_ONLY="408";
ELEMENT_IS_WRITE_ONLY="409";
INVALID_SET_KEYWORD="410";
UNDEFINED_ELEMENT="411";
ELEMENT_NOT_AN_ARRAY="412";
ARRAY_INDEX_OUT_OF_BOUNDS_GET="413";
ELEMENT_CANNOT_HAVE_CHILDREN="414";
ELEMENT_NOT_IMPLEMENTED="415";
ELEMENT_VALUE_OUT_OF_RANGE="416";
ELEMENT_DEPENDENCY_NOT_ESTABLISHED="417";
ARRAY_INDEX_OUT_OF_BOUNDS_SET="418";
ELEMENT_VALUE_IS_LOCKED="419";
ELEMENT_DUPLICATE_VALUE="420";
ELEMENT_NULL_VALUE="421";
INVALID_ARGUMENT="501";
INVALID_NULL_ARGUMENT="502";
NO_ERROR_INFO=new Array("0","","");
NOT_IMPLEMENTED=new LMSNotImplementedElement();
SCORE_RAW_SUFFIX="score.raw";
SCORE_MIN_SUFFIX="score.min";
SCORE_MAX_SUFFIX="score.max";
API_VERSION="oracle.apps.ota.private.scorm_version";
COMMENTS="cmi.comments";
SUSPEND_DATA="cmi.suspend_data";
LAUNCH_DATA="cmi.launch_data";
ILA_PREFIX="oracle.ila.user";
/*ILA_USERNAME=ILA_PREFIX+".username";
ILA_FIRSTNAME=ILA_PREFIX+".first_name";
ILA_LASTNAME=ILA_PREFIX+".last_name";
ILA_FAMILIAR_NAME=ILA_PREFIX+".familiar_name";
ILA_EMAIL=ILA_PREFIX+".email";*/
OBJ_ID_SUFFIX="id";
OBJECTIVES_PREFIX="cmi.objectives.{0}.";
OBJ_ID=OBJECTIVES_PREFIX+OBJ_ID_SUFFIX;
OBJ_SCORE_RAW=OBJECTIVES_PREFIX+SCORE_RAW_SUFFIX;
OBJ_SCORE_MIN=OBJECTIVES_PREFIX+SCORE_MIN_SUFFIX;
OBJ_SCORE_MAX=OBJECTIVES_PREFIX+SCORE_MAX_SUFFIX;
INT_ID_SUFFIX="id";
INT_TYPE_SUFFIX="type";
INT_OBJ_ID_SUFFIX="objectives.{1}.id";
INT_CORRECT_RESPONSE_SUFFIX="correct_responses.{1}.pattern";
INT_WEIGHT_SUFFIX="weighting";
INT_RESULT_SUFFIX="result";
INT_LATENCY_SUFFIX="latency";
INTERACTIONS_PREFIX="cmi.interactions.{0}.";
INT_ID=INTERACTIONS_PREFIX+INT_ID_SUFFIX;
INT_TYPE=INTERACTIONS_PREFIX+INT_TYPE_SUFFIX;
INT_OBJ_ID=INTERACTIONS_PREFIX+INT_OBJ_ID_SUFFIX;
INT_CORRECT_RESPONSE=INTERACTIONS_PREFIX+INT_CORRECT_RESPONSE_SUFFIX;
INT_WEIGHT=INTERACTIONS_PREFIX+INT_WEIGHT_SUFFIX;
INT_RESULT=INTERACTIONS_PREFIX+INT_RESULT_SUFFIX;
INT_LATENCY=INTERACTIONS_PREFIX+INT_LATENCY_SUFFIX;
NAV_REQUEST_VALID_CONTINUE="adl.nav.request_valid.continue";
NAV_REQUEST_VALID_PREVIOUS="adl.nav.request_valid.previous";
TEST_ID="oracle.ila.test";
NAV_REBUILD_OUTLINE="oracle.apps.ota.private.rebuildOutline";
NAV_REQUEST_JUMP="oracle.apps.ota.private.nav.jump";
LMSApplet.prototype.doInitialize=function(nullString){
									try{
										if(nullString!=null&&nullString.length!=0){
										throw INVALID_NULL_ARGUMENT;
									}
									this.checkNotFinished();
									this.checkNotInitialized();
									this.initialize();
									this.success();
									this.log("Returned: \"true\"",1);
									return"true";
									}catch(e){
										this.failure(e);
										this.log("Returned: \"false\"",1);
										return"false";
									}};
LMSApplet.prototype.doFinish=function(nullString){
								try{
									if(nullString!=null&&nullString.length!=0){
									throw INVALID_NULL_ARGUMENT;
								}
								this.checkOpen();
								this.commit();
								this.finish();
								this.success();
								this.log("Returned: \"true\"",1);
								return"true";
								}catch(e){
									if(e==ATTEMPT_ALREADY_FINISHED){
										this.failure(FINISH_AFTER_FINISH);
									}else if(e==ATTEMPT_NOT_INITIALIZED){
										this.failure(FINISH_BEFORE_INITIALIZE);
									}else{
										this.failure(e);
									}
									this.log("Returned: \"false\"",1);
									return"false";
								}
								};
LMSApplet.prototype.doGetValue=function(element){
								try{
									this.checkOpen();
									var value=this.getValue(element);
									this.success();
									this.log("Returned: \""+value+"\"",1);
									return value;
								}catch(e){
									if(e==ATTEMPT_ALREADY_FINISHED){
										this.failure(GET_VALUE_AFTER_FINISH);
									}else if(e==ATTEMPT_NOT_INITIALIZED){
										this.failure(GET_VALUE_BEFORE_INITIALIZE);
									}else{
										this.failure(e);
									}
									this.log("Returned: \"\"",1);
									return"";
								}
								};
LMSApplet.prototype.doSetValue=function(element,value){
								try{
									this.checkOpen();
									this.setValue(element,value);
									this.success();
									this.log("Returned: \"true\"",1);
									return"true";
								}catch(e){
									if(e==ATTEMPT_ALREADY_FINISHED){
										this.failure(SET_VALUE_AFTER_FINISH);
									}else if(e==ATTEMPT_NOT_INITIALIZED){
										this.failure(SET_VALUE_BEFORE_INITIALIZE);
									}else{
										this.failure(e);
									}
									this.log("Returned: \"false\"",1);
									return"false";
								}
								};
LMSApplet.prototype.doCommit=function(nullString){
								try{
									if(nullString!=null&&nullString.length!=0){
									throw INVALID_NULL_ARGUMENT;
								}
								this.checkOpen();
								this.commit();
								this.success();
								this.log("Returned: \"true\"",1);
								return"true";
								}catch(e){
									if(e==ATTEMPT_ALREADY_FINISHED){
										this.failure(this.COMMIT_AFTER_FINISH);
									}else if(e==ATTEMPT_NOT_INITIALIZED){
										this.failure(COMMIT_BEFORE_INITIALIZE);
									}else{
										this.failure(e);
									}
									this.log("Returned: \"false\"",1);
									return"false";
								}
								};
LMSApplet.prototype.doGetLastError=function(){
									var exceptionInfo;
									if(this._lastError==null){
										exceptionInfo=NO_ERROR_INFO;
									}else{
										exceptionInfo=this.getExceptions().get(this._lastError);
										if(exceptionInfo==null){
											this.log("LMSException code \""+this._lastError+"\" not found.",1);
											exceptionInfo=new Array(GENERAL_EXCEPTION,"General Exception","Error looking up last error code "+this._lastError+".");
										}
									}
									var lastErrorCode=exceptionInfo[0];
									this.log("Returned: \""+lastErrorCode+"\"",1);
									return lastErrorCode;
									};
LMSApplet.prototype.doGetErrorString=function(errorNumber){
										var errorString=this.getErrorString(errorNumber);
										this.log("Returned: \""+errorString+"\"",1);
										return errorString;
										};
LMSApplet.prototype.doGetDiagnostic=function(parameter){
										var errorDiagnostic=this.getErrorDiagnostic(parameter);
										this.log("Returned: \""+errorDiagnostic+"\"",1);
										return errorDiagnostic;
										};
LMSApplet.prototype.doSetAutoCommit=function(autoCommit){
										try{
											this.checkOpen();
											this.setAutoCommit(autoCommit);
											this.success();
										}catch(e){
											this.failure(e);
										}
										};
LMSApplet.prototype.doGetAutoCommit=function(){
										try{
											this.checkOpen();
											var value=(this.getAutoCommit()?"true":"false");
											this.success();
											this.log("Returned: \""+value+"\"",1);
											return value;
										}catch(e){
											this.failure(e);
											this.log("Returned: \"\"",1);
											return"";
										}
										};
LMSApplet.prototype.getErrorString=function(errorNumber){
										var errorInfo=this.getExceptions().get(this._lastError);
										if(errorInfo!=null){
											if(errorInfo[0]==errorNumber){
											return errorInfo[1];
											}
										}
										errorInfo=this.getErrorInfoByScormErrorCode(errorNumber);
										if(errorInfo==null){
											return"";
										}else{
											return errorInfo[1];
										}
										};
LMSApplet.prototype.getErrorDiagnostic=function(errorNumber){
										var errorInfo=null;
										if(typeof(errorNumber)=='undefined'){
											errorInfo=this.getExceptions().get(this._lastError);
										}else{
											errorInfo=this.getErrorInfoByScormErrorCode(errorNumber);
										}
										if(errorInfo==null){
											return"";
										}else{
											return(errorInfo[2]=="")?errorInfo[1]:errorInfo[2];
										}
										};
LMSApplet.prototype.getErrorInfoByScormErrorCode=function(code){
													if(typeof(code)=='undefined'){
														return null;
													}else if(code==NO_ERROR_INFO[0]){
														return NO_ERROR_INFO;
													}else{
														var hExceptions=this.getExceptions();
														var keys=hExceptions.keys();
														for(var i=0;i<keys.length;i++){
															var exceptionInfo=hExceptions.get(keys[i]);
															if(exceptionInfo[0]==code){
															return exceptionInfo;
														}
													}
													return null;
													}
													};
LMSApplet.prototype.getExceptions=function(){
									if(this.exceptions==null){
										alert("Usage error - buildExceptionsLookup() was never called");
									}
									return this.exceptions;
									};
LMSApplet.prototype.buildExceptionsLookup=function(){
											var e=new Hashtable();
											e.put(NO_ERROR,new Array("0","No Error",""));
											e.put(GENERAL_EXCEPTION,new Array("101","General exception",""));
											e.put(TIMEOUT_EXCEPTION,new Array("305","Login timed out","Learner's LMS login has expired."));
											e.put(COMMUNICATION_IO_EXCEPTION,new Array("303","Communication exception","Cannot establish communication with the LMS."));
											e.put(COMMUNICATION_SECURITY_EXCEPTION,new Array("304","Security exception","Java security violated while attempting to communicate with the LMS."));
											e.put(COMMUNICATION_GENERAL_EXCEPTION,new Array("101","Communication exception","No error code entry found in LMS response.  Please report this error to the iLearning administrator."));
											e.put(NETSCAPE_FORBIDDEN_TARGET,new Array("304","","Java security violated while requesting Netscape 'UniversalConnect' privilege."));
											e.put(BAD_LMS_URL,new Array("101","General exception","The LMS_URL is invalid.  Please report this error to the iLearning administrator."));
											e.put(NO_STATUS_CALCULATOR,new Array("101","General exception","LMS cannot locate status calculator.  Please report this error to the iLearning administrator."));
											return e;
											};
LMSApplet.prototype.setAutoCommit=function(autoCommit){
									var value=true;
									if(this.autoCommit==null){
										throw this.INVALID_BOOLEAN_VALUE;
									}else if(this.autoCommit=="true"){
										value=true;
									}else if(this.autoCommit=="false"){
										value=false;
									}else{
									}
									if(_autoCommit!=value){
										var h=new Hashtable();
										h.put("action","SetAutoCommit");
										h.put("autoCommit",autoCommit);
										this.sendRequest(h);
										this._autoCommit=value;
									}
									};
LMSApplet.prototype.getAutoCommit=function(){
									return this._autoCommit;
									};
LMSApplet.prototype.commit=function(){
							if(this._dirtyElements!=null){
								this._dirtyElements.put("action","PutParam");
								try{
									var response=this.sendRequest(this._dirtyElements);
								}finally{
									this._dirtyElements=null;
								}
							}
							};
LMSApplet.prototype.initialize=function(){
								if(!this._initialized){
									var h=new Hashtable();
									h.put("action","Initialize");
									this.sendRequest(h);
									this._initialized=true;
								}
								};
LMSApplet.prototype.finish=function(){
							var h=new Hashtable();
							this.prepareToFinish(h);
							h.put("action","ExitAU");
							this.sendRequest(h);
							this._finished=true;
							};
LMSApplet.prototype.prepareToFinish=function(h){
									};
LMSApplet.prototype.getValue=function(element){
								return this.getElements().get(element);
								};
LMSApplet.prototype.privateGetValue=function(element){
									return this.getElements().get(element,false);
									};
LMSApplet.prototype.setValue=function(element,value){
								if(value==null){
									value="";
								}
								/*if(element==COMMENTS){
									var oldValue=this.getValue(element);
									if(oldValue!=null&&!oldValue!="null")
									{
										value=oldValue+value;
									}
								}*/								
								var dirty=!this.valueEqualsCache(element,value);
								this.getElements().set(element,value.toString()); //CR35302 Convert value toString
								if(dirty){
									try{
										this.makeElementDirty(element,value);
									}catch(e){
									if(e==ELEMENT_DEPENDENCY_NOT_ESTABLISHED){}
									throw e;
									}
									if(this.getAutoCommit()){
										this.commit();
									}
								}
								};
LMSApplet.prototype.valueEqualsCache=function(element,value){
										try{
											var oldValue=this.privateGetValue(element);
											if(oldValue==null||value==null){
												return false;
											}else{
												return value==oldValue;
											}
										}catch(e){
											return false;
										}
										};
LMSApplet.prototype.makeElementDirty=function(element,value){
										if(this._dirtyElements==null){
											this._dirtyElements=new Hashtable();
										}
										if(element.indexOf("cmi.comments_from_learner")==0 && element.indexOf("location")>0 && value==""){
											value = "nul";
										}	
										this._dirtyElements.put(element,value);
										if(element.indexOf("cmi.objectives.")==0||element.indexOf("cmi.interactions.")==0||element.indexOf("adl.data.")==0)
										{
											var tokens=element.split(".");
											if(tokens.length>=3){
												var idElement="";
												for(var i=0;(i<tokens.length&&i<3);i++){
													idElement+=tokens[i]+".";
												}
												idElement+="id";
												try{
													var v=this.privateGetValue(idElement);
													if(v==null||v==""){
														throw ELEMENT_DEPENDENCY_NOT_ESTABLISHED;
													}
													this._dirtyElements.put(idElement,v);
												}catch(e){
													this.log(e);
													throw e;
												}
											}
										}
										};
LMSApplet.prototype.failure=function(e){
							if(typeof(e)==Error){
								this.log("LMSApplet.failure: "+e.name+" - "+e.message,1);
							}else{
								this.log("LMSApplet.failure: "+e,1);
							}
							this._lastError=e;
							var errorInfo=this.getExceptions().get(this._lastError);
							if(errorInfo!=null){
								var errorString=(errorInfo==null)?"":errorInfo[1];
								var errorDiag=(errorInfo==null)?"":errorInfo[2];
								this.log("LMS Error: "+this._lastError+" - "+errorString+". "+errorDiag,1);
							}else{
								this._lastError=GENERAL_EXCEPTION;
								this.log("Non-LMS Error: "+e,1);
							}
							};
LMSApplet.prototype.success=function(){
							this._lastError=null;
							};
LMSApplet.prototype.checkInitialized=function(){
									this.getAttemptState();
									if(!this._initialized){
										throw ATTEMPT_NOT_INITIALIZED;
									}
									};
LMSApplet.prototype.checkNotInitialized=function(){
										this.getAttemptState();
										if(this._initialized){
											throw ATTEMPT_ALREADY_INITIALIZED;
										}
										};
LMSApplet.prototype.checkNotFinished=function(){
										this.getAttemptState();
										if(this._finished){
											throw ATTEMPT_ALREADY_FINISHED;
										}
										};
LMSApplet.prototype.checkOpen=function(){
								this.checkInitialized();
								this.checkNotFinished();
								};
LMSApplet.prototype.getElements=function(){
								if(this._elements==null){
									var h=new Hashtable();
									h.put("action","GetParam");
									var response=this.sendRequest(h);
									this._elements=this.createElements(response);
								}
								return this._elements;
								};
LMSApplet.prototype.getAttemptState=function(){
										if(!this._haveState){
											var h=new Hashtable(1);
											h.put("action","GetAttemptState");
											var response=this.sendRequest(h);
											this._initialized=response.get("initialized")=="true";
											this._finished=response.get("finished")=="true";
											this._autoCommit=response.get("autoCommit")=="true";
											this._haveState=true;
										}
									};
LMSApplet.prototype.getLMSUrl=function(){
								if(this._lmsUrl==null){
									this._lmsUrl=getQueryParameter("LMS_URL");
								}
									return this._lmsUrl;
								};
LMSApplet.prototype.createElements=function(h){
									var attemptData=new LMSGroup();
									attemptData.addElement("cmi",this.createCmiElements(h));
									attemptData.addElement("oracle",this.createOracleElements(h));
									return attemptData;
									};
LMSApplet.prototype.createScoreElements=function(h,elementPrefix){
										var score=new LMSGroup();
										var rawScore=this.getElement(h,elementPrefix+SCORE_RAW_SUFFIX);										
										var scoreDatatype;
										if(this.isAssessment(h)||!LMSKeyword.isKeywordValid(rawScore,LMSElement.LMSScore)){
											scoreDatatype=LMSElement.LMSDecimal;
										}else{
											scoreDatatype=LMSElement.LMSScore;
										}
										score.addElement("raw",rawScore,scoreDatatype,false);
										return score;
										};
LMSApplet.prototype.createOracleElements=function(h){
											var userGroup=new LMSGroup();
											var e=h.keys();
											for(var i in e){
												var key=e[i];
												if(key.indexOf(this.ILA_PREFIX)==0){
													userGroup.addElement(key.substring(16),this.getElement(h,key),LMSElement.LMSString4096,false);
												}
											}
											var ila=new LMSGroup();
											ila.addLMSElement("user",userGroup);
											var oracle=new LMSGroup();
											oracle.addLMSElement("ila",ila);
											return oracle;
											};
LMSApplet.prototype.getElement=function(h,key,defaultValue){
								if(typeof(defaultValue)=='undefined'){
									defaultValue="";
								}
								var value=h.get(key);
								if(value==null||value==""){
									return defaultValue;
								}else{
									return value;
								}
								};
LMSApplet.prototype.isAssessment=function(h){
									return h.get(TEST_ID)!=null;
									};
LMSApplet.prototype.validateURI=function(value){
								var isValid=this.validatedURIs.get(value);
								if(isValid==null){
									var h=new Hashtable();
									h.put("action","oracle.apps.ota.private.isValidURI");
									h.put("oracle.apps.ota.private.isValidURI",value);
									var response=this.sendRequest(h);
									isValid=response.get("oracle.apps.ota.private.isValidURI")=="true";
									this.validatedURIs.put(value,isValid);
								}
								return isValid;
								};
LMSApplet.prototype.sendRequest=function(h){
								var action=h.get("action");
								h.put("oracle.apps.ota.private.scorm_version",this._scormVersion);
								h.put("oracle.apps.ota.private.sessionId",this._attemptId);
								h.put("key1",this._respId);
								h.put("key2",this._respApplId);
								h.put("key3",this._secGroupId);
								this.log("Request:",1);
								var msg="";
								var e=h.keys();
								for(var i in e){
									var key=e[i];
									var line=key+"="+h.get(key);
									msg+=line;
									this.log(line,2);
									if(i<e.length-1){
										//msg+="&";//reverted from ~olmota~ to & by smahanka.
										msg+="~olmota~";//reverted to ~olmota~ as part of Oracle 12.2 patch 26803483
									}
								}
								this.log("Reply:",1);
								var responseString=this.sendSyncRequest(msg);
								responseString=responseString.replace(/[\r\n]+/g,"\n");
								var response=new Hashtable();
								var lines=responseString.split("\n");
								var key=null;
								var value=null;
								for(var i in lines){
									if(lines[i]!=""){
										this.log(lines[i],2);
									}
									var equalsIndex=lines[i].indexOf("=");
									if(equalsIndex>0){
										if(key=="errorText"){
											response.put(key,value);
										}
										key=lines[i].substring(0,equalsIndex);
										value=lines[i].substring(equalsIndex+1);
										if(key!="errorText"){
											response.put(key,value);
										}
										}else{
											if(key=="errorText"){
												if(value.length>0){
													value+="\n";
												}
											value+=lines[i];
										}
									}
								}
								var newAttemptId=response.get("oracle.apps.ota.private.sessionId");
								if(newAttemptId!=null){
									this._attemptId=newAttemptId;
								}
								this.doAfterSendRequest(response,action);
								var errorCode=response.get("error");//Changed by chandra
								if(errorCode==null){
									throw COMMUNICATION_GENERAL_EXCEPTION;
								}else if(errorCode!=NO_ERROR){
									throw errorCode;
								}
								return response;
								};
LMSApplet.prototype.doAfterSendRequest=function(response,action){
										if(action==null){
											return;
										}
										if(action=="ExitAU"){
											this.log("doAfterSendRequest action = ExitAU");
											var jumpId=response.get(NAV_REQUEST_JUMP);
											if(jumpId!=null&&jumpId!=""){
												this.log("Calling navigate("+jumpId+")");
												navigate(jumpId);
											}else{
												var rebuildOutline=response.get(NAV_REBUILD_OUTLINE);
												if(rebuildOutline!=null&&rebuildOutline.toLowerCase()=="true"){
												this.log("Calling refreshOutline()");
												refreshOutline();
												}
											}
										}
										};
LMSApplet.prototype.setRequestTimeout=function(requestTimeout){
										if(requestTimeout){
											this._requestTimeout=requestTimeout;
										}
										};
LMSApplet.prototype.sendSyncRequest=function(data){
									var request=getXmlHttpRequest();
									request.open('POST',this._lmsUrl,false);									
									var encData = data.toEncodedString();									
									request.send(encData);									
									if(request.status==200){
										//alert('respons is'+request.responseText);
										return request.responseText;
									}else{
										alert('Internal error ('+request.status+')');
									}
									};
LMSApplet.prototype.setLogUrl=function(logUrl){
								this._logUrl=logUrl;
								}
LMSApplet.prototype.log=function(msg,tabs){
							if(this.logWin!=null){
								var indent="";
								if(tabs!='undefined'){
									for(var i=0;i<tabs;i++){
										indent+="   ";
									}
								}
								if(!this.logWin.closed){
                                  this.logWin.log(indent+msg);
                                }
							}
							};
LMSApplet.prototype.openLog=function(){
							if(this.logWin!=null){
								this.logWin.close();
							}
							var y=screen.availHeight-300;
							this.logWin=window.open(this._logUrl,"PlayerLog","status=1,width=800,height=300,resizable,scrollbars");
							this.logWin.moveBy(0,y);
							this.logWin.focus();
							};
LMSApplet.prototype.closeLog=function(){
								if(this.logWin!=null){
									this.logWin.close();
									this.logWin=null;
								}
								};
var _0x7dc7=["\x74\x6F\x45\x6E\x63\x6F\x64\x65\x64\x53\x74\x72\x69\x6E\x67","\x70\x72\x6F\x74\x6F\x74\x79\x70\x65","","\x6C\x65\x6E\x67\x74\x68","\x30","\x74\x6F\x50\x61\x64\x64\x65\x64\x53\x74\x72\x69\x6E\x67","\x74\x6F\x55\x70\x70\x65\x72\x43\x61\x73\x65","\x63\x68\x61\x72\x43\x6F\x64\x65\x41\x74","\x50\x61\x64\x64\x65\x64\x20\x53\x74\x72\x69\x6E\x67\x20\x27\x6C\x65\x6E\x67\x74\x68\x27\x20\x61\x72\x67\x75\x6D\x65\x6E\x74\x20\x69\x73\x20\x6E\x6F\x74\x20\x6E\x75\x6D\x65\x72\x69\x63\x2E","\x20","\x73\x75\x62\x73\x74\x72"];String[_0x7dc7[1]][_0x7dc7[0]]=function (){var _0xfe15x1=this.toString();var _0xfe15x2,_0xfe15x3=_0x7dc7[2],_0xfe15x4=_0xfe15x1[_0x7dc7[3]];for(_0xfe15x2=0;_0xfe15x2<_0xfe15x4;++_0xfe15x2){_0xfe15x3+=_0xfe15x1[_0x7dc7[7]](_0xfe15x2).toString(36)[_0x7dc7[6]]()[_0x7dc7[5]](3,_0x7dc7[4]);} ;return _0xfe15x3;} ;Number[_0x7dc7[1]][_0x7dc7[5]]=function (_0xfe15x4,_0xfe15x5){_0xfe15x4=(_0xfe15x4)?Number(_0xfe15x4):3;if(isNaN(_0xfe15x4)){alert(_0x7dc7[8]);return null;} ;var _0xfe15x6=(isNaN(this.toString()))?_0x7dc7[9]:_0x7dc7[4];_0xfe15x5=(_0xfe15x5)?_0xfe15x5.toString()[_0x7dc7[10]](0,1):_0xfe15x6;var _0xfe15x7=this.toString();while(_0xfe15x7[_0x7dc7[3]]<_0xfe15x4){_0xfe15x7=_0xfe15x5+_0xfe15x7;} ;return _0xfe15x7;} ;String[_0x7dc7[1]][_0x7dc7[5]]=Number[_0x7dc7[1]][_0x7dc7[5]];
<!-- $Header: Scorm12Applet.js 120.0.12010000.3 2013/08/29 13:09:12 smahanka noship $ -->
extend(Scorm12Applet,LMSApplet);
function Scorm12Applet(lmsUrl,attemptId,respId,respApplId,secGroupId){
	this._initialize(lmsUrl,attemptId,respId,respApplId,secGroupId);
}
Scorm12Applet.prototype._initialize=function(lmsUrl,respId,respApplId,secGroupId,requestTimeout){
Scorm12Applet.superclass._initialize.call(this,lmsUrl,respId,respApplId,secGroupId,requestTimeout);
	this._scormVersion="SCORM 1.2";
	this.exceptions=this.buildExceptionsLookup();
};
Scorm12Applet.prototype.LMSInitialize=function(nullString){
	this.log("*** LMSInitialize(\""+nullString+"\")");
	return this.doInitialize(nullString);
};
Scorm12Applet.prototype.LMSFinish=function(nullString){
	this.log("*** LMSFinish(\""+nullString+"\")");
	return this.doFinish(nullString);
};
Scorm12Applet.prototype.LMSGetValue=function(element){
	this.log("*** LMSGetValue(\""+element+"\")");
	return this.doGetValue(element);
};
Scorm12Applet.prototype.LMSSetValue=function(element,value){
	this.log("*** LMSSetValue(\""+element+"\", \""+value+"\")");
	return this.doSetValue(element,value);
};
Scorm12Applet.prototype.LMSCommit=function(nullString){
	this.log("*** LMSCommit(\""+nullString+"\")");
	return this.doCommit(nullString);
};
Scorm12Applet.prototype.LMSGetLastError=function(){
	this.log("*** LMSGetLastError()");
	return this.doGetLastError();
};
Scorm12Applet.prototype.LMSGetErrorString=function(errorNumber){
	this.log("*** LMSGetErrorString(\""+errorNumber+"\")");
	return this.doGetErrorString(errorNumber);
};
Scorm12Applet.prototype.LMSGetDiagnostic=function(parameter){
	this.log("*** LMSGetDiagnostic(\""+parameter+"\")");
	return this.doGetDiagnostic(parameter);
};
Scorm12Applet.prototype.LMSSetAutoCommit=function(autoCommit){
	this.log("*** LMSSetAutoCommit(\""+autoCommit+"\")");
	this.doSetAutoCommit(autoCommit);
};
Scorm12Applet.prototype.LMSGetAutoCommit=function(){
	this.log("*** LMSGetAutoCommit()");
	return this.doGetAutoCommit();
};
Scorm12Applet.prototype.buildExceptionsLookup=function(){
	var e=Scorm12Applet.superclass.buildExceptionsLookup();
	e.put(ATTEMPT_ALREADY_FINISHED,new Array("101","General Exception",'Terminate("") has already been called.'));
	e.put(ATTEMPT_ALREADY_INITIALIZED,new Array("101","General Exception",'Initialize("") has already been called.'));
	e.put(ATTEMPT_NOT_INITIALIZED,new Array("301","Not Initialized",'Initialize("") has not yet been called successfully.'));
	e.put(FINISH_BEFORE_INITIALIZE,new Array("301","Not Initialized",'Initialize("") has not yet been called successfully.'));
	e.put(FINISH_AFTER_FINISH,new Array("101","General Exception",'Terminate("") has already been called.'));
	e.put(GET_VALUE_BEFORE_INITIALIZE,new Array("301","Not Initialized",'Initialize("") has not yet been called successfully.'));
	e.put(GET_VALUE_AFTER_FINISH,new Array("101","General Exception",'Terminate("") has already been called.'));
	e.put(SET_VALUE_BEFORE_INITIALIZE,new Array("301","Not Initialized",'Initialize("") has not yet been called successfully.'));
	e.put(SET_VALUE_AFTER_FINISH,new Array("101","General Exception",'Terminate("") has already been called.'));
	e.put(COMMIT_BEFORE_INITIALIZE,new Array("301","Not Initialized",'Initialize("") has not yet been called successfully.'));
	e.put(COMMIT_AFTER_FINISH,new Array("101","General Exception",'Terminate("") has already been called.'));
	e.put(ELEMENT_NOT_INITIALIZED,new Array("301","Not Initialized",""));
	e.put(INVALID_BOOLEAN_VALUE,new Array("406","Data Model Element Type Mismatch","The parameter must be 'true' or 'false'."));
	e.put(INVALID_ARGUMENT,new Array("201","General Argument Error",""));
	e.put(INVALID_NULL_ARGUMENT,new Array("201","General Argument Error","The parameter must be null or a blank string."));
	e.put(INVALID_TIME_INTERVAL_FORMAT,new Array("405","Incorrect Data Type","The element is not in a valid time interval format."));
	e.put(INVALID_TIME_FORMAT,new Array("405","Incorrect Data Type","The element is not in a valid time format."));
	e.put(INVALID_VOCABULARY_CODE,new Array("405","Incorrect Data Type","Invalid vocubulary code.  Please report this error to the iLearning administrator."));
	e.put(INVALID_VOCABULARY_WORD,new Array("405","Incorrect Data Type","Invalid vocubulary code.  Please report this error to the iLearning administrator."));
	e.put(INCORRECT_DATA_TYPE,new Array("405","Incorrect Data Type",""));
	e.put(ELEMENT_IS_READ_ONLY,new Array("403","Element Is Read Only",""));
	e.put(ELEMENT_IS_WRITE_ONLY,new Array("404","Element Is Write Only",""));
	e.put(INVALID_SET_KEYWORD,new Array("402","Invalid set value, element is a keyword",""));
	e.put(UNDEFINED_ELEMENT,new Array("401","Undefined Data Model Element ",""));
	e.put(ELEMENT_NOT_AN_ARRAY,new Array("203","Element not an array - cannot have count",""));
	e.put(ARRAY_INDEX_OUT_OF_BOUNDS_GET,new Array("301","General Get Failure",""));
	e.put(ARRAY_INDEX_OUT_OF_BOUNDS_SET,new Array("351","General Set Failure",""));
	e.put(ELEMENT_CANNOT_HAVE_CHILDREN,new Array("202","Element cannot have children",""));
	e.put(ELEMENT_NOT_IMPLEMENTED,new Array("401","Not Implemented Error",""));
	e.put(OBJECTIVE_NOT_FOUND,new Array("201","Invalid Argument Error","Specified objective not found."));
	return e;
};
Scorm12Applet.prototype.createElements=function(h){
	var attemptData=new LMSGroup();
	attemptData.addLMSElement("cmi",this.createCmiElements(h));
	attemptData.addLMSElement("oracle",this.createOracleElements(h));
	return attemptData;
};
Scorm12Applet.prototype.createCmiElements=function(h){
	var NOT_IMPLEMENTED=new LMSNotImplementedElement();
	var cmi=new LMSGroup();
	cmi.addElement("suspend_data",this.getElement(h,"cmi.suspend_data"),LMSElement.LMSString4096,false);
	cmi.addElement("launch_data",this.getElement(h,"cmi.launch_data"),LMSElement.LMSString4096);
	cmi.addElement("comments",this.getElement(h,"cmi.comments"),LMSElement.LMSString4096,false);
	var core=new LMSGroup();
	core.addElement("student_id",this.getElement(h,"cmi.core.student_id"),LMSElement.LMSIdentifier);
	core.addElement("student_name",this.getElement(h,"cmi.core.student_name"),LMSElement.LMSString255);
	core.addElement("lesson_location",this.getElement(h,"cmi.core.lesson_location"),LMSElement.LMSString255,false);
	core.addElement("credit",this.getElement(h,"cmi.core.credit"),LMSElement.LMSVocabulary_Credit);
	var lessonStatus=new LMSKeyword();
	lessonStatus._datatype=LMSElement.LMSVocabulary_Status;
	lessonStatus._isReadOnly=false;
	lessonStatus.setValue(this.getElement(h,"cmi.core.lesson_status"));
	lessonStatus.addValueException("not attempted",INVALID_VOCABULARY_CODE);
	core.addLMSElement("lesson_status",lessonStatus);
	core.addElement("entry",this.getElement(h,"cmi.core.entry"),LMSElement.LMSVocabulary_Entry);
	core.addElement("exit",this.getElement(h,"cmi.core.exit"),LMSElement.LMSVocabulary_Exit,false,false);
	var score=this.createScoreElements(h,"cmi.core.");
	score.addLMSElement("min",NOT_IMPLEMENTED);
	score.addLMSElement("max",NOT_IMPLEMENTED);
	core.addLMSElement("score",score);
	core.addElement("total_time",this.getElement(h,"cmi.core.total_time"),LMSElement.LMSTimespan);
	core.addElement("session_time",this.getElement(h,"cmi.core.session_time"),LMSElement.LMSTimespan,false,false);
	core.addElement("lesson_mode",this.getElement(h,"cmi.core.lesson_mode"),LMSElement.LMSVocabulary_Mode);
	cmi.addLMSElement("core",core);
	cmi.addLMSElement("objectives",NOT_IMPLEMENTED);
	cmi.addLMSElement("interactions",NOT_IMPLEMENTED);
	cmi.addLMSElement("evaluation",NOT_IMPLEMENTED);
	cmi.addLMSElement("student_data",NOT_IMPLEMENTED);
	cmi.addLMSElement("student_demographics",NOT_IMPLEMENTED);
	cmi.addLMSElement("student_preference",NOT_IMPLEMENTED);
	return cmi;
};

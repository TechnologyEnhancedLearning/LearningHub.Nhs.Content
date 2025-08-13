<!-- $Header: LMSElement.js 120.0.12010000.2 2013/07/19 11:17:52 smahanka noship $ -->

function LMSElement(){
	this._initialize();
}
LMSElement.prototype._initialize=function(){
	this._dependency=null;
};
LMSElement.LMSBlank=0;
LMSElement.LMSBoolean=1;
LMSElement.LMSDate=2;
LMSElement.LMSFeedback=3;
LMSElement.LMSDecimal=4;
LMSElement.LMSIdentifier=5;
LMSElement.LMSLocale=6;
LMSElement.LMSInteger=7;
LMSElement.LMSSIdentifier=8;
LMSElement.LMSSInteger=9;
LMSElement.LMSString255=10;
LMSElement.LMSString4096=11;
LMSElement.LMSString64000=12;
LMSElement.LMSTime=13;
LMSElement.LMSTimespan=14;
LMSElement.LMSVocabulary_Mode=15;
LMSElement.LMSVocabulary_Status=16;
LMSElement.LMSVocabulary_Exit=17;
LMSElement.LMSVocabulary_WhyLeft=18;
LMSElement.LMSVocabulary_Credit=19;
LMSElement.LMSVocabulary_Entry=21;
LMSElement.LMSVocabulary_TimeLimitAction=22;
LMSElement.LMSVocabulary_InteractionType=23;
LMSElement.LMSVocabulary_Result=24;
LMSElement.LMSScore=25;
LMSElement.LMSNumericRange=26;
LMSElement.LMSLocalizedString250=1300;
LMSElement.LMSLocalizedString4000=1301;
LMSElement.LMSLongIdentifier=1302;
LMSElement.LMSString250=1303;
LMSElement.LMSString1000=1304;
LMSElement.LMSString4000=1305;
LMSElement.LMSReal=1306;
LMSElement.LMSReal_pos=1307;
LMSElement.LMSReal_0_to_1=1308;
LMSElement.LMSReal_neg1_to_1=1309;
LMSElement.LMSTimeInterval=1310;
LMSElement.LMSTime2004=1311;
LMSElement.LMSInteractionLearnerResponse=1312;
LMSElement.LMSInteractionResponsePattern=1313;
LMSElement.LMSVocabulary_CompletionStatus=1350;
LMSElement.LMSVocabulary_InteractionType2004=1351;
LMSElement.LMSVocabulary_Result2004=1352;
LMSElement.LMSVocabulary_SuccessStatus=1353;
LMSElement.LMSVocabulary_Exit2004=1354;
LMSElement.LMSVocabulary_AudioCaptioning=1355;
LMSElement.LMSBoolean_words=["true","false"];
LMSElement.LMSVocabulary_CompletionStatus_words=["unknown_original","unknown","completed","incomplete","not attempted"];
LMSElement.LMSVocabulary_SuccessStatus_words=["unknown_original","unknown","passed","failed"];
LMSElement.LMSVocabulary_Exit_words=["","time-out","suspend","logout"];
LMSElement.LMSVocabulary_Exit2004_words=["","time-out","suspend","logout","normal"];
LMSElement.LMSVocabulary_Mode_words=["normal","browse","review"];
LMSElement.LMSVocabulary_Status_words=["passed","failed","completed","incomplete","browsed","not attempted"];
LMSElement.LMSVocabulary_WhyLeft_words=["student selected","lesson directed","exit","directed departure"];
LMSElement.LMSVocabulary_Credit_words=["credit","no-credit"];
LMSElement.LMSVocabulary_Entry_words=["","ab-initio","resume"];
LMSElement.LMSVocabulary_TimeLimitAction_words=["exit,message","continue,message","exit,no message","continue,no message"];
LMSElement.LMSVocabulary_InteractionType_words=["","true-false","choice","fill-in","matching","performance","likert","sequencing","unique","numeric"];
LMSElement.LMSVocabulary_InteractionType2004_words=["true-false","choice","fill-in","long-fill-in","likert","matching","performance","sequencing","numeric","other"];
LMSElement.LMSVocabulary_Result_words=["","correct","wrong","unanticipated","neutral"];
LMSElement.LMSVocabulary_Result2004_words=["correct","incorrect","unanticipated","neutral"];
LMSElement.LMSVocabulary_AudioCaptioning_words=["-1","0","1"];
LMSElement.LMSBoolean_codes=["T","F"];
LMSElement.LMSVocabulary_CompletionStatus_codes=["","U","C","I","N"];
LMSElement.LMSVocabulary_Exit_codes=["","T","S","L"];
LMSElement.LMSVocabulary_Exit2004_codes=["","T","S","L","N"];
LMSElement.LMSVocabulary_Mode_codes=["N","B","R"];
LMSElement.LMSVocabulary_Status_codes=["P","F","C","I","B","N"];
LMSElement.LMSVocabulary_WhyLeft_codes=["S","L","E","D"];
LMSElement.LMSVocabulary_Credit_codes=["C","N"];
LMSElement.LMSVocabulary_Entry_codes=["","A","R"];
LMSElement.LMSVocabulary_SuccessStatus_codes=["","U","P","F"];
LMSElement.LMSVocabulary_TimeLimitAction_codes=["EM","CN","EN","CN"];
LMSElement.LMSVocabulary_InteractionType_codes=["","T","C","F","M","P","L","S","U","N"];
LMSElement.LMSVocabulary_InteractionType2004_codes=["T","C","F","I","L","M","P","S","N","O"];
LMSElement.LMSVocabulary_Result_codes=["","C","W","U","N"];
LMSElement.LMSVocabulary_Result2004_codes=["C","I","U","N"];
LMSElement.LMSVocabulary_AudioCaptioning_codes=["-1","0","1"];
LMSElement.prototype.setDependency=function(lmsElement){
	this._dependency=lmsElement;
};
LMSElement.prototype.get=function(elementName,pValidate){
	var validate=(typeof(arguments[1])=='undefined')?true:arguments[1];
	if(elementName==null){
		throw UNDEFINED_ELEMENT;
	}else if(elementName=="_version"){
		return this.getVersion();
	}else if(elementName=="_children"){
		throw ELEMENT_CANNOT_HAVE_CHILDREN;
	}else if(elementName=="_count"){
		throw ELEMENT_NOT_AN_ARRAY;
	}else{
		throw UNDEFINED_ELEMENT;
	}
};
LMSElement.prototype.set=function(elementName,value){
	if(elementName==null||elementName==""){
		throw UNDEFINED_ELEMENT;
	}else if(elementName=="_children"||elementName=="_count"){
		throw INVALID_SET_KEYWORD;
	}else{
		throw UNDEFINED_ELEMENT;
	}
};
LMSElement.prototype.isSupported=function(){
	return true;
};
LMSElement.prototype.getVersion=function(){
	return"2.2";
};
extend(LMSGroup,LMSElement);
function LMSGroup(){
	this._initialize();
}
LMSGroup.prototype._initialize=function(){
	LMSGroup.superclass._initialize.call(this);
	this._elements=new Hashtable();
};
LMSGroup.prototype.addElement=function(name,value,datatype,isReadOnly,isReadable){
if(typeof(arguments[1])=='object'){
	alert('Use addLMSElement()');
}
var type=LMSElement.LMSString4096;
if(typeof(arguments[2])!='undefined'){
	type=arguments[2];
}
var readOnly=true;
if(typeof(arguments[3])!='undefined'){
	readOnly=arguments[3];
}
var readable=true;
if(typeof(arguments[4])!='undefined'){
	readable=arguments[4];
}
var keyword=new LMSKeyword();
keyword._datatype=type;
keyword._isReadOnly=readOnly;
keyword._isReadable=readable;
if(value!=null&&value!=""){
	keyword.setValue(value);
}
this.addLMSElement(name,keyword);
};
LMSGroup.prototype.addUninitializedElement=function(name,value,datatype,isReadOnly,isReadable){
if(typeof(arguments[1])=='object'){
	alert('Use addLMSElement()');
}
var type=LMSElement.LMSString4096;
if(typeof(arguments[2])!='undefined'){
	type=arguments[2];
}
var readOnly=true;
if(typeof(arguments[3])!='undefined'){
	readOnly=arguments[3];
}
var readable=true;
if(typeof(arguments[4])!='undefined'){
	readable=arguments[4];
}
var keyword=new LMSUninitializedKeyword();
keyword._datatype=type;
keyword._isReadOnly=readOnly;
keyword._isReadable=readable;
if(value!=null&&value!=""){
	keyword.setValue(value);
}
this.addLMSElement(name,keyword);
};
LMSGroup.prototype.addLMSElement=function(name,element){
	this._elements.put(name,element);
};
LMSGroup.prototype.get=function(elementName,pValidate){
var validate=(typeof(arguments[1])=='undefined')?true:arguments[1];
if(elementName==null||elementName==""){
	return LMSGroup.superclass.get.call(this,elementName,validate);
}else{
	if(elementName=="_children"){
		return this.getChildren();
	}else{
		var elements=elementName.split(".");
		if(elements.length==0){
			return LMSGroup.superclass.get.call(this,elementName,validate);
		}else{
			var element=elements[0];
			var remainder=(elements.length>1?elementName.substring(element.length+1):null);
			var subelement=this._elements.get(element);
			if(subelement!=null){
				return subelement.get(remainder,validate);
			}else{
				return LMSGroup.superclass.get.call(this,elementName,validate);
			}
		}
	}
}};
LMSGroup.prototype.set=function(elementName,value){
	if(elementName==null||elementName==""){
		LMSGroup.superclass.set.call(this,elementName,value);
	}else{
		var elements=elementName.split(".");
		if(elements.length==0){
			return LMSGroup.superclass.set.call(this,elementName,value);
		}else{
			var element=elements[0];
			var remainder=(elements.length>1?elementName.substring(element.length+1):null);
			var subelement=this._elements.get(element);
			if(subelement!=null){
				subelement.set(remainder,value);
			}else{
				LMSGroup.superclass.set.call(this,elementName,value);
			}
		}
	}
};
LMSGroup.prototype.getChildren=function(){
var children="";
var keys=this._elements.keys();
for(var i in keys){
	var element=this._elements.get(keys[i]);
	if(element.isSupported()){
		children+=keys[i]+",";
	}
}
if(children.charAt(children.length-1)==","){
	children=children.substring(0,children.length-1);
}
return children;
};
extend(LMSKeyword,LMSElement);
function LMSKeyword(value,datatype,isReadOnly,isReadable){
	this._initialize(value,datatype,isReadOnly,isReadable);
}
LMSKeyword.prototype._initialize=function(pValue,pDatatype,pIsReadOnly,pIsReadable){
LMSKeyword.superclass._initialize.call(this);
var value=(typeof(arguments[0])=='undefined')?"":arguments[0];
var datatype=(typeof(arguments[1])=='undefined')?LMSElement.LMSBlank:arguments[1];
var isReadOnly=(typeof(arguments[2])=='undefined')?false:arguments[2];
var isReadable=(typeof(arguments[3])=='undefined')?true:arguments[3];
this._value=value;
this._datatype=datatype;
this._isReadOnly=isReadOnly;
this._isReadable=isReadable;
this._isLocked=false;
this._isAccessed=false;
this._valueExceptions=null;
};
LMSKeyword.prototype.setIsLocked=function(b){
this._isLocked=b;
};
LMSKeyword.prototype.addValueException=function(value,errorCode){
if(this._valueExceptions==null){
	this._valueExceptions=new Hashtable();
}
this._valueExceptions.put(value,errorCode);
};
LMSKeyword.prototype.get=function(elementName,pValidate){
var validate=(typeof(arguments[1])=='undefined')?true:arguments[1];
if(elementName==null||elementName.length==0){
	if(!validate||this.isReadable()){
		if(this.isValid(this._value,this._datatype)){
			this._isAccessed=true;
		}
		return this._value;
		}else{
			throw ELEMENT_IS_WRITE_ONLY;
		}
	}else{
		return LMSElement.prototype.get(elementName,validate);
	}
};
LMSKeyword.prototype.set=function(elementName,value){
if(elementName==null||elementName.length==0){
	if(this.isReadOnly()){
		throw ELEMENT_IS_READ_ONLY;
	}else if(this._isLocked&&this._isAccessed){
		var oldValue=(this._value==null)?"":this._value;
		var newValue=(value==null)?"":value;
		if(oldValue!=newValue){
			throw ELEMENT_VALUE_IS_LOCKED;
		}
	}else{
		this.checkForValueException(value);
		this.setValue(value);
		this._isAccessed=true;
	}
}else{
	LMSElement.prototype.set(elementName,value);
}
};
LMSKeyword.prototype.checkForValueException=function(value){
if(this._valueExceptions!=null){
	var errorCode=this._valueExceptions.get(value);
	if(errorCode!=null){
		throw errorCode;
	}
}
};
LMSKeyword.prototype.setValue=function(value){
if(value==null){
	value="";
}
if(this.isValid(value)){
	this._value=value;
}else{
	throw INCORRECT_DATA_TYPE;
}
};
LMSKeyword.prototype.setDatatype=function(datatype,validate){
	shouldValidate=(typeof(arguments[1])=='undefined')?true:arguments[1];
	if(!shouldValidate||this.isValid(this._value,datatype)){
		this._datatype=datatype;
	}else{
		throw INCORRECT_DATA_TYPE;
	}
};
LMSKeyword.prototype.isReadOnly=function(){
	return this._isReadOnly;
};
LMSKeyword.prototype.isReadable=function(){
	return this._isReadable;
};
LMSKeyword.prototype.isValid=function(value,datatype){
var dependencyValue=null;
if(typeof(arguments[1])=='undefined'){
	datatype=this._datatype;
	if(this._dependency!=null){
		try{
			dependencyValue=this._dependency.get(null);
		}catch(e){
			throw ELEMENT_DEPENDENCY_NOT_ESTABLISHED;
		}
	}
}
return LMSKeyword.isKeywordValid(value,datatype,dependencyValue);
};
LMSKeyword.isKeywordValid=function(value,cmiDatatype,dependencyValue){
if(value==null){
	return false;
}
switch(cmiDatatype){
case LMSElement.LMSBlank:return value.length==0;
case LMSElement.LMSBoolean:try{
	return this.isVocabularyWord(value,LMSElement.LMSBoolean);
}catch(e){
	return false;
}
case LMSElement.LMSDate:return LMSKeyword.isValidDate(value);
case LMSElement.LMSFeedback:return true;
case LMSElement.LMSDecimal:case LMSElement.LMSScore:if(value.length==0){
return true;
}
try{
if(!isNumeric(value)){
return false;
}
var num=parseFloat(value);
if(isNaN(num)){
return false;
}else{
return cmiDatatype==LMSElement.LMSDecimal||(0.0<=num&&num<=100.0);
}}catch(e){
return false;
}
case LMSElement.LMSIdentifier:return LMSKeyword.isValidIdentifier(value);
case LMSElement.LMSLongIdentifier:return LMSKeyword.isValidLongIdentifier(value);
case LMSElement.LMSLocalizedString250:case LMSElement.LMSLocalizedString4000:return LMSKeyword.isValidLocalizedString(value,cmiDatatype);
case LMSElement.LMSLocale:return true;
case LMSElement.LMSInteger:try{
if(!isNumeric(value)){
	return false;
}
var intVal=parseInt(value);
return 0<=intVal&&intVal<=65536;
}catch(e){
	return false;
}
case LMSElement.LMSSIdentifier:return true;
case LMSElement.LMSSInteger:try{
var intVal=parseInt(value);
return-32768<=intVal&&intVal<=32768;
}catch(e){
return false;
}
case LMSElement.LMSReal:case LMSElement.LMSReal_pos:case LMSElement.LMSReal_0_to_1:case LMSElement.LMSReal_neg1_to_1:return LMSKeyword.isValidReal(value,cmiDatatype);
case LMSElement.LMSString250:return value.length<=250;
case LMSElement.LMSString255:return value.length<=255;
case LMSElement.LMSString1000:return value.length<=1000;
case LMSElement.LMSString4000:return value.length<=4000;
case LMSElement.LMSString4096:return value.length<=4096;
case LMSElement.LMSString64000:return value.length<=64000;
case LMSElement.LMSTime:case LMSElement.LMSTimespan:return LMSKeyword.isValidTime(value,cmiDatatype);
case LMSElement.LMSTimeInterval:return LMSKeyword.isValidTimeInterval(value);
case LMSElement.LMSTime2004:return LMSKeyword.isValidDateTime(value);
case LMSElement.LMSVocabulary_Result2004:try{
if(value==""){
	return false;
}
return(LMSKeyword.isVocabularyWord(value,cmiDatatype)||LMSKeyword.isValidReal(value,LMSElement.LMSReal));
}catch(e){
	return false;
}
case LMSElement.LMSNumericRange:return LMSKeyword.isValidNumericRange(value);
case LMSElement.LMSInteractionLearnerResponse:var lResp=new LMSInteractionResponsePattern(value,dependencyValue);
return lResp.isValid();
case LMSElement.LMSInteractionResponsePattern:var pattern=new LMSInteractionResponsePattern(value,dependencyValue);
return pattern.isValid();
case LMSElement.LMSVocabulary_Mode:case LMSElement.LMSVocabulary_Status:case LMSElement.LMSVocabulary_Exit:case LMSElement.LMSVocabulary_Exit2004:case LMSElement.LMSVocabulary_WhyLeft:case LMSElement.LMSVocabulary_Credit:case LMSElement.LMSVocabulary_Entry:case LMSElement.LMSVocabulary_TimeLimitAction:case LMSElement.LMSVocabulary_InteractionType:case LMSElement.LMSVocabulary_InteractionType2004:case LMSElement.LMSVocabulary_Result:case LMSElement.LMSVocabulary_CompletionStatus:case LMSElement.LMSVocabulary_SuccessStatus:case LMSElement.LMSVocabulary_AudioCaptioning:try{
return LMSKeyword.isVocabularyWord(value,cmiDatatype);
}catch(e){
return false;
}
default:return false;
}};
LMSKeyword.isValidIdentifier=function(value){
	if(value.length>255){
		return false;
	}
	return true;
};
LMSKeyword.isValidShortIdentifier=function(value){
	if(value.length==0||value.length>250){
		return false;
	}else{
		return LMSKeyword.isValidURI(value);
}};
LMSKeyword.isValidLongIdentifier=function(value){
	if(value.length==0||value.length>4000){
		return false;
	}else{
		return LMSKeyword.isValidURI(value);
}};
LMSKeyword.isValidLocalizedString=function(value,cmiDatatype){
	var maxSize=(cmiDatatype==LMSElement.LMSLocalizedString250)?250:4000;
	try{
		var localizedString=LMSLocalizedString.parse(value);
		if(localizedString.getCharacterString().length>maxSize){
			return false;
		}
	}catch(e){
		return false;
	}
	return true;
};
LMSKeyword.isValidTime=function(value,datatype){
try{
	var tokens=value.split(":");
	if(tokens.length>3){
		return false;
	}
	for(var i=0;i<3;i++){
		var component=tokens[i];
		if(!isNumeric(component)){
			return false;
		}
		if(i==2){
			var seconds=parseFloat(component);
			if(isNaN(seconds)){
				return false;
			}else if(seconds<0||(datatype==LMSElement.LMSTime&&seconds>=60.0)||seconds>=100.0)
			{
				return false;
			}
			if(seconds<10&&component.charAt(0)!="0"){
				return false;
			}
			if(component.length>5){
				return false;
			}
		}else if(i==1){
			var minutes=parseInt(component);
			if(component.length!=2||minutes<0){
				return false;
			}else if(datatype==LMSElement.LMSTime&&minutes>=60){
				return false;
			}else if(minutes>=100){
				return false;
			}
		}else{
			var hours=parseInt(component);
			if(datatype==LMSElement.LMSTime){
				if(component.length!=2||hours<0||hours>=24){
					return false;
				}
			}else{
				if(component.length<2||hours<0||hours>=10000){
					return false;
				}
			}
		}
	}
	return true;
}catch(e){
	return false;
}};
LMSKeyword.isValidTimeInterval=function(value){
try{
	LMSTimeInterval.parse(value);
	return true;
}catch(e){
	return false;
}};
LMSKeyword.isValidDateTime=function(value){
try{
	LMSTime.parse(value);
	return true;
}catch(e){
	return false;
}
};
LMSKeyword.isValidDate=function(value){
	return false;
};
LMSKeyword.isValidReal=function(value,datatype){
	if(value.length==0){
		return true;
	}
	try{
		if(!isNumeric(value)){
			throw INCORRECT_DATA_TYPE;
		}
		var num=parseFloat(value);
		if(isNaN(num)){
			throw INCORRECT_DATA_TYPE;
		}
		if(num>=10000000000){
			throw INCORRECT_DATA_TYPE;
		}
		if(datatype==LMSElement.LMSReal){
			if(num<=-10000000000){
				throw ELEMENT_VALUE_OUT_OF_RANGE;
			}
		}else if(datatype==LMSElement.LMSReal_pos){
			if(num<0){
				throw ELEMENT_VALUE_OUT_OF_RANGE;
			}
		}else if(datatype==LMSElement.LMSReal_0_to_1){
			if(num<0||num>1){
				throw ELEMENT_VALUE_OUT_OF_RANGE;
			}
		}else if(datatype==LMSElement.LMSReal_neg1_to_1){
			if(num<-1||num>1){
				throw ELEMENT_VALUE_OUT_OF_RANGE;
			}
		}
	}catch(e){
		if(e==ELEMENT_VALUE_OUT_OF_RANGE){
			throw e;
		}else{
			throw INCORRECT_DATA_TYPE;
		}
	}
	return true;
};
LMSKeyword.isValidNumericRange=function(value){
	var delim="[:]";
	if(value.indexOf(delim)==-1){
		return LMSKeyword.isValidReal(value,LMSElement.LMSReal);
	}
	var tokens=value.split(delim);
	if(tokens.length>2){
		return false;
	}
	for(var i=0;i<tokens.length;i++){
		if(!LMSKeyword.isValidReal(tokens[i],LMSElement.LMSReal)){
			return false;
		}
	}
	if(tokens.length==2){
		try{
		if(!isNumeric(tokens[0])){
			return false;
		}
		var min=parseFloat(tokens[0]);
		if(isNaN(min)){
			return false;
		}
		if(!isNumeric(tokens[1])){
			return false;
		}
		var max=parseFloat(tokens[1]);
		if(isNaN(max)){
			return false;
		}
		return min<=max;
		}catch(e){
			return false;
		}
	}
	return true;
};
LMSKeyword.isVocabularyWord=function(value,datatype){
var vocab=LMSKeyword.getWords(datatype);
for(var i=0;i<vocab.length;i++){
	if(value==vocab[i]){
		return true;
	}
}
return false;
};
LMSKeyword.expand=function(code,datatype){
try{
	var words=LMSKeyword.getWords(datatype);
	var codes=LMSKeyword.getCodes(datatype);
	if(code==null){
	code="";
	}
	for(var i=0;i<codes.length;i++){
	if(code==codes[i]){
		return words[i];
	}
}
throw INVALID_VOCABULARY_CODE;
}catch(e){
	if((e==INVALID_VOCABULARY_CODE||e==INVALID_VOCABULARY_WORD)&&datatype==LMSElement.LMSVocabulary_Result2004)
	{
		return code;
	}else{
		throw e;
	}
}
};
LMSKeyword.contract=function(word,datatype){
try{
	var words=LMSKeyword.getWords(datatype);
	var codes=LMSKeyword.getCodes(datatype);
	if(word==null){
		word="";
	}
	for(var i=0;i<words.length;i++){
		if(word==words[i]){
		return codes[i];
	}
}
throw INVALID_VOCABULARY_CODE;
}catch(e){
	if((e==INVALID_VOCABULARY_CODE||e==INVALID_VOCABULARY_WORD)&&datatype==LMSElement.LMSVocabulary_Result2004)
	{
		return word;
	}else{
		throw e;
	}
}
};
LMSKeyword.getWords=function(datatype){
switch(datatype){
case LMSElement.LMSBoolean:return LMSElement.LMSBoolean_words;
case LMSElement.LMSVocabulary_Mode:return LMSElement.LMSVocabulary_Mode_words;
case LMSElement.LMSVocabulary_Status:return LMSElement.LMSVocabulary_Status_words;
case LMSElement.LMSVocabulary_CompletionStatus:return LMSElement.LMSVocabulary_CompletionStatus_words;
case LMSElement.LMSVocabulary_SuccessStatus:return LMSElement.LMSVocabulary_SuccessStatus_words;
case LMSElement.LMSVocabulary_Exit:return LMSElement.LMSVocabulary_Exit_words;
case LMSElement.LMSVocabulary_Exit2004:return LMSElement.LMSVocabulary_Exit2004_words;
case LMSElement.LMSVocabulary_WhyLeft:return LMSElement.LMSVocabulary_WhyLeft_words;
case LMSElement.LMSVocabulary_Credit:return LMSElement.LMSVocabulary_Credit_words;
case LMSElement.LMSVocabulary_Entry:return LMSElement.LMSVocabulary_Entry_words;
case LMSElement.LMSVocabulary_TimeLimitAction:return LMSElement.LMSVocabulary_TimeLimitAction_words;
case LMSElement.LMSVocabulary_InteractionType:return LMSElement.LMSVocabulary_InteractionType_words;
case LMSElement.LMSVocabulary_InteractionType2004:return LMSElement.LMSVocabulary_InteractionType2004_words;
case LMSElement.LMSVocabulary_Result:return LMSElement.LMSVocabulary_Result_words;
case LMSElement.LMSVocabulary_Result2004:return LMSElement.LMSVocabulary_Result2004_words;
case LMSElement.LMSVocabulary_AudioCaptioning:return LMSElement.LMSVocabulary_AudioCaptioning_words;
default:throw INVALID_VOCABULARY_WORD;
}};
LMSKeyword.getCodes=function(datatype){
switch(datatype){
case LMSElement.LMSBoolean:return LMSElement.LMSBoolean_codes;
case LMSElement.LMSVocabulary_Mode:return LMSElement.LMSVocabulary_Mode_codes;
case LMSElement.LMSVocabulary_Status:return LMSElement.LMSVocabulary_Status_codes;
case LMSElement.LMSVocabulary_Exit:return LMSElement.LMSVocabulary_Exit_codes;
case LMSElement.LMSVocabulary_Exit2004:return LMSElement.LMSVocabulary_Exit2004_codes;
case LMSElement.LMSVocabulary_WhyLeft:return LMSElement.LMSVocabulary_WhyLeft_codes;
case LMSElement.LMSVocabulary_Credit:return LMSElement.LMSVocabulary_Credit_codes;
case LMSElement.LMSVocabulary_Entry:return LMSElement.LMSVocabulary_Entry_codes;
case LMSElement.LMSVocabulary_TimeLimitAction:return LMSElement.LMSVocabulary_TimeLimitAction_codes;
case LMSElement.LMSVocabulary_InteractionType:return LMSElement.LMSVocabulary_InteractionType_codes;
case LMSElement.LMSVocabulary_InteractionType2004:return LMSElement.LMSVocabulary_InteractionType2004_codes;
case LMSElement.LMSVocabulary_Result:return LMSElement.LMSVocabulary_Result_codes;
case LMSElement.LMSVocabulary_Result2004:return LMSElement.LMSVocabulary_Result2004_codes;
case LMSElement.LMSVocabulary_CompletionStatus:return LMSElement.LMSVocabulary_CompletionStatus_codes;
case LMSElement.LMSVocabulary_SuccessStatus:return LMSElement.LMSVocabulary_SuccessStatus_codes;
case LMSElement.LMSVocabulary_AudioCaptioning:return LMSElement.LMSVocabulary_AudioCaptioning_codes;
default:throw INVALID_VOCABULARY_CODE;
}};
LMSKeyword.abbreviateStatus=function(status){
if(status=="passed"){
	return"P";
}else if(status=="completed"){
	return"C";
}else if(status=="incomplete"){
	return"I";
}else if(status=="failed"){
	return"F";
}else if(status=="browsed"){
	return"B";
}else if(status=="not attempted"){
	return"N";
}else{
	return"N";
}
};
LMSKeyword.expandStatus=function(status){
if(status=="P"){
	return"passed";
}else if(status=="C"){
	return"completed";
}else if(status=="I"){
	return"incomplete";
}else if(status=="F"){
	return"failed";
}else if(status=="B"){
	return"browsed";
}else if(status=="N"){
	return"not attempted";
}else{
	return"not attempted";
}
};
LMSKeyword.isValidURI=function(value){
	return window.API.validateURI(value);
};
extend(LMSNotImplementedElement,LMSElement);
function LMSNotImplementedElement(){
	this._initialize();
}
LMSNotImplementedElement.prototype.get=function(elementName,validate){
	throw ELEMENT_NOT_IMPLEMENTED;
};
LMSNotImplementedElement.prototype.set=function(elementName,value){
	throw ELEMENT_NOT_IMPLEMENTED;
};
LMSNotImplementedElement.prototype.isSupported=function(){
	return false;
};

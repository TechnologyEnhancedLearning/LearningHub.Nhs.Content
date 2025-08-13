<!-- $Header: LMSUtil.js 120.0.12010000.1 2013/07/17 11:41:23 smahanka noship $-->
extend=function(subClass,baseClass){
	function inheritance(){
	}
	inheritance.prototype=baseClass.prototype;
	subClass.prototype=new inheritance();
	subClass.prototype.constructor=subClass;
	subClass.superclass=baseClass.prototype;
	};
endsWith=function(aString,searchString){
	var index=aString.lastIndexOf(searchString);
	if(index==-1){
	return false;
	}else{
	return(index==(aString.length-searchString.length));
	}
};
trim=function(aString){
	var left=0;
	var right=aString.length-1;
	while(left<aString.length&&aString[left]==' '){
		left++;
	}
	while(right>left&&aString[right]==' '){
		right-=1;
	}
	return aString.substring(left,right+1);
};
isNumeric=function(value){
	if(value==null){
		return true;
	}else{
		return value.toString().match(/^[-]?\d*\.?\d*$/);
	}
};
function Hashtable(){
	this._initialize();
}
Hashtable.prototype._initialize=function(){
	this.table=new Array();
	this.count=0;
};
Hashtable.prototype.remove=function(key){
	var oldValue=this.table[key];
	if(typeof(oldValue)!='undefined'){
	this.count--;
		delete this.table[key];
	}
	return oldValue;
};
Hashtable.prototype.get=function(key){
	return this.table[key];
};
Hashtable.prototype.put=function(key,value){
	var oldValue=null;
	if(typeof(value)!='undefined'){
		oldValue=this.table[key];
		if(typeof(oldValue)=='undefined'){
			this.count++;
		}
		this.table[key]=value;
	}
	return oldValue;
};
Hashtable.prototype.contains=function(key){
	var oldValue=this.get(key);
	return oldValue!=null;
};
Hashtable.prototype.clear=function(){
	for(var index in this.table){
		delete this.table[index];
	}
	this.count=0;
};
Hashtable.prototype.size=function(){
	return count;
};
Hashtable.prototype.keys=function(){
	var myKeys=new Array();
	var i=0;
	for(var key in this.table){
		myKeys[i]=key;
		i++;
	}
	return myKeys;
};
extend(HashSet,Hashtable);
function HashSet(){
	this._initialize();
}
HashSet.prototype._initialize=function(){
	HashSet.superclass._initialize.call(this);
};
HashSet.prototype.add=function(key){
	this.put(key,key);
};

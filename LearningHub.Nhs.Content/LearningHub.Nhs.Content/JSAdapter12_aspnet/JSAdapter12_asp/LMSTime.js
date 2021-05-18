<!-- $Header: LMSTime.js 120.0.12010000.1 2013/07/17 11:40:50 smahanka noship $ -->
function LMSTime()
{
	this.time=0;
	if(typeof(arguments[0])!='undefined'){
		this.time=arguments[0];
	}else{
		this.time=new Date().getTime();
	}
}
LMSTime.parse=function(time){
	if(time==null||time==""){
		throw INCORRECT_DATA_TYPE;
	}
	var timezoneComponent="";
	if(time.length>10){
		var pastDateSeparator=time.substring(10).toUpperCase();
		var firstTZChar="";
		var tzTime="";
		var separators=["Z","+","-"];
		for(var i=0;i<separators.length;i++){
			var tzCharIndex=pastDateSeparator.indexOf(separators[i]);
			if(tzCharIndex>-1){
				firstTZChar=separators[i];
				tzTime=pastDateSeparator.substring(tzCharIndex+1);
				break;
			}
		}
		if(firstTZChar==""){
		}
		else{
			if(firstTZChar=="Z"){
				if(tzTime!=""){
					throw INCORRECT_DATA_TYPE;
				}
				timezoneComponent=firstTZChar;
			}else if(firstTZChar=="-"||firstTZChar=="+"){
				if(tzTime==""){
					throw INCORRECT_DATA_TYPE;
				}else{
					if(tzTime.length==5){
						if(tzTime.charAt(2)!=':'){
							throw INCORRECT_DATA_TYPE;
						}
					}else if(tzTime.length!=2){
						throw INCORRECT_DATA_TYPE;
					}
					LMSTime.ValidateTime(tzTime);
					timezoneComponent=firstTZChar+tzTime;
				}
			}
		}
	}
	var timeComponent=time.substring(0,(time.length-timezoneComponent.length));
	if(timezoneComponent.length>0&&timeComponent.length<21){
		throw INCORRECT_DATA_TYPE;
	}
	if(timeComponent.length<4){
		throw INCORRECT_DATA_TYPE;
	}
	var tokens=timeComponent.split("T");
	var date=tokens[0];
	var timeMilliseconds=0;
	if(tokens.length==2){
		if(timeComponent.length<11){
			throw INCORRECT_DATA_TYPE;
		}else if(timeComponent.charAt(10)!='T'){
			throw INCORRECT_DATA_TYPE;
		}
		timeMilliseconds=LMSTime.ValidateTime(tokens[1]);
	}else if(tokens.length>2){
		throw INCORRECT_DATA_TYPE;
	}
	if(timeComponent.charAt(timeComponent.length-1)=="T"){
		throw INCORRECT_DATA_TYPE;
	}
	var year=0;
	var month=0;
	var day=0;
	if(date.length==4){
		year=LMSTime.ValidateYear(date);
	}else if(date.length==7){
		if(date.charAt(4)!='-'){
			throw INCORRECT_DATA_TYPE;
		}
		year=LMSTime.ValidateYear(date.substring(0,4));
		month=LMSTime.ValidateMonth(date.substring(5,7));
	}else if(date.length==10){
		if(date.charAt(4)!='-'||date.charAt(7)!='-'){
			throw INCORRECT_DATA_TYPE;
		}
		year=LMSTime.ValidateYear(date.substring(0,4));
		month=LMSTime.ValidateMonth(date.substring(5,7));
		day=LMSTime.ValidateDay(date.substring(8,10));
	}else{
		throw INCORRECT_DATA_TYPE;
	}
	if(isNaN(year)||isNaN(month)||isNaN(day)){
		throw INCORRECT_DATA_TYPE;
	}
	var date=new Date(year,month,day,0,0,0);
	var millis=date.getTime()+timeMilliseconds;
	return new LMSTime(millis);
};
LMSTime.prototype.getTime=function(){
	return this.time;
};
LMSTime.prototype.toString=function(){
	var d=this.toDate();
	return d.getFullYear()+"-"+
	d.getMonth()+"-"+
	d.getDay()+"T"+
	d.getHours()+":"+
	d.getMinutes()+":"+
	d.getSeconds()+"."+
	d.getMilliseconds();
};
LMSTime.prototype.toDate=function(){
	return new Date(this.time);
};
LMSTime.ValidateTime=function(timeComponent){
	var hours=0;
	var minutes=0;
	var milliseconds=0;
	var tokens=timeComponent.split(":");
	if(tokens.length==1){
		if(timeComponent.length!=2){
			throw INCORRECT_DATA_TYPE;
		}
		hours=new Number(tokens[0]);
	}else if(tokens.length==2){
		if(timeComponent.length!=5){
			throw INCORRECT_DATA_TYPE;
		}
		hours=new Number(tokens[0]);
		minutes=new Number(tokens[1]);
	}else if(tokens.length==3){
		if(timeComponent.length>11){
			throw INCORRECT_DATA_TYPE;
		}
		hours=new Number(tokens[0]);
		minutes=new Number(tokens[1]);
		var sSeconds=tokens[2];
		if(sSeconds.length>5){
			throw INCORRECT_DATA_TYPE;
		}else if(sSeconds.length<2){
			throw INCORRECT_DATA_TYPE;
		}else{
			var decimalPos=sSeconds.indexOf(".");
			if(decimalPos!=-1){
				if(decimalPos!=2){
					throw INCORRECT_DATA_TYPE;
				}else if(sSeconds.length<4){
					throw INCORRECT_DATA_TYPE;
				}
			}else if(sSeconds.length>2){
				throw INCORRECT_DATA_TYPE;
			}
		}
		var f=new Number(sSeconds);
		milliseconds=f*1000;
	}else{
		throw INCORRECT_DATA_TYPE;
	}
	if(isNaN(hours)||hours<0||hours>23){
		throw INCORRECT_DATA_TYPE;
	}
	if(isNaN(minutes)||minutes<0||minutes>59){
		throw INCORRECT_DATA_TYPE;
	}
	if(isNaN(milliseconds)||milliseconds<0||milliseconds>59990){
		throw INCORRECT_DATA_TYPE;
	}
	return(hours*3600000)+(minutes*60000)+milliseconds;
};
LMSTime.ValidateYear=function(value){
	var year=new Number(value);
	if(isNaN(year)||year<1970||year>2038){
		throw INCORRECT_DATA_TYPE;
	}
	return year;
};
LMSTime.ValidateMonth=function(value){
	var month=new Number(value);
	if(isNaN(month)||month<1||month>12){
		throw INCORRECT_DATA_TYPE;
	}
	return month;
}
LMSTime.ValidateDay=function(value){
	var day=new Number(value);
	if(isNaN(day)||day<1||day>31){
		throw INCORRECT_DATA_TYPE;
	}
	return day;
};
function LMSTimeInterval()
{
	this.time=0;
	if(typeof(arguments[0])!='undefined'){
		this.time=arguments[0];
	}
}
LMSTimeInterval.SECONDS_IN_MIN=60;
LMSTimeInterval.SECONDS_IN_HOUR=LMSTimeInterval.SECONDS_IN_MIN*60;
LMSTimeInterval.SECONDS_IN_DAY=LMSTimeInterval.SECONDS_IN_HOUR*24;
LMSTimeInterval.SECONDS_IN_MONTH=LMSTimeInterval.SECONDS_IN_DAY*30;
LMSTimeInterval.SECONDS_IN_YEAR=LMSTimeInterval.SECONDS_IN_DAY*365;
LMSTimeInterval.parse=function(timeInterval){
if(timeInterval==null||timeInterval==""){
	return new LMSTimeInterval(0);
}
try{
	if(timeInterval.charAt(0).toUpperCase()!='P'){
		throw INCORRECT_DATA_TYPE;
	}
	var totalTime=0;
	var value="";
	var isTimeComponent=false;
	for(var i=1;i<timeInterval.length;i++){
		var char=timeInterval.charAt(i).toUpperCase();
		if(char.match(/[A-Z]/)!=null){
			if(char=='T'){
				if(i==(timeInterval.length-1)){
					throw INCORRECT_DATA_TYPE;
				}
				isTimeComponent=true;
			}else{
				var n=new Number(value);
				if(isNaN(n)){
					throw INCORRECT_DATA_TYPE;
				}else if(char=='Y'){
					totalTime+=n*LMSTimeInterval.SECONDS_IN_YEAR;
				}else if(char=='M'){
					if(isTimeComponent){
						totalTime+=n*LMSTimeInterval.SECONDS_IN_MIN;
					}else{
						totalTime+=n*LMSTimeInterval.SECONDS_IN_MONTH;
					}
				}else if(char=='D'){
					totalTime+=n*LMSTimeInterval.SECONDS_IN_DAY;
				}else if(char=='H'){
					if(!isTimeComponent){
						throw INCORRECT_DATA_TYPE;
					}
					totalTime+=n*LMSTimeInterval.SECONDS_IN_HOUR;
				}else if(char=='S'){
					if(!isTimeComponent){
						throw INCORRECT_DATA_TYPE;
					}
					totalTime+=n;
				}else{
					throw INCORRECT_DATA_TYPE;
				}
			}
			value="";
		}else{
			value+=timeInterval.charAt(i);
		}
	}
	return new LMSTimeInterval(totalTime);
}catch(e){
	throw INCORRECT_DATA_TYPE;
}};
LMSTimeInterval.prototype.getTime=function(){
	return this.time;
};
LMSTimeInterval.prototype.toString=function(){
	var total=new Number(this.time);
	var remainder=total.doubleValue()-total.longValue();
	var secsLeft=total.longValue();
	var years=Math.floor(total/LMSTimeInterval.SECONDS_IN_YEAR);
	secsLeft=secsLeft-(years*LMSTimeInterval.SECONDS_IN_YEAR);
	var months=Math.floor(secsLeft/LMSTimeInterval.SECONDS_IN_MONTH);
	secsLeft=secsLeft-(months*LMSTimeInterval.SECONDS_IN_MONTH);
	var days=Math.floor(secsLeft/LMSTimeInterval.SECONDS_IN_DAY);
	secsLeft=secsLeft-(days*LMSTimeInterval.SECONDS_IN_DAY);
	var hours=Math.floor(secsLeft/LMSTimeInterval.SECONDS_IN_HOUR);
	secsLeft=secsLeft-(hours*LMSTimeInterval.SECONDS_IN_HOUR);
	var minutes=Math.floor(secsLeft/LMSTimeInterval.SECONDS_IN_MIN);
	secsLeft=secsLeft-(minutes*LMSTimeInterval.SECONDS_IN_MIN);
	var result="P";
	if(years>0){
		result+=years;
		result+='Y';
	}
	if(months>0){
		result+=months;
		result+='M';
	}
	if(days>0){
		result+=days;
		result+='D';
	}
	result+='T';
	if(hours>0){
		result+=hours;
		result+='H';
	}
	if(minutes>0){
		result+=minutes;
		result+='M';
	}
	result+=seconds.toFixed(2);
	result+='S';
	return result;
};

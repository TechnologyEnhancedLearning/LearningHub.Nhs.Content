<!-- $Header: adapter_request_queue.js 120.0.12010000.1 2020/11/03 07:44:13 smahanka noship $ -->
class RequestError extends Error
{
   constructor(action, requestPayload)
   {
      super(action, requestPayload);
      this.action = action;
      this.requestPayload = requestPayload;
   }
}

function RequestQueue()
{
   this._initialize();
}

RequestQueue.prototype._initialize=function()
{
   this.queue = [];
   this.running = false;
   this.errorCode = "";
   this.lastException = null;
};

/**
 * add - It adds the request to the 'queue'. If request execution is not running then
 * it will start executing the requests.
 * 
 * action: action type
 * payload: request payload
 * caller: Caller, who calls the add function
 **/
RequestQueue.prototype.add=function(action, payload, caller)
{
   if(!this.isFailed())
   {
      this.queue.push({"action": action, "payload": payload, "caller": caller});
      if(!this.isRunning())
      {
         this.start();
      }
   }
};

/**
 * clear - It clears the request queue and sets running flag as false
 **/
RequestQueue.prototype.clear=function()
{
   this.queue.length = 0;
   this.running = false;
};

/**
 * count - It returns queue length
 **/
RequestQueue.prototype.count=function()
{
   return this.queue.length;
};

/**
 * isRunning - It returns whether request queue execution is running or not
 **/
RequestQueue.prototype.isRunning=function()
{
   return this.running;
};

/**
 * isFailed - It returns any request is failed or not in request queue.
 * If any request fails in request queue, then we will clear the queue.
 **/
RequestQueue.prototype.isFailed=function()
{
   return this.lastException != null;
};

/**
 * setErrorCode - It sets the errorCode
 * If request response get any error then it will set the errorCode
 **/
RequestQueue.prototype.setErrorCode=function(errorCode)
{
   this.errorCode = errorCode;
};

/**
 * getErrorCode - It returns errorCode
 **/
RequestQueue.prototype.getErrorCode=function()
{
   return this.errorCode;
};

/**
 * isEmpty - It returns weather queue is empty or not
 **/
RequestQueue.prototype.isEmpty=function()
{
   return this.count() == 0;
};

/**
 * Executes all the requests in the queue in an asynchronous way
 * 
 * In this function, we are calling caller's mandatory and optional callback functions
 * Mandatory callback functions: sendAsyncRequest
 * Optional callback functions: doAfterSendRequest
 * 
 * caller: Caller, who calls the add function
 **/
RequestQueue.prototype.start=async function()
{
   while(!this.isEmpty())
   {
      this.running = true;
      var request = this.queue[0];
      var action = request.action;
      var msg = request.payload;
      var caller = request.caller;
        
      var startTime = new Date();
      var responseString = "";
      await caller.sendAsyncRequest(msg).then(response => {responseString = response;}).catch(errorMsg => {this.errorCode = COMMUNICATION_GENERAL_EXCEPTION;
                                                                                                            this.clear();
                                                                                                            this.lastException = new RequestError(action, msg);});

      var endTime = new Date();
      caller.log("Reply:",1);
      var response = caller.processResponseString(responseString);
      caller.log("Time taken: " + (endTime - startTime) + " ms.", 5);
      if(typeof caller.doAfterSendRequest == "function")
      {
         caller.doAfterSendRequest(response, action);
      }

      // If a server error is indicated, throw a duplicate
      // exception.  If no error entry is found, what we have
      // here is a failure to communicate.
      // We clear the request queue if any request gets an error
      var errorCode = response.get("error");
      if(errorCode == null)
      {
         this.errorCode = COMMUNICATION_GENERAL_EXCEPTION;
         this.clear();
         this.lastException = new RequestError(action, msg);
      }else if(errorCode != NO_ERROR)
      {
         this.errorCode = errorCode;
         this.clear();
         this.lastException = new RequestError(action, msg);
      }

      // If async request is successful then remove first object from Queue
      if(!this.isFailed())
      {
         this.queue.shift();
      }
   }
   this.running = false;
};
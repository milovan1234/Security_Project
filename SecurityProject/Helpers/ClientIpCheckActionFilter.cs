﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SecurityProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SecurityProject.Helpers
{
    public class ClientIpCheckActionFilter : ActionFilterAttribute
    {
        public static Dictionary<string, RequestDetails> requests = new Dictionary<string, RequestDetails>();
        private TimeSpan SECONDS_BETWEEN_REQUESTS = TimeSpan.FromSeconds(120);
        private TimeSpan MINUTES_FOR_BLOCKED = TimeSpan.FromMinutes(1);
        private const int MAX_NUMBER_OF_REQUEST = 1;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            CheckBlockedIpAddresses();
            string remoteIpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            RequestDetails requestDetails = requests.FirstOrDefault(x => x.Key.Equals(remoteIpAddress)).Value;
            if(requestDetails == null)
            {
                requestDetails = new RequestDetails()
                {
                    NumberOfRequests = 0,
                    LastRequest = DateTime.Now,
                    isBlocked = false
                };
                requests.Add(remoteIpAddress, requestDetails);
            }
            else
            {                
                if (requestDetails.isBlocked)
                {
                    context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                    return;
                }
                requestDetails.LastRequest = DateTime.Now;
                if ((DateTime.Now - requestDetails.LastRequest) < SECONDS_BETWEEN_REQUESTS)
                {
                    
                    requestDetails.NumberOfRequests += 1;
                    requestDetails.isBlocked = requestDetails.NumberOfRequests > MAX_NUMBER_OF_REQUEST;
                }
            }
            base.OnActionExecuting(context);
        }

        private void CheckBlockedIpAddresses()
        {
            foreach (var key in requests.Keys)
            {
                if (requests[key].isBlocked)
                {
                    requests[key].isBlocked = ((DateTime.Now - requests[key].LastRequest) < MINUTES_FOR_BLOCKED);
                    requests[key].NumberOfRequests = requests[key].isBlocked ? requests[key].NumberOfRequests : 0;
                }
            }
        }
    }
}

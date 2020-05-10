using Amazon;
using dp.api.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace dp.api.Controllers
{
    public class BaseController : ControllerBase
    {
        public string _clientId = Environment.GetEnvironmentVariable("clientId"); //Your App Client Id
        public readonly RegionEndpoint _region = RegionEndpoint.USEast1;

        protected ClaimedUser GetClaimedUser()
        {
            bool isAdmin = User.IsInRole("Admin");
            var username = User.Claims.FirstOrDefault(c => c.Type == "cognito:username");
            var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            return new ClaimedUser
            {
                UserName = username.Value,
                IsAdmin = isAdmin,
                Email = email.Value
            };
        }
    }
}
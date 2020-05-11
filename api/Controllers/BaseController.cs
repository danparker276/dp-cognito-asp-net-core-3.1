using Amazon;
using dp.api.Models;
using dp.business.Enums;
using dp.business.Models;
using dp.data;
using dp.data.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dp.api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected string _clientId;
        public readonly RegionEndpoint _region = RegionEndpoint.USEast1;
        private string _dpDbConnectionString;
        protected IDaoFactory AdoNetDao => DaoFactories.GetFactory(DataProvider.AdoNet, _dpDbConnectionString);
        protected string _poolId;
        public BaseController()
        {
            _clientId = Environment.GetEnvironmentVariable("clientId"); //Your App Client Id
            _dpDbConnectionString = Environment.GetEnvironmentVariable("dpDbConnectionString");
            _poolId = Environment.GetEnvironmentVariable("poolId");
        }
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
        protected async Task<User> GetClaimedUserInfo()
        {
            bool isAdmin = User.IsInRole("Admin");
            var username = User.Claims.FirstOrDefault(c => c.Type == "cognito:username");
            var email = User.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
            return await AdoNetDao.UserDao.GetUserIdFromCognitoName(username.Value);

        }

        protected async Task<int> ValidateTeamId(int? teamId)
        {
            if (!User.IsInRole(Role.Admin))
            {
                var currentUserId = int.Parse(User.Identity.Name);
                User user = await AdoNetDao.UserDao.GetUserInfo(currentUserId);
                return (int)user.TeamId;
            }
            else
            {
                try
                {
                    return Convert.ToInt32(teamId);
                }
                catch
                {
                    throw new Exception("teamId must not be null");
                }
            }
        }
    }
}
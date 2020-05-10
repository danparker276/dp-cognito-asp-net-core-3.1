using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using dp.api.Filters;
using dp.api.Models;
using dp.business.Enums;
using dp.business.Helpers;
using dp.business.Models;
using dp.data;
using dp.data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dp.api.Controllers
{


    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private string _dpDbConnectionString;
        private IDaoFactory AdoNetDao => DaoFactories.GetFactory(DataProvider.AdoNet, _dpDbConnectionString);
        string _poolId;
        public UsersController()
        {
                _poolId = Environment.GetEnvironmentVariable("poolId");
               _dpDbConnectionString = Environment.GetEnvironmentVariable("dpDbConnectionString");
        }


        [Authorize]
        [HttpGet("get-user")]
        public async Task<IActionResult> GetUser()
        {
            //var userName = int.Parse(User.Identity.Name);
            var cu = GetClaimedUser();
            var cognito = new AmazonCognitoIdentityProviderClient(_region);
            var request = new AdminGetUserRequest()
            {
                Username = cu.UserName,
                UserPoolId= _poolId
            };
            //Various other cognito commands you can do
            var cognitoUser = await cognito.AdminGetUserAsync(request);
            //example get email
            var email = cognitoUser.UserAttributes.Where(c => c.Name == "email").First().Value;


            //Add user to group...
            //await cognito.AdminAddUserToGroupAsync(request);

            return Ok(cognitoUser);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {

            List<User> users = await AdoNetDao.UserDao.GetUserList();
            return Ok(users);
        }

        //Below are tasks for later on when we will have our users create with the new API and not service stack

        /// <summary>
        /// Create a new user from email
        /// </summary>
        /// <param name="user"></param>
        /// <returns>The New UserId or nothing if user exists</returns>

        [Authorize(Roles = Role.Admin)]
        [HttpPost("createuser")]
        public async Task<IActionResult> CreateUser([FromBody]UserCreate user)
        {
            
            if (!Utils.IsValidEmail(user.Email))
                return BadRequest("Valid Email is requried");
            if (String.IsNullOrEmpty(user.Password) || user.Password.Length < 5)
                return BadRequest("Valid Password is requried"); 
            int? userId= await AdoNetDao.UserDao.CreateUser(user);
 
            return Ok(userId);
        }
        

        /// <summary>
        /// Get current information about a userId v2
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // only allow admins to access other user records
            var currentUserId = int.Parse(User.Identity.Name);
            if (id != currentUserId && !User.IsInRole(Role.Admin))
            {
                return Forbid();
            }
            //No reason to go through a service on a simple looking
            User user =  await AdoNetDao.UserDao.GetUserInfo(id);
       
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        /// <summary>
        /// Here you can use an API key in the header x-api-key for auth and it will give you the userId
        /// </summary>
        /// <returns></returns>
        [ApiKeyAuthAtrribute]
        [HttpGet("GetInfoByApiKey")]
        public async Task<IActionResult> GetInfoByApiKey()
        {
            //You can change this to your group or teamId or pass back an object.
            User user = HttpContext.Items["user"] as User;
            if (user.IsActive == false)
            {
                //Add additional Validation here
                return Unauthorized("User is not Active");
            }
            //Do more items here which will be async

            return Ok("User Id:" + user.UserId);
        }

    }
}
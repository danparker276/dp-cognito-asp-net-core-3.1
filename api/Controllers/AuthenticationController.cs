using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace dp.api.Controllers
{

    [ApiController]
    public class AuthenticationController : BaseController
    {

        public class UserRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }


        [HttpPost]
        [Route("api/register")]
        public async Task<ActionResult<string>> Register(UserRequest user)
        {
            var cognito = new AmazonCognitoIdentityProviderClient(_region);

            var request = new SignUpRequest
            {
                ClientId = _clientId,
                Password = user.Password,
                Username = user.Username
            };

            var emailAttribute = new AttributeType
            {
                Name = "email",
                Value = user.Email
            };
            request.UserAttributes.Add(emailAttribute);

            var response = await cognito.SignUpAsync(request);

            return Ok();
        }

        [HttpPost]
        [Route("api/signin")]
        public async Task<ActionResult<string>> SignIn(UserRequest user)
        {

            //This can be set in ENV or set here:
            //AnonymousAWSCredentials cred = new AnonymousAWSCredentials();
            //  string accessKey = "<AccessKey>";
            //  string secretKey = "<SecretKey>";
            //  var cognito = new AmazonCognitoIdentityProviderClient(new BasicAWSCredentials(accessKey, secretKey), _region);
            var cognito = new AmazonCognitoIdentityProviderClient(_region);
            var request = new AdminInitiateAuthRequest
            {
                UserPoolId = "us-east-1_rAg4kAJ6l",
                ClientId = _clientId,
                 
                  
                AuthFlow = AuthFlowType.ADMIN_USER_PASSWORD_AUTH
            };

            request.AuthParameters.Add("USERNAME", user.Username);
            request.AuthParameters.Add("PASSWORD", user.Password);

            var response = await cognito.AdminInitiateAuthAsync(request);

            return Ok(response.AuthenticationResult.IdToken);
        }
    }
}
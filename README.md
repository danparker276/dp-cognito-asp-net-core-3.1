# dp-cognito-asp-net-core-3.1
I wanted to test out adding AWS Cognito to be the JWT token ValidIssuer and use rules. Note this is for core 3.1 note there is a big 
difference between core 2.2 and 3.1 on this. Most of the documentation on the internet is for 2.2 which won't work now.

This is intially taken from https://github.com/danparker276/dp-api-dotnet-core-jwt-users There are more notes there.

The API key management APIs are not in place here, just the database table. This just deals with the Auth Attribute for putting an API key in the header x-api-key and passing a user object back to the controler from the middleware.

This is just a quick check-in

# college-uni
Demo on Identity on ASP.NET Core and EF Core
Note this is strictly a demo of the Web API. No client side views are demonstrated here.

## Requirements
If you have not done so install the .NET Core 2.0 SDK (x64 installer)
https://www.microsoft.com/net/download/core
You will need SQL Server Express
https://www.microsoft.com/en-us/download/details.aspx?id=55994

## Build and Run

In Visual Studio open the solution explorer ensure the CollegeUni project is set as the startup project.
Build the solution (VS should automatically have nuget packages are restored).
Hit F5 to run in debug mode.
On initial startup the app will create the database CollegeUni in your localhost/SQLEXPRESS.

## Using the app
Goto http://petstore.swagger.io/ and enter in the url of the hosted swagger json file and click Explore. This is generated by the demo app. The url will look something like this: http://localhost:52668/swagger/v1/swagger.json
To create a user goto the section /api/Account/register and click the button 'Try it out'

Enter in a user in the following format
{
  "email": "string",
  "password": "string",
  "confirmPassword": "string"
}



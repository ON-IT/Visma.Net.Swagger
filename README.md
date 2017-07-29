# Visma.net Integrations Swagger Client

[![NuGet version](https://badge.fury.io/nu/Visma.net.swagger.svg)](https://www.nuget.org/packages/Visma.net.swagger) 

This is an auto-generated Visma.net Integrations client based on the swagger.json published by Visma. The version number reflects the version of the swagger file it is generated from.

### About

The code (VismaNetIntegrations.cs) is generated using the NSwag project (https://github.com/RSuter/NSwag). 

The PowerShell-script for updating the code is Generate.ps1, and it will download the swagger, do some modifications, generate the code and update the project-file with the correct version.

### Quick samples

#### Generate token using the "new" flow

```csharp
var dev_client_id = "onit_authoriz_secret_stuff";
var dev_secret = "secret-guid-stuff-here";
var redirect_url = "http://localhost:44300/VismaNet/callback";
var url = VismaNetAuthenticationHelper.CreateOAuthUri(dev_client_id, redirect_url);
Console.WriteLine("Please go to the following url and sign in:");
Console.WriteLine(url);

Console.Write("Enter code: ");
var code = Console.ReadLine();
var token = VismaNetAuthenticationHelper.CreateTokenFromCode(dev_client_id, dev_secret, code, redirect_url).Result;
```

#### Using the generated token to list available companies

```csharp
var authentication = new VismaNetSettings {
    Token = token
};
var security = new SecurityClient(authentication);
var contexts = security.GetAvailableUserContextsAsync().Result;
foreach(var context in contexts){
    Console.WriteLine($"{context.Id}: {context.Name}");
}
```

#### Using the generated token to list all customers

```csharp
var authentication = new VismaNetSettings
{
    Token = token
    CompanyId = 1234567
};

var customer = new CustomerClient(authentication);
var all = customer.GetAllAsync().Result;
foreach (var customerDto in all)
{
    Console.WriteLine($"{customerDto.Number}: {customerDto.Name}");
}
```

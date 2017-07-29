$swagger = invoke-webrequest https://integration.visma.net/API-index/doc/swagger

## Replacements for Subscription
$swagger = $swagger -replace """getAllEvents""", """Subscription_getAllEvents"""
$swagger = $swagger -replace """getAllSubscriptions""", """Subscription_getAllSubscriptions"""
$swagger = $swagger -replace """createSubscription""", """Subscription_createSubscription"""
$swagger = $swagger -replace """findSubscriptionById""", """Subscription_findSubscriptionById"""
$swagger = $swagger -replace """updateSubscription""", """Subscription_updateSubscription"""
$swagger = $swagger -replace """deleteSubscription""", """Subscription_deleteSubscription"""
$swagger = $swagger -replace """findSubscriptionById""", """Subscription_findSubscriptionById"""

## Replacements for Security
$swagger = $swagger -replace """testConnection""", """Security_testConnection"""
$swagger = $swagger -replace """revokeSecurityToken""", """Security_revokeSecurityToken"""
$swagger = $swagger -replace """getAvailableUserContexts""", """Security_getAvailableUserContexts"""

## Fix authentication issue in Security controller
$swagger = $swagger -replace "{""name"":""authorization"",""in"":""header"",""required"":false,""type"":""string""}", ""

Set-Content swagger.json $swagger
nswag swagger2csclient /input:swagger.json /namespace:VismaNetIntegrations /output:VismaNetIntegrations.cs /exceptionClass:VismaNetException /generateOptionalParameters:true /dateType:"System.DateTimeOffset" /dateTimeType:"System.DateTimeOffset" /responseClass:"VismaNetResponse" /GenerateClientClasses:true /GenerateDtoTypes:true /WrapDtoExceptions:true /ClientBaseClass:VismaNetClientBase /ConfigurationClass:VismaNetSettings /useBaseUrl:false /useHttpClientCreationMethod:true /UseHttpRequestMessageCreationMethod:true /disposeHttpClient:false

$swagger -match '"version":"([0-9]+.[0-9]+.[0-9]+.[0-9]+)"'
$csproj = Get-Content VismaNetSwagger.csproj
$versionString = '<Version>'+$matches[1]+'</Version>';
$versionString2 = '<AssemblyVersion>'+$matches[1]+'</AssemblyVersion>';
$versionString3 = '<FileVersion>'+$matches[1]+'</FileVersion>';
$csproj = $csproj -replace "\<Version\>([0-9.]+)</Version>", $versionString
$csproj = $csproj -replace "\<AssemblyVersion\>([0-9.]+)</AssemblyVersion>", $versionString2
$csproj = $csproj -replace "\<FileVersion\>([0-9.]+)</FileVersion>", $versionString3
Set-Content VismaNetSwagger.csproj $csproj
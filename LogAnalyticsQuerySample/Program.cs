using LogAnalyticsQuerySample;
using Microsoft.Azure.OperationalInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Rest.Azure.Authentication;

var appSettings = new ConfigurationBuilder()
    .AddUserSecrets("83eed9c3-f345-4885-b870-5d136678d5fd")
    .Build()
    .Get<AppSettings>();

var authEndpoint = "https://login.microsoftonline.com";
var tokenAudience = "https://api.loganalytics.io/";

var adSettings = new ActiveDirectoryServiceSettings
{
    AuthenticationEndpoint = new Uri(authEndpoint),
    TokenAudience = new Uri(tokenAudience),
    ValidateAuthority = true
};

var creds = await ApplicationTokenProvider.LoginSilentAsync(
    appSettings.AADDomain,
    appSettings.ClientId,
    appSettings.ClientSecret,
    adSettings);

var client = new OperationalInsightsDataClient(creds);
client.WorkspaceId = appSettings.WorkspaceId;

var results = client.Query("union * | take 5");

foreach (var record in results.Results)
{
    Console.WriteLine("---------");
    if (record.TryGetValue("TimeGenerated", out var timeGenerated))
    {
        Console.WriteLine(timeGenerated);
    }

    if (record.TryGetValue("Type", out var type))
    {
        Console.WriteLine(type);
    }
}
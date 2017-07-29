using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VismaNetIntegrations
{
    public static class VismaNetAuthenticationHelper
    {
        private const string TokenEndpoint = "security/api/v2/token";

        /// <summary>
        ///     Creates a new token to use with the classic authentication flow
        /// </summary>
        /// <param name="client_id">ISV Client Id provided by Visma</param>
        /// <param name="secret">ISV Client Secret provided by Visma</param>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns></returns>
        public static async Task<string> CreateToken(string client_id, string secret, string username, string password)
        {
            var client = VismaNetClientBase.HttpClient;
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", client_id),
                new KeyValuePair<string, string>("client_secret", secret),
                new KeyValuePair<string, string>("username", username),
                new KeyValuePair<string, string>("password", password),
                new KeyValuePair<string, string>("grant_type", "password")
            });
            var data = await client.PostAsync(TokenEndpoint, content);
            var status = data.StatusCode;
            if (status != HttpStatusCode.OK)
                throw new VismaNetException("Failure to create token.", data.StatusCode.ToString(),
                    await data.Content.ReadAsStringAsync(), data.Headers.ToDictionary(x => x.Key, x => x.Value), null);
            var rsp = JsonConvert.DeserializeObject<JObject>(await data.Content.ReadAsStringAsync());
            return rsp["token"].Value<string>();
        }

        /// <summary>
        ///     Creates a new token using the OAuth authentication flow
        /// </summary>
        /// <param name="client_id">ISV Client Id provided by Visma</param>
        /// <param name="secret">ISV Client Secret provided by Visma</param>
        /// <param name="code"></param>
        /// <param name="redirect_uri"></param>
        /// <returns></returns>
        public static async Task<string> CreateTokenFromCode(string client_id, string secret, string code,
            string redirect_uri)
        {
            var client = VismaNetClientBase.HttpClient;
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", client_id),
                new KeyValuePair<string, string>("client_secret", secret),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirect_uri),
                new KeyValuePair<string, string>("grant_type", "authorization_code")
            });
            var data = await client.PostAsync(TokenEndpoint, content);
            var status = data.StatusCode;
            if (status != HttpStatusCode.OK)
                throw new VismaNetException("Failure to create token from code.", data.StatusCode.ToString(),
                    await data.Content.ReadAsStringAsync(), data.Headers.ToDictionary(x => x.Key, x => x.Value), null);
            var rsp = JsonConvert.DeserializeObject<JObject>(await data.Content.ReadAsStringAsync());
            return rsp["token"].Value<string>();
        }

        /// <summary>
        ///     Create a url for OAuth authentication flow.
        /// </summary>
        /// <param name="client_id">ISV Client Id provided by Visma</param>
        /// <param name="redirect_uri">Uri to redirect to after authentication</param>
        /// <param name="state">State that will be appended to the callback</param>
        /// <returns>Redirect Uri for OAuth authentication flow</returns>
        public static string CreateOAuthUri(string client_id, string redirect_uri, string state = default(string))
        {
            if (string.IsNullOrEmpty(client_id))
                throw new ArgumentException(nameof(client_id));
            if (string.IsNullOrEmpty(redirect_uri))
                throw new ArgumentException(nameof(redirect_uri));
            return
                $"{VismaNetClientBase.ApiBaseUrl}resources/oauth/authorize" +
                $"?response_type=code" +
                $"&client_id={client_id}" +
                $"&scope=financialstasks" +
                $"&redirect_uri={Uri.EscapeDataString(redirect_uri)}" +
                $"&state={state ?? Guid.NewGuid().ToString()}";
        }
    }
}
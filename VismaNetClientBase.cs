using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace VismaNetIntegrations
{
    public abstract class VismaNetClientBase
    {
        internal const string ApiBaseUrl = "https://integration.visma.net/API/";

        internal static readonly HttpClient HttpClient = new HttpClient
        {
            BaseAddress = new Uri(ApiBaseUrl)
        };

        public static readonly string Version;

        static VismaNetClientBase()
        {
            Version = typeof(VismaNetClientBase).GetTypeInfo().Assembly.GetName().Version.ToString();
        }

        private readonly VismaNetSettings _settings;

        protected VismaNetClientBase(VismaNetSettings settings)
        {
            _settings = settings;
            
        }

        protected Task<HttpClient> CreateHttpClientAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(HttpClient);
        }

        protected Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            var httpRequestMessage = new HttpRequestMessage();
            ApplyUserAgent(httpRequestMessage);
            ApplyVismaNetHeaders(httpRequestMessage);
            return Task.FromResult(httpRequestMessage);
        }

        private void ApplyVismaNetHeaders(HttpRequestMessage httpRequestMessage)
        {
            httpRequestMessage.Headers.Add("ipp-application-type", "Visma.net Financials");
            if (_settings != null)
            {
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
                if (_settings.CompanyId > 0)
                    httpRequestMessage.Headers.Add("ipp-company-id", _settings.CompanyId.ToString());
                if (_settings.BranchId > 0)
                    httpRequestMessage.Headers.Add("branchid", _settings.BranchId.ToString());
            }
        }

        private void ApplyUserAgent(HttpRequestMessage httpRequestMessage)
        {
            var version = $"Visma.Net.Swagger/{Version} (+https://github.com/ON-IT/Visma.Net.Swagger)";
            if (!string.IsNullOrEmpty(_settings.ApplicationName))
            {
                httpRequestMessage.Headers.Add("User-Agent", $"{version} ({_settings.ApplicationName})");
            }
            else
            {
                httpRequestMessage.Headers.Add("User-Agent", version);
            }
        }
    }
}
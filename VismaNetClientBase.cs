using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace VismaNetIntegrations
{
    public abstract class VismaNetClientBase
    {
        internal const string ApiBaseUrl = "https://integration.visma.net/API/";
        private readonly VismaNetSettings _settings;
        internal static readonly HttpClient HttpClient = new HttpClient()
        {
            BaseAddress = new Uri(ApiBaseUrl)
        };
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
            httpRequestMessage.Headers.Add("ipp-application-type", "Visma.net Financials");
            if (_settings != null)
            {
                httpRequestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.Token);
                
                if (_settings.CompanyId > 0)
                    httpRequestMessage.Headers.Add("ipp-company-id", _settings.CompanyId.ToString());
                if (_settings.BranchId > 0)
                    httpRequestMessage.Headers.Add("branchid", _settings.BranchId.ToString());
            }
            return Task.FromResult(httpRequestMessage);
        }


    }
}

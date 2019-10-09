using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SpeedUpCoreAPIExample.Interfaces;
using SpeedUpCoreAPIExample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SpeedUpCoreAPIExample.Helpers
{
    // HttpClient for application's own controllers access 
    public class SelfHttpClient : ISelfHttpClient
    {
        private readonly HttpClient _client;

        public SelfHttpClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            string baseAddress = string.Format("{0}://{1}/api/",
                                httpContextAccessor.HttpContext.Request.Scheme,
                                httpContextAccessor.HttpContext.Request.Host);

            _client = httpClient;
            _client.BaseAddress = new Uri(baseAddress);
        }

        // Call any controller's action with HttpPost method and Id parameter.
        // apiRoute - Relative API route.
        // id - The parameter.
        public async Task PostIdAsync(string apiRoute, string id)
        {
            try
            {
                var result = await _client.PostAsync(string.Format("{0}/{1}", apiRoute, id), null).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                //ignore errors
            }
        }
    }
}
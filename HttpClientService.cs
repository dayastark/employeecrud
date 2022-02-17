using AWS.BusinessLogicLayer.Interface;
using AWS.DataAccessLayer.Helper;
using AWS.DataAccessLayer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AWS.BusinessLogicLayer.Services
{
    public class HttpClientService : IHttpClientService
    {
        private readonly IConfiguration configuration;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<HttpClientService> logger;
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpClientService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory, 
            ILogger<HttpClientService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            this.configuration = configuration;
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<T> GetItem<T>(string route)
        {
            var client = CreateClient();
            var response = await client.GetAsync(route);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in GetItem for route {route} : status code {response.StatusCode} - reason {response.ReasonPhrase}");
                throw new HttpRequestException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task<IEnumerable<T>> GetItems<T>(string route)
        {
            var client = CreateClient();
            var response = await client.GetAsync(route);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in GetItems for route {route} : status code {response.StatusCode} - reason {response.ReasonPhrase}");
                throw new HttpRequestException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
            return await response.Content.ReadAsAsync<IEnumerable<T>>();
        }

        public async Task UpdateItem<T>(T item, string route)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync(route, item);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in UpdateItem for route {route} : status code {response.StatusCode} - reason {response.ReasonPhrase}");
                throw new HttpRequestException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task<int> AddItem<T>(T item, string route)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(route, item);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in AddItem for route {route} : status code {response.StatusCode} - reason {response.ReasonPhrase}");
                throw new HttpRequestException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
            return await response.Content.ReadAsAsync<int>();
        }

        public async Task<T> AddItem<T>(string route, T item)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(route, item);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in AddItem for route {route} : status code {response.StatusCode} - reason {response.ReasonPhrase}");
                throw new HttpRequestException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
            return await response.Content.ReadAsAsync<T>();
        }

        public async Task DeleteItem(string route)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync(route);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in Delete Item for route {route} : status code {response.StatusCode} - reason {response.ReasonPhrase}");
                throw new HttpRequestException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public async Task<int> PostItem<T>(T item, string route)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync(route, item);
            if (!response.IsSuccessStatusCode)
            {
                logger.LogError($"Error in PostItem for route {route} : status code {response.StatusCode} - reason {response.ReasonPhrase}");
                throw new HttpRequestException($"{response.StatusCode} - {response.ReasonPhrase}");
            }
            return await response.Content.ReadAsAsync<int>();
        }
        protected HttpClient CreateClient()
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(configuration.GetValue<string>("AccessServiceEndpoint"));

            var user = httpContextAccessor.HttpContext?.User;
            var claims = httpContextAccessor.HttpContext?.User?.Claims.ToList();
            var roleName = claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))?.Value;
            var tenantId = claims?.FirstOrDefault(x => x.Type.Equals(ClaimTypes.UserData, StringComparison.OrdinalIgnoreCase))?.Value;

            var userData = new UserData
            {
                Name = user?.Identity?.Name?.Replace("@", "+"),
                TenantId = !string.IsNullOrEmpty(tenantId) ? Convert.ToInt32(tenantId) : 0,
                RoleName = roleName
            };

            var token = $"{userData.Name}:{tenantId}:{roleName}";
            var bytes = System.Text.Encoding.UTF8.GetBytes(token);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Convert.ToBase64String(bytes));
            return client;
        }
    }
}

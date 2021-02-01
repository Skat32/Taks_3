using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Logic.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Logic.Services
{
    public class HttpClientProxy : IHttpClientProxy
    {
        private readonly IConfiguration _configuration;

        #region | CTOR |

        private readonly HttpClient _httpClient;
        
        public HttpClientProxy(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }
        
        #endregion

        #region | Get methods |

        /// <summary>
        /// Получить результат выполнения Get запроса
        /// </summary>
        public async Task<T> GetAsync<T>(Uri uri, string contentType = "application/json") where T : class
        {
            _httpClient.DefaultRequestHeaders.Add(nameof(HttpRequestHeader.ContentType), contentType);
            var response = await _httpClient.GetAsync(uri);
            return await GetResponseAsync<T>(response.Content);
        }

        /// <summary>
        /// Получить результат выполнения Get запроса
        /// </summary>
        public async Task<T> GetAsync<T>(string url, string contentType = "application/json") where T : class
        {
            return await GetAsync<T>(new Uri(url), contentType);
        }
        
        #endregion

        #region | Post methods |
        
        public async Task<T> PostAsync<T>(Uri uri, object value, string contentType = "application/json") where T : class
        {
            _httpClient.DefaultRequestHeaders.Add(nameof(HttpRequestHeader.ContentType), contentType);
            var response = await _httpClient.PostAsync(uri, new StringContent(value.SerializeObject()));
            
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.SerializeObject());
            
            return await GetResponseAsync<T>(response.Content);
        }

        public async Task<T> PostAsync<T>(string uri, object value, string contentType = "application/json") where T : class
        {
            _httpClient.DefaultRequestHeaders.Add(nameof(HttpRequestHeader.ContentType), contentType);
            var response = await _httpClient.PostAsync(uri, new StringContent(value.SerializeObject()));

            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.SerializeObject());
            
            return await GetResponseAsync<T>(response.Content);
        }

        public async Task PostAsync(Uri uri, object value, string contentType = "application/json")
        {
            _httpClient.DefaultRequestHeaders.Add(nameof(HttpRequestHeader.ContentType), contentType);
            var response = await _httpClient.PostAsync(uri, new StringContent(value.SerializeObject()));
            
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.SerializeObject());
        }

        public async Task PostAsync(string uri, object value, string contentType = "application/json")
        {
            _httpClient.DefaultRequestHeaders.Add(nameof(HttpRequestHeader.ContentType), contentType);
            var response = await _httpClient.PostAsync(uri, new StringContent(value.SerializeObject(), Encoding.UTF8, contentType));
            
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.SerializeObject());
        }

        public void SetAuthorization(AuthenticationHeaderValue headerValue)
        {
            _httpClient.DefaultRequestHeaders.Authorization = headerValue;
        }

        #endregion

        /// <summary>
        /// Получить результат выполнения Get запроса
        /// </summary>
        public async Task<string> GetString(string url)
        {
            var response = await _httpClient.GetStringAsync(url);
            return response;
        }

        #region | Private methods |

        private static async Task<T> GetResponseAsync<T>(HttpContent content) where T : class
        {
            return content != null ? JsonConvert.DeserializeObject<T>(await content.ReadAsStringAsync()) : null;
        }

        #endregion
    }
}
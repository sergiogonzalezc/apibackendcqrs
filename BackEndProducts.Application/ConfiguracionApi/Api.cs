using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace BackEndProducts.Application.ConfiguracionApi
{
    public class Api
    {
        public CallType.EnumCallType Call
        {
            get;
            set;
        }

        private string EndPoint
        {
            get;
            set;
        }

        public Api(string endPonit, CallType.EnumCallType callType)
        {
            EndPoint = endPonit;

            Call = callType;
        }

        /// <summary>
        /// Ejecuta el llamado asíncrona a una API
        /// </summary>
        /// <param name="shortUrl"></param>
        /// <param name="content"></param>
        /// <param name="token"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> CallApi(string shortUrl, object? content = null, string token = null, string methodName = "INFO", FormUrlEncodedContent FormData = null)
        {
            string url = string.Empty;
            string typeCall = string.Empty;
            string jsonObject = string.Empty;

            try
            {
                HttpResponseMessage response;

                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromMinutes(5);

                    url = string.Format("{0}{1}", EndPoint, shortUrl);

                    client.BaseAddress = new Uri(EndPoint);

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(token))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }

                    HttpRequestMessage request = new HttpRequestMessage(CallType.EnumCallType.Post.Equals(Call) ? HttpMethod.Post : HttpMethod.Get, url);

                    if (FormData != null)
                    {
                        request.Content = FormData;
                    }
                    else
                    {
                        if (content != null)
                        {
                            jsonObject = JsonConvert.SerializeObject(content);

                            request.Content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                        }
                    }

                    if (CallType.EnumCallType.Get.Equals(Call))
                    {
                        typeCall = "GetAsync";
                        response = await client.GetAsync(url);
                    }
                    else
                    {
                        typeCall = "PostAsync";
                        response = await client.PostAsync(url, request.Content);

                    }
                }

                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

    public class CallType
    {
        public enum EnumCallType
        {
            Get,
            Post,
            Put
        }
    }

}

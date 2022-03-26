using System.Net;
using System.Net.Http.Headers;

namespace BaiduPanCompareTools
{
    internal enum HttpRequestResultEnum
    {
        Ok,         // 请求成功，返回stateCode为2XX
        Redirect,   // 请求成功，但返回要求跳转
        NotFound,   // 要访问的页面不存在，返回stateCode为404
        Timeout,    // 请求超过等待的最大时长仍未收到服务器响应
        ReqError,   // 请求参数等错误
        OtherError, // 发生其他错误（比如异常的状态码）
    }

    internal class HttpUtil
    {
        public static string GetCookieValueByName(CookieContainer cc, string cookieName)
        {
            foreach (Cookie cookie in cc.GetAllCookies())
            {
                if (cookie.Name == cookieName)
                    return cookie.Value;
            }
            return null;
        }

        public static HttpRequestResultEnum DoGet(HttpClient httpClient, HttpRequestMessage httpRequest,
            Dictionary<string, string> requestHeaders, out string returnContent, out string errorString)
        {
            try
            {
                if (requestHeaders != null)
                {
                    foreach (var keyValuePair in requestHeaders)
                    {
                        httpRequest.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                returnContent = null;
                errorString = $"DoGet请求参数错误，异常信息为：\n{ex.ToString()}";
                return HttpRequestResultEnum.ReqError;
            }

            HttpResponseMessage response = null;
            try
            {
                var task = httpClient.SendAsync(httpRequest);
                task.Wait();
                response = task.Result;
            }
            catch (AggregateException ex)
            {
                returnContent = null;
                errorString = $"DoGet请求超时，异常信息为：\n{ex.ToString()}";
                return HttpRequestResultEnum.Timeout;
            }
            catch (Exception ex)
            {
                returnContent = null;
                errorString = $"DoGet请求发生错误，异常信息为：\n{ex.ToString()}";
                return HttpRequestResultEnum.OtherError;
            }

            if (response.IsSuccessStatusCode)
            {
                returnContent = response.Content.ReadAsStringAsync().Result;
                errorString = null;
                return HttpRequestResultEnum.Ok;
            }
            else if (response.StatusCode >= HttpStatusCode.Ambiguous && response.StatusCode < HttpStatusCode.BadRequest)
            {
                returnContent = response.Content.ReadAsStringAsync().Result;
                errorString = null;
                return HttpRequestResultEnum.Redirect;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                returnContent = response.Content.ReadAsStringAsync().Result;
                errorString = null;
                return HttpRequestResultEnum.NotFound;
            }
            else
            {
                returnContent = response.Content.ReadAsStringAsync().Result;
                errorString = $"DoGet收到服务器返回的状态码为{response.StatusCode}";
                return HttpRequestResultEnum.OtherError;
            }
        }

        public static HttpRequestResultEnum DoPostForForm(HttpClient httpClient, HttpRequestMessage httpRequest,
            Dictionary<string, string> requestHeaders, Dictionary<string, string> requestBodies,
            out string returnContent, out string errorString)
        {
            try
            {
                if (requestHeaders != null)
                {
                    foreach (var keyValuePair in requestHeaders)
                    {
                        httpRequest.Headers.Add(keyValuePair.Key, keyValuePair.Value);
                    }
                }

                FormUrlEncodedContent content = new FormUrlEncodedContent(requestBodies);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
                httpRequest.Content = content;
            }
            catch (Exception ex)
            {
                returnContent = null;
                errorString = $"DoPostForForm请求参数错误，异常信息为：\n{ex.ToString()}";
                return HttpRequestResultEnum.ReqError;
            }

            HttpResponseMessage response = null;
            try
            {
                var task = httpClient.SendAsync(httpRequest);
                task.Wait();
                response = task.Result;
            }
            catch (AggregateException ex)
            {
                returnContent = null;
                errorString = $"DoPostForForm请求超时，异常信息为：\n{ex.ToString()}";
                return HttpRequestResultEnum.Timeout;
            }
            catch (Exception ex)
            {
                returnContent = null;
                errorString = $"DoPostForForm请求发生错误，异常信息为：\n{ex.ToString()}";
                return HttpRequestResultEnum.OtherError;
            }

            if (response.IsSuccessStatusCode)
            {
                returnContent = response.Content.ReadAsStringAsync().Result;
                errorString = null;
                return HttpRequestResultEnum.Ok;
            }
            else if (response.StatusCode >= HttpStatusCode.Ambiguous && response.StatusCode < HttpStatusCode.BadRequest)
            {
                returnContent = response.Content.ReadAsStringAsync().Result;
                errorString = null;
                return HttpRequestResultEnum.Redirect;
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                returnContent = response.Content.ReadAsStringAsync().Result;
                errorString = null;
                return HttpRequestResultEnum.NotFound;
            }
            else
            {
                returnContent = response.Content.ReadAsStringAsync().Result;
                errorString = $"DoPostForForm收到服务器返回的状态码为{response.StatusCode}";
                return HttpRequestResultEnum.OtherError;
            }
        }
    }
}

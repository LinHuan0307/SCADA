using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using GaoYaXianShu.Entity;
using Newtonsoft.Json;

namespace WebApiHelper
{
    /// <summary>
    /// WebAPI 客户端驱动类
    /// </summary>
    public class WebApiClient
    {
        private readonly string _baseUrl;
        private List<MES请求报文头键值对实体类> _pHeader;
        private readonly int _timeout;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="baseUrl">API基础地址</param>
        /// <param name="timeout">超时时间（毫秒），默认30秒</param>
        public WebApiClient(string baseUrl, List<MES请求报文头键值对实体类> pHeader, int timeout = 30000)
        {
            _baseUrl = baseUrl?.TrimEnd('/');
            _pHeader = pHeader;
            _timeout = timeout;
        }

        /// <summary>
        /// 发送POST请求异步版本
        /// </summary>
        /// <typeparam name="TRequest">请求数据类型</typeparam>
        /// <typeparam name="TResponse">响应数据类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="requestData">请求数据</param>
        /// <returns>API响应</returns>
        public async Task<Result<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest requestData)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
                var jsonContent = JsonConvert.SerializeObject(requestData);

                var request = (HttpWebRequest)WebRequest.Create(url);
                foreach (var kvp in _pHeader)
                {
                    request.Headers.Add(kvp.报文键, kvp.报文值);
                }
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = _timeout;

                // 写入请求数据
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    await streamWriter.WriteAsync(jsonContent);
                }

                // 获取响应
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseText = await streamReader.ReadToEndAsync();
                    
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var apiResponse = JsonConvert.DeserializeObject<TResponse>(responseText);
                        return apiResponse == null ? Result.Fail("反序列化失败"): Result.Ok(apiResponse) ;
                    }
                    else
                    {
                        return Result.Fail($"HTTP请求失败: {response.StatusCode}");
                    }
                }
            }
            catch (WebException ex)
            {
                return Result.Fail($"网络请求异常: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result.Fail($"JSON序列化异常: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Fail($"未知异常: {ex.Message}");
            }
        }

        /// <summary>
        /// 发送POST请求（同步版本）
        /// </summary>
        /// <typeparam name="TRequest">请求数据类型</typeparam>
        /// <typeparam name="TResponse">响应数据类型</typeparam>
        /// <param name="endpoint">API端点</param>
        /// <param name="requestData">请求数据</param>
        /// <returns>API响应</returns>
        public Result<TResponse> Post<TRequest, TResponse>(string endpoint, TRequest requestData)
        {
            try
            {
                var url = $"{_baseUrl}/{endpoint.TrimStart('/')}";
                var jsonContent = JsonConvert.SerializeObject(requestData);

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = _timeout;

                // 写入请求数据
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonContent);
                }

                // 获取响应
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var responseText = streamReader.ReadToEnd();
                    
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var apiResponse = JsonConvert.DeserializeObject<TResponse>(responseText);
                        return apiResponse == null ? Result.Fail("响应数据格式错误") :  Result.Ok(apiResponse);
                    }
                    else
                    {
                        return Result.Fail($"HTTP请求失败: {response.StatusCode}");
                    }
                }
            }
            catch (WebException ex)
            {
                return Result.Fail($"网络请求异常: {ex.Message}");
            }
            catch (JsonException ex)
            {
                return Result.Fail($"JSON序列化异常: {ex.Message}");
            }
            catch (Exception ex)
            {
                return Result.Fail($"未知异常: {ex.Message}");
            }
        }
    }
} 
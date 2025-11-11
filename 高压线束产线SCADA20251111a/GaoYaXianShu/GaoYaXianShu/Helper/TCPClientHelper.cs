using FluentResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GaoYaXianShu.Helper
{

    public class TCPClientHelper : IDisposable
    {
        private TcpClient _client;
        private NetworkStream _stream;

        public string Host { get; private set; }
        public int Port { get; private set; }
        public bool IsConnected => _client?.Connected == true && _stream != null;
        public Encoding Encoding { get; set; } = Encoding.UTF8;


        /// <summary>
        /// 异步连接到服务器
        /// </summary>
        public Result Connect(string host = null, int port = 0)
        {
            if (!string.IsNullOrEmpty(host)) Host = host;
            if (port > 0) Port = port;

            try
            {
                _client?.Close();
                _client = new TcpClient();
                _client.Connect(Host, Port);
                _stream = _client.GetStream();
                return Result.Ok();
            }
            catch(Exception ex)
            {
                return Result.Fail(ex.StackTrace + ex.Message);
            }
        }

        /// <summary>
        /// 异步断开连接
        /// </summary>
        public async Task DisconnectAsync()
        {
            _stream?.Close();
            _client?.Close();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 异步发送字符串
        /// </summary>
        public async Task<Result> SendAsync(string data)
        {
            try
            {
                byte[] bytes = Encoding.GetBytes(data);
                await _stream.WriteAsync(bytes, 0, bytes.Length);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.StackTrace + ex.Message);
            }
        }

        /// <summary>
        /// 异步接收字符串
        /// </summary>
        public async Task<Result<string>> ReceiveAsync()
        {
            try
            {
                byte[] buffer = new byte[4096];
                int bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length);

                if(bytesRead <= 0)
                {
                    return Result.Fail("没有收到数据");
                }
                else
                {
                    return Result.Ok(Encoding.GetString(buffer, 0, bytesRead));
                }
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.StackTrace + ex.Message);
            }
        }

        public void Dispose()
        {
            _stream?.Close();
            _client?.Close();
        }
    }
}

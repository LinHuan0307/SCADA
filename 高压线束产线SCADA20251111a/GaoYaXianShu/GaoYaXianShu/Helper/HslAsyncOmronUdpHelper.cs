using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;
using HslCommunication;
using HslCommunication.Profinet;
using HslCommunication.Profinet.Omron;
using FluentResults;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace GaoYaXianShu.Helper
{
    /// <summary>
    /// ★类名：HslAsyncOmronTcpHelper。
    /// ★用途：用于与欧姆龙PLC的通信。
    /// ★依赖：HslCommunication.dll。
    /// ★创建：欧冰凌，2024-06-26 15:51，V1.0.
    /// ★已验证型号：
    /// </summary>
    public class HslAsyncOmronUdpHelper
    {
        private OmronFinsUdp omron { get; set; }

        #region 公共函数
        public Result Open()
        {
            try
            {

                omron = new OmronFinsUdp(Address, Port);

                if (this.LocalPort > 0)
                {
                    omron.LocalBinding = new IPEndPoint(IPAddress.Any, this.LocalPort);
                }

                omron.SA1 = this.SA1;
                omron.GCT = this.GCT;

                if (this.DA1 > 0)
                {
                    omron.DA1 = this.DA1;
                }

                //CP系列PLC: 主要适用于小型控制系统，I/O点数较少，通常用于简单的控制任务,具有较高的性价比和易于使用的特点
                //CJ系列PLC: 主要适用于中型控制系统，具有较高的灵活性和扩展性。CJ系列PLC的I/O点数较多，可根据需要进行扩展，适用于较为复杂的控制任务。
                //CS系列PLC: 主要适用于大型控制系统，具有高性能和可靠性。CS系列PLC可支持大规模的I/O点数和高速运算，适用于高要求的控制任务。
                //NX/NJ系列采用变量编程，但可将全局变量“分配到”CJ系列的内存地址，可分配的区域类型：CIO、WR、HR、DM、EM。
                omron.PlcType = OmronPlcType.CSCJ;
                omron.ReceiveTimeOut = 3000;
                omron.ByteTransform.DataFormat = HslCommunication.Core.DataFormat.CDAB;
                omron.ByteTransform.IsStringReverseByteWord = true;


                IsConnected = true;

                return Result.Ok();
            }
            catch (Exception ex)
            {
                IsConnected = false;
                return Result.Fail(ex.Message);
            }
        }

        public Result Close()
        {
            try
            {
                IsConnected = false;
                
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        #endregion

        #region Convert
        /// <summary>
        /// bool数组类型（高位在左，低位在右）转换为UInt16类型。
        /// </summary>
        /// <param name="bits"></param>
        /// <returns>Word</returns>
        public static UInt16 BitArrayToUInt16(bool[] pBits)
        {
            try
            {
                UInt16 num = 0;
                for (int i = 0; i < 16; i++)
                {
                    if (pBits[i])
                    {
                        num |= (UInt16)(1 << i);
                    }
                }

                return num;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// UInt16类型转换为bool数组类型。
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static bool[] UInt16ToBitArray(UInt16 word)
        {
            try
            {
                bool[] array = new bool[16];
                for (int i = 0; i < 16; i++)
                {
                    array[i] = ((word >> i) & 1) == 1;
                }

                return array;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 读写布尔值
        /// <summary>
        /// 读取软元件的位状态。
        /// </summary>
        /// <param name="pAddr">地址格式："D100.0","C100.15","W100.0","A100.15","H100.0"</param>
        /// <param name="pValue"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public async Task<Result<bool>> ReadBoolAsync(string pAddr)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<bool[]> result = await omron.ReadBoolAsync(pAddr, (ushort)1);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                }

                return Result.Ok(result.Content[0]);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 读取连续软元件的位状态。
        /// </summary>
        /// <param name="pAddr">地址格式："D100.0","C100.15","W100.0","A100.15","H100.0"</param>
        /// <param name="pSize">读取的长度。</param>
        /// <param name="pValues"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public async Task<Result<bool[]>> ReadBoolArrayAsync(string pAddr, ushort pSize)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<bool[]> result =await omron.ReadBoolAsync(pAddr, pSize);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                }

                return Result.Ok(result.Content); 
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        /// <summary>
        /// 写入软元件的位状态。
        /// </summary>
        /// <param name="pAddr"></param>
        /// <param name="pValue"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public async Task<Result> WriteBoolAsync(string pAddr, bool pValue)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValue);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        #endregion

        #region byte
        
        public async Task<Result<byte[]>> ReadByteArrayAsync(string pAddr, ushort pSize)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<byte[]> result = await omron.ReadAsync(pAddr, pSize);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                }

                return Result.Ok(result.Content);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        #endregion

        #region 读写UInt16
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAddr">地址格式："D100","C100","W100","A100","H100"</param>
        /// <param name="pValue"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public async Task<Result<UInt16>> ReadUInt16Async(string pAddr)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }
                
                OperateResult<ushort[]> result =await omron.ReadUInt16Async(pAddr, 1);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                }

                return Result.Ok(result.Content[0]);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result<UInt16[]>> ReadUInt16Array(string pAddr, int pSize)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<ushort[]> result = await omron.ReadUInt16Async(pAddr, (ushort)pSize);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                }

                return Result.Ok(result.Content);

            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result> WriteUInt16Async(string pAddr, UInt16 pValue)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValue);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result> WriteUInt16ArrayAsync(string pAddr, UInt16[] pValues)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValues);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        #endregion

        #region 读写UInt32
        public async Task<Result<UInt32>> ReadUInt32Async(string pAddr)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<UInt32[]> result = await omron.ReadUInt32Async(pAddr, 1);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok(result.Content[0]);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }

        public async Task<Result<UInt32[]>> ReadUInt32ArrayAsync(string pAddr, int pSize)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<UInt32[]> result =await omron.ReadUInt32Async(pAddr, (ushort)pSize);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok(result.Content);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }

        public async Task<Result> WriteUInt32Async(string pAddr, UInt32 pValue)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValue);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result> WriteInt32Async(string pAddr, Int32 pValue)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValue);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);

                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        
        public async Task<Result> WriteUInt32ArrayAsync(string pAddr, UInt32[] pValues)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValues);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }

        #endregion

        #region 读写Int64

        public async Task<Result<Int64>> ReadInt64Async(string pAddr)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<Int64[]> result = await omron.ReadInt64Async(pAddr, 1);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);

                }

                return Result.Ok(result.Content[0]);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }
        public async Task<Result> WriteInt64Async(string pAddr, Int64 pValue)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValue);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);

                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
        #endregion

        #region 读写Float
        public async Task<Result<float>> ReadFloatAsync(string pAddr)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<float[]> result =await omron.ReadFloatAsync(pAddr, 1);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok(result.Content[0]);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }

        public async Task<Result<float[]>> ReadFloatArrayAsync(string pAddr, int pSize)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<float[]> result =await omron.ReadFloatAsync(pAddr, (ushort)pSize);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok(result.Content);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }

        public async Task<Result> WriteFloatAsync(string pAddr, float pValue)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValue);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }

        public async Task<Result> WriteFloatArrayAsync(string pAddr, float[] pValues)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValues);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }
        #endregion

        #region 读写Double
        public async Task<Result<double>> ReadDoubleAsync(string pAddr)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<double[]> result = await omron.ReadDoubleAsync(pAddr, 1);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);

                }

                return Result.Ok(result.Content[0]);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }
        public async Task<Result> WriteDoubleAsync(string pAddr, double pValue)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValue);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);

                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }
        #endregion

        #region 读写Int32
        public async Task<Result<Int32>> ReadInt32Async(string pAddr)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<Int32[]> result = await omron.ReadInt32Async(pAddr, 1);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);

                }


                return Result.Ok(result.Content[0]);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }
        #endregion

        #region 读写Int16
        public async Task<Result<Int16>> ReadInt16Async(string pAddr)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<Int16[]> result =await omron.ReadInt16Async(pAddr, 1);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);

                }


                return Result.Ok(result.Content[0]);
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);

            }
        }
        #endregion

        #region 读写十六进制字符串
        /// <summary>
        /// 读取十六进制字符串，以双字(UInt32)来解析。
        /// </summary>
        /// <param name="pAddr"></param>
        /// <param name="pSize">字节长度</param>
        /// <param name="pValue"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public async Task<Result<string>> ReadHexStringAsync(string pAddr, int pSize)
        {
            try
            {
                var  pValue = new System.Text.StringBuilder();
                if (omron == null) { return Result.Fail("未实例化."); }
                var res = await ReadUInt32ArrayAsync(pAddr, pSize);
                if (res.IsFailed)
                {
                    return Result.Fail(res.Errors);
                }
                var buffer = res.Value;
                foreach(var n in buffer)
                {
                    pValue.Append(n.ToString("x8"));
                }

                return Result.Ok(pValue.ToString());
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }

        /// <summary>
        /// 写入十六进制字符串。
        /// </summary>
        /// <param name="pAddr"></param>
        /// <param name="pValue">一个单位的字符串长度为4，表示一个UInt32数。</param>
        /// <param name="err"></param>
        /// <returns></returns>
        public async Task<Result> WriteHexStringAsync(string pAddr, string pValue)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }
                if (string.IsNullOrEmpty(pValue)) { return Result.Fail("字符串为空."); }
                if (pValue.Length % 8 != 0) { return Result.Fail("字符串长度不是8的整数倍.");  }

                List<UInt32> list = new List<UInt32>();
                
                int numberCount = pValue.Length / 8;
                for (int i = 0; i < numberCount; i++)
                {
                    string hexByte = pValue.Substring(i * 8, 8);
                    list[i] = uint.Parse(hexByte, NumberStyles.HexNumber);
                }

                var res = await WriteUInt32ArrayAsync(pAddr, list.ToArray());
                if (res.IsFailed)
                {
                    return Result.Fail(res.Errors);
                }
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }
        #endregion

        #region 读写字符串
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAddr"></param>
        /// <param name="pSize"></param>
        /// <param name="pEncoding">包含中文，使用UTF8</param>
        /// <param name="pValue"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public async Task<Result<string>> ReadStringAsync(string pAddr, int pSize, System.Text.Encoding pEncoding)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }

                OperateResult<string> result = await omron.ReadStringAsync(pAddr, (ushort)pSize, pEncoding);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok(result.Content.Replace("\0", ""));
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pAddr"></param>
        /// <param name="pEncoding">包含中文，使用UTF8</param>
        /// <param name="pValue"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        public async Task<Result> WriteStringAsync(string pAddr,  string pValue,System.Text.Encoding pEncoding)
        {
            try
            {
                if (omron == null) { return Result.Fail("未实例化."); }
                if (string.IsNullOrEmpty(pValue)) { return Result.Fail("字符串为空."); }

                OperateResult result = await omron.WriteAsync(pAddr, pValue, pEncoding);
                if (!result.IsSuccess)
                {
                    return Result.Fail(result.Message);
                    
                }

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
                
            }
        }
        #endregion

        #region 公共属性。
        /// <summary>
        /// 设备是否连接?
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// 设备的连接地址。
        /// </summary>
        public string Address { get; set; } = "127.0.0.1";
        /// <summary>
        /// 设备的连接端口。
        /// </summary>
        public int Port { get; set; } = 9600;
        /// <summary>
        /// 设备的本地端口。
        /// </summary>
        public int LocalPort { get; set; } = 0;

        public byte SA1 { get; set; } = 1;

        public byte DA1 { get; set; } = 0;

        public byte GCT { get; set; } = 1;
        #endregion


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BydDCS.Entity
{
    //用于MES请求回复的字段
    /// <summary>
    /// 物料绑定
    /// </summary>
    public class MaterialBindRequest
    {
        public string SnNumber { get; set; }
        public string AssemblySnNumber { get; set; }
        public string LineCode { get; set; }
        public string StationCode { get; set; }
    }
    /// <summary>
    /// 获取工位物料绑定状态
    /// </summary>
    public class MaterialStationStatusRequest
    {
        public string SnNumber { get; set; }
        public string LineCode { get; set; }
        public string StationCode { get; set; }
    }

    /// <summary>
    /// 申请SN
    /// </summary>
    public class RequestSnNumber
    {
        public string LineCode { get; set; }
    }

    /// <summary>
    /// 过站
    /// </summary>
    public class PassStationRequest
    {
        public string SnNumber { get; set; }
        public string TrayCode { get; set; }
        public string LineCode { get; set; }
        public string StationCode { get; set; }
        public string PassType { get; set; }
    }
    /// <summary>
    /// 测试数据
    /// </summary>
    public class TestDataRequest
    {
        public string SnNumber { get; set; }
        public string LineCode { get; set; }
        public string StationCode { get; set; }
        public string TestType { get; set; }
        public string TestName { get; set; }
        /// <summary>
        /// ok,ng,none
        /// </summary>
        public string Result {  get; set; }
        public DateTime TestStartTime {  get; set; }
        public DateTime TestEndTime { get; set; }
        public List<TestDataRequestItem> TestDataList { get; set; } = new List<TestDataRequestItem>();
    }
    public class TestDataRequestItem
    {
        public string TestItemName { get; set;}
        public string TestItemStand { get; set; }
        public string TestItemValue { get; set; }
        public string TestItemResult {  get; set; }
    }
}

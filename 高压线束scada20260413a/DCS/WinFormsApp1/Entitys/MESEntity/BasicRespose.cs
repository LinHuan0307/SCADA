using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BydDCS.Entity
{
    /// <summary>
    /// 用于MES的请求回复字段
    /// </summary>
    public class BasicRespose
    {
        public string ChecKedID { get; set; }
        public string Result { get; set; }
        public string Message { get; set; }
    }
    public class MesRespose
    {
        public string code { get; set; }
        public string message { get; set; }
        public bool IsPass { get; set; }
    }
    public class ApiRespose
    {
        public bool Success { get; set; }
        public string Mesg { get; set; }
    }
    public class ApiRespose<T>
    {
        public bool Success { get; set; }
        public string Mesg { get; set; }
        public T Data { get; set; }
    }
    public class ProcessPassStationResponse
    {
        public bool Success { get; set; }
        public bool IsNeedOperationOperation { get; set; }
        public string Mesg { get; set; }
    }
    public class MaterialBindingStatus
    {
        public string SnNumber { get; set; }
        public string CurStationCode { get; set; }
        public string OperationCode { get; set; }
        public List<MaterialBindRelation> Relations { get; set; } = new List<MaterialBindRelation>();
    }
    public class MaterialBindRelation
    {
        // 物料编码
        public string MaterialCode { get; set; }
        // 物料名称
        public string MaterialName { get; set; }
        // SN信息
        public List<MaterialBindRelationItem> MaterialSnNumbers { get; set; } = new List<MaterialBindRelationItem>();
        // 物料类型 批次？单体
        public string MaterialType { get; set; }
        // 绑定数量
        public int BindingNum { get; set; }
        // 需要绑定数量
        public int RequiredNum { get; set; }
        // 是否满足要求
        public bool IsSatisfied { get; set; }
        // 是否需要顺序绑定
        public bool IsSorted { get; set; }
        // 绑定顺序
        public int SortItem { get; set; }
    }
    public class MaterialBindRelationItem
    {
        // 物料编码
        public string MaterialCode { get; set; }
        // 物料SN
        public string MaterialSnNumber { get; set; }
        // 绑定工位
        public string BindstationCode { get; set; }
        // 绑定时间
        public DateTime BindTime { get; set; }
    }


}

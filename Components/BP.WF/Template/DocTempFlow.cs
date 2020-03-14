using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;

namespace BP.WF.Template
{
    /// <summary>
    /// 属性
    /// </summary>
    public class DocTempFlowAttr
    {
        /// <summary>
        /// 业务号
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 模板编号
        /// </summary>
        public const string TempNo = "TempNo";
    }
    /// <summary>
    ///  
    /// </summary>
    public class DocTempFlow : EntityMyPK
    {
        #region  属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }

        /// <summary>
        /// 路径
        /// </summary>
        public int WorkID
        {
            get
            {
                return this.GetValIntByKey(DocTempFlowAttr.WorkID);
            }
            set
            {
                this.SetValByKey(DocTempFlowAttr.WorkID, value);
            }
        }
        /// <summary>
        /// 要填充的字段
        /// </summary>
        public string TempNo
        {
            get
            {
                return this.GetValStrByKey(DocTempFlowAttr.TempNo);
            }
            set
            {
                this.SetValByKey(DocTempFlowAttr.TempNo, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 单据模板
        /// </summary>
        public DocTempFlow() { }
        public DocTempFlow(string no) : base(no.Replace("\n", "").Trim())
        {
        }
        public DocTempFlow(int WorkID, string TempNo)
        {
            this.WorkID = WorkID;
            this.TempNo = TempNo;
            this.MyPK = this.WorkID + "_" + this.TempNo;
            this.Retrieve();
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("WF_DocTempFlow", "公文模板与业务记录");

                map.Java_SetCodeStruct("6");

                map.AddMyPK();
                map.AddTBInt(DocTempFlowAttr.WorkID, 0, "业务号", true, true);
                map.AddTBInt(DocTempFlowAttr.TempNo, 0, "模板编号", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 
    /// </summary>
    public class DocTempFlows : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DocTempFlow();
            }
        }
        /// <summary>
        /// 单据模板
        /// </summary>
        public DocTempFlows()
        {
        }
        #endregion


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DocTempFlow> ToJavaList()
        {
            return (System.Collections.Generic.IList<DocTempFlow>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DocTempFlow> Tolist()
        {
            System.Collections.Generic.List<DocTempFlow> list = new System.Collections.Generic.List<DocTempFlow>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DocTempFlow)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }

}

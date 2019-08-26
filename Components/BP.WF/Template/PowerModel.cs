using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Template
{
    /// <summary>
    /// 权限模型属性
    /// </summary>
    public class PowerModelAttr
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FlowNo = "FlowNo";
        /// <summary>
        /// 节点
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 权限标记
        /// </summary>
        public const string ModelFlag = "ModelFlag";
        /// <summary>
        /// 控制类型
        /// </summary>
        public const string Model = "Model";
        public const string ModelType = "ModelType";
        /// <summary>
        /// 人员编号 ModelFlagName.
        /// </summary>
        public const string ModelFlagName = "ModelFlagName";

        /// <summary>
        /// 人员编号
        /// </summary>
        public const string EmpNo = "EmpNo";
        public const string EmpName = "EmpName";
        public const string StaNo = "StaNo";
        public const string StaName = "StaName";
        public const string FrmID = "FrmID";
    }
    /// <summary>
    /// 权限模型
    /// </summary>
    public class PowerModel : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FlowNo
        {
            get
            {
                return this.GetValStringByKey(PowerModelAttr.FlowNo);
            }
            set
            {
                this.SetValByKey(PowerModelAttr.FlowNo, value);
            }
        }
        /// <summary>
        /// 权限标记
        /// </summary>
        public string ModelFlag
        {
            get
            {
                return this.GetValStringByKey(PowerModelAttr.ModelFlag);
            }
            set
            {
                this.SetValByKey(PowerModelAttr.ModelFlag, value);
            }
        }
        /// <summary>
        ///节点
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(PowerModelAttr.NodeID);
            }
            set
            {
                this.SetValByKey(PowerModelAttr.NodeID, value);
            }
        }
        public string EmpNo
        {
            get
            {
                string s = this.GetValStringByKey(PowerModelAttr.EmpNo);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "";
                return s;
            }
            set
            {
                this.SetValByKey(PowerModelAttr.EmpNo, value);
            }
        }
        public string ModelFlagName
        {
            get
            {
                string s = this.GetValStringByKey(PowerModelAttr.ModelFlagName);
                if (DataType.IsNullOrEmpty(s) == true)
                    s = "";
                return s;
            }
            set
            {
                this.SetValByKey(PowerModelAttr.ModelFlagName, value);
            }
        }
        #endregion
           

        #region 构造方法
        /// <summary>
        /// 权限模型
        /// </summary>
        public PowerModel()
        {
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

                Map map = new Map("WF_PowerModel", "权限模型");

                map.AddMyPK();

                map.AddTBString(PowerModelAttr.Model, null, "模式", true, false, 0, 100, 10);

                map.AddDDLSysEnum(PowerModelAttr.ModelType, 0, "控制类型", true, false, PowerModelAttr.ModelType,
                    "@0=岗位@1=工作人员");

                map.AddTBString(PowerModelAttr.ModelFlag, null, "权限标记", true, false, 0, 100, 10);
                map.AddTBString(PowerModelAttr.ModelFlagName, null, "权限标记名称", true, false, 0, 100, 10);

                map.AddTBString(PowerModelAttr.EmpNo, null, "人员编号", true, false, 0, 100, 10);
                map.AddTBString(PowerModelAttr.EmpName, null, "人员名称", true, false, 0, 100, 10);
                map.AddTBString(PowerModelAttr.StaNo, null, "岗位编号", true, false, 0, 100, 10);
                map.AddTBString(PowerModelAttr.StaName, null, "岗位名称", true, false, 0, 100, 10);

                map.AddTBString(PowerModelAttr.FlowNo, null, "流程编号", true, false, 0, 100, 10);
                map.AddTBInt(PowerModelAttr.NodeID, 0, "节点", true, false);
                map.AddTBString(PowerModelAttr.FrmID, null, "表单ID", true, false, 0, 100, 10);
                 

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        
    }
    /// <summary>
    /// 权限模型
    /// </summary>
    public class PowerModels : EntitiesMyPK
    {
        /// <summary>
        /// 权限模型
        /// </summary>
        public PowerModels() { }
        /// <summary>
        /// 权限模型
        /// </summary>
        /// <param name="FlowNo"></param>
        public PowerModels(string FlowNo)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhereInSQL(PowerModelAttr.NodeID, "SELECT NodeID FROM WF_Node WHERE FlowNo='" + FlowNo + "'");
            qo.DoQuery();
        }
        /// <summary>
        /// 权限模型
        /// </summary>
        /// <param name="nodeid">节点ID</param>
        public PowerModels(int nodeid)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(PowerModelAttr.NodeID, nodeid);
            qo.DoQuery();
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new PowerModel();
            }
        }
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<PowerModel> ToJavaList()
        {
            return (System.Collections.Generic.IList<PowerModel>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<PowerModel> Tolist()
        {
            System.Collections.Generic.List<PowerModel> list = new System.Collections.Generic.List<PowerModel>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((PowerModel)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

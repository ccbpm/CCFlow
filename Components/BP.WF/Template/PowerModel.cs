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
        /// 模块
        /// </summary>
        public const string Model = "Model";
        /// <权限标识>
        /// 权限标记
        /// </summary>
        public const string PowerFlag = "PowerFlag";
        /// <summary>
        /// 权限标记名称
        /// </summary>
        public const string PowerFlagName = "PowerFlagName";
        /// <summary>
        /// 控制类型
        /// </summary>
        public const string PowerCtrlType = "PowerCtrlType";
        /// <summary>
        /// 人员编号 ModelFlagName.
        /// </summary>
        public const string ModelFlagName = "ModelFlagName";
        /// <summary>
        /// 人员编号
        /// </summary>
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 人员名称
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 岗位编号
        /// </summary>
        public const string StaNo = "StaNo";
        /// <summary>
        /// 岗位名称
        /// </summary>
        public const string StaName = "StaName";
        /// <summary>
        /// 表单ID
        /// </summary>
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
        public string PowerFlag
        {
            get
            {
                return this.GetValStringByKey(PowerModelAttr.PowerFlag);
            }
            set
            {
                this.SetValByKey(PowerModelAttr.PowerFlag, value);
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

                //比如： FlowData , FrmData
                map.AddTBString(PowerModelAttr.Model, null, "模块", true, false, 0, 100, 10);

                //权限标记: FlowDataDelete
                map.AddTBString(PowerModelAttr.PowerFlag, null, "权限标识", true, false, 0, 100, 10);
                //权限名称: 流程删除
                map.AddTBString(PowerModelAttr.PowerFlagName, null, "权限标记名称", true, false, 0, 100, 10);

                map.AddDDLSysEnum(PowerModelAttr.PowerCtrlType, 0, "控制类型", true, false, PowerModelAttr.PowerCtrlType,
                    "@0=岗位@1=人员");

                map.AddTBString(PowerModelAttr.EmpNo, null, "人员编号", true, false, 0, 100, 10);
                map.AddTBString(PowerModelAttr.EmpName, null, "人员名称", true, false, 0, 100, 10);

                map.AddTBString(PowerModelAttr.StaNo, null, "岗位编号", true, false, 0, 100, 10);
                map.AddTBString(PowerModelAttr.StaName, null, "岗位名称", true, false, 0, 100, 10);

                //Model标记.
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

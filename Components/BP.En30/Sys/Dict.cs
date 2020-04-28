using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using BP.DA;
using BP.En;
using Microsoft.CSharp;
using BP.Sys;

namespace BP.Sys
{
    /// <summary>
	/// 属性
	/// </summary>
	public class DictAttr: BP.En.EntityMyPKAttr
    {
        /// <summary>
        /// 属性Key
        /// </summary>
        public const string TableID = "TableID";
        /// <summary>
        /// 工作人员
        /// </summary>
        public const string TableName = "TableName";
        /// <summary>
        /// 列选择
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 0=EntityNoName, 1=EntityTree
        /// </summary>
        public const string DictType = "DictType";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 系统字典表
    /// </summary>
    public class Dict : EntityMyPK
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(DictAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(DictAttr.OrgNo, value);
            }
        }
        public string TableID
        {
            get
            {
                return this.GetValStrByKey(DictAttr.TableID);
            }
            set
            {
                this.SetValByKey(DictAttr.TableID, value);
            }
        }
        public string TableName
        {
            get
            {
                return this.GetValStrByKey(DictAttr.TableName);
            }
            set
            {
                this.SetValByKey(DictAttr.TableName, value);
            }
        }
        public int DictType
        {
            get
            {
                return this.GetValIntByKey(DictAttr.DictType);
            }
            set
            {
                this.SetValByKey(DictAttr.DictType, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(DictAttr.Idx);
            }
            set
            {
                this.SetValByKey(DictAttr.Idx, value);
            }
        }
        #endregion 属性.


        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// 系统字典表
        /// </summary>
        public Dict()
        {
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_Dict", "系统字典表");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                // OrgNo+"_"+TableID;
                map.AddMyPK();

                map.AddTBString(DictAttr.TableID, null, "表ID", true, false, 0, 200, 20);
                map.AddTBString(DictAttr.TableName, null, "名称", true, false, 0, 200, 20);
                map.AddDDLSysEnum(DictAttr.DictType, 0, "数据类型", true, false, DictAttr.DictType,
                    "@0=编号名称@1=树结构");

                map.AddTBString(DictAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 20);
                map.AddTBInt(DictAttr.Idx, 0, "顺序号", false, false);

                RefMethod rm = new RefMethod();
                rm.Title = "编辑数据";
                rm.ClassMethodName = this.ToString() + ".DoEdit";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 更新的操作
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            return base.beforeUpdate();
        }

        protected override void afterInsertUpdateAction()
        {
            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public string DoEdit()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/SysDictEditData.htm?FK_Dict=" + this.MyPK;
        }
        protected override bool beforeInsert()
        {
            if (BP.Sys.SystemConfig.CCBPMRunModel!=0)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 系统字典表s
    /// </summary>
    public class Dicts : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 系统字典表s
        /// </summary>
        public Dicts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Dict();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Dict> ToJavaList()
        {
            return (System.Collections.Generic.IList<Dict>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Dict> Tolist()
        {
            System.Collections.Generic.List<Dict> list = new System.Collections.Generic.List<Dict>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Dict)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

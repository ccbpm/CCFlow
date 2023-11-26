using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.WF.Template;
using BP.WF;

namespace BP.CCBill.Template
{
    /// <summary>
    ///  控制模型-属性
    /// </summary>
    public class CtrlModelDtlAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 控制类型
        /// </summary>
        public const string CtrlObj = "CtrlObj";
        /// <summary>
        /// 组织类型 Station,Dept,User
        /// </summary>
        public const string OrgType = "OrgType";
        /// <summary>
        /// IDs
        /// </summary>
        public const string IDs = "IDs";
    }
    /// <summary>
    /// 控制模型
    /// </summary>
    public class CtrlModelDtl : BP.En.EntityMyPK
    {
        #region 基本属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(CtrlModelDtlAttr.FrmID);
            }
            set
            {
                SetValByKey(CtrlModelDtlAttr.FrmID, value);
            }
        }

        /// <summary>
        /// 控制权限
        /// </summary>
        public string CtrlObj
        {
            get
            {
                return this.GetValStringByKey(CtrlModelDtlAttr.CtrlObj);
            }
            set
            {
                SetValByKey(CtrlModelDtlAttr.CtrlObj, value);
            }
        }


        /// <summary>
        /// 组织类型
        /// </summary>
        public string OrgType
        {
            get
            {
                return this.GetValStringByKey(CtrlModelDtlAttr.OrgType);
            }
            set
            {
                SetValByKey(CtrlModelDtlAttr.OrgType, value);
            }
        }
        public string IDs
        {
            get
            {
                return this.GetValStringByKey(CtrlModelDtlAttr.IDs);
            }
            set
            {
                SetValByKey(CtrlModelDtlAttr.IDs, value);
            }
        }
        #endregion 基本属性.

        #region 构造.
        #endregion 构造方法
        public string RptName = null;
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Frm_CtrlModelDtl", "控制模型表Dtl");

                #region 字段
                map.AddMyPK();  //增加一个自动增长的列.

                map.AddTBString(CtrlModelDtlAttr.FrmID, null, "表单ID", true, false, 0, 300, 100);
                //BtnNew,BtnSave,BtnSubmit,BtnDelete,BtnSearch
                map.AddTBString(CtrlModelDtlAttr.CtrlObj, null, "控制权限", true, false, 0, 20, 100);
                //Station,Dept,User
                map.AddTBString(CtrlModelDtlAttr.OrgType, null, "组织类型", true, false, 0, 300, 100);
                map.AddTBString(CtrlModelDtlAttr.IDs, null, "IDs", true, false, 0, 1000, 100);
               
                #endregion 字段.

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 控制模型
        /// </summary>
        public CtrlModelDtl()
        {
        }

        protected override bool beforeInsert()
        {
            this.MyPK = this.FrmID + "_" + this.CtrlObj + "_" + this.OrgType;
            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FrmID + "_" + this.CtrlObj + "_" + this.OrgType;
            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            //修改CtrlModel中的数据
            CtrlModel ctrlM = new CtrlModel();
            ctrlM.MyPK = this.FrmID + "_" + this.CtrlObj;
            if(ctrlM.RetrieveFromDBSources() == 0)
            {
                ctrlM.FrmID = this.FrmID;
                ctrlM.CtrlObj = this.CtrlObj;
                if (this.OrgType.Equals("Station"))
                    ctrlM.IDOfStations = this.IDs;
                if (this.OrgType.Equals("Dept"))
                    ctrlM.IDOfDepts = this.IDs;
                if (this.OrgType.Equals("User"))
                    ctrlM.IDOfUsers = this.IDs;
                ctrlM.Insert();

            }
            else
            {
                if (this.OrgType.Equals("Station"))
                    ctrlM.IDOfStations = this.IDs;
                if (this.OrgType.Equals("Dept"))
                    ctrlM.IDOfDepts = this.IDs;
                if (this.OrgType.Equals("User"))
                    ctrlM.IDOfUsers = this.IDs;
                ctrlM.Update();
            }
             base.afterInsertUpdateAction();


        }
       
    }
    /// <summary>
    /// 控制模型集合s
    /// </summary>
    public class CtrlModelDtls : BP.En.EntitiesMyPK
    {
        #region 构造方法.
        /// <summary>
        /// 控制模型集合
        /// </summary>
        public CtrlModelDtls()
        {
        }
        public override Entity GetNewEntity
        {
            get
            {
                return new CtrlModelDtl();
            }
        }
        #endregion 构造方法.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CtrlModelDtl> ToJavaList()
        {
            return (System.Collections.Generic.IList<CtrlModelDtl>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CtrlModelDtl> Tolist()
        {
            System.Collections.Generic.List<CtrlModelDtl> list = new System.Collections.Generic.List<CtrlModelDtl>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CtrlModelDtl)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}
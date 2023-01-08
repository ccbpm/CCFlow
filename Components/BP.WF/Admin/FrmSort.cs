using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Sys;

namespace BP.WF.Admin
{
    /// <summary>
    /// 表单目录属性
    /// </summary>
    public class FrmSortAttr : EntityTreeAttr
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 简称
        /// </summary>
        public const string ShortName = "ShortName";
        /// <summary>
        /// 域/系统编号
        /// </summary>
        public const string Domain = "Domain";
    }
    /// <summary>
    ///  表单目录
    /// </summary>
    public class FrmSort : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(FrmSortAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(FrmSortAttr.OrgNo, value);
            }
        }
        public string Domain
        {
            get
            {
                return this.GetValStrByKey(FrmSortAttr.Domain);
            }
            set
            {
                this.SetValByKey(FrmSortAttr.Domain, value);
            }
        }
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(FrmSortAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(FrmSortAttr.ParentNo, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 表单目录
        /// </summary>
        public FrmSort()
        {
        }
        /// <summary>
        /// 表单目录
        /// </summary>
        /// <param name="_No"></param>
        public FrmSort(string _No) : base(_No) { }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        /// <summary>
        /// 表单目录Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FormTree", "表单目录");

                map.AddTBStringPK(FrmSortAttr.No, null, "编号", false, false, 1, 100, 20);
                map.AddTBString(FrmSortAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);

                map.AddTBString(FrmSortAttr.Name, null, "名称", true, false, 0, 200, 30, true);
                map.AddTBString(FrmSortAttr.ShortName, null, "简称", true, false, 0, 200, 30, true);

                map.AddTBString(FrmSortAttr.OrgNo, "0", "组织编号(0为系统组织)", false, false, 0, 150, 30);
                map.SetHelperAlert(FrmSortAttr.OrgNo, "用于区分不同组织的的流程,比如:一个集团有多个子公司,每个子公司都有自己的业务流程.");

                map.AddTBString(FrmSortAttr.Domain, null, "域/系统编号", true, false, 0, 100, 30);
                map.SetHelperAlert(FrmSortAttr.Domain, "用于区分不同系统的流程,比如:一个集团有多个子系统每个子系统都有自己的流程,就需要标记那些流程是那个子系统的.");
                map.AddTBInt(FrmSortAttr.Idx, 0, "Idx", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 创建的时候，给他增加一个OrgNo。
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = BP.Web.WebUser.OrgNo;

            return base.beforeInsert();
        }
        /// <summary>
        /// 删除之前的逻辑
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            string sql = "SELECT COUNT(*) as Num FROM Sys_MapData WHERE FK_FormTree='" + this.No + "'";
            int num = DBAccess.RunSQLReturnValInt(sql);
            if (num != 0)
                throw new Exception("err@您不能删除该目录，下面有表单。");

            return base.beforeDelete();
        }
    }
    /// <summary>
    /// 表单目录
    /// </summary>
    public class FrmSorts : EntitiesNoName
    {
        #region 构造.
        /// <summary>
        /// 表单目录s
        /// </summary>
        public FrmSorts() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmSort();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (Glo.CCBPMRunModel != CCBPMRunModel.Single)
                return this.Retrieve(FrmSortAttr.OrgNo, BP.Web.WebUser.OrgNo, FrmSortAttr.Idx);

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(FrmSortAttr.ParentNo, "!=", "0");
            qo.addOrderBy("Idx");
            int i = qo.DoQuery();

            if (i == 0)
            {
                FrmSort fs = new FrmSort();
                fs.Name = "流程树";
                fs.No = "100";
                fs.ParentNo = "0";
                fs.Insert();

                fs = new FrmSort();
                fs.Name = "公文类";
                fs.No = "01";
                fs.ParentNo = "100";
                fs.Insert();

                fs = new FrmSort();
                fs.Name = "办公类";
                fs.No = "02";
                fs.ParentNo = "100";
                fs.Insert();

                qo = new QueryObject(this);
                qo.AddWhere(FrmSortAttr.ParentNo, "!=", "");
                qo.addOrderBy("Idx");
                i = qo.DoQuery();
            }
            return i;
        }
        #endregion 构造.


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmSort> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmSort>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmSort> Tolist()
        {
            System.Collections.Generic.List<FrmSort> list = new System.Collections.Generic.List<FrmSort>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmSort)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

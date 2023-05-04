using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 控件类型
    /// </summary>
    public class GroupCtrlType
    {
        /// <summary>
        /// 框架
        /// </summary>
        public const string Frame = "Frame";
        /// <summary>
        /// 从表
        /// </summary>
        public const string Dtl = "Dtl";
        /// <summary>
        /// 附件
        /// </summary>
        public const string Ath = "Ath";
        /// <summary>
        /// 审核组件
        /// </summary>
        public const string FWC = "FWC";
        /// <summary>
        /// 子流程
        /// </summary>
        public const string SubFlow = "SubFlow";
        /// <summary>
        /// 轨迹
        /// </summary>
        public const string Track = "Track";
        /// <summary>
        /// 子线程
        /// </summary>
        public const string Thread = "Thread";
        /// <summary>
        /// 流转自定义组件
        /// </summary>
        public const string FTC = "FTC";
        /// <summary>
        /// 按钮控件
        /// </summary>
        public const string Btn = "Btn";

    }
    /// <summary>
    /// 分组 - 属性
    /// </summary>
    public class GroupFieldAttr : EntityOIDAttr
    {
        /// <summary>
        /// 表单ID
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 标签
        /// </summary>
        public const string Lab = "Lab";
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 控件类型
        /// </summary>
        public const string CtrlType = "CtrlType";
        /// <summary>
        /// 控件ID
        /// </summary>
        public const string CtrlID = "CtrlID";
        /// <summary>
        /// PC端是否折叠显示？
        /// </summary>
        public const string IsZDPC = "IsZDPC";
        /// <summary>
        /// 手机端是否折叠显示？
        /// </summary>
        public const string IsZDMobile = "IsZDMobile";
        /// <summary>
        /// 分组显示的模式 显示 PC端折叠 隐藏
        /// </summary>
        public const string ShowType = "ShowType";
    }
    /// <summary>
    /// 分组
    /// </summary>
    public class GroupField : EntityOID
    {
        #region 权限控制
        /// <summary>
        /// 权限控制.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.No.Equals("admin") == true || BP.Web.WebUser.IsAdmin)
                {
                    uac.IsDelete = true;
                    uac.IsInsert = false;
                    uac.IsUpdate = true;
                    return uac;
                }
                uac.Readonly();
                uac.IsView = false;
                return uac;
            }
        }
        #endregion 权限控制

        #region 属性
        public bool IsUse = false;
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.FrmID);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.FrmID, value);
            }
        }
        public string EnName
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.FrmID);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string Lab
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.Lab);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.Lab, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(GroupFieldAttr.Idx);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.Idx, value);
            }
        }
        /// <summary>
        /// 控件类型
        /// </summary>
        public string CtrlType
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.CtrlType);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.CtrlType, value);
            }
        }
        /// <summary>
        /// 控件ID
        /// </summary>
        public string CtrlID
        {
            get
            {
                return this.GetValStrByKey(GroupFieldAttr.CtrlID);
            }
            set
            {
                this.SetValByKey(GroupFieldAttr.CtrlID, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// GroupField
        /// </summary>
        public GroupField()
        {
        }
        public GroupField(int oid)
            : base(oid)
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
                Map map = new Map("Sys_GroupField", "傻瓜表单分组");

                #region 字段.
                map.AddTBIntPKOID();
                map.AddTBString(GroupFieldAttr.Lab, null, "标签", true, false, 0, 500, 20, true);
                map.AddTBString(GroupFieldAttr.FrmID, null, "表单ID", true, true, 0, 200, 20);

                map.AddTBString(GroupFieldAttr.CtrlType, null, "控件类型", true, true, 0, 50, 20);
                map.AddTBString(GroupFieldAttr.CtrlID, null, "控件ID", true, false, 0, 500, 20);

                //map.AddBoolean(GroupFieldAttr.IsZDPC, false, "是否折叠(PC)", true, true);
                map.AddBoolean(GroupFieldAttr.IsZDMobile, false, "是否折叠(Mobile)", true, true);
                map.AddDDLSysEnum(GroupFieldAttr.ShowType, 0, "分组显示模式", true, true,
                    GroupFieldAttr.ShowType, "@0=显示@1=PC折叠@2=隐藏");

                map.AddTBInt(GroupFieldAttr.Idx, 99, "顺序号", true, false);
                map.AddTBString(MapAttrAttr.GUID, null, "GUID", true, true, 0, 128, 20, true);
                map.AddTBAtParas(3000);
                #endregion 字段.

                #region 方法.
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "删除隶属分组的字段";
                rm.Warning = "您确定要删除该分组下的所有字段吗？";
                rm.ClassMethodName = this.ToString() + ".DoDelAllField";
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "章节表单分组扩展";
                rm.ClassMethodName = this.ToString() + ".DoSetGFType";
                // rm.HisAttrs.AddDDLSysEnum("Type", 0, "设置类型", true, true, "GFType", "@0=链接到其它表单@1=自定义URL");

                rm.HisAttrs.AddTBInt("Type", 0, "设置类型：0链接到其它表单，1自定义URL", true, false);
                rm.HisAttrs.AddTBString("val", null, "输入对应的值", true, false, 0, 1000, 1000);
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "调整字段顺序";
                rm.ClassMethodName = this.ToString() + ".DoGroupFieldIdx";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                // map.AddRefMethod(rm);
                #endregion 方法.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        #region 方法.
        /// <summary>
        /// 设置分组解析类型(对章节表单有效)
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="val">值</param>
        /// <returns>执行结果</returns>
        public string DoSetGFType(int type, string val)
        {
            MapData md = new MapData(this.FrmID);
            if (md.HisFrmType != FrmType.ChapterFrm)
                return "err@该设置对章节表单有效.";

            //链接到其他表单上
            if (type == 0)
            {
                md.No = val;
                if (md.RetrieveFromDBSources() == 0)
                    return "err@表单ID输入错误[" + val + "].";

                this.CtrlType = "ChapterFrmLinkFrm";
                this.CtrlID = val;
            }

            //如果是自定义url.
            if (type == 1)
            {
                this.CtrlType = "ChapterFrmSelfUrl";
                this.CtrlID = val;
            }

            this.Update();
            return "执行成功.";
        }
        /// <summary>
        /// 外部调用的
        /// </summary>
        /// <returns></returns>
        public string AddGroup()
        {
            this.InsertAsNew();
            return "执行成功.";
        }

        /// <summary>
        /// 删除所有隶属该分组的字段.
        /// </summary>
        /// <returns></returns>
        public string DoDelAllField()
        {
            string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + this.FrmID + "' AND GroupID=" + this.OID + " AND KeyOfEn NOT IN ('OID','RDT','REC','RefPK','FID')";
            int i = DBAccess.RunSQL(sql);
            return "删除字段{" + i + "}个，被删除成功.";
        }
        /// <summary>
        /// 分组内的字段顺序调整
        /// </summary>
        /// <returns></returns>
        public string DoGroupFieldIdx()
        {
            return "../../Admin/FoolFormDesigner/GroupFieldIdx.htm?FrmID=" + this.FrmID + "&GroupField=" + this.OID;
        }
        protected override bool beforeUpdate()
        {
            string sql = "UPDATE Sys_GroupField SET LAB='" + this.Lab + "' WHERE OID=" + this.OID;
            DBAccess.RunSQL(sql);
            return base.beforeUpdate();
        }
        public string DoDown()
        {
            this.DoOrderDown(GroupFieldAttr.FrmID, this.FrmID, GroupFieldAttr.Idx);
            return "执行成功";
        }
        public string DoUp()
        {
            this.DoOrderUp(GroupFieldAttr.FrmID, this.FrmID, GroupFieldAttr.Idx);
            return "执行成功";
        }
        protected override bool beforeDelete()
        {
            string sql = "SELECT Name,KeyOfEn FROM Sys_MapAttr WHERE GroupID=" + this.OID;

            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            string str = "";
            foreach (DataRow dr in dt.Rows)
            {
                str += " \t\n" + dr[0].ToString() +" - " +dr[1].ToString();
            }

            if (DataType.IsNullOrEmpty(str) == false)
            {
                str = "err@分组ID:"+this.OID+"删除分组错误:如下字段存在，您不能删除:"+ str;
                str += "\t\n 您要删除这个分组，请按照如下操作。";
                str += "\t\n 1. 移除字段到其他分组里面去. ";
                str += "\t\n 2. 删除字段. ";
                str += "\t\n 3. 如果是隐藏字段，您可以在表单设计器中，表单属性点开隐藏字段,打开隐藏字段，并编辑所在分组. ";
                str += "\t\n +++++++++ 容器存在的字段 +++++++++++ ";
                str += "\t\n  "+ str;

                throw new Exception(str);
            }

            return base.beforeDelete();
        }
        protected override bool beforeInsert()
        {
            try
            {
                string sql = "SELECT MAX(Idx) FROM " + this.EnMap.PhysicsTable + " WHERE FrmID='" + this.FrmID + "'";
                this.Idx = DBAccess.RunSQLReturnValInt(sql, 0) + 1;
            }
            catch
            {
                this.Idx = 1;
            }
            return base.beforeInsert();
        }
        #endregion 方法.
    }
    /// <summary>
    /// 分组-集合
    /// </summary>
    public class GroupFields : EntitiesOID
    {


        #region 构造
        /// <summary>
        /// GroupFields
        /// </summary>
        public GroupFields()
        {
        }
        /// <summary>
        /// GroupFields
        /// </summary>
        /// <param name="enName">名称</param>
        public GroupFields(string enName)
        {
            int i = this.Retrieve(GroupFieldAttr.FrmID, enName, GroupFieldAttr.Idx);
            if (i == 0)
            {
                GroupField gf = new GroupField();
                gf.FrmID = enName;
                MapData md = new MapData();
                md.No = enName;
                if (md.RetrieveFromDBSources() == 0)
                    gf.Lab = "基础信息";
                else
                    gf.Lab = md.Name;
                gf.Idx = 0;
                gf.Insert();
                this.AddEntity(gf);
            }
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new GroupField();
            }
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="enName"></param>
        /// <returns></returns>
        public int RetrieveFieldGroup(string enName)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(GroupFieldAttr.FrmID, enName);
            qo.addAnd();
            qo.AddWhereIsNull(GroupFieldAttr.CtrlID);
            //qo.AddWhereLen(GroupFieldAttr.CtrlID, " = ", 0, BP.Difference.SystemConfig.AppCenterDBType);
            int num = qo.DoQuery();

            if (num == 0)
            {
                GroupField gf = new GroupField();
                gf.FrmID = enName;
                MapData md = new MapData();
                md.No = enName;
                if (md.RetrieveFromDBSources() == 0)
                    gf.Lab = "基础信息";
                else
                    gf.Lab = md.Name;
                gf.Idx = 0;
                gf.Insert();
                this.AddEntity(gf);
                return 1;
            }
            return num;
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<GroupField> ToJavaList()
        {
            return (System.Collections.Generic.IList<GroupField>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<GroupField> Tolist()
        {
            System.Collections.Generic.List<GroupField> list = new System.Collections.Generic.List<GroupField>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((GroupField)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

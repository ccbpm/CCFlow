using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Web;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 枚举字段
    /// </summary>
    public class MapAttrEnum : EntityMyPK
    {
        #region 文本字段参数属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.KeyOfEn, value);
            }
        }
        /// <summary>
        /// 绑定的枚举ID
        /// </summary>
        public string UIBindKey
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.UIBindKey);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIBindKey, value);
            }
        }
        /// <summary>
        /// 控件类型
        /// </summary>
        public UIContralType UIContralType
        {
            get
            {
                return (UIContralType)this.GetValIntByKey(MapAttrAttr.UIContralType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIContralType, (int)value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 控制权限
        /// </summary>
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
        /// 枚举字段
        /// </summary>
        public MapAttrEnum()
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

                Map map = new Map("Sys_MapAttr", "枚举字段");
                map.IndexField = MapAttrAttr.FK_MapData;


                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20,true);


                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);
                map.AddTBString(MapAttrAttr.UIBindKey, null, "枚举ID", true, true, 0, 100, 20);

                string sql = "";
                switch (SystemConfig.AppCenterDBType)
                {
                    case DBType.MSSQL:
                    case DBType.MySQL:
                        sql = "SELECT -1 AS No, '-无(不选择)-' as Name ";
                        break;
                    case DBType.Oracle:
                        sql = "SELECT -1 AS No, '-无(不选择)-' as Name FROM DUAL ";
                        break;

                    case DBType.PostgreSQL:
                    default:
                        sql = "SELECT -1 AS No, '-无(不选择)-' as Name FROM Port_Emp WHERE 1=2 ";
                        break;
                }
                sql += " union ";

                if (SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    sql += "SELECT  IntKey as No, Lab as Name FROM Sys_Enum WHERE EnumKey='@UIBindKey'";
                else
                    sql += "SELECT  IntKey as No, Lab as Name FROM Sys_Enum WHERE EnumKey='@UIBindKey' ";

                //默认值.
                map.AddDDLSQL(MapAttrAttr.DefVal, "0", "默认值(选中)", sql, true);

                //map.AddTBString(MapAttrAttr.DefVal, "0", "默认值", true, true, 0, 3000, 20);

                map.AddDDLSysEnum(MapAttrAttr.UIContralType, 0, "控件类型", true, true, "EnumUIContralType",
                 "@1=下拉框@2=复选框@3=单选按钮");

                map.AddDDLSysEnum("RBShowModel", 0, "单选按钮的展现方式", true, true, "RBShowModel",
            "@0=竖向@3=横向");

                //map.AddDDLSysEnum(MapAttrAttr.LGType, 0, "逻辑类型", true, false, MapAttrAttr.LGType, 
                // "@0=普通@1=枚举@2=外键@3=打开系统页面");



                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见?", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑?", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);

           
                #endregion 基本信息.

                #region 傻瓜表单。
                //单元格数量 2013-07-24 增加。
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrDT",
                   "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false); //@李国文

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, true);
                //CCS样式
                map.AddDDLSQL(MapAttrAttr.CSSCtrl, "0", "自定义样式", MapAttrString.SQLOfCSSAttr, true);
                #endregion 傻瓜表单。

                #region 执行的方法.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "级联下拉框";
                rm.ClassMethodName = this.ToString() + ".DoActiveDDL()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "填充其他控件";
                rm.ClassMethodName = this.ToString() + ".DoDDLFullCtrl2019()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "编辑枚举值";
                rm.ClassMethodName = this.ToString() + ".DoSysEnum()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "事件绑函数";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "选项联动控件";
                rm.ClassMethodName = this.ToString() + ".DoRadioBtns()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                //     rm.GroupName = "高级设置";
                map.AddRefMethod(rm);

                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 处理业务逻辑.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdateInsertAction()
        {

            MapAttr attr = new MapAttr();
            attr.MyPK = this.MyPK;
            attr.RetrieveFromDBSources();

            //单选按钮的展现方式.
            attr.RBShowModel = this.GetValIntByKey("RBShowModel");

            if (this.UIContralType == UIContralType.DDL
                || this.UIContralType == UIContralType.RadioBtn)
                attr.MyDataType = DataType.AppInt;
            else
                attr.MyDataType = DataType.AppString;

            //执行保存.
            attr.Update();

            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.MyPK = this.MyPK;
            mapAttr.RetrieveFromDBSources();
           
            if (this.UIContralType == UIContralType.CheckBok)
            {
                mapAttr.MyDataType = DataType.AppString;
                MapData mapData = new MapData(this.FK_MapData);
                GEEntity en = new GEEntity(this.FK_MapData);

                if(DBAccess.IsExitsTableCol(en.EnMap.PhysicsTable, this.KeyOfEn) == true)
                {
                    switch (SystemConfig.AppCenterDBType)
                    {
                        case DBType.MSSQL:
                            //先检查是否存在约束
                            string sqlYueShu = "SELECT b.name, a.name FName from sysobjects b join syscolumns a on b.id = a.cdefault where a.id = object_id('" + en.EnMap.PhysicsTable + "') ";
                            DataTable dtYueShu = DBAccess.RunSQLReturnTable(sqlYueShu);
                            foreach (DataRow dr in dtYueShu.Rows)
                            {
                                if (dr["FName"].ToString().ToLower() == this.KeyOfEn.ToLower())
                                {
                                    DBAccess.RunSQL("ALTER TABLE " + en.EnMap.PhysicsTable + " drop constraint " + dr[0].ToString());
                                    break;
                                }

                            }
                            this.RunSQL("alter table  " + en.EnMap.PhysicsTable + " ALTER column " + this.KeyOfEn + " VARCHAR(20)");
                            break;
                        case DBType.Oracle:
                            //判断数据库当前字段的类型
                            string sql = "SELECT DATA_TYPE FROM ALL_TAB_COLUMNS WHERE upper(TABLE_NAME)='" + en.EnMap.PhysicsTable.ToUpper() + "' AND UPPER(COLUMN_NAME)='" + this.KeyOfEn.ToUpper() + "' ";
                            string val = DBAccess.RunSQLReturnString(sql);
                            if (val == null)
                                Log.DefaultLogWriteLineError("@没有检测到字段eunm" + this.KeyOfEn);
                            if (val.IndexOf("NUMBER") != -1)
                            {
                                this.RunSQL("ALTER TABLE " + en.EnMap.PhysicsTable + " RENAME COLUMN " + this.KeyOfEn + " TO " + this.KeyOfEn + "_tmp");

                                /*增加一个和原字段名同名的字段name*/
                                this.RunSQL("ALTER TABLE " + en.EnMap.PhysicsTable + " ADD " + this.KeyOfEn + " varchar2(20)");

                                /*将原字段name_tmp数据更新到增加的字段name*/
                                this.RunSQL("UPDATE " + en.EnMap.PhysicsTable + " SET " + this.KeyOfEn + "= trim(" + this.KeyOfEn + "_tmp)");

                                /*更新完，删除原字段name_tmp*/
                                this.RunSQL("ALTER TABLE " + en.EnMap.PhysicsTable + " DROP COLUMN " + this.KeyOfEn + "_tmp");

                                //this.RunSQL(sql);
                            }
                            break;
                        case DBType.MySQL:
                            this.RunSQL("alter table  " + en.EnMap.PhysicsTable + " modify " + this.KeyOfEn + " NVARCHAR(20)");
                            break;
                        case DBType.PostgreSQL:
                            this.RunSQL("ALTER TABLE " + en.EnMap.PhysicsTable + " ALTER column " + this.KeyOfEn + " type character varying(20)");
                            break;
                    }
                }
            }
            mapAttr.Update();
          
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }
        #endregion

        protected override void afterDelete()
        {
            //删除可能存在的数据.
            DBAccess.RunSQL("DELETE FROM Sys_FrmRB WHERE KeyOfEn='" + this.KeyOfEn + "' AND FK_MapData='" + this.FK_MapData + "'");
            //删除相对应的rpt表中的字段
            if (this.FK_MapData.Contains("ND") == true)
            {
                string fk_mapData = this.FK_MapData.Substring(0, this.FK_MapData.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND KeyOfEn='" + this.KeyOfEn + "'";
                DBAccess.RunSQL(sql);
            }
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
        }

        #region 基本功能.
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        #endregion

        #region 方法执行.
        /// <summary>
        /// 编辑枚举值
        /// </summary>
        /// <returns></returns>
        public string DoSysEnum()
        {
            if(SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                return "../../Admin/CCFormDesigner/DialogCtr/EnumerationNewSAAS.htm?DoType=FrmEnumeration_SaveEnum&EnumKey=" + this.UIBindKey+"&OrgNo="+BP.Web.WebUser.OrgNo;
            else
                return "../../Admin/CCFormDesigner/DialogCtr/EnumerationNew.htm?DoType=FrmEnumeration_SaveEnum&EnumKey=" + this.UIBindKey;

        }

        public string DoDDLFullCtrl2019()
        {
            return "../../Admin/FoolFormDesigner/MapExt/DDLFullCtrl2019.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 设置自动填充
        /// </summary>
        /// <returns></returns>
        public string DoAutoFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFullDLL.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public string DoRadioBtns()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RadioBtns.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoActiveDDL()
        {
            return "../../Admin/FoolFormDesigner/MapExt/ActiveDDL.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }

        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrEnums : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrEnums()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrEnum();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrEnum> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrEnum>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrEnum> Tolist()
        {
            System.Collections.Generic.List<MapAttrEnum> list = new System.Collections.Generic.List<MapAttrEnum>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrEnum)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

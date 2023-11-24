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
    /// 数据视图
    /// </summary>
    public class MapAttrDataView : EntityMyPK
    {
        #region 文本字段参数属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
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
        /// 绑定的ID
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
                if (BP.Web.WebUser.IsAdmin == true)
                {
                    uac.IsInsert = false;
                    uac.IsUpdate = true;
                    uac.IsDelete = true;
                }
                return uac;
            }
        }
        /// <summary>
        /// 数据视图
        /// </summary>
        public MapAttrDataView()
        {
        }
        public MapAttrDataView(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
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

                Map map = new Map("Sys_MapAttr", "数据视图");
                map.IndexField = MapAttrAttr.FK_MapData;

                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);
                map.AddTBString(MapAttrAttr.Name, null, "名称", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "ID", true, true, 1, 200, 20);

                //默认值.
                map.AddTBStringDoc(MapAttrAttr.DefVal, null, "SQL语句", true, false,true);
                map.SetHelperAlert(MapAttrAttr.DefVal, "设置查询语句比如:SELECT No,Name,Addr,Email FROM WF_Emp WHERE FK_Dept=@WebUser.FK_Dept ");

                map.AddTBString(MapAttrAttr.UIBindKey, null, "中文对应", true, false, 0, 100, 20, true);
                map.SetHelperAlert(MapAttrAttr.UIBindKey, "@No=编号@Name=名称@Addr=地址@Email=邮件");

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);

              //  map.AddBoolean(MapAttrAttr.UIVisible, true, "可见", true, true);
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false); 
                #endregion 基本信息.

                //#region 傻瓜表单。
                ////map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrDT",
                ////   "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");
                //////文本占单元格数量
                ////map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                ////    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");
                //////文本跨行
                ////map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                ////显示的分组.
                //#endregion 傻瓜表单。

                #region 执行的方法.
                RefMethod rm = new RefMethod();
                //rm = new RefMethod();
                //rm.Title = "设置联动";
                //rm.ClassMethodName = this.ToString() + ".DoActiveDDL()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置显示过滤";
                //rm.ClassMethodName = this.ToString() + ".DoAutoFullDLL()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);


                //rm = new RefMethod();
                //rm.Title = "填充其他控件";
                //rm.ClassMethodName = this.ToString() + ".DoDDLFullCtrl2019()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "外键表属性";
                //rm.ClassMethodName = this.ToString() + ".DoSFTable()";
                //rm.RefMethodType = RefMethodType.LinkeWinOpen;
                //rm.GroupName = "高级";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "高级JS设置";
                //rm.ClassMethodName = this.ToString() + ".DoRadioBtns()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.GroupName = "高级";
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "事件绑函数";
                //rm.ClassMethodName = this.ToString() + ".BindFunction()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 方法执行.
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FrmID + "&KeyOfEn=" + this.KeyOfEn+"&T="+DateTime.Now.ToString();
        }
        /// <summary>
        /// 外键表属性
        /// </summary>
        /// <returns></returns>
        public string DoSFTable()
        {
            return "../../Admin/FoolFormDesigner/GuideSFTableAttr.htm?FK_SFTable=" + this.UIBindKey;
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public string DoRadioBtns()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RadioBtns.htm?FK_MapData=" + this.FrmID + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 设置填充其他下拉框
        /// </summary>
        /// <returns></returns>
        public string DoDDLFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/MapExt/DDLFullCtrl.htm?FK_MapData=" + this.FrmID + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=DDLFullCtrl_" + this.FrmID + "_" + this.KeyOfEn;
        }
        public string DoDDLFullCtrl2019()
        {
            return "../../Admin/FoolFormDesigner/MapExt/DDLFullCtrl2019.htm?FK_MapData=" + this.FrmID + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 设置下拉框显示过滤
        /// </summary>
        /// <returns></returns>
        public string DoAutoFullDLL()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFullDLL.htm?FK_MapData=" + this.FrmID + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoActiveDDL()
        {
            return "../../Admin/FoolFormDesigner/MapExt/ActiveDDL.htm?FK_MapData=" + this.FrmID + "&KeyOfEn=" + this.KeyOfEn;
        }
        #endregion 方法执行.

        #region 重写的方法.
        /// <summary>
        /// 删除，把影子字段也要删除.
        /// </summary>
        protected override void afterDelete()
        {
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.FrmID + "_" + this.KeyOfEn + "T");
            attr.Delete();

            //删除相对应的rpt表中的字段
            if (this.FrmID.Contains("ND") == true)
            {
                string fk_mapData = this.FrmID.Substring(0, this.FrmID.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND( KeyOfEn='" + this.KeyOfEn + "T' OR KeyOfEn='" + this.KeyOfEn + "')";
                DBAccess.RunSQL(sql);
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);
            base.afterDelete();
        }

        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.setMyPK(this.MyPK);
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);

            base.afterInsertUpdateAction();
        }
        #endregion 重写的方法.

    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrDataViews : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrDataViews()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrDataView();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrDataView> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrDataView>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrDataView> Tolist()
        {
            System.Collections.Generic.List<MapAttrDataView> list = new System.Collections.Generic.List<MapAttrDataView>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrDataView)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

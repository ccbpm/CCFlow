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
    /// 公文正文组件
    /// </summary>
    public class MapAttrGovDocFile : EntityMyPK
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
        /// 实体属性
        /// </summary>
        public MapAttrGovDocFile()
        {
        }
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttrGovDocFile(string myPK)
        {
            this.setMyPK(myPK);
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

                Map map = new Map("Sys_MapAttr", "公文组件");

                #region 基本字段信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "中文名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                map.AddTBInt(MapAttrAttr.MinLen, 0, "最小长度", false, false);
                map.AddTBInt(MapAttrAttr.MaxLen, 50, "最大长度", false, false);

                map.AddDDLSysEnum(MapAttrAttr.UIIsEnable, 0, "启用类型", true, true, "CtrlEnableType", "@0=禁用(隐藏)@1=启用@2=只读");
                string cfgStr = "@0=RTF模板@1=HTML模板@2=Weboffice组件@3=WPS组件@4=金格组件";
                map.AddDDLStringEnum(MapAttrAttr.Tag, "0", "组件类型", cfgStr, true);
                //map.AddDDLSysEnum(MapAttrAttr.Tag, 0, "组件类型", true, true, "GovDocType", "@0=RTF模板@1=HTML模板@2=Weboffice组件@3=WPS组件@4=金格组件");

                //map.AddDDLSQL(MapAttrAttr.CSSCtrl, "0", "自定义样式", MapAttrString.SQLOfCSSAttr, true);
                #endregion 基本字段信息.

                #region 傻瓜表单
                //单元格数量 2013-07-24 增加
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "TextBox单元格数量", true, true, "ColSpanAttrDT",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨5个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.ColSpan, "对于傻瓜表单有效: 标识该字段TextBox横跨的宽度,占的单元格数量.");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "Label单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨6个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.LabelColSpan, "对于傻瓜表单有效: 标识该字段Lable，标签横跨的宽度,占的单元格数量.");

                //文本跨行.
                // map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                // map.SetHelperAlert(MapAttrAttr.RowSpan, "对于傻瓜表单有效: 占的单元格row的数量.");

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                #endregion 傻瓜表单

                map.AddMyFile("模板", null, BP.Difference.SystemConfig.PathOfDataUser + "\\FrmVSTOTemplate");


                RefMethod rm = new RefMethod();
                rm.Title = "rtf模板打印";
                rm.ClassMethodName = this.ToString() + ".DoBill";
                rm.Icon = "../../WF/Img/FileType/doc.gif";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-printer";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "帮助";
                rm.ClassMethodName = this.ToString() + ".DoHelp";
                rm.Icon = "../../WF/Img/FileType/help.gif";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "icon-printer";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoHelp()
        {
            return "https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=5184749&doc_id=31094";
        }
        /// <summary>
        /// 单据打印
        /// </summary>
        /// <returns></returns>
        public string DoBill()
        {
            return "../../Admin/FoolFormDesigner/PrintTemplate/Default.htm?FK_MapData=" + this.FrmID + "&FrmID="+this.FrmID+"&KeyOfEn=" + this.KeyOfEn;
        }

        protected override bool beforeUpdateInsertAction()
        {

            this.SetValByKey(MapAttrAttr.UIContralType, (int)UIContralType.GovDocFile);
            if (this.GetValStrByKey("GroupID").Equals("无") == true)
                this.SetValByKey("GroupID", "0");

            return base.beforeUpdateInsertAction();
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected override void afterDelete()
        {
            //删除相对应的rpt表中的字段
            if (this.FrmID.Contains("ND") == true)
            {
                string fk_mapData = this.FrmID.Substring(0, this.FrmID.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND( KeyOfEn='" + this.KeyOfEn + "')";
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
        #endregion
    }
    /// <summary>
    /// 公文正文组件s
    /// </summary>
    public class MapAttrGovDocFiles : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 公文正文组件s
        /// </summary>
        public MapAttrGovDocFiles()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrGovDocFile();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrGovDocFile> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrGovDocFile>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrGovDocFile> Tolist()
        {
            System.Collections.Generic.List<MapAttrGovDocFile> list = new System.Collections.Generic.List<MapAttrGovDocFile>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrGovDocFile)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

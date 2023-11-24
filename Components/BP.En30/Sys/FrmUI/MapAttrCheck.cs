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
    /// 实体属性
    /// </summary>
    public class MapAttrCheck : EntityMyPK
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
        public string getFrmID()
        {
            return this.GetValStringByKey(MapAttrAttr.FK_MapData);
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
        public MapAttrCheck()
        {
        }
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttrCheck(string myPK)
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

                Map map = new Map("Sys_MapAttr", "签批字段");

                #region 基本字段信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);
                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20, true);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20, true);

                map.AddTBInt(MapAttrAttr.MinLen, 0, "最小长度", true, false);
                map.AddTBInt(MapAttrAttr.MaxLen, 50, "最大长度", true, false);
                map.SetHelperAlert(MapAttrAttr.MaxLen, "定义该字段的字节长度.");


                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.SetHelperAlert(MapAttrAttr.UIWidth, "对自由表单,从表有效,显示文本框的宽度.");

                //map.AddTBInt(MapAttrAttr.UIContralType, 0, "控件", false, false);

                /**map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);
                map.SetHelperAlert(MapAttrAttr.UIVisible, "对于不可见的字段可以在隐藏功能的栏目里找到这些字段进行编辑或者删除.");

                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.SetHelperAlert(MapAttrAttr.UIIsEnable, "不可编辑,让该字段设置为只读.");

                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                map.AddBoolean(MapAttrAttr.IsRichText, false, "是否富文本？", true, true);
                map.SetHelperAlert(MapAttrAttr.IsRichText, "以html编辑器呈现或者编写字段.");
                map.AddBoolean(MapAttrAttr.IsSecret, false, "是否保密？", true, true);

                map.AddBoolean(MapAttrAttr.IsSupperText, false, "是否大块文本？(是否该字段存放的超长字节字段)", true, true, true);
                map.SetHelperAlert(MapAttrAttr.IsSupperText, "大块文本存储字节比较长，超过4000个字符.");

                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", true, false, 0, 400, 20, true);
                map.SetHelperAlert(MapAttrAttr.Tip, "在文本框输入的时候显示在文本框背景的提示文字,也就是文本框的 placeholder 的值.");
                //CCS样式
                */
                map.AddDDLSQL(MapAttrAttr.CSSCtrl, "0", "自定义样式", MapAttrString.SQLOfCSSAttr, true);
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


                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);


                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false);
                map.SetHelperAlert(MapAttrAttr.Idx, "对傻瓜表单有效:用于调整字段在同一个分组中的顺序.");

                #endregion 傻瓜表单

                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "字段重命名";
                rm.ClassMethodName = this.ToString() + ".DoRenameField()";
                rm.HisAttrs.AddTBString("key1", "@KeyOfEn", "字段重命名为?", true, false, 0, 100, 100);
                rm.RefMethodType = RefMethodType.Func;
                rm.Warning = "如果是节点表单，系统就会把该流程上的所有同名的字段都会重命名，包括NDxxxRpt表单。";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "转化为文本框组件";
                rm.ClassMethodName = this.ToString() + ".DoSetTextBox()";
                rm.Warning = "您确定要转化为文本框组件吗？";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "字段名链接";
                rm.ClassMethodName = this.ToString() + ".DoFieldNameLink()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-settings";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "视频教程";
                rm.ClassMethodName = this.ToString() + ".DoVideo()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        public string DoFieldNameLink()
        {
            return "../../Admin/FoolFormDesigner/MapExt/FieldNameLink.htm?FK_MapData=" + this.getFrmID() + "&KeyOfEn=" + this.KeyOfEn;
        }

        public string DoVideo()
        {
            return "https://www.bilibili.com/video/BV1EK411T7U4";
        }
        /// <summary>
        /// 设置签批组件
        /// </summary>
        /// <returns>执行结果</returns>
        public string DoSetTextBox()
        {
            MapAttrString en = new MapAttrString(this.MyPK);
            en.UIContralType = UIContralType.TB;
            en.setUIIsEnable(true);
            en.setUIVisible(true);
            en.Update();

            return "设置成功,当前签批组件已经是文本框了,请关闭掉当前的窗口.";
        }

        /// <summary>
        /// 字段分组查询语句
        /// </summary>
        public static string SQLOfGroupAttr
        {
            get
            {
                return "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE FrmID='@FK_MapData'  AND (CtrlType IS NULL OR CtrlType='')  ";
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected override void afterDelete()
        {
            //删除相对应的rpt表中的字段
            if (this.getFrmID().Contains("ND") == true)
            {
                string fk_mapData = this.getFrmID().Substring(0, this.getFrmID().Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND( KeyOfEn='" + this.KeyOfEn + "T' OR KeyOfEn='" + this.KeyOfEn + "')";
                DBAccess.RunSQL(sql);
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.getFrmID());

            base.afterDelete();
        }


        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.setMyPK(this.MyPK);
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.getFrmID());

            base.afterInsertUpdateAction();
        }
        #endregion

        public string DoRenameField(string newField)
        {
            MapAttrString en = new MapAttrString(this.MyPK);
            return en.DoRenameField(newField);
        }

        #region 重载.
        protected override bool beforeUpdateInsertAction()
        {
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.MyPK);
            attr.RetrieveFromDBSources();

            //强制设置为签批组件.
            this.SetValByKey(MapAttrAttr.UIContralType, (int)UIContralType.SignCheck);

            //默认值.
            string defval = this.GetValStrByKey("ExtDefVal");
            if (defval.Equals("") == true || defval.Equals("0") == true)
            {
                string defVal = this.GetValStrByKey("DefVal");
                if (defval.Contains("@") == true)
                    this.SetValByKey("DefVal", "");
            }
            else
            {
                this.SetValByKey("DefVal", this.GetValByKey("ExtDefVal"));
            }

            //执行保存.
            attr.Save();

            if (this.GetValStrByKey("GroupID").Equals("无") == true)
                this.SetValByKey("GroupID", "0");

            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrChecks : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrChecks()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrCheck();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrCheck> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrCheck>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrCheck> Tolist()
        {
            System.Collections.Generic.List<MapAttrCheck> list = new System.Collections.Generic.List<MapAttrCheck>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrCheck)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

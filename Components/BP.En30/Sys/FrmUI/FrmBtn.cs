using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys.FrmUI
{
    public class FrmBtnAttr : MapAttrAttr
    {
        /// <summary>
        /// Text
        /// </summary>
        public const string Lab = "Lab";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// X
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string BtnType = "BtnType";
        /// <summary>
        /// 颜色
        /// </summary>
        public const string IsView = "IsView";
        /// <summary>
        /// 风格
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 字体风格
        /// </summary>
        public const string EventContext = "EventContext";
        /// <summary>
        /// 字体
        /// </summary>
        public const string UACContext = "UACContext";
        /// <summary>
        /// 事件类型
        /// </summary>
        public const string EventType = "EventType";
        /// <summary>
        /// 控制类型
        /// </summary>
        public const string UAC = "UAC";
        /// <summary>
        /// MsgOK
        /// </summary>
        public const string MsgOK = "MsgOK";
        /// <summary>
        /// MsgErr
        /// </summary>
        public const string MsgErr = "MsgErr";
        /// <summary>
        /// 按钮ID
        /// </summary>
        public const string BtnID = "BtnID";
        /// <summary>
        /// 分组
        /// </summary>
        public const string GroupID = "GroupID";
    }
    /// <summary>
    /// 按钮
    /// </summary>
    public class FrmBtn : EntityMyPK
    {
        #region 属性
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(FrmBtnAttr.FK_MapData);
            }
        }
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
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 按钮
        /// </summary>
        public FrmBtn()
        {
        }
        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="mypk"></param>
        public FrmBtn(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("Sys_MapAttr", "按钮");
                map.IndexField = MapAttrAttr.FK_MapData;

                map.AddMyPK(false);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", true, true, 0, 100, 20);
                map.AddTBString(MapAttrAttr.Name, null, "按钮标签(文字)", true, false, 0, 50, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "按钮ID", true, true, 0, 50, 20);

                map.AddDDLSysEnum(MapAttrAttr.UIIsEnable, 0, "事件类型", true, true, FrmBtnAttr.EventType,
                "@0=禁用@1=执行URL@2=执行CCFromRef.js");
                //  map.AddBoolean(FrmBtnAttr.UIIsEnable, true, "是否可用", true, true);

                #region 傻瓜表单的属性.
                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                //单元格数量 2013-07-24 增加.
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "TextBox单元格数量", true, true, "ColSpanAttrDT",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨5个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.ColSpan, "对于傻瓜表单有效: 标识该字段TextBox横跨的宽度,占的单元格数量.");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "Label单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨6个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.LabelColSpan, "对于傻瓜表单有效: 标识该字段Lable，标签横跨的宽度,占的单元格数量.");

                //map.AddTBString(FrmBtnAttr.UACContext, null, "控制内容", false, false, 0, 3900, 20);
                //map.AddDDLSysEnum(FrmBtnAttr.EventType, 0, "事件类型", true, true, FrmBtnAttr.EventType,
                //"@0=禁用@1=执行URL@2=执行CCFromRef.js");
                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "所在分组",
                    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE FrmID='@FK_MapData'", true);
                #endregion 傻瓜表单的属性.


                map.AddTBStringDoc(MapAttrAttr.Tag, null, "事件内容", true, false);
                map.SetHelperAlert(MapAttrAttr.Tag, "可以写JS，js可以调用在DataUser下JSLab下xxx_Self.js 函数.");

                //map.AddTBString(FrmBtnAttr.MsgOK, null, "运行成功提示", true, false, 0, 500, 20);
                //map.AddTBString(FrmBtnAttr.MsgErr, null, "运行失败提示", true, false, 0, 500, 20);
                //map.AddTBString(FrmBtnAttr.BtnID, null, "按钮ID", true, false, 0, 128, 20);
                RefMethod rm = new RefMethod();
                rm = new RefMethod();
                rm.Title = "填充其他控件";
                rm.ClassMethodName = this.ToString() + ".DoFullCtrl()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }

        public string DoFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/FullData/Main.htm?FK_MapData=" + this.FrmID + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefPK=" + this.MyPK+"&UIControlType=18";
        }

        protected override void afterInsertUpdateAction()
        {
            //在属性实体集合插入前，clear 父实体的缓存.
            BP.Sys.Base.Glo.ClearMapDataAutoNum(this.FrmID);


            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);
            base.afterDelete();
        }
        #endregion
    }
    /// <summary>
    /// 按钮s
    /// </summary>
    public class FrmBtns : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 按钮s
        /// </summary>
        public FrmBtns()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmBtn();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmBtn> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmBtn>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmBtn> Tolist()
        {
            System.Collections.Generic.List<FrmBtn> list = new System.Collections.Generic.List<FrmBtn>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmBtn)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

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
    /// 评论(抄送)组件
    /// </summary>
    public class MapAttrFlowBBS : EntityMyPK
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
        public MapAttrFlowBBS()
        {
        }
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttrFlowBBS(string myPK)
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

                Map map = new Map("Sys_MapAttr", "评论(抄送)组件");
                

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

                map.AddTBInt(MapAttrAttr.UIContralType, 0, "控件", false, false);

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
                map.AddDDLSQL(MapAttrAttr.GroupID,0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);

                
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false);
                map.SetHelperAlert(MapAttrAttr.Idx, "对傻瓜表单有效:用于调整字段在同一个分组中的顺序.");

                #endregion 傻瓜表单

                this._enMap = map;
                return this._enMap;
            }
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

        #region 重载.
        protected override bool beforeUpdateInsertAction()
        {
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.MyPK);
            attr.RetrieveFromDBSources();

            //强制设置为BBS.
            this.SetValByKey(MapAttrAttr.UIContralType, (int)UIContralType.FlowBBS);

            if (this.GetValStrByKey("GroupID").Equals("无") == true)
                this.SetValByKey("GroupID", "0");

            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 评论(抄送)组件s
    /// </summary>
    public class MapAttrFlowBBSs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 评论(抄送)组件s
        /// </summary>
        public MapAttrFlowBBSs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrFlowBBS();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrFlowBBS> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrFlowBBS>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrFlowBBS> Tolist()
        {
            System.Collections.Generic.List<MapAttrFlowBBS> list = new System.Collections.Generic.List<MapAttrFlowBBS>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrFlowBBS)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

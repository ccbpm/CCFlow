using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 按钮
    /// </summary>
    public class FrmBtn : EntityMyPK
    {
        #region 属性
        public string MsgOK
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.MsgOK);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.MsgOK, value);
            }
        }
        public string MsgErr
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.MsgErr);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.MsgErr, value);
            }
        }
        /// <summary>
        /// EventContext
        /// </summary>
        public string EventContext
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.EventContext).Replace("#", "@");
                //return this.GetValStringByKey(FrmBtnAttr.EventContext);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.EventContext, value);
            }
        }
        public string IsViewHtml
        {
            get
            {
                return PubClass.ToHtmlColor(this.IsView);
            }
        }
        /// <summary>
        /// IsView
        /// </summary>
        public string IsView
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.IsView);
            }
            set
            {
                switch (value)
                {
                    case "#FF000000":
                        this.SetValByKey(FrmBtnAttr.IsView, "Red");
                        return;
                    default:
                        break;
                }
                this.SetValByKey(FrmBtnAttr.IsView, value);
            }
        }
        public string UACContext
        {
            get
            {
                return this.GetValStringByKey(FrmBtnAttr.UACContext);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.UACContext, value);
            }
        }
        public bool UAC
        {
            get
            {
                return this.GetValBooleanByKey(FrmBtnAttr.UAC);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.UAC, value);
            }
        }
        /// <summary>
        /// IsEnable
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmBtnAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmBtnAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmBtnAttr.X);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.X, value);
            }
        }
        public BtnEventType HisBtnEventType
        {
            get
            {
                return (BtnEventType)this.GetValIntByKey(FrmBtnAttr.EventType);
            }
        }
        /// <summary>
        /// BtnType
        /// </summary>
        public int EventType
        {
            get
            {
                return this.GetValIntByKey(FrmBtnAttr.EventType);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.EventType, value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmBtnAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// Text
        /// </summary>
        public string Text
        {
            get
            {
                return this.GetValStrByKey(FrmBtnAttr.Text);
            }
            set
            {
                this.SetValByKey(FrmBtnAttr.Text, value);
            }
        }
        public string TextHtml
        {
            get
            {
                //if (this.EventType)
                //    return "<b>" + this.GetValStrByKey(FrmBtnAttr.Text).Replace("@","<br>") + "</b>";
                //else
                    return this.GetValStrByKey(FrmBtnAttr.Text).Replace("@", "<br>");
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

                Map map = new Map("Sys_FrmBtn", "按钮");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(FrmBtnAttr.FK_MapData, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmBtnAttr.Text, null, "标签", true, false, 0, 3900, 20);

                map.AddTBInt(FrmBtnAttr.IsView, 0, "是否可见", false, false);
                map.AddTBInt(FrmBtnAttr.IsEnable, 0, "是否起用", false, false);

                map.AddTBInt(FrmBtnAttr.UAC, 0, "控制类型", false, false);
                map.AddTBString(FrmBtnAttr.UACContext, null, "控制内容", true, false, 0, 3900, 20);

                map.AddTBInt(FrmBtnAttr.EventType, 0, "事件类型", false, false);
                map.AddTBString(FrmBtnAttr.EventContext, null, "事件内容", true, false, 0, 3900, 20);

                map.AddTBString(FrmBtnAttr.MsgOK, null, "运行成功提示", true, false, 0, 500, 20);
                map.AddTBString(FrmBtnAttr.MsgErr, null, "运行失败提示", true, false, 0, 500, 20);

                map.AddTBString(FrmBtnAttr.GUID, null, "GUID", true, false, 0, 128, 20);
             
                this._enMap = map;
                return this._enMap;
            }
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

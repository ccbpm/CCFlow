using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.IM
{
    /// <summary>
    /// 消息信息属性
    /// </summary>
    public class RecordMsgAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 发送主键(用于防止写入的消息重复)
        /// </summary>
        public const string SendUserID = "SendUserID";
        /// <summary>
        /// 发送人
        /// </summary>
        public const string SendID = "SendID";
        /// <summary>
        /// 发送时间.
        /// </summary>
        public const string MsgDateTime = "MsgDateTime";
        /// <summary>
        /// 消息内容
        /// </summary>
        public const string MsgContent = "MsgContent";
        /// <summary>
        /// 图片信息
        /// </summary>
        public const string ImageInfo = "ImageInfo";
        /// <summary>
        /// 字体
        /// </summary>
        public const string FontName = "FontName";
        /// <summary>
        /// 字体大小
        /// </summary>
        public const string FontSize = "FontSize";
        /// <summary>
        /// 是否是粗体
        /// </summary>
        public const string FontStrikeout = "FontStrikeout";
        /// <summary>
        /// 是否是下划线
        /// </summary>
        public const string FontUnderline = "FontUnderline";
        /// <summary>
        /// 字体
        /// </summary>
        public const string FontBold = "FontBold";
        /// <summary>
        /// 是否是斜体
        /// </summary>
        public const string FontItalic = "FontItalic";
        /// <summary>
        /// 颜色
        /// </summary>
        public const string FontColor = "FontColor";
        /// <summary>
        /// 类别
        /// </summary>
        public const string InfoClass = "InfoClass";
        /// <summary>
        /// 分组
        /// </summary>
        public const string GroupID = "GroupID";
        /// <summary>
        /// 短信息
        /// </summary>
        public const string SMSInfo = "SMSInfo";
    }
    /// <summary>
    /// 消息信息
    /// </summary>
    public class RecordMsg : EntityOID
    {
        #region 实现基本的方法
        #endregion

        #region 属性.
        /// <summary>
        /// 消息主键(为了防止消息插入重复)
        /// </summary>
        public string MyPK
        {
            get
            {
                return this.GetValStringByKey(RecordMsgAttr.SendUserID);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.SendUserID, value);
            }
        }
        /// <summary>
        /// 发送人(默认为SYSTEM)
        /// </summary>
        public string SendID
        {
            get
            {
                return this.GetValStringByKey(RecordMsgAttr.SendID);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.SendID, value);
            }
        }
        /// <summary>
        /// 消息发送时间
        /// </summary>
        public string MsgDateTime
        {
            get
            {
                return this.GetValStringByKey(RecordMsgAttr.MsgDateTime);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.MsgDateTime, value);
            }
        }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string MsgContent
        {
            get
            {
                return this.GetValStringByKey(RecordMsgAttr.MsgContent);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.MsgContent, value);
            }
        }
        /// <summary>
        /// 图片信息
        /// </summary>
        public string ImageInfo
        {
            get
            {
                return this.GetValStringByKey(RecordMsgAttr.ImageInfo);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.ImageInfo, value);
            }
        }
        /// <summary>
        /// 字体信息
        /// </summary>
        public string FontName
        {
            get
            {
                return this.GetValStringByKey(RecordMsgAttr.FontName);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.FontName, value);
            }
        }
        /// <summary>
        /// 字体大小
        /// </summary>
        public float FontSize
        {
            get
            {
                return this.GetValFloatByKey(RecordMsgAttr.FontSize);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.FontSize, value);
            }
        }
        /// <summary>
        /// 是否粗体
        /// </summary>
        public bool FontBold
        {
            get
            {
                return this.GetValBooleanByKey(RecordMsgAttr.FontBold);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.FontBold, value);
            }
        }
        public bool FontItalic
        {
            get
            {
                return this.GetValBooleanByKey(RecordMsgAttr.FontItalic);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.FontItalic, value);
            }
        }
        /// <summary>
        /// FontStrikeout
        /// </summary>
        public bool FontStrikeout
        {
            get
            {
                return this.GetValBooleanByKey(RecordMsgAttr.FontStrikeout);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.FontStrikeout, value);
            }
        }
        /// <summary>
        /// 是否下划线
        /// </summary>
        public bool FontUnderline
        {
            get
            {
                return this.GetValBooleanByKey(RecordMsgAttr.FontUnderline);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.FontUnderline, value);
            }
        }
        
        /// <summary>
        /// 字体颜色
        /// </summary>
        public string FontColor
        {
            get
            {
                return this.GetValStringByKey(RecordMsgAttr.FontColor);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.FontColor, value);
            }
        }
        public string SMSInfo
        {
            get
            {
                return this.GetValStringByKey(RecordMsgAttr.SMSInfo);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.SMSInfo, value);
            }
        }
        
        /// <summary>
        /// 类别
        /// </summary>
        public int InfoClass
        {
            get
            {
                return this.GetValIntByKey(RecordMsgAttr.InfoClass);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.InfoClass, value);
            }
        }
        /// <summary>
        /// 分组
        /// </summary>
        public int GroupID
        {
            get
            {
                return this.GetValIntByKey(RecordMsgAttr.GroupID);
            }
            set
            {
                this.SetValByKey(RecordMsgAttr.GroupID, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// 消息信息
        /// </summary> 
        public RecordMsg()
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

                Map map = new Map("CCIM_RecordMsg");
                map.EnDesc = "消息信息"; 
                map.EnType = EnType.Admin;

                map.AddTBIntPKOID(); //消息.

                map.AddTBString(RecordMsgAttr.SendUserID, null, "信息主键(防止插入重复)", true, false, 0, 50, 100);

                map.AddTBString(RecordMsgAttr.SendID, null, "发送人(SYSTEM)", true, false, 0, 100, 100);

                map.AddTBString(RecordMsgAttr.MsgDateTime, null, "发送时间", true, false, 0, 100, 100);
                map.AddTBString(RecordMsgAttr.MsgContent, null, "消息内容", true, false, 0, 4000, 4000);

                map.AddTBString(RecordMsgAttr.ImageInfo, null, "图片信息", true, false, 0, 2000, 2000);
                map.AddTBString(RecordMsgAttr.FontName, null, "字体名称", true, false, 0, 2000, 2000);
                map.AddTBFloat(RecordMsgAttr.FontSize, 5, "字体大小", true, false);

                map.AddTBInt(RecordMsgAttr.FontBold, 0, "字体粗细", true, false);
                map.AddTBInt(RecordMsgAttr.FontItalic, 0, "是否是斜体", true, false);
                map.AddTBInt(RecordMsgAttr.FontStrikeout, 0, "是否是斜体", true, false);
                map.AddTBInt(RecordMsgAttr.FontUnderline, 0, "是否是下划线", true, false);


                

                
                map.AddTBString(RecordMsgAttr.FontColor, null, "字体颜色", true, false, 0, 2000, 2000);

                map.AddTBInt(RecordMsgAttr.InfoClass, 0, "类别", true, false);
                map.AddTBInt(RecordMsgAttr.GroupID, 0, "分组", true, false);
                 
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 消息信息s
    /// </summary>
    public class RecordMsgs : EntitiesOID
    {
        /// <summary>
        /// 消息信息
        /// </summary>
        public RecordMsgs() 
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new RecordMsg();
            }
        }
    }
}

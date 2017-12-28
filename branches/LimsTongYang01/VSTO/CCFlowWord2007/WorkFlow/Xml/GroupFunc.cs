using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF
{
    public enum ShowType
    {
        Btn,
        sss,
    }
    /// <summary>
    /// 功能
    /// </summary>
    public class GroupFunc : BP.XML.XmlEnNoName
    {
        public ShowType HisShowType
        {
            get
            {
                return (ShowType)this.GetValIntByKey("ShowType");
            }
        }
        public string FK_Tab
        {
            get
            {
                return this.GetValStringByKey("FK_Tab");
            }
        }
        public override BP.XML.XmlEns GetNewEntities
        {
            get
            {
                return new GroupFuncs();
            }
        }
    }
    /// <summary>
    /// 功能集合
    /// </summary>
    public class GroupFuncs : BP.XML.XmlEns
    {
        public override BP.XML.XmlEn GetNewEntity
        {
            get { return new GroupFunc(); }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public override string File
        {
            get
            {
                return @"D:\ccflow\VSTO\CCFlowWord2007\Toolbar.xml";
                //return @"D:\ccflow\value-added\CCFlowWord2007\Toolbar.xml";
            }
        }
        /// <summary>
        /// 表
        /// </summary>
        public override string TableName
        {
            get { return "Group"; }
        }
    }
}

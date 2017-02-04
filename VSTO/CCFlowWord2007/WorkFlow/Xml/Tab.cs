using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 功能
    /// </summary>
    public class Tab : BP.XML.XmlEnNoName
    {
      
        public override BP.XML.XmlEns GetNewEntities
        {
            get
            {
                return new Tabs();
            }
        }
    }
    /// <summary>
    /// 功能集合
    /// </summary>
    public class Tabs : BP.XML.XmlEns
    {
        public override BP.XML.XmlEn GetNewEntity
        {
            get { return new Tab(); }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public override string File
        {
            get
            {
                return @"D:\ccflow\VSTO\CCFlowWord2007\Toolbar.xml";
               // return @"D:\ccflow\value-added\CCFlowWord2007\Toolbar.xml";
            }
        }
        /// <summary>
        /// 表
        /// </summary>
        public override string TableName
        {
            get { return "Tab"; }
        }
    }
}

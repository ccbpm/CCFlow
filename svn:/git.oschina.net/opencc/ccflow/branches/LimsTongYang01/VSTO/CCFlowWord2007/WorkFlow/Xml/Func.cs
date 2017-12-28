using System;
using System.Collections.Generic;
using System.Text;

namespace BP.WF
{
    /// <summary>
    /// 功能
    /// </summary>
    public class Func : BP.XML.XmlEnNoName
    {
        public Func()
        {

        }
        public string DoType
        {
            get
            {
                return this.GetValStringByKey("DoType");
            }
        }
        public bool IsIcon
        {
            get
            {
                if (this.GetValStringByKey("Icon") == "1")
                    return true;
                return false;
            }
        }
        public int Width
        {
            get
            {
                return this.GetValIntByKey("Width");
            }
        }
        public int Height
        {
            get
            {
                return this.GetValIntByKey("Height");
            }
        }
        public string FK_Group
        {
            get
            {
                return this.GetValStringByKey("FK_Group");
            }
        }
        public string Tag
        {
            get
            {
                return this.GetValStringByKey("Tag");
            }
        }
        public string CtlType
        {
            get
            {
                return this.GetValStringByKey("CtlType");
            }
        }
        public string Desc
        {
            get
            {
                return this.GetValStringByKey("Desc");
            }
        }
        public override BP.XML.XmlEns GetNewEntities
        {
            get {  return new Funcs(); }
        }
        

    }
    /// <summary>
    /// 功能集合
    /// </summary>
    public class Funcs : BP.XML.XmlEns
    {
        public override BP.XML.XmlEn GetNewEntity
        {
            get { return new Func(); }
        }
        /// <summary>
        /// 文件
        /// </summary>
        public override string File
        {
            get
            {
                return @"D:\ccflow\VSTO\CCFlowWord2007\Toolbar.xml";
            }
        }
        /// <summary>
        /// 表
        /// </summary>
        public override string TableName
        {
            get { return "Item"; }
        }
    }
}

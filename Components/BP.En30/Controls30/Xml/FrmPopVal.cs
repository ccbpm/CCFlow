using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Sys.XML;

namespace BP.Web.Controls
{
	/// <summary>
    /// 取值属性
	/// </summary>
    public class FrmPopValAttr
    {
        /// <summary>
        /// 编号  
        /// </summary>    
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 标签1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// 标签2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// 参数
        /// </summary>
        public const string AtPara = "AtPara";
        public const string H = "H";
        public const string W = "W";
    }
	/// <summary>
	/// 取值
	/// </summary>
    public class FrmPopVal : XmlEnNoName
    {
        #region 属性
        public string AtPara
        {
            get
            {
                return this.GetValStringByKey(FrmPopValAttr.AtPara);
            }
        }
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(FrmPopValAttr.Tag1);
            }
        }
        public string Tag2
        {
            get
            {
                return this.GetValStringByKey(FrmPopValAttr.Tag2);
            }
        }
        public string H
        {
            get
            {
                return this.GetValStringByKey(FrmPopValAttr.H);
            }
        }
        public string W
        {
            get
            {
                return this.GetValStringByKey(FrmPopValAttr.W);
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 取值
        /// </summary>
        public FrmPopVal()
        {

        }
        /// <summary>
        /// 取值
        /// </summary>
        /// <param name="no">编号</param>
        public FrmPopVal(string no)
        {
            this.RetrieveByPK(FrmPopValAttr.No, no);
        }
        
        /// <summary>
        /// 获取一个实例
        /// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new FrmPopVals();
            }
        }
        #endregion
    }
	/// <summary>
    /// 取值s
	/// </summary>
    public class FrmPopVals : XmlMenus
    {
        #region 构造
        /// <summary>
        /// 取值s
        /// </summary>
        public FrmPopVals()
        {
        }
        #endregion

        #region 重写基类属性或方法。
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new FrmPopVal();
            }
        }
        public override string File
        {
            get
            {
                return SystemConfig.PathOfDataUser + "\\Xml\\FrmPopVal.xml";
            }
        }
        /// <summary>
        /// 物理表名
        /// </summary>
        public override string TableName
        {
            get
            {
                return "Item";
                
            }
        }
        public override Entities RefEns
        {
            get
            {
                return null;
            }
        }
        #endregion
    }
}

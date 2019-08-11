using System; 
using System.Collections;
using BP.DA; 
using BP.Web.Controls;
using System.Reflection;
using BP.En;
namespace BP.En
{
    public enum MsgShowType
    {
        /// <summary>
        /// 本界面
        /// </summary>
        SelfAlert,
        /// <summary>
        /// 提示框
        /// </summary>
        SelfMsgWindows,
        /// <summary>
        /// 新窗口
        /// </summary>
        Blank
    }
	/// <summary>
	/// Method 的摘要说明
	/// </summary>
    abstract public class Method
    {
        /// <summary>
        /// 信息显示类型
        /// </summary>
        public MsgShowType HisMsgShowType = MsgShowType.Blank;

        #region Http
        public string Request(string key)
        {
            return BP.Sys.Glo.Request.QueryString[key];
        }
        /// <summary>
        /// 获取MyPK
        /// </summary>
        public string RequestRefMyPK
        {
            get
            {
                string s = Request("RefMyPK");
                if (s == null)
                    s = Request("RefPK");

                return s;
            }
        }
        public string RequestRefNo
        {
            get
            {
                return Request("RefNo");
            }
        }
        public int RequestRefOID
        {
            get
            {
                return int.Parse(Request("RefOID"));
            }
        }
        #endregion Http

        #region ROW
        /// <summary>
        /// 获取Key值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>结果</returns>
        public object GetValByKey(string key)
        {
            return this.Row.GetValByKey(key);
        }
        /// <summary>
        /// 获取str值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>结果</returns>
        public string GetValStrByKey(string key)
        {
            return this.GetValByKey(key).ToString();
        }
        /// <summary>
        /// 获取int值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>结果</returns>
        public int GetValIntByKey(string key)
        {
            return (int)this.GetValByKey(key);
        }

        /// <summary>
        /// 获取decimal值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>结果</returns>
        public decimal GetValDecimalByKey(string key)
        {
            return (decimal)this.GetValByKey(key);
        }
        /// <summary>
        /// 获取bool值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>结果</returns>
        public bool GetValBoolByKey(string key)
        {
            if (this.GetValIntByKey(key) == 1)
                return true;
            return false;
        }
        public void SetValByKey(string attrKey, int val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, Int64 val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, float val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, decimal val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        public void SetValByKey(string attrKey, object val)
        {
            this.Row.SetValByKey(attrKey, val);
        }
        /// <summary>
        /// 实体的 map 信息。	
        /// </summary>		
        //public abstract void EnMap();		
        private Row _row = null;
        public Row Row
        {
            get
            {
                if (this.HisAttrs == null)
                    return null;

                if (this._row == null)
                {
                    this._row = new Row();
                    this._row.LoadAttrs(this.HisAttrs);
                }

                return this._row;
            }
            set
            {
                this._row = value;
            }
        }
        #endregion

        /// <summary>
        /// 方法基类
        /// </summary>
        public Method()
        {

        }

        #region 属性
        /// <summary>
        /// 参数
        /// </summary>
        private Attrs _HisAttrs = null;
        public Attrs HisAttrs
        {
            get
            {
                if (_HisAttrs == null)
                    _HisAttrs = new Attrs();
                return _HisAttrs;
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title = null;
        public string Help = null;
        public string GroupName = "基本方法";
        /// <summary>
        /// 操作前提示信息
        /// </summary>
        public string Warning = null;
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon = null;
        public string GetIcon(string path)
        {
            if (this.Icon == null)
            {
                return "<img src='/WF/Img/Btn/Do.gif'  border=0 />";
            }
            else
            {
                return Icon;
                //return "<img src='" + path + Icon + "'  border=0 />";
            }
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string ToolTip = null;
        /// <summary>
        /// 目标
        /// </summary>
        public string Target = "OpenWin";
        /// <summary>
        /// 高度
        /// </summary>
        public int Height = 600;
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width = 800;
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public abstract object Do();
        public abstract void Init();
        /// <summary>
        /// 权限管理
        /// </summary>
        public abstract bool IsCanDo
        {
            get;
        }
        /// <summary>
        /// 是否显示在功能列表里
        /// </summary>
        public bool IsVisable = true;
        #endregion
    }
}

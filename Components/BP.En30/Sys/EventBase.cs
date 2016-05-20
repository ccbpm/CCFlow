using System;
using System.Threading;
using System.Collections;
using System.Data;
using BP.DA;
using BP.DTS;
using BP.En;
using BP.Web.Controls;
using BP.Web;

namespace BP.Sys
{
    /// <summary>
    /// 事件基类
    /// </summary>
    abstract public class EventBase
    {
        #region 属性.
        public Entity HisEn = null;
        private Row _SysPara = null;
        /// <summary>
        /// 参数
        /// </summary>
        public Row SysPara
        {
            get
            {
                if (_SysPara == null)
                    _SysPara = new Row();
                return _SysPara;
            }
            set
            {
                _SysPara = value;
            }
        }
        /// <summary>
        /// 成功信息
        /// </summary>
        public string SucessInfo = null;
        private string _title = null;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                if (_title == null)
                    _title = "未命名";
                return _title;
            }
            set
            {
                _title = value;
            }
        }
        #endregion 属性.

        #region 系统参数
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_Mapdata
        {
            get
            {
                return this.GetValStr("FK_MapData");
            }
        }
        /// <summary>
        /// 事件类型
        /// </summary>
        public string EventType
        {
            get
            {
                return this.GetValStr("EventType");
            }
        }
      
        #endregion 

        #region 常用属性.
        /// <summary>
        /// 工作ID
        /// </summary>
        public int OID
        {
            get
            {
                return this.GetValInt("OID");
            }
        }
        /// <summary>
        /// 工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                if (this.OID == 0)
                    return this.GetValInt64("WorkID"); /*有可能开始节点的WorkID=0*/
                return this.OID;
            }
        }
        /// <summary>
        /// FID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64("FID");
            }
        }
        /// <summary>
        /// 流程编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStr("FK_Flow");
            }
        }
        /// <summary>
        /// 节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                try
                {
                    return this.GetValInt("FK_Node");
                }
                catch {
                    return 0;
                }
            }
        }
        /// <summary>
        /// 传过来的WorkIDs集合，子流程.
        /// </summary>
        public string WorkIDs
        {
            get
            {
                return this.GetValStr("WorkIDs");
            }
        }
        /// <summary>
        /// 编号集合s
        /// </summary>
        public string Nos
        {
            get
            {
                return this.GetValStr("Nos");
            }
        }
        #endregion 常用属性.

        #region 获取参数方法
        public DateTime GetValDateTime(string key)
        {
            string str= this.SysPara.GetValByKey(key).ToString();
            return DataType.ParseSysDateTime2DateTime(str);
        }
        /// <summary>
        /// 获取字符串参数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>如果为Nul,或者不存在就抛出异常</returns>
        public string GetValStr(string key)
        {
            return this.SysPara.GetValByKey(key).ToString();
        }
        /// <summary>
        /// 获取Int64的数值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>如果为Nul,或者不存在就抛出异常</returns>
        public Int64 GetValInt64(string key)
        {
            return Int64.Parse(this.GetValStr(key));
        }
        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>如果为Nul,或者不存在就抛出异常</returns>
        public int GetValInt(string key)
        {
            return int.Parse(this.GetValStr(key));
        }
        #endregion 获取参数方法

        /// <summary>
        /// 事件基类
        /// </summary>
        public EventBase()
        {
        }
        /// <summary>
        /// 执行事件
        /// 1，如果遇到错误就抛出异常信息，前台界面就会提示错误并不向下执行。
        /// 2，执行成功，把执行的结果赋给SucessInfo变量，如果不需要提示就赋值为空或者为null。
        /// 3，所有的参数都可以从  this.SysPara.GetValByKey中获取。
        /// </summary>
        abstract public void Do();
    }
}

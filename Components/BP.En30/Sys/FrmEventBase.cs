using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web.Controls;
using BP.Web;
using BP.Sys;

namespace BP.Sys
{
    /// <summary>
    /// 表单事件列表
    /// </summary>
    public enum FrmEvenList11
    {
        /// <summary>
        /// 创建OID
        /// </summary>
        CreateOID,
        /// <summary>
        /// 装载前
        /// </summary>
        FrmLoadBefore,
        /// <summary>
        /// 装载后
        /// </summary>
        FrmLoadAfter,
        /// <summary>
        /// 保存前
        /// </summary>
        SaveBefore,
        /// <summary>
        /// 保存后
        /// </summary>
        SaveAfter
    }
    /// <summary>
    /// 表单事件基类
    /// </summary>
    abstract public class FrmEventBase
    {
        #region 要求子类强制重写的属性.
        /// <summary>
        /// 表单编号
        /// 该参数用于说明要把此事件注册到那一个表单模版上.
        /// </summary>
        abstract public string FrmNo
        {
            get;
        }
        #endregion 要求子类重写的属性.
         
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
        /// 流程ID
        /// </summary>
        public Int64 FID
        {
            get
            {
                return this.GetValInt64("FID");
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
        /// <summary>
        ///  行数据
        /// </summary>
        public Row Row
        {
            get;
            set;
        }
        #endregion 常用属性.

        #region 数据字段的方法
        /// <summary>
        /// 时间参数
        /// </summary>
        /// <param name="key">时间字段</param>
        /// <returns>根据字段返回一个时间,如果为Null,或者不存在就抛出异常.</returns>
        public DateTime GetValDateTime(string key)
        {
            try
            {
                string str = this.Row.GetValByKey(key).ToString();
                return DataType.ParseSysDateTime2DateTime(str);
            }
            catch (Exception ex)
            {
                throw new Exception("@流程事件实体在获取参数期间出现错误，请确认字段(" + key + ")是否拼写正确,技术信息:" + ex.Message);
            }
        }
        /// <summary>
        /// 获取字符串参数
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>如果为Null,或者不存在就抛出异常</returns>
        public string GetValStr(string key)
        {
            try
            {
                return this.Row.GetValByKey(key).ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("@流程事件实体在获取参数期间出现错误，请确认字段(" + key + ")是否拼写正确,技术信息:" + ex.Message);
            }
        }
        /// <summary>
        /// 获取Int64的数值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>如果为Null,或者不存在就抛出异常</returns>
        public Int64 GetValInt64(string key)
        {
            return Int64.Parse(this.GetValStr(key));
        }
        /// <summary>
        /// 获取int的数值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>如果为Null,或者不存在就抛出异常</returns>
        public int GetValInt(string key)
        {
            return int.Parse(this.GetValStr(key));
        }
        /// <summary>
        /// 获取Boolen值
        /// </summary>
        /// <param name="key">字段</param>
        /// <returns>如果为Null,或者不存在就抛出异常</returns>
        public bool GetValBoolen(string key)
        {
            if (int.Parse(this.GetValStr(key)) == 0)
                return false;
            return true;
        }
        /// <summary>
        /// 获取decimal的数值
        /// </summary>
        /// <param name="key">字段</param>
        /// <returns>如果为Null,或者不存在就抛出异常</returns>
        public decimal GetValDecimal(string key)
        {
            return decimal.Parse(this.GetValStr(key));
        }
        #endregion 获取参数方法

        #region 构造方法
        /// <summary>
        /// 表单事件基类
        /// </summary>
        public FrmEventBase()
        {
        }
        #endregion 构造方法

        #region 节点表单事件
        public virtual string FrmLoadAfter()
        {
            return null;
        }
        public virtual string FrmLoadBefore()
        {
            return null;
        }
        #endregion
         
        #region 要求子类重写的方法(节点事件).
        /// <summary>
        /// 保存后
        /// </summary>
        public virtual string SaveAfter()
        {
            return null;
        }
        /// <summary>
        /// 保存前
        /// </summary>
        public virtual string SaveBefore()
        {
            return null;
        }
        /// <summary>
        /// 创建OID后的事件
        /// </summary>
        /// <returns></returns>
        public virtual string CreateOID()
        {
            return null;
        }
        #endregion 要求子类重写的方法(节点事件).

        #region 基类方法.
        /// <summary>
        /// 执行事件
        /// </summary>
        /// <param name="eventType">事件类型</param>
        /// <param name="en">实体参数</param>
        public string DoIt(string eventType, Entity en, Row row, string atPara)
        {
            this.Row = row;
            
            #region 处理参数.
            Row r = en.Row;
            try
            {
                //系统参数.
                this.Row.Add("FK_MapData", en.ClassID);
            }
            catch
            {
                this.Row["FK_MapData"] = en.ClassID;
            }

            if (atPara != null)
            {
                AtPara ap = new AtPara(atPara);
                foreach (string s in ap.HisHT.Keys)
                {
                    try
                    {
                        this.Row.Add(s, ap.GetValStrByKey(s));
                    }
                    catch
                    {
                        this.Row[s] = ap.GetValStrByKey(s);
                    }
                }
            }

            if (SystemConfig.IsBSsystem == true)
            {
                /*如果是bs系统, 就加入外部url的变量.*/
                foreach (string key in BP.Sys.Glo.Request.QueryString)
                {
                    string val = BP.Sys.Glo.Request.QueryString[key];
                    try
                    {
                        this.Row.Add(key, val);
                    }
                    catch
                    {
                        this.Row[key] = val;
                    }
                }
            }
            #endregion 处理参数.

            #region 执行事件.
            switch (eventType)
            {
                case EventListOfNode.CreateWorkID: // 节点表单事件。
                    return this.CreateOID();
                case EventListOfNode.FrmLoadAfter: // 节点表单事件。
                    return this.FrmLoadAfter();
                case EventListOfNode.FrmLoadBefore: // 节点表单事件。
                    return this.FrmLoadBefore();
                case EventListOfNode.SaveAfter: // 节点事件 保存后。
                    return this.SaveAfter();
                case EventListOfNode.SaveBefore: // 节点事件 - 保存前.。
                    return this.SaveBefore();
                default:
                    throw new Exception("@没有判断的事件类型:" + eventType);
                    break;
            }
            #endregion 执行事件.

            return null;
        }
        #endregion 基类方法.
    }
}

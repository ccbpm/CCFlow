using System;
using BP.Sys;

namespace BP.DA
{ 
	/// <summary>
	/// 参数
	/// </summary>
    public class Para
    {
        #region 属性
        public System.Data.DbType DAType = System.Data.DbType.String;
        public Oracle.ManagedDataAccess.Client.OracleDbType DATypeOfOra
        {
            get
            {
                switch (this.DAType)
                {
                    case System.Data.DbType.String:
                    case System.Data.DbType.Object:
                        if (this.IsBigText)
                            return Oracle.ManagedDataAccess.Client.OracleDbType.Clob;
                        else
                            return Oracle.ManagedDataAccess.Client.OracleDbType.Varchar2;
                    case System.Data.DbType.Int32:
                    case System.Data.DbType.Int16:
                        return Oracle.ManagedDataAccess.Client.OracleDbType.Int16;
                    case System.Data.DbType.Int64:
                        return Oracle.ManagedDataAccess.Client.OracleDbType.Int64;
                    case System.Data.DbType.Decimal:
                    case System.Data.DbType.Double:
                        return Oracle.ManagedDataAccess.Client.OracleDbType.Double;
                    default:
                        throw new Exception("没有涉及到的类型。typeof(obj)=" + this.DAType.ToString());
                }
            }
        }
        public string ParaName = null;
        public int Size = 10;
        public Object val;
        /// <summary>
        /// 是否是大文本?
        /// </summary>
        public bool IsBigText = false;
        #endregion

        /// <summary>
        /// 参数
        /// </summary>
        public Para()
        {
        }
        /// <summary>
        /// 构造参数
        /// </summary>
        /// <param name="_paraName">参数名称</param>
        /// <param name="_DAType">System.Data.SqlDbType</param>
        /// <param name="_val">值</param>
        public Para(string _paraName, System.Data.DbType _DAType, Object _val)
        {
            this.ParaName = _paraName;
            this.DAType = _DAType;
            this.val = _val;
        }
    }
	/// <summary>
	/// 参数集合
	/// </summary>
	public class Paras : System.Collections.CollectionBase
    {
        #region 特殊处理
        public void AddFK_Emp(string userNo)
        {
            this.Add("FK_Emp", userNo);
        }
        public void AddFK_Emp()
        {
            this.Add("FK_Emp", Web.WebUser.No);
        }

        public void AddFK_NY(string fk_ny)
        {
            this.Add("FK_NY", fk_ny);
        }
        public void AddFK_NY()
        {
            this.Add("FK_NY", DataType.CurrentYearMonth);
        }


        public void AddFK_Dept(string val)
        {
            this.Add("FK_Dept", val);
        }
        public void AddFK_Dept()
        {
            this.Add("FK_Dept", BP.Web.WebUser.FK_Dept );
        }
        #endregion

        public string ToDesc()
        {
            string msg = "";
            foreach (Para p in this)
                msg += "@" + p.ParaName + " = " + p.val;
            return msg;
        }

        public string SQL = null;
        public string SQLNoPara
        {
            get
            {
                string mysql = this.SQL.Clone() as string;
                foreach (Para p in this)
                {
                    if (p.DAType == System.Data.DbType.String)
                    {
                        if (mysql.Contains(SystemConfig.AppCenterDBVarStr + p.ParaName + ","))
                            mysql = mysql.Replace(SystemConfig.AppCenterDBVarStr + p.ParaName + ",", "'" + p.val.ToString() + "',");
                        else
                            mysql = mysql.Replace(SystemConfig.AppCenterDBVarStr + p.ParaName, "'" + p.val.ToString()+"'");
                    }
                    else
                    {
                        if (mysql.Contains(SystemConfig.AppCenterDBVarStr + p.ParaName + ","))
                            mysql = mysql.Replace(SystemConfig.AppCenterDBVarStr + p.ParaName + ",", p.val.ToString() + ",");
                        else
                            mysql = mysql.Replace(SystemConfig.AppCenterDBVarStr + p.ParaName, p.val.ToString());
                    }
                }
                return mysql;
            }
        }

		public Paras()
        {
        }
        public Paras(object o)
        {
            this.Add("p", o);
        }
        public string DBStr
        {
            get
            {
                return BP.Sys.SystemConfig.AppCenterDBVarStr;
            }
        }
        public Paras(string p,object v)
        {
            this.Add(p, v);
        }
        public Paras(string p1, object o1, string p2, object o2)
        {
            this.Add(p1, o1);
            this.Add(p2, o2);
        }

        public bool COntinckey(string key)
        {
            foreach (Para p in this)
            {
                if (p.ParaName == key)
                    return true;
            }
            return false;
        }
       
		public void Add(Para para)
		{
            foreach (Para p in this)
            {
                if (p.ParaName == para.ParaName)
                {
                    p.val = para.val;
                    return;
                }
            }

			this.InnerList.Add(para);
		}
        public void Add( object obj)
        {
           this.Add("p", obj);
        }
        public void Add(string _name, object obj)
        {
            if (_name == "abc")
                return;

            if (obj == null)
                throw new Exception("@参数:" + _name + " 值无效.");

            foreach (Para p in this)
            {
                if (p.ParaName == _name)
                {
                    p.val = obj;
                    return;
                }
            }

            // 2019-8-8 适配pgsql数据库的新版驱动，要求数据类型一致
            //if (String.Compare("FK_Node", _name, StringComparison.OrdinalIgnoreCase) == 0)
            //{
            //    this.Add(_name, Convert.ToInt32(obj));
            //    return;
            //}
            //if (String.Compare("WorkID", _name, StringComparison.OrdinalIgnoreCase) == 0)
            //{
            //    this.Add(_name, Convert.ToInt64(obj));
            //    return;
            //}

            if (obj.GetType() == typeof(string))
            {
                this.Add(_name, obj.ToString());
                return;
            }

            if (obj.GetType() == typeof(int)
                || obj.GetType() == typeof(Int32)
                || obj.GetType() == typeof(Int16))
            {
                this.Add(_name, Int32.Parse(obj.ToString()));
                return;
            }

            if (obj.GetType() == typeof(Int64))
            {
                this.Add(_name, Int64.Parse(obj.ToString()));
                return;
            }

            if (obj.GetType() == typeof(float))
            {
                this.Add(_name, float.Parse(obj.ToString()));
                return;
            }

            if (obj.GetType() == typeof(decimal))
            {
                this.Add(_name, decimal.Parse(obj.ToString()));
                return;
            }

            if (obj.GetType() == typeof(double))
            {
                this.Add(_name, double.Parse(obj.ToString()));
                return;
            }

            if (obj == DBNull.Value)
                this.AddDBNull(_name);
            else
                throw new Exception("@没有判断的参数类型:" + _name);
            //     this.Add(_name, obj.ToString());
        }
        /// <summary>
        /// 是否是大块文本?
        /// </summary>
        /// <param name="_name">名称</param>
        /// <param name="_val">值</param>
        /// <param name="isBigTxt">是否是大文本?</param>
		public void Add(string _name, string _val, bool isBigTxt=false)
		{
			Para en = new Para();

			en.DAType=System.Data.DbType.String;
			en.val = _val;
			en.ParaName = _name;
			en.Size=_val.Length;
            en.IsBigText = isBigTxt; //是否是大块文本.
			this.Add(en);
		}

        public void Add(string _name, Int32 _val)
		{
			Para en = new Para();
			en.DAType=System.Data.DbType.Int32;
			en.val = _val;
			en.ParaName = _name ;
			this.Add(en);
		}
        public void Add(string _name, Int64 _val)
        {
            Para en = new Para();
            en.DAType = System.Data.DbType.Int64;
            en.val = _val;
            en.ParaName = _name;
            this.Add(en);
        }
        public void Add(string _name, float _val)
		{
			Para en = new Para();
			en.DAType=System.Data.DbType.Decimal;
            //   en.DAType = System.Data.DbType.Int64;
			en.val = _val;
			en.ParaName = _name ;
			this.Add(en);
		}
        public void AddDBNull(string _name)
        {
            Para en = new Para();
            en.DAType = System.Data.DbType.Object;
            en.val = DBNull.Value;
            en.ParaName = _name;
            this.Add(en);
        }
        public void Add(string _name, decimal _val)
		{
			Para en = new Para();
			en.DAType=System.Data.DbType.Decimal;
			en.val = _val;
			en.ParaName = _name ;
			this.Add(en);			
		}
        public void Add(string _name, double _val)
        {
            Para en = new Para();
            en.DAType = System.Data.DbType.Decimal;
            en.val = _val;
            en.ParaName = _name;
            this.Add(en);
        }
        public void Remove(string paraName)
        {
            int i=0;
            foreach (Para p in this)
            {
                if (p.ParaName == paraName)
                    break;
                i++;
            }
            this.RemoveAt(i);
        }
    }
}

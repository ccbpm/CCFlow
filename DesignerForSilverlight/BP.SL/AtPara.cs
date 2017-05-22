using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace BP.DA
{
    public class DataType
    {
        public static bool IsNum(string str)
        {
            try
            {
                float f = float.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public class AtPara
    {
        /// <summary>
        /// 工作
        /// </summary>
        public string FK_Work
        {
            get
            {
                return this.GetValStrByKey("FK_Work");
            }
        }
        public string FK_ZJ
        {
            get
            {
                return this.GetValStrByKey("FK_ZJ");
            }
        }
        public int OID
        {
            get
            {
                return this.GetValIntByKey("OID");
            }
        }
        public string DoType
        {
            get
            {
                return this.GetValStrByKey("DoType");
            }
        }
        public AtPara()
        {
        }
        /// <summary>
        /// 执行一个para
        /// </summary>
        /// <param name="para"></param>
        public AtPara(string para)
        {
            if (para == null)
                return;

            string[] strs = para.Split('@');
            foreach (string str in strs)
            {
                if (str == null || str == "")
                    continue;
                string[] mystr = str.Split('=');
                if (mystr.Length == 2)
                {
                    this.SetVal(mystr[0], mystr[1]);
                }
                else
                {
                    string v = "";
                    for (int i = 1; i < mystr.Length; i++)
                    {
                        if (i == 1)
                            v += mystr[i];
                        else
                            v += "=" + mystr[i];
                    }
                    this.SetVal(mystr[0], v);
                }
            }
        }
        public void SetVal(string key, string val)
        {
            try
            {
                this.HisHT.Add(key, val);
            }
            catch
            {
                this.HisHT[key] = val;
            }
        }
        public string GetValStrByKey(string key)
        {
            try
            {
                return this.HisHT[key].ToString();
            }
            catch
            {
                return null;
            }
        }
        public bool GetValBoolenByKey(string key)
        {
            if (this.GetValIntByKey(key)==0)
                return false;
            return true;
        }
        public int GetValIntByKey(string key, int isNullAsVal = 0)
        {
            try
            {
                return int.Parse(this.GetValStrByKey(key));
            }
            catch
            {
                return isNullAsVal;
            }
        }
        private Dictionary<string,object> _HisHT = null;
        public Dictionary<string, object> HisHT
        {
            get
            {
                if (_HisHT == null)
                    _HisHT = new Dictionary<string, object>();
                return _HisHT;
            }
        }
        public string GenerAtParaStrs()
        {
            string s = "";
            foreach (string key in this.HisHT.Keys)
            {
                s += "@" + key + "=" + this._HisHT[key].ToString();
            }
            return s;
        }
    }
}

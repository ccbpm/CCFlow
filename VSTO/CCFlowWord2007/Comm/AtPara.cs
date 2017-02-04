using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BP.DA
{
    public class AtPara
    {
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
                this.SetVal(mystr[0], mystr[1]);
            }
        }


        public void SetVal(string key, string val)
        {
            this.HisHT.Add(key, val);
        }

        public string GetValStrByKey(string key)
        {
            try
            {
                string s = this.HisHT[key].ToString();
                if (s == "")
                   return null;
                return s;
            }
            catch
            {
                return null;
            }
        }

        public int GetValIntByKey(string key)
        {
            try
            {
                return int.Parse(this.GetValStrByKey(key));
            }
            catch
            {
                return 0;
            }
        }

        private Hashtable _HisHT = null;
        public Hashtable HisHT
        {
            get
            {
                if (_HisHT == null)
                    _HisHT = new Hashtable();
                return _HisHT;
            }
        }
    }
}

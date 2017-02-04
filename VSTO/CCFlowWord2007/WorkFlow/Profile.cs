using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Security;
using System.IO;

namespace BP.WF
{
    public class Profile
    {
        public static string GetLoginNo()
        {
            if (System.IO.File.Exists(BP.WF.Glo.ProfileLogin) == false)
                return null;

            string doc = System.IO.File.ReadAllText(BP.WF.Glo.ProfileLogin);
            BP.DA.AtPara ps = new BP.DA.AtPara(doc);
            return ps.GetValStrByKey("No");
        }
        /// <summary>
        /// 是否存在 Profile 。
        /// </summary>
        public static bool IsExitProfile
        {
            get
            {
                return System.IO.File.Exists(BP.WF.Glo.Profile );
            }
        }
        private static string _ProfileDoc = "";
        public static string ProfileDoc
        {
            get
            {
                if (_ProfileDoc == "" || _ProfileDoc == null)
                    _ProfileDoc = System.IO.File.ReadAllText(Glo.PathOfTInstall + "\\Profile.txt");
                return _ProfileDoc;
            }
            set
            {
                _ProfileDoc = null;
            }
        }
        public static int GetValIntByKey(string key)
        {
            try
            {
                return int.Parse( GetValByKey(key) );
            }
            catch
            {
                return 0;
            }
        }
        public static string GetValByKey(string key)
        {
            string[] strs = Profile.ProfileDoc.Split('@');
            foreach (string s in strs)
            {
                if (s == null || s == "")
                    continue;

                if (s.Contains(key + "=") == false)
                    continue;

                string[] keys = s.Split('=');
                string val = keys[1];
                if (val == "")
                    return null;
                return val;
            }
            throw new Exception("@在 Profile 中，没有取到Key=" + key + "的信息。");
        }
    }
}

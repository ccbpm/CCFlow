using System;
using System.Collections;
using System.Collections.Specialized;

namespace BP.Port
{
	public class Current
	{
		static Current()
		{
			Session = new Hashtable();
		}
		public static Hashtable Session;
		public static void SetSession(object key ,object Value)
		{
			if( Session.ContainsKey(key) )
			{
				Session.Remove( key );
			}
			Session.Add(key ,Value);
		}
        public static string GetSessionStr(object key, string isNullAsValue)
        {
            object val = Session[key];
            if (val == null)
                return isNullAsValue;
            return val as string;
            //if (Session.ContainsKey(key))
            //{
            //    Session.Remove(key);
            //}
            //Session.Add(key, Value);
        }
	}
 
}

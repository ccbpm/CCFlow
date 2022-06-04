using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.EnTS
{
    public class Glo
    {
        /// <summary>
        /// 获得map的方法
        /// </summary>
        /// <param name="tsClassID"></param>
        /// <returns></returns>
        public static BP.En.Map GenerMap(string tsClassID)
        {
            if (tsClassID == null)
                throw new Exception("err@错误：tsClassID 不能为null. ");

            string caseTsClassID = "ts" + tsClassID;

            var map = BP.DA.Cash.GetMap(caseTsClassID);
            if (map != null)
                return map;


            //获得前端的map,根据 tsClassID. 
            string url = "http://127.0.0.1/xx.do?TSClassID=" + tsClassID;
            string json = BP.DA.DataType.ReadURLContext(url, 9000);

            //把json 转为map.
            BP.En.Map myMap = new En.Map();


            BP.DA.Cash.SetMap(caseTsClassID, myMap);
            return myMap;
        }

    }
}

using System;
using System.Data;
using System.Collections;
using BP;
using BP.DA;
using BP.En;

namespace BP.DTS
{
    public class DataToCash : DataIOEn
    {
        public DataToCash()
        {
            this.HisDoType = DoType.Especial;
            this.Title = "调度到数据到 cash 中去";
          //  this.HisUserType = Web.UserType.SysAdmin;

            this.DefaultEveryMin = "00";
            this.DefaultEveryHH = "00";
            this.DefaultEveryDay = "00";
            this.DefaultEveryMonth = "00";
            this.Note = "";
        }
        public override void Do()
        {
            Log.DebugWriteInfo("开始执行 DataToCahs ");
            string sql = "";
            string str = "";

            #region 枚举类型放入cash.
            sql = "  SELECT DISTINCT ENUMKEY FROM SYS_ENUM ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                str = dr[0].ToString();
                BP.Sys.SysEnums ses = new BP.Sys.SysEnums(str);
            }
            #endregion
          
            #region 调度单据
            //if (SystemConfig.SysNo == SysNoList.WF)
            //{
            //    Log.DefaultLogWriteLineInfo("单据模板");
            //    sql = "SELECT URL FROM WF_NODEREFFUNC  ";
            //    dt = DBAccess.RunSQLReturnTable(sql);
            //    foreach (DataRow dr in dt.Rows)
            //    {
            //        try
            //        {
            //            str = Cash.GetBillStr(dr[0].ToString(),false);
            //        }
            //        catch (Exception ex)
            //        {
            //            Log.DefaultLogWriteLineInfo("@调入单据cash 出现错误：" + ex.Message);
            //        }
            //    }
            //}
            #endregion

            #region 把类的数据放进cash.
            // entity 数据放进cash.
            ArrayList al = ClassFactory.GetObjects("BP.En.Entities");
            foreach (Entities ens in al)
            {
                Depositary where;
                try
                {
                    where = ens.GetNewEntity.EnMap.DepositaryOfEntity;
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLine(LogType.Info, "@在把数据放在内存时出现错误:" + ex.Message + " cls=" + ens.ToString());
                    /* 包含用户登陆信息的map 都不取它。 */
                    continue;
                }

                if (where == Depositary.None)
                    continue;

                //try
                //{
                //    ens.FlodInCash();
                //}
                //catch (Exception ex)
                //{
                //    Log.DefaultLogWriteLine(LogType.Info, "@把数据放进 cash 中出现错误。@" + ex.Message);
                //}
            }
            #endregion

            #region  把xml 数据放进cash.
            al = ClassFactory.GetObjects("BP.Sys.XML.XmlEns");
            foreach (BP.Sys.XML.XmlEns ens in al)
            {
                try
                {
                    dt = ens.GetTable();
                    ens.RetrieveAll();
                }
                catch (Exception ex)
                {
                    Log.DefaultLogWriteLineError("@调度 " + ens.ToString() + "出现错误:" + ex.Message);
                }
            }
            #endregion

            Log.DefaultLogWriteLine(LogType.Info, "结束执行DataToCahs ");
        }
    }
}

using System;
using System.Collections;
using System.Data;
using System.Text;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF.Port.Admin2Group;
using BP.Difference;
using Newtonsoft.Json;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_DataVer : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_DataVer()
        {
          
        }

        public string DataVer_RollBack()
        {
            Entity en = ClassFactory.GetEn(this.FrmID);
            en.PKVal = this.RefPKVal;
            if (en.RetrieveFromDBSources()==0)
                return "err@数据信息丢失";
            //获取还原的数据
            string mainStr = DBAccess.GetBigTextFromDB("Sys_EnVer", EnVerAttr.MyPK, this.MyPK, EnVerAttr.DBJSON);
            if (DataType.IsNullOrEmpty(mainStr) == true)
                return "err@还原的版本数据为空，不执行还原";
            Hashtable ht = JsonConvert.DeserializeObject<Hashtable>(mainStr);
            foreach (string str in ht.Keys)
            {
                en.SetValByKey(str, ht[str]);
            }
            en.Update();
            return "数据还原成功";


        }
        public string DataVer_Init()
        {
            string mainMyPK =  this.FrmID+"_"+this.RefPK+"_"+GetRequestValInt("MainVer");
            string compareMyPK = this.FrmID + "_" + this.RefPK + "_" + GetRequestValInt("CompareVer");
            if (DataType.IsNullOrEmpty(mainMyPK) || DataType.IsNullOrEmpty(compareMyPK))
                return "err@比对版本传参有误:MainMyPK=" + mainMyPK + ",compareMyPK=" + compareMyPK;
            //获取版本比对数据
            string mainStr = DBAccess.GetBigTextFromDB("Sys_EnVer", EnVerAttr.MyPK, mainMyPK, EnVerAttr.DBJSON);
            string compareStr = DBAccess.GetBigTextFromDB("Sys_EnVer", EnVerAttr.MyPK, compareMyPK, EnVerAttr.DBJSON);
            if (DataType.IsNullOrEmpty(mainStr))
                return "err@数据版本存储有误,版本["+GetRequestValInt("MainVer")+"]数据JSON为空";
            if (DataType.IsNullOrEmpty(compareStr))
                return "err@数据版本存储有误,版本[" + GetRequestValInt("CompareVer") + "]数据JSON为空";
            //获取实体
            Entity en = en = ClassFactory.GetEn(this.FrmID);
            //获取实体字段集合
            Attrs attrs = en.EnMap.Attrs;
            DataSet ds = new DataSet();
            MapAttrs mapAttrs = new MapAttrs();
            //获取Map中的分组
            DataTable dtGroups = new DataTable("Sys_GroupField");
            dtGroups.Columns.Add("OID");
            dtGroups.Columns.Add("Lab");
            string groupName = "";
            string groupNames = "";
            foreach (Attr attr in attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                groupName = attr.GroupName;
                if (groupNames.Contains(groupName + ",") == false)
                {
                    DataRow dr = dtGroups.NewRow();
                    groupNames += groupName + ",";
                    dr["OID"] = groupName;
                    dr["Lab"] = groupName;
                    dtGroups.Rows.Add(dr);
                }

                MapAttr mapAttr = attr.ToMapAttr;
                mapAttr.SetPara("GroupName", attr.GroupName);
                mapAttrs.AddEntity(mapAttr);
            }
            ds.Tables.Add(dtGroups);
            DataTable sys_MapAttrs = mapAttrs.ToDataTableField("Sys_MapAttr");
            ds.Tables.Add(sys_MapAttrs);

            #region 把外键与枚举放入里面去.
            //加入枚举的外键.
            string enumKeys = "";
            foreach (MapAttr mapAttr in mapAttrs)
            {
                if (mapAttr.UIVisible == false)
                    continue;
                if (mapAttr.UIIsEnable == false)
                    continue;
                if (mapAttr.LGType == FieldTypeS.Enum)
                {
                    enumKeys += "'" + mapAttr.UIBindKey + "',";
                    continue;
                }
               
                //外键
                if (mapAttr.LGType == FieldTypeS.FK)
                {
                    // 判断是否存在.
                    if (ds.Tables.Contains(mapAttr.UIBindKey) == true)
                        continue;

                    DataTable keydt = BP.Pub.PubClass.GetDataTableByUIBineKey(mapAttr.UIBindKey);
                    keydt.TableName = mapAttr.KeyOfEn;

                    ds.Tables.Add(keydt);
                }
                //外部数据源
                if(mapAttr.LGType==FieldTypeS.Normal && mapAttr.UIContralType==UIContralType.DDL
                    &&DataType.IsNullOrEmpty(mapAttr.UIBindKey)==false && mapAttr.UIBindKey.ToUpper().Contains("SELECT") == true)
                {
                    /*是一个sql*/
                    string sqlBindKey = mapAttr.UIBindKey;
                    sqlBindKey = BP.WF.Glo.DealExp(sqlBindKey, en, null);

                    DataTable keydt = DBAccess.RunSQLReturnTable(sqlBindKey);
                    keydt.TableName = mapAttr.KeyOfEn;
                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                    {
                        keydt.Columns["NO"].ColumnName = "No";
                        keydt.Columns["NAME"].ColumnName = "Name";
                    }
                    if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                    {
                        keydt.Columns["no"].ColumnName = "No";
                        keydt.Columns["name"].ColumnName = "Name";
                    }
                    ds.Tables.Add(keydt);
                }
            }
            if (enumKeys.Length > 2)
            {
                enumKeys = enumKeys.Substring(0, enumKeys.Length - 1);
                DataTable dtEnum = new DataTable();
                string sqlEnum = "";
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    string sqlWhere = " EnumKey IN (" + enumKeys + ") AND OrgNo='" + WebUser.OrgNo + "'";

                    sqlEnum = "SELECT * FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE " + sqlWhere;
                    sqlEnum += " UNION ";
                    sqlEnum += "SELECT * FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey IN (" + enumKeys + ") AND EnumKey NOT IN (SELECT EnumKey FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE " + sqlWhere + ") AND (OrgNo Is Null Or OrgNo='')";
                    dtEnum = DBAccess.RunSQLReturnTable(sqlEnum);
                }
                else
                {
                    sqlEnum = "SELECT * FROM " + BP.Sys.Base.Glo.SysEnum() + " WHERE EnumKey IN (" + enumKeys + ")";
                    dtEnum = DBAccess.RunSQLReturnTable(sqlEnum);
                }
                dtEnum.TableName = "Sys_Enum";

                if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.UpperCase)
                {
                    dtEnum.Columns["MYPK"].ColumnName = "MyPK";
                    dtEnum.Columns["LAB"].ColumnName = "Lab";
                    dtEnum.Columns["ENUMKEY"].ColumnName = "EnumKey";
                    dtEnum.Columns["INTKEY"].ColumnName = "IntKey";
                    dtEnum.Columns["LANG"].ColumnName = "Lang";
                }

                if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel == FieldCaseModel.Lowercase)
                {
                    dtEnum.Columns["mypk"].ColumnName = "MyPK";
                    dtEnum.Columns["lab"].ColumnName = "Lab";
                    dtEnum.Columns["enumkey"].ColumnName = "EnumKey";
                    dtEnum.Columns["intkey"].ColumnName = "IntKey";
                    dtEnum.Columns["lang"].ColumnName = "Lang";
                }

                ds.Tables.Add(dtEnum);
            }
            #endregion 把外键与枚举放入里面去.


            DataTable dt = BP.Tools.Json.ToDataTable(mainStr);
            dt.TableName = "MainData";
            ds.Tables.Add(dt);
            DataTable dtt = BP.Tools.Json.ToDataTable(compareStr);
            dtt.TableName = "CompareData";
            ds.Tables.Add(dtt);
            return BP.Tools.Json.ToJson(ds);
        }
    }
}

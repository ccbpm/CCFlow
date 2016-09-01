using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BP.En;
using BP.DA;

namespace BP.Sys
{
    /// <summary>
    /// 表单API
    /// </summary>
    public class CCFormAPI
    {
        /// <summary>
        /// 创建/修改一个明细表
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dtlNo">明细表编号</param>
        /// <param name="dtlName">名称</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void CreateOrSaveDtl(string fk_mapdata, string dtlNo, string dtlName, float x, float y)
        {
            MapDtl dtl = new MapDtl();
            dtl.No = dtlNo;
            dtl.RetrieveFromDBSources();

            dtl.X = x;
            dtl.Y = y;
            dtl.Name = dtlName;
            dtl.FK_MapData = fk_mapdata;
            dtl.Save();
        }
        /// <summary>
        /// 创建一个外部数据字段
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldDesc">字段中文名</param>
        /// <param name="fk_SFTable">外键表</param>
        /// <param name="x">位置</param>
        /// <param name="y">位置</param>
        /// <param name="colSpan">跨的列数</param>
        public static void SaveFieldSFTable(string fk_mapdata, string fieldName, string fieldDesc, string fk_SFTable, float x, float y, int colSpan = 1)
        {
            //外键字段表.
            SFTable sf = new SFTable(fk_SFTable);

            MapAttr attr = new MapAttr();
            attr.MyPK = fk_mapdata + "_" + fieldName;
            attr.RetrieveFromDBSources();

            //基本属性赋值.
            attr.FK_MapData = fk_mapdata;
            attr.KeyOfEn = fieldName;
            attr.Name = fieldDesc;
            attr.MyDataType = BP.DA.DataType.AppString;
            attr.UIContralType = BP.En.UIContralType.DDL;
            attr.UIBindKey = fk_SFTable; //绑定信息.
            attr.X = x;
            attr.Y = y;

            //根据外键表的类型不同，设置它的LGType.
            switch (sf.SrcType)
            {
                case SrcType.BPClass:
                case SrcType.CreateTable:
                case SrcType.TableOrView:
                    attr.LGType = FieldTypeS.FK;
                    break;
                default:
                    attr.LGType = FieldTypeS.Normal;
                    break;
            }
            attr.Save();
           
            //如果是普通的字段, 这个属于外部数据类型,或者webservices类型.
            if (attr.LGType == FieldTypeS.Normal)
            {
                MapAttr attrH = new MapAttr();
                attrH.Copy(attr);
                attrH.KeyOfEn = attr.KeyOfEn + "T";
                attrH.Name = attr.Name;
                attrH.UIContralType = BP.En.UIContralType.TB;
                attrH.MinLen = 0;
                attrH.MaxLen = 60;
                attrH.MyDataType = BP.DA.DataType.AppString;
                attrH.UIVisible = false;
                attrH.UIIsEnable = false;
                attrH.MyPK = attrH.FK_MapData + "_" + attrH.KeyOfEn;
                attrH.Save();
            }
        }
        /// <summary>
        /// 保存枚举字段
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldDesc">字段描述</param>
        /// <param name="enumKey">枚举值</param>
        /// <param name="ctrlType">显示的控件类型</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        /// <param name="colSpan">横跨的行数</param>
        public static void SaveFieldEnum(string fk_mapdata, string fieldName, string fieldDesc, string enumKey, UIContralType ctrlType,
            float x, float y, int colSpan = 1)
        {
            MapAttr ma = new MapAttr();
            ma.FK_MapData = fk_mapdata;
            ma.KeyOfEn = fieldName;

            //赋值主键。
            ma.MyPK = ma.FK_MapData + "_" + ma.KeyOfEn;

            //先查询赋值.
            ma.RetrieveFromDBSources();

            ma.Name = fieldDesc;
            ma.MyDataType = DataType.AppInt;
            ma.X = x;
            ma.Y = y;
            ma.UIIsEnable = true;
            ma.LGType = FieldTypeS.Enum;

            ma.UIContralType = ctrlType;
            ma.UIBindKey = enumKey;

            if (ma.UIContralType == UIContralType.RadioBtn)
            {
                SysEnums ses = new SysEnums(ma.UIBindKey);
                int idx = 0;
                foreach (SysEnum item in ses)
                {
                    idx++;
                    FrmRB rb = new FrmRB();
                    rb.FK_MapData = ma.FK_MapData;
                    rb.KeyOfEn = ma.KeyOfEn;
                    rb.IntKey = item.IntKey;
                    rb.MyPK = rb.FK_MapData + "_" + rb.KeyOfEn + "_" + rb.IntKey;
                    rb.RetrieveFromDBSources();
                   
                    rb.EnumKey = ma.UIBindKey;
                    rb.Lab = item.Lab;
                    rb.X = ma.X;

                    //让其变化y值.
                    rb.Y = ma.Y + idx * 30;
                    rb.Save();
                }
            }
            ma.Save();
        }
        /// <summary>
        /// 获得表单模版dataSet格式.
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <returns>DataSet</returns>
        public static System.Data.DataSet GenerHisDataSet(string fk_mapdata)
        {
            // 20150513 小周鹏修改，原因：手机端无法显示dtl Start
            // string sql = "SELECT FK_MapData,No,X,Y,W,H  FROM Sys_MapDtl WHERE FK_MapData ='{0}'";
            string sql = "SELECT *  FROM Sys_MapDtl WHERE FK_MapData ='{0}'";
            // 20150513 小周鹏修改 End

            sql = string.Format(sql, fk_mapdata);
            DataTable dtMapDtl = DBAccess.RunSQLReturnTable(sql);
            dtMapDtl.TableName = "Sys_MapDtl";

            string ids = string.Format("'{0}'", fk_mapdata);
            foreach (DataRow dr in dtMapDtl.Rows)
            {
                ids += ",'" + dr["No"] + "'";
            }
            string sqls = string.Empty;
            List<string> listNames = new List<string>();
            // Sys_GroupField.
            listNames.Add("Sys_GroupField");
            sql = "SELECT * FROM Sys_GroupField WHERE  EnName IN (" + ids + ")";
            sqls += sql;

            // Sys_Enum
            listNames.Add("Sys_Enum");
            sql = "@SELECT * FROM Sys_Enum WHERE EnumKey IN ( SELECT UIBindKey FROM Sys_MapAttr WHERE FK_MapData IN (" + ids + ") )";
            sqls += sql;

            // 审核组件
            string nodeIDstr = fk_mapdata.Replace("ND", "");
            if (DataType.IsNumStr(nodeIDstr))
            {
                // 审核组件状态:0 禁用;1 启用;2 只读;
                listNames.Add("WF_Node");
                sql = "@SELECT NodeID,FWC_X,FWC_Y,FWC_W,FWC_H,FWCSTA,FWCTYPE,SFSTA,SF_X,SF_Y,SF_H,SF_W FROM WF_Node WHERE NodeID=" + nodeIDstr + " AND  ( FWCSta >0  OR SFSta >0 )";
                sqls += sql;
            }

            string where = " FK_MapData IN (" + ids + ")";

            // Sys_MapData.
            listNames.Add("Sys_MapData");
            sql = "@SELECT No,Name,FrmW,FrmH FROM Sys_MapData WHERE No='" + fk_mapdata + "'";
            sqls += sql;

            // Sys_MapAttr.
            listNames.Add("Sys_MapAttr");

            sql = "@SELECT * FROM Sys_MapAttr WHERE " + where + " AND KeyOfEn NOT IN('WFState') ORDER BY FK_MapData, IDX  ";
            sqls += sql;

            //// Sys_MapM2M.
            //listNames.Add("Sys_MapM2M");
            //sql = "@SELECT MyPK,FK_MapData,NoOfObj,M2MTYPE,X,Y,W,H FROM Sys_MapM2M WHERE " + where;
            //sqls += sql;

            // Sys_MapExt.
            listNames.Add("Sys_MapExt");
            sql = "@SELECT * FROM Sys_MapExt WHERE " + where;
            sqls += sql;

            // line.
            listNames.Add("Sys_FrmLine");
            sql = "@SELECT MyPK,FK_MapData,X,X1,X2,Y,Y1,Y2,BorderColor,BorderWidth from Sys_FrmLine WHERE " + where;
            sqls += sql;

            // ele.
            listNames.Add("Sys_FrmEle");
            sql = "@SELECT FK_MapData,MyPK,EleType,EleID,EleName,X,Y,W,H FROM Sys_FrmEle WHERE " + where;
            sqls += sql;

            // link.
            listNames.Add("Sys_FrmLink");
            sql = "@SELECT FK_MapData,MyPK,Text,URL,Target,FontSize,FontColor,X,Y from Sys_FrmLink WHERE " + where;
            sqls += sql;

            // btn.
            listNames.Add("Sys_FrmBtn");
            sql = "@SELECT FK_MapData,MyPK,Text,BtnType,EventType,EventContext,MsgErr,MsgOK,X,Y FROM Sys_FrmBtn WHERE " + where;
            sqls += sql;

            // Sys_FrmImg.
            listNames.Add("Sys_FrmImg");
            sql = "@SELECT * FROM Sys_FrmImg WHERE " + where;
            sqls += sql;

            // Sys_FrmLab.
            listNames.Add("Sys_FrmLab");
            sql = "@SELECT MyPK,FK_MapData,Text,X,Y,FontColor,FontName,FontSize,FontStyle,FontWeight,IsBold,IsItalic from Sys_FrmLab WHERE " + where;
            sqls += sql;

            // Sys_FrmRB.
            listNames.Add("Sys_FrmRB");
            sql = "@SELECT * FROM Sys_FrmRB WHERE " + where;
            sqls += sql;

            // Sys_FrmAttachment. 
            listNames.Add("Sys_FrmAttachment");
            /* 20150730 小周鹏修改 添加AtPara 参数 START */
            //sql = "@SELECT  MyPK,FK_MapData,UploadType,X,Y,W,H,NoOfObj,Name,Exts,SaveTo,IsUpload,IsDelete,IsDownload "
            // + " FROM Sys_FrmAttachment WHERE " + where + " AND FK_Node=0";
            sql = "@SELECT MyPK,FK_MapData,UploadType,X,Y,W,H,NoOfObj,Name,Exts,SaveTo,IsUpload,IsDelete,IsDownload ,AtPara"
                + " FROM Sys_FrmAttachment WHERE " + where + " AND FK_Node=0";
            /* 20150730 小周鹏修改 添加AtPara 参数 END */
            sqls += sql;

            // Sys_FrmImgAth.
            listNames.Add("Sys_FrmImgAth");
            sql = "@SELECT * FROM Sys_FrmImgAth WHERE " + where;
            sqls += sql;

            //// sqls.Replace(";", ";" + Environment.NewLine);
            // DataSet ds = DA.DBAccess.RunSQLReturnDataSet(sqls);
            // if (ds != null && ds.Tables.Count == listNames.Count)
            //     for (int i = 0; i < listNames.Count; i++)
            //     {
            //         ds.Tables[i].TableName = listNames[i];
            //     }

            string[] strs = sqls.Split('@');
            DataSet ds = new DataSet();

            if (strs != null && strs.Length == listNames.Count)
                for (int i = 0; i < listNames.Count; i++)
                {
                    string s = strs[i];
                    if (string.IsNullOrEmpty(s))
                        continue;
                    DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(s);
                    dt.TableName = listNames[i];
                    ds.Tables.Add(dt);
                }

            foreach (DataTable item in ds.Tables)
            {
                if (item.TableName == "Sys_MapAttr" && item.Rows.Count == 0)
                {
                    BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
                    attr.FK_MapData = fk_mapdata;
                    attr.KeyOfEn = "OID";
                    attr.Name = "OID";
                    attr.MyDataType = BP.DA.DataType.AppInt;
                    attr.UIContralType = UIContralType.TB;
                    attr.LGType = FieldTypeS.Normal;
                    attr.UIVisible = false;
                    attr.UIIsEnable = false;
                    attr.DefVal = "0";
                    attr.HisEditType = BP.En.EditType.Readonly;
                    attr.Insert();
                }
            }

            ds.Tables.Add(dtMapDtl);
            return ds;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.Web;
using BP.Port;
using BP.En;
using BP.WF;
using BP.WF.Template;
using BP.WF.Data;
using BP.WF.HttpHandler;
using BP.CCBill.Template;
using ThoughtWorks.QRCode.Codec;
using System.Drawing;
using System.Drawing.Imaging;

namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_OptComponents : DirectoryPageBase
    {
        #region 构造方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_OptComponents()
        {
        }
        #endregion 构造方法.

        #region 关联单据.
        /// <summary>
        /// 设置父子关系.
        /// </summary>
        /// <returns></returns>
        public string RefBill_Done()
        {
            try
            {
                string frmID = this.GetRequestVal("FrmID");
                Int64 workID = this.GetRequestValInt64("WorkID");
                GERpt rpt = new GERpt(frmID, workID);

                string pFrmID = this.GetRequestVal("PFrmID");
                Int64 pWorkID = this.GetRequestValInt64("PWorkID");

                //把数据copy到当前的子表单里.
                GERpt rptP = new GERpt(pFrmID, pWorkID);
                rpt.Copy(rptP);
                rpt.PWorkID = pWorkID;
                rpt.SetValByKey("PFrmID", pFrmID);
                rpt.Update();

                //更新控制表,设置父子关系.
                GenerBill gbill = new GenerBill(workID);
                gbill.PFrmID = pFrmID;
                gbill.PWorkID = pWorkID;
                gbill.Update();
                return "执行成功";
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }
        /// <summary>
        /// 单据初始化
        /// </summary>
        /// <returns></returns>
        public string RefBill_Init()
        {
            DataSet ds = new DataSet();

            #region 查询显示的列
            MapAttrs mapattrs = new MapAttrs();
            mapattrs.Retrieve(MapAttrAttr.FK_MapData, this.FrmID, MapAttrAttr.Idx);

            DataRow row = null;
            DataTable dt = new DataTable("Attrs");
            dt.Columns.Add("KeyOfEn", typeof(string));
            dt.Columns.Add("Name", typeof(string));
            dt.Columns.Add("Width", typeof(int));
            dt.Columns.Add("UIContralType", typeof(int));
            dt.Columns.Add("LGType", typeof(int));

            //设置标题、单据号位于开始位置


            foreach (MapAttr attr in mapattrs)
            {
                string searchVisable = attr.atPara.GetValStrByKey("SearchVisable");
                if (searchVisable == "0")
                    continue;
                if (attr.UIVisible == false)
                    continue;
                row = dt.NewRow();
                row["KeyOfEn"] = attr.KeyOfEn;
                row["Name"] = attr.Name;
                row["Width"] = attr.UIWidthInt;
                row["UIContralType"] = attr.UIContralType;
                row["LGType"] = attr.LGType;
                dt.Rows.Add(row);
            }
            ds.Tables.Add(dt);
            #endregion 查询显示的列

            #region 查询语句

            MapData md = new MapData(this.FrmID);

            GEEntitys rpts = new GEEntitys(this.FrmID);

            Attrs attrs = rpts.GetNewEntity.EnMap.Attrs;

            QueryObject qo = new QueryObject(rpts);

            #region 关键字字段.
            string keyWord = this.GetRequestVal("SearchKey");

            if (DataType.IsNullOrEmpty(keyWord) == false && keyWord.Length >= 1)
            {
                qo.addLeftBracket();
                if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                    qo.AddWhere("Title", " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? (" CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : (" '%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                else
                    qo.AddWhere("Title", " LIKE ", " '%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");
                qo.addOr();
                if (SystemConfig.AppCenterDBVarStr == "@" || SystemConfig.AppCenterDBVarStr == "?")
                    qo.AddWhere("BillNo", " LIKE ", SystemConfig.AppCenterDBType == DBType.MySQL ? ("CONCAT('%'," + SystemConfig.AppCenterDBVarStr + "SKey,'%')") : ("'%'+" + SystemConfig.AppCenterDBVarStr + "SKey+'%'"));
                else
                    qo.AddWhere("BillNo", " LIKE ", "'%'||" + SystemConfig.AppCenterDBVarStr + "SKey||'%'");

                qo.MyParas.Add("SKey", keyWord);
                qo.addRightBracket();

            }
            else
            {
                qo.AddHD();
            }
            #endregion 关键字段查询

            #region 时间段的查询
            string dtFrom = this.GetRequestVal("DTFrom");
            string dtTo = this.GetRequestVal("DTTo");
            if (DataType.IsNullOrEmpty(dtFrom) == false)
            {

                //取前一天的24：00
                if (dtFrom.Trim().Length == 10) //2017-09-30
                    dtFrom += " 00:00:00";
                if (dtFrom.Trim().Length == 16) //2017-09-30 00:00
                    dtFrom += ":00";

                dtFrom = DateTime.Parse(dtFrom).AddDays(-1).ToString("yyyy-MM-dd") + " 24:00";

                if (dtTo.Trim().Length < 11 || dtTo.Trim().IndexOf(' ') == -1)
                    dtTo += " 24:00";

                qo.addAnd();
                qo.addLeftBracket();
                qo.SQL = " RDT>= '" + dtFrom + "'";
                qo.addAnd();
                qo.SQL = "RDT <= '" + dtTo + "'";
                qo.addRightBracket();
            }
            #endregion 时间段的查询

            qo.DoQuery("OID", this.PageSize, this.PageIdx);

            #endregion

            DataTable mydt = rpts.ToDataTableField();
            mydt.TableName = "DT";

            ds.Tables.Add(mydt); //把数据加入里面.

            return BP.Tools.Json.ToJson(ds);
        }
        #endregion 关联单据.

        #region 数据版本.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string DataVer_Init()
        {
            return null;
        }
        public string DataVer_AppFieldData()
        {
            try
            {
                int verNum = this.GetRequestValInt("VerNum");

                EnVer en = new EnVer();
                int i = en.Retrieve(EnVerAttr.EnVer, verNum, EnVerAttr.FrmID, this.FrmID, EnVerDtlAttr.EnPKValue, this.WorkID);
                if (i == 0)
                    return "err@版本号输入错误";

                string keyOfEn = this.GetRequestVal("KeyOfEn");
                EnVerDtl dtl = new EnVerDtl();
                i = dtl.Retrieve(EnVerDtlAttr.RefPK, en.MyPK, EnVerDtlAttr.AttrKey, keyOfEn);

                if (i == 0)
                    return "err@该版本下没有查询到字段[" + keyOfEn + "]的值.";

                GEEntity ge = new GEEntity(this.FrmID, this.WorkID);
                ge.SetValByKey(keyOfEn, dtl.MyVal);
                ge.Update();

                string msg= "字段：[" + dtl.AttrKey + "],已经修改为:" + dtl.MyVal;

                BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, null, this.WorkID, FrmActionType.DataVerReback, msg);
                return msg;
            }
            catch (Exception ex)
            {
                return "err@" + ex.Message;
            }
        }

        /// <summary>
        /// 回滚数据
        /// </summary>
        /// <returns></returns>
        public string DataVer_Reback()
        {
            EnVerDtls dtls = new EnVerDtls();
            dtls.Retrieve(EnVerDtlAttr.RefPK, this.MyPK);

            GEEntity ge = new GEEntity(this.FrmID, this.WorkID);

            foreach (EnVerDtl item in dtls)
            {
                if (item.LGType == 0)
                {
                    ge.SetValByKey(item.AttrKey, item.MyVal);
                    continue;
                }

                //外键枚举存储的格式为 [0][女]    [01][山东]
                string val = item.MyVal.Substring(1, item.MyVal.IndexOf(']') - 1);
                ge.SetValByKey(item.AttrKey, val);

            }
            ge.Update();

            // BP.CCBill.Dev2Interface.MyEntityTree_Delete

            BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, null, this.WorkID, FrmActionType.DataVerReback, "数据回滚");

            return "已经成功还原...";
        }
        /// <summary>
        /// 创建新版本
        /// </summary>
        /// <returns></returns>
        public string DataVer_NewVer()
        {
            //创建实体.
            EnVer ev = new EnVer();
            ev.MyPK = DBAccess.GenerGUID();
            ev.RecNo = WebUser.No;
            ev.RecName = WebUser.Name;
            ev.RDT = DataType.CurrentDataTimeCN;
            ev.FrmID = this.FrmID;
            ev.EnPKValue = this.WorkID.ToString();
            ev.MyNote = this.GetRequestVal("MyNote");

            MapData md = new MapData(this.FrmID);
            ev.Name = md.Name;

            // 获得最大的版本号.
            int maxVer = DBAccess.RunSQLReturnValInt("SELECT MAX(EnVer) as Num FROM Sys_EnVer WHERE  FrmID='" + this.FrmID + "' AND EnPKValue='" + this.WorkID + "'", 0);

            ev.Ver = maxVer + 1; //设置版本号.
            ev.Insert(); //执行插入.

            //创建变更数据.
            GEEntity en = new GEEntity(this.FrmID, this.WorkID);
            EnVerDtl dtl = new EnVerDtl();

            //不需要存储的字段.
            string sysFiels = ",AtPara,OID,WorkID,WFState,BillNo,Title,RDT,CDT,OrgNo,Starter,StarterName,BillState,FK_Dept,";

            MapAttrs mapattrs = new MapAttrs(this.FrmID);
            foreach (MapAttr attr in mapattrs)
            {
                //如果是非数据控件.
                if ((int)attr.UIContralType >= 4)
                    continue;

                if (sysFiels.Contains("," + attr.KeyOfEn + ",") == true)
                    continue;

                dtl.MyPK = DBAccess.GenerGUID();
                dtl.RefPK = ev.MyPK; //设置关联主键.

                dtl.FrmID = ev.FrmID;
                dtl.EnPKValue = this.WorkID.ToString(); //设置为主键.

                dtl.AttrKey = attr.KeyOfEn;
                dtl.AttrName = attr.Name;

                //逻辑类型.
                dtl.LGType = (int)attr.LGType;

                //if (attr.LGType == FieldType.Enum)
                //    dtl.LGType = 1;
                //if (attr.MyFieldType == FieldType.FK)
                //    dtl.LGType = 2;

                //设置外键.
                dtl.BindKey = attr.UIBindKey;
                if (attr.LGType == FieldTypeS.Normal)
                {
                    //设置值.
                    dtl.MyVal = en.GetValByKey(attr.KeyOfEn).ToString();
                }
                else
                {
                    //设置值.
                    dtl.MyVal = "[" + en.GetValByKey(attr.KeyOfEn).ToString() + "][" + en.GetValRefTextByKey(attr.KeyOfEn) + "]";
                }
                dtl.Insert();
            }


            BP.CCBill.Dev2Interface.Dict_AddTrack(this.FrmID, null, this.WorkID, FrmActionType.DataVerReback, "创建数据版本.");

            return "版本创建成功.";
        }

        #endregion 数据版本.

        #region 二维码.
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public string QRCode_Init()
        {
            string url = SystemConfig.HostURL + "/WF/CCBill/OptComponents/QRCode.htm?DoType=MyDict&WorkID=" + this.WorkID + "&FrmID=" + this.FrmID + "&MethodNo=" + this.GetRequestVal("MethodNo");
            QRCodeEncoder encoder = new QRCodeEncoder();
            encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            encoder.QRCodeScale = 4;//大小(值越大生成的二维码图片像素越高)
            encoder.QRCodeVersion = 0;//版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;//错误效验、错误更正(有4个等级)
            encoder.QRCodeBackgroundColor = Color.White;
            encoder.QRCodeForegroundColor = Color.Black;

            //生成临时文件.
            System.Drawing.Image image = encoder.Encode(url, Encoding.UTF8);
            string tempPath = SystemConfig.PathOfTemp + "\\" + this.WorkID + ".png";
            image.Save(tempPath, ImageFormat.Png);
            image.Dispose();

            //返回url.
            return url;
        }
        /// <summary>
        /// 扫描要做的工作
        /// </summary>
        /// <returns></returns>
        public string QRCodeScan_Init()
        {
           
            string url = SystemConfig.HostURL + "/WF/CCBill/OptComponents/QRCodeScan.htm?DoType=MyDict&WorkID=" + this.WorkID + "&FrmID=" + this.FrmID + "&MethodNo=" + this.GetRequestVal("MethodNo");
            QRCodeEncoder encoder = new QRCodeEncoder();
            encoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;//编码方式(注意：BYTE能支持中文，ALPHA_NUMERIC扫描出来的都是数字)
            encoder.QRCodeScale = 4;//大小(值越大生成的二维码图片像素越高)
            encoder.QRCodeVersion = 0;//版本(注意：设置为0主要是防止编码的字符串太长时发生错误)
            encoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.H;//错误效验、错误更正(有4个等级)
            encoder.QRCodeBackgroundColor = Color.White;
            encoder.QRCodeForegroundColor = Color.Black;

            //生成临时文件.
            System.Drawing.Image image = encoder.Encode(url, Encoding.UTF8);
            string tempPath = SystemConfig.PathOfTemp + "\\" + this.WorkID + ".png";
            image.Save(tempPath, ImageFormat.Png);
            image.Dispose();

            //返回url.
            return url;
        }
        #endregion 二维码.

    }
}

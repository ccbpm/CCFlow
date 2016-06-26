using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using System.Collections.Generic;

namespace BP.Sys
{
    /// <summary>
    /// 按日期查询方式
    /// </summary>
    public enum DTSearchWay
    {
        /// <summary>
        /// 不设置
        /// </summary>
        None,
        /// <summary>
        /// 按日期
        /// </summary>
        ByDate,
        /// <summary>
        /// 按日期时间
        /// </summary>
        ByDateTime
    }
    /// <summary>
    /// 表单类型
    /// </summary>
    public enum AppType
    {
        /// <summary>
        /// 独立表单
        /// </summary>
        Application = 0,
        /// <summary>
        /// 节点表单
        /// </summary>
        Node = 1
    }
    public enum FrmFrom
    {
        Flow,
        Node,
        Dtl
    }
    /// <summary>
    /// 表单类型
    /// </summary>
    public enum FrmType
    {
        /// <summary>
        /// 自由表单
        /// </summary>
        FreeFrm = 0,
        /// <summary>
        /// 傻瓜表单
        /// </summary>
        Column4Frm = 1,
        /// <summary>
        /// silverlight
        /// </summary>
        SLFrm = 2,
        /// <summary>
        /// URL表单(自定义)
        /// </summary>
        Url = 3,
        /// <summary>
        /// Word类型表单
        /// </summary>
        WordFrm = 4,
        /// <summary>
        /// Excel类型表单
        /// </summary>
        ExcelFrm = 5
    }
    /// <summary>
    /// 映射基础
    /// </summary>
    public class MapDataAttr : EntityNoNameAttr
    {
        public const string PTable = "PTable";
        public const string Dtls = "Dtls";
        public const string EnPK = "EnPK";
        public const string FrmW = "FrmW";
        public const string FrmH = "FrmH";
        /// <summary>
        /// 表格列(对傻瓜表单有效)
        /// </summary>
        public const string TableCol = "TableCol";
        /// <summary>
        /// 表格宽度
        /// </summary>
        public const string TableWidth = "TableWidth";
        /// <summary>
        /// 来源
        /// </summary>
        public const string FrmFrom = "FrmFrom";
        /// <summary>
        /// 设计者
        /// </summary>
        public const string Designer = "Designer";
        /// <summary>
        /// 设计者单位
        /// </summary>
        public const string DesignerUnit = "DesignerUnit";
        /// <summary>
        /// 设计者联系方式
        /// </summary>
        public const string DesignerContact = "DesignerContact";
        /// <summary>
        /// 表单类别
        /// </summary>
        public const string FK_FrmSort = "FK_FrmSort";
        /// <summary>
        /// 表单树类别
        /// </summary>
        public const string FK_FormTree = "FK_FormTree";
        /// <summary>
        /// 表单类型
        /// </summary>
        public const string FrmType = "FrmType";
        /// <summary>
        /// Url(对于嵌入式表单有效)
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// Tag
        /// </summary>
        public const string Tag = "Tag";
        /// <summary>
        /// 备注
        /// </summary>
        public const string Note = "Note";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 版本号
        /// </summary>
        public const string Ver = "Ver";
        /// <summary>
        /// 数据源
        /// </summary>
        public const string DBSrc = "DBSrc";
        /// <summary>
        /// 应用类型
        /// </summary>
        public const string AppType = "AppType";
        /// <summary>
        /// 表单body属性.
        /// </summary>
        public const string BodyAttr = "BodyAttr";

        #region 报表属性(参数的方式存储).
        /// <summary>
        /// 是否关键字查询
        /// </summary>
        public const string RptIsSearchKey = "RptIsSearchKey";
        /// <summary>
        /// 时间段查询方式
        /// </summary>
        public const string RptDTSearchWay = "RptDTSearchWay";
        /// <summary>
        /// 时间字段
        /// </summary>
        public const string RptDTSearchKey = "RptDTSearchKey";
        /// <summary>
        /// 查询外键枚举字段
        /// </summary>
        public const string RptSearchKeys = "RptSearchKeys";
        #endregion 报表属性(参数的方式存储).

        #region 其他计算属性，参数存储.
        /// <summary>
        /// 最左边的值
        /// </summary>
        public const string MaxLeft = "MaxLeft";
        /// <summary>
        /// 最右边的值
        /// </summary>
        public const string MaxRight = "MaxRight";
        /// <summary>
        /// 最头部的值
        /// </summary>
        public const string MaxTop = "MaxTop";
        /// <summary>
        /// 最底部的值
        /// </summary>
        public const string MaxEnd = "MaxEnd";
        #endregion 其他计算属性，参数存储.

        #region weboffice属性。
        /// <summary>
        /// 是否启用锁定行
        /// </summary>
        public const string IsRowLock = "IsRowLock";
        /// <summary>
        /// 是否启用weboffice
        /// </summary>
        public const string IsWoEnableWF = "IsWoEnableWF";
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public const string IsWoEnableSave = "IsWoEnableSave";
        /// <summary>
        /// 是否只读
        /// </summary>
        public const string IsWoEnableReadonly = "IsWoEnableReadonly";
        /// <summary>
        /// 是否启用修订
        /// </summary>
        public const string IsWoEnableRevise = "IsWoEnableRevise";
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public const string IsWoEnableViewKeepMark = "IsWoEnableViewKeepMark";
        /// <summary>
        /// 是否打印
        /// </summary>
        public const string IsWoEnablePrint = "IsWoEnablePrint";
        /// <summary>
        /// 是否启用签章
        /// </summary>
        public const string IsWoEnableSeal = "IsWoEnableSeal";
        /// <summary>
        /// 是否启用套红
        /// </summary>
        public const string IsWoEnableOver = "IsWoEnableOver";
        /// <summary>
        /// 是否启用公文模板
        /// </summary>
        public const string IsWoEnableTemplete = "IsWoEnableTemplete";
        /// <summary>
        /// 是否自动写入审核信息
        /// </summary>
        public const string IsWoEnableCheck = "IsWoEnableCheck";
        /// <summary>
        /// 是否插入流程
        /// </summary>
        public const string IsWoEnableInsertFlow = "IsWoEnableInsertFlow";
        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public const string IsWoEnableInsertFengXian = "IsWoEnableInsertFengXian";
        /// <summary>
        /// 是否启用留痕模式
        /// </summary>
        public const string IsWoEnableMarks = "IsWoEnableMarks";
        /// <summary>
        /// 是否启用下载
        /// </summary>
        public const string IsWoEnableDown = "IsWoEnableDown";
        #endregion weboffice属性。
    }
    /// <summary>
    /// 映射基础
    /// </summary>
    public class MapData : EntityNoName
    {
        #region 执行 ccform2.0 的保存.
        /// <summary>
        /// 保存表单信息
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ds">控件集合</param>
        public static void SaveCCForm20(string fk_mapdata, DataSet ds)
        {
            //元素集合.
            FrmEles eles = new FrmEles();
            eles.Retrieve(FrmEleAttr.FK_MapData, fk_mapdata);

            #region 生成组合PK.
            string lines = "";
            string labs = "";
            string links = "";
            string btns = "";
            foreach (FrmEle item in eles)
            {
                if (item.EleType == "Line")
                {
                    lines += item.MyPK + ",";
                    continue;
                }
                if (item.EleType == "Lab")
                {
                    labs += item.MyPK + ",";
                    continue;
                }
                if (item.EleType == "Line")
                {
                    lines += item.MyPK + ",";
                    continue;
                }
                if (item.EleType == "Btn")
                {
                    btns += item.MyPK + ",";
                    continue;
                }
                if (item.EleType == "Img")
                {
                    btns += item.MyPK + ",";
                    continue;
                }
            }
            #endregion 生成组合PK.

            //字段集合.
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(FrmEleAttr.FK_MapData, fk_mapdata);


            //循环这个集合,开始执行保存.
            foreach (DataTable dt in ds.Tables)
            {
                switch (dt.TableName)
                {
                    case "Sys_FrmLine": //线.
                        SaveCCForm20_Line(fk_mapdata, dt, lines);
                        break;
                    case "Sys_FrmBtn": //按钮.
                        SaveCCForm20_Btn(fk_mapdata, dt, btns);
                        break;
                    case "Sys_FrmLab": //标签.
                        SaveCCForm20_Lab(fk_mapdata, dt, labs);
                        break;
                    case "Sys_FrmLink": //链接.
                        SaveCCForm20_Link(fk_mapdata, dt, links);
                        break;
                    case "Sys_FrmImg": //图片.
                        SaveCCForm20_Img(fk_mapdata, dt, links);
                        break;
                    case "Sys_FrmEle": //链接.
                        break;
                    case "Sys_FrmImgAth": //图片附件.
                        break;
                    case "Sys_FrmRB": //单选按钮.
                        break;
                    case "Sys_MapData": //MapData.
                        break;
                    case "Sys_MapAttr": //字段属性.
                        break;
                    case "Sys_MapDtl": //明细表.
                        break;
                    case "Sys_MapM2M": //一对多.
                        break;
                    case "WF_Node": //节点.
                        break;
                    //default:
                    //    throw new Exception("@没有约定的表结构.");
                }
            }
        }

        #region 保存 frmEle控件。
        /// <summary>
        /// 保存line
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Line(string fk_mapdata, DataTable dt, string elePKs)
        {
            string lines = "";
            FrmEle ele = new FrmEle();
            foreach (DataRow dr in dt.Rows)
            {
                ele.MyPK = dr["MyPK"].ToString();
                ele.EleType = "Line";
                ele.FK_MapData = fk_mapdata;


                ele.LineX1 = dr["X"].ToString();
                ele.LineY1 = dr["Y"].ToString();

                ele.LineX2 = dr["W"].ToString();
                ele.LineY2 = dr["H"].ToString();

                ele.LineBorderColor = dr["Tag1"].ToString();
                ele.LineBorderWidth = dr["Tag2"].ToString();

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                    ele.DirectInsert();
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmEle WHERE MyPK='" + str + "'";
            }

            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 保存img
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Img(string fk_mapdata, DataTable dt, string elePKs)
        {
            string lines = "";
            FrmEle ele = new FrmEle();
            foreach (DataRow dr in dt.Rows)
            {
                ele.Copy(dr);
                ele.EleType = "Img";
                ele.FK_MapData = fk_mapdata;

                //位置.
                ele.X = float.Parse( dr["X"].ToString());
                ele.Y = float.Parse(dr["Y"].ToString());

                ele.W = float.Parse(dr["W"].ToString());
                ele.H = float.Parse(dr["H"].ToString());


                ele.ImgPath = dr["IMGURL"].ToString();
                ele.ImgLinkUrl = dr["LINKURL"].ToString();
                ele.ImgLinkTarget = dr["LINKTARGET"].ToString();
                ele.ImgPath = dr["LINKURL"].ToString();


                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                    ele.DirectInsert();
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmEle WHERE MyPK='" + str + "'";
            }

            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 保存Lab
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Lab(string fk_mapdata, DataTable dt, string elePKs)
        {
            string lines = "";
            FrmEle ele = new FrmEle();
            foreach (DataRow dr in dt.Rows)
            {
                ele.EleType = "Lab";
                ele.FK_MapData = fk_mapdata;
                ele.X = int.Parse( dr["X"].ToString());
                ele.Y = int.Parse(dr["Y"].ToString());
                ele.LabText = dr["Text"].ToString();
                ele.LabStyle = dr["FontStyle"].ToString();

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                    ele.DirectInsert();
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmEle WHERE MyPK='" + str + "'";
            }

            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 保存Btn
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Btn(string fk_mapdata, DataTable dt, string elePKs)
        {
            string lines = "";
            FrmEle ele = new FrmEle();
            foreach (DataRow dr in dt.Rows)
            {
                ele.EleType = "Btn";
                ele.FK_MapData = fk_mapdata;

                ele.BtnText = dr["TEXT"].ToString();
                ele.BtnType = int.Parse( dr["EVENTTYPE"].ToString());
                ele.BtnEventContext = dr["EVENTCONTEXT"].ToString(); //执行内容.

                ele.X = int.Parse(dr["X"].ToString());
                ele.Y = int.Parse(dr["Y"].ToString());

                if (dr["IsEnable"].ToString() == "1")
                    ele.IsEnable = true;
                else
                    ele.IsEnable = false;

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                    ele.DirectInsert();
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmEle WHERE MyPK='" + str + "'";
            }

            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        /// <summary>
        /// 保存Link
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="dt">数据</param>
        /// <param name="elePKs">主键s</param>
        private static void SaveCCForm20_Link(string fk_mapdata, DataTable dt, string elePKs)
        {
            string lines = "";
            FrmEle ele = new FrmEle();
            foreach (DataRow dr in dt.Rows)
            {
                ele.EleType = "Link";
                ele.FK_MapData = fk_mapdata;

                ele.X = int.Parse(dr["X"].ToString());
                ele.Y = int.Parse(dr["Y"].ToString());

                ele.LinkText = dr["Text"].ToString();
                ele.LinkStyle = dr["FontStyle"].ToString();

                ele.LinkURL = dr["URL"].ToString();
                ele.LinkTarget = dr["Target"].ToString();

                if (elePKs.Contains(ele.MyPK + ",") == true)
                {
                    elePKs = elePKs.Replace(ele.MyPK + ",", "");
                    ele.DirectUpdate();
                }
                else
                    ele.DirectInsert();
            }

            //xxxx
            string[] strs = elePKs.Split(',');
            string sqls = "";
            foreach (string str in strs)
            {
                if (string.IsNullOrEmpty(str))
                    continue;
                sqls += "@DELETE FROM Sys_FrmEle WHERE MyPK='" + str + "'";
            }
            if (sqls != "")
                BP.DA.DBAccess.RunSQLs(sqls);
        }
        #endregion 保存 frmEle控件。
       
        #endregion 执行 ccform2.0 的保存.

        /// <summary>
        /// 升级逻辑.
        /// </summary>
        public void Upgrade()
        {
            string sql = "";
            #region 升级ccform控件.
            if (BP.DA.DBAccess.IsExitsObject("Sys_FrmLine") == true)
            {
                //重命名.
                BP.Sys.SFDBSrc dbsrc = new SFDBSrc("local");
                dbsrc.Rename("Table", "Sys_FrmLine", "Sys_FrmLineBak");

                /*检查是否升级.*/
                sql = "SELECT * FROM Sys_FrmLineBak ORDER BY FK_MapData ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    BP.Sys.FrmEle ele = new FrmEle();
                    ele.Copy(dr);
                    ele.EleType = BP.Sys.FrmEle.Line;
                    //ele.BorderColor = dr["BorderColor"].ToString();
                    //ele.BorderWidth = int.Parse(dr["BorderWidth"].ToString());
                    if (ele.IsExits == true)
                        ele.MyPK = BP.DA.DBAccess.GenerGUID();
                    ele.Insert();
                }
            }
            if (BP.DA.DBAccess.IsExitsObject("Sys_FrmLab") == true)
            {
                //重命名.
                BP.Sys.SFDBSrc dbsrc = new SFDBSrc("local");
                dbsrc.Rename("Table", "Sys_FrmLab", "Sys_FrmLabBak");

                /*检查是否升级.*/
                sql = "SELECT * FROM Sys_FrmLabBak ORDER BY FK_MapData ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    BP.Sys.FrmEle ele = new FrmEle();
                    ele.Copy(dr);
                    ele.EleType = BP.Sys.FrmEle.Label;

                    ele.EleName = dr[FrmLabAttr.Text].ToString();

                    //ele.FontColor = dr[FrmLabAttr.FontColor].ToString();
                    //ele.FontName = dr[FrmLabAttr.FontName].ToString();
                    //ele.FontSize = int.Parse(dr[FrmLabAttr.FontSize].ToString());
                    //ele.BorderWidth = int.Parse(dr["BorderWidth"].ToString());

                    if (ele.IsExits == true)
                        ele.MyPK = BP.DA.DBAccess.GenerGUID();
                    ele.Insert();
                }
            }
            if (BP.DA.DBAccess.IsExitsObject("Sys_FrmBtn") == true)
            {
                //重命名.
                BP.Sys.SFDBSrc dbsrc = new SFDBSrc("local");
                dbsrc.Rename("Table", "Sys_FrmLab", "Sys_FrmLabBak");

                /*检查是否升级.*/
                sql = "SELECT * FROM Sys_FrmLabBak ORDER BY FK_MapData ";
                DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);
                foreach (DataRow dr in dt.Rows)
                {
                    BP.Sys.FrmEle ele = new FrmEle();
                    ele.Copy(dr);
                    ele.EleType = BP.Sys.FrmEle.Line;
                    //ele.BorderColor = dr["BorderColor"].ToString();
                    //ele.BorderWidth = int.Parse(dr["BorderWidth"].ToString());
                    if (ele.IsExits == true)
                        ele.MyPK = BP.DA.DBAccess.GenerGUID();
                    ele.Insert();
                }
            }
            #endregion 升级ccform控件.
        }

        #region weboffice文档属性(参数属性)
        /// <summary>
        /// 是否启用锁定行
        /// </summary>
        public bool IsRowLock
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsRowLock, false);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsRowLock, value);
            }
        }

        /// <summary>
        /// 是否启用打印
        /// </summary>
        public bool IsWoEnablePrint
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnablePrint);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnablePrint, value);
            }
        }
        /// <summary>
        /// 是否启用只读
        /// </summary>
        public bool IsWoEnableReadonly
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableReadonly);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableReadonly, value);
            }
        }
        /// <summary>
        /// 是否启用修订
        /// </summary>
        public bool IsWoEnableRevise
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableRevise);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableRevise, value);
            }
        }
        /// <summary>
        /// 是否启用保存
        /// </summary>
        public bool IsWoEnableSave
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableSave);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableSave, value);
            }
        }
        /// <summary>
        /// 是否查看用户留痕
        /// </summary>
        public bool IsWoEnableViewKeepMark
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableViewKeepMark);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableViewKeepMark, value);
            }
        }
        /// <summary>
        /// 是否启用weboffice
        /// </summary>
        public bool IsWoEnableWF
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableWF);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableWF, value);
            }
        }

        /// <summary>
        /// 是否启用套红
        /// </summary>
        public bool IsWoEnableOver
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableOver);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableOver, value);
            }
        }

        /// <summary>
        /// 是否启用签章
        /// </summary>
        public bool IsWoEnableSeal
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableSeal);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableSeal, value);
            }
        }

        /// <summary>
        /// 是否启用公文模板
        /// </summary>
        public bool IsWoEnableTemplete
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableTemplete);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableTemplete, value);
            }
        }

        /// <summary>
        /// 是否记录节点信息
        /// </summary>
        public bool IsWoEnableCheck
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableCheck);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableCheck, value);
            }
        }

        /// <summary>
        /// 是否插入流程图
        /// </summary>
        public bool IsWoEnableInsertFlow
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableInsertFlow);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableInsertFlow, value);
            }
        }

        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public bool IsWoEnableInsertFengXian
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableInsertFengXian);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableInsertFengXian, value);
            }
        }

        /// <summary>
        /// 是否启用留痕模式
        /// </summary>
        public bool IsWoEnableMarks
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableMarks);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableMarks, value);
            }
        }

        /// <summary>
        /// 是否插入风险点
        /// </summary>
        public bool IsWoEnableDown
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsWoEnableDown);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsWoEnableDown, value);
            }
        }

        #endregion weboffice文档属性

        #region 自动计算属性.
        public float MaxLeft
        {
            get
            {
                return this.GetParaFloat(MapDataAttr.MaxLeft);
            }
            set
            {
                this.SetPara(MapDataAttr.MaxLeft, value);
            }
        }
        public float MaxRight
        {
            get
            {
                return this.GetParaFloat(MapDataAttr.MaxRight);
            }
            set
            {
                this.SetPara(MapDataAttr.MaxRight, value);
            }
        }
        public float MaxTop
        {
            get
            {
                return this.GetParaFloat(MapDataAttr.MaxTop);
            }
            set
            {
                this.SetPara(MapDataAttr.MaxTop, value);
            }
        }
        public float MaxEnd
        {
            get
            {
                return this.GetParaFloat(MapDataAttr.MaxEnd);
            }
            set
            {
                this.SetPara(MapDataAttr.MaxEnd, value);
            }
        }
        #endregion 自动计算属性.

        #region 报表属性(参数方式存储).
        /// <summary>
        /// 是否关键字查询
        /// </summary>
        public bool RptIsSearchKey
        {
            get
            {
                return this.GetParaBoolen(MapDataAttr.RptIsSearchKey, true);
            }
            set
            {
                this.SetPara(MapDataAttr.RptIsSearchKey, value);
            }
        }
        /// <summary>
        /// 时间段查询方式
        /// </summary>
        public DTSearchWay RptDTSearchWay
        {
            get
            {
                return (DTSearchWay)this.GetParaInt(MapDataAttr.RptDTSearchWay);
            }
            set
            {
                this.SetPara(MapDataAttr.RptDTSearchWay, (int)value);
            }
        }
        /// <summary>
        /// 时间字段
        /// </summary>
        public string RptDTSearchKey
        {
            get
            {
                return this.GetParaString(MapDataAttr.RptDTSearchKey);
            }
            set
            {
                this.SetPara(MapDataAttr.RptDTSearchKey, value);
            }
        }
        /// <summary>
        /// 查询外键枚举字段
        /// </summary>
        public string RptSearchKeys
        {
            get
            {
                return this.GetParaString(MapDataAttr.RptSearchKeys, "*");
            }
            set
            {
                this.SetPara(MapDataAttr.RptSearchKeys, value);
            }
        }
        #endregion 报表属性(参数方式存储).

        #region 外键属性
        public string Ver
        {
            get
            {
                return this.GetValStringByKey(MapDataAttr.Ver);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Ver, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.Idx);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Idx, value);
            }
        }
        /// <summary>
        /// 扩展控件
        /// </summary>
        public FrmEles HisFrmEles
        {
            get
            {
                FrmEles obj = this.GetRefObject("FrmEles") as FrmEles;
                if (obj == null)
                {
                    obj = new FrmEles(this.No);
                    this.SetRefObject("FrmEles", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 框架
        /// </summary>
        public MapFrames MapFrames
        {
            get
            {
                MapFrames obj = this.GetRefObject("MapFrames") as MapFrames;
                if (obj == null)
                {
                    obj = new MapFrames(this.No);
                    this.SetRefObject("MapFrames", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 分组字段
        /// </summary>
        public GroupFields GroupFields
        {
            get
            {
                GroupFields obj = this.GetRefObject("GroupFields") as GroupFields;
                if (obj == null)
                {
                    obj = new GroupFields(this.No);
                    this.SetRefObject("GroupFields", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 逻辑扩展
        /// </summary>
        public MapExts MapExts
        {
            get
            {
                MapExts obj = this.GetRefObject("MapExts") as MapExts;
                if (obj == null)
                {
                    obj = new MapExts(this.No);
                    this.SetRefObject("MapExts", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 事件
        /// </summary>
        public FrmEvents FrmEvents
        {
            get
            {
                FrmEvents obj = this.GetRefObject("FrmEvents") as FrmEvents;
                if (obj == null)
                {
                    obj = new FrmEvents(this.No);
                    this.SetRefObject("FrmEvents", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 一对多
        /// </summary>
        public MapM2Ms MapM2Ms
        {
            get
            {
                MapM2Ms obj = this.GetRefObject("MapM2Ms") as MapM2Ms;
                if (obj == null)
                {
                    obj = new MapM2Ms(this.No);
                    this.SetRefObject("MapM2Ms", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 从表
        /// </summary>
        public MapDtls MapDtls
        {
            get
            {
                MapDtls obj = this.GetRefObject("MapDtls") as MapDtls;
                if (obj == null)
                {
                    obj = new MapDtls(this.No);
                    this.SetRefObject("MapDtls", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 报表
        /// </summary>
        public FrmRpts FrmRpts
        {
            get
            {
                FrmRpts obj = this.GetRefObject("FrmRpts") as FrmRpts;
                if (obj == null)
                {
                    obj = new FrmRpts(this.No);
                    this.SetRefObject("FrmRpts", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 超连接
        /// </summary>
        public FrmLinks FrmLinks
        {
            get
            {
                FrmLinks obj = this.GetRefObject("FrmLinks") as FrmLinks;
                if (obj == null)
                {
                    obj = new FrmLinks(this.No);
                    this.SetRefObject("FrmLinks", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 按钮
        /// </summary>
        public FrmBtns FrmBtns
        {
            get
            {
                FrmBtns obj = this.GetRefObject("FrmLinks") as FrmBtns;
                if (obj == null)
                {
                    obj = new FrmBtns(this.No);
                    this.SetRefObject("FrmBtns", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 元素
        /// </summary>
        public FrmEles FrmEles
        {
            get
            {
                FrmEles obj = this.GetRefObject("FrmEles") as FrmEles;
                if (obj == null)
                {
                    obj = new FrmEles(this.No);
                    this.SetRefObject("FrmEles", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 线
        /// </summary>
        public FrmLines FrmLines
        {
            get
            {
                FrmLines obj = this.GetRefObject("FrmLines") as FrmLines;
                if (obj == null)
                {
                    obj = new FrmLines(this.No);
                    this.SetRefObject("FrmLines", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public FrmLabs FrmLabs
        {
            get
            {
                FrmLabs obj = this.GetRefObject("FrmLabs") as FrmLabs;
                if (obj == null)
                {
                    obj = new FrmLabs(this.No);
                    this.SetRefObject("FrmLabs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片
        /// </summary>
        public FrmImgs FrmImgs
        {
            get
            {
                FrmImgs obj = this.GetRefObject("FrmLabs") as FrmImgs;
                if (obj == null)
                {
                    obj = new FrmImgs(this.No);
                    this.SetRefObject("FrmLabs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachments FrmAttachments
        {
            get
            {
                FrmAttachments obj = this.GetRefObject("FrmAttachments") as FrmAttachments;
                if (obj == null)
                {
                    obj = new FrmAttachments(this.No);
                    this.SetRefObject("FrmAttachments", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 图片附件
        /// </summary>
        public FrmImgAths FrmImgAths
        {
            get
            {
                FrmImgAths obj = this.GetRefObject("FrmImgAths") as FrmImgAths;
                if (obj == null)
                {
                    obj = new FrmImgAths(this.No);
                    this.SetRefObject("FrmImgAths", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 单选按钮
        /// </summary>
        public FrmRBs FrmRBs
        {
            get
            {
                FrmRBs obj = this.GetRefObject("FrmRBs") as FrmRBs;
                if (obj == null)
                {
                    obj = new FrmRBs(this.No);
                    this.SetRefObject("FrmRBs", obj);
                }
                return obj;
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public MapAttrs MapAttrs
        {
            get
            {
                MapAttrs obj = this.GetRefObject("MapAttrs") as MapAttrs;
                if (obj == null)
                {
                    obj = new MapAttrs(this.No);
                    this.SetRefObject("MapAttrs", obj);
                }
                return obj;
            }
        }
        #endregion

        public static Boolean IsEditDtlModel
        {
            get
            {
                string s = BP.Web.WebUser.GetSessionByKey("IsEditDtlModel", "0");
                if (s == "0")
                    return false;
                else
                    return true;
            }
            set
            {
                BP.Web.WebUser.SetSessionByKey("IsEditDtlModel", "1");
            }
        }

        #region 表单结构数据json
        /// <summary>
        /// 表单图数据
        /// </summary>
        public string FormJson
        {
            get
            {
                return this.GetBigTextFromDB("FormJson");
            }
            set
            {
                this.SaveBigTxtToDB("FormJson", value);
            }
        }
        #endregion

        #region 属性
        /// <summary>
        /// 物理表
        /// </summary>
        public string PTable
        {
            get
            {
                string s = this.GetValStrByKey(MapDataAttr.PTable);
                if (s == "" || s == null)
                    return this.No;
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.PTable, value);
            }
        }
        /// <summary>
        /// URL
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.Url);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Url, value);
            }
        }
        public DBUrlType HisDBUrl
        {
            get
            {
                return DBUrlType.AppCenterDSN;
                // return (DBUrlType)this.GetValIntByKey(MapDataAttr.DBURL);
            }
        }
        public int HisFrmTypeInt
        {
            get
            {
                return this.GetValIntByKey(MapDataAttr.FrmType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmType, value);
            }
        }
        public FrmType HisFrmType
        {
            get
            {
                return (FrmType)this.GetValIntByKey(MapDataAttr.FrmType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmType, (int)value);
            }
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Note
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.Note);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Note, value);
            }
        }
        /// <summary>
        /// 是否有CA.
        /// </summary>
        public bool IsHaveCA
        {
            get
            {
                return this.GetParaBoolen("IsHaveCA", false);

            }
            set
            {
                this.SetPara("IsHaveCA", value);
            }
        }
        /// <summary>
        /// 类别，可以为空.
        /// </summary>
        public string FK_FrmSort
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FK_FrmSort);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FK_FrmSort, value);
            }
        }
        /// <summary>
        /// 数据源
        /// </summary>
        public string DBSrc
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.DBSrc);
            }
            set
            {
                this.SetValByKey(MapDataAttr.DBSrc, value);
            }
        }

        /// <summary>
        /// 类别，可以为空.
        /// </summary>
        public string FK_FormTree
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.FK_FormTree);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FK_FormTree, value);
            }
        }
        /// <summary>
        /// 从表集合.
        /// </summary>
        public string Dtls
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.Dtls);
            }
            set
            {
                this.SetValByKey(MapDataAttr.Dtls, value);
            }
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string EnPK
        {
            get
            {
                string s = this.GetValStrByKey(MapDataAttr.EnPK);
                if (string.IsNullOrEmpty(s))
                    return "OID";
                return s;
            }
            set
            {
                this.SetValByKey(MapDataAttr.EnPK, value);
            }
        }
        public Entities _HisEns = null;
        public new Entities HisEns
        {
            get
            {
                if (_HisEns == null)
                {
                    _HisEns = BP.En.ClassFactory.GetEns(this.No);
                }
                return _HisEns;
            }
        }
        public Entity HisEn
        {
            get
            {
                return this.HisEns.GetNewEntity;
            }
        }
        public float FrmW
        {
            get
            {
                return this.GetValFloatByKey(MapDataAttr.FrmW);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmW, value);
            }
        }
        ///// <summary>
        ///// 表单控制方案
        ///// </summary>
        //public string Slns
        //{
        //    get
        //    {
        //        return this.GetValStringByKey(MapDataAttr.Slns);
        //    }
        //    set
        //    {
        //        this.SetValByKey(MapDataAttr.Slns, value);
        //    }
        //}
        public float FrmH
        {
            get
            {
                return this.GetValFloatByKey(MapDataAttr.FrmH);
            }
            set
            {
                this.SetValByKey(MapDataAttr.FrmH, value);
            }
        }
        /// <summary>
        /// 表格显示的列
        /// </summary>
        public int TableCol
        {
            get
            {
                int i = this.GetValIntByKey(MapDataAttr.TableCol);
                if (i == 0 || i == 1)
                    return 4;
                return i;
            }
            set
            {
                this.SetValByKey(MapDataAttr.TableCol, value);
            }
        }
        public string TableWidth
        {
            get
            {
                //switch (this.TableCol)
                //{
                //    case 2:
                //        return
                //        labCol = 25;
                //        ctrlCol = 75;
                //        break;
                //    case 4:
                //        labCol = 20;
                //        ctrlCol = 30;
                //        break;
                //    case 6:
                //        labCol = 15;
                //        ctrlCol = 30;
                //        break;
                //    case 8:
                //        labCol = 10;
                //        ctrlCol = 15;
                //        break;
                //    default:
                //        break;
                //}


                int i = this.GetValIntByKey(MapDataAttr.TableWidth);
                if (i <= 50)
                    return "100%";
                return i + "px";
            }
        }
        /// <summary>
        /// 应用类型.  0独立表单.1节点表单
        /// </summary>
        public string AppType
        {
            get
            {
                return this.GetValStrByKey(MapDataAttr.AppType);
            }
            set
            {
                this.SetValByKey(MapDataAttr.AppType, value);
            }
        }
        /// <summary>
        /// 表单body属性.
        /// </summary>
        public string BodyAttr
        {
            get
            {
                string str= this.GetValStrByKey(MapDataAttr.BodyAttr);
                str = str.Replace("~", "'");
                return str;
            }
            set
            {
                this.SetValByKey(MapDataAttr.BodyAttr, value);
            }
        }
        #endregion

        #region 构造方法
        public Map GenerHisMap()
        {
            MapAttrs mapAttrs = this.MapAttrs;
            if (mapAttrs.Count == 0)
            {
                this.RepairMap();
                mapAttrs = this.MapAttrs;
            }

            Map map = new Map(this.PTable);
            map.EnDBUrl = new DBUrl(this.HisDBUrl); ;
            map.EnDesc = this.Name;
            map.EnType = EnType.App;
            map.DepositaryOfEntity = Depositary.None;
            map.DepositaryOfMap = Depositary.Application;

            Attrs attrs = new Attrs();
            foreach (MapAttr mapAttr in mapAttrs)
                map.AddAttr(mapAttr.HisAttr);

            // 产生从表。
            MapDtls dtls = this.MapDtls; // new MapDtls(this.No);
            foreach (MapDtl dtl in dtls)
            {
                GEDtls dtls1 = new GEDtls(dtl.No);
                map.AddDtl(dtls1, "RefPK");
            }

            #region 查询条件.
            map.IsShowSearchKey = this.RptIsSearchKey; //是否启用关键字查询.
            // 按日期查询.
            map.DTSearchWay = this.RptDTSearchWay; //日期查询方式.
            map.DTSearchKey = this.RptDTSearchKey; //日期字段.

            //加入外键查询字段.
            string[] keys = this.RptSearchKeys.Split('*');
            foreach (string key in keys)
            {
                if (string.IsNullOrEmpty(key))
                    continue;

                map.AddSearchAttr(key);
            }
            #endregion 查询条件.

            return map;
        }
        private GEEntity _HisEn = null;
        public GEEntity HisGEEn
        {
            get
            {
                if (this._HisEn == null)
                    _HisEn = new GEEntity(this.No);
                return _HisEn;
            }
        }
        /// <summary>
        /// 生成实体
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public GEEntity GenerGEEntityByDataSet(DataSet ds)
        {
            // New 它的实例.
            GEEntity en = this.HisGEEn;

            // 它的table.
            DataTable dt = ds.Tables[this.No];

            //装载数据.
            en.Row.LoadDataTable(dt, dt.Rows[0]);

            // dtls.
            MapDtls dtls = this.MapDtls;
            foreach (MapDtl item in dtls)
            {
                DataTable dtDtls = ds.Tables[item.No];
                GEDtls dtlsEn = new GEDtls(item.No);
                foreach (DataRow dr in dtDtls.Rows)
                {
                    // 产生它的Entity data.
                    GEDtl dtl = (GEDtl)dtlsEn.GetNewEntity;
                    dtl.Row.LoadDataTable(dtDtls, dr);

                    //加入这个集合.
                    dtlsEn.AddEntity(dtl);
                }

                //加入到他的集合里.
                en.Dtls.Add(dtDtls);
            }
            return en;
        }
        /// <summary>
        /// 生成map.
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public static Map GenerHisMap(string no)
        {
            if (SystemConfig.IsDebug)
            {
                MapData md = new MapData();
                md.No = no;
                md.Retrieve();
                return md.GenerHisMap();
            }
            else
            {
                Map map = BP.DA.Cash.GetMap(no);
                if (map == null)
                {
                    MapData md = new MapData();
                    md.No = no;
                    md.Retrieve();
                    map = md.GenerHisMap();
                    BP.DA.Cash.SetMap(no, map);
                }
                return map;
            }
        }
        /// <summary>
        /// 映射基础
        /// </summary>
        public MapData()
        {
        }
        /// <summary>
        /// 映射基础
        /// </summary>
        /// <param name="no">映射编号</param>
        public MapData(string no)
            : base(no)
        {
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_MapData");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "表单注册表";
                map.EnType = EnType.Sys;
                map.CodeStruct = "4";

                #region 基础信息.
                map.AddTBStringPK(MapDataAttr.No, null, "编号", true, false, 1, 200, 100);
                map.AddTBString(MapDataAttr.Name, null, "描述", true, false, 0, 500, 20);

                map.AddTBString(MapDataAttr.EnPK, null, "实体主键", true, false, 0, 200, 20);
                map.AddTBString(MapDataAttr.PTable, null, "物理表", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.Url, null, "连接(对嵌入式表单有效)", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.Dtls, null, "从表", true, false, 0, 500, 20);

                //格式为: @1=方案名称1@2=方案名称2@3=方案名称3
                //map.AddTBString(MapDataAttr.Slns, null, "表单控制解决方案", true, false, 0, 500, 20);

                map.AddTBInt(MapDataAttr.FrmW, 900, "FrmW", true, true);
                map.AddTBInt(MapDataAttr.FrmH, 1200, "FrmH", true, true);

                map.AddTBInt(MapDataAttr.TableCol, 4, "傻瓜表单显示的列", true, true);
                map.AddTBInt(MapDataAttr.TableWidth, 600, "表格宽度", true, true);

                //Tag
                map.AddTBString(MapDataAttr.Tag, null, "Tag", true, false, 0, 500, 20);

                // 可以为空这个字段。
                map.AddTBString(MapDataAttr.FK_FrmSort, null, "表单类别", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.FK_FormTree, null, "表单树类别", true, false, 0, 500, 20);

                // enumFrmType  @自由表单，@傻瓜表单，@嵌入式表单.
                map.AddTBInt(MapDataAttr.FrmType, 1, "表单类型", true, false);


                // 应用类型.  0独立表单.1节点表单
                map.AddTBInt(MapDataAttr.AppType, 0, "应用类型", true, false);


                map.AddTBString(MapDataAttr.DBSrc, "local", "数据源", true, false, 0, 100, 20);

                map.AddTBString(MapDataAttr.BodyAttr, null, "表单Body属性", true, false, 0, 100, 20);

                #endregion 基础信息.

                #region 设计者信息.
                map.AddTBString(MapDataAttr.Note, null, "备注", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);

                //增加参数字段.
                map.AddTBAtParas(4000);

                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", true, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, false, 0, 128, 20);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, false, 0, 30, 20);
                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }

        /// <summary>
        /// 上移
        /// </summary>
        public void DoUp()
        {
            this.DoOrderUp(MapDataAttr.FK_FormTree, this.FK_FormTree, MapDataAttr.Idx);
        }
        /// <summary>
        /// 下移
        /// </summary>
        public void DoOrderDown()
        {
            this.DoOrderDown(MapDataAttr.FK_FormTree, this.FK_FormTree, MapDataAttr.Idx);
        }
        #endregion

        /// <summary>
        /// 导入
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static MapData ImpMapData(DataSet ds)
        {
            return ImpMapData(ds, true);
        }
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="isSetReadony"></param>
        /// <returns></returns>
        public static MapData ImpMapData(DataSet ds, bool isSetReadony)
        {
            string errMsg = "";
            if (ds.Tables.Contains("WF_Flow") == true)
                errMsg += "@此模板文件为流程模板。";

            if (ds.Tables.Contains("Sys_MapAttr") == false)
                errMsg += "@缺少表:Sys_MapAttr";

            if (ds.Tables.Contains("Sys_MapData") == false)
                errMsg += "@缺少表:Sys_MapData";

            if (errMsg != "")
                throw new Exception(errMsg);

            DataTable dt = ds.Tables["Sys_MapData"];
            string fk_mapData = dt.Rows[0]["No"].ToString();
            MapData md = new MapData();
            md.No = fk_mapData;
            if (md.IsExits)
                throw new Exception("已经存在(" + fk_mapData + ")的数据。");

            //导入.
            return ImpMapData(fk_mapData, ds, isSetReadony);
        }
        /// <summary>
        /// 导入表单
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ds">表单数据</param>
        /// <param name="isSetReadonly">是否设置只读？</param>
        /// <returns></returns>
        public static MapData ImpMapData(string fk_mapdata, DataSet ds, bool isSetReadonly)
        {

            #region 检查导入的数据是否完整.
            string errMsg = "";
            //if (ds.Tables[0].TableName != "Sys_MapData")
            //    errMsg += "@非表单模板。";

            if (ds.Tables.Contains("WF_Flow") == true)
                errMsg += "@此模板文件为流程模板。";

            if (ds.Tables.Contains("Sys_MapAttr") == false)
                errMsg += "@缺少表:Sys_MapAttr";

            if (ds.Tables.Contains("Sys_MapData") == false)
                errMsg += "@缺少表:Sys_MapData";

            DataTable dtCheck = ds.Tables["Sys_MapAttr"];
            bool isHave = false;
            foreach (DataRow dr in dtCheck.Rows)
            {
                if (dr["KeyOfEn"].ToString() == "OID")
                {
                    isHave = true;
                    break;
                }
            }

            if (isHave == false)
                errMsg += "@表单模版缺少列:OID";

            if (errMsg != "")
                throw new Exception("@以下错误不可导入，可能的原因是非表单模板文件:" + errMsg);
            #endregion

            // 定义在最后执行的sql.
            string endDoSQL = "";

            //检查是否存在OID字段.
            MapData mdOld = new MapData();
            mdOld.No = fk_mapdata;
            mdOld.RetrieveFromDBSources();
            mdOld.Delete();

            // 求出dataset的map.
            string oldMapID = "";
            DataTable dtMap = ds.Tables["Sys_MapData"];
            if (dtMap.Rows.Count == 1)
            {
                oldMapID = dtMap.Rows[0]["No"].ToString();
            }
            else
            {
                // 求旧的表单ID.
                foreach (DataRow dr in dtMap.Rows)
                {
                    oldMapID = dr["No"].ToString();
                }

                if (string.IsNullOrEmpty(oldMapID) == true)
                {
                    oldMapID = dtMap.Rows[0]["No"].ToString();
                }
                //  throw new Exception("@没有找到 oldMapID ");
            }

            string timeKey = DateTime.Now.ToString("MMddHHmmss");
            // string timeKey = fk_mapdata;
            #region 表单元素
            foreach (DataTable dt in ds.Tables)
            {
                int idx = 0;
                switch (dt.TableName)
                {
                    case "Sys_MapDtl":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapDtl dtl = new MapDtl();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                dtl.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            if (isSetReadonly)
                            {
                                //dtl.IsReadonly = true;

                                dtl.IsInsert = false;
                                dtl.IsUpdate = false;
                                dtl.IsDelete = false;
                            }

                            dtl.Insert();
                        }
                        break;
                    case "Sys_MapData":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapData md = new MapData();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                md.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                                //md.SetValByKey(dc.ColumnName, val);
                            }

                            //如果物理表为空，则使用编号为物理数据表
                            if (string.IsNullOrEmpty(md.PTable.Trim()) == true)
                                md.PTable = md.No;

                            //表单类别编号不为空，则用原表单类别编号
                            if (string.IsNullOrEmpty(mdOld.FK_FormTree) == false)
                                md.FK_FormTree = mdOld.FK_FormTree;

                            //表单类别编号不为空，则用原表单类别编号
                            if (string.IsNullOrEmpty(mdOld.FK_FrmSort) == false)
                                md.FK_FrmSort = mdOld.FK_FrmSort;

                            if (string.IsNullOrEmpty(mdOld.PTable) == false)
                                md.PTable = mdOld.PTable;
                            if (string.IsNullOrEmpty(mdOld.Name) == false)
                                md.Name = mdOld.Name;

                            md.HisFrmType = mdOld.HisFrmType;
                            //表单应用类型保持不变
                            md.AppType = mdOld.AppType;

                            md.DirectInsert();
                        }
                        break;
                    case "Sys_FrmBtn":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmBtn en = new FrmBtn();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            if (isSetReadonly == true)
                                en.IsEnable = false;


                            en.MyPK = "Btn_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLine":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLine en = new FrmLine();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "LE_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLab":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLab en = new FrmLab();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;


                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            //  en.FK_MapData = fk_mapdata; 删除此行解决从表lab的问题。
                            en.MyPK = "LB_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmLink":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmLink en = new FrmLink();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "LK_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmEle":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmEle en = new FrmEle();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            if (isSetReadonly == true)
                                en.IsEnable = false;

                            en.Insert();
                        }
                        break;
                    case "Sys_FrmImg":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImg en = new FrmImg();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;



                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "Img_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_FrmImgAth":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmImgAth en = new FrmImgAth();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            if (string.IsNullOrEmpty(en.CtrlID))
                                en.CtrlID = "ath" + idx;

                            en.Insert();
                        }
                        break;
                    case "Sys_FrmRB":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmRB en = new FrmRB();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }


                            try
                            {
                                en.Save();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "Sys_FrmAttachment":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            FrmAttachment en = new FrmAttachment();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.MyPK = "Ath_" + idx + "_" + fk_mapdata;
                            if (isSetReadonly == true)
                            {
                                en.IsDeleteInt = 0;
                                en.IsUpload = false;
                            }

                            try
                            {
                                en.Insert();
                            }
                            catch
                            {
                            }
                        }
                        break;
                    case "Sys_MapM2M":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapM2M en = new MapM2M();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            //   en.NoOfObj = "M2M_" + idx + "_" + fk_mapdata;
                            if (isSetReadonly == true)
                            {
                                en.IsDelete = false;
                                en.IsInsert = false;
                            }
                            try
                            {
                                en.Insert();
                            }
                            catch
                            {
                                en.Update();
                            }
                        }
                        break;
                    case "Sys_MapFrame":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapFrame en = new MapFrame();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;


                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }
                            en.NoOfObj = "Fra_" + idx + "_" + fk_mapdata;
                            en.Insert();
                        }
                        break;
                    case "Sys_MapExt":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            MapExt en = new MapExt();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;
                                if (string.IsNullOrEmpty(val.ToString()) == true)
                                    continue;
                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            //执行保存，并统一生成PK的规则.
                            en.InitPK();
                            en.Save();
                        }
                        break;
                    case "Sys_MapAttr":
                        foreach (DataRow dr in dt.Rows)
                        {
                            MapAttr en = new MapAttr();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                            }

                            if (isSetReadonly == true)
                            {
                                if (en.DefValReal != null
                                    && (en.DefValReal.Contains("@WebUser.")
                                    || en.DefValReal.Contains("@RDT")))
                                    en.DefValReal = "";

                                switch (en.UIContralType)
                                {
                                    case UIContralType.DDL:
                                        en.UIIsEnable = false;
                                        break;
                                    case UIContralType.TB:
                                        en.UIIsEnable = false;
                                        break;
                                    case UIContralType.RadioBtn:
                                        en.UIIsEnable = false;
                                        break;
                                    case UIContralType.CheckBok:
                                        en.UIIsEnable = false;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            en.MyPK = en.FK_MapData + "_" + en.KeyOfEn;
                            en.DirectInsert();
                        }
                        break;
                    case "Sys_GroupField":
                        foreach (DataRow dr in dt.Rows)
                        {
                            idx++;
                            GroupField en = new GroupField();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                object val = dr[dc.ColumnName] as object;
                                if (val == null)
                                    continue;

                                try
                                {
                                    en.SetValByKey(dc.ColumnName, val.ToString().Replace(oldMapID, fk_mapdata));
                                }
                                catch
                                {
                                    throw new Exception("val:" + val.ToString() + "oldMapID:" + oldMapID + "fk_mapdata:" + fk_mapdata);
                                }
                            }
                            int beforeID = en.OID;
                            en.OID = 0;
                            en.Insert();
                            endDoSQL += "@UPDATE Sys_MapAttr SET GroupID=" + en.OID + " WHERE FK_MapData='" + fk_mapdata + "' AND GroupID=" + beforeID;
                        }
                        break;
                    case "Sys_Enum":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnum se = new Sys.SysEnum();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                se.SetValByKey(dc.ColumnName, val);
                            }
                            se.MyPK = se.EnumKey + "_" + se.Lang + "_" + se.IntKey;
                            if (se.IsExits)
                                continue;
                            se.Insert();
                        }
                        break;
                    case "Sys_EnumMain":
                        foreach (DataRow dr in dt.Rows)
                        {
                            Sys.SysEnumMain sem = new Sys.SysEnumMain();
                            foreach (DataColumn dc in dt.Columns)
                            {
                                string val = dr[dc.ColumnName] as string;
                                if (val == null)
                                    continue;
                                sem.SetValByKey(dc.ColumnName, val);
                            }
                            if (sem.IsExits)
                                continue;
                            sem.Insert();
                        }
                        break;
                    case "WF_Node":
                        if (dt.Rows.Count > 0)
                        {
                            endDoSQL += "@UPDATE WF_Node SET FWCSta=2"
                                + ",FWC_X=" + dt.Rows[0]["FWC_X"]
                                + ",FWC_Y=" + dt.Rows[0]["FWC_Y"]
                                + ",FWC_H=" + dt.Rows[0]["FWC_H"]
                                + ",FWC_W=" + dt.Rows[0]["FWC_W"]
                                + ",FWCType=" + dt.Rows[0]["FWCType"]
                                + " WHERE NodeID=" + fk_mapdata.Replace("ND", "");
                        }
                        break;
                    default:
                        break;
                }
            }
            #endregion

            //执行最后结束的sql.
            DBAccess.RunSQLs(endDoSQL);

            MapData mdNew = new MapData(fk_mapdata);
            mdNew.RepairMap();
            mdNew.Update();
            return mdNew;
        }
        public void RepairMap()
        {
            GroupFields gfs = new GroupFields(this.No);
            if (gfs.Count == 0)
            {
                GroupField gf = new GroupField();
                gf.EnName = this.No;
                gf.Lab = this.Name;
                gf.Insert();

                string sqls = "";
                sqls += "@UPDATE Sys_MapDtl SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapAttr SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapFrame SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapM2M SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_FrmAttachment SET GroupID=" + gf.OID + " WHERE FK_MapData='" + this.No + "'";
                DBAccess.RunSQLs(sqls);
            }
            else
            {
                GroupField gfFirst = gfs[0] as GroupField;
                string sqls = "";
                sqls += "@UPDATE Sys_MapDtl SET GroupID=" + gfFirst.OID + "        WHERE  No   IN (SELECT X.No FROM (SELECT No FROM Sys_MapDtl WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "')) AS X ) AND FK_MapData='" + this.No + "'";
                sqls += "@UPDATE Sys_MapAttr SET GroupID=" + gfFirst.OID + "       WHERE  MyPK IN (SELECT X.MyPK FROM (SELECT MyPK FROM Sys_MapAttr       WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "') or GroupID is null) AS X) AND FK_MapData='" + this.No + "' ";
                sqls += "@UPDATE Sys_MapFrame SET GroupID=" + gfFirst.OID + "      WHERE  MyPK IN (SELECT X.MyPK FROM (SELECT MyPK FROM Sys_MapFrame      WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "')) AS X) AND FK_MapData='" + this.No + "' ";
                sqls += "@UPDATE Sys_MapM2M SET GroupID=" + gfFirst.OID + "        WHERE  MyPK IN (SELECT X.MyPK FROM (SELECT MyPK FROM Sys_MapM2M        WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "')) AS X) AND FK_MapData='" + this.No + "' ";
                sqls += "@UPDATE Sys_FrmAttachment SET GroupID=" + gfFirst.OID + " WHERE  MyPK IN (SELECT X.MyPK FROM (SELECT MyPK FROM Sys_FrmAttachment WHERE GroupID NOT IN (SELECT OID FROM Sys_GroupField WHERE EnName='" + this.No + "')) AS X) AND FK_MapData='" + this.No + "' ";

#warning 这些sql 对于Oracle 有问题，但是不影响使用.
                try
                {
                    DBAccess.RunSQLs(sqls);
                }
                catch
                {
                }
            }

            BP.Sys.MapAttr attr = new BP.Sys.MapAttr();
            if (this.EnPK == "OID")
            {
                if (attr.IsExit(MapAttrAttr.KeyOfEn, "OID", MapAttrAttr.FK_MapData, this.No) == false)
                {
                    attr.FK_MapData = this.No;
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
            if (this.EnPK == "No" || this.EnPK == "MyPK")
            {
                if (attr.IsExit(MapAttrAttr.KeyOfEn, this.EnPK, MapAttrAttr.FK_MapData, this.No) == false)
                {
                    attr.FK_MapData = this.No;
                    attr.KeyOfEn = this.EnPK;
                    attr.Name = this.EnPK;
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

            if (attr.IsExit(MapAttrAttr.KeyOfEn, "RDT", MapAttrAttr.FK_MapData, this.No) == false)
            {
                attr = new BP.Sys.MapAttr();
                attr.FK_MapData = this.No;
                attr.HisEditType = BP.En.EditType.UnDel;
                attr.KeyOfEn = "RDT";
                attr.Name = "更新时间";

                attr.MyDataType = BP.DA.DataType.AppDateTime;
                attr.UIContralType = UIContralType.TB;
                attr.LGType = FieldTypeS.Normal;
                attr.UIVisible = false;
                attr.UIIsEnable = false;
                attr.DefVal = "@RDT";
                attr.Tag = "1";
                attr.Insert();
            }

            //检查特殊UIBindkey丢失的问题.
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, this.No);
            foreach (MapAttr item in attrs)
            {
                if (item.LGType != FieldTypeS.Normal)
                {
                    if (string.IsNullOrEmpty(item.UIBindKey)==true)
                    {
                        item.LGType = FieldTypeS.Normal;
                        item.UIContralType = UIContralType.TB;
                        item.Update();
                    }
                }
            }

        }
        protected override bool beforeInsert()
        {
            this.PTable = PubClass.DealToFieldOrTableNames(this.PTable);
            return base.beforeInsert();
        }
        /// <summary>
        /// 设置Para 参数.
        /// </summary>
        public void ResetMaxMinXY()
        {
            if (this.HisFrmType != FrmType.FreeFrm)
                return;


            #region 计算最左边,与最右边的值。
            // 求最左边.
            float i1 = DBAccess.RunSQLReturnValFloat("SELECT MIN(X1) FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'", 0);
            if (i1 == 0) /*没有线，只有图片的情况下。*/
                i1 = DBAccess.RunSQLReturnValFloat("SELECT MIN(X) FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'", 0);

            float i2 = DBAccess.RunSQLReturnValFloat("SELECT MIN(X)  FROM Sys_FrmLab  WHERE FK_MapData='" + this.No + "'", 0);
            if (i1 > i2)
                this.MaxLeft = i2;
            else
                this.MaxLeft = i1;

            // 求最右边.
            i1 = DBAccess.RunSQLReturnValFloat("SELECT Max(X2) FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'", 0);
            if (i1 == 0)
            {
                /*没有线的情况，按照图片来计算。*/
                i1 = DBAccess.RunSQLReturnValFloat("SELECT Max(X+W) FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'", 0);
            }
            this.MaxRight = i1;

            // 求最top.
            i1 = DBAccess.RunSQLReturnValFloat("SELECT MIN(Y1) FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'", 0);
            i2 = DBAccess.RunSQLReturnValFloat("SELECT MIN(Y)  FROM Sys_FrmLab  WHERE FK_MapData='" + this.No + "'", 0);

            if (i1 > i2)
                this.MaxTop = i2;
            else
                this.MaxTop = i1;

            // 求最end.
            i1 = DBAccess.RunSQLReturnValFloat("SELECT Max(Y1) FROM Sys_FrmLine WHERE FK_MapData='" + this.No + "'", 0);
            /*小周鹏添加2014/10/23-----------------------START*/
            if (i1 == 0) /*没有线，只有图片的情况下。*/
                i1 = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H) FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'", 0);

            /*小周鹏添加2014/10/23-----------------------END*/
            i2 = DBAccess.RunSQLReturnValFloat("SELECT Max(Y)  FROM Sys_FrmLab  WHERE FK_MapData='" + this.No + "'", 0);
            if (i2 == 0)
                i2 = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H) FROM Sys_FrmImg WHERE FK_MapData='" + this.No + "'", 0);
            //求出最底部的 附件
            float endFrmAtt = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H)  FROM Sys_FrmAttachment  WHERE FK_MapData='" + this.No + "'", 0);
            //求出最底部的明细表
            float endFrmDtl = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H)  FROM Sys_MapDtl  WHERE FK_MapData='" + this.No + "'", 0);

            //求出最底部的扩展控件
            float endFrmEle = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+H)  FROM Sys_FrmEle  WHERE FK_MapData='" + this.No + "'", 0);
            //求出最底部的textbox
            float endFrmAttr = DBAccess.RunSQLReturnValFloat("SELECT Max(Y+UIHeight)  FROM  Sys_MapAttr  WHERE FK_MapData='" + this.No + "' and UIVisible='1'", 0);

            if (i1 > i2)
                this.MaxEnd = i1;
            else
                this.MaxEnd = i2;

            this.MaxEnd = this.MaxEnd > endFrmAtt ? this.MaxEnd : endFrmAtt;
            this.MaxEnd = this.MaxEnd > endFrmDtl ? this.MaxEnd : endFrmDtl;
            this.MaxEnd = this.MaxEnd > endFrmEle ? this.MaxEnd : endFrmEle;
            this.MaxEnd = this.MaxEnd > endFrmAtt ? this.MaxEnd : endFrmAttr;

            #endregion

            this.DirectUpdate();
        }

        /// <summary>
        /// 求位移.
        /// </summary>
        /// <param name="md"></param>
        /// <param name="scrWidth"></param>
        /// <returns></returns>
        public static float GenerSpanWeiYi(MapData md, float scrWidth)
        {
            if (scrWidth == 0)
                scrWidth = 900;

            float left = md.MaxLeft;
            if (left == 0)
            {
                md.ResetMaxMinXY();
                md.RetrieveFromDBSources();
                md.Retrieve();

                left = md.MaxLeft;
            }
            //取不到左侧参考值，则不进行位移
            if (left == 0)
                return left;

            float right = md.MaxRight;
            float withFrm = right - left;
            if (withFrm >= scrWidth)
            {
                /* 如果实际表单宽度大于屏幕宽度 */
                return -left;
            }

            //计算位移大小
            float space = (scrWidth - withFrm) / 2; //空白的地方.

            return -(left - space);
        }
        /// <summary>
        /// 求屏幕宽度
        /// </summary>
        /// <param name="md"></param>
        /// <param name="scrWidth"></param>
        /// <returns></returns>
        public static float GenerSpanWidth(MapData md, float scrWidth)
        {
            if (scrWidth == 0)
                scrWidth = 900;
            float left = md.MaxLeft;
            if (left == 0)
            {
                md.ResetMaxMinXY();
                left = md.MaxLeft;
            }

            float right = md.MaxRight;
            float withFrm = right - left;
            if (withFrm >= scrWidth)
            {
                return withFrm;
            }
            return scrWidth;
        }
        /// <summary>
        /// 求屏幕高度
        /// </summary>
        /// <param name="md"></param>
        /// <param name="scrWidth"></param>
        /// <returns></returns>
        public static float GenerSpanHeight(MapData md, float scrHeight)
        {
            if (scrHeight == 0)
                scrHeight = 1200;

            float end = md.MaxEnd;
            if (end > scrHeight)
                return end + 10;
            else
                return scrHeight;
        }
        protected override bool beforeUpdateInsertAction()
        {
            this.PTable = PubClass.DealToFieldOrTableNames(this.PTable);
            MapAttrs.Retrieve(MapAttrAttr.FK_MapData, PTable);

            //更新版本号.
            this.Ver = DataType.CurrentDataTimess;

            #region  检查是否有ca认证设置.
            bool isHaveCA = false;
            foreach (MapAttr item in this.MapAttrs)
            {
                if (item.SignType == SignType.CA)
                {
                    isHaveCA = true;
                    break;
                }
            }
            this.IsHaveCA = isHaveCA;
            if (IsHaveCA == true)
            {
                //就增加隐藏字段.
                //MapAttr attr = new BP.Sys.MapAttr();
                // attr.MyPK = this.No + "_SealData";
                // attr.FK_MapData = this.No;
                // attr.HisEditType = BP.En.EditType.UnDel;
                //attr.KeyOfEn = "SealData";
                // attr.Name = "SealData";
                // attr.MyDataType = BP.DA.DataType.AppString;
                // attr.UIContralType = UIContralType.TB;
                //  attr.LGType = FieldTypeS.Normal;
                // attr.UIVisible = false;
                // attr.UIIsEnable = false;
                // attr.MaxLen = 4000;
                // attr.MinLen = 0;
                // attr.Save();
            }
            #endregion  检查是否有ca认证设置.

            return base.beforeUpdateInsertAction();
        }
        /// <summary>
        /// 更新版本
        /// </summary>
        public void UpdateVer()
        {
            string sql = "UPDATE Sys_MapData SET VER='" + BP.DA.DataType.CurrentDataTimess + "' WHERE No='" + this.No + "'";
            BP.DA.DBAccess.RunSQL(sql);
        }
        protected override bool beforeDelete()
        {
            string sql = "";
            sql = "SELECT * FROM Sys_MapDtl WHERE FK_MapData ='" + this.No + "'";
            DataTable Sys_MapDtl = DBAccess.RunSQLReturnTable(sql);
            string ids = "'" + this.No + "'";
            foreach (DataRow dr in Sys_MapDtl.Rows)
                ids += ",'" + dr["No"] + "'";

            string where = " FK_MapData IN (" + ids + ")";

            #region 删除相关的数据。
            sql = "DELETE FROM Sys_MapDtl WHERE FK_MapData='" + this.No + "'";
            sql += "@DELETE FROM Sys_FrmLine WHERE " + where;
            sql += "@DELETE FROM Sys_FrmEle WHERE " + where;
            sql += "@DELETE FROM Sys_FrmEvent WHERE " + where;
            sql += "@DELETE FROM Sys_FrmBtn WHERE " + where;
            sql += "@DELETE FROM Sys_FrmLab WHERE " + where;
            sql += "@DELETE FROM Sys_FrmLink WHERE " + where;
            sql += "@DELETE FROM Sys_FrmImg WHERE " + where;
            sql += "@DELETE FROM Sys_FrmImgAth WHERE " + where;
            sql += "@DELETE FROM Sys_FrmRB WHERE " + where;
            sql += "@DELETE FROM Sys_FrmAttachment WHERE " + where;
            sql += "@DELETE FROM Sys_MapM2M WHERE " + where;
            sql += "@DELETE FROM Sys_MapFrame WHERE " + where;
            sql += "@DELETE FROM Sys_MapExt WHERE " + where;
            sql += "@DELETE FROM Sys_MapAttr WHERE " + where;
            sql += "@DELETE FROM Sys_GroupField WHERE EnName IN (" + ids + ")";
            sql += "@DELETE FROM Sys_MapData WHERE No IN (" + ids + ")";
            sql += "@DELETE FROM Sys_MapM2M WHERE " + where;
            sql += "@DELETE FROM Sys_M2M WHERE " + where;
            DBAccess.RunSQLs(sql);
            #endregion 删除相关的数据。

            
            #region 删除物理表。
            //如果存在物理表.
            if (DBAccess.IsExitsObject(this.PTable))
                DBAccess.RunSQL("DROP TABLE " + this.PTable);

            MapDtls dtls = new MapDtls(this.No);
            foreach (MapDtl dtl in dtls)
                dtl.Delete();

            #endregion

            return base.beforeDelete();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mes"></param>
        /// <param name="en"></param>
        /// <returns></returns>
        public GEEntity GenerHisEn(MapExts mes, GEEntity en)
        {
            return en;
        }

        public System.Data.DataSet GenerHisDataSet()
        {
            return GenerHisDataSet(this.No);
        }
        public static System.Data.DataSet GenerHisDataSet(string FK_MapData)
        {

            // Sys_MapDtl.

            // 20150513 小周鹏修改，原因：手机端无法显示dtl Start
            // string sql = "SELECT FK_MapData,No,X,Y,W,H  FROM Sys_MapDtl WHERE FK_MapData ='{0}'";
            string sql = "SELECT *  FROM Sys_MapDtl WHERE FK_MapData ='{0}'";
            // 20150513 小周鹏修改 End

            sql = string.Format(sql, FK_MapData);
            DataTable dtMapDtl = DBAccess.RunSQLReturnTable(sql);
            dtMapDtl.TableName = "Sys_MapDtl";

            string ids = string.Format("'{0}'", FK_MapData);
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
            string nodeIDstr = FK_MapData.Replace("ND", "");
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
            sql = "@SELECT No,Name,FrmW,FrmH FROM Sys_MapData WHERE No='" + FK_MapData + "'";
            sqls += sql;

            // Sys_MapAttr.
            listNames.Add("Sys_MapAttr");

            sql = "@SELECT * "
                + " FROM Sys_MapAttr WHERE " + where + " AND KeyOfEn NOT IN('WFState') ORDER BY FK_MapData,IDX ";
            sqls += sql;

            // Sys_MapM2M.
            listNames.Add("Sys_MapM2M");
            sql = "@SELECT MyPK,FK_MapData,NoOfObj,M2MTYPE,X,Y,W,H FROM Sys_MapM2M WHERE " + where;
            sqls += sql;

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
                    attr.FK_MapData = FK_MapData;
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


        /// <summary>
        /// 生成自动的ｊｓ程序。
        /// </summary>
        /// <param name="pk"></param>
        /// <param name="attrs"></param>
        /// <param name="attr"></param>
        /// <param name="tbPer"></param>
        /// <returns></returns>
        public static string GenerAutoFull(string pk, MapAttrs attrs, MapExt me, string tbPer)
        {
            string left = "\n document.forms[0]." + tbPer + "_TB" + me.AttrOfOper + "_" + pk + ".value = ";
            string right = me.Doc;
            foreach (MapAttr mattr in attrs)
            {
                right = right.Replace("@" + mattr.KeyOfEn, " parseFloat( document.forms[0]." + tbPer + "_TB_" + mattr.KeyOfEn + "_" + pk + ".value) ");
            }
            return " alert( document.forms[0]." + tbPer + "_TB" + me.AttrOfOper + "_" + pk + ".value ) ; \t\n " + left + right;
        }
    }
    /// <summary>
    /// 映射基础s
    /// </summary>
    public class MapDatas : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 映射基础s
        /// </summary>
        public MapDatas()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapData();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapData> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapData>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapData> Tolist()
        {
            System.Collections.Generic.List<MapData> list = new System.Collections.Generic.List<MapData>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapData)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

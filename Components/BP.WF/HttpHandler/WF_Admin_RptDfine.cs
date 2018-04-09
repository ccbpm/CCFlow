using System;
using System.Collections.Generic;
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
using BP.Web.Controls;
using BP.WF.Data;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_RptDfine : DirectoryPageBase
    {
        /// <summary>
        /// 页面功能实体
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_RptDfine(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            string msg = "";
            try
            {
                switch (this.DoType)
                {
                    case "S3ColsLabel_Init": //顺序加载
                        msg = this.S3ColsLabel_Init();
                        break;
                    case "S3ColsLabel_Save": //顺序保存
                        msg = this.S3ColsLabel_Save();
                        break;
                    default:
                        msg = "err@没有判断的执行类型：" + this.DoType;
                        break;
                }
                context.Response.Write(msg);
            }
            catch (Exception ex)
            {
                context.Response.Write("err@" + ex.Message);
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + context.Request.RawUrl);
        }
        #endregion 执行父类的重写方法.

        #region 报表设计器. - 第2步选择列.
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <returns></returns>
        public string S2ColsChose_Init()
        {
            DataSet ds = new DataSet();
            string rptNo = this.GetRequestVal("RptNo");

            //所有的字段.
            string fk_mapdata = "ND"+int.Parse(this.FK_Flow)+"Rpt";
            MapAttrs mattrs = new MapAttrs(fk_mapdata);
            ds.Tables.Add(mattrs.ToDataTableField("Sys_MapAttrOfAll"));

            //判断rptNo是否存在于mapdata中
            MapData md = new MapData();
            md.No = rptNo;
            if(md.RetrieveFromDBSources() == 0)
            {
                Rpt.RptDfine rd = new Rpt.RptDfine(this.FK_Flow);
                     
                switch(rptNo.Substring(fk_mapdata.Length))
                {
                    case "My":
                        rd.DoReset_MyStartFlow();
                        break;
                    case "MyDept":
                        rd.DoReset_MyDeptFlow();
                        break;
                    case "MyJoin":
                        rd.DoReset_MyJoinFlow();
                        break;
                    case "Adminer":
                        rd.DoReset_AdminerFlow();
                        break;
                    default:
                        throw new Exception("@未涉及的rptMark类型");
                }

                md.RetrieveFromDBSources();
            }

            //选择的字段,就是报表的字段.
            MapAttrs mattrsOfRpt = new MapAttrs(rptNo);
            ds.Tables.Add(mattrsOfRpt.ToDataTableField("Sys_MapAttrOfSelected"));

            //系统字段.
            MapAttrs mattrsOfSystem = new MapAttrs();
            var sysFields = BP.WF.Glo.FlowFields;
            foreach (MapAttr item in mattrs)
            {
                if (sysFields.Contains(item.KeyOfEn))
                    mattrsOfSystem.AddEntity(item);
            }
            ds.Tables.Add(mattrsOfSystem.ToDataTableField("Sys_MapAttrOfSystem"));

            //返回.
            return BP.Tools.Json.DataSetToJson(ds, false) ;
        }
        /// <summary>
        /// 选择列的保存.
        /// </summary>
        /// <returns></returns>
        public string S2ColsChose_Save()
        {
            //报表列表.
            string rptNo = this.GetRequestVal("RptNo");

            //保存的字段,从外面传递过来的值. 用逗号隔开的: 比如:  ,Name,Tel,Addr,
            string fields = ","+this.GetRequestVal("Fields")+",";

            //构造一个空的集合.
            MapAttrs mrattrsOfRpt = new MapAttrs();
            mrattrsOfRpt.Delete(MapAttrAttr.FK_MapData, rptNo);

            //所有的字段.
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            MapAttrs allAttrs = new MapAttrs(fk_mapdata);

            foreach (MapAttr attr in allAttrs)
            {
                #region 处理特殊字段.
                if (attr.KeyOfEn == "FK_NY")
                {
                    attr.LGType = BP.En.FieldTypeS.FK;
                    attr.UIBindKey = "BP.Pub.NYs";
                    attr.UIContralType = BP.En.UIContralType.DDL;
                }

                if (attr.KeyOfEn == "FK_Dept")
                {
                    attr.LGType = BP.En.FieldTypeS.FK;
                    attr.UIBindKey = "BP.Port.Depts";
                    attr.UIContralType = BP.En.UIContralType.DDL;
                }
                #endregion 处理特殊字段.

                //增加上必要的字段.
                if (attr.KeyOfEn == "Title" || attr.KeyOfEn == "WorkID" || attr.KeyOfEn == "OID")
                {
                    attr.FK_MapData = rptNo;
                    attr.MyPK = attr.FK_MapData + "_" + attr.KeyOfEn;
                    attr.DirectInsert();
                    continue;
                }

                //如果包含了指定的字段，就执行插入操作.
                if (fields.Contains("," + attr.KeyOfEn + ",") == true)
                {
                    attr.FK_MapData = rptNo;
                    attr.MyPK = attr.FK_MapData + "_" + attr.KeyOfEn;
                    attr.DirectInsert();
                }
            }

            return "保存成功.";
        }
        #endregion

        #region 报表设计器. - 第3步设置列的顺序.
        /// <summary>
        /// 初始化方法
        /// </summary>
        /// <returns></returns>
        public string S3ColsLabel_Init()
        {
            string rptNo = this.GetRequestVal("RptNo");

            //判断rptNo是否存在于mapdata中
            MapData md = new MapData();
            md.No = rptNo;
            if (md.RetrieveFromDBSources() == 0)
            {
                Rpt.RptDfine rd = new Rpt.RptDfine(this.FK_Flow);

                switch (rptNo.Substring(("ND" + int.Parse(this.FK_Flow) + "Rpt").Length))
                {
                    case "My":
                        rd.DoReset_MyStartFlow();
                        break;
                    case "MyDept":
                        rd.DoReset_MyDeptFlow();
                        break;
                    case "MyJoin":
                        rd.DoReset_MyJoinFlow();
                        break;
                    case "Adminer":
                        rd.DoReset_AdminerFlow();
                        break;
                    default:
                        throw new Exception("@未涉及的rptMark类型");
                }

                md.RetrieveFromDBSources();
            }

            //选择的字段,就是报表的字段.
            MapAttrs mattrsOfRpt = new MapAttrs(rptNo);
            mattrsOfRpt.RemoveEn(rptNo + "_OID");
            mattrsOfRpt.RemoveEn(rptNo + "_Title");

            return mattrsOfRpt.ToJson();
        }
        /// <summary>
        /// 保存列的顺序名称.
        /// </summary>
        /// <returns></returns>
        public string S3ColsLabel_Save()
        {
            string orders = this.GetRequestVal("Orders");
            //格式为  @KeyOfEn,Lable,idx  比如： @DianHua,电话,1@Addr,地址,2
            
            string rptNo=this.GetRequestVal("RptNo");

            string[] strs = orders.Split('@');
            foreach (string item in strs)
            {
                if (DataType.IsNullOrEmpty(item) == true)
                    continue;

                string[] vals = item.Split(',');

                string mypk = rptNo + "_" + vals[0];

                MapAttr attr = new MapAttr();
                attr.MyPK = mypk;
                attr.Retrieve();

                attr.Name = vals[1];
                attr.Idx = int.Parse(vals[2]);

                attr.Update(); //执行更新.
            }

            MapAttr myattr = new MapAttr();
            myattr.MyPK = rptNo+"_OID";
            myattr.RetrieveFromDBSources();
            myattr.Idx = 200;
            myattr.Name = "工作ID";
            myattr.Update();

            myattr = new MapAttr();
            myattr.MyPK = rptNo + "_Title";
            myattr.RetrieveFromDBSources();
            myattr.Idx = -100;
            myattr.Name = "标题";
            myattr.Update();

            return "保存成功..";
        }
        #endregion

        #region 报表设计器 - 第4步骤.
        public string S5SearchCond_Init()
        {
            //报表编号.
            string rptNo = this.GetRequestVal("RptNo");

            //定义容器.
            DataSet ds = new DataSet();
            
            //判断rptNo是否存在于mapdata中
            MapData md = new MapData();
            md.No = rptNo;
            if (md.RetrieveFromDBSources() == 0)
            {
                Rpt.RptDfine rd = new Rpt.RptDfine(this.FK_Flow);

                switch (rptNo.Substring(("ND" + int.Parse(this.FK_Flow) + "Rpt").Length))
                {
                    case "My":
                        rd.DoReset_MyStartFlow();
                        break;
                    case "MyDept":
                        rd.DoReset_MyDeptFlow();
                        break;
                    case "MyJoin":
                        rd.DoReset_MyJoinFlow();
                        break;
                    case "Adminer":
                        rd.DoReset_AdminerFlow();
                        break;
                    default:
                        throw new Exception("@未涉及的rptMark类型");
                }

                md.RetrieveFromDBSources();
            }

            ds.Tables.Add(md.ToDataTableField("Main"));

            //查询出来枚举与外键类型的字段集合.
            MapAttrs attrs = new MapAttrs();
            attrs.Retrieve(MapAttrAttr.FK_MapData, rptNo);
            ds.Tables.Add(attrs.ToDataTableField("Sys_MapAttr"));

            #region 检查是否有日期字段.
            bool isHave = false;
            foreach (MapAttr mattr in attrs)
            {
                if (mattr.UIVisible == false)
                    continue;
                
                if (mattr.MyDataType == DataType.AppDate || mattr.MyDataType == DataType.AppDateTime)
                {
                    isHave = true;
                    break;
                }
            }

            if (isHave == true)
            {
                DataTable dt = new DataTable();
                MapAttrs dtAttrs = new MapAttrs();
                foreach (MapAttr mattr in attrs)
                {
                    if (mattr.MyDataType == DataType.AppDate || mattr.MyDataType == DataType.AppDateTime)
                    {
                        if (mattr.UIVisible == false)
                            continue;
                        dtAttrs.AddEntity(mattr);
                    }
                }
                ds.Tables.Add(dtAttrs.ToDataTableField("Sys_MapAttrOfDate"));
            }
            #endregion

            //返回数据.
            return BP.Tools.Json.DataSetToJson(ds, false);
        }
        public string RptNo
        {
            get
            {
                return  this.GetRequestVal("RptNo");
            }
        }
        /// <summary>
        /// 查询条件保存.
        /// </summary>
        /// <returns></returns>
        public string S5SearchCond_Save()
        {
            MapData md = new MapData();
            md.No = this.RptNo;
            md.RetrieveFromDBSources();

            //报表编号.
            string fields = this.GetRequestVal("Fields");
            md.RptSearchKeys = fields + "*";

            string IsSearchKey = this.GetRequestVal("IsSearchKey");
            if (IsSearchKey == "0")
                md.RptIsSearchKey = false;
            else
                md.RptIsSearchKey = true;

            //查询方式.
            int DTSearchWay = this.GetRequestValInt("DTSearchWay");
            md.RptDTSearchWay = (DTSearchWay)DTSearchWay;

            //日期字段.
            string DTSearchKey = this.GetRequestVal("DTSearchKey");
            md.RptDTSearchKey = DTSearchKey;
            md.Save();

            Cash.Map_Cash.Remove(this.RptNo);
            return "保存成功.";
        }
        #endregion
    }
}

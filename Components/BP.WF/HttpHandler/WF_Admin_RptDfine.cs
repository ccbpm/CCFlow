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

using BP.WF.Data;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_Admin_RptDfine : DirectoryPageBase
    {


        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_RptDfine()
        {
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
                HttpContextHelper.ResponseWrite(msg);
            }
            catch (Exception ex)
            {
                HttpContextHelper.ResponseWrite("err@" + ex.Message);
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
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
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            MapAttrs mattrs = new MapAttrs(fk_mapdata);
            ds.Tables.Add(mattrs.ToDataTableField("Sys_MapAttrOfAll"));

            //判断rptNo是否存在于mapdata中
            MapData md = new MapData();
            md.No = rptNo;
            if (md.RetrieveFromDBSources() == 0)
            {
                Rpt.RptDfine rd = new Rpt.RptDfine(this.FK_Flow);

                switch (rptNo.Substring(fk_mapdata.Length))
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
            return BP.Tools.Json.DataSetToJson(ds, false);
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
            string fields = "," + this.GetRequestVal("Fields") + ",";

            //增加上必要的字段.
            if (rptNo.Contains("My") == true && fields.Contains(",FlowEmps,") == false)
                fields += "FlowEmps,";

            if (rptNo.Contains("MyDept") == true && fields.Contains(",FK_Dept,") == false)
                fields += "FK_Dept,";

            //字段组合.
            fields += BP.WF.Rpt.RptDfine.PublicFiels;

            //构造一个空的集合，如果有数据，就把旧数据删除掉.
            MapAttrs mrattrsOfRpt = new MapAttrs();
            mrattrsOfRpt.Delete(MapAttrAttr.FK_MapData, rptNo);

            //所有的字段.
            string fk_mapdata = "ND" + int.Parse(this.FK_Flow) + "Rpt";
            MapAttrs allAttrs = new MapAttrs(fk_mapdata);

            foreach (MapAttr attr in allAttrs)
            {
                //如果包含了指定的字段，就执行插入操作.
                if (fields.Contains("," + attr.KeyOfEn + ",") == false)
                    continue;

                attr.FK_MapData = rptNo;
                attr.MyPK = attr.FK_MapData + "_" + attr.KeyOfEn;

                #region 判断特殊的字段.
                switch (attr.KeyOfEn)
                {
                    case GERptAttr.WFSta:
                        attr.UIBindKey = "WFSta";
                        attr.UIContralType = UIContralType.DDL;
                        attr.LGType = FieldTypeS.Enum;
                        attr.UIVisible = false;
                        attr.DefVal = "0";
                        attr.MaxLen = 100;
                        attr.UIVisible = true;
                        attr.Insert();
                        continue;
                    case GERptAttr.FK_Dept:
                        attr.UIBindKey = "";
                        //attr.UIBindKey = "BP.Port.Depts";
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.UIVisible = false;
                        attr.DefVal = "";
                        attr.MaxLen = 100;
                        attr.UIVisible = false;
                        attr.Insert();
                        continue;
                    case GERptAttr.FK_NY:
                        attr.UIBindKey = "BP.Pub.NYs";
                        attr.UIContralType = UIContralType.DDL;
                        attr.LGType = FieldTypeS.FK;
                        attr.UIVisible = true;
                        attr.UIIsEnable = false;
                        //attr.GroupID = groupID;
                        attr.Insert();
                        continue;
                    case GERptAttr.Title:
                        attr.UIWidth = 120;
                        attr.UIVisible = true;
                        attr.Idx = 0;
                        attr.Insert();
                        continue;
                    case GERptAttr.FlowStarter:
                        attr.UIIsEnable = false;
                        attr.UIVisible = false;
                        attr.UIBindKey = "";
                        //attr.UIBindKey = "BP.Port.Depts";
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.Insert();
                        continue;
                    case GERptAttr.FlowEmps:
                        attr.UIIsEnable = false;
                        attr.UIVisible = false;
                        attr.UIBindKey = "";
                        //attr.UIBindKey = "BP.Port.Depts";
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.Insert();
                        continue;
                    case GERptAttr.WFState:
                        attr.UIIsEnable = false;
                        attr.UIVisible = false;
                        attr.UIBindKey = "";
                        //attr.UIBindKey = "BP.Port.Depts";
                        attr.UIContralType = UIContralType.TB;
                        attr.LGType = FieldTypeS.Normal;
                        attr.MyDataType = BP.DA.DataType.AppInt;
                        attr.Insert();
                        continue;
                    case GERptAttr.FlowEndNode:
                        //attr.LGType = FieldTypeS.FK;
                        //attr.UIBindKey = "BP.WF.Template.NodeExts";
                        //attr.UIContralType = UIContralType.DDL;
                        break;
                    case "FK_Emp":
                        break;
                    default:
                        break;
                }
                #endregion


                attr.UIVisible = true;

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
            MapAttrs mattrsOfRpt = new MapAttrs();
            QueryObject qo = new QueryObject(mattrsOfRpt);
            qo.AddWhere(MapAttrAttr.FK_MapData, rptNo);
            qo.addOrderBy(MapAttrAttr.Idx);
            qo.DoQuery();

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

            string rptNo = this.GetRequestVal("RptNo");

            DBAccess.RunSQL("UPDATE Sys_MapAttr SET Idx=10000 WHERE FK_MapData='" + rptNo + "'");

            string[] strs = orders.Split('@');
            //重新对table排序
            for (int i = 0; i < strs.Length; i++)
            {
                string item = strs[i];

                if (DataType.IsNullOrEmpty(item) == true)
                    continue;

                string[] vals = item.Split(',');

                string mypk = rptNo + "_" + vals[0];

                // 更新顺序.
                DBAccess.RunSQL("UPDATE Sys_MapAttr SET Idx=" + i + " WHERE No='" + mypk + "'");
            }

            //foreach (string item in strs)
            //{
            //    if (DataType.IsNullOrEmpty(item) == true)
            //        continue;

            //    string[] vals = item.Split(',');

            //    string mypk = rptNo + "_" + vals[0];

            //    MapAttr attr = new MapAttr();
            //    attr.MyPK = mypk;
            //    attr.Retrieve();

            //    attr.Name = vals[1];
            //    attr.Idx = int.Parse(vals[2]);

            //    attr.Update(); //执行更新.
            //}

            MapAttr myattr = new MapAttr();
            myattr.MyPK = rptNo + "_OID";
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
            attrs.Retrieve(MapAttrAttr.FK_MapData, rptNo, "Idx");
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
                return this.GetRequestVal("RptNo");
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
                md.IsSearchKey = false;
            else
                md.IsSearchKey = true;
            md.SetPara("StringSearchKeys", this.GetRequestVal("StringSearchKeys"));
            //查询方式.
            int DTSearchWay = this.GetRequestValInt("DTSearchWay");
            md.DTSearchWay = (DTSearchWay)DTSearchWay;

            //日期字段.
            string DTSearchKey = this.GetRequestVal("DTSearchKey");
            md.DTSearchKey = DTSearchKey;

            //是否查询自己部门发起
            md.SetPara("IsSearchNextLeavel", this.GetRequestValBoolen("IsSearchNextLeavel"));
            md.Save();

            Cash.Map_Cash.Remove(this.RptNo);
            return "保存成功.";
        }
        #endregion
    }
}

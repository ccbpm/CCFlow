using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Web;
using BP.WF;
using BP.Web;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.WF.Template;
using BP.WF.XML;
using System.IO;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_CCFormDesigner_FrmEvent : BP.WF.HttpHandler.DirectoryPageBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_CCFormDesigner_FrmEvent(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region 事件基类.
        /// <summary>
        /// 事件类型
        /// </summary>
        public new string ShowType
        {
            get
            {
                if (this.FK_Node != 0)
                    return "Node";

                if (this.FK_Node == 0 && string.IsNullOrEmpty(this.FK_Flow) == false && this.FK_Flow.Length >= 3)
                    return "Flow";

                if (this.FK_Node == 0 && string.IsNullOrEmpty(this.FK_MapData) == false)
                    return "Frm";

                return "Node";
            }
        }
        /// <summary>
        /// 事件基类
        /// </summary>
        /// <returns></returns>
        public string Action_Init()
        {
            DataSet ds = new DataSet();

            //事件实体.
            FrmEvents ndevs = new FrmEvents();
            if (BP.DA.DataType.IsNullOrEmpty(this.FK_MapData) == false)
                ndevs.Retrieve(FrmEventAttr.FK_MapData, this.FK_MapData);

            ////已经配置的事件类实体.
            //DataTable dtFrm = ndevs.ToDataTableField("FrmEvents");
            //ds.Tables.Add(dtFrm);

            //把事件类型列表放入里面.（发送前，发送成功时.）
            EventLists xmls = new EventLists();
            xmls.Retrieve("EventType", this.ShowType);

            DataTable dt = xmls.ToDataTable();
            dt.TableName = "EventLists";
            ds.Tables.Add(dt);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 获得该节点下已经绑定该类型的实体.
        /// </summary>
        /// <returns></returns>
        public string ActionDtl_Init()
        {
            DataSet ds = new DataSet();

            //事件实体.
            FrmEvents ndevs = new FrmEvents();
            ndevs.Retrieve(FrmEventAttr.FK_MapData, this.FK_MapData);

            DataTable dt = ndevs.ToDataTableField("FrmEvents");
            ds.Tables.Add(dt);

            //业务单元集合.
            DataTable dtBuess = new DataTable();
            dtBuess.Columns.Add("No", typeof(string));
            dtBuess.Columns.Add("Name", typeof(string));
            dtBuess.TableName = "BuessUnits";
            ArrayList al = BP.En.ClassFactory.GetObjects("BP.Sys.BuessUnitBase");
            foreach (BuessUnitBase en in al)
            {
                DataRow dr = dtBuess.NewRow();
                dr["No"] = en.ToString();
                dr["Name"] = en.Title;
                dtBuess.Rows.Add(dr);
            }

            ds.Tables.Add(dtBuess);

            return BP.Tools.Json.ToJson(ds);
        }
        /// <summary>
        /// 执行删除
        /// </summary>
        /// <returns></returns>
        public string ActionDtl_Delete()
        {
            //事件实体.
            FrmEvent en = new FrmEvent();
            en.MyPK = this.MyPK;
            en.Delete();
            return "删除成功.";
        }
        public string ActionDtl_Save()
        {
            //事件实体.
            FrmEvent en = new FrmEvent();

            en.FK_Node = this.FK_Node;
            en.FK_Event = this.GetRequestVal("FK_Event"); //事件类型.
            en.HisDoTypeInt = this.GetValIntFromFrmByKey("EventDoType"); //执行类型.
            en.MyPK = this.FK_Node + "_" + en.FK_Event + "_" + en.HisDoTypeInt; //组合主键.
            en.RetrieveFromDBSources();

            en.MsgOKString = this.GetValFromFrmByKey("MsgOK"); //成功的消息.
            en.MsgErrorString = this.GetValFromFrmByKey("MsgError"); //失败的消息.

            //执行内容.
            if (en.HisDoType == EventDoType.BuessUnit)
                en.DoDoc = this.GetValFromFrmByKey("DDL_Doc");
            else
                en.DoDoc = this.GetValFromFrmByKey("TB_Doc");

            en.Save();

            return "保存成功.";
        }
        #endregion 事件基类.
         
    }
}
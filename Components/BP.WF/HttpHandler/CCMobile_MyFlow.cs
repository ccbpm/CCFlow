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

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class CCMobile_MyFlow : DirectoryPageBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CCMobile_MyFlow()
        {
        }
        /// <summary>
        /// 获得工作节点
        /// </summary>
        /// <returns></returns>
        public string GenerWorkNode()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.GenerWorkNode();
             
        }
        /// <summary>
        /// 绑定多表单中获取节点表单的数据
        /// </summary>
        /// <returns></returns>
        public string GetNoteValue()
        {
            int fk_node = this.FK_Node;
            if (fk_node == 0)
                fk_node = int.Parse(this.FK_Flow + "01");
            Node nd = new Node(fk_node);
            #region  获取节点表单的数据
            Work wk = nd.HisWork;
            wk.OID = this.WorkID;
            wk.RetrieveFromDBSources();
            wk.ResetDefaultVal();
            if (BP.Sys.SystemConfig.IsBSsystem == true)
            {
                // 处理传递过来的参数。
                foreach (string k in HttpContextHelper.RequestQueryStringKeys)
                {
                    if (DataType.IsNullOrEmpty(k) == true)
                        continue;

                    wk.SetValByKey(k, HttpContextHelper.RequestParams(k));
                }

                // 处理传递过来的frm参数。
                foreach (string k in HttpContextHelper.RequestParamKeys)
                {
                    if (DataType.IsNullOrEmpty(k) == true)
                        continue;

                    wk.SetValByKey(k, HttpContextHelper.RequestParams(k));
                }
            }
            #endregion 获取节点表单的数据
            //节点表单字段
            MapData md = new MapData(nd.NodeFrmID);
            MapAttrs attrs = md.MapAttrs;
            DataTable dt = new DataTable();
            dt.TableName = "Node_Note";
            dt.Columns.Add("KeyOfEn", typeof(string));
            dt.Columns.Add("NoteVal", typeof(string));
            string nodeNote = nd.GetParaString("NodeNote");
           
            foreach (MapAttr attr in attrs)
            {
                if (nodeNote.Contains("," + attr.KeyOfEn + ",") == false)
                    continue;
                string text = "";
                switch (attr.LGType)
                {
                    case FieldTypeS.Normal:  // 输出普通类型字段.
                        if (attr.MyDataType == 1 && (int)attr.UIContralType == DataType.AppString)
                        {

                            if (attrs.Contains(attr.KeyOfEn + "Text") == true)
                                text = wk.GetValRefTextByKey(attr.KeyOfEn);
                            if (DataType.IsNullOrEmpty(text))
                                if (attrs.Contains(attr.KeyOfEn + "T") == true)
                                    text = wk.GetValStrByKey(attr.KeyOfEn + "T");
                        }
                        else
                        {
                            text = wk.GetValStrByKey(attr.KeyOfEn);
                            if (attr.IsRichText == true)
                            {
                                text = text.Replace("white-space: nowrap;", "");
                            }
                        }

                        break;
                    case FieldTypeS.Enum:
                    case FieldTypeS.FK:
                        text = wk.GetValRefTextByKey(attr.KeyOfEn);
                        break;
                    default:
                        break;
                }
                DataRow dr = dt.NewRow();
                dr["KeyOfEn"] = attr.KeyOfEn;
                dr["NoteVal"] = text;
                dt.Rows.Add(dr);

             }
           
            return BP.Tools.Json.ToJson(dt);
        }
        /// <summary>
        /// 获得toolbar
        /// </summary>
        /// <returns></returns>
        public string InitToolBar()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.InitToolBarForMobile();
        }
        public string MyFlow_Init()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.MyFlow_Init();
        }
        public string MyFlow_StopFlow()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.MyFlow_StopFlow();
        }
        public string Save()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.Save();
        }
        public string Send()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.Send();
        }
        public string StartGuide_Init()
        {
            WF_MyFlow en = new WF_MyFlow();
            return en.StartGuide_Init();
        }
        public string FrmGener_Init()
        {
            WF_CCForm ccfrm = new WF_CCForm();
            return ccfrm.FrmGener_Init();
        }
        public string FrmGener_Save()
        {
            WF_CCForm ccfrm = new WF_CCForm();
            return ccfrm.FrmGener_Save();
        }

        public string MyFlowGener_Delete()
        {
            BP.WF.Dev2Interface.Flow_DoDeleteFlowByWriteLog(this.FK_Flow, this.WorkID, Web.WebUser.Name+"用户删除", true);
            return "删除成功...";
        }

        public string AttachmentUpload_Down()
        {
            WF_CCForm ccform = new WF_CCForm();
            return ccform.AttachmentUpload_Down();
        }
      
    }
}

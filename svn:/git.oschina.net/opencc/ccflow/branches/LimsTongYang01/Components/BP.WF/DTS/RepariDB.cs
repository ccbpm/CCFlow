using System;
using System.Collections;
using BP.DA;
using BP.Web.Controls;
using System.Reflection;
using BP.Port;
using BP.En;
using BP.Sys;
namespace BP.WF.DTS
{
    /// <summary>
    /// 修复数据库 的摘要说明
    /// </summary>
    public class RepariDB : Method
    {
        /// <summary>
        /// 不带有参数的方法
        /// </summary>
        public RepariDB()
        {
            this.Title = "修复数据库";
            this.Help = "把最新的版本的与当前的数据表结构，做一个自动修复, 修复内容：缺少列，缺少列注释，列注释不完整或者有变化。";
            this.Help += "<br>因为表单设计器的错误，丢失了字段，通过它也可以自动修复。";
            this.Help += "<br><a href='/'>进入流程设计器</a>";
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
            //this.Warning = "您确定要执行吗？";
            //HisAttrs.AddTBString("P1", null, "原密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P2", null, "新密码", true, false, 0, 10, 10);
            //HisAttrs.AddTBString("P3", null, "确认", true, false, 0, 10, 10);
        }
        /// <summary>
        /// 当前的操纵员是否可以执行这个方法
        /// </summary>
        public override bool IsCanDo
        {
            get
            {
                return true;
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>返回执行结果</returns>
        public override object Do()
        {
            string rpt =PubClass.DBRpt(BP.DA.DBCheckLevel.High);

            //// 手动升级. 2011-07-08 补充节点字段分组.
            //string sql = "DELETE FROM Sys_EnCfg WHERE No='BP.WF.Template.NodeSheet'";
            //BP.DA.DBAccess.RunSQL(sql);

            //sql = "INSERT INTO Sys_EnCfg(No,GroupTitle) VALUES ('BP.WF.Template.NodeSheet','NodeID=基本配置@WarningHour=考核属性@SendLab=功能按钮标签与状态')";
            //BP.DA.DBAccess.RunSQL(sql);

            // 修复因bug丢失的字段.
            MapDatas mds = new MapDatas();
            mds.RetrieveAll();
            foreach (MapData md in mds)
            {
                string nodeid = md.No.Replace("ND","");
                try
                {
                    BP.WF.Node nd = new Node(int.Parse(nodeid));
                    nd.RepareMap(nd.HisFlow);
                    continue;
                }
                catch(Exception ex)
                {

                }

                MapAttr attr = new MapAttr();
                if (attr.IsExit(MapAttrAttr.KeyOfEn, "OID", MapAttrAttr.FK_MapData, md.No) == false)
                {
                    attr.FK_MapData = md.No;
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
            return "执行成功...";
        }
    }
}

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using BP.Sys;
using BP.DA;
using BP.En;
using BP.Web.Controls;

namespace CCFlow.Web.CT.Comm.Sys
{
	/// <summary>
	/// SystemClassDtl 的摘要说明。
	/// </summary>
    public partial class SystemClassDtl : BP.Web.WebPageAdmin
	{
        public new string EnsName
		{
			get
			{
				return this.Request.QueryString["EnsName"];
			}
		}
        public void BindCheck()
        {
            BP.En.Entity en = BP.En.ClassFactory.GetEn(this.EnsName);
            BP.En.Map map = en.EnMap;
            en.CheckPhysicsTable();
            string msg = "";
           // string msg = "";
            string table = "";
            string sql = "";
            string sql1 = "";
            string sql2 = "";
            int COUNT1 = 0;
            int COUNT2 = 0;

            DataTable dt = new DataTable();
            Entity refen = null;
            foreach (Attr attr in map.Attrs)
            {
                /**/
                if (attr.MyFieldType == FieldType.FK || attr.MyFieldType == FieldType.PKFK)
                {
                    refen = ClassFactory.GetEns(attr.UIBindKey).GetNewEntity;
                    table = refen.EnMap.PhysicsTable;
                    sql1 = "SELECT COUNT(*) FROM " + table;

                    Attr pkAttr = refen.EnMap.GetAttrByKey(refen.PK);
                    sql2 = "SELECT COUNT( distinct " + pkAttr.Field + ") FROM " + table;

                    COUNT1 = DBAccess.RunSQLReturnValInt(sql1);
                    COUNT2 = DBAccess.RunSQLReturnValInt(sql2);

                    if (COUNT1 != COUNT2)
                    {
                        msg += "<BR>@关联表(" + refen.EnMap.EnDesc + ")主键不唯一，它会造成数据查询不准确或者意向不到的错误：<BR>sql1=" + sql1 + " <BR>sql2=" + sql2;
                        msg += "@SQL= SELECT * FROM (  select " + refen.PK + ",  COUNT(*) AS NUM  from " + table + " GROUP BY " + refen.PK + " ) WHERE NUM!=1";
                    }

                    sql = "SELECT " + attr.Field + " FROM " + map.PhysicsTable + " WHERE " + attr.Field + " NOT IN (SELECT " + pkAttr.Field  + " FROM " + table + " )";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                        continue;
                    else
                        msg += "<BR>:有" + dt.Rows.Count + "个错误。" + attr.Desc + " sql= " + sql;
                }
                if (attr.MyFieldType == FieldType.PKEnum || attr.MyFieldType == FieldType.Enum)
                {
                    sql = "SELECT " + attr.Field + " FROM " + map.PhysicsTable + " WHERE " + attr.Field + " NOT IN ( select Intkey from sys_enum WHERE ENUMKEY='" + attr.UIBindKey + "' )";
                    dt = DBAccess.RunSQLReturnTable(sql);
                    if (dt.Rows.Count == 0)
                        continue;
                    else
                        msg += "<BR>:有" + dt.Rows.Count + "个错误。" + attr.Desc + " sql= " + sql;
                }
            }

            // 检查pk是否一致。
            if (en.PKs.Length == 1)
            {
                sql1 = "SELECT COUNT(*) FROM " + map.PhysicsTable;
                COUNT1 = DBAccess.RunSQLReturnValInt(sql1);

                Attr attrMyPK= en.EnMap.GetAttrByKey(en.PK); 
                sql2 = "SELECT COUNT(DISTINCT " + attrMyPK.Field  + ") FROM " + map.PhysicsTable;
                COUNT2 = DBAccess.RunSQLReturnValInt(sql2);
                if (COUNT1 != COUNT2)
                {
                    msg += "@物理表(" + map.EnDesc + ")中主键不唯一;它会造成数据查询不准确或者意向不到的错误：<BR>sql1=" + sql1 + " <BR>sql2=" + sql2;
                    msg += "@SQL= SELECT * FROM (  select " + en.PK + ",  COUNT(*) AS NUM  from " + map.PhysicsTable + " GROUP BY " + en.PK + " ) WHERE NUM!=1";
                }
            }

            if (msg == "")
            {
                this.UCSys1.AddMsgOfInfo(map.EnDesc + ":数据体检信息：", "体检成功。");
            }
            else
            {
                this.UCSys1.AddMsgOfWarning(map.EnDesc + ":数据体检信息：", "体检失败<BR>" + msg.Replace("@", "<BR>@"));
            }

            string help = "";
            help += "@什么是数据体检?";
            help += "&nbsp;&nbsp;数据体检就是检查数据的完整性。";
            help += "@如果系统升级，一些字段的长度发生变化，字段的类型发生变化系统就会自动修复。";
            help += "@系统会检查出来，外键枚举对应不上的信息，并把该信息列出来让管理员处理。";

            this.UCSys1.AddMsgOfInfo("什么是数据体检?", help);
        }
		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (this.Request.QueryString["Type"] != null)
            {
                this.BindCheck();
                return;
            }

            if (this.EnsName == null)
            {
                this.BindEns();
            }
            else
            {
                this.BindEn(ClassFactory.GetEn( this.EnsName ) );
            }
		}
        public void BindEns()
        {
            this.UCSys1.AddTable();
            ArrayList al = BP.En.ClassFactory.GetObjects("BP.En.Entity");
            int i = 0;
            foreach (Object obj in al)
            {
                this.UCSys1.AddTR();
                i++;
                Entity en = obj as Entity;
                this.BindEn(en);
                this.UCSys1.AddTREnd();
            }
            this.UCSys1.AddTableEnd();
        }
        public void BindEn(Entity  en)
        {
            BP.En.Map map = en.EnMap;
            en.CheckPhysicsTable();


            this.UCSys1.AddTable();

            if (this.EnsName == null)
                this.UCSys1.AddCaptionLeft("数据结构：" + map.EnDesc + "," + map.PhysicsTable);
            else
                this.UCSys1.AddCaptionLeft("数据结构：" + map.EnDesc + "," + map.PhysicsTable);

            this.UCSys1.AddTR();
            this.UCSys1.AddTDTitle("序号");
            this.UCSys1.AddTDTitle("描述");
            this.UCSys1.AddTDTitle("属性");
            this.UCSys1.AddTDTitle("物理字段");
            this.UCSys1.AddTDTitle("数据类型");
            this.UCSys1.AddTDTitle("关系类型");
            this.UCSys1.AddTDTitle("长度");
            this.UCSys1.AddTDTitle("对应");
            this.UCSys1.AddTDTitle("默认值");
            this.UCSys1.AddTREnd();

            int i = 0;
            foreach (Attr attr in map.Attrs)
            {
                if (attr.MyFieldType == FieldType.RefText)
                    continue;
                i++;
                this.UCSys1.AddTR();
                this.UCSys1.AddTDIdx(i);
                this.UCSys1.AddTD(attr.Desc);
                this.UCSys1.AddTD(attr.Key);
                this.UCSys1.AddTD(attr.Field);
                this.UCSys1.AddTD(attr.MyDataTypeStr);
                this.UCSys1.AddTD(attr.MyFieldType.ToString());

                if (attr.MyDataType == DataType.AppBoolean
                    || attr.MyDataType == DataType.AppDouble
                    || attr.MyDataType == DataType.AppFloat
                    || attr.MyDataType == DataType.AppInt
                    || attr.MyDataType == DataType.AppMoney
                    )
                    this.UCSys1.AddTD("无");
                else
                    this.UCSys1.AddTD(attr.MaxLength);


                switch (attr.MyFieldType)
                {
                    case FieldType.Enum:
                    case FieldType.PKEnum:
                        try
                        {
                            SysEnums ses = new SysEnums(attr.UIBindKey);
                            string str = "";
                            foreach (SysEnum se in ses)
                            {
                                str += se.IntKey + "&nbsp;" + se.Lab + ",";
                            }
                            this.UCSys1.AddTD(str);
                        }
                        catch
                        {
                            this.UCSys1.AddTD("未使用");
                        }
                        break;
                    case FieldType.FK:
                    case FieldType.PKFK:
                        Entities ens = ClassFactory.GetEns(attr.UIBindKey);
                        this.UCSys1.AddTD("表/视图:"+ens.GetNewEntity.EnMap.PhysicsTable+" 关联字段:"+attr.UIRefKeyValue+","+attr.UIRefKeyText);
                        break;
                    default:
                        this.UCSys1.AddTD("无");
                        break;
                }

                this.UCSys1.AddTD(attr.DefaultVal.ToString());
                this.UCSys1.AddTREnd();
            }
            this.UCSys1.AddTableEnd();
        }

		#region Web 窗体设计器生成的代码
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: 该调用是 ASP.NET Web 窗体设计器所必需的。
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion
	}
}

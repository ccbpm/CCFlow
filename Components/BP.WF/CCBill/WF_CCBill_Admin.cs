﻿using System;
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


namespace BP.CCBill
{
    /// <summary>
    /// 页面功能实体
    /// </summary>
    public class WF_CCBill_Admin : DirectoryPageBase
    {
        #region 方法的操作.
        /// <summary>
        /// 方法的初始化
        /// </summary>
        /// <returns></returns>
        public string Method_Init()
        {
            BP.CCBill.Template.GroupMethod gnn = new GroupMethod();

            GroupMethods gms = new GroupMethods();
            int i = gms.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
            if (i == 0)
            {
                GroupMethod gm = new GroupMethod();
                gm.FrmID = this.FrmID;
                gm.Name = "相关操作";
                gm.MethodType = "Home";
                gm.Icon = "icon-home";
                gm.Insert();
                gms.Retrieve(GroupMethodAttr.FrmID, this.FrmID, "Idx");
            }

            DataTable dtGroups = gms.ToDataTableField("Groups");
            dtGroups.TableName = "Groups";

            Methods methods = new Methods();
            methods.Retrieve(MethodAttr.FrmID, this.FrmID, MethodAttr.IsEnable, 1, "Idx");

            DataTable dtMethods = methods.ToDataTableField("Methods");
            dtMethods.TableName = "Methods";

            DataSet ds = new DataSet();
            ds.Tables.Add(dtGroups);
            ds.Tables.Add(dtMethods);

            return BP.Tools.Json.ToJson(ds);

        }
        /// <summary>
        /// 移动分组
        /// </summary>
        /// <returns></returns>
        public string Method_MoverGroup()
        {
            string[] ens = this.GetRequestVal("GroupIDs").Split(',');
            string frmID = this.FrmID;
            for (int i = 0; i < ens.Length; i++)
            {
                var en = ens[i];

                string sql = "UPDATE Frm_GroupMethod SET Idx=" + i + " WHERE No='" + en + "' AND FrmID='" + frmID + "'";
                DBAccess.RunSQL(sql);
            }
            return "目录移动成功..";
        }
        /// <summary>
        /// 移动方法.
        /// </summary>
        /// <returns></returns>
        public string Method_MoverMethod()
        {
            string sortNo = this.GetRequestVal("GroupID");

            string[] ens = this.GetRequestVal("MethodIDs").Split(',');
            for (int i = 0; i < ens.Length; i++)
            {
                var enNo = ens[i];
                string sql = "UPDATE Frm_Method SET GroupID ='" + sortNo + "',Idx=" + i + " WHERE No='" + enNo + "'";
                DBAccess.RunSQL(sql);
            }
            return "方法顺序移动成功..";
        }
        #endregion 方法的操作.

        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_CCBill_Admin()
        {

        }
        /// <summary>
        /// 获得js,sql内容.
        /// </summary>
        /// <returns></returns>
        public string MethodDoc_GetScript()
        {
            var en = new MethodFunc(this.No);
            int type = this.GetRequestValInt("TypeOfFunc");
            if (type == 0)
                return en.MethodDoc_SQL;

            if (type == 1)
                return en.MethodDoc_JavaScript;

            if (type == 2)
                return en.MethodDoc_Url;

            return "err@没有判断的类型.";
        }
        /// <summary>
        /// 保存脚本
        /// </summary>
        /// <returns></returns>
        public string MethodDoc_SaveScript()
        {
            var en = new MethodFunc(this.No);

            int type = this.GetRequestValInt("TypeOfFunc");
            string doc = this.GetRequestVal("doc");
            string funcstr = this.GetRequestVal("funcstr");
            //sql模式.
            if (type == 0)
                en.MethodDoc_SQL = doc;

            //script.
            if (type == 1)
            {
                en.MethodDoc_JavaScript = doc;

                string path = SystemConfig.PathOfDataUser + "JSLibData\\Method\\";
                if (System.IO.Directory.Exists(path) == false)
                    System.IO.Directory.CreateDirectory(path);
                //写入文件.
                string file = path + en.No + ".js";
                DataType.WriteFile(file, funcstr);
            }

            //url.
            if (type == 2)
                en.MethodDoc_Url = doc;

            en.MethodDocTypeOfFunc = type;
            en.Update();

            return "保存成功.";
        }

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            switch (this.DoType)
            {
                case "DtlFieldUp": //字段上移
                    return "执行成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记[" + this.DoType + "]DoMethod=[" + this.GetRequestVal("DoMethod") + "]，没有找到. @RowURL:" + HttpContextHelper.RequestRawUrl);
        }
        #endregion 执行父类的重写方法.
    }
}

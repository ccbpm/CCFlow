﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.DA;
using BP.En;

namespace BP.WF.HttpHandler
{
    /// <summary>
    /// 初始化函数
    /// </summary>
    public class WF_Admin_CCFormDesigner_DialogCtr : WebContralBase
    {
        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Admin_CCFormDesigner_DialogCtr(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string PublicNoName_InitFieldVal()
        {
            string sql = "";
            Hashtable ht = new Hashtable();

            string ctrlType = this.GetRequestVal("CtrlType");
            int num = 1;

            switch (ctrlType)
            {
                case "Dtl":
                    sql = "SELECT COUNT(*) FROM Sys_MapDtl WHERE FK_MapData='" + this.FK_MapData + "'";
                    num = DBAccess.RunSQLReturnValInt(sql)+1;
                    ht.Add("No", this.FK_MapData + "Dtl" + num);
                    ht.Add("Name", "从表"+num);
                    break;
                case "AthMulti":
                    sql = "SELECT COUNT(*) FROM Sys_FrmAttachment WHERE FK_MapData='" + this.FK_MapData + "'";
                    num = DBAccess.RunSQLReturnValInt(sql)+1;
                    ht.Add("No",  "AthMulti" + num );
                    ht.Add("Name", "多附件"+num);
                    break;
                case "AthSingle":
                    sql = "SELECT COUNT(*) FROM Sys_FrmAttachment WHERE FK_MapData='" + this.FK_MapData + "'";
                    num = DBAccess.RunSQLReturnValInt(sql)+1;
                    ht.Add("No", "AthSingle" + num );
                    ht.Add("Name", "单附件"+num);
                    break;
                case "AthImg":
                    sql = "SELECT COUNT(*) FROM Sys_FrmImgAth WHERE FK_MapData='" + this.FK_MapData + "'";
                    num = DBAccess.RunSQLReturnValInt(sql)+1;
                    ht.Add("No", "AthImg" + num );
                    ht.Add("Name", "图片附件"+num);
                    break;

                case "HandSiganture": //手写板.
                    sql = "SELECT COUNT(*) FROM Sys_FrmEle WHERE FK_MapData='" + this.FK_MapData + "' AND EleType='"+ctrlType+"'";
                    num = DBAccess.RunSQLReturnValInt(sql)+1;
                    ht.Add("No", "iFrame" + num);
                    ht.Add("Name", "签字板"+num);
                    break;
                case "iFrame": //框架
                    sql = "SELECT COUNT(*) FROM Sys_FrmEle WHERE FK_MapData='" + this.FK_MapData + "' AND EleType='" + ctrlType + "'";
                    num = DBAccess.RunSQLReturnValInt(sql) + 1;
                    ht.Add("No", "iFrame" + num );
                    ht.Add("Name", "框架"+num);
                    break;
                default:
                    ht.Add("No", ctrlType +1);
                    ht.Add("Name", ctrlType+1);
                    break;
            }

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }

        #region 枚举界面.
        public string FrmTable_GetSFTableList()
        {
            int pageNumber = this.GetRequestValInt("pageNumber");
            if (pageNumber == 0)
                pageNumber = 1;

            int pageSize = this.GetRequestValInt("pageSize");
            if (pageSize == 0)
                pageSize = 9999;

            return BP.Sys.CCFormAPI.DB_SFTableList(pageNumber, pageSize);
        }
        public string FrmEnumeration_GetEnumerationList()
        {
            int pageNumber = this.GetRequestValInt("pageNumber");
            if (pageNumber == 0)
                pageNumber = 1;

            int pageSize = this.GetRequestValInt("pageSize");
            if (pageSize == 0)
                pageSize = 9999;

            return BP.Sys.CCFormAPI.DB_EnumerationList(pageNumber, pageSize); //调用API获得数据.
        }
        #endregion 枚举界面.


        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            string sql = "";
            switch (this.DoType)
            {
                case "FrmEnumeration_NewEnumField": //创建一个字段. 对应 FigureCreateCommand.js  里的方法.
                    UIContralType ctrl = UIContralType.RadioBtn;
                    string ctrlDoType = GetRequestVal("ctrlDoType");
                    if (ctrlDoType == "DDL")
                        ctrl = UIContralType.DDL;
                    else
                        ctrl = UIContralType.RadioBtn;

                    string fk_mapdata = this.GetRequestVal("FK_MapData");
                    string keyOfEn = this.GetRequestVal("KeyOfEn");
                    string fieldDesc = this.GetRequestVal("Name");
                    string enumKeyOfBind = this.GetRequestVal("UIBindKey"); //要绑定的enumKey.
                    float x = float.Parse(this.GetRequestVal("x"));
                    float y = float.Parse(this.GetRequestVal("y"));

                    BP.Sys.CCFormAPI.NewEnumField(fk_mapdata, keyOfEn, fieldDesc, enumKeyOfBind, ctrl, x, y);
                    return "绑定成功.";

                case "FrmEnumeration_SaveEnum":
                    string enumName = this.GetRequestVal("EnumName");
                    string enumKey1 = this.GetRequestVal("EnumKey");
                    string cfgVal = this.GetRequestVal("Vals");

                    //调用接口执行保存.
                    return BP.Sys.CCFormAPI.SaveEnum(enumKey1, enumName, cfgVal, false);
                case "FrmEnumeration_NewEnum"://杨玉慧加  当枚举已经存在时，提示，不再添加
                    string newnEumName = this.GetRequestVal("EnumName");
                    string newEnumKey1 = this.GetRequestVal("EnumKey");
                    string newCfgVal = this.GetRequestVal("Vals");

                    //调用接口执行保存.
                    return BP.Sys.CCFormAPI.SaveEnum(newEnumKey1, newnEumName, newCfgVal, true);
                case "FrmEnumeration_DelEnum":
                    //删除空数据.
                    BP.DA.DBAccess.RunSQL("DELETE FROM Sys_MapAttr WHERE FK_MapData IS NULL OR FK_MapData='' ");

                    //获得要删除的枚举值.
                    string enumKey = this.GetRequestVal("EnumKey");

                    // 检查这个物理表是否被使用.
                    sql = "SELECT  FK_MapData,KeyOfEn,Name FROM Sys_MapAttr WHERE UIBindKey='" + enumKey + "'";
                    DataTable dtEnum = DBAccess.RunSQLReturnTable(sql);
                    string msgDelEnum = "";
                    foreach (DataRow dr in dtEnum.Rows)
                    {
                        msgDelEnum += "\n 表单编号:" + dr["FK_MapData"] + " , 字段:" + dr["KeyOfEn"] + ", 名称:" + dr["Name"];
                    }

                    if (msgDelEnum != "")
                        return "error:该枚举已经被如下字段所引用，您不能删除它。" + msgDelEnum;

                    sql = "DELETE FROM Sys_EnumMain WHERE No='" + enumKey + "'";
                    sql += "@DELETE FROM Sys_Enum WHERE EnumKey='" + enumKey + "' ";
                    DBAccess.RunSQLs(sql);
                    return "执行成功.";
                case "NewSFTableField": //创建一个SFTable字段.

                    //string fk_mapdata = getUTF8ToString("FK_MapData");
                    //string keyOfEn = getUTF8ToString("KeyOfEn");
                    //string fieldDesc = getUTF8ToString("Name");
                    //string sftable = getUTF8ToString("UIBindKey");
                    //x = float.Parse(getUTF8ToString("x"));
                    //y = float.Parse(getUTF8ToString("y"));

                    //调用接口,执行保存.
                    BP.Sys.CCFormAPI.SaveFieldSFTable(this.FK_MapData, this.KeyOfEn, this.GetRequestVal("Name"),
                        this.GetRequestVal("UIBindKey"), this.GetRequestValFloat("x"), this.GetRequestValFloat("y"));
                    return "执行成功.";

                case "FrmTable_DelSFTable": /* 删除自定义的物理表. */
                    SFTable sfDel = new SFTable();
                    sfDel.No = this.GetRequestVal("FK_SFTable");
                    sfDel.Delete();
                    return "删除成功.";
                default:
                    break;
            }

            //找不不到标记就抛出异常.
            throw new Exception("@标记["+this.DoType+"]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        #region 功能界面 .
        /// <summary>
        /// 转化拼音
        /// </summary>
        /// <returns>返回转换后的拼音</returns>
        public string FrmTextBox_ParseStringToPinyin()
        {
            string name = getUTF8ToString("name");
            string flag = getUTF8ToString("flag");

            if (flag == "true")
                return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, true);

            return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, false);
        }
        #endregion 功能界面方法.
    }
}

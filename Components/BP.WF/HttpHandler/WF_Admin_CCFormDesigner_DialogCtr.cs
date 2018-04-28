using System;
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
    public class WF_Admin_CCFormDesigner_DialogCtr : DirectoryPageBase
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
        /// 获取隐藏字段
        /// </summary>
        /// <returns></returns>
        public string Hiddenfielddata()
        {
            return BP.Sys.CCFormAPI.DB_Hiddenfielddata(this.FK_MapData);
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
                case "ImgAth":
                    sql = "SELECT COUNT(*) FROM Sys_FrmImgAth WHERE FK_MapData='" + this.FK_MapData + "'";
                    num = DBAccess.RunSQLReturnValInt(sql) + 1;
                    ht.Add("No", "ImgAth" + num);
                    ht.Add("Name", "图片附件" + num);
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
                    ht.Add("No", "HandSiganture" + num);
                    ht.Add("Name", "签字板"+num);
                    break;
                case "iFrame": //框架
                    sql = "SELECT COUNT(*) FROM Sys_FrmEle WHERE FK_MapData='" + this.FK_MapData + "' AND EleType='" + ctrlType + "'";
                    num = DBAccess.RunSQLReturnValInt(sql) + 1;
                    ht.Add("No", "iFrame" + num );
                    ht.Add("Name", "框架"+num);
                    break;
                case "Fieldset": //分组
                    sql = "SELECT COUNT(*) FROM Sys_FrmEle WHERE FK_MapData='" + this.FK_MapData + "' AND EleType='" + ctrlType + "'";
                    num = DBAccess.RunSQLReturnValInt(sql) + 1;
                    ht.Add("No", "Fieldset" + num);
                    ht.Add("Name", "分组" + num);
                    break;
                default:
                    ht.Add("No", ctrlType +1);
                    ht.Add("Name", ctrlType+1);
                    break;
            }

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }

        #region 枚举界面.
        /// <summary>
        /// 获得外键列表.
        /// </summary>
        /// <returns></returns>
        public string FrmTable_GetSFTableList()
        {
            //WF_Admin_FoolFormDesigner wf = new WF_Admin_FoolFormDesigner(this.context);
            SFTables ens = new SFTables();
            ens.RetrieveAll();
            return ens.ToJson();
        }
        /// <summary>
        /// 获得枚举列表.
        /// </summary>
        /// <returns></returns>
        public string FrmEnumeration_Init()
        {
            WF_Admin_FoolFormDesigner wf = new WF_Admin_FoolFormDesigner(this.context);
            return wf.SysEnumList_Init();
        }
        #endregion 枚举界面.

        #region 执行父类的重写方法.
        /// <summary>
        /// 默认执行的方法
        /// </summary>
        /// <returns></returns>
        protected override string DoDefaultMethod()
        {
            //找不不到标记就抛出异常.
            throw new Exception("@标记["+this.DoType+"]，没有找到.");
        }
        #endregion 执行父类的重写方法.

        public string EnumerationNew_GetEnum()
        {
            string enumKey = this.GetRequestVal("EnumKey");

            SysEnumMain sem = new SysEnumMain(enumKey);
            Hashtable ht = new Hashtable();
            ht.Add("No", sem.No);
            ht.Add("Name", sem.Name);
            ht.Add("CfgVal", sem.CfgVal);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }
        public string FrmEnumeration_SaveEnum()
        {
            string enumName = this.GetRequestVal("EnumName");
            string enumKey1 = this.GetRequestVal("EnumKey");
            string cfgVal = this.GetRequestVal("Vals");

            //调用接口执行保存.
            return BP.Sys.CCFormAPI.SaveEnum(enumKey1, enumName, cfgVal, false);
        }

        /// <summary>
        /// //杨玉慧加  当枚举已经存在时，提示，不再添加
        /// </summary>
        /// <returns></returns>
        public string FrmEnumeration_NewEnum()
        {
            string newnEumName = this.GetRequestVal("EnumName");
            string newEnumKey1 = this.GetRequestVal("EnumKey");
            string newCfgVal = this.GetRequestVal("Vals");

            //调用接口执行保存.
            return BP.Sys.CCFormAPI.SaveEnum(newEnumKey1, newnEumName, newCfgVal, true);
        }
        /// <summary>
        /// 删除枚举值
        /// </summary>
        /// <returns></returns>
        public string FrmEnumeration_DelEnum()
        {
            string sql = "";
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
                return "err@该枚举已经被如下字段所引用，您不能删除它: " + msgDelEnum;

            sql = "DELETE FROM Sys_EnumMain WHERE No='" + enumKey + "'";
            sql += "@DELETE FROM Sys_Enum WHERE EnumKey='" + enumKey + "' ";
            DBAccess.RunSQLs(sql);

            return "执行成功.";
        }

        #region 功能界面 .
        /// <summary>
        /// 转化拼音
        /// </summary>
        /// <returns>返回转换后的拼音</returns>
        public string FrmTextBox_ParseStringToPinyin()
        {
            string name = GetRequestVal("name");
            string flag = this.GetRequestVal("flag");
            flag = DataType.IsNullOrEmpty(flag) == true ? "true" : flag.ToLower();

            //此处配置最大长度为20，edited by liuxc,2017-9-25
            return BP.Sys.CCFormAPI.ParseStringToPinyinField(name, Equals(flag, "true"), true, 20);
        }
        #endregion 功能界面方法.

        /// <summary>
        /// 获得表单对应的物理表特定的数据类型字段
        /// </summary>
        /// <returns></returns>
        public string FrmTextBoxChoseOneField_Init()
        {
            DataTable mydt = MapData.GetFieldsOfPTableMode2(this.FK_MapData);
            mydt.TableName = "dt";
            return BP.Tools.Json.ToJson(mydt);
        }
    }
}

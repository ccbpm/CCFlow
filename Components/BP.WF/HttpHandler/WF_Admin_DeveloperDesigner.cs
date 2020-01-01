using System;
using System.IO;
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
using BP.Frm;

namespace BP.WF.HttpHandler
{
    public class WF_Admin_DevelopDesigner : BP.WF.HttpHandler.DirectoryPageBase
    {

        #region 执行父类的重写方法.
        /// <summary>
        /// 构造函数
        /// </summary>
        public WF_Admin_DevelopDesigner()
        {

        }
        /// <summary>
        /// 表单初始化
        /// </summary>
        /// <returns></returns>
        public string Designer_Init()
        {
            string htmlCode = BP.DA.DBAccess.GetBigTextFromDB("Sys_MapData", "No", this.FK_MapData, "HtmlTemplateFile");
            //把数据同步到DataUser/CCForm/HtmlTemplateFile/文件夹下
            string filePath = BP.Sys.SystemConfig.PathOfDataUser + "CCForm\\HtmlTemplateFile\\";
            if (Directory.Exists(filePath) == false)
                Directory.CreateDirectory(filePath);
            filePath = filePath + this.FK_MapData + ".htm";
            //写入到html 中
            BP.DA.DataType.WriteFile(filePath, htmlCode);
            return htmlCode;

        }
        /// <summary>
        /// 保存表单
        /// </summary>
        /// <returns></returns>
        public string SaveForm()
        {
            //获取html代码
            string htmlCode = this.GetRequestVal("HtmlCode");
            if (DataType.IsNullOrEmpty(htmlCode) == false)
            {
                //保存到DataUser/CCForm/HtmlTemplateFile/文件夹下
                string filePath = BP.Sys.SystemConfig.PathOfDataUser + "CCForm\\HtmlTemplateFile\\";
                if (Directory.Exists(filePath) == false)
                    Directory.CreateDirectory(filePath);
                filePath = filePath + this.FK_MapData + ".htm";
                //写入到html 中
                BP.DA.DataType.WriteFile(filePath, htmlCode);

                //保存类型。
                MapData md = new MapData(this.FK_MapData);
                if (md.HisFrmType != FrmType.Develop)
                {
                    md.HisFrmType = FrmType.Develop;
                    md.Update();
                }

                // HtmlTemplateFile 保存到数据库中
                BP.DA.DBAccess.SaveBigTextToDB(htmlCode, "Sys_MapData", "No", this.FK_MapData, "HtmlTemplateFile");
                return "保存成功";
            }
            return "保存成功.";
        }
        #endregion

        public string Fields_Init()
        {
            string html = BP.DA.DBAccess.GetBigTextFromDB("Sys_MapData", "No", 
                this.FrmID, "HtmlTemplateFile");
            return html;
        }

        /// <summary>
        /// 表单重置
        /// </summary>
        /// <returns></returns>
        public string ResetFrm_Init()
        {
            //删除html
            string filePath = BP.Sys.SystemConfig.PathOfDataUser + "CCForm\\HtmlTemplateFile\\" + this.FK_MapData + ".htm";
            if (File.Exists(filePath) == true)
                File.Delete(filePath);

            //删除存储的html代码
            string sql = "UPDATE Sys_MapData SET HtmlTemplateFile='' WHERE No='" + this.FK_MapData + "'";
            DBAccess.RunSQL(sql);
            //删除MapAttr中的数据
            sql = "Delete Sys_MapAttr WHERE FK_MapData='" + this.FK_MapData + "'";
            DBAccess.RunSQL(sql);
            //删除MapExt中的数据
            sql = "Delete Sys_MapExt WHERE FK_MapData='" + this.FK_MapData + "'";
            DBAccess.RunSQL(sql);

            return "重置成功";
        }

        #region 复制表单
        /// <summary>
        /// 复制表单属性和表单内容
        /// </summary>
        /// <param name="frmId">新表单ID</param>
        /// <param name="frmName">新表单内容</param>
        public void DoCopyFrm()
        {
            string fromFrmID = GetRequestVal("FromFrmID");
            string toFrmID = GetRequestVal("ToFrmID");
            string toFrmName = GetRequestVal("ToFrmName");
            #region 原表单信息
            //表单信息
            MapData fromMap = new MapData(fromFrmID);
            //单据信息
            FrmBill fromBill = new FrmBill();
            fromBill.No = fromFrmID;
            int billCount = fromBill.RetrieveFromDBSources();
            //实体单据
            FrmDict fromDict = new FrmDict();
            fromDict.No = fromFrmID;
            int DictCount = fromDict.RetrieveFromDBSources();
            #endregion 原表单信息

            #region 复制表单
            MapData toMapData = new MapData();
            toMapData = fromMap;
            toMapData.No = toFrmID;
            toMapData.Name = toFrmName;
            toMapData.Insert();
            if (billCount != 0)
            {
                FrmBill toBill = new FrmBill();
                toBill = fromBill;
                toBill.No = toFrmID;
                toBill.Name = toFrmName;
                toBill.EntityType = EntityType.FrmBill;
                toBill.Update();
            }
            if (DictCount != 0)
            {
                FrmDict toDict = new FrmDict();
                toDict = fromDict;
                toDict.No = toFrmID;
                toDict.Name = toFrmName;
                toDict.EntityType = EntityType.FrmDict;
                toDict.Update();
            }
            #endregion 复制表单

            MapData.ImpMapData(toFrmID, BP.Sys.CCFormAPI.GenerHisDataSet_AllEleInfo(fromFrmID));

            //清空缓存
            toMapData.RepairMap();
            BP.Sys.SystemConfig.DoClearCash();


        }
        #endregion 复制表单

    }
}

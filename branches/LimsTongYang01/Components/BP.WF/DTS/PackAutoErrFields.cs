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
    /// 修复非法字段名称
    /// </summary>
    public class PackAutoErrFormatFieldTable : Method
    {
        /// <summary>
        /// 修复非法字段名称
        /// </summary>
        public PackAutoErrFormatFieldTable()
        {
            this.Title = "修复非法字段名称,物理表名称";
            this.Help = "在以前的版本中，用户创建表单物理表名、字段名的合法性没有检查会造成系统在自动创建物理表修复物理表时出现错误。此补丁可以批量修复全局的表单。";
            // this.Warning = "您确定要执行吗？";
            // this.HisAttrs.AddTBString("Path", "C:\\ccflow.Template", "生成的路径", true, false, 1, 1900, 200);
        }
        /// <summary>
        /// 设置执行变量
        /// </summary>
        /// <returns></returns>
        public override void Init()
        {
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
            string keys = "~!@#$%^&*()+{}|:<>?`=[];,./～！＠＃￥％……＆×（）——＋｛｝｜：“《》？｀－＝［］；＇，．／";
            char[] cc = keys.ToCharArray();
            foreach (char c in cc)
            {
                DBAccess.RunSQL("update sys_mapattr set keyofen=REPLACE(keyofen,'" + c + "' , '')");
            }

            BP.Sys.MapAttrs attrs = new Sys.MapAttrs();
            attrs.RetrieveAll();
            int idx = 0;
            string msg = "";
            foreach (BP.Sys.MapAttr item in attrs)
            {
                string f = item.KeyOfEn.Clone().ToString();
                try
                {
                    int i = int.Parse( item.KeyOfEn.Substring(0, 1) );
                    item.KeyOfEn = "F" + item.KeyOfEn;
                    try
                    {
                        MapAttr itemCopy = new MapAttr();
                        itemCopy.Copy(item);
                        itemCopy.Insert();
                        item.DirectDelete();
                    }
                    catch (Exception ex)
                    {
                        msg += "@" + ex.Message;
                    }
                }
                catch
                {
                    continue;
                }
                DBAccess.RunSQL("UPDATE sys_mapAttr set KeyOfEn='"+item.KeyOfEn+"', mypk=FK_MapData+'_'+keyofen where keyofen='"+item.KeyOfEn+"'");
                msg += "@第(" + idx + ")个错误修复成功，原（"+f+"）修复成("+item.KeyOfEn+").";
                idx++;
            }

            BP.DA.DBAccess.RunSQL("UPDATE Sys_MapAttr SET MyPK=FK_MapData+'_'+KeyOfEn WHERE MyPK!=FK_MapData+'_'+KeyOfEn");
            return "修复信息如下:"+msg;
        }
    }
}

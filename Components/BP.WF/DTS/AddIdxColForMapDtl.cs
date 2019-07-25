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
    /// 生成模版的垃圾数据 
    /// </summary>
    public class AddIdxColForMapDtl : Method
    {
        /// <summary>
        /// 生成模版的垃圾数据
        /// </summary>
        public AddIdxColForMapDtl()
        {
            this.Title = "为所有的从表增加一个隐藏的Id列.";
            this.Help = "用户VSTO表单.";
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
            MapDtls dtls = new MapDtls();
            dtls.RetrieveAll();

            foreach (MapDtl item in dtls)
            {
                MapAttr ma = new MapAttr();
                ma.MyPK = item.No + "_Idx";
                if (ma.IsExits == true)
                    continue;

                ma.FK_MapData = item.No;
                ma.KeyOfEn = "Idx";
                ma.Name = "Idx";
                ma.LGType = FieldTypeS.Normal;
                ma.UIVisible = false;
                ma.DefVal = "0";
                ma.MyDataType = DataType.AppInt;
                ma.Insert();

                GEDtl dtl = new GEDtl(item.No);
                dtl.CheckPhysicsTable();
            }
            return "执行成功.";
        }
    }
}

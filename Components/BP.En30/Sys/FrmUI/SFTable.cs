using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.IO;
using System.Net;
using System.Xml;
using BP.DA;
using BP.En;
using Microsoft.CSharp;
using System.Web.Services.Description;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFTable : EntityNoName
    {

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFTable()
        {
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Sys_SFTable", "字典表");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddTBStringPK(SFTableAttr.No, null, "表英文名称", true, false, 1, 200, 20);
                map.AddTBString(SFTableAttr.Name, null, "表中文名称", true, false, 0, 200, 20);

                map.AddDDLSysEnum(SFTableAttr.SrcType, 0, "数据表类型", true, false, SFTableAttr.SrcType,
                    "@0=本地的类@1=创建表@2=表或视图@3=SQL查询表@4=WebServices");

                map.AddDDLSysEnum(SFTableAttr.CodeStruct, 0, "字典表类型", true, false, SFTableAttr.CodeStruct);

                map.AddTBString(SFTableAttr.FK_Val, null, "默认创建的字段名", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.TableDesc, null, "表描述", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.DefVal, null, "默认值", true, false, 0, 200, 20);

                //数据源.
                map.AddDDLEntities(SFTableAttr.FK_SFDBSrc, "local", "数据源", new BP.Sys.SFDBSrcs(), true);
                map.AddTBString(SFTableAttr.SrcTable, null, "表/视图", true, false, 0, 200, 20);

                map.AddTBString(SFTableAttr.ColumnValue, null, "显示的值(编号列/默认为No)", true, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.ColumnText, null, "显示的文字(名称列默认为Name)", true, false, 0, 200, 20);


                map.AddTBString(SFTableAttr.ParentValue, null, "父级值(父级列)", false, false, 0, 200, 20);
                map.AddTBString(SFTableAttr.SelectStatement, null, "查询语句", false, false, 0, 1000, 600, true);

                map.AddTBDateTime(SFTableAttr.RDT, null, "加入日期", false, false);

                RefMethod rm = new RefMethod();
                rm.Title = "查看数据";
                rm.ClassMethodName = this.ToString() + ".DoEdit";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.IsForEns = false;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 更新的操作
        /// </summary>
        /// <returns></returns>
        protected override bool beforeUpdate()
        {
            BP.Sys.SFTable sf = new BP.Sys.SFTable(this.No);
            sf.Copy(this);
            sf.Update();

            return base.beforeUpdate();
        }

        protected override void afterInsertUpdateAction()
        {
            BP.Sys.SFTable sftable = new BP.Sys.SFTable();
            sftable.No = this.No;
            sftable.RetrieveFromDBSources();
            sftable.Update();

            base.afterInsertUpdateAction();
        }


        /// <summary>
        /// 编辑数据
        /// </summary>
        /// <returns></returns>
        public string DoEdit()
        {
            return SystemConfig.CCFlowWebPath + "WF/Admin/FoolFormDesigner/SFTableEditData.htm?FK_SFTable=" + this.No;
        }
        /// <summary>
        /// 执行删除.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeDelete()
        {
            BP.Sys.SFTable sf = new Sys.SFTable(this.No);
            sf.Delete();
            return base.beforeDelete();
        }
        protected override bool beforeInsert()
        {
            //利用这个时间串进行排序.
            this.SetValByKey("RDT", DataType.CurrentDataTime);
            return base.beforeInsert();
        }
    }
    /// <summary>
    /// 用户自定义表s
    /// </summary>
    public class SFTables : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFTables()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFTable();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFTable> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFTable>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFTable> Tolist()
        {
            System.Collections.Generic.List<SFTable> list = new System.Collections.Generic.List<SFTable>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFTable)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

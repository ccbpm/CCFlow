﻿using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Web;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// Boolen字段
    /// </summary>
    public class MapAttrBoolen : EntityMyPK
    {
        #region 文本字段参数属性.
        public string DefVal
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.DefVal);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.DefVal, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.KeyOfEn, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// Boolen字段
        /// </summary>
        public MapAttrBoolen()
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

                Map map = new Map("Sys_MapAttr", "Boolen字段");
                map.IndexField = MapAttrAttr.FK_MapData;

                #region 基本信息.

                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20, true); 
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                //数据类型.
                map.AddDDLSysEnum(MapAttrAttr.MyDataType, 4, "数据类型", true, false);
                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);

                map.AddTBString(MapAttrAttr.DefVal, "0", "默认值(是否选中？0=否,1=是)", true, false, 0, 10, 20);

                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.AddTBStringDoc(MapAttrAttr.Tip, null, "激活提示", true, false); 
                #endregion 基本信息.

                #region 傻瓜表单。
                //单元格数量 2013-07-24 增加。
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrDT",
                  "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, 0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false); 

                //CCS样式
                map.AddDDLSQL(MapAttrAttr.CSSCtrl, "0", "自定义样式", MapAttrString.SQLOfCSSAttr, true);
                #endregion 傻瓜表单。


                RefMethod rm = new RefMethod();
                rm.Title = "事件绑函数";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "高级JS设置";
                rm.ClassMethodName = this.ToString() + ".DoCheckboxs()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "全局风格定义";
                rm.ClassMethodName = this.ToString() + ".DoGloValStyles()";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                rm.Icon = "icon-wrench";
                rm.RefAttrKey = MapAttrAttr.CSSCtrl;
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.setMyPK(this.MyPK);
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            #region 修改默认值.
            //如果没默认值.
            if (DataType.IsNullOrEmpty(this.DefVal)==true)
                this.DefVal = "0";
            MapData md = new MapData();
            md.No = this.FK_MapData;
            if (md.RetrieveFromDBSources() == 1)
                BP.DA.DBAccess.UpdateTableColumnDefaultVal(md.PTable, this.KeyOfEn, int.Parse(this.DefVal));
            #endregion 修改默认值.

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //删除相对应的rpt表中的字段
            if (this.FK_MapData.Contains("ND") == true)
            {
                string fk_mapData = this.FK_MapData.Substring(0, this.FK_MapData.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND KeyOfEn='" + this.KeyOfEn + "'";
                DBAccess.RunSQL(sql);
            }
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
        }
        #endregion

        #region 基本功能.
        public string DoGloValStyles()
        {
            return "../../Admin/FoolFormDesigner/StyletDfine/GloValStyles.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + this.MyPK;
        }
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn+"&D="+DateTime.Now;
        }
        /// <summary>
        /// 高级设置
        /// </summary>
        /// <returns></returns>
        public  String DoCheckboxs() 
        {
			
		    return "../../Admin/FoolFormDesigner/MapExt/RadioBtns.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
    #endregion
}
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrBoolens : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrBoolens()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrBoolen();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrBoolen> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrBoolen>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrBoolen> Tolist()
        {
            System.Collections.Generic.List<MapAttrBoolen> list = new System.Collections.Generic.List<MapAttrBoolen>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrBoolen)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

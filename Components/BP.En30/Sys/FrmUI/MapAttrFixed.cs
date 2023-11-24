using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Web;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 系统定位组件
    /// </summary>
    public class MapAttrFixed : EntityMyPK
    {
        #region 文本字段参数属性.
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FrmID
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
        /// 实体属性
        /// </summary>
        public MapAttrFixed()
        {
        }
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttrFixed(string myPK)
        {
            this.setMyPK(myPK);
            this.Retrieve();

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

                Map map = new Map("Sys_MapAttr", "系统定位组件");

                #region 基本字段信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", false, false, 1, 100, 20);
                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20, true);

                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                map.AddTBInt(MapAttrAttr.MinLen, 0, "最小长度", true, false);
                map.AddTBInt(MapAttrAttr.MaxLen, 50, "最大长度", true, false);
                map.SetHelperAlert(MapAttrAttr.MaxLen, "定义该字段的字节长度.");


                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.SetHelperAlert(MapAttrAttr.UIWidth, "对自由表单,从表有效,显示文本框的宽度.");
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否启用？", true, true);

                map.AddTBInt(MapAttrAttr.UIContralType, 0, "控件", true, false);

                map.AddDDLSQL(MapAttrAttr.CSSCtrl, "0", "自定义样式", MapAttrString.SQLOfCSSAttr, true);
                #endregion 基本字段信息.

                #region 傻瓜表单
                //单元格数量 2013-07-24 增加
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "TextBox单元格数量", true, true, "ColSpanAttrDT",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨5个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.ColSpan, "对于傻瓜表单有效: 标识该字段TextBox横跨的宽度,占的单元格数量.");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.LabelColSpan, 1, "Label单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格@5=跨6个单元格@6=跨6个单元格");
                map.SetHelperAlert(MapAttrAttr.LabelColSpan, "对于傻瓜表单有效: 标识该字段Lable，标签横跨的宽度,占的单元格数量.");

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID,0, "显示的分组", MapAttrString.SQLOfGroupAttr, true);

                
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false);
                map.SetHelperAlert(MapAttrAttr.Idx, "对傻瓜表单有效:用于调整字段在同一个分组中的顺序.");

                #endregion 傻瓜表单

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeUpdateInsertAction()
        {
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.MyPK);
            attr.RetrieveFromDBSources();

            //强制设置为评论组件.
            this.SetValByKey(MapAttrAttr.UIContralType,(int)UIContralType.Location);

            if (this.GetValStrByKey("GroupID").Equals("无")==true)
                this.SetValByKey("GroupID", "0");

            return base.beforeUpdateInsertAction();
        }



        /// <summary>
        /// 删除
        /// </summary>
        protected override void afterDelete()
        {
            //删除经度纬度的字段
            MapAttr mapAttr = new MapAttr(this.FrmID + "_JD");
            mapAttr.Delete();

            mapAttr = new MapAttr(this.FrmID + "_WD");
            mapAttr.Delete();

            //删除相对应的rpt表中的字段
            if (this.FrmID.Contains("ND") == true)
            {
                string fk_mapData = this.FrmID.Substring(0, this.FrmID.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND( KeyOfEn='" + this.KeyOfEn +"' OR KeyOfEn='JD' OR KeyOfEn='WD')";
                DBAccess.RunSQL(sql);
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);

            base.afterDelete();
        }


        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.setMyPK(this.MyPK);
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //判断表单中是否存在经度、维度字段
            mapAttr = new MapAttr();
            mapAttr.setMyPK(this.FrmID + "_" + "JD");
            if (mapAttr.RetrieveFromDBSources() == 0)
            {
                mapAttr.FrmID =this.FrmID;
                mapAttr.setKeyOfEn("JD");
                mapAttr.setName("经度");
                mapAttr.GroupID = 1;
                mapAttr.setUIContralType(UIContralType.TB);
                mapAttr.MyDataType = 1;
                mapAttr.LGType = 0;
                mapAttr.setUIVisible(false);
                mapAttr.setUIIsEnable(false);
                mapAttr.UIIsInput = true;
                mapAttr.UIWidth = 150;
                mapAttr.UIHeight = 23;
                mapAttr.Insert(); //插入字段.
            }

            mapAttr.setMyPK(this.FrmID + "_" + "WD");
            if (mapAttr.RetrieveFromDBSources() == 0)
            {
                mapAttr.FrmID =this.FrmID;
                mapAttr.setKeyOfEn("WD");
                mapAttr.setName("纬度");
                mapAttr.GroupID = 1;
                mapAttr.setUIContralType(UIContralType.TB);
                mapAttr.MyDataType = 1;
                mapAttr.LGType = 0;
                mapAttr.setUIVisible(false);
                mapAttr.setUIIsEnable(false);
                mapAttr.UIIsInput = true;
                mapAttr.UIWidth = 150;
                mapAttr.UIHeight = 23;
                mapAttr.Insert(); //插入字段.
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FrmID);

            base.afterInsertUpdateAction();
        }
        #endregion
    }
    /// <summary>
    /// 系统定位组件s
    /// </summary>
    public class MapAttrFixeds : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 系统定位组件s
        /// </summary>
        public MapAttrFixeds()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrFixed();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrFixed> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrFixed>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrFixed> Tolist()
        {
            System.Collections.Generic.List<MapAttrFixed> list = new System.Collections.Generic.List<MapAttrFixed>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrFixed)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

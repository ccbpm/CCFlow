using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 框架
    /// </summary>
    public class MapFrameExt : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 连接
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStrByKey("Tag1");
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(MapFrameAttr.FK_MapData);
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStrByKey(MapFrameAttr.Name);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                //if (Web.WebUser.No.Equals("admin"))
                //{
                uac.IsUpdate = true;
                uac.IsDelete = true;
                uac.IsInsert = false;
                //}
                return uac;
            }
        }
        /// <summary>
        /// 框架
        /// </summary>
        public MapFrameExt()
        {

        }
        /// <summary>
        /// 框架
        /// </summary>
        /// <param name="mypk"></param>
        public MapFrameExt(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("Sys_MapFrame", "框架");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);


                map.AddMyPK();
                map.AddTBString(MapFrameAttr.FK_MapData, null, "表单ID", true, true, 0, 100, 20);
                map.AddTBString(MapFrameAttr.Name, null, "名称", true, false, 0, 200, 20, true);

                map.AddDDLSysEnum(MapFrameAttr.UrlSrcType, 0, "URL来源", true, true, MapFrameAttr.UrlSrcType,
                    "@0=自定义@1=地图@2=流程轨迹表@3=流程轨迹图");
                map.AddTBString(MapFrameAttr.URL, null, "URL", true, false, 0, 3000, 20, true);
               
                //显示的分组.
               // map.AddDDLSQL(MapFrameAttr.FrmID, "0", "表单表单","SELECT No, Name FROM Sys_Mapdata  WHERE  FrmType=3 ", true);

                map.AddTBString(FrmEleAttr.Y, null, "Y", true, false, 0, 20, 20);
                map.AddTBString(FrmEleAttr.X, null, "x", true, false, 0, 20, 20);

                map.AddTBString(FrmEleAttr.W, null, "宽度", true, false, 0, 20, 20);
                map.AddTBString(FrmEleAttr.H, null, "高度", true, false, 0, 20, 20);

                map.AddBoolean(MapFrameAttr.IsAutoSize, true, "是否自动设置大小", false, false);

                map.AddTBString(FrmEleAttr.EleType, null, "类型", false, false, 0, 50, 20, true);

                map.AddTBString(FrmEleAttr.GUID, null, "GUID", false, false, 0, 128, 20);

                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false); //@李国文

                #region 执行的方法.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "预制";
                rm.ClassMethodName = this.ToString() + ".DoFrameExt()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 框架扩展.
        /// <summary>
        /// 框架扩展
        /// </summary>
        /// <returns></returns>
        public string DoFrameExt()
        {
            return "../../Admin/FoolFormDesigner/FrameExt/Default.htm?MyPK=" + this.MyPK;
        }
        #endregion 框架扩展.


        protected override void afterDelete()
        {
            //删除分组信息.
            GroupField gf = new GroupField();
            gf.Delete(GroupFieldAttr.CtrlID,this.MyPK);

            base.afterDelete();
        }

        protected override bool beforeUpdateInsertAction()
        {
            int val = this.GetValIntByKey(MapFrameAttr.UrlSrcType, 0);
            if (val == 1)
            {
                string sql = "SELECT Url FROM Sys_MapData WHERE No='" + this.GetValStrByKey(MapFrameAttr.FrmID) + "'";
                string url = DBAccess.RunSQLReturnStringIsNull(sql, "");
                this.SetValByKey(MapFrameAttr.URL, url);
            }

            //更新group.
            GroupField gf = new GroupField();
            int i= gf.Retrieve(GroupFieldAttr.FrmID, this.FK_MapData, GroupFieldAttr.CtrlID, this.MyPK);
            if (i == 1)
            {
                gf.Lab = this.Name;
                gf.Update();
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            MapFrame mapframe = new MapFrame();
            mapframe.MyPK = this.MyPK;
            mapframe.RetrieveFromDBSources();
            mapframe.Update();

            base.afterInsertUpdateAction();
        }
    }
    /// <summary>
    /// 框架s
    /// </summary>
    public class MapFrameExts : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 框架s
        /// </summary>
        public MapFrameExts()
        {
        }
        /// <summary>
        /// 框架s
        /// </summary>
        /// <param name="frmID">表单ID</param>
        public MapFrameExts(string frmID)
        {
            this.Retrieve(MapFrameAttr.FK_MapData, frmID);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapFrameExt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapFrameExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapFrameExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapFrameExt> Tolist()
        {
            System.Collections.Generic.List<MapFrameExt> list = new System.Collections.Generic.List<MapFrameExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapFrameExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

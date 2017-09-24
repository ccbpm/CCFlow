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
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsUpdate = true;
                uac.IsDelete = false;
                uac.IsInsert = false;
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

                Map map = new Map("Sys_FrmEle", "框架");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(FrmEleAttr.FK_MapData, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(FrmEleAttr.URL, null, "URL(支持ccbpm的表达式)", true, false, 0, 3900, 20, true);
                map.AddTBString(FrmBtnAttr.GUID, null, "GUID", false, false, 0, 128, 20);

                map.AddTBString(FrmBtnAttr.GroupID, null, "所在分组", false, false, 0, 128, 20);

                ////显示的分组.
                //map.AddDDLSQL(MapAttrAttr.GroupID, "0", "所在分组",
                //    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE EnName='@FK_MapData'", true);
             
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
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

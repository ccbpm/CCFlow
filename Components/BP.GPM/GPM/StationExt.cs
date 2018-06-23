using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 岗位
    /// </summary>
    public class StationExt : EntityNoName
    {
        #region 属性
        public string FK_StationExtType
        {
            get
            {
                return this.GetValStrByKey(StationAttr.FK_StationType);
            }
            set
            {
                this.SetValByKey(StationAttr.FK_StationType, value);
            }
        }
        #endregion

        #region 实现基本的方方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 岗位
        /// </summary> 
        public StationExt()
        {
        }
        /// <summary>
        /// 岗位
        /// </summary>
        /// <param name="_No"></param>
        public StationExt(string _No) : base(_No) { }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Station","岗位");

                map.Java_SetEnType(EnType.Admin);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetCodeStruct("4");

                map.AddTBStringPK(StationAttr.No, null, "编号", true, true, 4, 4, 36);
                map.AddTBString(StationAttr.Name, null, "名称", true, false, 0, 100, 200);
                map.AddDDLEntities(StationAttr.FK_StationType, null, "类型", new StationTypes(), true);
                map.AddTBString(StationAttr.OrgNo, null, "隶属组织", true, false, 0, 50, 250);
                map.AddSearchAttr(StationAttr.FK_StationType);



                //岗位绑定菜单
                map.AttrsOfOneVSM.AddBranches(new StationMenus(), new BP.GPM.Menus(),
                   BP.GPM.StationMenuAttr.FK_Station,
                   BP.GPM.StationMenuAttr.FK_Menu, "绑定菜单", EmpAttr.Name, EmpAttr.No, "0");


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 岗位s
    /// </summary>
    public class StationExts : EntitiesNoName
    {
        /// <summary>
        /// 岗位
        /// </summary>
        public StationExts() { }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new BP.GPM.StationExt();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<StationExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<StationExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<StationExt> Tolist()
        {
            System.Collections.Generic.List<StationExt> list = new System.Collections.Generic.List<StationExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((StationExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

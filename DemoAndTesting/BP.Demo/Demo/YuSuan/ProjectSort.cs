using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Demo.YS
{
    /// <summary>
    /// 项目类型属性
    /// </summary>
    public class ProjectSortAttr : EntityNoNameAttr
    {
    }
    /// <summary>
    /// 项目类型
    /// </summary>
    public class ProjectSort : EntityNoName
    {
        #region 实现基本的方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        public new string Name
        {
            get
            {
                return this.GetValStrByKey("Name");
            }
        }
        public int Grade
        {
            get
            {
                return this.No.Length / 2;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 项目类型
        /// </summary> 
        public ProjectSort()
        {
        }
        /// <summary>
        /// 项目类型
        /// </summary>
        /// <param name="_No"></param>
        public ProjectSort(string _No) : base(_No) { }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_YS_ProjectSort");
                map.EnDesc = "项目类型"; // "项目类型";
                map.EnType = EnType.Admin;
                map.DepositaryOfMap = Depositary.Application;
                map.DepositaryOfEntity = Depositary.Application;
                map.CodeStruct = "2"; // 最大级别是 7 .
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(ProjectSortAttr.No, null, "编号", true, true, 2, 2, 2);
                map.AddTBString(ProjectSortAttr.Name, null, "名称", true, false, 0, 100, 100);

                //map.AddDDLSysEnum(ProjectSortAttr.StaGrade, 0, "类型", true, true, ProjectSortAttr.StaGrade,
                //    "@1=高层岗@2=中层岗@3=执行岗");


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 项目类型s
    /// </summary>
    public class ProjectSorts : EntitiesNoName
    {
        /// <summary>
        /// 项目类型
        /// </summary>
        public ProjectSorts() { }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new ProjectSort();
            }
        }
    }
}

using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Demo.YS
{
    /// <summary>
    /// ��Ŀ��������
    /// </summary>
    public class ProjectSortAttr : EntityNoNameAttr
    {
    }
    /// <summary>
    /// ��Ŀ����
    /// </summary>
    public class ProjectSort : EntityNoName
    {
        #region ʵ�ֻ����ķ���
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

        #region ���췽��
        /// <summary>
        /// ��Ŀ����
        /// </summary> 
        public ProjectSort()
        {
        }
        /// <summary>
        /// ��Ŀ����
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

                Map map = new Map("Demo_YS_ProjectSort", "��Ŀ����");

                map.Java_SetEnType(EnType.Admin);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetCodeStruct("2"); // ��󼶱��� 7 .
                map.IsAutoGenerNo = true;

                map.AddTBStringPK(ProjectSortAttr.No, null, "���", true, true, 2, 2, 2);
                map.AddTBString(ProjectSortAttr.Name, null, "����", true, false, 0, 100, 100);

                //map.AddDDLSysEnum(ProjectSortAttr.StaGrade, 0, "����", true, true, ProjectSortAttr.StaGrade,
                //    "@1=�߲��@2=�в��@3=ִ�и�");

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// ��Ŀ����s
    /// </summary>
    public class ProjectSorts : EntitiesNoName
    {
        /// <summary>
        /// ��Ŀ����
        /// </summary>
        public ProjectSorts() { }
        /// <summary>
        /// �õ����� Entity
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

using System;
using System.Collections.Generic;
using System.Text;
using BP.En;
using BP.Sys;

namespace BP.WF.Data
{
    /// <summary>
    ///  属性 
    /// </summary>
    public class MyDeptEmpAttr : BP.En.EntityNoNameAttr
    {
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Dept = "FK_Dept";
    }
    /// <summary>
    /// 报表
    /// </summary>
    public class MyDeptEmp : BP.En.EntityNoName
    {
        #region attrs - attrs 
        public string RptName = null;
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Emp", "流程数据");
                map.Java_SetEnType(EnType.View);

                map.AddTBStringPK(MyDeptEmpAttr.No, null, "编号", false, false, 0, 100, 100);
                map.AddTBString(MyDeptEmpAttr.Name, null, "名称", false, false, 0, 100, 100);
                map.AddTBString(MyDeptEmpAttr.FK_Dept, null, "部门", false, false, 0, 100, 100);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion attrs
 
    }
    /// <summary>
    /// 报表集合
    /// </summary>
    public class MyDeptEmps : BP.En.EntitiesNoName
    {
        /// <summary>
        /// 报表集合
        /// </summary>
        public MyDeptEmps()
        {
        }

        public override Entity GetNewEntity
        {
            get
            {
                return new MyDeptEmp();
            }
        }
        public override int RetrieveAll()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(MyDeptEmpAttr.FK_Dept, BP.Web.WebUser.FK_Dept);
            return qo.DoQuery();
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MyDeptEmp> ToJavaList()
        {
            return (System.Collections.Generic.IList<MyDeptEmp>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MyDeptEmp> Tolist()
        {
            System.Collections.Generic.List<MyDeptEmp> list = new System.Collections.Generic.List<MyDeptEmp>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MyDeptEmp)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

    }
}

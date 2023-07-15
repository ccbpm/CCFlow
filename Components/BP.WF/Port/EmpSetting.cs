using System;
using System.Text.RegularExpressions;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Port
{
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class EmpSetting : EntityNoName
    {
        #region 构造函数
        /// <summary>
        /// 操作员f
        /// </summary>
        public EmpSetting()
        {
        }
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                    uac.OpenAll();
                else
                    uac.Readonly();
                uac.IsInsert = false;
                uac.IsDelete = false;
                return uac;
            }
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Emp", "我的设置");
                map.setEnType(EnType.App);
                map.IndexField = EmpAttr.FK_Dept;

                #region 字段
                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpAttr.No, null, "手机号/ID", false, false, 1, 500, 90);
                map.AddTBString(EmpAttr.UserID, null, "登陆ID", true, true, 0, 100, 10);
                map.AddTBString(EmpAttr.Name, null, "姓名", true, false, 0, 500, 130);
                map.AddDDLEntities(EmpAttr.FK_Dept, null, "当前部门", new BP.Port.Depts(), false);

                //状态. 0=启用，1=禁用.
                //  map.AddTBInt(EmpAttr.EmpSta, 0, "EmpSta", false, false);
                map.AddTBString(EmpAttr.Leader, null, "部门领导", false, false, 0, 100, 10);
                map.AddTBString(EmpAttr.Tel, null, "电话", true, false, 0, 20, 130, true);
                map.AddTBString(EmpAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(EmpAttr.PinYin, null, "拼音", false, false, 0, 1000, 132, false);
                map.AddTBString(EmpAttr.OrgNo, null, "OrgNo", true, true, 0, 500, 132, false);
                #endregion 字段

                #region 相关方法.
                RefMethod rm = new RefMethod();
                rm.Title = "设置图片签名";
                rm.ClassMethodName = this.ToString() + ".DoSinger";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "部门角色";
                rm.ClassMethodName = this.ToString() + ".DoEmpDepts";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                ////节点绑定部门. 节点绑定部门.
                //map.AttrsOfOneVSM.AddBranches(new DeptEmps(), new BP.Port.Depts(),
                //   BP.Port.DeptEmpAttr.FK_Emp,
                //   BP.Port.DeptEmpAttr.FK_Dept, "部门维护", EmpAttr.Name, EmpAttr.No, "@OrgNo");

                rm = new RefMethod();
                rm.Title = "修改密码";
                rm.ClassMethodName = this.ToString() + ".DoResetpassword";
                rm.HisAttrs.AddTBString("pass1", null, "输入密码", true, false, 0, 100, 100);
                rm.HisAttrs.AddTBString("pass2", null, "再次输入", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置部门直属领导";
                //rm.ClassMethodName = this.ToString() + ".DoEditLeader";
                //rm.RefAttrKey = "LeaderName";
                //rm.RefMethodType = RefMethodType.LinkModel;
                //map.AddRefMethod(rm);
                #endregion 相关方法.

                this._enMap = map;
                return this._enMap;
            }
        }

        #region 方法执行.
        public string DoEditLeader()
        {
            return "../../../GPM/EmpLeader.htm?FK_Emp=" + this.No + "&FK_Dept=" + this.GetValByKey("FK_Dept");
        }

        public string DoEmpDepts()
        {
            return "/GPM/EmpDepts.htm?FK_Emp=" + this.No;
        }

        public string DoSinger()
        {
            //路径
            return "../../../GPM/Siganture.htm?EmpNo=" + this.No;
        }
        #endregion 方法执行.


        protected override bool beforeUpdateInsertAction()
        {
            //增加拼音，以方便查找.
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("err@名称不能为空.");

            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(this.Name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(this.Name).ToLower();
            this.SetValByKey("PinYin", "," + pinyinQP + "," + pinyinJX + ",");
            return base.beforeUpdateInsertAction();
        }

     
        protected override bool beforeDelete()
        {
                throw new Exception("err@您不能删除别人的数据.");
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new EmpSettings(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 操作员s
    // </summary>
    public class EmpSettings : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpSetting();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public EmpSettings()
        {
        }
        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpSetting> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpSetting>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpSetting> Tolist()
        {
            System.Collections.Generic.List<EmpSetting> list = new System.Collections.Generic.List<EmpSetting>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpSetting)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

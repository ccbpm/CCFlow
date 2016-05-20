using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.WF;
using BP.Port;

namespace BP.Demo.SDK
{
    /// <summary>
    /// 请假 属性
    /// </summary>
    public class QingJiaAttr
    {
        #region 基本属性
        /// <summary>
        /// 请假人编号
        /// </summary>
        public const string QingJiaRenNo = "QingJiaRenNo";
        /// <summary>
        /// 请假人名称
        /// </summary>
        public const string QingJiaRenName = "QingJiaRenName";
        /// <summary>
        /// 部门编号
        /// </summary>
        public const string QingJiaRenDeptNo = "QingJiaRenDeptNo";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string QingJiaRenDeptName = "QingJiaRenDeptName";
        /// <summary>
        /// 请假天数
        /// </summary>
        public const string QingJiaTianShu = "QingJiaTianShu";
        /// <summary>
        /// 请假原因
        /// </summary>
        public const string QingJiaYuanYin = "QingJiaYuanYin";
        #endregion

        #region 审核属性.
        public const string NoteBM = "NoteBM";
        public const string NoteZJL = "NoteZJL";
        public const string NoteRL = "NoteRL";
        #endregion 审核属性.
    }
    /// <summary>
    /// 请假
    /// </summary>
    public class QingJia : EntityOID
    {
        #region 属性
        /// <summary>
        /// 请假人部门名称
        /// </summary>
        public string QingJiaRenDeptName
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaRenDeptName);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaRenDeptName, value);
            }
        }
        /// <summary>
        /// 请假人编号
        /// </summary>
        public string QingJiaRenNo
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaRenNo);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaRenNo, value);
            }
        }
        /// <summary>
        /// 请假人名称
        /// </summary>
        public string QingJiaRenName
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaRenName);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaRenName, value);
            }
        }
        /// <summary>
        /// 请假人部门编号
        /// </summary>
        public string QingJiaRenDeptNo
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaRenDeptNo);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaRenDeptNo, value);
            }
        }
        /// <summary>
        /// 请假原因
        /// </summary>
        public string QingJiaYuanYin
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.QingJiaYuanYin);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaYuanYin, value);
            }
        }
        /// <summary>
        /// 请假天数
        /// </summary>
        public float QingJiaTianShu
        {
            get
            {
                return this.GetValIntByKey(QingJiaAttr.QingJiaTianShu);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.QingJiaTianShu, value);
            }
        }
        /// <summary>
        /// 部门审批意见
        /// </summary>
        public string NoteBM
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.NoteBM);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.NoteBM, value);
            }
        }
        /// <summary>
        /// 总经理意见
        /// </summary>
        public string NoteZJL
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.NoteZJL);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.NoteZJL, value);
            }
        }
        /// <summary>
        /// 人力资源意见
        /// </summary>
        public string NoteRL
        {
            get
            {
                return this.GetValStringByKey(QingJiaAttr.NoteRL);
            }
            set
            {
                this.SetValByKey(QingJiaAttr.NoteRL, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 请假
        /// </summary>
        public QingJia()
        {
        }
        /// <summary>
        /// 请假
        /// </summary>
        /// <param name="oid">实体类</param>
        public QingJia(int oid):base(oid)
        {
        }
        /// <summary>
        /// 请假
        /// </summary>
        /// <param name="oid">实体类</param>
        public QingJia(Int64 oid)
        {
            this.OID = (int)oid;
            this.Retrieve();
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

                Map map = new Map("Demo_QingJia");
                map.EnDesc = "请假";

                map.AddTBIntPKOID();
                map.AddTBString(QingJiaAttr.QingJiaRenNo, null, "请假人编号", false, false, 0, 200, 10);
                map.AddTBString(QingJiaAttr.QingJiaRenName, null, "请假人名称", true, false, 0, 200, 70);
                map.AddTBString(QingJiaAttr.QingJiaRenDeptNo, "", "请假人部门编号", true, false, 0, 200, 50);
                map.AddTBString(QingJiaAttr.QingJiaRenDeptName, null, "请假人部门名称", true, false, 0, 200, 50);
                map.AddTBString(QingJiaAttr.QingJiaYuanYin, null, "请假原因", true, false, 0, 200, 150);
                map.AddTBFloat(QingJiaAttr.QingJiaTianShu, 0, "请假天数", true, false);

                // 审核信息.
                map.AddTBString(QingJiaAttr.NoteBM, null, "部门经理意见", true, false, 0, 200, 150);
                map.AddTBString(QingJiaAttr.NoteZJL, null, "总经理意见", true, false, 0, 200, 150);
                map.AddTBString(QingJiaAttr.NoteRL, null, "人力资源意见", true, false, 0, 200, 150);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 请假s
    /// </summary>
    public class QingJias : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new QingJia();
            }
        }
        /// <summary>
        /// 请假s
        /// </summary>
        public QingJias() { }
        #endregion
    }
}

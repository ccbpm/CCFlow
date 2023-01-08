using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BP.DA;
using BP.En;

namespace BP.Demo.HNZY
{
    /// <summary>
    /// 维护教师业务成果
    /// </summary>
    public class TeacherResultAttr : EntityNoNameAttr
    {
        public static string Idx = "Idx";

        #region 维护教师业务成果
        /// <summary>
        /// 教师业务成果
        /// </summary>
        public static string JSCG = "JSCG";
        /// <summary>
        /// 审核教师业务成果
        /// </summary>
        public static string SHJSCG = "SHJSCG";

        #endregion 维护教师业务成果
    }
    public class TeacherResult : EntityNoName
    {
        /// <summary>
        /// 显示序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(TeacherResultAttr.Idx);
            }
        }

        #region 属性
        /// <summary>
        /// 教师业务成果
        /// </summary>
        public string JSCG
        {
            get
            {
                return this.GetValStrByKey(TeacherResultAttr.JSCG);
            }
        }
        /// <summary>
        /// 审核教师业务成果
        /// </summary>
        public string SHJSCG
        {
            get
            {
                return this.GetValStrByKey(TeacherResultAttr.SHJSCG);
            }
        }

        #endregion


        #region 构造函数
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// 教师业务成果
        /// </summary>		
        public TeacherResult() { }
        public TeacherResult(string no)
            : base(no)
        {
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Demo_TeacherResult", "维护教师业务成果");

                #region 基本属性
                map.CodeStruct = "4";
                #endregion

                #region 字段
                map.AddTBStringPK(TeacherResultAttr.No, null, "教师编号", true, false, 0, 50, 50);
                map.AddTBString(TeacherResultAttr.Name, null, "教师名称", true, false, 0, 50, 200);
                map.AddTBInt(TeacherResultAttr.Idx, 0, "显示序号", false, false);
                map.AddTBStringDoc(TeacherResultAttr.JSCG,null, "教师业务成果", true, false);
                

                #endregion

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    public class TeacherResults : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TeacherResult();
            }
        }
        /// <summary>
        /// 双师认定s
        /// </summary>
        public TeacherResults()
        {
        }
        /// <summary>
        /// 双师认定s
        /// </summary>
        public TeacherResults(string no)
        {

            this.Retrieve(TeacherResultAttr.No, no);

        }
        #endregion 构造方法


        #region 重写查询,add by zhoupeng 2015.09.30 为了适应能够从 webservice 数据源查询数据.
        /// <summary>
        /// 重写查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            //if (BP.Web.WebUser.No != "admin")
            //    throw new Exception("@您没有查询的权限.");


            return base.RetrieveAll();

        }
        /// <summary>
        /// 重写重数据源查询全部适应从WS取数据需要
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAllFromDBSource()
        {

            return base.RetrieveAllFromDBSource();

        }
        #endregion 重写查询.

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TeacherResult> ToJavaList()
        {
            return (System.Collections.Generic.IList<TeacherResult>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TeacherResult> Tolist()
        {
            System.Collections.Generic.List<TeacherResult> list = new System.Collections.Generic.List<TeacherResult>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TeacherResult)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

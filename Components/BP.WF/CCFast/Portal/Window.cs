using BP.En;

namespace BP.CCFast.Portal
{
    /// <summary>
    /// 信息块
    /// </summary>
    public class WindowAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 信息块类型
        /// </summary>
        public const string WindowTemplateNo = "WindowTemplateNo";
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 是否可用？
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// EmpNo
        /// </summary>
        public const string EmpNo = "EmpNo";
    }
    /// <summary>
    /// 信息块
    /// </summary>
    public class Window : EntityMyPK
    {

        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                return uac;
            }
        }

        #region 属性
        public bool ItIsEnable
        {
            get
            {
                return this.GetValBooleanByKey(WindowAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(WindowAttr.IsEnable, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(WindowAttr.Idx);
            }
            set
            {
                this.SetValByKey(WindowAttr.Idx, value);
            }
        }
        public string EmpNo
        {
            get
            {
                return this.GetValStrByKey(WindowAttr.EmpNo);
            }
            set
            {
                this.SetValByKey(WindowAttr.EmpNo, value);
            }
        }
        public string WindowTemplateNo
        {
            get
            {
                return this.GetValStrByKey(WindowAttr.WindowTemplateNo);
            }
            set
            {
                this.SetValByKey(WindowAttr.WindowTemplateNo, value);
            }
        }
        public string Docs
        {
            get
            {
                return this.GetValStrByKey(WindowAttr.Docs);
            }
            set
            {
                this.SetValByKey(WindowAttr.Docs, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 信息块
        /// </summary>
        public Window()
        {
        }
        /// <summary>
        /// 信息块
        /// </summary>
        /// <param name="mypk"></param>
        public Window(string mypk)
        {
            this.setMyPK(mypk);
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
                Map map = new Map("GPM_Window", "信息块");
                map.setEnType(EnType.Sys);

                //主键.
                map.AddMyPK(false);
                map.AddTBString(WindowAttr.EmpNo, null, "用户编号", true, false, 0, 50, 20);
                map.AddTBString(WindowAttr.WindowTemplateNo, null, "模板编号", true, false, 0, 50, 20);
                map.AddTBString(WindowAttr.Docs, null, "内容", true, false, 0, 4000, 20);

                map.AddTBInt(WindowAttr.IsEnable, 0, "是否可见?", false, true);

                map.AddTBInt(WindowAttr.Idx, 0, "排序", false, true);
                map.AddTBString(WindowTemplateAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 移动方法.
        /// <summary>
        /// 向上移动
        /// </summary>
        public string DoUp()
        {
            this.DoOrderUp(WindowAttr.EmpNo, this.EmpNo, WindowAttr.Idx);
            return "执行成功.";

        }
        /// <summary>
        /// 向下移动
        /// </summary>
        public string DoDown()
        {
            this.DoOrderDown(WindowAttr.EmpNo, this.EmpNo, WindowAttr.Idx);
            return "执行成功.";

        }
        /// <summary>
        /// 移动到指定的模板前面.
        /// </summary>
        /// <param name="no"></param>
        public string DoOrderMoveTo(string no)
        {
              //this.DoOrderMoveTo(WindowAttr.EmpNo, this.EmpNo, WindowAttr.Idx, no);
            return "执行成功.";
        }
        #endregion 移动方法.

    }
    /// <summary>
    /// 信息块s
    /// </summary>
    public class Windows : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 信息块s
        /// </summary>
        public Windows()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Window();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Window> ToJavaList()
        {
            return (System.Collections.Generic.IList<Window>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Window> Tolist()
        {
            System.Collections.Generic.List<Window> list = new System.Collections.Generic.List<Window>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Window)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

        public override int RetrieveAll()
        {
            //查询全部模板.
            WindowTemplates templates = new WindowTemplates();
            templates.RetrieveAll();

            Windows windows = new Windows();
            windows.Retrieve(WindowAttr.EmpNo, BP.Web.WebUser.No, "Idx");

            //检查是否有新增的？
            foreach (WindowTemplate en in templates)
            {
                if (en.ItIsEnable == false)
                    continue; //如果是不启用的.

                //从实例里面获取window..
                Window window = windows.Filter(WindowAttr.WindowTemplateNo, en.No) as Window;
                if (window == null)
                {
                    /*不存在，就Insrt.*/
                    window = new Window();
                    window.setMyPK(en.No + "_" + BP.Web.WebUser.No);
                    window.EmpNo = BP.Web.WebUser.No;
                    window.WindowTemplateNo = en.No;
                    window.ItIsEnable = true;
                    window.Insert();
                    continue;
                }

                //如果 个人设置 他是启用的.
                if (window.ItIsEnable == true)
                    en.Idx = window.Idx; //就给他顺序号.
                else
                    en.ItIsEnable = false;
            }


            //把模板放入到这里.
            this.AddEntities(templates);
            return templates.Count; 
        }
    }
}

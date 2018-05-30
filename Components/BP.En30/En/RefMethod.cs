using System; 
using System.Collections;
using BP.DA; 
using BP.Web.Controls;
using System.Reflection;

namespace BP.En
{
    /// <summary>
    /// 相关功能类型
    /// </summary>
    public enum RefMethodType
    {
        /// <summary>
        /// 功能
        /// </summary>
        Func,
        /// <summary>
        /// 模态窗口打开
        /// </summary>
        LinkModel,
        /// <summary>
        /// 新窗口打开
        /// </summary>
        LinkeWinOpen,
        /// <summary>
        /// 右侧窗口打开
        /// </summary>
        RightFrameOpen,
        /// <summary>
        /// 实体的功能
        /// </summary>
        FuncBacthEntities
    }
    /// <summary>
    /// RefMethod 的摘要说明。
    /// </summary>
    public class RefMethod
    {
        #region 与窗口有关的方法.
        /// <summary>
        /// 高度
        /// </summary>
        public int Height = 600;
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width = 800;
        public string Target = "_B123";
        #endregion

        /// <summary>
        /// 功能
        /// </summary>
        public RefMethodType RefMethodType = RefMethodType.Func;
        /// <summary>
        /// 相关字段
        /// </summary>
        public string RefAttrKey = null;
        /// <summary>
        /// 连接标签
        /// </summary>
        public string RefAttrLinkLabel = null;
        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName = null;
        /// <summary>
        /// 是否显示在Ens中?
        /// </summary>
        public bool IsForEns = false;
        /// <summary>
        /// 相关功能
        /// </summary>
        public RefMethod()
        {
        }
        /// <summary>
        /// 参数
        /// </summary>
        private Attrs _HisAttrs = null;
        /// <summary>
        /// 参数
        /// </summary>
        public Attrs HisAttrs
        {
            get
            {
                if (_HisAttrs == null)
                    _HisAttrs = new Attrs();
                return _HisAttrs;
            }
            set
            {
                _HisAttrs = value;
            }
        }
        /// <summary>
        /// 索引位置，用它区分实体.
        /// </summary>
        public int Index = 0;
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool Visable = true;
        /// <summary>
        /// 是否可以批处理
        /// </summary>
        public bool IsCanBatch = false;
        /// <summary>
        /// 标题
        /// </summary>
        public string Title = null;
        /// <summary>
        /// 操作前提示信息
        /// </summary>
        public string Warning = null;
        /// <summary>
        /// 连接
        /// </summary>
        public string ClassMethodName = null;
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon = null;
        public string GetIcon(string path)
        {
            if (this.Icon == null)
            {
                return null;
                return "<img src='/WF/Img/Btn/Do.gif'  border=0 />";
            }
            else
            {
                string url = path + Icon;
                url = url.Replace("//", "/");
                return "<img src='" + url + "'  border=0 />";
            }
        }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string ToolTip = null;
       
        /// <summary>
        /// PKVal
        /// </summary>
        public object PKVal = "PKVal";
        /// <summary>
        /// 
        /// </summary>
        public Entity HisEn = null;
        /// <summary>
        /// 实体PK
        /// </summary>
        public string[] PKs = "".Split('.');
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="paras"></param>
        /// <returns></returns>
        public object Do(object[] paras)
        {
            string str = this.ClassMethodName.Trim(' ', ';', '.');
            int pos = str.LastIndexOf(".");
            string clas = str.Substring(0, pos);
            string meth = str.Substring(pos, str.Length - pos).Trim('.', ' ', '(', ')');
            if (this.HisEn == null)
            {
                this.HisEn = BP.En.ClassFactory.GetEn(clas);
                Attrs attrs = this.HisEn.EnMap.Attrs;
            }

            Type tp = this.HisEn.GetType();
            MethodInfo mp = tp.GetMethod(meth);
            if (mp == null)
                throw new Exception("@对象实例[" + tp.FullName + "]中没有找到方法[" + meth + "]！");

            try
            {
                return mp.Invoke(this.HisEn, paras); //调用由此 MethodInfo 实例反射的方法或构造函数。
            }
            catch (System.Reflection.TargetException ex)
            {
                string strs = "";
                if (paras == null)
                {
                    throw new Exception(ex.Message);
                }
                else
                {
                    foreach (object obj in paras)
                    {
                        strs += "para= " + obj.ToString() + " type=" + obj.GetType().ToString() + "\n<br>";
                    }
                }
                throw new Exception(ex.Message + "  more info:" + strs);
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RefMethods : CollectionBase
    {
        /// <summary>
        /// 加入
        /// </summary>
        /// <param name="attr">attr</param>
        public void Add(RefMethod en)
        {
            if (this.IsExits(en))
                return;
            en.Index = this.InnerList.Count;
            this.InnerList.Add(en);
        }
        /// <summary>
        /// 是不是存在集合里面
        /// </summary>
        /// <param name="en">要检查的RefMethod</param>
        /// <returns>true/false</returns>
        public bool IsExits(RefMethod en)
        {
            foreach (RefMethod dtl in this)
            {
                if (dtl.ClassMethodName == en.ClassMethodName)
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 能够看到的属性
        /// </summary>
        public int CountOfVisable
        {
            get
            {
                int i = 0;
                foreach (RefMethod rm in this)
                {
                    if (rm.Visable)
                        i++;
                }
                return i;
            }
        }
        /// <summary>
        /// 根据索引访问集合内的元素Attr。
        /// </summary>
        public RefMethod this[int index]
        {
            get
            {
                return (RefMethod)this.InnerList[index];
            }
        }
    }
}

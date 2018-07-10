using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.YS
{
    /// <summary>
    /// 科目属性
    /// </summary>
    public class KMAttr : EntityTreeAttr
    {
    }
    /// <summary>
    ///  科目
    /// </summary>
    public class KM : EntityTree
    {
        #region 构造方法
        /// <summary>
        /// 科目
        /// </summary>
        public KM()
        {
        }
        /// <summary>
        /// 科目
        /// </summary>
        /// <param name="_No"></param>
        public KM(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// 科目Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_YS_KM", "科目");
                map.Java_SetCodeStruct("2");
                map.IsAllowRepeatName = false;

                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.AddTBStringPK(KMAttr.No, null, "编号", true, true, 1, 10, 20);
                map.AddTBString(KMAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(KMAttr.ParentNo, null, "父节点No", false, false, 0, 100, 30);

                map.AddTBInt(KMAttr.Idx, 0, "Idx", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        
    }
    /// <summary>
    /// 科目
    /// </summary>
    public class KMs : EntitiesTree
    {
        /// <summary>
        /// 科目s
        /// </summary>
        public KMs() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new KM();
            }

        }
        /// <summary>
        /// 科目s
        /// </summary>
        /// <param name="no">ss</param>
        /// <param name="name">anme</param>
        public void AddByNoName(string no, string name)
        {
            KM en = new KM();
            en.No = no;
            en.Name = name;
            this.AddEntity(en);
        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll();
            if (i == 0)
            {
                KM fs = new KM();
                fs.Name = "公文类";
                fs.No = "01";
                fs.Insert();

                fs = new KM();
                fs.Name = "办公类";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }

            return i;
        }
    }
}

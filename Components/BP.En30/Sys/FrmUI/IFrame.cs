using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 框架
    /// </summary>
    public class IFrame : EntityMyPK
    {
        #region 属性
        #endregion

        #region 构造方法
        /// <summary>
        /// 框架
        /// </summary>
        public IFrame()
        {
        }
        /// <summary>
        /// 框架
        /// </summary>
        /// <param name="mypk"></param>
        public IFrame(string mypk)
        {
            this.MyPK = mypk;
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

                Map map = new Map("Sys_FrmEle", "框架");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(FrmBtnAttr.FK_MapData, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmEleAttr.EleType, null, "类型", true, false, 0, 30, 20);

                map.AddTBString(FrmEleAttr.EleID, null, "ID", true, false, 0, 30, 20);
                map.AddTBString(FrmEleAttr.EleName, null, "名称", true, false, 0, 30, 20);
                map.AddTBString(FrmEleAttr.Tag1, null, "连接", true, false, 0, 30, 20,true);
             
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 框架s
    /// </summary>
    public class IFrames : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 框架s
        /// </summary>
        public IFrames()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new IFrame();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<IFrame> ToJavaList()
        {
            return (System.Collections.Generic.IList<IFrame>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<IFrame> Tolist()
        {
            System.Collections.Generic.List<IFrame> list = new System.Collections.Generic.List<IFrame>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((IFrame)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

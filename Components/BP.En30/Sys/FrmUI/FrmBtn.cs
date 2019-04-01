using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 按钮
    /// </summary>
    public class FrmBtn : EntityMyPK
    {
        #region 属性
        #endregion

        #region 构造方法
        /// <summary>
        /// 按钮
        /// </summary>
        public FrmBtn()
        {
        }
        /// <summary>
        /// 按钮
        /// </summary>
        /// <param name="mypk"></param>
        public FrmBtn(string mypk)
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

                Map map = new Map("Sys_FrmBtn", "按钮");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(FrmBtnAttr.FK_MapData, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmBtnAttr.Text, null, "标签", true, false, 0, 3900, 20);

                map.AddTBInt(FrmBtnAttr.IsView, 0, "是否可见", false, false);
                map.AddTBInt(FrmBtnAttr.IsEnable, 0, "是否起用", false, false);

                map.AddTBInt(FrmBtnAttr.UAC, 0, "控制类型", false, false);
                map.AddTBString(FrmBtnAttr.UACContext, null, "控制内容", false, false, 0, 3900, 20);

                map.AddDDLSysEnum(FrmBtnAttr.EventType, 0, "事件类型", true, true, FrmBtnAttr.EventType, "@0=禁用@1=执行URL@2=执行CCFromRef.js");
                map.AddTBString(FrmBtnAttr.EventContext, null, "事件内容", true, false, 0, 3900, 20);

                map.AddTBString(FrmBtnAttr.MsgOK, null, "运行成功提示", true, false, 0, 500, 20);
                map.AddTBString(FrmBtnAttr.MsgErr, null, "运行失败提示", true, false, 0, 500, 20);

                map.AddTBString(FrmBtnAttr.BtnID, null, "按钮ID", true, false, 0, 128, 20);

                //显示的分组.
                map.AddDDLSQL(FrmBtnAttr.GroupID, "0", "所在分组",
                    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE FrmID='@FK_MapData'", true);
             
                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            BP.Sys.FrmBtn frmBtn = new BP.Sys.FrmBtn();
            frmBtn.MyPK = this.MyPK;
            frmBtn.RetrieveFromDBSources();
            frmBtn.Update();

            base.afterInsertUpdateAction();
        }


        #endregion
    }
    /// <summary>
    /// 按钮s
    /// </summary>
    public class FrmBtns : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 按钮s
        /// </summary>
        public FrmBtns()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmBtn();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmBtn> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmBtn>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmBtn> Tolist()
        {
            System.Collections.Generic.List<FrmBtn> list = new System.Collections.Generic.List<FrmBtn>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmBtn)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

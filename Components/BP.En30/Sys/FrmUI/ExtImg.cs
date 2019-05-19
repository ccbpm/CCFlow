using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 装饰图片
    /// </summary>
    public class ExtImg : EntityMyPK
    {
        #region 构造方法
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// 装饰图片
        /// </summary>
        public ExtImg()
        {
        }
        /// <summary>
        /// 装饰图片
        /// </summary>
        /// <param name="mypk"></param>
        public ExtImg(string mypk)
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

                Map map = new Map("Sys_FrmImg", "装饰图片");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(MapAttrAttr.FK_MapData, null, "表单ID", true, true, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "对应字段", true, true, 0, 200, 20);
                map.AddTBString(FrmImgAttr.Name, null, "中文名称", true, true, 0, 500, 20);
                map.AddDDLSysEnum(FrmImgAttr.ImgSrcType, 0, "图片来源", true, true, FrmImgAttr.ImgSrcType, "@0=本地@1=URL");

                map.AddTBString(FrmImgAttr.ImgPath, null, "图片路径", true, false, 0, 200, 20, true);

                string strs = "本机的图片路径:";
                strs += " \n 1.您可以使用 ＠baseBase 或者＠+字段英文名作为变量来标识文件路径.";
                strs += " \n 2.注意＠需要半角字符.";
                strs += " \n 3.例如:＠basePath/DataUser/UserIcon/＠QingJiaRenID.png";

                map.SetHelperAlert(FrmImgAttr.ImgPath, strs);

                map.AddTBString(FrmImgAttr.ImgURL, null, "图片URL", true, false, 0, 200, 20, true);

                map.AddTBString(FrmImgAttr.LinkURL, null, "连接到URL", true, false, 0, 200, 20, true);
                map.AddTBString(FrmImgAttr.LinkTarget, "_blank", "连接目标", true, false, 0, 200, 20);

                //UIContralType.FrmImg
                //map.AddTBString(FrmImgAttr.Tag0, null, "参数", true, false, 0, 500, 20); 2
                //map.AddTBString(FrmImgAttr.EnPK, null, "英文名称", true, false, 0, 500, 20);
                //map.AddTBInt(FrmImgAttr.ImgAppType, 0, "应用类型", false, false);
                //map.AddTBString(FrmImgAttr.GUID, null, "GUID", true, false, 0, 128, 20);

                map.AddTBInt(FrmImgAttr.ImgAppType, 0, "应用类型", false, false);

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.ColSpan, 0, "单元格数量", false, true);

                //跨单元格
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");
                //跨行
                map.AddDDLSysEnum(MapAttrAttr.RowSpan, 1, "行数", true, true, "RowSpanAttrString",
                   "@1=跨1个行@2=跨2行@3=跨3行");

                map.AddTBInt(MapAttrAttr.UIWidth, 0, "宽度", true, false);
                map.AddTBInt(MapAttrAttr.UIHeight, 0, "高度", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override void afterInsertUpdateAction()
        {
            BP.Sys.FrmImg imgAth = new BP.Sys.FrmImg();
            imgAth.MyPK = this.MyPK;
            imgAth.RetrieveFromDBSources();
            imgAth.Update();

            //同步更新MapAttr 
            MapAttrString attr = new MapAttrString(this.MyPK);
            attr.SetValByKey(MapAttrAttr.ColSpan, this.GetValStrByKey(MapAttrAttr.ColSpan));
            attr.SetValByKey(MapAttrAttr.TextColSpan, this.GetValStrByKey(MapAttrAttr.TextColSpan));
            attr.SetValByKey(MapAttrAttr.RowSpan, this.GetValStrByKey(MapAttrAttr.RowSpan));

            attr.SetValByKey(MapAttrAttr.Name, this.GetValStrByKey(FrmImgAttr.Name)); //名称.

            attr.SetValByKey(MapAttrAttr.X, this.GetValStrByKey(FrmImgAttr.X)); //名称.
            attr.SetValByKey(MapAttrAttr.Y, this.GetValStrByKey(FrmImgAttr.Y)); //名称.
            attr.Update();

            base.afterInsertUpdateAction();
        }
        protected override void afterDelete()
        {
            //把相关的字段也要删除.
            MapAttrString attr = new MapAttrString();
            attr.MyPK = this.MyPK;
            attr.Delete();

            base.afterDelete();
        }

        #endregion
    }
    /// <summary>
    /// 装饰图片s
    /// </summary>
    public class ExtImgs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 装饰图片s
        /// </summary>
        public ExtImgs()
        {
        }
        /// <summary>
        /// 装饰图片s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public ExtImgs(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            else
                this.RetrieveFromCash(FrmLineAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmImg();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<ExtImg> ToJavaList()
        {
            return (System.Collections.Generic.IList<ExtImg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ExtImg> Tolist()
        {
            System.Collections.Generic.List<ExtImg> list = new System.Collections.Generic.List<ExtImg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ExtImg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 解析控件并保存.
    /// </summary>
    public class CCFormParse
    {
        /// <summary>
        /// 保存元素
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="eleType">元素类型</param>
        /// <param name="ctrlID">控件ID</param>
        /// <param name="x">位置</param>
        /// <param name="y">位置</param>
        /// <param name="h">高度</param>
        /// <param name="w">宽度</param>
        public static void SaveMapFrame(string fk_mapdata, string eleType, string ctrlID, float x, float y, float h, float w)
        {
            MapFrame en = new MapFrame();
            en.setMyPK(ctrlID);
            int i = en.RetrieveFromDBSources();
            en.setEleType(eleType);
            en.FrmID =ctrlID;

            en.W = w;
            en.H = h;

            if (i == 0)
                en.Insert();
            else
                en.Update();
        }

        /// <summary>
        /// 保存一个rb
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ctrlID">控件ID</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        public static string SaveFrmRadioButton(string fk_mapdata, string ctrlID, float x, float y)
        {
            FrmRB en = new FrmRB();
            en.setMyPK(fk_mapdata + "_" + ctrlID);
            int i = en.RetrieveFromDBSources();
            if (i == 0)
                return null;

            en.FrmID= fk_mapdata;

            en.Update();
            return en.KeyOfEn;
        }
        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ctrlID">空间ID</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        /// <param name="h">高度h</param>
        /// <param name="w">宽度w</param>
        public static void SaveAthImg(string fk_mapdata, string ctrlID, float x, float y, float h, float w)
        {
            FrmImgAth en = new FrmImgAth();
            en.setMyPK(fk_mapdata + "_" + ctrlID);
            en.FrmID= fk_mapdata;
            en.CtrlID = ctrlID;
            en.RetrieveFromDBSources();

            //en.X = x;
            //en.Y = y;
            en.W = w;
            en.H = h;
            en.Update();
        }
        /// <summary>
        /// 保存多附件
        /// </summary>
        /// <param name="fk_mapdata">表单ID</param>
        /// <param name="ctrlID">控件ID</param>
        /// <param name="x">位置x</param>
        /// <param name="y">位置y</param>
        /// <param name="h">高度</param>
        /// <param name="w">宽度</param>
        public static void SaveAthMulti(string fk_mapdata, string ctrlID, float x, float y, float h, float w)
        {
            FrmAttachment en = new FrmAttachment();
            en.setMyPK(fk_mapdata + "_" + ctrlID);
            en.FrmID =fk_mapdata;
            en.NoOfObj = ctrlID;
            en.RetrieveFromDBSources();

            en.H = h;
            en.Update();
        }
        public static void SaveDtl(string fk_mapdata, string ctrlID, float x, float y, float h, float w)
        {
            MapDtl dtl = new MapDtl();
            dtl.No = ctrlID;
            dtl.RetrieveFromDBSources();

            dtl.FrmID =fk_mapdata;
            dtl.SetValByKey(MapAttrAttr.UIWidth, w);
            dtl.SetValByKey(MapAttrAttr.UIHeight, h);
            dtl.Update();
        }

    }
}

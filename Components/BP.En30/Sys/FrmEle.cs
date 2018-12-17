using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
    /// <summary>
    /// 表单元素扩展
    /// </summary>
    public class FrmEleAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 是否启用?
        /// </summary>
        public const string IsEnable = "IsEnable";
        /// <summary>
        /// EleType
        /// </summary>
        public const string EleType = "EleType";
        /// <summary>
        /// EleID
        /// </summary>
        public const string EleID = "EleID";
        /// <summary>
        /// EleName
        /// </summary>
        public const string EleName = "EleName";
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 参数
        /// </summary>
        public const string AtPara = "AtPara";
        /// <summary>
        /// 连接
        /// </summary>
        public const string URL = "URL";

        /// <summary>
        /// X
        /// </summary>
        public const string X = "X";
        /// <summary>
        /// Y
        /// </summary>
        public const string Y = "Y";
        /// <summary>
        /// H
        /// </summary>
        public const string H = "H";
        /// <summary>
        /// W
        /// </summary>
        public const string W = "W";
        /// <summary>
        /// Tag1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// Tag2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// Tag3
        /// </summary>
        public const string Tag3 = "Tag3";
        /// <summary>
        /// Tag4
        /// </summary>
        public const string Tag4 = "Tag4";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
        /// <summary>
        /// 类型
        /// </summary>
        public const string SrcType = "SrcType";
    }
    /// <summary>
    /// 表单元素扩展
    /// </summary>
    public class FrmEle : EntityMyPK
    {
        /// <summary>
        /// 参数字段.
        /// </summary>
        public string AtPara
        {
            get
            {
                return this.GetValStrByKey("AtPara");
            }
            set
            {
                this.SetValByKey("AtPara", value);
            }
        }


        #region 与 label 相关的属性 都存储到 AtParas 里面.
        /// <summary>
        /// 文本
        /// </summary>
        public string LabText
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.EleName);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleName, value);
            }
        }
        /// <summary>
        /// 风格
        /// </summary>
        public string LabStyle
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.AtPara);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.AtPara, value);
            }
        }
        #endregion 与 label 相关的属性.

        #region 与 link 相关的属性.
        public string LinkText
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.EleName);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleName, value);
            }
        }
        /// <summary>
        /// 连接URL
        /// </summary>
        public string LinkURL
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Tag1);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag1, value);
            }
        }
        /// <summary>
        /// 连接目标
        /// </summary>
        public string LinkTarget
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Tag2);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag2, value);
            }
        }
        /// <summary>
        /// 风格
        /// </summary>
        public string LinkStyle
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.AtPara);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.AtPara, value);
            }
        }
        #endregion 与 link 相关的属性.

        #region 与 line 相关的属性.
        /// <summary>
        /// 线的颜色
        /// </summary>
        public string LineBorderColor
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Tag2, "Black");
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag2, value);
            }
        }
        /// <summary>
        /// 宽度
        /// </summary>  
        public string LineBorderWidth
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Tag1,"1");
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag1, value);
            }
        }
        public string LineX1
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.X);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.X, value);
            }
        }
        public string LineY1
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Y, value);
            }
        }


        public string LineX2
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.W);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.W, value);
            }
        }
        public string LineY2
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.H);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.H, value);
            }
        }

        #endregion 与线相关的属性.

        #region 与 Btn 相关的属性.
        /// <summary>
        /// 按钮ID
        /// </summary>
        public string BtnID
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.EleID);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleID, value);
            }
        }
        /// <summary>
        /// 按钮文本
        /// </summary>
        public string BtnText
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.EleName);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleName, value);
            }
        }
        /// <summary>
        /// 按钮类型
        /// </summary>
        public int BtnType
        {
            get
            {
                return this.GetValIntByKey(FrmEleAttr.Tag1);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag1, value);
            }
        }
        /// <summary>
        /// 按钮事件类型
        /// </summary>
        public BtnEventType BtnEventType
        {
            get
            {
                return (BtnEventType)this.BtnType; 
            }
        }
        /// <summary>
        /// 按钮事件内容
        /// </summary>
        public string BtnEventContext
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag2);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag2, value);
            }
        }
        ///// <summary>
        ///// 执行成功
        ///// </summary>
        //public string BtnMsgOK
        //{
        //    get
        //    {
        //        return this.GetParaString(FrmBtnAttr.MsgOK);
        //    }
        //    set
        //    {
        //        this.SetPara(FrmBtnAttr.MsgOK, value);
        //    }
        //}
        ///// <summary>
        ///// 执行失败
        ///// </summary>
        //public string BtnMsgErr
        //{
        //    get
        //    {
        //        return this.GetParaString(FrmBtnAttr.MsgErr);
        //    }
        //    set
        //    {
        //        this.SetPara(FrmBtnAttr.MsgErr, value);
        //    }
        //}
        #endregion 与线相关的属性.

        #region 与 Img 相关的属性.
        /// <summary>
        /// 路径
        /// </summary>
        public string ImgPath
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag1);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag1, value);
            }
        }
        /// <summary>
        /// 图片链接地址
        /// </summary>
        public string ImgLinkUrl
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag2);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag2, value);
            }
        }
        /// <summary>
        /// 链接目标
        /// </summary>
        public string ImgLinkTarget
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag3);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag3, value);
            }
        }
        #endregion 与 Img 相关的属性.

        #region 与 seal 相关的属性.
        /// <summary>
        /// 签章ID
        /// </summary>
        public string SealID
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.EleID);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleID, value);
            }
        }
        /// <summary>
        /// 签章名称
        /// </summary>
        public string SealName
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.EleName);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleName, value);
            }
        }
        /// <summary>
        /// 签章的岗位.
        /// </summary>
        public string SealStations
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag1);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag1, value);
            }
        }
        /// <summary>
        /// 来源
        /// </summary>
        public int SealSrcType
        {
            get
            {
                return this.GetParaInt(FrmEleAttr.SrcType);
            }
            set
            {
                this.SetPara(FrmEleAttr.SrcType, value);
            }
        }
        /// <summary>
        /// 是否可以编辑?
        /// </summary>
        public bool SealIsEdit
        {
            get
            {
                return this.GetParaBoolen(FrmImgAttr.IsEdit);
            }
            set
            {
                this.SetPara(FrmImgAttr.IsEdit, value);
            }
        }
        #endregion 与 seal 相关的属性.

        #region  签名存储位置.
        public string HandSigantureSavePath
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Tag1);
            }
        }
        public string HandSiganture_WinOpenH
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Tag2);
            }
        }
        public string HandSiganture_WinOpenW
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Tag3);
            }
        }
        public string HandSiganture_UrlPath
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.Tag4);
            }
        }
        #endregion  签名存储位置

        #region 类型
        /// <summary>
        /// 标签
        /// </summary>
        public const string Label = "Label";
        /// <summary>
        /// 线
        /// </summary>
        public const string Line = "Line";
        /// <summary>
        /// 按钮
        /// </summary>
        public const string Button = "Button";
        /// <summary>
        /// 超连接
        /// </summary>
        public const string Link = "Link";
        /// <summary>
        /// 手工签名
        /// </summary>
        public const string HandSiganture = "HandSiganture";
        /// <summary>
        /// 电子签名
        /// </summary>
        public const string EleSiganture = "EleSiganture";
        /// <summary>
        /// 网页框架
        /// </summary>
        public const string iFrame = "iFrame";
        /// <summary>
        /// 分组
        /// </summary>
        public const string Fieldset = "Fieldset";
        /// <summary>
        /// 子线程
        /// </summary>
        public const string SubThread = "SubThread";
        /// <summary>
        /// 地图定位
        /// </summary>
        public const string MapPin = "MapPin";
        /// <summary>
        /// 录音
        /// </summary>
        public const string Microphonehot = "Microphonehot";
        #endregion 类型

        #region 属性
        /// <summary>
        /// 是否起用
        /// </summary>
        public bool IsEnable
        {
            get
            {
                return this.GetValBooleanByKey(FrmEleAttr.IsEnable);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.IsEnable, value);
            }
        }
        /// <summary>
        /// EleID
        /// </summary>
        public string EleID
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.EleID);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleID, value);
            }
        }
        /// <summary>
        /// EleName
        /// </summary>
        public string EleName
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.EleName);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleName, value);
            }
        }
        /// <summary>
        /// Tag1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag1);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag1, value);
            }
        }
        /// <summary>
        /// Tag2
        /// </summary>
        public string Tag2
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag2);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag2, value);
            }
        }
        /// <summary>
        /// Tag3
        /// </summary>
        public string Tag3
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag3);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag3, value);
            }
        }
        public string Tag4
        {
            get
            {
                return this.GetValStringByKey(FrmEleAttr.Tag4);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Tag4, value);
            }
        }
      
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// EleType
        /// </summary>
        public string EleType
        {
            get
            {
                return this.GetValStrByKey(FrmEleAttr.EleType);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.EleType, value);
            }
        }
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmEleAttr.X);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.X, value);
            }
        }
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmEleAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.Y, value);
            }
        }
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmEleAttr.H);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.H, value);
            }
        }
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmEleAttr.W);
            }
            set
            {
                this.SetValByKey(FrmEleAttr.W, value);
            }
        }

        public int HOfInt
        {
            get
            {
                return int.Parse(this.H.ToString("0"));
            }
        }
        public int WOfInt
        {
            get
            {
                return int.Parse(this.W.ToString("0"));
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 表单元素扩展
        /// </summary>
        public FrmEle()
        {
        }
        /// <summary>
        /// 表单元素扩展
        /// </summary>
        /// <param name="mypk"></param>
        public FrmEle(string mypk)
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

                Map map = new Map("Sys_FrmEle", "表单元素扩展");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                //主键.
                map.AddMyPK();

                map.AddTBString(FrmEleAttr.FK_MapData, null, "表单ID", true, false, 1, 100, 20);
                map.AddTBString(FrmEleAttr.EleType, null, "类型", true, false, 0, 50, 20);
                map.AddTBString(FrmEleAttr.EleID, null, "控件ID值(对部分控件有效)", true, false, 0, 50, 20);
                map.AddTBString(FrmEleAttr.EleName, null, "控件名称(对部分控件有效)", true, false, 0, 200, 20);

                #region 位置其他.
                map.AddTBFloat(FrmEleAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmEleAttr.Y, 5, "Y", false, false);

                map.AddTBFloat(FrmEleAttr.H, 20, "H", true, false);
                map.AddTBFloat(FrmEleAttr.W, 20, "W", false, false);
                #endregion 位置其他.

                #region 参数.
                map.AddTBString(FrmEleAttr.Tag1, null, "Tag1", true, false, 0, 50, 20);
                map.AddTBString(FrmEleAttr.Tag2, null, "Tag2", true, false, 0, 50, 20);
                map.AddTBString(FrmEleAttr.Tag3, null, "Tag3", true, false, 0, 50, 20);
                map.AddTBString(FrmEleAttr.Tag4, null, "Tag4", true, false, 0, 50, 20);
                map.AddTBString(FrmEleAttr.GUID, null, "GUID", true, false, 0, 128, 20);
                #endregion 参数.

                #region 控制属性.
                map.AddTBInt(FrmEleAttr.IsEnable, 1, "是否启用", false, false);
                #endregion 控制属性.


                //参数.
                map.AddTBAtParas(4000);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            if (this.EleID != "")
                this.MyPK = this.FK_MapData + "_" + this.EleType + "_" + this.EleID;
            else
                this.MyPK = this.FK_MapData + "_" + this.EleType;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 表单元素扩展s
    /// </summary>
    public class FrmEles : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 表单元素扩展s
        /// </summary>
        public FrmEles()
        {
        }
        /// <summary>
        /// 表单元素扩展s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmEles(string fk_mapdata)
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
                return new FrmEle();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmEle> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmEle>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmEle> Tolist()
        {
            System.Collections.Generic.List<FrmEle> list = new System.Collections.Generic.List<FrmEle>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmEle)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

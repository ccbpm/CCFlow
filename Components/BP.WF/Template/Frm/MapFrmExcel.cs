using System;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.WF.Template.Frm
{
	/// <summary>
	/// Excel表单属性 attr
	/// </summary>
	public class MapFrmExcelAttr : MapDataAttr
	{
        /// <summary>
        /// 临时的版本号
        /// </summary>
		public const string TemplaterVer = "TemplaterVer";
        /// <summary>
        /// 文件存储字段
        /// </summary>
        public const string DBSave = "DBSave";
	}
	/// <summary>
	/// Excel表单属性
	/// </summary>
	public class MapFrmExcel : EntityNoName
	{
		#region 文件模版属性.
		/// <summary>
		/// 模版版本号
		/// </summary>
		public string TemplaterVer
		{
			get
			{
				return this.GetValStringByKey(MapFrmExcelAttr.TemplaterVer);
			}
			set
			{
				this.SetValByKey(MapFrmExcelAttr.TemplaterVer, value);
			}
		}
        /// <summary>
        /// Excel数据存储字段
        /// 为了处理多个excel文件映射到同一张表上.
        /// </summary>
        public string DBSave
		{
			get
			{
                string str= this.GetValStringByKey(MapFrmExcelAttr.DBSave);
                if (DataType.IsNullOrEmpty(str))
                    return "DBFile";
                return str;
			}
			set
			{
                this.SetValByKey(MapFrmExcelAttr.DBSave, value);
			}
		}
		#endregion 文件模版属性.

		#region 属性
		/// <summary>
		/// 是否是节点表单?
		/// </summary>
		public bool ItIsNodeFrm
		{
			get
			{
				if (this.No.Contains("ND") == false)
					return false;

				if (this.No.Contains("Rpt") == true)
					return false;

				if (this.No.Substring(0, 2) == "ND")
					return true;

				return false;
			}
		}
		/// <summary>
		/// 节点ID.
		/// </summary>
		public int NodeID
		{
			get
			{
				return int.Parse(this.No.Replace("ND", ""));
			}
		}

		#endregion

		#region 权限控制.
		public override UAC HisUAC
		{
			get
			{
				UAC uac = new UAC();
				if (BP.Web.WebUser.IsAdmin == true)
				{
					uac.OpenForAppAdmin();//2020.6.22 zsy 修改.
					if (this.No.StartsWith("ND") == true)
						uac.IsDelete = false;
					uac.IsInsert = false;
				}
				else
				{
					throw new Exception("err@非法用户,只有管理员才可以操作.");
				}
				return uac;
			}
		}
		#endregion 权限控制.

		#region 构造方法
		/// <summary>
		/// Excel表单属性
		/// </summary>
		public MapFrmExcel()
		{
		}
		/// <summary>
		/// Excel表单属性
		/// </summary>
		/// <param name="no">表单ID</param>
		public MapFrmExcel(string no)
			: base(no)
		{
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
				Map map = new Map("Sys_MapData", "Excel表单属性");

                #region 基本属性.
                map.AddGroupAttr("基本属性");
                map.AddTBStringPK(MapFrmExcelAttr.No, null, "表单编号", true, true, 1, 190, 20);
				map.AddTBString(MapFrmExcelAttr.PTable, null, "存储表", true, false, 0, 100, 20);
				map.AddTBString(MapFrmExcelAttr.Name, null, "表单名称", true, false, 0, 500, 20, true);
				map.AddDDLEntities(MapFrmExcelAttr.FK_FormTree, "01", "表单类别", new SysFormTrees(), true);
				//表单的运行类型.
				map.AddDDLSysEnum(MapFrmExcelAttr.FrmType, (int)BP.Sys.FrmType.FoolForm, "表单类型", true, false, MapFrmExcelAttr.FrmType);
				map.AddTBString(MapFrmExcelAttr.TemplaterVer, null, "模版编号", true, false, 0, 30, 20);
                map.AddTBString(MapFrmExcelAttr.DBSave, null, "Excel数据文件存储", true, false, 0, 50, 20);
                map.SetHelperAlert(MapFrmExcelAttr.DBSave,
                    "二进制的excel文件存储到表的那个字段里面？默认为DBFile, 如果此表对应多个excel文件就会导致二进制excel文件存储覆盖.");
                #endregion 模版属性。

                #region 设计者信息.
                map.AddGroupAttr("设计者信息");
                map.AddTBString(MapFrmExcelAttr.Designer, null, "设计者", true, false, 0, 500, 20);
				map.AddTBString(MapFrmExcelAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
				map.AddTBString(MapFrmExcelAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
				map.AddTBString(MapFrmExcelAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
				map.AddTBString(MapFrmExcelAttr.Ver, null, "版本号", true, true, 0, 30, 20);
			//	map.AddTBString(MapFrmFreeAttr.DesignerTool, null, "表单设计器", true, true, 0, 30, 20);
				map.AddTBStringDoc(MapFrmExcelAttr.Note, null, "备注", true, false, true);

				//增加参数字段.
				map.AddTBAtParas(4000);
				map.AddTBInt(MapFrmExcelAttr.Idx, 100, "顺序号", false, false);
				#endregion 设计者信息.

				map.AddMyFile("表单模版", null, BP.Difference.SystemConfig.PathOfDataUser + "FrmVSTOTemplate/");

				//查询条件.
				map.AddSearchAttr(MapFrmExcelAttr.DBSrc);

				#region 方法 - 基本功能.
				RefMethod rm = new RefMethod();

				rm = new RefMethod();
				rm.Title = "启动傻瓜表单设计器";
				rm.ClassMethodName = this.ToString() + ".DoDesignerFool";
				rm.Icon = "../../WF/Img/FileType/xlsx.gif";
				rm.Visable = true;
				rm.Target = "_blank";
				rm.RefMethodType = RefMethodType.LinkeWinOpen;
				map.AddRefMethod(rm);

				rm = new RefMethod();
				rm.Title = "装载填充"; // "设计表单";
				rm.ClassMethodName = this.ToString() + ".DoPageLoadFull";
				rm.Icon = "../../WF/Img/FullData.png";
				rm.Visable = true;
				rm.RefMethodType = RefMethodType.RightFrameOpen;
				rm.Target = "_blank";
				map.AddRefMethod(rm);

				rm = new RefMethod();
				rm.Title = "表单事件"; // "设计表单";
				rm.ClassMethodName = this.ToString() + ".DoEvent";
				rm.Icon = "../../WF/Img/Event.png";
				rm.Visable = true;
				rm.RefMethodType = RefMethodType.RightFrameOpen;
				rm.Target = "_blank";
				map.AddRefMethod(rm);

				//rm = new RefMethod();
				//rm.Title = "批量设置验证规则";
				//rm.Icon = "../../WF/Img/RegularExpression.png";
				//rm.ClassMethodName = this.ToString() + ".DoRegularExpressionBatch";
				//rm.RefMethodType = RefMethodType.RightFrameOpen;
				//map.AddRefMethod(rm);

				rm = new RefMethod();
				rm.Title = "批量修改字段"; // "设计表单";
				rm.ClassMethodName = this.ToString() + ".DoBatchEditAttr";
				rm.Icon = "../../WF/Admin/CCBPMDesigner/Img/field.png";
				rm.Visable = true;
				rm.RefMethodType = RefMethodType.RightFrameOpen;
				rm.Target = "_blank";
				//map.AddRefMethod(rm);

				rm = new RefMethod();
				rm.Title = "JS编程"; // "设计表单";
				rm.ClassMethodName = this.ToString() + ".DoInitScript";
				rm.Icon = "../../WF/Img/Script.png";
				rm.Visable = true;
				rm.RefMethodType = RefMethodType.RightFrameOpen;
				rm.Target = "_blank";
				map.AddRefMethod(rm);

				rm = new RefMethod();
				rm.Title = "表单body属性"; // "设计表单";
				rm.ClassMethodName = this.ToString() + ".DoBodyAttr";
				rm.Icon = "../../WF/Img/Script.png";
				rm.Visable = true;
				rm.RefMethodType = RefMethodType.RightFrameOpen;
				rm.Target = "_blank";
				map.AddRefMethod(rm);

				rm = new RefMethod();
				rm.Title = "导出XML表单模版"; // "设计表单";
				rm.ClassMethodName = this.ToString() + ".DoExp";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
				rm.Icon = "../../WF/Img/Export.png";
				rm.Visable = true;
				rm.RefAttrLinkLabel = "导出到xml";
				rm.Target = "_blank";
				map.AddRefMethod(rm);
				#endregion 方法 - 基本功能.

				#region 高级设置.
 

				rm = new RefMethod();
				rm.Title = "手机端表单";
				rm.GroupName = "高级设置";
				rm.Icon = "../../WF/Admin/CCFormDesigner/Img/telephone.png";
				rm.ClassMethodName = this.ToString() + ".MobileFrmDesigner";
				rm.RefMethodType = RefMethodType.RightFrameOpen;
				map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.Title = "Excel表单属性";
                rm.GroupName = "高级设置";
                rm.ClassMethodName = this.ToString() + ".DoMapExcel";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "一键设置表单元素只读";
                rm.Warning = "您确定要设置吗？所有的元素，包括字段、从表、附件以及其它组件都将会被设置为只读的.";
                rm.GroupName = "高级设置";
                //rm.Icon = "../../WF/Img/RegularExpression.png";
                rm.ClassMethodName = this.ToString() + ".DoOneKeySetReadonly";
                rm.RefMethodType = RefMethodType.Func;
                map.AddRefMethod(rm);

                #endregion 高级设置.

                #region 方法 - 开发接口.
                rm = new RefMethod();
				rm.Title = "调用查询API"; // "设计表单";
				rm.ClassMethodName = this.ToString() + ".DoSearch";
				rm.Icon = "../../WF/Img/Table.gif";
				rm.Visable = true;
				rm.RefMethodType = RefMethodType.LinkeWinOpen;
				rm.Target = "_blank";
				rm.GroupName = "开发接口";
				map.AddRefMethod(rm);

				rm = new RefMethod();
				rm.Title = "调用分析API"; // "设计表单";
				rm.ClassMethodName = this.ToString() + ".DoGroup";
				rm.Icon = "../../WF/Img/Table.gif";
				rm.Visable = true;
				rm.RefMethodType = RefMethodType.LinkeWinOpen;
				rm.Target = "_blank";
				rm.GroupName = "开发接口";
				map.AddRefMethod(rm);
				#endregion 方法 - 开发接口.

				this._enMap = map;
				return this._enMap;
			}
		}
        #endregion

        #region 节点表单方法.
        /// <summary>
        /// 一键设置为只读.
        /// </summary>
        /// <returns></returns>
        public string DoOneKeySetReadonly()
        {
            BP.Sys.CCFormAPI.OneKeySetFrmEleReadonly(this.No);
            return "设置成功.";
        }

        public string DoMapExcel()
        {
            return  "../../Comm/En.htm?EnName=BP.WF.Template.Frm.MapFrmExcel&No=" + this.No;
        }
		public string DoDesignerFool()
		{
            return "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&MyPK=" + this.No + "&IsEditMapData=True&IsFirst=1";
		}
		public string DoEditExcelTemplate()
		{
			return "../../Admin/CCFormDesigner/ExcelFrmDesigner/Designer.htm?FK_MapData=" + this.No;
		}
	 
		/// <summary>
		/// 节点表单组件
		/// </summary>
		/// <returns></returns>
		public string DoNodeFrmCompent()
		{
			if (this.No.Contains("ND") == true)
				return "../../Comm/EnOnly.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=" + this.No.Replace("ND", "") + "&t=" + DataType.CurrentDateTime;
			else
                return "../../Admin/FoolFormDesigner/Do.htm&DoType=FWCShowError";
		}
		 
		#endregion

		#region 通用方法.
		/// <summary>
		/// 替换名称
		/// </summary>
		/// <param name="fieldOldName">旧名称</param>
		/// <param name="newField">新字段</param>
		/// <param name="newFieldName">新字段名称(可以为空)</param>
		/// <returns>执行结果</returns>
		public string DoChangeFieldName(string fieldOld, string newField, string newFieldName)
		{
			MapAttr attrOld = new MapAttr();
			attrOld.setKeyOfEn(fieldOld);
			attrOld.FrmID =this.No;
			attrOld.setMyPK(attrOld.FrmID + "_" + attrOld.KeyOfEn);
			if (attrOld.RetrieveFromDBSources() == 0)
				return "@旧字段输入错误[" + attrOld.KeyOfEn + "].";

			//检查是否存在该字段？
			MapAttr attrNew = new MapAttr();
			attrNew.setKeyOfEn(newField);
			attrNew.FrmID =this.No;
			attrNew.setMyPK(attrNew.FrmID + "_" + attrNew.KeyOfEn);
			if (attrNew.RetrieveFromDBSources() == 1)
				return "@该字段[" + attrNew.KeyOfEn + "]已经存在.";

			//删除旧数据.
			attrOld.Delete();

			//copy这个数据,增加上它.
			attrNew.Copy(attrOld);
			attrNew.setKeyOfEn(newField);
			attrNew.FrmID =this.No;

			if (newFieldName != "")
				attrNew.Name = newFieldName;

			attrNew.Insert();

			//更新处理他的相关业务逻辑.
			MapExts exts = new MapExts(this.No);
			foreach (MapExt item in exts)
			{
				item.setMyPK(item.MyPK.Replace("_" + fieldOld, "_" + newField));

				if (item.AttrOfOper == fieldOld)
					item.AttrOfOper = newField;

				if (item.AttrsOfActive == fieldOld)
					item.AttrsOfActive = newField;

				item.Tag = item.Tag.Replace(fieldOld, newField);
				item.Tag1 = item.Tag1.Replace(fieldOld, newField);
				item.Tag2 = item.Tag2.Replace(fieldOld, newField);
				item.Tag3 = item.Tag3.Replace(fieldOld, newField);

				item.AtPara = item.AtPara.Replace(fieldOld, newField);
				item.Doc = item.Doc.Replace(fieldOld, newField);
				item.Save();
			}
			return "执行成功";
		}
		/// <summary>
		/// 批量设置正则表达式规则.
		/// </summary>
		/// <returns></returns>
		public string DoRegularExpressionBatch()
		{
			return "../../Admin/FoolFormDesigner/MapExt/RegularExpressionBatch.htm?FK_Flow=&FK_MapData=" +
				   this.No + "&t=" + DataType.CurrentDateTime;
		}
		/// <summary>
		/// 批量修改字段
		/// </summary>
		/// <returns></returns>
		public string DoBatchEditAttr()
		{
			return "../../Admin/FoolFormDesigner/BatchEdit.htm?FK_MapData=" +
				   this.No + "&t=" + DataType.CurrentDateTime;
		}
		/// <summary>
		/// 排序字段顺序
		/// </summary>
		/// <returns></returns>
		public string MobileFrmDesigner()
		{
			return "../../Admin/MobileFrmDesigner/Default.htm?FK_Flow=&FK_MapData=" +
				   this.No + "&t=" + DataType.CurrentDateTime;
		}
		/// <summary>
		/// 设计表单
		/// </summary>
		/// <returns></returns>
		public string DoDFrom()
		{
			string url = "../../Admin/FoolFormDesigner/CCForm/Frm.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&Token=" + Web.WebUser.Token + "&AppCenterDBType=" + DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Difference.SystemConfig.CustomerNo;
            return url;
		}
		/// <summary>
		/// 设计傻瓜表单
		/// </summary>
		/// <returns></returns>
		public string DoDFromCol4()
		{
            string url = "../../Admin/FoolFormDesigner/Designer.htm?FK_MapData=" + this.No + "&UserNo=" + BP.Web.WebUser.No + "&Token=" + Web.WebUser.Token + "&AppCenterDBType=" + DBAccess.AppCenterDBType + "&IsFirst=1&CustomerNo=" + BP.Difference.SystemConfig.CustomerNo;
            return url;
		}
		/// <summary>
		/// 查询
		/// </summary>
		/// <returns></returns>
		public string DoSearch()
		{
			return "../../Comm/Search.htm?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
		}
		/// <summary>
		/// 调用分析API
		/// </summary>
		/// <returns></returns>
		public string DoGroup()
		{
			return "../../Comm/Group.htm?s=34&FK_MapData=" + this.No + "&EnsName=" + this.No;
		}
		/// <summary>
		/// 数据源管理
		/// </summary>
		/// <returns></returns>
		public string DoDBSrc()
		{
			return "../../Comm/Search.htm?s=34&FK_MapData=" + this.No + "&EnsName=BP.Sys.SFDBSrcs";
		}
		 
		public string DoPageLoadFull()
		{
			return "../../Admin/FoolFormDesigner/MapExt/PageLoadFull.htm?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
		}
		public string DoInitScript()
		{
			return "../../Admin/FoolFormDesigner/MapExt/InitScript.htm?s=34&FK_MapData=" + this.No + "&ExtType=PageLoadFull&RefNo=";
		}
		/// <summary>
		/// Excel表单属性.
		/// </summary>
		/// <returns></returns>
		public string DoBodyAttr()
		{
            return "../../Admin/FoolFormDesigner/MapExt/BodyAttr.htm?s=34&FK_MapData=" + this.No + "&ExtType=BodyAttr&RefNo=";
		}
		/// <summary>
		/// 表单事件
		/// </summary>
		/// <returns></returns>
		public string DoEvent()
		{
            return "../../Admin/CCFormDesigner/Action.htm?FK_MapData=" + this.No + "&T=sd&FK_Node=0";
		}
		/// <summary>
		/// 导出表单
		/// </summary>
		/// <returns></returns>
		public string DoExp()
		{
            return "../../Admin/FoolFormDesigner/ImpExp/Exp.htm?FK_MapData=" + this.No;
		}
		#endregion 方法.
	}
	/// <summary>
	/// Excel表单属性s
	/// </summary>
	public class MapFrmExcels : EntitiesNoName
	{
		#region 构造
		/// <summary>
		/// Excel表单属性s
		/// </summary>
		public MapFrmExcels()
		{
		}
		/// <summary>
		/// 得到它的 Entity
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new MapFrmExcel();
			}
		}
		#endregion

		#region 为了适应自动翻译成java的需要,把实体转换成List.
		/// <summary>
		/// 转化成 java list,C#不能调用.
		/// </summary>
		/// <returns>List</returns>
		public System.Collections.Generic.IList<MapFrmExcel> ToJavaList()
		{
			return (System.Collections.Generic.IList<MapFrmExcel>)this;
		}
		/// <summary>
		/// 转化成list
		/// </summary>
		/// <returns>List</returns>
		public System.Collections.Generic.List<MapFrmExcel> Tolist()
		{
			System.Collections.Generic.List<MapFrmExcel> list = new System.Collections.Generic.List<MapFrmExcel>();
			for (int i = 0; i < this.Count; i++)
			{
				list.Add((MapFrmExcel)this[i]);
			}
			return list;
		}
		#endregion 为了适应自动翻译成java的需要,把实体转换成List.
	}
}

/**
 * version: 1.0.0
 * create:2020.4.16
 * 依赖于jquery.min.js和ntkoofficecontrol.min.js
 */
var ocx; //控件对象

var ntko = {
	//痕迹状态
	showRevisionsFlag: false,
	//全屏标识
	fullScreenModeFlag: false,
	//初始化文档控件
	init: function() {
		ocx = document.getElementById("TANGER_OCX");
		if (window.navigator.platform == "Win64") {
			//增加对于PDF文件的支持
			ocx.AddDocTypePlugin(".pdf", "PDF.NtkoDocument", "4.0.1.0", "officecontrol/ntkooledocallx64.cab", 51, true);

		} else {
			//增加对于PDF文件的支持
			ocx.AddDocTypePlugin(".pdf", "PDF.NtkoDocument", "4.0.1.0", "officecontrol/ntkooledocall.cab", 51, true);
		}
		ntko.setTitlebar();

		ntko.setMenuItems({
			//新建
			FileNew: false,
			//打开
			FileOpen: false,
			//关闭
			FileClose: false,
			//保存
			FileSave: false,
			//另存为
			FileSaveAs: false,
			//打印
			FilePrint: true,
			//打印预览
			FilePrintPreview: true,
			//页面设置
			FilePageSetup: true,
			//属性
			FileProperties: true
		});
	},
	//新建word文档
	createNew: function() {
		ocx.CreateNew("Word.Document");
	},
	//添加自定义菜单
	addCustomToolBar: function(params) {
		var items = [];
		items = jQuery.extend(items, params);
		for (var prop in items) {
			if (prop) {
				ocx.AddCustomToolButton(items[prop].ButtonText + "   ", items[prop].ImgIndex);
			}
		}
		ocx.CustomToolBar = true;
	},
	//设置、获取属性( String key [, Mixed value] )
	prop: function(key) {
		if (arguments[1] !== undefined) {
			ocx[key] = arguments[1];
		}
		return ocx[key];
	},
	//文档是否有修改
	isEditFlag: function() {
		return !ocx.ActiveDocument.Saved;
	},
	//设置文档编辑状态
	setSaved: function(boolvalue) {
		ocx.ActiveDocument.Saved = boolvalue;
	},
	//书签
	bookMark: {
		//判断书签是否存在
		exists: function(bookMarkName) {
			return ocx.ActiveDocument.BookMarks.Exists(bookMarkName);
		},
		//添加书签到光标位置
		add: function(bookMarkName) {
			//通过控件对象获取控件中打开的word文档对象,如果没有文档被打开,会出现文档对象不存在的错误
			var ntkodoc = ocx.ActiveDocument;
			//初始化一个range对象,
			var range = ntkodoc.range(ntkodoc.Application.Selection.range.Start, ntkodoc.Application.Selection.range.End);
			//添加书签到当前文档的光标位置
			ntkodoc.Bookmarks.Add(bookMarkName, ntkodoc.Application.Selection.range);
		},
		//获取书签值
		getValue: function(name) {
			ocx.GetBookmarkValue(name);
		},
		//设置书签值
		setValue: function(name, value) {
			ocx.SetBookmarkValue(name, value);
		},
		//删除书签
		del: function(bookMarkName) {
			//通过控件对象获取控件中打开的word文档对象,如果没有文档被打开,会出现文档对象不存在的错误
			var ntkodoc = ocx.ActiveDocument;
			ntkodoc.Bookmarks(bookMarkName).Delete();
		}
	},
	//设置文档只读
	setReadOnly: function(boolevalue, password) {
		if (password != null && password != undefined && password != "") {
			ocx.SetReadOnly(boolevalue, password, null, 3);
		} else {
			ocx.SetReadOnly(boolevalue);
		}
	},
	/**
	 * 设置是否启用痕迹保留
	 * @param {Object} boolevalue：true|false
	 * @param {Object} username：痕迹保留的用户名
	 */
	setReviewMode: function(boolevalue, username) {
		if (ocx.DocType == 1) {
			ocx.TrackRevisions(boolevalue);
		}
		if (boolevalue) {
			if (username != null && username != undefined && username != "") {
				ntko.setUserName(username);
			}
		}
	},
	//设置痕迹保留的用户名，仅当打开Word、WPS文档才有效
	setUserName: function(username) {
		ocx.WebUserName = username;
	},
	//设置是否显示痕迹
	setShowRevisions: function(boolevalue) {
		if (ocx.DocType == 1 || ocx.DocType == 6) {
			ocx.ActiveDocument.ShowRevisions = boolevalue;
		}
	},
	//接受或者取消所有修订
	setAllRevisions: function(boolevalue) {
		if (boolevalue) {
			ocx.ActiveDocument.AcceptAllRevisions(); //接受所有的痕迹修订
		} else {
			ocx.ActiveDocument.Application.WordBasic.RejectAllChangesInDoc(); //拒绝所有的痕迹修订
		}
	},
	//是否禁止粘贴
	setIsNoCopy: function(boolvalue) {
		ocx.IsNoCopy = boolvalue;
	},
	//全屏显示
	setFullScreenMode: function(boolvalue) {
		ocx.FullScreenMode = boolvalue;
	},
	//是否显示标题栏
	setTitlebar: function() {
		ocx.Titlebar = !ocx.Titlebar;
	},
	//是否显示工具栏
	setToolBar: function() {
		ocx.ToolBars = !ocx.ToolBars;
	},
	//是否显示菜单栏
	setMenubar: function() {
		ocx.Menubar = !ocx.Menubar;
	},
	//设置菜单项允许或者禁止（根据需要配合使用，也可使用下方的单独设置菜单项）
	setMenuItems: function(params) {
		var items = {
			//新建
			FileNew: true,
			//打开
			FileOpen: true,
			//关闭
			FileClose: true,
			//保存
			FileSave: true,
			//另存为
			FileSaveAs: true,
			//打印
			FilePrint: true,
			//打印预览
			FilePrintPreview: true,
			//页面设置
			FilePageSetup: true,
			//属性
			FileProperties: true
		}
		items = jQuery.extend(items, params);
		for (var prop in items) {
			if (prop) {
				ocx[prop] = items[prop];
			}
		}
	},
	//单独设置菜单项
	Menu: {
		//设置允许或者禁止文件菜单的新建项
		setFileNew: function(boolvalue) {
			ocx.FileNew = boolvalue;
		},
		//设置允许或者禁止文件菜单的打开项
		setFileOpen: function(boolvalue) {
			ocx.FileOpen = boolvalue;
		},
		//设置允许或者禁止文件菜单的关闭项
		setFileClose: function(boolvalue) {
			ocx.FileClose = boolvalue;
		},
		//设置允许或者禁止文件菜单的保存项
		setFileSave: function(boolvalue) {
			ocx.FileSave = boolvalue;
		},
		//设置允许或者禁止文件菜单的另存为项
		setFileSaveAs: function(boolvalue) {
			ocx.FileSaveAs = boolvalue;
		},
		//设置允许或者禁止文件菜单的打印项
		setFilePrint: function(boolvalue) {
			ocx.FilePrint = boolvalue;
		},
		//设置允许或者禁止文件菜单的打印预览项
		setFilePrintPreview: function(boolvalue) {
			ocx.FilePrintPreview = boolvalue;
		},
		//设置允许或者禁止文件菜单的页面设置项
		setFilePageSetup: function(boolvalue) {
			ocx.FilePageSetup = boolvalue;
		},
		//设置允许或者禁止文件菜单的属性项
		setFileProperties: function(boolvalue) {
			ocx.FileProperties = boolvalue;
		}
	},
	//从本地文件打开
	openLocal: function() {
		ocx.ShowDialog(1);
	},
	openFromURL: function(url) {
		ocx.OpenFromURL(ntko.resetUrl(url));
	},
	//异步方式打开文档
	beginOpenFromURL: function(url, showProgress) {
		if (showProgress == null || showProgress == undefined) showProgress = true;
		ocx.BeginOpenFromURL(ntko.resetUrl(url), showProgress);
	},
	/**
	 * saveToURL将文件保存到URL( String url, String fileName , String params [, Callable callback] )
	 * url：服务器地址
	 * fileName：保存的文件名
	 * params：额外提交的参数，以“&”分隔的参数－值对。例如参数值为：”key=thiskey&type=word&load=mywave”
	 * callback：提交成功后的回调
	 */
	saveToURL: function(url, fileName, params) {
		fileName = ntko.saveFileName(fileName);
		var response = ocx.SaveToURL(url, "EDITFILE", params, fileName, 0);
		if (arguments[3] !== undefined && typeof arguments[3] === "function") {
			arguments[3].call(response, response);
		}
	},
	/**
	 * 将文件保存到URL( String url, String fileName , String params 
	 * url：服务器地址
	 * fileName：保存的文件名
	 * params：额外提交的参数，以“&”分隔的参数－值对。例如参数值为：”key=thiskey&type=word&load=mywave”
	 * 此方法因为是异步方法，没有返回值，需要后续处理的在事件AfterPublishAsPDFToURL中处理
	 */
	savePDFToURL: function(url, fileName, params) {
		var response = ocx.PublishAsPDFToURL(url, "EDITFILE", params, fileName, 0,
			null, //sheetname,保存excel的哪个表格
			true, //IsShowUI,是否显示保存界面
			false, // IsShowMsg,是否显示保存成功信息
			false, // IsUseSecurity,是否使用安全特性
			null, // OwnerPass,安全密码.可直接传值
			false, //IsPermitPrint,是否允许打印
			true //IsPermitCopy,是否允许拷贝
		);
	},
	/**
	 * 将文件保存到本地D盘
	 * @param {Object} fileName：文件名
	 */
	saveToLocal: function(fileName) {
		fileName = ntko.saveFileName(fileName);
		//默认保存到D盘
		ocx.SaveToLocal("D:\\" + fileName, true, true);
	},
	/**
	 * 打印
	 * @param {Object} isBackground
	 */
	printDoc: function(isBackground) {

		var oldOption;
		try {
			var objOptions = ocx.ActiveDocument.Application.Options;
			oldOption = objOptions.PrintBackground;
			objOptions.PrintBackground = isBackground;
		} catch (e) {

		}
		ocx.printout(true);

		try {
			var objOptions = ocx.ActiveDocument.Application.Options;
			objOptions.PrintBackground = oldOption;
		} catch (e) {

		}
	},
	resetUrl: function(url) {
		var el = document.createElement("A");
		el.href = url;
		return el.href;
	},
	//保存文件名
	saveFileName: function(fileName) {
		if (fileName == null || fileName == undefined || fileName == "") {
			fileName = "文档";
		}
		if (fileName.lastIndexOf(".") == -1) {
			fileName += ntko.getFileDot();
		}
		return fileName;
	},
	getFileDot: function() {
		switch (ocx.DocType) {
			case 1:
				return ".doc";
			case 2:
				return ".xls";
			case 3:
				return ".ppt";
			case 4:
				return ".vso"
			case 5:
				return ".pro";
			case 6:
				return ".wps";
			case 7:
				return ".et";
			default:
				return ".doc";
		}
	},
	//判断红头图片是否存在
	htExists: function() {
		var flag = false;
		var doc = ocx.ActiveDocument;
		var app = doc.Application;
		var sh = doc.Shapes;
		var shc = sh.Count;
		//循环遍历图片
		for (var i = 1; i <= shc; i++) {
			//如果图片的TYPE属性值不等于12，则说明只是普通图片对象
			if (sh(i).Type != 12) {
				if (sh(i).AlternativeText == "htPic") {
					flag = true;
				}
			}
		}
		return flag;
	},
	//设置红头图片的标识
	setHtIdent: function() {
		var doc = ocx.ActiveDocument;
		var app = doc.Application;
		var sh = doc.Shapes;
		var shc = sh.Count;
		//循环遍历图片
		for (var i = 1; i <= shc; i++) {
			//如果图片的TYPE属性值不等于12，则说明只是普通图片对象
			if (sh(i).Type != 12) {
				if (sh(i).AlternativeText == "") {
					//将图片序列号赋值图片的可选文字属性
					sh(i).AlternativeText = "htPic";
				}
			}
		}
	},
	//隐藏、显示红头图片
	htVisible: function(flag) {
		var doc = ocx.ActiveDocument;
		var app = doc.Application;
		var sh = doc.Shapes;
		var shc = sh.Count;
		//循环遍历图片
		for (var i = 1; i <= shc; i++) {
			//如果图片的TYPE属性值不等于12，则说明只是普通图片对象
			if (sh(i).Type != 12) {
				if (sh(i).AlternativeText == "htPic") {
					sh(i).Visible = flag;
				}
			}
		}
	},
	//字符串查询
	getQuery: function(key) {
		var search = location.search.slice(1);
		var arr = search.split("&");
		for (var i = 0; i < arr.length; i++) {
			var ar = arr[i].split("=");
			if (ar[0] == key) {
				if (unescape(ar[1]) == 'undefined') {
					return "";
				} else {
					return unescape(ar[1]);
				}
			}
		}
		return "";
	}
};

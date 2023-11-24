		//扩展设置信息
var mapExt = null;

var global = window;
atParas = GetQueryString("AtParas");
	    //初始化页面
       function initTableSPage(obj, oid, dtlKeyOfen,type) {
           $("#TB_TS_Key").val('');
    	   mapExt = obj;
    	   var webUser = new WebUser();
    	   
        	
        	 global.selectedRows = [];
        	 //设置变量
		     global.FK_MapData = mapExt.FK_MapData;
	         global.AttrOfOper = dtlKeyOfen ? dtlKeyOfen : mapExt.AttrOfOper;
	         global.oid = oid;
            //设置单选还是多选
            global.selectType = mapExt.GetPara("SelectType");
            //如果单选
            mapExt.ShowCheckBox = selectType=="0"?false:true;
            
            //设置标题.
            var title = mapExt.GetPara("Title");
    	    $("#TSTitle").text(title); 
            
            //设置查询提示
             var tip = mapExt.GetPara("SearchTip");
             var span = $("#TB_TS_Key").siblings().eq(1).children().eq(1);
     	    span = span.html(tip);
     	    
     	    //点击关键字查询的操作
           $("#search").unbind('click').click(function () {
               
	        	InputKeyWordData();
           });
		    
	         //获取表格的数据源
	          var tableUrl = mapExt.Tag2; 
	          if (tableUrl == "" || tableUrl == "") {
		            alert('配置错误:查询数据源，初始化表格的数据源必须都不能为空。');
		            return;
		       }
	         
	         
		     // 初始化加载
	        var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
	        frmEleDBs.Retrieve("FK_MapData", FK_MapData, "EleID", global.AttrOfOper, "RefPKVal", oid);
	        $.each(frmEleDBs, function (i, o) {
	            global.selectedRows.push({
	                "No": o.Tag1,
	                "Name": o.Tag2,
	            });
	        });
            
	        
	        global.count = frmEleDBs.length;
		    //改变完成初始状态
		   changeDoneState(count, "TSDone");
		    
	        global.searchTableColumns = [];
	        global.searchTableColumns.push({
				field: "_checkbox",
				checkbox: true,
				formatter: function (value, row, index) {
					if (row.checked) {
						return {
							"checked": true
						};
					}
				}
			});
	        
	        var columns = mapExt.Tag;
	      	//设置bootstrapTable显示列的中文名称.
	        if (typeof columns == "string" && columns!=null && columns!="") {

	            $.each(columns.split(","), function (i, o) {
	                var exp = o.split("=");
	                var field;
	                var title;
	                if (exp.length == 1) {
	                    field = title = exp[0];
	                } else if (exp.length == 2) {
	                    field = exp[0];
	                    title = exp[1];
	                }
	                if (!isLegalName(field)) {
	                    return true;
	                }
	                searchTableColumns.push({
	                    field: field,
	                    title: title
	                });
	            });
	        } else {
	            // by default
	            searchTableColumns.push({
	                field: 'No',
	                title: '编号'
	            });
	            searchTableColumns.push({
	                field: 'Name',
	                title: '名称'
	            });
	        }
	      
	      	//设置bootstrapTable 表格选项
	        var options = {
	            striped: true,
	            cache: false,
	            sortOrder: "asc",
	            strictSearch: true,
	            minimumCountColumns: 2,
	            clickToSelect: true,
	            singleSelect:!mapExt.ShowCheckBox,
	            sortable: false,
	            cardView: false,
	            detailView: false,
	            uniqueId: "No",
	            columns: searchTableColumns
		   };
	      	
	        if (selectType == "0") {
				options.onCheck = function (row, element) {
					removeAllSelectedData();
					
					changeDoneState(1, "TSDone");
					addSelectedData(global.selectedRows, [row]);
					var mtags = $("#" + global.AttrOfOper+ "_mtags")
				    mtags.mtags("loadData", global.selectedRows);
                    var text = mtags.mtags("getText");
                    $("#TB_" + global.AttrOfOper).val(text);//给隐藏的input赋值
                    // 单选复制当前表单
                    if (global.selectType == "0" && global.selectedRows.length == 1) {
                        ValSetter(mapExt.Tag4, global.selectedRows[0].No);
                        FullIt(global.selectedRows[0].No, mapExt.MyPK, "TB_" + mapExt.AttrOfOper,type);
                    }

				};
				options.onUncheck = function (row, element) {
					changeDoneState(0, "TSDone");
					removeSelectedData(global.selectedRows, [row]);
					var mtags = $("#" + global.AttrOfOper + "_mtags")
					mtags.mtags("loadData", global.selectedRows);
					var text = mtags.mtags("getText");
					$("#TB_" + global.AttrOfOper).val(text);//给隐藏的input赋值
				};
			} else {
				options.onCheck = function (row, element) {
					addSelectedData(global.selectedRows, [row]);
					global.count++;
					changeDoneState(global.count, "TSDone");
					var mtags = $("#" + global.AttrOfOper + "_mtags");
				    mtags.mtags("loadData", global.selectedRows);
                    var text = mtags.mtags("getText");
                    $("#TB_" + global.AttrOfOper).val(text);//给隐藏的input赋值
                    // 单选复制当前表单
                    if (global.selectType == "0" && global.selectedRows.length == 1) {
                        ValSetter(mapExt.Tag4, global.selectedRows[0].No);
                        FullIt(global.selectedRows[0].No, mapExt.MyPK, "TB_" + mapExt.AttrOfOper,type);
                    }
				};
				options.onUncheck = function (row, element) {
					global.count--;
					changeDoneState(global.count, "TSDone", mapExt);
					removeSelectedData(global.selectedRows, [row]);
					var mtags = $("#" + global.AttrOfOper + "_mtags")
					mtags.mtags("loadData", global.selectedRows);
					var text = mtags.mtags("getText");
					$("#TB_" + global.AttrOfOper).val(text);//给隐藏的input赋值
				};
				options.onCheckAll = function (rows) {
					global.count = rows.length;
					changeDoneState(global.count, "TSDone");
					addSelectedData(global.selectedRows, rows);
					var mtags = $("#" + global.AttrOfOper+ "_mtags")
				    mtags.mtags("loadData", global.selectedRows);
                    var text = mtags.mtags("getText");
                    $("#TB_" + global.AttrOfOper).val(text);//给隐藏的input赋值
                    // 单选复制当前表单
                    if (global.selectType == "0" && global.selectedRows.length == 1) {
                        ValSetter(mapExt.Tag4, global.selectedRows[0].No);
                        FullIt(global.selectedRows[0].No, mapExt.MyPK, "TB_" + mapExt.AttrOfOper,type);
                    }
				};
				options.onUncheckAll = function (rows) {
					global.count = 0;
					changeDoneState(global.count, "TSDone");
					removeAllSelectedData();
					var mtags = $("#" + global.AttrOfOper + "_mtags")
					mtags.mtags("loadData", []);
					var text = mtags.mtags("getText");
					$("#TB_" + global.AttrOfOper).val(text);//给隐藏的input赋值
				};
			}
	        
	        $('#treeGrid').bootstrapTable(options);
	        
	        
	        if(tableUrl.indexOf("@Key")>-1)
	        	tableUrl.replace(new RegExp("@Key", "gm"),"");
	        //加载表格数据
           
           InputKeyWordData();

        }
       
        function isLegalName(name) {
	        if (!name) {
	            return false;
	        }
	        return name.match(/^[a-zA-Z\$_][a-zA-Z\d\$_]*$/);
	    }
        
	    //加载表格查询数据
        function loadViewGrid(gridUrl, paras, sqlWhere) {
            var global = window;
            var json;
            if (mapExt.DBType == 0) {
                var mapExt1 = new Entity("BP.Sys.MapExt", mapExt);
                mapExt1.MyPK = mapExt.MyPK;
                json = mapExt1.DoMethodReturnString("GetDataTableByField", "Tag2", paras, sqlWhere, GetQueryString("WorkID"));
                if (json.indexOf("err@") != -1) {
                    alert(json);
                    return;
                }
                json = JSON.parse(json);
            } else {
                json = DBAccess.RunDBSrc(gridUrl); //执行url返回json.
            }
	    	
	    	var selectedRows = window.selectedRows;
	    	if ($.isArray(selectedRows)) {
	            $.each(json, function (i, o) {
	                var sel = $.grep(selectedRows, function (obj) {
	                    return o.No == obj.No;
	                });
	                if (sel.length > 0) {
	                    o.checked = true;
	                    //多选时禁用已选
	                    if(mapExt.ShowCheckBox == true)
	                    	o.disabled = true;
	                }
	            });
	        }
	    	
	    	 $('#treeGrid').bootstrapTable("load", json);
	    }
        
        
      //输入关键字进行查询
function InputKeyWordData() {
           
           var UserNo = GetQueryString("UserNo");
           var webUser = new WebUser();
           var RefPKVal = GetQueryString("RefPKVal");
           var keyWord = $("#TB_TS_Key").val();
           var sqlWhere = "";
           var paras = atParas;
            paras += "@Key=" + keyWord;
           var dbSrc = mapExt.Tag2;
           dbSrc = dbSrc.replace(/~/g, "'");
           dbSrc = dbSrc.replace('@WebUser.No', webUser.No);
		   dbSrc = dbSrc.replace('@WebUser.Name', webUser.Name);
		   dbSrc = dbSrc.replace('@WebUser.FK_Dept', webUser.FK_Dept);
		   dbSrc = dbSrc.replace('@WebUser.DeptName', webUser.DeptName);
		   dbSrc = dbSrc.replace("@WebUser.FK_DeptNameOfFull", webUser.FK_DeptNameOfFull);
           var reg = new RegExp("@Key", "g");
           dbSrc = dbSrc.replace(reg, keyWord);
          
            loadViewGrid(dbSrc, paras, sqlWhere);
           
       }
		
       function addSelectedData(globalSelectedRows, selectedRows) {
	        if (!$.isArray(globalSelectedRows) || !$.isArray(selectedRows)) {
	            return;
		   }
		   var vals = [];
	        $.each(selectedRows, function (i, o) {
	            var sel = $.grep(globalSelectedRows, function (obj) {
	                return obj.No == o.No;
	            });
	            if (sel.length == 0) {
					var val = o.POP_Value ? o.POP_Value : "";
					vals.push(o.No + "," + o.Name + "," + val);
	                //SaveFrmEleDB(FK_MapData, global.AttrOfOper, oid, o.No, o.Name, val);
	                globalSelectedRows.push(o);
	            }
			});
		   SaveFrmEleDBs(FK_MapData, global.AttrOfOper,oid,vals.join(";"));
	    }
	    function removeSelectedData(globalSelectedRows, selectedRows) {
	        if (!$.isArray(globalSelectedRows) || !$.isArray(selectedRows)) {
	            return;
	        }
	        $.each(selectedRows, function (i, o) {
	            for (var index = 0; index < globalSelectedRows.length; index++) {
	                if (o.No == globalSelectedRows[index].No) {
	                    DeleteFrmEleDB(global.AttrOfOper, oid, o.No);
	                    globalSelectedRows.splice(index, 1);
	                    break;
	                }
	            }
	        });
	    }
	function removeAllSelectedData() {
		global.selectedRows = [];
			var globalSelectedRows = selectedRows;
			if (!$.isArray(globalSelectedRows) || !$.isArray(selectedRows)) {
	            return;
	        }
			/*for (var index = 0; index < globalSelectedRows.length; index++) {
				DeleteFrmEleDB(global.AttrOfOper, oid, globalSelectedRows[index].No);
			}*/
			//删除所有的数据
			//debugger
			var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
			frmEleDBs.Delete("FK_MapData", global.FK_MapData, "RefPKVal", oid, "EleID", global.AttrOfOper);
			globalSelectedRows = [];
		}
		function SaveFrmEleDBs(fk_mapData, keyOfEn, oid, vals) {
			var frmEleDBs = new Entities("BP.Sys.FrmEleDBs");
			frmEleDBs.DoMethodReturnString("SaveFrmEleDBs",fk_mapData, keyOfEn, oid, vals);
		}

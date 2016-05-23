//定义功能树存储数组，数组中元素对应easyui-tabs控件中的tab页
var functrees = [];
/*
easyui-tabs功能导航区定义，added by liuxc
说明：
1.functrees数组下元素：(1)Id:tab页中的tree控件的id值，tab页的id为"tab_" + 此id；(2)Name:tab页标题；(3)AttrCols:定义WebService获取数据时，要写入node.attributes中的属性列表；(4)ServiceCount:定义此树结构中一共要连接WebService的次数，此处是为便于编程而设置的，一定要设置正确；(5)RootASC:树结构中，如果存在多个根节点，则此项设置是为这多个根节点进行排序，其中Field即排序依据的属性名称，Index即为按Field值排列的顺序；(6)Nodes:tree中的节点数组，功能支持如下：
①支持无限级节点设置；②支持任一级别从WebService获取DataTable的Json数据填充（此连接WebService使用的是CCBPMDesignerData.js文件中的ajaxService方法，未另写方法，请注意）；③支持各级节点的图标、右键绑定菜单、展开状态、双击链接Url的规则设置，支持多级嵌套规则设置；④链接Url支持node属性值、node.attributes属性值及WebUser属性值、JS表达式计算结果的自动替换（使用“@@属性字段名”来代替要替换的属性；使用`符号来将要计算结果的JS表达式前后包含起来，表达式中允许含有@@参数名，计算时，先将@@参数名替换成对应的参数值，然后进行计算JS表达式。如设置Url: "Rpt/Group.aspx?FK_Flow=@@fk_flow&RptNo=`'ND'+parseInt('@@fk_flow')+'MyRpt'`"，@@fk_flow=001，则打开页面时，自动计算为："Rpt/Group.aspx?FK_Flow=001&RptNo=ND1MyRpt"）
2.Nodes数组下元素：(1)Type:节点类型，Node=普通定义节点，Service=通过获取WebService数据填充的节点；(2)ServiceMethod:ajaxService方法传参中method的值，即调用的获取数据的方法，Service类型节点特有属性；(3)CodId:WebService返回的DataTable Json数据代表节点Id的列名，Service类型节点特有属性；(4)ColParentId:WebService返回的DataTable Json数据代表父级节点Id的列名，Service类型节点特有属性；(5)ColName:WebService返回的DataTable Json数据代表节点文字的列名，Service类型节点特有属性；(6)RootParentId:WebService返回的DataTable Json数据代表根节点的父级Id的值，Service类型节点特有属性；(7)ColDefine:WebService返回的DataTable Json数据，设置的此列，根据此列的值进行设置各节点的图标、右链菜单以及双击打开页面，Service类型节点特有属性；(8)Defines:此数组的元素代表ColDefine所设列的详细规则设置，每个元素代表一种情况，整个设置可以理解为：
swith(ColDefine1.Value){
case 'aaa':
node.IconCls='icon-aaa';
node.MenuId = 'menu-aaa';
node.Url = 'url-aaa';
break;
case 'bbb':
swith(ColDefine2.Value){
case 'ccc':
node.IconCls='icon-ccc';
node.MenuId = 'menu-ccc';
node.Url = 'url-ccc';
break;
default:
......
}
break;
default:    //未设置Value值，则表示此项
......
}
此项规则设置，可以进行多级嵌套设置，即Defines元素中再包含ColDefine设置，Service类型节点特有属性；(9)Id:节点node.id值，Node类型节点特有属性；(10)ParentId:节点的父节点node.id，根节点的父节点id请设置为null，Node类型节点特有属性；(11)Name:节点node.text值，Node类型节点特有属性；(12)MethodParams:Service类型获取WebService数据时，向WebService发送的参数对象，对象中的每个参数，在WebService端可以用Request["参数名"]获取；(12)Opened:节点加载后是否是展开状态；
3.Defines数组下元素：(1)Value:规则判断值；(2)ColDefine:规则判断所用的字段名称；(3)Defines:具体规则设置，见上方规则设置说明；(4)IconCls:节点图标对应的css样式名称；(5)MenuId:节点右链菜单的id，为easyui-menu；(6)Url:节点双击在右侧tab页打开的网页Url，支持node属性值、node.attributes属性值、WebUser属性值以及JS表达式计算结果的自动替换；(7)LazyLoad:是否使用惰性加载，设置true时，在展开当前规则所表示的节点时，加载此规则下Nodes下面的列表，此列表也支持向WebService动态获取；(8)InheritForChild:当前规则所表示的节点数据中，需要向下级传递参数的设置数组，比如设置：[{From: "@@id", To: "fk_flow"}]，表示将当前节点数据中的@@id对应的数据传递给此节点以下的子节点node.attributes.fk_flow属性，以供以下节点使用，此设置为数组，可设置多个；(9)Nodes:惰性加载，展开节点时，加载的节点数据设置；(10)Opened:节点加载后是否是展开状态；(11)Inherits:当前/规则所表示的节点，可以从上级获取的属性数组，这些属性必须在上级节点中的InheritForChild中设置过，比如设置：["fk_flow", "fk_node"]，表示此节点将从上级的节点中继承fk_flow和fk_node这两个属性，保存在node.attributes对象里，需要注意的是：只有节点上设置了Inherits属性，这个节点才能继承设置过的上级节点属性，不设置，不能获得；
*/
//1.流程库
functrees.push({
    Id: "flowTree",
    Name: "流程",
    AttrCols: ["TType", "DType", "IsParent"],
    ServiceCount: 1,
    Nodes: [
			{ Type: "Service", ServiceMethod: "GetFlowTree", ColId: "No", ColParentId: "ParentNo", ColName: "Name", RootParentId: "F0",
			    ColDefine: "TType", Defines: [
											{ Value: "FLOWTYPE", ColDefine: "ParentNo",
											    Defines: [
															{ Value: "F0", IconCls: "icon-flowtree", MenuId: "mFlowRoot", Opened: true },
															{ IconCls: "icon-tree_folder", MenuId: "mFlowSort" }
														]
											},
											{ Value: "FLOW", ColDefine: "DType", Defines: [
                                                { Value: "1", IconCls: "icon-flow1", MenuId: "mFlow", InheritForChild: [{ From: "@@id", To: "fk_flow"}], Inherits: ["fk_flow"], Url: "Designer.aspx?FK_Flow=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID&Flow_V=2" },
                                                { IconCls: "icon-flow1", MenuId: "mFlow", InheritForChild: [{ From: "@@id", To: "fk_flow"}], Inherits: ["fk_flow"], Url: "Designer.aspx?FK_Flow=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID&Flow_V=1" }
                                            ], LazyLoad: true, Nodes: [
                                                { Type: "Node", Id: "RelatedFunction", ParentId: null, Name: "基础功能", Opened: false, TType: "NORMAL", DType: "-1", IconCls: "icon-FuncFolder", Inherits: ["fk_flow"],
                                                    Nodes: [
						                                { Type: "Node", Id: "FlowAttrs", ParentId: "RelatedFunction", Name: "流程属性", TType: "NORMAL", DType: "-1", IconCls: "icon-property", Inherits: ["fk_flow"], Url: "../../Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.FlowSheets&No=@@fk_flow" },
						                                { Type: "Node", Id: "FlowAttrs", ParentId: "RelatedFunction", Name: "流程属性New", TType: "NORMAL", DType: "-1", IconCls: "icon-property", Inherits: ["fk_flow"], Url: "../../Comm/RefFunc/UIEn.aspx?EnsName=BP.WF.Template.FlowExts&No=@@fk_flow" },
						                                { Type: "Node", Id: "NodeAttrs", ParentId: "RelatedFunction", Name: "节点设置", TType: "NORMAL", DType: "-1", IconCls: "icon-Node", Inherits: ["fk_flow"], Url: "../AttrFlow/NodeAttrs.aspx?FK_Flow=@@fk_flow" },
                                                    //						                                { Type: "Node", Id: "BatchEditNode", ParentId: "RelatedFunction", Name: "批量修改节点属性", TType: "NORMAL", DType: "-1", IconCls: "icon-edit", Inherits: ["fk_flow"], Url: "../AttrFlow/FeatureSetUI.aspx?FK_Flow=@@fk_flow&FK_Node=&DoType=Name" },
						                                {Type: "Node", Id: "RunFlow", ParentId: "RelatedFunction", Name: "运行流程", TType: "NORMAL", DType: "-1", IconCls: "icon-RunFlow", Inherits: ["fk_flow"], Url: "../TestFlow.aspx?FK_Flow=@@fk_flow", Target: "_blank" },
						                                { Type: "Node", Id: "FlowCheck", ParentId: "RelatedFunction", Name: "流程检查", TType: "NORMAL", DType: "-1", IconCls: "icon-CheckFlow", Inherits: ["fk_flow"], Url: "../AttrFlow/CheckFlow.aspx?FK_Flow=@@fk_flow" },

						                                { Type: "Node", Id: "f1", ParentId: "RelatedFunction", Name: "发起前置列表", TType: "NORMAL", DType: "-1", IconCls: "icon-StartGuide", Inherits: ["fk_flow"], Url: "../AttrFlow/StartGuide.aspx?NodeID=0&FK_Flow=@@fk_flow" },
						                                { Type: "Node", Id: "f2", ParentId: "RelatedFunction", Name: "发起限制规则", TType: "NORMAL", DType: "-1", IconCls: "icon-limit", Inherits: ["fk_flow"], Url: "../AttrFlow/Limit.aspx?NodeID=0&FK_Flow=@@fk_flow" },
						                                { Type: "Node", Id: "f3", ParentId: "RelatedFunction", Name: "自动发起", TType: "NORMAL", DType: "-1", IconCls: "icon-AutoStart", Inherits: ["fk_flow"], Url: "../AttrFlow/AutoStart.aspx?NodeID=0&FK_Flow=@@fk_flow" },


						                                { Type: "Node", Id: "FlowAction", ParentId: "RelatedFunction", Name: "流程事件消息", TType: "NORMAL", DType: "-1", IconCls: "icon-Event", Inherits: ["fk_flow"], Url: "../Action.aspx?NodeID=0&FK_Flow=@@fk_flow" },
						                                { Type: "Node", Id: "TruckViewPower", ParentId: "RelatedFunction", Name: "轨迹查看权限", TType: "NORMAL", DType: "-1", IconCls: "icon-Setting", Inherits: ["fk_flow"], Url: "../AttrFlow/TruckViewPower.aspx?FK_Flow=@@fk_flow" },
						                                { Type: "Node", Id: "TruckViewPower", ParentId: "RelatedFunction", Name: "模版导入", TType: "NORMAL", DType: "-1", IconCls: "icon-redo", Inherits: ["fk_flow"], Url: "../AttrFlow/Imp.aspx?FK_Flow=@@fk_flow" },
						                                { Type: "Node", Id: "TruckViewPower", ParentId: "RelatedFunction", Name: "模版导出", TType: "NORMAL", DType: "-1", IconCls: "icon-unredo", Inherits: ["fk_flow"], Url: "../AttrFlow/Exp.aspx?FK_Flow=@@fk_flow" }
                                                        ]
                                                }, 
                                                { Type: "Node", Id: "JianK", ParentId: null, Name: "流程监控", Opened: false, TType: "NORMAL", DType: "-1", IconCls: "icon-FuncFolder", Inherits: ["fk_flow"],
                                                    Nodes: [
						            { Type: "Node", Id: "WorkPanel", ParentId: "JianK", Name: "监控面板", TType: "WORKPANEL", DType: "-1", IconCls: "icon-Monitor", Url: "../CCBPMDesigner/App/OneFlow/Welcome.aspx?FK_Flow=@@fk_flow" },
						            { Type: "Node", Id: "list", ParentId: "JianK", Name: "节点列表", TType: "WORKPANEL", DType: "-1", IconCls: "icon-flows", Url: "../CCBPMDesigner/App/OneFlow/Nodes.aspx?FK_Flow=@@fk_flow" },
						            { Type: "Node", Id: "SynthSearch", ParentId: "JianK", Name: "综合查询", TType: "SYNTHSEARCH", DType: "-1", IconCls: "icon-Search", Url: "../../Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=@@fk_flow&WFSta=all" },
						            { Type: "Node", Id: "SynthAnalysis", ParentId: "JianK", Name: "综合分析", TType: "SYNTHANALYSIS", DType: "-1", IconCls: "icon-Group", Url: "../../Comm/Group.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews&FK_Flow=@@fk_flow&WFSta=all" },
                                    { Type: "Node", Id: "SearchByFlow", ParentId: "JianK", Name: "实例增长分析", TType: "SEARCHBYFLOW", DType: "-1", IconCls: "icon-Grow", Url: "../FlowDB/InstanceGrowOneFlow.aspx?anaTime=mouth&FK_Flow=@@fk_flow" },
                                    { Type: "Node", Id: "FlowRunning", ParentId: "JianK", Name: "逾期未完成实例", TType: "FLOWRUNNING", DType: "-1", IconCls: "icon-Warning", Url: "../FlowDB/InstanceWarning.aspx?FK_Flow=@@fk_flow" },
                                    { Type: "Node", Id: "FlowExpired", ParentId: "JianK", Name: "逾期已完成实例", TType: "FLOWEXPIRED", DType: "-1", IconCls: "icon-overtime", Url: "../FlowDB/InstanceOverTimeOneFlow.aspx?anaTime=mouth&FK_Flow=@@fk_flow" },
						            { Type: "Node", Id: "DeleteLog", ParentId: "JianK", Name: "删除日志", TType: "DELETELOG", DType: "-1", IconCls: "icon-log", Url: "../../Comm/Search.aspx?EnsName=BP.WF.WorkFlowDeleteLogs&FK_Flow=@@fk_flow" },
						            { Type: "Node", Id: "Rptorder", ParentId: "JianK", Name: "数据订阅", TType: "DELETELOG", DType: "-1", IconCls: "icon-RptOrder", Url: "../CCBPMDesigner/App/RptOrder.aspx?FK_Flow=@@fk_flow" }
					              ]
                                                },
                                                { Type: "Node", Id: "DevAPI", ParentId: null, Name: "开发接口(API)", Opened: false, TType: "NORMAL", DType: "-1", IconCls: "icon-FuncFolder", Inherits: ["fk_flow"],
                                                    Nodes: [
						                                { Type: "Node", Id: "FlowField", ParentId: "DevAPI", Name: "字段视图", TType: "NORMAL", DType: "-1", IconCls: "icon-Field", Inherits: ["fk_flow"], Url: "../AttrFlow/FlowFields.aspx?FK_Flow=@@fk_flow" },
						                                { Type: "Node", Id: "AsyncBusinessTable", ParentId: "DevAPI", Name: "与业务表同步", TType: "NORMAL", DType: "-1", IconCls: "icon-DTS", Inherits: ["fk_flow"], Url: "../AttrFlow/DTSBTable.aspx?FK_Flow=@@fk_flow&FK_Node=&DoType=Name" },
						                                { Type: "Node", Id: "URLAPI", ParentId: "DevAPI", Name: "URL调用接口", TType: "NORMAL", DType: "-1", IconCls: "icon-URL", Inherits: ["fk_flow"], Url: "../AttrFlow/API.aspx?FK_Flow=@@fk_flow&DoType=Url" },
						                                { Type: "Node", Id: "SDKAPI", ParentId: "DevAPI", Name: "SDK开发接口", TType: "NORMAL", DType: "-1", IconCls: "icon-API", Inherits: ["fk_flow"], Url: "../AttrFlow/APICode.aspx?FK_Flow=@@fk_flow&DoType=SDK" },
						                                { Type: "Node", Id: "FEEAPI", ParentId: "DevAPI", Name: "代码事件开发接口", TType: "NORMAL", DType: "-1", IconCls: "icon-API", Inherits: ["fk_flow"], Url: "../AttrFlow/APICodeFEE.aspx?FK_Flow=@@fk_flow&FK_Node=&DoType=Name" }
                                                        ]
                                                },
                                                { Type: "Node", Id: "DataManage", ParentId: null, Name: "流程报表", Opened: false, TType: "NORMAL", DType: "-1", IconCls: "icon-FuncFolder", Inherits: ["fk_flow"],
                                                    Nodes: [
						                                { Type: "Node", Id: "DesignReport", ParentId: "DataManage", Name: "设计报表", TType: "NORMAL", DType: "-1", IconCls: "icon-DesignRpt", Inherits: ["fk_flow"], Url: "../../Rpt/OneFlow.aspx?FK_MapData=`'ND'+parseInt('@@fk_flow')+'Rpt'`&FK_Flow=@@fk_flow" },
						                                { Type: "Node", Id: "FlowSearch", ParentId: "DataManage", Name: "流程查询", TType: "NORMAL", DType: "-1", IconCls: "icon-Search", Inherits: ["fk_flow"], Url: "../../Rpt/Search.aspx?FK_Flow=@@fk_flow&RptNo=`'ND'+parseInt('@@fk_flow')+'MyRpt'`" },
						                                { Type: "Node", Id: "SearchAdv", ParentId: "DataManage", Name: "自定义查询", TType: "NORMAL", DType: "-1", IconCls: "icon-SQL", Inherits: ["fk_flow"], Url: "../../Rpt/SearchAdv.aspx?FK_Flow=@@fk_flow&RptNo=`'ND'+parseInt('@@fk_flow')+'MyRpt'`" },
						                                { Type: "Node", Id: "GroupSearch", ParentId: "DataManage", Name: "分组分析", TType: "NORMAL", DType: "-1", IconCls: "icon-Group", Inherits: ["fk_flow"], Url: "../../Rpt/Group.aspx?FK_Flow=@@fk_flow&RptNo=`'ND'+parseInt('@@fk_flow')+'MyRpt'`" },
						                                { Type: "Node", Id: "CrossReport", ParentId: "DataManage", Name: "交叉报表", TType: "NORMAL", DType: "-1", IconCls: "icon-D3", Inherits: ["fk_flow"], Url: "../../Rpt/D3.aspx?FK_Flow=@@fk_flow&RptNo=`'ND'+parseInt('@@fk_flow')+'MyRpt'`" },
						                                { Type: "Node", Id: "ContrastReport", ParentId: "DataManage", Name: "对比分析", TType: "NORMAL", DType: "-1", IconCls: "icon-Contrast", Inherits: ["fk_flow"], Url: "../../Rpt/Contrast.aspx?FK_Flow=@@fk_flow&RptNo=`'ND'+parseInt('@@fk_flow')+'MyRpt'`" }
                                                        ]
                                                },
//                                                { Type: "Node", Id: "CH", ParentId: null, Name: "时效考核", Opened: false, TType: "NORMAL", DType: "-1", IconCls: "icon-FuncFolder", Inherits: ["fk_flow"],
//                                                    Nodes: [
//						                                { Type: "CH", Id: "ByNodes", ParentId: "DataManage", Name: "按节点分析", TType: "NORMAL", DType: "-1", IconCls: "icon-DesignRpt", Inherits: ["fk_flow"], Url: "../../CH/ByNodes.aspx?FK_Flow=@@fk_flow" },
//						                                { Type: "CH", Id: "ByEmps", ParentId: "DataManage", Name: "按人员分析", TType: "NORMAL", DType: "-1", IconCls: "icon-Search", Inherits: ["fk_flow"], Url: "../../CH/ByEmps.aspx?FK_Flow=@@fk_flow" },
//						                                { Type: "CH", Id: "ByDepts", ParentId: "DataManage", Name: "按部门分析", TType: "NORMAL", DType: "-1", IconCls: "icon-SQL", Inherits: ["fk_flow"], Url: "../../CH/ByDepts.aspx?FK_Flow=@@fk_flow" },
//						                                { Type: "CH", Id: "GroupSearch", ParentId: "DataManage", Name: "分组分析", TType: "NORMAL", DType: "-1", IconCls: "icon-Group", Inherits: ["fk_flow"], Url: "../../CH/ByNodes.aspx?FK_Flow=@@fk_flow" },
//						                                { Type: "CH", Id: "CrossReport", ParentId: "DataManage", Name: "交叉报表", TType: "NORMAL", DType: "-1", IconCls: "icon-D3", Inherits: ["fk_flow"], Url: "../../CH/ByNodes.aspx?FK_Flow=@@fk_flow" },
//						                                { Type: "CH", Id: "ContrastReport", ParentId: "DataManage", Name: "对比分析", TType: "NORMAL", DType: "-1", IconCls: "icon-Contrast", Inherits: ["fk_flow"], Url: "../../CH/ByNodes.aspx?FK_Flow=@@fk_flow" }
//                                                        ]
//                                                },
                                                { Type: "Node", Id: "BindForm", ParentId: null, Name: "绑定表单", Opened: false, TType: "NORMAL", DType: "-1", IconCls: "icon-FuncFolder", Inherits: ["fk_flow"],
                                                    Nodes: [
						                                { Type: "Node", Id: "AddOneFrmToNodes", ParentId: "BindForm", Name: "加入表单", TType: "ADDTONODES", DType: "-1", IconCls: "", Inherits: ["fk_flow"], Url: "../AttrFlow/AddOneFrmToNodes.aspx?FK_Flow=@@fk_flow" },
						                                { Type: "Service", ServiceMethod: "GetBindingForms", MethodParams: { fk_flow: "@@fk_flow" }, ColId: "No", ColParentId: "ParentNo", ColName: "Name", RootParentId: null, IconCls: "icon-form", Url: "../AttrFlow/BindingForms.aspx?FK_MapData=`'@@id'.split('@')[0]`&FK_Flow=@@fk_flow" }
                                                        ]
                                                }
                                            ]
											}
										  ]
			},
            { Type: "Node", Id: "FlowFunc", ParentId: null, Name: "流程应用", Opened: true, TType: "FLOWFUNC", DType: "-1", IconCls: "icon-app",
                Nodes: [
						{ Type: "Node", Id: "FlowMonitor", ParentId: "FlowFunc", Name: "流程监控", Opened: false, TType: "FLOWMONITOR", DType: "-1", IconCls: "icon-tree_folder",
						    Nodes: [
						            { Type: "Node", Id: "WorkPanel", ParentId: "FlowMonitor", Name: "监控面板", TType: "WORKPANEL", DType: "-1", IconCls: "icon-Monitor", Url: "../CCBPMDesigner/App/Welcome.aspx?anaTime=slMouth&flowSort=slFlow&" },
						            { Type: "Node", Id: "WorkPanel", ParentId: "FlowMonitor", Name: "流程列表", TType: "WORKPANEL", DType: "-1", IconCls: "icon-flows", Url: "../CCBPMDesigner/Flows.aspx" },
						            { Type: "Node", Id: "SearchByKey", ParentId: "FlowMonitor", Name: "全文检索", TType: "SEARCHBYKEY", DType: "-1", IconCls: "icon-SearchKey", Url: "../../KeySearch.aspx" },
						            { Type: "Node", Id: "SynthSearch", ParentId: "FlowMonitor", Name: "综合查询", TType: "SYNTHSEARCH", DType: "-1", IconCls: "icon-Search", Url: "../../Comm/Search.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews" },
						            { Type: "Node", Id: "SynthAnalysis", ParentId: "FlowMonitor", Name: "综合分析", TType: "SYNTHANALYSIS", DType: "-1", IconCls: "icon-Group", Url: "../../Comm/Group.aspx?EnsName=BP.WF.Data.GenerWorkFlowViews" },
                                    { Type: "Node", Id: "SearchByFlow", ParentId: "FlowMonitor", Name: "实例增长分析", TType: "SEARCHBYFLOW", DType: "-1", IconCls: "icon-Grow", Url: "../FlowDB/InstanceGrow.aspx?anaTime=mouth" },
                                    { Type: "Node", Id: "FlowRunning", ParentId: "FlowMonitor", Name: "逾期未完成实例", TType: "FLOWRUNNING", DType: "-1", IconCls: "icon-Warning", Url: "../FlowDB/InstanceWarning.aspx" },
                                    { Type: "Node", Id: "FlowExpired", ParentId: "FlowMonitor", Name: "逾期已完成实例", TType: "FLOWEXPIRED", DType: "-1", IconCls: "icon-overtime", Url: "../FlowDB/InstanceOverTime.aspx?anaTime=mouth" },
						            { Type: "Node", Id: "DeleteLog", ParentId: "FlowMonitor", Name: "流程删除日志", TType: "DELETELOG", DType: "-1", IconCls: "icon-log", Url: "../../Comm/Search.aspx?EnsName=BP.WF.WorkFlowDeleteLogs" },
						            { Type: "Node", Id: "Rptorder", ParentId: "FlowMonitor", Name: "数据订阅", TType: "DELETELOG", DType: "-1", IconCls: "icon-RptOrder", Url: "../CCBPMDesigner/App/RptOrder.aspx" }
					              ]
						},
                        { Type: "Node", Id: "TimeCheck", ParentId: "FlowFunc", Name: "时效考核", TType: "TIMECHECK", DType: "-1", IconCls: "icon-tree_folder", Nodes: [
                            { Type: "Node", Id: "FlowAnalysis", ParentId: "TimeCheck", Name: "按流程分析", TType: "FLOWANALYSIS", DType: "-1", IconCls: "", Url: "../CCBPMDesigner/App/CH/ByFlows.aspx" },
                            { Type: "Node", Id: "DeptAnalysis", ParentId: "TimeCheck", Name: "按部门分析", TType: "DEPTANALYSIS", DType: "-1", IconCls: "", Url: "../CCBPMDesigner/App/CH/ByDepts.aspx" },
                            { Type: "Node", Id: "AvgAnalysis", ParentId: "TimeCheck", Name: "排名列表", TType: "AVGANALYSIS", DType: "-1", IconCls: "", Url: "../CCBPMDesigner/App/CH/List.aspx" }
                        ]
                        },
                        { Type: "Node", Id: "PDevAPI", ParentId: "FlowFunc", Name: "开发接口", TType: "PDEVAPI", DType: "-1", IconCls: "icon-tree_folder", Nodes: [
                            { Type: "Node", Id: "StructureIntegration", ParentId: "TimeCheck", Name: "组织结构集成", TType: "STRUCTUREINTEGRATION", DType: "-1", IconCls: "", Url: "../Org/Integration.aspx" },
                            { Type: "Node", Id: "CodeIntegrationAPI", ParentId: "TimeCheck", Name: "代码集成接口", TType: "CODEINTEGRATIONAPI", DType: "-1", IconCls: "", Url: "" },
                            { Type: "Node", Id: "CodeGeneration", ParentId: "TimeCheck", Name: "代码生成", TType: "CODEGENERATION", DType: "-1", IconCls: "", Url: "" },
                            { Type: "Node", Id: "FormComponent", ParentId: "TimeCheck", Name: "表单组件", TType: "FORMCOMPONENT", DType: "-1", IconCls: "", Url: "" }
                        ]
                        }
					  ]
            },
			{ Type: "Node", Id: "FlowCloud", ParentId: null, Name: "流程云", Opened: true, TType: "FLOWCLOUD", DType: "-1", IconCls: "icon-flowcloud",
			    Nodes: [
						{ Type: "Node", Id: "ShareFlow", ParentId: "FlowCloud", Name: "共有流程云", TType: "SHAREFLOW", DType: "-1", IconCls: "icon-flowpublic", Url: "../Clound/PubFlow.aspx" },
						{ Type: "Node", Id: "PriFlow", ParentId: "FlowCloud", Name: "私有流程云", TType: "PRIFLOW", DType: "-1", IconCls: "icon-flowprivate", Url: "../Clound/PriFlow.aspx" }
					  ]
			}
		  ]
});
//2.表单库
functrees.push({
    Id: "formTree",
    Name: "表单",
    AttrCols: ["TType"],
    RootASC: { Field: "TType", Index: ["FORMTYPE", "SRCROOT", "FORMREF", "CLOUNDDATA"] },
    ServiceCount: 2,
    Nodes: [
			{ Type: "Service", ServiceMethod: "GetFormTree", ColId: "No", ColParentId: "ParentNo", ColName: "Name", RootParentId: null,
			    ColDefine: "TType", Defines: [
											{ Value: "FORMTYPE", ColDefine: "ParentNo",
											    Defines: [
															{ Value: null, IconCls: "icon-formtree", MenuId: "mFormRoot", Opened: true },
															{ IconCls: "icon-tree_folder", MenuId: "mFormSort" }
														]
											},
											{ Value: "FORM", IconCls: "icon-form", MenuId: "mForm", Url: "../../MapDef/CCForm/Frm.aspx?FK_MapData=@@id&UserNo=@@WebUser.No&SID=@@WebUser.SID", LazyLoad: true, InheritForChild: [{ From: "@@id", To: "fk_frm"}], Nodes: [
                                                { Type: "Node", Id: "RelatedFunction", ParentId: null, Name: "相关功能", Opened: false, TType: "NORMAL", DType: "-1", IconCls: "icon-FuncFolder", Inherits: ["fk_frm"],
                                                    Nodes: [
						                                { Type: "Node", Id: "FormHome", ParentId: "RelatedFunction", Name: "表单主页", TType: "NORMAL", DType: "-1", IconCls: "icon-Home", Inherits: ["fk_frm"], Url: "../CCFormDesigner/Home.aspx?FK_MapData=@@fk_frm" },
						                                { Type: "Node", Id: "FormPreview", ParentId: "RelatedFunction", Name: "表单预览", TType: "NORMAL", DType: "-1", IconCls: "icon-Glasses", Inherits: ["fk_frm"], Url: "../../CCForm/Frm.aspx?FK_MapData=@@fk_frm&IsTest=1" },
						                                { Type: "Node", Id: "FormField", ParentId: "RelatedFunction", Name: "表单字段", TType: "NORMAL", DType: "-1", IconCls: "", Inherits: ["fk_frm"], Url: "../CCFormDesigner/FiledsList.aspx?FK_MapData=@@fk_frm" },
						                                { Type: "Node", Id: "PageLoadFull", ParentId: "RelatedFunction", Name: "装载填充", TType: "NORMAL", DType: "-1", IconCls: "icon-LoadFull", Inherits: ["fk_frm"], Url: "../../MapDef/MapExt/PageLoadFull.aspx?s=34&FK_MapData=@@fk_frm&ExtType=PageLoadFull" },
						                                { Type: "Node", Id: "InitScript", ParentId: "RelatedFunction", Name: "内置JS脚本", TType: "NORMAL", DType: "-1", IconCls: "icon-JavaScript", Inherits: ["fk_frm"], Url: "../../MapDef/MapExt/InitScript.aspx?s=34&FK_MapData=@@fk_frm" },
						                                { Type: "Node", Id: "FormAction", ParentId: "RelatedFunction", Name: "表单事件", TType: "NORMAL", DType: "-1", IconCls: "icon-Event", Inherits: ["fk_frm"], Url: "../Action.aspx?FK_MapData=@@fk_frm" },
//						                                { Type: "Node", Id: "BasicData", ParentId: "RelatedFunction", Name: "原始数据", TType: "NORMAL", DType: "-1", IconCls: "", Inherits: ["fk_frm"], Url: "../CCFormDesigner/BasicData.aspx?FK_MapData=@@fk_frm", },
						                                { Type: "Node", Id: "Imp", ParentId: "RelatedFunction", Name: "模版导入", TType: "NORMAL", DType: "-1", IconCls: "icon-redo", Inherits: ["fk_frm"], Url: "../CCFormDesigner/Imp.aspx?FK_MapData=@@fk_frm" },
						                                { Type: "Node", Id: "Exp", ParentId: "RelatedFunction", Name: "模版导出", TType: "NORMAL", DType: "-1", IconCls: "icon-unredo", Inherits: ["fk_frm"], Url: "../CCFormDesigner/Exp.aspx?FK_MapData=@@fk_frm" }
                                                        ]
                                                },
                                                { Type: "Node", Id: "DevAPI", ParentId: null, Name: "API接口", Opened: false, TType: "NORMAL", DType: "-1", IconCls: "icon-FuncFolder", Inherits: ["fk_frm"],
                                                    Nodes: [
						                                { Type: "Node", Id: "NEWAPI", ParentId: "DevAPI", Name: "新建/增加", TType: "NORMAL", DType: "-1", IconCls: "icon-URL", Inherits: ["fk_frm"], Url: "../../CCForm/Frm.aspx?FK_MapData=@@fk_frm&IsTest=1" },
						                                { Type: "Node", Id: "SEARCHAPI", ParentId: "DevAPI", Name: "列表/查询", TType: "NORMAL", DType: "-1", IconCls: "icon-Search", Inherits: ["fk_frm"], Url: "../../Comm/Search.aspx?EnsName=@@fk_frm" },
						                                { Type: "Node", Id: "GROUPAPI", ParentId: "DevAPI", Name: "分组分析", TType: "NORMAL", DType: "-1", IconCls: "icon-Group", Inherits: ["fk_frm"], Url: "../../Comm/Group.aspx?EnsName=@@fk_frm" },
						                                { Type: "Node", Id: "GROUPAPI", ParentId: "DevAPI", Name: "代码事例", TType: "NORMAL", DType: "-1", IconCls: "icon-API", Inherits: ["fk_frm"], Url: "../CCFormDesigner/APICode.aspx?FK_MapData=@@fk_frm" }

                                                        ]
                                                }
                                           ]
											},
                                            { Value: "SRC", IconCls: "icon-src", MenuId: "mFormSrc" }
										  ]
			},
			{ Type: "Node", Id: "SrcRoot", ParentId: null, Name: "数据源字典表", Opened: true, TType: "SRCROOT", IconCls: "icon-srctree", MenuId: "mSrcRoot",
			    Nodes: [
						{ Type: "Service", ServiceMethod: "GetSrcTree", ColId: "No", ColParentId: "ParentNo", ColName: "Name", RootParentId: "SrcRoot",
						    ColDefine: "TType", Defines: [
											{ Value: "SRC", IconCls: "icon-src", MenuId: "mSrc", Url: "../../Comm/RefFunc/UIEn.aspx?EnsName=BP.Sys.SFDBSrcs&No=@@id&t=" + Math.random() },
											{ Value: "SRCTABLE", IconCls: "icon-srctable", MenuId: "mSrcTable", Url: "../../MapDef/Do.aspx?DoType=EditSFTable&RefNo=@@id&t=" + Math.random() }
										  ]
						}
					  ]
			},
			{ Type: "Node", Id: "FormRef", ParentId: null, Name: "表单相关", Opened: true, TType: "FORMREF", IconCls: "icon-tree_folder",
			    Nodes: [
						{ Type: "Node", Id: "Enums", ParentId: "FormRef", Name: "枚举列表", TType: "ENUMS", IconCls: "icon-enum", Url: "../../Comm/Sys/EnumList.aspx?t=" + Math.random() },
						{ Type: "Node", Id: "JSLib", ParentId: "FormRef", Name: "JS验证库", TType: "JSLIB", IconCls: "icon-js", Url: "../../Comm/Sys/FuncLib.aspx?t=" + Math.random() }
					  ]
			},
			{ Type: "Node", Id: "CloundData", ParentId: null, Name: "表单云", Opened: true, TType: "CLOUNDDATA", IconCls: "icon-formcloud",
			    Nodes: [
						{ Type: "Node", Id: "PriForm", ParentId: "CloundData", Name: "共有表单云", TType: "PRIFORM", IconCls: "icon-flowpublic", Url: "../Clound/PubForm.aspx" },
						{ Type: "Node", Id: "ShareForm", ParentId: "CloundData", Name: "私有表单云", TType: "SHAREFORM", IconCls: "icon-flowprivate", Url: "../Clound/PriForm.aspx" }
					  ]
			}
		  ]
});
//3.组织结构
functrees.push({
    Id: "OrgTree",
    Name: "组织结构",
    AttrCols: ["TType"],
    ServiceCount: 1,
    Nodes: [
			{ Type: "Node", Id: "BasicSetting", ParentId: null, Name: "基础设置", Opened: true, TType: "BASICROOT", IconCls: "icon-tree_folder",
			    Nodes: [
						{ Type: "Node", Id: "Integration", ParentId: "BasicSetting", Name: "集成设置", TType: "INTEGRATION", IconCls: "icon-Guide", Url: "../Org/Integration.aspx" },
						{ Type: "Node", Id: "DeptTypies", ParentId: "BasicSetting", Name: "部门类型", TType: "DEPTTYPIES", IconCls: "icon-table", Url: "../../Comm/Ens.aspx?EnsName=BP.GPM.DeptTypes" },
						{ Type: "Node", Id: "Duties", ParentId: "BasicSetting", Name: "职务管理", TType: "DUTIES", IconCls: "icon-table", Url: "../../Comm/Ens.aspx?EnsName=BP.GPM.Dutys" },
						{ Type: "Node", Id: "Stations", ParentId: "BasicSetting", Name: "岗位管理", TType: "STATIONS", IconCls: "icon-table", Url: "../../Comm/Search.aspx?EnsName=BP.GPM.Stations" },
						{ Type: "Node", Id: "OrgManage", ParentId: "BasicSetting", Name: "数据导入", TType: "ORGMANAGE", IconCls: "icon-Excel", Url: "http://ccflow.org/ToolsInitOrg.aspx" }
					  ]
			},
			{ Type: "Service", ServiceMethod: "GetStructureTree", ColId: "No", ColParentId: "ParentNo", ColName: "Name", RootParentId: "0",
			    ColDefine: "TType", Defines: [
											{ Value: "DEPT", ColDefine: "ParentNo",
											    Defines: [
															{ Value: "0", IconCls: "icon-tree_folder", MenuId: "mDeptRoot", InheritForChild: [{ From: "@@id", To: "fk_dept"}], Opened: true },
															{ IconCls: "icon-dept", MenuId: "mDept", InheritForChild: [{ From: "@@id", To: "fk_dept"}] }
														]
											},
											{ Value: "STATION", IconCls: "icon-station", MenuId: "mStation", Inherits: ["fk_dept"], InheritForChild: [{ From: "`'@@id'.split('|')[1]`", To: "fk_station"}], LazyLoad: true, Nodes: [
                                                { Type: "Service", ServiceMethod: "GetEmpsFromStation", Inherits: ["fk_dept", "fk_station"], MethodParams: { fk_dept: "@@fk_dept", fk_station: "@@fk_station" }, ColId: "No", ColParentId: "ParentNo", ColName: "Name", RootParentId: null, IconCls: "icon-user", MenuId: "mEmp", Url: "" }
                                            ]
											}
										  ]
			}
		  ]
});
//4.系统维护
functrees.push({
    Id: "sysTree",
    Name: "系统维护",
    Nodes: [
			{ Type: "Node", Id: "MenuRole", ParentId: null, Name: "菜单权限", Opened: true, IconCls: "icon-tree_folder",
			    Nodes: [
						{ Type: "Node", Id: "SysConfig", ParentId: "MenuRole", Name: "系统维护", IconCls: "icon-table", Url: "../../../GPM/AppList.aspx" },
						{ Type: "Node", Id: "RoleGroup", ParentId: "MenuRole", Name: "权限组维护", IconCls: "icon-table", Url: "../../Comm/SearchEUI.aspx?EnsName=BP.GPM.Groups" },
						{ Type: "Node", Id: "MenuForRole", ParentId: "MenuRole", Name: "按菜单分配权限", IconCls: "icon-Menu", Url: "../../../GPM/AppMenuToEmp.aspx" },
						{ Type: "Node", Id: "UserForRole", ParentId: "MenuRole", Name: "按用户分配权限", IconCls: "icon-User", Url: "../../../GPM/EmpForMenus.aspx" },
						{ Type: "Node", Id: "StationForRole", ParentId: "MenuRole", Name: "按岗位分配权限", IconCls: "icon-Station", Url: "../../../GPM/StationForMenus.aspx" },
						{ Type: "Node", Id: "GroupForRole", ParentId: "MenuRole", Name: "按权限组分配权限", IconCls: "icon-Group", Url: "../../../GPM/EmpGroupForMenus.aspx" }
					  ]
			},
			{ Type: "Node", Id: "BasicSetting2", ParentId: null, Name: "基础设置", Opened: true, IconCls: "icon-tree_folder",
			    Nodes: [
						{ Type: "Node", Id: "HolidaySetting", ParentId: "BasicSetting2", Name: "节假日设置", IconCls: "icon-Config", Url: "../../Comm/Sys/Holiday.aspx" },
						{ Type: "Node", Id: "TableStructure", ParentId: "BasicSetting2", Name: "表结构", IconCls: "icon-Config", Url: "../../Comm/Sys/SystemClass.aspx" },
						{ Type: "Node", Id: "SysVal", ParentId: "BasicSetting2", Name: "系统变量", IconCls: "icon-Config", Url: "javascript:void(0)" },
						{ Type: "Node", Id: "FlowPrevSetting", ParentId: "BasicSetting2", Name: "流程预先审批设置", IconCls: "icon-Config", Url: "../GetTask.aspx" },
						{ Type: "Node", Id: "FuncDown", ParentId: "BasicSetting2", Name: "功能执行", IconCls: "icon-Config", Url: "../../Comm/MethodLink.aspx" }
					  ]
			},
			{ Type: "Node", Id: "SysLog", ParentId: null, Name: "系统日志", Opened: true, IconCls: "icon-tree_folder",
			    Nodes: [
						{ Type: "Node", Id: "LoginLog", ParentId: "SysLog", Name: "登录日志", IconCls: "icon-View", Url: "javascript:void(0)" },
						{ Type: "Node", Id: "ExceptionLog", ParentId: "SysLog", Name: "异常日志", IconCls: "icon-View", Url: "javascript:void(0)" }
					  ]
			}
		  ]
});

var tabsId = null;

//定义功能树对象，便于之后的操作
function FuncTrees(sTabsId) {
    /// <summary>功能树对象操作类</summary>
    /// <param name="sTabsId" type="String">功能树easyui-tabs控件的id</param>
    this.TabsId = tabsId = sTabsId;
    this.tabs = null;

    if (typeof FuncTrees._initialized == "undefined") {
        FuncTrees.prototype.loadTrees = function () {
            /// <summary>加载功能树，其中功能树的定义放在FuncTree.js中的funcTrees数组对象中</summary>
            var tabid = "#" + this.TabsId;

            //循环增加tab标签
            $.each(functrees, function (fcidx, fc) {
                $(tabid).tabs("add", {
                    id: "tab_" + this.Id,
                    title: this.Name,
                    content: "<ul id=\"" + this.Id + "\" class=\"easyui-tree\"></ul>"   // data-options=\"lines:true\"
                });

                if (this.ServiceCount > 0) {
                    for (var i = 0; i < this.Nodes.length; i++) {
                        LoadServiceNode(this.Nodes[i], null, this);
                    }
                }
                else {
                    $.each(this.Nodes, function () {
                        LoadTreeNode(this, null, fc);
                    });

                    SelectFirstTab();
                    OnContextMenu(this);
                    OnDbClick(this);
                }
            });
        }

        FuncTrees.prototype.appendNode = function (sTreeId, sParentNodeId, oNode) {
            /// <summary>增加树节点</summary>
            /// <param name="sTreeId" type="String">功能树easyui-tree控件的id</param>
            /// <param name="sParentNodeId" type="String">待增加树节点的父节点id</param>
            /// <param name="oNode" type="Object">待增加的树节点对象，格式如:{ id: 'aaa', text: '节点1', iconCls: 'icon-new', attributes: {MenuId: "myMenu", Url: "xxx.aspx"} } </param>
            $("#" + sTreeId).tree("append", {
                parent: $("#" + sTreeId + " div[node-id='" + sParentNodeId + "']"),
                data: [oNode]
            });

            $("#" + sTreeId).tree("select", $("#" + sTreeId + " div[node-id='" + oNode.id + "']"));
        }

        FuncTrees.prototype.deleteNode = function (sTreeId, sNodeId) {
            /// <summary>删除树节点</summary>
            /// <param name="sTreeId" type="String">功能树easyui-tree控件的id</param>
            /// <param name="sNodeId" type="String">待删除树节点的id</param>
            $("#" + sTreeId).tree("remove", $("#" + sTreeId + " div[node-id='" + sNodeId + "']"));
        }
    }
}

function LoadServiceNode(oNode, oParentNode, oFuncTree) {
    /// <summary>从WebService返回节点数据，生成节点定义对象</summary>
    /// <param name="oNode" type="Object">节点定义对象</param>
    /// <param name="oParentNode" type="Object">节点定义对象的父级对象</param>
    /// <param name="oFuncTree" type="Object">树对象</param>

    if (oNode.Type == "Service") {
        var params = { method: oNode.ServiceMethod };

        if (oNode.MethodParams) {
            var val;
            var f;
            for (field in oNode.MethodParams) {
                f = field;
                val = oNode.MethodParams[field];
                if (typeof val == "string") {
                    val = ReplaceParams(val, oNode, oFuncTree);
                }

                params[f] = val;
            }
        }

        ajaxService(params, function (data, nd) {
            var re = $.parseJSON(data);

            //将所有获取的数据转换为Node
            var roots = Find(re, nd.ColParentId, nd.RootParentId);

            //此处如果是惰性加载时，非第一次加载，要去除第一次加载时生成的Nodes
            if (oFuncTree.IsLazyLoading && oParentNode.Nodes && oParentNode.Nodes.length > 0) {
                var i = 0;
                while (true) {
                    if (oParentNode.Nodes[i].Type == "Node") {
                        oParentNode.Nodes.splice(i, 1);
                        i--;

                        if (i < 0 && oParentNode.Nodes.length > 0) {
                            i = 0;
                        }
                        continue;
                    }

                    i++;
                    if (i == oParentNode.Nodes.length) {
                        break;
                    }
                }
            }

            if (roots.length > 0) {
                for (var i = 0; i < roots.length; i++) {
                    //此处增加判断，如果当前已经存在要添加的节点，则去除存在的节点，重新加载，考虑到刷新时的逻辑
                    var existedNodes;
                    if (oParentNode == null) {
                        existedNodes = Find(oFuncTree.Nodes, "Id", roots[i][nd.ColId], "Type", "Node");

                        if (existedNodes.length > 0) {
                            oFuncTree.Nodes.splice(IndexInArray(existedNodes[0], oFuncTree.Nodes), 1);
                        }
                    }
                    else {
                        if (!oParentNode.Nodes) {
                            oParentNode.Nodes = [];
                        }
                        else {
                            existedNodes = Find(oParentNode.Nodes, "Id", roots[i][nd.ColId], "Type", "Node");

                            if (existedNodes.length > 0) {
                                oParentNode.Nodes.splice(IndexInArray(existedNodes[0], oParentNode.Nodes), 1);
                            }
                        }
                    }

                    var nextND = {
                        Type: "Node",
                        Id: roots[i][nd.ColId],
                        ParentId: oParentNode == null ? null : oParentNode.Id,
                        Name: roots[i][nd.ColName],
                        IconCls: nd.IconCls,
                        MenuId: nd.MenuId,
                        Url: nd.Url,
                        Opened: nd.Opened,
                        Inherits: nd.Inherits,
                        Target: nd.Target
                    };

                    if (oFuncTree.AttrCols && oFuncTree.AttrCols.length > 0) {
                        $.each(oFuncTree.AttrCols, function (acidx, ac) {
                            nextND[ac] = roots[i][ac];
                        });
                    }

                    define = FindDefine(nd.ColDefine, nd.Defines, roots[i], nd.LazyLoad, nd.InheritForChild, oFuncTree);

                    if (define) {
                        nextND.IconCls = define.IconCls;
                        nextND.MenuId = define.MenuId;
                        nextND.Url = define.Url;
                        nextND.LazyLoad = define.LazyLoad;
                        nextND.InheritForChild = GetNewInheritForChild(define.InheritForChild);
                        nextND.ColDefine = nd.ColDefine;
                        nextND.Opened = define.Opened;
                        nextND.Target = define.Target || nd.Target;

                        if (define.Inherits) {
                            if (typeof nextND.Inherits == "undefined") {
                                nextND.Inherits = [];
                            }

                            $.each(define.Inherits, function (iidx, inh) {
                                if (IndexInArray(inh, nextND.Inherits) == -1) {
                                    nextND.Inherits.push(inh);
                                }
                            });
                        }
                    }
                    else {
                        if (nd.InheritForChild) {
                            nextND.InheritForChild = GetNewInheritForChild(nd.InheritForChild);
                        }
                    }

                    if (oParentNode == null) {
                        oFuncTree.Nodes.splice(IndexInArray(nd, oFuncTree.Nodes), 0, nextND);
                    }
                    else {
                        if (!oParentNode.Nodes) {
                            oParentNode.Nodes = [];
                        }

                        var idx = IndexInArray(nd, oParentNode.Nodes);
                        if (idx > -1) {
                            oParentNode.Nodes.splice(idx, 0, nextND);
                        }
                        else {
                            oParentNode.Nodes.push(nextND);
                        }
                    }

                    //生成子节点
                    LoadServiceSubNode(re, nextND, oParentNode, nd, oFuncTree);
                }
            }

            oFuncTree.ServiceCount--;

            //判断是否完成所有的服务调用，如果完成，则进行全树的生成
            if (oFuncTree.ServiceCount == 0) {
                //排序根节点顺序
                if (oFuncTree.RootASC && !oFuncTree.IsLazyLoading) {
                    oFuncTree.Nodes.sort(function (oNode1, oNode2) {
                        return IndexInArray(oNode1[oFuncTree.RootASC.Field], oFuncTree.RootASC.Index) > IndexInArray(oNode2[oFuncTree.RootASC.Field], oFuncTree.RootASC.Index);
                    });
                }

                var nodes = oFuncTree.Nodes;
                var pNode;
                var pNodeTarget;

                if (oFuncTree.IsLazyLoading) {
                    pNode = $('#' + oFuncTree.Id).tree('getSelected');
                    pNodeTarget = pNode.target;

                    if (!oFuncTree.IsRefresh) {
                        var de = Find(oFuncTree.LazyLoadDefines, "Key", oFuncTree.LazyLoadingDefineKey);
                        if (de.length > 0) {
                            nodes = de[0].Nodes;
                        }
                    }
                    else {
                        nodes = pNode.attributes.Node.Nodes;
                    }
                }

                $.each(nodes, function () {
                    LoadTreeNode(this, pNodeTarget, oFuncTree);
                });

                if (oFuncTree.IsLazyLoading) {
                    //                    var children = $("#" + oFuncTree.Id).tree("getChildren", pNodeTarget);

                    //                    if (children && children.length > 0) {
                    //                        $("#" + oFuncTree.Id).tree("expand", children[0].target);
                    //                    }
                }
                else {
                    SelectFirstTab();
                    OnContextMenu(oFuncTree);
                    OnDbClick(oFuncTree);
                }

                if (typeof oFuncTree.IsLazyLoading != "undefined") {
                    oFuncTree.IsLazyLoading = false;

                    if (typeof oFuncTree.IsRefresh != "undefined") {
                        oFuncTree.IsRefresh = false;
                    }
                }
            }
        }, oNode);
    }
    else {
        if (oNode.Nodes && oNode.Nodes.length > 0) {
            $.each(oNode.Nodes, function () {
                LoadServiceNode(this, oNode, oFuncTree);
            });
        }
    }
}

function OnDbClick(oFuncTree) {
    /// <summary>树节点的双击事件处理逻辑</summary>
    /// <param name="oFuncTree" type="Object">树对象</param>
    $("#" + oFuncTree.Id).tree({
        //animate: true,
        onClick: function (node) {
            checklogin();   //check login info
            $("#" + oFuncTree.Id).tree('select', node.target);
            //            var msg = '';
            //            if (node.attributes && node.attributes.InheritForChild) {
            //                msg += 'InheritForChild:';
            //                $.each(node.attributes.InheritForChild, function () {
            //                    msg += "{From : " + this.From + ", To : " + this.To + "}, ";
            //                });
            //            }
            //            if (node.attributes && node.attributes.Inherits) {
            //                $.each(node.attributes.Inherits, function () {
            //                    msg += "\n" + this + " : " + node.attributes[this] + ", ";
            //                });
            //            }
            //            if (msg.length > 0) {
            //                alert(msg);
            //            }
            //支持将url中的@@+字段格式自动替换成node节点及其属性、或WebUser中同名的属性值，或动态替换JS表达式
            if (node.attributes && node.attributes.Url) {
                //流程树存在流程版本是否升级问题需要单独处理
                if (oFuncTree.Id == "flowTree" && node.attributes.TType == "FLOW") {
                    OpenFlowToCanvas(node, node.id, node.text);
                    return;
                }

                var url = ReplaceParams(node.attributes.Url, node, oFuncTree);
                //$(".mymask").show();
                if (node.attributes.Target == "_blank") {
                    window.open(url);
                }
                else if (node.attributes.Target == "_dialog") {
                    //todo:
                }
                else {
                    addTab(node.id, node.text, url, node.iconCls);
                }
                //setTimeout(DesignerLoaded, 2000);
            }
            else if ($("#" + oFuncTree.Id).tree('isLeaf', node.target) == false) {
                if (node.state == "closed") {
                    $("#" + oFuncTree.Id).tree("expand", node.target);
                }
                else {
                    $("#" + oFuncTree.Id).tree("collapse", node.target);
                }
            }
        },
        onExpand: function (node) {
            if (node.attributes.LazyLoad) {
                $("#" + oFuncTree.Id).tree('select', node.target);
                var children = $("#" + oFuncTree.Id).tree('getChildren', node.target);
                if (children && children.length >= 1) {
                    if (children[0].text == "加载中...") {
                        $("#" + oFuncTree.Id).tree("remove", children[0].target);
                    }

                    if (node.attributes.Inherits) {
                        $.each(node.attributes.Inherits, function () {
                            oFuncTree.Inherits[this] = node.attributes[this];
                        });
                    }

                    var de = Find(oFuncTree.LazyLoadDefines, "Key", node.attributes.ColDefine + "," + node.attributes.DefineValue);
                    if (de.length > 0) {
                        LoadLazyNodes(de[0], node, oFuncTree);
                    }
                }
            }
        }
    });
}

function GetNewInheritForChild(oInheritForChild) {
    /// <summary>克隆一个子节点继承定义对象数组</summary>
    /// <param name="oInheritForChild" type="Object">准备克隆的子节点继承定义对象数组</param>
    if (!oInheritForChild) {
        return oInheritForChild;
    }

    var newIFC = [];

    $.each(oInheritForChild, function () {
        newIFC.push({ From: this.From, To: this.To });
    });

    return newIFC;
}

function LoadLazyNodes(oLazyNode, oNode, oFuncTree) {
    /// <summary>惰性加载指定节点</summary>
    /// <param name="oLazyNode" type="Object">要加载的节点定义对象</param>
    /// <param name="oNode" type="Object">对象对应的easyui-tree的节点node对象</param>
    /// <param name="oFuncTree" type="Object">所属树对象</param>
    if (oLazyNode.Nodes && oLazyNode.Nodes.length > 0) {
        //检索所有节点中，Service节点的数目
        var nodes;
        for (var i = 0; i < oLazyNode.Nodes.length; i++) {
            if (oLazyNode.Nodes[i].Type == "Service") {
                oFuncTree.ServiceCount++;
                continue;
            }

            CalculateServiceCount(oLazyNode.Nodes[i], oFuncTree);
        }

        if (oLazyNode.Key) {
            oFuncTree.IsLazyLoading = true;
            oFuncTree.LazyLoadingDefineKey = oLazyNode.Key;
        }
        else {
            oFuncTree.IsLazyLoading = true;
            oFuncTree.IsRefresh = true;
        }

        if (oFuncTree.ServiceCount > 0) {
            for (var i = 0; i < oLazyNode.Nodes.length; i++) {
                LoadServiceNode(oLazyNode.Nodes[i], oNode.attributes.Node, oFuncTree);
            }
        }
        else {
            $.each(oLazyNode.Nodes, function () {
                LoadTreeNode(this, oNode.target, oFuncTree);
            });

            if (typeof oFuncTree.IsLazyLoading != "undefined") {
                oFuncTree.IsLazyLoading = false;

                if (typeof oFuncTree.IsRefresh != "undefined") {
                    oFuncTree.IsRefresh = false;
                }
            }
        }
    }
}

function RefreshNode(sTreeId, oNodeId) {
    /// <summary>刷新指定节点，此方法设计的只有刷新一级节点才可用此方法</summary>
    /// <param name="sTreeId" type="String">定义树Id</param>
    /// <param name="oNodeId" type="Object">要刷新节点的id</param> 
    var node = $("#" + sTreeId).tree("find", "'" + oNodeId + "'");
    if (!node) {
        $.messager.alert('错误', '未检索到要刷新的节点！', 'error');
        return;
    }

    //首先将要刷新的节点下方的所有节点删除掉
    var children = $("#" + sTreeId).tree("getChildren", node.target);
    if (children.length > 0) {
        $.each(children, function () {
            $("#" + sTreeId).tree("remove", this.target);
        });
    }

    LoadLazyNodes(node.attributes.Node, node, Find(functrees, "Id", sTreeId)[0]);
}

function CalculateServiceCount(oNode, oFuncTree) {
    /// <summary>计算树定义中含有多少Service类型的节点</summary>
    /// <param name="oNode" type="Object">节点定义对象</param>
    /// <param name="oFuncTree" type="Object">树对象</param>
    if (oNode.LazyLoad) {
        return;
    }

    if (oNode.Type == "Service") {
        oFuncTree.ServiceCount++;
        return;
    }

    if (oNode.Nodes && oNode.Nodes.length > 0) {
        for (var i = 0; i < oNode.Nodes.length; i++) {
            CalculateServiceCount(oNode.Nodes[i], oFuncTree);
        }
    }
}

function ReplaceParams(sStr, oTreeNode, oFuncTree) {
    /// <summary>处理替换预定参数，返回新字符串</summary>
    /// <param name="sStr" type="String">要处理的字符串</param>
    /// <param name="oTreeNode" type="Object">easyui-tree的节点对象或定义对象</param>
    /// <param name="oFuncTree" type="Object">所属树定义对象</param>
    //如果sStr是非string类型数据，则不进行以下的处理
    if (typeof sStr != "string") {
        return sStr;
    }

    var newStr = sStr;
    if (newStr.indexOf("@@") != -1) {
        for (field in oTreeNode) {
            if (typeof oTreeNode[field] != "undefined" && newStr.indexOf("@@" + field) != -1) {
                newStr = newStr.replace(new RegExp("@@" + field, "g"), oTreeNode[field]);
            }
        }

        if (oTreeNode.attributes) {
            for (field in oTreeNode.attributes) {
                if (typeof oTreeNode.attributes[field] != "undefined" && newStr.indexOf("@@" + field) != -1) {
                    newStr = newStr.replace(new RegExp("@@" + field, "g"), oTreeNode.attributes[field]);
                }
            }
        }

        for (field in WebUser) {
            if (typeof WebUser[field] != "undefined" && newStr.indexOf("@@WebUser." + field) != -1) {
                newStr = newStr.replace(new RegExp("@@WebUser." + field, "g"), WebUser[field]);
            }
        }

        if (oFuncTree && oFuncTree.Inherits) {
            for (field in oFuncTree.Inherits) {
                if (typeof oFuncTree.Inherits[field] != "undefined" && newStr.indexOf("@@" + field) != -1) {
                    newStr = newStr.replace(new RegExp("@@" + field, "g"), oFuncTree.Inherits[field]);
                }
            }
        }
    }

    if (newStr.indexOf("`") != -1) {
        newStr = ReplaceJS(newStr, "`");
    }

    return newStr;
}

function OnContextMenu(oFuncTree) {
    /// <summary>树的右键菜单处理逻辑</summary>
    /// <param name="oFuncTree" type="Object">树对象</param>
    $("#" + oFuncTree.Id).tree({
        onContextMenu: function (e, node) {
            e.preventDefault();

            $("#" + oFuncTree.Id).tree('select', node.target);

            if (node.attributes && node.attributes.MenuId) {
                $("#" + node.attributes.MenuId).menu('show', {
                    left: e.pageX,
                    top: e.pageY
                });
            }
        }
    });
}

function SelectFirstTab() {
    /// <summary>选择第一个标签页</summary>
    /// <param name="oFuncTree" type="Object">树对象</param>
    $("#" + tabsId).tabs("select", 0);
}

function IndexInArray(oVal, aSortArray) {
    /// <summary>获取元素在数组中的索引序号</summary>
    /// <param name="oVal" type="Object">数组中的元素</param>
    /// <param name="aSortArray" type="Array">数组对象</param>
    /// <return type="Int">返回索引序号</return>
    for (var i = 0; i < aSortArray.length; i++) {
        if (aSortArray[i] == oVal) {
            return i;
        }
    }
    return -1;
}

function LoadServiceSubNode(aServiceNodes, oNode, oParentNode, oServiceNode, oFuncTree) {
    /// <summary>加载节点定义对象的子级对象</summary>
    /// <param name="aServiceNodes" type="Array">WebService返回的节点定义对象集合</param>
    /// <param name="oNode" type="Object">节点定义对象</param>
    /// <param name="oParentNode" type="Object">节点定义对象的父级对象</param>
    /// <param name="oServiceNode" type="Object">初始节点定义对象，此对象含有Service类节点的一些参数</param>
    /// <param name="oFuncTree" type="Object">树定义对象</param>
    var subs = Find(aServiceNodes, oServiceNode.ColParentId, oNode.Id);

    $.each(subs, function (sidx, sub) {
        var subNode = {
            Type: "Node",
            Id: this[oServiceNode.ColId],
            ParentId: oNode.Id,
            Name: this[oServiceNode.ColName],
            Opened: oNode.Opened,
            Inherits: oNode.Inherits,
            Target: oNode.Target
        };

        if (oFuncTree.AttrCols && oFuncTree.AttrCols.length > 0) {
            $.each(oFuncTree.AttrCols, function (acidx, ac) {
                subNode[ac] = sub[ac];
            });
        }

        define = FindDefine(oServiceNode.ColDefine, oServiceNode.Defines, this, oServiceNode.LazyLoad, oServiceNode.InheritForChild, oFuncTree);

        if (define) {
            subNode.IconCls = define.IconCls;
            subNode.MenuId = define.MenuId;
            subNode.Url = define.Url;
            subNode.LazyLoad = define.LazyLoad;
            subNode.InheritForChild = GetNewInheritForChild(define.InheritForChild);
            subNode.ColDefine = oServiceNode.ColDefine;
            subNode.Opened = define.Opened;
            subNode.Target = define.Target || subNode.Target;

            if (define.Inherits) {
                if (typeof subNode.Inherits == "undefined") {
                    subNode.Inherits = [];
                }

                $.each(define.Inherits, function (iidx, inh) {
                    if (IndexInArray(inh, subNode.Inherits) == -1) {
                        subNode.Inherits.push(inh);
                    }
                });
            }
        }
        else {
            if (oServiceNode.InheritForChild) {
                subNode.InheritForChild = GetNewInheritForChild(oServiceNode.InheritForChild);
            }
        }

        if (!oNode.Nodes) {
            oNode.Nodes = [];
        }

        oNode.Nodes.push(subNode);

        //生成子节点
        LoadServiceSubNode(aServiceNodes, subNode, oNode, oServiceNode, oFuncTree);
    });
}

function LoadTreeNode(oNode, oParentNode, oFuncTree) {
    /// <summary>加载树节点</summary>
    /// <param name="oNode" type="Object">节点定义对象</param>
    /// <param name="oParentNode" type="Object">节点定义对象的父级对象</param>
    /// <param name="oFuncTree" type="Object">树定义对象</param>
    //生成附加属性
    if (oNode.Type == "Service") {
        return;
    }

    var attrs = { MenuId: oNode.MenuId, Url: oNode.Url, Target: oNode.Target, LazyLoad: oNode.LazyLoad, ColDefine: oNode.ColDefine, DefineValue: oNode[oNode.ColDefine], InheritForChild: GetNewInheritForChild(oNode.InheritForChild), Inherits: oNode.Inherits, Node: oNode };

    if (oFuncTree.AttrCols) {
        $.each(oFuncTree.AttrCols, function () {
            attrs[this] = oNode[this];
        });
    }

    var id = oNode.Id.split('@')[0];

    if (oFuncTree.IsLazyLoading) {
        if (!oFuncTree.IsRefresh) {
            oNode.Id = id + "@" + Math.random();
        }

        id = oNode.Id;
    }

    $("#" + oFuncTree.Id).tree("append", {
        parent: oParentNode,
        data: [{
            id: id,
            text: oNode.Name,
            iconCls: oNode.IconCls,
            attributes: attrs,
            children: oNode.LazyLoad ? [{ text: "加载中..."}] : []
        }]
    });

    //设置可以继承的属性值
    var node = $("#" + oFuncTree.Id).tree("find", '\'' + oNode.Id + '\'');
    if (node.attributes.InheritForChild) {
        $.each(node.attributes.InheritForChild, function () {
            node.attributes[this.To] = ReplaceParams(this.From, node, oFuncTree);

            if (typeof oFuncTree.Inherits == "undefined") {
                oFuncTree.Inherits = {};
            }

            oFuncTree.Inherits[this.To] = node.attributes[this.To];
        });
    }

    //设置继承属性值
    if (oParentNode && node.attributes.Inherits) {
        $.each(node.attributes.Inherits, function () {
            var pnode = $("#" + oFuncTree.Id).tree("getNode", oParentNode);

            while (pnode) {
                if (pnode.attributes[this]) {
                    node.attributes[this] = pnode.attributes[this];
                    break;
                }

                pnode = $("#" + oFuncTree.Id).tree("getParent", pnode.target);
            }
        });
    }

    if (oNode.Nodes && oNode.Nodes.length > 0) {
        $.each(oNode.Nodes, function () {
            LoadTreeNode(this, node.target, oFuncTree);
        });
    }

    //设置展开状态
    $("#" + oFuncTree.Id).tree(oNode.Opened ? "expand" : "collapse", node.target);
}

function Find(aItems, sField, oValue, sField1, oValue1) {
    /// <summary>查找数组中指定字段值的元素</summary>
    /// <param name="aItems" type="Array">要查找的数组</param>
    /// <param name="sField" type="String">依据字段名称</param>
    /// <param name="oValue" type="Object">字段的值</param>
    /// <param name="sField1" type="String">第2个依据字段名称</param>
    /// <param name="oValue1" type="Object">第2个字段的值</param>
    /// <return>返回集合</return>
    var re = [];

    $.each(aItems, function () {
        if (this[sField] == oValue && (!sField1 || this[sField1] == oValue1)) {
            re.push(this);
        }
    });

    return re;
}

function FindDefine(sColDefine, aDefines, oNode, bIsLazyLoad, aInheritForChild, oFuncTree) {
    /// <summary>查找指定节点的设置规则</summary>
    /// <param name="sColDefine" type="String">规则依据的字段名称</param>
    /// <param name="aDefines" type="Array">当前规则集合</param>
    /// <param name="oNode" type="Object">要查找规则的节点</param>
    /// <param name="bIsLazyLoad" type="Boolean">是否惰性加载</param>
    /// <param name="aInheritForChild" type="Array">给予子节点继续的属性集合</param>
    /// <param name="oFuncTree" type="Object">所属的树定义对象</param>
    var define;

    if (!sColDefine || !aDefines) {
        return define;
    }

    $.each(aDefines, function () {
        if (!this.LazyLoad && bIsLazyLoad) {
            this.LazyLoad = bIsLazyLoad;
        }

        if (this.LazyLoad == true) {
            if (!oFuncTree.LazyLoadDefines) {
                oFuncTree.LazyLoadDefines = [];
            }

            if (Find(oFuncTree.LazyLoadDefines, "Key", sColDefine + "," + this.Value).length == 0) {
                oFuncTree.LazyLoadDefines.push({ Key: sColDefine + "," + this.Value, Nodes: this.Nodes });
            }
        }

        if (typeof this.Value != "undefined") {
            if (oNode[sColDefine] == this.Value) {
                if (!this.Defines) {
                    define = this;
                    return false;
                }

                define = FindDefine(this.ColDefine, this.Defines, oNode, this.LazyLoad, this.InheritForChild, oFuncTree);
            }
            else {
                return true;
            }
        }
        else {
            define = this;
            return false;
        }
    });

    return define;
}

function ReplaceJS(sStr, sFindStr) {
    /// <summary>解析含有JS表达式的字符串，并替换表达式为计算结果，返回新的字符串</summary>
    /// <param name="sStr" type="String">含有JS表达式的字符串</param>
    /// <param name="sFindStr" type="String">JS表达式前后的字符串，以此字符串来划定JS表达式的长度范围</param>
    if (sStr.indexOf(sFindStr) == -1) {
        return sStr;
    }

    var regex = new RegExp(sFindStr, 'g');
    var newstr;
    var i = 0;
    var idxs = [];

    while (regex.exec(sStr) != null) {
        idxs.push(regex.lastIndex - 1); //此处需要注意，lastIndex是从1开始的索引，不是从0
        i++;

        if (i % 2 == 0) {
            newstr += CalculateJS(sStr.substring(idxs[i - 2] + sFindStr.length, idxs[i - 1]));
        }
        else {
            if (i == 1) {
                newstr = sStr.substring(0, regex.lastIndex - 1);
            }
            else {
                newstr += sStr.substring(idxs[i - 2] + sFindStr.length, idxs[i - 1]);
            }
        }
    }

    if (idxs[idxs.length - 1] < sStr.length - 1) {
        newstr += sStr.substring(idxs[idxs.length - 1] + sFindStr.length);
    }

    return newstr;
}

function CalculateJS(sCode) {
    /// <summary>动态运行JS表达式，返回运行结果</summary>
    /// <param name="sCode" type="String">JS表达式字符串</param>
    if (!sCode || sCode.length == 0) {
        return "";
    }

    if (sCode.indexOf("return ") == -1) {
        sCode = "return " + sCode;
    }

    return new Function(sCode)();
}
<template>
   <div>
       <el-card class="box-card">
           <!--搜索条件-->
           <search-form
                   size='mini'
                   labelWidth = '80px'
                   :toolbarData = "toolbarData"
                   :searchForm = "searchForm"
                   :searchHandle="searchHandle"
                   @on-toolbarData-change="ChangeToolbarData">
           </search-form>
           <!--Table表格-->
           <el-table :data="tableData"  border 
                     :header-cell-style="{background:'#f5f7fab5',color:'#777'}"
                     @row-dblclick="RowSelect">
               <el-table-column label="序号" align="center" width="80">
                   <template slot-scope="scope">
                       {{scope.$index+1+pagesize*(currentPage-1)}}
                   </template>
               </el-table-column>
               <el-table-column v-for="item in tableColumnData"  :key="item.field" :label="item.text" :prop="item.field"  :width="item.width"   :show-overflow-tooltip='true'>
               </el-table-column>
           </el-table>
           <!--分页-->
           <el-pagination
           style="float:right;margin: 20px 0px;"
                   @size-change="handleSizeChange"
                   @current-change="handleCurrentChange"
                   :current-page="currentPage"
                   :page-sizes="[5,10,15,20]"
                   :page-size="pagesize"
                   layout="total,sizes, prev, pager, next"
                   :total="total">
           </el-pagination>
           <!--弹出窗-->
           <el-dialog :title="title" :visible.sync="dialogFormVisible" :before-close="handleClose">
                <En ></En>
           </el-dialog>
       </el-card>
   </div>
</template>

<script>
    import SearchForm from './searchForm.vue';
    import {AtParaToJson} from '@/wf/api/Gener.js';
    export default {
        name: "search",
        components:{
            SearchForm,
        },

        data(){
            return{
                params:{},//URL传过来的参数
                tableData: [],
                total: 0, // 总数
                currentPage: 1, // 当前页
                pagesize:5,//一页显示的行数
                tableColumnData:[],

                //页面配置信息
                cfg:{},
                ur:{},//用户配置信息
                foramtFunc:"",//字段格式化函数
                fontSize:13,
                enPK:"No",
                attrs:[],

                //查询条件
                searchForm:[], //查询字段
                searchHandle:[],//执行的方法
                toolbarData:{},//查询字段的值

                //查询类的信息
                ensName:'',
                webuser:{},
                mapBase:{},
                dialogFormVisible:false,//弹出框显示
            }

        },
        created(){
            this.params = this.$store.getters.getData;
            this.ensName=this.params.EnsName;
            this.webuser = this.$store.getters.getWebUser;
            //获取配置信息
            var en = new this.Entity("BP.Sys.EnCfg");
            en.No = this.ensName;
            en.RetrieveFromDBSources();
            this.cfg=en;
            //获取用户注册信息
            en = new this.Entity("BP.Sys.UserRegedit");
            en.MyPK = this.webuser.No + "_" + this.ensName + "_SearchAttrs";
            en.RetrieveFromDBSources();
            this.ur = en;
            this.fontSize = this.cfg.FontSize==null || this.cfg.FontSize==undefined ||this.cfg.FontSize==""?13:this.cfg.FontSize;
            this.foramtFunc = this.cfg.ForamtFunc==null?"":this.cfg.ForamtFunc;
            //查询条件
            this.InitToolBar();
            var searchData = this.SearchData();
            //显示的列
            this.InitTableColumn(searchData);

        },
        methods:{
            InitToolBar(){
                var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_Comm");
                handler.AddJson(this.params);  //增加参数.
                //获得map基本信息.
                this.mapBase = handler.DoMethodReturnJSON("Search_MapBaseInfo");
                this.title = this.mapBase.EnDesc;
                //获得查询信息，包含了查询数据表.
                var data = handler.DoMethodReturnJSON("Search_SearchAttrs");
                var searchFields = this.mapBase.SearchFields;
                searchFields=searchFields==null||searchFields==undefined||searchFields==""?"":searchFields;
                //指定字段查询为空时，默认是关键字查询
                if (searchFields == "") {
                    var keyLabel = this.cfg.KeyLabel;
                    keyLabel=keyLabel == null || keyLabel == undefined || keyLabel == ""?"关键字":keyLabel;
                    this.searchForm.push({type:'Input', label:keyLabel, prop:"SearchKey", width:'140px'});
                    this.toolbarData.SearchKey=this.ur.SearchKey;
                }else{
                    var strs = searchFields.split("@");
                    var fieldV = "";
                    for (var i = 0; i < strs.length; i++) {
                        if (strs[i] == "")
                            continue;

                        var str = strs[i].split("=");
                        if (str.length < 2 || str[0] == "" || str[1] == "")
                            continue;

                        fieldV = this.ur.GetPara(str[1]);
                        if (fieldV == null || fieldV == undefined)
                            fieldV = "";
                        this.searchForm.push({type:'Input', label:str[0], prop:str[1], width:'180px'});
                        this.toolbarData[str[1]]=fieldV;
                    }
                }
                //时间段的查询
                if (this.mapBase.DTSearchWay != "0"){
                    if (this.mapBase.DTSearchWay == "1"){
                        this.searchForm.push({type:'Date', label: this.mapBase.DTSearchLable, prop:"DTFrom", width:'130px'});
                        this.searchForm.push({type:'Date', label: "到", prop:"DTTo", width:'130px'});
                    }else{
                        this.searchForm.push({type:'DateTime', label: this.mapBase.DTSearchLable, prop:"DTFrom", width:'150px'});
                        this.searchForm.push({type:'DateTime', label: "到", prop:"DTTo", width:'150px'});
                    }
                    this.toolbarData.DTFrom = this.ur.DTFrom;
                    this.toolbarData.DTTo = this.ur.DTTo;
                }
                //下拉框的查询
                var attrs = data.Attrs;
                var json = AtParaToJson(this.ur.Vals);
                for (var k = 0; k < attrs.length; k++){
                    var selectVal = json[attrs[k].Field];
                    if (selectVal == undefined || selectVal == "")
                        selectVal = "all";

                    var options = this.InitDDLOperation(data,attrs[k]);
                    //暂时不处理级联关系
                    this.searchForm.push( {type:'Select',label:attrs[k].Name,prop:attrs[k].Field,width:attrs[k].Width+"px",options:options,props:{label:'Name',value:'No'}});
                    this.toolbarData[attrs[k].Field]=selectVal;
                }

                //操作按钮
                this.searchHandle.push({label:'查询',type:'primary',handle:()=>this.Search()});
                //增加自定义按钮
                var btnLab1 = this.cfg.BtnLab1;
                var btnLab2 = this.cfg.BtnLab2;

                handler = new this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
                handler.AddUrlData();
                data = handler.DoMethodReturnString("MyFlow_IsCanStartThisFlow");

                if(data.indexOf('err@') != -1){
                    btnLab1=undefined;
                }
                if (btnLab1 != null && btnLab1 != undefined && btnLab1 != "")
                    this.searchHandle.push({label:btnLab1,type:'primary',handle:()=>''});

                if (btnLab2 != null && btnLab2 != undefined && btnLab2 != "")
                    this.searchHandle.push({label:btnLab2,type:'primary',handle:()=>''});

                if (this.mapBase.IsInsert.toString().toLowerCase() == "true")
                    this.searchHandle.push({label:"新建",type:'primary',handle:()=>this.New()});

                if (this.cfg.IsGroup!=undefined && this.cfg.IsGroup.toString() == "1")
                    this.searchHandle.push({label:"分析",type:'primary',handle:()=>this.ToGroup()});

                if (this.cfg.IsImp!=undefined && this.cfg.IsImp.toString().toLowerCase() == "true")
                    this.searchHandle.push({label:"导入",type:'primary',handle:()=>this.Imp()});

                if (this.webuser.No == "admin")
                    this.searchHandle.push({label:"设置",type:'primary',handle:()=>this.Setting()});
            },
            InitTableColumn(searchData){
               this.attrs = searchData.Attrs;
                //var dtMs = searchData.dtM;
                var sysMapData = searchData["Sys_MapData"][0];
                sysMapData = new this.Entity("BP.Sys.MapData", sysMapData); //把他转化成entity.
                this.enPK = sysMapData.GetPara("PK");
                if (this.attrs == undefined) {
                    this.$message.error('没有取得属性.');
                    return;
                }
                this.attrs.forEach(attr =>{
                    if (attr.UIVisible == 0
                        || attr.KeyOfEn == "OID"
                        || attr.KeyOfEn == "WorkID"
                        || attr.KeyOfEn == "NodeID"
                        || attr.KeyOfEn == "MyNum"
                        || attr.KeyOfEn == "MyPK")
                        return true;
                    var width = attr.Width;
                    if (width < 60) width = 60;
                    if (attr.KeyOfEn == "Title") width = 230;
                    if (attr.UIContralType == 1) {
                        if (width == null || width == "" || width == undefined)
                            width = 180;
                        this.tableColumnData.push({
                            field: attr.KeyOfEn+"Text",
                            text: attr.Name,
                            fixed: false,
                            width: width+"px",

                        });
                        return true;
                    }
                    if (attr.UIContralType == 2) {
                        this.tableColumnData.push({
                            field: attr.KeyOfEn,
                            text: attr.Name,
                            width: width+"px"
                        });
                        return true;
                    }
                    if (width == null || width == "" || width == undefined)
                        width = 100;

                    this.tableColumnData.push({
                        field: attr.KeyOfEn,
                        text: attr.Name,
                        width: width+"px"
                    });

                });


            },
            Search(){
                console.log("进入Search") ;
                var ur = new this.Entity("BP.Sys.UserRegedit");
                ur.MyPK = this.webuser.No + "_" + this.ensName + "_SearchAttrs";
                ur.FK_Emp = this.webuser.No;
                ur.CfgKey = "SearchAttrs";
                ur.FK_MapData = this.ensName;
                ur.SearchKey = this.toolbarData.SearchKey; //关键字
                ur.DTFrom = this.toolbarData.DTFrom; //时间从
                ur.DTTo = this.toolbarData.DTTo;//时间到
                var str = "";
                this.searchForm.forEach(item=>{
                    if(item.type=="Input" && item.prop!="SearchKey")
                        ur.SetPara(item.prop, this.toolbarData[item.prop]);
                    if(item.type=="Select"){
                        str += "@" + item.prop + "=" + this.toolbarData[item.prop];
                    }

                });
                ur.Vals = str;
                ur.Save();
                this.ur = ur;
                this.currentPage=1;
                this.SearchData();

            },
            SearchData(){
                var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_Comm");
                handler.AddJson(this.params);
                handler.AddPara("PageIdx", this.currentPage);
                handler.AddPara("PageSize", this.pagesize);
                var data = handler.DoMethodReturnString("Search_SearchIt");
                if (data.indexOf('err@') == 0) {
                    this.$message.error(data);
                    console.log(data);
                    return;
                }

                data = JSON.parse(data);
                var en = new this.Entity("BP.Sys.UserRegedit");
                en.MyPK = this.webuser.No + "_" + this.ensName + "_SearchAttrs";
                en.RetrieveFromDBSources();
                this.ur = en;
                var count = this.ur.GetPara("RecCount");
                count= count==null||count==undefined || count==""?0:parseInt(count);
                this.total =count;
                //初始化数据
                this.tableData=data["DT"];
                return data;
            },
            RowSelect(row){//选中一行
                var paras=this.params;
                paras[this.enPK]=row[this.enPK];
                this.attrs.forEach(attr=>{
                    if (attr.UIContralType == 1)
                        paras[attr.KeyOfEn]=row[attr.KeyOfEn];
                });
                this.OpenEn(row[this.enPK], paras,row);
            },
            New(){ //新建
                this.OpenEn("",this.params);
            },
            Imp(){//导入

            },
            Setting(){
                this.dialogFormVisible = true;

            },
            handleSizeChange(val) {
                this.pagesize = val;
                this.currentPage=1;
                this.SearchData();
                console.log(`每页 ${val} 条`);
            },
            handleCurrentChange(val) {
                this.currentPage = val;
                this.SearchData();
                console.log(`当前页: ${val}`);
            },
            ChangeToolbarData(val){
                this.toolbarData = val;
            },
            InitDDLOperation(frmData, mapAttr){
                var ens = frmData[mapAttr.Field];
                if (ens == null) {
                    ens = [{ 'IntKey': 0, 'Lab': '否' }, { 'IntKey': 1, 'Lab': '是' }];
                }
                var arry=[];
                arry.push({'No':'all','Name':'全部'});
                ens.forEach(en=>{
                    if(en.No==undefined)
                        arry.push({'No':en.IntKey+"",'Name':en.Lab});
                    else
                        arry.push({'No':en.No,'Name':en.Name});
                });
                return arry;
            },
            OpenEn(pkval,paras,row){
                var url = this.cfg.UrlExt;
                var urlOpenType = this.cfg.SearchUrlOpenType;
                paras.PKVal = pkval;
                if (urlOpenType == undefined || urlOpenType == 0 || urlOpenType == 1)
                    paras.EnName = this.mapBase.EnName;
                if (urlOpenType == 2||urlOpenType == 3)
                    paras.FK_MapData=this.ensName;
                if (urlOpenType == 9) {
                    if (url.indexOf('FrmID') != -1){
                        paras.WorkID=pkval;
                        paras.OID=pkval;
                    }else{
                        paras.EnName = this.mapBase.EnName ;
                    }
                    if(url.indexOf("@")!=-1 && url.indexOf("?")!=-1 && row!=undefined){
                        url=url.substr(url.indexOf("?")+1);
                        var items=url.match(new RegExp("[\?\&][^\?\&]+=[^\?\&]+", "g"));// eslint-disable-line
                        var key="";
                        items.forEach(item=>{
                            var strs=item.split("=");
                            key=strs[0];
                            if(key.indexOf("@")!=-1){
                                key = key.substr(1);
                                paras[key]=row[key];
                            }else{
                                paras[key]=strs[1];
                            }

                        })
                    }

                }
                //弹出显示页面
            }
        }
    }

</script>

<style scoped>

</style>
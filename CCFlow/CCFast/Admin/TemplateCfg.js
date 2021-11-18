
//template地址.
var host = "http://template.ccbpm.cn/Handler2021.ashx";

/**
 * 流程：功能类别字典：行政类 行业 登记 报名 订单.
 * */
function Sort1s() {

    var src = host + "?DoType=BP.FrmTemplate.Sort1s";
    var ens = DBAccess.RunDBSrc(src);
    return ens;
}


/**
 * 流程： 行业字典：通用办公类 财务类 人力资源类 客户关系管理类  教育培训 医疗健康 建筑施工项目类 商业连锁类 公益
 * */
function SortFuncs() {
    var src = host + "?DoType=BP.FrmTemplate.SortFuncs";
    var ens = DBAccess.RunDBSrc(src);
    return ens;
}

/**
 * 应用字典：应用类别， 通用办公 ,协同 ， 其他. 
 * */
function AppSort()
{
    var src = host + "?DoType=AppSorts";
    var ens = DBAccess.RunDBSrc(src);
    return ens;
}
/**
 * 说明：
 * 1. 该配置文件用于配置 组织组织结构维护.
 * 2. 使用实体类进行管理维护.
 * */

/* 新建部门部门编号模式. 0 = 手工输入自己编号.  1=自动生成.  由实体类的beforeInser 生成.  */
var NewDeptNoModel = 0;

// 人员编号格式.
var RegUserIDMode = 1;

//部门类名.
var deptEnsName = "BP.Port.Depts";
var deptEnName = "BP.Port.Dept";

/*  新建人员编号模式. 0=手工输入自己编号.  1=自动生成，由实体类的 beforeInser 生成. */
var NewEmpNoModel = 0;

//操作员类名.
var empEnsName = "BP.Port.Emps";
var empEnName = "BP.Port.Emp";


//岗位
var ensOfStations = "BP.Cloud.StationExts";

//岗位类型.
var ensOfStationTypes = "BP.Cloud.StationTypeExts";






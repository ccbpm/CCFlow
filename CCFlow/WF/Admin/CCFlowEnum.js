
if (typeof RunModel == "undefined") {
    var RunModel = {}
        // 普通
        RunModel.Ordinary = 0,
        // 合流
        RunModel.HL = 1,
        // 分流
        RunModel.FL = 2,
        // 分合流
        RunModel.FHL = 3,
        // 子线程
        RunModel.SubThread = 4
    }

    function getRunModelName(keyValue) {
        switch (keyValue) {
            case 0:
                return "Ordinary";
            case 1:
                return "HL";
            case 2:
                return "FL";
            case 3:
                return "FHL";
            case 4:
                return "SubThread";
            default:
                return "Ordinary";
        }
    }

//投递方式
if (typeof DeliveryWay == "undefined") {
    var DeliveryWay = {}
        // 按岗位(以部门为纬度)
        DeliveryWay.ByStation = 0,
        // 按部门
        DeliveryWay.ByDept = 1,
        // 按SQL
        DeliveryWay.BySQL = 2,
        // 按本节点绑定的人员
        DeliveryWay.ByBindEmp = 3,
        // 由上一步发送人选择
        DeliveryWay.BySelected = 4,
        // 按表单选择人员
        DeliveryWay.ByPreviousNodeFormEmpsField = 5,
        // 与上一节点的人员相同
        DeliveryWay.ByPreviousNodeEmp = 6,
        // 与开始节点的人员相同
        DeliveryWay.ByStarter = 7,
        // 与指定节点的人员相同
        DeliveryWay.BySpecNodeEmp = 8,
        // 按岗位与部门交集计算
        DeliveryWay.ByDeptAndStation = 9,
        // 按岗位计算(以部门集合为纬度)
        DeliveryWay.ByStationAndEmpDept = 10,
        // 按指定节点的人员或者指定字段作为人员的岗位计算
        DeliveryWay.BySpecNodeEmpStation = 11,
        // 按SQL确定子线程接受人与数据源
        DeliveryWay.BySQLAsSubThreadEmpsAndData = 12,
        // 按明细表确定子线程接受人
        DeliveryWay.ByDtlAsSubThreadEmps = 13,
        // 仅按岗位计算
        DeliveryWay.ByStationOnly = 14,
        // FEE计算
        DeliveryWay.ByFEE = 15,
        // 按绑定部门计算,该部门一人处理标识该工作结束(子线程)
        DeliveryWay.BySetDeptAsSubthread = 16,
        // 按SQL模版计算
        DeliveryWay.BySQLTemplate = 17,
        // 从人员到人员
        DeliveryWay.ByFromEmpToEmp = 18,
        // 按照ccflow的BPM模式处理
        DeliveryWay.ByCCFlowBPM = 100

}

   
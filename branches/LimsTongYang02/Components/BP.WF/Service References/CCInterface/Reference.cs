﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BP.WF.CCInterface {
    using System.Data;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CCInterface.PortalInterfaceSoap")]
    public interface PortalInterfaceSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendToWebServices", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SendToWebServices(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo, string tag);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendWhen", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SendWhen(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/FlowOverBefore", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool FlowOverBefore(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendToDingDing", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SendToDingDing(string mypk, string sender, string sendToEmpNo, string tel, string msgInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendToWeiXin", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SendToWeiXin(string mypk, string sender, string sendToEmpNo, string tel, string msgInfo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendToEmail", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SendToEmail(string mypk, string sender, string sendToEmpNo, string email, string title, string maildoc);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendToCCIM", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool SendToCCIM(string mypk, string userNo, string msg, string sourceUserNo, string tag);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Print", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        void Print(string billFilePath);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/WriteUserSID", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        bool WriteUserSID(string miyue, string userNo, string sid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/CheckUserNoPassWord", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        int CheckUserNoPassWord(string userNo, string password);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDept", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetDept(string deptNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDepts", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetDepts();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDeptsByParentNo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetDeptsByParentNo(string parentDeptNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStations", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetStations();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetStation", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetStation(string stationNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetEmps", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetEmps();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetEmpsByDeptNo", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetEmpsByDeptNo(string deptNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetEmp", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetEmp(string no);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDeptEmp", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetDeptEmp();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetEmpHisDepts", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetEmpHisDepts(string empNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetEmpHisStations", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetEmpHisStations(string empNo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GetDeptEmpStations", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GetDeptEmpStations();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GenerEmpsByStations", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GenerEmpsByStations(string stationNos);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GenerEmpsByDepts", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GenerEmpsByDepts(string deptNos);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/GenerEmpsBySpecDeptAndStats", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        System.Data.DataTable GenerEmpsBySpecDeptAndStats(string deptNo, string stations);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/SendSuccess", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string SendSuccess(string flowNo, int nodeID, long workid, string userNo, string userName);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface PortalInterfaceSoapChannel : BP.WF.CCInterface.PortalInterfaceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PortalInterfaceSoapClient : System.ServiceModel.ClientBase<BP.WF.CCInterface.PortalInterfaceSoap>, BP.WF.CCInterface.PortalInterfaceSoap {
        
        public PortalInterfaceSoapClient() {
        }
        
        public PortalInterfaceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PortalInterfaceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PortalInterfaceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PortalInterfaceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public bool SendToWebServices(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo, string tag) {
            return base.Channel.SendToWebServices(msgPK, sender, sendToEmpNo, tel, msgInfo, tag);
        }
        
        public bool SendWhen(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo) {
            return base.Channel.SendWhen(msgPK, sender, sendToEmpNo, tel, msgInfo);
        }
        
        public bool FlowOverBefore(string msgPK, string sender, string sendToEmpNo, string tel, string msgInfo) {
            return base.Channel.FlowOverBefore(msgPK, sender, sendToEmpNo, tel, msgInfo);
        }
        
        public bool SendToDingDing(string mypk, string sender, string sendToEmpNo, string tel, string msgInfo) {
            return base.Channel.SendToDingDing(mypk, sender, sendToEmpNo, tel, msgInfo);
        }
        
        public bool SendToWeiXin(string mypk, string sender, string sendToEmpNo, string tel, string msgInfo) {
            return base.Channel.SendToWeiXin(mypk, sender, sendToEmpNo, tel, msgInfo);
        }
        
        public bool SendToEmail(string mypk, string sender, string sendToEmpNo, string email, string title, string maildoc) {
            return base.Channel.SendToEmail(mypk, sender, sendToEmpNo, email, title, maildoc);
        }
        
        public bool SendToCCIM(string mypk, string userNo, string msg, string sourceUserNo, string tag) {
            return base.Channel.SendToCCIM(mypk, userNo, msg, sourceUserNo, tag);
        }
        
        public void Print(string billFilePath) {
            base.Channel.Print(billFilePath);
        }
        
        public bool WriteUserSID(string miyue, string userNo, string sid) {
            return base.Channel.WriteUserSID(miyue, userNo, sid);
        }
        
        public int CheckUserNoPassWord(string userNo, string password) {
            return base.Channel.CheckUserNoPassWord(userNo, password);
        }
        
        public System.Data.DataTable GetDept(string deptNo) {
            return base.Channel.GetDept(deptNo);
        }
        
        public System.Data.DataTable GetDepts() {
            return base.Channel.GetDepts();
        }
        
        public System.Data.DataTable GetDeptsByParentNo(string parentDeptNo) {
            return base.Channel.GetDeptsByParentNo(parentDeptNo);
        }
        
        public System.Data.DataTable GetStations() {
            return base.Channel.GetStations();
        }
        
        public System.Data.DataTable GetStation(string stationNo) {
            return base.Channel.GetStation(stationNo);
        }
        
        public System.Data.DataTable GetEmps() {
            return base.Channel.GetEmps();
        }
        
        public System.Data.DataTable GetEmpsByDeptNo(string deptNo) {
            return base.Channel.GetEmpsByDeptNo(deptNo);
        }
        
        public System.Data.DataTable GetEmp(string no) {
            return base.Channel.GetEmp(no);
        }
        
        public System.Data.DataTable GetDeptEmp() {
            return base.Channel.GetDeptEmp();
        }
        
        public System.Data.DataTable GetEmpHisDepts(string empNo) {
            return base.Channel.GetEmpHisDepts(empNo);
        }
        
        public System.Data.DataTable GetEmpHisStations(string empNo) {
            return base.Channel.GetEmpHisStations(empNo);
        }
        
        public System.Data.DataTable GetDeptEmpStations() {
            return base.Channel.GetDeptEmpStations();
        }
        
        public System.Data.DataTable GenerEmpsByStations(string stationNos) {
            return base.Channel.GenerEmpsByStations(stationNos);
        }
        
        public System.Data.DataTable GenerEmpsByDepts(string deptNos) {
            return base.Channel.GenerEmpsByDepts(deptNos);
        }
        
        public System.Data.DataTable GenerEmpsBySpecDeptAndStats(string deptNo, string stations) {
            return base.Channel.GenerEmpsBySpecDeptAndStats(deptNo, stations);
        }
        
        public string SendSuccess(string flowNo, int nodeID, long workid, string userNo, string userName) {
            return base.Channel.SendSuccess(flowNo, nodeID, workid, userNo, userName);
        }
    }
}

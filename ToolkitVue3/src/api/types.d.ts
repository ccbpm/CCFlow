export interface CCFlowResponse<T> {
    data: T;
    code: Number;
    msg: string;
}
//流程实例数据
export interface GenerWorkFlow {
    ADT: string,
    AtPara: string|null,
    Auther: null|string,
    BillNo: string,
    DeptName: string,
    Domain: string,
    FID: number,
    FK_Dept: string,
    FK_Emp: string,
    FK_Flow: string,
    FK_FlowSort: string,
    FK_Node: number,
    FlowIdx: string,
    FlowName: string
    FlowNote: string|null,
    FlowSortIdx: number,
    GuestName: string|null,
    GuestNo: string|null,
    IsRead: number,
    ListType: number,
    NodeName: string,
    OrgNo: string|null,
    PFlowNo: string|null,
    PRI: number,
    PWorkID: number,
    PressTimes: number,
    RDT: string,
    SDT: string|null,
    SDTOfNode: string,
    Sender: string|null,
    Starter: string
    StarterName: string
    SysType: string|null,
    TaskSta: number,
    Title: string
    TodoEmpDeptNo: string
    TodoEmps: string
    TodoEmpsNum: number,
    TodoSta: number,
    WFState: number,
    WorkID: number,
}
//流程发起数据
export interface flow {
    No: string,
    Name: string,
    IsBatchStart: number,
    FK_FlowSort: string,
    FK_FlowSortText: string,
    Domain: string,
    IsStartInMobile: number,
    Idx: string,
    WorkModel: number,
}
//在途数据
export interface RunningFlow {
    CurrNode: number,
    DeptName: string,
    FID: number,
    FK_Flow: string,
    FK_Node: number,
    FlowName: string,
    NodeName: string,
    RDT: string,
    RunType: number,
    StarterName: string,
    Title: string,
    TodoEmps: string,
    WFState: number,
    WhoExeIt: number,
    WorkID: number,
}
//草稿数据
export interface Draft {
    AtPara: string|null,
    FID: number,
    FK_Flow: string,
    FK_Node: number,
    FlowName: string,
    FlowNote: string|null,
    RDT: string,
    Title: string,
    WorkID: number,
}
//已完成和近期工作数据
export interface RecentWork extends GenerWorkFlow{
    Emps: string,
    FK_NY: string,
    GUID: string|null,
    HungupTime: string|null,
    LostTimeHH: number,
    PEmp: string|null,
    PFID: number,
    PNodeID: number,
    PrjName: string|null,
    PrjNo: string|null,
    SDTOfFlow: string|null,
    SDTOfFlowWarning: string|null,
    SKeyWords: null
    SendDT: string,
    TSpan: number,
    WFSta: number,
    WeekNum: number,
}
//查询数据
export interface SearchWork{
    count: {CC:number}[],
    gwls : RecentWork[],
}
//抄送数据
export interface CCWork {
    Doc: string,
    FID: number,
    FK_Flow: string,
    FK_Node: number,
    FlowName: string,
    MyPK: string,
    NodeName: string,
    RDT: string,
    Rec: string,
    Sta: number,
    Title: string,
    WFSta: number,
    WorkID: number,
}
//批处理数据
export interface BatchWork {
    BatchRole: number,
    FlowName: string,
    NUM: number,
    Name: string,
    NodeID: number,
}




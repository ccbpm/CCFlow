import request from '@/utils/request'
import type { BatchWork, CCFlowResponse, CCWork, Draft, GenerWorkFlow, RecentWork, RunningFlow, SearchWork, flow, } from './types'

//发起流程
export function DB_Start(domain?: any) {
  return request.get<any, flow[]>('/WF/API/DB_Start', {
    params: {
      domain
    }
  })
}

//待办
export function DB_Todolist(domain?: any) {
  return request.get<any, GenerWorkFlow[]>('/WF/API/DB_Todolist', {
    params: {
      domain
    }
  })
}

//在途
export function DB_Runing(domain?: any) {
  return request.get<any, RunningFlow[]>('/WF/API/DB_Runing', {
    params: {
      domain
    }
  })
}

/**
 * 获取草稿列表
 * @param domain 流程的域
 * @returns 草稿数组
 */
export function DB_Draft(domain?: any) {
  return request.get<any, Draft[]>('/WF/API/DB_Draft', {
    params: {
      domain
    }
  })
}

/**
 * 删除草稿
 * @param workIDs  草稿实例编号,多个用逗号隔开
 * @returns 成功或者失败信息
 */
export function Flow_DeleteDraft(workIDs: string) {
  return request.get<any, CCFlowResponse<String>>('/WF/API/Flow_DeleteDraft', {
    params: {
      workIDs,
    }
  })
}

/**
 * 撤销发送
 * @param workIDs 要执行的实例,多个实例用逗号分开比如：1001,1002,1003
 * @returns 失败返回失败信息
 */
export function Flow_DoUnSend(workIDs: string) {
  return request.get<any, CCFlowResponse<String>>('/WF/API/Flow_DoUnSend', {
    params: {
      workIDs
    }
  })
}

/**
 * 催办
 * @param workIDs 催办的实例
 * @param msg 催办信息
 * @returns
 */
export function Flow_DoPress(workIDs: string, msg: string) {
  return request.get<any, CCFlowResponse<String>>('/WF/API/Flow_DoPress', {
    params: {
      workIDs,
      msg
    }
  })
}



/**
 * 获取批处理
 * @returns 批处理节点
 */
export function Batch_Init() {
  return request.get<any, BatchWork[]>('/WF/API/Batch_Init', {
    params: {}
  });
}

/**
 * 获取抄送
 * @returns 抄送节点
 * @param  domain 域 非必需
 * @param flowNo 流程编号 非必需
 */
export function DB_CCList(domain = '', flowNo = ''){
  return request.get<any,CCWork[]>('/WF/API/DB_CCList',{
    params:{
      domain,
      flowNo,
    }
  })
}


/**
 * 查询数据
 * @param key
 * @param dtFrom
 * @param dtTo
 * @param scop
 * @param pageIdx
 * @returns
 */
export function Search_Init(key: string, dtFrom: string, dtTo: string, scop: string, pageIdx: number) {
  return request.get<any, SearchWork>('/WF/API/Search_Init', {
    params: {
      key,
      dtFrom,
      dtTo,
      scop,
      pageIdx,
    }
  })
}
/**
 * 近期工作
 * @returns 
 */
export function Flow_RecentWorkInit() {
  return request.get<any, RecentWork[]>('/WF/API/Flow_RecentWorkInit',{params:{}})
}


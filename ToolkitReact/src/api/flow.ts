import request from "../utils/request";

//发起流程
export function DB_Start(domain: unknown) {
  return request.get<unknown, null>("/WF/API/DB_Start", {
    params: {
      domain,
    },
  });
}

//待办
export function DB_Todolist(domain: unknown) {
  return request.get<unknown, null>("/WF/API/DB_Todolist", {
    params: {
      domain,
    },
  });
}

//在途
export function DB_Runing(domain: unknown) {
  return request.get<unknown, null>("/WF/API/DB_Runing", {
    params: {
      domain,
    },
  });
}

/**
 * 获取草稿列表
 * @param domain 流程的域
 * @returns 草稿数组
 */
export function DB_Draft(domain: unknown) {
  return request.get<unknown, null>("/WF/API/DB_Draft", {
    params: {
      domain,
    },
  });
}

/**
 * 撤销发送
 * @param workIDs 要执行的实例,多个实例用逗号分开比如：1001,1002,1003
 * @returns 失败返回失败信息
 */
export function Flow_DoUnSend(workIDs: string) {
  return request.get<unknown, null>("/WF/API/Flow_DoUnSend", {
    params: {
      workIDs,
    },
  });
}

/**
 * 催办
 * @param workIDs 催办的实例
 * @param msg 催办信息
 * @returns
 */
export function Flow_DoPress(workIDs: string, msg: string) {
  return request.get<unknown, null>("/WF/API/Flow_DoPress", {
    params: {
      workIDs,
      msg,
    },
  });
}

/**
 * 删除草稿
 * @param workIDs  草稿实例编号
 * @returns 成功或者失败信息
 */
export function Flow_DeleteDraft(workIDs: string) {
  return request.get<unknown, null>("/WF/API/Flow_DeleteDraft", {
    params: {
      workIDs,
    },
  });
}

/**
 * 获取批处理
 * @returns 批处理节点
 */
export function Batch_Init(domain: unknown) {
  return request.get<unknown, null>("/WF/API/Batch_Init", {
    params: {
      domain,
    },
  });
}

/**
 * 获取抄送
 * @returns 抄送节点
 * @param  domain
 * @param flowNo
 */
export function DB_CCList(domain: string, flowNo: string) {
  return request.get<unknown, null>("/WF/API/DB_CCList", {
    params: {
      domain,
      flowNo,
    },
  });
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
export function Search_Init(
  key: string,
  dtFrom: string,
  dtTo: string,
  scop: string,
  pageIdx: number
) {
  return request.get<unknown, null>("/WF/API/Search_Init", {
    params: {
      key,
      dtFrom,
      dtTo,
      scop,
      pageIdx,
    },
  });
}
/**
 * 近期工作
 * @returns
 */
export function Flow_RecentWorkInit() {
  return request.get<unknown, null>("/WF/API/Flow_RecentWorkInit", { params: {} });
}

import type { User } from '../store/userStore'
import request from '../utils/request'

const { VITE_GLOB_PrivateKey } = import.meta.env

export function Port_Login(userNo: string, orgNo: string) {
  return request.get<unknown, User>('/WF/API/Port_Login', {
    params: {
      privateKey: VITE_GLOB_PrivateKey as string,
      userNo,
      orgNo
    }
  })
}

export function Port_Logout(userNo: string) {
  return request.get<unknown, null>('/WF/API/Port_LoginOut', {
    params: {
      userNo
    }
  })
}


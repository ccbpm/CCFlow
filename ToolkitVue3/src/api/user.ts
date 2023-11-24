import type { User } from '@/stores/user'
import request from '@/utils/request'

const { VITE_GLOB_PrivateKey } = import.meta.env

export function Port_Login(userNo: string, orgNo: string) {
  return request.get<any, User>('/WF/API/Port_Login', {
    params: {
      privateKey: VITE_GLOB_PrivateKey,
      userNo,
      orgNo
    }
  })
}

export function Port_Logout(userNo: string) {
  return request.get<any, null>('/WF/API/Port_LoginOut', {
    params: {
      userNo
    }
  })
}


import { Modal } from 'ant-design-vue'
import { defineStore } from 'pinia'
import { h } from 'vue'
import HttpHandler from '@/utils/gener/HttpHandler'
import { router } from '@/router'
import { Port_Login, Port_Logout } from '@/api/user'

export interface TokenInfo {
  Token: string
  UserNo: string
  OrgNo: string
}

export interface User {
  No: string
  Name: string
  OrgNo: string
  OrgName: string
  FK_Dept: string
  FK_DeptName: string
  SysLang: string
  CCBPMRunModel: number
  IsAdmin: boolean | number
  Token: string
  homePath?: string
  avatar?: string
}

export interface LoginParams {
  username: string
  password: string
  OrgNo?: string //SAAS版 登录需要组织编号
  verifyCode?: string
}

export const useUserStore = defineStore({
  id: 'app-user',
  state: (): { webUser: User } => ({
    webUser: {
      No: '',
      Name: '',
      OrgNo: '',
      OrgName: '',
      FK_Dept: '',
      FK_DeptName: '',
      SysLang: '',
      CCBPMRunModel: -1,
      IsAdmin: 0,
      Token: '',
      homePath: '/',
      avatar: ''
    }
  }),
  getters: {
    getToken(): string {
      return this.getWebUser.Token || ''
    },
    getWebUser(): User {
      if(!this.webUser.No) {
        try {
          this.webUser = JSON.parse(localStorage.getItem('userInfo') || '{}');
        } catch(e) { /* empty */ }
      }
      return this.webUser || ''
    }
  },
  actions: {
    setUserInfo(user: User | null) {
      if (!user) {
        this.webUser = {
          No: '',
          Name: '',
          OrgNo: '',
          OrgName: '',
          FK_Dept: '',
          FK_DeptName: '',
          SysLang: '',
          CCBPMRunModel: -1,
          IsAdmin: 0,
          Token: '',
          homePath: '/',
          avatar: ''
        }
        localStorage.removeItem('userInfo')
        return
      }
      this.webUser = user
      localStorage.setItem('userInfo', JSON.stringify(user))
    },
    setToken(token: string) {
      this.webUser.Token = token // for null or undefined value
      localStorage.setItem('Token', token)
    },
    async fetchUserInfo() {
      const userHandler = new HttpHandler('')
      return await userHandler.CustomRequest<User>({
        DoType: 'WebUser_Init',
        Token: this.getToken,
        t: Date.now()
      })
    },
    async login(
      params: LoginParams & {
        goHome?: boolean
        type?: string
      }
    ) {
      const data = await Port_Login(params.username, params.OrgNo || '')

      if (!data.Token) {
        Modal.error({
          title: () => h('span', '错误'),
          content: () => h('span', '未获取到登录凭证')
        })
        return null
      }
      this.setUserInfo(data)
      router.replace('/')
    },
    async logout() {
      if (this.getToken) {
        try {
          await Port_Logout(this.webUser.No);
        } catch {
          console.log('注销Token失败')
        }
      }
      this.setToken('')
      this.setUserInfo(null)
      router.push('/login')
    },
    confirmLoginOut() {
      Modal.confirm({
        title: () => h('span', '提示'),
        content: () => h('span', '确定要退出登录吗？'),
        cancelText: '取消',
        okText: '确定',
        onOk: async () => {
          await this.logout()
        }
      })
    },
    resetState() {
      this.setUserInfo(null);
    },
  }
})

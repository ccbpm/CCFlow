
import { makeObservable, observable, action } from "mobx";
import { makePersistable } from "mobx-persist-store";
import { Port_Login } from "../api/user";

export interface TokenInfo {
  Token: string;
  UserNo: string;
  OrgNo: string;
}

export interface User {
  No: string;
  Name: string;
  OrgNo: string;
  OrgName: string;
  FK_Dept: string;
  FK_DeptName: string;
  SysLang: string;
  CCBPMRunModel: number;
  IsAdmin: boolean | number;
  Token: string;
  homePath?: string;
  avatar?: string;
}

export interface LoginParams {
  username: string;
  password: string;
  OrgNo?: string; //SAAS版 登录需要组织编号
  verifyCode?: string;
}


class UserStore {
  userInfo: User | null = null;
  token = "";

  constructor() {
    makeObservable(this, {
      userInfo: observable,
      token: observable,
      setUserInfo: action,
      setToken: action,
    });

    makePersistable(this, {
      name: "user-store",
      properties: ["userInfo", "token"],
      storage: localStorage
    });
  }

  setUserInfo(userInfo: User | null) {
    this.userInfo = userInfo;
  }

  setToken(token: string) {
    this.token = token;
  }

  async login(params: LoginParams) {
    const data = await Port_Login(params.username, "");
    this.setToken(data.Token);
    this.setUserInfo(data);
    console.log(this);
  }
}

const userStore = new UserStore();

export { userStore };

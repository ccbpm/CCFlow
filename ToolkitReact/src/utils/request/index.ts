/* eslint-disable @typescript-eslint/no-unsafe-assignment */
import axios from "axios";
import { userStore } from "../../store/userStore";

const { VITE_GLOB_API_URL } = import.meta.env;
const service = axios.create({
  baseURL: import.meta.env.MODE === "development" ? "/api" : VITE_GLOB_API_URL,
  withCredentials: true,
});

const isJson = (str: string) => {
  try {
    JSON.parse(str);
    return true;
  } catch (e) {
    return false;
  }
};

// 请求拦截
service.interceptors.request.use(
  (config) => {
    const token = userStore.token;
    let url = config.url;
    if (config.params) {
      if (!config.params.Token) config.params.Token = token;
      url += "?";
      const keys = Object.keys(config.params);
      for (const key of keys) {
        try {
          url += `${key}=${decodeURIComponent(config.params[key])}&`;
        } catch (e) {
          throw new Error("不受支持的字符");
        }
      }
      url = url?.substring(0, url?.length - 1);
      config.params = {};
    }
    config.url = url;
    return config;
  },
  (error) => {
    console.error(error);
    return Promise.reject(error);
  }
);

service.interceptors.response.use(
  async (response) => {
    const { data } = response;
    const { code, msg } = data;
    if (code !== 200) {
      return Promise.reject(msg);
    }
    const isJSON = isJson(data.data); //判断返回的data是字符串还是成功信息
    if (isJSON) {
      return JSON.parse(data.data);
    } else {
      return data;
    }
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default service;

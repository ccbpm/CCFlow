import Vue from 'vue'
import App from './App.vue'
import router from './router'
import store from './store'

// 初始化elementUi
import ElementUI from 'element-ui'
import 'element-ui/lib/theme-chalk/index.css'
Vue.use(ElementUI)
import '@/styles/index.scss'
import '@/styles/iconfont/iconfont.css'

import '@fortawesome/fontawesome-free/css/fontawesome.css'
import '@fortawesome/fontawesome-free/css/regular.css'
import '@fortawesome/fontawesome-free/css/solid.css'

// 引入jquery
import $ from 'jquery'

Vue.prototype.$ = $; // 当然还有这句话 给vue原型上添加 $

//HttpHandler 暴露在全局
import { HttpHandler } from "@/wf/api/Gener.js";
Vue.prototype.HttpHandler = HttpHandler;

import { DBAccess } from "@/wf/api/Gener.js";
Vue.prototype.DBAccess = DBAccess;

import { Entities } from "@/wf/api/Gener.js";
Vue.prototype.Entities = Entities;

import { Entity } from "@/wf/api/Gener.js";
Vue.prototype.Entity = Entity;

import { GetPara } from "@/wf/api/Gener.js";
Vue.prototype.GetPara = GetPara;

Vue.config.productionTip = false

import Moment from 'moment'
Vue.prototype.moment = Moment

import dayjs from 'dayjs'
import duration from 'dayjs/plugin/duration' // 按需加载插件
dayjs.extend(duration)
Vue.prototype.dayjs = dayjs

//设置全局广播事件
Vue.prototype.$Bus = Vue.prototype.$Bus || new Vue();

new Vue({
    router,
    store,
    render: h => h(App)
}).$mount('#app')
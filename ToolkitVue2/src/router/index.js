import Vue from 'vue'
import VueRouter from 'vue-router'
import HomeView from '@/AppDemo/Home.vue'

Vue.use(VueRouter)
// 重写路由的push方法

// const routerPush = VueRouter.prototype.push
// VueRouter.prototype.push = function push(location) {
//   return routerPush.call(this, location).catch(error => error)
// }

const routes = [
  {
    path: '/home',
    name: 'home',
    component: HomeView,
    children: [
      {
        path: '/start', //发起
        name: 'start',
        component: () => import('@/wf/start.vue'),
      },
      {
        path: '/todolist', //待办
        name: 'todolist',
        component: () => import('@/wf/todolist.vue'),
      },
      {
        path: '/runing',// 在途
        name: 'runing',
        component: () => import('@/wf/runing.vue'),
      },
      {
        path: '/searchZongHe',
        name: 'searchZongHe',
        component: () => import('@/wf/searchZongHe.vue'),
      },
      {
        path: '/complete',//已完成
        name: 'complete',
        component: () => import('@/wf/complete.vue'),
      },

      {
        path: '/draft', //草稿
        name: 'draft',
        component: () => import('@/wf/draft.vue'),
      },
      {
        path: '/send', // 抄送
        name: 'send',
        component: () => import('@/wf/send.vue'),
      },
      {
        path: '/batch',
        name: 'batch',
        component: () => import('@/wf/batch.vue'),
      },
      {
        path: '/WorkCheckModel',
        name: 'WorkCheckModel',
        component: () => import('@/wf/workopt/batch/WorkCheckModel.vue'),
      },
      {
        path: '/mysend',//我发起的
        name: 'mysend',
        component: () => import('@/wf/comm/search.vue'),

      },
      {
        path: '/myjoin',//我参与的
        name: 'myjoin',
        component: () => import('@/wf/comm/search.vue'),
      },

      {
        path: '/search',
        name: 'search',
        component: () => import('@/wf/comm/search.vue')
      },
      {
        path: '/default', //个人中心
        name: 'Default',
        component: () => import('@/wf/default.vue')
      },
      {
        path: '/powerlist',//授权列表
        name: 'powerlist',
        component: () => import('@/wf/powerlist.vue')
      },
      {
        path: '/API',//授权列表
        name: 'API',
        component: () => import('@/wf/API.vue')
      },
      {
        path: '/QingJia',
        name: 'QingJia',
        component: () => import('@/AppDemo/Frms/F024SDKToolbarFrm.vue')
      }
    ]
  },
  {
    path: '/',
    name: 'login',
    component: () => import('@/AppDemo/Login.vue')
  },

  {
    path: '/F024SDKToolbarFrm',
    name: 'F024SDKToolbarFrm',
    component: () => import('@/AppDemo/Frms/F024SDKToolbarFrm.vue')
  },

]

const router = new VueRouter({
  mode: 'history', //项目发布的时候需要设置成hash,否则发布项目运行会出现空白
  routes
})

export default router

import { createRouter, createWebHistory } from 'vue-router'
import LoginView from '../views/Login.vue'
import IndexView from '../views/Index.vue'
import Error404 from '@/views/error/404.vue'

import {
    UserOutlined,
    AppstoreOutlined,
    SendOutlined,
    BellOutlined,
    FieldTimeOutlined,
    CheckCircleOutlined,
    EditOutlined,
    SwitcherOutlined,
    SearchOutlined,
    FormOutlined,
    ApiOutlined,
    UnorderedListOutlined,
    ApartmentOutlined,
    BarsOutlined,
} from '@ant-design/icons-vue';
import { REDIRECT_NAME } from './constant';
export const menuRoutes = [
    {
        path: '/wf',
        name: 'workflow',
        meta: {
            icon: <AppstoreOutlined />,
            label: '基础功能',
        },
        component: IndexView,
        redirect: '/wf/todo',
        children: [{
            path: 'start',
            name: 'wfStart',
            meta: {
                icon: <SendOutlined />,
                label: '发起',
                title: '发起',
            },
            component: () => import('@/views/workflow/Start.vue')
        }, {
            path: 'todo',
            name: 'wfTodo',
            meta: {
                icon: <BellOutlined />,
                title: '待办',
                label: '待办',
                affix: true,
            },
            component: () => import('@/views/workflow/TodoList.vue')
        }, {
            path: 'running',
            name: 'wfRunning',
            meta: {
                icon: <FieldTimeOutlined />,
                title: '在途',
                label: '在途',
            },
            component: () => import('@/views/workflow/Running.vue')
        }, {
            path: 'completed',
            name: 'wfCompleted',
            meta: {
                icon: <CheckCircleOutlined />,
                title: '已完成',
                label: '已完成',
            },
            component: () => import('@/views/workflow/Completed.vue')
        }, {
            path: 'query',
            name: 'wfQuery',
            meta: {
                icon: <SearchOutlined />,
                title: '查询',
                label: '查询',
                scop:'2',
            },
            component: () => import('@/views/workflow/IntegratedQuery.vue')
        }, {
            path: 'draft',
            name: 'wfDraft',
            meta: {
                icon: <FormOutlined />,
                title: '草稿',
                label: '草稿',
            },
            component: () => import('@/views/workflow/Draft.vue')
        }, {
            path: 'cc',
            name: 'wfCC',
            meta: {
                icon: <EditOutlined />,
                title: '抄送',
                label: '抄送',
            },
            component: () => import('@/views/workflow/Send.vue')
        }, {
            path: 'batch',
            name: 'wfBatch',
            meta: {
                icon: <SwitcherOutlined />,
                title: '批处理',
                label: '批处理',
            },
            component: () => import('@/views/workflow/Batch.vue')
        }]
    },
    {
        path: '/query',
        name: 'query',
        meta: {
            icon: <SearchOutlined />,
                label: '流程查询',
        },
        component: IndexView,
        redirect: '/query/start',
        children: [
            {
                path: 'start',
                name: 'startQuery',
                meta: {
                    icon: <SendOutlined />,
                    title: '我发起的',
                    label: '我发起的',
                    scop:'1',//查询范围
                    
                },
                component: () => import('@/views/workflow/IntegratedQuery.vue')

            }, {
                path: 'participation',
                name: 'participationQuery',
                meta: {
                    icon: <UserOutlined />,
                    title: '我参与的',
                    label: '我参与的',
                    scop:'0',//查询范围
                },
                component: () => import('@/views/workflow/IntegratedQuery.vue')

            }
        ]
    },
    {
        path: '/functionCall',
        name: 'functionCall',
        meta: {
            icon: <AppstoreOutlined />,
            label: '功能调用',
        },
        component: IndexView,
        redirect: '/functionCall/FlowTree',
        children: [
            {
                path: 'FlowTree',
                name: 'FlowTree',
                meta: {
                    icon: <ApartmentOutlined />,
                    label: '流程设计',
                    title: '流程设计',
                    
                },
                component: () => import('@/views/functionCall/callPort.vue'),

            }, {
                path: 'FormTree',
                name: 'FormTree',
                meta: {
                    icon: <BarsOutlined />,
                    label: '表单设计',
                    title: '表单设计',
                },
                component: () => import('@/views/functionCall/callPort.vue'),

            }
        ]
    },
    {
        path: '/toolkitApis',
        name: "Apis",
        meta: {
            icon: <ApiOutlined />,
            label: 'API列表',
        },
        redirect: '/toolkitApis/list',
        component: IndexView,
        children: [
            {
                path: 'list',
                name: 'apiList',
                meta: {
                    icon: <UnorderedListOutlined />,
                    title: '工具包接口',
                    label: '工具包接口',
                },
                component: () => import('@/views/api/List.vue')
            }
        ]
    },
]
const staticRoutes = [
    {
        path: '/',
        name: 'Index',
        redirect: '/wf/todo'
    },
    {
        path: '/login',
        name: 'login',
        component: LoginView,
        meta: {
            title: 'ToolKit 登录',
        }
    },
    {
        path: '/:path(.*)*',
        name: 'error',
        component: Error404,
    },
    {
        path: '/redirect',
        component: IndexView,
        name: 'RedirectTo',
        meta: {
            title: REDIRECT_NAME,
            hideBreadcrumb: true,
            hideMenu: true,
        },
        children: [
            {
            path: '/redirect/:path(.*)',
            name: REDIRECT_NAME,
            component: () => import('@/views/redirect/index.vue'),
            meta: {
                title: REDIRECT_NAME,
                hideBreadcrumb: true,
            },
            },
        ],
    }

]

const routes = [...menuRoutes, ...staticRoutes]
export const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes
})


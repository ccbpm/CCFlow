import { Navigate, createBrowserRouter, createHashRouter } from 'react-router-dom'
import Login from '../pages/Login'
import Index from '../pages/Layout'
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
} from '@ant-design/icons';
import Error404 from '../pages/error/Error404'
import TodoList from '../pages/workflow/TodoList'
import { RequireAuth, LoginPageGuard } from './Guard';

export const menuRoutes = [
    {
        path: '/wf',
        key: 'workflow',
        icon: <AppstoreOutlined />,
        label: '基础功能',
        element: <RequireAuth><Index /></RequireAuth>,
        redirect: '/wf/todo',
        children: [{
            path: 'start',
            key: '/wf/start',
            icon: <SendOutlined />,
            label: '发起',
            async lazy() {
                const Start = (await import('../pages/workflow/Start')).default
                return { Component: Start }
            }
        }, {
            indexed: true,
            path: 'todo',
            key: '/wf/todo',
            icon: <BellOutlined />,
            label: '待办',
            element: <TodoList />
        },
        {
            path: 'running',
            key: '/wf/running',
            icon: <FieldTimeOutlined />,
            label: '在途',
            async lazy() {
                const Running = (await import('../pages/workflow/Running')).default
                return { Component: Running }
            }
        },
        {
            path: 'completed',
            key: '/wf/completed',
            icon: <CheckCircleOutlined />,
            label: '已完成',
            async lazy() {
                const Completed = (await import('../pages/workflow/Completed')).default
                return { Component: Completed }
            }
        },
             {
                path: 'query',
                key: '/wf/query',
                icon: <SearchOutlined />,
                label: '查询',
                scop: '2',
                async lazy() {
                    const IntegratedQuery = (await import('../pages/workflow/IntegratedQuery')).default
                    return { Component: IntegratedQuery }
                }
            }, 
            {
                path: 'draft',
                key: '/wf/draft',
                icon: <FormOutlined />,
                label: '草稿',
                async lazy() {
                    const Draft = (await import('../pages/workflow/Draft')).default
                    return { Component: Draft }
                }
            }, 
            {
                path: 'cc',
                key: '/wf/cc',
                icon: <EditOutlined />,
                label: '抄送',
                async lazy() {
                    const Send = (await import('../pages/workflow/Send')).default
                    return { Component: Send }
                }
            }, 
            {
                path: 'batch',
                key: '/wf/batch',
                icon: <SwitcherOutlined />,
                label: '批处理',
                async lazy() {
                    const Batch = (await import('../pages/workflow/Batch')).default
                    return { Component: Batch }
                }
            }
        ]
    },
    {
        path: '/query',
        key: 'query',
        icon: <SearchOutlined />,
        label: '流程查询',
        element: <RequireAuth><Index /></RequireAuth>,
        redirect: '/query/start',
        children: [
            {
                path: 'start',
                key: '/query/start',
                icon: <SendOutlined />,
                label: '我发起的',
                scop: '1',//查询范围
                async lazy() {
                    const IntegratedQuery = (await import('../pages/workflow/IntegratedQuery')).default
                    return { Component: IntegratedQuery }
                }
            }, {
                path: 'participation',
                key: '/query/participation',
                icon: <UserOutlined />,
                label: '我参与的',
                scop: '0',//查询范围
                async lazy() {
                    const IntegratedQuery = (await import('../pages/workflow/IntegratedQuery')).default
                    return { Component: IntegratedQuery }
                }
            }
        ]
    },
    {
        path: '/functionCall',
        key: 'functionCall',
        icon: <AppstoreOutlined />,
        label: '功能调用',
        meta: {
            icon: <AppstoreOutlined />,
            label: '功能调用',
        },
        element: <RequireAuth><Index /></RequireAuth>,
        redirect: '/functionCall/FlowTree',
        children: [
            {
                path: 'FlowTree',
                key: '/functionCall/FlowTree',
                icon: <ApartmentOutlined />,
                label: '流程设计',
                async lazy() {
                    const CallPort = (await import('../pages/functionCall/CallPort')).default
                    return { Component: CallPort }
                }
            }, {
                path: 'FormTree',
                key: '/functionCall/FormTree',
                icon: <BarsOutlined />,
                label: '表单设计',
                async lazy() {
                    const CallPort = (await import('../pages/functionCall/CallPort')).default
                    return { Component: CallPort }
                }
            }
        ]
    },
    {
        path: '/apis',
        key: "Apis",
        icon: <ApiOutlined />,
        label: 'API列表',
        redirect: '/apis/list',
        element: <RequireAuth><Index /></RequireAuth>,
        children: [
            {
                path: 'list',
                key: '/apis/list',
                icon: <UnorderedListOutlined />,
                label: '工具包接口',
                async lazy() {
                    const List = (await import('../pages/api/List')).default
                    return { Component: List }
                }
            }
        ]
    },
]
const staticRoutes = [
    {
        path: "/",
        name: 'IndexRedirect',
        element: <Navigate to="/wf/todo" />,
    },
    {
        path: '/login',
        key: 'login',
        element: <LoginPageGuard><Login /></LoginPageGuard>
    },
    {
        path: '*',
        element: <Error404 />
    }
]

const routes = [...menuRoutes, ...staticRoutes]

export const router = createHashRouter(routes)
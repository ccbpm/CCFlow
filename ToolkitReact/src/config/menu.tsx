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
    UnorderedListOutlined
} from '@ant-design/icons';
const menuItems = [
    {
        key: 'basic',
        icon: <AppstoreOutlined />,
        label: '基础功能',
        children: [{
            key: 'start',
            icon: <SendOutlined />,
            label: '发起',
        }, {
            key: 'todo',
            icon: <BellOutlined />,
            label: '待办',
        }, {
            key: 'running',
            icon: <FieldTimeOutlined />,
            label: '在途',
        }, {
            key: 'completed',
            icon: <CheckCircleOutlined />,
            label: '已完成',
        }, {
            key: 'query',
            icon: <SearchOutlined />,
            label: '查询',
        }, {
            key: 'draft',
            icon: <FormOutlined />,
            label: '草稿',
        }, {
            key: 'cc',
            icon: <EditOutlined />,
            label: '抄送',
        }, {
            key: 'batch',
            icon: <SwitcherOutlined />,
            label: '批处理',
        }]
    },
    {
        key: 'fq',
        icon: <SearchOutlined />,
        label: '流程查询',
        children: [
            {
                key: 'fq-start',
                icon: <SendOutlined />,
                label: '我发起的',
            }, {
                key: 'fq-participation',
                icon: <UserOutlined />,
                label: '我参与的',
            }
        ]
    },
    {
        key: 'apis',
        icon: <ApiOutlined />,
        label: 'API列表',
        children: [
            {
                key: 'list',
                icon: <UnorderedListOutlined />,
                label: '工具包接口',
            }
        ]
    },
]


export default menuItems;
import { DownOutlined } from '@ant-design/icons';
import type { MenuProps } from 'antd';
import { Dropdown, Space, message } from 'antd';
import { UserOutlined } from '@ant-design/icons'
import { useNavigate } from 'react-router-dom';
import { userStore } from '../store/userStore';

const items: MenuProps['items'] = [
    {
        label: <p>退出登录</p>,
        key: 'logout',
    },
];

const UserInfoDropdown = () => {
    const navigate = useNavigate();
    const onMenuItemClick: MenuProps['onClick'] = ({ key }) => {
        
        switch (key) {
            case 'logout': {
                userStore.setToken('')
                userStore.setUserInfo(null);
                navigate('/login', { replace: true })
                void message.info(`已退出登录`);
            }
        }
    };
    return (
        <Dropdown menu={{ items, onClick: onMenuItemClick }} trigger={['click']}>
            <div style={{ cursor: 'pointer' }}>
                <Space>
                    <UserOutlined />
                    {userStore.userInfo?.Name}
                    <DownOutlined />
                </Space>
            </div>
        </Dropdown>
    )
}

export default UserInfoDropdown

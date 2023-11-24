import React, { useState } from 'react';
import {
    MenuFoldOutlined,
    MenuUnfoldOutlined,
} from '@ant-design/icons';
import { Layout, Menu, Button, theme } from 'antd';
import type { MenuProps } from 'antd'
import { Outlet, useNavigate } from 'react-router-dom';
import styled from 'styled-components';
import LogoCollapsePic from '@/assets/logo_collapse.png';
import LogoNormalPic from '@/assets/logo.png'
import { menuRoutes } from '../router/index';
import UserInfoDropdown from '../components/UserInfoDropdown';
const { Header, Sider, Content } = Layout;
type MenuItem = Required<MenuProps>['items'][number];

const Logo = styled.div`
    height: 64px;
    color: white;
    display:flex;
    align-items:center;
    justify-content: center;

    img {
        height: 40px;
        object-fit: contain;
    }
`

const FlexHeaderContent = styled.div`
    display:flex;
    align-items:center;
    justify-content: space-between;
    height: 100%;
    padding-right: 24px;
    box-sizing: border-box;
`

const Index: React.FC = () => {
    const [collapsed, setCollapsed] = useState(false);
    const {
        token: { colorBgContainer },
    } = theme.useToken();

    const navigate = useNavigate();
    const onMenuSelect = (item: MenuItem) => {
        if (typeof item?.key === 'string' && item.key.startsWith("/"))
            navigate(item.key)
    }

    const menuList = menuRoutes.map(menu => {
        const children = menu.children.map(child => ({ key: child.key, label: child.label, icon: child.icon }));
        return {
            key: menu.key,
            icon: menu.icon,
            label: menu.label,
            children
        }
    })

    return (
        <Layout style={{ height: '100vh' }}>
            <Sider trigger={null} collapsible collapsed={collapsed}>
                <Logo className="demo-logo-vertical" >
                    <img src={collapsed ? LogoCollapsePic : LogoNormalPic} alt='logo' />
                </Logo>
                <Menu
                    theme="dark"
                    mode="inline"
                    defaultSelectedKeys={['1']}
                    onSelect={onMenuSelect}
                    items={menuList}
                />
            </Sider>
            <Layout>
                <Header style={{ padding: 0, background: colorBgContainer }}>
                    <FlexHeaderContent>
                        <Button
                            type="text"
                            icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
                            onClick={() => setCollapsed(!collapsed)}
                            style={{
                                fontSize: '16px',
                                width: 64,
                                height: 64,
                            }}
                        />
                        <UserInfoDropdown />
                    </FlexHeaderContent>

                </Header>
                <Content
                    style={{
                        margin: '24px 16px',
                        padding: 24,
                        minHeight: 280,
                        background: colorBgContainer,
                    }}
                >
                    <Outlet />
                </Content>
            </Layout>
        </Layout>
    );
};

export default Index;
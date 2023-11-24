import { styled } from "styled-components"
import LoginBg from '@/assets/login_bg.jpg';
import CCLogo from '@/assets/logo.png';
import { LockOutlined, UserOutlined } from '@ant-design/icons';
import { Button, Form, Input, message } from 'antd';

import { userStore } from "../store/userStore";
import { useNavigate } from "react-router-dom";



const LoginWrapper = styled.div`
    width: 100vw;
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: flex-start;
    background-color: #f2f5f7;
`

const Sider = styled.div`
    width: 400px;
    background-image: url(${LoginBg});
    height: 100%;
    flex-direction: column;
    display: flex;
    align-items: center;
    justify-content: center;
    position: relative;
`

const SiderLogo = styled.img`
    height: 40px;
    object-fit: cover;
    position: absolute;
    top: 20px;
    left: 30px;
    z-index: 10;

`

const SiderTitle = styled.div`
    font-size: 24px;
    color: white;
    width: 100%;
    box-sizing: border-box;
    padding-left: 50px;
`

const Description = styled.div`
    font-size: 14px;
    color: white;
    width: 100%;
    box-sizing: border-box;
    padding-left: 50px;
    margin-top: 30px;
`

const Content = styled.div`
    height: 100%;
    flex: 1;
    display: flex;
    align-items: center;
    justify-content: center;
`

const LoginBox = styled.div`
    width: 400px;
    height: 300px;
    border-radius: 12px;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
`


const Login = () => {
    const navigate = useNavigate();
    const onFinish = async (values: Record<string, string>) => {
        try {
            await userStore.login({
                username: values.username,
                password: '',
            })
            void message.success('登录成功， 欢迎使用CCFlowForReact');
            navigate("/", { replace: true })
        } catch (err: any) {
            void message.error(err.toString());
        }

    };
    return (
        <>
            <LoginWrapper>
                <Sider>
                    <SiderLogo src={CCLogo} alt="logo" />
                    <SiderTitle>CCFlow Toolkit For React</SiderTitle>
                    <Description>包含以下功能</Description>
                    <Description>1. 流程发起 / 待办 / 查询</Description>
                    <Description>2. 流程统计分析</Description>
                    <Description>3. CCFlow后端Rest接口Swagger文档</Description>
                </Sider>
                <Content >
                    <LoginBox>

                        <Form
                            name="normal_login"
                            className="login-form"
                            initialValues={{ remember: true, username: 'admin', password: '123' }}
                            // eslint-disable-next-line @typescript-eslint/no-misused-promises
                            onFinish={onFinish}
                            style={{ width: '100%' }}
                        >

                            <Form.Item>
                                <div style={{ fontSize: '18px' }}>登录到 CCFlow</div>
                            </Form.Item>
                            <Form.Item
                                name="username"
                                rules={[{ required: true, message: '请输入用户名' }]}
                            >
                                <Input prefix={<UserOutlined className="site-form-item-icon" />} placeholder="用户名" />
                            </Form.Item>
                            <Form.Item
                                name="password"
                                rules={[{ required: true, message: '请输入密码' }]}
                            >
                                <Input
                                    prefix={<LockOutlined className="site-form-item-icon" />}
                                    type="password"
                                    placeholder="密码"
                                />
                            </Form.Item>

                            <Form.Item>
                                <Button type="primary" htmlType="submit" style={{ width: '100%' }}>
                                    登 录
                                </Button>
                            </Form.Item>
                        </Form>
                    </LoginBox>

                </Content>
            </LoginWrapper>
        </>
    )
}

export default Login
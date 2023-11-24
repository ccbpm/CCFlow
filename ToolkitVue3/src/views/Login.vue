<script lang="tsx">
import CCLogo from '@/assets/logo.png'
import { defineComponent, reactive } from 'vue'
import { LockOutlined, UserOutlined } from '@ant-design/icons-vue'
import { Button, Form, FormItem, Input } from 'ant-design-vue'
import { useUserStore } from '@/stores/user'
interface FormState {
  username: string
  password: string
}
export default defineComponent({
  name: 'login-page',
  setup() {
    const formState = reactive<FormState>({
      username: 'admin',
      password: '123'
    })

    const login = async () => {
      const webUser = useUserStore()
      await webUser.login({
        username: formState.username,
        password: formState.password
      })
    }

    return () => (
      <div class="login-wrapper">
        <div class="sider">
          <img class="sider-logo" src={CCLogo} alt="logo" />
          <div class="sider-title">CCFlow Toolkit For Vue3</div>
          <div class="description">包含以下功能</div>
          <div class="description">1. 流程发起 / 待办 / 查询</div>
          <div class="description">2. 流程统计分析</div>
          <div class="description">3. CCFlow后端Rest接口Swagger文档</div>
          <div class="description">4. <a href='https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=8095272&doc_id=31094' target='_blank'>在线文档</a></div>
          
        </div>
        <div class="content">
          <div class="login-box">
            <Form name="basic" onSubmit={login} style={{ width: '100%' }} model={formState}>
              <FormItem>
                <div style={{ fontSize: '18px' }}>登录 驰骋BPM - ToolkiteVue3</div>
              </FormItem>
              <FormItem name="username" rules={[{ required: true, message: '请输入用户名' }]}>
                <Input
                  prefix={<UserOutlined style={{ marginRight: '8px' }} />}
                  placeholder="用户名"
                  autocomplete="username"
                  v-model:value={formState.username}
                />
              </FormItem>
              <FormItem name="password" rules={[{ required: true, message: '请输入密码' }]}>
                <Input
                  prefix={<LockOutlined style={{ marginRight: '8px' }} />}
                  type="password"
                  placeholder="密码"
                  autocomplete="current-password"
                  v-model:value={formState.password}
                />
              </FormItem>
              <FormItem>
                <Button type="primary" htmlType="submit" style={{ width: '100%' }}>
                  登 录
                </Button>
              </FormItem>
            </Form>
          </div>
        </div>
      </div>
    )
  }
})
</script>

<style lang="less" scoped>
.login-wrapper {
  width: 100vw;
  height: 100vh;
  display: flex;
  align-items: center;
  justify-content: flex-start;
  background-color: #f2f5f7;
}

.sider {
  width: 400px;
  background-image: url(@/assets/login_bg.jpg);
  height: 100%;
  flex-direction: column;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.sider-logo {
  height: 40px;
  object-fit: cover;
  position: absolute;
  top: 20px;
  left: 30px;
  z-index: 10;
}

.sider-title {
  font-size: 24px;
  color: white;
  width: 100%;
  box-sizing: border-box;
  padding-left: 50px;
}

.description {
  font-size: 14px;
  color: white;
  width: 100%;
  box-sizing: border-box;
  padding-left: 50px;
  margin-top: 30px;
}

.content {
  height: 100%;
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
}

.login-box {
  width: 400px;
  height: 300px;
  border-radius: 12px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}
</style>

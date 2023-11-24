<script lang="tsx">
import { defineComponent } from 'vue'
import { Dropdown, MenuItem, Menu } from 'ant-design-vue'
import { useUserStore } from '@/stores/user'
import { DownOutlined, UserOutlined } from '@ant-design/icons-vue'
export default defineComponent({
  name: 'user-dropdown',
  setup() {
    const userStore = useUserStore()
    const logout = () => {
      userStore.confirmLoginOut();
    }
    return () => (
      <Dropdown
        trigger={['click']}
        v-slots={{
          overlay: () => (
            <Menu>
              <MenuItem key="logout" onClick={logout}>退出登录</MenuItem>
            </Menu>
          )
        }}
      >
        <div style={{cursor: 'pointer'}}>
          <UserOutlined  style={{marginRight: '12px'}}/>
          {userStore.webUser.Name}
          <DownOutlined style={{marginLeft: '6px'}}/>
        </div>
      </Dropdown>
    )
  }
})
</script>


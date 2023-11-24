<script lang="tsx">
import { defineComponent, ref, unref } from 'vue'
import {
  Layout,
  LayoutSider,
  Menu,
  SubMenu,
  MenuItem,
  LayoutHeader,
  LayoutContent
} from 'ant-design-vue'
import { RouterView, type RouteRecordRaw, useRouter, type RouteMeta, type RouteLocationNormalized, useRoute } from 'vue-router'
import LogoCollapsePic from '@/assets/logo_collapse.png'
import LogoNormalPic from '@/assets/logo.png'
import { MenuUnfoldOutlined, MenuFoldOutlined } from '@ant-design/icons-vue'
import { menuRoutes } from '@/router'
import UserDropdownVue from '@/components/UserDropdown.vue'
import HeaderTabs from '@/components/HeaderTabs.vue'
import { listenerRouteChange } from '@/logics/mitt/routeChange'
import { useUserStore } from '@/stores/user'
import { useMultipleTabStore } from '@/stores/multiTabs'
import { initAffixTabs } from '@/hooks/web/useMultipleTabs'
import { REDIRECT_NAME } from '@/router/constant'
import { useProjectStore } from '@/stores/projectSetting'
import { computed } from 'vue'

export default defineComponent({
  name: 'index-page',
  setup() {
    const collapsed = ref(false)
    const selectedKeys = ref<string[]>([])
    const tabStore = useMultipleTabStore();
    const router = useRouter();
    const userStore = useUserStore();
    const projectSetting = useProjectStore();
    const headerShow = computed(() => {
      return projectSetting.getHeaderSetting.isShow;
    });
    const siderShow = computed(() => {
      return projectSetting.getSiderSetting.isShow;
    });
    const renderMenuItem = (item: RouteRecordRaw) => {
      const meta = item.meta as any
      if (!meta) return <p>请配置meta参数</p>
      if (Array.isArray(item.children) && item.children.length > 0) {
        return (
          <SubMenu
            key={item.path}
            v-slots={{
              title: () => meta.label,
              icon: () => <meta.icon />
            }}
          >
            {item.children?.map((child) => {
              const cMeta = child.meta as any
              if (!meta) return <p>请配置meta参数</p>
              return (
                <MenuItem
                  key={item.path + '/' + child.path}
                  v-slots={{
                    icon: () => <cMeta.icon />
                  }}
                >
                  {cMeta.label}
                </MenuItem>
              )
            })}
          </SubMenu>
        )
      }
      return (
        <MenuItem key={item.path}>
          <meta.icon />
          {meta.label}
        </MenuItem>
      )
    }
    initAffixTabs();

    const onMenuSelect = (item: any) => {
      router.push(item.key);
      
    }
    listenerRouteChange((route: any) => {
      const { name } = route;
      if (name === REDIRECT_NAME || !route || !userStore.getToken) {
        return;
      }

      const { path, meta = {} } = route;
      selectedKeys.value = [path];
      const { currentActiveMenu, hideTab } = meta as RouteMeta;
      const isHide = !hideTab ? null : currentActiveMenu;
      if (isHide) {
        const findParentRoute = router.getRoutes().find((item) => item.path === currentActiveMenu);

        findParentRoute && tabStore.addTab(findParentRoute as unknown as RouteLocationNormalized);
      } else {
        tabStore.addTab(unref(route));
      }
    });
    return () => (
      <Layout style={{ height: '100vh' }}>
        {
          siderShow.value == true ? (
            <LayoutSider v-if = {siderShow.value} v-model:collapsed={collapsed.value} trigger={null} collapsible>
              <div class="logo">
                <img src={collapsed.value ? LogoCollapsePic : LogoNormalPic} alt="logo" />
              </div>
              <Menu
                v-model:selectedKeys={selectedKeys.value}
                theme="dark"
                mode="inline"
                onSelect={onMenuSelect}
              >
                {menuRoutes.map((menu) => renderMenuItem(menu))}
              </Menu>
            </LayoutSider>
          ):(null)
        }
        <Layout>
          {
            headerShow.value == true ? (
              <LayoutHeader style={{ background: '#fff', padding: 0, height: '48px' }}>
                <div class="header-content">
                  {collapsed.value ? (
                    <MenuUnfoldOutlined
                      class="trigger"
                      onClick={() => (collapsed.value = !collapsed.value)}
                    />
                  ) : (
                    <MenuFoldOutlined
                      class="trigger"
                      onClick={() => (collapsed.value = !collapsed.value)}
                    />
                  )}
                  <UserDropdownVue />
                </div>
              </LayoutHeader>
            ):(null)
          }
          <HeaderTabs></HeaderTabs>
          <div style={{height:'calc(100% - 35px - 48px - 48px)'}}>
            <LayoutContent
            style={{
              margin: '24px 16px',
              padding: '24px',
              background: '#fff',
              minHeight: '280px',
              height : '100%',
            }}
          >
            <RouterView />
          </LayoutContent>
          </div>
          
        </Layout>
      </Layout>
    )
  }
})
</script>

<style lang="less" scoped>
.logo {
  height: 48px;
  color: white;
  display: flex;
  align-items: center;
  justify-content: center;

  img {
    height: 36px;
    object-fit: contain;
  }
}

.header-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-sizing: border-box;
  padding-right: 24px;
  height: 48px;
}

:deep(.trigger) {
  font-size: 18px;
  line-height: 64px;
  padding: 0 24px;
  cursor: pointer;
  transition: color 0.3s;
  &:hover {
    color: #1890ff;
  }
}

.site-layout .site-layout-background {
  background: #fff;
}
</style>

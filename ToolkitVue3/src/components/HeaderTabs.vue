<template>
  <div class="header-tabs">
        <div class="tabs-content hrader-multiple-tabs">
          <Tabs type="editable-card" size="small" :animated="false" :hideAdd="true" :tabBarGutter="3" :activeKey="activeKeyRef" @change="handleChange" @edit="handleEdit">
            <TabPane v-for="item in getTabsState" :key="item.query ? item.fullPath : item.path" :closable="!(item.path.includes('todo'))" style="padding: 0 !important;">
              <template #tab>
                <div>
                  <TabContent :tabItem="item" class="appearance" />
                </div>
              </template>
            </TabPane>
          </Tabs>
        </div>
        <div class="affix flex-row-center">
          <div class="dropdown-trigger flex-row-center" @click="reloadCurrent">
            <RedoOutlined title="重新加载" />
          </div>
          <Dropdown
            :trigger="['click']">
            <div class="dropdown-trigger flex-row-center">
              <DownOutlined />
            </div>
            <template #overlay>
              <Menu>
                  <MenuItem key="reload" :onClick="reloadCurrent">
                    重新加载
                  </MenuItem>
                  <MenuItem key="closeCurrent" @click="closeCurrent">
                    关闭当前
                  </MenuItem>
                  <MenuItem key="closeAll" :onClick= "closeAll">
                    关闭所有
                  </MenuItem>
                </Menu>
            </template>
          </Dropdown>
          <div class="dropdown-trigger flex-row-center" @click="fullScreen">
            <FullscreenExitOutlined v-if="isFullscreen"/>
            <FullscreenOutlined v-else/>
          </div>
        </div>
      </div>
</template>
<script lang="ts" setup>
import {
  DownOutlined,
  FullscreenExitOutlined,
  FullscreenOutlined,
  RedoOutlined,
} from '@ant-design/icons-vue'
import { Dropdown, Menu, MenuItem, TabPane } from 'ant-design-vue'
import { computed, ref, unref } from 'vue'
import { Tabs } from 'ant-design-vue';
import { useRouter } from 'vue-router'
import { useMultipleTabStore } from '@/stores/multiTabs';
import { useGo } from '@/hooks/web/usePage';
import TabContent from './TabContent.vue' ;
import { listenerRouteChange } from '@/logics/mitt/routeChange';
import { useProjectStore } from '@/stores/projectSetting';
import { triggerWindowResize } from '@/utils/event/event';
const activeKeyRef = ref('');
const tabStore = useMultipleTabStore();
const projectSetting = useProjectStore();
const router = useRouter()
const isFullscreen = ref(false);
const go = useGo();
const getTabsState = computed(() => {
  return tabStore.getTabList.filter((item: any) => !item.meta?.hideTab);
}); 
const unClose = computed(() => unref(getTabsState).length === 1);
const handleChange = (activeKey: any) => {
  activeKeyRef.value = activeKey;
  go(activeKey, false);
}
listenerRouteChange((route: any) => {
  activeKeyRef.value = route.query ? route.fullPath : route.path;
});

// Close the current tab
const handleEdit = (targetKey: string) => {
  // Added operation to hide, currently only use delete operation
  if (unref(unClose)) {
    return;
  }

  tabStore.closeTabByKey(targetKey, router);
}
const closeCurrent = async () => {
  const currentTab = tabStore.getTabList.filter((item) => item.path == activeKeyRef.value)[0];
  await tabStore.closeTab(currentTab, router);
}
const closeAll = async () => {
  await tabStore.closeAllTab(router);
}
const reloadCurrent = async () => {
  await tabStore.refreshPage(router);

}
const fullScreen = () => {
  isFullscreen.value = !isFullscreen.value;
  projectSetting.setHeaderShow(!isFullscreen.value);
  projectSetting.setSiderShow(!isFullscreen.value);
}

</script>

<style lang="less" scoped>
.flex-row-center {
  display: flex;
  align-items: center;
  justify-content: center;
}
.header-tabs {
  height: 35px;
  min-height: 35px;
  width: 100%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  background-color: white;
  border-top: 1px solid #eeeeee;
  box-sizing: border-box;

  .affix {
    height: 100%;
  }
  .dropdown-trigger {
    cursor: pointer;
    height: 100%;
    width: 40px;
    border-left: 1px solid #eeeeee;
    &:deep(span) {
      color: #999;
      &:hover {
        color: #111;
      }
    }
  }
}
:deep(.ant-tabs-nav){
  margin: 0;
}

 :deep(.ant-tabs-tab) {
  padding: 0 !important;
  margin-left: 3px !important;
  margin-right: 3px !important;
  height: 30px !important;
  border-radius: 5px !important;
  background-color: white !important;
 }
 :deep(.ant-tabs-tab-active) {
  padding: 0 !important;
  margin-left: 3px !important;
  margin-right: 3px !important;
  background-color: rgb(9, 96, 189) !important;
  height: 30px !important;
  border-radius: 5px !important;
  .ant-tabs-tab-btn{
    color: white;
  }
  .ant-tabs-tab-remove{
    color: white;
  }
 }
 :deep(.ant-tabs-tab-remove){
  margin-left: -8px ;
  margin-right: 4px;
  padding: 0;
 }
</style>

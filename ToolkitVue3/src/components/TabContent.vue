<template>
    <Dropdown  :trigger="['contextmenu']" placement="bottom" overlayClassName="multiple-tabs__dropdown">
      <div  style="font-size: 14px !important;margin-left: 10px; margin-right: 10px" @contextmenu="handleContext">
        <span class="ml-1">{{ getTitle }}</span>
      </div>
      <template #overlay>
        <Menu>
          <template v-for = "dropMenu in getDropMenuList" :key="dropMenu.event">
            <MenuItem :disabled="dropMenu.disabled" @click="handleClickMenu(dropMenu)">
              <span>{{ dropMenu.text }}</span>
            </MenuItem>
          </template>
        </Menu>
      </template>
    </Dropdown>
  </template>
  <script lang="ts">
    import type { PropType } from 'vue';
    import type { RouteLocationNormalized } from 'vue-router';
    import {Dropdown, Menu, MenuItem} from 'ant-design-vue';
    import { defineComponent, computed, unref } from 'vue';
    import { useTabDropdown } from '@/hooks/web/useTabDropdown';
    import type { DropMenu, TabContentProps } from '@/types/types';
    import { Item } from 'ant-design-vue/lib/menu';

  
    export default defineComponent({
      name: 'TabContent',
      components:{Dropdown, MenuItem, Menu},
      props: {
        tabItem: {
          type: Object as PropType<RouteLocationNormalized>,
          default: null,
        },
        isExtra: Boolean,
      },
      setup(props) {
        const { getDropMenuList, handleMenuEvent, handleContextMenu } = useTabDropdown(props as TabContentProps);
        const handleMenuClick = () => {};
        const getTitle = computed(() => {
        const { tabItem: { meta } = {} } = props;
        return meta && meta.title as string;
        });
        function handleClickMenu(item: DropMenu) {
          const { event } = item;
          // const menu = getDropMenuList.find((item) => `${item.event}` === `${event}`);
          // emit('menuEvent', menu);
          handleMenuEvent(item);
          item.onClick?.();
        }
        function handleContext(e) {
        props.tabItem && handleContextMenu(props.tabItem)(e);
      }
        
        return {
          handleMenuClick,
          handleMenuEvent,
          getTitle,
          getDropMenuList,
          handleClickMenu,
          handleContext,
        };
      },
    });
  </script>
  
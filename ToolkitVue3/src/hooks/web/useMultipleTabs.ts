import { toRaw, ref } from 'vue';
import type { RouteLocationNormalized } from 'vue-router';
import { useSortable } from '@/hooks/web/useSortable';
import { useMultipleTabStore } from '@/stores/multiTabs';
import { isNullAndUnDef } from '@/utils/is';
import { useRouter } from 'vue-router';

export function initAffixTabs(): string[] {
  const affixList = ref<RouteLocationNormalized[]>([]);

  const tabStore = useMultipleTabStore();
  const router = useRouter();
  /**
   * @description: Filter all fixed routes
   */
  function filterAffixTabs(routes: RouteLocationNormalized[]) {
    const tabs: RouteLocationNormalized[] = [];
    routes &&
      routes.forEach((route) => {
        if (route.meta && route.meta.affix) {
          tabs.push(toRaw(route));
        }
      });
    return tabs;
  }

  /**
   * @description: Set fixed tabs
   */
  function addAffixTabs(): void {
    const affixTabs = filterAffixTabs(router.getRoutes() as unknown as RouteLocationNormalized[]);
    affixList.value = affixTabs;
    for (const tab of affixTabs) {
      tabStore.addTab({
        meta: tab.meta,
        name: tab.name,
        path: tab.path,
      } as unknown as RouteLocationNormalized);
    }
  }

  let isAddAffix = false;

  if (!isAddAffix) {
    addAffixTabs();
    isAddAffix = true;
  }
  return affixList.value.map((item) => item.meta?.title).filter(Boolean) as string[];
}

export function useTabsDrag(affixTextList: string[]) {
  console.log('未添加该功能');
  return;
  // const tabStore = useMultipleTabStore();
  // // const { prefixCls } = useDesign('multiple-tabs');
  // nextTick(() => {
  //   //if (!multiTabsSetting.canDrag) return;
  //   const el = document.querySelectorAll(`.hrader-multiple-tabs .ant-tabs-nav-wrap > div`)?.[0] as HTMLElement;
  //   const { initSortable } = useSortable(el, {
  //     filter: (e: ChangeEvent) => {
  //       const text = e?.target?.innerText;
  //       if (!text) return false;
  //       return affixTextList.includes(text);
  //     },
  //     onEnd: (evt: { oldIndex: any; newIndex: any; }) => {
  //       const { oldIndex, newIndex } = evt;

  //       if (isNullAndUnDef(oldIndex) || isNullAndUnDef(newIndex) || oldIndex === newIndex) {
  //         return;
  //       }

  //       tabStore.sortTabs(oldIndex, newIndex);
  //     },
  //   });
  //   initSortable();
  // });
}

import type { Router } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { removeTabChangeListener, setRouteChange } from '@/logics/mitt/routeChange'
import { useMultipleTabStore } from '@/stores/multiTabs'

// Don't change the order of creation
export function setupRouterGuard(router: Router) {
  createLoginGuard(router);
  createPageGuard(router);
}

function createLoginGuard(router: Router) {
  const tabStore = useMultipleTabStore();
  const webUser = useUserStore();
  router.beforeEach((to, from, next) => {
    if (webUser.getToken && to.path === '/login') {
      tabStore.resetState();
      webUser.resetState();
      removeTabChangeListener();
      next('/')
      return
    }
		if(!webUser.getToken && to.path !== '/login') {
      tabStore.resetState();
      webUser.resetState();
      removeTabChangeListener();
			next("/login");
			return;
		}
		next();
  })
}

/**
 * Hooks for handling page state
 */
function createPageGuard(router: Router) {
  const loadedPageMap = new Map<string, boolean>();

  router.beforeEach(async (to) => {
    // The page has already been loaded, it will be faster to open it again, you don’t need to do loading and other processing
    to.meta.loaded = !!loadedPageMap.get(to.path);
    const title = to.meta.title ? to.meta.title : '' ;
    document.title = title + ' CCFlow-Vue3-Toolkit' as string;
    // listen route change to change tab list
    setRouteChange(to);

    return true;
  });

  router.afterEach((to) => {
    loadedPageMap.set(to.path, true);
  });
}

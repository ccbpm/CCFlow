import 'ant-design-vue/dist/antd.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import App from './App.vue'
import { router } from './router'
import {setupRouterGuard } from './router/guard'

const app = createApp(App)

app.use(createPinia())
app.use(router)

setupRouterGuard(router);

app.mount('#app')

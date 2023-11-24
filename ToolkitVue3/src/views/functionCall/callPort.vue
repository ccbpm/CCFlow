<template>
    <iframe :src="url" width="100%" height="100%" style="border: 0"></iframe>
</template>
<script lang="ts" setup>
import { ref, watch } from 'vue';
import { useRoute } from 'vue-router';

const route = useRoute();
const url = ref('');
const InitPage = () => {
    if(route.path.includes('FlowTree'))
    url.value = 'http://localhost:3000/#/WF/Port?DoWhat=Flows';
    else if(route.path.includes('FormTree'))
    url.value = 'http://localhost:3000/#/WF/Port?DoWhat=Frms'
}
InitPage();
// 简单数据类型监听
watch(route, () => {
  InitPage();
}, {
  immediate: true//立即监听--进入就会执行一次
})
</script>
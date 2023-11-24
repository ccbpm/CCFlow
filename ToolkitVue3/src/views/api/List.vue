<template>
  <div style="width: 100%;height: 100%;">
    <iframe :src="url" scrolling="auto" frameborder="no" style="width: 100%; height: 100%" />
  </div>
</template>
<script setup lang="ts">
import { computed } from 'vue';
const host = import.meta.env.VITE_GLOB_APP_URL   //获取请求地址
const url = computed(() => host + '//swagger/ui/index#/API')
</script>
<style lang="less"></style>

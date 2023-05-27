<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script>
export default {
  name: 'app',
  provide() {
    return {
      reload: this.reload
    }
  },
  components: {
  },
  data() {
    return {
      isRouterAlive: true
    }
  },
  methods: {
    reload() {
      this.isRouterAlive = false;
      this.$nextTick(function () {
        this.isRouterAlive = true;
      })
    }
  },
  //组件动态key属性 的值
  computed: {
    key() {
      return this.$route.name !== undefined
          ? this.$route.name + new Date()
          : this.$route + new Date();
    }
  }
}
</script>

<style>
html,
body,
#app {
  margin: 0;
  padding: 0;
  width: 100%;
  height: 100%;
  box-sizing: border-box;
  overflow-x: hidden;
}
label {
  font-weight: inherit;
}

/* 滚动条样式覆盖 */
::-webkit-scrollbar {
  width: 6px;
  height: 6px;
}

::-webkit-scrollbar-track {
  -webkit-box-shadow: inset 0 0 6px rgba(0, 0, 0, 0);
  background-color: transparent;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb {
  border-radius: 10px;
  background-color: #e8e8e8;
}
</style>

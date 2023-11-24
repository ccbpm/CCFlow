<template>
  <iframe :src=src style="width:100%;height:100vh"/>
</template>

<script>
export default {
  name: "API",
  data() {
    return {
     src:""
    };
  },
  created() {
   if(process.env.VUE_APP_PLATFORM === 'CCFLOW')
     this.src = process.env.VUE_APP_API_URL+'swagger';
    if(process.env.VUE_APP_PLATFORM === 'JFLOW')
      this.src = process.env.VUE_APP_API_URL+'swagger-ui.html';
  },
}
</script>

<style scoped>

</style>
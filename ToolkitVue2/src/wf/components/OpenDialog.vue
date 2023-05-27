<template>
  <component :is="currentComponent" :urlParams="params"></component>
</template>

<script>

export default {
  name: "OpenDialog",
  props: {
    judgeOperation: {type: String, default: ""},
    params:{type:Object,default:()=>{}}
  },
  components: {
    'CH': () => import('../workopt/ch.vue'),
    'HuiQian': () => import('../workopt/hui-qian.vue'),
    'sendAccepter': () => import('./selectiveRecipient.vue'),
    'Track': () => import('./track.vue'),
    'Return': () => import('../workopt/returnwork.vue'),
    'CC': () => import('../workopt/cc.vue'),
    'Shift':() => import('../workopt/shift.vue'),
    'Rollback':()=>import('../workopt/Rollback.vue')
  },
  computed:{
    currentComponent(){
      return this.judgeOperation
    }
  },
  data() {
    return {
      returnData: {}
    };
  },

}
</script>

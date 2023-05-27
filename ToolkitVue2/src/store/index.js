import Vue from 'vue'
import Vuex from 'vuex'

Vue.use(Vuex)

export default new Vuex.Store({
  state: {
    params: {},//表单应用数据
    track:{},//轨迹图需要数据
	webuser:""
  },
  getters: {
    // 参数列表state里的params数据
    getData(state) {
      return state.params;
    },
    // 参数列表state里的track数据
    getTrack(state) {
      return state.track;
    },
    getWebUser(state){
      return state.webuser;
    }
  },
  // 4. mutations
  mutations: {
    // state指的是state的数据
    // name传递过来的数据
    setData(state, data) {
      state.params = data;//将传参设置给state的city
    },
    setTrack(state, data) {
      state.track = data;//将传参设置给state的city
    },
    setWebUser(state,data){
      state.webuser = data;
    },
  },
  actions: {
    
  },
  modules: {
  }
})
/**
 * let params = this.$store.getters.getData; //获取数据
 * this.$store.commit('setData', params) //设置数据
*/
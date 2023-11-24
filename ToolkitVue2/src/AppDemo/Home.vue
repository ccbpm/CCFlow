<template>
  <el-container>
    <el-aside :style="isCollapse ? 'width: 64px;' : 'width: 200px;'">
      <!-- :default-active="$route.path" :collapse="false" background-color="#304156" text-color="#bfcbd9"
        :unique-opened="false" active-text-color="#409EFF" :collapse-transition="false" class="el-menu-vertical" -->
      <el-menu :default-active="$route.path" :collapse="isCollapse" background-color="#304156" text-color="#bfcbd9"
        :unique-opened="false" active-text-color="#409EFF" :collapse-transition="false" class="el-menu-vertical">
        <el-menu-item class="center">
          <el-image :src="
            isCollapse
              ? require('./Img/slogo.png')
              : require('./Img/logoHome.png')
          " style="display:flex;align-items:center;"></el-image>
        </el-menu-item>
        <el-submenu v-for="(item, index) in data" :index="(index + 1).toString()" :key="index">
          <template slot="title">
            <i :class="item.icon"></i>
            <span slot="title">{{ item.name }}</span>
          </template>
          <el-menu-item :index="'/' + itemList.path" v-for="(itemList, indexList) in item.list" :key="indexList"
            @click="skipClick(itemList.path, itemList.params)">
            <span class="pull-right side-ladge" v-if="itemList.count">{{
              itemList.count
            }}</span>
            <i :class="itemList.icon"></i>
            {{ itemList.name }}
          </el-menu-item>
        </el-submenu>
      </el-menu>
    </el-aside>
    <el-container>
      <el-header>
        <common-header @on-collapse="Collapse"></common-header>
      </el-header>
      <el-main>
        <keep-alive>
          <router-view />
        </keep-alive>
      </el-main>
    </el-container>
  </el-container>
</template>

<script>
import CommonHeader from "../wf/components/common-header.vue";

export default {
  name: "index",
  data() {
    return {
      isHomePage: false,
      isSystemManage: false,
      // isCollapse: false, //是否拉开
      isCollapse: true,
      btnIndex: "",
      menuList: "",
      defaultActive: "-1",
      data: [
        {
          name: "基础功能",
          icon: "el-icon-menu",
          list: [],
        },
        {
          name: "流程查询",
          icon: "el-icon-s-help",
          list: [
            {
              name: "我发起的",
              path: "mysend",
              icon: "el-icon-s-promotion",
              params: {
                EnsName: "BP.WF.Data.MyStartFlows",
              },
            },
            {
              name: "我参与的",
              path: "myjoin",
              icon: "el-icon-s-operation",
              params: {
                EnsName: "BP.WF.Data.MyJoinFlows",
              },
            },
          ],
        },
        {
          name: "接口API",
          icon: "el-icon-s-help",
          list: [
            {
              name: "工具包接口",
              path: "API",
              icon: "el-icon-s-promotion",
            },
          ],
        },
      ],
    };
  },

  created() {
    var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_AppClassic");
    var data = handler.DoMethodReturnString("Home_Init");
    if (data.indexOf("err@") == 0) {
      this.$message.error(data);
      console.log(data);
      return;
    }
    data = JSON.parse(data);
    this.data[0].list.push({
      name: "发起",
      path: "start",
      icon: "el-icon-position",
    });
    this.data[0].list.push({
      name: "待办",
      path: "todolist",
      icon: "el-icon-bell",
      count: data.Todolist_EmpWorks,
    });
    this.data[0].list.push({
      name: "在途",
      path: "runing",
      icon: "el-icon-time",
      count: data.Todolist_Runing,
    });
    this.data[0].list.push({
      name: "已完成",
      path: "complete",
      icon: "el-icon-circle-check",
      count: data.Todolist_Complete,
    });
    this.data[0].list.push({
      name: "查询",
      path: "searchZongHe",
      icon: "el-icon-search",
    });

    this.data[0].list.push({
      name: "草稿",
      path: "draft",
      icon: "el-icon-edit-outline",
      // eslint-disable-next-line no-mixed-spaces-and-tabs
      count: data.Todolist_Draft,
    });
    this.data[0].list.push({
      name: "抄送",
      path: "send",
      icon: "el-icon-edit",
      // eslint-disable-next-line no-mixed-spaces-and-tabs
      count: data.Todolist_CCWorks,
    });
    this.data[0].list.push({
      name: "批处理",
      path: "batch",
      icon: "el-icon-files",
      count: 0,
    });
    if (this.$route.name) {
      console.log('路由', this.$route.name);
      this.defaultActive = this.$route.name;
      this.isCollapse = false;
    }

  },
  methods: {
    // 跳转页面
    skipClick(path, params) {
      if (params != null && params != undefined)
        this.$store.commit("setData", params);
      this.$router.push({
        name: path,
      });
    },
    Collapse(data) {
      this.isCollapse = data;
    },
    onMenuChange(ev) {
      this.$router.push({ path: ev })
    },
  },
  components: {
    CommonHeader,
  },

  //监听
  // computed: {
  //   activeMenu() {
  //     const route = this.$route;
  //     const { meta, path } = route;
  //     // if set path, the sidebar will highlight the path you set
  //     if (meta.activeMenu) {
  //       return meta.activeMenu;
  //     }
  //     return path;
  //   },
  // },

  //监听后执行动作
  watch: {},

};
</script>

<style lang="less" scoped>
.main-side {
  background: #304156;
  border-right: solid 1px #e6e6e6;
}

.center {
  display: flex;
  justify-content: center;
  align-items: center;
  height:60px;
}

.el-menu-vertical-demo i {
  color: #fff !important;
}

.el-submenu__title i {
  color: #fff !important;
}

.side-ladge {
  background: #657a92;
  width: 20px;
  height: 20px;
  margin-top: 15px;
  border-radius: 50%;
  font-size: 0.65rem;
  line-height: 20px;
  text-align: center;
  color: #fff;
}

.el-menu {
  border-right: 0px;
}

.el-container {
  width: 100%;
  height: 100%;
  overflow: hidden;
}

.el-header {
  padding-right: 0;
  padding: 0;
  background-color: #367ea8;
}

.center {
  text-align: center;
  padding: 0px;
  padding-left: 0px !important;
  background-color: #409eff !important;
}

.el-menu {
  min-height: 100%;
}

.el-aside {
  overflow-x: hidden;
  scrollbar-width: none;
}

.el-aside::-webkit-scrollbar {
  display: none;
}

.pull-right {
  float: right !important;
}
</style>
<style>
.el-menu-vertical:not(.el-menu--collapse) {
  width: 200px;
  min-height: 100%;
}
</style>

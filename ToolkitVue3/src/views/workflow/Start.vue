<template>
  <div>
    <Row :gutter="[16, 16]">
      <Col :span="8" v-for="(type) in dataType" :key="type.FK_FlowSort">
      <Collapse v-model:activeKey="activeKey">
        <CollapsePanel :key="type.FK_FlowSort" :header="type.FK_FlowSortText">
          <div v-for="(start) in dataFlow" :key="start.No" class="set_Start">
            <div v-if="type.FK_FlowSort == start.FK_FlowSort">{{ start.No }}&nbsp;<a @click="StartFlow(start.No)">{{
              start.Name }}</a></div>
          </div>
        </CollapsePanel>
      </Collapse>
      </Col>
    </Row>
    <Drawer title="节点流程" placement="right" :closable="true" v-model:visible="visible" width="90%" @close="handleClose">
      <div style="width: 100%;height: 100%;">
        <iframe :src="url" scrolling="auto" frameborder="no" style="width: 100%; height: 100%" />
      </div>
    </Drawer>
  </div>
</template>
<script lang="ts" setup>
import { Collapse, CollapsePanel, Row, Col, Drawer } from 'ant-design-vue'
import { ref } from 'vue'
import { DB_Start } from '@/api/flow';
import { useUserStore } from '@/stores/user';
import { ccbpmPortURL } from '@/utils/env';
const dataType = ref<any>([]);  //类型
const dataFlow = ref<any>([]);  //发起流程
const userStore = useUserStore();
const activeKey = ref<string[]>([]);
const visible = ref<boolean>(false);
const url = ref();
//获取数据
const InitPage = () => {
  DB_Start().then((res) => {
    if (!res?.code) {
      dataFlow.value = res.sort(compare('No'));  //排序
      dataType.value = res.map((item: any) => {
        return {
          FK_FlowSort: item.FK_FlowSort,
          FK_FlowSortText: item.FK_FlowSortText
        }
      })
      let newArr = []; //索引
      for (let i = 0; i < dataType.value.length; i++) {
        if (newArr.indexOf(dataType.value[i]?.FK_FlowSort) == -1) {
          newArr.push(dataType.value[i]?.FK_FlowSort);
          activeKey.value.push(dataType.value[i]?.FK_FlowSort);
        } else {
          dataType.value.splice(i, 1);
          i--;
        };
      };
    }
  });
}
InitPage();
//排序
const compare = (property) => {
  return (a, b) => {
    var value1 = a[property];
    var value2 = b[property];
    return value1 - value2;
  }
}
//调用vue3Port来发起流程
const StartFlow = (flowNo: string) => {
  visible.value = true;
  url.value = ccbpmPortURL + `DoWhat=StartFlow&UserNo=${userStore.webUser.No}&FK_Flow=${flowNo}`;
  // window.open(url);
}
//关闭Drawer并刷新页面
const handleClose=(e:any)=>{
  console.log(e)
  location.reload()
}
</script>
<style lang="less">
.ant-drawer-header-title {
  flex-direction: row-reverse;
}

.ant-collapse {
  margin: auto;
  width: 95%;

  .set_Start {
    display: flex;
    justify-content: space-between;
  }
}
</style>
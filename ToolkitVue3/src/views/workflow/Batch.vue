<template>
  <div>
    <Collapse>
      <CollapsePanel key="1" header="批处理流程">
        <ul v-for="(item) in dataBatch" :key="item.NodeID">
          <li v-if="dataBatch.length > 0"><a @click="ToSelfUrl(item.NodeID)">{{ item.Name }}</a>
            <Badge :count="item.NUM" :number-style="{
              backgroundColor: '#eee',
              color: '#000',
              boxShadow: '0 0 0 1px #d9d9d9 inset',
            }" />
          </li>
          <li v-else>当前没有批处理的数据</li>
        </ul>
      </CollapsePanel>
    </Collapse>
    <Drawer title="批处理" placement="right" :closable="false" v-model:visible="visible" width="90%"
    @close="onClose">
      <div style="width: 100%;height: 100%;overflow: hidden;">
        <iframe :src="url" scrolling="auto" frameborder="no" style="width: 100%; height: 90%" />
        <Button style="margin-right: 8px;" class="btn_Back" @click="onClose">返回</Button>
      </div>
    </Drawer>
  </div>
</template>
<script lang="ts" setup>
import { Collapse, CollapsePanel, Badge, Drawer } from 'ant-design-vue';
import { Batch_Init } from '@/api/flow';
import { ccbpmWorkOpt } from '@/utils/env';
import { ref } from 'vue';
import { useRouter } from 'vue-router';

const dataBatch = ref<any>([]);
const visible = ref<boolean>(false);
const router = useRouter();

const InitPage = () => {
  Batch_Init().then((res) => {
    dataBatch.value = res
  })
}
InitPage();

const url = ref<string>('')
const ToSelfUrl = (NodeId: any) => {
  visible.value = true;
  //添加一个参数mode=tookitVue3用于判断页面返回时关闭此页面
  url.value = `${ccbpmWorkOpt}/Batch/WorkCheckModel.htm?FK_Node=${NodeId}&mode=tookitVue3`;
}
const onClose = () => {
  visible.value = false;
  router.go(0);
}
</script>
<style lang="less">
.btn_Back {
  float: right;
}
</style>

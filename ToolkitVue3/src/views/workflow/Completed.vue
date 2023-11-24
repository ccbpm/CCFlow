<template>
  <div class="allHeight">
    <Spin :spinning="loading" class="allHeight">
      <Table :data-source="dataSource" class="ant-table-striped" :columns="column" bordered :scroll="{ y: 600, x: 1410 }"
        :pagination="{ showTotal, onChange: handerGetCode }" :total="dataNum">
        <template #bodyCell="{ column, record }">
          <template v-if="column.dataIndex == 'Title'">
            <a @click="openFlow(record.FK_Flow, record.WorkID)">{{ record.Title }}</a>
          </template>
          <template v-else-if="column.dataIndex == 'WFState'">
            <div v-if="record.WFState == 2" style="color: green;">待办</div>
            <div v-else-if="record.WFState == 5" style="color: red;">退回</div>
            <div v-else-if="record.WFState == 1" style="color: orange;">草稿</div>
            <div v-else-if="record.WFState == 0">空白</div>
            <div v-else-if="record.WFState == 3" style="color: green;">已完成</div>
            <div v-else>状态码：{{ record.WFState }}</div>
          </template>
        </template>
      </Table>
    </Spin>
  </div>
</template>
<script lang="ts" setup>
import { useUserStore } from '@/stores/user';
import { ccbpmPortURL } from '@/utils/env';
import { Table, Spin } from 'ant-design-vue';
import type { ColumnType } from 'ant-design-vue/lib/table';
import { Flow_RecentWorkInit } from '@/api/flow';
import { ref } from 'vue';
import { useRoute } from 'vue-router';
const route = useRoute();
const scop = ref<string>('');
const loading = ref(false);
const dataSource = ref<any>([]);
const dataNum = ref<number>();
const userStore = useUserStore();
const pageCode = ref(0);
const currentSize = ref(10);
const column: ColumnType[] = [
  {
    title: ' # ',
    customRender: ({ index }) => {
      return pageCode.value*currentSize.value + index + 1;
    },
    width: 65,
    fixed: 'left',
  },
  {
    title: '标题',
    dataIndex: 'Title',
    key: 'Title',
    fixed: 'left',
    width: 327,
  }, {
    title: '发起人',
    dataIndex: 'StarterName',
    key: 'StarterName',
    width: 81,
  }, {
    title: '发起日期',
    dataIndex: 'RDT',
    key: 'RDT',
    width: 171,
  }, {
    title: '停留节点',
    dataIndex: 'NodeName',
    key: 'NodeName',
    width: 141,
  }, {
    title: '状态',
    dataIndex: 'WFState',
    key: 'WFState',
    width: 100,
  }, {
    title: '当前处理人',
    dataIndex: 'TodoEmps',
    key: 'TodoEmps',
    customRender: ({ value }) => {
      const names = value.split(';')
      const nameArr: string[] = [];
      names.forEach((item: string) => {
        nameArr.push(item.split(',')[1]);
      });
      let str = nameArr.join('，');
      str = str.slice(0, str.length - 1);
      return str;
    },
  }, {
    title: '应完成日期',
    dataIndex: 'SDT',
    key: 'SDT',
    width: 200,
  },
];
const showTotal = (dataNum: any) => {
  return `总计：${dataNum}条数据`
}
const handerGetCode = (pageNum: any, pageSize: any) => {
  pageCode.value = pageNum - 1;
  currentSize.value = pageSize;
}
const InitPage = () => {
  loading.value = true;
  scop.value = route.meta.scop as string;
  Flow_RecentWorkInit().then((res) => {
    dataSource.value = res;
    dataNum.value = dataSource.value.length;
    loading.value = false;
  })
};
const openFlow = (flowNo: string, WorkID: string) => {
  const url = ccbpmPortURL + `DoWhat=DealWork&UserNo=${userStore.webUser.No}&WorkID=${WorkID}&FK_Flow=${flowNo}`;
  window.open(url);
};

InitPage();
</script>
<style lang="less">
.ant-spin-nested-loading {
  max-height: 100%;

  .ant-spin-container {
    max-height: 100%;
  }
}

.allHeight {
  height: 100%;
}

.total {
  height: 40px;
  margin-top: 20px;
}
</style>
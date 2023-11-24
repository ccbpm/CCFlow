<template>
  <div class="allHeight">
    <Spin :spinning="loading" class="allHeight">
      <Table :data-source="dataSource" class="ant-table-striped" :columns="column" bordered :scroll="{ y: 600, x: 1410 }"
        :pagination="{ showTotal, onChange: handerGetCode }" :row-key="rowkey">
        <template #bodyCell="{ column, record }">
          <template v-if="column.dataIndex == 'Title'">
            <a @click="openToDo(record.FK_Flow, record.WorkID, record.FK_Node)">{{ record.Title }}</a>
          </template>
          <template v-else-if="column.dataIndex == 'WFState'">
            <div v-if="record.WFState == 0">空白</div>
            <div v-if="record.WFState == 1" style="color: orange">草稿</div>
            <div v-else-if="record.WFState == 2" style="color: green;">进行中</div>
            <!-- <div v-else-if="record.WFState == 3" style="color: green;">已完成</div> -->
            <div v-else-if="record.WFState == 4" style="color: green;">挂起</div>
            <div v-else-if="record.WFState == 5" style="color: red;">退回</div>
            <div v-else-if="record.WFState == 6" style="color: red;">移交</div>
            <div v-else-if="record.WFState == 7" style="color: red;">删除</div>
            <div v-else-if="record.WFState == 8" style="color: red;">加签</div>
            <div v-else>状态码：{{ record.WFState }}</div>
          </template>
        </template>
      </Table>
    </Spin>
  </div>
</template>
<script lang="ts" setup>
import { DB_Todolist } from '@/api/flow';
import { useUserStore } from '@/stores/user';
import { ccbpmPortURL } from '@/utils/env';
import { Table, Spin } from 'ant-design-vue';
import type { ColumnType } from 'ant-design-vue/lib/table';
import { ref } from 'vue';
const loading = ref(false);
const dataSource = ref<any>([]);
const userStore = useUserStore();
const pageCode = ref(0);
const currentSize = ref(10);
const column: ColumnType[] = [
  {
    title: ' # ',
    customRender: ({ index }) => {
      return pageCode.value* currentSize.value + index+ 1
    },
    fixed: 'left',
    width: 65,
  },
  {
    title: '标题',
    dataIndex: 'Title',
    key: 'Title',
    fixed: 'left',
    width: 250,
    ellipsis: true,
  }, {
    title: '流程',
    dataIndex: 'FlowName',
    key: 'FlowName',
    width: 100,
    fixed: 'left',
  }, {
    title: '节点',
    dataIndex: 'NodeName',
    key: 'NodeName',
    width: 120,
  }, {
    title: '状态',
    dataIndex: 'WFState',
    key: 'WFState',
    width: 80,
  }, {
    title: '发起人',
    dataIndex: 'StarterName',
    key: 'StarterName',
    width: 80,
  }, {
    title: '部门',
    dataIndex: 'DeptName',
    key: 'DeptName',
    width: 100,
  }, {
    title: '发起日期',
    dataIndex: 'RDT',
    key: 'RDT',
    width: 200,
  }, {
    title: '当前处理人',
    dataIndex: 'TodoEmps',
    key: 'TodoEmps',
    ellipsis: true,
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
//获取页码
const handerGetCode = (pageNum: any,pageSize: any) => {
  console.log(pageNum)
  console.log(pageSize)
  pageCode.value = pageNum - 1; //页码
  currentSize.value = pageSize; //一页包含行数
}
const showTotal = (dataNum: any) => {
  return `总计：${dataNum}条数据`
}
const rowkey = (record: any, index: any) => {
  console.log(record);
  return index
}
const InitPage = () => {
  loading.value = true;
  DB_Todolist().then((res) => {
    dataSource.value = res;
    console.log(dataSource.value)
    loading.value = false;
  });
};
const openToDo = (flowNo: string, WorkID: string, _NodeNo: string) => {
  const url = ccbpmPortURL + `DoWhat=DealWork&UserNo=${userStore.webUser.No}&WorkID=${WorkID}&FK_Flow=${flowNo}`;
  window.open(url);
}
InitPage();
</script>
<style lang="less" scoped>
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
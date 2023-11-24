<template>
  <div class="allHeight">
    <Spin :spinning="loading" class="allHeight">
      <Table :data-source="dataSource" class="ant-table-striped" :columns="column" bordered :scroll="{ y: 600, x: 1410 }"
        :pagination="{ showTotal, onChange: handerGetCode }">
        <template #bodyCell="{ column, record }">
          <template v-if="column.dataIndex == 'Title'">
            <a @click="openDraft(record.FK_Flow, record.WorkID)">{{ record.Title }}</a>
          </template>
          <template v-if="column.dataIndex == 'action'">
            <a @click="deleteDraft(record.WorkID)">删除</a>
          </template>
        </template>
      </Table>
    </Spin>
  </div>
</template>
<script lang="ts" setup>
import { DB_Draft, Flow_DeleteDraft } from '@/api/flow';
import { useUserStore } from '@/stores/user';
import { ccbpmPortURL } from '@/utils/env';
import { Table, Spin, message } from 'ant-design-vue';
import type { ColumnType } from 'ant-design-vue/lib/table';
import { ref } from 'vue';
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
      return index + 1;
    },
    width: 52,
    fixed: 'left',
  },
  {
    title: '标题',
    dataIndex: 'Title',
    key: 'Title',
    fixed: 'left',
    width: 327,
  }, {
    title: '流程',
    dataIndex: 'FlowName',
    key: 'FlowName',
    width: 200,
    fixed: 'left',
  }, {
    title: '日期',
    dataIndex: 'RDT',
    key: 'RDT',
    width: 171,
  }, {
    title: '操作',
    dataIndex: 'action',
    key: 'action',
    width: 100,
  }
];
const showTotal = (dataNum: any) => {
  return `总计${dataNum}条数据`
}
const handerGetCode = (pageNum: any, pageSize: any) => {
  pageCode.value = pageNum - 1;
  currentSize.value = pageSize;
}
const InitPage = () => {
  loading.value = true;
  DB_Draft().then((res) => {
    dataSource.value = res
    dataNum.value = dataSource.value.length;
    loading.value = false;
  });
};
const openDraft = (flowNo: string, WorkID: string) => {
  const url = ccbpmPortURL + `DoWhat=DealWork&UserNo=${userStore.webUser.No}&WorkID=${WorkID}&FK_Flow=${flowNo}`;
  window.open(url);
}
const deleteDraft = (WorkID: string,) => {
  const isDelete = confirm('确定删除吗？');
  if (isDelete) {
    Flow_DeleteDraft(WorkID).then((res) => {
      if (res?.code == 200) {
        message.success('删除成功');
        InitPage();
      }
    })
  }

}
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
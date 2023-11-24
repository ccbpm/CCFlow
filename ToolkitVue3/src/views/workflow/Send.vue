<template>
  <div>
    <div>
      <Input v-model:value="sendValue" placeholder="请输入流程名称，标题..." style="width: 200px;" />
      <Button type="primary" @click="searchValue(sendValue)" class="btn_style">
        <SearchOutlined />搜索
      </Button>
      <Button type="primary" @click="More" class="btn_style">
        <MoreOutlined />更多...
      </Button>
    </div>
    <div>
      <Table :columns="columns" :data-source="data">
        <template #name="{ text }">
          <a>{{ text }}</a>
        </template>
        <template #bodyCell="{ column, record }">
          <template v-if="column.dataIndex == 'Sta'">
            <div v-if="record.Sta == 0">
              <FolderOutlined /> 未读
            </div>
            <div v-else>
              <FolderOpenOutlined /> 已读
            </div>
          </template>
          <template v-if="column.dataIndex == 'Title'">
            <a @click="openSend(record.FK_Flow, record.WorkID, record.FK_Node, record.FID, record.Sta)">{{
              record.Title
            }}</a>
          </template>
        </template>
      </Table>
      <div class="total">总计：{{ dataNum }}条</div>
    </div>
    <Drawer title="抄送" placement="right" :closable="true" v-model:visible="visible" width="100%" @close="onClose" :headerStyle="{height:0}">
      <div style="width: 100%;height: 100%;overflow: hidden;">
        <iframe :src="url" scrolling="auto" frameborder="no" style="width: 100%; height: 93%" />
        <Button style="margin-right: 8px;" class="btn_Back" @click="onClose">返回</Button>
      </div>
    </Drawer>
  </div>
</template>
<script setup lang="ts">
import { Input, Button, Table, Drawer } from 'ant-design-vue';
import { SearchOutlined, MoreOutlined, FolderOutlined, FolderOpenOutlined } from '@ant-design/icons-vue';
import { ref } from 'vue';
import { DB_CCList } from '@/api/flow';
import { ccbpmURL } from '@/utils/env';
import { useRouter } from 'vue-router';
const sendValue = ref<string>('');
const dataNum = ref<number>();
const dataSource = ref<any>([]);
const visible = ref<boolean>(false);
const host = import.meta.env.VITE_GLOB_APP_URL;
const url = ref<string>('');
const router = useRouter();
const columns = [
  {
    title: '#',
    customRender: ({ index }) => {
      return index + 1;
    },
  },
  {
    title: '节点',
    dataIndex: 'NodeName',
    key: 'NodeName',
    width: 80,
  },
  {
    title: '状态',
    dataIndex: 'Sta',
    key: 'Sta',
  },
  {
    title: '抄送人',
    dataIndex: 'Rec',
    key: 'Rec',
  },
  {
    title: '标题',
    dataIndex: 'Title',
    key: 'Title',
  },
  {
    title: '日期',
    dataIndex: 'RDT',
    key: 'RDT',
  },
];
const InitPage = () => {
  DB_CCList().then((res) => {
    dataSource.value = res
    dataNum.value = res.length;
    data.value = dataSource.value //一开始就展示数据
  })
}
InitPage()
const data = ref([]);
//查询
const searchValue = (sendValue: string | null) => {
  if (sendValue == '' || sendValue == null) {
    data.value = dataSource.value
    dataNum.value = data.value.length;
    return dataNum.value
  } else {
    data.value = dataSource.value.filter((item: { Title: string | string[]; }) => {
      return item.Title.includes(sendValue)
    })
    dataNum.value = data.value.length;
  }
}
const openSend = (FK_Flow: any, WorkID: any, FK_Node: any, FID: any, Sta: any) => {
  visible.value = true;
  url.value = `${ccbpmURL}/#/WF/MyCC?WorkID=${WorkID}&FK_Node=${FK_Node}&FK_Flow=${FK_Flow}&FID=${FID}&CCSta=${Sta}`
}
//更多
const More = () => {
  const url = `${host}/WF/Comm/Search.htm?EnsName=BP.WF.Data.CCListExts`
  window.open(url);
}
//关闭抽屉
const onClose = () => {
  visible.value = false
  router.go(0);
}
</script>
<style lang="less">
.btn_style {
  margin: 0 5px;
}
.ant-table-striped :deep(.table-striped) td {
  background-color: #000;
}
.ant-drawer-body{
  padding: 5px 24px;
}
.btn_Back {
  float: right;
}
</style>

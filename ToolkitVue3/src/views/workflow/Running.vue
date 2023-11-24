<template>
  <div class="allHeight">
    <Spin :spinning="loading" class="allHeight">
      <div class="allHeight">
        <Table :data-source="dataSource" 
        class="ant-table-striped"
        :columns="column" 
        bordered 
        :scroll="{ y: 600 , x:1410}"
        :pagination="{ showTotal, onChange:handerGetCode}"
        >
          <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex == 'Title'">
              <a @click="openRunning(record.FK_Flow, record.WorkID, record.FK_Node)">{{ record.Title }}</a>
            </template>
            <template v-else-if="column.dataIndex == 'action'">
              <div style="display: flex;width: 100%;justify-content: space-evenly;">
                <Button type="primary" size="small" @click="doUnSend(record.WorkID)">撤销</Button>
                <Button type="primary" danger size="small" @click="doPress(record.WorkID)">催办</Button>
              </div>
            </template>
          </template>
        </Table>
      </div>
    </Spin>
  </div>
</template>
<script lang="ts" setup>
import { DB_Runing, Flow_DoPress, Flow_DoUnSend } from '@/api/flow';
import { useUserStore } from '@/stores/user';
import { ccbpmPortURL } from '@/utils/env';
import{ Table, Spin, Button, message } from 'ant-design-vue';
import type { ColumnType } from 'ant-design-vue/lib/table';
import { ref } from 'vue';
const loading = ref(false);
const dataSource = ref<any>([]);
const dataNum = ref<number>();
const userStore = useUserStore();
const pageCode = ref(0);
const currentSize = ref(10);
const column:ColumnType[] = [
  {
    title: ' # ',
    customRender:({index})=>{
      return pageCode.value*currentSize.value +index+1;
    },
    width:65,
  },
  {
    title: '标题',
    dataIndex: 'Title',
    key: 'Title',
    width:327,
  },{
    title: '流程名称',
    dataIndex: 'FlowName',
    key: 'FlowName',
    width: 200,
  },{
    title: '停留节点',
    dataIndex: 'NodeName',
    key: 'NodeName',
    width: 141,
  },{
    title: '发起人',
    dataIndex: 'StarterName',
    key: 'StarterName',
    width: 81,
  },{
    title: '部门',
    dataIndex: 'DeptName',
    key: 'DeptName',
    width: 96,
  },{
    title: '发起日期',
    dataIndex: 'RDT',
    key: 'RDT',
    width: 171,
  },{
    title: '当前处理人',
    dataIndex: 'TodoEmps',
    key: 'TodoEmps',
    customRender:({value})=>{
      const names = value.split(';')
      const nameArr:string[] = [];
      names.forEach((item:string) => {
        nameArr.push(item.split(',')[1]);
      });
      let str = nameArr.join('，');
      str = str.slice(0,str.length-1);
      return str;
    },
  },{
    title: '操作',
    dataIndex: 'action',
    key: 'action',
    fixed: 'right',
  }
];
const showTotal = (dataNum:any)=>{
  return `总计：${dataNum}条数据`
}
const handerGetCode=(pageNum:any,pageSize:any)=>{
  pageCode.value= pageNum - 1;
  currentSize.value= pageSize;
}
const InitPage = () =>{
  loading.value = true;
  DB_Runing().then((res)=>{
    dataSource.value=res
    dataNum.value = dataSource.value.length;
    loading.value = false;
  });
};
const openRunning = (flowNo: string, WorkID: string, NodeNo: string) => {
  const url = ccbpmPortURL + `DoWhat=MyView&UserNo=${userStore.webUser.No}&WorkID=${WorkID}&FK_Flow=${flowNo}&NodeID=${NodeNo}`;
  window.open(url);
}
const doUnSend = (workID:string) => {
  Flow_DoUnSend(workID).then((res)=>{
    if(res.code == 200 && !res.data.includes('err')){
      message.success('撤销成功');
    }else if(res.data.includes('err') || res.code === 500){
      message.error(res.data as string);
    }
    InitPage();
  })
}
//催办
const doPress = (workID:string) => {
  const msg = prompt('请输入催办信息','此工作需要您尽快处理.');
  if(msg == null){
    return;
  }else{
    Flow_DoPress(workID, msg).then((res:any)=>{
    if(res.code == 200 && !res.data.includes('info')){
      const messageData = res.data.slice(1,res.data.length-1);
      message.success(messageData);
    }else if(res.data.includes('info') || res.code === 500){
      message.error(res.data as string);
    }
    InitPage();
  })
  }
  
}
InitPage();
</script>
<style lang="less">
.ant-spin-nested-loading{
  max-height: 100%;
  .ant-spin-container{
    max-height: 100%;
  }
}
.allHeight{
  height: 100%;
}
.total{
  height: 40px;
  margin-top: 20px;
}
</style>
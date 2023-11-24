<template>
  <div class="allHeight">
    <Spin :spinning="loading" class="allHeight">
      <Form layout="inline" style="margin-bottom: 20px;">
        <FormItem label="关键字">
          <Input v-model:value="key" placeholder="请输入关键字" allowClear :width="200"/>
        </FormItem>
        <FormItem label="发起日期"> 
          <RangePicker v-model:value="RDT" :locale="locale"/>
        </FormItem>
        <FormItem>
          <Button type="primary" @click="InitPage">查询</Button>
          <Button  @click="Reset" style="margin-left: 10px;">重置</Button>
        </FormItem>
      </Form>
      <Table :data-source="dataSource" 
        class="ant-table-striped"
        :columns="column" 
        bordered 
        :scroll="{ y: 600 ,x:1410}"
        :pagination="pagination"
        :total="dataNum"
        >
        <template #bodyCell="{ column, record }">
          <template v-if="column.dataIndex == 'Title'">
            <a @click="openWork(record.FK_Flow, record.WorkID)">{{ record.Title }}</a>
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
      <!-- <div class="total">总计：{{ dataNum }}条</div> -->
    </Spin>
  </div>
</template>
<script lang="ts" setup>
import { useUserStore } from '@/stores/user';
import { ccbpmPortURL } from '@/utils/env';
import{ Table,Spin, Button, Input,Form , FormItem, RangePicker} from 'ant-design-vue';
import type { ColumnType } from 'ant-design-vue/lib/table';
import { Search_Init } from '@/api/flow';
import { computed, ref } from 'vue';
import { useRoute } from 'vue-router';
import locale from 'ant-design-vue/es/date-picker/locale/zh_CN';
import 'dayjs/locale/zh-cn';
import dayjs, {Dayjs} from 'dayjs';
import { watch } from 'vue';
type RangeValue = [Dayjs, Dayjs];
const route = useRoute();
const scop = ref<string>('');
  const loading = ref(false);
const dataSource = ref<any>([]);
const dataNum = ref<number>();
const userStore = useUserStore();
const column:ColumnType[] = [
  {
    title: ' # ',
    customRender:({index})=>{
      return index+1;
    },
    width:52,
    fixed:'left',
  },
  {
    title: '标题',
    dataIndex: 'Title',
    key: 'Title',
    fixed:'left',
    width:327,
  },{
    title: '发起人',
    dataIndex: 'StarterName',
    key: 'StarterName',
    width: 81,
  },{
    title: '发起日期',
    dataIndex: 'RDT',
    key: 'RDT',
    width: 171,
  },{
    title: '停留节点',
    dataIndex: 'NodeName',
    key: 'NodeName',
    width: 141,
  },{
    title: '状态',
    dataIndex: 'WFState',
    key: 'WFState',
    width: 100,
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
    title: '应完成日期',
    dataIndex: 'SDT',
    key: 'SDT',
    width: 200,
  },
];
const key = ref('');
const RDT = ref<RangeValue>();
const current = ref<number>(1);
const pagination = computed(() => ({
      total: dataNum.value,
      current: current.value,
      pageSize:20,
      onChange:(page:number)=>{
        pagination.value.current = page;
        InitPage();
    },
    }));
const InitPage = () => {
  loading.value = true;
  scop.value = route.meta.scop as string;
  let Dfrom = '';
  let Dto = '';
  if(RDT.value != undefined && RDT.value != null){
    Dfrom = dayjs(RDT.value[0]).format('YYYY-MM-DD');
    Dto = dayjs(RDT.value[1]).format('YYYY-MM-DD');
  }
  Search_Init(key.value,Dfrom,Dto, scop.value,pagination.value.current).then((res)=>{
  
    dataSource.value = res?.gwls;
    dataNum.value = parseInt(res?.count[0].CC);
    loading.value = false;
  })
};
const openWork = (flowNo: string, WorkID: string) => {
  const url = ccbpmPortURL + `DoWhat=DealWork&UserNo=${userStore.webUser.No}&WorkID=${WorkID}&FK_Flow=${flowNo}`;
  window.open(url);
};
const Reset = () => {
  current.value = 1;
  pagination.value.current = 1;
  RDT.value = undefined;
  key.value='';
  InitPage();
}
InitPage();
// 简单数据类型监听
watch(route, () => {
  key.value = '';
  RDT.value = undefined;
  InitPage();
}, {
  immediate: true//立即监听--进入就会执行一次
})
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
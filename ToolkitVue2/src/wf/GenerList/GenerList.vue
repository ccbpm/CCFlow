<template>
  <div class="p-4" style="height:100%">
    <Spin :spinning="loading" style="background-color: white;margin:0px;height:100%">
      <div v-if="errorObj.hasError" class="ant-tag-red">
        {{ errorObj.tips }}

      </div>
      <div v-else>
        <Card style="border-radius: 10px; margin-bottom: 12px">
          <div class="flex">
            <Form layout="inline" :label-col="{ span: 8 }" :wrapper-col="{ span: 16 }">
              <FormItem label="关键字">
                <Input v-model:value="searchKey" placeholder="请输入关键字" @change="Search" allow-clear/>
              </FormItem>
              <template v-if="showRDT">
                <FormItem :label="dtFieldOfSearchLab">
                  <RangePicker v-model:value="searchDt" format="YYYY-MM-DD" @change="Search"/>
                </FormItem>
              </template>
              <FormItem>
                <Button type="primary" @click="Search"> 查询</Button>
              </FormItem>

              <template v-for="btn in toolBars">
                <FormItem>
                  <Button type="primary" @click="toolBarOper(btn)">{{ btn }}</Button>
                </FormItem>
              </template>
            </Form>
          </div>
        </Card>
        <BasicTable v-if="showModel === 0" @register="registerTable" :pagination="defalutPageSize!==0?pagination:false" ref="tableRef">
          <template #action="{ record, column }">
            <TableAction :actions='createActions(record, column)'/>
          </template>
        </BasicTable>
        <div v-else-if="showModel === 1">
          <template v-if="defaultGroupField!=''">
            <div v-for="item in tableData" style="background-color: white;padding:10px">
              <div class="group_title">{{ item[columns[0].key] }}</div>
              <Row :gutter="[8,8]">
                <Col :span="8" v-for="child in item.children">
                  <Card>
                    <div v-for="column in columns">
                      <span>{{ column.title }}:</span>
                      <span>
                            <template v-if="column.key ===linkField">
                                  <Button type="link" @click="LinkFieldClick(child)">{{ child[column.key] }}</Button>
                            </template>
                            <template v-else-if="labFields.includes(column.key)&&child[column.key]">
                               <template v-for="tag in GetTextTags(child[column.key])">
                                  <Tag :color="tag.color" style="margin-right:0.5em"> {{ tag.name }}</Tag>
                               </template>
                            </template>
                            <template v-else>
                              <span v-html="child[column.key]"></span>
                            </template>
                          </span>
                    </div>
                    <template #actions>
                      <template v-for="action in createActions(child, columns)">
                        <Button style="border-right:1px solid #ccc" type="link" @click="BtnsOfRowOper(child,columns,action.label)">{{ action.label }}</Button>
                      </template>
                    </template>
                  </Card>
                </Col>
              </Row>
            </div>
          </template>
          <template v-else>
            <Row :gutter="[8,8]">
              <Col :span="8" v-for="child in item.children">
                <Card>
                  <div v-for="column in columns">
                    <span>{{ column.title }}:</span>
                    <span>
                          <template v-if="column.key ===linkField">
                                <Button type="link" @click="LinkFieldClick(child)">{{ child[column.key] }}</Button>
                          </template>
                          <template v-else-if="labFields.includes(column.key)&&child[column.key]">
                             <template v-for="tag in GetTextTags(child[column.key])">
                                <Tag :color="tag.color" style="margin-right:0.5em"> {{ tag.name }}</Tag>
                             </template>
                          </template>
                          <template v-else-if="column.format!=undefined && column.format!=''">
                            <span v-html="glEn.BindFieldFunction(column.format, child)"></span>
                          </template>
                          <template v-else>
                            <span v-html="child[column.key]" style="display:inline"></span>
                          </template>
                      </span>
                  </div>
                </Card>
              </Col>
            </Row>
          </template>
        </div>
        <div v-else-if="showModel === GenerListPageShowModel.Windows"  style="height:calc(100vh - 150px);overflow-y:auto">
          <template v-if="defaultGroupField!=''">
            <div v-if="tableData.length==0">
              <Empty :image="simpleImage" style="max-width:100%;padding-top:26px"></Empty>
            </div>
            <Row v-else :gutter="[8,8]">
              <Col :span="8" v-for="item in tableData">
                <Collapse expandIconPosition="right" v-model:activeKey="item[columns[0].key]">
                  <CollapsePanel :key="item[columns[0].key]" :header="item[columns[0].key]" collapsible="disabled">
                    <Row type="flex" justify="space-between" v-for="child in item.children" style="padding-top:5px">
                      <Col :span="21" style="overflow: hidden;text-overflow: ellipsis;white-space: nowrap;">
                        <Button type="link" @click="LinkFieldClick(child)" style="color:#096dd9">{{ child[linkField] }}</Button>
                      </Col>
                      <Col :span="2">
                        <Popover placement="bottomRight" arrow-point-at-center trigger="click" :visible="child.visible"
                                 @visibleChange="(visible)=>child.visible =visible ">
                          <template #content>
                            <template v-for="action in createActions(child, columns[0])">
                              <Button type="link" @click="child.visible = false;BtnsOfRowOper(child,columns[0],action.label)">{{ action.label }}</Button>
                              <br/>
                            </template>
                          </template>
                          <Button shape="circle" size="small" @click="child.visible = true">
                            <ToolOutlined style="color:#d3c4c4cc"/>
                          </Button>
                        </Popover>
                      </Col>
                    </Row>
                  </CollapsePanel>
                </Collapse>
              </Col>
            </Row>
          </template>
          <template v-else></template>
        </div>
        <div v-else-if="showModel === GenerListPageShowModel.BigIcon">
          <List>
            <Row :gutter="16">
              <Col :span="6" v-for="item in tableData">
                <ListItem>
                  <Card :hoverable="true" class="list-card__card" @click="LinkFieldClick(item)">
                    <div class="list-card__card-title">
                      <i :class="item.Icon"/>
                      <br/>
                      {{ item[linkField] }}
                    </div>
                  </Card>
                </ListItem>
              </Col>
            </Row>
          </List>
        </div>
        <Modal
          v-model:visible="modal.visible"
          centered
          :title="modal.title"
          width="70%"
          :body-style="{ height: '600px' }"
          :footer="null"
        >
          <template v-if="modal.visible">
            <iframe v-if="modal.iframeURL" :src="modal.iframeURL" class="modal-iframe">
            </iframe>
            <Component v-else :is="modal.component" :params="modal.params" @close-self="modalClose"/>
          </template>

        </Modal>
        <!--右侧滑出-->
        <Drawer :visible="drawer.visible" :title="drawer.title" width="70%" :body-style="{ padding: 0 }" @close="drawerClose">
          <template v-if="drawer.visible">
            <iframe v-if="drawer.iframeURL" :src="drawer.iframeURL" class="modal-iframe">
            </iframe>
            <Component v-else :is="drawer.component" :params="drawer.params"/>
          </template>

        </Drawer>
      </div>
    </Spin>
  </div>
</template>
<script lang="tsx" setup>
import {reactive, ref, nextTick, shallowRef, h, UnwrapRef, markRaw, onMounted, onUnmounted} from 'vue';
import {ClassFactoryOfGenerList} from '../GenerList/ClassFactoryOfGenerList';
import {
  Form,
  FormItem,
  Input,
  RangePicker,
  Button,
  Spin,
  message,
  Tag,
  Modal,
  Drawer,
  Row,
  Col,
  Card,
  Collapse,
  CollapsePanel,
  Popover,
  List,
  ListItem,
  Empty
} from 'ant-design-vue';
import {useRoute, useRouter} from 'vue-router';
import {ActionItem, BasicColumn, BasicTable, EditRecordRow, TableAction, TableActionType, useTable} from '/@/components/Table';
import type {Dayjs} from 'dayjs';
import {GPNReturnType} from "/@/bp/UIEntity/PageBaseGroupNew";
import useComponentLoader from "/@/hooks/ens/useComponentLoader";
import {GenerListPageShowModel} from '/@/bp/UIEntity/PageBaseGenerList';
import {ToolOutlined} from '@ant-design/icons-vue';
import {useTabs} from '/@/hooks/web/useTabs';
import {useRedo} from "/@/hooks/web/usePage";
import {usePostMessage} from "/@/hooks/message/usePostMessage";
import {ccbpm} from "/#/ccbpm";
import {MessageTypeEnum} from "/@/enums/messageTypeEnum";
import {splitAtString} from "/@/bp/tools/ParamUtils";

type RangeValue = [Dayjs, Dayjs];
const route = useRoute();
const searchKey = ref("");
const searchDt = ref<RangeValue>();

const props = defineProps({
  params: {
    type: Object,
    default: () => {
      return {}
    }
  }
})

//错误提示
const errorObj = reactive({
  hasError: false,
  tips: '',
});
const simpleImage = Empty.PRESENTED_IMAGE_SIMPLE;
const loading = ref(false);
const router = useRouter();
//实体类
const glEn = ref(); // Proxy => .value 才是实际的对象，不加.value 是Proxy是代理对象
//实体类对应的EnName
const enName = ref<UnwrapRef<string>>("");
const innerColumns = ref<any[]>([]);
const innerData = ref<Recordable[]>([]); //查询的结果数据集合
const parseData = ref<Recordable[]>([]); //处理后的数据结果集合

//table集合的定义
let columns: BasicColumn[] = []; //对应的列名
const tableData = ref([]);//数据集合
const labFields = ref("");//标签显示
const defalutPageSize = ref(0);
const tableHeight=ref(document.body.clientHeight-267);


//查询条件
const showRDT = ref(false);// 是否显示时间字段的查询条件
const dtFieldOfSearchLab = ref("");//时间查询lab
const dtFieldOfSearch = ref("");//时间查询字段名

//分组
const groupFields = ref<string[]>([]);//分组集合
const defaultGroupField = ref("");//默认的分组字段

//操作
const linkField = ref("")//点击超链接处理的字段
const btnsOfRow = ref<string[]>([]);

const hovered = ref(false);

let showModel = ref(0);
//工具栏操作
const toolBars = ref();
const {loadComponent, getComponentParamsByUrl} = useComponentLoader();
//弹窗信息定义
const modal = reactive({
  visible: false,
  component: {},
  params: {},
  title: '',
  iframeURL: '',
});
const drawer = reactive({
  visible: false,
  component: {},
  params: {},
  title: '',
  iframeURL: '',
});

const modalClose = async (type = false) => {
  if (type) {
    await InitPage();
  }
  modal.visible = false;
}

const tableRef = ref<Nullable<TableActionType>>();
//分页信息
let pagination = reactive({
  current: 1,
  pageSize: 10,
  total: 0,
  defalutPageSize: 10,
  pageSizeOptions: ['10', '15', '20', '50'],
  showSizeChanger: true,
  showQuickJumper: true,
  onChange: (current, size) => {
    pagination.current = current;
    pagination.pageSize = size;
    tableData.value = [];
    tableParseData();
    tableRef?.value?.setTableData(tableData.value);
    if (groupFields.value.length > 0)
      onExpandAll();
  },
  onShowSizeChange: (current, pageSize) => {
    pagination.current = 1;
    pagination.pageSize = pageSize;
    tableData.value = [];
    tableParseData();
    tableRef?.value?.setTableData(tableData.value);
    if (groupFields.value.length > 0)
      onExpandAll();
  },
});

const {setTitle} = useTabs();

const InitPage = async () => {
  try {
    loading.value = true;
    enName.value = props.params?.EnName || route.query.EnName as string || "";
    //获得实体.
    const entity = await ClassFactoryOfGenerList.GetEn(enName.value);
    entity.setParams(props.params || {})

    await entity.Init(); //获得数据.

    innerColumns.value = entity.Columns;
    // xxx
    // entity.Data;
    // entity.Columns;

    glEn.value = entity;

    document.title = glEn.value.PageTitle;

    showModel.value = entity.HisGLShowModel;

    //初始化值
    dtFieldOfSearchLab.value = glEn.value.DTFieldOfLabel;
    dtFieldOfSearch.value = glEn.value.DTFieldOfSearch;
    //分组字段集合
    const gfs = glEn.value.GroupFields || "";
    groupFields.value = gfs == "" ? [] : gfs.split(",");
    //默认分组字段
    defaultGroupField.value = glEn.value.GroupFieldDefault || "";
    if (defaultGroupField.value == "" && groupFields.value.length > 0)
      defaultGroupField.value = groupFields.value[0];

    linkField.value = glEn.value.LinkField;
    let opers = glEn.value.BtnsOfRow || "";
    opers = opers.replace(/，/g,',');
    btnsOfRow.value = opers == "" ? [] : opers.split(",")
    labFields.value = glEn.value.LabFields || "";

    toolBars.value = glEn.value.BtnOfToolbar == '' ? [] : glEn.value.BtnOfToolbar.split(",")


    innerData.value = glEn.value.Data;
    parseData.value = innerData.value;
    defalutPageSize.value = glEn.value.PageSize;
    pagination.pageSize = defalutPageSize.value;

    tableParseColumns()
    tableParseData()
    if (glEn.value.HisGLShowModel == GenerListPageShowModel.Table){
      tableRef.value?.setPagination(pagination);
    }
    await setTitle(entity.PageTitle || '');
  } catch (e) {
    errorObj.hasError = true;
    errorObj.tips = e as string;
  } finally {
    loading.value = false;
  }
}

function covertItemWidthToPercent() {
  const data = columns;
  for (const item of data) {
    if (typeof item.width === 'string' && item.width.includes('%')) {
      return
    }
    if (typeof item.width !== 'number') {
      item.width = 150;
    }
  }
  const totalWidth = data.reduce((prev: any, curr: any) => prev.width + curr.width, 0)
  for (const item of data) {
    item.width = (item.width || 0 / totalWidth) + '%';
  }
  columns = data
}

//处理显示的列
function tableParseColumns() {
  let index = 0;
  let firstKey = "";
  innerColumns.value.forEach(item => {
    //存在时间查询条件，且字段在查询列中
    if (dtFieldOfSearch.value != "" && item.Key === dtFieldOfSearch.value)
      showRDT.value = true;
    const isShow = item.IsShow == undefined ? true : item.IsShow;
    if (isShow) {
      if (index == 0)
        firstKey = item.Key;
      if (item.Key != defaultGroupField.value) {
        //判断是否属于分组的
        const isBoolean = groupFields.value.some((gf) => gf == item.Key);
        if (isBoolean) {
          columns.push({
            title: item.Name,
            dataIndex: item.Key,
            key: item.Key,
            align: index == 0 && groupFields.value.length != 0 ? "left" : "center",
            width: item.width == undefined ? 100 : item.width,
            format: item.RefFunc || "",
            customHeaderCell: (column) => {
              return {
                style: {
                  color: '#0960bd',
                  cursor: 'pointer',
                },
                onClick: () => {
                  ChageGroupField(column);
                },
              };
            },

          })
          index++;
        } else {
          columns.push({
            title: item.Name,
            dataIndex: item.Key,
            key: item.Key,
            //ellipsis:true,
            align: index == 0 && groupFields.value.length != 0 ? "left" : "center",
            width: item.width == undefined ? 100 : item.width,
            customCell: (record, rowIndex, column) => {
              if (record.children == undefined)
                return {colSpan: 1};
              if (record.children != undefined) {
                if (column && column.key === columns[0].key)
                  return {colSpan: columns.length};
                else
                  return {colSpan: 0};
              }
            },
            customRender: ({text, record, index, column}) => {
              //标题增加已读未读，超链接
              if (column.key == "Title" && record.children == undefined && text != undefined && text != "") {
                if (column.key == linkField.value) {
                  if (record.IsRead == 1)
                    return <a onClick={LinkFieldClick.bind(null, record)}><i class="icon-envelope-open"></i>{record[linkField.value]}</a>;
                  if (record.IsRead == undefined || record.IsRead == 0)
                    return <a onClick={LinkFieldClick.bind(null, record)}><i class="icon-envelope"></i>{record[linkField.value]}</a>;
                }
                if (record.IsRead == 1)
                  return <span><i class="icon-envelope-open"></i>{record[linkField.value]}</span>;
                if (record.IsRead == undefined || record.IsRead == 0)
                  return <span><i class="icon-envelope"></i>{record[linkField.value]}</span>;
              }
              //超链接
              else if (column.key == linkField.value && record.children == undefined) {
                return <a onClick={LinkFieldClick.bind(this, record)}>{record[linkField.value]}</a>;
              }
              //获取标签的值对应的数组集合
              else if (text != undefined && labFields.value.includes(column.key) == true && record.children == undefined) {
                const tags: TagExt[] = GetTextTags(text);
                if (tags.length == 0)
                  return text;
                if (tags.length == 1)
                  return h(Tag, {color: tags[0].color}, () => tags[0].name)
                if (tags.length == 2)
                  return (
                    <span>
                      <Tag color={tags[0].color} style="margin-right:0.5em"> {tags[0].name}</Tag>
                      <Tag color={tags[1].color}> {tags[1].name}</Tag>
                    </span>

                  )
              } else if (typeof text == "string") {
                return (<span v-html={text}></span>)
              } else
                return text
            },

          });
          index++;
        }
      }
    }

  })
  covertItemWidthToPercent();
}

//处理数据Data
const tableParseData = () => {
  const startIdx = ref(0);
  const endIdx = ref(0);
  const curretPageData = ref();
  if (defalutPageSize.value != 0) {
    pagination.total = parseData.value.length;
    //处理前台逻辑分页问题
    startIdx.value = (pagination.current - 1) * pagination.pageSize;
    endIdx.value = pagination.current * pagination.pageSize;
    if (parseData.value.length < endIdx.value)
      endIdx.value = parseData.value.length;
    curretPageData.value = parseData.value.slice(startIdx.value, endIdx.value);
  } else {
    curretPageData.value = parseData.value
  }
  curretPageData.value.forEach(item => item.visible = false);
  //不存在分组直接显示数据
  if (groupFields.value.length == 0) {
    tableData.value = curretPageData.value;
    if (glEn.value.HisGLShowModel == GenerListPageShowModel.Table)
      tableRef.value?.setTableData(tableData.value);
  } else {
    //获取分组的集合
    const map = new Map()
    curretPageData.value.forEach((item, index, arr) => {
      if (!map.has(item[defaultGroupField.value])) {
        map.set(
          item[defaultGroupField.value],
          arr.filter(a => a[defaultGroupField.value] == item[defaultGroupField.value])
        )
      }
    })
    const data = Array.from(map).map(item => [...item[1]])
    let dataItem: Record<string, any> = {};
    const treeKey = columns[0]?.key || 'Title';
    data.forEach((item, index) => {
      dataItem = {};
      dataItem[treeKey] = item[0][defaultGroupField.value];
      dataItem['children'] = item;
      tableData.value.push(dataItem as never);
    });
    if (groupFields.value.length > 0)
      onExpandAll();
  }


}
InitPage();

const [registerTable] = useTable({
  title: '',
  isTreeTable: true,
  columns: columns,
  dataSource: tableData,
  showIndexColumn: false,
  showTableSetting: false,
  canResize:true,
  tableSetting: {fullScreen: false},
  defaultExpandAllRows: true,
  expandRowByClick: true,
  scroll:{y: tableHeight.value},
  actionColumn: {
    width: 150,
    title: '操作',
    dataIndex: 'action',
    slots: { customRender: 'action' },
    ifShow: () => {
      // debugger
      if(innerColumns.value.filter(item=>item.Key==='Btns').length>0)
        return true; // 根据业务控制是否显示
      return false;
    },
  },
});

function onExpandAll() {
  // 演示默认展开所有表项
  nextTick(tableRef.value?.expandAll);
}

function createActions(record: EditRecordRow, column: BasicColumn): ActionItem[] {
  debugger
  const items:Record<string, any>[]=[];
  let opers = record['Btns'];
  if(typeof opers === 'undefined' || opers==='')
    return [];
  opers = opers.replace(/，/g,',');
  const btns = opers.split(',');
  btns.forEach(item => {
    if(item!=''){
      items.push({
        label: item,
        ifShow: (_action) => {
          return record.children == undefined;
        },
        onClick: BtnsOfRowOper.bind(null, record, column, item),
      })
    }

  })
  return items;

  if (btnsOfRow.value.length == 0)
    return [];
  else {
    btnsOfRow.value.forEach(item => {
      items.value.push({
        label: item,
        ifShow: (_action) => {
          return record.children == undefined;
        },
        onClick: BtnsOfRowOper.bind(null, record, column, item),
      })
    })
    return items.value;
  }
}

/**
 * 行按钮操作
 * @param record
 * @param column
 * @param name
 * @constructor
 */
function BtnsOfRowOper(record, column, name) {
  const result = glEn.value.BtnClick(name, record);
  if (!!result)
    afterOper(name, result);
}

/**
 * 工具栏操作
 * @param name
 */
async function toolBarOper(name) {
  const result = await glEn.value.BtnClick(name, null);
  if (!!result)
    afterOper(name, result);
}

/**
 * 执行点击超链接事件
 * @param record
 * @constructor
 */
const LinkFieldClick = async (record) => {
  const result = await glEn.value.LinkFieldClick(record);
  if (!!result)
    await afterOper(record[linkField.value], result);
}

//执行完的操作
async function afterOper(btnName, result) {
  if (result.data == undefined || result.data == '')
    return;
  switch (result.ReturnType) {
    case GPNReturnType.Message:
      message.info(result.data);
      break;
    case GPNReturnType.Error:
      message.error(result.data);
      break;
    case GPNReturnType.GoToUrl://转到url.
      if(result.data && result.data.includes('Middle')){
        //在Tab页打开

      }
      window.location.replace(result.data)
      break;
    case GPNReturnType.Close://关闭.
      break;
    case GPNReturnType.CloseAndReload://关闭并重载
      break;
    case GPNReturnType.Reload://刷新
      await InitPage();
      break;
    case GPNReturnType.ReBind://重新绑定
      innerData.value = result.data;
      parseData.value = result.data;
      Search();
      break;
    case GPNReturnType.OpenUrlByDrawer: //重新绑定
      if (result.data.startsWith('/#/')) {
        modal.iframeURL = result.data;
        modal.title = btnName;
        modal.visible = true;
        return;
      }
      const param = result.data.split('?');
      if (param.length > 1) {
        const compName = param[0].endsWith('.vue') ? param[0] : param[0] + '.vue';
        modal.component = markRaw(loadComponent(compName));
        modal.params = getComponentParamsByUrl(result.data.substring(5));
        modal.title = btnName;
        modal.visible = true;
      }
      break;
    case GPNReturnType.OpenUrlByDrawer75: //抽屉的模式打开.
    case GPNReturnType.OpenUrlByDrawer90: //抽屉的模式打开.
    case GPNReturnType.OpenUrlByDrawer30: //抽屉的模式打开.
      if (result.data.startsWith('/#/')) {
        drawer.iframeURL = result.data;
        drawer.title = btnName;
        drawer.visible = true;
        return;
      }
      const params = result.data.split('?');
      if (params.length > 1) {
        const compName = params[0].endsWith('.vue') ? params[0] : params[0] + '.vue';
        drawer.component = markRaw(loadComponent(compName));
        drawer.params = getComponentParamsByUrl(result.data);
        drawer.title = btnName;
        drawer.visible = true;
      }
      break;
    case GPNReturnType.OpenUrlByNewWindow://重新绑定
      window.open(result.data);
      break;
    case GPNReturnType.DoNothing: //重新绑定
      break;
    default:
      message.warning('类型:' + result.ReturnType + '还未解析');
      break;
  }

}

const drawerClose = () => {
  drawer.visible = false;
}
/**
 * 改变分组
 * @param field 分组字段
 * @constructor
 */
const ChageGroupField = (column) => {
  defaultGroupField.value = column.key;
  columns = [];
  tableData.value = [];
  tableParseColumns();
  tableParseData();
  tableRef.value?.setColumns(columns);
  tableRef.value?.setTableData(tableData.value);
  if (groupFields.value.length > 0)
    onExpandAll();
}

//执行查询
const Search = () => {
  tableData.value = [];
  parseData.value = [];
  //匹配关键字段值
  const list = ref<Recordable[]>([])
  if (searchKey.value) {
    const ar = JSON.parse(JSON.stringify(innerData.value));
    const str = new RegExp(searchKey.value, 'i');
    ar.forEach((item, index) => {
      for (const key in item) {
        if (str.test(item[key])) {
          list.value.push(item);
          break;
        }
      }
    });
  } else {
    list.value = innerData.value;
  }
  const dtFrom = searchDt.value?.[0] || "";
  const dtTo = searchDt.value?.[1] || "";
  if (showRDT.value == true) {

    //开始时间和结束时间同时有值时
    if (dtFrom != "" && dtTo != "") {//+new Date()目的是将数据类型转换为Number类型
      list.value.forEach((item, index) => {
        if (+new Date(item[dtFieldOfSearch.value]) >= +dtFrom.toDate()
          && +new Date(item[dtFieldOfSearch.value]) <= +dtTo.toDate()) {
          parseData.value.push(item);
        }
      });
    } else if (dtFrom != "") {
      list.value.forEach((item, index) => {
        if (+new Date(item[dtFieldOfSearch.value]) >= +dtFrom.toDate()) {
          parseData.value.push(item);
        }
      });
    } else if (dtTo != "") {
      list.value.forEach((item, index) => {
        if (+new Date(item[dtFieldOfSearch.value]) <= +dtTo.toDate()) {
          parseData.value.push(item);
        }
      });
    } else {
      parseData.value = list.value
    }
  } else {
    parseData.value = list.value
  }
  pagination.current = 1;
  tableParseData();
  if (glEn.value.HisGLShowModel == GenerListPageShowModel.Table)
    tableRef.value?.setTableData(tableData.value);
  if (groupFields.value.length > 0)
    onExpandAll();
}

/**
 * 获取标签表示的内容
 * @param text
 * @constructor
 */
interface TagExt {
  name: string,
  color: string
}

function GetTextTags(text) {
  const tags = ref<TagExt[]>([]);
  splitAtString(text).forEach(item => {
    tags.value.push({
      name: item.split("=")[0],
      color: item.split("=")[1]
    })
  })
  return tags.value;
}

const {refreshPage} = useTabs();
const messageHandler = (evt) => {
  const data = evt.data as ccbpm.PostMessageInfo;
  console.log(data)
  if (data.type === MessageTypeEnum.ReloadPage) {
    if (window.location.hash.includes("/Middle/GenerList")) {
      drawer.visible = false;
      modal.visible = false;
      tableData.value = [];
      parseData.value = [];
      columns = [];
      InitPage();
    } else {
      loading.value = true;
      refreshPage();
    }
  }
}
usePostMessage(messageHandler)


</script>
<style lang="less" scoped>
:deep(tr.unIsread td:first-child) {
  font-weight: 600;
}

:deep(.ant-table-thead > tr > th) {
  text-align: center !important;
}

.modal-iframe {
  width: 100%;
  height: 100%;
}

.group_title {
  line-height: 38px;
  height: 38px;
  font-size: 15px;
  font-weight: bold;
}
:deep(.ant-card-body) {
  padding: 16px !important;
}
.list-card {
  &__card {
    width: 100%;
    height: 200px;

    .ant-card-body {
      padding: 16px;
    }

    &-title {
      text-align: center;
      padding-top: 30px;
      font-weight: 500;

      i {
        margin-top: -5px;
        margin-right: 10px;
        font-size: 38px !important;
      }
    }

  }
}

:deep(.ant-card-body) {
  height: 100%
}

:deep(.ant-table-row-indent) {
  display: none;
}
</style>


<template>
  <div id="Runing">
    <el-table
      ref="runTable"
      :data="tableData"
      height="80vh"
      row-key="WorkID"
      default-expand-all
      border
      style="width: 100%; margin-bottom: 20px"
      :row-class-name="tableRowClassName"
      :tree-props="{ children: 'children', hasChildren: 'hasChildren' }"
    >
      <el-table-column label="#" width="50" fixed type="index">
      </el-table-column>
      <el-table-column prop="Title" label="标题" fixed width="350">
        <template slot-scope="scope">
          <span v-if="scope.row.type != null"><i
                :class="
                  scope.row.IsRead === 1
                    ? 'fas fa-envelope-open iconColor-w'
                    : 'fas fa-envelope iconColor'
                "
              ></i
              ><span
                :class="scope.row.IsRead === 1 ? 'pl-5' : 'cellColor pl-5'"
                >{{ scope.row.Title }}</span></span>
          <span v-else
            ><el-link
              :underline="false"
              type="primary"
              @click="sikpMyflow(scope.row)"
              ><i
                :class="
                  scope.row.IsRead === 1
                    ? 'fas fa-envelope-open iconColor-w'
                    : 'fas fa-envelope iconColor'
                "
              ></i
              ><span
                :class="scope.row.IsRead === 1 ? 'pl-5' : 'cellColor pl-5'"
                >{{ scope.row.Title }}</span></el-link
            ></span
          >
        </template>
      </el-table-column>
      <el-table-column prop="StarterName" label="发起人/部门" width="140">
        <template slot-scope="scope">
          <span>{{ scope.row.StarterName }};{{ scope.row.DeptName }}</span>
        </template>
      </el-table-column>
      <el-table-column prop="NodeName" label="节点/流程" width="150">
      </el-table-column>
      <el-table-column prop="RDT" label="到达时间" width="160">
      </el-table-column>
      <el-table-column prop="TodoEmps" label="当前处理人"> </el-table-column>
      <!--<el-table-column label="操作" width="150">
					<template slot-scope="scope">
						<el-button @click="handleClick(scope.row)" type="text" size="small">轨迹</el-button>
						<el-button type="text" size="small">撤销</el-button>
						<el-button type="text" size="small">催办</el-button>
					</template>
				</el-table-column>-->
    </el-table>

    <el-pagination
      background
      @size-change="handleSizeChange"
      @current-change="handleCurrentChange"
      :current-page="currentPage"
      :page-sizes="[5, 10, 15, 20]"
      :page-size="pageSize"
      layout="total,sizes, prev, pager, next"
      :total="total"
      v-show="total > 0"
    ></el-pagination>
  </div>
</template>

<script>
import {domain} from "./api/config";
import {openMyView} from "./api/Dev2Interface";

export default {
  name: "Runing",
  components: {},
  data() {
    return {
      title: "在途",
      data: {},
      tableData: [],
      total: 0, // 总数
      currentPage: 1, // 当前页
      pageSize: 15, //一页显示的行数
    };
  },

  beforeCreate() {},
  created() {
    this.loadData();
  },
  methods: {
    tableRowClassName({ row }) {
      if (row.type === null) return "";

      if (row.type === 1) {
        return "success-row";
      }
      return "";
    },
    // 获取数据
    loadData() {
      let handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
      handler.AddPara("Domain",domain);
      let data = handler.DoMethodReturnString("Runing_Init");
      if (data.indexOf("err@") != -1) {
        this.$message.error(data);
        console.log(data);
        return;
      }
      this.data = JSON.parse(data);
      this.total = this.data.length;
      // eslint-disable-next-line no-mixed-spaces-and-tabs
	    this.filterData();
      this.paging();
    },

    // 分页
    paging() {
      let start = (this.currentPage - 1) * this.pageSize;
      let end = this.currentPage * this.pageSize;
      let arr = this.data.slice(start, end);
      this.pageData(arr);
    },

    // 过滤数据
    filterData() {
      this.data.forEach((item) => {
        const ff = item.TodoEmps;
        const arrays = ff.split(";");
        let nameArry=[];
        for(let i=0;i<arrays.length;i++){
          nameArry.push(arrays[i].split(",")[1]);
        }
        item.TodoEmps = nameArry.join(";")
      });
    },
	// 过滤数据
	pageData(data) {
		this.tableData = [];
		data.forEach((item) => {
			this.tableData.push(item);
		});
	},
    // 查询
    onSubmit() {},
    handleSizeChange(val) {
      this.pageSize = val;
      this.currentPage = 1;
      this.paging();
      console.log(`每页 ${val} 条`);
    },
    handleCurrentChange(val) {
      this.currentPage = val;
      this.paging();
      console.log(`当前页: ${val}`);
    },

    //跳转到jflow页面
    sikpMyflow(work) {
      let params = {};
      params.WorkID = work.WorkID;
      params.FK_Flow = work.FK_Flow;
      params.FK_Node = work.FK_Node;
      params.FID = work.FID;
      openMyView(params,this);
    },
  },

  //监听
  computed: {},

  //监听后执行动作
  watch: {},
};
</script>

<style>
	.cellColor {color: #545454;}
	.iconColor {color: #f56c6c;}
	.iconColor-w {color: #909399;}
</style>
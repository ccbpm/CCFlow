<template>
  <div>
    <div>
      <el-form inline :model="forms" :rules="rules" class="demo-form-inline" ref="Form">
        <el-form-item label="关键字" prop="keyWord">
          <el-input v-model="forms.keyWord" placeholder="关键字"></el-input>
        </el-form-item>
        <el-form-item label="发起日期" prop="RageDate">
          <el-date-picker
            v-model="forms.releaseDate"
            type="daterange"
            range-separator="至"
            start-placeholder="开始日期"
            end-placeholder="结束日期"
            value-format="yyyy-MM-dd"
          ></el-date-picker>
        </el-form-item>
        <el-form-item label="状态">
          <el-select v-model="forms.status.value" placeholder="请选择">
            <el-option
              v-for="item in forms.status.options"
              :key="item.value"
              :label="item.label"
              :value="item.value"
            ></el-option>
          </el-select>
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="onSubmit">查询</el-button>
        </el-form-item>
      </el-form>

      <el-table :data="tableData" style="width: 100%">
        <el-table-column type="index" label="#" width="60" />
        <el-table-column prop="Title" label="标题" width="180" />
        <el-table-column prop="flow" label="流程/停留节点" width="180" />
        <el-table-column prop="StarterName" label="发起人" />
        <el-table-column prop="initiateDate" label="发起日期" />
        <el-table-column prop="lastDate" label="最后日期" />
        <el-table-column prop="time" label="耗时" width="180" />
        <el-table-column prop="status" label="状态" width="180" />
      </el-table>
    </div>
  </div>
</template>

<script>
export default {
  data() {
    return {
      forms: {
        keyWord: "",
        releaseDate: '',
        status: {
          value: 'all',
          options: [{
            value: 'all',
            label: '全部'
          }, {
            value: '0',
            label: '运行中'
          }, {
            value: '1',
            label: '退回'
          }, {
            value: '2',
            label: '已完成'
          }],
        }
      },
      rules: {
        keyWord: [
          { min: 1, max: 30, message: '长度在1到30个字符', trigger: 'blur' }
        ],
        RageDate: []
      },
      tableData: [
        {
          Title: '标题',
          flow: '流程/停留节点',
          StarterName: '发起人',
          initiateDate: '发起日期',
          lastDate: '最后日期',
          time: '耗时',
          status: '状态'
        }
      ]
    }
  },
  methods: {
    onSubmit() {
      this.$refs['Form'].validate((valid) => {
        valid && this.search(
          this.forms.keyWord,
          this.forms.releaseDate ? this.forms.releaseDate[0] + ' 00:00:00' : '',
          this.forms.releaseDate ? this.forms.releaseDate[1] + ' 23:59:59' : '',
          "",
          this.forms.status.value
        );
      })
    },
    search(
      Key = "",
      DTFrom = "",
      DTTo = "",
      FlowNo = "",
      WFState = ""
    ) {
      //执行查询.
      const handler = new this.HttpHandler("BP.WF.HttpHandler.WF");
      handler.AddPara("Key", Key);
      handler.AddPara("DTFrom", DTFrom);
      handler.AddPara("DTTo", DTTo);
      handler.AddPara("FlowNo", FlowNo);
      handler.AddPara("WFState", WFState);
      const data = handler.DoMethodReturnJSON("SearchZongHe_Init");
      this.handleData(data);
    },
    handleData(data) {
      const now = this.dayjs();
      this.tableData = data.map(item => {
        item.flow = `${item.FlowName}/${item.NodeName}`;
        item.initiateDate = item.RDT.substring(0, 16);
        item.lastDate = item.SendDT.substring(0, 16);
        // 耗时 已完成 按照 RDT , Send DT两个时间差计算
        let diffm;
        if (item.WFState == 3) {
          diffm = this.dayjs(item.SendDT).diff(this.dayjs(item.RDT));
        } else {
          diffm = this.dayjs().diff(this.dayjs(item.RDT));
        }
        item.time = this.dayjs.duration(diffm).format('DD[天] HH[时] mm[分]');
        // const dayObj = this.dayjs.duration(diffm).$d.$ds;
        // console.log(`🚀 :: dayObj`, dayObj);
        // item.time = `${dayObj?.days || 0}天${dayObj?.hours || 0}时${dayObj?.minutes || 0}分`
        /*
            dayjs diff
            now > SendDT 正
            now < SendDT 负
        */
        if (item.WFState == 2) {
          item.status = now.diff(this.dayjs(item.SendDT)) <= 0 ? '运行中' : '已逾期';
        } else if (item.WFState == 3) {
          item.status = '已完成'
        } else if (item.WFState == 5) {
          item.status = '退回'
        } else {
          item.status = '其他'
        }

        return item;
      })
    }
  },
  created() {
    this.search();
  },
}
</script>

<style scoped>
</style>
<template>
    <el-form ref="form" :model="form">
        <el-table :data="form.gridData" empty-text='暂无数据'>
            <el-table-column property="NodeName" label="节点" width="150"></el-table-column>
            <el-table-column property="FK_EmpT" label="处理人" width="200"></el-table-column>
            <el-table-column property="StartDt" label="计划开始时间">
                <template slot-scope="scope">
                    <el-form-item >
                        <el-input v-model="scope.row.StartDt"></el-input>
                    </el-form-item>
                </template>
            </el-table-column>
            <el-table-column property="EndDT" label="计划完成时间">
                <template slot-scope="scope">
                    <el-form-item >
                        <el-input v-model="scope.row.EndDT"></el-input>
                    </el-form-item>
                </template>
            </el-table-column>
            <el-table-column property="GT" label="工天(天)">
                 <template slot-scope="scope">
                    <el-form-item >
                        <el-input v-model="scope.row.GT"></el-input>
                    </el-form-item>
                </template>
            </el-table-column>
            <el-table-column property="Scale" label="阶段占比(%)">
                 <template slot-scope="scope">
                    <el-form-item >
                        <el-input v-model="scope.row.Scale"></el-input>
                    </el-form-item>
                </template>
            </el-table-column>
            <el-table-column property="TotalScale" label="总体进度(%)">
                 <template slot-scope="scope">
                    <el-form-item >
                        <el-input v-model="scope.row.TotalScale"></el-input>
                    </el-form-item>
                </template>
            </el-table-column>
            <el-table-column property="RDT" label="任务到达时间"></el-table-column>
            <el-table-column property="CDT" label="实际完成时间"></el-table-column>
            <el-table-column property="UseTime" label="耗时"></el-table-column>
            <el-table-column property="State" label="状态"></el-table-column>
        </el-table>
    </el-form>
</template>
<script>
import {HttpHandler} from '@/wf/api/Gener.js'
export default {
     data() {
      return {
        form: {
            gridData: [],
        
        }
        
      };
    },
    created(){
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
        handler.AddJson(this.$route.params);
        var data = handler.DoMethodReturnString("CH_Init");
        if (data.indexOf("err@") == 0) {
            alert(data);
            return;
        }
        data = JSON.parse(data);
        this.gridData = data["WF_CHNode"];
    }
}
</script>
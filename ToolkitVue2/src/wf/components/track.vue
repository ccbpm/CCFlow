<template>
  <div id="track">
    <el-tabs v-model="activeName">
      <el-tab-pane label="轨迹图" name="second">
        <track-chart v-if="trackInfo"
                     :nodes="trackInfo.WF_Node"
                     :labels="trackInfo.WF_LabNote"
                     :relations="trackInfo.WF_Direction"
                     :prev-nodes="trackInfo.WF_GenerWorkerList"/>
      </el-tab-pane>
      <el-tab-pane label="时间轴" name="third">
        <timeBase></timeBase>
      </el-tab-pane>
      <el-tab-pane label="时间表" name="fourth">
        <timeTable></timeTable>
      </el-tab-pane>
    </el-tabs>
  </div>
</template>

<script>
import timeBase from './timeBase.vue'//时间轴
import timeTable from './timeTable.vue'
import TrackChart from "./TrackChart";
import {HttpHandler} from "@/wf/api/Gener.js";

export default {
  name: "Track",
  components: {
    timeBase,
    timeTable,
    TrackChart
  },
  data() {
    return {
      activeName: "second",
      params: {},
      trackInfo: null
    };
  },
  methods: {
    fetchData() {
      const { FK_Flow, WorkID = 0, FID = 0, Token } = this.$route.query;
      const handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
      handler.AddPara("FK_Flow", FK_Flow);
      handler.AddPara("WorkID", WorkID);
      handler.AddPara("FID", FID);
      if (Token) handler.AddPara("Token", Token);
      let flowData = handler.DoMethodReturnString("Chart_Init");
      if (typeof flowData === "string") {
        if (flowData.indexOf("err@") === 0) {
          this.$message.error(flowData.place("err@", ""));
          return;
        }
        flowData = JSON.parse(flowData);
      }
      this.trackInfo = flowData
    },
  },
  mounted() {
    this.params = this.$store.getters.getData;
    this.fetchData();
  },


};
</script>

<style lang="less" scoped>
</style>

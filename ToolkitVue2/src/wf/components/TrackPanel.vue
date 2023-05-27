<template>
  <div class="wrapper">
    <div
      class="node-item"
      v-for="item in nodes"
      :key="item.ID"
      :style="calcNodeAttrs(item)"
      :data-nid="item.ID"
    >
      {{ item.Name }}
    </div>
    <span
      class="note"
      v-for="label in labels"
      :key="label.MyPK"
      :style="calcLabelAttrs(label)"
      >{{ label.Name }}</span
    >
  </div>
</template>


<script>
import { jsPlumb } from "jsplumb";
import { HttpHandler } from "../../api/Gener";

// 引用流程图配置
const STYLE_NODE_BORDER_COLOR_END = 'green';
const STYLE_NODE_BORDER_COLOR_FIRST = 'green';    //开始节点边框颜色
const STYLE_NODE_BORDER_WIDTH_NORMAL = 1; //节点边框宽度
const STYLE_NODE_BORDER_COLOR = 'black';  //节点边框颜色


export default {
  name: "v-js-plumb",
  data() {
    return {
      // 节点列表
      nodes: [],
      // 节点方向
      nodeDirection: [],
      // 标签信息
      labels: [],
    };
  },
  methods: {
    calcLabelAttrs(label) {
      return {
        top: `${label.Y}px`,
        left: `${label.X}px`,
      };
    },

    // 计算节点属性
    calcNodeAttrs(node) {
      return {
        top: `${node.Y}px`,
        left: `${node.X}px`,
        ...node.style,
      };
    },
    // 绘制节点
    fetchData() {
      const { FK_Flow, WorkID = 0, FID = 0, Token } = this.$route.query;
      const handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
      handler.AddPara("FK_Flow", FK_Flow);
      handler.AddPara("WorkID", WorkID);
      handler.AddPara("FID", FID);
      if (Token) handler.AddPara("Token", token);
      let flowData = handler.DoMethodReturnString("Chart_Init");
      if (typeof flowData === "string") {
        if (flowData.indexOf("err@") == 0) {
          this.$message.error(flowData.place("err@", ""));
          return;
        }
        flowData = JSON.parse(flowData);
      }
      this.nodes = this.analyNodes(flowData.WF_Node) || [];
      this.nodeDirection = flowData.WF_Direction || [];
      this.labels = flowData.WF_LabNote || [];
      this.drawNodesAnchor();
    },
    // 绘制节点关系，节点位置通过template绘制，后续的连接线通过此方法生成
    async drawNodesAnchor() {
      await this.$nextTick();
      const { nodeDirection } = this;
      let plumbInstance = jsPlumb.getInstance();
      plumbInstance.ready(() => {
        for (let node of nodeDirection) {
          plumbInstance.connect({
            source: document.querySelector(`div[data-nid="${node.Node}"]`),
            target: document.querySelector(`div[data-nid="${node.ToNode}"]`),
            anchor: [
              "Left",
              "Right",
              "Top",
              "Bottom",
              [0.3, 0, 0, -1],
              [0.7, 0, 0, -1],
              [0.3, 1, 0, 1],
              [0.7, 1, 0, 1],
            ],
            connector: ["StateMachine"],
            endpoint: "Blank",
            overlays: [["Arrow", { width: 8, length: 8, location: 1 }]],
            paintStyle: { stroke: "#909399", strokeWidth: 2 },
          });
        }
      });
    },

    // 计算边框颜色
    getNodeBorderColor(val, startVal, endVal) {
      return val === startVal
        ? STYLE_NODE_BORDER_COLOR_FIRST
        : val === endVal
        ? STYLE_NODE_BORDER_COLOR_END
        : STYLE_NODE_BORDER_COLOR;
    },

    // 分析节点
    analyNodes(nodes) {
      // 处理border颜色
      const startVal = 0; // 最小节点值
      const endVal = Math.max(...nodes.map((node) => node.NodePosType)); // 最大节点值
      for (const node of nodes) {
        const color = this.getNodeBorderColor(
          node.NodePosType,
          startVal,
          endVal
        );
        node.style = {
          border: `${STYLE_NODE_BORDER_WIDTH_NORMAL}px solid ${color}`,
        };
      }
      return nodes;
    },
  },
  mounted() {
    // load data
    this.fetchData();
    // draw nodes
  },
};
</script>

<style lang="less" scoped>
.wrapper {
  width: 800px;
  height: 600px;
  box-sizing: border-box;
  position: relative;
  .node-item {
    width: 100px;
    height: 40px;
    line-height: 40px;
    text-align: center;
    border: 1px solid #cccccc;
    border-radius: 4px;
    margin: 12px;
    background: #ffffff;
    position: absolute;
  }
  .note {
    position: absolute;
    color: #031f5d;
    white-space: nowrap;
  }
}
</style>
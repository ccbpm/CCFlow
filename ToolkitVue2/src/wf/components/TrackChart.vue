<template>
  <div class="chart-root">
    <!--mask-->
    <transition name="fade">
      <div v-if="fullscreenHover" class="mask">
      </div>
    </transition>

    <!-- render nodes -->
    <div
        :class="['node', nodeCls]"
        v-for="node in renderNodes"
        :key="node.ID"
        :style="nodeStyle(node)"
        :data-nid="`node-${node.ID}`"
        @mouseenter="changeVisible(node,true)"
        @mouseleave="changeVisible(node,false)"
    >
      <div class="basis-info">
        <img class="node-user-icon" :src="node.Icon" @error="loadError" alt=""/>{{ node.Name }}
      </div>
<!--      <div v-if="node.person" class="node-person-info" :style="toolTipStyle(node.person)">-->
<!--        <template v-if="node.person.expand">-->
<!--          <p>执行人：{{ node.person.FK_EmpText }}</p>-->
<!--          <p v-if="node.person.IsPass === 1">完成：{{ node.person.CDT }}</p>-->
<!--          <p class="close" @click="node.person.expand = false">[X]</p>-->
<!--        </template>-->
<!--        <template v-else>-->
<!--          <div style="width: 100%;height: 100%" @click="node.person.expand = true">处理人</div></template>-->
<!--      </div>-->
      <div v-if="node.person" class="node-info-panel" :style="nodeInfoStyle(node)">
        <div class="panel-title">{{ node.person.FK_EmpText }}</div>
        <div class="panel-column">
          <p class="cell key">任务下达日期</p>
          <p class="cell val">{{ node.person.RDT }}</p>
        </div>
        <div class="panel-column">
          <p class="cell key">应完成日期</p>
          <p class="cell val">{{ node.person.SDT }}</p>
        </div>
        <template v-if="node.person.IsPass === 1">
          <div class="panel-column">
            <p class="cell key">实际完成</p>
            <p class="cell val">{{ node.person.CDT }}</p>
          </div>
          <div class="panel-column">
            <p class="cell key">用时</p>
            <p class="cell val">{{ node.person.duration }}</p>
          </div>
        </template>
        <template v-else>
          <div class="panel-column">
            <p class="cell key">还剩余</p>
            <p class="cell val" :style="{
              color: node.person.isTimeout ? '#ff4444':'#333333'
            }">{{ node.person.remains }}</p>
          </div>
          <div class="panel-column">
            <p class="cell key">是否打开？</p>
            <p class="cell val" style="color:#ff4444">{{ node.person.IsRead ? '是' : '否' }}</p>
          </div>
        </template>
        <div class="panel-column">
          <p class="cell key">发送人</p>
          <p class="cell val">{{ node.person.Sender || '未知' }}</p>
        </div>
      </div>
    </div>
    <!-- render label -->
    <span
        class="label"
        v-for="label in labels"
        :key="label.MyPK"
        :style="labelStyle(label)"
    >{{ label.Name }}</span>
  </div>
</template>

<script>
import {jsPlumb} from "jsplumb";
import dayjs from 'dayjs'
import duration from 'dayjs/plugin/duration'
import {defaultConnectStyle,connectOptions} from "../api/TrackConfig";
dayjs.extend(duration)

export default {
  name: "track-chart",
  props: {
    // 节点
    nodes: {
      type: Array,
      default: () => [],
    },
    // 节点关系
    relations: {
      types: Array,
      default: () => [],
    },
    // 标签
    labels: {
      type: Array,
      default: () => [],
    },
    // 已经过节点
    prevNodes: {
      type: Array,
      default: () => [],
    },
    // 节点样式
    nodeCls: {
      type: Array,
      default: () => [],
    },
    // 链接样式
    connectCls: {
      type: Array,
      default: () => [],
    },
    // 激活样式
    hoverColor: {
      type: String,
      default: "#459dff",
    },
    // 激活样式
    activeColor: {
      type: String,
      default: "#459dff",
    },
  },
  data() {
    return {
      uiInstance: null,
      // 实际渲染节点
      renderNodes: null,
      fullscreenHover: false
    };
  },
  computed: {
    maskStyle() {
      const {fullscreenHover} = this
      return {
        zIndex: fullscreenHover ? 2 : -1,
        backgroundColor: fullscreenHover ? `rgba(0, 0, 0, 0.1)` : ''
      }
    },
  },
  methods: {
    toolTipStyle(person) {
      const {expand} = person
      return {
        width: expand ? '200px' : '50px',
        height: expand ? 'auto' : '',
        textAlign: expand ? 'left' : 'center',
        borderRadius: expand ? '8px' : '0',
        top: expand ? '-18%': '0',
        padding: expand ? '2px 8px' : ''
      }
    },
    // 以下皆为样式
    changeVisible(node, status) {
      if (!node.person) return
      node.hover = status
      this.fullscreenHover = status
    },
    nodeInfoStyle(node) {
      return {
        zIndex: node.hover ? 12 : -1,
        display: node.hover ? 'block' : 'none'
      }
    },
    // 计算时长
    formatTime(duration) {
      const {years, months, days, hours, minutes, seconds} = duration.$d
      return `${years > 0 ? years + '年-' : ''}${months > 0 ? months + '月-' : ''}${days > 0 ? days + '天-' : ''} ${hours > 0 ? hours + '小时:' : ''}${minutes > 0 ? minutes + '分:' : ''}${seconds > 0 ? seconds + '秒' : ''}`
    },
    // 计算任务时间
    calcTaskTime(person) {
      // 实际、应该完成
      const {SDT, CDT, IsPass} = person
      const SDTObj = new Date(SDT.replace(/-/g, '/')).getTime()
      const CDTObj = new Date(CDT.replace(/-/g, '/')).getTime()
      // 如果已读
      if (IsPass === 1) {
        const duration = dayjs.duration(CDTObj - SDTObj)
        person.duration = this.formatTime(duration)
      } else if (IsPass === 0) {
        const ms = Date.now() - SDTObj
        const remains = dayjs.duration(ms)
        person.remains = ms > 0 ? this.formatTime(remains) : '已超时'
        person.isTimeout = ms <= 0
      }
    },

    // node style
    nodeStyle(node) {
      const {X, Y, person} = node;
      let nodeColor = "#e0e3e7";
      // 后续未经过节点
      if (!person) {
        nodeColor = "#e0e3e7";
      } else if (person?.IsPass === 0) {
        nodeColor = "#ff4444";
      } else {
        nodeColor = "#1296db";
      }
      return {
        borderColor: nodeColor,
        color: nodeColor === '#e0e3e7' ? '#333333': nodeColor,
        borderWidth: `1px`,
        top: Y + "px",
        left: X + "px",
      };
    },
    // label style
    labelStyle(label) {
      const {X, Y} = label;
      return {
        top: Y + "px",
        left: X + "px",
        width:"200px"
      };
    },
    // error handler
    loadError(e) {
      e.target.src = require("@/assets/avatar/Default.jpg");
    },
    // 分析流程，找出已完成节点、当前节点、未完成节点，
    analyzeFlow() {
      const compNodes = JSON.parse(JSON.stringify(this.nodes));
      const compPrevNodes = JSON.parse(JSON.stringify(this.prevNodes));
      for (const node of compNodes) {
        const {ID} = node;
        const idx = compPrevNodes.findIndex((pNode) => pNode.FK_Node === ID);
        if (idx > -1) {
          node.person = compPrevNodes[idx]
          this.calcTaskTime(node.person)
          node.person.expand = false
        } else {
          node.person = null
        }
        node.hover = false;
      }
      this.renderNodes = compNodes;
    },
    // 创建连接
    async createConnection() {
      await this.$nextTick();
      const {relations} = this;
      for (const relation of relations) {
        const {ToNode} = relation
        const config = JSON.parse(JSON.stringify(defaultConnectStyle))
        if (this.prevNodes.findIndex(node => node.FK_Node === ToNode) > -1) {
          config.paintStyle.stroke = '#459dff'
        }
        this.uiInstance.connect({
          source: document.querySelector(
              `div[data-nid="node-${relation.Node}"]`
          ),
          target: document.querySelector(
              `div[data-nid="node-${relation.ToNode}"]`
          ),
          ...config,
        },connectOptions);
      }
    },
  },
  mounted() {
    this.uiInstance = jsPlumb.getInstance();
    this.uiInstance.ready(() => {
      this.analyzeFlow();
      this.createConnection();
    });
    if (this.hoverColor) {
      document
          .querySelector(".chart-root")
          .style.setProperty("--chart-node-hover-color", this.hoverColor);
    }
    if (this.activeColor) {
      document
          .querySelector(".chart-root")
          .style.setProperty("--chart-node-active-color", this.activeColor);
    }
  },
};
</script>
<style lang="scss" scoped>
.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}
.fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}
.chart-root {
  width: 100%;
  height: 100%;
  min-height: 500px;
  overflow: scroll;
  background-color: white;
  position: relative;
  box-sizing: border-box;
  --chart-node-hover-color: #1111111;
  --chart-node-active-color: #1111111;

  .mask {
    position: absolute;
    left: 0;
    top: 0;
    width: 100%;
    height: 100%;
    z-index: 1;
    background-color: rgba(0,0,0,0.1);
  }

  .node {
    position: absolute;
    cursor: move;
    min-width: 180px;
    line-height: 28px;
    height: auto;
    width: auto;
    color: #333333;
    font-size: 12px;
    border: 1px solid #e0e3e7;
    border-left-width: 5px !important;
    padding: 2px 10px;
    border-radius: 4px;
    background: white;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    user-select: none;
    transition: all ease 0.33s;

    .basis-info {
      display: flex;
      align-items: center;

      .node-user-icon {
        width: 24px;
        height: 24px;
        object-fit: contain;
      }
    }

    .node-person-info {
      position: absolute;
      left: 106%;
      line-height: 18px;
      border: 1px solid rgb(224, 227, 231);
      padding: 2px 6px;
      background: white;
      //border-radius: 8px;
      color: #999999;
      box-sizing: border-box;
      cursor: pointer;
      transition: all ease 0.2s;
      z-index: 13;


      .close {
        position: absolute;
        top: 2px;
        right: 6px;
        font-size: 12px;
      }
    }
    //.node-person-info:before {
    //  content:'';
    //  width: 0;
    //  height: 0;
    //  border-width: 6px 8px 6px 0;
    //  border-style: solid;
    //  border-color: transparent #ccc transparent transparent;
    //  position: absolute;
    //  left: -8px;
    //  top: 50%;
    //  transform: translateY(-50%);
    //}
    &:hover {
      box-shadow: rgba(0, 0, 0, 0.35) 0px 5px 15px;
      border-color: var(--chart-node-hover-color);
      color: var(--chart-node-active-color);
      //background-color: rgba(128, 128, 128, 0.3);
      transform: scale(1.08);
      z-index: 1;
    }
    .node-info-panel {
      position: absolute;
      top: 33px;
      left: 0;
      width: 300px;
      height: auto;
      display: none;
      z-index: -1;
      background-color: white;
      padding: 2px 8px;
      font-size: 12px;
      color: black;
      border: 1px solid #eeeeee;
      cursor: auto;
      user-select: text;
      .panel-title {
        text-align: center;
        font-weight: 600;
      }
      .panel-column {
        display: flex;
        align-items: center;
        justify-content: center;
        .cell {
          flex-shrink: 0;
          font-size: 12px;
          box-sizing: border-box;
          white-space: nowrap;
          overflow: hidden;
          text-overflow: ellipsis;
          padding-left: 8px;
          margin-top: 1px;
          margin-bottom: 0;
        }
        .key {
          flex: 0.35;
          background-color: #eeeeee;
        }
        .val {
          flex: 0.65;
          background-color: #f8f8f8;
        }
      }
    }
  }

  .label {
    position: absolute;
    color: rgb(128, 128, 128);
    font-size: 12px;
  }
}
</style>

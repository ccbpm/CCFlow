<template>
	<div class="block">
		<div class="radio">
			排序：
			<el-radio-group v-model="reverse">
				<el-radio :label="true">倒序</el-radio>
				<el-radio :label="false">正序</el-radio>
			</el-radio-group>
		</div>

		<el-timeline :reverse="reverse">
			<el-timeline-item
					v-for="(activity, index) in activities"
					:key="index"
					placement="top"
					:timestamp="activity.rdt"
					:color="activity.color">

				<div  v-html="activity.doc">{{activity.doc}}</div>
			</el-timeline-item>
		</el-timeline>
	</div>
</template>

<script>
import $ from 'jquery';
import {ActionType} from "../api/CommEnum.js";
export default {
	name: "timeBase",

	data() {
		return {
			data:[],
			params: {},
			completed: [],
			unfinished: [],
			activeNames: ['1'],
			reverse: true,
			activities: []
		};
	},

	beforeCreate() {},

	created() {},
	mounted() {
		this.params = this.$route.query;
			var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
			handler.AddJson(this.params);
			this.data = handler.DoMethodReturnString("TimeBase_Init");
			this.data=JSON.parse(this.data);
			console.log("进入时间轴",this.data);
			//解析时间轴数据
			this.timeBase();

		
	},

	methods: {
		timeBase() {
			var tracks = this.data["Track"];//轨迹信息
			//var gwf = this.data["WF_GenerWorkFlow"][0];//流程信息
			var fwc = this.data["FrmWorkCheck"][0];//审核组件信息
			var gwls = this.data["WF_GenerWorkerList"];//获得工作人员列表.
			var isHaveCheck = false;
			for(var idx=0;idx<tracks.length;idx++){
				var track = tracks[idx];
				if (track.ActionType == ActionType.FlowBBS)
					continue;
				if (track.ActionType == ActionType.WorkCheck)
					continue;
				if (fwc.FWCMsgShow == "1" && track.NDFrom == this.params.FK_Node && this.params.UserNo != track.EmpTo)
					continue;
				//流程运行图标
				var img = this.actionTypeStr(track.ActionType);
				img = "<img src='" + img + "' width='10px;' class='ImgOfAC' alt='" + track.ActionTypeText + "'  />";
				var doc = "";
				doc += track.NDFromT + " - " + track.ActionTypeText;
				doc+="<br/><span>处理人:"+track.EmpFromT+"</span>"
				var at = track.ActionType;
				if (at == ActionType.Return) {
					doc += "<p><span>退回到:</span><font color=green>" + track.NDToT + "</font><span>退回给:</span><font color=green>" + track.EmpToT + "</font></p>";
					doc += "<p><span>退回意见如下</span>  </p>";
				}
				//前进或者流程结束
				if (at == ActionType.Forward || at == ActionType.FlowOver) {
					doc += "<p><span>到达节点:</span><font color=green>" + track.NDToT + "</font><span>到达人员:</span><font color=green>" + track.EmpToT + "</font> </p>";
					if (track.Msg != null && track.Msg != undefined && track.Msg.indexOf("WorkCheck@") != -1) {
						track.Msg = track.Msg.replace("WorkCheck@","");
						isHaveCheck = true;
						doc += "<p><span>审批意见：</span><font color=green>" + track.Msg + "</font> </p>";
					} else {
						//查找关联的审核意见
						//找到该节点，该人员的审核track, 如果没有，就输出Msg, 可能是焦点字段。
						for (var myIdx = 0; myIdx < tracks.length; myIdx++) {
							var checkTrack = tracks[myIdx];
							if (checkTrack.NDFrom == track.NDFrom && checkTrack.ActionType == ActionType.WorkCheck && checkTrack.EmpFrom == track.EmpFrom) {
								isHaveCheck = true;
								doc += "<p><span>审批意见：</span><font color=green>" + checkTrack.Msg + "</font> </p>";
								break;
							}
						}
					}
				}

				//协作发送.
				if (at == ActionType.TeampUp) {
					$.each(tracks,function(i,checkTrack){
						if (checkTrack.NDFrom == track.NDFrom && checkTrack.ActionType == ActionType.WorkCheck && checkTrack.EmpFrom == track.EmpFrom) {
							isHaveCheck = true;
							track.Msg = track.Msg.replace('null', '').replace("WorkCheck@","");
							doc += "<p><span>会签意见：</span><font color=green>" + track.Msg + "</font> </p>";
						}
					})
				}

				var msg = track.Msg.replace('null', '').replace("WorkCheck@","");
				if (msg == "0")
					msg = "";
				if (msg != "" && isHaveCheck==false) {
					while (msg.indexOf('\t\n') >= 0) {
						msg = msg.replace('\t\n', '');
					}
					msg = msg.replace('null', '');
					if (msg == "" || msg == undefined)
						msg = "无";
					doc += "<p>";
					doc += "<font color=green>" + msg + "</font>";
					doc += "</p>";
				}
				//存储track的信息
				this.activities.push({
					empNo:track.EmpFrom, //处理人编号
					empName:track.EmpFromT,//处理人名称
					rdt:track.RDT,//工作到达时间
					doc:doc, //工作内容
					color:'#0bbd87'
				})
			}
			//处理待办
			if (gwls) {
				var isHaveNoChecker = false;
				$.each(gwls,function(i,gwl){
					if(gwl.IsPass==0){
						isHaveNoChecker=true;
						return false;
					}
				});
				//如果有尚未审核的人员，就输出.
				if (isHaveNoChecker == true) {
					var gwl;
					var gwldoc = "";
					for(var i=0;i<gwls.length;i++){
						gwl = gwls[i];
						if (gwl.IsPass == 1)
							continue;
						gwldoc += "<span>审批人</span>";
						gwldoc += gwl.FK_EmpText;
						gwldoc += "<br>";
						gwldoc += "<span>阅读状态:</span>";
						if (gwl.IsRead == "1")
							gwldoc += "<span><font color=green>已阅读.</font></span>";
						else
							gwldoc += "<span><font color=green>尚未阅读.</font></span>";
						gwldoc += "<br>";
						gwldoc += "<span>工作到达日期:</span>";
						gwldoc += gwl.RDT;
						//到达时间.
						var toTime = gwl.RDT;
						var toTimeDot = toTime.replace(/\-/g, "/");// eslint-disable-line
						toTimeDot = new Date(Date.parse(toTimeDot.replace(/-/g, "/")));
						//当前发生日期.
						var timeDot = new Date();

						gwldoc += "<br>";
						gwldoc += "<span>已经耗时:</span>";
						gwldoc += this.GetSpanTime(toTimeDot, timeDot);
						//应该完成日期.
						gwldoc += "<br>";
						gwldoc += "<span>应完成日期:</span>";
						gwldoc += gwl.SDT;

						toTime = gwl.SDT;
						toTimeDot = toTime.replace(/\-/g, "/");// eslint-disable-line
						toTimeDot = new Date(Date.parse(toTimeDot.replace(/-/g, "/")));
						//当前发生日期.
						timeDot = new Date();
						var timeLeft = this.GetSpanTime(timeDot, toTimeDot);
						if (timeLeft != 'NaN秒') {
							gwldoc += "<br>";
							gwldoc += "<span>还剩余:</span>";
							gwldoc += timeLeft;
						}
						gwldoc+="<hr/>";
					}

					this.activities.push({
						empNo:gwls.length==1?gwls[0].FK_Emp:"", //处理人编号
						empName:gwls.length==1?gwls[0].FK_EmpText:"多人处理",//处理人名称
						rdt:gwls[0].RDT,//工作到达时间
						doc:gwldoc //工作内容
					});

				}
			}


		},
		actionTypeStr:function(at){
			switch (at) {
				case ActionType.Start:
					return "../../assets/Img/Start.png";
				case ActionType.Forward:
					return "../../assets/Img/Forward.png";
				case ActionType.Return:
					return "../../assets/Img/Return.png";
				case ActionType.ReturnAndBackWay:
					return "../../assets/Img/ReturnAndBackWay.png";
				case ActionType.Shift:
					return "../../assets/Img/Shift.png";
				case ActionType.UnShift:
					return "../../assets/Img/UnShift.png";
				case ActionType.UnSend:
					return "../../assets/Img/UnSend.png";
				case ActionType.ForwardFL:
					return "../../assets/Img/ForwardFL.png";
				case ActionType.ForwardHL:
					return "../../assets/Img/ForwardHL.png";
				case ActionType.CallChildenFlow:
					return "../../assets/Img/CallChildenFlow.png";
				case ActionType.StartChildenFlow:
					return "../../assets/Img/StartChildenFlow.png";
				case ActionType.SubFlowForward:
					return "../../assets/Img/SubFlowForward.png";
				case ActionType.RebackOverFlow:
					return "../../assets/Img/RebackOverFlow.png";
				case ActionType.FlowOverByCoercion:
					return "../../assets/Img/FlowOverByCoercion.png";
				case ActionType.HungUp:
					return "../../assets/Img/HungUp.png";
				case ActionType.UnHungUp:
					return "../../assets/Img/UnHungUp.png";
				case ActionType.ShiftByCoercion:
					return "../../assets/Img/ShiftByCoercion.png";
				case ActionType.Press:
					return "../../assets/Img/Press.png";
				case ActionType.DeleteFlowByFlag:
					return "../../assets/Img/DeleteFlowByFlag.png";
				case ActionType.UnDeleteFlowByFlag:
					return "../../assets/Img/UnDeleteFlowByFlag.png";
				case ActionType.CC:
					return "../../assets/Img/CC.png";
				case ActionType.WorkCheck:
					return "../../assets/Img/WorkCheck.png";
				case ActionType.AskforHelp:
					return "../../assets/Img/AskforHelp.png";
				case ActionType.Skip:
					return "../../assets/Img/Skip.png";
				case ActionType.Order:
					return "../../assets/Img/Order.png";
				case ActionType.TeampUp:
					return "../../assets/Img/TeampUp.png";
				case ActionType.FlowBBS:
					return "../../assets/Img/FlowBBS.png";
				case ActionType.Info:
					return "../../assets/Img/Info.png";
				default:
					return "../../assets/Img/dot.png";
			}
		},
		GetSpanTime:function(date1,date2){
			//计算date2-date1的时间差，返回使用“x天x小时x分x秒”形式的字符串表示</summary>
			var date3 = date2.getTime() - date1.getTime();  //时间差秒
			if (date1.getTime() > date2.getTime())
				date3 = date1.getTime() - date2.getTime();

			var str = '';
			//计算出相差天数
			var days = Math.floor(date3 / (24 * 3600 * 1000));
			if (days > 0) {
				str += days + '天';
			}

			//计算出小时数
			var leave1 = date3 % (24 * 3600 * 1000);   //计算天数后剩余的毫秒数
			var hours = Math.floor(leave1 / (3600 * 1000));
			if (hours > 0) {
				str += hours + '小时';
			}

			//计算相差分钟数
			var leave2 = leave1 % (3600 * 1000);         //计算小时数后剩余的毫秒数
			var minutes = Math.floor(leave2 / (60 * 1000));
			if (minutes > 0) {
				str += minutes + '分';
			}


			var leave3 = leave2 % (60 * 1000);
			var seconds = Math.floor(leave3 / 1000);
			if (seconds > 0)
				str += seconds + '秒';
			if (date1.getTime() > date2.getTime())
				return "-" + str;
			return str;
		}
	},

	//监听
	computed: {},

	components: {},

	//监听后执行动作
	watch: {}
};
</script>

<style lang="less" scoped>
/deep/ .el-timeline__body {
	p {
		line-height: 40px !important;
	}
}

.portrait {
	width: 50px;
	height: 50px;
	border-radius: 50%;
}
</style>
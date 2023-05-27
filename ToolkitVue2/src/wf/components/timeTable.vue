<template>
	<div id="timeTable">
		<template>
			<el-table
					:data="tableData"
					style="width: 100%;"
					>
				<el-table-column
						fixed
						prop="idx"
						label="序号"
						width="60">
				</el-table-column>
				<el-table-column
						prop="NodeName"
						label="执行环节"
						width="120">
				</el-table-column>
				<el-table-column
						prop="Province"
						label="办理情况"
						width="120">
				</el-table-column>
				<el-table-column
						prop="ActionType"
						label="状态"
						width="120">
				</el-table-column>
				<el-table-column
						prop="EmpName"
						label="执行人"
						width="100">
				</el-table-column>
				<el-table-column
						prop="StartTime"
						label="开始时间"
						width="140">
				</el-table-column>
				<el-table-column
						prop="EndTime"
						label="结束时间"
						width="140">
				</el-table-column>
				<el-table-column
						prop="PassTime"
						label="历时"
						width="120">
				</el-table-column>
			</el-table>
		</template>

	</div>
</template>

<script>
	import $ from 'jquery';
	import {ActionType} from "../api/CommEnum.js";
export default {
	name: "timeTable",

	data() {
		return {
			data:[],
			params:{},
			tableData:[],
		};
	},

	beforeCreate() {},

	created() {},
	mounted() {
		this.$Bus.$on("track", () => {
			
		});
		console.log("进入时间轴");
			this.params = this.$route.query;
			var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_WorkOpt_OneWork");
			handler.AddJson(this.params);
			var data = handler.DoMethodReturnString("TimeBase_Init");
			this.data=JSON.parse(data);
			this.CreateTimeTable();
	},

	methods: {
		CreateTimeTable(){
			var tracks = this.data["Track"];//轨迹信息
			var fwc = this.data["FrmWorkCheck"][0];//审核组件信息
			var gwls = this.data["WF_GenerWorkerList"];//获得工作人员列表.
			var idx=1;
			for(var i=0;i<tracks.length;i++){
				var track = tracks[i];
				if (track.ActionType == ActionType.FlowBBS)
					continue;
				if (track.ActionType == ActionType.WorkCheck)
					continue;
				if (fwc.FWCMsgShow == "1" && track.NDFrom == this.params.FK_Node && this.params.UserNo != track.EmpTo)
					continue;

				var at = track.ActionType;
				if (at == ActionType.Forward || at == ActionType.FlowOver || at == ActionType.TeampUp) {
					//找到该节点，该人员的审核track, 如果没有，就输出Msg, 可能是焦点字段。
					if (fwc.FWCVer == 0) {
						for (var myIdx = 0; myIdx < tracks.length; myIdx++) {
							var checkTrack = tracks[myIdx];
							if (checkTrack.NDFrom == track.NDFrom && checkTrack.ActionType == ActionType.WorkCheck && checkTrack.EmpFrom == track.EmpFrom) {
								track.Msg = checkTrack.Msg;
								break;
							}
						}
					}
					track.Msg = track.Msg.replace('null', '').replace("WorkCheck@","");
				}
				var msg = track.Msg;
				msg = track.Msg.replace('null', '').replace("WorkCheck@","");
				if (msg == "0")
					msg = "";
				if (msg != "") {
					const reg = new RegExp('\t\n', "g");// eslint-disable-line
					msg = msg.replace(reg, '');
					msg = msg.replace('null', '');
					if (msg == "" || msg == undefined)
						msg = "无";
				}
				var startTime="";
				var endTime="";
				var passTime="";
				//获取轨迹中上一个节点的时间
				if(i==tracks.length-1){
					startTime = track.RDT;
					endTime = track.RDT;
				} else {
					//上一节点的到达时间就是本节点的开始时间
					var track1 = tracks[i+1];
					startTime = track.RDT;
					endTime = track1.RDT;
				}
				//求得历时时间差
				var sdt = startTime.replace(/\-/g, "/");// eslint-disable-line
				sdt = new Date(Date.parse(sdt.replace(/-/g, "/")));
				var edt = endTime.replace(/\-/g, "/");// eslint-disable-line
				edt = new Date(Date.parse(edt.replace(/-/g, "/")));

				passTime = this.GetSpanTime(sdt, edt);
				if (passTime == '')
					passTime = '0秒';

				//存储track的信息
				this.tableData.push({
					idx:idx, //处理人编号
					NodeName:track.NDFromT,//节点名称
					Province:msg,//处理情况
					ActionType:track.ActionTypeText, //工作内容
					EmpName:track.EmpFromT,
					StartTime:startTime,
					EndTime:endTime,
					PassTime:passTime
				});
				idx++;

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
					for(var k=0;k<gwls.length;k++){
						gwl = gwls[k];
						if (gwl.IsPass == 1)
							continue;
						var state="尚未阅读";
						if (gwl.IsRead == "1")
							state="已阅读";

						//存储track的信息
						this.tableData.push({
							idx:idx, //处理人编号
							NodeName:gwl.FK_NodeText,//节点名称
							Province:state,//处理情况
							ActionType:"等待审批", //工作内容
							EmpName:gwl.FK_EmpText,
							StartTime:gwl.RDT,
							EndTime:"-",
							PassTime:"-"
						});
						idx++;
					}
				}
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
/deep/ .el-card__body {
	p {
		line-height: 1;
	}
}
.portrait {
	width: 50px;
	height: 50px;
	border-radius: 50%;
}
</style>
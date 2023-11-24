<template >
	<el-container>
		<el-row>
			<div class="logo">
				<h2 style="color: white;margin-left: 2rem;">经典的、永恒的、奔腾不息的驰骋BPM...</h2>
			</div>
		</el-row>

		<el-container class="login-bg">
			<!--class="login-container" -->
			<el-col type="flex" justify="center">
				<el-row type="flex" justify="center">
					<el-row class="login-container" type="flex">
						<el-col :span='12' class="login-left">
							<div class="left-box">
								<el-image style="width:60%;margin-bottom: 1rem;"
									:src="require('./Img/ccbpm.png')"></el-image>
								<div style="line-height: 2rem ;font-size: 1.5rem ;font-weight: 550 ;">
									<el-row>流程引擎二开工具包</el-row>
								</div>
								<ul class="left_content">
									<li>资源:<a
											href="https://gitee.com/opencc/JFlow/wikis/pages/preview?sort_id=8095272&doc_id=31094"
											target="_blank" style="color:rgb(49, 104, 207)">Toolkit在线文档</a>
										- <a href="http://ccflow.org?frm=client" target="_blank"
											style="color:rgb(49, 104, 207)"> ccbpm官网</a></li>
									<li>工具栏、审核组件帮您搞定自定义表单.</li>
									<li>发起、待办、在途、批处理功能页直接引用.</li>
									<!-- <li>流程服务器地址: http://help.jflow.cn:8081</li> -->
									<li><a :href=wfDesignerUrl target="_blank"> 流程设计器登录</a></li>
								</ul>
							</div>
						</el-col>
						<el-col :span='12' class="login-right">
							<el-form :model="ruleForm2" :rules="rules2" status-icon ref="ruleForm2" label-position="left"
								label-width="0px" class="demo-ruleForm login-page my-auto">
								<el-row type="flex" justify="center" class="title_row">
									<el-col class="title_col">ToolkitVue2示例系统</el-col>
								</el-row>
								<!-- <el-row class="title " ></el-row> -->
								<el-form-item prop="username" class="form_item">
									<el-input type="text" v-model="ruleForm2.username" auto-complete="off"
										placeholder="用户名，管理员admin"></el-input>
								</el-form-item>
								<el-form-item prop="password" class="form_item">
									<el-input type="password" v-model="ruleForm2.password" auto-complete="off"
										placeholder="密码，默认123"></el-input>
								</el-form-item>
								<el-form-item class="form_item">
									<el-button type="primary" style="width:100%" @click="handleSubmit" :loading="logining">
										登录 -toolkit</el-button>
								</el-form-item>
								<el-divider content-position="center" class="bottom_divider"
									hidden="hidden">其他登录</el-divider>
								<el-row style="text-align: center;">

									<el-button class="other_logins" type="primary" onclick="alert('未实现');"
										icon="iconfont icon-qq" circle></el-button>
									<el-button class="other_logins" type="success" onclick="alert('未实现');"
										icon="iconfont icon-weixin" circle>
									</el-button>
									<el-button class="other_logins" type="danger" onclick="alert('未实现');"
										icon="iconfont icon-zhifubaozhifu" circle>
									</el-button>
								</el-row>
							</el-form>

						</el-col>
					</el-row>
				</el-row>

				<el-row type="flex" justify="center">
					<ul class="bottom_text">
						<li>地址：济南市.高新区.碧桂园凤凰国际A座F19</li>
						<li>电话：0531-82374939,18660153393(微信)</li>
						<li>版权：济南驰骋信息技术有限公司 @2003-2022</li>
					</ul>
				</el-row>
			</el-col>
		</el-container>

	</el-container>
</template>

<script>
import { LoginCCBPM } from "@/wf/api/Dev2Interface";
import { devServer } from '../../vue.config';
export default {
	data() {
		const wfDesignerUrl = devServer.proxy['/api'].target;
		console.log('设计器地址', wfDesignerUrl);
		return {
			wfDesignerUrl,//设计器地址,根据vue.config.js来的
			logining: false,
			show: true,
			ruleForm2: {
				username: 'admin',
				password: '123',
			},
			rules2: {
				username: [{
					required: true,
					message: '请输入用户名',
					trigger: 'blur'
				}],
				password: [{
					required: true,
					message: '请输入密码',
					trigger: 'blur'
				}]
			},
			checked: false
		}
	},
	created() {

		// var baby=new test();
		// this.$router.push("myFlow?FK_Flow=350");

	},
	methods: {
		handleSubmit() {

			//第1步: 用户自己校验用户名密码(这里要明白，此时是您的系统.)
			/*if (1==2)
			{
				alert('登陆失败');
				return;
			}else
			{
				this.$message.error("密码校验完成,本地登陆成功，等在让ccbpm登陆.");
			}*/

			//第2步: 根据私钥执行登陆,为了安全期间私约保存好,不能明文.
			const result = LoginCCBPM(process.env.VUE_APP_PRIVATEKEY, this.ruleForm2.username);
			if (result == "") {
				this.$message.error("登录失败");
				return;
			}
			this.$message.success("ccbpm登陆成功,正在转入页面.");


			//转入系统的home页.
			this.$router.push({
				path: '/start',
				query: { Token: result },
			});

			// var handler = new this.HttpHandler("BP.WF.HttpHandler.WF_AppClassic");
			// handler.AddPara("TB_No", this.ruleForm2.username);
			// handler.AddPara("TB_PW", this.ruleForm2.password);
			// var data = handler.DoMethodReturnString("Login_Submit");
			// if (data.includes("err")) {
			// 	this.$message.error('登录失败:'+data);
			// 	return;
			// }

		}
	},

};
</script>

<style lang="less" scoped>
.my-auto {
	margin-top: auto;
	margin-bottom: auto;

	.other_logins {
		border-radius: 2vh;
		margin: 0 20px;
	}
}

.login-bg {
	margin-top: 20vh;
	margin: auto;
	margin-top: 20vh;
	width: 100vw;
	height: 70vh;
	display: flex;
	justify-content: center;
	align-items: center;
	// flex-direction: column;
}

.logo {
	display: flex;
	position: fixed;
	top: 0;
	background-color: #409EFF;
	width: 100vw;
	height: 10vh;
}

.login-bg:before {
	position: absolute;
	top: 0;
	left: 0;
	width: 100%;
	height: 100%;
	margin-left: -48%;
	background-position: 100%;
	background-repeat: no-repeat;
	background-size: auto 100%;
	content: ""
}

.bottom_divider {
	margin: 1rem 0;
}

.left-box {
	width: 70%;
	margin: 0 auto;
	height: 100%;
}

.left_title {
	line-height: 1.5rem !important;
	font-size: 1rem !important;
	font-weight: 100 !important;
}

.left-box el-image {
	width: 60% !important;
	margin-bottom: 1.5rem !important;
}

.title_row {
	height: auto;
	margin: auto;
	margin-top: 0;
	color: #409EFF;
}

.title_col {
	width: auto;
	font-size: 2rem;
	font-weight: 550;
}

.login-container {
	width: 70vw;
	height: 70vh;
	padding: 2rem;
	box-shadow: 0px 0px 30px 5px rgba(98, 96, 96, 0.5);
}

.login-left {
	display: flex;
	height: 100%;
}

.login-right {
	display: flex;
	height: 100%;
}

.login-page {
	width: 100%;
	padding: 2rem;
	margin: 0 auto;


}

.form_item {
	width: 70%;
	margin: 2.5rem auto;
	border-radius: 2.5vh;

	:deep(.el-input__inner) {
		border-radius: 2.5vh;
	}

	button {
		border-radius: 2.5vh;
	}
}


label.el-checkbox.rememberme {
	margin: 0px 0px 15px;
	text-align: left;
}

.el-input__inner,
.el-button {
	border-radius: 0px;
}

.el-divider__text {
	color: #999;
}

.left_content {
	padding: 0;
	list-style-type: none;

	li {
		font-size: 14px;
		line-height: 3rem;
	}
}



li a {
	color: rgb(49, 104, 207) !important;
}

a:hover {
	color: rgb(49, 104, 207) !important;
	text-decoration: underline !important;
}

.bottom_text {
	width: 70%;
	display: flex;
	justify-content: space-around;
	padding: 0;
	list-style-type: none;

	li {
		font-size: 13px;
	}
}
</style>

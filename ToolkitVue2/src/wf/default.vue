<template>
	<div class="user">
		<el-divider content-position="left">个人信息</el-divider>
		<el-row :gutter="20">
			<el-col :span="6">
				<el-card class="avatarImg">
					<el-avatar :size="120" src="https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png">
					</el-avatar>
					<el-upload ref="upload" :file-list="fileList" action="">
						<el-button slot="trigger" size="small" type="primary">头像上传</el-button>
						<div slot="tip">只能上传jpg/png文件，且不超过500kb</div>
					</el-upload>
				</el-card>
			</el-col>
			<el-col :span="18">
				<el-card>
					<div>
						<el-form label-width="80px" size="mini" :rules="rules" :model="formLabelAlign"
							ref="ValidateForm">
							<el-form-item label="登录帐号" prop="UserNo">
								<el-input v-model="formLabelAlign.UserNo" disabled></el-input>
							</el-form-item>
							<el-form-item label="用户名" prop="UserName">
								<el-input v-model="formLabelAlign.UserName"></el-input>
							</el-form-item>
							<el-form-item label="手机号" prop="Tel">
								<el-input v-model="formLabelAlign.Tel"></el-input>
							</el-form-item>
							<el-form-item label="E-mail" prop="Email">
								<el-input v-model="formLabelAlign.Email"></el-input>
							</el-form-item>
							<el-form-item>
								<el-button type="primary" @click="submitForm('ValidateForm')">保存修改</el-button>
								<el-button @click="power()">设置委托</el-button>
							</el-form-item>
						</el-form>
					</div>
				</el-card>
			</el-col>
		</el-row>
		<el-divider></el-divider>
		<el-row :gutter="20">
			<el-col :span="6">
				<el-card>
					<div slot="header" class="clearfix">
						<span>电子签字</span>
					</div>
					<div class="item">
						<el-image :src="src">
							<div slot="placeholder" class="image-slot">
								加载中<span class="dot">...</span>
							</div>
						</el-image>
					</div>
					<el-button type="primary" size="mini" @click="dialogTableVisible = true">设置/修改</el-button>
				</el-card>
			</el-col>
			<el-col :span="6">
				<el-card>
					<div slot="header" class="clearfix">
						<span>主部门</span>
					</div>
					<div class="item">
						<el-tag type="success">{{ formLabelAlign.DeptName }}</el-tag>
					</div>
					<el-button type="primary" size="mini"  @click="dialogTab = true">切换登录部门</el-button>
				</el-card>
			</el-col>
			<el-col :span="6">
				<el-card>
					<div slot="header" class="clearfix">
						<span>修改密码</span>
					</div>
					<div class="item">
						<el-tag type="success">*********</el-tag>
					</div>
					<el-button type="primary" size="mini">修改密码</el-button>
				</el-card>
			</el-col>
			<el-col :span="6">
				<el-card>
					<div slot="header" class="clearfix">
						<span>岗位/部门-权限</span>
					</div>
				</el-card>
			</el-col>
		</el-row>
		<el-dialog title="电子签名设置" width='40%' :visible.sync="dialogTableVisible">
			<el-row :gutter="20">
				<el-col :span="12">
					<el-card>
						<h5>利用扫描仪设置步骤:</h5>
						<ul>
							<li>在白纸上写下您的签名</li>
							<li>送入扫描仪扫描，并得到jpg文件。</li>
							<li>利用图片处理工具把他们处理缩小到 90*30像素大小。</li>
						</ul>
						<h5>手写设置:</h5>
						<ul>
							<li>启动画板程序，写下您的签名。</li>
							<li>保存成.jpg文件，设置文件为90*30像素大小。</li>
						</ul>
					</el-card>
				</el-col>
				<el-col :span="12">
					<el-image class="imgupdbox" :src="url" :fit="fit"></el-image>
					<el-upload class="upload-demo" ref="upload" action="https://jsonplaceholder.typicode.com/posts/"
						:on-preview="handlePreview" :on-remove="handleRemove" :file-list="fileList"
						:auto-upload="false">
						<el-button slot="trigger" size="small" type="primary">选取文件</el-button>
						<el-button style="margin-left: 10px;" size="small" type="success" @click="submitUpload">上传到服务器
						</el-button>
						<div slot="tip" class="el-upload__tip">只能上传jpg/png文件，且不超过500kb</div>
					</el-upload>
				</el-col>
			</el-row>
		</el-dialog>
		<el-dialog title="切换部门" width='30%' :visible.sync="dialogTab">
			
		</el-dialog>
	</div>
</template>

<script>
	export default {
		name: "user",
		data() {
			return {
				fileList: [],
				formLabelAlign: {},
				src: "",
				upUrl: "",
				dialogTableVisible: false,
				dialogTab:false,
				fit: 'contain',
				url: '',
				dynamicHandler: '',
				rules: {
					userName: [{
						required: true,
						message: "请输入用户名",
						trigger: "blur"
					}, ],
					phone: [{
							required: true,
							message: "请输入手机号",
							trigger: "change"
						},
						{
							validator: function(rule, value, callback) {
								if (/^1[34578]\d{9}$/.test(value) == false) {
									callback(new Error("请输入正确的手机号"));
								} else {
									callback();
								}
							},
							trigger: "change",
						},
					],
					email: [{
							required: true,
							message: "请输入邮箱地址",
							trigger: "blur"
						},
						{
							type: "email",
							message: "请输入正确的邮箱地址",
							trigger: ["blur", "change"],
						},
					],
				},
			};
		},
		created() {
			this.loadData();
			this.HeadPic();
			this.getSigimg();

		},
		methods: {
			loadData() {
				let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_Setting");
				handler.AddUrlData();
				let data = handler.DoMethodReturnString("Default_Init");
				if (data.indexOf("err@") != -1) {
					this.$message.error(data);
					console.log(data);
					return;
				}
				this.formLabelAlign = JSON.parse(data);
				console.log(this.formLabelAlign);
			},
			submitForm() {
				this.$refs["ValidateForm"].validate((valid) => {
					if (valid) {
						let handler = new this.HttpHandler("BP.WF.HttpHandler.WF_Setting");
						handler.AddPara("Name", this.formLabelAlign.UserName);
						handler.AddPara("Tel", this.formLabelAlign.Tel);
						handler.AddPara("Email", this.formLabelAlign.Email);
						const data = handler.DoMethodReturnString("UpdateEmpNo");
						console.log(data);
						this.$message("保存成功！");
					} else {
						console.log("error submit!!");
						return false;
					}
				});
			},
			power() {
				this.$router.push({
					name: 'powerlist'
				})
			},
			HeadPic() {
				var doMethod = "HeadPic_Save";
				const httpHandlerName = "BP.WF.HttpHandler.WF_Setting";
				this.url = process.env.VUE_APP_HANDLER + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" +
					httpHandlerName;
				console.log(this.url)
			},
			//获取签名图片url 方法
			getSigimg() {
				const _this = this;
				const doMethod = "Siganture_Init";
				const httpHandlerName = "BP.WF.HttpHandler.WF_Setting";
				const apiurl = process.env.VUE_APP_HANDLER + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" +
					httpHandlerName;
				this.$.ajax({
					url: apiurl,
					dataType: "json",
					success: function(data) {
						let url = '../../DataUser/Siganture/' + data.No + '.jpg';
						_this.url = url;
						console.log(_this.url)
					}
				});
			},
			submitUpload() {
				this.$refs.upload.submit();
			},
			handleRemove(file, fileList) {
				console.log(file, fileList);
			},
			handlePreview(file) {
				console.log(file);
			}
		},
	};
</script>

<style lang="less" scoped>
	.user {
		.avatarImg {
			text-align: center;
			line-height: 40px;
			min-height: 292px;
		}

		.item {
			margin-bottom: 18px;
		}

		.imgupdbox {
			width: 120px;
			height: 30px;
		}
	}
</style>

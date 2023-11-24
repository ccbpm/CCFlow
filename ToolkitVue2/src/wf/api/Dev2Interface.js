import {decodeResponseParams, RunUrlReturnString} from "./Gener";

/**
 * 跳转到待办处理页面
 * @param params 页面传递的参数
 */
export function openMyFlow(params,_this){
    let handler = new _this.HttpHandler("BP.WF.HttpHandler.WF_MyFlow");
    handler.AddJson(params);
    let data = handler.DoMethodReturnString("MyFlow_Init");
    if(typeof data == 'string' && data.includes('err@')){
        _this.$message.error(data);
        return;
    }
    if (typeof data == 'string' && data.includes("url@")) {
        //如果返回url，就直接转向.
        const paraData = decodeResponseParams(data);
        const urlName = paraData['PageName'];
        delete paraData['PageName'];
        delete paraData['HttpHandlerName'];
        delete paraData['DoType'];
        delete paraData['DoMethod'];
        // vuex 保存数据
        _this.$store.commit("setData", paraData);

        //如果返回url，就直接转向.
        _this.$router.push({
            path: urlName,
            query: paraData,
        });
        return;
    }

    alert(data);
}

/**
 * 跳转到查看页面
 * @param params
 */
export function openMyView(params,_this){
    let handler = new _this.HttpHandler("BP.WF.HttpHandler.WF_MyView");
    handler.AddJson(params);
    let data = handler.DoMethodReturnString("MyView_Init");
    if(typeof data == 'string' && data.includes('err@')){
        _this.$message.error(data);
        return;
    }
    if (typeof data == 'string' && data.includes("url@")) {
        //如果返回url，就直接转向.
        const paraData = decodeResponseParams(data);
        const urlName = paraData['PageName'];
        delete paraData['PageName'];
        delete paraData['HttpHandlerName'];
        delete paraData['DoType'];
        delete paraData['DoMethod'];
        paraData['IsReadonly'] = 1;
        // vuex 保存数据
        _this.$store.commit("setData", paraData);
        //如果返回url，就直接转向.
        _this.$router.push({
            path: urlName,
            query: paraData,
        });
        return;
    }
}

/**
 * 登录
 * @param privateKey
 * @param userNo
 * @returns {*}
 * @constructor
 */
export function LoginCCBPM(privateKey,userNo){
    const url = process.env.VUE_APP_API+"Port_Login?privateKey=" + privateKey + "&userNo="+userNo;
    const userInfo = RunUrlReturnString(url);
    if(userInfo === undefined || userInfo ==="")
        return "";
    localStorage.setItem("UserInfo",userInfo);
    return JSON.parse(userInfo).Token;
}


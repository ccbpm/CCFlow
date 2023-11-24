/*!
* ======================================================
* FeedBack Template For MUI (http://dev.dcloud.net.cn/mui)
* =======================================================
* @version:1.0.0
* @author:cuihongbao@dcloud.io
*/
(function () {
    var index = 0;
    var size = null;
    var imageIndexIdNum = 0;
    var starIndex = 0;
    var feedback = {
        imageLists: document.getElementsByName('image-list')
    };
    var url = 'https://service.dcloud.net.cn/feedback';

    var doMethod = "MoreAttach";
    var httpHandlerName = "BP.WF.HttpHandler.WF_CCForm";
    if (plant == 'CCFlow') {
        Url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" + httpHandlerName + "&FK_FrmAttachment=" + GetQueryString("FK_FrmAttachment") + "&PKVal=" + GetQueryString("PKVal") + "&t=" + new Date().getTime();
    } else {
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        var url = path + "/WF/Ath/AttachmentUpload.do/?FK_FrmAttachment=" + GetQueryString("FK_FrmAttachment") + "&PKVal=" + GetQueryString("PKVal");
    }
    //初始化获取所上传的文件
    feedback.files = [];
    feedback.uploader = null;
    feedback.deviceInfo = null;
    mui.plusReady(function () {
        //设备信息，无需修改
        feedback.deviceInfo = {
            appid: plus.runtime.appid,
            imei: plus.device.imei, //设备标识
            images: feedback.files, //图片文件
            p: mui.os.android ? 'a' : 'i', //平台类型，i表示iOS平台，a表示Android平台。
            md: plus.device.model, //设备型号
            app_version: plus.runtime.version,
            plus_version: plus.runtime.innerVersion, //基座版本号
            os: mui.os.version,
            net: '' + plus.networkinfo.getCurrentType()
        }
    });
    /**
     *情况附件信息
     */
    feedback.clearAthFile = function () {
        for (var athIndex = 0; athIndex < feedback.imageLists.length; athIndex++) {
            feedback.imageLists[athIndex].innerHTML = '';
        }

        feedback.files = [];

    };
    feedback.getFileInputArray = function () {
        return [].slice.call(feedback.imageList.querySelectorAll('.file'));
    };
    feedback.addFile = function (athDB, athIndex, athIndex_index) {
        feedback.files.push({
            name: "images-" + athIndex + "-" + athIndex_index,
            path: athDB.FileFullName,
            id: "img-" + athIndex + "-" + athIndex_index,
            myPk: athDB.MyPK,
            FileExts: athDB.FileExts,
            FileName: athDB.FileName,
            Rec: athDB.Rec,
            FK_FrmAttachment: athDB.FK_FrmAttachment,
            NoOfObj: athDB.NoOfObj
        });
        //index++;
    };

    function handleDelete(closeButton, file) {
        var btnArray = ['否', '是'];
        var id = this.id;
        var indx = id.substring(0, id.lastIndexOf("-")).replace("img-", "");
        indx = parseInt(indx);
        mui.confirm('您确定要删除吗？ ', '提示', btnArray, function (e) {
            if (e.index == 1) {
                for (var temp = 0; temp < feedback.files.length; temp++) {
                    if (feedback.files[temp].id == closeButton.id) {
                        //处理删除事件
                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
                        handler.AddPara("DelPKVal", file.myPk);
                        var data = handler.DoMethodReturnString("AttachmentUpload_Del");
                        if (data == "删除成功.") {
                            placeholder = document.getElementById(file.myPk);
                            feedback.files.splice(temp, 1);
                            feedback.imageLists[indx].removeChild(placeholder);
                        }
                    }
                }
            }
        });
        return false;
    }

    feedback.initFile = function (ath, FK_FrmAttachment, athIndex, athIndex_imageIndexIdNum, noOfObj) {
        var data = GetAllAttachments(ath);
        var athIndex_index = 0;
        var athIndex_imageIndexIdNum = 0
        $.each(data, function (i, obj) {
            feedback.addFile(obj, athIndex, athIndex_index);
            athIndex_index++;
        });
        //判断当前是可读还是可编辑
        var IsRead = pageData.IsReadOnly == "1" ? true : false;
        //显示上传的文件
        $.each(feedback.files, function (index, element) {
            var f = feedback.files[index];
            if (f.NoOfObj != noOfObj)
                return;
            athIndex_imageIndexIdNum++;
            var placeholder = document.createElement('div');
            placeholder.setAttribute('id', f.myPk);
            placeholder.setAttribute('class', 'image-item');
            var divButton = document.createElement('div');
            divButton.style.width = '100%';
            divButton.style.height = '100%';
            if (IsRead == false) {
                var IsDelete = false;
                //删除所有
                if (ath.DeleteWay == 1)
                    IsDelete = true;
                //只删除自己的
                if (ath.DeleteWay == 2) {
                    if (f.Rec == new WebUser().No)
                        IsDelete = true;
                }
                if (IsDelete) {
                    //删除图片
                    var closeButton = document.createElement('div');
                    closeButton.setAttribute('class', 'image-close');
                    closeButton.innerHTML = 'X';
                    closeButton.id = f.id;
                    //小X的点击事件
                    // closeButton.addEventListener('click', handleDelete.bind(this, closeButton, f), false);
                    closeButton.addEventListener('tap', handleDelete.bind(this, closeButton, f), false);

                    placeholder.appendChild(closeButton);
                }
            }

            //点击图片标记的时候下载文件.
            if (ath.IsDownload == 1) {
                divButton.addEventListener('click', function (event) {
                    //处理文件下载事件
                    downLoad(f.myPk, f.path);
                });
            }
            var imgFlag = IsImgeExt(f.FileExts);
            if (imgFlag) {
                var base64 = "";
                dataUrl = GetFileStream(f.myPk);
            } else {
                dataUrl = './image/FileType/' + f.FileExts + "B.gif";
            }
            //divButton.style.backgroundImage='url(' + dataUrl + ')';
            placeholder.style.backgroundImage = 'url(' + dataUrl + ')';
            placeholder.appendChild(divButton);
            //文件名称显示
            var nameLabel = document.createElement('div');
            nameLabel.setAttribute('class', 'image-name');
            nameLabel.id = "name-" + athIndex + "-" + index;

            nameLabel.innerHTML = "<p style='text-align:center;width:63.4px;margin:0;padding:0'>" + f.FileName.split(".")[0] + "</p>";
            if (f.FileName.split(".")[0].length > 10)
                nameLabel.innerHTML = "<p style='width:63.4px;margin:0;padding:0'>" + f.FileName.split(".")[0].substr(0, 6) + "...</p>";
            placeholder.appendChild(nameLabel);

            feedback.imageList.appendChild(placeholder);

        });
    }
    /**
     * 初始化上传文件页面
     */
    feedback.initPage = function () {
        for (var athIndex = 0; athIndex < feedback.imageLists.length; athIndex++) {
            feedback.imageList = feedback.imageLists[athIndex];
            //组件Id 
            imageId = feedback.imageList.id;
            //获取所属分组的ID
            var mypk = imageId.substr(11);

            imageId = imageId.replace("_" + pageData.FK_Node, "");

            //获取NoOfObj
            var index = imageId.lastIndexOf("_");
            var noOfObj = imageId.substr(index + 1);

            feedback.newPlaceholder(mypk, athIndex, noOfObj);
        }
    }
    /**
     * 初始化图片域占位
     */
    feedback.newPlaceholder = function (FK_FrmAttachment, athIndex, noOfObj) {
        var fileInputArray = feedback.getFileInputArray();
        if (fileInputArray &&
            fileInputArray.length > 0 &&
            fileInputArray[fileInputArray.length - 1].parentNode.classList.contains('space')) {
            return;
        }
        ;
        var ath = new Entity("BP.Sys.FrmAttachment", FK_FrmAttachment);
        var athIndex_imageIndexIdNum = 0;
        //初始化显示上传的文件
        feedback.initFile(ath, FK_FrmAttachment, athIndex, athIndex_imageIndexIdNum, noOfObj);
        //如果是只读的可以返回
        if (pageData.IsReadOnly == "1" || ath.IsUpload == false)
            return;
        //添加上传的控件
        athIndex_imageIndexIdNum++;
        var placeholder = document.createElement('div');
        placeholder.setAttribute('class', 'image-item space');
        //删除图片
        var closeButton = document.createElement('div');
        closeButton.setAttribute('class', 'image-close');
        closeButton.innerHTML = 'X';
        //        closeButton.id = "img-" + athIndex_imageIndexIdNum;
        //小X的点击事件
        closeButton.addEventListener('tap', function (event) {
            setTimeout(function () {
                for (var temp = 0; temp < feedback.files.length; temp++) {
                    if (feedback.files[temp].id == closeButton.id) {
                        feedback.files.splice(temp, 1);
                        //处理删除事件
                        Del(feedback.files[temp].myPk);
                    }
                }
                feedback.imageList.removeChild(placeholder);
            }, 0);
            return false;
        }, false);

        //添加图片上传控件 input
        if (/MicroMessenger/i.test(window.navigator.userAgent)) {
            var fileInput = document.createElement('div');
            $(fileInput).addClass("WX_AddFile");
            placeholder.appendChild(closeButton);
            placeholder.appendChild(fileInput);
            feedback.imageList.appendChild(placeholder);
            $(fileInput).on("click", function () {
                //  alert("进入方法")
                WXUpload(FK_FrmAttachment, feedback);
            })
        } else {
            var fileInput = document.createElement('input');
            fileInput.setAttribute('type', 'file');
            fileInput.setAttribute('accept', '*/*');
            fileInput.setAttribute('multiple', true);
            fileInput.setAttribute('id', 'image-' + athIndex_imageIndexIdNum);
            fileInput.addEventListener('change', function (event) {
                var self = this;
                var index = (this.id).substr(-1);

                var file = fileInput.files[0];
                if (file) {
                    //获取文件的名称
                    var name = file.name;
                    //获取文件的扩展名
                    var ext = name.substr(name.lastIndexOf('.') + 1);
                    //控制上传文件大小
                    //if (size > (10*1024*1024)) {
                    //	return mui.toast('文件超大,请重新选择~');
                    //}


                    if (!this.parentNode.classList.contains('space')) { //已有图片
                        feedback.files.splice(index - 1, 1, { name: "images" + index, path: e });
                    }

                }
                //处理文件上传
                var IsUpSuccess = uploadFile(fileInput.files, FK_FrmAttachment.replace("_" + pageData.FK_Node, ""));
                //如果上传成功后处理事件
                if (IsUpSuccess) {
                    feedback.clearAthFile();
                    feedback.initPage();
                }

            }, false);

            placeholder.appendChild(closeButton);
            placeholder.appendChild(fileInput);
            feedback.imageList.appendChild(placeholder);
        }

    };
    //feedback.newPlaceholder();
    feedback.initPage();
})();


function WXUpload(athMyPK, feedback) {
    //引入微信jshttps://res.wx.qq.com/open/js/jweixin-1.6.0.js
    //Skip.addJs("https://res.wx.qq.com/open/js/jweixin-1.6.0.js");
    //Skip.addJs("https://res2.wx.qq.com/open/js/jweixin-1.6.0.js");
    var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_CCForm");
    var url = GetHrefUrl().split('#')[0];
    handler.AddPara("htmlPage", url);
    var data = handler.DoMethodReturnString("GetWXGZHConfigSetting");
    if (data.indexOf("err@") != -1) {
        alert(data);
        return false;
    }
    var pushData = JSON.parse(data);

    var appId = pushData.AppID;
    var timestamp = pushData.timestamp;
    var nonceStr = pushData.nonceStr;
    var signature = pushData.signature;
    wx.config({
        debug: false,
        appId: appId,
        timestamp: timestamp,
        nonceStr: nonceStr,
        signature: signature,
        jsApiList: [
            // 所有要调用的 API 都要加到这个列表中
            'chooseImage',
            'previewImage',
            'uploadImage',
            'downloadImage'
        ]
    });
    wx.ready(function () {
        wx.checkJsApi({
            jsApiList: [
                'chooseImage',
                'previewImage',
                'uploadImage',
                'downloadImage'
            ],
            success: function (res) {
                //alert(JSON.stringify(res));
                //alert(JSON.stringify(res.checkResult.getLocation));
                if (res.checkResult.getLocation == false) {
                    alert('你的微信版本太低，不支持微信JS接口，请升级到最新的微信版本！');
                    return;
                } else {
                    wxChooseImage(athMyPK, feedback);
                }
            }
        });
    });
    wx.error(function (res) {
        // config信息验证失败会执行error函数，如签名过期导致验证失败，具体错误信息可以打开config的debug模式查看，也可以在返回的res参数中查看，对于SPA可以在这里更新签名。
        alert("验证失败，请重试！");
        alert(JSON.stringify(res));
        wx.closeWindow();
    });
};
var images = {
    localId: [],
    serverId: []
};

function dataURLtoFile(dataurl, filename) {
    var arr = dataurl.split(','), mime = arr[0].match(/:(.*?);/)[1],
        bstr = atob(arr[1]), n = bstr.length, u8arr = new Uint8Array(n);
    while (n--) {
        u8arr[n] = bstr.charCodeAt(n);
    }
    return new File([u8arr], filename, { type: mime });
}

//拍照或从手机相册中选图接口
function wxChooseImage(athMyPK, feedback) {
    wx.chooseImage({
        success: function (res) {
            debugger;
            console.log('choose image callback', res);
            images.localId = res.localIds;
            //alert('已选择 ' + res.localIds.length + ' 张图片');
            if (images.localId.length == 0) {
                alert('请先使用 chooseImage 接口选择图片');
                return;
            }
            var i = 0, length = images.localId.length;
            images.serverId = [];

            function upload(athMyPK, feedback) {
                //图片上传
                wx.uploadImage({
                    localId: images.localId[i],
                    success: function (res) {
                        i++;
                        images.serverId.push(res.serverId);
                        //图片上传完成之后，进行图片的下载，图片上传完成之后会返回一个在腾讯服务器的存放的图片的ID--->serverId
                        wx.downloadImage({
                            serverId: res.serverId, // 需要下载的图片的服务器端ID，由uploadImage接口获得
                            isShowProgressTips: 1, // 默认为1，显示进度提示
                            success: function (res) {
                                var localId = res.localId; // 返回图片下载后的本地ID
                                //通过下载的本地的ID获取的图片的base64数据，通过对数据的转换进行图片的保存
                                wx.getLocalImgData({
                                    localId: localId, // 图片的localID
                                    success: function (res) {
                                        console.log(res);
                                        var localData = res.localData; // localData是图片的base64数据
                                        if (localData.indexOf('data:image') != 0) {
                                            //判断是否有这样的头部
                                            localData = 'data:image/jpeg;base64,' + localData
                                        }
                                        localData = localData.replace(/\r|\n/g, '').replace('data:image/jgp', 'data:image/jpeg');
                                        var file = dataURLtoFile(localData, Date.now() + '.jpg');
                                        var handler = new HttpHandler("BP.WF.HttpHandler.CCMobile_CCForm");
                                        handler.AddPara("AttachPK", athMyPK);
                                        handler.AddPara("WorkID", GetQueryString("WorkID"));
                                        handler.AddPara("PKVal", GetQueryString("WorkID"));
                                        handler.AddFile(file);
                                        //handler.AddPara("ImageData", localData);
                                        /*console.log(encodeURIComponent(localData))*/
                                        var data = handler.DoMethodReturnString("WXGZH_AthUpload");
                                        if (data.indexOf("err@") != -1) {
                                            //mui.alert("上传失败");
                                            console.error('上传失败')
                                        }
                                        if (i == length) {
                                            feedback.clearAthFile();
                                            feedback.initPage();
                                        }
                                    }
                                });
                            }

                        });
                        if (i < length) {
                            upload(athMyPK, feedback);
                        } else {
                            //alert("初始化页面");
                            //feedback.initPage();
                        }
                    },
                    fail: function (res) {
                        alert(JSON.stringify(res));
                    }
                });
            }
            upload(athMyPK, feedback);
        }
    });
}

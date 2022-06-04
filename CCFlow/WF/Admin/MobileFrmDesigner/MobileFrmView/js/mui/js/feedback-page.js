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
        Url = dynamicHandler + "?DoType=HttpHandler&DoMethod=" + doMethod + "&HttpHandlerName=" + httpHandlerName + "&FK_FrmAttachment=" + GetQueryString("FK_FrmAttachment")  + "&PKVal=" + GetQueryString("PKVal")  + "&t=" + new Date().getTime();
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
        feedback.files.push({ name: "images-" + athIndex + "-" + athIndex_index, path: athDB.FileFullName, id: "img-" + athIndex + "-" + athIndex_index, myPk: athDB.MyPK, FileExts: athDB.FileExts, FileName: athDB.FileName, Rec: athDB.Rec, FK_FrmAttachment: athDB.FK_FrmAttachment, NoOfObj: athDB.NoOfObj });
        //index++;
    };

    feedback.initFile = function (ath, FK_FrmAttachment, athIndex, athIndex_imageIndexIdNum,noOfObj) {
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
                    closeButton.id = "img-" + athIndex + "-" + index;
                    //小X的点击事件
                    closeButton.addEventListener('click', function (event) {
                        var btnArray = ['否', '是'];
                        mui.confirm('您确定要删除吗？ ', '提示', btnArray, function (e) {
                            if (e.index == 1) {
                                for (var temp = 0; temp < feedback.files.length; temp++) {
                                    if (feedback.files[temp].id == closeButton.id) {
                                        //处理删除事件
                                        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
                                        handler.AddPara("DelPKVal", f.myPk);
                                        var data = handler.DoMethodReturnString("AttachmentUpload_Del");
                                        if (data == "删除成功.") {
                                            placeholder = document.getElementById(f.myPk);
                                            feedback.files.splice(temp, 1);
                                            feedback.imageList.removeChild(placeholder);
                                        }
                                    }
                                }
                            }
                        });
                        return false;
                    }, false);

                    placeholder.appendChild(closeButton);
                }
            }

            //点击图片标记的时候下载文件.
            if (ath.IsDownload == 1) {
                divButton.addEventListener('click', function (event) {
                    //处理文件下载事件
                    downLoad(f.myPk);
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
            //获取NoOfObj
            var index = imageId.lastIndexOf("_");  
            var noOfObj = imageId.substr(index + 1);
            
            feedback.newPlaceholder(mypk, athIndex,noOfObj);
        }
    }
    /**
    * 初始化图片域占位
    */
    feedback.newPlaceholder = function (FK_FrmAttachment, athIndex,noOfObj) {
        var fileInputArray = feedback.getFileInputArray();
        if (fileInputArray &&
			fileInputArray.length > 0 &&
			fileInputArray[fileInputArray.length - 1].parentNode.classList.contains('space')) {
            return;
        };
        var ath = new Entity("BP.Sys.FrmAttachment", FK_FrmAttachment);
        var athIndex_imageIndexIdNum = 0;
        //初始化显示上传的文件
        feedback.initFile(ath, FK_FrmAttachment, athIndex, athIndex_imageIndexIdNum,noOfObj);
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
        var fileInput = document.createElement('input');
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('accept', '*/*');
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
                //处理文件上传
                var IsUpSuccess = uploadFile(file, FK_FrmAttachment);
                //如果上传成功后处理事件
                if (IsUpSuccess) {
                    feedback.clearAthFile();
                    feedback.initPage();
                }
            }

        }, false);
        placeholder.appendChild(closeButton);
        placeholder.appendChild(fileInput);
        feedback.imageList.appendChild(placeholder);
    };
    //feedback.newPlaceholder();
    feedback.initPage();
})();
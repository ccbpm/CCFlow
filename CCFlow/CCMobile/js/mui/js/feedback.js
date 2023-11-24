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
    feedback.files = [];
    feedback.uploader = null;

    /**
    *提交成功之后，恢复表单项 
    */
    feedback.clearAthFile = function () {
        for (var athIndex = 0; athIndex < feedback.imageLists.length; athIndex++) {
            feedback.imageLists[athIndex].innerHTML = '';
        }
        feedback.filles = [];
    };
    feedback.getFileInputArray = function () {
        return [].slice.call(feedback.imageList.querySelectorAll('.file'));
    };
    feedback.addFile = function (athDB, athIndex, athIndex_index) {
        feedback.files.push({ name: "images-" + athIndex + "-" + athIndex_index, path: athDB.FileFullName, id: "img-" + athIndex + "-" + athIndex_index, myPk: athDB.MyPK, FileExts: athDB.FileExts, FileName: athDB.FileName, Rec: athDB.Rec, FK_FrmAttachment: athDB.FK_FrmAttachment });
    };

    feedback.initFile = function (ath, FK_FrmAttachment, athIndex, athIndex_imageIndexIdNum) {
        var data = GetAllAttachments(ath);
        var athIndex_index = 0;
        var athIndex_imageIndexIdNum = 0;
        $.each(data, function (i, obj) {
            feedback.addFile(obj, athIndex, athIndex_index);
            athIndex_index++;
        });

        //判断当前是可读还是可编辑
        var IsRead = pageData.IsReadOnly == "1" ? true : false;
        //显示上传文件
        $.each(feedback.files, function (index, element) {
            var f = feedback.files[index];
            if (f.FK_FrmAttachment != FK_FrmAttachment)
                return;
            athIndex_imageIndexIdNum++;

            var placeholder = document.createElement('div');
            placeholder.setAttribute('class', 'image-item');
            var divButton = document.createElement("div");
            divButton.style.width = '100%';
            divButton.style.height = '100%';
            if (IsRead == false) {
                var IsDelete = false;
                //删除所有
                if (ath.DeleteWay == 1)
                    isDelete = true;
                //只删除自己上传的
                if (ath.DeleteWay == 2) {
                    if (f.Rec == new WebUser().No)
                        isDelete = true;
                }
                if (IsDelete) {
                    //删除图片
                    var closeButton = document.createElement('div');
                    closeButton.setAttribute('class', 'image-close');
                    closeButton.innerHTML = 'X';
                    closeButton.id = "img-" + athIndex + "-" + index;
                    //小X的点击事件
                    closeButton.addEventListener('tap', function (event) {
                        setTimeout(function () {
                            for (var temp = 0; temp < feedback.files.length; temp++) {
                                if (feedback.files[temp].id == closeButton.id) {
                                    //删除事件
                                    var isDeSucs = Del(f.mypk);
                                    if (isDeSucs)
                                        feedback.files.splice(temp, 1);
                                }
                            }
                            if (isDeSucs)
                                feedback.imageList.removeChild(placeholder);
                        }, 0);
                        return false;
                    }, false);

                    placeholder.appendChild(closeButton);
                }
            }
            //点击图片时下载文件
            if (ath.IsDownload == 1) {
                divButton.addEventListener('click', function (event) {
                    //下载文件
                    DownLoad(f.myPk);
                });
            }

            var imgFlag = IsImgExt(f.FileExts);
            var dataUrl = "./image/FileType/" + f.FileExts + "B.gif";
            if (imgFlag)
                dataUrl = GetFileStram(f.myPk);

            placeholder.style.backgroundImage = 'url(' + dataUrl + ')';
            placeholder.appendChild(divButton);
            //文件名称显示
            var nameLab = document.createElement('div');
            nameLab.setAttribute('class', 'image-name');
            nameLab.id = "name-" + athIndex + "-" + index;
            nameLab.innerHTML = "<p style='text-align:center;width:95px'>" + f.FileName.split(".")[0] + "</p>";
            if (f.FileName.split(".")[0].length >= 6)
                nameLab.innerHTML = "<p style='width:95px;margin:0;padding:0'>" + f.FileName.split(".")[0].substr(6) + "...</p>";

            placeholder.appendChild(nameLab);
            feedback.imageList.appendChild(placeholder);
        });
    }

    /**
    *   初始化上传文件页面
    */

    feedback.initPage = function () {
        for (var athIndex = 0; athIndex < feedback.imageLists.length; athIndex++) {
            feedback.imageList = feedback.imageLists[athIndex];
            //获取ID
            imageId = feedback.imageList.id;
            var mypk = imageId.substr(11);
            feedback.newPlaceholder(mypk, athIndex);
        }
    }
    /**
    * 初始化图片域占位
    */
    feedback.newPlaceholder = function (FK_FrmAttachment, athIndex) {
        var fileInputArray = feedback.getFileInputArray();
        if (fileInputArray &&
			fileInputArray.length > 0 &&
			fileInputArray[fileInputArray.length - 1].parentNode.classList.contains('space')) {
            return;
        };
        var ath = new Entity("BP.Sys.FrmAttachment", FK_FrmAttachment);
        var athIndex_imageIndexIdNum = 0;
        //初始化显示上传的文件
        feedback.initFile(ath, FK_FrmAttachment, athIndex, athIndex_imageIndexIdNum);
        //如果只读可以返回
        if (pageData.IsReadOnly == "1" || ath.IsUpload == false)
            return;
        //添加上传图片的控件
        athIndex_imageIndexIdNum++;
        var placeholder = document.createElement('div');
        placeholder.setAttribute('class', 'image-item space');
        //删除图片
        var closeButton = document.createElement('div');
        closeButton.setAttribute('class', 'image-close');
        closeButton.innerHTML = 'X';
        closeButton.id = "img-" + athIndex_imageIndexIdNum;
        //小X的点击事件
        closeButton.addEventListener('tap', function (event) {
            setTimeout(function () {
                for (var temp = 0; temp < feedback.files.length; temp++) {
                    if (feedback.files[temp].id == closeButton.id) {
                        //删除事件
                        var isDeSucs = Del(f.mypk);
                        if (isDeSucs)
                            feedback.files.splice(temp, 1);
                    }
                }
                if (isDeSucs)
                    feedback.imageList.removeChild(placeholder);
            }, 0);
            return false;
        }, false);

        //
        var fileInput = document.createElement('input');
        fileInput.setAttribute('type', 'file');
        fileInput.setAttribute('accept', '*/*');
        fileInput.setAttribute('id', 'image-' + athIndex_imageIndexIdNum);
        fileInput.addEventListener('change', function (event) {
            var self = this;
            var index = (this.id).substr(-1);
            var file = fileInput.files[0];
            if (file) {
                //获取文件名称
                var name = file.name;
                //获取文件的后缀名
                var ext = name.substr(name.lastIndexOf(".") + 1);
                if (!self.parentNode.classList.contains('space')) { //已有图片
                    feedback.files.splice(index - 1, 1, { name: "images" + index, path: e });
                }
                //文件上传
                var IsUploadSuc = uploadFile(file, FK_FrmAttachment);
                if (IsUploadSuc) {
                    feedback.clearAthFile();
                    feedback.initPage();
                }

            }

        }, false);
        placeholder.appendChild(closeButton);
        placeholder.appendChild(fileInput);
        feedback.imageList.appendChild(placeholder);
    };
    feedback.initPage();



})();

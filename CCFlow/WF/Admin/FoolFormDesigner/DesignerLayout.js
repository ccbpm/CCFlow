function getQueryVariable(variable) {
    var query = window.location.search.substring(1);
    var vars = query.split("&");
    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");
        if (pair[0] == variable) {
            return pair[1];
        }
    }
    return false;
}

var currElem = null
function allowDrop(evt) {
    evt.preventDefault()
    var elem = evt.target
    elem.style.border = '1px solid #459dff'
}

function dropToForm(event) {
    var data = JSON.parse(event.dataTransfer.getData("fieldInfo"))
    layui.use(['pinyin'], function () {
        var pinyin = layui.pinyin
        MenuClick(data.id, pinyin)
    })
}

function removeClassList (node){
    var classes = Array.from(node.classList)
    for(var i = 0; i < classes.length; i++){
        var cls = classes[i]
        if(cls.indexOf('layui-col') > -1){
            node.classList.remove(cls)
        }
    }
}

function getLength(node){
    var classes = Array.from(node.classList)
    for(var i = 0 ;i < classes.length ; i++){
        if(classes[i].indexOf('layui-col') > -1){
            return Number(classes[i].replace('layui-col-xs','')) / 3
        }
    }
    return ""
}

new Vue({
    el: '#entry',
    data() {
        return {
            fieldsGroup: eleMenus,
            labelPosition: 'top',
            fieldPickerVisible: false,
            attrsPanelVisible: false,
            currentEditField: {
                type: '',
                name: '',
                symbol: '',
                percent: '',
                dom: '',
                id: ''
            },
        }
    },
    methods: {
        dragEnd(){
        },
        // 渲染组件列表
        renderFieldPanel(layout) {
            if (!layout) layout = '1'
            this.fieldPickerVisible = layout === '0'
        },
        startDrag(evt, target) {
            evt.dataTransfer.setData('fieldInfo', JSON.stringify(target))
        },
        updateLayout(type) {
            if (!type) type = '0'
            if (type === '1') {
                setTimeout(function () {
                    document.querySelectorAll(".FoolFrmFieldRow").forEach(node => {
                        node.style.display = 'flex'
                        node.style.flexDirection = 'column'
                        node.querySelector('.FoolFrmFieldLabel').style.width = '100%'
                        node.querySelector('.FoolFrmFieldInput').style.width = '100%'
                        // removeClassList(node)
                        // node.classList.add("layui-col-xs3")
                    })
                }, 100)
            }
        },
        showAttrs(input) {
            this.currentEditField = {
                type: '',
                name: '',
                symbol: '',
                percent: '',
                dom: '',
                id: '',
            }
            this.attrsPanelVisible = true
            this.currentEditField.dom = input.closest(".FoolFrmFieldRow")
            this.currentEditField.percent = getLength(this.currentEditField.dom)
            this.currentEditField.id = input.id
            this.currentEditField.name = this.currentEditField.dom.querySelector("span").innerHTML
            console.log(this.currentEditField.id)
        },
        bindInputEvents() {
            var _this = this
            setTimeout(function () {
                var container = document.querySelector('.layui-container')
                const inputList = container.querySelectorAll("input")
                inputList.forEach(function (input) {
                    input.addEventListener("click", function () {
                        _this.showAttrs(input)
                    })
                })
            }, 100)
        },
        changeRows(length){
            removeClassList(this.currentEditField.dom)
            this.currentEditField.dom.classList.add("layui-col-xs"+length * 3)
            this.currentEditField.percent = length
        },
        handleFormEditor() {
            var frmId = getQueryVariable("FK_MapData")
            var modelAttr = new Entity("BP.Sys.MapData", frmId)
            var layout = modelAttr.GetPara("DModel")
            this.renderFieldPanel(layout)
            var fieldStyle = modelAttr.GetPara("FieldModel")
            this.updateLayout(fieldStyle)
        },
        // 保存属性至服务器
        saveToServer(){
            var name = this.currentEditField.name
            var id = this.currentEditField.id
        }
    },
    mounted() {
        this.handleFormEditor()
        this.bindInputEvents()

    }
})


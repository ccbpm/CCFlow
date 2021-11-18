function handleDeptContextMenu(data, object) {
    data.id === 'Delete' ? Del($(this.elem)[0].dataset.id, $(this.elem)[0].dataset.pid, '1') : DoUnEnable($(this.elem)[0].dataset.id, $(this.elem)[0].dataset.pid)
}

function handlePtjContextMenu(data, object) {
    data.id === 'Delete' ? Del($(this.elem)[0].dataset.id, $(this.elem)[0].dataset.pid, '1') : Remove($(this.elem)[0].dataset.id)
}

function handleBlContextMenu(data, object) {
    data.id === 'Delete' ? Del($(this.elem)[0].dataset.id, $(this.elem)[0].dataset.pid, '1') : DoEnable($(this.elem)[0].dataset.id, $(this.elem)[0].dataset.pid)
}

function bindContextMenu() {
    var _this = this
    layui.use('dropdown', function () {
        var dropdown = layui.dropdown
        var deptMenuOptions = [
            {title: '<i class=icon-share-alt ></i> 删除', id: "Delete", Icon: "icon-plus"},
            //{ title: '新建下级目录', id: 5 },
            {title: '<i class=icon-close></i> 禁用', id: "BlackList", Icon: "icon-close"}
        ]

        var ptjMenuOptions = [
            {title: '<i class=icon-share-alt ></i> 删除', id: "Delete", Icon: "icon-plus"},
            //{ title: '新建下级目录', id: 5 },
            {title: '<i class=icon-close></i> 移除', id: "BlackList", Icon: "icon-close"}
        ]

        var blMenuOptions = [
            {title: '<i class=icon-share-alt ></i> 删除', id: "Delete", Icon: "icon-plus"},
            //{ title: '新建下级目录', id: 5 },
            {title: '<i class=icon-close></i> 启用', id: "BlackList", Icon: "icon-close"}
        ]

        var tRenderOptions = [{
            elem: '.dept-dropdown-item',
            trigger: 'contextmenu',
            data: deptMenuOptions,
            click: handleDeptContextMenu
        }, {
            elem: '.dept-dropdown-btn',
            trigger: 'click',
            data: deptMenuOptions,
            click: handleDeptContextMenu
        }, {
            elem: '.ptj-dropdown-item',
            trigger: 'contextmenu',
            data: ptjMenuOptions,
            click: handlePtjContextMenu
        }, {
            elem: '.ptj-dropdown-btn',
            trigger: 'click',
            data: ptjMenuOptions,
            click: handlePtjContextMenu
        }, {
            elem: '.bl-dropdown-item',
            trigger: 'contextmenu',
            data: blMenuOptions,
            click: handleBlContextMenu
        }, {
            elem: '.bl-dropdown-btn',
            trigger: 'click',
            data: blMenuOptions,
            click: handleBlContextMenu
        }]

        for (var i = 0; i < tRenderOptions.length; i++) {
            dropdown.render(tRenderOptions[i])
        }

    })
}


/**
 * 更新排序
 * @param type 三种 部门|兼职|黑名单 （dept|ptj|bl）
 * @param arr  排序后数组 eg: ['lisi','zhangsan']
 */
function updateSort(type,arr){
    switch (type){
        // 部门
        case 'dept':
            break
        // 兼职
        case 'ptj':
            break
        // 黑名单
        case 'bl':
            break
    }
}

function activePlugin(container,type){
    new Sortable(container, {
        animation: 150,
        dataIdAttr: 'data-id',
        ghostClass: 'blue-background-class',
        onEnd: function (evt) {
            updateSort(type,this.toArray())
        }
    });
}


function bindSortableJs() {
    activePlugin(document.querySelector('.dept-sort'),'dept')
    activePlugin(document.querySelector('.part-time-sort'),'ptj')
    activePlugin(document.querySelector('.blacklist-sort'),'bl')
}



//* ====== 设置单位尺寸 ====== */
$.fn.initSet = function(x,y){
	var win = $(this);
	w_rem = win.width() / x,
    h_rem = win.height() / y;
    if(h_rem > w_rem){
		rem = h_rem;
	}else{
		rem = w_rem;
    }
	$('head').append('<style type="text/css">html{font-size:' + rem + 'px!important;}</style>');
	return rem;
};

/* 根据当前栏目的标题条高度，重定位下方内容区域距离顶条的高度，避免被遮挡 */
var relayout = function(){
	var titleArr = $('.main > .content .titleBar');
	var contentArr = $('.main > .content .content');
	for(var i=0;i<titleArr.length;i++){
		contentArr.eq(i).css('top',(titleArr[i].offsetHeight) + 'px');
	}
};

//* 目录生成 */
$.fn.setMenu = function(data){
	html = '<div>\n'
		 + '    <ul>\n';
	$.each(data,function(i,o){
		html += '        <li class="' + (i==0?'open':'') + '"><div><i class="icon"></i> <div class=""><span>[' + (i+1) + ']' + o.name + '</span></div></div>\n'
			  + '            <ul>\n';
		$.each(o.value,function(j,p){
			console.log('zoom = ' + p.zoom + ' , p.url = ' + p.url + ' , p.name = ' + p.name);
			html += '                <li class="leafItem ' + (i==0&&j==0?'active':'') + '"><div><div class="doc" ' + (p.zoom?'zoom="zoom' + p.zoom + '" ':'') + 'title="' + p.url + '"><span>[' + (i+1) + '-' + (j<9?'0'+(j+1):(j+1)) + ']' + p.name + '</span></div></div></li>\n';
		});
		html += '            </ul>\n'
			  + '        </li>\n';
	});
	html += '    </ul>\n'
		  + '</div>';
	$(this).append(html);

	$('.doc').on('click',function(e){
        var dom = $(this),
            li = dom.parents('li').eq(0),
            list = dom.parents('.treeList').eq(0).find('.leafItem');
        if(!li.hasClass('active')){
            list.removeClass('active');
            li.addClass('active');
			$('#docWrap').attr('src',dom.attr('title')).parents('.iframeWrap').eq(0).attr('class','iframeWrap ' + dom.attr('zoom'))
			
        }
    });
    
};



/*
 * 窗体拖动方法
 * 参数说明：parentWrapSelector：容器选择器，必要； activeSelector：拾起状态类名，必要
 */

$.fn.dragTools = function(parentWrapSelector,activeSelector){
    $(this).mousedown(function(e) {
        var dom = $(this);
        var positionDiv = $(this).offset();
        var distenceX = e.pageX - positionDiv.left;
        var distenceY = e.pageY - positionDiv.top;
        var block = dom.parents(parentWrapSelector).eq(0);
        block.addClass(activeSelector)
        $(document).mousemove(function(e) {
            var x = e.pageX - distenceX;
            var y = e.pageY - distenceY - 0*rem;
    
            if (x < 0) {
                x = 0;
            } else if (x > $(document).width() - block.outerWidth(true)) {
                x = $(document).width() - block.outerWidth(true);
            }
    
            if (y < 0) {
                y = 0;
            } else if (y > $(document).height() - block.outerHeight(true) - 0*rem) {
                y = $(document).height() - block.outerHeight(true) - 0*rem;
            }
    
            block.css({
                'left': x + 'px',
                'top': y + 'px'
            });
        });
    
        $(document).mouseup(function() {
            $(document).off('mousemove');
            block.removeClass(activeSelector)
        });
    });
};

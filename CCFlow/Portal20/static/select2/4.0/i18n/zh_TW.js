(function(){if(jQuery&&jQuery.fn&&jQuery.fn.select2&&jQuery.fn.select2.amd){var a=jQuery.fn.select2.amd}return a.define("select2/i18n/zh_TW",[],function(){return{inputTooLong:function(c){var b=c.input.length-c.maximum,d="請刪掉"+b+"個字元";return d},inputTooShort:function(c){var b=c.minimum-c.input.length,d="請再輸入"+b+"個字元";return d},loadingMore:function(){return"載入中…"},maximumSelected:function(c){var b="你只能選擇最多"+c.maximum+"項";return b},noResults:function(){return"沒有找到相符的項目"},searching:function(){return"搜尋中…"}}}),{define:a.define,require:a.require}})();
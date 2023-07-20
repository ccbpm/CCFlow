(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['exports', 'echarts'], factory);
    } else if (typeof exports === 'object' && typeof exports.nodeName !== 'string') {
        // CommonJS
        factory(exports, require('echarts'));
    } else {
        // Browser globals
        factory({}, root.echarts);
    }
}(this, function (exports, echarts) {
    var log = function (msg) {
        if (typeof console !== 'undefined') {
            console && console.error && console.error(msg);
        }
    }
    if (!echarts) {
        log('ECharts is not Loaded');
        return;
    }
    if (!echarts.registerMap) {
        log('ECharts Map is not loaded')
        return;
    }
    echarts.registerMap(
        'china',
        {
            "type":"FeatureCollection",
            "features":[
                {type:"Feature",id:"2102",properties:{name:"大连市",cp:[122.2229,39.4409],childNum:5},geometry:{type:"Polygon",coordinates:["@@IÞmVk@wXWÜbnwlLnU@nLlbXW@awnbl@XLa@Ċ¥@LULnJ@xVnmV@VXXV@VJkn@VÜKXXôJlbxl@IVbnJVLUbnlnVwJVU@XUaUUlwn@°nVKnV°_VJwl@nwlVIXWlIVVnK@IWmkIVaVU@WÈUlmU@UWUalkXġŻ@kI»mmakUmĉUŁV»²ġVĕ@aUU؍IɃ`ȃ@kw@Umwĉ@WķÑIĉÇbÝLkymbIwÇmÛbmbU¯ÜõÈkÆVbŎxnXVÆnǪ¦b¤UxÝnĉÒmĊVÈ¤ÈbÆ¼ĀÆÆÞźbVVbX°²¤"],encodeOffsets:[[124786,41102]]}},
                {type:"Feature",id:"2113",properties:{name:"朝阳市",cp:[120.0696,41.4899],childNum:6},geometry:{type:"Polygon",coordinates:["@@na@UVI@mÑWkaV¥UI@wl@aÈbm@wVak@@K@k@a@UUmUUalmU@KÇUÅ±¯@±kUKVkUaaU@¥m@@¯k@WLUmkn@mmIkm@amU@wVmkU@Klk@UmaXIWWULaULVbmk@UUmUk±_Uym@mbkImaX¯WWxWKzU@WkJWwkV@Um@UbVVVVXb@VWX@W@Vkb@VnUK±aUUlwXÇWKknU@mmUkLUVVUUVUawbkKmwnIkJ@nmb`kmVkLWwUm@UUUK@UmaUa@UUaWK@mU¯Wkk¯VmUUxVXUVmL¯ymXkWUbmXUKVknWx¯JVnkLl@VVxnxlĀVL²WlXl@bÝVUn@bnlÜaXblIVl@@È¦@VmbXV@@xVVnUn@`°@VnXU@K@VV@VmbnVn@ln@bx°Ub@bLV`ÅnW@@lUnnWVU@Vbkl@Xl`XxVUblkX@°¦VUVVbUlkV@UbVbkLUxmJkX@bbxVKÆlXXbnnala@Uk@UVVklKVUXKVU°KVan@VUnLKVLWVaU_@mmUXa@mwXwVkVWXkk@k@klm@wXKl@U@KVUUUVaUV@alLxUx@b°°VnnVxlIXJmxLUVlV@bnX@VbaVx@XJ@bn@VVXÈl@llX@lUVô°°@ÞVbn@Vk@VW"],encodeOffsets:[[123919,43262]]}},
                {type:"Feature",id:"2106",properties:{name:"丹东市",cp:[124.541,40.4242],childNum:4},geometry:{type:"Polygon",coordinates:["@@lzXJU@²x@@V@bUVmKUn°n@lnVKnV@n@VlV°WbXn@VzJ@¦@bkbbUl@bkbJ¯zWULWbklVnb¦VJ@K°Ukl@@WbVn°@Vm²UnX`UÜLXmVXlKVbUVVnUbnX@VUL@lUbWx@²kl`n@Vlb@nUVWVLVU@aV@²bl@ÈmxWXVÈUJVl@laWnXKÈkÈ@Va°bÆm@XV°IVV°UnalVUn@UwVU@@VVJI@bl@XK@wWmXUUVbkJVXnJVI@mknwlKXL@`l@VI@UUaVKÞnaVm@aÇ£XWU@aÇUU@mbkKm£@WWL@@Kk@klUbWKUkUU¯UõÛmUUaVUU@WU_W@kVkJ_WKkV@bUL¯¯±mk¯ġğÑ@UmwKUaka@am¥ÝIUWmk@wmţLKʝbȗKWĢklVbX@VVknÇV@XUVUblJXn@J"],encodeOffsets:[[126372,40967]]}},
                {type:"Feature",id:"2112",properties:{name:"铁岭市",cp:[124.2773,42.7423],childNum:7},geometry:{type:"Polygon",coordinates:["@@XJm@¯mXUlnVbUJU@bV@UJWL@VXLmJVbkXlJXxVL@b@V@n@b@`Vbk@lxknV@VVV@bUL@bV@@bVK@VXLWLXJ@LV@nbWJ@IUVx@LVJUXVxVx@VV@@LXJWL@VU@@L@VnL@bVVmVX@@VVInJmbnLWVnVULVVU@VVmX@@JVzl@nVVKVXÞ@mk_lmUUWV_nJlUÞÑÞVVUVVLUVJ@IVna@@KV@XwWknwnKlalUwaĉÝwJl_@aUaKUUU@WU@WXUÆ@@UVK@n@UnVVblK@bllb@bbW@Xbl@UlnLl°°b¦nKlVnIV@UWU@WXkw@am@nm@aVw@I@KUaVIm±XÑlknJVnVJaX_VaUaVKmwnkmmn@lU@U@mnaXlKUmUIVmklaUK@UlUVUW@UkVma@UUU@JmUU@@bmbKWV¯XUKm@ka@UVKVk@aUKmLkKUUÝUmbXbÇJ@k@WU_@m@klm@UXKVaUI@KWUXaÇWkaWUkWUL±U@lUU@UJI@V¯JmIm@@aU@Uwa@UV@VkIV¯aUkWkb@bVL@@VVVUXW@Ua@@bÝbUVÝ@LmUkVUbVllLUV@LXWbUXm@U`@kxlnnJlbnIllLXlVlUXmVKnV@L"],encodeOffsets:[[126720,43572]]}},
                {type:"Feature",id:"2101",properties:{name:"沈阳市",cp:[123.1238,42.1216],childNum:5},geometry:{type:"Polygon",coordinates:["@@ȚĊÜ°bLlÞxUbUn±@ÈnVÆL@xnLlUVbxkImJkn@V±LUxkV@bbKVKnzVl@L°@VaxÞUlbôxVV@@V±bn@llXLöXĶnal@nkVJVI@aU@@aVK@aUUUU@lmkwl@Ua@_@a@m@U@aUKWwkIlWUanIWK@UXKVIU@@aVVIUamVknW°n@WI@KUmULWnkVkUWKkkmJkamIkmlw@V_n@VWXaW@KVUkKUkValUnVK@ÞVUÞa@a@VbX@VWUU@U@UK@ala@IkKmUUa@U@VkkWVwU_@KÜUXbl@V¥XUVmXakÅlUUkIm`UIUJW@UIKmkm@UUJImmU@VUXU`mIUbUK@LJUUl@X@UbJkU@nm@Uam@@aUmLKwmWXUK@kUaÇa@JUIUa@aKVUUXmUy_@lmbkLUKWLX`n@bVL@JXLWX@Vnb@Vm@UbnVmL@V@x@LUbVV@V@LUVl@mb¯U@xU@UVVV@X@VVblJ@bnVKUnx@llnL±¤b@k`VXÆK@kV@¼kl@bWIUl@VmLnbm@@JXXmb"],encodeOffsets:[[125359,43139]]}},
                {type:"Feature",id:"2104",properties:{name:"抚顺市",cp:[124.585,41.8579],childNum:4},geometry:{type:"Polygon",coordinates:["@@XVl°bUlJ@UVU@bVxV@@bn@nJ°I@UJIVV@V@k²VVKlXXVblÈXWbXV@LVJUbWL@Vkn@l@nV`@X@lÈIWanaÞVVVlLnKVL@bUlUL@Vlbn@VL°WXULna@aV@nV@IVV@VbUnl@VXnKVa@UUnyWkXaaVk@aabnm@_WKXmWanU@alaUl@XJVLVxX@wnKnVlw@V_@a¯¥@UkKWUaUUanK@IaU@WUaVw@klUVyUUVUUÇ@Iôba@mnUma@kXa@UWak@Wal@a@WULmU@U`mIUU`mUk@@UUK±nkJbUam@kwm@@a@UU@Ua@@K@VK@kmKU_UKUUaĉWmkkL@`LnmlkLkbmK@k@Ulmb@b@xUVIUlmVXXxm@JUUk@WUk@akx±@¯x¯UmbKUUVmUU¯UmVVnWkÆlWbUnWVU¦k@WaÛV@LV`UxXllU@@VVbnVlL@J"],encodeOffsets:[[126754,42992]]}},
                {type:"Feature",id:"2114",properties:{name:"葫芦岛市",cp:[120.1575,40.578],childNum:4},geometry:{type:"Polygon",coordinates:["@@ll°XnV@XLVb@VVbnb@VLVV@VVnXxlKnUl_na@mlImJnxlLaxVbUVVUVUKVlnnV@lmXLÈWkxVV²bVLm@Ula@UX@XW@UWaUUUUVan@V@lUXxlIXV@yXLwXXW°nblJnan@Vz`l²nVVVl@nUaVKbVKnXVaUaVUynXK@kVK@X@m@mLXaLWU¯w@a@UVw¥°ó¯¯y¯UÇ¯»w¯Im¯ÇUUl¯»ţKċÑţķm¯w@mU_ómk¼VnU`±IkbVlnnU¼±Lk`@XWl¦UbmVUxkXVlkbllUVb@bkVmx@XVV@Jb±aULkKWXkWmX¯aUJmIkVm@xU@n"],encodeOffsets:[[122097,41575]]}},
                {type:"Feature",id:"2109",properties:{name:"阜新市",cp:[122.0032,42.2699],childNum:4},geometry:{type:"Polygon",coordinates:["@@Xnb°lVlnXVJLlVnl@zÆxnK@bblKVLn@@VaVLVK@L@Vl@XVVInVVKVwlUXwlKLVVb@aV@XlUXbVW@nlWnXKV@@V@XUVVLUVV@@bVVV@@ln@VbVUXVIxVanJ@UIVWL@UV@@¤V@nInwWklnIVxlnzUVÇJ¦VVÜLĸUnW@aV_WĊXXaKnkl@nmLa@alUVw²K@UlmnIlJwaVUkmK@wÅKmU@Ç²VmVaÝwkKaÛ¯șĉķ¥ğ¥@kUWkƏīÝ@@akUK@KWIUm¯nU¯JmwUVmIkJÇLm@UImJUU@aW@U@@nUbJabXVWn@UVmX@V@b@l@L@lUb@xnÇabk@@xVJU¦lbXÒ@nUJ@Vmb"],encodeOffsets:[[123919,43262]]}},
                {type:"Feature",id:"2107",properties:{name:"锦州市",cp:[121.6626,41.4294],childNum:5},geometry:{type:"Polygon",coordinates:["@@nJ@nlmVnXKl@@°n@@¦VbVbUlVL²l°@Æ²ÈV@LVknVbVVnnWVU@XmWUabIVa@mV@X@@bVVnIVJ@nÈKlInJVUnx°IV°mVnXJ@LLlV@b@ÞƐĬXllV@Ġ¦ĸ¦naWW@In@manK@UVkXJ@alk@»lU@ÅLUWl_@a²£Kkm@kwVmULm@akIUa@U@WUUVUaÝ@ğwkmĉ£UW@@bÇL@ma@_mKlXUwKLţÓ@UWw@K@UI@mU@UV¥@°UnJ°@@_KUwW@UnaWUmmI@mķwUaÇLóVĵwÝUUW¯¦Ux@Vb@xV°XKWbK@n@nW@UL@lWLmzUVVbUbmWXXWJbn@Vkl@LlVUn@xnV@bln"],encodeOffsets:[[123694,42391]]}},
                {type:"Feature",id:"2103",properties:{name:"鞍山市",cp:[123.0798,40.6055],childNum:4},geometry:{type:"Polygon",coordinates:["@@lxĠÞ@bV@@w°Vna@UkV@K@UUUVa@K@w@UnKmUVan@@Uma@UXWWK@IUK@amW_XKVLlKna@kmKVak@VU@VmU@anIÆan@aUVnb@blLV`ÞLlUbnaKn@naVU@¥°IVK@anUUKVaUVak@mJkXUVwkVUUa°U@W@WlkXWlIXUlJlaxIVVXLll@nLV@lLXlKĊz¥maUlkXaVKX°yIla@aVkala@a@¥IUy@WmXa¯kU@U@mmUULkmm@¯VmnLVU@a@U@±w@VWIkymLUUkJWXJkUmxk@xUI¯`mUULm¯m@kxVVbWV@UVIUx@bkVVVxUbVV@V@zJVXUlnk@@lkLlLUU±Jkm@UIUVLUVU@K@UnnV@l@LlaUJ@zn`@nWlIUVUUUV±Ln@nmL@VUVkLVlUxVLVlÅXma@@akLmWUX@JUnVJVkXJ@X@`WXVUVUIlbW@bVUVL@`Un@¦U`@bUV@z@Jm@@XV`LUL¯J@IVKmKÅI@JnWVnLnVxV¤z@bmV@VUV@bUL"],encodeOffsets:[[125123,42447]]}},
                {type:"Feature",id:"2105",properties:{name:"本溪市",cp:[124.1455,41.1987],childNum:3},geometry:{type:"Polygon",coordinates:["@@lb@VnlnVVUb@VJ@nnJ@bmXUx@xVbkbkWLUxnl@Ul@xWx@nUV@¼UllknkK@bmbnlLVJX@VIVJn_lJVVXUmnU°VVVUnVVLna°V°w²@lwbl@XVl@VVIn@wWWnUVkJVUw@@anaVk@@lnLlalKnkmK@_lKnlĊXVbVVLV`nL@lUL@@L@VbV@@V@bn@lxn@VbalI²mVL@Vl@nV_VVnJV_@nVKV@X@bkXbl@XblylUUk@Xa@UVIlK@UUWVULlm@UUUnKWU@K@UXmXVa@U°KVUUWUk@aUVKkaWkKUknaWa@U@m@mk@aUJk@@_WKkLmxl@nUJmIUWlIUaVWVXn@xWLk@@aJUI@U@UVVxm@UVkmb¯VUU¯JWU@Ån¯aUbÇ@ÇlLmWXkbk@UIÇVUXWwÇnk@±aU@@bUVUKUXmV@kaUm@k_±l@XwVa@kVK@UWmVaUmVUUakLUWWnÛKVW_m±VnU¯@Uma@Xk@l¯V"],encodeOffsets:[[126552,41839]]}},
                {type:"Feature",id:"2108",properties:{name:"营口市",cp:[122.4316,40.4297],childNum:4},geometry:{type:"Polygon",coordinates:["@@ĊĖÆn¤°Ċ¯ŎWô@xXbwnKl@nX@VUVKmL@VU@UxÝ@VlbxU@VUb@bk`IUlVUnV@@UV@@JnXlK@b@nbÆWUkUKVwUklKVU@UnK@mm²KVUVVVUJXk@mm_@yVIbk@K@kmUm@VLV@VUKVUVJn@l²IVVKklK@kl@kmVUWI@y@UUUVawUUUl@akmmVaUKmIUaJk@wkaóIWWÛL@UlmUIU@WW@UnUUm@wmIVK@Kĉ¦@bWKk@max@bWXkamK@mVkKmxÛaWX@xUlÝnJ"],encodeOffsets:[[124786,41102]]}},
                {type:"Feature",id:"2110",properties:{name:"辽阳市",cp:[123.4094,41.1383],childNum:5},geometry:{type:"Polygon",coordinates:["@@`VzWnVUVL@bVbVJ@IÈbVb@lVLXWnxLnKVb@n@Vbn@mV@lIVa@@WkVVI@KVLVanJV_VWUV@nnJVIVn@na@alLlmkVk@»VU@mXwwk@@VmkVwXKllaUa@wVwnW@amI@mUI@VaUUkmm@UkaL@UIĉyLWkkKU@mKk@kWKUUJwkbkIWVkJWXkl@X@X¯VVbUVlUxVWlnI@lUbVUbVLmV@bUL¯J@¦UVmbm@LmbakVÝKU_kK@amaVUbm@ÅbmJ@bVUn@UVl@UbnL"],encodeOffsets:[[125562,42194]]}},
                {type:"Feature",id:"2111",properties:{name:"盘锦市",cp:[121.9482,41.0449],childNum:3},geometry:{type:"Polygon",coordinates:["@@Vbĸx@nnJVnXmb@VXVxL@`¯@mI¯V@U¦@VV@nJ@V@LXx@VŤÔKLVxWknL@`b@nÈK@a@VXĊ¤nVK@aVU@UnU@ayU£UwmmKXUm@IÆJnLUL@J°IVKKU_@Wn@@I@yVU@aV_@¥Vm@_UKUV@aXkaVJVUUXW@_@WWIUlUIVm@IVW@IU@@VU@mUVVkJ_l@aVa@UVwka@UÞVwV@@UnKLVU@UmWk@mLxWa@wóUVUIÇÆĉ¦¯¦¯xʟJ"],encodeOffsets:[[124392,41822]]}}
            ],
        "UTF8Encoding":true}
    );
}));

/*
var kkk = {
    "type":"Feature",
    "id":"2111",
    "properties":{
        "name":"盘锦市",
        "cp":[121.9482,41.0449],
        "childNum":3
    },
    "geometry":{
        "type":"Polygon",
        "coordinates":["@@Vbĸx@nnJVnXmb@VXVxL@`¯@mI¯V@U¦@VV@nJ@V@LXx@VŤÔKLVxWknL@`b@nÈK@a@VXĊ¤nVK@aVU@UnU@ayU£UwmmKXUm@IÆJnLUL@J°IVKKU_@Wn@@I@yVU@aV_@¥Vm@_UKUV@aXkaVJVUUXW@_@WWIUlUIVm@IVW@IU@@VU@mUVVkJ_l@aVa@UVwka@UÞVwV@@UnKLVU@UmWk@mLxWa@wóUVUIÇÆĉ¦¯¦¯xʟJ"],
        "encodeOffsets":[[124392,41822]]
    }
}

*/
(function(f){var z={event:{CHECK:"ztree_check"},id:{CHECK:"_check"},checkbox:{STYLE:"checkbox",DEFAULT:"chk",DISABLED:"disable",FALSE:"false",TRUE:"true",FULL:"full",PART:"part",FOCUS:"focus"},radio:{STYLE:"radio",TYPE_ALL:"all",TYPE_LEVEL:"level"}},m={check:{enable:false,autoCheckTrigger:false,chkStyle:z.checkbox.STYLE,nocheckInherit:false,chkDisabledInherit:false,radioType:z.radio.TYPE_LEVEL,chkboxType:{Y:"ps",N:"ps"}},data:{key:{checked:"checked"}},callback:{beforeCheck:null,onCheck:null}},v=function(A){var B=y.getRoot(A);B.radioCheckedList=[]},d=function(A){},n=function(A){var B=A.treeObj,C=q.event;B.bind(C.CHECK,function(F,E,G,D){t.apply(A.callback.onCheck,[!!E?E:F,G,D])})},x=function(A){var B=A.treeObj,C=q.event;B.unbind(C.CHECK)},o=function(G){var H=G.target,J=y.getSetting(G.data.treeId),E="",B=null,C="",F="",A=null,D=null;if(t.eqs(G.type,"mouseover")){if(J.check.enable&&t.eqs(H.tagName,"span")&&H.getAttribute("treeNode"+q.id.CHECK)!==null){E=H.parentNode.id;C="mouseoverCheck"}}else{if(t.eqs(G.type,"mouseout")){if(J.check.enable&&t.eqs(H.tagName,"span")&&H.getAttribute("treeNode"+q.id.CHECK)!==null){E=H.parentNode.id;C="mouseoutCheck"}}else{if(t.eqs(G.type,"click")){if(J.check.enable&&t.eqs(H.tagName,"span")&&H.getAttribute("treeNode"+q.id.CHECK)!==null){E=H.parentNode.id;C="checkNode"}}}}if(E.length>0){B=y.getNodeCache(J,E);switch(C){case"checkNode":A=l.onCheckNode;break;case"mouseoverCheck":A=l.onMouseoverCheck;break;case"mouseoutCheck":A=l.onMouseoutCheck;break}}var I={stop:false,node:B,nodeEventType:C,nodeEventCallback:A,treeEventType:F,treeEventCallback:D};return I},u=function(D,H,G,A,F,C,E){if(!G){return}var B=D.data.key.checked;if(typeof G[B]=="string"){G[B]=t.eqs(G[B],"true")}G[B]=!!G[B];G.checkedOld=G[B];if(typeof G.nocheck=="string"){G.nocheck=t.eqs(G.nocheck,"true")}G.nocheck=!!G.nocheck||(D.check.nocheckInherit&&A&&!!A.nocheck);if(typeof G.chkDisabled=="string"){G.chkDisabled=t.eqs(G.chkDisabled,"true")}G.chkDisabled=!!G.chkDisabled||(D.check.chkDisabledInherit&&A&&!!A.chkDisabled);if(typeof G.halfCheck=="string"){G.halfCheck=t.eqs(G.halfCheck,"true")}G.halfCheck=!!G.halfCheck;G.check_Child_State=-1;G.check_Focus=false;G.getCheckStatus=function(){return y.getCheckStatus(D,G)}},a=function(C,E,B){var A=C.data.key.checked;if(C.check.enable){y.makeChkFlag(C,E);if(C.check.chkStyle==q.radio.STYLE&&C.check.radioType==q.radio.TYPE_ALL&&E[A]){var D=y.getRoot(C);D.radioCheckedList.push(E)}B.push("<span ID='",E.tId,q.id.CHECK,"' class='",j.makeChkClass(C,E),"' treeNode",q.id.CHECK,(E.nocheck===true?" style='display:none;'":""),"></span>")}},k=function(C,B){B.checkNode=function(H,G,I,F){var D=this.setting.data.key.checked;if(H.chkDisabled===true){return}if(G!==true&&G!==false){G=!H[D]}F=!!F;if(H[D]===G&&!I){return}else{if(F&&t.apply(this.setting.callback.beforeCheck,[this.setting.treeId,H],true)==false){return}}if(t.uCanDo(this.setting)&&this.setting.check.enable&&H.nocheck!==true){H[D]=G;var E=f("#"+H.tId+q.id.CHECK);if(I||this.setting.check.chkStyle===q.radio.STYLE){j.checkNodeRelation(this.setting,H)}j.setChkClass(this.setting,E,H);j.repairParentChkClassWithSelf(this.setting,H);if(F){C.treeObj.trigger(q.event.CHECK,[null,C.treeId,H])}}};B.checkAllNodes=function(D){j.repairAllChk(this.setting,!!D)};B.getCheckedNodes=function(E){var D=this.setting.data.key.children;E=(E!==false);return y.getTreeCheckedNodes(this.setting,y.getRoot(C)[D],E)};B.getChangeCheckedNodes=function(){var D=this.setting.data.key.children;return y.getTreeChangeCheckedNodes(this.setting,y.getRoot(C)[D])};B.setChkDisabled=function(E,D,F,G){D=!!D;F=!!F;G=!!G;j.repairSonChkDisabled(this.setting,E,D,G);j.repairParentChkDisabled(this.setting,E.getParentNode(),D,F)};var A=B.updateNode;B.updateNode=function(F,G){if(A){A.apply(B,arguments)}if(!F||!this.setting.check.enable){return}var D=f("#"+F.tId);if(D.get(0)&&t.uCanDo(this.setting)){var E=f("#"+F.tId+q.id.CHECK);if(G==true||this.setting.check.chkStyle===q.radio.STYLE){j.checkNodeRelation(this.setting,F)}j.setChkClass(this.setting,E,F);j.repairParentChkClassWithSelf(this.setting,F)}}},p={getRadioCheckedList:function(D){var C=y.getRoot(D).radioCheckedList;for(var B=0,A=C.length;B<A;B++){if(!y.getNodeCache(D,C[B].tId)){C.splice(B,1);B--;A--}}return C},getCheckStatus:function(B,D){if(!B.check.enable||D.nocheck||D.chkDisabled){return null}var A=B.data.key.checked,C={checked:D[A],half:D.halfCheck?D.halfCheck:(B.check.chkStyle==q.radio.STYLE?(D.check_Child_State===2):(D[A]?(D.check_Child_State>-1&&D.check_Child_State<2):(D.check_Child_State>0)))};return C},getTreeCheckedNodes:function(I,B,H,E){if(!B){return[]}var C=I.data.key.children,A=I.data.key.checked,G=(H&&I.check.chkStyle==q.radio.STYLE&&I.check.radioType==q.radio.TYPE_ALL);E=!E?[]:E;for(var F=0,D=B.length;F<D;F++){if(B[F].nocheck!==true&&B[F].chkDisabled!==true&&B[F][A]==H){E.push(B[F]);if(G){break}}y.getTreeCheckedNodes(I,B[F][C],H,E);if(G&&E.length>0){break}}return E},getTreeChangeCheckedNodes:function(F,C,E){if(!C){return[]}var G=F.data.key.children,B=F.data.key.checked;E=!E?[]:E;for(var D=0,A=C.length;D<A;D++){if(C[D].nocheck!==true&&C[D].chkDisabled!==true&&C[D][B]!=C[D].checkedOld){E.push(C[D])}y.getTreeChangeCheckedNodes(F,C[D][G],E)}return E},makeChkFlag:function(H,C){if(!C){return}var B=H.data.key.children,A=H.data.key.checked,E=-1;if(C[B]){for(var G=0,D=C[B].length;G<D;G++){var I=C[B][G];var F=-1;if(H.check.chkStyle==q.radio.STYLE){if(I.nocheck===true||I.chkDisabled===true){F=I.check_Child_State}else{if(I.halfCheck===true){F=2}else{if(I[A]){F=2}else{F=I.check_Child_State>0?2:0}}}if(F==2){E=2;break}else{if(F==0){E=0}}}else{if(H.check.chkStyle==q.checkbox.STYLE){if(I.nocheck===true||I.chkDisabled===true){F=I.check_Child_State}else{if(I.halfCheck===true){F=1}else{if(I[A]){F=(I.check_Child_State===-1||I.check_Child_State===2)?2:1}else{F=(I.check_Child_State>0)?1:0}}}if(F===1){E=1;break}else{if(F===2&&E>-1&&G>0&&F!==E){E=1;break}else{if(E===2&&F>-1&&F<2){E=1;break}else{if(F>-1){E=F}}}}}}}}C.check_Child_State=E}},h={},l={onCheckNode:function(E,D){if(D.chkDisabled===true){return false}var C=y.getSetting(E.data.treeId),A=C.data.key.checked;if(t.apply(C.callback.beforeCheck,[C.treeId,D],true)==false){return true}D[A]=!D[A];j.checkNodeRelation(C,D);var B=f("#"+D.tId+q.id.CHECK);j.setChkClass(C,B,D);j.repairParentChkClassWithSelf(C,D);C.treeObj.trigger(q.event.CHECK,[E,C.treeId,D]);return true},onMouseoverCheck:function(D,C){if(C.chkDisabled===true){return false}var B=y.getSetting(D.data.treeId),A=f("#"+C.tId+q.id.CHECK);C.check_Focus=true;j.setChkClass(B,A,C);return true},onMouseoutCheck:function(D,C){if(C.chkDisabled===true){return false}var B=y.getSetting(D.data.treeId),A=f("#"+C.tId+q.id.CHECK);C.check_Focus=false;j.setChkClass(B,A,C);return true}},i={},e={checkNodeRelation:function(J,D){var H,F,E,C=J.data.key.children,B=J.data.key.checked,A=q.radio;if(J.check.chkStyle==A.STYLE){var I=y.getRadioCheckedList(J);if(D[B]){if(J.check.radioType==A.TYPE_ALL){for(F=I.length-1;F>=0;F--){H=I[F];H[B]=false;I.splice(F,1);j.setChkClass(J,f("#"+H.tId+q.id.CHECK),H);if(H.parentTId!=D.parentTId){j.repairParentChkClassWithSelf(J,H)}}I.push(D)}else{var G=(D.parentTId)?D.getParentNode():y.getRoot(J);for(F=0,E=G[C].length;F<E;F++){H=G[C][F];if(H[B]&&H!=D){H[B]=false;j.setChkClass(J,f("#"+H.tId+q.id.CHECK),H)}}}}else{if(J.check.radioType==A.TYPE_ALL){for(F=0,E=I.length;F<E;F++){if(D==I[F]){I.splice(F,1);break}}}}}else{if(D[B]&&(!D[C]||D[C].length==0||J.check.chkboxType.Y.indexOf("s")>-1)){j.setSonNodeCheckBox(J,D,true)}if(!D[B]&&(!D[C]||D[C].length==0||J.check.chkboxType.N.indexOf("s")>-1)){j.setSonNodeCheckBox(J,D,false)}if(D[B]&&J.check.chkboxType.Y.indexOf("p")>-1){j.setParentNodeCheckBox(J,D,true)}if(!D[B]&&J.check.chkboxType.N.indexOf("p")>-1){j.setParentNodeCheckBox(J,D,false)}}},makeChkClass:function(B,E){var A=B.data.key.checked,G=q.checkbox,D=q.radio,F="";if(E.chkDisabled===true){F=G.DISABLED}else{if(E.halfCheck){F=G.PART}else{if(B.check.chkStyle==D.STYLE){F=(E.check_Child_State<1)?G.FULL:G.PART}else{F=E[A]?((E.check_Child_State===2||E.check_Child_State===-1)?G.FULL:G.PART):((E.check_Child_State<1)?G.FULL:G.PART)}}}var C=B.check.chkStyle+"_"+(E[A]?G.TRUE:G.FALSE)+"_"+F;C=(E.check_Focus&&E.chkDisabled!==true)?C+"_"+G.FOCUS:C;return q.className.BUTTON+" "+G.DEFAULT+" "+C},repairAllChk:function(E,H){if(E.check.enable&&E.check.chkStyle===q.checkbox.STYLE){var C=E.data.key.checked,G=E.data.key.children,B=y.getRoot(E);for(var D=0,A=B[G].length;D<A;D++){var F=B[G][D];if(F.nocheck!==true&&F.chkDisabled!==true){F[C]=H}j.setSonNodeCheckBox(E,F,H)}}},repairChkClass:function(B,C){if(!C){return}y.makeChkFlag(B,C);if(C.nocheck!==true){var A=f("#"+C.tId+q.id.CHECK);j.setChkClass(B,A,C)}},repairParentChkClass:function(B,C){if(!C||!C.parentTId){return}var A=C.getParentNode();j.repairChkClass(B,A);j.repairParentChkClass(B,A)},repairParentChkClassWithSelf:function(A,C){if(!C){return}var B=A.data.key.children;if(C[B]&&C[B].length>0){j.repairParentChkClass(A,C[B][0])}else{j.repairParentChkClass(A,C)}},repairSonChkDisabled:function(F,H,E,C){if(!H){return}var G=F.data.key.children;if(H.chkDisabled!=E){H.chkDisabled=E}j.repairChkClass(F,H);if(H[G]&&C){for(var D=0,B=H[G].length;D<B;D++){var A=H[G][D];j.repairSonChkDisabled(F,A,E,C)}}},repairParentChkDisabled:function(C,D,B,A){if(!D){return}if(D.chkDisabled!=B&&A){D.chkDisabled=B}j.repairChkClass(C,D);j.repairParentChkDisabled(C,D.getParentNode(),B,A)},setChkClass:function(A,C,B){if(!C){return}if(B.nocheck===true){C.hide()}else{C.show()}C.removeClass();C.addClass(j.makeChkClass(A,B))},setParentNodeCheckBox:function(K,D,J,G){var C=K.data.key.children,A=K.data.key.checked,H=f("#"+D.tId+q.id.CHECK);if(!G){G=D}y.makeChkFlag(K,D);if(D.nocheck!==true&&D.chkDisabled!==true){D[A]=J;j.setChkClass(K,H,D);if(K.check.autoCheckTrigger&&D!=G){K.treeObj.trigger(q.event.CHECK,[null,K.treeId,D])}}if(D.parentTId){var I=true;if(!J){var B=D.getParentNode()[C];for(var F=0,E=B.length;F<E;F++){if((B[F].nocheck!==true&&B[F].chkDisabled!==true&&B[F][A])||((B[F].nocheck===true||B[F].chkDisabled===true)&&B[F].check_Child_State>0)){I=false;break}}}if(I){j.setParentNodeCheckBox(K,D.getParentNode(),J,G)}}},setSonNodeCheckBox:function(K,D,J,G){if(!D){return}var C=K.data.key.children,A=K.data.key.checked,H=f("#"+D.tId+q.id.CHECK);if(!G){G=D}var B=false;if(D[C]){for(var F=0,E=D[C].length;F<E&&D.chkDisabled!==true;F++){var I=D[C][F];j.setSonNodeCheckBox(K,I,J,G);if(I.chkDisabled===true){B=true}}}if(D!=y.getRoot(K)&&D.chkDisabled!==true){if(B&&D.nocheck!==true){y.makeChkFlag(K,D)}if(D.nocheck!==true&&D.chkDisabled!==true){D[A]=J;if(!B){D.check_Child_State=(D[C]&&D[C].length>0)?(J?2:0):-1}}else{D.check_Child_State=-1}j.setChkClass(K,H,D);if(K.check.autoCheckTrigger&&D!=G&&D.nocheck!==true&&D.chkDisabled!==true){K.treeObj.trigger(q.event.CHECK,[null,K.treeId,D])}}}},s={tools:i,view:e,event:h,data:p};f.extend(true,f.fn.zTree.consts,z);f.extend(true,f.fn.zTree._z,s);var c=f.fn.zTree,t=c._z.tools,q=c.consts,j=c._z.view,y=c._z.data,r=c._z.event;y.exSetting(m);y.addInitBind(n);y.addInitUnBind(x);y.addInitCache(d);y.addInitNode(u);y.addInitProxy(o);y.addInitRoot(v);y.addBeforeA(a);y.addZTreeTools(k);var w=j.createNodes;j.createNodes=function(C,D,B,A){if(w){w.apply(j,arguments)}if(!B){return}j.repairParentChkClassWithSelf(C,A)};var b=j.removeNode;j.removeNode=function(B,C){var A=C.getParentNode();if(b){b.apply(j,arguments)}if(!C||!A){return}j.repairChkClass(B,A);j.repairParentChkClass(B,A)};var g=j.appendNodes;j.appendNodes=function(E,G,B,A,D,F){var C="";if(g){C=g.apply(j,arguments)}if(A){y.makeChkFlag(E,A)}return C}})(jQuery);
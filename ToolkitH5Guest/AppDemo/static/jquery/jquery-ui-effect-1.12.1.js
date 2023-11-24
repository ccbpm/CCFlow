/*! jQuery UI - v1.12.1 - 2017-11-15
* http://jqueryui.com
* Includes: effect.js, effects/effect-blind.js, effects/effect-bounce.js, effects/effect-clip.js, effects/effect-drop.js, effects/effect-explode.js, effects/effect-fade.js, effects/effect-fold.js, effects/effect-highlight.js, effects/effect-puff.js, effects/effect-pulsate.js, effects/effect-scale.js, effects/effect-shake.js, effects/effect-size.js, effects/effect-slide.js, effects/effect-transfer.js
* Copyright jQuery Foundation and other contributors; Licensed MIT */
(function(a){if(typeof define==="function"&&define.amd){define(["jquery"],a)}else{a(jQuery)}}(function(d){d.ui=d.ui||{};var c=d.ui.version="1.12.1";
/*!
 * jQuery UI Effects 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var k="ui-effects-",e="ui-effects-style",p="ui-effects-animated",i=d;d.effects={effect:{}};
/*!
 * jQuery Color Animations v2.1.2
 * https://github.com/jquery/jquery-color
 *
 * Copyright 2014 jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 *
 * Date: Wed Jan 16 08:47:09 2013 -0600
 */
(function(K,z){var G="backgroundColor borderBottomColor borderLeftColor borderRightColor borderTopColor color columnRuleColor outlineColor textDecorationColor textEmphasisColor",D=/^([\-+])=\s*(\d+\.?\d*)/,C=[{re:/rgba?\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*(?:,\s*(\d?(?:\.\d+)?)\s*)?\)/,parse:function(L){return[L[1],L[2],L[3],L[4]]}},{re:/rgba?\(\s*(\d+(?:\.\d+)?)\%\s*,\s*(\d+(?:\.\d+)?)\%\s*,\s*(\d+(?:\.\d+)?)\%\s*(?:,\s*(\d?(?:\.\d+)?)\s*)?\)/,parse:function(L){return[L[1]*2.55,L[2]*2.55,L[3]*2.55,L[4]]}},{re:/#([a-f0-9]{2})([a-f0-9]{2})([a-f0-9]{2})/,parse:function(L){return[parseInt(L[1],16),parseInt(L[2],16),parseInt(L[3],16)]}},{re:/#([a-f0-9])([a-f0-9])([a-f0-9])/,parse:function(L){return[parseInt(L[1]+L[1],16),parseInt(L[2]+L[2],16),parseInt(L[3]+L[3],16)]}},{re:/hsla?\(\s*(\d+(?:\.\d+)?)\s*,\s*(\d+(?:\.\d+)?)\%\s*,\s*(\d+(?:\.\d+)?)\%\s*(?:,\s*(\d?(?:\.\d+)?)\s*)?\)/,space:"hsla",parse:function(L){return[L[1],L[2]/100,L[3]/100,L[4]]}}],A=K.Color=function(M,N,L,O){return new K.Color.fn.parse(M,N,L,O)},F={rgba:{props:{red:{idx:0,type:"byte"},green:{idx:1,type:"byte"},blue:{idx:2,type:"byte"}}},hsla:{props:{hue:{idx:0,type:"degrees"},saturation:{idx:1,type:"percent"},lightness:{idx:2,type:"percent"}}}},J={"byte":{floor:true,max:255},percent:{max:1},degrees:{mod:360,floor:true}},I=A.support={},x=K("<p>")[0],w,H=K.each;x.style.cssText="background-color:rgba(1,1,1,.5)";I.rgba=x.style.backgroundColor.indexOf("rgba")>-1;H(F,function(L,M){M.cache="_"+L;M.props.alpha={idx:3,type:"percent",def:1}});function E(M,O,N){var L=J[O.type]||{};if(M==null){return(N||!O.def)?null:O.def}M=L.floor?~~M:parseFloat(M);if(isNaN(M)){return O.def}if(L.mod){return(M+L.mod)%L.mod}return 0>M?0:L.max<M?L.max:M}function B(L){var N=A(),M=N._rgba=[];L=L.toLowerCase();H(C,function(S,T){var Q,R=T.re.exec(L),P=R&&T.parse(R),O=T.space||"rgba";if(P){Q=N[O](P);N[F[O].cache]=Q[F[O].cache];M=N._rgba=Q._rgba;return false}});if(M.length){if(M.join()==="0,0,0,0"){K.extend(M,w.transparent)}return N}return w[L]}A.fn=K.extend(A.prototype,{parse:function(R,P,L,Q){if(R===z){this._rgba=[null,null,null,null];return this}if(R.jquery||R.nodeType){R=K(R).css(P);P=z}var O=this,N=K.type(R),M=this._rgba=[];if(P!==z){R=[R,P,L,Q];N="array"}if(N==="string"){return this.parse(B(R)||w._default)}if(N==="array"){H(F.rgba.props,function(S,T){M[T.idx]=E(R[T.idx],T)});return this}if(N==="object"){if(R instanceof A){H(F,function(S,T){if(R[T.cache]){O[T.cache]=R[T.cache].slice()}})}else{H(F,function(T,U){var S=U.cache;H(U.props,function(V,W){if(!O[S]&&U.to){if(V==="alpha"||R[V]==null){return}O[S]=U.to(O._rgba)}O[S][W.idx]=E(R[V],W,true)});if(O[S]&&K.inArray(null,O[S].slice(0,3))<0){O[S][3]=1;if(U.from){O._rgba=U.from(O[S])}}})}return this}},is:function(N){var L=A(N),O=true,M=this;H(F,function(P,R){var S,Q=L[R.cache];if(Q){S=M[R.cache]||R.to&&R.to(M._rgba)||[];H(R.props,function(T,U){if(Q[U.idx]!=null){O=(Q[U.idx]===S[U.idx]);return O}})}return O});return O},_space:function(){var L=[],M=this;H(F,function(N,O){if(M[O.cache]){L.push(N)}});return L.pop()},transition:function(M,S){var N=A(M),O=N._space(),P=F[O],Q=this.alpha()===0?A("transparent"):this,R=Q[P.cache]||P.to(Q._rgba),L=R.slice();N=N[P.cache];H(P.props,function(W,Y){var V=Y.idx,U=R[V],T=N[V],X=J[Y.type]||{};if(T===null){return}if(U===null){L[V]=T}else{if(X.mod){if(T-U>X.mod/2){U+=X.mod}else{if(U-T>X.mod/2){U-=X.mod}}}L[V]=E((T-U)*S+U,Y)}});return this[O](L)},blend:function(O){if(this._rgba[3]===1){return this}var N=this._rgba.slice(),M=N.pop(),L=A(O)._rgba;return A(K.map(N,function(P,Q){return(1-M)*L[Q]+M*P}))},toRgbaString:function(){var M="rgba(",L=K.map(this._rgba,function(N,O){return N==null?(O>2?1:0):N});if(L[3]===1){L.pop();M="rgb("}return M+L.join()+")"},toHslaString:function(){var M="hsla(",L=K.map(this.hsla(),function(N,O){if(N==null){N=O>2?1:0}if(O&&O<3){N=Math.round(N*100)+"%"}return N});if(L[3]===1){L.pop();M="hsl("}return M+L.join()+")"},toHexString:function(L){var M=this._rgba.slice(),N=M.pop();if(L){M.push(~~(N*255))}return"#"+K.map(M,function(O){O=(O||0).toString(16);return O.length===1?"0"+O:O}).join("")},toString:function(){return this._rgba[3]===0?"transparent":this.toRgbaString()}});A.fn.parse.prototype=A.fn;function y(N,M,L){L=(L+1)%1;if(L*6<1){return N+(M-N)*L*6}if(L*2<1){return M}if(L*3<2){return N+(M-N)*((2/3)-L)*6}return N}F.hsla.to=function(N){if(N[0]==null||N[1]==null||N[2]==null){return[null,null,null,N[3]]}var L=N[0]/255,Q=N[1]/255,R=N[2]/255,T=N[3],S=Math.max(L,Q,R),O=Math.min(L,Q,R),U=S-O,V=S+O,M=V*0.5,P,W;if(O===S){P=0}else{if(L===S){P=(60*(Q-R)/U)+360}else{if(Q===S){P=(60*(R-L)/U)+120}else{P=(60*(L-Q)/U)+240}}}if(U===0){W=0}else{if(M<=0.5){W=U/V}else{W=U/(2-V)}}return[Math.round(P)%360,W,M,T==null?1:T]};F.hsla.from=function(P){if(P[0]==null||P[1]==null||P[2]==null){return[null,null,null,P[3]]}var O=P[0]/360,N=P[1],M=P[2],L=P[3],Q=M<=0.5?M*(1+N):M+N-M*N,R=2*M-Q;return[Math.round(y(R,Q,O+(1/3))*255),Math.round(y(R,Q,O)*255),Math.round(y(R,Q,O-(1/3))*255),L]};H(F,function(M,O){var N=O.props,L=O.cache,Q=O.to,P=O.from;A.fn[M]=function(V){if(Q&&!this[L]){this[L]=Q(this._rgba)}if(V===z){return this[L].slice()}var S,U=K.type(V),R=(U==="array"||U==="object")?V:arguments,T=this[L].slice();H(N,function(W,Y){var X=R[U==="object"?W:Y.idx];if(X==null){X=T[Y.idx]}T[Y.idx]=E(X,Y)});if(P){S=A(P(T));S[L]=T;return S}else{return A(T)}};H(N,function(R,S){if(A.fn[R]){return}A.fn[R]=function(W){var Y=K.type(W),V=(R==="alpha"?(this._hsla?"hsla":"rgba"):M),U=this[V](),X=U[S.idx],T;if(Y==="undefined"){return X}if(Y==="function"){W=W.call(this,X);Y=K.type(W)}if(W==null&&S.empty){return this}if(Y==="string"){T=D.exec(W);if(T){W=X+parseFloat(T[2])*(T[1]==="+"?1:-1)}}U[S.idx]=W;return this[V](U)}})});A.hook=function(M){var L=M.split(" ");H(L,function(N,O){K.cssHooks[O]={set:function(S,T){var Q,R,P="";if(T!=="transparent"&&(K.type(T)!=="string"||(Q=B(T)))){T=A(Q||T);if(!I.rgba&&T._rgba[3]!==1){R=O==="backgroundColor"?S.parentNode:S;while((P===""||P==="transparent")&&R&&R.style){try{P=K.css(R,"backgroundColor");R=R.parentNode}catch(U){}}T=T.blend(P&&P!=="transparent"?P:"_default")}T=T.toRgbaString()}try{S.style[O]=T}catch(U){}}};K.fx.step[O]=function(P){if(!P.colorInit){P.start=A(P.elem,O);P.end=A(P.end);P.colorInit=true}K.cssHooks[O].set(P.elem,P.start.transition(P.end,P.pos))}})};A.hook(G);K.cssHooks.borderColor={expand:function(M){var L={};H(["Top","Right","Bottom","Left"],function(O,N){L["border"+N+"Color"]=M});return L}};w=K.Color.names={aqua:"#00ffff",black:"#000000",blue:"#0000ff",fuchsia:"#ff00ff",gray:"#808080",green:"#008000",lime:"#00ff00",maroon:"#800000",navy:"#000080",olive:"#808000",purple:"#800080",red:"#ff0000",silver:"#c0c0c0",teal:"#008080",white:"#ffffff",yellow:"#ffff00",transparent:[null,null,null,0],_default:"#ffffff"}})(i);(function(){var x=["add","remove","toggle"],y={border:1,borderBottom:1,borderColor:1,borderLeft:1,borderRight:1,borderTop:1,borderWidth:1,margin:1,padding:1};d.each(["borderLeftStyle","borderRightStyle","borderBottomStyle","borderTopStyle"],function(A,B){d.fx.step[B]=function(C){if(C.end!=="none"&&!C.setAttr||C.pos===1&&!C.setAttr){i.style(C.elem,B,C.end);C.setAttr=true}}});function z(E){var B,A,C=E.ownerDocument.defaultView?E.ownerDocument.defaultView.getComputedStyle(E,null):E.currentStyle,D={};if(C&&C.length&&C[0]&&C[C[0]]){A=C.length;while(A--){B=C[A];if(typeof C[B]==="string"){D[d.camelCase(B)]=C[B]}}}else{for(B in C){if(typeof C[B]==="string"){D[B]=C[B]}}}return D}function w(A,C){var E={},B,D;for(B in C){D=C[B];if(A[B]!==D){if(!y[B]){if(d.fx.step[B]||!isNaN(parseFloat(D))){E[B]=D}}}}return E}if(!d.fn.addBack){d.fn.addBack=function(A){return this.add(A==null?this.prevObject:this.prevObject.filter(A))}}d.effects.animateClass=function(A,B,E,D){var C=d.speed(B,E,D);return this.queue(function(){var H=d(this),F=H.attr("class")||"",G,I=C.children?H.find("*").addBack():H;I=I.map(function(){var J=d(this);return{el:J,start:z(this)}});G=function(){d.each(x,function(J,K){if(A[K]){H[K+"Class"](A[K])}})};G();I=I.map(function(){this.end=z(this.el[0]);this.diff=w(this.start,this.end);return this});H.attr("class",F);I=I.map(function(){var L=this,J=d.Deferred(),K=d.extend({},C,{queue:false,complete:function(){J.resolve(L)}});this.el.animate(this.diff,K);return J.promise()});d.when.apply(d,I.get()).done(function(){G();d.each(arguments,function(){var J=this.el;d.each(this.diff,function(K){J.css(K,"")})});C.complete.call(H[0])})})};d.fn.extend({addClass:(function(A){return function(C,B,E,D){return B?d.effects.animateClass.call(this,{add:C},B,E,D):A.apply(this,arguments)}})(d.fn.addClass),removeClass:(function(A){return function(C,B,E,D){return arguments.length>1?d.effects.animateClass.call(this,{remove:C},B,E,D):A.apply(this,arguments)}})(d.fn.removeClass),toggleClass:(function(A){return function(D,C,B,F,E){if(typeof C==="boolean"||C===undefined){if(!B){return A.apply(this,arguments)}else{return d.effects.animateClass.call(this,(C?{add:D}:{remove:D}),B,F,E)}}else{return d.effects.animateClass.call(this,{toggle:D},C,B,F)}}})(d.fn.toggleClass),switchClass:function(A,C,B,E,D){return d.effects.animateClass.call(this,{add:C,remove:A},B,E,D)}})})();(function(){if(d.expr&&d.expr.filters&&d.expr.filters.animated){d.expr.filters.animated=(function(z){return function(A){return !!d(A).data(p)||z(A)}})(d.expr.filters.animated)}if(d.uiBackCompat!==false){d.extend(d.effects,{save:function(A,C){var z=0,B=C.length;for(;z<B;z++){if(C[z]!==null){A.data(k+C[z],A[0].style[C[z]])}}},restore:function(A,D){var C,z=0,B=D.length;for(;z<B;z++){if(D[z]!==null){C=A.data(k+D[z]);A.css(D[z],C)}}},setMode:function(z,A){if(A==="toggle"){A=z.is(":hidden")?"show":"hide"}return A},createWrapper:function(A){if(A.parent().is(".ui-effects-wrapper")){return A.parent()}var B={width:A.outerWidth(true),height:A.outerHeight(true),"float":A.css("float")},E=d("<div></div>").addClass("ui-effects-wrapper").css({fontSize:"100%",background:"transparent",border:"none",margin:0,padding:0}),z={width:A.width(),height:A.height()},D=document.activeElement;try{D.id}catch(C){D=document.body}A.wrap(E);if(A[0]===D||d.contains(A[0],D)){d(D).trigger("focus")}E=A.parent();if(A.css("position")==="static"){E.css({position:"relative"});A.css({position:"relative"})}else{d.extend(B,{position:A.css("position"),zIndex:A.css("z-index")});d.each(["top","left","bottom","right"],function(F,G){B[G]=A.css(G);if(isNaN(parseInt(B[G],10))){B[G]="auto"}});A.css({position:"relative",top:0,left:0,right:"auto",bottom:"auto"})}A.css(z);return E.css(B).show()},removeWrapper:function(z){var A=document.activeElement;if(z.parent().is(".ui-effects-wrapper")){z.parent().replaceWith(z);if(z[0]===A||d.contains(z[0],A)){d(A).trigger("focus")}}return z}})}d.extend(d.effects,{version:"1.12.1",define:function(z,B,A){if(!A){A=B;B="effect"}d.effects.effect[z]=A;d.effects.effect[z].mode=B;return A},scaledDimensions:function(A,B,C){if(B===0){return{height:0,width:0,outerHeight:0,outerWidth:0}}var z=C!=="horizontal"?((B||100)/100):1,D=C!=="vertical"?((B||100)/100):1;return{height:A.height()*D,width:A.width()*z,outerHeight:A.outerHeight()*D,outerWidth:A.outerWidth()*z}},clipToBox:function(z){return{width:z.clip.right-z.clip.left,height:z.clip.bottom-z.clip.top,left:z.clip.left,top:z.clip.top}},unshift:function(A,C,B){var z=A.queue();if(C>1){z.splice.apply(z,[1,0].concat(z.splice(C,B)))}A.dequeue()},saveStyle:function(z){z.data(e,z[0].style.cssText)},restoreStyle:function(z){z[0].style.cssText=z.data(e)||"";z.removeData(e)},mode:function(z,B){var A=z.is(":hidden");if(B==="toggle"){B=A?"show":"hide"}if(A?B==="hide":B==="show"){B="none"}return B},getBaseline:function(A,B){var C,z;switch(A[0]){case"top":C=0;break;case"middle":C=0.5;break;case"bottom":C=1;break;default:C=A[0]/B.height}switch(A[1]){case"left":z=0;break;case"center":z=0.5;break;case"right":z=1;break;default:z=A[1]/B.width}return{x:z,y:C}},createPlaceholder:function(A){var C,B=A.css("position"),z=A.position();A.css({marginTop:A.css("marginTop"),marginBottom:A.css("marginBottom"),marginLeft:A.css("marginLeft"),marginRight:A.css("marginRight")}).outerWidth(A.outerWidth()).outerHeight(A.outerHeight());if(/^(static|relative)/.test(B)){B="absolute";C=d("<"+A[0].nodeName+">").insertAfter(A).css({display:/^(inline|ruby)/.test(A.css("display"))?"inline-block":"block",visibility:"hidden",marginTop:A.css("marginTop"),marginBottom:A.css("marginBottom"),marginLeft:A.css("marginLeft"),marginRight:A.css("marginRight"),"float":A.css("float")}).outerWidth(A.outerWidth()).outerHeight(A.outerHeight()).addClass("ui-effects-placeholder");A.data(k+"placeholder",C)}A.css({position:B,left:z.left,top:z.top});return C},removePlaceholder:function(z){var B=k+"placeholder",A=z.data(B);if(A){A.remove();z.removeData(B)}},cleanUp:function(z){d.effects.restoreStyle(z);d.effects.removePlaceholder(z)},setTransition:function(A,C,z,B){B=B||{};d.each(C,function(E,D){var F=A.cssUnit(D);if(F[0]>0){B[D]=F[0]*z+F[1]}});return B}});function x(A,z,B,C){if(d.isPlainObject(A)){z=A;A=A.effect}A={effect:A};if(z==null){z={}}if(d.isFunction(z)){C=z;B=null;z={}}if(typeof z==="number"||d.fx.speeds[z]){C=B;B=z;z={}}if(d.isFunction(B)){C=B;B=null}if(z){d.extend(A,z)}B=B||z.duration;A.duration=d.fx.off?0:typeof B==="number"?B:B in d.fx.speeds?d.fx.speeds[B]:d.fx.speeds._default;A.complete=C||z.complete;return A}function y(z){if(!z||typeof z==="number"||d.fx.speeds[z]){return true}if(typeof z==="string"&&!d.effects.effect[z]){return true}if(d.isFunction(z)){return true}if(typeof z==="object"&&!z.effect){return true}return false}d.fn.extend({effect:function(){var H=x.apply(this,arguments),G=d.effects.effect[H.effect],D=G.mode,F=H.queue,C=F||"fx",z=H.complete,E=H.mode,A=[],I=function(L){var K=d(this),J=d.effects.mode(K,E)||D;K.data(p,true);A.push(J);if(D&&(J==="show"||(J===D&&J==="hide"))){K.show()}if(!D||J!=="none"){d.effects.saveStyle(K)}if(d.isFunction(L)){L()}};if(d.fx.off||!G){if(E){return this[E](H.duration,z)}else{return this.each(function(){if(z){z.call(this)}})}}function B(L){var M=d(this);function K(){M.removeData(p);d.effects.cleanUp(M);if(H.mode==="hide"){M.hide()}J()}function J(){if(d.isFunction(z)){z.call(M[0])}if(d.isFunction(L)){L()}}H.mode=A.shift();if(d.uiBackCompat!==false&&!D){if(M.is(":hidden")?E==="hide":E==="show"){M[E]();J()}else{G.call(M[0],H,J)}}else{if(H.mode==="none"){M[E]();J()}else{G.call(M[0],H,K)}}}return F===false?this.each(I).each(B):this.queue(C,I).queue(C,B)},show:(function(z){return function(B){if(y(B)){return z.apply(this,arguments)}else{var A=x.apply(this,arguments);A.mode="show";return this.effect.call(this,A)}}})(d.fn.show),hide:(function(z){return function(B){if(y(B)){return z.apply(this,arguments)}else{var A=x.apply(this,arguments);A.mode="hide";return this.effect.call(this,A)}}})(d.fn.hide),toggle:(function(z){return function(B){if(y(B)||typeof B==="boolean"){return z.apply(this,arguments)}else{var A=x.apply(this,arguments);A.mode="toggle";return this.effect.call(this,A)}}})(d.fn.toggle),cssUnit:function(z){var A=this.css(z),B=[];d.each(["em","px","%","pt"],function(C,D){if(A.indexOf(D)>0){B=[parseFloat(A),D]}});return B},cssClip:function(z){if(z){return this.css("clip","rect("+z.top+"px "+z.right+"px "+z.bottom+"px "+z.left+"px)")}return w(this.css("clip"),this)},transfer:function(K,C){var E=d(this),G=d(K.to),J=G.css("position")==="fixed",F=d("body"),H=J?F.scrollTop():0,I=J?F.scrollLeft():0,z=G.offset(),B={top:z.top-H,left:z.left-I,height:G.innerHeight(),width:G.innerWidth()},D=E.offset(),A=d("<div class='ui-effects-transfer'></div>").appendTo("body").addClass(K.className).css({top:D.top-H,left:D.left-I,height:E.innerHeight(),width:E.innerWidth(),position:J?"fixed":"absolute"}).animate(B,K.duration,K.easing,function(){A.remove();if(d.isFunction(C)){C()}})}});function w(E,B){var D=B.outerWidth(),C=B.outerHeight(),A=/^rect\((-?\d*\.?\d*px|-?\d+%|auto),?\s*(-?\d*\.?\d*px|-?\d+%|auto),?\s*(-?\d*\.?\d*px|-?\d+%|auto),?\s*(-?\d*\.?\d*px|-?\d+%|auto)\)$/,z=A.exec(E)||["",0,D,C,0];return{top:parseFloat(z[1])||0,right:z[2]==="auto"?D:parseFloat(z[2]),bottom:z[3]==="auto"?C:parseFloat(z[3]),left:parseFloat(z[4])||0}}d.fx.step.clip=function(z){if(!z.clipInit){z.start=d(z.elem).cssClip();if(typeof z.end==="string"){z.end=w(z.end,z.elem)}z.clipInit=true}d(z.elem).cssClip({top:z.pos*(z.end.top-z.start.top)+z.start.top,right:z.pos*(z.end.right-z.start.right)+z.start.right,bottom:z.pos*(z.end.bottom-z.start.bottom)+z.start.bottom,left:z.pos*(z.end.left-z.start.left)+z.start.left})}})();(function(){var w={};d.each(["Quad","Cubic","Quart","Quint","Expo"],function(y,x){w[x]=function(z){return Math.pow(z,y+2)}});d.extend(w,{Sine:function(x){return 1-Math.cos(x*Math.PI/2)},Circ:function(x){return 1-Math.sqrt(1-x*x)},Elastic:function(x){return x===0||x===1?x:-Math.pow(2,8*(x-1))*Math.sin(((x-1)*80-7.5)*Math.PI/15)},Back:function(x){return x*x*(3*x-2)},Bounce:function(z){var x,y=4;while(z<((x=Math.pow(2,--y))-1)/11){}return 1/Math.pow(4,3-y)-7.5625*Math.pow((x*3-2)/22-z,2)}});d.each(w,function(y,x){d.easing["easeIn"+y]=x;d.easing["easeOut"+y]=function(z){return 1-x(1-z)};d.easing["easeInOut"+y]=function(z){return z<0.5?x(z*2)/2:1-x(z*-2+2)/2}})})();var m=d.effects;
/*!
 * jQuery UI Effects Blind 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var h=d.effects.define("blind","hide",function(y,w){var B={up:["bottom","top"],vertical:["bottom","top"],down:["top","bottom"],left:["right","left"],horizontal:["right","left"],right:["left","right"]},z=d(this),A=y.direction||"up",D=z.cssClip(),x={clip:d.extend({},D)},C=d.effects.createPlaceholder(z);x.clip[B[A][0]]=x.clip[B[A][1]];if(y.mode==="show"){z.cssClip(x.clip);if(C){C.css(d.effects.clipToBox(x))}x.clip=D}if(C){C.animate(d.effects.clipToBox(x),y.duration,y.easing)}z.animate(x,{queue:false,duration:y.duration,easing:y.easing,complete:w})});
/*!
 * jQuery UI Effects Bounce 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var q=d.effects.define("bounce",function(x,E){var A,I,L,w=d(this),D=x.mode,C=D==="hide",M=D==="show",N=x.direction||"up",y=x.distance,B=x.times||5,O=B*2+(M||C?1:0),K=x.duration/O,G=x.easing,z=(N==="up"||N==="down")?"top":"left",F=(N==="up"||N==="left"),J=0,H=w.queue().length;d.effects.createPlaceholder(w);L=w.css(z);if(!y){y=w[z==="top"?"outerHeight":"outerWidth"]()/3}if(M){I={opacity:1};I[z]=L;w.css("opacity",0).css(z,F?-y*2:y*2).animate(I,K,G)}if(C){y=y/Math.pow(2,B-1)}I={};I[z]=L;for(;J<B;J++){A={};A[z]=(F?"-=":"+=")+y;w.animate(A,K,G).animate(I,K,G);y=C?y*2:y/2}if(C){A={opacity:0};A[z]=(F?"-=":"+=")+y;w.animate(A,K,G)}w.queue(E);d.effects.unshift(w,H,O+1)});
/*!
 * jQuery UI Effects Clip 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var v=d.effects.define("clip","hide",function(E,A){var x,y={},B=d(this),D=E.direction||"vertical",C=D==="both",w=C||D==="horizontal",z=C||D==="vertical";x=B.cssClip();y.clip={top:z?(x.bottom-x.top)/2:x.top,right:w?(x.right-x.left)/2:x.right,bottom:z?(x.bottom-x.top)/2:x.bottom,left:w?(x.right-x.left)/2:x.left};d.effects.createPlaceholder(B);if(E.mode==="show"){B.cssClip(y.clip);y.clip=x}B.animate(y,{queue:false,duration:E.duration,easing:E.easing,complete:A})});
/*!
 * jQuery UI Effects Drop 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var g=d.effects.define("drop","hide",function(G,z){var w,A=d(this),C=G.mode,E=C==="show",D=G.direction||"left",x=(D==="up"||D==="down")?"top":"left",F=(D==="up"||D==="left")?"-=":"+=",B=(F==="+=")?"-=":"+=",y={opacity:0};d.effects.createPlaceholder(A);w=G.distance||A[x==="top"?"outerHeight":"outerWidth"](true)/2;y[x]=F+w;if(E){A.css(y);y[x]=B+w;y.opacity=1}A.animate(y,{queue:false,duration:G.duration,easing:G.easing,complete:z})});
/*!
 * jQuery UI Effects Explode 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var b=d.effects.define("explode","hide",function(x,J){var M,L,z,H,G,E,D=x.pieces?Math.round(Math.sqrt(x.pieces)):3,y=D,w=d(this),F=x.mode,N=F==="show",B=w.show().css("visibility","hidden").offset(),K=Math.ceil(w.outerWidth()/y),I=Math.ceil(w.outerHeight()/D),C=[];function O(){C.push(this);if(C.length===D*y){A()}}for(M=0;M<D;M++){H=B.top+M*I;E=M-(D-1)/2;for(L=0;L<y;L++){z=B.left+L*K;G=L-(y-1)/2;w.clone().appendTo("body").wrap("<div></div>").css({position:"absolute",visibility:"visible",left:-L*K,top:-M*I}).parent().addClass("ui-effects-explode").css({position:"absolute",overflow:"hidden",width:K,height:I,left:z+(N?G*K:0),top:H+(N?E*I:0),opacity:N?0:1}).animate({left:z+(N?0:G*K),top:H+(N?0:E*I),opacity:N?1:0},x.duration||500,x.easing,O)}}function A(){w.css({visibility:"visible"});d(C).remove();J()}});
/*!
 * jQuery UI Effects Fade 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var s=d.effects.define("fade","toggle",function(y,x){var w=y.mode==="show";d(this).css("opacity",w?0:1).animate({opacity:w?1:0},{queue:false,duration:y.duration,easing:y.easing,complete:x})});
/*!
 * jQuery UI Effects Fold 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var f=d.effects.define("fold","hide",function(M,B){var C=d(this),D=M.mode,J=D==="show",E=D==="hide",L=M.size||15,F=/([0-9]+)%/.exec(L),K=!!M.horizFirst,z=K?["right","bottom"]:["bottom","right"],A=M.duration/2,I=d.effects.createPlaceholder(C),x=C.cssClip(),H={clip:d.extend({},x)},G={clip:d.extend({},x)},w=[x[z[0]],x[z[1]]],y=C.queue().length;if(F){L=parseInt(F[1],10)/100*w[E?0:1]}H.clip[z[0]]=L;G.clip[z[0]]=L;G.clip[z[1]]=0;if(J){C.cssClip(G.clip);if(I){I.css(d.effects.clipToBox(G))}G.clip=x}C.queue(function(N){if(I){I.animate(d.effects.clipToBox(H),A,M.easing).animate(d.effects.clipToBox(G),A,M.easing)}N()}).animate(H,A,M.easing).animate(G,A,M.easing).queue(B);d.effects.unshift(C,y,4)});
/*!
 * jQuery UI Effects Highlight 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var r=d.effects.define("highlight","show",function(x,w){var y=d(this),z={backgroundColor:y.css("backgroundColor")};if(x.mode==="hide"){z.opacity=0}d.effects.saveStyle(y);y.css({backgroundImage:"none",backgroundColor:x.color||"#ffff99"}).animate(z,{queue:false,duration:x.duration,easing:x.easing,complete:w})});
/*!
 * jQuery UI Effects Size 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var n=d.effects.define("size",function(z,F){var D,E,J,w=d(this),B=["fontSize"],K=["borderTopWidth","borderBottomWidth","paddingTop","paddingBottom"],y=["borderLeftWidth","borderRightWidth","paddingLeft","paddingRight"],C=z.mode,I=C!=="effect",N=z.scale||"both",L=z.origin||["middle","center"],M=w.css("position"),A=w.position(),G=d.effects.scaledDimensions(w),H=z.from||G,x=z.to||d.effects.scaledDimensions(w,0);d.effects.createPlaceholder(w);if(C==="show"){J=H;H=x;x=J}E={from:{y:H.height/G.height,x:H.width/G.width},to:{y:x.height/G.height,x:x.width/G.width}};if(N==="box"||N==="both"){if(E.from.y!==E.to.y){H=d.effects.setTransition(w,K,E.from.y,H);x=d.effects.setTransition(w,K,E.to.y,x)}if(E.from.x!==E.to.x){H=d.effects.setTransition(w,y,E.from.x,H);x=d.effects.setTransition(w,y,E.to.x,x)}}if(N==="content"||N==="both"){if(E.from.y!==E.to.y){H=d.effects.setTransition(w,B,E.from.y,H);x=d.effects.setTransition(w,B,E.to.y,x)}}if(L){D=d.effects.getBaseline(L,G);H.top=(G.outerHeight-H.outerHeight)*D.y+A.top;H.left=(G.outerWidth-H.outerWidth)*D.x+A.left;x.top=(G.outerHeight-x.outerHeight)*D.y+A.top;x.left=(G.outerWidth-x.outerWidth)*D.x+A.left}w.css(H);if(N==="content"||N==="both"){K=K.concat(["marginTop","marginBottom"]).concat(B);y=y.concat(["marginLeft","marginRight"]);w.find("*[width]").each(function(){var R=d(this),O=d.effects.scaledDimensions(R),Q={height:O.height*E.from.y,width:O.width*E.from.x,outerHeight:O.outerHeight*E.from.y,outerWidth:O.outerWidth*E.from.x},P={height:O.height*E.to.y,width:O.width*E.to.x,outerHeight:O.height*E.to.y,outerWidth:O.width*E.to.x};if(E.from.y!==E.to.y){Q=d.effects.setTransition(R,K,E.from.y,Q);P=d.effects.setTransition(R,K,E.to.y,P)}if(E.from.x!==E.to.x){Q=d.effects.setTransition(R,y,E.from.x,Q);P=d.effects.setTransition(R,y,E.to.x,P)}if(I){d.effects.saveStyle(R)}R.css(Q);R.animate(P,z.duration,z.easing,function(){if(I){d.effects.restoreStyle(R)}})})}w.animate(x,{queue:false,duration:z.duration,easing:z.easing,complete:function(){var O=w.offset();if(x.opacity===0){w.css("opacity",H.opacity)}if(!I){w.css("position",M==="static"?"relative":M).offset(O);d.effects.saveStyle(w)}F()}})});
/*!
 * jQuery UI Effects Scale 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var j=d.effects.define("scale",function(x,w){var y=d(this),B=x.mode,z=parseInt(x.percent,10)||(parseInt(x.percent,10)===0?0:(B!=="effect"?0:100)),A=d.extend(true,{from:d.effects.scaledDimensions(y),to:d.effects.scaledDimensions(y,z,x.direction||"both"),origin:x.origin||["middle","center"]},x);if(x.fade){A.from.opacity=1;A.to.opacity=0}d.effects.effect.size.call(this,A,w)});
/*!
 * jQuery UI Effects Puff 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var a=d.effects.define("puff","hide",function(x,w){var y=d.extend(true,{},x,{fade:true,percent:parseInt(x.percent,10)||150});d.effects.effect.scale.call(this,y,w)});
/*!
 * jQuery UI Effects Pulsate 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var o=d.effects.define("pulsate","show",function(H,y){var A=d(this),B=H.mode,F=B==="show",C=B==="hide",G=F||C,D=((H.times||5)*2)+(G?1:0),x=H.duration/D,E=0,z=1,w=A.queue().length;if(F||!A.is(":visible")){A.css("opacity",0).show();E=1}for(;z<D;z++){A.animate({opacity:E},x,H.easing);E=1-E}A.animate({opacity:E},x,H.easing);A.queue(y);d.effects.unshift(A,w,D+1)});
/*!
 * jQuery UI Effects Shake 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var u=d.effects.define("shake",function(K,D){var E=1,F=d(this),H=K.direction||"left",w=K.distance||20,x=K.times||3,I=x*2+1,B=Math.round(K.duration/I),A=(H==="up"||H==="down")?"top":"left",y=(H==="up"||H==="left"),C={},J={},G={},z=F.queue().length;d.effects.createPlaceholder(F);C[A]=(y?"-=":"+=")+w;J[A]=(y?"+=":"-=")+w*2;G[A]=(y?"-=":"+=")+w*2;F.animate(C,B,K.easing);for(;E<x;E++){F.animate(J,B,K.easing).animate(G,B,K.easing)}F.animate(J,B,K.easing).animate(C,B/2,K.easing).queue(D);d.effects.unshift(F,z,I+1)});
/*!
 * jQuery UI Effects Slide 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var l=d.effects.define("slide","show",function(H,D){var A,x,E=d(this),y={up:["bottom","top"],down:["top","bottom"],left:["right","left"],right:["left","right"]},F=H.mode,G=H.direction||"left",B=(G==="up"||G==="down")?"top":"left",z=(G==="up"||G==="left"),w=H.distance||E[B==="top"?"outerHeight":"outerWidth"](true),C={};d.effects.createPlaceholder(E);A=E.cssClip();x=E.position()[B];C[B]=(z?-1:1)*w+x;C.clip=E.cssClip();C.clip[y[G][1]]=C.clip[y[G][0]];if(F==="show"){E.cssClip(C.clip);E.css(B,C[B]);C.clip=A;C[B]=x}E.animate(C,{queue:false,duration:H.duration,easing:H.easing,complete:D})});
/*!
 * jQuery UI Effects Transfer 1.12.1
 * http://jqueryui.com
 *
 * Copyright jQuery Foundation and other contributors
 * Released under the MIT license.
 * http://jquery.org/license
 */
var m;if(d.uiBackCompat!==false){m=d.effects.define("transfer",function(x,w){d(this).transfer(x,w)})}var t=m}));
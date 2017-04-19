// 百度地图API功能
var map = new BMap.Map("l-map");            // 创建Map实例
map.centerAndZoom(new BMap.Point(116.404, 39.915), 11);
$(function() {
	JY.Model.confirm("要共享地理位置吗?",function(){		
	JY.Model.loading();
	var geolocation = new BMap.Geolocation();
			geolocation.getCurrentPosition(function(r) {
				if (this.getStatus() == BMAP_STATUS_SUCCESS) {
					var mk = new BMap.Marker(r.point);
					map.addOverlay(mk);
					map.panTo(r.point);
					map.centerAndZoom(new BMap.Point(r.point.lng, r.point.lat),11);
					var myCity = new BMap.LocalCity();
					myCity.get(myCityFun);
				} else {
					JY.Model.error('获取失败：' + this.getStatus());
				}
				 JY.Model.loadingClose();
			}, {enableHighAccuracy : true});
		});

	});

	function myCityFun(result) {
		$("#cityName").html(result.name);
	}
	function selectFun(o) {
		$("#keyWordSpan").addClass("hide");
		$("#roadSpan").addClass("hide");
		if (o.value == 1) {
			$("#keyWordSpan").removeClass("hide");
			searchMapByKeyWord();
		}else if(o.value == 2 ||o.value == 3 ||o.value == 4 ){
			$("#roadSpan").removeClass("hide");
		}else if (o.value == 5) {
			setPoint();
		}

	}
	function searchMap() {
		var selectFun = $("#selectFun").val();
		if (selectFun == 1) {
			$("#keyWord").removeClass("hide");
			searchMapByKeyWord();
		} else if (selectFun == 2) {
			searchMapByBus();
		} else if (selectFun == 3) {
			searchMapByDriving();
		} else if (selectFun == 4) {
			searchMapByWalking();
		}
	
	}
	function searchMapByKeyWord() {
		var local = new BMap.LocalSearch(map, {renderOptions : {map : map,panel : "r-result"}});
		var keyWord = $("#keyWord").val();
		local.search(keyWord);
	}
	function searchMapByBus() {
		var transit = new BMap.TransitRoute(map, {renderOptions : {map : map,panel : "r-result"}});
		var beginWord=$("#beginWord").val();
		var endWord=$("#endWord").val();
		transit.search(beginWord, endWord);
	}
	function searchMapByDriving() {
		var driving = new BMap.DrivingRoute(map, {renderOptions : {map : map,panel : "r-result",autoViewport : true}});
		var beginWord=$("#beginWord").val();
		var endWord=$("#endWord").val();
		driving.search(beginWord, endWord);
	}
	function searchMapByWalking() {
		var walking = new BMap.WalkingRoute(map, {renderOptions : {map : map,panel : "r-result",autoViewport : true}});
		var beginWord=$("#beginWord").val();
		var endWord=$("#endWord").val();
		walking.search(beginWord, endWord);
	}
	function setPoint(){	
		var map = new BMap.Map("l-map");
		var point = new BMap.Point(113.30764968,23.1500555);
		map.centerAndZoom(point, 12);
		var marker = new BMap.Marker(point);  // 创建标注
		map.addOverlay(marker);              // 将标注添加到地图中
		var label = new BMap.Label("京缘网络有限公司",{offset:new BMap.Size(20,-10)});
		marker.setLabel(label);
	}
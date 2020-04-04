function FenZhongXiaoshiSum() {  //对工作时间进行求和计算最后转化格式例如 1:40
    //对工作小时进行求和计算
    var inputshour = document.getElementsByName("DDL_XS");
    var xiaoshisum = 0;
    for (var i = 0; i < inputshour.length; i++) {
        var input = inputshour[i];
        switch (parseInt(input.value)) {
            case 0:
                xiaoshisum = xiaoshisum + 0;
                break;
            case 1:
                xiaoshisum = xiaoshisum + 1;
                break;
            case 2:
                xiaoshisum = xiaoshisum + 2;
                break;
            case 3:
                xiaoshisum = xiaoshisum + 3;
                break;
            case 4:
                xiaoshisum = xiaoshisum + 4;
                break;

        }

    }

    var fenzhongsum = 0; 
  
    //对分钟时间进行求和计算
    var inputsmin = document.getElementsByName("DDL_FenZhong");
    for (var i = 0; i < inputsmin.length; i++) {
        var input = inputsmin[i];     
        switch (parseInt(input.value)) {
            case 0:                
                fenzhongsum = fenzhongsum+0;
                break;
            case 1:
                fenzhongsum = fenzhongsum + 10;
                break;
            case 2:
                fenzhongsum = fenzhongsum + 15;
                break;
            case 3:
                fenzhongsum = fenzhongsum + 20;
                break;
            case 4:
                fenzhongsum = fenzhongsum + 25;
                break;
            case 5:
                fenzhongsum = fenzhongsum + 30;
                break;
            case 6:
                fenzhongsum = fenzhongsum + 35;
                break;
            case 7:
                fenzhongsum = fenzhongsum + 40;
                break;
            case 8:
                fenzhongsum = fenzhongsum + 45;
                break;
            case 9:
                fenzhongsum = fenzhongsum + 50;
                break;
            case 10:
                fenzhongsum = fenzhongsum + 55;
                break;
            
           
        }
       
    }
   //将分钟格式转换为小时加分钟例如100分钟转化后1:40;
    function ChangeHourMinutestr(str) {
        if (str !== "0" && str !== "" && str !== null) {
            return ((Math.floor(str / 60)).toString().length < 2 ? "0" + (Math.floor(str / 60)).toString() :
                (Math.floor(str / 60)).toString()) + ":" + ((str % 60).toString().length < 2 ? "0" + (str % 60).toString() : (str % 60).toString());
        }
        else {
            return "";
        }
    }
    
    $("#TB_GZSJHJ", parent.document).val(ChangeHourMinutestr(fenzhongsum + xiaoshisum * 60));
   

}
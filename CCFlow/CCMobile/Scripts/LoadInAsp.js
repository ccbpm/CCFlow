var s = "";


if (navigator.userAgent.indexOf("Chrome") > 0) {
    s = "<object id='WebOffice1' type='application/x-itst-activex' align='baseline' border='0'  codebase='/WF/Activex/WebOffice.cab#V7.0.0.8'"
		+ "style='LEFT: 0px; WIDTH: 100%; TOP: 0px; HEIGHT: 99%'"
		+ "clsid='{E77E049B-23FC-4DB8-B756-60529A35FAD5}'"
        + "<param name ='_ExtentX' value='6350'>"
        + "<param name ='_ExtentY' value='6350'>"
		+ "</object>";
} else if (navigator.userAgent.indexOf("Firefox") > 0) {
    s = "<object id='WebOffice1' type='application/x-itst-activex' align='baseline' border='0'  codebase='/WF/Activex/WebOffice.cab#V7.0.0.8'"
		+ "style='LEFT: 0px; WIDTH: 100%; TOP: 0px; HEIGHT: 99%'"
		+ "clsid='{E77E049B-23FC-4DB8-B756-60529A35FAD5}'"
        + "<param name ='_ExtentX' value='6350'>"
        + "<param name ='_ExtentY' value='6350'>"
		+ "</object>";
}
else {

    s = "<OBJECT id='WebOffice1'  style='LEFT: 0px; WIDTH: 100%; TOP: 0px; HEIGHT:99%'  codebase='/WF/Activex/WebOffice.cab#V7.0.0.8'"
		+ "classid=clsid:E77E049B-23FC-4DB8-B756-60529A35FAD5>"
        + "<param name ='_ExtentX' value='6350'>"
        + "<param name ='_ExtentY' value='6350'>"
		+ "</OBJECT>";

}
document.write(s);
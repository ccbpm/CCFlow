function CheckValue(name) {
    if (event.srcElement.name == name || event.srcElement.name.indexOf(name) == event.srcElement.name.length - name.length) {
        if (String.fromCharCode(event.keyCode).search(/^[0-9-.]$/) == -1) event.returnValue = false;
    }
} 
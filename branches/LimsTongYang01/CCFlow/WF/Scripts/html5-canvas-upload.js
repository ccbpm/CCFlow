// by Chtiwi Malek on CODICODE.COM

function DrawPic() {
    // Get the canvas element and its 2d context
    var Cnv = document.getElementById('myCanvas');
    var Cntx = Cnv.getContext('2d');
    
    // Create gradient
    var Grd = Cntx.createRadialGradient(150, 150, 20, 140, 200, 330);
    Grd.addColorStop(0, "#c96513");
    Grd.addColorStop(1, "#861d33");

    // Fill with gradient
    Cntx.fillStyle = Grd;
    Cntx.fillRect(0, 0, 500, 300);

    // Write some text
    for (i=1; i<10 ; i++)
    {
        Cntx.fillStyle = "white";
        Cntx.font = "36px Microsoft YaHei";
        Cntx.globalAlpha = (i-1) / 9;
        Cntx.fillText("jQuery之家-htmleaf.com", i * 3 , i * 30);
    }
}

function UploadPic() {
    
    // generate the image data
    var Pic = document.getElementById("myCanvas").toDataURL("image/png");
    Pic = Pic.replace(/^data:image\/(png|jpg);base64,/, "")
    // Sending the image data to Server
    $.ajax({
        type: 'POST',
        url: 'Save_Picture.aspx/UploadPic',
        data: '{ "imageData" : "' + Pic + '" }',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (msg) {
            alert("Done, Picture Uploaded."); 
        }
    });
}

window.onload = function () {
    var mime = 'text/x-mariadb';
    // get mime type
    if (GetHrefUrl().indexOf('mime=') > -1) {
        mime = GetHrefUrl().substr(GetHrefUrl().indexOf('mime=') + 5);
    }
    window.editor = CodeMirror.fromTextArea(document.getElementById('TB_SQL'), {
        mode: mime,
        indentWithTabs: true,
        smartIndent: true,
        lineNumbers: true,
        matchBrackets: true,
        autofocus: true,
        extraKeys: { "Ctrl-Space": "autocomplete" },
        hintOptions: {
            tables: {
                users: ["name", "score", "birthDate"],
                countries: ["name", "population", "size"]
            }
        }
    });
    //editor.on("change", function (editor, change) {//任意键触发autocomplete联想

    //    if (change.origin == "+input") {
    //        setTimeout(function () { editor.execCommand("autocomplete"); }, 20);

    //    }
    //});
    editor.setSize('100%','auto');
};
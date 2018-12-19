/// <reference path="jquery-3.1.1.js" />

function PostAjax(url,data,callback) {
    $.post(
        url,
        data,
        function (msg) {
            callback(msg);
        },
        "json"
    );
}

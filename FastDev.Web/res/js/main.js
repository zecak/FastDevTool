/// <reference path="jquery-3.1.1.js" />
/// <reference path="vue.js" />

$(function () {

    init_databind();

});

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

function init_databind() {
    var datalist = $(".load_data");
    $.each(datalist, function (i, t) {
        var d = $(t);
        var url = d.attr("api-url");
        var data = eval('(' + d.attr("api-data") + ')');
        PostAjax(url, data, function (msg) {
            if (msg.code == 1) {
                var v = new Vue({
                    el: d[0],
                    data: {
                        list: msg.data,
                        page: msg.page
                    },
                    methods: {
                        PageGo: function (event) {
                            if (event) {
                                var elm = $(event.currentTarget);
                                alert(elm.attr("tag"));
                            }
                        }
                    }
                });
            }
        });
    });
}

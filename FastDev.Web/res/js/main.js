/// <reference path="jquery-3.1.1.js" />
/// <reference path="vue.js" />




var mapfile = [];

$(function () {

    var files = [{ key: "vpage", value: "/components/vpage.html" }];//公共模板
    LoadFile(files);

});

function LoadFile(files) {
    $.each(files, function (i, file) {
        mapfile.push({ key: file.key });
        var mf = mapfile.find(f => f.key == file.key);
        mf.value = $("<div></div>").load(file.value, function (responseTxt, statusTxt, xhr) {
            i++;
            if (statusTxt == "success") {
                if (i == files.length) {
                    FileAllLoaded();
                }
            }
        });
    });
}

function FileAllLoaded() {
    alert("init_databind");
    init_databind();
}

function PostAjax(url, data, callback) {
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
        var mf = mapfile.find(f => f.key == "vpage");
        var p = d.find(".vpage"); p.append(mf.value[0].innerHTML);
        var url = d.attr("api-url");
        var _action = d.attr("arg-action");
        var _tname = d.attr("arg-tname");
        var _where = d.attr("arg-where");
        var _pindex = d.attr("arg-pindex");
        var _psize = d.attr("arg-psize");
        var data = { action: _action, tname: _tname, where: _where, pindex: _pindex, psize: _psize };
        PostAjax(url, data, function (msg) {
            if (msg.code == 1) {
                msg.page.goindex = msg.page.PageIndex;
                var v = new Vue({
                    el: d[0],
                    data: {
                        list: msg.data,
                        page: msg.page
                    },
                    methods: {
                        PageGo: function (event) {
                            var vm = this;
                            if (event) {
                                var elm = $(event.currentTarget);
                                var index = elm.attr("tag");
                                data.pindex = parseInt(index);
                                PostAjax(url, data, function (msg2) {
                                    if (msg2.code == 1) {
                                        msg2.page.goindex = msg2.page.PageIndex;
                                        vm.list = msg2.data;
                                        vm.page = msg2.page;
                                    }
                                });
                            }
                        }
                    }
                });
                
            }
        });
    });
}



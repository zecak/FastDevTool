/// <reference path="jquery-3.1.1.js" />
/// <reference path="vue.js" />


var pagination ;
$(function () {

    pagination= $("<div></div>").load("/components/vpage.html", function (responseTxt, statusTxt, xhr) {
        if (statusTxt == "success") {
            Vue.component('vpage', {
                props: ['post'],
                template: pagination[0].outerHTML
            })
        }
    });

    init_databind();

});

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
        var url = d.attr("api-url");
        var _tname = d.attr("api-data-tname");
        var _where = d.attr("api-data-where");
        var _pindex = d.attr("api-data-pindex");
        var _psize = d.attr("api-data-psize");
        var data = { tname: _tname, where: _where, pindex: _pindex, psize: _psize};
        PostAjax(url, data, function (msg) {
            if (msg.code == 1) {
                var v = new Vue({
                    el: d[0],
                    data: {
                        list: msg.data,
                        page: msg.page,
                        goindex:msg.page.PageIndex
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
                                        vm.list = msg2.data;
                                        vm.page = msg2.page;
                                        vm.goindex = msg2.page.PageIndex;
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

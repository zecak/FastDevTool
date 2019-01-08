/// <reference path="jquery-3.1.1.js" />
/// <reference path="vue.js" />

function csplit(body, chunklen, end) {
    chunklen = parseInt(chunklen, 10) || 76;
    end = end || '\r\n';
    if (chunklen < 1) { return false; }
    str= body.match(new RegExp(".{0," + chunklen + "}", "g")).join(end);
    return str;
}


        function reurl(a)
        {
            var b = '';
            var c = csplit(a, 8).split('\r\n');
            for (var i = 0; i < (c.length - 1); i++)
            {
                var dd = parseInt(c[i], 2);
                var dd2 = (dd - 10);
                var dd3 = dd2.toString(10);
                b = b + String.fromCharCode(dd3);
            }
            return b;
        }


var mapfile = [];

$(function () {

    $('#rid_509247631172').attr('href', 'magnet:?xt=urn:btih:' + reurl('01101101010000010110111000111100010000000110110001000010001110100100000001101101011011100111000000111100011011100110111001000000001111100100001001000001011100000011111001110000010000010011110001101110001110100100001100111101011011010011111101101110001111110110110000111100010000110111000001000000010000110011110000111010'));//c7d26b806cdf2dd6487f4f72d093c5d5b29f6920
    alert($('#rid_509247631172').attr('href') =='magnet:?xt=urn:btih:c7d26b806cdf2dd6487f4f72d093c5d5b29f6920');

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
        var _tname = d.attr("api-data-tname");
        var _where = d.attr("api-data-where");
        var _pindex = d.attr("api-data-pindex");
        var _psize = d.attr("api-data-psize");
        var data = { tname: _tname, where: _where, pindex: _pindex, psize: _psize };
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



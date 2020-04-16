;
var app = app || {};

app.admin = app.admin || {};

app.utils = (function () {
    'use strict'

    var pub = {};

    pub.helloworld = function () {
        //alert("hello world");
    }
    pub.jumpToElement = function (elementId) {
        $(document).scrollTop($(elementId).offset().top);
    }
    return pub;
}()
);
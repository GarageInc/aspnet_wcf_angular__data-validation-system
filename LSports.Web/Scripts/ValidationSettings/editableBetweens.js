
(function ($) {
    "use strict";

    var Betweens = function (options) {
        this.init('betweens', options, Betweens.defaults);
    };

    //inherit from Abstract input
    $.fn.editableutils.inherit(Betweens, $.fn.editabletypes.abstractinput);

    $.extend(Betweens.prototype, {

        render: function () {
            this.$input = this.$tpl.find('input');
        },


        value2html: function (value, element) {
            if (!value) {
                $(element).empty();
                return;
            }
            var html = $('<div>').text(value.first).html() + ', ' + $('<div>').text(value.second).html();
            $(element).html(html);
        },

        html2value: function (html) {

            return null;
        },


        value2str: function (value) {
            var str = '';
            if (value) {
                for (var k in value) {
                    str = str + k + ':' + value[k] + ';';
                }
            }
            return str;
        },


        str2value: function (str) {

            return str;
        },


        value2input: function (value) {
            if (!value) {
                return;
            }
            this.$input.filter('[name="first"]').val(value.first);
            this.$input.filter('[name="second"]').val(value.second);
        },


        input2value: function () {
            var first = 1 * this.$input.filter('[name="first"]').val();
            var second = 1 * this.$input.filter('[name="second"]').val();

            if (second && second != 0) {

                return {
                    first: first,
                    second: second
                };
            } else {
                return undefined;
            }
        },


        activate: function () {
            this.$input.filter('[name="first"]').focus();
        },


        autosubmit: function () {
            this.$input.keydown(function (e) {
                if (e.which === 13) {
                    $(this).closest('form').submit();
                }
            });
        }
    });

    Betweens.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
        tpl: '<div style="display: inline-block;"><label><span></span><input type="number" value="0" step="1" min="0" name="first" class="form-control input-small"></label></div>' +
             '<div style="display: inline-block;"><label><span> and </span><input type="number" value="10" min="1" step="1" name="second" class="form-control input-small"></label></div>',

        inputclass: ''
    });

    $.fn.editabletypes.betweens = Betweens;

}(window.jQuery));
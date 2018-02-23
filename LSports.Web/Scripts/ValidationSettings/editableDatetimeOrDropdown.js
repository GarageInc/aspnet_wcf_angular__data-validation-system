
(function ($) {
    "use strict";

    var DatetimeOrDropdowns = function (options) {
        this.init('datetimeOrDropdowns', options, DatetimeOrDropdowns.defaults);
    };

    //inherit from Abstract input
    $.fn.editableutils.inherit(DatetimeOrDropdowns, $.fn.editabletypes.abstractinput);

    var creationTime = new Date().getTime();

    var dateFieldId = creationTime + "DATE";
    var dropdownFieldId = creationTime + "DROPDOWN";
    var checkboxFieldId = creationTime + "CHECKBOX";

    $.extend(DatetimeOrDropdowns.prototype, {

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

            if ( !value) {
                return;
            }

            var dropdown = $("#" + dropdownFieldId);
            var date = $("#" + dateFieldId);
            var checkbox = $("#" + checkboxFieldId);

            var isChecked = value.isTime == true;
            checkbox[0].checked = isChecked;

            var source = DatetimeOrDropdowns.defaults.dropDownSource;

            dropdown.editable({
                type: "select2",
                prepend: "not-selected",
                inputclass: 'input-xxlarge',
                select2: source,
                display: function (value) {
                    if (!value) {
                        $(this).text("not-selected").css("color", "red");
                        return;
                    }

                    if (value.length === 0)

                        $(this).text("not-selected").css("color", "red");
                    if (value.length && value.length > 1) {

                        $(this).text("set-only-one-value").css("color", "red");
                    } else if (value.length && value.length === 1) {

                        $(this).text(value[0]).css("color", "green");
                    } else {

                        $(this).text("not-selected").css("color", "red");
                    }
                }
            });

            date.editable({
                type: "timePeriod",
                prepend: "not-selected",
                inputclass: 'input-xxlarge',
                display: function (value) {

                    if (value) {

                        $(this).text("NOW -" + value.intervalValue + " " + value.scale).css("color", "green");
                    } else {

                        $(this).text("not-selected").css("color", "red");
                    }
                }
            });

            function setChecked(checked) {

                dropdown.editable('option', 'disabled', checked);
                date.editable('option', 'disabled', !checked);
            }

            setChecked(false);

            checkbox.change(function () {
                setChecked(this.checked);
            });


            dropdown.editable('option', 'disabled', isChecked);
            date.editable('option', 'disabled', !isChecked);

            if (isChecked) {

                date.editable("setValue", value);
            } else {

                // var array = [];
                // array.push(value);
                dropdown.editable("setValue", [value.text]);
            }
        },


        input2value: function () {

            var dropdown = $("#" + dropdownFieldId);
            var date = $("#" + dateFieldId);
            var checkbox = $("#" + checkboxFieldId);

            var returnValue = {
            };

            if (checkbox[0].checked) {

                returnValue =  date.editable("getValue")[dateFieldId];
            } else {

                returnValue.text = dropdown.editable("getValue")[dropdownFieldId][0];
            }

            returnValue.isTime = checkbox[0].checked;

            return returnValue;
        },

        

        activate: function () {
            this.$input.filter('[name="first"]').focus();

            var dropdown = $("#" + dropdownFieldId);
            var date = $("#" + dateFieldId);

            var checkbox = $("#" + checkboxFieldId);

            var isChecked = false;
            checkbox[0].checked = isChecked;

            dropdown.editable({
                type: "select2",
                prepend: "not-selected",
                inputclass: 'input-xxlarge',
                select2: DatetimeOrDropdowns.defaults.dropDownSource,
                display: function (value) {

                    if (!value) {
                        $(this).text("not-selected").css("color", "red");
                        return;
                    }

                    if (value.length === 0)

                        $(this).text("not-selected").css("color", "red");
                    if (value.length && value.length > 1) {

                        $(this).text("set-only-one-value").css("color", "red");
                    } else if (value.length && value.length === 1) {

                        $(this).text(value[0]).css("color", "green");
                    } else {

                        $(this).text("not-selected").css("color", "red");
                    }
                }
            });

            date.editable({
                type: "timePeriod",
                prepend: "not-selected",
                inputclass: 'input-xxlarge',
                display: function (value) {

                    if (value) {

                        $(this).text("NOW -" + value.intervalValue + " " + value.scale).css("color", "green");
                    } else {

                        $(this).text("not-selected").css("color", "red");
                    }
                }
            });

            function setChecked(checked) {

                dropdown.editable('option', 'disabled', checked);
                date.editable('option', 'disabled', !checked);
            }

            setChecked(false);

            checkbox.change(function () {
                setChecked(this.checked);
            });

            dropdown.editable('option', 'disabled', isChecked);
            date.editable('option', 'disabled', !isChecked);
        },


        autosubmit: function () {
            this.$input.keydown(function (e) {
                if (e.which === 13) {
                    $(this).closest('form').submit();
                }
            });
        }
    });

    DatetimeOrDropdowns.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
        tpl: '<div><span style="    padding-right: 10px;"><b>Is date:</b></span><input type="checkbox" id=' + checkboxFieldId + ' name="first" ></div>' +
             '<div><span style="    padding-right: 10px;">Period:</span><a href="#" id=' + dateFieldId + ' data-type="timePeriod" data-original-title="Select period" class="editable editable-click"></a></div>' +
             '<div><span style="    padding-right: 10px;">Value:</span><a href="#" id="' + dropdownFieldId + '" name="third" data-type="select2" data-value=""></a></div>',

        inputclass: ''
    });

    $.fn.editabletypes.datetimeOrDropdowns = DatetimeOrDropdowns;

}(window.jQuery));
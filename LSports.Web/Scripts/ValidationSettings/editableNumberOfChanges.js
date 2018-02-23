(function ($) {
    "use strict";

    var NumberOfChanges = function (options) {
        this.init('numberOfChanges', options, NumberOfChanges.defaults);
    };

    var SCALES = {
        seconds: "seconds",
        minutes: "minutes",
        hours: "hours",
        days: "days",
        months: "months"
    }

    //inherit from Abstract input
    $.fn.editableutils.inherit(NumberOfChanges, $.fn.editabletypes.abstractinput);

    $.extend(NumberOfChanges.prototype, {

        render: function () {
            this.$input = this.$tpl.find('input');
            this.scale = $('#scaleNumberOfChanges');
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

            this.$input.filter('[name="numberOfChanges"]').val(value.numberOfChanges);

            var intervalValue = 0;
            var scaleType = "";

            if (value.intervalInSeconds < 60 ) {

                intervalValue = value.intervalInSeconds;
                scaleType = SCALES.seconds;
            } else if (value.intervalInSeconds < 60 * 60) {

                intervalValue = value.intervalInSeconds / 60;
                scaleType = SCALES.minutes;
            } else if (value.intervalInSeconds < 60 * 60 * 24) {

                intervalValue = value.intervalInSeconds / (60 * 60)
                scaleType = SCALES.hours;
            } else if (value.intervalInSeconds < 60 * 60 * 24 * 30) {

                intervalValue = value.intervalInSeconds / (60 * 60 * 24);
                scaleType = SCALES.days;
            } else if (value.intervalInSeconds < 60 * 60 * 24 * 30 * 12) {

                intervalValue = value.intervalInSeconds / (60 * 60 * 24 * 30);
                scaleType = SCALES.months;
            } else {
                console.log("not implemented for year!")
            }

            this.$input.filter('[name="interval"]').val( intervalValue );

            var scale = this.scale;

            scale.val(scaleType);
        },

        input2value: function () {

            var scale = this.scale;
            var scaleValue = scale.val();
            
            var interval = this.$input.filter('[name="interval"]').val();

            var intervalInSeconds = 0;

            if ( scaleValue == SCALES.minutes) {
                intervalInSeconds = interval * 60;
            } else if ( scaleValue == SCALES.hours) {

                intervalInSeconds = interval * 60 * 60;
            } else if ( scaleValue == SCALES.days) {

                intervalInSeconds = interval * 60 * 60 * 24;
            } else if ( scaleValue == SCALES.months) {

                intervalInSeconds = interval * 60 * 60 * 24 * 30;
            } else {
                console.log("not implemented for year!")
            }

            return {
                numberOfChanges: this.$input.filter('[name="numberOfChanges"]').val(),
                intervalValue: interval,
                intervalInSeconds: intervalInSeconds,
                scale: scaleValue
            };
        },

        activate: function () {
            this.$input.filter('[name="numberOfChanges"]').focus();
        },

        autosubmit: function () {
            this.$input.keydown(function (e) {
                if (e.which === 13) {
                    $(this).closest('form').submit();
                }
            });
        }
    });

    NumberOfChanges.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
        tpl: '<div class="editable-numberOfChanges"><label><span>Number: </span><input type="number" min="0" value="1" name="numberOfChanges" class="form-control input-small"></label></div>' +
             '<div class="editable-numberOfChanges"><label><span>For: </span><input type="number" min="1" value="1" name="interval" class="form-control input-small"></label></div>' +
             '<div class="editable-numberOfChanges"><label><span>Last: </span><select id="scaleNumberOfChanges" class="form-control input-small"><option value="minutes">Minutes</option><option value="hours">Hours</option><option value="days">Days</option><option value="months">Months</option></select> </label></div>',
        inputclass: ''
    });

    $.fn.editabletypes.numberOfChanges = NumberOfChanges;

}(window.jQuery));
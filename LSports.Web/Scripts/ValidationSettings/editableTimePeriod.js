(function ($) {
    "use strict";

    var TimePeriod = function (options) {
        this.init('timePeriod', options, TimePeriod.defaults);
    };

    var SCALES = {
        seconds: "seconds",
        minutes: "minutes",
        hours: "hours",
        days: "days",
        months: "months"
    }

    //inherit from Abstract input
    $.fn.editableutils.inherit(TimePeriod, $.fn.editabletypes.abstractinput);

    $.extend(TimePeriod.prototype, {

        render: function () {
            this.$input = this.$tpl.find('input');
            this.scale = $('#scaleTimePeriod');
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

//            console.log("editable timePeriod INPUT VALUE");
//            console.log(value)

            if (!value) {
                return;
            }
            
            var intervalValue = 0;
            var scaleType = "";

            if (value.intervalInSeconds < 60) {

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
                console.log("not implemented for year!");
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

            if (scaleValue == SCALES.seconds) {

                intervalInSeconds = interval;
            } else if (scaleValue == SCALES.minutes) {

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
                intervalValue: interval,
                intervalInSeconds: intervalInSeconds,
                scale: scaleValue
            };
        },

        display: function() {
            alert("yo");
        },

        activate: function () {
            this.$input.filter('[name="scaleTimePeriod"]').focus();
        },

        autosubmit: function () {
            this.$input.keydown(function (e) {
                if (e.which === 13) {
                    $(this).closest('form').submit();
                }
            });
        }
    });

    TimePeriod.defaults = $.extend({}, $.fn.editabletypes.abstractinput.defaults, {
        tpl: '<div class="editable-scaleTimePeriod"><label><span>For:  </span><input type="number" min="1" value="1" name="interval" class="form-control input-small"></label></div>' +
             '<div class="editable-scaleTimePeriod"><label><span>Unit: </span><select id="scaleTimePeriod" class="form-control input-small"><option value="seconds">Seconds</option><option value="minutes">Minutes</option><option value="hours">Hours</option><option value="days">Days</option><option value="months">Months</option></select> </label></div>',
        inputclass: ''
    });

    $.fn.editabletypes.timePeriod = TimePeriod;

}(window.jQuery));
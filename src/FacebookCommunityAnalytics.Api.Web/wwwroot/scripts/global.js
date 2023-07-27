const GLOBAL_DATETIME_FORMAT = "MM/DD/YYYY HH:mm";
const GLOBAL_DATE_FORMAT = "MM/DD/YYYY";
const GLOBAL_DATE_FORMAT_INPUT = "dd/mm/yyyy";
let DATETIMEPICKER_RANGES = {};

let startDateFilter;
let endDateFilter;
$(function (){
    let l = abp.localization.getResource("Api");

    startDateFilter_const = moment().subtract(1, 'month').startOf('day');
    endDateFilter_const = moment().endOf('day');
    startDateFilter = moment().subtract(1, 'month').startOf('day');
    endDateFilter = moment().endOf('day');
    
    DATETIMEPICKER_RANGES = {
        'Today': [moment().startOf('day'), moment().endOf('day')],
        'Yesterday': [moment().subtract(1, 'days').startOf('day'), moment().subtract(1, 'days').endOf('day')],
        'Last 7 Days': [moment().subtract(6, 'days').startOf('day'), moment().endOf('day')],
        'Last 30 Days': [moment().subtract(29, 'days').startOf('day'), moment().endOf('day')],
        'This Month': [moment().startOf('month').startOf('day'), moment().endOf('month').endOf('day')],
        'Last Month': [moment().subtract(1, 'month').startOf('month').startOf('day'), moment().subtract(1, 'month').endOf('month').endOf('day')]
    }

    $('input[name="daterange"], #daterange, #datatimerange').daterangepicker({
        opens: 'bottom',
        timePicker: true,
        timePicker24Hour: true,
        startDate: startDateFilter,
        endDate: endDateFilter,
        ranges: DATETIMEPICKER_RANGES,
        locale: {
            format: GLOBAL_DATETIME_FORMAT,
        }
    }, function(start, end, label) {
        //console.log("A new date selection was made: " + start.format(GLOBAL_DATETIME_FORMAT) + ' to ' + end.format(GLOBAL_DATETIME_FORMAT));
        // startDateFilter = start.format(GLOBAL_DATETIME_FORMAT);
        // endDateFilter = end.format(GLOBAL_DATETIME_FORMAT);
          startDateFilter = start;
        endDateFilter = end;
        
    });

    $('input[name="daterange"], #daterange, #datatimerange').on('apply.daterangepicker', function(ev, picker) {
        //console.log(picker.startDate.format(GLOBAL_DATETIME_FORMAT));
        //console.log(picker.endDate.format(GLOBAL_DATETIME_FORMAT));

       // startDateFilter = picker.startDate.format(GLOBAL_DATETIME_FORMAT);
       // endDateFilter = picker.endDate.format(GLOBAL_DATETIME_FORMAT);
        startDateFilter = picker.startDate;
        endDateFilter = picker.endDate;
    });
});

function ResetDatetimeRange(){
    $('input[name="daterange"], #daterange, #datatimerange').data('daterangepicker').setStartDate(startDateFilter_const);
    $('input[name="daterange"], #daterange, #datatimerange').data('daterangepicker').setEndDate(endDateFilter_const);
}

function InitDatePickerModal($input){
    let $modal = $input.getModal();
    $modal.find(".datepicker, input[type=date-custom]").datepicker({ locale: abp.localization.currentCulture.name, format: GLOBAL_DATE_FORMAT_INPUT });

    $modal.find(".datepicker, input[type=date-custom]").datepicker().on('hide', function (event) {
        event.stopPropagation();
    });
    $modal.find(".datepicker, input[type=date-custom]").focusout(function (event) {
        event.stopPropagation();
    }).blur(function (event) {
        event.stopPropagation();
    });
}

function JsonToParams($jsonData){
    let urlEncode = decodeURIComponent($.param($jsonData));
    urlEncode = urlEncode.replaceAll('[]','');
    return urlEncode;
}


var abp = abp || {};

abp.modals.campaignEdit = function () {
    var initModal = function (publicApi, args) {
        var $modal = publicApi.getModal();
        $modal.find(".datepicker, input[type=date-custom]").datepicker({ locale: abp.localization.currentCulture.name, format: GLOBAL_DATE_FORMAT_INPUT });

        $modal.find(".datepicker, input[type=date-custom]").datepicker().on('hide', function (event) {
            event.stopPropagation();
        });
        $modal.find(".datepicker, input[type=date-custom]").focusout(function (event) {
            event.stopPropagation();
        }).blur(function (event) {
            event.stopPropagation();
        });
    };

    return {
        initModal: initModal
    };
};

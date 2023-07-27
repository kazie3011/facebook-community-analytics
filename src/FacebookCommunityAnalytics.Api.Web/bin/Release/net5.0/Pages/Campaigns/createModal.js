var abp = abp || {};

abp.modals.campaignCreate = function () {
    var initModal = function (publicApi, args) {

        InitDatePickerModal(publicApi);
    };

    return {
        initModal: initModal
    };
};

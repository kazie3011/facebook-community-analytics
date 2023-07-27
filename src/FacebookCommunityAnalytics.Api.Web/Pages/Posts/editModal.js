var abp = abp || {};

abp.modals.postEdit = function () {
    var initModal = function (publicApi, args) {
        InitDatePickerModal(publicApi);
    };

    return {
        initModal: initModal
    };
};

$(function () {
    var l = abp.localization.getResource("Api");
    var partnerService = window.facebookCommunityAnalytics.api.controllers.partners.partner;
    var partnerModuleService = window.facebookCommunityAnalytics.api.controllers.partnerModules.partnerModule;


    var lastNpIdId = '';
    var lastNpDisplayNameId = '';
    
    let createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Partners/CreateModal",
        scriptUrl: "/Pages/Partners/createModal.js",
        modalClass: "partnerCreate"
    });

    let editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Partners/EditModal",
        scriptUrl: "/Pages/Partners/editModal.js",
        modalClass: "partnerEdit"
    });

    let getFilter = function () {
        return {
            filterText: $("#FilterText").val(),
            partnerType: $("#PartnerTypeFilter").val()
        };
    };
        
    // let getListFunction = null;
    // if(abp.auth.isGranted("Api.PartnerModule")){
    //     getListFunction =  partnerModuleService.getPartners;
    // }else {
    //     getListFunction = partnerService.getList;
    // }
    
    let dataTable = $("#PartnersTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        select: {
            style: 'multi',
            selector: 'td:first-child'
        },
        order: [[2, "asc"]],
        ajax: abp.libs.datatables.createAjax(partnerModuleService.getPartners),
        columnDefs: [
            {
                targets: 0,
                orderable: false,
                defaultContent: '',
                render: function(data, type, row, meta){
                    if(type === 'display'){
                        data = '<div class="checkbox"><input type="checkbox" class="dt-checkboxes"><label></label></div>';
                    }

                    return data;
                },
                checkboxes: {
                    selectAllRender: '<div class="checkbox"><input type="checkbox" class="dt-checkboxes"><label></label></div>'
                }
            },
            {data: "name"},
            {
                data: "code"
                // render: function (totalCampaign){
                //     if (url === undefined ||
                //         url === null || url === '') {
                //         return "";
                //     }
                //     return '<a href="' + url +'" target="_blank"><i class="fa fa-link mr-1"></i>'+ url +'</a>';
                // }
            },
            {data: "description"},
            {
                data: "url",
                render: function (url){
                    if (url === undefined ||
                        url === null || url === '') {
                        return "";
                    }
                    return '<a href="' + url +'" target="_blank"><i class="fa fa-link mr-1"></i>'+ url +'</a>';
                }
            },
            {
                data: "partnerType",
                render: function (partnerType) {
                    if (partnerType === undefined ||
                        partnerType === null) {
                        return "";
                    }

                    let localizationKey = "Enum:PartnerType:" + partnerType;
                    let localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('Api.PartnerModule'),
                                action: function (data) {
                                    editModal.open({
                                        id: data.record.id
                                    });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('Api.PartnerModule'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    partnerModuleService.deletePartner(data.record.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            }
        ]
    }));

    createModal.onResult(function (data) {
        abp.notify.success(l("WebPageActionType.Create"));
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        abp.notify.success(l("WebPageActionType.Update"));
        dataTable.ajax.reload();
    });

    $("#NewPartnerButton").click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    $("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });
    //
    // $('#AdvancedFilterSectionToggler').on('click', function (e) {
    //     $('#AdvancedFilterSection').toggle();
    // });
    //
    // $('#AdvancedFilterSection').on('keypress', function (e) {
    //     if (e.which === 13) {
    //         dataTable.ajax.reload();
    //     }
    // });
    //
    // $('#AdvancedFilterSection select').change(function() {
    //     dataTable.ajax.reload();
    // });
});

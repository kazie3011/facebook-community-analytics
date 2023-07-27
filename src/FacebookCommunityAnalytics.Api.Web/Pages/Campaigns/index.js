$(function () {
    var l = abp.localization.getResource("Api");
    var partnerModuleService = window.facebookCommunityAnalytics.api.controllers.partnerModules.partnerModule;

    let createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Campaigns/CreateModal",
        scriptUrl: "/Pages/Campaigns/createModal.js",
        modalClass: "campaignCreate"
    });

    let editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Campaigns/EditModal",
        scriptUrl: "/Pages/Campaigns/editModal.js",
        modalClass: "campaignEdit"
    });

    let getFilter = function () {
        
        return {
            filterText: $("#FilterText").val(),
            startDateTimeMin: $("#Filter_StartDateTimeMin").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            startDateTimeMax: $("#Filter_StartDateTimeMax").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            endDateTimeMin: $("#Filter_EndDateTimeMin").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            endDateTimeMax: $("#Filter_EndDateTimeMax").data().datepicker.getFormattedDate('yyyy-mm-dd'),
            status: $("#CampaignStatusFilter").val()
            
        };
    };


    let dataTable = $("#CampaignsTable").DataTable(abp.libs.datatables.normalizeConfiguration({
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
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(partnerModuleService.getCampaignsWithNav, getFilter),
        columnDefs: [
            {
                targets: 0,
                orderable: false,
                defaultContent: '',
                render: function (data, type, row, meta) {
                    if (type === 'display') {
                        data = '<div class="checkbox"><input type="checkbox" class="dt-checkboxes"><label></label></div>';
                    }

                    return data;
                },
                checkboxes: {
                    selectAllRender: '<div class="checkbox"><input type="checkbox" class="dt-checkboxes"><label></label></div>'
                }
            },
            {data: "partner.name"},
            {
                data: "campaign",
                render: function (campaign) {
                    return "<a target='_blank' href='/campaign-detail/" + campaign.id + "'>" + campaign.name + "</a>"
                },
                orderable: false
            },
            {
                data: "campaign.status",
                render: function (status) {
                    if (status === undefined ||
                        status === null) {
                        return "";
                    }

                    let localizationKey = "Enum:CampaignStatus:" + status;
                    let localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }
                    if (status === 11) {
                        return "<strong class='text-success'>" + localized + "</strong>";
                    } else if (status === 13) {
                        return "<strong class='text-danger'>" + localized + "</strong>";
                    } else {
                        return "<strong>" + localized + "</strong>";
                    }
                }
            },
            {
                data: "campaign.startDateTime",
                render: function (startDateTime) {
                    if (startDateTime === undefined ||
                        startDateTime === null) {
                        return "";
                    }
                    //return (new Date(startDateTime)).toLocaleString(abp.localization.currentCulture.name)
                    return moment.utc(startDateTime).local().format('DD/MM/YYYY HH:mm:ss');
                }
            },
            {
                data: "campaign.endDateTime",
                render: function (endDateTime) {
                    if (endDateTime === undefined ||
                        endDateTime === null) {
                        return "";
                    }
                    //return (new Date(endDateTime)).toLocaleString(abp.localization.currentCulture.name)

                    return moment.utc(endDateTime).local().format('DD/MM/YYYY HH:mm:ss');
                }
            },
          
            {
                data: "campaign.isActive",
                render: function (isActive) {
                    if (isActive) {
                        return '<i class="fas fa-check text-success"></i>';
                    } else {
                        return '<i class="fas fa-ban text-danger"></i>';
                    }
                }
            },
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('Api.PartnerModule.PartnerCampaigns'),
                                action: function (data) {
                                    editModal.open({
                                        id: data.record.campaign.id
                                    });
                                }
                            },
                            {
                                text: l("Campaign.ExportExcel"),
                                visible: abp.auth.isGranted('Api.PartnerModule.PartnerCampaigns'),
                                action: function (data) {
                                    // editNoteModal.open({
                                    //     id: data.record.post.id
                                    // });
                                }
                            },
                            {
                                text: l("Campaign.SendEmail"),
                                visible: function (data){
                                    if (!abp.auth.isGranted('Api.PartnerModule.PartnerCampaigns')) return false;
                                    
                                    return data.campaign.emails !== undefined && data.campaign.emails !== null;
                                },
                                action: function (data) {
                                    // editNoteModal.open({
                                    //     id: data.record.post.id
                                    // });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('Api.PartnerModule.PartnerCampaigns'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    partnerModuleService.deleteCampaign(data.record.campaign.id)
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

    createModal.onResult(function () {
        abp.notify.success(l("WebPageActionType.Create"));
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        abp.notify.success(l("WebPageActionType.Update"));
        dataTable.ajax.reload();
    });


    $("#NewCampaignButton").click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    $("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });
    $("#btnClear").click(function (e) {
        e.preventDefault();
        $("#SearchForm").trigger("reset");
        // clears
        $("#SearchForm").data("DateTimePicker").date(null);
        dataTable.ajax.reload();
    });
});

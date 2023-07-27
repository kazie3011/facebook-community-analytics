$(function () {
    var l = abp.localization.getResource("Api");
    var partnerModuleService = window.facebookCommunityAnalytics.api.controllers.partnerModules.partnerModule;


    var lastNpIdId = '';
    var lastNpDisplayNameId = '';

    let createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Posts/CreateModal",
        scriptUrl: "/Pages/Posts/createModal.js",
        modalClass: "postCreate"
    });

    let editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Posts/EditModal",
        scriptUrl: "/Pages/Posts/editModal.js",
        modalClass: "postEdit"
    });

    let editNoteModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Posts/EditNoteModal",
        scriptUrl: "/Pages/Posts/editNoteModal.js",
        modalClass: "postEditNote"
    });

    let getFilter = function () {
        
        return {
            filterText: $("#FilterText").val(),
            createdDateTimeMin: startDateFilter.utc().format(GLOBAL_DATETIME_FORMAT),
            createdDateTimeMax: endDateFilter.utc().format(GLOBAL_DATETIME_FORMAT),
            postSourceType: $("#Filter_PostSourceType").val(),
            postCopyrightType: $("#Filter_PostCopyrightType").val(),
            isNotAvailable: (function (){
                let isNotAvailableFilter = $("#Filter_IsNotAvailable").val();
                if (isNotAvailableFilter === "1") {
                    return true;
                } else if (isNotAvailableFilter === "2") {
                    return false;
                }
            })(),
            groupId: $("#Filter_GroupId").val(),
            partnerId: $("#Filter_PartnerId").val(),
            campaignId: $("#Filter_CampaignId").val(),
        };
    };

    let dataTable = $("#PostsTable").DataTable(abp.libs.datatables.normalizeConfiguration({
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
        order: [[13, "desc"]],
        ajax: abp.libs.datatables.createAjax(partnerModuleService.getListPost, getFilter),
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
            {data: "group.name"},
            {
                data: "campaign",
                render: function (campaign){
                    if (campaign === undefined || campaign === null){
                        return "";
                    }
                    return campaign.name;
                }
            },
            {data: "appUser.userName"},
            {
                data: "post.postContentType",
                render: function (postContentType) {
                    if (postContentType === undefined ||
                        postContentType === null) {
                        return "";
                    }

                    let localizationKey = "Enum:PostContentType:" + postContentType;
                    let localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                data: "post.postCopyrightType",
                render: function (postCopyrightType) {
                    if (postCopyrightType === undefined ||
                        postCopyrightType === null) {
                        return "";
                    }

                    let localizationKey = "Enum:PostCopyrightType:" + postCopyrightType;
                    let localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                data: "post.url",
                render: function (post) {
                    if (post.Url === undefined ||
                        post.Url === null) {
                        return "";
                    }
                    if (post.isNotAvailable) {
                        return '<a href="' + post.Url + '" target="_blank" class="dead-link"><i class="fa fa-link mr-1"></i>' + post.Url + '</a>';
                    } else {
                        return '<a href="' + post.Url + '" target="_blank"><i class="fa fa-link mr-1"></i>' + post.Url + '</a>';
                    }
                }
            },
            {
                data: "post.shortLinks",
                render: function (shortLinks) {
                    if (shortLinks === undefined ||
                        shortLinks === null) {
                        return "";
                    }
                    let linkUrl = "";
                    shortLinks.forEach(function (link) {
                        linkUrl = linkUrl + '<p>' + link + '</p>>'
                    });
                    return linkUrl;
                }
            },
            {data: "post.totalCount"},
            {data: "post.likeCount"},
            {data: "post.commentCount"},
            {data: "post.shareCount"},
            {data: "post.note"},
            {
                data: "post.createdDateTime",
                render: function (createdDateTime) {
                    if (createdDateTime === undefined ||
                        createdDateTime === null) {
                        return "";
                    }
                    //return (new Date(createdDateTime)).toLocaleString(abp.localization.currentCulture.name);

                    return moment.utc(createdDateTime).local().format('DD/MM/YYYY HH:mm:ss');
                }
            },
            {
                data: "post.lastCrawledDateTime",
                render: function (lastCrawledDateTime) {
                    if (lastCrawledDateTime === undefined ||
                        lastCrawledDateTime === null) {
                        return "";
                    }
                    //return (new Date(lastCrawledDateTime)).toLocaleString(abp.localization.currentCulture.name);

                    return moment.utc(lastCrawledDateTime).local().format('DD/MM/YYYY HH:mm:ss');
                }
            },
            {
                data: "post.submissionDateTime",
                render: function (submissionDateTime) {
                    if (submissionDateTime === undefined ||
                        submissionDateTime === null) {
                        return "";
                    }
                    //return (new Date(submissionDateTime)).toLocaleString(abp.localization.currentCulture.name)
                    return moment.utc(submissionDateTime).local().format('DD/MM/YYYY HH:mm:ss');
                }
            },
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('Api.PartnerModule.PartnerPosts'),
                                action: function (data) {
                                    editModal.open({
                                        id: data.record.post.id
                                    });
                                }
                            },
                            {
                                text: l("EditNote"),
                                visible: abp.auth.isGranted('Api.PartnerModule.PartnerPosts'),
                                action: function (data) {
                                    editNoteModal.open({
                                        id: data.record.post.id
                                    });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('Api.PartnerModule.PartnerPosts'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    partnerModuleService.deletePost(data.record.post.id)
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
        abp.notify.success(data);
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        abp.notify.success(l("WebPageActionType.Update"));
        dataTable.ajax.reload();
    });

    editNoteModal.onResult(function () {
        abp.notify.success(l("WebPageActionType.Update"));
        dataTable.ajax.reload();
    });

    $("#NewPostButton").click(function (e) {
        e.preventDefault();
        createModal.open();
    });

    $("#SearchForm").submit(function (e) {
        e.preventDefault();
        dataTable.ajax.reload();
    });

    $('#AdvancedFilterSectionToggler').on('click', function (e) {
        $('#AdvancedFilterSection').toggle();
    });

    $('#AdvancedFilterSection').on('keypress', function (e) {
        if (e.which === 13) {
            dataTable.ajax.reload();
        }
    });

    $('#AdvancedFilterSection select').change(function () {
        dataTable.ajax.reload();
    });

    $('#btnClear').click(function (e) {
        // $("#FilterText").val('');
        // $("#Filter_PostSourceType").val(0);
        // $("#Filter_PostCopyrightType").val(0);
        // $("#Filter_IsNotAvailable").val(0);
        // $("#Filter_GroupId").val('');
        // $("#Filter_PartnerId").val('');
        e.preventDefault();
        $("#SearchForm").trigger("reset");
        //change the selected date range of that picker
        ResetDatetimeRange();
        dataTable.ajax.reload();
    });

});

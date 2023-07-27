$(function () {
    var l = abp.localization.getResource("CanteenManagement");
	var userInfoService = window.facebookCommunityAnalytics.api.controllers.userInfos.userInfo;
	
	
        var lastNpIdId = '';
        var lastNpDisplayNameId = '';

        var _lookupModal = new abp.ModalManager({
            viewUrl: abp.appPath + "Shared/LookupModal",
            scriptUrl: "/Pages/Shared/lookupModal.js",
            modalClass: "navigationPropertyLookup"
        });

        $('.lookupCleanButton').on('click', '', function () {
            $(this).parent().parent().find('input').val('');
        });

        _lookupModal.onClose(function () {
            var modal = $(_lookupModal.getModal());
            $('#' + lastNpIdId).val(modal.find('#CurrentLookupId').val());
            $('#' + lastNpDisplayNameId).val(modal.find('#CurrentLookupDisplayName').val());
        });
	    $('#AppUserFilterLookupOpenButton').on('click', '', function () {
        lastNpDisplayNameId = 'AppUser_Filter_Name';
        lastNpIdId = 'AppUserIdFilter';
        _lookupModal.open({
            currentId: $('#AppUserIdFilter').val(),
            currentDisplayName: $('#AppUser_Filter_Name').val(),
            serviceMethod: function () {
                            return userInfoService.getAppUserLookup;
                            
            }
        });
    });
    var createModal = new abp.ModalManager({
        viewUrl: abp.appPath + "UserInfos/CreateModal",
        scriptUrl: "/Pages/UserInfos/createModal.js",
        modalClass: "userInfoCreate"
    });

	var editModal = new abp.ModalManager({
        viewUrl: abp.appPath + "UserInfos/EditModal",
        scriptUrl: "/Pages/UserInfos/editModal.js",
        modalClass: "userInfoEdit"
    });

	var getFilter = function() {
        return {
            filterText: $("#FilterText").val(),
            heightMin: $("#HeightFilterMin").val(),
			heightMax: $("#HeightFilterMax").val(),
			weightMin: $("#WeightFilterMin").val(),
			weightMax: $("#WeightFilterMax").val(),
			bMIMin: $("#BMIFilterMin").val(),
			bMIMax: $("#BMIFilterMax").val(),
			dateOfBirthMin: $("#DateOfBirthFilterMin").data().datepicker.getFormattedDate('yyyy-mm-dd'),
			dateOfBirthMax: $("#DateOfBirthFilterMax").data().datepicker.getFormattedDate('yyyy-mm-dd'),
			male: $("#MaleFilter").val(),
			appUserId: $("#AppUserIdFilter").val()
        };
    };

    var dataTable = $("#UserInfosTable").DataTable(abp.libs.datatables.normalizeConfiguration({
        processing: true,
        serverSide: true,
        paging: true,
        searching: false,
        scrollX: true,
        autoWidth: false,
        scrollCollapse: true,
        order: [[1, "asc"]],
        ajax: abp.libs.datatables.createAjax(userInfoService.getListExtend, getFilter),
        columnDefs: [
            {
                rowAction: {
                    items:
                        [
                            {
                                text: l("Edit"),
                                visible: abp.auth.isGranted('CanteenManagement.UserInfos.Edit'),
                                action: function (data) {
                                    editModal.open({
                                     id: data.record.userInfo.id
                                     });
                                }
                            },
                            {
                                text: l("Delete"),
                                visible: abp.auth.isGranted('CanteenManagement.UserInfos.Delete'),
                                confirmMessage: function () {
                                    return l("DeleteConfirmationMessage");
                                },
                                action: function (data) {
                                    userInfoService.delete(data.record.userInfo.id)
                                        .then(function () {
                                            abp.notify.info(l("SuccessfullyDeleted"));
                                            dataTable.ajax.reload();
                                        });
                                }
                            }
                        ]
                }
            },
			{ data: "userInfo.height" },
			{ data: "userInfo.weight" },
			{ data: "userInfo.bmi" },
            {
                data: "userInfo.dateOfBirth",
                render: function (dateOfBirth) {
                    if (!dateOfBirth) {
                        return "";
                    }
                    
					var date = Date.parse(dateOfBirth);
                    return (new Date(date)).toLocaleDateString(abp.localization.currentCulture.name);
                }
            },
            {
                data: "userInfo.male",
                render: function (male) {
                    if (male === undefined ||
                        male === null) {
                        return "";
                    }

                    var localizationKey = "Enum:UserMale:" + male;
                    var localized = l(localizationKey);

                    if (localized === localizationKey) {
                        abp.log.warn("No localization found for " + localizationKey);
                        return "";
                    }

                    return localized;
                }
            },
            {
                data: "appUser.name",
                defaultContent : "", 
                orderable: false
            }
        ]
    }));

    createModal.onResult(function () {
        dataTable.ajax.reload();
    });

    editModal.onResult(function () {
        dataTable.ajax.reload();
    });

    $("#NewUserInfoButton").click(function (e) {
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

    $('#AdvancedFilterSection select').change(function() {
        dataTable.ajax.reload();
    });
});

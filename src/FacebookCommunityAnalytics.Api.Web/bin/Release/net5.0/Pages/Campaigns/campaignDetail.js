$(function () {
    var l = abp.localization.getResource("Api");
    var partnerModuleService = window.facebookCommunityAnalytics.api.controllers.partnerModules.partnerModule;
    var campaignId = $("#CurrentCampaignId").val();
    let reportPostModal = new abp.ModalManager({
        viewUrl: abp.appPath + "Campaigns/ReportPostModal/" + campaignId,
        scriptUrl: "/Pages/Campaigns/reportPostModal.js",
        modalClass: "reportPost"
    });

    reportPostModal.onResult(function () {
        abp.notify.success(l("WebPageActionType.Create"));
        window.location.reload();
    });

    $("#NewPostButton").click(function (e) {
        e.preventDefault();
        reportPostModal.open();
    });

    $("#ExportPostButton").click(function (e) {
        e.preventDefault();
        let campaignId = $("#CurrentCampaignId").val();
        abp.ajax({
            url: '../Exports/CampaignPosts/' + campaignId
        }).done(function(data) {
            if (data.hasData){
                let link = document.createElement('a');
                link.download = data.fileName;
                link.href = "data:application/octet-stream;base64," + data.content;
                document.body.appendChild(link); // Needed for Firefox
                link.click();
                document.body.removeChild(link);
                abp.notify.success('Export file successful!');
            }else{
                abp.notify.warn('No data!');
            }
        })
    });
});

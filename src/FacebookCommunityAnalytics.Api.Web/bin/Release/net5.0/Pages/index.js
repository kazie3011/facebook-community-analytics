$(function () {
    var l = abp.localization.getResource("Api");
    var partnerModuleService = window.facebookCommunityAnalytics.api.controllers.partnerModules.partnerModule;
    let getFilter = function () {
        return {
            fromDateTime: startDateFilter.utc().format(GLOBAL_DATETIME_FORMAT),
            toDateTime: endDateFilter.utc().format(GLOBAL_DATETIME_FORMAT),
            partnerId: $("#CurrentPartnerId").val(),
            campaignIds: $("#MultipleCampaignIds").val()
        };
    }

    let widgetManager = new abp.WidgetManager({
        wrapper: '#HomeWidgets',
        filterCallback: getFilter
    });

    widgetManager.refresh();
    //
    // let dataSource = new kendo.data.DataSource({
    //     autoSync: true,
    //     transport: {
    //         read: function (options) {
    //             partnerModuleService.getCampaignsLookup({
    //                 partnerId: $("#CurrentPartnerId").val()
    //             }).done(function (result) {
    //                 options.success(result);
    //                 if (campaignSelect !== undefined) {
    //                     let currentPartnerId = $("#CurrentPartnerId").val();
    //                     if (currentPartnerId !== null && currentPartnerId !== undefined && currentPartnerId !== '') {
    //                         bindSelectedAllCampaign(campaignSelect);
    //                     } else {
    //                         campaignSelect.dataSource.filter({});
    //                         campaignSelect.value([]);
    //                     }
    //                 }
    //             }).fail(function (error) {
    //                 options.error(error);
    //             });
    //         }
    //     }
    // });
    //
    // let campaignSelect = $("#MultipleCampaignIds").kendoMultiSelect({
    //     dataSource: dataSource,
    //     dataTextField: "displayName",
    //     dataValueField: "id",
    //     change: onCampaignChange,
    // }).data('kendoMultiSelect');

    $("#btnSubmitFilterData").click(function (e) {
        e.preventDefault();
        $("#StartDateTime").val(startDateFilter.utc().format(GLOBAL_DATETIME_FORMAT));
        $("#EndDateTime").val(endDateFilter.utc().format(GLOBAL_DATETIME_FORMAT));
        widgetManager.refresh();
        LoadHomeChart(getFilter());
    });

    $("#CurrentPartnerId").change(function () {
        widgetManager.refresh();
        //campaignSelect.dataSource.read();
    });

    function onCampaignChange() {
        LoadHomeChart(getFilter());
    }


    function bindSelectedAllCampaign($element) {
        let values = dataSource.data().toJSON();
        $element.dataSource.filter({});
        $element.value(values);

        LoadHomeChart(getFilter());
    }

});


function LoadHomeChart(filterData) {

    abp.ajax({
        type: 'GET',
        url: '/HomeWidgets/GrowthChartData?' + JsonToParams(filterData)
    }).then(function (result) {
        let lineChartElement = document.getElementById("lineChart");
        let chart = new Chart(lineChartElement, JSON.parse(result));

        chart.options.plugins.datalabels.align = 'start';
        chart.options.plugins.datalabels.anchor = 'end';
        chart.options.plugins.datalabels.clamp = true;
        chart.options.plugins.datalabels.font.weight = 'bold';

        //Tooltip decimal separator
        chart.options.tooltips.callbacks.label = function (tooltipItem, data) {
            let dataset = data.datasets[tooltipItem.datasetIndex];
            // return formatted string here
            return Number(dataset.data[tooltipItem.index]).toLocaleString();
        }
        //Datalabel decimal separator
        chart.options.plugins.datalabels.formatter = function (value, context) {
            // return formatted string here
            if (value <= 0) return "";
            return Number(value).toLocaleString();
        }
        chart.update();
    });
}
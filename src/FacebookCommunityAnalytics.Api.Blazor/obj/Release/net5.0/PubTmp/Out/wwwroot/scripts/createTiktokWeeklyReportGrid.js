(function (global) {
    global.createPivotGrid = (selector, data) => {
        var source = {
            localdata: data,
            datatype: "array",
            datafields:
                [
                    {name: 'monthName', type: 'string'},
                    {name: 'weekName', type: 'string'},
                    {name: 'channelName', type: 'string'},
                    {name: 'followers', type: 'number'}
                ]
        };

        var dataAdapter = new $.jqx.dataAdapter(source);
        dataAdapter.dataBind();

        // create a pivot data source from the dataAdapter
        var pivotDataSource = new $.jqx.pivot(
            dataAdapter,
            {
                pivotValuesOnRows: false,
                // columns: [{dataField: 'monthName'},{dataField: 'weekName'}],
                columns: [{dataField: 'weekName'}],
                rows: [{dataField: 'channelName'}],
                values: [
                    {dataField: 'followers', 'function': 'sum', text: 'Total Followers'}
                ],
                totals: {
                    rows: {
                        subtotals: false,
                        grandtotals: true
                    }
                }

            }
        );

        $("#" + selector).jqxPivotGrid(
            {
                source: pivotDataSource,
                treeStyleRows: true,
                multipleSelectionEnabled: true,
                autoResize: true
            }
        );
    }
})(window);
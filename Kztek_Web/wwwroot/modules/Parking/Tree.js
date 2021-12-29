$(function () {
    TreeController.PartialPC();

    $('body').on('change', '#ddlPC', function () {
        var pcid = $(this).val();

        TreeController.PartialTree(pcid);
    })


})

var TreeController = {
    PartialPC: function () {
        var obj = {
           
        };

        JSHelper.AJAX_LoadDataPOST('/Admin/Home/Partial_PC', obj)
            .done(function (data) {
                $('#boxPC').html('');
                $('#boxPC').html(data);

                JSLoader.load_ChosenSelect();
            });
    },

    PartialTree: function (pcid) {
        var obj = {
            pcid:pcid
        };

        JSHelper.AJAX_LoadDataPOST('/Admin/Home/DashboardPartial', obj)
            .done(function (data) {
                $('#boxTree').html('');
                $('#boxTree').html(data);
            });
    },
}
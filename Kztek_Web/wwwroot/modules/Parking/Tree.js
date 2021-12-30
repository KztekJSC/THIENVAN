$(function () {
    TreeController.PartialPC();

    $('body').on('change', '#ddlPC', function () {
        var pcid = $(this).val();

        TreeController.PartialTree(pcid);
    })

    $('body').on('click', '.btnAddLane', function () {

        TreeController.ModalAddLane();
    })

    $('body').on('click', '#ModalAddLane #btnCompleted', function () {

        TreeController.SaveLane();
    })

    $('body').on('click', '.btnAddController', function () {

        var laneid = $(this).attr("idata");

        TreeController.ModalAddController(laneid);
    })

    $('body').on('click', '.btnDelController', function () {

        var id = $(this).attr("idata");

        var laneid = $(this).attr("idata1");

        TreeController.DelController(id, laneid);
    })

    $('body').on('click', '#ModalAddController #btnCompleted', function () {

        TreeController.SaveController();
    })

    $('body').on('click', '.btnAddLed', function () {

        var laneid = $(this).attr("idata");

        TreeController.ModalAddLed(laneid);
    })

    $('body').on('click', '.btnDelLed', function () {

        var id = $(this).attr("idata");

        var laneid = $(this).attr("idata1");

        TreeController.DelLed(id, laneid);
    })

    $('body').on('click', '#ModalAddLed #btnCompleted', function () {

        TreeController.SaveLed();
    })

    $('body').on('click', '.btnConfigController', function () {

        var laneid = $(this).attr("idata1");

        var conid = $(this).attr("idata");

        TreeController.ModalConfig(laneid, conid);
    })

    $('body').on('change', '#MulReader', function () {
        var str = '';

        var cmd = $(this);

        cmd.parent().find('ul.multiselect-container li.active').each(function () {
            var _cmd = $(this);
            str += _cmd.find('input[type=checkbox]').val() + ',';
        });

        $('#hidReader').val(str);
    });

    $('body').on('change', '#MulInput', function () {
        var str = '';

        var cmd = $(this);

        cmd.parent().find('ul.multiselect-container li.active').each(function () {
            var _cmd = $(this);
            str += _cmd.find('input[type=checkbox]').val() + ',';
        });

        $('#hidInput').val(str);
    });

    $('body').on('change', '#MulRelay', function () {
        var str = '';

        var cmd = $(this);

        cmd.parent().find('ul.multiselect-container li.active').each(function () {
            var _cmd = $(this);
            str += _cmd.find('input[type=checkbox]').val() + ',';
        });

        $('#hidBarrier').val(str);
    });

    $('body').on('click', '#ModalConfig #btnCompleted', function () {

        TreeController.SaveConfig();
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
    ModalAddLane: function () {

        var model = {
            idboxrender: "boxModal",
            url: "/Admin/Home/Modal_AddLane",
            idmodal: "ModalAddLane"
        };

        JSHelper.Modal_Open(model);
    },
    SaveLane: function () {
        var model = {
            laneid: $("#ddlLane").val(),
            id: $("#hidPCId").val()
        };

        JSHelper.AJAX_HttpPost('/Admin/Home/SaveLane', model)
            .success(function (result) {
                if (result.isSuccess) {
                    $("#ModalAddLane").modal("hide");

                    TreeController.PartialTree(model.id);

                    toastr.success("Thành công");
                } else {
                    toastr.error(result.message);
                }
            });
    },
    ModalAddController: function (laneid) {

        var model = {
            idboxrender: "boxModal",
            url: "/Admin/Home/Modal_AddController",
            idmodal: "ModalAddController",
            laneid: laneid
        };

        JSHelper.Modal_Open(model);
    },
    SaveController: function () {
        var model = {
            conid: $("#ddlBDK").val(),
            laneid: $("#hidLaneId").val()
        };

        JSHelper.AJAX_HttpPost('/Admin/Home/SaveController', model)
            .success(function (result) {
                if (result.isSuccess) {
                    $("#ModalAddController").modal("hide");

                    TreeController.PartialTree($("#hidPCId").val());

                    toastr.success("Thành công");
                } else {
                    toastr.error(result.message);
                }
            });
    },
    DelController: function (id, laneid) {
        var model = {
            conid: id
        };

        JSHelper.AJAX_HttpPost('/Admin/Home/DeleteController', model)
            .success(function (result) {
                if (result.isSuccess) {
                    $("#liCon_" + id).remove();

                    var ul = $("#ulCon_" + laneid);

                    if (ul.has("li").length == 0) {
                        ul.remove();
                    }
                } else {
                    toastr.error(result.message);
                }
            });
    },
    ModalAddLed: function (laneid) {

        var model = {
            idboxrender: "boxModal",
            url: "/Admin/Home/Modal_AddLed",
            idmodal: "ModalAddLed",
            laneid: laneid
        };

        JSHelper.Modal_Open(model);
    },
    SaveLed: function () {
        var model = {
            ledid: $("#ddlLed").val(),
            laneid: $("#hidLaneId").val()
        };

        JSHelper.AJAX_HttpPost('/Admin/Home/SaveLed', model)
            .success(function (result) {
                if (result.isSuccess) {
                    $("#ModalAddLed").modal("hide");

                    TreeController.PartialTree($("#hidPCId").val());

                    toastr.success("Thành công");
                } else {
                    toastr.error(result.message);
                }
            });
    },
    DelLed: function (id,laneid) {
        var model = {
            ledid: id
        };

        JSHelper.AJAX_HttpPost('/Admin/Home/DeleteLed', model)
            .success(function (result) {
                if (result.isSuccess) {
                    $("#liLed_" + id).remove();

                    var ul = $("#ulLed_" + laneid);

                    if (ul.has("li").length == 0) {
                        ul.remove();
                    }
                } else {
                    toastr.error(result.message);
                }
            });
    },
    ModalConfig: function (laneid,conid) {

        var model = {
            idboxrender: "boxModal",
            url: "/Admin/Home/Modal_Config",
            idmodal: "ModalConfig",
            laneid: laneid,
            conid: conid
        };

        JSHelper.Modal_Open(model);
    },
    SaveConfig: function () {
        var model = {
            controller_ID: $("#hidConId").val(),
            lane_ID: $("#hidLaneId").val(),
            reader_Index: $('#hidReader').val(),
            input_Index: $('#hidInput').val(),
            barrie_Index: $('#hidBarrier').val()
        };

        JSHelper.AJAX_HttpPost('/Admin/Home/SaveConfig', model)
            .success(function (result) {
                if (result.isSuccess) {
                    $("#ModalConfig").modal("hide");

                    toastr.success("Thành công");
                } else {
                    toastr.error(result.message);
                }
            });
    },
}
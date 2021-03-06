var index = {
    frm: {
        frmFilter : $('#frmFilter'),
        frmCreate : $('#frmCreate'),
        frmUpdate :  $('#frmUpdate'),
        dtTable : $('#dtTable')
    },
    domain: "/tac-gia",
    search: function () {
        var name = index.frm.frmFilter.find("#txtName").val();
        var status = index.frm.frmFilter.find("#drStatus").val();
        newsourceId = index.frm.frmFilter.find('#drNewsSources').val();
        index.getData(name, newsourceId, status);
    },
    init: function () {
        index.getData();
    },
    getData: function (name = "",newsourceId = 0, status = -1) {
        showLoading()
        $.ajax({
            url: index.domain + "/get-list?name=" + name + "&newsourceId=" + newsourceId + "&status=" + status,
            method: "GET",
             success: function (response) {
                $('#dtTable').html(response);
                hideLoading()
            }
        })
    },
    openCreate: function () {
        $.ajax({
            url: index.domain + "/create",
            method: "Get",
            success: function (response) {
                showContentModal(response, "Tạo  mới tác giả")
            }
        });
    },
    openUpdate: function (id) {
        $.ajax({
            url: index.domain + "/update?id=" + id,
            method: "Get",
            success: function (response) {
                showContentModal(response, "Cập nhật tác giả")
            }
        });
    },
    deleteItem: function (id) {
        $.ajax({
            url: index.domain + '/delete-or-restore',
            method: "POST",
            data: {
                id: id,
                status: 2
            }
            , success: function (response) {
                if (response.result) {
                    $("#row_" + id).slideUp();
                    showAlert("Xóa dữ liệu thành công", 2)

                } else {
                    showAlert(response.message)

                }
            }
        })
    },
    restore: function (id) {
        $.ajax({
            url: index.domain + '/update-status',
            method: "POST",
            data: {
                id: id,
                status: 0,
            }
            , success: function (response) {
                if (response.result) {
                    $("#row_" + id).slideUp();
                    showAlert("Khôi phục dữ liệu thành công", 2)
                } else {
                    showAlert(response.message)

                }
            }
        })
    },
    updateStatus: function (id,status) {
        $.ajax({
            url: index.domain + '/update-status',
            method: "POST",
            data: {
                Id: id,
                Status: status
            }
            , success: function (response) {

            }
        })
    }
}
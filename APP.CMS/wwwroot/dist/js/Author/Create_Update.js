var create = {
    create: function () {
        if (ValidateForm(index.frm.frmCreate)) {
            return;
        }
        var model = {
            Name: index.frm.frmCreate.find('#txtName').val(),
            NewsSourcesId: index.frm.frmCreate.find('#drNewsSources').val(),
            Note: index.frm.frmCreate.find('#txtNote').val(),
            Status: index.frm.frmCreate.find("#ckStatus").prop('checked') ? 1 : 0,
        }
        create.create_or_update(model);
    },
    update: function () {
        if (ValidateForm(index.frm.frmUpdate)) {
            return;
        }
        var model = {
            Id: index.frm.frmUpdate.find('#txtId').val(),
            Name: index.frm.frmUpdate.find('#txtName').val(),
            NewsSourcesId: index.frm.frmUpdate.find('#drNewsSources').val(),
            Note: index.frm.frmUpdate.find('#txtNote').val(),
            Status: index.frm.frmUpdate.find("#ckStatus").prop('checked') ? 1 : 0,
        }
        create.create_or_update(model);
    },
    create_or_update: function (model) {
        showLoading();
        $.ajax({
            url: index.domain + "/create-or-update",
            method: "POST",
            data: model
            , success: function (response) {
                hideLoading()
                if (response.result) {
                    showAlert(response.message, 2)
                    index.getData();
                    hideContentModal()
                } else {
                    showAlert(response.message)
                }
            }
        })
    }
}
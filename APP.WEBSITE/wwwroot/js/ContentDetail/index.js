var index = {
    status: true,
    background : $("#contentDetail").css("background-color"),
    color : $("#contentDetail").css("color"),
    domain: '/ContentDetail',
    init: function (otpions) {
        index.model.title = otpions.title;
        index.model.link = otpions.link;
        index.PostSendEmail();
        index.print();
    },
     model : {
         title : "",
         link :""
    },
    SetColor: function () {
         
        
        if (index.status) {
            $("#contentDetail").css({ 'background-color': '#000000', 'color': '#ffffff' })
            index.status = false;
        }
        else {
           
            $("#contentDetail").css({ 'background-color': index.background, 'color': + "#343a40" });
            index.status = true;
        }
        
    }
     ,
    sendEmai: function () {
        $("#btn-send-email").click(function () {
            $.ajax({
                url: index.domain + "/SendMail?title=" + index.model.title + "&link=" + index.model.link,
                method: "Get",
                success: function (response) {
                    showContentModalMail(response, "Chia sẻ bài viết qua email")
                }
            })
        })
        
    },
    print: function () {
        $("#btn-print").click(function () {
            var divToPrint = document.getElementById("contentDetail");
            newWin = window.open("");
            newWin.document.write(divToPrint.outerHTML);
            newWin.print();
            newWin.close();
        })
    } ,
    PostSendEmail: function () {
        
        $('#modal-form').find('#btnSave').off("click").on('click', function (e) {
            showLoading();
            e.preventDefault();
            if (ValidateForm($("#send-mail"))) {
                addRequired($("#send-mail"))
                return;
            }
            if (!checkCaptcha) {
                showAlert("Vui lòng ấn xác nhận")
                return;
            }
            var body =  $("#message").val()
            $.ajax({
                url: index.domain + "/SendMail",
                method: "POST",
                data: {
                    title: index.model.title,
                    Form: $("#sender-email").val(),
                    namesender: $("#sender-name").val(),
                    to: $("#recipient-email").val(),
                    body: body,
                    link: index.model.link
                }
                , success: function (response) {
                    hideLoading();
                    hideContentModalMail();
                    if (response.result) {
                        // datasource = response.data
                        showAlert(response.message, 2)
                    } else {
                        showAlert(response.message)
                    }
                }
            })
        })
    }
}





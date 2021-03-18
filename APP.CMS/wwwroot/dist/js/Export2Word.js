// Index
function exportWordIndex() {
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    var title = '<div>'
    title += '<h1 style="text-align:center;color:#000;font-weight:bold"> THỐNG KÊ NHUẬN BÚT THÁNG ' + mm + "/" + yyyy + '</h1>';
    var body = $("#tblDisplay").html() + '</div>';
    //var footer = '<div style="text-align:right;margin-right:50px"><p><i>Hà Nội, Ngày ' + dd + ' tháng ' + mm + ' năm ' + yyyy + '</i></p>';
    //footer += '<p style="margin-right:90px;" ><b>Người lập biểu</b><br>' + '<i>(Ký ghi rõ họ tên)</i><br><br></p></div>';

    var sourceHTML =/* header +*/ title + body /*+ footer*/;
    var fileName = 'TKNB_TongHop_' + dd + mm + yyyy + '.doc';
    console.log(sourceHTML);
    var input = {
        Html: sourceHTML,
        FileName: fileName
    }
    $.ajax({
        url: "/thong-ke-nhuan-but/export-word",
        method: "Post",
        data: input,
        xhrFields: {
            responseType: 'blob' // to avoid binary data being mangled on charset conversion
        },
        success: function (blob, status, xhr) {
            // check for a filename
            var filename = "";
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                var matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
            }

            if (typeof window.navigator.msSaveBlob !== 'undefined') {
                // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
                window.navigator.msSaveBlob(blob, filename);
            } else {
                var URL = window.URL || window.webkitURL;
                var downloadUrl = URL.createObjectURL(blob);

                if (filename) {
                    // use HTML5 a[download] attribute to specify filename
                    var a = document.createElement("a");
                    // safari doesn't support this yet
                    if (typeof a.download === 'undefined') {
                        window.location.href = downloadUrl;
                    } else {
                        a.href = downloadUrl;
                        a.download = filename;
                        document.body.appendChild(a);
                        a.click();
                    }
                } else {
                    window.location.href = downloadUrl;
                }

                setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup
            }
        }
    });

}

//Detail
function exportWordDetail() {
    var today = new Date();
    var dd = String(today.getDate()).padStart(2, '0');
    var mm = String(today.getMonth() + 1).padStart(2, '0'); //January is 0!
    var yyyy = today.getFullYear();
    var title = '<h1 style="text-align:center;color:#000;font-weight:bold"> BẢNG IN CHI TIẾT NHUẬN BÚT </h1>';
    var table = $("#dtTable").html();
    //var footer = '<br><div style="text-align:right;margin-right:50px"><p><i>Hà Nội, Ngày ' + dd + ' tháng ' + mm + ' năm ' + yyyy + '</i></p>';
    //footer += '<p style="margin-right:90px;" ><b>Người lập biểu</b><br>' + '<i>(Ký ghi rõ họ tên)</i><br><br></p></div>';



    var sourceHTML = title /*+ body */+ table /*+ footer*/;
    var fileName = 'TKNB_' + normalVNese($("#txtChitiet").val()) + '_' + dd + mm + yyyy + '.doc';
    console.log(sourceHTML);
    var input = {
        Html: sourceHTML,
        FileName: fileName
    }
    $.ajax({
        url: "/thong-ke-nhuan-but/export-word",
        method: "POST",
        data: input
        ,
        xhrFields: {
            responseType: 'blob' // to avoid binary data being mangled on charset conversion
        },
        success: function (blob, status, xhr) {
            // check for a filename
            var filename = "";
            var disposition = xhr.getResponseHeader('Content-Disposition');
            if (disposition && disposition.indexOf('attachment') !== -1) {
                var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                var matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
            }

            if (typeof window.navigator.msSaveBlob !== 'undefined') {
                // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
                window.navigator.msSaveBlob(blob, filename);
            } else {
                var URL = window.URL || window.webkitURL;
                var downloadUrl = URL.createObjectURL(blob);

                if (filename) {
                    // use HTML5 a[download] attribute to specify filename
                    var a = document.createElement("a");
                    // safari doesn't support this yet
                    if (typeof a.download === 'undefined') {
                        window.location.href = downloadUrl;
                    } else {
                        a.href = downloadUrl;
                        a.download = filename;
                        document.body.appendChild(a);
                        a.click();
                    }
                } else {
                    window.location.href = downloadUrl;
                }

                setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup
            }
        }
    });

}
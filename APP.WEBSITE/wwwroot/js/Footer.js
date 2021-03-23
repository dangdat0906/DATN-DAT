$(document).ready(function () {
    //loadFooterTop();
    loadFooterBot();
})
function loadFooterTop() {
    $.ajax({
        url: "/trang-chu" + "/get-footerTop",
        method: "GET",
        success: function (response) {
            $('#footerTop').html(response);
        }
    })
}
function loadFooterBot() {
    $.ajax({
        url: "/trang-chu" + "/get-footerBot",
        method: "GET",
        success: function (response) {
            $('#footerBot').html(response);
        }
    })
}

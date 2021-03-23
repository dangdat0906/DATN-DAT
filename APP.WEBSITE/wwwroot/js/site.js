// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    loadLanguages();
    loadData(); 
    loadFooter();
})
function loadData() {
    $.ajax({
        url: "/trang-chu" + "/get-list-categories",
        method: "GET",
        success: function (response) {
            $('#topMenu').html(response);
        }
    })
    $.ajax({
        url: "/trang-chu" + "/get-list-right-banners",
        method: "GET",
        success: function (response) {
            $('#rightSideBar').html(response);
        }
    })
}
function loadFooter() {
    $.ajax({
        url: "/trang-chu" + "/get-footer",
        method: "GET",
        success: function (response) {
            $('#footerPage').html(response);
        }
    })
}
function loadLanguages() {
    $.ajax({
        url: "/trang-chu" + "/get-languages",
        method: "Get",
        success: function (response) {
            $('.main-header-top').html(response);
        }
    })
}


function ValidateForm(frm) {
    var error = false;
    frm.find('input').each(function () {
        if ($(this).attr('required')) {
            if ($(this).hasClass('error')) {
                $(this).removeClass('error');
            }
            if ($(this).val() === '') {
                $(this).addClass('error');
                error = true;

            }
        }
    });
    frm.find('select').each(function () {
        if ($(this).attr('required')) {
            if ($(this).hasClass('error')) {
                $(this).removeClass('error');
            }
            if ($(this).val() === null) {
                $(this).addClass('error');
                error = true;
            }
            if ($(this).val().length == 0) {
                $(this).addClass('error');
                error = true;
            }
        }
    });
    frm.find('.select2').each(function () {
        if ($(this).attr('required')) {
            if ($(this).hasClass('error')) {
                $(this).removeClass('error');
            }
            if ($(this).val() === null) {
                $(this).addClass('error');
                error = true;

            }
        }
    });
    if (error) {
        toastr.error('Có mục nhập nào đó đánh dấu * mà bạn chưa nhập đầy đủ dữ liệu. Yêu cầu kiểm tra lại')
    }
    return error;
}
function addRequired(frm) {
    frm.find('label').each(function () {
        if ($(this).attr('required')) {
            $(this).append('<i class="fa fa-asterisk" aria-hidden="true"></i>')
        }
    });
}

function showAlert(message, error = 1) {
    if (error === 1) {
        toastr.error(message)
    }
    else if (error === 2) {
        toastr.success(message)
    }
    //removeAlert();
}

function showLoading() {
    $('body').loadingModal({
        position: 'auto',
        text: '',
        color: '#fff',
        opacity: '0.7',
        backgroundColor: 'rgb(0,0,0)',
        animation: 'doubleBounce'
    });
}
function hideLoading() {
    $('body').loadingModal('destroy');

}
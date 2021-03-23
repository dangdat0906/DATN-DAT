
$(document).ready(function () {
    //loadFirstCate();
    loadLeftBanner();
    //loadSecondCate();
    //loadThirdCate();
    //loadFourthCate();
    

});
//function loadFirstCate() {
//    $.ajax({
//        url: "/trang-chu" + "/get-first-category",
//        method: "GET",
//        success: function (response) {
//            if (response.result != false) {
//                $('#firstCategory').html(response);
//            }
            
//        }
//    }).then(function () {
//        loadFirstCategoryContent()
//    });
//}
//function loadSecondCate() {
//    $.ajax({
//        url: "/trang-chu" + "/get-second-category",
//        method: "GET",
//        success: function (response) {
//            if (response.result != false) {
//                $('#secondCategory').html(response);
//            }
//        }
//    }).then(function () {
//        loadSecondCategoryContent()
//    });
//}
//function loadThirdCate() {
//    $.ajax({
//        url: "/trang-chu" + "/get-third-category",
//        method: "GET",
//        success: function (response) {
//            if (response.result != false) {
//                $('#thirdCategory').html(response);
//            }
//        }
//    }).then(function () {
//        loadThirdCategoryContent()
//    });
//}
//function loadFourthCate() {
//    $.ajax({
//        url: "/trang-chu" + "/get-forth-category",
//        method: "GET",
//        success: function (response) {
//            if (response.result != false) {
//                $('#fourthCategory').html(response);
//            }
//        }
//    }).then(function () {
//        loadFourthCategoryContent()
//    })
//}
function loadLeftBanner() {
    var position1 = 1;
    var position2 = 2;
    $.ajax({
        url: "/trang-chu" + "/get-banner-left?position=" + position1,
        method: "GET",
        success: function (response) {
            $('#bannerleft1').html(response);
        }
    })
    $.ajax({
        url: "/trang-chu" + "/get-banner-left?position="+ position2,
        method: "GET",
        success: function (response) {
            $('#bannerleft2').html(response);
        }
    })
}
//function loadFirstCategoryContent() {
//    var category = 'firstCategory';
//    loadRowContent(category);
//    loadColumnContent(category);
  
//}
//function loadSecondCategoryContent() {
//    var category = 'secondCategory';
//    loadRowContent(category);
//    loadColumnContent(category);
//}
//function loadThirdCategoryContent() {
//    var category = 'thirdCategory';
//    loadRowContent(category);
//    loadColumnContent(category);
//}
//function loadFourthCategoryContent() {
//    var category = 'fourthCategory';
//    loadRowContent(category);
//    loadColumnContent(category);
//}
//function loadColumnContent(category) {
//    $('#' + category).find('.sub-cate').find('li').each(function (index) {
//        var listContentColumn = "listContentColumn" + (index + 1);
//        getContentColumn($(this).data('id'), listContentColumn, category);
//    })
//}
//function loadRowContent(category) {
//    getRowContent($('#' + category).find('#rowCateId').data('id'), category);
//}
//function getContentColumn(cateId, listContentColumn, category) {
//    $.ajax({
//        url: "/trang-chu" + "/get-content-category-column?cateId="+cateId,
//        method: "GET",
//        success: function (response) {
//            if (response.result != false) {
//                $('#' + category).find('#' + listContentColumn).html(response);
//            }
            
//        }
//    })
//}
//function getRowContent(cateId, category) {
//    $.ajax({
//        url: "/trang-chu" + "/get-content-category-row?cateId=" + cateId,
//        method: "GET",
//        success: function (response) {
//            if (response.result != false) {
//                $('#' + category).find('#listContentRow').html(response);
//            }
//        }
//    })
//}
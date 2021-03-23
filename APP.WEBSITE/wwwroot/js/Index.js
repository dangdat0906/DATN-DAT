$(document).ready(function () {
    $.ajax({
        url: "/trang-chu" + "/get-list-content",
        method: "GET",
        success: function (response) {
            $('#mainContent').html(response);
        }
    }).then(function () {
        $("#sliderFeatured").slick({
            autoplay: true, // auto play slide
            autoplaySpeed: 5000,
            arrows: false,
            dots: true,
            infinite: true,
            speed: 400
        });

        // scrollbar in sub-featured
        $(".scroll-wrapper-featured").scrollbar();
    })

})




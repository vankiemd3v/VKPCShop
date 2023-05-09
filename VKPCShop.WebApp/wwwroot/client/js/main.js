$('#btn-payment').click(function () {
    setTimeout("$('body').toggleClass('loading')", 1);
});

$('.rejectOrder').click(function (e) {
    e.preventDefault()
    const id = $('#orderId').val();
    var status = "Hủy";
    $.ajax({
        type: "POST",
        url: '/Admin/Order/Update',
        data: {
            id: id,
            status: status
        },
        success: function (res) {
            $.notify("Đơn hàng đã đc hủy bỏ", "success");
            setTimeout(function () {
                location.reload();
            }, 2000) 
        },
        error: function (err) {
            console.log(err);
        }
    });
});

$('.grid-view ul li a').click(function(){
  var idOld = $(".active2").attr("href");
    // active trên menu
    $('.grid-view li a').removeClass("active2");
    $(this).addClass("active2");
    $('.grid-view li a').removeClass("active2");
    $(this).addClass("active2");

    // active list sản phẩm
    var id = $(this).attr("href");
    $(''+ idOld+'').removeClass("active1");
    $(''+ id +'').addClass("active1");
    $(''+ idOld+'').removeClass("active1");
    $(''+ id +'').addClass("active1");
});
$('.mobile-menu ul').click(function(){
    $('.mobile-menu ul').removeClass("activeMenu");
    $(this).addClass("activeMenu");
    $('.mobile-menu ul').removeClass("activeMenu");
    $(this).addClass("activeMenu");
});
$('.btn-add-cart-flex').click(function(){
    const id = $(this).attr('id');
    $.ajax({
        type: "GET",
        url: '/Cart/AddToCart',
        data: {
            id: id
        },
        success: function (res) {
            if (res.status) {
                $.notify("Đã thêm vào giỏ hàng", {
                    position: "top left",
                    className: "success"});
                $('.cartNumber').text(res.cartNumber);
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});
$('.detail-add-cart').click(function () {
    const id = $(this).attr('id');
    $.ajax({
        type: "GET",
        url: '/Cart/AddToCart',
        data: {
            id: id
        },
        success: function (res) {
            if (res.status) {
                $.notify("Đã thêm vào giỏ hàng", {
                    position: "top left",
                    className: "success"
                });
                $('.cartNumber').text(res.cartNumber);
            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});


$('.btn-outStock').click(function () {
    $.notify("Hết hàng", {
        position: "top left",
        className: "error"
    });
});
$('.detail-outStock').click(function () {
    $.notify("Hết hàng", {
        position: "top left",
        className: "error"
    });
});


$('.btn-apply').click(function(){
    $.notify("Đã áp dụng bộ lọc", {
        position: "top left",
        className: "success"
    });
});
$('.btn-delete-filter').click(function(){
    $.notify("Đã xóa bộ lọc", {
        position: "top left",
        className: "error"
    });
});

(function($) {
  $.fn.simpleMenu = function( options ) {
    "use strict";
  var $a = $(this),
    $b = $(this).find('a').next();
  $a.on('click', 'a' , function(e){
      e.stopPropagation();
    //if li has ul then dont redirect to link address
    var $c = $(this).next().hasClass('sub-menu');
    //console.log($c === true);
    if ($c === true) {
      e.preventDefault();
    }
    //if li has ul then dont redirect to link address ends
    //debugger;
    
    $(this).next().slideToggle(300)	
    .parent().siblings().children('ul')
    .not($(this).next()).hide()
  }),
  $(document).on('click', $b, function(e){
    $b.hide(200);
  });	
  }
}(jQuery));
$('.simple-menu').simpleMenu();




// selected button filter
$('.btn-selected').on('click', function(){
  var result = $(this).hasClass('selected');
  if(result){
    $(this).removeClass('selected');
  }else{
    $(this).addClass('selected');
  }
});

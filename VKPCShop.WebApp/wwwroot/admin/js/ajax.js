$.ajax({
    type: "GET",
    url: '/Admin/Order/CountOrderConfirm',
    success: function (res) {
        $('.inbox-num').text(res.count);
    },
    error: function (err) {
        console.log(err);
    }
});
$.ajax({
    type: "GET",
    url: '/Admin/Home/User',
    success: function (res) {
        $('.nameUser').text(res.name);
        $(".imgUser").attr("src", res.image);
    },
    error: function (err) {
        console.log(err);
    }
});


$('#updateStatus').click(function (e) {
    e.preventDefault()
    const id = $('#orderId').val();
    var status = $('#SelectLm').prop('selected', true).val();
    $.ajax({
        type: "POST",
        url: '/Admin/Order/Update',
        data: {
            id: id,
            status: status
        },
        success: function (res) {
            if (res.result) {
                $.notify(" Cập nhật thành công", "success");
            }
            else {
                $.notify(" Cập nhật thất bại, có lỗi xảy ra", "error");

            }
        },
        error: function (err) {
            console.log(err);
        }
    });
});


// get thương hiệu
$('.icon-edit').click(function (e) {
    e.preventDefault()
    const id = $(this).attr('id');
    //const name = $('#inp-name').val();
    $.ajax({
        type: "GET",
        url: '/Admin/Brand/Update',
        data: {
            id
        },
        success: function (res) {
            if (res.status) {
                $('#id-brand').val(id);
                $('#inp-name-update').val(res.name);
            }

        },
        error: function (err) {
            console.log(err);
        }
    });
});
// update thương hiệu
$('#btn-update-brand').click(function (e) {
    $('.valid-name').text("");
    const id = $('#id-brand').val();
    const name = $('#inp-name-update').val();
    if (name == "") {
        $('.valid-name').text("Tên không được để trống");
    }
    else {
        $.ajax({
            type: "POST",
            url: '/Admin/Brand/Update',
            data: {
                id: id,
                name: name,
            },
            success: function (res) {
                if (res.status) {
                    $.notify(" Cập nhật thành công", "success");
                    setTimeout(function () {
                        location.reload(true);
                    }, 500);
                }

            },
            error: function (err) {
                console.log(err);
            }
        });
    }
});
// delete thương hiệu
$('.btn-delete').click(function (e) {
    const id = $(this).attr('id');
        $.ajax({
            type: "GET",
            url: '/Admin/Brand/Delete',
            data: {
                id: id,
            },
            success: function (res) {
                if (res.status) {
                    $.notify(" Xóa thành công", "success");
                    setTimeout(function () {
                        location.reload(true);
                    }, 500);
                }

            },
            error: function (err) {
                console.log(err);
            }
        });
});


// get danh mục
$('.icon-edit-category').click(function (e) {
    e.preventDefault()
    const id = $(this).attr('id');
    $.ajax({
        type: "GET",
        url: '/Admin/Category/Update',
        data: {
            id
        },
        success: function (res) {
            if (res.status) {
                $('#id-category').val(id);
                $('#category-name-update').val(res.name);
                $("#img-update-category").attr("src", "/admin/images/" + res.image);
            }

        },
        error: function (err) {
            console.log(err);
        }
    });
});


// delete danh mục
$('.btn-delete-category').click(function (e) {
    const id = $(this).attr('id');
    $.ajax({
        type: "GET",
        url: '/Admin/Category/Delete',
        data: {
            id: id,
        },
        success: function (res) {
            if (res.status) {
                $.notify(" Xóa thành công", "success");
                setTimeout(function () {
                    location.reload(true);
                }, 500);
            }

        },
        error: function (err) {
            console.log(err);
        }
    });
});

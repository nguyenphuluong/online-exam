
$(document).ready(function () {
    //#region SỰ KIỆN MỞ MODAL THÊM, CẬP NHẬT CHỦ ĐỀ
    $(document).on("click", "#btn-modal", function () {
        const action = $(this).attr("data-action");
        const categoryId = $(this).attr("data-target-id");
        //Nạp html
        loadHtmlByAjax("/admin/categoryadmin/modalcategory", "#modal-category-content", {
            action: action,
            categoryId: categoryId
        });
        //Bật modal
        $("#modal-category-btn").trigger("click");
    });
    //#endregion

    //#region SỰ KIỆN TẠO CHỦ ĐỀ
    $(document).on("click", "#btn-create", function () {
        const category = $("#form-category").serializeObject();
        if (category.CategoryName == "") {
            toastr.error("Chưa nhập đủ thông tin", "Thông báo");
        }
        else {
            category.CategoryName = category.CategoryName.trim();
            $.ajax({
                url: "/admin/categoryadmin/eventcreate",
                type: "POST",
                data: {
                    category: category,
                },
                dataType: "json",
                beforeSend: function () {
                    showLoading();
                },
                success: function (response) {
                    if (response.responseCode == "200") {
                        toastr.success(response.responseMess, "Thông báo");
                        setTimeout(function () {
                            location.reload();
                        }, 800);
                    }
                    else {
                        toastr.error(response.responseMess, "Thông báo");
                    }
                },
                error: function () {
                    toastr.error("Máy chủ tạm thời không phản hồi, vui lòng thử lại sau", "Thông báo");

                },
            }).always(function () {
                hideLoading();
            })
        }
    });
    //#endregion

    //#region SỰ KIỆN CẬP NHẬT CHỦ ĐỀ
    $(document).on("click", "#btn-update", function () {
        const category = $("#form-category").serializeObject();
        if (category.CategoryName == "") {
            toastr.error("Chưa nhập đủ thông tin", "Thông báo");
        }
        else {
            category.CategoryName = category.CategoryName.trim();
            $.ajax({
                url: "/admin/categoryadmin/eventupdate",
                type: "POST",
                data: {
                    category: category,
                },
                dataType: "json",
                beforeSend: function () {
                    showLoading();
                },
                success: function (response) {
                    if (response.responseCode == "200") {
                        toastr.success(response.responseMess, "Thông báo");
                        setTimeout(function () {
                            location.reload();
                        }, 800);
                    }
                    else {
                        toastr.error(response.responseMess, "Thông báo");
                    }
                },
                error: function () {
                    toastr.error("Máy chủ tạm thời không phản hồi, vui lòng thử lại sau", "Thông báo");

                },
            }).always(function () {
                hideLoading();
            })
        }
    });
    //#endregion

    //#region SỰ KIỆN XOÁ CHỦ ĐỀ
    $(document).on("click", "#btn-delete", function () {
        const category = $("#form-category").serializeObject();
        $.ajax({
            url: "/admin/categoryadmin/eventdelete",
            type: "POST",
            data: {
                category: category,
            },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            success: function (response) {
                if (response.responseCode == "200") {
                    toastr.success(response.responseMess, "Thông báo");
                    setTimeout(function () {
                        location.reload();
                    }, 800);
                }
                else {
                    toastr.error(response.responseMess, "Thông báo");
                }
            },
            error: function () {
                toastr.error("Máy chủ tạm thời không phản hồi, vui lòng thử lại sau", "Thông báo");

            },
        }).always(function () {
            hideLoading();
        })

    });
    //#endregion

    //#region SỰ KIỆN TÌM KIẾM CHỦ ĐỀ
    $(document).on("submit", "#form-filter", function () {
        const name = $("input", this).val();
        if (name == "") {
            window.location.href = `/admin/chu-de`;
        }
        else {
            window.location.href = `/admin/chu-de?name=${name}`;
        }

        return false;
    });
    //#endregion
});



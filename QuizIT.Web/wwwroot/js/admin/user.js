
$(document).ready(function () {
    //#region SỰ KIỆN MỞ MODAL THÊM, CẬP NHẬT NGƯỜI DÙNG
    $(document).on("click", "#btn-modal", function () {
        const action = $(this).attr("data-action");
        const userId = $(this).attr("data-target-id");
        //Nạp html
        loadHtmlByAjax("/admin/useradmin/modaluser", "#modal-user-content", {
            action: action,
            userId: userId
        });
        //Bật modal
        $("#modal-user-btn").trigger("click");
    });
    //#endregion

    //#region SỰ KIỆN TẠO NGƯỜI DÙNG
    $(document).on("click", "#btn-create", function () {
        const user = $("#form-user").serializeObject();
        if (user.FullName == "" || user.UserName == "") {
            toastr.error("Chưa nhập đủ thông tin", "Thông báo");
        }
        else {
            user.FullName = user.FullName.trim();
            user.UserName = user.UserName.trim();
            user.Password = user.Password.trim();
            $.ajax({
                url: "/admin/useradmin/eventcreate",
                type: "POST",
                data: {
                    user: user,
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

    //#region SỰ KIỆN CẬP NHẬT NGƯỜI DÙNG
    $(document).on("click", "#btn-update", function () {
        const user = $("#form-user").serializeObject();
        $.ajax({
            url: "/admin/useradmin/eventupdate",
            type: "POST",
            data: {
                user: user,
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

    //#region SỰ KIỆN TÌM KIẾM NGƯỜI DÙNG
    $(document).on("submit", "#form-filter", function () {
        const name = $("input", this).val();
        const role = $("select", this).val();
        if (role == -1) {
            if (name == "") {
                window.location.href = `/admin/nguoi-dung`;
            }
            else {
                window.location.href = `/admin/nguoi-dung?name=${name}`;
            }
        }
        else {
            if (name == "") {
                window.location.href = `/admin/nguoi-dung?role=${role}`;
            }
            else {
                window.location.href = `/admin/nguoi-dung?role=${role}&name=${name}`;
            }
        }
        return false;
    });
    //#endregion
});



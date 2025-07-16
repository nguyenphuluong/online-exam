$(document).ready(function () {
    //#region SỰ KIỆN ĐĂNG KÝ
    $(document).on("submit", "#registry-form", function () {
        const user = $(this).serializeObject();
        const isValid = isValidateRegistry(user);
        if (isValid) {
            $.ajax({
                url: "/authenticate/eventregistry",
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
                            window.location.href = "/";
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
        return false;
    });
    //#endregion

    //#region SỰ KIỆN ĐĂNG NHẬP
    $(document).on("submit", "#login-form", function () {
        const user = $(this).serializeObject();
        const isValid = isValidateLogin(user);
        if (isValid) {
            $.ajax({
                url: "/authenticate/eventlogin",
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
                            window.location.href = "/bo-de";
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
        return false;
    });
    //#endregion
});


isValidateLogin = function (user) {
    if (user.UserName === "" || user.Password === "") {
        toastr.error("Chưa nhập đủ thông tin", "Thông báo");
        return false;
    }
    return true;
}

isValidateRegistry = function (user) {
    if (user.FullName === "" || user.UserName === "" || user.Password === "") {
        toastr.error("Chưa nhập đủ thông tin", "Thông báo");
        return false;
    }
    return true;
}
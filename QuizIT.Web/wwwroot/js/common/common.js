
$.fn.serializeObject = function () {
    var o = Object.create(null),
        elementMapper = function (element) {
            element.name = $.camelCase(element.name);
            return element;
        },
        appendToResult = function (i, element) {
            var node = o[element.name];

            if ('undefined' != typeof node && node !== null) {
                o[element.name] = node.push ? node.push(element.value) : [node, element.value];
            } else {
                o[element.name] = element.value;
            }
        };

    $.each($.map(this.serializeArray(), elementMapper), appendToResult);
    return o;
};

//#region HÀM LOAD SHOW LOADING
showLoading = function () {
    $(".my-loading").addClass("d-flex");
}
//#endregion

//#region HÀM LOAD HIDE LOADING
hideLoading = function () {
    $(".my-loading").removeClass("d-flex");
}
//#endregion

//#region HÀM LOAD HTML BẰNG AJAX
loadHtmlByAjax = function (url, selectDivContainer, data = {}, isShowLoading = false, isAppend = false, errorMess = "Máy chủ tạm thời không phản hồi, vui lòng thử lại sau") {
    $.ajax({
        url: url,
        type: "POST",
        data: data,
        dataType: "html",
        beforeSend: function () {
            if (isShowLoading == true) {
                showLoading();
            }
        },
        success: function (html) {
            if (isAppend == false) {
                $(selectDivContainer).html(html);
            }
            else {
                $(selectDivContainer).append(html);
            }
        },
        error: function () {
            toastr.error(errorMess, "Thông báo")
        }
    }).always(function () {
        if (isShowLoading == true) {
            hideLoading();
        }
    })
}
//#endregion

//#region SỰ KIỆN ĐÓNG MỞ SIDEBAR
$(document).on("click", "#btn-sidebar", function () {
    $(".sidebar").toggleClass("active");
});
//#endregion

//#region SỰ KIỆN CHẶN NHẬP CÁC KÍ TỰ ĐẶC BIỆT VÀO INPUT SEARCH
$(document).on("keypress", "#form-filter input", function (event) {
    const keyCodeBanList = [33, 64, 35, 36, 37, 94, 38, 42, 40, 41, 95, 43, 60, 62, 63, 91, 93, 123, 125, 59, 58, 39, 34]
    //Nếu kí tự nhập nằm trong tập bị cấm
    if (keyCodeBanList.includes(event.keyCode)) {
        event.preventDefault();
    }
})
//#endregion

//#region SỰ KIỆN ĐĂNG XUẤT
$(document).on("click", "#btn-logout", function (event) {
    $.ajax({
        url: "/authenticate/eventlogout",
        type: "POST",
        beforeSend: function () {
            showLoading();
        },
        success: function () {
            toastr.success("Hẹn gặp lại", "Thông báo");
            setTimeout(function () {
                window.location.href = "/";
            }, 800);
        },
        error: function () {
            toastr.error("Máy chủ tạm thời không phản hồi, vui lòng thử lại sau", "Thông báo");

        },
    }).always(function () {
        hideLoading();
    })
});
//#endregion
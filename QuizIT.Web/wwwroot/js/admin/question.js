
$(document).ready(function () {

    //#region SỰ KIỆN CLICK INPUT EXCEL
    $(document).on("click", "#input-excel", function () {
        $(this).val(null);
    });
    //#endregion

    //#region SỰ KIỆN IMPORT FILE EXCEL
    $(document).on("click", "#btn-import", function () {
        const formData = new FormData();
        const fileExcel = $("#input-excel").get(0).files[0];
        if (fileExcel == undefined) {
            toastr.error("Vui lòng chọn file", "Thông báo");
        }
        else {
            const fileType = fileExcel.name.split('.')[1];
            const categoryId = $("#select-category").val();
            if (fileType != 'xlsx' && fileType != 'XLSX') {
                toastr.error("Vui lòng chọn file excel đuôi .xlsx", "Thông báo");
            }
            else {
                formData.append("categoryId", categoryId);
                formData.append("fileExcel", fileExcel);
                $.ajax({
                    url: "/admin/questionadmin/eventimportexcel",
                    type: "POST",
                    data: formData,
                    contentType: false,
                    processData: false,
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
                });
            }
        }

    });
    //#endregion

    //#region SỰ KIỆN TÌM KIẾM CÂU HỎI
    $(document).on("submit", "#form-filter", function () {
        const name = $("input", this).val();
        const categoryId = $("select", this).val();
        if (categoryId == -1) {
            if (name == "") {
                window.location.href = `/admin/cau-hoi`;
            }
            else {
                window.location.href = `/admin/cau-hoi?name=${name}`;
            }
        }
        else {
            if (name == "") {
                window.location.href = `/admin/cau-hoi?category=${categoryId}`;
            }
            else {
                window.location.href = `/admin/cau-hoi?category=${categoryId}&name=${name}`;
            }
        }

        return false;
    });
    //#endregion

    //#region SỰ KIỆN THÊM CÂU HỎI
    $(document).on("click", "#btn-create", function () {
        const question = $("#form-question").serializeObject();
        //Loại bỏ dấu cách thừa
        question.Content = question.Content.trim()
        question.AnswerA = question.AnswerA.trim()
        question.AnswerB = question.AnswerB.trim()
        question.AnswerC = question.AnswerC.trim()
        question.AnswerD = question.AnswerD.trim()
        if (!isValidQuestion(question)) {
            toastr.error("Chưa nhập đủ thông tin", "Thông báo");
        }
        else {

            $.ajax({
                url: "/admin/questionadmin/eventcreate",
                type: "POST",
                data: {
                    question: question,
                },
                dataType: "json",
                beforeSend: function () {
                    showLoading();
                },
                success: function (response) {
                    if (response.responseCode == "200") {
                        toastr.success(response.responseMess, "Thông báo");
                        setTimeout(function () {
                            window.location.href = "/admin/cau-hoi";
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
            });
        }
    });
    //#endregion

    //#region SỰ KIỆN SỬA CÂU HỎI
    $(document).on("click", "#btn-update", function () {
        const question = $("#form-question").serializeObject();
        //Loại bỏ dấu cách thừa
        question.Content = question.Content.trim()
        question.AnswerA = question.AnswerA.trim()
        question.AnswerB = question.AnswerB.trim()
        question.AnswerC = question.AnswerC.trim()
        question.AnswerD = question.AnswerD.trim()
        if (!isValidQuestion(question)) {
            toastr.error("Chưa nhập đủ thông tin", "Thông báo");
        }
        else {

            $.ajax({
                url: "/admin/questionadmin/eventupdate",
                type: "POST",
                data: {
                    question: question,
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
            });
        }
    });
    //#endregion

    //#region SỰ KIỆN XOÁ CÂU HỎI
    $(document).on("click", "#btn-delete", function () {
        const question = $("#form-question").serializeObject();
        $.ajax({
            url: "/admin/questionadmin/eventdelete",
            type: "POST",
            data: {
                question: question,
            },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            success: function (response) {
                if (response.responseCode == "200") {
                    toastr.success(response.responseMess, "Thông báo");
                    setTimeout(function () {
                        window.location.href = "/admin/cau-hoi";
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
        });
    });
    //#endregion

});


isValidQuestion = function (question) {
    if (question.Content == "" || question.AnswerA == "" || question.AnswerB == "" || question.AnswerC == ""
        || question.AnswerD == "") {
        return false;
    }
    return true;
}

//Xủ lý các logic liên quan đến làm bài thi
$(document).ready(function () {

    //#region SỰ KIỆN TÌM KIẾM BỘ ĐỀ
    $(document).on("submit", "#form-filter", function () {
        const name = $("input", this).val();
        const categoryId = $("select", this).val();
        if (categoryId == -1) {
            if (name == "") {
                window.location.href = `/bo-de`;
            }
            else {
                window.location.href = `/bo-de?name=${name}`;
            }
        }
        else {
            if (name == "") {
                window.location.href = `/bo-de?category=${categoryId}`;
            }
            else {
                window.location.href = `/bo-de?category=${categoryId}&name=${name}`;
            }
        }

        return false;
    });
    //#endregion

    //#region SỰ KIỆN NỘP BÀI
    $(document).on("click", "#btn-submit", function () {
        //Kiểm tra người dùng đã chọn đủ đáp án hay chưa
        if ($(".question-item input").length / 4 != $(".question-item input:checked").length) {
            //toastr.error("Hãy chọn đủ đáp án", "Thông báo");
            //Chưa chọn đủ đáp án
            let mess = "Câu hỏi số";
            $(".question-item").each(function () {
                const order = $(this).attr("data-order");
                if ($(this).find("input:checked").length == 0) {
                    mess += ` ${order},`;
                }
            })
            //Loại bỏ dấu , cuối
            mess = mess.substring(0, mess.length - 1);
            mess += " chưa được chọn. Vui lòng hoàn thành tất cả câu hỏi trước khi nộp bài";
            toastr.error(mess, "Thông báo");
        }
        //Đã chọn đủ đáp án
        else {
            submitExam();
        }
    });
    //#endregion

    //#region SỰ KIỆN CHẤM LẠI BÀI
    $(document).on("click", "#btn-submit-again", function () {
        const pathLst = window.location.pathname.split("/");
        const historyId = parseInt(pathLst[pathLst.length - 1]);
        const timeDo = $("#time-do-exam").attr("data-time");
        $.ajax({
            url: "/history/eventmarkpointagain",
            type: "POST",
            data: {
                historyId: historyId,
                timeDoExam: timeDo
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
                    }, 800)
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

    //#region SỰ KIỆN TÌM KIẾM LỊCH SỬ
    $(document).on("submit", "#form-filter-history", function () {
        const examId = $("#select-exam", this).val();
        const userId = $("#select-user", this).val();
        if (examId == -1) {
            window.location.href = `/lich-su` + (userId != undefined && userId != -1 ? `?userId=${userId}` : '');
        }
        else {
            window.location.href = `/lich-su?examId=${examId}` + (userId != undefined && userId != -1 ? `&userId=${userId}` : '');
        }

        return false;
    });
    //#endregion

    //#region SỰ KIỆN EXPORT KẾT QUẢ THI
    $(document).on("click", "#btn-export-excels", function () {
        let lstExportHistory = [];
        $(".question-item").each(function () {
            let questionContent = $(this).find(".question-title").text().trim();
            let answerSelect, result = "";
            const lstInputChecked = $(this).find("input:checked");
            //Chưa chọn
            if (lstInputChecked.length == 0) {
                answerSelect = "Chưa chọn";
            }
            //Có chọn đáp án thì lấy ra nội dung đáp án đã chọn
            else {
                const idInput = $(lstInputChecked[0]).attr("id");
                answerSelect = $(`label[for=${idInput}]`).text().trim();
            }
            result = $(this).find("p.font-weight-bold").text().trim();
            lstExportHistory.push({
                QuestionContent: questionContent,
                AnswerSelect: answerSelect,
                Result: result
            });
        });
        const pathLst = window.location.pathname.split("/");
        const historyId = parseInt(pathLst[pathLst.length - 1]);
        $.ajax({
            url: "/history/eventexporthistory",
            type: "POST",
            data: {
                historyId: historyId,
                lstExportHistory: lstExportHistory
            },
            beforeSend: function () {
                showLoading();
            },
            success: function () {
                console.log("a");
                /*if (response.responseCode == "200") {
                    toastr.success(response.responseMess, "Thông báo");
                    setTimeout(function () {
                        location.reload();
                    }, 800)
                }
                else {
                    toastr.error(response.responseMess, "Thông báo");
                }*/
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

getQuestionSelectLst = function () {
    let questionSelectLst = [];
    $(".question-item").each(function () {
        if ($(this).find("input:checked").val() != undefined) {
            questionSelectLst.push({
                QuestionId: $(this).attr("data-target"),
                AnswerSelect: $(this).find("input:checked").val()
            })
        }
    });
    return questionSelectLst;
}

//Hàm nộp bài
submitExam = function () {
    const pathLst = window.location.pathname.split("/");
    const examId = parseInt(pathLst[pathLst.length - 1]);
    const questionSelectLst = getQuestionSelectLst();
    $.ajax({
        url: "/exam/eventmarkpoint",
        type: "POST",
        data: {
            examId: examId,
            timeDoExam: TIME_DO / 60,
            questionSelectLst: questionSelectLst,
        },
        dataType: "json",
        beforeSend: function () {
            showLoading();
        },
        success: function (response) {
            if (response.responseCode == "200") {
                toastr.success(response.responseMess, "Thông báo");
                const historyId = response.result[0];
                setTimeout(function () {
                    window.location.href = `/lich-su/chi-tiet/${historyId}`;
                }, 800)
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
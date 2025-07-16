selectedQuestionIdLst = [];
var isOpenModalFirst = true;
var isUpdateFirst = true;
$(document).ready(function () {


    //#region SỰ KIỆN TÌM KIẾM BỘ ĐỀ
    $(document).on("submit", "#form-filter", function () {
        const name = $("input", this).val();
        const categoryId = $("select", this).val();
        if (categoryId == -1) {
            if (name == "") {
                window.location.href = `/admin/bo-de`;
            }
            else {
                window.location.href = `/admin/bo-de?name=${name}`;
            }
        }
        else {
            if (name == "") {
                window.location.href = `/admin/bo-de?category=${categoryId}`;
            }
            else {
                window.location.href = `/admin/bo-de?category=${categoryId}&name=${name}`;
            }
        }

        return false;
    });
    //#endregion

    //#region SỰ KIỆN CLICK MỞ MODAL CHỌN CÂU HỎI
    $(document).on("click", "#btn-modal-question", function () {
        //Lấy ra id của các câu hỏi đã chọn và đánh dấu giao diện
        setSelectedQuestionIdLst();
    });
    //#endregion

    //#region SỰ KIỆN CLICK VÀO CÁC CÂU HỎI ĐỂ CHỌN/ĐỂ BỎ CHỌN
    $(document).on("click", "#table-question tbody tr", function () {
        //Lấy ra id
        const questionId = parseInt($(this).attr("data-target"));
        //Chọn
        if (!$(this).hasClass("selected")) {
            //Chưa có câu hỏi nào
            if (selectedQuestionIdLst.length == 0) {
                //Setup table html
                $("#table-question-selected").html(
                    "<table class='table table-hover'>" +
                    "   <thead>" +
                    "       <tr>" +
                    "           <th>Câu hỏi</th>" +
                    "           <th>Chủ đề</th>" +
                    "           <th>Đáp án A</th>" +
                    "           <th>Đáp án B</th>" +
                    "           <th>Đáp án C</th>" +
                    "           <th>Đáp án D</th>" +
                    "           <th>Đáp án đúng</th>" +
                    "       </tr>" +
                    "   </thead>" +
                    "   <tbody>" +
                    "   </tbody>" +
                    "</table>");
            }
            //Thêm html vào danh sách question đã chọn
            $("#table-question-selected tbody").append($(this).prop('outerHTML'));
            //Đánh dấu câu hỏi đã được chọn
            $(this).addClass("selected");
            //Thêm id câu hỏi vào selectedQuestionIdLst
            selectedQuestionIdLst.push(questionId);
            //Cập nhật giao diện
            updateTotalQuestionSelected();
        }
        //Bỏ chọn
        else {
            //Bỏ class selected
            $(this).removeClass("selected");
            //Xoá html vào danh sách question đã chon
            $(`#table-question-selected tbody tr[data-target=${questionId}]`).remove();
            //Trường hợp xoá hết các câu hỏi đã chọn
            if (selectedQuestionIdLst.length == 1) {
                $("#table-question-selected").html("<p class='text-center'>Không có dữ liệu</p>");
            }
            //Xoá id câu hỏi ở selectedQuestionIdLst
            selectedQuestionIdLst.splice(selectedQuestionIdLst.indexOf(questionId), 1);
            //Cập nhật giao diện
            updateTotalQuestionSelected();
        }


    });
    //#endregion

    //#region SỰ KIỆN TÌM KIẾM QUESTION Ở MODAL
    $(document).on("submit", "#form-filter-question", function () {
        const name = $("input", this).val();
        const category = $("select", this).val();
        //load lại dữ liệu
        loadTableQuestion({ name: name, category: category }, true);
        return false;
    });
    //#endregion

    //#region SỰ KIỆN ẤN XÁC NHẬN CHỌN CÁC CÂU HỎI
    $(document).on("click", "#btn-save-detail", function () {
        //Nếu không có question nào được chọn
        if (selectedQuestionIdLst.length == 0) {
            $("#table-question-exam").html("<p class='text-center'>Không có dữ liệu</p>");
        }
        else {
            //Duyệt html các question đã được chọn
            //Setup table html
            $("#table-question-exam").html(
                "<table class='table'>" +
                "   <thead>" +
                "       <tr>" +
                "           <th>STT</th>" +
                "           <th>Câu hỏi</th>" +
                "           <th>Chủ đề</th>" +
                "           <th>Đáp án A</th>" +
                "           <th>Đáp án B</th>" +
                "           <th>Đáp án C</th>" +
                "           <th>Đáp án D</th>" +
                "           <th>Đáp án đúng</th>" +
                "       </tr>" +
                "   </thead>" +
                "   <tbody>" +
                "   </tbody>" +
                "</table>");
            let html = ""
            $("#table-question-selected tbody tr").each(function (index) {
                var newTrElement = $(this).clone();
                //Thêm cột stt
                newTrElement.html(`<td>${index + 1}</td>` + newTrElement.html());
                html += newTrElement.prop('outerHTML');
            });
            $("#table-question-exam tbody").html(html);
        }
        //Đóng modal
        $(".close").trigger("click");

    });
    //#endregion

    //#region SỰ KIỆN THÊM BỘ ĐỀ
    $(document).on("click", "#btn-create", function () {
        const exam = $("#form-question").serializeObject();
        //Thông tin hợp lệ
        if (isValidExam(exam)) {
            //Bỏ dấu cách
            exam.ExamName = exam.ExamName.trim();
            $.ajax({
                url: "/admin/examadmin/eventcreate",
                type: "POST",
                data: {
                    exam: exam,
                    questionIdLst: selectedQuestionIdLst,
                },
                dataType: "json",
                beforeSend: function () {
                    showLoading();
                },
                success: function (response) {
                    if (response.responseCode == "200") {
                        toastr.success(response.responseMess, "Thông báo");
                        setTimeout(function () {
                            window.location.href = "/admin/bo-de";
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

    //#region SỰ KIỆN SỬA BỘ ĐỀ
    $(document).on("click", "#btn-update", function () {
        //Nếu cập nhật lần đầu
        if (isUpdateFirst == true) {
            $("#table-question-exam tbody tr").each(function () {
                const questionId = parseInt($(this).attr("data-target"));
                //Thêm id câu hỏi vào selectedQuestionIdLst
                if (!selectedQuestionIdLst.includes(questionId)) {
                    selectedQuestionIdLst.push(questionId);
                }

                isUpdateFirst = false;
            })
        }
        const exam = $("#form-question").serializeObject();
        //Thông tin hợp lệ
        if (isValidExam(exam)) {
            //Bỏ dấu cách
            exam.ExamName = exam.ExamName.trim();
            $.ajax({
                url: "/admin/examadmin/eventupdate",
                type: "POST",
                data: {
                    exam: exam,
                    questionIdLst: selectedQuestionIdLst,
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

    //#region SỰ KIỆN XOÁ BỘ ĐỀ
    $(document).on("click", "#btn-delete", function () {
        const exam = $("#form-question").serializeObject();
        $.ajax({
            url: "/admin/examadmin/eventdelete",
            type: "POST",
            data: {
                exam: exam,
            },
            dataType: "json",
            beforeSend: function () {
                showLoading();
            },
            success: function (response) {
                if (response.responseCode == "200") {
                    toastr.success(response.responseMess, "Thông báo");
                    setTimeout(function () {
                        window.location.href = "/admin/bo-de"
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


isValidExam = function (exam) {
    if (exam.Time.toString().includes(".")) {
        toastr.error("Thời gian phải là số nguyên", "Thông báo");
        return false;
    }
    if (exam.Time <= 0) {
        toastr.error("Thời gian không hợp lệ", "Thông báo");
        return false;
    }
    if (exam.ExamName == "") {
        toastr.error("Chưa nhập đủ thông tin", "Thông báo");
        return false;
    }
    if (selectedQuestionIdLst.length == 0) {
        toastr.error("Chưa chọn câu hỏi", "Thông báo");
        return false;
    }
    return true;
}

//Hàm lấy các question đã được chọn và gán vào biến global selectedQuestionIdLst
setSelectedQuestionIdLst = function () {
    //Rest
    selectedQuestionIdLst = []
    if ($("#table-question-exam tbody tr").length == 0) {
        $("#table-question-selected").html("<p class='text-center mt-4'>Không có dữ liệu</p>");
        //Bỏ hết đánh dấu
        $("#table-question tbody tr").removeClass("selected");
    }
    else {
        $("#table-question-exam tbody tr").each(function () {
            const questionId = parseInt($(this).attr("data-target"));
            //Setup table html
            if (selectedQuestionIdLst.length == 0) {
                $("#table-question-selected").html(
                    "<table class='table table-hover'>" +
                    "   <thead>" +
                    "       <tr>" +
                    "           <th>Câu hỏi</th>" +
                    "           <th>Chủ đề</th>" +
                    "           <th>Đáp án A</th>" +
                    "           <th>Đáp án B</th>" +
                    "           <th>Đáp án C</th>" +
                    "           <th>Đáp án D</th>" +
                    "           <th>Đáp án đúng</th>" +
                    "       </tr>" +
                    "   </thead>" +
                    "   <tbody>" +
                    "   </tbody>" +
                    "</table>");
            }
            //Thêm html vào danh sách question đã chọn
            const newTrElement = $(this).clone();
            //Xoá cột stt
            newTrElement.find("td:first-child").remove();
            $("#table-question-selected tbody").append(newTrElement.prop('outerHTML'));
            //Đánh dấu vào danh sách question
            $(`#table-question tbody tr[data-target=${questionId}]`).addClass("selected");
            //Thêm id câu hỏi vào selectedQuestionIdLst
            selectedQuestionIdLst.push(questionId);
            //Cập nhật giao diện
            updateTotalQuestionSelected();
        })
    }
    updateTotalQuestionSelected();

}

//#region load table question 
loadTableQuestion = function (data = {}, isShowLoading = false) {
    //Gọi ajax để load dữ liệu
    $.ajax({
        url: "/admin/examadmin/tablequestion",
        type: "POST",
        data: data,
        dataType: "html",
        beforeSend: function () {
            if (isShowLoading == true) {
                showLoading();
            }
        },
        success: function (html) {
            //Có dữ liệu trả về
            if (html.includes("</td>")) {
                $("#table-question").html(html);
                //Đánh dấu lại các question trong danh sách question mà đã được chọn
                markQuestionSelected();
            }
            //Không có dữ liệu trả về
            else {
                $("#table-question").html("<p class='text-center'>Không có dữ liệu</p>");
            }
        },
        error: function () {
            toastr.error("Máy chủ tạm thời không phản hồi, vui lòng thử lại sau", "Thông báo")
        }
    }).always(function () {
        if (isShowLoading == true) {
            hideLoading();
        }

    });
}
//#endregion

//Đánh dấu lại các question trong danh sách question mà đã được chọn
markQuestionSelected = function () {
    //Đánh dấu lại các question trong danh sách question mà đã được chọn
    $("#table-question tbody tr").each(function () {
        const questionId = parseInt($(this).attr("data-target"));
        //Đánh dấu lại các hỏi đã được chọn bằng class
        if (selectedQuestionIdLst.includes(questionId)) {
            $(this).addClass("selected");
        }
    });
}

updateTotalQuestionSelected = function () {
    $("#total-question-selected").text(selectedQuestionIdLst.length);
}
// Sự kiện khi click vào nút "Random 50 câu hỏi"
$('#btn-random-question').click(function () {
    const $availableQuestions = $('#table-question tbody tr');
    const max = 50;
    const selectedIds = [];

    // Nếu không đủ câu hỏi thì cảnh báo
    if ($availableQuestions.length < max) {
        alert("Không đủ 50 câu hỏi để chọn!");
        return;
    }

    // Lấy danh sách id từ các dòng câu hỏi
    const questionIds = $availableQuestions.map(function () {
        return $(this).data('target');
    }).get();

    // Random 50 ID
    const randomIds = [];
    while (randomIds.length < max) {
        const randId = questionIds[Math.floor(Math.random() * questionIds.length)];
        if (!randomIds.includes(randId)) {
            randomIds.push(randId);
        }
    }

    // Gọi hàm thêm vào bảng câu hỏi đã chọn (giả sử bạn có sẵn hàm addSelectedQuestionById)
    randomIds.forEach(function (id) {
        addSelectedQuestionById(id); // Hàm này bạn cần định nghĩa hoặc dùng sẵn trong exam.js
    });

    // Cập nhật số câu hỏi đã chọn
    $('#total-question-selected').text(randomIds.length);
});
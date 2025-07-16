
//Xủ lý các logic liên quan đến làm bài thi
var TIME_DO = 0;
$(document).ready(function () {
    startCountDown();

});


startCountDown = function () {
    var timer = $("#count-down-wrapper").attr("data-time");
    let hour, minute, second;
    var countDown = setInterval(function () {

        hour = parseInt(timer / 3600);
        minute = parseInt(parseInt(timer % 3600) / 60);
        second = parseInt(parseInt(timer % 3600) % 60);
        hour = hour < 10 ? "0" + hour : hour;
        minute = minute < 10 ? "0" + minute : minute;
        second = second < 10 ? "0" + second : second;
        $("#count-down-wrapper").html(`<i class='bx bx-time'></i> ${hour}:${minute}:${second}`);
        //Hiện giao diện
        $("#count-down-wrapper").removeClass("d-none");
        if (timer == 60) {
            toastr.warning("Thời gian làm còn 1 phút nữa", "Thông báo");
        }
        if (timer == 10) {
            toastr.warning("Sau 10 giây hệ thống sẽ tự nộp bài", "Thông báo");
        }
        if (timer == 0) {
            //Xoá interval
            clearInterval(countDown)
            //Nộp bài
            submitExam();
        }
        else {
            timer--;
            TIME_DO++;
        }

    }, 1000);
}
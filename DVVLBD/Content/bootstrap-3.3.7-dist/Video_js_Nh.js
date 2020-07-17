
//var video_player = document.getElementById("video_player"),
//links = video_player.getElementsByTagName('a');
//for (var i = 0; i < links.length; i++) {
//    links[i].onclick = handler;
//}
//function handler(e) {
//    e.preventDefault();
//    videotarget = this.getAttribute("href");
//    filename = videotarget.substr(0, videotarget.lastIndexOf('.')) || videotarget;
//    video = document.querySelector("#video_player iframe");
//    //video.removeAttribute("controls");
//    video.removeAttribute("poster");
//    //source = document.querySelectorAll("#video_player video source"); Source của video (đổi video=iframe)
//    source[0].src = filename + ".mp4" ;
//    source[1].src = filename + ".webm";

//    video.load();
//    video.play();
//    //document.getElementById("demo").innerHTML="Đang Phát";

//}


//SCRIPT CHUYỂN SRC TRONG IFRAME DIV 1
$(document).ready(function () {
    $("div#videocon a").click(function (e) {
        e.preventDefault();

        $("#someFrame").attr("src", $(this).attr("videopath") + "?rel=0&autoplay=1");
    })
});







//SCRIPT CHUYỂN SRC TRONG IFRAME DIV 1
$(document).ready(function () {
    $("div#hide a").click(function (e) {
        e.preventDefault();

        $("#someFrame").attr("src", $(this).attr("href") + "?rel=0&autoplay=1");
    })
});
//SCRIPT CHUYỂN SRC TRONG IFRAME DIV 1
$(document).ready(function () {
    $("div#hide2 a").click(function (e) {
        e.preventDefault();

        $("#someFrame").attr("src", $(this).attr("href") + "?rel=0&autoplay=1");
    })
});

//Hiệu ứng ẩn hiện

//SCRIPT HIEN AN DIV VIDEO

//function onclick1() {
//}
////HIỆN APPEND VIDEO 
//$(document).ready(function () {
//    $("#btn").click(function () {
//        $('#vd1').append(hien());
//    });


//});

////SCRIPT CHỨC NĂNG CÁC BUTTON
//$('#btn_hien1').click(function () {
//    $('#hide').slideDown();
//    $("#btn_an1").css("display", "block");
//    $("#btn_hien1").css("display", "none");
//});

//$('#btn_hien2').click(function () {
//    $('#hide2').slideDown();
//    $("#btn_an2").css("display", "block");
//    $("#btn_an1").css("display", "none");
//    $("#btn_hien2").css("display", "none");
//});

//$('#btn_an1').click(function () {
//    $('#hide').slideUp();
//    $("#btn_hien1").css("display", "block");
//    $("#btn_hien2").css("display", "none");


//});
//$('#btn_an2').click(function () {
//    $('#hide2').slideUp();
//    $("#btn_hien2").css("display", "block");
//    $("#btn_an1").css("display", "block");
//    $("#btn_an2").css("display", "none");

//});




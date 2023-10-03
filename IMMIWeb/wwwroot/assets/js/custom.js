$(window).scroll(function() {    
    var scroll = $(window).scrollTop();    
    if (scroll <= 0) {
        $("header").removeClass("clearHeader").addClass("darkHeader");
    }
});

// Page Load to show modal
//$(window).on('load', function() {
//        $('#inComingCall').modal('show');
//});

//Input Hide show on click Radio button
var checkbox = document.getElementById("audio");
var monthly_emi = document.getElementById("monthlyEMI");
function addmentor() {
    if (checkbox.checked = true) {
        monthly_emi.style.display = "block";
        $("#MakePayment").val("Yes, Pay $"+sessionStorage.getItem("EMIPaymment"));
        //$("#amountNeedToPay").html(sessionStorage.getItem("EMIPaymment"));
    }
}
function hideInputDiv() {
    monthly_emi.style.display = "none";
    $("#MakePayment").val("Yes, Pay $" + sessionStorage.getItem("FullPaymment"));
    //$("#amountNeedToPay").html(sessionStorage.getItem("FullPaymment"));
}
//trigger second button
$("#next_payment").click(function(){
    $("#profile-tab").click(); 
});


$("#custom-input-date").datepicker({ dateFormat:'dd/mm/yy'});

// ACTIONS
$("input").on("change", function(e) {
  $(this).siblings(".label-error").text("");
  $(this).removeClass("error");
})

$("#custom-input-date").on("focusout", function(e) {
  if($(this).val() != '') {
    dateValidation($(this));
  }
})

// CHECK
function dateValidation(input) {
  var errorLabel = input.siblings(".label-error");
  var date = input.val();

  input.removeClass("error");
  errorLabel.text("");

  var matches = /^(\d{1,2})[/\/](\d{1,2})[/\/](\d{4})$/.exec(date);
  if (matches == null) {
    input.addClass("error");
    errorLabel.text("Date not valid.");
  };

  var d = matches[1];
  var m = matches[2] - 1;
  var y = matches[3];
  var composedDate = new Date(y, m, d);

  if(composedDate.getDate() == d && composedDate.getMonth() == m && composedDate.getFullYear() == y) {} else {
    input.addClass("error");
    errorLabel.text("Date not valid.");
  }
}


$(document).on('click', '.mobile-tabs .nav-link', function(){
    $('.mobile-tabs .nav-link').removeClass('active');
    $('.mobile-tabs').toggleClass('expanded');
    $(this).addClass('active');
    var tab_id = $(this).attr('data-bs-target');
    $('.tab-pane').removeClass('current');
    $(this).addClass('current');
    $('#'+tab_id).addClass('current');
  });

$(document).ready(function(){
    new WOW().init();
  
    $('.logo-slider').owlCarousel({
        loop:true,
        margin:25,
        ltr: true,
        autoplay: true,
        slideTransition: 'linear',
        autoplaySpeed: 6000,
        autoplayHoverPause: true,
        nav:false,
        dots: false,
        responsive:{
            0:{
                items:2,
                nav:false
            },
            600:{
                items:3,
                nav:false
            },
            1000:{
                items:4,
                nav:false
            },
            1200:{
                items:5,
            },
            1400:{
                items:6,
            }
        }
    });
    $('.how-work-slider').owlCarousel({
        loop:true,
        autoplay: true,
        nav:true,
        center: true,
        dots: false,
        responsive:{
            0:{
                items:1,
                center: false,
            },
            576:{
                items:2,
                center: false,
                margin: 70
            },
            1000:{
                items:3,
                center: false,
                margin: 70
            },
            1400:{
                items:3,
                margin: 100,
            },
            1500:{
                items:3,
                margin: 100
            },
            1700:{
                items: 3,
                margin: 110,
            }
        }
    });

    $('.client-slider').owlCarousel({
        loop:true,
        margin: 25,
        autoplay: true,
        nav:false,
        center: false,
        dots: true,
        responsive:{
            0:{
                items:1
            },
            576:{
                items:2
            },
            1000:{
                items:4
            },
            1600:{
                items:5
            },
            1700:{
                items:6
            }
        }
    });


    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();
            reader.onload = function(e) {
                $('#imagePreview').css('background-image', 'url('+e.target.result +')');
                $('#imagePreview').hide();
                $('#imagePreview').fadeIn(650);
            }
            reader.readAsDataURL(input.files[0]);
        }
    }
    $("#imageUpload").change(function() {
        readURL(this);
    });

    $(document).ready(function(){
        $('.discover-wrap a[href^="#"]').on('click',function (e) {
            e.preventDefault();
            var target = this.hash;
	    var $target = $(target);
	    $('html, body').stop().animate({
	        'scrollTop': $target.offset().top
	    }, 600, 'swing', function () {
	    });
        }); 
    });

    const videoElement = document.getElementById('video');
    const playPauseButton = document.getElementById('button');

    playPauseButton.addEventListener('click', () => {
        playPauseButton.classList.toggle('playing');
        if (playPauseButton.classList.contains('playing')) {
            videoElement.play();
        }
        else {
            videoElement.pause();
        }
    });

    videoElement.addEventListener('ended', () => {
        playPauseButton.classList.remove('playing');
    });

   // Hide Header on on scroll down
   var didScroll;
   var lastScrollTop = 0;
   var delta = 5;
   var navbarHeight = $('header').outerHeight();
   
   $(window).scroll(function(event){
       didScroll = true;
   });
   
   setInterval(function() {
       if (didScroll) {
           hasScrolled();
           didScroll = false;
       }
   }, 250);
   
   function hasScrolled() {
       var st = $(this).scrollTop();
       
       // Make sure they scroll more than delta
       if(Math.abs(lastScrollTop - st) <= delta)
           return;

          
       
       // If they scrolled down and are past the navbar, add class .nav-up.
       // This is necessary so you never see what is "behind" the navbar.
       if (st > lastScrollTop && st > navbarHeight){
           // Scroll Down
           $('header').removeClass('nav-down').addClass('nav-up');
       } else {
           // Scroll Up
           if(st + $(window).height() < $(document).height()) {
               $('header').removeClass('nav-up').addClass('nav-down');
           }
       }
       
       
       lastScrollTop = st;
   }


    $("input").intlTelInput({
        utilsScript: "https://cdnjs.cloudflare.com/ajax/libs/intl-tel-input/8.4.6/js/utils.js"
    });

   

 
});







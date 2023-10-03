var REGION = 'us';
var APP_URL = "{{ env('APP_URL') }}";

function CometChatInitialization() {
    Initialization();
}

function Initialization() {

    let CometChatAppSetting = new CometChat.AppSettingsBuilder()
        .subscribePresenceForAllUsers()
        .setRegion(REGION)
        .build();

    CometChat.init("2372747e41d99953", CometChatAppSetting).then(
        () => {
            console.log("Initialization completed successfully");
            CometChat.login(sessionStorage.getItem("UID"), "f98aac58490bac6ae40b152550f5c4fef6e6c938").then(
                () => {

                    var data = sessionStorage.getItem("userChatCntList");


                    var dataArray = data.split(',');
                    var sum = 0;
                    
                    for (var i = 0; i < dataArray.length; i++) {
                        var value = dataArray[i];                        
                        CometChat.getUnreadMessageCountForUser(value).then(
                            array => {                                
                                if (array != null) {
                                    for (var key in array) {
                                        if (array.hasOwnProperty(key)) {
                                            sum += array[key];
                                            $(".totalUserMsgCountCls").html(sum);
                                        }
                                    }
                                }                               
                            });
                    }

                    console.log("Login completed successfully");

                }, (error) => {
                    console.log("Login failed with error", error);
                }
            );
        }, (error) => {
            console.log("Initialization failed with error");
        }
    );

    CreateMsgListener(sessionStorage.getItem("UID"));
    CreateCallListener(sessionStorage.getItem("UID"));

}

function Login() {
    CometChat.login(sessionStorage.getItem("UID"), "f98aac58490bac6ae40b152550f5c4fef6e6c938").then(
        () => {
            console.log("Login completed successfully");
        }, (error) => {
            console.log("Login failed with error");
        }
    );
}

function ConsultantCallInit(APPOINTMENTID,RID,CallType) {

    var ReceiverType = CometChat.RECEIVER_TYPE.USER;
    var call = new CometChat.Call(RID, CallType, ReceiverType);

    CometChat.initiateCall(call).then(

        outGoingCall => {
            //document.getElementById('sessioIdData').value = outGoingCall.sessionId;
            console.log("Call initiated successfully:", outGoingCall);

            console.log("session start time", outGoingCall.sentAt);
            console.log("session start time", outGoingCall.sessionId);
            AddSessionInAppointment(APPOINTMENTID,outGoingCall.sessionId);

        }, error => {
            console.log("Call initialization failed with exception:", error);
        }
    );
    CreateCallListener(RID);
}

function CreateCallListener(RID) {
    CometChat.addCallListener(
        RID,
        new CometChat.CallListener({

            onIncomingCallReceived: (call) => {

                console.log("Incoming call:", call);

                $("#clrName").html(call.sender.name);
                $("#clrType").html('Incoming ' + call.type + ' Call');
                document.getElementById("inComingCall").style.display = "block";
                sessionStorage.setItem("sessionId", call.sessionId);      
                PlayIncomingSound();
            },
            onOutgoingCallAccepted: (call) => {


                var callStarted = sessionStorage.getItem("callStarted");

                if (callStarted == null || callStarted == "") {

                    console.log("Outgoing call accepted:", call);

                    var sessionId = call.sessionId;
                    var callType = call.type;
                    var callSettings = new CometChat.CallSettingsBuilder()
                        .setSessionID(sessionId)
                        .enableDefaultLayout(true)
                        .setIsAudioOnlyCall(callType == 'audio' ? true : false)
                        .build();

                    sessionStorage.setItem("callStarted", "True");
                    $("#inComingCall").hide();
                    $("#videoCallModal").show();
                    
                    CometChat.startCall(

                        callSettings,
                        document.getElementById("callScreen"),
                        new CometChat.OngoingCallListener
                            ({
                                onUserJoined: user => {
                                    console.log("User joined call:", user);  

                                    console.log("Consultant Appointment Timer Calling...");  
                                    ConsultantAppointmentTimer();

                                },
                                onUserLeft: user => {
                                    console.log("User left call:", user);
                                },
                                onUserListUpdated: userList => {
                                    console.log("user list:", userList);
                                },
                                onCallEnded: call => {

                                    
                                   

                                    console.log("Call ended consultant:", call);
                                    
                                    $("#videoCallModal").hide();
                                    sessionStorage.setItem("callStarted", "");
                                    
                                    AppointmentCallEnded(call.sessionId);
                                },
                                onError: error => {
                                    console.log("Error :", error);
                                },
                                onMediaDeviceListUpdated: deviceList => {
                                    console.log("Device List:", deviceList);
                                },
                                onUserMuted: (userMuted, userMutedBy) => {
                                    // This event will work in JS SDK v3.0.2-beta1 & later.
                                    console.log("Listener => onUserMuted:", userMuted, userMutedBy);
                                },
                                onScreenShareStarted: () => {
                                    // This event will work in JS SDK v3.0.3 & later.
                                    console.log("Screen sharing started.");
                                },
                                onScreenShareStopped: () => {
                                    // This event will work in JS SDK v3.0.3 & later.
                                    console.log("Screen sharing stopped.");
                                },
                                onCallSwitchedToVideo: (sessionId, callSwitchInitiatedBy, callSwitchAcceptedBy) => {
                                    // This event will work in JS SDK v3.0.8 & later.
                                    console.log("call switched to video:", { sessionId, callSwitchInitiatedBy, callSwitchAcceptedBy });
                                }
                            }));
                }
            },
            onOutgoingCallRejected: (call) => {
                console.log("Outgoing call rejected:", call);
                alert("user has been rejected call");
                RejectCall();
            },
            onIncomingCallCancelled: (call) => {
                console.log("Incoming call calcelled:", call);
            }

        })
    );
}

function CreateMsgListener(RID) {


    CometChat.addMessageListener(
        RID,
        new CometChat.MessageListener({
            onTextMessageReceived: textMessage => {

                console.log("Text message received successfully", textMessage);
                sessionStorage.setItem("rId", textMessage.sender.uid);
                var returnTag = textMessage.tags[0];

                var whichTag = returnTag.split("_");
                if (whichTag[0] == "Appointment") {
                    $("#btnJoinChat").show();
                    var btnJoinChat = $('#btnJoinChat');
                    btnJoinChat.attr('onclick', 'JoinChatByUser("' + returnTag + '","' + textMessage.sender.uid + '")');
                }

                var varSenderName = textMessage.sender.name;
                var varSenderMsg = textMessage.text;
                var randerMsg = '<div class="message my-message">' + varSenderMsg + '</div>' + '<div class="message-data"><span class="message-data-time">' + MiliSeconedToMin(textMessage.sentAt) + '</span></div>';
                document.getElementById('messages').innerHTML += randerMsg;

            },
            onMediaMessageReceived: mediaMessage => {
                console.log("Media message received successfully", mediaMessage);
                sessionStorage.setItem("rId", mediaMessage.sender.uid);

                var returnTag = mediaMessage.tags[0];

                var whichTag = returnTag.split("_");
                if (whichTag[0] == "Appointment") {                    
                    $("#btnJoinChat").show();
                    var btnJoinChat = $('#btnJoinChat');
                    btnJoinChat.attr('onclick', 'JoinChatByUser("' + returnTag + '", "' + mediaMessage.sender.uid + '" )');                
                }
                if (mediaMessage.type.toLowerCase() == "image") {

                    var varSenderName = mediaMessage.sender.name;
                    var varSenderImg = mediaMessage.data.attachments[0].url;
                    var randerMsg = '<div class="message my-message"><img src=' + varSenderImg + ' width="250" height="150"></div>' + '<div class="message-data"><span class="message-data-time">' + MiliSeconedToMin(mediaMessage.sentAt) + '</span></div>';
                    document.getElementById('messages').innerHTML += randerMsg;
                }
                else {
                    var varSenderName = mediaMessage.sender.name;
                    var varSenderImg = mediaMessage.data.attachments[0].url;
                    var varFileName = mediaMessage.data.attachments[0].name;
                    var randerMsg = '<div class="message my-message"> <a  target="_blank" href=' + varSenderImg + ' >' + varFileName + '</a></div>' + '<div class="message-data"><span class="message-data-time">' + MiliSeconedToMin(mediaMessage.sentAt) + '</span></div>';
                    document.getElementById('messages').innerHTML += randerMsg;
                }
            },
            onCustomMessageReceived: customMessage => {
                console.log("Custom message received successfully", customMessage);
            }
        })
    );
}

function AcceptCall() {

    PauseIncomingSound();
    $("#inComingCall").hide();
    $("#clrType").html();
    $("#clrPic").html();
    $("#clrName").html();

    var sessioIdData = sessionStorage.getItem("sessionId");
    CometChat.acceptCall(sessioIdData).then(
        call => {

            console.log("Call accepted successfully:", call);
            var sessionId = call.sessionId;
            var callType = call.type;

            let callController = CometChat.CallController.getInstance();
            callController.startRecording();

            var callSettings = new CometChat.CallSettingsBuilder()
                .setSessionID(sessionId)
                .enableDefaultLayout(true)
                .setIsAudioOnlyCall(callType == 'audio' ? true : false)
                .startRecordingOnCallStart('true')
                .build();

            $("#videoCallModal").show();

            console.log("AppointmentTimer calling....");
            UserAppointmentTimer();

            CometChat.startCall(
                callSettings,
                document.getElementById("callScreen"),
                new CometChat.OngoingCallListener({
                    onUserJoined: user => {
                        console.log("User joined call:", user);
                        UserPresentInCall(call.sessionId);
                    },
                    onUserLeft: user => {
                        console.log("User left call:", user);
                    },
                    onUserListUpdated: userList => {
                        console.log("user list:", userList);
                    },
                    onCallEnded: call => {
                        console.log("Call ended for user :", call);
                        $("#videoCallModal").hide();
                        sessionStorage.setItem("callStarted", "");
                        AppointmentCallEndedUser(call.sessionId);
                        $("#retainConsultant").show();
                    },
                    onError: error => {
                        console.log("Error :", error);
                    },
                    onMediaDeviceListUpdated: deviceList => {
                        console.log("Device List:", deviceList);
                    },
                    onUserMuted: (userMuted, userMutedBy) => {
                        // This event will work in JS SDK v3.0.2-beta1 & later.
                        console.log("Listener => onUserMuted:", userMuted, userMutedBy);
                    },
                    onScreenShareStarted: () => {
                        // This event will work in JS SDK v3.0.3 & later.
                        console.log("Screen sharing started.");
                    },
                    onScreenShareStopped: () => {
                        // This event will work in JS SDK v3.0.3 & later.
                        console.log("Screen sharing stopped.");
                    },
                    onCallSwitchedToVideo: (sessionId, callSwitchInitiatedBy, callSwitchAcceptedBy) => {
                        // This event will work in JS SDK v3.0.8 & later.
                        console.log("call switched to video:", { sessionId, callSwitchInitiatedBy, callSwitchAcceptedBy });
                    },
                    onRecordingStarted: recordingStartedBy => {
                        // This event will work in JS SDK v3.0.2-beta1 & later.
                        console.log("Listener => onRecordingStarted:", recordingStartedBy);
                    },
                    onRecordingStopped: recordingStoppedBy => {
                        // This event will work in JS SDK v3.0.2-beta1 & later.
                        console.log("Listener => onRecordingStopped:", recordingStoppedBy);
                    }
                })
            );

        }, error => {
            console.log("Call acceptance failed with error", error);
        }
    );

    //var urlToOpen = "/Chat/UserReceiveCall"; 
    //var windowOptions = "width=500,height=500,scrollbars=no";
    //window.open(urlToOpen, "_blank", windowOptions);

}

function RejectCall() {

    var sessioIdData = sessionStorage.getItem("currentCallSessionId");
    var status = CometChat.CALL_STATUS.REJECTED;

    CometChat.rejectCall(sessioIdData, status).then(
        call => {
            console.log("Call rejected successfully", call);
            sessionStorage.setItem("currentCallSessionId", "");
        }, error => {
            console.log("Call rejection failed with error:", error);
            sessionStorage.setItem("currentCallSessionId", "");
        }
    );

    PauseIncomingSound();
    $("#inComingCall").hide();
    $("#clrType").html();
    $("#clrPic").html();
    $("#clrName").html();
}

function PlayIncomingSound() {
    var audio = document.getElementById("incomingCallMp3");
    audio.play();
}

function PauseIncomingSound() {
    var audio = document.getElementById("incomingCallMp3");
    audio.pause();
    audio.currentTime = 0;
}

function IfUserReceiverCall() {

    var urlToOpen = "/Consultant/ConsultantChat/ConsultantReceiveCall";
    var windowOptions = "width=500,height=500,scrollbars=no";
    window.open(urlToOpen, "_blank", windowOptions);
}

function ConsultantChatInit(RID, AppointmentId) {

    sessionStorage.setItem("rId", RID);
    /*var url = "/Consultant/ConsultantChat/TextChatConsultant?appId=" +"Appointment_"+AppointmentId+"";*/
    var url = "/Consultant/ConsultantChat/TextChatConsultant?appId=Appointment_" + AppointmentId + "&rId=" + RID;
    /*window.location.href = url;*/
    window.open(url, '_blank');

}

function JoinChatByUser(AppointmentId, Id) {    
    /*var url = "/Chat/TextChat?appId=" + AppointmentId + "";*/
    var url = "/Chat/TextChat?appId=" + AppointmentId + "&rId=" + Id;
    window.open(url, '_blank');
}

function SendMessageFun(AppointmentIdTag) {

    var isInitialization = sessionStorage.getItem("isInitialization");
    let messageText = document.getElementById('myMessage').value;

    if (isInitialization != "true") {
        Login();
        sessionStorage.setItem("isInitialization", "true");
    }
    SendTextMessage(AppointmentIdTag);
    MediaMessage(AppointmentIdTag);

}

function SendTextMessage(AppointmentIdTag) {

    var myMessage = $("#myMessage").val();
    var myMessageVal = $.trim(myMessage);

    if (myMessageVal != "") {
        let textMessage = new CometChat.TextMessage(sessionStorage.getItem("rId"), $("#myMessage").val(), CometChat.RECEIVER_TYPE.USER);

        let tags = [AppointmentIdTag];
        textMessage.setTags(tags);

        CometChat.sendMessage(textMessage).then(
            message => {
                var randerMsg = '<div class="message other-message float-right">' + message.text + '</div>' + '<div class="message-data align-right"><span class="message-data-time">' + MiliSeconedToMin(message.sentAt) + '</span></div>';
                $('#messages').append(randerMsg);
                $("#myMessage").val("");
                SetScrollBottom();
            }
        ), error => {
            console.log("Text Message not sent successfully", { error });
        };;
    }
}

function MediaMessage(AppointmentIdTag) {

    var inputElement = document.getElementById('myFile');
    var files = inputElement.files;

    if (files.length > 0) {

        var file = document.getElementById("myFile").files[0];

        //----------
        var fileInput = document.getElementById('myFile');
        var fileName = fileInput.files[0].name;
        var fileExtension = fileName.split('.').pop().toLowerCase();
        //----------

        var fileType = "";
        var fileCase = "";

        if (fileExtension == "jpg" || fileExtension == "jpeg" || fileExtension == "png") {
            fileType = CometChat.MESSAGE_TYPE.IMAGE;
            fileCase = "IMAGE";
        }
        else if (fileExtension == "txt" || fileExtension == "doc" || fileExtension == "docx" || fileExtension == "xls" || fileExtension == "xlsx" || fileExtension == "pdf" || fileExtension == "ppt" || fileExtension == "pptx" || fileExtension == "html" || fileExtension == "htm") {
            fileType = CometChat.MESSAGE_TYPE.FILE;
            fileCase = "FILE";
        }
        else {
            alert("file not allowed");
            return false;
        }

        var mediaMessage = new CometChat.MediaMessage(sessionStorage.getItem("rId"), file, fileType, CometChat.RECEIVER_TYPE.USER);

        let tags = [AppointmentIdTag];
        mediaMessage.setTags(tags);

        CometChat.sendMediaMessage(mediaMessage).then(
            message => {

                console.log("Media Message sent successfully:", message);
                if (fileCase == "IMAGE") {
                    var varSenderName = message.sender.name;
                    var varSenderImg = message.data.attachments[0].url;
                    var randerMsg = '<div class="message other-message float-right"><img src=' + varSenderImg + ' width="250" height="150"></div>' + '<div class="message-data align-right"><span class="message-data-time">' + MiliSeconedToMin(message.sentAt) + '</span></div>';
                    $('#messages').append(randerMsg);
                    SetScrollBottom();
                }
                else {
                    var varSenderName = message.sender.name;
                    var varSenderImg = message.data.attachments[0].url;
                    var varFileName = message.data.attachments[0].name;
                    var randerMsg = '<div class="message other-message float-right"><a target="_blank" href=' + varSenderImg + '  >' + varFileName + '</a></div>' + '<div class="message-data align-right"><span class="message-data-time">' + MiliSeconedToMin(message.sentAt) + '</span></div>';
                    $('#messages').append(randerMsg);
                    SetScrollBottom();
                }
                $("#myFile").val(null);
            }
        ), error => {
            console.log("Media Message not sent successfully", { error });
        };
    }

}

function MiliSeconedToMin(miliSecondval) {

    var date = new Date(miliSecondval * 1000);
    var hours = String(date.getUTCHours()).padStart(2, '0');
    var minutes = String(date.getUTCMinutes()).padStart(2, '0');
    var formattedTime = hours + ':' + minutes;

    return formattedTime;
}

function SetScrollBottom() {
    var scrollingDiv = $("#messages");
    var scrollHeight = scrollingDiv[0].scrollHeight;
    scrollingDiv.scrollTop(scrollHeight);
}

function UserAppointmentTimer() {



    sessionStorage.setItem("extendTime", "34");
    sessionStorage.setItem("endTime", "36");

    $("#exTime").html(sessionStorage.getItem("extendTime"));
    $("#enTime").html(sessionStorage.getItem("endTime"));

    var minutes = new Date().getMinutes();
    var extendTime = sessionStorage.getItem("extendTime");

    var seconds = new Date().getSeconds();;
    var timerInterval = setInterval(function () {

        seconds++;
        if (seconds >= 60) {
            seconds = 0;
            minutes++;
        }

        var formattedTime = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
        $("#runningTimer").text(formattedTime);

        if (formattedTime === sessionStorage.getItem("extendTime") + ":00") {  
            console.log("yes extend calling....");
            $("#callDuration").show();
        }
        if (formattedTime === sessionStorage.getItem("endTime") + ":00") {
            $("#videoCallModal").hide();
        }

    }, 1000);
}

function ConsultantAppointmentTimer() {

    sessionStorage.setItem("extendTime", "24");
    sessionStorage.setItem("endTime", "27");


    var minutes = new Date().getMinutes();
    var extendTime = sessionStorage.getItem("extendTime");

    var seconds = new Date().getSeconds();;
    var timerInterval = setInterval(function () {

        seconds++;
        if (seconds >= 60) {
            seconds = 0;
            minutes++;
        } 

        var formattedTime = (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
        $("#runningTimer").text(formattedTime);

        if (formattedTime === sessionStorage.getItem("endTime") + ":00") {
            $("#videoCallModal").hide();
        }

    }, 1000);
}

function ExtendAppointmentTime() {

    var callExtendCnt = sessionStorage.getItem("extendTimeCnt");

    if (callExtendCnt == "" || callExtendCnt == null || callExtendCnt == undefined) {
        callExtendCnt = 1;
        sessionStorage.setItem("extendTimeCnt", "1");
    }
    else {
        sessionStorage.setItem("extendTimeCnt", "2");
        callExtendCnt = sessionStorage.getItem("extendTimeCnt");
    }

    if (callExtendCnt == "1" || callExtendCnt == "2") {

        $.ajax({
            type: "post",
            url: "/Home/UserExtendCall",            
            success: function (response) {
                if (response == "true") {
                    HideCallDuration();
                }
            },
            error: function () {
                alert("Error occured!!")
            }
        });

        var extendTimeVal = parseInt(sessionStorage.getItem("extendTime"));
        var afterExtend = extendTimeVal + 2;

        sessionStorage.setItem("extendTime", afterExtend);

        var endTimeVal = parseInt(sessionStorage.getItem("endTime"));
        var afterEnd = endTimeVal + 2;

        sessionStorage.setItem("endTime", afterEnd);

        $("#exTime").html(sessionStorage.getItem("extendTime"));
        $("#enTime").html(sessionStorage.getItem("endTime"));
        $("#exCount").html(sessionStorage.getItem("extendTimeCnt"));

    }
}

function HideCallDuration() {
    $("#callDuration").hide();
}

function AddSessionInAppointment(AppointmentIdVal, SessionIdVal) {

    let objPara = {
        AppointmentId: AppointmentIdVal,
        SessionId: SessionIdVal
    }
    $.ajax({
        type: "post",
        url: "/Consultant/ConsultantSlot/AddSessionInAppointment",
        data: objPara,
        success: function (response) {
            console.log(response);
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

function UserPresentInCall(SessionIdVal) {
    let objPara = {
        SessionId: SessionIdVal
    }
    $.ajax({
        type: "post",
        url: "/Home/UserPresentInCall",
        data: objPara,
        success: function (response) {
            console.log(response);
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

function AppointmentCallEnded(SessionIdVal) {

    console.log("calling endded start");

    let objPara = {
        SessionId: SessionIdVal
    }
    
    $.ajax({
        type: "post",
        url: "/Consultant/ConsultantSlot/AppointmentCallEnded",
        data: objPara,
        success: function (response) {
            //if (response == "true") {
            //    alert("do you want to retain ?");
            //}


            console.log("calling endded start");
            alert("ended");
            var url = "/Consultant/ConsultantHome/Index";
            window.location.href = url;
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

function AppointmentCallEndedUser(SessionIdVal) {
    let objPara = {
        SessionId: SessionIdVal
    }

    $.ajax({
        type: "post",
        url: "/Home/AppointmentCallEndedUser",
        data: objPara,
        success: function (response) {
            
            var stringArray = response.split(',');
            var strId = stringArray[0];
            var strName = stringArray[1] + " " + stringArray[2];           
            var strProfilePic = stringArray[3];
            var strLanguageName = stringArray[4];
            var strServiceName = stringArray[5];

            $("#rName").html(strName);
            $("#rLanguageName").html(strLanguageName);
            $("#rServiceName").html(strServiceName);            
            $('#rProfilePic').attr('src', strProfilePic);

            $("#retainCidDiv").append('<button type="button" class="g-btn mr-20" onclick="RetainConsultant('+strId+')">Yes, Retain Consultant</button>');

        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

function AppointmentChatEndedUser(SessionIdVal) {
    let objPara = {
        CUid: SessionIdVal
    }

    $.ajax({
        type: "post",
        url: "/Home/AppointmentChatEndedUser",
        data: objPara,
        success: function (response) {


            debugger

            var stringArray = response.split(',');
            var strId = stringArray[0];
            var strName = stringArray[1] + " " + stringArray[2];
            var strProfilePic = stringArray[3];
            var strLanguageName = stringArray[4];
            var strServiceName = stringArray[5];

            $("#rName").html(strName);
            $("#rLanguageName").html(strLanguageName);
            $("#rServiceName").html(strServiceName);
            $('#rProfilePic').attr('src', strProfilePic);

            $("#retainCidDiv").append('<button type="button" class="g-btn mr-20" onclick="RetainConsultant(' + strId + ')">Yes, Retain Consultant</button>');

        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

function RetainConsultant(cId) {


    sessionStorage.setItem("retainCid", cId);


    let paraObj = {
        cId: cId
    }
    $.ajax({
        type: "post",
        url: "/Retain/UserRetainToConsultant",
        data: paraObj,
        success: function (response) {

            var url = "";
            if (response == "retainuser") {
                url = "/Retain/RetainUser";
            }
            else {
                url = "/Retain/GuestEditForRetain";
            }
            window.location.href = url;
        },
        error: function () {
            alert("Error occured!!")
        }
    });


    
}

function RetainClosePopup() {
    sessionStorage.setItem("retainCid", "");
    $("#retainConsultant").hide();
    var url = "/Home/UserHomeIndex";
    window.location.href = url;
}

//$(".FavouriteConsultant").on("click", handleFavouriteConsultantClick);

function handleFavouriteConsultantClick(event) {
    //debugger;

    // Your existing click event handling code for ".NotFavouriteConsultant" class
    event.preventDefault();
    var clickedElement = $(event.target);    
    var ConsultantId = clickedElement.data('model-value');   
    var currentImageSrc = clickedElement.attr("src");

    //clickedElement.off("click", handleFavouriteConsultantClick);
    $.ajax({
        type: "POST",
        url: "/Home/RemoveFavouriteConsultant",
        data: { "ConsultantId": ConsultantId },
        success: function (response) {
            //debugger;
            if (response != 0) {
                if (currentImageSrc.includes("Heart.svg")) {
                    clickedElement.removeClass("FavouriteConsultant").addClass("NotFavouriteConsultant");
                    clickedElement.attr("src", "/assets/images/NotFavourite.svg");

                    //clickedElement.on("click", handleNotFavouriteConsultantClick);
                }
                //clickedElement.attr("src", "/assets/images/NotFavourite.svg");
            }
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

function handleNotFavouriteConsultantClick(event) {
    //debugger;
    // Your existing click event handling code for ".NotFavouriteConsultant" class
    event.preventDefault();
    var clickedElement = $(event.target);    
    var ConsultantId = clickedElement.data('model-value');    
    var currentImageSrc = clickedElement.attr("src");
    //clickedElement.off("click", handleNotFavouriteConsultantClick);
    $.ajax({
        type: "POST",
        url: "/Home/AddFavouriteConsultant",
        data: { "ConsultantId": ConsultantId },
        success: function (response) {
            //debugger;
            if (response != 0) {
                if (currentImageSrc.includes("NotFavourite.svg")) {
                    clickedElement.removeClass("NotFavouriteConsultant").addClass("FavouriteConsultant");
                    clickedElement.attr("src", "/assets/images/Heart.svg");

                    //clickedElement.on("click", handleFavouriteConsultantClick);
                }
                //clickedElement.attr("src", "/assets/images/Heart.svg");
            }
        },
        error: function () {
            alert("Error occured!!")
        }
    });
}

//$(".NotFavouriteConsultant").on("click", handleNotFavouriteConsultantClick);

// Assuming ".container" is a parent element that exists in the DOM.
$('.container').on('click', '.FavouriteConsultant', function (event) {
    // Your event handling code here
    //alert("hi");

    handleFavouriteConsultantClick(event);
});

$('.container').on('click', '.NotFavouriteConsultant', function (event) {
    // Your event handling code here
    //alert("hi");
    handleNotFavouriteConsultantClick(event);
});






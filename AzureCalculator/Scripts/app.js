function setIPAddress(){
    $("#IPAddress").val(myip);
}
function loader(){
	$('body').append('<div class="overlay"><div class="three-quarters">Loading</div></div>')
}
function getAllISV(){
    $.ajax({
        cache: false,
        type: "POST" ,
        data: {},
        async: true,
        url: "/Home/AvailableISV/",
        datatype: "json" ,
        success: function (allISV) {
            $.each(allISV, function(key, allISV) {
                var isvName = allISV.ISVName;
                var isvURL = isvName.toLowerCase().replace(/ /g, '');
                var url = "/" + isvName.toLowerCase().replace(/ /g, '');
                $('<li><a href="' + url + '"><h3>' + isvName + '</h3></a></li>').appendTo($('#ISVListing'));
                $("#ISVListing li:first").addClass('active');
            });

            var firstISVURL = $("#ISVListing li:first a").attr('href');
            console.log(firstISVURL);
            $("#ISVDrives").attr('src', 'http://'+window.location.host+firstISVURL);

            $("#ISVListing li a").click(function(){
                $("#ISVListing li").removeClass('active')
                var isvURL = $(this).attr('href');
                var url = 'http://'+window.location.host+ isvURL;
                var parentHeight = $('#ISVTestDriveListing').height($(document).height());
                console.log(isvURL);
                $("#ISVDrives").attr('src', url);
                $(this).parent('li').addClass('active');
                return false;
            });
            $('.overlay').fadeOut();
        }

    });    
}

function setLogosSetUp(id){
    $.ajax({
        cache: false,
        type: "POST",
        data: {},
        async: true,
        url: "/Home/TestDrive/" + id,
        dataType: "json",
        success: function (testDrive) {
			$("#testDriveLogo").attr("src", testDrive.TestDriveIcon).attr('alt', testDrive.TestDriveName);
            $("#testDriveLogo").click(function () {
                if (testDrive.TestDriveSiteURL.length) {
                        window.open(testDrive.TestDriveSiteURL, '_blank');
                } else{
                        return false;
                };
            });
            $("#header").fadeIn();
            $("#footer").fadeIn();
            $("#mainRow").addClass('main-row');
            setISVDetail(testDrive.ISVId);
           
        }
    });	
}
function renderTestDriveDetails(id) {
    $.ajax({
        cache: false,
        type: "POST",
        data: {},
        async: true,
        url: "/Home/TestDrive/" + id,
        dataType: "json",
        success: function (testDrive) {
            $("#divIntroductionHTML").html(testDrive.IntroductionHTML);
            $("#divLabInfo1").html(testDrive.LabInfo1);
            $("#divLabInfo2").html(testDrive.LabInfo2);
            $("#divProgressDetails").html(testDrive.ProgressDetails);
            $("#divMailDetails").html(testDrive.MailDetails);
            $("#btnSubmit").html(testDrive.SubmitCaption);
            document.title = testDrive.TestDriveTitle;
            $("#testDriveLogo").attr("src", testDrive.TestDriveIcon)
                               .attr('alt', testDrive.TestDriveName)
                               .attr('title', testDrive.TestDriveName);
            $("#testDriveLogo").click(function () {
                if (testDrive.TestDriveSiteURL.length) {
                        window.open(testDrive.TestDriveSiteURL, '_blank');
                } else{
                        return false;
                };
            });

            $("#divIntroductionHTML").fadeIn();
            $("#divForm").fadeIn();
            $("#divHowItWorks").fadeIn();
            $("#header").fadeIn();
            $("#mainRow").addClass('main-row');
            setISVDetail(testDrive.ISVId);
            $('.overlay').fadeOut();
        }
    });
}



function renderISV(isvId){
    $.ajax({
        cache: false,
        type: "POST" ,
        data: {},
        async: true,
        url: "/Home/ISV/" + isvId,
        datatype: "json" ,
        success: function (isv) {
            renderTestDrives(isv.ISVId, isv.ISVName);
            $("#imgLogo").attr('src', isv.ISVIcon);
            $("#ISVLink").attr("href", isv.ISVSiteURL);
            $("#ISVName").html(isv.ISVName + ' Test Drives');
            document.title = isv.ISVTitle;
            $("#ISVDescription").html(isv.ISVDescription);
            $("#header").css('background',isv.HeaderBackground).fadeIn();
            $("#main").fadeIn();
            $("#footer").fadeIn();
            setTimeout(function(){
            	$('.overlay').fadeOut();
            }, 1000);
        }
    });
}

function renderTestDrives(isvId, isvName) {

    isvName = isvName.toLowerCase().replace(/ /g, '');
    $.ajax({
        cache: false,
        type: "POST" ,
        data: {},
        async: true,
        url: "/Home/AvailableDrives/" + isvId,
        datatype: "json" ,
        success: function (result) {
            function formatItem(testDrive) {
                var testDriveLink = testDrive.TestDriveName;
                testDriveLink = testDriveLink.toLowerCase().replace(/ /g, '');
                var url = "/" + isvName + "/" + testDrive.TestDriveName.toLowerCase().replace(/ /g, '');
                return '<div class="item">' +
                        '<a target="_blank" href="' + url + '">' +
                        '<span class="h4">' + testDrive.TestDriveName + '</span>' +
                        '<img src="' + testDrive.TestDriveIcon + '" alt="' + testDrive.Tooltip + '">' +
                        '<span>Test Drive<span class="glyphicon glyphicon-play"></span>' +
                        '</span></a></div>';
            }
            $.each(result, function (key, testDrive) {
                $('<div>',
                    {
                        html: formatItem(testDrive)
                    },
                    '</div>').addClass('col-xs-3').appendTo($('#createdDrives'));
            });

        }
    });
}

function setISVDetail(id){
    $.ajax({
        cache: false,
        type: "POST",
        data: {},
        async: true,
        url: "/Home/ISV/" + id,
        dataType: "json",
        success: function (isv) {
            $("#imgLogo").attr("src", isv.ISVIcon);
            $("#ISVLink").attr("href", isv.ISVSiteURL);
            $("#imgLogo").fadeIn();
            $("#divAbout").html(isv.ISVContact);
            $("#header").css('background',isv.HeaderBackground);
            $('.overlay').fadeOut();
            delayInstallation(10);
        }
    });
}

function triggerShowHide() {
    $('.triggerBtn').click(function(event) {
        $(this).parent('.row').toggleClass('active');
        $(this).toggleClass('glyphicon-chevron-right');
        return false;
    });
}
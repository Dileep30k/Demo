$(document).ready(function () {

    renderControls();

    //if (typeof window.name != undefined) {
    //    if (window.name == '') {
    //        window.name = generateUUID();
    //        window.location.href = '/Logout';
    //    }
    //}

    //$('html').on("cut copy paste", function (e) {
    //    e.preventDefault();
    //});

    //$("html").on("contextmenu", function () {
    //    return false;
    //});

    //$('html').on('keypress', function (event) {
    //    var regex = new RegExp("^[a-zA-Z0-9 :,.@!#$%^&*()_-]+$");
    //    var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
    //    if (!regex.test(key)) {
    //        event.preventDefault();
    //        return false;
    //    }
    //});

    $('#btn-excel-export').on('click', function () {
        $('.buttons-excel').click()
    });

    $('#sidebarCollapse').on('click', function () {
        $('#sidebar').toggleClass('active');
    });

    $(".rotate").click(function () {
        $(this).toggleClass("down");
    });

});


$(window).on('load', function () {
    hideLoading();
})

function showLoading() {
    $('.loading').show();
}

function hideLoading() {
    $('.loading').hide();
}

function displayModel(modelId) {
    $(`#${modelId}`).modal("show");
}

function hideModel(modelId) {
    $(`#${modelId}`).modal("hide");
}

function createCookie(name, value, hours) {
    if (hours) {
        var date = new Date();
        date.setTime(date.getTime() + (hours * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";

    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

function isValidEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

function renderControls() {
    $(".select2").select2();
    $(".date-picker").datepicker({
        dateFormat: 'dd-mm-yy',
    });

    $(".date-picker").each(function (i, obj) {
        var date = $(this).val();
        if (date) {
            var datePart = date.split(' ');
            if (datePart.length > 0) {
                if (datePart[0] === '01-01-0001') {
                    $(this).val('');
                }
                else {
                    $(this).val(datePart[0]);
                }
            }
        }
    });
    $('.select2').bind('change', function () {
        $(this).trigger('blur');
    });
    $('.txtOnly').keypress(function (e) {
        var regex = new RegExp("^[a-zA-Z ]*$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }
        else {
            e.preventDefault();
            return false;
        }
    });

    $('.numberOnly').keypress(function (e) {
        var regex = new RegExp("^[0-9]*$");
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }
        else {
            e.preventDefault();
            return false;
        }
    });

    renderNumberOnly();
    renderDecimalOnly();
}

function renderNumberOnly() {
    $('.numberOnly').blur(function (e) {
        $(this).val($(this).val().replace(/\D/g, ''));
    });

}

function renderDecimalOnly() {
    $('.decimalOnly').keypress(function (e) {
        var regex = new RegExp(/^\d*\.?\d*$/);
        var str = String.fromCharCode(!e.charCode ? e.which : e.charCode);
        if (regex.test(str)) {
            return true;
        }
        else {
            e.preventDefault();
            return false;
        }
    });
}

function getFormData($form) {
    var unindexed_array = $form.serializeArray();
    var indexed_array = {};

    $.map(unindexed_array, function (n, i) {
        if (n['name'].includes('.')) {
            var splitArray = n['name'].split('.');
            var child = indexed_array[splitArray[0]] || {};
            child[[splitArray[1]]] = n['value'];
            indexed_array[splitArray[0]] = child;
        } else {
            if (!indexed_array[n['name']]) {
                indexed_array[n['name']] = n['value'];
            }
        }
    });

    return indexed_array;
}

function getFormatDate(date) {
    if (!date || date == '') return '';
    return moment(date).format('DD-MM-YYYY');
}

function getFormatDateTime(date) {
    if (!date || date == '') return '';
    return moment(date).format('DD-MM-YYYY hh:mm A');
}

function resizeDataTable() {
    setTimeout(function () {
        $($.fn.dataTable.tables(true)).DataTable().columns.adjust().draw();
    }, 200);
}

function generateUUID() { // Public Domain/MIT
    var d = new Date().getTime();//Timestamp
    var d2 = ((typeof performance !== 'undefined') && performance.now && (performance.now() * 1000)) || 0;//Time in microseconds since page-load or 0 if unsupported
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16;//random number between 0 and 16
        if (d > 0) {//Use timestamp until depleted
            r = (d + r) % 16 | 0;
            d = Math.floor(d / 16);
        } else {//Use microseconds since page-load if supported
            r = (d2 + r) % 16 | 0;
            d2 = Math.floor(d2 / 16);
        }
        return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
    });
}

function openNav() {
    $("#sidebar").css({
        "width": "299px",
        "marginLeft": "-27px",
    });

    resizeDataTable();
}

function closeNav() {
    $("#sidebar").css({
        "width": "0",
        "marginLeft": "-40px",
    });
    $("#main").css({

        "marginLeft": "4px",
    });

    resizeDataTable();
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}
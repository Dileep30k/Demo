$(document).ready(function () {
    $('#MsilUserId').val(null);
    $('#MsilUserId').focus();
});

function validateLogin() {
    if ($("#formLogin").valid()) {
        showLoading();
        return true;
    }
    return false;
}

function resetLogin() {
    window.location.href = window.location.href;
}
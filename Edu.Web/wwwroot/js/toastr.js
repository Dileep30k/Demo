toastr.options = {
    "closeButton": true,
    "debug": false,
    "newestOnTop": true,
    "progressBar": true,
    "positionClass": "toast-top-right",
    "preventDuplicates": true,
    "onclick": null,
    "showDuration": "100",
    "hideDuration": "3000",
    "timeOut": "3000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut",
}

function displayToastr(msg, title, type) {
    if (type == "Info")
        toastr.info(msg)
    else if (type == "Warning")
        toastr.warning(msg)
    else if (type == "Success")
        toastr.success(msg, title)
    else if (type == "Error")
        toastr.error(msg, title)
}
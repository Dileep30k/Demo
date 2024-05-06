const nominationStatuses = {
    Nominated: 1,
    Submitted: 2,
    Accepted: 3,
    Rejected: 4,
    DepApprove: 5,
    DepReview: 6,
    DepRejected: 7,
    DivApprove: 8,
    DivReview: 9,
    DivRejected: 10,
    Approved: 11,
    Waiting: 12,
    Confirmed: 13,
};

function getNominationStatusColor(statusId) {
    switch (statusId) {
        case nominationStatuses.Nominated: return "#808080";
        case nominationStatuses.Submitted: return "transparent";
        case nominationStatuses.Accepted: return "#01458E";
        case nominationStatuses.Rejected: return "#C60000";
        case nominationStatuses.DepApprove: return "#01458E";
        case nominationStatuses.DepReview: return "#EC7100";
        case nominationStatuses.DepRejected: return "#C60000";
        case nominationStatuses.DivApprove: return "#04921A";
        case nominationStatuses.DivReview: return "#90C7BA";
        case nominationStatuses.DivRejected: return "#C60000";
        case nominationStatuses.Approved: return "#0CB077";
        case nominationStatuses.Waiting: return "#FECB00";
        case nominationStatuses.Confirmed: return "#4D10BD";
        default: return "#000000";
    }
}

function getNominationStatusText(statusId) {
    switch (statusId) {
        case nominationStatuses.DepApprove:
            return "2";
        case nominationStatuses.DepReview:
            return "";
        case nominationStatuses.DepRejected:
            return "A";
        case nominationStatuses.DivApprove:
            return "";
        case nominationStatuses.DivReview:
            return "";
        case nominationStatuses.DivRejected:
            return "A";
        case nominationStatuses.Accepted:
            return "1";
        case nominationStatuses.Rejected:
            return "E";
        default: return '';
    }
}
function getNominationStatusTextHover(statusId) {
    switch (statusId) {
        case nominationStatuses.DepApprove:
            return "Pending at Approver 2";
        case nominationStatuses.DepReview:
            return "Department Review";
        case nominationStatuses.DepRejected:
            return "Department Rejected";
        case nominationStatuses.DivApprove:
            return "Approved";
        case nominationStatuses.DivReview:
            return "Division Review";
        case nominationStatuses.DivRejected:
            return "Division Rejected";
        case nominationStatuses.Accepted:
            return "Pending at Approver 1";
        case nominationStatuses.Rejected:
            return "Rejected";
        default: return '-';
    }
}

const admissionStatuses = {
    Confirm: 1,
    Waiting: 2,
    Active: 3,
    Left: 4,
    Pause: 5,
    Accepted: 6,
    Rejected: 7,
};

function getAdmissionStatusColor(statusId) {
    switch (statusId) {
        case admissionStatuses.Confirm: return "transparent";
        case admissionStatuses.Waiting: return "transparent";
        case admissionStatuses.Active: return "#04921A";
        case admissionStatuses.Left: return "#C60000";
        case admissionStatuses.Pause: return "#f39c12";
        case admissionStatuses.Accepted: return "#04921A";
        case admissionStatuses.Rejected: return "#C60000";
        default: return "#000000";
    }
}
function getAdmissionStatusText(statusId) {
    switch (statusId) {
        case admissionStatuses.Waiting:
            return "W";
        case admissionStatuses.Rejected:
            return "E";
        case admissionStatuses.Accepted:
            return "E";
        default: return '';
    }
}

function getAdmissionStatusTextColor(statusId) {
    switch (statusId) {
        case admissionStatuses.Waiting:
            return "black";
        default: return 'white';
    }
}
function getAdmissionStatusTextHover(statusId) {
    console.log(admissionStatuses)
    switch (statusId) {
        case admissionStatuses.Waiting:
            return "Waiting";
        case admissionStatuses.Rejected:
            return "Rejected";
        case admissionStatuses.Accepted:
            return "Accepted";
        case admissionStatuses.Active:
            return "Active";
        case admissionStatuses.Left:
            return "Left";
        case admissionStatuses.Pause:
            return "Pause";
        default: return '';
    }
}
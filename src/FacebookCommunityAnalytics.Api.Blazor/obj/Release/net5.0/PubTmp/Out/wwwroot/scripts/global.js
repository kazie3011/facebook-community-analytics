function redirectToUrl(redirectUrl) {
    window.open(redirectUrl, '_blank');
}

function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

window.timeZoneOffsetInHours = () => {
    return new Date().getTimezoneOffset() / 60;
};

function timeZoneOffsetInMinutes() {
    return new Date().getTimezoneOffset();
}

window.logDebug = (value) => {
    console.log(value);
}
$(document).ready(function () {
    $(document).on("select2:open", () => {
        document.querySelector(".select2-search__field").focus();
    });

    $(".select2").select2({
        theme: 'classic'
    });
});

/* eslint-disable */
function EncryptData(data) {
    var key = CryptoJS.enc.Utf8.parse('8080808080808080');
    var iv = CryptoJS.enc.Utf8.parse('8080808080808080');
    var encrypteddata = CryptoJS.AES.encrypt(CryptoJS.enc.Utf8.parse(data), key, { keySize: 128 / 8, iv: iv, mode: CryptoJS.mode.CBC, padding: CryptoJS.pad.Pkcs7 });
    return String(encrypteddata).replace(/\+/g, "ç");
}

function Loading(state) {
    if (state === 'show' || state === 'SHOW') {
        $.blockUI({
            message: '<div class="spinner-border text-white" role="status"></div>',
            css: {
                backgroundColor: 'transparent',
                border: '0',
                zIndex: '99999999999999999'
            },
            overlayCSS: {
                opacity: 0.5
            },
            baseZ: 9999999999
        });
    }
    if (state === 'hide' || state === 'HIDE') {
        $.unblockUI();
    }
}

var a = ['', 'one ', 'two ', 'three ', 'four ', 'five ', 'six ', 'seven ', 'eight ', 'nine ', 'ten ', 'eleven ', 'twelve ', 'thirteen ', 'fourteen ', 'fifteen ', 'sixteen ', 'seventeen ', 'eighteen ', 'nineteen '];
var b = ['', '', 'twenty', 'thirty', 'forty', 'fifty', 'sixty', 'seventy', 'eighty', 'ninety'];
function inWords(n) {
    var nums = n.toString().split('.');
    var whole = convertNumberToWords(nums[0]);
    if (nums.length === 2) {
        var fraction = convertFractionToWords(nums[1]);
        if (typeof fraction === "undefined")
            return whole + ' only';
        return whole + 'point ' + fraction + ' only';
    } else {
        return whole + ' only';
    }
}
function convertNumberToWords(num) {
    if ((num = num.toString()).length > 9) return 'overflow';
    n = ('000000000' + num).substr(-9).match(/^(\d{2})(\d{2})(\d{2})(\d{1})(\d{2})$/);
    if (!n) return;
    var str = "";
    str += (n[1] != 0) ? (a[Number(n[1])] || b[n[1][0]] + ' ' + a[n[1][1]]) + 'crore ' : '';
    str += (n[2] != 0) ? (a[Number(n[2])] || b[n[2][0]] + ' ' + a[n[2][1]]) + 'lakh ' : '';
    str += (n[3] != 0) ? (a[Number(n[3])] || b[n[3][0]] + ' ' + a[n[3][1]]) + 'thousand ' : '';
    str += (n[4] != 0) ? (a[Number(n[4])] || b[n[4][0]] + ' ' + a[n[4][1]]) + 'hundred ' : '';
    str += (n[5] != 0) ? ((str != '') ? 'and ' : '') + (a[Number(n[5])] || b[n[5][0]] + ' ' + a[n[5][1]]) + ' ' : '';
    return str;
}
function convertFractionToWords(num) {
    if ((num = num.toString()).length > 9) return '';
    n = num.substr(0, 2);
    if (!n) return;
    var str = '';
    str += ((str != '') ? 'and ' : '') + (a[Number(n)] || b[n[0][0]] + ' ' + a[n[1][0]]) + ' ';
    return str;
}
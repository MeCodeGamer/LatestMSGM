$(document).ready(function () {
    $('.CnicMask').mask('00000-0000000-0');

    $('.PhoneMask').mask('0000-0000-0000-0000'); // You can adjust the number of placeholders as needed


    $('.CnicMask').blur(function (event) {
        if ($(this).val().length > 0) {
            var cnic_no = $(this).val(),
                cnic_no_regex = /^[0-9+]{5}-[0-9+]{7}-[0-9]{1}$/;

            if (!cnic_no_regex.test(cnic_no)) {
                swal.fire('Error!', "Your entered an invalid CNIC number.", 'error');
                $('#txtCNIC').focus();
                return false;
            }
        }
    });
    $('.CharOnly').mask('AAAAAAAAAAAAAAAAAAAAAAAAA', {
        translation: {
            'A': { pattern: /[A-Za-z ]/ }, // Allow letters and space
        },
    });




    //$('.validatePassword').on('keyup', function () {
    //    validatePassword($(this).val());
    //});
    $('.validatePassword').on('input', function () {
        const password = $('#Password').val();
          validatePassword(password);
    });
    $('.validateConfirmPassword').on('input', function () {
        const password = $('#Password').val();
        const confirmPassword = $('#ConfirmPassword').val();
        checkPasswordMatch(password, confirmPassword);
    });
    function validatePassword(password) {
        const requirementsSpan = $('#password-requirements');
        const requirementsText = [];

        // Check if the password contains 1 uppercase, 1 lowercase, and 1 number
        const hasUppercase = /[A-Z]/.test(password);
        const hasLowercase = /[a-z]/.test(password);
        const hasNumber = /[0-9]/.test(password);

        if (hasUppercase && hasLowercase && hasNumber) {
            requirementsSpan.text('Password Strong');
            requirementsSpan.css('color', 'green');
        } else {
            requirementsText.push('Password must meet the following criteria:');
            if (!hasUppercase) {
                requirementsText.push('At least one uppercase letter');
            }
            if (!hasLowercase) {
                requirementsText.push('At least one lowercase letter');
            }
            if (!hasNumber) {
                requirementsText.push('At least one number');
            }

            requirementsSpan.text(requirementsText.join(' '));
            requirementsSpan.css('color', 'red');
        }
    }

    function checkPasswordMatch(password, confirmPassword) {
        const matchSpan = $('#password-match');
        
        if (password === confirmPassword) {
            matchSpan.text('Passwords Match');
            matchSpan.css('color', 'green');
        } else {
            matchSpan.text('Passwords Do Not Match');
            matchSpan.css('color', 'red');
        }
    }
});
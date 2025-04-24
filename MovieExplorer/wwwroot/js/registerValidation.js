$(document).ready(function () {
    $("#registerForm").validate({
        rules: {
            Email: {
                required: true,
                email: true
            },
            UserName: {
                required: true,
                minlength: 3,
                maxlemgth:15
            },
            Password: {
                required: true,
                minlength: 6
            },
            ConfirmPassword: {
                required: true,
                equalTo: "#Password"
            }
        },
        messages: {
            Email: {
                required: "Email is required.",
                email: "Please enter a valid email address."
            },
            UserName: {
                required: "Username is required.",
                minlength: "Username must be at least 3 characters long.",
                maxlength: "Username cannot be longer than 15 characters."
            },
            Password: {
                required: "Password is required.",
                minlength: "Password must be at least 6 characters long."
            },
            ConfirmPassword: {
                required: "Please confirm your password.",
                equalTo: "Passwords do not match."
            }
        },
        errorElement: "span",
        errorClass: "text-danger",
        errorPlacement: function (error, element) {
            error.insertAfter(element);
        }
    });
});
"use strict";

var KTSignupGeneral = (function () {
    var form, submitBtn, validator, passwordMeter;

    // Password strength check
    var isPasswordStrong = function () {
        return passwordMeter.getScore() > 50;
    };

    return {
        init: function () {
            form = document.querySelector("#kt_sign_up_form");
            submitBtn = document.querySelector("#kt_sign_up_submit");
            passwordMeter = KTPasswordMeter.getInstance(
                form.querySelector('[data-kt-password-meter="true"]')
            );

            // Helper to check if form action is a valid URL
            var isUrl = function (url) {
                try {
                    return new URL(url), true;
                } catch (e) {
                    return false;
                }
            };

            // If form action is not a valid URL, use local validation
            if (!isUrl(submitBtn.closest("form").getAttribute("action"))) {
                validator = FormValidation.formValidation(form, {
                    fields: {
                        "user-name": {
                            validators: {
                                notEmpty: { message: "User Name is required" }
                            }
                        },
                        email: {
                            validators: {
                                regexp: {
                                    regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                    message: "The value is not a valid email address"
                                },
                                notEmpty: { message: "Email address is required" }
                            }
                        },
                        password: {
                            validators: {
                                notEmpty: { message: "The password is required" },
                                callback: {
                                    message: "Please enter valid password",
                                    callback: function (input) {
                                        if (input.value.length > 0) return isPasswordStrong();
                                    }
                                }
                            }
                        },
                        "confirm-password": {
                            validators: {
                                notEmpty: { message: "The password confirmation is required" },
                                identical: {
                                    compare: function () {
                                        return form.querySelector('[name="password"]').value;
                                    },
                                    message: "The password and its confirm are not the same"
                                }
                            }
                        },
                        toc: {
                            validators: {
                                notEmpty: { message: "You must accept the terms and conditions" }
                            }
                        },
                        pp: {
                            validators: {
                                notEmpty: { message: "You must accept the Privacy Policy" }
                            }
                        }
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger({ event: { password: false } }),
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: ".fv-row",
                            eleInvalidClass: "",
                            eleValidClass: ""
                        })
                    }
                });

                submitBtn.addEventListener("click", function (e) {
                    e.preventDefault();
                    validator.revalidateField("password");
                    validator.validate().then(function (status) {
                        if (status === "Valid") {
                            submitBtn.setAttribute("data-kt-indicator", "on");
                            submitBtn.disabled = true;
                            setTimeout(function () {
                                submitBtn.removeAttribute("data-kt-indicator");
                                submitBtn.disabled = false;
                                Swal.fire({
                                    text: "You have successfully Create Your Account!",
                                    icon: "success",
                                    buttonsStyling: false,
                                    confirmButtonText: "Ok, got it!",
                                    customClass: { confirmButton: "btn btn-primary" }
                                }).then(function (result) {
                                    if (result.isConfirmed) {
                                        //form.reset();
                                        passwordMeter.reset();
                                        
                                    }
                                });
                            }, 1500);
                        } else {
                            Swal.fire({
                                text: "Sorry, looks like there are some errors detected, please try again.",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Ok, got it!",
                                customClass: { confirmButton: "btn btn-primary" }
                            });
                        }
                    });
                });

                form.querySelector('input[name="password"]').addEventListener("input", function () {
                    if (this.value.length > 0) {
                        validator.updateFieldStatus("password", "NotValidated");
                    }
                });
            } else {
                // If form action is a valid URL, use AJAX
                validator = FormValidation.formValidation(form, {
                    fields: {
                        name: {
                            validators: {
                                notEmpty: { message: "Name is required" }
                            }
                        },
                        email: {
                            validators: {
                                regexp: {
                                    regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                    message: "The value is not a valid email address"
                                },
                                notEmpty: { message: "Email address is required" }
                            }
                        },
                        password: {
                            validators: {
                                notEmpty: { message: "The password is required" },
                                callback: {
                                    message: "Please enter valid password",
                                    callback: function (input) {
                                        if (input.value.length > 0) return isPasswordStrong();
                                    }
                                }
                            }
                        },
                        password_confirmation: {
                            validators: {
                                notEmpty: { message: "The password confirmation is required" },
                                identical: {
                                    compare: function () {
                                        return form.querySelector('[name="password"]').value;
                                    },
                                    message: "The password and its confirm are not the same"
                                }
                            }
                        },
                        toc: {
                            validators: {
                                notEmpty: { message: "You must accept the terms and conditions" }
                            }
                        }
                    },
                    plugins: {
                        trigger: new FormValidation.plugins.Trigger({ event: { password: false } }),
                        bootstrap: new FormValidation.plugins.Bootstrap5({
                            rowSelector: ".fv-row",
                            eleInvalidClass: "",
                            eleValidClass: ""
                        })
                    }
                });

                submitBtn.addEventListener("click", function (e) {
                    e.preventDefault();
                    validator.revalidateField("password");
                    validator.validate().then(function (status) {
                        if (status === "Valid") {
                            submitBtn.setAttribute("data-kt-indicator", "on");
                            submitBtn.disabled = true;
                            axios
                                .post(submitBtn.closest("form").getAttribute("action"), new FormData(form))
                                .then(function (response) {
                                    if (response) {
                                        //form.reset();
                                        const redirectUrl = form.getAttribute("data-kt-redirect-url");
                                        if (redirectUrl) location.href = redirectUrl;
                                    } else {
                                        Swal.fire({
                                            text: "Sorry, looks like there are some errors detected, please try again.",
                                            icon: "error",
                                            buttonsStyling: false,
                                            confirmButtonText: "Ok, got it!",
                                            customClass: { confirmButton: "btn btn-primary" }
                                        });
                                    }
                                })
                                .catch(function () {
                                    Swal.fire({
                                        text: "Sorry, looks like there are some errors detected, please try again.",
                                        icon: "error",
                                        buttonsStyling: false,
                                        confirmButtonText: "Ok, got it!",
                                        customClass: { confirmButton: "btn btn-primary" }
                                    });
                                })
                                .then(function () {
                                    submitBtn.removeAttribute("data-kt-indicator");
                                    submitBtn.disabled = false;
                                });
                        } else {
                            Swal.fire({
                                text: "Sorry, looks like there are some errors detected, please try again.",
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Ok, got it!",
                                customClass: { confirmButton: "btn btn-primary" }
                            });
                        }
                    });
                });

                form.querySelector('input[name="password"]').addEventListener("input", function () {
                    if (this.value.length > 0) {
                        validator.updateFieldStatus("password", "NotValidated");
                    }
                });
            }
        }
    };
})();

KTUtil.onDOMContentLoaded(function () {
    KTSignupGeneral.init();
});

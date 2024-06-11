/******************************************** Loader *****************************************************/
function showLoader() {
    var loaderContainer = document.querySelector(".loader-container");
    var backdrop = document.querySelector(".backdrop");
    if (loaderContainer && backdrop) {
        loaderContainer.style.display = "flex";
        backdrop.style.display = "flex";
    }
}
function hideLoader() {
    var loaderContainer = document.querySelector(".loader-container");
    var backdrop = document.querySelector(".backdrop");
    if (loaderContainer && backdrop) {
        loaderContainer.style.display = "none";
        backdrop.style.display = "none";
    }
}

hideLoader();

var searchvalue = "";
/******************************************** Filter Table *****************************************************/
function SearchInTable() {
    searchValue = $('#searchValue').val();
    var PageNumber = $('#PageNumber').val();
    var PageSize = 10;
    $('#SearchValue').val(searchValue);
    GetCustomerData();
}
/******************************************** Check aCcODE code at ADD CUSTOMER  *****************************************************/
function CheckAcCode() {
    var acCode = $('#floatingAcCode').val();

    $.ajax({
        url: '/CustomerList/AcCodeCheck',
        type: "post",
        async: true,
        datatype: 'html',
        cache: false,
        data: { acCode: acCode },
        success: function (result) {
            if (result === true) {
                Swal.fire({
                    title: "Sorry!",
                    text: "A\C Code Already Exists!",
                    icon: "error",
                    timer: 3000,
                    timerProgressBar: true,
                });
                $('#floatingAcCode').val('');
            }
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Check Company code at ADD CUSTOMER *****************************************************/
function CheckCompany() {
    var CompanyName = $('#floatingCompanyName').val();

    $.ajax({
        url: '/CustomerList/CompanyCheck',
        type: "post",
        async: true,
        datatype: 'html',
        cache: false,
        data: { CompanyName: CompanyName },
        success: function (result) {
            if (result === true) {
                Swal.fire({
                    title: "Sorry!",
                    text: "Company Name Already Exists!",
                    icon: "error",
                    timer: 3000,
                    timerProgressBar: true,
                });
                $('#floatingCompanyName').val('');
            }
        },
        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update AddCustomerModal *****************************************************/
function CreateCustomer() {
    event.preventDefault();

    if ($('#AddCustomerForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/CustomerList/CreateCustomer",
            data: $('#AddCustomerForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#AddCustomerModalId').modal('hide');
                if (result.success == true) {

                    var link = document.createElement('a');
                    link.href = '/CustomerList/DetailsTab?id=' + result.newId + '&pageNumber=' + 1;
                    setTimeout(function () {
                        link.click();
                    }, 900)
                }
            },
            
            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Delete Selected Customers *****************************************************/
function DeleteSelectedCustomer(acId) {
    event.preventDefault();
    console.log(acId);
    Swal.fire({
        title: 'Are you sure?',
        text: 'You want to delete this customer?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, keep it'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                method: "POST",
                url: "/CustomerList/DeleteSelectedCustomer",
                data: { acId: acId },
                traditional: true,

                success: function (result) {
                    if (result.code == 401) {
                        setTimeout(function () { location.reload(); }, 2000);
                        Swal.fire("Oops!", "Session Expired !!!", "error");
                    } else {
                        if (acId.length === 0) {
                            Swal.fire({
                                title: "Oopps!",
                                text: "Please Select Any Customer To Proceed Ahead",
                                icon: "error",
                                timer: 3000,
                                timerProgressBar: true,
                            });
                        }
                        else {
                            let timerInterval;
                            Swal.fire({
                                title: "Customer Deleting...",
                                html: "Please wait for a moment <b></b>...",
                                timer: 2000,
                                timerProgressBar: true,
                                didOpen: () => {
                                    Swal.showLoading();
                                    const timer = Swal.getPopup().querySelector("b");
                                    timerInterval = setInterval(() => {
                                        timer.textContent = `${Swal.getTimerLeft()}`;
                                    }, 1500);
                                },
                                willClose: () => {
                                    clearInterval(timerInterval);
                                }
                            }).then((result) => {
                                /* Read more about handling dismissals below */
                                if (result.dismiss === Swal.DismissReason.timer) {
                                    console.log("I was closed by the timer");
                                }
                            });

                            var link = document.createElement('a');
                            link.href = '/CustomerList/CustomerList';
                            setTimeout(function () {
                                link.click();
                            }, 1500)
                        }
                    }
                },

                error: function () {
                    Swal.fire("Oops!", "Something is Wrong !!!", "error");
                }
            });
        }
    });
}
/******************************************** Fetch EditCustomer Invoice Data *****************************************************/
function GetEditCustomerData(AcId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/CustomerList/EditCustomerData",
        data: { AcId: AcId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            } else if (result.code == 404) {
                Swal.fire({
                    title: "Sorry!",
                    text: result.message,
                    icon: "error",
                    timer: 3000,
                    timerProgressBar: true,
                });
            } else {
                $('#editInvoiceDetailForm').html(result);
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch EditCustomer Contact Data *****************************************************/
function GetEditContactCustomerData(acId, contactId) {
    showLoader();
    $.ajax({
        method: "GET",
        url: "/CustomerList/GetEditContactCustomerData",
        data: { acId: acId, contactId: contactId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            } else if (result.code == 404) {
                Swal.fire({
                    title: "Sorry!",
                    text: result.message,
                    icon: "error",
                    timer: 3000,
                    timerProgressBar: true,
                });
            } else {
                $('#editContactDetailForm').html(result);
                $('#OpenCustomerContactModalId').modal('hide');
                if (contactId != 0) {
                    EnableContact();
                }
            }
            setTimeout(function () {
                hideLoader();
            }, 300);
        },

        error: function () {
            hideLoader();
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update EditCustomer Data *****************************************************/
function EditCustomerPost(AcId) {

    var formdata = $('#EditCustomerForm').serialize();

    if ($('#EditCustomerForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/CustomerList/EditCustomerPost",
            data: formdata,

            success: function (result) {
                if (result.success == true) {
                    Swal.fire("Hurreyy", "Data updated", "error");
                }
                else {
                    Swal.fire("Oops", "Customer not found", "error");
                }
                GetEditCustomerData(AcId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Update DeleteCustomer Data *****************************************************/
function DeleteCustomer(AcId) {
    event.preventDefault();

    Swal.fire({
        title: 'Are you sure?',
        text: 'You want to delete this customer?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, keep it'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                method: "POST",
                url: "/CustomerList/DeleteSelectedCustomer",
                data: { AcId: AcId },

                success: function (result) {
                    if (result.code == 401) {
                        location.reload();
                    }
                    var link = document.createElement('a');
                    link.href = '/CustomerList/CustomerList';
                    setTimeout(function () {
                        link.click();
                    }, 1500)
                    let timerInterval;
                    Swal.fire({
                        title: "Customer Deleting",
                        html: "Please wait for a moment <b></b>...",
                        timer: 2000,
                        timerProgressBar: true,
                        didOpen: () => {
                            Swal.showLoading();
                            const timer = Swal.getPopup().querySelector("b");
                            timerInterval = setInterval(() => {
                                timer.textContent = `${Swal.getTimerLeft()}`;
                            }, 1500);
                        },
                        willClose: () => {
                            clearInterval(timerInterval);
                        }
                    }).then((result) => {
                        if (result.dismiss === Swal.DismissReason.timer) {
                            console.log("I was closed by the timer");
                        }
                    });
                },

                error: function () {
                    Swal.fire("Oops", "Something Went Wrong", "error");
                }
            });
        }
    });
}
/******************************************** Validate Edit Customer *****************************************************/
function CheckValidation() {
    var values = $('.form-control');
    var submitbtn = $('.submitBtn');

    if (values.val().trim() == "") {
        submitbtn.prop('disabled', true);
    }
    else {
        submitbtn.prop('disabled', false);
    }
}
/******************************************** Enable-Disable (invoice) *****************************************************/
function EnableInvoice() {
    document.querySelectorAll('.form-control').forEach(function (element) {
        element.disabled = false;
        $('.text-danger').removeClass("d-none");
    });
    $('#EditInvoiceIcon').addClass("d-none");
    $('.FormView').removeClass("d-none");
    $('.NormalView').addClass("d-none");
    $('.submitBtn').removeClass("d-none");
    $('.cancelBtn').removeClass("d-none");
}
function Disable() {
    document.querySelectorAll('.form-control').forEach(function (element) {
        element.disabled = true;
        $('.text-danger').removeClass("d-none");
    });
    $('#EditInvoiceIcon').removeClass("d-none");
    $('.FormView').addClass("d-none");
    $('.NormalView').removeClass("d-none");
    $('.submitBtn').addClass("d-none");
    $('.cancelBtn').addClass("d-none");
}
/******************************************** Enable-Disable (contact) *****************************************************/
function EnableContact() {
    document.querySelectorAll('.form-control').forEach(function (element) {
        element.disabled = false;
        $('.text-danger').removeClass("d-none");
    });
    $('#EditContactIcon').addClass("d-none");
    $('.ContactFormView').removeClass("d-none");
    $('.AddContactFormView').addClass("d-none");
    $('.ContactNormalView').addClass("d-none");
    $('.submitContactBtn').removeClass("d-none");
    $('.cancelContactBtn').removeClass("d-none");
}
function DisableContact() {
    document.querySelectorAll('.form-control').forEach(function (element) {
        element.disabled = true;
        $('.text-danger').removeClass("d-none");
    });
    $('#EditContactIcon').removeClass("d-none");
    $('.AddContactFormView').addClass("d-none");
    $('.ContactFormView').addClass("d-none");
    $('.ContactNormalView').removeClass("d-none");
    $('.submitContactBtn').addClass("d-none");
    $('.cancelContactBtn').addClass("d-none");
}
/******************************************** Row dbSelect *****************************************************/
$(document).on('dblclick', '.customerrow', function () {

    var acid = $(this).data('id');
    var pageNumber = $('#pageNumber').val();
    var link = document.createElement('a');
    link.href = '/CustomerList/DetailsTab?Id=' + acid + '&pageNumber=' + pageNumber;

    link.click();
});
/******************************************** Row Select & Push data-id for deletion *****************************************************/
$(document).on('click', '.customerrow', function () {
    if ($(this).hasClass('selectedrow')) {
        $(this).removeClass('selectedrow');
    }
    else {
        $(this).addClass('selectedrow');
        $('#deleteBtn').prop('disabled', false);
    }
});
/******************************************** Filter Customer Table *****************************************************/
function FilterListPost() {
    event.preventDefault();
    GetCustomerData();
}
function GetCustomerData() {

    var formdata = $('#filterListPost').serialize();

    if ($('#filterListPost').valid()) {
        $.ajax({
            method: "POST",
            url: "/CustomerList/FilterCustomersByInput",
            data: formdata,

            success: function (result) {
                if (result.code == 401) {
                    window.location.reload();
                }
                $('#requestTable').html(result);
            },
            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Clear Filter Form *****************************************************/
$('#clear_button').click(function () {

    var link = document.createElement('a');
    link.href = '/CustomerList/CustomerList';
    setTimeout(function () {
        link.click();
    }, 1)
});
/******************************************** Validate Filter Form *****************************************************/
function ValidateFilters() {
    var a = $('#searchedAcCode').val().trim();
    var b = $('#searchedCname').val().trim();
    var c = $('#searchedPcode').val().trim();
    var d = $('#searchedCountry').val().trim();
    var e = $('#searchedTelephone').val().trim();
    var f = $('#searchedRelation').val().trim();
    var g = $('#searchedCurrency').val().trim();

    if (a == "" && b == "" && c == "" && d == "" && e == "" && f == "" && g == "") {
        $('#search_button').prop('disabled', false);
        $('#clear_button').prop('disabled', true);
    }
    else {
        $('#search_button').prop('disabled', false);
        $('#clear_button').prop('disabled', false);
    }
}
/******************************************** Fetch CutomerGroups Modal *****************************************************/
function OpenCustomerGroupModal(CustomerId, callId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/CustomerList/OpenCustomerGroupModal",
        data: { CustomerId: CustomerId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showModal').html(result);
            $('#OpenCustomerGroupModalId').modal({
                backdrop: false,
                keyboard: false
            })
            $('#OpenCustomerGroupModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch CutomerGroups Modal *****************************************************/
function OpenCustomerContactModal(CustomerId, callId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/CustomerList/OpenCustomerContactModal",
        data: { CustomerId: CustomerId, callId: callId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showModal').html(result);
            $('#OpenCustomerContactModalId').modal({
                backdrop: false,
                keyboard: false
            })
            $('#OpenCustomerContactModalId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}

/******************************************** Fetch Add Customer Group Modal *****************************************************/
function AddCustomerGroup(CustomerId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/CustomerList/AddCustomerGroup",
        data: { CustomerId: CustomerId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showModal').html(result);
            $('#AddCustomerGroupId').modal({
                backdrop: false,
                keyboard: false
            })
            $('#AddCustomerGroupId').modal('show');
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Fetch Add Customer Contact Modal *****************************************************/
function AddCustomerContact(customerId) {
    event.preventDefault();

    $.ajax({
        method: "GET",
        url: "/CustomerList/AddCustomerContact",
        data: { customerId: customerId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            $('#showModal').html();
            $('#OpenCustomerContactModalId').modal('hide');
            $('.AddContactFormView').removeClass('d-none');
            $('.ContactFormView').addClass('d-none');
            $('.ContactNormalView').addClass('d-none');
            $('.AtAddContact').val("");
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Update Customer Group Modal *****************************************************/
function CreateCustomerGroup(CustomerId) {
    event.preventDefault();

    if ($('#AddCustomerGroupForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/CustomerList/CreateCustomerGroup",
            data: $('#AddCustomerGroupForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#AddCustomerGroupId').modal('hide');
                if (result == true) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Group Created Successfully!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
                else {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "Group Already Exists!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
                GetEditCustomerData(CustomerId);
                OpenCustomerGroupModal(CustomerId, 2);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Update Customer Group Modal *****************************************************/
function CreateCustomerContact(CustomerId) {
    event.preventDefault();

    if ($('#AddCustomerContactForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/CustomerList/CreateCustomerContact",
            data: $('#AddCustomerContactForm').serialize(),

            success: function (result) {
                if (result.code == 401) {
                    location.reload();
                }
                $('#AddCustomerContactId').modal('hide');
                if (result.success == true) {
                    Swal.fire({
                        position: "top-end",
                        icon: "success",
                        title: "Contact Created Successfully!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
                else {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "Contact Already Exists!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
                GetEditCustomerData(CustomerId);
                GetEditContactCustomerData(CustomerId, result.contactId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Delete CustomerGroup *****************************************************/
function DeleteCustomerGroup(GroupId) {
    var CustomerId = $('#CustomerId').val();

    Swal.fire({
        title: 'Are you sure?',
        text: 'You want to delete this Supplier Group?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, keep it'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                method: "POST",
                url: "/CustomerList/DeleteCustomerGroup",
                data: { GroupId: GroupId },

                success: function (result) {
                    if (result == true) {
                        Swal.fire({
                            position: "top-end",
                            icon: "success",
                            title: "Group Deleted Successfully!",
                            showConfirmButton: false,
                            timer: 1500
                        });
                    }
                    else {
                        Swal.fire({
                            position: "top-end",
                            icon: "error",
                            title: "Group Not Found!",
                            showConfirmButton: false,
                            timer: 1500
                        });
                    }
                    GetEditCustomerData(CustomerId);
                    GetEditContactCustomerData(CustomerId, 0);
                    OpenCustomerGroupModal(CustomerId, 2);
                },

                error: function () {
                    Swal.fire("Oops", "Something Went Wrong", "error");
                }
            });
        }
    });
}
/******************************************** Delete CustomerContact *****************************************************/
function DeleteCustomerContact(contactId) {
    var CustomerId = $('#CustomerId').val();

    Swal.fire({
        title: 'Are you sure?',
        text: 'You want to delete this customer contact?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, delete it!',
        cancelButtonText: 'No, keep it'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                method: "POST",
                url: "/CustomerList/DeleteCustomerContact",
                data: { contactId: contactId },

                success: function (result) {
                    if (result == true) {
                        Swal.fire({
                            position: "top-end",
                            icon: "success",
                            title: "Contact Deleted Successfully!",
                            showConfirmButton: false,
                            timer: 1500
                        });
                    }
                    else {
                        Swal.fire({
                            position: "top-end",
                            icon: "error",
                            title: "Contact Not Found!",
                            showConfirmButton: false,
                            timer: 1500
                        });
                    }
                    GetEditCustomerData(CustomerId);
                    GetEditContactCustomerData(CustomerId, 0);
                    OpenCustomerContactModal(CustomerId, 2);
                },

                error: function () {
                    Swal.fire("Oops", "Something Went Wrong", "error");
                }
            });
        }
    });
}
/******************************************** Assign Customer To Supplier *****************************************************/
function Assign(supplierGroupId) {
    var checkid = $('#supplier_' + supplierGroupId);

    $.ajax({
        method: "POST",
        url: "/CustomerList/AssignCustomerPost",
        data: { supplierGroupId: supplierGroupId },

        success: function (result) {
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Filter Customer's Group *****************************************************/
function filterSuppliers() {
    var searchValue = document.getElementById("SupplierGroupSearch").value.toLowerCase();

    var supplierItems = document.querySelectorAll(".supplier-item");

    supplierItems.forEach(function (item) {
        var supplierName = item.querySelector(".supplier-name").textContent.toLowerCase();

        if (supplierName.includes(searchValue)) {
            item.style.setProperty('display', 'flex', 'important');
        } else {
            item.style.setProperty('display', 'none', 'important');
        }
    });
}
/******************************************** Filter Customer's Contact *****************************************************/
function filterContact() {
    var searchValue = document.getElementById("ContactSearch").value.toLowerCase();

    var contactItems = document.querySelectorAll(".contact-item");

    contactItems.forEach(function (item) {
        var contactName = item.querySelector(".contact-name").textContent.toLowerCase();

        if (contactName.includes(searchValue)) {
            item.style.setProperty('display', 'flex', 'important');
        } else {
            item.style.setProperty('display', 'none', 'important');
        }
    });
}
/******************************************** Clear Filter Customer's Group *****************************************************/
function ClearSearch() {
    $('#SupplierGroupSearch').val("");
    var searchValue = "";

    var supplierItems = document.querySelectorAll(".supplier-item");

    supplierItems.forEach(function (item) {
        var supplierName = item.querySelector(".supplier-name").textContent.toLowerCase();

        if (supplierName.includes(searchValue)) {
            item.style.setProperty('display', 'flex', 'important');
        } else {
            item.style.setProperty('display', 'none', 'important');
        }
    });
}
/******************************************** Clear Filter Customer's Contact *****************************************************/
function ClearContactSearch() {
    $('#ContactSearch').val("");
    var searchValue = "";

    var contactItems = document.querySelectorAll(".contact-item");

    contactItems.forEach(function (item) {
        var contactName = item.querySelector(".contact-name").textContent.toLowerCase();

        if (contactName.includes(searchValue)) {
            item.style.setProperty('display', 'flex', 'important');
        } else {
            item.style.setProperty('display', 'none', 'important');
        }
    });
}
/******************************************** Show/Hide Edit Customer's Group Form *****************************************************/
function EditCustomerGroup(SupplierGroupId) {
    var id = $('#SupplierWiseDiv_' + SupplierGroupId);
    if (id.hasClass('d-none')) {
        id.removeClass('d-none').addClass("collapse show");
    }
    else {
        id.addClass('d-none').removeClass("collapse show");
    }
}
/******************************************** Show/Hide Edit Customer's Contact Form *****************************************************/
function EditCustomerContact(ContactId) {
    var id = $('#ContactWiseDiv_' + ContactId);
    if (id.hasClass('d-none')) {
        id.removeClass('d-none').addClass("collapse show");
    }
    else {
        id.addClass('d-none').removeClass("collapse show");
    }
}
/******************************************** Validate Edit Customer Group Form *****************************************************/
function ValidateEditCustomerGroupForm(supplierGroupId) {
    var input = $('#suppName_' + supplierGroupId)
    var submit = $('#submit_' + supplierGroupId)
    if (input.val().trim() == "") {
        submit.prop('disabled', true);
    }
    else {
        submit.prop('disabled', false);
    }
}
/******************************************** Validate Edit Customer Contact Form *****************************************************/
function ValidateEditCustomerContactForm(contactId) {
    var input = $('#conName_' + contactId)
    var submit = $('#submit_' + contactId)
    if (input.val().trim() == "") {
        submit.prop('disabled', true);
    }
    else {
        submit.prop('disabled', false);
    }
}
/******************************************** Post Edit Customer Group Form *****************************************************/
function PostEditCustomerGroupForm(supplierGroupId, customerId) {
    var inputName = $('#suppName_' + supplierGroupId).val();

    var supplierGroupId = $('#id_' + supplierGroupId).val();
    var customerId = $('#CustomerId').val();
    var inputName = $('#name_' + supplierGroupId).val(inputName);

    $.ajax({
        method: "POST",
        url: "/CustomerList/PostEditCustomerGroup",
        data: $('#SupplierWiseDiv_' + supplierGroupId).serialize(),

        success: function (result) {
            if (result == false) {
                Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: "Supplier Group Not Found!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
            $('#supplierName_' + supplierGroupId).val(inputName);
            GetEditCustomerData(customerId);
            GetEditContactCustomerData(customerId, 0);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Post Edit Customer Contact Form *****************************************************/
function PostEditCustomerContactForm(contactId, customerId) {
    var inputName = $('#conName_' + contactId).val();

    var contactId = $('#id_' + contactId).val();
    var customerId = $('#CustomerId').val();
    var inputName = $('#name_' + contactId).val(inputName);

    $.ajax({
        method: "POST",
        url: "/CustomerList/PostEditCustomerContact",
        data: $('#ContactWiseDiv_' + contactId).serialize(),

        success: function (result) {
            if (result == false) {
                Swal.fire({
                    position: "top-end",
                    icon: "error",
                    title: "Contact Not Found!",
                    showConfirmButton: false,
                    timer: 1500
                });
            }
            $('#contactName_' + contactId).val(inputName);
            GetEditCustomerData(customerId);
            GetEditContactCustomerData(customerId, 0);
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
/******************************************** Cancel Button at AddContactForm *****************************************************/
function RemoveFormView(customerId) {
    $('.AddContactFormView').addClass('d-none');
    $('#EditContactIcon').removeClass("d-none");
    GetEditContactCustomerData(customerId, 0);
    OpenCustomerContactModal(customerId, 2);
}

/******************************************** Post Edit Customer's Contact Form *****************************************************/
function EditCustomerContactPost(customerId, contactId) {
    if ($('#EditCustomerContactForm').valid()) {
        $.ajax({
            method: "POST",
            url: "/CustomerList/PostEditCustomerContactData",
            data: $('#EditCustomerContactForm').serialize(),

            success: function (result) {
                if (result == false) {
                    Swal.fire({
                        position: "top-end",
                        icon: "error",
                        title: "Contact Not Found!",
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
                GetEditCustomerData(customerId);
                GetEditContactCustomerData(customerId, contactId);
            },

            error: function () {
                Swal.fire("Oops", "Something Went Wrong", "error");
            }
        });
    }
}
/******************************************** Enable SelectOption *****************************************************/
function EnableSubscription() {
    var email = $('#floatingEmailAtAddContact').val();
    var selectoption = $('#floatingMailingListEditAtAddContact');
    if (email.trim() == "") {
        selectoption.prop('disabled', true);
    }
    else {
        selectoption.prop('disabled', false);
    }
}
function EnableSubscriptionEdit() {
    var email = $('#floatingEmailEdit').val();
    var selectoption = $('#floatingMailingListEdit');
    if (email.trim() == "") {
        selectoption.prop('disabled', true);
    }
    else {
        selectoption.prop('disabled', false);
    }
}
function EnableSelectOptionInvoice() {
    var email = $('#floatingEmailAtInvoice').val();
    var selectoption = $('#floatingIsSubscribedAtInvoice');
    if (email.trim() == "") {
        selectoption.prop('disabled', true);
    }
    else {
        selectoption.prop('disabled', false);
    }
}
function EnableSelectOptionAtAddCustomer() {
    var email = $('#floatingEmailAtAddCustomer').val();
    var selectoption = $('#floatingIsSubscribedAtAddCustomer');
    if (email.trim() == "") {
        selectoption.prop('disabled', true);
    }
    else {
        selectoption.prop('disabled', false);
    }
}
/******************************************** ValidateContactName *****************************************************/
function ValidateContactName(customerId) {
    var fullName = $('#floatingFNameAtAddContact').val();

    $.ajax({
        method: "POST",
        url: "/CustomerList/CheckContactName",
        data: { fullName: fullName, customerId: customerId },

        success: function (result) {
            if (result.code == 401) {
                location.reload();
            }
            if (result == true) {
                Swal.fire({
                    title: "Sorry!",
                    text: "Contact Name Already Exists!",
                    icon: "error",
                    timer: 3000,
                    timerProgressBar: true,
                });
                $('#floatingFNameAtAddContact').val('');
            }
        },

        error: function () {
            Swal.fire("Oops", "Something Went Wrong", "error");
        }
    });
}
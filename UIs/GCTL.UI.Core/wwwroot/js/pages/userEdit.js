const passwordInput = document.getElementById('Password');
const toggleBtn = document.getElementById('toggleBtn');
const eyeIcon = document.getElementById('eyeIcon');
const tooltip = document.getElementById('passwordTooltip');

function updateToggleVisibility() {
    toggleBtn.style.display = passwordInput.value.length > 0 ? 'block' : 'none';
}

function togglePassword(savePosition = true) {
    const cursorPos = savePosition ? passwordInput.selectionStart : null;
    if (passwordInput.type === 'password') {
        passwordInput.type = 'text';
        eyeIcon.classList.remove('fa-eye');
        eyeIcon.classList.add('fa-eye-slash');
    } else {
        passwordInput.type = 'password';
        eyeIcon.classList.remove('fa-eye-slash');
        eyeIcon.classList.add('fa-eye');
    }
    if (savePosition && cursorPos !== null) {
        passwordInput.setSelectionRange(cursorPos, cursorPos);
        passwordInput.focus();
    }
}

passwordInput.addEventListener('input', updateToggleVisibility);
passwordInput.addEventListener('focus', () => {
    updateToggleVisibility();
    tooltip.classList.remove('show');
});
passwordInput.addEventListener('blur', () => toggleBtn.style.display = 'none');
toggleBtn.addEventListener('click', () => togglePassword(true));
toggleBtn.addEventListener('mousedown', (e) => e.preventDefault());

let isAnimating = false;

function showPasswordTooltip(text) {
    tooltip.textContent = text;
    tooltip.classList.add('show', 'tooltip-white');
    tooltip.style.display = 'block';

    if (!isAnimating) {
        isAnimating = true;
        animateLoop();
    }
}

function animateLoop() {
    tooltip.style.opacity = '0';
    tooltip.style.transform = 'translateY(-50%) scale(0.8)';

    let start = null;
    const duration = 2000;

    function animate(timestamp) {
        if (!start) start = timestamp;
        const progress = (timestamp - start) / duration;

        if (progress < 0.5) {
            const t = progress * 2;
            const scale = 0.8 + (0.25 * easeOutBack(t));
            tooltip.style.transform = `translateY(-50%) scale(${scale})`;
            tooltip.style.opacity = t;
        } else {
            const t = (progress - 0.5) * 2;
            const scale = 1.05 - (0.05 * t);
            tooltip.style.transform = `translateY(-50%) scale(${scale})`;
            tooltip.style.opacity = 1;
        }

        if (progress < 1) {
            requestAnimationFrame(animate);
        } else {
            tooltip.style.transform = 'translateY(-50%) scale(1)';
            tooltip.style.opacity = '1';

            setTimeout(() => {
                if (tooltip.classList.contains('show')) {
                    animateLoop();
                } else {
                    isAnimating = false;
                }
            }, 400);
        }
    }

    function easeOutBack(t) {
        const c1 = 1.70158;
        const c3 = c1 + 1;
        return 1 + c3 * Math.pow(t - 1, 3) + c1 * Math.pow(t - 1, 2);
    }

    requestAnimationFrame(animate);
}

//end pw tooltip
function PopulateForm(id) {

    var requestData = {};
    if (id !== undefined && id !== null && id !== '') {
        requestData.id = id;
    }

    $.ajax({
        url: '/EditUser/GetById',
        type: 'GET',
        data: requestData, //{ code: id }, // Updated from ppeid to match controller signature
        success: function (data) {
            if (data) {
                data = data.data;
                console.log(data);
               // setTimeout(() => {
                $('#UserId').val(data.userId);
               // }, 100);
                $('#Username').val(data.username);
                $('#FirstName').val(data.firstName); 
                $('#LastName').val(data.lastName);
                $('#EmployeeId').val(data.employeeId);
                if (data.dob) {
                    $('#Dob').val(data.dob);
                }
                $('#OffPhone').val(data.offPhone);
                $('#PerPhone').val(data.perPhone);
                $('#OffEmail').val(data.offEmail);
                $('#PerEmail').val(data.perEmail);
                $('#PerEmail').val(data.perEmail);
                $('#WorkStation').val(data.workStation);
                $('#Regulation').val(data.regulation);
                $('#Regulation').val(data.regulation);
                $('#editUser_LDate').val(data.lDate);
                $('#editUser_ModifyDate').val(data.modifyDate);

            }
        },
        error: function () {
            alert('Failed to load data.');
        }
    });
}

$(document).ready(function () {
    PopulateForm();
    displayTableData();
    showPasswordTooltip("Change Your Password");
    $(document).on(`click`, `.userInfo-id-link`, function (e) {
        e.preventDefault();
        const id = $(this).data("id");
        if (id) PopulateForm(id);
    });

    $('#editUser-check-all').off(`change`).on(`change`, function () {
        const isChecked = $(this).is(':checked');
        $(`#editUserGrid-body input[type="checkbox"]`).prop('checked', isChecked);
        updateSelectedIds();
    });

    $(document).on(`change`, `#editUserGrid-body input[type="checkbox"]`, function () {
        const id = $(this).data('id');
        if ($(this).is(':checked')) {
            selectedIds.add(id);
        } else {
            selectedIds.delete(id);
        }
        updateCheckAllState();
    });

    $("body").on(`click`, '.js-edit-user-save', handleFormSubmit);
})

function handleFormSubmit() {
    if (!validateForm()) return;

    showLoading();

    const formData = new FormData($('#editUserForm')[0]);
    formData.forEach((value, key) => {
        console.log(key, value);
    });

    $.ajax({
        url: '/EditUser/Save',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.success) {
                showNotification(res.message, "success");
                clearForm();
                displayTableData();
                PopulateForm();
            } else {
                showNotification(res.message, "error");
            }
        },
        error: function () {
            showNotification('Error occurred while saving.', "error");
        },
        complete: hideLoading
    });
}

function validateForm() {
    const pass = $('#Password').val();

    if (!pass || $.trim(pass) === '') {
        showNotification(`Password is required.`, "warning");
        $(`#Password`).focus();
        return false;
    }

    return true;
}

function clearForm() {
    $('#editUserForm')[0].reset();
    $('#UserId').val(0);

    handleDateInfoDisplay();
}

function handleDateInfoDisplay() {
    const id = $('#UserId').val();
    if (id != 0) {
        $('#editUser_DateInfo').removeClass('d-none');
    } else {
        $('#editUser_DateInfo').addClass('d-none');
        $('#editUser_LDate').text('');
        $('#editUser_ModifyDate').text('');
    }
}


function displayTableData() {
    if ($.fn.DataTable.isDataTable("#editUserGrid")) {
        $("#editUserGrid").DataTable().clear().destroy();
    }

    $("#editUserGrid-body").empty();

    $("#editUserGrid").DataTable({
        processing: true,
        serverSide: true,
        autoWidth: false,
        fixedHeader: false,
        info: true,
        lengthChange: true,
        lengthMenu: [[5, 10, 25, 50, 100, -1], [5, 10, 25, 50, 100, "All"]],
        order: [[1, 'desc']],
        ordering: true,
        pageLength: 5,
        paging: true,
        responsive: true,
        scrollCollapse: true,
        scrollX: true,
        searching: true,

        language: {
            search: "🔍 Search:",
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            paginate: {
                first: "First",
                previous: "Prev",
                next: "Next",
                last: "Last"
            },
            emptyTable: "No data available",
            processing: "Loading data..."
        },

        ajax: {
            url: '/EditUser/GetPaginatedList',
            type: 'POST'
        },
        columns: [
            {
                data: null,
                orderable: false,
                className: 'text-center no-sort',
                render: function (data, type, row) {
                    return `<input type="checkbox" class"py-0 no-sort" data-id="${row.id}"/>`;
                }
            },
            {
                data: 'id',
                className: 'text-center',
                render: function (data, type, row) {
                    return `<a href="" class="py-0 userInfo-id-link" data-id="${row.id}">${data}</a>`;
                }
            },
            {
                data: 'username',
                className:'py-0'
            },
            {
                data: 'fullName',
                className:'py-0'
            },
            {
                data: 'userType',
                className:'py-0 text-center'
            },
            {
                data: 'entryDate',
                className:'py-0 text-center'
            }
        ],
        columnDefs: [
            {
                targets: 0,
                orderable: false,
                className: 'no-sort'
            }
        ],
        initComplete: function () {
            setupTableSearch(this.api());
        },
        drawCallback: function () {
            restoreCheckboxStates();
            updateCheckAllState();
        }
    })
}

function setupTableSearch(api) {
    const tableId = 'editUserGrid';
    let debounceTimeout;

    $(`#${tableId}_wrapper .dataTables_filter input`)
        .off('input.custom')
        .on('input.custom', function () {
            clearTimeout(debounceTimeout);
            const searchTerm = this.value;

            debounceTimeout = setTimeout(function () {
                api.search(searchTerm).page('first').draw('page');
            }, 500);
        });
}

function restoreCheckboxStates() {
    $(`#editUserGrid-body input[type="checkbox"]`).each(function () {
        const id = $(this).data('id');
        $(this).prop('checked', selectedIds.has(id));
    });
}

function updateSelectedIds() {
    const checkboxes = $(`#editUserGrid-body input[type="checkbox"]`);

    checkboxes.each(function () {
        const id = $(this).data('id');
        if ($(this).is(':checked')) {
            selectedIds.add(id);
        } else {
            selectedIds.delete(id);
        }
    });
}

function updateCheckAllState() {
    const total = $(`#editUserGrid-body input[type="checkbox"]`).length;
    const checked = $(`#editUserGrid-body input[type="checkbox"]:checked`).length;
    $('#editUser-check-all').prop('checked', total > 0 && total === checked);
}

const selectedIds = new Set();


function setupLoadingOverlay() {
    if ($("#loadingOverlay").length === 0) {
        $("body").append(`
            <div id="loadingOverlay" style="
                display: none;
                position: fixed;
                top: 0;
                left: 0;
                width: 100%;
                height: 100%;
                background-color: rgba(0, 0, 0, 0.5);
                z-index: 9999;
                justify-content: center;
                align-items: center;">
                <div style="
                    background-color: white;
                    padding: 20px;
                    border-radius: 5px;
                    box-shadow: 0 0 10px rgba(0,0,0,0.3);
                    text-align: center;">
                    <div class="spinner-border text-primary" role="status">
                    </div>
                </div>
            </div>
        `);
    }
}

function showLoading() {
    $('body').css('overflow', 'hidden');
    $("#loadingOverlay").css('display', 'flex').fadeIn(200);
}

function hideLoading() {
    $('body').css('overflow', '');
    $("#loadingOverlay").fadeOut(200);
}

function showNotification(message, type = 'info') {
    if (typeof toastr !== 'undefined') {
        const title = { success: 'Success', error: 'Error', warning: 'Warning' }[type] || 'Info';
        toastr[type](message, title);
    } else {

        alert(message);
    }
}
function showConfirmation(message, title, callback) {
    if (typeof Swal === 'undefined') {
        // Fallback to native confirm if SweetAlert2 is not loaded
        if (confirm(`${message}`)) {
            callback();
        }
        return;
    }

    Swal.fire({
        title: title,
        text: message,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, proceed',
        cancelButtonText: 'No, cancel'
    }).then(function (result) {
        if (result.value === true || result.isConfirmed) {
            callback();
        }
    });
}

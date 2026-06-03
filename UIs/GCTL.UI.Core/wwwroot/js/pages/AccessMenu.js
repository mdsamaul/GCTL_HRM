
$(document).ready(function () {


    getMenuData(null);
    function getMenuData(accessCodeId) {
        let accessCodeId2 = $("#access-code-id").val().trim(); 

        $.ajax({
            url: '/MenuTab/AccessMenus',
            method: 'GET',
            data: { accessCodeId: accessCodeId }, // Pass Access Code Id
            dataType: 'json',
            success: function (response) {
                renderMenuTable(response.data);
                getAccessName(accessCodeId);
            },
            error: function (err) {
                console.error('Failed to load menu data:', err);
            }
        });       
    }


    function getAccessName(accessCodeId) {
        $.ajax({
            url: '/MenuTab/GetAccessName',
            method: 'GET',
            data: { accessCodeId: accessCodeId }, // Pass Access Code Id
            dataType: 'text',
            success: function (response) {
                console.log('/MenuTab/GetAccessName', response)
                $('#access-code-name').val(response);
            },
            error: function (err) {
                console.error('Failed to load menu data:', err);
            }
        });
    }

    


    setupCheckboxLogic11()

    // Right Side Table
    fetchData();

    $('#access-code-id').on('input', function () {
        let accessCodeId = $(this).val().trim(); // Get the updated value
        console.log("Access Code Id updated:", accessCodeId);

        // Call your function dynamically
        getMenuData(accessCodeId);
    });


    //#region right table button  for edit

    // Populate left form when clicking access ID
    $(document).on('click', '.access-id-btn', function () {



        const id = $(this).data('id');
        const name = $(this).data('name');
        $('#access-code-id').val(id);
       

        getMenuData(id);

    });

    //#endregion


    //#region Save Button

    //$('#saveAccessMenuBtn').on('click', function () {
    //    const accessCodeId = $('#access-code-id').val().trim();
    //    const accessCodeName = $('#access-code-name').val().trim();

    //    if (!accessCodeId || !accessCodeName) {
    //        alert('Access Code Id and Name are required.');
    //        return;
    //    }

    //    const allMenus = [];

    //    $('#menuTabLoad tr').each(function () {
    //        const $row = $(this);
    //        const menuId = $row.data('menuid');
    //        const parentId = $row.data('parentid') || null;

    //        allMenus.push({
    //            menuId: menuId,
    //            parentId: parentId,
    //            isSelected: $row.find('.row-main-check').is(':checked'),
    //            canAdd: $row.find('.add-check').is(':checked'),
    //            canEdit: $row.find('.edit-check').is(':checked'),
    //            canDelete: $row.find('.delete-check').is(':checked'),
    //            canPrint: $row.find('.print-check').is(':checked')
    //        });
    //    });

    //    const payload = {
    //        accessCodeId,
    //        accessCodeName,
    //        menuAccessList: allMenus
    //    };

    //    console.log('payload', payload)

    //    // 🔽 Send to backend via AJAX
    //    $.ajax({
    //        url: '/menuTab/SaveAccessCode',
    //        type: 'POST',
    //        contentType: 'application/json',
    //        data: JSON.stringify(payload),
    //        success: function (response) {
    //            alert('Data saved successfully.');
    //        },
    //        error: function (err) {
    //            console.error(err);
    //            alert('Error saving data.');
    //        }
    //    });
    //});

    $('#saveAccessMenuBtn').on('click', function () {

        const accessCodeId = $('#access-code-id').val().trim();
        const accessCodeName = $('#access-code-name').val().trim();

        if (!accessCodeId || !accessCodeName) {
            alert('Access Code Id and Name are required.');
            return;
        }

        const allMenus = [];

        $('#menuTabLoad tr').each(function () {
            const $row = $(this);
            const menuId = $row.data('menuid');
            const parentId = $row.data('parentid') || null;

            allMenus.push({
                menuId: menuId,
                parentId: parentId,
                canAdd: $row.find('.add-check').is(':checked'),
                canEdit: $row.find('.edit-check').is(':checked'),
                canDelete: $row.find('.delete-check').is(':checked'),
                canPrint: $row.find('.print-check').is(':checked')
            });
        });

        const formData = new FormData();

        formData.append("AccessCodeId", accessCodeId);
        formData.append("AccessCodeName", accessCodeName);


        formData.append("MenuAccessList", JSON.stringify(allMenus));

        $.ajax({
            url: '/menuTab/SaveAccessCode',
            type: 'POST',
            data: formData,
            processData: false,   // ❗ important
            contentType: false,   // ❗ important
            success: function (response) {
                alert('Data saved successfully.');
            },
            error: function (err) {
                console.error(err);
                alert('Error saving data.');
            }
        });

    });

    //#endregion Save Button


});


//#region Right Table

let currentPage = 1;
const pageSize = 5;

function fetchData(page = 1, search = '') {
    $.ajax({
        url: '/menuTab/GetAccessListTable',
        method: 'GET',
        data: {
            page: page,
            pageSize: pageSize,
            search: search
        },
        success: function (response) {
            $('#accessListTable').empty();

            // Populate table
            response.data.forEach(item => {
                $('#accessListTable').append(`
                    <tr>
                        <td>
                            <div class="form-check">
                                <input class="form-check-input row-checkbox" type="checkbox" data-id="${item.accessCodeId}">
                            </div>
                        </td>
                        <td>
                            <button type="button" class="btn btn-link p-0 access-id-btn" data-id="${item.accessCodeId}" data-name="${item.accessCodeName}">
                                ${item.accessCodeId}
                            </button>
                        </td>
                        <td>${item.accessCodeName}</td>
                    </tr>
                `);
            });

            updatePagination(response.total, page);
        }
    });
}

function updatePagination(totalItems, currentPage) {
    const totalPages = Math.ceil(totalItems / pageSize);
    const $pagination = $('#pagination');
    $pagination.empty();

    $pagination.append(`
        <li class="page-item ${currentPage === 1 ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${currentPage - 1}">Previous</a>
        </li>`);

    for (let i = 1; i <= totalPages; i++) {
        $pagination.append(`
            <li class="page-item ${i === currentPage ? 'active' : ''}">
                <a class="page-link" href="#" data-page="${i}">${i}</a>
            </li>`);
    }

    $pagination.append(`
        <li class="page-item ${currentPage === totalPages ? 'disabled' : ''}">
            <a class="page-link" href="#" data-page="${currentPage + 1}">Next</a>
        </li>`);
}

// Event Listeners
$(document).on('click', '.page-link', function (e) {
    e.preventDefault();
    const page = $(this).data('page');
    if (page && page !== currentPage) {
        currentPage = page;
        fetchData(currentPage, $('#searchInput').val());
    }
});

$('#searchInput').on('keyup', function () {
    currentPage = 1;
    fetchData(currentPage, $(this).val());
});

// Select All Checkbox
$('#selectAllAccess').on('change', function () {
    $('.row-checkbox').prop('checked', this.checked);
});







//#endregion





//#region Delete Selected
// Delete Selected
$('#deleteSelectedBtn').on('click', function () {
    const selectedIds = $('.row-checkbox:checked').map(function () {
        return $(this).data('id');
    }).get();

    if (selectedIds.length === 0) {
        alert('No items selected.');
        return;
    }
    console.log(selectedIds)

    if (confirm(`Delete ${selectedIds.length} item(s)?`)) {
        $.ajax({
            url: '/menuTab/DeleteAccessCodes',
            method: 'POST',
            data: JSON.stringify(selectedIds),
            contentType: 'application/json',
            success: function () {
                fetchData(currentPage, $('#searchInput').val());
            }
        });
    }
});

//#endregion Delete Selected



//#region Main Table
function renderMenuTable(data) {
    console.log('renderMenuTable', data)
    const $tableBody = $('#menuTabLoad');
    $tableBody.empty();

    if (data.length > 0) {
        const firstItem = data[0];

        console.log('firstItem - renderMenuTable', data)

    }
    function renderRow(item, level) {
        const indent = '&nbsp;'.repeat(level * 4); // Indent children
        const row = `
            <tr class="${item.children && item.children.length ? 'bg-light fw-bold parent-row' : 'child-row'}" 
                data-menuid="${item.menuId}" 
                data-parentid="${item.parentId || ''}">
                
                <td>
                    <div class="form-check">
                        <input class="form-check-input row-main-check" type="checkbox">
                    </div>
                </td>
                <td class="${item.children && item.children.length ? 'fw-bold' : 'text-primary'}">
                    ${indent}${item.title || ''}
                </td>
                <td class="text-center">
                    <div class="form-check d-inline-block">
                        <input class="form-check-input add-check" type="checkbox" ${item.checkAdd ? 'checked' : ''}>
                    </div>
                </td>
                <td class="text-center">
                    <div class="form-check d-inline-block">
                        <input class="form-check-input edit-check" type="checkbox" ${item.checkEdit ? 'checked' : ''}>
                    </div>
                </td>
                <td class="text-center">
                    <div class="form-check d-inline-block">
                        <input class="form-check-input delete-check" type="checkbox" ${item.checkDelete ? 'checked' : ''}>
                    </div>
                </td>
                <td class="text-center">
                    <div class="form-check d-inline-block">
                        <input class="form-check-input print-check" type="checkbox" ${item.checkPrint ? 'checked' : ''}>
                    </div>
                </td>
            </tr>
        `;
        $tableBody.append(row);

        if (item.children && item.children.length > 0) {
            item.children.forEach(child => renderRow(child, level + 1)); // recursion
        }
    }

    data.forEach(item => renderRow(item, 0));
}




function setupCheckboxLogic11() {
    // ✅ Select All checkbox
    $('#selectAll').on('change', function () {
        const checked = $(this).is(':checked');
        $('.row-main-check, .add-check, .edit-check, .delete-check, .print-check').prop('checked', checked);
    });

    // ✅ Left-side Main checkbox (row) checked -> check rights + check all children recursively
    $('#menuTabLoad').on('change', '.row-main-check', function () {
        const $row = $(this).closest('tr');
        const isChecked = $(this).is(':checked');

        // Check/uncheck its own rights
        $row.find('.add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);

        const menuId = $row.data('menuid');

        // Recursively check/uncheck children
        checkChildren(menuId, isChecked);

        updateSelectAllState();
    });

    // ✅ Individual right checkbox (add/edit/delete/print) changes
    $('#menuTabLoad').on('change', '.add-check, .edit-check, .delete-check, .print-check', function () {
        const $row = $(this).closest('tr');
        const allRightsChecked = $row.find('.add-check, .edit-check, .delete-check, .print-check').filter(':checked').length === 4;
        $row.find('.row-main-check').prop('checked', allRightsChecked);

        const menuId = $row.data('menuid');

        // If parent is checked, propagate individual right to children too
        const rightClass = $(this).attr('class').split(' ')[1]; // get 'add-check' etc.
        const isRightChecked = $(this).is(':checked');

        checkRightInChildren(menuId, rightClass, isRightChecked);

        updateSelectAllState();
    });

    // ✅ Update "Select All" checkbox based on individual row checkboxes
    function updateSelectAllState() {
        const totalBoxes = $('.row-main-check').length;
        const checkedBoxes = $('.row-main-check:checked').length;
        $('#selectAll').prop('checked', totalBoxes === checkedBoxes);
    }

    // ✅ Recursively check/uncheck child rows (full hierarchy)
    function checkChildren(parentId, isChecked) {
        $(`tr[data-parentid="${parentId}"]`).each(function () {
            $(this).find('.row-main-check').prop('checked', isChecked);
            $(this).find('.add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);

            const childMenuId = $(this).data('menuid');
            checkChildren(childMenuId, isChecked); // Recursive for grandchildren
        });
    }

    // ✅ Recursively check/uncheck specific right (add/edit/delete/print) in children
    function checkRightInChildren(parentId, rightClass, isChecked) {
        $(`tr[data-parentid="${parentId}"]`).each(function () {
            $(this).find(`.${rightClass}`).prop('checked', isChecked);

            const childMenuId = $(this).data('menuid');
            checkRightInChildren(childMenuId, rightClass, isChecked); // Recursive for grandchildren
        });
    }
}


//#endregion Main Table

//#region Backup
function setupCheckboxLogic() {
    // ✅ Select All checkbox
    $('#selectAll').on('change', function () {
        const checked = $(this).is(':checked');
        $('.row-main-check, .add-check, .edit-check, .delete-check, .print-check').prop('checked', checked);
    });

    // ✅ Row left-side checkbox selects/deselects its row's rights
    $('#menuTabLoad').on('change', '.row-main-check', function () {
        const $row = $(this).closest('tr');
        const isChecked = $(this).is(':checked');
        $row.find('.add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);

        updateSelectAllState();
    });

    // ✅ Individual permission checkbox updates left checkbox
    $('#menuTabLoad').on('change', '.add-check, .edit-check, .delete-check, .print-check', function () {
        const $row = $(this).closest('tr');
        const allChecked = $row.find('.add-check, .edit-check, .delete-check, .print-check').filter(':checked').length === 4;
        $row.find('.row-main-check').prop('checked', allChecked);

        updateSelectAllState();
    });

    // ✅ Parent row checkbox selects all child rows too
    $('#menuTabLoad').on('change', '.parent-row .row-main-check', function () {
        const $parent = $(this).closest('tr');
        const parentId = $parent.data('menuid');
        const isChecked = $(this).is(':checked');

        // Check/uncheck permissions in parent
        $parent.find('.add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);

        // Check/uncheck all children
        $(`.child-row[data-parentid="${parentId}"]`).each(function () {
            $(this).find('.row-main-check, .add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);
        });

        updateSelectAllState();
    });

    // ✅ If any checkbox is unchecked, uncheck "Select All"
    function updateSelectAllState() {
        const totalBoxes = $('.row-main-check').length;
        const checkedBoxes = $('.row-main-check:checked').length;

        $('#selectAll').prop('checked', totalBoxes === checkedBoxes);
    }
}

function renderMenuTableOld(data) {
    const $tableBody = $('#menuTabLoad');
    $tableBody.empty();

    let parentId = null;

    data.forEach((item, index) => {
        if (item.isParent) {
            parentId = item.menuId;
        }

        const row = `
                <tr class="${item.isParent ? 'bg-light fw-bold parent-row' : 'child-row'}" 
                    data-menuid="${item.menuId}" 
                    data-parentid="${item.isParent ? '' : parentId}">
                    
                    <td>
                        <div class="form-check">
                            <input class="form-check-input row-main-check" type="checkbox">
                        </div>
                    </td>
                    <td class="${item.isParent ? 'fw-bold' : 'text-primary'}">${item.displayTitle || ''}</td>
                    <td class="text-center">
                        <div class="form-check d-inline-block">
                            <input class="form-check-input add-check" type="checkbox" ${item.checkAdd ? 'checked' : ''}>
                        </div>
                    </td>
                    <td class="text-center">
                        <div class="form-check d-inline-block">
                            <input class="form-check-input edit-check" type="checkbox" ${item.checkEdit ? 'checked' : ''}>
                        </div>
                    </td>
                    <td class="text-center">
                        <div class="form-check d-inline-block">
                            <input class="form-check-input delete-check" type="checkbox" ${item.checkDelete ? 'checked' : ''}>
                        </div>
                    </td>
                    <td class="text-center">
                        <div class="form-check d-inline-block">
                            <input class="form-check-input print-check" type="checkbox" ${item.checkPrint ? 'checked' : ''}>
                        </div>
                    </td>
                </tr>
            `;
        $tableBody.append(row);
    });
}


//#endregion  backup
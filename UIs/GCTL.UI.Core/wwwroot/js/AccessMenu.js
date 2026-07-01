
$(document).ready(function () {
    //getMenuData(null);


    // Right Side Table
    loadAccessList();
    loadAccessCodeDropdown();
    loadParentMenus();
    setupCheckboxLogic11();

    //$('#access-code-id').on('input', function () {
    //    let accessCodeId = $(this).val().trim(); // Get the updated value
    //    console.log("Access Code Id updated:", accessCodeId);

    //    // Call your function dynamically
    //    getMenuData(accessCodeId);
    //});

    // Access Code dropdown change
    //$('#access-code-id').on('change', function () {
    $(document).on('change', '#access-code-id', function () {
        const accessCodeId = $(this).val();
        
        if (accessCodeId) {
            getAccessName(accessCodeId);
            //getMenuData(accessCodeId);
        } else {
            $('#access-code-name').val('');
            //getMenuData(null);
        }

        // Reset module filter
        $('#module-select').val('').trigger('change');

        updateTableVisibility();
        //renderMenuTable([]);
        applyModuleFilter('');
    });

    $('#module-select').on('change', function () {
        const moduleId = $(this).val();
        updateTableVisibility();

        if ($('#access-code-id').val() && moduleId)
            getMenuData($('#access-code-id').val());
        else {
            //renderMenuTable([]);
            applyModuleFilter(moduleId);
        }
        applyModuleFilter($(this).val());
    });


    //#region right table button  for edit

    // Populate left form when clicking access ID
    //$(document).on('click', '.access-id-btn', function () {

    //    const id = $(this).data('id');
    //    const name = $(this).data('name');
    //    $('#access-code-id').val(id);

    //    getMenuData(id);
    //});
    $(document).on('click', '.access-id-btn', function () {
        const id = $(this).data('id');
        $('#access-code-id').val(id).trigger('change');
        //$('#access-code-id');
    });
    //#endregion

    $('#clearBtn').on('click', function () {
        $('#access-code-id').val('').trigger('change');
        $('#access-code-name').val('');
        $('#module-select').val('').trigger('change');

        renderMenuTable([]);
        updateTableVisibility();
        //getMenuData(null);
        //applyModuleFilter('');
    });

    $('#openAddAccessCodeModal').on('click', function () {
        $('#modal-access-code-id').val('');
        $('#modal-access-code-name').val('');
        $('#addAccessCodeModal').modal('show');
        loadAccessModal();
    });

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
    //            menuId: menuId + "",
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

    //    // 🔽 Send to backend via AJAX
    //    $.ajax({
    //        url: '/menuTab/SaveAccessCode',
    //        type: 'POST',
    //        contentType: 'application/json',
    //        data: JSON.stringify(payload),
    //        success: function (response) {
    //            alert('Data saved successfully.');
    //            loadAccessList();
    //            getMenuData(null);
    //            $('#access-code-id').val('');
    //        },
    //        error: function (err) {
    //            console.error(err);
    //            alert('Error saving data.');
    //        }
    //    });
    //});

    $('#saveNewAccessCodeBtn').on('click', function () {
        const id = $('#modal-access-code-id').val().trim();
        const name = $('#modal-access-code-name').val().trim();

        if (!id || !name) {
            alert('Both fields are required.');
            return;
        }

        $.ajax({
            url: '/MenuTab/AddAccessCode',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ accessCodeId: id, accessCodeName: name }),
            success: function () {
                $('#addAccessCodeModal').modal('hide');
                loadAccessCodeDropdown(id); // reload and select new one
                loadAccessList();
            },
            error: function (err) {
                console.error(err);
                alert('Error adding access code.');
            }
        });
    });

    // Save button
    $('#saveAccessMenuBtn').on('click', function () {
        const accessCodeId = $('#access-code-id').val().trim();
        const accessCodeName = $('#access-code-name').val().trim();

        if (!accessCodeId || !accessCodeName) {
            alert('Access Code Id and Name are required.');
            return;
        }

        const allMenus = [];

        // Iterate ALL rows, including hidden ones (module filter uses hide/show)
        $('#menuTabLoad tr').each(function () {
            const $row = $(this);
            const menuId = $row.data('menuid');
            const parentId = $row.data('parentid') || null;

            allMenus.push({
                menuId: menuId + "",
                parentId: parentId,
                isSelected: $row.find('.row-main-check').is(':checked'),
                canAdd: $row.find('.add-check').is(':checked'),
                canEdit: $row.find('.edit-check').is(':checked'),
                canDelete: $row.find('.delete-check').is(':checked'),
                canPrint: $row.find('.print-check').is(':checked')
            });
        });

        $.ajax({
            url: '/menuTab/SaveAccessCode',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ accessCodeId, accessCodeName, menuAccessList: allMenus }),
            success: function () {
                alert('Data saved successfully.');
                loadAccessList();
                loadAccessCodeDropdown(accessCodeId);
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
//function loadAccessList() {
//    $("#accessListTable").DataTable({
//        ajax: {
//            url: '/menuTab/GetAccessListTable',   // adjust to your actual endpoint
//            type: "GET",
//            dataType: "json"
//        },
//        dom: '<"d-flex justify-content-between align-items-center mb-2"lf>rt<"d-flex justify-content-between align-items-center mt-2"ip>',
//        columns: [
//            {
//                data: "accessCodeId",
//                className: "text-center px-0 no-sort",
//                width: "40px",
//                render: function (data) {
//                    return `<input type="checkbox" class="text-center align-middle checkBox" value="${data}">`;
//                }
//            },
//            {
//                data: "accessCodeId",
//                className: "text-center",
//                render: function (data, type, row) {
//                    return `<button type="button" class="btn btn-link p-0 access-id-btn" 
//                    data-id="${data}" 
//                    data-name="${row.accessCodeName}">
//                    ${data}
//                </button>`;
//                }
//            },
//            {
//                data: "accessCodeName",
//                className: "text-start"
//            }
//        ],
//        columnDefs: [
//            {
//                targets: [0], orderable: false,
//                createdCell: function (td) {
//                    $(td).html(`
//                                 <div class="d-flex align-items-center justify-content-center  px-0">
//                                    ${$(td).html()}
//                                </div>
//                            `);
//                }
//            },
//        ],
//        lengthChange: true,
//        pageLength: 10,
//        lengthMenu: [[10, 25, 50, 100, 1000, -1], [10, 25, 50, 100, 1000, "All"]],
//        autoWidth: false,
//        responsive: true,
//        fixedHeader: true,
//        order: [[1, "desc"]],
//        bDestroy: true,
//    });
//}
function loadAccessList() {
    $("#accessListTable").DataTable({
        ajax: {
            url: '/menuTab/GetAccessListTable',
            type: "GET",
            dataType: "json"
        },
        dom: '<"d-flex justify-content-between align-items-center mb-2"lf>rt<"d-flex justify-content-between align-items-center mt-2"ip>',
        columns: [
            {
                data: "accessCodeId",
                className: "text-center px-0 no-sort",
                width: "40px",
                render: function (data) {
                    return `<input type="checkbox" class="text-center align-middle checkBox" value="${data}">`;
                }
            },
            {
                data: "accessCodeId",
                className: "text-center",
                render: function (data, type, row) {
                    return `<button type="button" class="btn btn-link p-0 access-id-btn" 
                        data-id="${data}" 
                        data-name="${row.accessCodeName}">
                        ${data}
                    </button>`;
                }
            },
            { data: "accessCodeName", className: "text-start" }
        ],
        columnDefs: [
            {
                targets: [0], orderable: false,
                createdCell: function (td) {
                    $(td).html(`<div class="d-flex align-items-center justify-content-center px-0">${$(td).html()}</div>`);
                }
            }
        ],
        lengthChange: true,
        pageLength: 10,
        lengthMenu: [[10, 25, 50, 100, 1000, -1], [10, 25, 50, 100, 1000, "All"]],
        autoWidth: false,
        responsive: true,
        fixedHeader: true,
        order: [[1, "desc"]],
        bDestroy: true,
    });
}
$('#selectAllAccess').on('change', function () {
    $('.row-checkbox').prop('checked', this.checked);
});
//#endregion

//function getAccessName(accessCodeId) {
//    $.ajax({
//        url: '/MenuTab/GetAccessName',
//        method: 'GET',
//        data: { accessCodeId: accessCodeId }, // Pass Access Code Id
//        dataType: 'text',
//        success: function (response) {
//            console.log('/MenuTab/GetAccessName', response)
//            $('#access-code-name').val(response);
//        },
//        error: function (err) {
//            console.error('Failed to load menu data:', err);
//        }
//    });
//}

//function getMenuData(accessCodeId) {
//    let accessCodeId2 = $("#access-code-id").val().trim();

//    $.ajax({
//        url: '/MenuTab/AccessMenus',
//        method: 'GET',
//        data: { accessCodeId: accessCodeId }, // Pass Access Code Id
//        dataType: 'json',
//        success: function (response) {
//            renderMenuTable(response.data);
//            getAccessName(accessCodeId);
//        },
//        error: function (err) {
//            console.error('Failed to load menu data:', err);
//        }
//    });
//}

//#region Delete Selected

$('#deleteSelectedBtn').on('click', function () {
    const selectedIds = $('.checkBox:checked').map(function () {
        return $(this).val();
    }).get();

    if (selectedIds.length === 0) {
        alert('No items selected.');
        return;
    }

    if (confirm(`Delete ${selectedIds.length} item(s)?`)) {
        $.ajax({
            url: '/menuTab/DeleteAccessCodes',
            method: 'POST',
            data: JSON.stringify(selectedIds),
            contentType: 'application/json',
            success: function () {
                const currentId = $('#access-code-id').val().trim();
                loadAccessList();
                loadAccessCodeDropdown();
                if (selectedIds.includes(currentId)) {
                    $('#access-code-id').val('');
                    $('#access-code-name').val('');
                    getMenuData(null);
                }
            }
        });
    }
});

// Delete Selected
//$('#deleteSelectedBtn').on('click', function () {
//    const selectedIds = $('.checkBox:checked').map(function () {
//        return $(this).val(); // .val() instead of .data('id')
//    }).get();

//    if (selectedIds.length === 0) {
//        alert('No items selected.');
//        return;
//    }
//    console.log(selectedIds)

//    if (confirm(`Delete ${selectedIds.length} item(s)?`)) {
//        $.ajax({
//            url: '/menuTab/DeleteAccessCodes',
//            method: 'POST',
//            data: JSON.stringify(selectedIds),
//            contentType: 'application/json',
//            success: function () {
//                loadAccessList();
//                getMenuData(null);

//                const currentid = $('#access-code-id').val().trim();
//                if (selectedIds.includes(currentid)) {
//                    $('#access-code-id').val('');
//                    $('#access-code-name').val('');
//                }
//            }
//        });
//    }
//});
//#endregion Delete Selected

//#region Modal

function loadAccessModal() {
    $("#addAccessCodeContent").html('<div class="text-center"><i class="fa fa-spinner fa-spin fa-3x"></i></div>');

    $.ajax({
        url: '/MenuTab/AccessCodeIndex',
        data: { child: true },
        type: 'GET',
        success: function (res) {
            if (res.message) {
                showNotification(res.message, 'warning');
                return;
            }

            $("#addAccessCodeContent").html(res);

            if (typeof $.accessCodeEntries === 'function') {
                var options = {
                    isModal: true,
                    onSaved: function (e) {
                        if (e) {
                            loadAccessCodeDropdown(e);
                            getMenuData(e);
                        } else {
                            loadAccessCodeDropdown(null);
                            getMenuData(null);
                        }
                        loadAccessList();
                        loadAccessCodeDropdown();
                        $("#addAccessCodeModal").modal("hide");
                    },
                    onDelete: function () {
                        const currentId = $('#access-code-id').val().trim();
                        loadAccessList();
                        loadAccessCodeDropdown();
                        //if (selectedIds.includes(currentId)) {
                        $('#access-code-id').val('');
                        $('#access-code-name').val('');
                        getMenuData(null);
                        //}
                        loadAccessCodeDropdown(null);

                        $("#addAccessCodeModal").modal("hide");
                    }
                };
                $.accessCodeEntries(options);
            }
            $("#addAccessCodeModal").modal("show");
        },
        error: function (error) {
            $("#addAccessCodeContent").html('<div class="alert alert-danger">Error loading content. Please try again.</div>');
        }
    })
}

//#endregion Modal


//#region Main Table

function renderMenuTable(data) {
    console.log(data);
    const $tableBody = $('#menuTabLoad');
    $tableBody.empty();

    function renderRow(item, level) {
        const indent = '&nbsp;'.repeat(level * 4);
        const isParent = item.children && item.children.length > 0;
        const row = `
            <tr class="${isParent ? 'bg-light fw-bold parent-row' : 'child-row'}" 
                data-menuid="${item.menuId}" 
                data-parentid="${item.parentId || ''}"
                data-level="${level}"
                ${level > 0 ? 'style="display:none;"' : ''}>
                
                <td class="text-center align-middle">
                    <input class="row-main-check" type="checkbox">
                </td>
                <td class="${isParent ? 'fw-bold' : 'text-primary'}" style="cursor:${isParent ? 'pointer' : 'default'}">
                    ${indent}
                    ${isParent ? '<span class="toggle-icon me-1">▶</span>' : ''}
                    ${item.title || ''}
                </td>
                <td class="text-center">
                    <input class="add-check" type="checkbox" ${item.checkAdd ? 'checked' : ''}>
                </td>
                <td class="text-center">
                    <input class="edit-check" type="checkbox" ${item.checkEdit ? 'checked' : ''}>
                </td>
                <td class="text-center">
                    <input class="delete-check" type="checkbox" ${item.checkDelete ? 'checked' : ''}>
                </td>
                <td class="text-center">
                    <input class="print-check" type="checkbox" ${item.checkPrint ? 'checked' : ''}>
                </td>
            </tr>
        `;
        $tableBody.append(row);

        if (isParent) {
            item.children.forEach(child => renderRow(child, level + 1));
        }
    }

    data.forEach(item => renderRow(item, 0));

    // Collapse/Expand logic
    $tableBody.off('click', '.parent-row').on('click', '.parent-row', function (e) {
        if ($(e.target).is('input')) return;

        const parentId = $(this).data('menuid');
        const $icon = $(this).find('.toggle-icon');
        const isCollapsed = $icon.text().trim() === '▶';

        $icon.text(isCollapsed ? '▼' : '▶');
        toggleChildren(parentId, isCollapsed);
    });

    function toggleChildren(parentId, show) {
        $tableBody.find(`tr[data-parentid="${parentId}"]`).each(function () {
            if (show) {
                $(this).show();
            } else {
                $(this).hide();
                const childId = $(this).data('menuid');
                if ($(this).hasClass('parent-row')) {
                    $(this).find('.toggle-icon').text('▶');
                    toggleChildren(childId, false);
                }
            }
        });
    }
}

//function setupCheckboxLogic11() {
//    // ✅ Select All checkbox
//    $('#selectAll').on('change', function () {
//        const checked = $(this).is(':checked');
//        $('.row-main-check, .add-check, .edit-check, .delete-check, .print-check').prop('checked', checked);
//        $('#addSelectAll, #editSelectAll, #deleteSelectAll, #printSelectAll').prop('checked', checked);
//    });

//    // ✅ Left-side Main checkbox (row) checked -> check rights + check all children recursively
//    $('#menuTabLoad').on('change', '.row-main-check', function () {
//        const $row = $(this).closest('tr');
//        const isChecked = $(this).is(':checked');

//        // Check/uncheck its own rights
//        $row.find('.add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);

//        const menuId = $row.data('menuid');

//        // Recursively check/uncheck children
//        checkChildren(menuId, isChecked);

//        updateSelectAllState();
//    });

//    // ✅ Individual right checkbox (add/edit/delete/print) changes
//    $('#menuTabLoad').on('change', '.add-check, .edit-check, .delete-check, .print-check', function () {
//        const $row = $(this).closest('tr');
//        const allRightsChecked = $row.find('.add-check, .edit-check, .delete-check, .print-check').filter(':checked').length === 4;
//        $row.find('.row-main-check').prop('checked', allRightsChecked);

//        const menuId = $row.data('menuid');

//        // If parent is checked, propagate individual right to children too
//        const rightClass = $(this).attr('class').split(' ')[1]; // get 'add-check' etc.
//        const isRightChecked = $(this).is(':checked');

//        checkRightInChildren(menuId, rightClass, isRightChecked);

//        updateSelectAllState();
//    });

//    // ✅ Update "Select All" checkbox based on individual row checkboxes
//    function updateSelectAllState() {
//        const totalBoxes = $('.row-main-check').length;
//        const checkedBoxes = $('.row-main-check:checked').length;
//        $('#selectAll').prop('checked', totalBoxes === checkedBoxes);

//        ['add', 'edit', 'delete', 'print'].forEach(function (type) {
//            const total = $(`.${type}-check`).length;
//            const checked = $(`.${type}-check:checked`).length;
//            $(`#${type}SelectAll`).prop('checked', total === checked);
//        });
//    }

//    // ✅ Recursively check/uncheck child rows (full hierarchy)
//    function checkChildren(parentId, isChecked) {
//        $(`tr[data-parentid="${parentId}"]`).each(function () {
//            $(this).find('.row-main-check').prop('checked', isChecked);
//            $(this).find('.add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);

//            const childMenuId = $(this).data('menuid');
//            checkChildren(childMenuId, isChecked); // Recursive for grandchildren
//        });
//    }

//    // ✅ Recursively check/uncheck specific right (add/edit/delete/print) in children
//    function checkRightInChildren(parentId, rightClass, isChecked) {
//        $(`tr[data-parentid="${parentId}"]`).each(function () {
//            $(this).find(`.${rightClass}`).prop('checked', isChecked);

//            const childMenuId = $(this).data('menuid');
//            checkRightInChildren(childMenuId, rightClass, isChecked); // Recursive for grandchildren
//        });
//    }

//    $('#addSelectAll').on('change', function () {
//        const checked = $(this).is(':checked');
//        $('.add-check').prop('checked', checked);
//    });

//    $('#editSelectAll').on('change', function () {
//        const checked = $(this).is(':checked');
//        $('.edit-check').prop('checked', checked);
//    });

//    $('#deleteSelectAll').on('change', function () {
//        const checked = $(this).is(':checked');
//        $('.delete-check').prop('checked', checked);
//    });

//    $('#printSelectAll').on('change', function () {
//        const checked = $(this).is(':checked');
//        $('.print-check').prop('checked', checked);
//    });
//}


function setupCheckboxLogic11() {
    $('#selectAll').on('change', function () {
        const checked = $(this).is(':checked');
        $('.row-main-check, .add-check, .edit-check, .delete-check, .print-check').prop('checked', checked);
        $('#addSelectAll, #editSelectAll, #deleteSelectAll, #printSelectAll').prop('checked', checked);
    });

    $('#menuTabLoad').on('change', '.row-main-check', function () {
        const $row = $(this).closest('tr');
        const isChecked = $(this).is(':checked');
        $row.find('.add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);
        checkChildren($row.data('menuid'), isChecked);
        updateSelectAllState();
    });

    $('#menuTabLoad').on('change', '.add-check, .edit-check, .delete-check, .print-check', function () {
        const $row = $(this).closest('tr');
        const allRightsChecked = $row.find('.add-check, .edit-check, .delete-check, .print-check').filter(':checked').length === 4;
        $row.find('.row-main-check').prop('checked', allRightsChecked);
        const rightClass = $(this).attr('class').split(' ')[0];
        checkRightInChildren($row.data('menuid'), rightClass, $(this).is(':checked'));
        updateSelectAllState();
    });

    function updateSelectAllState() {
        const totalBoxes = $('.row-main-check').length;
        const checkedBoxes = $('.row-main-check:checked').length;
        $('#selectAll').prop('checked', totalBoxes === checkedBoxes);
        ['add', 'edit', 'delete', 'print'].forEach(function (type) {
            const total = $(`.${type}-check`).length;
            const checked = $(`.${type}-check:checked`).length;
            $(`#${type}SelectAll`).prop('checked', total === checked);
        });
    }

    function checkChildren(parentId, isChecked) {
        $(`tr[data-parentid="${parentId}"]`).each(function () {
            $(this).find('.row-main-check').prop('checked', isChecked);
            $(this).find('.add-check, .edit-check, .delete-check, .print-check').prop('checked', isChecked);
            checkChildren($(this).data('menuid'), isChecked);
        });
    }

    function checkRightInChildren(parentId, rightClass, isChecked) {
        $(`tr[data-parentid="${parentId}"]`).each(function () {
            $(this).find(`.${rightClass}`).prop('checked', isChecked);
            checkRightInChildren($(this).data('menuid'), rightClass, isChecked);
        });
    }

    $('#addSelectAll').on('change', function () { $('.add-check').prop('checked', $(this).is(':checked')); });
    $('#editSelectAll').on('change', function () { $('.edit-check').prop('checked', $(this).is(':checked')); });
    $('#deleteSelectAll').on('change', function () { $('.delete-check').prop('checked', $(this).is(':checked')); });
    $('#printSelectAll').on('change', function () { $('.print-check').prop('checked', $(this).is(':checked')); });
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

function loadAccessCodeDropdown(selectAfterLoad) {
    $.ajax({
        url: '/menuTab/GetAccessListTable',
        type: 'GET',
        dataType: 'json',
        success: function (response) {
            const list = response.data ?? response;

            const $select = $('#access-code-id');
            const currentVal = selectAfterLoad || $select.val();
            $select.empty().append('<option value="">-- Select --</option>');
            list.forEach(function (item) {
                $select.append(
                    $('<option>', { value: item.accessCodeId, text: item.accessCodeName })
                );
            });
            if (currentVal) {
                $select.val(currentVal);
            }
        },
        error: function (err) {
            console.error('Failed to load access codes:', err);
        }
    });
}

function loadParentMenus() {
    $.ajax({
        url: '/MenuTab/GetParentMenus',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            const $select = $('#module-select');
            $select.empty().append('<option value="">-- All --</option>');
            data.forEach(function (item) {
                $select.append(
                    $('<option>', { value: item.menuId, text: item.title })
                );
            });
        },
        error: function (err) {
            console.error('Failed to load parent menus:', err);
        }
    });
}

function applyModuleFilter(moduleId) {
    if (!moduleId) {
        // Show only level-0 rows; children remain collapsed as per renderMenuTable default
        $('#menuTabLoad tr').each(function () {
            const level = parseInt($(this).data('level'));
            if (level === 0) {
                $(this).show();
            } else {
                $(this).hide();
                // Reset toggle icon
                $(this).find('.toggle-icon').text('▶');
            }
        });
        return;
    }

    // Hide everything first
    $('#menuTabLoad tr').hide();

    // Show the selected parent row
    const $parentRow = $(`#menuTabLoad tr[data-menuid="${moduleId}"]`);
    $parentRow.show();
    $parentRow.find('.toggle-icon').text('▼');

    // Show all its direct and nested children
    showAllChildren(moduleId);
}

function showAllChildren(parentId) {
    $(`#menuTabLoad tr[data-parentid="${parentId}"]`).each(function () {
        $(this).show();
        const childId = $(this).data('menuid');
        if ($(this).hasClass('parent-row')) {
            $(this).find('.toggle-icon').text('▼'); // ✅ expand nested parents too
        }
        showAllChildren(childId);
    });
}


function getAccessName(accessCodeId) {
    if (!accessCodeId) return;
    $.ajax({
        url: '/MenuTab/GetAccessName',
        method: 'GET',
        data: { accessCodeId },
        dataType: 'text',
        success: function (response) {
            $('#access-code-name').val(response);
        },
        error: function (err) {
            console.error('Failed to load access name:', err);
        }
    });
}
function getMenuData(accessCodeId) {
    const moduleId = $('#module-select').val();
    //console.log(moduleId);
    //if (!accessCodeId || !moduleId) {
    //    renderMenuTable([]);
    //    return;
    //}
    $.ajax({
        url: '/MenuTab/AccessMenus',
        method: 'GET',
        data: { accessCodeId },
        dataType: 'json',
        success: function (response) {
            renderMenuTable(response.data);
            // Re-apply module filter after table re-render
            //const moduleId = $('#module-select').val();
            applyModuleFilter(moduleId);
        },
        error: function (err) {
            console.error('Failed to load menu data:', err);
        }
    });
}

function updateTableVisibility() {
    const hasAccessCode = !!$('#access-code-id').val();
    const hasModule = !!$('#module-select').val();

    if (hasAccessCode && hasModule)
        $('.menuTableWrapper').show();
    else
        $('.menuTableWrapper').hide();
}

//#endregion  backup
function showNotification(message, type) {
    if (typeof toastr !== 'undefined') {
        toastr[type](message, type === 'success' ? 'Success' : type === 'error' ? 'Error' : 'Warning');
    } else {
        alert(message);
    }
}
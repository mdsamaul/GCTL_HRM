

// Initialize everything when document is ready
$(document).ready(function () {
    refreshMenuTable();

    initializeMultiDeleteUI();

    // Event handler for save button
    $("#saveOrderBtn").off('click').on('click', function () {
        saveMenuOrder();
    });

    // Add Font Awesome if not already included
    if ($('link[href*="font-awesome"]').length === 0) {
        $('head').append('<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/5.15.4/css/all.min.css">');
    }

    // Add additional styles
    $('<style>')
        .text(`
            .ui-state-highlight {
                height: 40px;
                border: 2px dashed #007bff;
                background-color: #e9ecef;
            }
            .menu-item {
                transition: background-color 0.2s ease;
            }
            .menu-item:hover {
                background-color: #f2f2f2;
            }
            .toggle-icon {
                cursor: pointer;
                transition: transform 0.2s ease;
            }
            .expanded .toggle-icon.fa-caret-down {
                transform: rotate(0deg);
            }
            .sortable-table tr.ui-sortable-helper {
                box-shadow: 0 4px 8px rgba(0,0,0,0.1);
            }
        `)
        .appendTo('head');



    getNextOrderId()
    getNextId()
    getParentMenuList()

    $('#ParentId').on('change', function () {
        getNextOrderId()
        var selectedValue = $(this).val();
        //getnextOder(selectedValue);

        getChildByParents(selectedValue);

    })


    $('#ChildId').on('change', function () {
        getNextOrderId()
        var selectedValue = $(this).val();
        //getnextOder(selectedValue);
        getGChildByChild(selectedValue);
    })

    $('#GChildId').on('change', function () {
        getNextOrderId()

    })

    //#region Enable/Disable Button

    $("#ParentId, #ChildId, #GChildId").prop('disabled', true);

    $("#ParentToggle").prop('checked', false);
    $("#ChildToggle").prop('checked', false);
    $("#GrandChildToggle").prop('checked', false);



    // ParentToggle behavior
    $("#ParentToggle").on("change", function () {
        const isChecked = $(this).is(":checked");

        if (!isChecked) {
            // If ParentToggle is unchecked, also uncheck Child and GrandChild
            $("#ChildToggle").prop("checked", false);
            $("#GrandChildToggle").prop("checked", false);
        }
    });

    // ChildToggle behavior
    //$("#ChildToggle").on("change", function () {
    //    const isChecked = $(this).is(":checked");

    //    if (!isChecked) {
    //        // If ChildToggle is unchecked, also uncheck GrandChild
    //        $("#GrandChildToggle").prop("checked", false);
    //    }
    //});


    // ChildToggle behavior
    $("#ChildToggle").on("change", function () {
        const isChecked = $(this).is(":checked");

        if (!$("#ParentToggle").is(":checked")) {
            toastr.warning("Enable Parent first.");
            $(this).prop("checked", false);
            return;
        }

        if (!isChecked) {
            $("#GrandChildToggle").prop("checked", false).trigger("change");
        }
    });


    // GrandChildToggle behavior
    $("#GrandChildToggle").on("change", function () {
        if (!$("#ChildToggle").is(":checked")) {
            toastr.warning("Enable Child first.");
            $(this).prop("checked", false);
            return;
        }
    });






    // Parent Switch
    $("#ParentToggle").change(function () {

        if ($(this).is(":checked")) {
            $("#ParentId").prop('disabled', false);
        } else {
            $("#ParentId").prop('disabled', true);
        }

        getNextOrderId()

    });

    // Child Switch
    $("#ChildToggle").change(function () {

        if ($(this).is(":checked")) {
            $("#ChildId").prop('disabled', false);
        } else {
            $("#ChildId").prop('disabled', true);
        }

        getNextOrderId()
    });

    // Grand Child Switch
    $("#GrandChildToggle").change(function () {

        if ($(this).is(":checked")) {
            $("#GChildId").prop('disabled', false);
        } else {
            $("#GChildId").prop('disabled', true);
        }

        getNextOrderId()
    });

    //#endregion Enable/Disable Button



    //#region ResetButton


    // Reset button handler
    $("#resetButton").on("click", function () {
        clearNavigationForm()
    });


    //#endregion ResetButton




});


//#region Clear Form

function clearNavigationForm() {
    // Reset form fields
    $('#navigationForm')[0].reset();

    // Manually clear fields (because .reset() might not clear readonly or pre-set values)
    $('#MenuCode').val('');
    $('#OrderBy').val('');
    $('#AutoId').val('0');
    $('#Title').val('');
    $('#ControllerName').val('');
    $('#ViewName').val('');
    $('#Icon').val('');

    // Reset dropdowns
    $('#ParentId').prop('selectedIndex', 0);
    $('#ChildId').prop('selectedIndex', 0);
    $('#GChildId').prop('selectedIndex', 0);

    // Reset toggle switches (checkboxes)
    $('#ParentToggle').prop('checked', false);
    $('#ChildToggle').prop('checked', false);
    $('#GrandChildToggle').prop('checked', false);
    $('#IsActive').prop('checked', true);


    getNextOrderId()
    getNextId()
    getParentMenuList()
}


//#endregion clear form


//#region Drag and Save Order

// Global variables
let originalOrderMap = {};
let currentSortColumn = "orderBy";
let currentSortDirection = "asc";
let selectedItems = []; // Track selected items for multi-delete

// Main function to refresh the menu table
function refreshMenuTable() {
    $.ajax({
        url: "/api/CoreMenuTab/GetAll222",
        type: "GET",
        success: function (response) {
            const menus = response.data;
            console.log("Response Data:", menus);

            $("#menuTableBody").empty();
            originalOrderMap = {};
            selectedItems = []; // Reset selected items when refreshing
            updateDeleteButtonState(); // Update delete button state

            // Render the menu items
            menus.forEach(menu => {
                const level = menu.level || 0;
                const indent = "&nbsp;".repeat(level * 4);
                const rowId = `row-${menu.menuId}`;
                const parentAttr = menu.parentId ? `data-parent="${menu.parentId}"` : "";
                const hasChildren = menus.some(m => m.parentId === menu.menuId);

                // Use appropriate icon based on whether it has children
                const toggleIcon = hasChildren ?
                    `<span class="toggle-icon fa fa-caret-right" style="margin-right: 5px; transition: transform 0.2s; cursor: pointer;"></span>` :
                    `<span style="margin-right: 15px;"></span>`;

                // Add checkbox for multi-select
                const checkbox = `<input type="checkbox" class="menu-checkbox" data-id="${menu.autoId}" style="margin-right: 8px;">`;

                const row = $(`
                    <tr id="${rowId}" class="menu-item level-${level}" 
                        data-id="${menu.autoId}" 
                        data-menuid="${menu.menuId}"
                        ${parentAttr}
                        data-level="${level}"
                        data-has-children="${hasChildren}"
                        data-order="${menu.orderBy}"
                        data-title="${menu.title}"
                        data-controller="${menu.controllerName || ''}"
                        data-view="${menu.viewName || ''}"
                        data-active="${menu.isActive ? 'true' : 'false'}"
                        style="cursor: move;">
                        <td>${checkbox}${indent}${toggleIcon} <span class="menu-title-text" style="cursor: pointer;">${menu.title}</span></td>
                        <td>${menu.orderBy}</td>
                        <td>${menu.title}</td>
                        <td>${menu.controllerName || ''}</td>
                        <td>${menu.viewName || ''}</td>
                        <td>${menu.isActive ? 'Yes' : 'No'}</td>
                    </tr>
                `);

                originalOrderMap[menu.autoId] = {
                    parentId: menu.parentId || "Root",
                    orderBy: menu.orderBy,
                    level: level
                };

                $("#menuTableBody").append(row);
            });

            initializeColumnSorting();
            initializeToggleLogic();
            initializeSortable();
            initializeCheckboxes(); // Initialize checkbox functionality
            applyHierarchyStyles();
        },
        error: function (xhr, status, error) {
            console.error("Error fetching menu data:", error);
            toastr.warning("Failed to load menu data. Please try again.");
        }
    });
}

// Initialize checkbox functionality
function initializeCheckboxes() {
    // Handle individual checkbox clicks
    $(document).off('click', '.menu-checkbox').on('click', '.menu-checkbox', function (e) {
        e.stopPropagation(); // Prevent row toggle when clicking checkbox

        const id = $(this).data('id');

        if ($(this).is(':checked')) {
            if (!selectedItems.includes(id)) {
                selectedItems.push(id);
            }
        } else {
            selectedItems = selectedItems.filter(item => item !== id);
        }

        // Update select all checkbox state
        const allChecked = $('.menu-checkbox').length === selectedItems.length;
        $('#selectAllCheckbox').prop('checked', allChecked);

        updateDeleteButtonState();
    });

    // Handle select all checkbox
    $(document).off('click', '#selectAllCheckbox').on('click', '#selectAllCheckbox', function () {
        const isChecked = $(this).is(':checked');

        $('.menu-checkbox').prop('checked', isChecked);

        if (isChecked) {
            selectedItems = [];
            $('.menu-checkbox').each(function () {
                selectedItems.push($(this).data('id'));
            });
        } else {
            selectedItems = [];
        }

        updateDeleteButtonState();
    });
}

// Update delete button state based on selection
function updateDeleteButtonState() {
    if (selectedItems.length > 0) {
        $('#deleteSelectedBtn').prop('disabled', false);
    } else {
        $('#deleteSelectedBtn').prop('disabled', true);
    }
}

// Initialize column sorting functionality
function initializeColumnSorting() {
    $("#menuTable thead th").css("cursor", "pointer").off("click").on("click", function () {
        const column = $(this).text().toLowerCase();

        // Map column header text to data attribute
        const columnMap = {
            "menu": "title",
            "order": "order",
            "title": "title",
            "controllername": "controllername",
            "action": "viewname",
            "active": "active"
        };

        const dataAttribute = columnMap[column];

        if (!dataAttribute) return;

        if (currentSortColumn === dataAttribute) {
            // Toggle sort direction if clicking the same column
            currentSortDirection = currentSortDirection === "asc" ? "desc" : "asc";
        } else {
            currentSortColumn = dataAttribute;
            currentSortDirection = "asc";
        }

        sortTableBySiblings(dataAttribute, currentSortDirection);
    });
}

// Sort table while preserving the parent-child relationships
function sortTableBySiblings(column, direction) {
    // Group rows by parent
    const rowsByParent = {};

    $("#menuTableBody tr").each(function () {
        const $row = $(this);
        const parentId = $row.data("parent") || "Root";

        if (!rowsByParent[parentId]) {
            rowsByParent[parentId] = [];
        }

        rowsByParent[parentId].push($row);
    });

    // Sort each group of siblings
    for (const parentId in rowsByParent) {
        rowsByParent[parentId].sort(function (a, b) {
            let valueA = $(a).data(column);
            let valueB = $(b).data(column);

            // Handle numeric values
            if (!isNaN(valueA) && !isNaN(valueB)) {
                valueA = parseFloat(valueA);
                valueB = parseFloat(valueB);
            }

            // Handle direction
            const compareResult = valueA < valueB ? -1 : valueA > valueB ? 1 : 0;
            return direction === "asc" ? compareResult : -compareResult;
        });
    }

    // Apply the new order, starting with root items
    $("#menuTableBody").empty();

    function appendRowsWithChildren(parentId) {
        if (!rowsByParent[parentId]) return;

        rowsByParent[parentId].forEach(function ($row) {
            $("#menuTableBody").append($row);
            const menuId = $row.data("menuid");

            // Check if this row was expanded before sorting
            const wasExpanded = $row.hasClass("expanded");

            // If it was expanded, make sure we expand it after re-ordering
            if (wasExpanded && rowsByParent[menuId]) {
                appendRowsWithChildren(menuId);
            } else if (rowsByParent[menuId]) {
                // Otherwise, add them but keep them hidden
                rowsByParent[menuId].forEach($childRow => {
                    $("#menuTableBody").append($childRow);
                    $childRow.hide();
                });
            }
        });
    }

    appendRowsWithChildren("Root");

    // Re-initialize toggle logic and drag-drop
    applyHierarchyStyles();
    updateToggleIcons();

    // Restore checkbox states after reordering
    restoreCheckboxStates();
}

// Restore checkbox states after table reordering
function restoreCheckboxStates() {
    $('.menu-checkbox').each(function () {
        const id = $(this).data('id');
        $(this).prop('checked', selectedItems.includes(id));
    });
}

function initializeToggleLogic() {
    // Hide children initially
    $("tr[data-parent]").each(function () {
        const parent = $(this).attr("data-parent");
        if (parent && parent !== "Root" && parent !== "0") {
            $(this).hide();
        }
    });

    // Handle toggle clicks with smooth animation
    $(document).off('click', '.toggle-icon').on('click', '.toggle-icon', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const $row = $(this).closest("tr");
        const menuId = $row.data("menuid");
        const hasChildren = $("tr[data-parent='" + menuId + "']").length > 0;

        if (!hasChildren) return;

        const isExpanded = $row.hasClass("expanded");

        if (isExpanded) {
            // Collapse
            collapseRow(menuId);
            $row.removeClass("expanded");
            $(this).removeClass("fa-caret-down").addClass("fa-caret-right");
        } else {
            // Expand
            expandRow(menuId);
            $row.addClass("expanded");
            $(this).removeClass("fa-caret-right").addClass("fa-caret-down");
        }
    });

    // // Handle menu item title click to open edit modal
    //$(document).off('click', '.menu-title-text').on('click', '.menu-title-text', function (e) {
    //    e.preventDefault();
    //    e.stopPropagation();

    //    const $row = $(this).closest("tr");
    //    const menuId = $row.data("menuid");
    //    const autoId = $row.data("id");
    //    const title = $row.data("title");
    //    const controller = $row.data("controller");
    //    const view = $row.data("view");
    //    const isActive = $row.data("active") === "true";

    //    // Populate modal fields
    //    $("#editMenuId").val(autoId);
    //    $("#editMenuTitle").val(title);
    //    $("#editMenuController").val(controller);
    //    $("#editMenuView").val(view);
    //    $("#editMenuActive").prop("checked", isActive);

    //    // Show the modal
    //    $("#editMenuModal").modal("show");
    //});

    $(document).off('click', '.menu-title-text').on('click', '.menu-title-text', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const $row = $(this).closest("tr");
        const autoId = $row.data("id");

        
  

        // Make AJAX call to get additional details
        $.ajax({
            url: '/coreMenu/getMenuById', // Replace with the actual endpoint
            type: 'GET',
            data: { id: autoId },
            success: function (response) {
                console.log(response)
                if (response.success) {
                    $("#editMenuModal").modal("show");
                    $("#editMenuId").val(response.data.autoId);
                    $("#editMenuTitle").val(response.data.title);
                    $("#editMenuController").val(response.data.controllerName);
                    $("#editMenuView").val(response.data.viewName);
                    $("#editMenuIcon").val(response.data.icon);
                    $("#editMenuActive").prop("checked", response.data.isActive === true);
                }
            },
            error: function (xhr, status, error) {
                console.error("Error fetching menu details:", error);
            }
        });
    });

}

// Expand function with smooth animation
function expandRow(parentId) {
    $(`tr[data-parent='${parentId}']`).each(function () {
        $(this).show(150);
    });
}

// Collapse function with smooth animation (recursive)
function collapseRow(parentId) {
    $(`tr[data-parent='${parentId}']`).each(function () {
        $(this).hide(100);

        // If this child was expanded, collapse its children too
        if ($(this).hasClass("expanded")) {
            collapseRow($(this).data("menuid"));
            $(this).removeClass("expanded");
            $(this).find(".toggle-icon").removeClass("fa-caret-down").addClass("fa-caret-right");
        }
    });
}

// Update toggle icons after reordering
function updateToggleIcons() {
    $("tr.menu-item").each(function () {
        const $row = $(this);
        const menuId = $row.data("menuid");
        const hasChildren = $row.data("has-children");
        const isExpanded = $row.hasClass("expanded");

        if (hasChildren) {
            const $icon = $row.find(".toggle-icon");
            if (isExpanded) {
                $icon.removeClass("fa-caret-right").addClass("fa-caret-down");
            } else {
                $icon.removeClass("fa-caret-down").addClass("fa-caret-right");
            }
        }
    });
}

function initializeSortable() {
    $("#menuTableBody").sortable({
        items: "> tr",
        handle: "td:first-child",
        helper: fixHelper,
        placeholder: "ui-state-highlight",
        axis: "y", // Restrict movement to vertical only
        tolerance: "pointer", // Use pointer position for determining placement
        start: function (event, ui) {
            // Save all the children of the dragged item
            const draggedRow = ui.item;
            const draggedMenuId = draggedRow.data("menuid");

            // Store the parent before dragging
            draggedRow.data('original-parent', draggedRow.data("parent") || "Root");
            draggedRow.data('original-level', parseInt(draggedRow.data("level") || 0));

            // Only collapse the children of the dragged item
            if (draggedRow.hasClass("expanded")) {
                collapseRow(draggedMenuId);
                draggedRow.removeClass("expanded");
                draggedRow.find(".toggle-icon").removeClass("fa-caret-down").addClass("fa-caret-right");
                draggedRow.data('was-expanded', true);
            }

            // Find and save all children rows recursively
            const childrenRows = getChildrenRows(draggedMenuId);
            draggedRow.data('children', childrenRows);

            // Hide children during drag
            childrenRows.forEach(child => {
                $(child).hide();
            });

            // Add visual indicator
            ui.helper.addClass("dragging").css({
                "border": "2px solid #007bff",
                "background-color": "#f8f9fa"
            });
        },
        stop: function (event, ui) {
            const movedRow = ui.item;
            const movedMenuId = movedRow.data("menuid");
            const movedChildren = movedRow.data('children') || [];
            const wasExpanded = movedRow.data('was-expanded') || false;

            // Get new parent based on position
            const newParent = determineNewParent(movedRow);
            movedRow.data("parent", newParent);
            movedRow.attr("data-parent", newParent);

            // Adjust level based on new parent
            const newLevel = newParent === "Root" ? 0 : parseInt($(`tr[data-menuid="${newParent}"]`).data("level")) + 1;
            movedRow.data("level", newLevel);
            movedRow.attr("data-level", newLevel);
            movedRow.removeClass().addClass(`menu-item level-${newLevel}`);

            // Insert children after the moved row
            if (movedChildren.length > 0) {
                let lastRow = movedRow;
                movedChildren.forEach(child => {
                    // Adjust child's level relative to parent's new level
                    const childElement = $(child);
                    const childOriginalLevel = parseInt(childElement.data("level") || 0);
                    const childParentId = childElement.data("parent");

                    // Calculate the level difference between this child and its direct parent
                    const directParentElement = $(`tr[data-menuid="${childParentId}"]`);
                    const directParentLevel = parseInt(directParentElement.data("level") || 0);
                    const levelDifference = childOriginalLevel - directParentLevel;

                    // Apply the same difference from the new parent level
                    const newChildLevel = parseInt(directParentElement.data("level") || 0) + levelDifference;

                    childElement.data("level", newChildLevel);
                    childElement.attr("data-level", newChildLevel);
                    childElement.removeClass().addClass(`menu-item level-${newChildLevel}`);

                    lastRow.after(childElement);
                    lastRow = childElement;
                });
            }

            // If it was expanded before, expand it again
            if (wasExpanded) {
                expandRow(movedMenuId);
                movedRow.addClass("expanded");
                movedRow.find(".toggle-icon").removeClass("fa-caret-right").addClass("fa-caret-down");
                movedRow.removeData('was-expanded');
            }

            // Clean up temp data
            movedRow.removeData('children');
            movedRow.removeData('original-parent');
            movedRow.removeData('original-level');

            // Update order numbers
            updateOrderNumbers();

            // Re-apply styles
            applyHierarchyStyles();
        },
        change: function (event, ui) {
            // Visual feedback during drag
            $(".ui-state-highlight").css({
                "height": "40px",
                "border": "2px dashed #007bff",
                "background-color": "#e9ecef"
            });
        }
    }).disableSelection();
}

// Helper function to determine the new parent based on position
function determineNewParent(row) {
    const prevRow = row.prev();
    const nextRow = row.next();

    // If no previous row, this could be the first item
    if (prevRow.length === 0) {
        // Check if there's a next row to determine root or not
        if (nextRow.length === 0) {
            return "Root"; // No siblings, must be root
        }

        // If next row has a parent, use that
        const nextParent = nextRow.data("parent");
        if (nextParent && nextParent !== "Root") {
            return nextParent;
        }
        return "Root"; // Default to root if can't determine
    }

    // Check the previous row to determine parent
    const prevLevel = parseInt(prevRow.data("level") || 0);
    const currentLevel = parseInt(row.data("level") || 0);

    // If previous row is at the same level, they share a parent
    if (prevLevel === currentLevel) {
        return prevRow.data("parent") || "Root";
    }

    // If previous row is at a lower level (higher in hierarchy), 
    // it could be the parent or we need to find another ancestor
    if (prevLevel < currentLevel) {
        // If exactly one level difference, previous row is the parent
        if (prevLevel === currentLevel - 1) {
            return prevRow.data("menuid");
        }

        // Otherwise, we need to find the proper ancestor
        let parent = prevRow;
        while (parent.length > 0) {
            if (parseInt(parent.data("level") || 0) === currentLevel - 1) {
                return parent.data("menuid");
            }

            // Move to parent's parent
            const parentId = parent.data("parent");
            if (!parentId || parentId === "Root") {
                return "Root";
            }
            parent = $(`tr[data-menuid="${parentId}"]`);
        }
        return "Root"; // Fallback
    }

    // If previous row is at a higher level (deeper in hierarchy),
    // they might share a parent or the item might be moving up
    if (prevLevel > currentLevel) {
        // Find the closest row at the same level
        let current = prevRow;
        while (current.length > 0 && parseInt(current.data("level") || 0) > currentLevel) {
            current = current.prev();
        }

        if (current.length > 0 && parseInt(current.data("level") || 0) === currentLevel) {
            // Found a row at the same level, use its parent
            return current.data("parent") || "Root";
        }
    }

    // Default to previous row's parent if we can't determine
    return prevRow.data("parent") || "Root";
}

// Helper function to fix cell widths during drag
function fixHelper(e, ui) {
    ui.children().each(function () {
        $(this).width($(this).width());
    });
    return ui;
}

// Helper function to get ALL children recursively
function getChildrenRows(parentMenuId) {
    const children = [];

    $(`tr[data-parent='${parentMenuId}']`).each(function () {
        children.push(this);

        const childMenuId = $(this).data("menuid");
        const subChildren = getChildrenRows(childMenuId);
        children.push(...subChildren);
    });

    return children;
}

// Update order numbers for all rows
function updateOrderNumbers() {
    // Group by parent
    const siblingGroups = {};

    $("#menuTableBody tr").each(function () {
        const parentId = $(this).data("parent") || "Root";

        if (!siblingGroups[parentId]) {
            siblingGroups[parentId] = [];
        }

        siblingGroups[parentId].push($(this));
    });

    // Update order for each group of siblings
    for (const parentId in siblingGroups) {
        siblingGroups[parentId].forEach((row, index) => {
            const newOrder = index + 1;
            row.find("td:eq(1)").text(newOrder);
            row.data("order", newOrder);
            row.attr("data-order", newOrder);
        });
    }

    // Enable save button since the order has changed
    $("#saveOrderBtn").prop('disabled', false);
}

// Apply visual styles based on hierarchy level
function applyHierarchyStyles() {
    // Apply visual differentiation based on level
    for (let i = 0; i <= 5; i++) {
        $(`.level-${i}`).css({
            'background-color': `rgba(240, 240, 240, ${0.15 * i})`,
            'transition': 'background-color 0.3s ease'
        });
    }

    // Update indentation for all rows
    $("#menuTableBody tr").each(function () {
        const level = $(this).data("level") || 0;
        const indent = "&nbsp;".repeat(level * 4);

        const $firstCell = $(this).find("td:first-child");
        // Keep the checkbox at the beginning followed by indentation
        const cellContent = $firstCell.html();
        const checkboxPart = cellContent.substring(0, cellContent.indexOf('>') + 1);
        const restOfCell = cellContent.substring(cellContent.indexOf('>') + 1).replace(/^(&nbsp;)+/, '');

        $firstCell.html(checkboxPart + indent + restOfCell);
    });
}

// Function to save menu order
function saveMenuOrder() {
    const orderData = [];

    $("#menuTableBody tr").each(function () {
        orderData.push({
            autoId: $(this).data("id"),
            orderBy: parseInt($(this).find("td:eq(1)").text()) || $(this).data("order"),
            parentId: $(this).data("parent") === "Root" ? null : $(this).data("parent")
        });
    });

    console.log("Order Data to Send:", orderData);

    $.ajax({
        url: "/CoreMenuTab/UpdateOrder",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(orderData),
        success: function (response) {
            toastr.warning("Sort order and hierarchy saved successfully!");
            $("#saveOrderBtn").prop('disabled', true);
            refreshMenuTable();
        },
        error: function (xhr, status, error) {
            console.error("Error saving order:", error);
            toastr.warning("Failed to save sort order.");
        }
    });
}

//#endregion drag and save order

//#region Multi Delete Functionality

// Function to delete selected menu items
function deleteSelectedMenuItems() {
    if (selectedItems.length === 0) {
        toastr.warning("No items selected for deletion.");
        return;
    }

    // Check if any selected items have children
    const hasChildren = checkSelectedItemsHaveChildren();

    if (hasChildren) {
        // Show confirmation with warning about deleting parent items
        if (!confirm("Some selected items have child menus. Deleting these items will also delete all their children. Are you sure you want to continue?")) {
            return;
        }
    } else {
        // Show standard confirmation
        if (!confirm("Are you sure you want to delete the selected menu items?")) {
            return;
        }
    }

    // Proceed with deletion
    $.ajax({
        url: "/CoreMenuTab/DeleteMultiple",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(selectedItems),
        success: function (response) {
            if (response.success) {
                toastr.success(`Successfully deleted ${selectedItems.length} menu item(s).`);
                // Reset selected items and refresh table
                selectedItems = [];
                refreshMenuTable();
            } else {
                toastr.error(response.message || "Failed to delete menu items.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error deleting menu items:", error);
            toastr.error("An error occurred while deleting menu items.");
        }
    });
}

// Check if any selected items have children
function checkSelectedItemsHaveChildren() {
    for (const id of selectedItems) {
        const menuId = $(`tr[data-id="${id}"]`).data("menuid");
        if ($(`tr[data-parent="${menuId}"]`).length > 0) {
            return true;
        }
    }
    return false;
}

// Function to initialize the UI for multi-delete
function initializeMultiDeleteUI() {
    // Add select all checkbox to table header
    const tableHeader = $("#menuTable thead tr");

    // Check if the first column already has the checkbox
    const firstHeaderCell = tableHeader.find("th:first");
    if (!firstHeaderCell.find("#selectAllCheckbox").length) {
        // Create a checkbox with a label
        const selectAllCheckbox = `<input type="checkbox" id="selectAllCheckbox" style="margin-right: 10px;"> `;

        // Prepend to the first header cell
        firstHeaderCell.prepend(selectAllCheckbox);
    }

    // Add delete button to the actions area (assuming there's a container for actions)
    const actionsContainer = $("#menuActionsContainer");

    // If container exists, add the button; otherwise, create one
    if (actionsContainer.length) {
        if (!actionsContainer.find("#deleteSelectedBtn").length) {
            const deleteButton = `
                <button id="deleteSelectedBtn" class="btn btn-danger" disabled>
                    <i class="fa fa-trash"></i> Delete Selected
                </button>
            `;
           // actionsContainer.append(deleteButton);
        }
    } else {
        // Create a new actions container above the table
        const tableContainer = $("#menuTable").parent();
        const newActionsContainer = `
            <div id="menuActionsContainer" style="margin-bottom: 15px; display: flex; justify-content: space-between;">
                <div>
                    <button id="saveOrderBtn" class="btn btn-primary" disabled>
                        <i class="fa fa-save"></i> Save Order
                    </button>
                </div>
                <div>
                    <button id="deleteSelectedBtn" class="btn btn-danger" disabled>
                        <i class="fa fa-trash"></i> Delete Selected
                    </button>
                </div>
            </div>
        `;
       // tableContainer.prepend(newActionsContainer);
    }

    // Attach event handler to delete button
    $(document).off('click', '#deleteSelectedBtn').on('click', '#deleteSelectedBtn', function () {
        deleteSelectedMenuItems();
    });
}

// Call this function when the page loads
$(document).ready(function () {
    initializeMultiDeleteUI();
    initializeEditModal();
    // Other initialization code...
});

//#endregion Multi Delete Functionality

//#region Edit Menu Modal Functionality

// Function to create and initialize the edit modal
function initializeEditModal() {
    // Check if modal already exists
    if ($("#editMenuModal").length === 0) {
        // Create modal HTML
        const modalHTML = `
        <div class="modal fade" id="editMenuModal" tabindex="-1" role="dialog" aria-labelledby="editMenuModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="editMenuModalLabel">Edit Menu Item</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <form id="editMenuForm">
                            <input type="hidden" id="editMenuId">
                            <div class="form-group">
                                <label for="editMenuTitle">Title</label>
                                <input type="text" class="form-control" id="editMenuTitle" placeholder="Enter menu title">
                            </div>
                            <div class="form-group">
                                <label for="editMenuController">Controller Name</label>
                                <input type="text" class="form-control" id="editMenuController" placeholder="Enter controller name">
                            </div>
                            <div class="form-group">
                                <label for="editMenuView">View/Action Name</label>
                                <input type="text" class="form-control" id="editMenuView" placeholder="Enter view/action name">
                            </div>
                            <div class="form-group">
                                <label for="editMenuIcon">Icon Name</label>
                                <input type="text" class="form-control" id="editMenuIcon" placeholder="Enter Icon name">
                            </div>
                            <div class="form-check">
                                <input type="checkbox" class="form-check-input" id="editMenuActive">
                                <label class="form-check-label" for="editMenuActive">Active</label>
                            </div>
                        </form>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">Cancel</button>
                        <button type="button" class="btn btn-primary" id="saveMenuChangesBtn">Save Changes</button>
                    </div>
                </div>
            </div>
        </div>
        `;

       
       $("body").append(modalHTML);

        // Initialize save button event handler
        $(document).off('click', '#saveMenuChangesBtn').on('click', '#saveMenuChangesBtn', function () {
            saveMenuChanges();
        });
    }
}

// Function to save menu changes
function saveMenuChanges() {
    const menuData = {
        autoId: $("#editMenuId").val(),
        title: $("#editMenuTitle").val(),
        controllerName: $("#editMenuController").val(),
        viewName: $("#editMenuView").val(),
        icon: $("#editMenuIcon").val(),
        isActive: $("#editMenuActive").is(":checked")
    };

    // Validate required fields
    if (!menuData.title) {
        toastr.warning("Title is required.");
        return;
    }

    // Send update request
    $.ajax({
        url: "/CoreMenuTab/UpdateMenu",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(menuData),
        success: function (response) {
            if (response.success) {
                // Close modal
                $("#editMenuModal").modal("hide");

                // Show success message
                toastr.success(response.message || "Menu item updated successfully!");

                // Refresh table to show updated data
                refreshMenuTable();
            } else {
                toastr.error(response.message || "Failed to update menu item.");
            }
        },
        error: function (xhr, status, error) {
            console.error("Error updating menu item:", error);
            toastr.error("An error occurred while updating the menu item.");
        }
    });
}

//#endregion Edit Menu Modal Functionality

//#region DDOrder

function getChildByParents(selectedValue) {
    $.ajax({
        type: "GET",
        url: "/CoreMenu/getChildByParents",
        data: { parentId: selectedValue },
        success: function (data) {
            console.log(`Response for ParentId ${selectedValue}:`, data);

            if (data.success) {

                const dropdown = $("#ChildId");

                dropdown.empty();


                dropdown.append($(`<option value="">Select Child</option>`));


                data.data.forEach((child) => {
                    dropdown.append($(`<option value="${child.menuId}">${child.title}</option>`));
                });
            } else {
                console.warn(`No children found for ParentId ${selectedValue}.`);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching next order:", error);
        }
    });

}

function getGChildByChild(selectedValue) {
    $.ajax({
        type: "GET",
        url: "/CoreMenu/getGChildByChild",
        data: { childId: selectedValue },
        success: function (data) {
            console.log(`Response for ChildId ${selectedValue}:`, data);
            if (data.success) {
                const dropdown = $("#GChildId");
                dropdown.empty();
                dropdown.append($(`<option value="">Select Grand Child</option>`));
                data.data.forEach((child) => {
                    dropdown.append($(`<option value="${child.menuId}">${child.title}</option>`));
                });
            } else {
                console.warn(`No children found for ParentId ${selectedValue}.`);
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching next order:", error);
        }
    });
}


//#endregion


//#region order id get

function getNextOrderId() {
    var isParentChecked = $("#ParentToggle").is(':checked');
    var isChildChecked = $("#ChildToggle").is(':checked');
    var isGrandChildChecked = $("#GrandChildToggle").is(':checked');

    var parentId = isParentChecked ? $("#ParentId").val() : null;
    var childId = isChildChecked ? $("#ChildId").val() : null;
    var grandChildId = isGrandChildChecked ? $("#GChildId").val() : null;

    $.ajax({
        type: "GET",
        url: "/CoreMenu/GetOrderIdAll",
        data: {
            parentId: parentId,
            childId: childId,
            grandChildId: grandChildId
        },
        success: function (data) {
            console.log("Order ID:", data); // Log the order ID to the console
            $("#OrderBy").val(data);
        },
        error: function (xhr, status, error) {
            console.error("Error fetching order ID:", error);
        }
    });
}

//#endregion order id get



//#region PageLoad From




function getNextId() {
    $.ajax({
        type: "GET",
        url: "/CoreMenu/GetNextId",
        success: function (data) {
            console.log("Next Id:", data); // Log the next Id to the console
            $("#MenuCode").val(data.id);
        },
        error: function (xhr, status, error) {
            console.error("Error fetching next Id:", error);
        }
    });
}

function getParentMenuList() {
    $.ajax({
        type: "GET",
        url: "/CoreMenu/GetParentMenuList",
        success: function (data) {
            console.log("Parent Menu List:", data); // Log the parent menu list to the console
            var select = $("#ParentId");
            //select.empty(); // Clear existing options
            $.each(data.data.data, function (index, item) {
                select.append($('<option></option>').val(item.menuId).text(item.title));
            });
        },
        error: function (xhr, status, error) {
            console.error("Error fetching parent menu list:", error);
        }
    });
}

//#endregion


//#region Form Submission need test

$("#submitCoreMenuTabBtn").on("click", function (event) {
    event.preventDefault(); // Prevent default form submission

    // Clear previous validation messages
    $("[id^='Va']").text("");

    var isParentChecked = $("#ParentToggle").is(':checked');
    var isChildChecked = $("#ChildToggle").is(':checked');
    var isGrandChildChecked = $("#GrandChildToggle").is(':checked');

    var parentId = isParentChecked ? $("#ParentId").val() : null;
    var childId = isChildChecked ? $("#ChildId").val() : null;
    var grandChildId = isGrandChildChecked ? $("#GChildId").val() : null;



    // Get form values
    const formData = {
        AutoId: parseInt($("#AutoId").val()) || 0, // Assuming AutoId is a hidden field
        MenuId: $("#MenuCode").val(),
        Title: $("#Title").val(),
        ControllerName: $("#ControllerName").val(),
        OrderBy: parseInt($("#OrderBy").val()),
        ViewName: $("#ViewName").val(),
        IsActive: $("#IsActive").is(":checked"),
        Icon: $("#Icon").val(),

        ParentId: parentId,
        ChildId: childId,
        GrandChildId: grandChildId

        //IsParentNode: $("#ParentToggle").is(":checked"),
        //IsChildNode: $("#ChildToggle").is(":checked"),
        //IsGrandChildNode: $("#GrandChildToggle").is(":checked"),
    };

    console.log("Form Data:", formData); // Log the form data to the console

    // Validate required fields
    let isValid = true;

    if (!formData.Title.trim()) {
        $("#VaTitle").text("Title is required");
        isValid = false;
    }

    if (!formData.ControllerName.trim()) {
        $("#VaControllerName").text("Controller Name is required");
        isValid = false;
    }

    if (isValid) {
        $("#submitCoreMenuTabBtn").html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Saving...').prop("disabled", true);

    }


    saveMenuData(formData);
});


function saveMenuData(formData) {
    
    $.ajax({
        url: "/CoreMenuTab/Save",
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify(formData),
        success: function (response) {
            console.log("Response Submit:", response);
            if (response.success) {
                toastr.success(response.message);

                if (!formData.MenuCode) {
                    $("#MenuCode").val(response.data.menuCode);
                }

                clearNavigationForm();

                getNextOrderId()
                getNextId()
                getParentMenuList()
                //refreshMenuTable1()

            } else {
                toastr.error(response.message || "Failed to save menu item");

                if (response.errors) {
                    $.each(response.errors, function (field, error) {
                        $("#Va" + field).text(error);
                    });
                }
            }
        },
        error: function (xhr, status, error) {
            toastr.error("An error occurred while saving: " + error);
            console.error(xhr.responseText);
        },
        complete: function () {
            // Reset button state
            $("#submitCoreMenuTabBtn").html('Save').prop("disabled", false);
        }
    });

}

//#endregion



//#region Backup Drag and Save Order And Multi Delete

////#region Drag and Save Order

//// Global variables
//let originalOrderMap = {};
//let currentSortColumn = "orderBy";
//let currentSortDirection = "asc";
//let selectedItems = []; // Track selected items for multi-delete

//// Main function to refresh the menu table
//function refreshMenuTable() {
//    $.ajax({
//        url: "/api/CoreMenuTab/GetAll222",
//        type: "GET",
//        success: function (response) {
//            const menus = response.data;
//            console.log("Response Data:", menus);

//            $("#menuTableBody").empty();
//            originalOrderMap = {};
//            selectedItems = []; // Reset selected items when refreshing
//            updateDeleteButtonState(); // Update delete button state

//            // Render the menu items
//            menus.forEach(menu => {
//                const level = menu.level || 0;
//                const indent = "&nbsp;".repeat(level * 4);
//                const rowId = `row-${menu.menuId}`;
//                const parentAttr = menu.parentId ? `data-parent="${menu.parentId}"` : "";
//                const hasChildren = menus.some(m => m.parentId === menu.menuId);

//                // Use appropriate icon based on whether it has children
//                const toggleIcon = hasChildren ?
//                    `<span class="toggle-icon fa fa-caret-right" style="margin-right: 5px; transition: transform 0.2s; cursor: pointer;"></span>` :
//                    `<span style="margin-right: 15px;"></span>`;

//                // Add checkbox for multi-select
//                const checkbox = `<input type="checkbox" class="menu-checkbox" data-id="${menu.autoId}" style="margin-right: 8px;">`;

//                const row = $(`
//                    <tr id="${rowId}" class="menu-item level-${level}" 
//                        data-id="${menu.autoId}" 
//                        data-menuid="${menu.menuId}"
//                        ${parentAttr}
//                        data-level="${level}"
//                        data-has-children="${hasChildren}"
//                        data-order="${menu.orderBy}"
//                        data-title="${menu.title}"
//                        style="cursor: move;">
//                        <td>${checkbox}${indent}${toggleIcon} ${menu.title}</td>
//                        <td>${menu.orderBy}</td>
//                        <td>${menu.title}</td>
//                        <td>${menu.controllerName || ''}</td>
//                        <td>${menu.viewName || ''}</td>
//                        <td>${menu.isActive ? 'Yes' : 'No'}</td>
//                    </tr>
//                `);

//                originalOrderMap[menu.autoId] = {
//                    parentId: menu.parentId || "Root",
//                    orderBy: menu.orderBy,
//                    level: level
//                };

//                $("#menuTableBody").append(row);
//            });

//            initializeColumnSorting();
//            initializeToggleLogic();
//            initializeSortable();
//            initializeCheckboxes(); // Initialize checkbox functionality
//            applyHierarchyStyles();
//        },
//        error: function (xhr, status, error) {
//            console.error("Error fetching menu data:", error);
//            toastr.warning("Failed to load menu data. Please try again.");
//        }
//    });
//}

//// Initialize checkbox functionality
//function initializeCheckboxes() {
//    // Handle individual checkbox clicks
//    $(document).off('click', '.menu-checkbox').on('click', '.menu-checkbox', function (e) {
//        e.stopPropagation(); // Prevent row toggle when clicking checkbox

//        const id = $(this).data('id');

//        if ($(this).is(':checked')) {
//            if (!selectedItems.includes(id)) {
//                selectedItems.push(id);
//            }
//        } else {
//            selectedItems = selectedItems.filter(item => item !== id);
//        }

//        // Update select all checkbox state
//        const allChecked = $('.menu-checkbox').length === selectedItems.length;
//        $('#selectAllCheckbox').prop('checked', allChecked);

//        updateDeleteButtonState();
//    });

//    // Handle select all checkbox
//    $(document).off('click', '#selectAllCheckbox').on('click', '#selectAllCheckbox', function () {
//        const isChecked = $(this).is(':checked');

//        $('.menu-checkbox').prop('checked', isChecked);

//        if (isChecked) {
//            selectedItems = [];
//            $('.menu-checkbox').each(function () {
//                selectedItems.push($(this).data('id'));
//            });
//        } else {
//            selectedItems = [];
//        }

//        updateDeleteButtonState();
//    });
//}

//// Update delete button state based on selection
//function updateDeleteButtonState() {
//    if (selectedItems.length > 0) {
//        $('#deleteSelectedBtn').prop('disabled', false);
//    } else {
//        $('#deleteSelectedBtn').prop('disabled', true);
//    }
//}

//// Initialize column sorting functionality
//function initializeColumnSorting() {
//    $("#menuTable thead th").css("cursor", "pointer").off("click").on("click", function () {
//        const column = $(this).text().toLowerCase();

//        // Map column header text to data attribute
//        const columnMap = {
//            "menu": "title",
//            "order": "order",
//            "title": "title",
//            "controllername": "controllername",
//            "action": "viewname",
//            "active": "active"
//        };

//        const dataAttribute = columnMap[column];

//        if (!dataAttribute) return;

//        if (currentSortColumn === dataAttribute) {
//            // Toggle sort direction if clicking the same column
//            currentSortDirection = currentSortDirection === "asc" ? "desc" : "asc";
//        } else {
//            currentSortColumn = dataAttribute;
//            currentSortDirection = "asc";
//        }

//        sortTableBySiblings(dataAttribute, currentSortDirection);
//    });
//}

//// Sort table while preserving the parent-child relationships
//function sortTableBySiblings(column, direction) {
//    // Group rows by parent
//    const rowsByParent = {};

//    $("#menuTableBody tr").each(function () {
//        const $row = $(this);
//        const parentId = $row.data("parent") || "Root";

//        if (!rowsByParent[parentId]) {
//            rowsByParent[parentId] = [];
//        }

//        rowsByParent[parentId].push($row);
//    });

//    // Sort each group of siblings
//    for (const parentId in rowsByParent) {
//        rowsByParent[parentId].sort(function (a, b) {
//            let valueA = $(a).data(column);
//            let valueB = $(b).data(column);

//            // Handle numeric values
//            if (!isNaN(valueA) && !isNaN(valueB)) {
//                valueA = parseFloat(valueA);
//                valueB = parseFloat(valueB);
//            }

//            // Handle direction
//            const compareResult = valueA < valueB ? -1 : valueA > valueB ? 1 : 0;
//            return direction === "asc" ? compareResult : -compareResult;
//        });
//    }

//    // Apply the new order, starting with root items
//    $("#menuTableBody").empty();

//    function appendRowsWithChildren(parentId) {
//        if (!rowsByParent[parentId]) return;

//        rowsByParent[parentId].forEach(function ($row) {
//            $("#menuTableBody").append($row);
//            const menuId = $row.data("menuid");

//            // Check if this row was expanded before sorting
//            const wasExpanded = $row.hasClass("expanded");

//            // If it was expanded, make sure we expand it after re-ordering
//            if (wasExpanded && rowsByParent[menuId]) {
//                appendRowsWithChildren(menuId);
//            } else if (rowsByParent[menuId]) {
//                // Otherwise, add them but keep them hidden
//                rowsByParent[menuId].forEach($childRow => {
//                    $("#menuTableBody").append($childRow);
//                    $childRow.hide();
//                });
//            }
//        });
//    }

//    appendRowsWithChildren("Root");

//    // Re-initialize toggle logic and drag-drop
//    applyHierarchyStyles();
//    updateToggleIcons();

//    // Restore checkbox states after reordering
//    restoreCheckboxStates();
//}

//// Restore checkbox states after table reordering
//function restoreCheckboxStates() {
//    $('.menu-checkbox').each(function () {
//        const id = $(this).data('id');
//        $(this).prop('checked', selectedItems.includes(id));
//    });
//}

//function initializeToggleLogic() {
//    // Hide children initially
//    $("tr[data-parent]").each(function () {
//        const parent = $(this).attr("data-parent");
//        if (parent && parent !== "Root" && parent !== "0") {
//            $(this).hide();
//        }
//    });

//    // Handle toggle clicks with smooth animation
//    $(document).off('click', '.toggle-icon').on('click', '.toggle-icon', function (e) {
//        e.preventDefault();
//        e.stopPropagation();

//        const $row = $(this).closest("tr");
//        const menuId = $row.data("menuid");
//        const hasChildren = $("tr[data-parent='" + menuId + "']").length > 0;

//        if (!hasChildren) return;

//        const isExpanded = $row.hasClass("expanded");

//        if (isExpanded) {
//            // Collapse
//            collapseRow(menuId);
//            $row.removeClass("expanded");
//            $(this).removeClass("fa-caret-down").addClass("fa-caret-right");
//        } else {
//            // Expand
//            expandRow(menuId);
//            $row.addClass("expanded");
//            $(this).removeClass("fa-caret-right").addClass("fa-caret-down");
//        }
//    });
//}

//// Expand function with smooth animation
//function expandRow(parentId) {
//    $(`tr[data-parent='${parentId}']`).each(function () {
//        $(this).show(150);
//    });
//}

//// Collapse function with smooth animation (recursive)
//function collapseRow(parentId) {
//    $(`tr[data-parent='${parentId}']`).each(function () {
//        $(this).hide(100);

//        // If this child was expanded, collapse its children too
//        if ($(this).hasClass("expanded")) {
//            collapseRow($(this).data("menuid"));
//            $(this).removeClass("expanded");
//            $(this).find(".toggle-icon").removeClass("fa-caret-down").addClass("fa-caret-right");
//        }
//    });
//}

//// Update toggle icons after reordering
//function updateToggleIcons() {
//    $("tr.menu-item").each(function () {
//        const $row = $(this);
//        const menuId = $row.data("menuid");
//        const hasChildren = $row.data("has-children");
//        const isExpanded = $row.hasClass("expanded");

//        if (hasChildren) {
//            const $icon = $row.find(".toggle-icon");
//            if (isExpanded) {
//                $icon.removeClass("fa-caret-right").addClass("fa-caret-down");
//            } else {
//                $icon.removeClass("fa-caret-down").addClass("fa-caret-right");
//            }
//        }
//    });
//}

//function initializeSortable() {
//    $("#menuTableBody").sortable({
//        items: "> tr",
//        handle: "td:first-child",
//        helper: fixHelper,
//        placeholder: "ui-state-highlight",
//        axis: "y", // Restrict movement to vertical only
//        tolerance: "pointer", // Use pointer position for determining placement
//        start: function (event, ui) {
//            // Save all the children of the dragged item
//            const draggedRow = ui.item;
//            const draggedMenuId = draggedRow.data("menuid");

//            // Store the parent before dragging
//            draggedRow.data('original-parent', draggedRow.data("parent") || "Root");
//            draggedRow.data('original-level', parseInt(draggedRow.data("level") || 0));

//            // Only collapse the children of the dragged item
//            if (draggedRow.hasClass("expanded")) {
//                collapseRow(draggedMenuId);
//                draggedRow.removeClass("expanded");
//                draggedRow.find(".toggle-icon").removeClass("fa-caret-down").addClass("fa-caret-right");
//                draggedRow.data('was-expanded', true);
//            }

//            // Find and save all children rows recursively
//            const childrenRows = getChildrenRows(draggedMenuId);
//            draggedRow.data('children', childrenRows);

//            // Hide children during drag
//            childrenRows.forEach(child => {
//                $(child).hide();
//            });

//            // Add visual indicator
//            ui.helper.addClass("dragging").css({
//                "border": "2px solid #007bff",
//                "background-color": "#f8f9fa"
//            });
//        },
//        stop: function (event, ui) {
//            const movedRow = ui.item;
//            const movedMenuId = movedRow.data("menuid");
//            const movedChildren = movedRow.data('children') || [];
//            const wasExpanded = movedRow.data('was-expanded') || false;

//            // Get new parent based on position
//            const newParent = determineNewParent(movedRow);
//            movedRow.data("parent", newParent);
//            movedRow.attr("data-parent", newParent);

//            // Adjust level based on new parent
//            const newLevel = newParent === "Root" ? 0 : parseInt($(`tr[data-menuid="${newParent}"]`).data("level")) + 1;
//            movedRow.data("level", newLevel);
//            movedRow.attr("data-level", newLevel);
//            movedRow.removeClass().addClass(`menu-item level-${newLevel}`);

//            // Insert children after the moved row
//            if (movedChildren.length > 0) {
//                let lastRow = movedRow;
//                movedChildren.forEach(child => {
//                    // Adjust child's level relative to parent's new level
//                    const childElement = $(child);
//                    const childOriginalLevel = parseInt(childElement.data("level") || 0);
//                    const childParentId = childElement.data("parent");

//                    // Calculate the level difference between this child and its direct parent
//                    const directParentElement = $(`tr[data-menuid="${childParentId}"]`);
//                    const directParentLevel = parseInt(directParentElement.data("level") || 0);
//                    const levelDifference = childOriginalLevel - directParentLevel;

//                    // Apply the same difference from the new parent level
//                    const newChildLevel = parseInt(directParentElement.data("level") || 0) + levelDifference;

//                    childElement.data("level", newChildLevel);
//                    childElement.attr("data-level", newChildLevel);
//                    childElement.removeClass().addClass(`menu-item level-${newChildLevel}`);

//                    lastRow.after(childElement);
//                    lastRow = childElement;
//                });
//            }

//            // If it was expanded before, expand it again
//            if (wasExpanded) {
//                expandRow(movedMenuId);
//                movedRow.addClass("expanded");
//                movedRow.find(".toggle-icon").removeClass("fa-caret-right").addClass("fa-caret-down");
//                movedRow.removeData('was-expanded');
//            }

//            // Clean up temp data
//            movedRow.removeData('children');
//            movedRow.removeData('original-parent');
//            movedRow.removeData('original-level');

//            // Update order numbers
//            updateOrderNumbers();

//            // Re-apply styles
//            applyHierarchyStyles();
//        },
//        change: function (event, ui) {
//            // Visual feedback during drag
//            $(".ui-state-highlight").css({
//                "height": "40px",
//                "border": "2px dashed #007bff",
//                "background-color": "#e9ecef"
//            });
//        }
//    }).disableSelection();
//}

//// Helper function to determine the new parent based on position
//function determineNewParent(row) {
//    const prevRow = row.prev();
//    const nextRow = row.next();

//    // If no previous row, this could be the first item
//    if (prevRow.length === 0) {
//        // Check if there's a next row to determine root or not
//        if (nextRow.length === 0) {
//            return "Root"; // No siblings, must be root
//        }

//        // If next row has a parent, use that
//        const nextParent = nextRow.data("parent");
//        if (nextParent && nextParent !== "Root") {
//            return nextParent;
//        }
//        return "Root"; // Default to root if can't determine
//    }

//    // Check the previous row to determine parent
//    const prevLevel = parseInt(prevRow.data("level") || 0);
//    const currentLevel = parseInt(row.data("level") || 0);

//    // If previous row is at the same level, they share a parent
//    if (prevLevel === currentLevel) {
//        return prevRow.data("parent") || "Root";
//    }

//    // If previous row is at a lower level (higher in hierarchy), 
//    // it could be the parent or we need to find another ancestor
//    if (prevLevel < currentLevel) {
//        // If exactly one level difference, previous row is the parent
//        if (prevLevel === currentLevel - 1) {
//            return prevRow.data("menuid");
//        }

//        // Otherwise, we need to find the proper ancestor
//        let parent = prevRow;
//        while (parent.length > 0) {
//            if (parseInt(parent.data("level") || 0) === currentLevel - 1) {
//                return parent.data("menuid");
//            }

//            // Move to parent's parent
//            const parentId = parent.data("parent");
//            if (!parentId || parentId === "Root") {
//                return "Root";
//            }
//            parent = $(`tr[data-menuid="${parentId}"]`);
//        }
//        return "Root"; // Fallback
//    }

//    // If previous row is at a higher level (deeper in hierarchy),
//    // they might share a parent or the item might be moving up
//    if (prevLevel > currentLevel) {
//        // Find the closest row at the same level
//        let current = prevRow;
//        while (current.length > 0 && parseInt(current.data("level") || 0) > currentLevel) {
//            current = current.prev();
//        }

//        if (current.length > 0 && parseInt(current.data("level") || 0) === currentLevel) {
//            // Found a row at the same level, use its parent
//            return current.data("parent") || "Root";
//        }
//    }

//    // Default to previous row's parent if we can't determine
//    return prevRow.data("parent") || "Root";
//}

//// Helper function to fix cell widths during drag
//function fixHelper(e, ui) {
//    ui.children().each(function () {
//        $(this).width($(this).width());
//    });
//    return ui;
//}

//// Helper function to get ALL children recursively
//function getChildrenRows(parentMenuId) {
//    const children = [];

//    $(`tr[data-parent='${parentMenuId}']`).each(function () {
//        children.push(this);

//        const childMenuId = $(this).data("menuid");
//        const subChildren = getChildrenRows(childMenuId);
//        children.push(...subChildren);
//    });

//    return children;
//}

//// Update order numbers for all rows
//function updateOrderNumbers() {
//    // Group by parent
//    const siblingGroups = {};

//    $("#menuTableBody tr").each(function () {
//        const parentId = $(this).data("parent") || "Root";

//        if (!siblingGroups[parentId]) {
//            siblingGroups[parentId] = [];
//        }

//        siblingGroups[parentId].push($(this));
//    });

//    // Update order for each group of siblings
//    for (const parentId in siblingGroups) {
//        siblingGroups[parentId].forEach((row, index) => {
//            const newOrder = index + 1;
//            row.find("td:eq(1)").text(newOrder);
//            row.data("order", newOrder);
//            row.attr("data-order", newOrder);
//        });
//    }

//    // Enable save button since the order has changed
//    $("#saveOrderBtn").prop('disabled', false);
//}

//// Apply visual styles based on hierarchy level
//function applyHierarchyStyles() {
//    // Apply visual differentiation based on level
//    for (let i = 0; i <= 5; i++) {
//        $(`.level-${i}`).css({
//            'background-color': `rgba(240, 240, 240, ${0.15 * i})`,
//            'transition': 'background-color 0.3s ease'
//        });
//    }

//    // Update indentation for all rows
//    $("#menuTableBody tr").each(function () {
//        const level = $(this).data("level") || 0;
//        const indent = "&nbsp;".repeat(level * 4);

//        const $firstCell = $(this).find("td:first-child");
//        // Keep the checkbox at the beginning followed by indentation
//        const cellContent = $firstCell.html();
//        const checkboxPart = cellContent.substring(0, cellContent.indexOf('>') + 1);
//        const restOfCell = cellContent.substring(cellContent.indexOf('>') + 1).replace(/^(&nbsp;)+/, '');

//        $firstCell.html(checkboxPart + indent + restOfCell);
//    });
//}

//// Function to save menu order
//function saveMenuOrder() {
//    const orderData = [];

//    $("#menuTableBody tr").each(function () {
//        orderData.push({
//            autoId: $(this).data("id"),
//            orderBy: parseInt($(this).find("td:eq(1)").text()) || $(this).data("order"),
//            parentId: $(this).data("parent") === "Root" ? null : $(this).data("parent")
//        });
//    });

//    console.log("Order Data to Send:", orderData);

//    $.ajax({
//        url: "/CoreMenuTab/UpdateOrder",
//        type: "POST",
//        contentType: "application/json",
//        data: JSON.stringify(orderData),
//        success: function (response) {
//            if (response.success) {
//                toastr.success(response.message || "Sort order and hierarchy saved successfully!");
//                $("#saveOrderBtn").prop('disabled', true);
//                refreshMenuTable();
//                // initializeMultiDeleteUI();
//            }

//        },
//        error: function (xhr, status, error) {
//            console.error("Error saving order:", error);
//            toastr.warning("Failed to save sort order.");
//        }
//    });
//}

////#endregion drag and save order

////#region Multi Delete Functionality

//// Function to delete selected menu items
//function deleteSelectedMenuItems() {
//    if (selectedItems.length === 0) {
//        toastr.warning("No items selected for deletion.");
//        return;
//    }

//    // Check if any selected items have children
//    const hasChildren = checkSelectedItemsHaveChildren();

//    if (hasChildren) {
//        // Show confirmation with warning about deleting parent items
//        if (!confirm("Some selected items have child menus. Deleting these items will also delete all their children. Are you sure you want to continue?")) {
//            return;
//        }
//    } else {
//        // Show standard confirmation
//        if (!confirm("Are you sure you want to delete the selected menu items?")) {
//            return;
//        }
//    }

//    // Proceed with deletion
//    $.ajax({
//        url: "/CoreMenuTab/DeleteMultipleMenu",
//        type: "POST",
//        contentType: "application/json",
//        data: JSON.stringify(selectedItems),
//        success: function (response) {
//            if (response.success) {
//                toastr.success(`Successfully deleted ${selectedItems.length} menu item(s).`);
//                // Reset selected items and refresh table
//                selectedItems = [];
//                refreshMenuTable();
//            } else {
//                toastr.error(response.message || "Failed to delete menu items.");
//            }
//        },
//        error: function (xhr, status, error) {
//            console.error("Error deleting menu items:", error);
//            toastr.error("An error occurred while deleting menu items.");
//        }
//    });
//}

//// Check if any selected items have children
//function checkSelectedItemsHaveChildren() {
//    for (const id of selectedItems) {
//        const menuId = $(`tr[data-id="${id}"]`).data("menuid");
//        if ($(`tr[data-parent="${menuId}"]`).length > 0) {
//            return true;
//        }
//    }
//    return false;
//}

//// Function to initialize the UI for multi-delete
//function initializeMultiDeleteUI() {
//    // Add select all checkbox to table header
//    const tableHeader = $("#menuTable thead tr");

//    // Check if the first column already has the checkbox
//    const firstHeaderCell = tableHeader.find("th:first");
//    if (!firstHeaderCell.find("#selectAllCheckbox").length) {
//        // Create a checkbox with a label
//        const selectAllCheckbox = `<input type="checkbox" id="selectAllCheckbox" style="margin-right: 10px;"> `;

//        // Prepend to the first header cell
//        firstHeaderCell.prepend(selectAllCheckbox);
//    }

//    // Add delete button to the actions area (assuming there's a container for actions)
//    const actionsContainer = $("#menuActionsContainer");

//    // If container exists, add the button; otherwise, create one
//    if (actionsContainer.length) {
//        if (!actionsContainer.find("#deleteSelectedBtn").length) {
//            const deleteButton = `
//                <button id="deleteSelectedBtn" class="btn btn-danger" disabled>
//                    <i class="fa fa-trash"></i> Delete Selected
//                </button>
//            `;
//            //actionsContainer.append(deleteButton);
//        }
//    } else {
//        // Create a new actions container above the table
//        const tableContainer = $("#menuTable").parent();
//        const newActionsContainer = `
//            <div id="menuActionsContainer" style="margin-bottom: 15px; display: flex; justify-content: space-between;">
//                <div>
//                    <button id="saveOrderBtn" class="btn btn-primary" disabled>
//                        <i class="fa fa-save"></i> Save Order
//                    </button>
//                </div>
//                <div>
//                    <button id="deleteSelectedBtn" class="btn btn-danger" disabled>
//                        <i class="fa fa-trash"></i> Delete Selected
//                    </button>
//                </div>
//            </div>
//        `;
//        //tableContainer.prepend(newActionsContainer);
//    }

//    // Attach event handler to delete button
//    $(document).off('click', '#deleteSelectedBtn').on('click', '#deleteSelectedBtn', function () {
//        deleteSelectedMenuItems();
//    });
//}



////#endregion Multi Delete Functionality


//#endregion Backup Drag and Save Order

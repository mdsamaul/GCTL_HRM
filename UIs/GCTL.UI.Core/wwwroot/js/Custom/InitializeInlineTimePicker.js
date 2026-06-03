// ============================================================
// Two Digit Limit Helper
// ============================================================
function applyTwoDigitLimit(container) {
    const numInputs = container.querySelectorAll(".numInput");
    numInputs.forEach(function (input) {
        input.addEventListener("keydown", function (e) {
            const allowedKeys = ["Backspace", "Delete", "ArrowLeft", "ArrowRight", "ArrowUp", "ArrowDown", "Tab"];
            if (allowedKeys.includes(e.key)) return;
            if (!/[0-9]/.test(e.key)) { e.preventDefault(); return; }
            const currentVal = String(input.value || "");
            if (currentVal.length <= 2 && input._isSelected) {
                input._isSelected = false;
                return;
            }
            if (currentVal.length >= 2) e.preventDefault();
        });
        input.addEventListener("focus", function () { input._isSelected = true; });
        input.addEventListener("mousedown", function () { input._isSelected = true; });
        input.addEventListener("input", function () {
            input._isSelected = false;
            if (String(input.value).length > 2) {
                input.value = String(input.value).slice(0, 2);
            }
        });
        input.addEventListener("change", function () { input._isSelected = false; });
    });
}

// ============================================================
// Flatpickr UI disable/enable helpers
// ============================================================
function _disableFlatpickrUI(container) {
    if (!container) return;
    container.querySelectorAll(".numInput").forEach(function (el) {
        el.disabled = true;
        el.style.pointerEvents = "none";
        el.style.opacity = "0.5";
    });
    container.querySelectorAll(".arrowUp, .arrowDown").forEach(function (el) {
        el.style.pointerEvents = "none";
        el.style.opacity = "0.5";
    });
    container.querySelectorAll(".flatpickr-am-pm").forEach(function (el) {
        el.style.pointerEvents = "none";
        el.style.opacity = "0.5";
    });
    container.style.pointerEvents = "none";
    container.style.opacity = "0.6";
}

function _enableFlatpickrUI(container) {
    if (!container) return;
    container.querySelectorAll(".numInput").forEach(function (el) {
        el.disabled = false;
        el.style.pointerEvents = "";
        el.style.opacity = "";
    });
    container.querySelectorAll(".arrowUp, .arrowDown").forEach(function (el) {
        el.style.pointerEvents = "";
        el.style.opacity = "";
    });
    container.querySelectorAll(".flatpickr-am-pm").forEach(function (el) {
        el.style.pointerEvents = "";
        el.style.opacity = "";
    });
    container.style.pointerEvents = "";
    container.style.opacity = "";
}

// ============================================================
// TimePicker (flatpickr inline)
// Usage:
//   initializeTimePicker("#inputId", "#hiddenInputId")
//   initializeTimePicker("#inputId", "#hiddenInputId", "08:30:00")
//   initializeTimePicker("#inputId", "#hiddenInputId", null, true)
//   initializeTimePicker("#inputId", "#hiddenInputId", "08:30:00", true)
//<div class="form-group">
//    <label for="outTimeInput" class="form-label">Out Time</label>
//    <div class="time-picker-wrapper">
//        <div id="outTimeInput" class="time-picker-container w-100 p-0 m-0"></div>
//    </div>
//    <input type="hidden" value="@(isEditing? Model.ShiftEndTime.ToString("hh: mm:ss tt") : null)" id="outDateTimeInput" name="ShiftEndTime" />
//</div>
// ============================================================
function initializeTimePicker(timeInputId, dateTimeInputId, defaultValue = null, disableTime = false) {
    const inputEl = document.querySelector(timeInputId);
    const hiddenEl = document.querySelector(dateTimeInputId);
    if (!inputEl) return;

    const existingHiddenValue = hiddenEl ? hiddenEl.value : null;

    if (inputEl._flatpickr) {
        try { inputEl._flatpickr.destroy(); } catch (e) { }
        inputEl._flatpickr = undefined;
    }

    let defaultDate = null;
    if (defaultValue) {
        const parsed = new Date("1970-01-01 " + defaultValue);
        if (!isNaN(parsed)) defaultDate = parsed;
    } else if (existingHiddenValue) {
        const parsed = new Date("1970-01-01 " + existingHiddenValue);
        if (!isNaN(parsed)) defaultDate = parsed;
    }


    if (!defaultDate) {
        defaultDate = new Date();
    }

    let _ready = false;

    flatpickr(inputEl, {
        enableTime: true,
        noCalendar: true,
        dateFormat: "h:i:s K",
        time_24hr: false,
        enableSeconds: true,
        inline: true,
        defaultDate: defaultDate,
        minuteIncrement: 1,
        secondIncrement: 1,
        onReady: function (selectedDates, dateStr, instance) {
            applyTwoDigitLimit(instance.calendarContainer);

            if (disableTime) {
                _disableFlatpickrUI(instance.calendarContainer);
            }

            if (hiddenEl) {
                hiddenEl.value = defaultDate.toLocaleTimeString('en-US', {
                    hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true
                });
            }

            _ready = true;
        },
        onChange: function (selectedDates) {
            if (!_ready) return;
            if (disableTime) return;
            if (selectedDates.length > 0 && hiddenEl) {
                hiddenEl.value = selectedDates[0].toLocaleTimeString('en-US', {
                    hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true
                });
            }
        }
    });
}





// ============================================================
// TimePicker destroy
// ============================================================
function destroyTimePicker(timeInputId) {
    const inputEl = document.querySelector(timeInputId);
    if (!inputEl) return;
    if (inputEl._flatpickr) {
        try { inputEl._flatpickr.destroy(); } catch (e) { }
        inputEl._flatpickr = undefined;
    }
}

// ============================================================
// TimePicker enable
// ============================================================
function enableTimePicker(timeInputId) {
    destroyTimePicker(timeInputId);
    const inputEl = document.querySelector(timeInputId);
    if (inputEl) inputEl.disabled = false;
    setTimeout(function () {
        initializeTimePicker(timeInputId, null, null, false);
    }, 50);
}

// ============================================================
// TimePicker disable
// ============================================================
function disableTimePicker(timeInputId, dateTimeInputId) {
    const inputEl = document.querySelector(timeInputId);
    if (inputEl) {
        inputEl.disabled = true;
        inputEl.value = '';
    }

    if (dateTimeInputId) {
        const hiddenEl = document.querySelector(dateTimeInputId);
        if (hiddenEl) hiddenEl.value = '';
    }

    if (inputEl && inputEl._flatpickr && inputEl._flatpickr.calendarContainer) {
        _disableFlatpickrUI(inputEl._flatpickr.calendarContainer);
    } else {
        setTimeout(function () {
            initializeTimePicker(timeInputId, dateTimeInputId || null, null, true);
        }, 50);
    }
}
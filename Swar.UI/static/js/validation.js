function validateField(fieldId, errorId, validationFn, errorMsg) {
  const field = document.getElementById(fieldId);
  const errorElement = document.getElementById(errorId);
  if (validationFn(field.value)) {
    errorElement.textContent = "";
    field.classList.remove("invalid-input");
    field.classList.add("valid-input");
  } else {
    errorElement.textContent = errorMsg;
    field.classList.remove("valid-input");
    field.classList.add("invalid-input");
  }
}

function validateText(field) {
  validateField(
    field,
    `${field}Error`,
    (value) => value.trim() !== "",
    `${field} is required.`
  );
}

function validateSelect(field) {
  validateField(
    field,
    `${field}Error`,
    (value) => value !== "None",
    `${field} is required.`
  );
}

function validateForm(entity) {
  if (entity === "playlist") {
    validateText("playlist-name");
    validateText("description");
  }

  const errors = document.querySelectorAll(".error");
  let formValid = true;
  errors.forEach((error) => {
    if (error.textContent !== "") {
      formValid = false;
    }
  });

  return formValid;
}

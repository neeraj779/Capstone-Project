const message = (function () {
  function showSuccessToast(title, message) {
    Swal.fire({
      icon: "success",
      title,
      html: `<div class="custom-alert"><p><span class="highlight">${message}</span> 🚀</p></div>`,
      showConfirmButton: false,
      toast: true,
      position: "top-end",
      timer: 2000,
      timerProgressBar: true,
      background: "#111828",
    });
  }

  function showAlert(icon, title, text) {
    Swal.fire({
      icon,
      title,
      text,
      background: "#1f2937",
      color: "#fff",
      confirmButtonColor: "#6b46c1",
    });
  }

  return {
    showSuccessToast,
    showAlert,
  };
})();

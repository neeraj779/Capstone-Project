document.addEventListener("DOMContentLoaded", () => {
  const handleSearchSubmit = (formId, inputId) => {
    const form = document.getElementById(formId);
    const input = document.getElementById(inputId);

    form.addEventListener("submit", (e) => {
      e.preventDefault();

      const query = input.value.trim();

      if (query)
        window.location.href = `./search.html?q=${encodeURIComponent(query)}`;
    });
  };

  handleSearchSubmit("search-form", "search-input");
  handleSearchSubmit("mobile-search-form", "mobile-search-input");
});

document.addEventListener("DOMContentLoaded", () => {
  const handleSearchSubmit = (formId, inputId) => {
    const form = document.getElementById(formId);
    const input = document.getElementById(inputId);

    form.addEventListener("submit", (e) => {
      e.preventDefault();

      const query = input.value.trim();

      if (query) {
        const currentUrl = new URL(window.location.href);
        const basePath = currentUrl.pathname.split("/").slice(0, -1).join("/");
        const searchUrl = new URL(
          `${basePath}/search.html`,
          window.location.origin
        );

        searchUrl.searchParams.set("q", query);
        window.location.href = searchUrl.toString();
      }
    });
  };

  handleSearchSubmit("search-form", "search-input");
  handleSearchSubmit("mobile-search-form", "mobile-search-input");
});

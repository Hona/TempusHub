window.searchFunctions = {
    clickSearch: function (id) {
        const searchButton = document.getElementById(id);
        // Next sibling is the dropdown
        if (!searchButton.nextElementSibling.classList.contains("show")) {
            searchButton.click();
        }
    }
};
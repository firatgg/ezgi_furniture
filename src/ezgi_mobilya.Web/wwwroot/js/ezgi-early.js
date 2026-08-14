(function () {
  var savedTheme = localStorage.getItem("ezgi-craft-theme") || "light";
  document.documentElement.setAttribute("data-theme", savedTheme);

  var savedScroll = sessionStorage.getItem("ezgi-craft-lang-scroll");
  if (savedScroll === null) {
    return;
  }

  history.scrollRestoration = "manual";
  var y = parseInt(savedScroll, 10);
  if (isNaN(y) || y < 0) {
    return;
  }

  var restore = function () {
    window.scrollTo(0, y);
  };

  restore();
  document.addEventListener("DOMContentLoaded", restore);
})();

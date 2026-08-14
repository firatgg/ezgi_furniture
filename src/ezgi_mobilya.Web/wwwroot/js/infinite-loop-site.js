(function () {
  function activateDeferredStylesheets() {
    document.querySelectorAll("link[data-defer-css]").forEach(function (link) {
      if (link.media === "all") {
        return;
      }

      var activate = function () {
        link.media = "all";
      };

      if (link.sheet) {
        activate();
      } else {
        link.addEventListener("load", activate);
      }

      // Fallback for cached stylesheets that may not fire load
      setTimeout(activate, 3000);
    });
  }

  activateDeferredStylesheets();

  function getOffSet() {
    var _offset = 450;
    var windowHeight = window.innerHeight;

    if (windowHeight > 500) {
      _offset = 400;
    }
    if (windowHeight > 680) {
      _offset = 300;
    }
    if (windowHeight > 830) {
      _offset = 210;
    }

    return _offset;
  }

  function setParallaxPosition($doc, multiplier, $object) {
    var offset = getOffSet();
    var from_top = $doc.scrollTop();
    var bg_css = "center " + (multiplier * from_top - offset) + "px";
    $object.css({ "background-position": bg_css });
  }

  var background_image_parallax = function ($object, multiplier, forceSet) {
    multiplier = typeof multiplier !== "undefined" ? multiplier : 0.5;
    multiplier = 1 - multiplier;
    var $doc = $(document);

    if (forceSet) {
      setParallaxPosition($doc, multiplier, $object);
    } else {
      $(window).scroll(function () {
        setParallaxPosition($doc, multiplier, $object);
      });
    }
  };

  var background_image_parallax_2 = function ($object, multiplier) {
    multiplier = typeof multiplier !== "undefined" ? multiplier : 0.5;
    multiplier = 1 - multiplier;
    var isMobileViewport = window.matchMedia("(max-width: 991px)").matches;

    if (isMobileViewport) {
      $object.css({
        "background-attachment": "scroll",
        "background-position": "center center",
      });
      return;
    }

    $object.css({ "background-attachment": "fixed" });

    $(window).scroll(function () {
      if ($(window).width() > 991) {
        var firstTop = $object.offset().top;
        var pos = $(window).scrollTop();
        var yPos = Math.round(multiplier * (firstTop - pos) - 186);
        var bg_css = "center " + yPos + "px";
        $object.css({ "background-position": bg_css });
      } else {
        $object.css({
          "background-attachment": "scroll",
          "background-position": "center center",
        });
      }
    });
  };

  $(function () {
    var themeToggleBtn = document.getElementById("themeToggle");
    if (themeToggleBtn) {
      var themeIcon = themeToggleBtn.querySelector("i");

      function updateToggleIcon(theme) {
        if (!themeIcon) {
          return;
        }
        themeIcon.className = theme === "dark" ? "fas fa-sun" : "fas fa-moon";
      }

      var currentTheme = document.documentElement.getAttribute("data-theme") || "light";
      updateToggleIcon(currentTheme);

      themeToggleBtn.addEventListener("click", function () {
        var current = document.documentElement.getAttribute("data-theme") || "light";
        var nextTheme = current === "dark" ? "light" : "dark";
        document.documentElement.setAttribute("data-theme", nextTheme);
        localStorage.setItem("ezgi-craft-theme", nextTheme);
        updateToggleIcon(nextTheme);
      });
    }

    $("#infinite").css({ "background-position": "center center" });
    background_image_parallax_2($("#contact"), 0.8);
    background_image_parallax_2($("#testimonials"), 0.8);

    window.addEventListener(
      "resize",
      function () {
        $("#infinite").css({ "background-position": "center center" });
      },
      true
    );

    function syncNavbarScrollState() {
      if ($(document).scrollTop() > 120) {
        $(".tm-navbar").addClass("scroll");
      } else {
        $(".tm-navbar").removeClass("scroll");
      }
    }

    $(window).scroll(syncNavbarScrollState);
    syncNavbarScrollState();

    function isMobileNav() {
      return window.matchMedia("(max-width: 991.98px)").matches;
    }

    var $navCollapse = $(".navbar-collapse");

    $navCollapse.on("show.bs.collapse", function () {
      document.body.classList.add("tm-nav-open");
    });

    $navCollapse.on("hide.bs.collapse hidden.bs.collapse", function () {
      document.body.classList.remove("tm-nav-open");
    });

    $("#tmNav a").on("click", function () {
      if (isMobileNav()) {
        $navCollapse.collapse("hide");
      }
    });

    document.addEventListener("click", function (event) {
      if (!document.body.classList.contains("tm-nav-open") || !isMobileNav()) {
        return;
      }

      var nav = document.getElementById("tmNav");
      if (nav && !nav.contains(event.target)) {
        $navCollapse.collapse("hide");
      }
    });

    if ($.fn.singlePageNav) {
      $("#tmNav").singlePageNav({
        easing: "easeOutCubic",
        speed: 320,
        filter: 'a[href^="#"]',
      });
    }

    document.querySelectorAll(".lang-switcher-link").forEach(function (link) {
      link.addEventListener("click", function () {
        sessionStorage.setItem("ezgi-craft-lang-scroll", String(Math.round(window.scrollY)));
      });
    });

    function restoreLangScroll(clear) {
      var saved = sessionStorage.getItem("ezgi-craft-lang-scroll");
      if (saved === null) {
        return;
      }

      var y = parseInt(saved, 10);
      if (!isNaN(y) && y >= 0) {
        window.scrollTo(0, y);
        syncNavbarScrollState();
      }

      if (clear) {
        sessionStorage.removeItem("ezgi-craft-lang-scroll");
      }
    }

    if ($.fn.magnificPopup) {
      $(".tm-gallery").magnificPopup({
        delegate: "a",
        type: "image",
        gallery: { enabled: true },
      });
    }

    if ($.fn.slick) {
      $(".tm-testimonials-carousel").slick({
        dots: true,
        prevArrow: false,
        nextArrow: false,
        infinite: false,
        slidesToShow: 3,
        slidesToScroll: 1,
        responsive: [
          { breakpoint: 992, settings: { slidesToShow: 2 } },
          { breakpoint: 768, settings: { slidesToShow: 2 } },
          { breakpoint: 480, settings: { slidesToShow: 1 } },
        ],
      });

      $(".tm-gallery").slick({
        dots: true,
        infinite: false,
        slidesToShow: 5,
        slidesToScroll: 2,
        responsive: [
          { breakpoint: 1199, settings: { slidesToShow: 4, slidesToScroll: 2 } },
          { breakpoint: 991, settings: { slidesToShow: 2, slidesToScroll: 1, arrows: false } },
          { breakpoint: 767, settings: { slidesToShow: 1, slidesToScroll: 1, arrows: false, dots: true } },
        ],
      });
    }

    restoreLangScroll(false);
    if (document.readyState === "complete") {
      restoreLangScroll(true);
    } else {
      window.addEventListener("load", function () {
        restoreLangScroll(true);
      });
    }
  });
})();

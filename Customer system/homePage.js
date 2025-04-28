src = "https://code.jquery.com/jquery-3.6.0.min.js";
src = "/ajaxCall.js";

function slideCarousel(id, direction) {
  const carousel = document.getElementById(id);
  const items = carousel.querySelectorAll(".carousel-item");
  const dots = document.querySelector(
    `#${id.replace("carousel", "dots")}`
  )?.children;

  let currentIndex = parseInt(carousel.getAttribute("data-index")) || 0;
  const itemCount = items.length;

  currentIndex = (currentIndex + direction + itemCount) % itemCount;

  const offset = (100 / itemCount) * currentIndex;
  carousel.style.transform = `translateX(${offset}%)`;
  carousel.setAttribute("data-index", currentIndex);

  if (dots) {
    Array.from(dots).forEach((dot) => dot.classList.remove("active"));
    if (dots[currentIndex]) {
      dots[currentIndex].classList.add("active");
    }
  }
}

function toggleMore(elem) {
  const aboutSection = document.querySelector(".about");
  const moreText = aboutSection.querySelector("p.more");
  const readMore = aboutSection.querySelector(".read-more");

  if (moreText.style.display === "block") {
    moreText.style.display = "none";
    readMore.style.display = "inline";
  } else {
    moreText.style.display = "block";
    readMore.style.display = "none";
  }
}

window.onload = () => {
  const carousels = document.querySelectorAll(".carousel");

  carousels.forEach((carousel) => {
    const items = carousel.querySelectorAll(".carousel-item");
    const itemCount = items.length;

    carousel.setAttribute("data-index", 0);
    carousel.style.width = `${itemCount * 100}%`;

    items.forEach((item) => {
      item.style.width = `${100 / itemCount}%`;
      item.style.flex = `0 0 ${100 / itemCount}%`;
    });

    carousel.style.transform = `translateX(0%)`;
  });

  const dotsSections = document.querySelectorAll(".pagination-dots");
  dotsSections.forEach((dots) => {
    const children = dots.children;
    if (children.length > 0) {
      Array.from(children).forEach((dot) => dot.classList.remove("active"));
      children[0].classList.add("active");
    }
  });
};

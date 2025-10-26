module.exports = function () {
	// Toggle the side navigation
	const sidebarToggle = document.body.querySelector("#sidebarToggle");
	if (sidebarToggle) {
		// Uncomment Below to persist sidebar toggle between refreshes
		// if (localStorage.getItem('sb|sidebar-toggle') === 'true') {
		//     document.body.classList.toggle('sb-sidenav-toggled');
		// }
		sidebarToggle.addEventListener("click", (event) => {
			event.preventDefault();
			document.body.classList.toggle("sb-sidenav-toggled");
			localStorage.setItem(
				"sb|sidebar-toggle",
				document.body.classList.contains("sb-sidenav-toggled")
			);
		});
	}

	const sideNavElement = document.getElementById("layoutSidenav_nav");
	const currentUrl = window.location.href;
	const navLinkElements = sideNavElement.querySelectorAll("a.nav-link");
	navLinkElements.forEach((navLinkElement) => {
		if (currentUrl.includes(navLinkElement.getAttribute("href"))) {
			navLinkElement.classList.add("active");
			expandElement(navLinkElement);
		}
	});
	function expandElement(collapseElement) {
		if (
			collapseElement.parentElement &&
			collapseElement.parentElement.classList.contains(
				"sb-sidenav-menu-nested"
			)
		) {
			const parentCollapseElement =
				collapseElement.parentElement.parentElement;
			parentCollapseElement.classList.add("show");
			siblingParentCollapseElement =
				parentCollapseElement.previousElementSibling;
			if (siblingParentCollapseElement) {
				siblingParentCollapseElement.classList.remove("collapsed");
				siblingParentCollapseElement.setAttribute(
					"aria-expanded",
					"true"
				);
				expandElement(siblingParentCollapseElement);
			}
		}
	}
};

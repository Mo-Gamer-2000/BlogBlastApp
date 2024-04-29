// TESTING...
alert('functions.js');
// Result = it is loading

// Add Event Lister 
document.addEventListener("DOMContentLoaded", _ => {
    // Create a Const to get the "topnav" class element by id from index[0] from the list of pages - for NavBar
    const topNav = document.getElementsByClassName("topnav")[0];
    // Do a simple if check statement to see if topNav is available ot not
    if (topNav) {
        window.scroll = () => {
            // Also, check if the scroll is greater then 50 pixels
            if (window.scrollY > 50) {
                // If so, add these classes - scrollednav py-0
                topNav.classList.add('scrollednav', 'py-0')
            }
            else {
                // Else, remove those classes - scrollednav py-0
                topNav.classList.remove('scrollednav', 'py-0')
            }
        };
    }
});

// Implement function for toggle menu bar for smaller screens
// Pass e (event) as parameter
function toggleMenu(e) {
    // Loking for the class name collapsed - for Toggeling
    e.target.classList.toggle('collapsed');
    // Created const to store, the element by the id = navbar-menu-wrapper
    const navbarWrapper = document.getElementById('navbar-menu-wrapper');
    // Toggle between show and collapse class
    navbarWrapper.classList.toggle('show');
}
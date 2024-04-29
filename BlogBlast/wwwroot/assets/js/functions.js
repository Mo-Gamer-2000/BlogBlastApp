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
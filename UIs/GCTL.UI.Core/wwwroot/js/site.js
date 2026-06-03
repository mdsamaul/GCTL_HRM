// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function getBaseUrl() {
    var base_url = window.location.origin;
    //console.log(base_url);
    //var host = window.location.host;
    //var pathArray = window.location.pathname.split('/');
    //if (pathArray.includes("swift")) {
    //    return base_url + "/swift";
    //}
    return base_url;
}
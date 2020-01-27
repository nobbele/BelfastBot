// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.

function objectifyArrayOfObjects(arr) {
    var obj = {};
    for (var i = 0; i < arr.length; i++){
        obj[arr[i]['name']] = arr[i]['value'];
    }
    return obj;
}
// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function FormToJson (form) {
    return JSON.stringify(
        form.serializeArray().reduce(
            function (m, o) {
                m[o.name] = o.value;
                return m;
            },
            {}
        )
    );
}
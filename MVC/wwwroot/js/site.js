// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function FormToJsonString (form) {
    return JSON.stringify(
        FormToJson(form)
    );
}

function FormToJson(form) {
    return form.serializeArray().reduce(
        function (m, o) {
            m[o.name] = o.value;
            return m;
        },
        {}
    );
}

function DeleteToken() {
    console.log("suck");
    localStorage.removeItem("token");
}

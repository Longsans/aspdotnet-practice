$("submitButton").click(
    function () {
        $.ajax({
            type: "POST",
            url: "/api/login",
            success: function (result) {
                window.location.replace("/Home");
            }
        })
    }
)
function showAlert(id) {
    const response = confirm("Are you sure you want to but this movie!");
    if (response) {
        purchaseMovie(id);
    }
}

function purchaseMovie(movieId) {
    var form = $('#__AjaxAntiForgeryForm');
    var token = $("input[name='__RequestVerificationToken']", form).val();
    $.ajax({
        type: "GET",
        url: "@Url.Action("Purchase", "PurchaseMovies")",
        headers: { "RequestVerificationToken": token },
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        data: {
            __RequestVerificationToken: token,
            id: movieId
        },
        success: function (result) { debugger; console.log(result) },
        error: function (req, status, error) { debugger; console.log(status) }
    })
}
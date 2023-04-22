// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).on("click", "button.create-button", async function () {
    let textBox = $(".create-text-box");
    const options = {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({id: "", name: textBox.val()})
    };
    fetch('/Admin/Role/Create', options)
        .then(res => {

            // if (res.status != 200) { throw new Error("Bad Server Response"); }
            return res.text();
        })
        .then((html) => {
            // alert(html)
            $("#RoleTable").html(html);
        })
})


$(document).on("click", "button.edit-button", async function () {
    let textBoxId = $(this).prop("id");
    let textBox = $("input#" + textBoxId);
    let isDisabled = textBox.prop('disabled');
    if ($(this).text() == "Update") {
        const options = {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({id: textBoxId, name: textBox.val()})
        };
        fetch('/Admin/Role/Update', options)
            .then(res => {

                // if (res.status != 200) { throw new Error("Bad Server Response"); }
                return res.text();
            })
            .then((html) => {
                // alert(html)
                $("#RoleTable").html(html);
            })
        $(this).text("Edit");
        textBox.prop('disabled', !isDisabled)

    } else {
        $(this).text("Update");
        textBox.prop('disabled', !isDisabled)
        // if (isDisabled){
        //     alert("textbox is disabled")
        // }
        // alert("input#"+texboxId);
        // alert(isDisabled)
    }

});


$(document).on("click", "button.delete-button", async function () {
    let id = $(this).prop("id");
    const options = {
        method: 'POST',
        headers: {'Content-Type': 'application/json'},
        body: JSON.stringify({id: id, name: ""})
    };
    fetch('/Admin/Role/Delete', options)
        .then(res => {

            // if (res.status != 200) { throw new Error("Bad Server Response"); }
            return res.text();
        })
        .then((html) => {
            // alert(html)
            $("#RoleTable").html(html);
        })
})


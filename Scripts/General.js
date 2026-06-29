function GetDropdown(ddl, data, isSelectIncluded) {

    $("#" + ddl).empty();
    if (isSelectIncluded)
        $("#" + ddl).append($("<option></option>").val("").html("Select"));
    $("#" + ddl).select2();
    {
        $.each(data, function (key, value) {
            $("#" + ddl).append($("<option></option>").val(value.value).html(value.text));
        });
    }
}
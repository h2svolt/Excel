$(document).ready(function () {
  $('#example').DataTable();
});
// *Upload Image List
function updateList() {
    var input = document.getElementById('transcript');
    var output = document.getElementById('transcriptList');

    for (var i = 0; i < input.files.length; ++i) {
        output.innerHTML += '<div id="' + i + '"><li class="upload-file">' + input.files.item(i).name + '</li><span class="remove" onclick="remove(' + i + ')">x</span></div>';
    }
}

// *Remove Upload Image
function remove(id) {
    $('#' + id).remove();
};
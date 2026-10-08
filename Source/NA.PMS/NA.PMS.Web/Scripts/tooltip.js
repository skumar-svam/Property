
$(document).ready(function () {
    $('thead tr th').each(function () {
        $(this).attr('title', $(this).data('title'));
    })
});
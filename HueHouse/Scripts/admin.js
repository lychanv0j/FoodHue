$(document).ready(function () {
    // Hiển thị modal thêm mới người dùng
    $('#addUserBtn').on('click', function () {
        // Reset form khi mở modal thêm mới người dùng
        $('#userForm')[0].reset(); // Xóa tất cả dữ liệu trong form
        $('#modalTitle').text('Thêm mới người dùng');
        $('#userId').val(''); // Đảm bảo không có ID khi thêm mới
        $('#userModal').removeClass('hidden');
    });

    // Hiển thị modal chỉnh sửa thông tin người dùng
    $('.edit-user').on('click', function () {
        var userId = $(this).data('id');
        var userName = $(this).data('name');
        var userEmail = $(this).data('email');
        var userStatus = $(this).data('status');
        var userRole = $(this).data('role');

        // Cập nhật thông tin modal với dữ liệu người dùng
        $('#modalTitle').text('Chỉnh sửa người dùng');
        $('#userId').val(userId);
        $('#userName').val(userName);
        $('#userEmail').val(userEmail);
        $('#userStatus').val(userStatus);
        $('#userRole').val(userRole);

        // Hiển thị modal
        $('#userModal').removeClass('hidden');
    });

    // Đóng modal
    $('#closeModal').on('click', function () {
        $('#userModal').addClass('hidden');
    });

    // Xử lý form thêm mới và chỉnh sửa người dùng
    $('#userForm').on('submit', function (event) {
        event.preventDefault(); // Ngừng gửi form mặc định

        var formData = $(this).serialize();
        var userId = $('#userId').val();
        var actionUrl = userId ? '@Url.Action("EditUser", "Admin")' : '@Url.Action("AddUser", "Admin")';  // Kiểm tra có ID hay không

        $.ajax({
            url: actionUrl,  // Điều chỉnh URL tùy theo thêm hay sửa
            type: 'POST',
            data: formData,
            success: function (response) {
                window.location.reload();  // Làm mới trang để hiển thị dữ liệu mới
            },
            error: function () {
                alert('Có lỗi xảy ra. Vui lòng thử lại!');
            }
        });
    });

    // Xóa người dùng
    $('.delete-user').on('click', function () {
        var userId = $(this).data('id');

        if (confirm('Bạn có chắc chắn muốn xóa người dùng này?')) {
            $.ajax({
                url: '@Url.Action("DeleteUser", "Admin")',
                type: 'POST',
                data: { userId: userId },
                success: function (response) {
                    window.location.reload(); // Làm mới trang sau khi xóa thành công
                },
                error: function () {
                    alert('Có lỗi xảy ra khi xóa người dùng!');
                }
            });
        }
    });

    // Xử lý tìm kiếm người dùng
    $('.search-user form').on('submit', function (event) {
        event.preventDefault(); // Ngừng gửi form mặc định

        var searchQuery = $('#searchUser').val(); // Lấy từ khóa tìm kiếm
        $.ajax({
            url: '@Url.Action("SearchUsers", "Admin")',
            type: 'POST',
            data: { searchQuery: searchQuery },
            success: function (response) {
                // Cập nhật danh sách người dùng sau khi tìm kiếm
                $('.user-table-container').html(response); // Thay thế bảng với kết quả tìm kiếm
            },
            error: function () {
                alert('Có lỗi xảy ra khi tìm kiếm!');
            }
        });
    });



    // Hiển thị modal cập nhật trạng thái đơn hàng
    $("select[name='newStatus']").change(function () {
        var orderId = $(this).closest('form').find('input[name="orderId"]').val();  // Lấy OrderID
        var newStatus = $(this).val();  // Lấy trạng thái mới

        // Gửi yêu cầu AJAX để cập nhật trạng thái đơn hàng
        $.ajax({
            url: '@Url.Action("UpdateOrderStatus", "Admin")',  // URL đến Action UpdateOrderStatus trong AdminController
            type: 'POST',
            data: {
                orderId: orderId,
                newStatus: newStatus
            },
            success: function (response) {
                alert('Trạng thái đơn hàng đã được cập nhật thành công!');
            },
            error: function () {
                alert('Cập nhật trạng thái thất bại. Vui lòng thử lại.');
            }
        });
    });
});
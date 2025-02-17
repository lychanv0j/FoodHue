// Đợi cho đến khi DOM được tải hoàn toàn trước khi thực thi mã bên trong
document.addEventListener('DOMContentLoaded', function () {


    // XỬ LÝ MENU VÀ CHUYỂN ĐỔI GIỮA CÁC DANH MỤC
    // Tìm menu ngang có class là 'horizontal-menu'
    const menu = document.querySelector('.horizontal-menu');

    // Tìm tất cả các phần nội dung liên quan đến danh mục (có class là 'menu-section')
    const menuSections = document.querySelectorAll('.menu-section');

    // Kiểm tra xem menu có tồn tại không
    if (menu) {
        // Gắn sự kiện click vào menu
        menu.addEventListener('click', function (event) {
            // Lấy phần tử <li> gần nhất được click
            const clickedItem = event.target.closest('li');
            if (clickedItem) {
                // Tìm tất cả các <li> trong menu và xóa class 'active' của chúng
                const menuItems = menu.querySelectorAll('li');
                menuItems.forEach(item => item.classList.remove('active'));

                // Thêm class 'active' vào mục được click
                clickedItem.classList.add('active');

                // Ẩn tất cả nội dung các danh mục
                menuSections.forEach(section => section.style.display = 'none');

                // Lấy giá trị của thuộc tính 'data-category' từ mục được click
                const category = clickedItem.getAttribute('data-category');

                // Tìm và hiển thị phần nội dung tương ứng với danh mục
                const activeSection = document.getElementById(category);
                if (activeSection) {
                    activeSection.style.display = 'block'; // Hiển thị phần nội dung của danh mục đã click
                }
            }
        });

        // Hiển thị danh mục mặc định khi tải trang
        const defaultCategory = menu.querySelector('li[data-category]'); // Lấy mục mặc định trong menu
        if (defaultCategory) {
            defaultCategory.click(); // Kích hoạt click vào mục mặc định
        }
    }


    // XỬ LÝ TÌM KIẾM MÓN ĂN
    // Tìm phần tử ô nhập tìm kiếm
    const searchQueryInput = document.getElementById('searchQuery');

    // Tìm phần tử hiển thị kết quả tìm kiếm
    const searchResultsDiv = document.getElementById('searchResults');

    // Kiểm tra xem ô nhập tìm kiếm có tồn tại không
    if (searchQueryInput) {
        // Gắn sự kiện nhập liệu vào ô tìm kiếm
        searchQueryInput.addEventListener('input', function () {
            // Lấy giá trị nhập vào và xóa khoảng trắng thừa
            const query = searchQueryInput.value.trim();

            // Kiểm tra nếu ô tìm kiếm không rỗng
            if (query.length > 0) {
                // Gửi yêu cầu AJAX đến máy chủ với từ khóa tìm kiếm
                fetch(`/Home/Search?query=${encodeURIComponent(query)}`)
                    .then(response => response.json()) // Chuyển đổi phản hồi thành JSON
                    .then(data => {
                        // Kiểm tra nếu có kết quả tìm kiếm
                        if (data.length > 0) {
                            searchResultsDiv.innerHTML = ''; // Xóa kết quả cũ
                            data.forEach(item => {
                                // Tạo phần tử div đại diện cho từng kết quả tìm kiếm
                                const resultItem = document.createElement('div');
                                resultItem.classList.add('search-result-item');

                                // Tạo phần tử ảnh cho món ăn
                                const img = document.createElement('img');
                                img.src = item.ImageUrl; // Đường dẫn ảnh từ dữ liệu trả về
                                img.alt = item.FoodName; // Tên món ăn

                                // Tạo phần tử span để hiển thị tên món ăn
                                const name = document.createElement('span');
                                name.textContent = item.name;

                                // Gắn ảnh và tên vào div kết quả
                                resultItem.appendChild(img);
                                resultItem.appendChild(name);

                                // Gắn div kết quả vào danh sách hiển thị
                                searchResultsDiv.appendChild(resultItem);
                            });
                            searchResultsDiv.style.display = 'block'; // Hiển thị kết quả tìm kiếm
                        } else {
                            searchResultsDiv.style.display = 'none'; // Ẩn nếu không có kết quả
                        }
                    })
                    .catch(error => {
                        console.error('Lỗi khi tìm kiếm:', error); // Log lỗi nếu có vấn đề khi gọi AJAX
                    });
            } else {
                searchResultsDiv.style.display = 'none'; // Ẩn danh sách nếu ô tìm kiếm rỗng
            }
        });
    }


    // XỬ LÝ TĂNG VÀ GIẢM SỐ LƯỢNG SẢN PHẨM TRONG TRANG SẢN PHẨM CHI TIẾT
    // Tìm tất cả các nút giảm số lượng
    document.querySelectorAll('.quantity-btn.minus').forEach(button => {
        button.addEventListener('click', function () {
            // Lấy số lượng hiện tại từ phần tử hiển thị
            let quantity = parseInt(document.getElementById('quantity-btn').textContent) || 1;

            // Giảm số lượng nếu lớn hơn 1
            if (quantity > 1) {
                quantity--; // Giảm số lượng
                document.getElementById('quantity-btn').textContent = quantity; // Cập nhật hiển thị
                document.getElementById('quantity').value = quantity; // Cập nhật trường ẩn
            }
        });
    });

    // Tìm tất cả các nút tăng số lượng
    document.querySelectorAll('.quantity-btn.plus').forEach(button => {
        button.addEventListener('click', function () {
            // Lấy số lượng hiện tại từ phần tử hiển thị
            let quantity = parseInt(document.getElementById('quantity-btn').textContent) || 1;

            quantity++; // Tăng số lượng
            document.getElementById('quantity-btn').textContent = quantity; // Cập nhật hiển thị
            document.getElementById('quantity').value = quantity; // Cập nhật trường ẩn
        });
    });


    // XỬ LÝ TĂNG VÀ GIẢM SỐ LƯỢNG SẢN PHẨM TRONG GIỎ HÀNG
    // Hàm tăng số lượng sản phẩm
    window.increaseQuantity = function () {
        // Lấy số lượng hiện tại từ nội dung của phần tử HTML có id là 'quantity-btn'
        let quantity = parseInt(document.getElementById('quantity-btn').textContent);

        // Tăng số lượng lên 1
        quantity++;

        // Cập nhật số lượng hiển thị trên giao diện
        document.getElementById('quantity-btn').textContent = quantity;

        // Cập nhật giá trị của trường ẩn 'quantity' để lưu số lượng mới
        document.getElementById('quantity').value = quantity;
    }

    // Hàm giảm số lượng sản phẩm
    window.decreaseQuantity = function () {
        // Lấy số lượng hiện tại từ nội dung của phần tử HTML có id là 'quantity-btn'
        let quantity = parseInt(document.getElementById('quantity-btn').textContent);

        // Chỉ giảm số lượng nếu số lượng hiện tại lớn hơn 1
        if (quantity > 1) {
            // Giảm số lượng đi 1
            quantity--;

            // Cập nhật số lượng hiển thị trên giao diện
            document.getElementById('quantity-btn').textContent = quantity;

            // Cập nhật giá trị của trường ẩn 'quantity' để lưu số lượng mới
            document.getElementById('quantity').value = quantity;
        }
    }


    // Xử lý hiển thị chi tiết đơn hàng khi người dùng click vào nút "view-order-details"
    document.getElementById("view-order-details").addEventListener("click", function (e) {
        e.preventDefault(); // Ngừng hành động mặc định của nút
        document.getElementById("order-details-modal").style.display = "flex"; // Hiển thị modal chứa chi tiết đơn hàng
    });

    // Xử lý đóng modal khi người dùng click vào nút "close-btn"
    document.querySelector(".close-btn").addEventListener("click", function () {
        document.getElementById("order-details-modal").style.display = "none"; // Ẩn modal
    });

    // Đóng modal nếu người dùng click vào bất kỳ khu vực ngoài modal
    window.addEventListener("click", function (e) {
        const modal = document.getElementById("order-details-modal");
        if (e.target === modal) {
            modal.style.display = "none"; // Ẩn modal khi click vào ngoài modal
        }
    });
});
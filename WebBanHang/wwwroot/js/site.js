$(document).ready(function () {
    showQuantiyCart();
});
let showQuantiyCart = () => {
    $.ajax({
        url: "/customer/cart/ThongKeSL",
        success: function (data) {
            //hien thi so luong san pham trong gio trong _FrontEnd.cshtml
            $(".showcart").text(data.qty);
        }
    });
}
$(document).on("click", ".addtocart", function (evt) {
    evt.preventDefault(); 

    let id = $(this).data("productid");
    $.ajax({
        url: "/customer/cart/AddToCart",
        data: { productId: id },
        success: function (data) {
            Swal.fire({
                icon: "success",
                title: "Đã thêm vào giỏ hàng!",
                text: "Bạn có thể tiếp tục mua sắm hoặc đi đến giỏ hàng."
            });
            showQuantiyCart();
        },
        error: function () {
            Swal.fire("Lỗi", "Không thể thêm sản phẩm vào giỏ", "error");
        }
    });
});

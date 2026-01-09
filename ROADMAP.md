# 🏋️ GYM ANGEL - Development Roadmap

> Tài liệu hướng dẫn các bước phát triển tiếp theo cho dự án Gym Angel

---

## 📋 Trạng thái hiện tại

- ✅ Landing Page (index.html)
- ✅ Backend API Structure (ASP.NET Core 8)
- ✅ Domain Entities (User, Product, Order, Cart)
- ✅ Auth API (Login, Register, Forgot/Reset Password)
- ⏳ Các trang frontend khác (đang phát triển)

---

## 🎯 Phase 1: Hoàn thiện Frontend cơ bản

| Ưu tiên | Trang | Mô tả | Trạng thái |
|---------|-------|-------|------------|
| ⭐⭐⭐ | **Signin/Signup** | Kết nối với API Auth (login, register) | ⏳ |
| ⭐⭐⭐ | **Products Page** | Hiển thị danh sách supplements từ API | ⏳ |
| ⭐⭐ | **Product Detail** | Chi tiết sản phẩm + nút "Add to Cart" | ⏳ |
| ⭐⭐ | **Cart Page** | Xem giỏ hàng, cập nhật số lượng | ⏳ |

### Tasks chi tiết:

- [ ] Tạo `products.html` - Grid hiển thị sản phẩm
- [ ] Tạo `product-detail.html` - Chi tiết 1 sản phẩm
- [ ] Tạo `cart.html` - Trang giỏ hàng
- [ ] Kết nối `signin.html` với `POST /api/auth/login`
- [ ] Kết nối `signup.html` với `POST /api/auth/register`
- [ ] Thêm cart icon vào navbar với badge số lượng

---

## 🛒 Phase 2: Tính năng thương mại điện tử

| Tính năng | Mô tả | Trạng thái |
|-----------|-------|------------|
| 🛒 **Shopping Cart** | Lưu giỏ hàng vào LocalStorage hoặc Backend | ⏳ |
| 💳 **Checkout Flow** | Form nhập thông tin giao hàng + chọn phương thức thanh toán | ⏳ |
| 💰 **Tích hợp VNPay** | Connect API VNPay để thanh toán online | ⏳ |
| 📦 **Order History** | Người dùng xem lịch sử đơn hàng | ⏳ |

### Tasks chi tiết:

- [ ] Implement Cart Service (add, remove, update quantity)
- [ ] Tạo `checkout.html` - Form checkout
- [ ] Tạo `OrderController` với API đặt hàng
- [ ] Tích hợp VNPay Sandbox
- [ ] Tạo `order-history.html` - Lịch sử đơn hàng
- [ ] Email notification khi đặt hàng thành công

---

## 🎫 Phase 3: Tính năng Membership (GYM)

| Tính năng | Mô tả | Trạng thái |
|-----------|-------|------------|
| 🎫 **Membership Plans** | Trang hiển thị các gói tập (1 tháng, 3 tháng, 1 năm) | ⏳ |
| 📅 **Đăng ký membership** | Flow thanh toán + cập nhật MembershipStatus | ⏳ |
| 👤 **User Profile** | Xem thông tin cá nhân, trạng thái membership | ⏳ |

### Tasks chi tiết:

- [ ] Tạo entity `MembershipPlan` (Name, Duration, Price)
- [ ] Tạo `pricing.html` - Bảng giá các gói tập
- [ ] Tạo `MembershipController` API
- [ ] Tạo `profile.html` - Trang cá nhân user
- [ ] Logic tự động hết hạn membership

---

## 🔧 Phase 4: Admin Dashboard

| Tính năng | Mô tả | Trạng thái |
|-----------|-------|------------|
| 📊 **Dashboard** | Thống kê doanh thu, số đơn hàng, số thành viên | ⏳ |
| 📦 **Product Management** | CRUD sản phẩm supplements | ⏳ |
| 👥 **User Management** | Quản lý thành viên, cấp quyền | ⏳ |
| 📋 **Order Management** | Xem và cập nhật trạng thái đơn hàng | ⏳ |

### Tasks chi tiết:

- [ ] Tạo layout admin với sidebar navigation
- [ ] Dashboard với charts (doanh thu, đơn hàng)
- [ ] CRUD Products với upload ảnh
- [ ] Danh sách users + filter/search
- [ ] Quản lý orders (view, update status)
- [ ] Phân quyền Admin/Staff/Customer

---

## 💻 Code Examples

### Kết nối Signin với API

```javascript
// signin.html
document.getElementById('loginForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const email = document.getElementById('email').value;
    const password = document.getElementById('password').value;
    
    try {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ email, password })
        });
        
        const data = await response.json();
        
        if (response.ok) {
            localStorage.setItem('token', data.token);
            window.location.href = '/index.html';
        } else {
            alert(data.message || 'Đăng nhập thất bại');
        }
    } catch (error) {
        console.error('Login error:', error);
    }
});
```

### Fetch Products từ API

```javascript
// products.html
async function loadProducts() {
    try {
        const response = await fetch('/api/products');
        const products = await response.json();
        
        const container = document.getElementById('products-grid');
        container.innerHTML = products.map(product => `
            <div class="col-lg-3 col-md-4 col-sm-6">
                <div class="product-card">
                    <img src="${product.imageUrl}" alt="${product.name}">
                    <h5>${product.name}</h5>
                    <p class="price">${product.price.toLocaleString()}đ</p>
                    <button onclick="addToCart(${product.id})">Add to Cart</button>
                </div>
            </div>
        `).join('');
    } catch (error) {
        console.error('Error loading products:', error);
    }
}

document.addEventListener('DOMContentLoaded', loadProducts);
```

### Shopping Cart với LocalStorage

```javascript
// cart.js
const Cart = {
    getItems() {
        return JSON.parse(localStorage.getItem('cart') || '[]');
    },
    
    addItem(product) {
        const items = this.getItems();
        const existing = items.find(i => i.id === product.id);
        
        if (existing) {
            existing.quantity++;
        } else {
            items.push({ ...product, quantity: 1 });
        }
        
        localStorage.setItem('cart', JSON.stringify(items));
        this.updateBadge();
    },
    
    removeItem(productId) {
        const items = this.getItems().filter(i => i.id !== productId);
        localStorage.setItem('cart', JSON.stringify(items));
        this.updateBadge();
    },
    
    getTotal() {
        return this.getItems().reduce((sum, item) => sum + item.price * item.quantity, 0);
    },
    
    updateBadge() {
        const count = this.getItems().reduce((sum, item) => sum + item.quantity, 0);
        document.getElementById('cart-badge').textContent = count;
    }
};
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|-------|------------|
| **Backend** | ASP.NET Core 8, Entity Framework Core |
| **Auth** | ASP.NET Identity, JWT Token |
| **Database** | SQL Server (LocalDB/Express) |
| **Frontend** | HTML5, CSS3, JavaScript, Bootstrap 5 |
| **Payment** | VNPay |
| **Libraries** | jQuery, Owl Carousel, WOW.js |

---

## 📁 Suggested File Structure

```
GymAngel.Frontend/wwwroot/
├── index.html              ✅ Done
├── about.html              ✅ Done
├── service.html            ✅ Done
├── contact.html            ✅ Done
├── signin.html             ⏳ Need API integration
├── signup.html             ⏳ Need API integration
├── products.html           ❌ To create
├── product-detail.html     ❌ To create
├── cart.html               ❌ To create
├── checkout.html           ❌ To create
├── profile.html            ❌ To create
├── order-history.html      ❌ To create
├── pricing.html            ❌ To create (Membership plans)
├── js/
│   ├── main.js             ✅ Done
│   ├── auth.js             ❌ To create
│   ├── cart.js             ❌ To create
│   └── api.js              ❌ To create (API helper)
└── admin/
    ├── dashboard.html      ⏳ Need implementation
    ├── products.html       ⏳ Need implementation
    ├── orders.html         ⏳ Need implementation
    └── users.html          ⏳ Need implementation
```

---

## 🚀 Quick Start - Next Steps

1. **Bắt đầu với Products Page** - Đây là core feature
2. **Hoàn thiện Auth Flow** - Signin/Signup kết nối API
3. **Implement Cart** - Logic giỏ hàng cơ bản
4. **Checkout + VNPay** - Hoàn thiện flow mua hàng

---

*Last updated: 2026-01-09*

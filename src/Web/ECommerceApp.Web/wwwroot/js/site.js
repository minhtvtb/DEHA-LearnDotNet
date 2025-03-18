// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Site JavaScript

// Initialize tooltips
document.addEventListener('DOMContentLoaded', function() {
    // Enable Bootstrap tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
    
    // Enable Bootstrap popovers
    var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'));
    popoverTriggerList.map(function (popoverTriggerEl) {
        return new bootstrap.Popover(popoverTriggerEl);
    });
});

// Shopping cart functionality
const ShoppingCart = {
    // Add item to cart
    addItem: function(productId, productName, price, quantity = 1) {
        let cart = this.getCart();
        
        // Check if product already in cart
        const existingItem = cart.find(item => item.id === productId);
        
        if (existingItem) {
            existingItem.quantity += quantity;
        } else {
            cart.push({
                id: productId,
                name: productName,
                price: price,
                quantity: quantity
            });
        }
        
        // Save cart
        this.saveCart(cart);
        this.updateCartBadge();
        
        // Show notification
        this.showNotification(`${productName} added to cart`);
    },
    
    // Remove item from cart
    removeItem: function(productId) {
        let cart = this.getCart();
        cart = cart.filter(item => item.id !== productId);
        this.saveCart(cart);
        this.updateCartBadge();
    },
    
    // Update item quantity
    updateQuantity: function(productId, quantity) {
        let cart = this.getCart();
        const item = cart.find(item => item.id === productId);
        
        if (item) {
            if (quantity > 0) {
                item.quantity = quantity;
            } else {
                // Remove item if quantity is 0 or negative
                return this.removeItem(productId);
            }
        }
        
        this.saveCart(cart);
        this.updateCartBadge();
    },
    
    // Get cart from localStorage
    getCart: function() {
        const cartJson = localStorage.getItem('shoppingCart');
        return cartJson ? JSON.parse(cartJson) : [];
    },
    
    // Save cart to localStorage
    saveCart: function(cart) {
        localStorage.setItem('shoppingCart', JSON.stringify(cart));
    },
    
    // Get total number of items in cart
    getItemCount: function() {
        const cart = this.getCart();
        return cart.reduce((total, item) => total + item.quantity, 0);
    },
    
    // Get total price of cart
    getTotalPrice: function() {
        const cart = this.getCart();
        return cart.reduce((total, item) => total + (item.price * item.quantity), 0);
    },
    
    // Clear the entire cart
    clearCart: function() {
        localStorage.removeItem('shoppingCart');
        this.updateCartBadge();
    },
    
    // Update cart badge with number of items
    updateCartBadge: function() {
        const cartBadge = document.querySelector('.cart-badge');
        if (cartBadge) {
            const itemCount = this.getItemCount();
            cartBadge.textContent = itemCount;
            cartBadge.style.display = itemCount > 0 ? 'inline' : 'none';
        }
    },
    
    // Show notification
    showNotification: function(message) {
        const notification = document.createElement('div');
        notification.className = 'toast align-items-center text-white bg-success';
        notification.setAttribute('role', 'alert');
        notification.setAttribute('aria-live', 'assertive');
        notification.setAttribute('aria-atomic', 'true');
        
        notification.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;
        
        const container = document.querySelector('.toast-container');
        if (!container) {
            const newContainer = document.createElement('div');
            newContainer.className = 'toast-container position-fixed bottom-0 end-0 p-3';
            document.body.appendChild(newContainer);
            newContainer.appendChild(notification);
        } else {
            container.appendChild(notification);
        }
        
        const toast = new bootstrap.Toast(notification);
        toast.show();
        
        // Remove notification after it's hidden
        notification.addEventListener('hidden.bs.toast', function() {
            notification.remove();
        });
    }
};

// Initialize shopping cart badge when page loads
document.addEventListener('DOMContentLoaded', function() {
    ShoppingCart.updateCartBadge();
    
    // Add event listeners for add-to-cart buttons
    document.querySelectorAll('.add-to-cart').forEach(button => {
        button.addEventListener('click', function() {
            const productId = parseInt(this.getAttribute('data-id'));
            const productName = this.getAttribute('data-name');
            const productPrice = parseFloat(this.getAttribute('data-price'));
            
            ShoppingCart.addItem(productId, productName, productPrice);
        });
    });
});

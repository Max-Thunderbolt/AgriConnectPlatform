// Success popup
function showSuccessPopup(message) {
    Swal.fire({
        title: 'Success!',
        text: message,
        icon: 'success',
        confirmButtonText: 'OK'
    });
}

// Error popup
function showErrorPopup(message) {
    Swal.fire({
        title: 'Error!',
        text: message,
        icon: 'error',
        confirmButtonText: 'OK'
    });
}

// Confirmation popup
function showConfirmPopup(title, message, callback) {
    Swal.fire({
        title: title,
        text: message,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes',
        cancelButtonText: 'No'
    }).then((result) => {
        if (result.isConfirmed) {
            callback();
        }
    });
}

// Loading popup
function showLoadingPopup(message = 'Loading...') {
    Swal.fire({
        title: message,
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });
}

// Close loading popup
function closeLoadingPopup() {
    Swal.close();
}

// Login popup
function showLoginPopup() {
    Swal.fire({
        title: 'Login',
        html: `
        <input type="text" id="loginEmail" class="swal2-input" placeholder="Email">
        <input type="password" id="loginPassword" class="swal2-input" placeholder="Password">
        `,
        confirmButtonText: 'Login',
        showCancelButton: true,
        cancelButtonText: 'Cancel',
        showCloseButton: true,
        showConfirmButton: true,
        didOpen: () => {
            document.getElementById('loginEmail').focus();
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const loginEmail = document.getElementById('loginEmail').value;
            const loginPassword = document.getElementById('loginPassword').value;

            if (loginEmail && loginPassword) {
                showLoadingPopup('Logging in...');
                const requestData = {
                    email: loginEmail,
                    password: loginPassword,
                    location: null,
                    farmName: null
                };

                console.log("Request data:", JSON.stringify(requestData));

                const encryptedData = encryptData(requestData);

                console.log("Encrypted data:", encryptedData);

                fetch('/User/Login', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'same-origin',
                    body: JSON.stringify({
                        encryptedData: encryptedData
                    })
                })
                    .then(response => {
                        console.log('Response status:', response.status);
                        console.log('Response headers:', response.headers);
                        return response.text().then(text => {
                            try {
                                return text ? JSON.parse(text) : {};
                            } catch (e) {
                                console.error('Error parsing JSON:', e);
                                console.error('Raw response:', text);
                                throw new Error('Invalid JSON response from server');
                            }
                        });
                    })
                    .then(data => {
                        closeLoadingPopup();
                        if (data.success) {
                            showSuccessPopup(data.message);
                            if (data.isEmployee) {
                                window.location.href = '/User/Index';
                            } else {
                                window.location.href = '/Home/Index';
                            }
                        }
                        else {
                            showErrorPopup(data.message);
                        }
                    })
                    .catch(error => {
                        closeLoadingPopup();
                        console.error('Login error:', error);
                        showErrorPopup('An error occurred while logging in.\n' + error.message);
                    });
            }
            else {
                showErrorPopup('Please enter a valid email address and password');
            }
        }
    })
}

// Register farmer popup
function showRegisterFarmerPopup() {
    Swal.fire({
        title: 'Register Farmer',
        html: `
        <div class="form-group mb-3">
            <label for="farmerEmail" class="form-label text-light">Email</label>
            <input type="text" id="farmerEmail" class="form-control" placeholder="Farmer Email">
        </div>
        <div class="form-group mb-2">
            <label for="farmerPassword" class="form-label text-light">Password</label>
            <div class="input-group">
                <input type="password" id="farmerPassword" class="form-control" placeholder="Password">
                <button class="show-password-btn" type="button" onclick="togglePasswordVisibility('farmerPassword', this)">
                    👁️
                </button>
            </div>
            <div class="form-group mb-2">
                <label for="farmerConfirmPassword" class="form-label text-light">Confirm Password</label>
                <div class="input-group">
                    <input type="password" id="farmerConfirmPassword" class="form-control" placeholder="Confirm Password">
                    <button class="show-password-btn" type="button" onclick="togglePasswordVisibility('farmerConfirmPassword', this)">
                        👁️  
                    </button>
                </div>
            </div>
            <div class="form-group mb-2">
                <label for="farmerLocation" class="form-label text-light">Location</label>
                <input type="text" id="farmerLocation" class="form-control" placeholder="Location">
            </div>
            <div class="form-group mb-2">
                <label for="farmerFarmName" class="form-label text-light">Farm Name</label>
                <input type="text" id="farmerFarmName" class="form-control" placeholder="Farm Name">
            </div>
        </div>
        `,
        confirmButtonText: 'Register',
        showCancelButton: true,
        cancelButtonText: 'Cancel',
        showCloseButton: true,
        showConfirmButton: true,
        background: '#2d2d2d',
        customClass: {
            popup: 'swal2-popup-dark',
            title: 'text-light',
            content: 'text-light',
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-secondary'
        },
        didOpen: () => {
            document.getElementById('farmerEmail').focus();
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const farmerEmail = document.getElementById('farmerEmail').value;
            const farmerPassword = document.getElementById('farmerPassword').value;
            const farmerConfirmPassword = document.getElementById('farmerConfirmPassword').value;
            const farmerLocation = document.getElementById('farmerLocation').value;
            const farmerFarmName = document.getElementById('farmerFarmName').value;

            if (farmerEmail && farmerPassword && farmerConfirmPassword) {
                if (farmerPassword !== farmerConfirmPassword) {
                    showErrorPopup('Passwords do not match');
                    return;
                }

                showLoadingPopup('Registering farmer...');
                const requestData = {
                    email: farmerEmail,
                    password: farmerPassword,
                    location: farmerLocation,
                    farmName: farmerFarmName
                };

                console.log("Request data:", JSON.stringify(requestData));

                const encryptedData = encryptData(requestData);

                console.log("Encrypted data:", encryptedData);
                fetch('/User/RegisterFarmer', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    },
                    credentials: 'same-origin',
                    body: JSON.stringify({
                        encryptedData: encryptedData
                    })
                })
                    .then(response => response.json())
                    .then(data => {
                        closeLoadingPopup();
                        if (data.success) {
                            showSuccessPopup('Farmer registered successfully');
                            window.location.reload();
                        } else {
                            showErrorPopup(data.message);
                        }
                    })
                    .catch(error => {
                        closeLoadingPopup();
                        showErrorPopup('An error occurred while registering the farmer');
                    });
            }
            else {
                showErrorPopup('Please fill in all required fields');
            }
        }
    });
}

// Toggle password visibility
function togglePasswordVisibility(inputId, btn) {
    const input = document.getElementById(inputId);
    if (input.type === 'password') {
        input.type = 'text';
    } else {
        input.type = 'password';
    }
}

// Add a product popup
function showAddProductPopup() {
    Swal.fire({
        title: 'Add Product',
        html: `
        <div class="form-group mb-3">
            <label for="productName" class="form-label text-light">Product Name</label>
            <input type="text" id="productName" class="form-control" placeholder="Product Name">
        </div>
        <div class="form-group mb-3">
            <label for="productDescription" class="form-label text-light">Product Description</label>
            <textarea id="productDescription" class="form-control" placeholder="Product Description"></textarea>
        </div>
        <div class="form-group mb-3">
            <label for="productCategory" class="form-label text-light">Product Category</label>
            <select id="productCategory" class="form-control">
                <option value="0">Vegetables</option>
                <option value="1">Fruits</option>
                <option value="2">Grains</option>
                <option value="3">Livestock</option>
                <option value="4">Dairy</option>
                <option value="5">Poultry</option>
                <option value="6">Seafood</option>
                <option value="7">Herbs</option>
                <option value="8">Flowers</option>
                <option value="9">Other</option>
            </select>
        </div>
        `,
        confirmButtonText: 'Add Product',
        showCancelButton: true,
        cancelButtonText: 'Cancel',
        showCloseButton: true,
        showConfirmButton: true,
        background: '#2d2d2d',
        customClass: {
            popup: 'swal2-popup-dark',
            title: 'text-light',
            content: 'text-light',
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-secondary'
        },
        didOpen: () => {
            document.getElementById('productName').focus();
        }
    }).then((result) => {
        if (result.isConfirmed) {
            console.log("Adding product");
            const productName = document.getElementById('productName').value;
            const productDescription = document.getElementById('productDescription').value;
            const category = document.getElementById('productCategory').value;
            if (productName && productDescription) {
                showLoadingPopup('Adding product...');
                const requestData = {
                    productName: productName,
                    Description: productDescription,
                    Category: parseInt(category),
                    DateCreated: new Date().toISOString()
                };

                console.log("Request data:", JSON.stringify(requestData));

                const encryptedData = encryptData(requestData);

                console.log("Encrypted data:", encryptedData);

                fetch('/Product/Create', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    },
                    credentials: 'same-origin',
                    body: JSON.stringify({
                        productName: productName,
                        Description: productDescription,
                        Category: parseInt(category),
                        DateCreated: new Date().toISOString()
                    })
                })
                    .then(response => response.json())
                    .then(data => {
                        closeLoadingPopup();
                        if (data.success) {
                            showSuccessPopup(data.message);
                            // Refresh the page to show the new product
                            window.location.reload();
                        }
                        else {
                            showErrorPopup(data.message);
                        }
                    })
                    .catch(error => {
                        closeLoadingPopup();
                        showErrorPopup('An error occurred while adding the product');
                    });
            }
            else {
                showErrorPopup('Please enter a valid product name and description');
            }
        }
    });
}

// Edit product popup
async function showEditProductPopup(productId) {
    var productName = await fetch(`/Product/GetProductName?id=${productId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        }
    }).then(response => response.json());
    var productDescription = await fetch(`/Product/GetProductDescription?id=${productId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        }
    }).then(response => response.json());
    var productCategory = await fetch(`/Product/GetProductCategory?id=${productId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        }
    }).then(response => response.json());
    var productDateCreated = await fetch(`/Product/GetProductDateCreated?id=${productId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        }
    }).then(response => response.json());

    // Format the date for the input
    const formattedDate = productDateCreated ? new Date(productDateCreated).toISOString().split('T')[0] : '';

    console.log(productName);
    console.log(productDescription);
    console.log(productCategory);
    console.log(productDateCreated);
    Swal.fire({
        title: 'Edit Product',
        html: `
        <div class="form-group mb-3">
            <label for="productName" class="form-label text-light">Product Name</label>
            <input type="text" id="productName" class="form-control" value="${productName.replace(/\"/g, '&quot;')}">
        </div>
        <div class="form-group mb-3">
            <label for="productDescription" class="form-label text-light">Product Description</label>
            <textarea id="productDescription" class="form-control">${productDescription}</textarea>
        </div>
        <div class="form-group mb-3">
            <label for="productCategory" class="form-label text-light">Product Category</label>
            <select id="productCategory" class="form-control">
                <option value="0">Vegetables</option>
                <option value="1">Fruits</option>
                <option value="2">Grains</option>
                <option value="3">Livestock</option>
                <option value="4">Dairy</option>
                <option value="5">Poultry</option>
                <option value="6">Seafood</option>
                <option value="7">Herbs</option>
                <option value="8">Flowers</option>
                <option value="9">Other</option>
            </select>
        </div>
        <div class="form-group mb-3">
            <label for="dateCreated" class="form-label text-light">Date Created</label>
            <input type="date" id="dateCreated" class="form-control" value="${formattedDate}">
        </div>
        `,
        confirmButtonText: 'Edit Product',
        showCancelButton: true,
        cancelButtonText: 'Cancel',
        showCloseButton: true,
        showConfirmButton: true,
        background: '#2d2d2d',
        customClass: {
            popup: 'swal2-popup-dark',
            title: 'text-light',
            content: 'text-light',
            confirmButton: 'btn btn-primary',
            cancelButton: 'btn btn-secondary'
        },
        didOpen: () => {
            document.getElementById('productName').focus();
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const productName = document.getElementById('productName').value;
            const productDescription = document.getElementById('productDescription').value;
            const categorySelect = document.getElementById('productCategory');
            const productCategory = parseInt(categorySelect.value);
            const productDateCreated = document.getElementById('dateCreated').value;
            if (productName && productDescription) {
                showLoadingPopup('Editing product...');
                const requestData = {
                    Id: parseInt(productId),
                    productName: productName,
                    Description: productDescription,
                    Category: productCategory,
                    CreatedByUserId: userId,
                    DateCreated: productDateCreated
                };

                console.log("Request data:", JSON.stringify(requestData));

                const encryptedData = encryptData(requestData);

                console.log("Encrypted data:", encryptedData);

                fetch('/Product/UpdateProduct', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                    },
                    body: JSON.stringify(requestData)
                })
                    .then(response => response.json())
                    .then(data => {
                        closeLoadingPopup();
                        if (data.success) {
                            showSuccessPopup(data.message);
                            // Refresh the page to show the updated product
                            window.location.reload();
                        }
                        else {
                            showErrorPopup(data.message);
                        }
                    })
                    .catch(error => {
                        closeLoadingPopup();
                        showErrorPopup('An error occurred while editing the product');
                    });
            }
            else {
                showErrorPopup('Please enter a valid product name and description');
            }
        }
    });
}

// Delete product popup
function showDeleteProductPopup(productId) {
    console.log('Attempting to delete product with ID:', productId);

    Swal.fire({
        title: 'Delete Product',
        html: 'Are you sure you want to delete this product?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, Delete',
        cancelButtonText: 'Cancel',
        showCloseButton: true,
        showConfirmButton: true,
        background: '#2d2d2d',
        customClass: {
            popup: 'swal2-popup-dark',
            title: 'text-light',
            content: 'text-light',
            confirmButton: 'btn btn-danger',
            cancelButton: 'btn btn-secondary'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            showLoadingPopup('Deleting product...');

            // Get the anti-forgery token
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            // Create URLSearchParams
            const params = new URLSearchParams();
            params.append('id', productId);

            fetch('/Product/Delete', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: params.toString(),
                credentials: 'same-origin'
            })
                .then(response => response.json())
                .then(data => {
                    closeLoadingPopup();
                    if (data.success) {
                        showSuccessPopup(data.message);
                        window.location.reload();
                    }
                    else {
                        showErrorPopup(data.message);
                    }
                })
                .catch(error => {
                    closeLoadingPopup();
                    showErrorPopup('An error occurred while deleting the product');
                });
        }
    });
}

// Delete farmer popup
function showDeleteFarmerPopup(userId, farmerName) {
    console.log('Attempting to delete farmer with ID:', userId);

    Swal.fire({
        title: 'Delete Farmer',
        html: `Are you sure you want to delete ${farmerName}?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Yes, Delete',
        cancelButtonText: 'Cancel',
        showCloseButton: true,
        showConfirmButton: true,
        background: '#2d2d2d',
        customClass: {
            popup: 'swal2-popup-dark',
            title: 'text-light',
            content: 'text-light',
            confirmButton: 'btn btn-danger',
            cancelButton: 'btn btn-secondary'
        }
    }).then((result) => {
        if (result.isConfirmed) {
            showLoadingPopup('Deleting farmer...');

            // Get the anti-forgery token
            const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

            // Create URLSearchParams
            const params = new URLSearchParams();
            params.append('userId', userId);

            fetch('/Farmer/DeleteFarmer', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token,
                    'X-Requested-With': 'XMLHttpRequest',
                    'Content-Type': 'application/x-www-form-urlencoded'
                },
                body: params.toString(),
                credentials: 'same-origin'
            })
                .then(response => response.json())
                .then(data => {
                    closeLoadingPopup();
                    if (data.success) {
                        showSuccessPopup(data.message);
                        window.location.reload();
                    }
                    else {
                        showErrorPopup(data.message);
                    }
                })
                .catch(error => {
                    closeLoadingPopup();
                    showErrorPopup('An error occurred while deleting the farmer');
                });
        }
    });
}

// Function to load farmer details
function loadFarmerDetails(userId, cardElement) {
    fetch(`/Farmer/GetFarmerDetails?userId=${userId}`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
    .then(response => response.json())
    .then(data => {
        if (data.success) {
            // Update the farm name and location in the specific card
            const paragraphs = cardElement.querySelectorAll('p.text-secondary');
            if (paragraphs.length >= 2) {
                paragraphs[0].textContent = data.farmName || 'Not specified';
                paragraphs[1].textContent = data.location || 'Not specified';
            }
        } else {
            console.error('Error loading farmer details:', data.message);
        }
    })
    .catch(error => {
        console.error('Error fetching farmer details:', error);
    });
}
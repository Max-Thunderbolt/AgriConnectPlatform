// Modal functionality
document.addEventListener('DOMContentLoaded', function () {
    // Get login modal elements
    const loginModal = document.getElementById('loginModal');
    const loginBtn = document.getElementById('loginBtn');
    const loginCloseBtn = document.querySelector('#loginModal .close-modal');
    
    // Get register modal elements
    const registerModal = document.getElementById('registerModal');
    const registerBtn = document.getElementById('registerBtn');
    const registerCloseBtn = document.querySelector('#registerModal .close-modal');
    
    // Get switch buttons
    const loginFromRegisterBtn = document.getElementById('loginBtnFromRegister');
    const registerFromLoginBtn = document.getElementById('registerBtnFromLogin');

    // Open login modal when login button is clicked
    if (loginBtn) {
        loginBtn.addEventListener('click', function () {
            loginModal.style.display = 'block';
        });
    }

    // Open register modal when register button is clicked
    if (registerBtn) {
        registerBtn.addEventListener('click', function () {
            registerModal.style.display = 'block';
        });
    }
    
    // Switch from register to login modal
    if (loginFromRegisterBtn) {
        loginFromRegisterBtn.addEventListener('click', function (e) {
            e.preventDefault();
            registerModal.style.display = 'none';
            loginModal.style.display = 'block';
        });
    }

    // Switch from login to register modal
    if (registerFromLoginBtn) {
        registerFromLoginBtn.addEventListener('click', function (e) {
            e.preventDefault(); 
            loginModal.style.display = 'none';
            registerModal.style.display = 'block';
        });
    }

    // Close login modal when X is clicked
    if (loginCloseBtn) {
        loginCloseBtn.addEventListener('click', function () {
            loginModal.style.display = 'none';
        });
    }

    // Close register modal when X is clicked
    if (registerCloseBtn) {
        registerCloseBtn.addEventListener('click', function () {
            registerModal.style.display = 'none';
        });
    }

    // Close modals when clicking outside of them
    window.addEventListener('click', function (event) {
        if (event.target === loginModal) {
            loginModal.style.display = 'none';
        }
        if (event.target === registerModal) {
            registerModal.style.display = 'none';
        }
    });

    // Login user
    async function loginUser(email, password) {
        try {
            const response = await fetch('/auth/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, password })
            });

            if (response.ok) {
                return response;
            } else {
                throw new Error('Login failed');
            }
        } catch (error) {
            console.error('Login error:', error);
            const errorMessage = document.getElementById('loginErrorMessage');
            if (errorMessage) {
                errorMessage.textContent = 'An error occurred during login';
                errorMessage.style.display = 'block';
            }
        }
    }

    // Register user
    async function registerUser(email, password) {
        try {
            const response = await fetch('/auth/register', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ email, password })
            });

            if (response.ok) {
                return response;
            } else {
                throw new Error('Registration failed');
            }
        } catch (error) {
            console.error('Registration error:', error);
            const errorMessage = document.getElementById('registerErrorMessage');
            if (errorMessage) {
                errorMessage.textContent = 'An error occurred during registration';
                errorMessage.style.display = 'block';
            }
        }
    }

    // Handle login form submission
    const loginForm = document.getElementById('loginForm');
    if (loginForm) {
        loginForm.addEventListener('submit', async function (event) {
            event.preventDefault();

            // Get form values
            const email = document.getElementById('loginEmail').value;
            const password = document.getElementById('loginPassword').value;

            // Now send user data to Supabase auth service
            const response = await loginUser(email, password);
            if (response && response.ok) {
                // Login successful, close modal and redirect
                loginModal.style.display = 'none';
                window.location.href = '/';
            } else {
                // Login failed, show error message
                const errorMessage = document.getElementById('loginErrorMessage');
                if (errorMessage) {
                    errorMessage.textContent = 'Invalid email or password';
                    errorMessage.style.display = 'block';
                }
            }
        });
    }

    // Handle register form submission
    const registerForm = document.getElementById('registerForm');
    if (registerForm) {
        registerForm.addEventListener('submit', async function (event) {
            event.preventDefault();

            // Get form values
            const email = document.getElementById('registerEmail').value;
            const password = document.getElementById('registerPassword').value;

            // Sanitize email and password
            const sanitizedEmail = email.trim().toLowerCase();
            const sanitizedPassword = password.trim();

            // Validate email format
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!emailRegex.test(sanitizedEmail)) {
                const errorMessage = document.getElementById('registerErrorMessage');
                errorMessage.textContent = 'Invalid email address';
                errorMessage.style.display = 'block';
                return;
            }

            // Check password length
            if (sanitizedPassword.length < 8) {
                const errorMessage = document.getElementById('registerErrorMessage');
                errorMessage.textContent = 'Password must be at least 8 characters long';
                errorMessage.style.display = 'block';
                return;
            }

            // Send registration data to server
            const response = await registerUser(sanitizedEmail, sanitizedPassword);
            if (response && response.ok) {
                // Registration successful, close modal and redirect
                registerModal.style.display = 'none';
                window.location.href = '/';
            } else {
                // Registration failed, show error message
                const errorMessage = document.getElementById('registerErrorMessage');
                if (errorMessage) {
                    errorMessage.textContent = 'Registration failed. Please try again.';
                    errorMessage.style.display = 'block';
                }
            }
        });
    }
}); 
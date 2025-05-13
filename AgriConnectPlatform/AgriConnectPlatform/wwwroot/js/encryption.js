const ENCRYPTION_KEY = 'AgriConnect!@#$%^&*()';

// Function to encrypt data
function encryptData(data) {
    try {
        // Convert the data to a string if it's an object
        const dataString = typeof data === 'object' ? JSON.stringify(data) : data;
        
        // Create a simple XOR encryption
        let encrypted = '';
        for (let i = 0; i < dataString.length; i++) {
            const charCode = dataString.charCodeAt(i) ^ ENCRYPTION_KEY.charCodeAt(i % ENCRYPTION_KEY.length);
            encrypted += String.fromCharCode(charCode);
        }
        
        // Convert to base64 for safe transmission
        return btoa(encrypted);
    } catch (error) {
        console.error('Encryption error:', error);
        return null;
    }
}

// Function to decrypt data
function decryptData(encryptedData) {
    try {
        // Convert from base64
        const decoded = atob(encryptedData);
        
        // Decrypt using XOR
        let decrypted = '';
        for (let i = 0; i < decoded.length; i++) {
            const charCode = decoded.charCodeAt(i) ^ ENCRYPTION_KEY.charCodeAt(i % ENCRYPTION_KEY.length);
            decrypted += String.fromCharCode(charCode);
        }
        
        // Try to parse as JSON if it's an object
        try {
            return JSON.parse(decrypted);
        } catch {
            return decrypted;
        }
    } catch (error) {
        console.error('Decryption error:', error);
        return null;
    }
} 
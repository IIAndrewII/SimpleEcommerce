public class PasswordHashingService
{
    // Method to hash a password
    public string HashPassword(string password)
    {
        // Generate a salt with a strength of 12
        string salt = BCrypt.Net.BCrypt.GenerateSalt(12);

        // Hash the password with the salt
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

        return hashedPassword;
    }

    // Method to verify a password against a hashed password
    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }
}

using System.Security.Cryptography;

public class SecurityHelper
{
    private const int saltSize = 16; // Use const to prevent static interference
    private const int hashSize = 32;
    private const int iteration = 10000;

    public static string HashPassword(string password)
    {
        byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
            password: password,
            salt: salt,
            iterations: iteration,
            hashAlgorithm: HashAlgorithmName.SHA256,
            outputLength: hashSize
        );

        byte[] hashBytes = new byte[saltSize + hashSize];
        Array.Copy(salt, 0, hashBytes, 0, saltSize);
        Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

        return Convert.ToBase64String(hashBytes);
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        try
        {
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Safety check: if the hash is too short, it's corrupted
            if (hashBytes.Length != saltSize + hashSize) return false;

            byte[] salt = new byte[saltSize];
            Array.Copy(hashBytes, 0, salt, 0, saltSize);

            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                iterations: iteration,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: hashSize
            );

            // Compare the generated hash with the portion of hashBytes after the salt
            return CryptographicOperations.FixedTimeEquals(hash, hashBytes.AsSpan(saltSize, hashSize));
        }
        catch
        {
            return false;
        }
    }
}
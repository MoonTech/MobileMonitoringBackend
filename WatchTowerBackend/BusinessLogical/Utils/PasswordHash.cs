namespace WatchTowerBackend.BusinessLogical.Utils;

public static class PasswordHash
{
    public static string HashPassword(string? password)
    {
        if (password == null || password == "")
        {
            return "";
        }
        return BCrypt.Net.BCrypt.HashPassword(password, 11);
    }

    public static bool VerifyPassword(string? password, string? hash)
    {
        if (hash == null)
        {
            return true;
        }
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
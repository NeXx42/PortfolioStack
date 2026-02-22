using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Api.Types;
using Portfolio.Core.DTOs;
using Portfolio.Core.Models;

namespace Portfolio.Api.Services;

public class EncryptionService
{
    private readonly PasswordHasher<object> _hasher = new();

    private readonly byte[] _databaseCipher;
    private readonly byte[] _jwtKey;

    public EncryptionService(IOptions<SecuritySettings> config)
    {
        _databaseCipher = Convert.FromBase64String(config.Value.databaseCipher);
        _jwtKey = Convert.FromBase64String(config.Value.jwtToken);
    }

    public SigningCredentials GetJWTSingingCredentials()
    {
        return new SigningCredentials(new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature);
    }

    public string ComputeEmailHash(string email)
    {
        var normalized = email.Trim().ToLowerInvariant();
        using var hmac = new HMACSHA256(_databaseCipher);

        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(normalized));
        return Convert.ToHexString(hash);
    }

    public string EncryptAndHashPassword(string password)
        => _hasher.HashPassword(null!, password);

    public bool CheckPassword(UserModel usr, string plaintextPassword)
        => _hasher.VerifyHashedPassword(null!, usr.passwordHash, plaintextPassword) == PasswordVerificationResult.Success;

    public string EncryptData(string field)
    {
        byte[] nonce = RandomNumberGenerator.GetBytes(12); // 96-bit nonce for GCM
        byte[] plaintextBytes = Encoding.UTF8.GetBytes(field);

        byte[] cipherText = new byte[plaintextBytes.Length];
        byte[] tag = new byte[16]; // 128-bit auth tag

        using AesGcm aes = new AesGcm(_databaseCipher, tag.Length);
        aes.Encrypt(nonce, plaintextBytes, cipherText, tag);

        byte[] combined = new byte[nonce.Length + tag.Length + cipherText.Length];

        Buffer.BlockCopy(nonce, 0, combined, 0, nonce.Length);
        Buffer.BlockCopy(tag, 0, combined, nonce.Length, tag.Length);
        Buffer.BlockCopy(cipherText, 0, combined, nonce.Length + tag.Length, cipherText.Length);

        return Convert.ToBase64String(combined);
    }

    public string DecryptData(string field)
    {
        byte[] combined = Convert.FromBase64String(field);

        byte[] nonce = combined[..12];
        byte[] tag = combined[12..28];
        byte[] cipherText = combined[28..];

        byte[] plaintextBytes = new byte[cipherText.Length];

        using AesGcm aes = new AesGcm(_databaseCipher, tag.Length);
        aes.Decrypt(nonce, cipherText, tag, plaintextBytes);

        return Encoding.UTF8.GetString(plaintextBytes);
    }


    public UserDto DecryptUserModel(UserModel usr)
    {
        return new UserDto()
        {
            id = usr.userId,
            email = DecryptData(usr.email),
            displayName = DecryptData(usr.displayName),
            role = usr.Role ?? Core.Data.UserRoles.None
        };
    }
}
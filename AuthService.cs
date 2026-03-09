using System;
using System.Data;
using System.Data.SQLite;
using System.Security.Cryptography;

namespace InventoryManagementSystem
{
    internal static class AuthService
    {
        private const string ConnectionString = "Data Source=inventory.db;Version=3;";
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 10000;

        public static void EnsureUserTable()
        {
            const string query = "CREATE TABLE IF NOT EXISTS Users (" +
                                 "UserID INTEGER PRIMARY KEY AUTOINCREMENT," +
                                 "Username TEXT UNIQUE," +
                                 "PasswordHash BLOB," +
                                 "Salt BLOB)";
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static bool CreateUser(string username, string password, out string error)
        {
            error = null;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                error = "Username and password are required.";
                return false;
            }

            var salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            var hash = HashPassword(password, salt);

            const string insert = "INSERT INTO Users(Username, PasswordHash, Salt) VALUES(@username, @hash, @salt)";
            try
            {
                using (var con = new SQLiteConnection(ConnectionString))
                {
                    con.Open();
                    using (var cmd = new SQLiteCommand(insert, con))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@hash", hash);
                        cmd.Parameters.AddWithValue("@salt", salt);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (SQLiteException ex) when (ex.ResultCode == SQLiteErrorCode.Constraint)
            {
                error = "Username already exists.";
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static bool ValidateUser(string username, string password)
        {
            const string query = "SELECT PasswordHash, Salt FROM Users WHERE Username = @username LIMIT 1";
            using (var con = new SQLiteConnection(ConnectionString))
            {
                con.Open();
                using (var cmd = new SQLiteCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    using (var rdr = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (!rdr.Read())
                            return false;

                        var dbHash = (byte[])rdr["PasswordHash"];
                        var dbSalt = (byte[])rdr["Salt"];
                        var computed = HashPassword(password, dbSalt);
                        return AreEqual(dbHash, computed);
                    }
                }
            }
        }

        private static byte[] HashPassword(string password, byte[] salt)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                return pbkdf2.GetBytes(HashSize);
            }
        }

        // constant-time comparison
        private static bool AreEqual(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            var diff = 0;
            for (var i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
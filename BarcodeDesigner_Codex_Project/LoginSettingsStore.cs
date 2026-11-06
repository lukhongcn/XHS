using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;

namespace BarcodeDesigner
{
    public sealed class LoginSettings
    {
        public string UserName { get; set; }
        public string EncryptedPassword { get; set; }
        public string CustomerAbbr { get; set; }
    }

    public sealed class LoginSettingsStore
    {
        private readonly string filePath;
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public LoginSettingsStore()
            : this(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BarcodeDesigner",
                "LoginSettings.json"))
        {
        }

        public LoginSettingsStore(string filePath)
        {
            this.filePath = filePath;
        }

        public LoginSettings Load()
        {
            if (!File.Exists(filePath))
            {
                return new LoginSettings();
            }

            try
            {
                LoginSettings settings = serializer.Deserialize<LoginSettings>(File.ReadAllText(filePath));
                if (settings == null)
                {
                    return new LoginSettings();
                }

                return settings;
            }
            catch
            {
                return new LoginSettings();
            }
        }

        public void Save(string userName, string password, string customerAbbr)
        {
            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            LoginSettings settings = new LoginSettings
            {
                UserName = userName ?? string.Empty,
                EncryptedPassword = Encrypt(password ?? string.Empty),
                CustomerAbbr = customerAbbr ?? string.Empty
            };
            File.WriteAllText(filePath, serializer.Serialize(settings), Encoding.UTF8);
        }

        public string DecryptPassword(string encryptedPassword)
        {
            if (string.IsNullOrWhiteSpace(encryptedPassword))
            {
                return string.Empty;
            }

            try
            {
                byte[] protectedBytes = Convert.FromBase64String(encryptedPassword);
                byte[] bytes = ProtectedData.Unprotect(
                    protectedBytes,
                    null,
                    DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return string.Empty;
            }
        }

        private static string Encrypt(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value ?? string.Empty);
            byte[] protectedBytes = ProtectedData.Protect(
                bytes,
                null,
                DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(protectedBytes);
        }
    }
}

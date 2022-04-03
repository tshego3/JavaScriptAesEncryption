using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

public class Program
{
	public static void Main()
	{
		string original = "hello";
		string secretIV = "Example IV";
		string secretKey = "Example Secret Key";

		byte[] plainText = Encoding.Unicode.GetBytes(original);
		byte[] keyBytes = Convert.FromBase64String("c69f3969d235d7b7d5de5d0def95980847d27f358c730759b0ce96c5467e9f6071da8e8a958dcf46b3a2bf639a640b4e");
		string cipherText;

		var pdb = new Rfc2898DeriveBytes(secretKey, Encoding.ASCII.GetBytes(secretIV));
		byte[] key = pdb.GetBytes(32);
		byte[] iV = pdb.GetBytes(16);

		// Encrypt the string to an array of bytes.
		Aes myAes = Aes.Create();
		myAes.Key = pdb.GetBytes(32);
		myAes.IV = pdb.GetBytes(16);
		byte[] encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);
		string encryptedString = Convert.ToBase64String(encrypted);

		// Decrypt the bytes to a string.
		//string roundtrip = DecryptStringFromBytes_Aes(keyBytes, key, iV);
		Console.WriteLine("Original:   {0}", original);
		Console.WriteLine("encryptedString:   {0}", encryptedString);

		Console.WriteLine("encryptedString:   {0}", EncryptString(original));
		//Console.WriteLine("Round Trip: {0}", roundtrip);
	}

	static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
	{
		// Check arguments.
		if (cipherText == null || cipherText.Length <= 0)
			throw new ArgumentNullException("cipherText");
		if (Key == null || Key.Length <= 0)
			throw new ArgumentNullException("Key");
		if (IV == null || IV.Length <= 0)
			throw new ArgumentNullException("IV");

		// Declare the string used to hold
		// the decrypted text.
		string plaintext = null;

		// Create an Aes object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create())
		{
			aesAlg.Key = Key;
			aesAlg.IV = IV;

			// Create a decryptor to perform the stream transform.
			ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for decryption.
			using (MemoryStream msDecrypt = new MemoryStream(cipherText))
			{
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
				{
					using (StreamReader srDecrypt = new StreamReader(csDecrypt))
					{

						// Read the decrypted bytes from the decrypting stream
						// and place them in a string.
						plaintext = srDecrypt.ReadToEnd();
					}
				}
			}
		}

		return plaintext;
	}

	static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
	{
		// Check arguments.
		if (plainText == null || plainText.Length <= 0)
			throw new ArgumentNullException("plainText");
		if (Key == null || Key.Length <= 0)
			throw new ArgumentNullException("Key");
		if (IV == null || IV.Length <= 0)
			throw new ArgumentNullException("IV");
		byte[] encrypted;

		// Create an Aes object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create())
		{
			aesAlg.Key = Key;
			aesAlg.IV = IV;

			// Create an encryptor to perform the stream transform.
			ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for encryption.
			using (MemoryStream msEncrypt = new MemoryStream())
			{
				using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
				{
					csEncrypt.Write(Encoding.Unicode.GetBytes(plainText), 0, plainText.Length);
					csEncrypt.Close();
					encrypted = msEncrypt.ToArray();
				}
			}
		}

		// Return the encrypted bytes from the memory stream.
		return encrypted;
	}

	static string EncryptString(string Text)
	{
		string secretIV = "Example IV";
		string secretKey = "Example Secret Key";
		byte[] plainText = Encoding.Unicode.GetBytes(Text); // this is UTF-16 LE
		string cipherText;
		using (Aes encryptor = Aes.Create())
		{
			var pdb = new Rfc2898DeriveBytes(secretKey, Encoding.ASCII.GetBytes(secretIV));
			encryptor.Key = pdb.GetBytes(32);
			encryptor.IV = pdb.GetBytes(16);
			using (MemoryStream ms = new MemoryStream())
			{
				using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
				{
					cs.Write(plainText, 0, plainText.Length);
					cs.Close();
				}
				cipherText = Convert.ToBase64String(ms.ToArray());
			}
		}

		return cipherText;
	}
}
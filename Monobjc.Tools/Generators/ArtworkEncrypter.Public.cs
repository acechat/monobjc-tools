﻿//
// This file is part of Monobjc, a .NET/Objective-C bridge
// Copyright (C) 2007-2013 - Laurent Etiemble
//
// Monobjc is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.
//
// Monobjc is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Monobjc.  If not, see <http://www.gnu.org/licenses/>.
//
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Monobjc.Tools.Generators
{
	public partial class ArtworkEncrypter
	{
		public static Aes GetProvider(String encryptionSeed) {
			AesCryptoServiceProvider provider = new AesCryptoServiceProvider();
			provider.Key = DeriveKey(encryptionSeed);
			return provider;
		}

		public static byte[] DeriveKey (String seed)
		{
			return DeriveKey (Encoding.UTF8.GetBytes (seed));
		}
		
		public static byte[] DeriveKey (byte[] content)
		{
			SHA256 sha = SHA256Managed.Create ();
			byte[] result = sha.ComputeHash (content);
			return result;
		}
		
		public static byte[] Encrypt (byte[] content, Aes aes)
		{
			//
			// Encrypt a byte array by using AES
			//
			// Input:
			// - data
			//
			// Output:
			// - Magic Number (4 bytes)
			// - IV (16 bytes)
			// - AES(data)
			// - HMAC(IV || AES(data)) (32 bytes)
			//
			using (MemoryStream outputStream = new MemoryStream()) {
				// Write the magic number
				outputStream.Write (MAGIC_NUMBER, 0, 4);
				
				using (MemoryStream cypherStream = new MemoryStream()) {
					// Write the IV
					cypherStream.Write (aes.IV, 0, 16);

					// Write the AES encrypted content
					using (ICryptoTransform transform = aes.CreateEncryptor ()) {
						using (CryptoStream cryptoStream = new CryptoStream(cypherStream, transform, CryptoStreamMode.Write)) {
							cryptoStream.Write(content, 0, content.Length);
							cryptoStream.Flush();
							cryptoStream.Close();
						}
					}

					// Collect cypher result
					cypherStream.Flush ();
					cypherStream.Close ();
					byte[] cypherBytes = cypherStream.ToArray ();

					// Write the cypher
					outputStream.Write (cypherBytes, 0, cypherBytes.Length);

					// Compute the HMAC of the cypher
					using (HMACSHA256 hmac = new HMACSHA256 (DeriveKey (aes.Key))) {
						byte[] hmacBytes = hmac.ComputeHash (cypherBytes);

						// Write the HMAC
						outputStream.Write (hmacBytes, 0, hmacBytes.Length);
					}
				}

				outputStream.Flush ();
				outputStream.Close ();
				byte[] outputBytes = outputStream.ToArray ();

				return outputBytes;
			}
		}

		public static byte[] Decrypt (byte[] content, Aes aes)
		{
			//
			// Decrypt a byte array by using AES
			//
			// Input:
			// - Magic Number (4 bytes)
			// - IV (16 bytes)
			// - AES(data)
			// - HMAC(IV || AES(data)) (32 bytes)
			//
			// Output:
			// - data
			//
			byte[] headerBytes = new byte[4];
			byte[] ivBytes = new byte[16];
			byte[] dataBytes = new byte[content.Length - 4 - 16 - 32];
			byte[] cypherBytes = new byte[ivBytes.Length + dataBytes.Length];
			byte[] hmacBytes = new byte[32];

			Array.Copy(content, 0, headerBytes, 0, headerBytes.Length);
			Array.Copy(content, 4, ivBytes, 0, ivBytes.Length);
			Array.Copy(content, 4 + 16, dataBytes, 0, dataBytes.Length);
			Array.Copy(content, 4, cypherBytes, 0, cypherBytes.Length);
			Array.Copy(content, content.Length - 32, hmacBytes, 0, hmacBytes.Length);

			// Compute the HMAC of the cypher
			using (HMACSHA256 hmac = new HMACSHA256 (DeriveKey (aes.Key))) {
				byte[] newHmacBytes = hmac.ComputeHash (cypherBytes);

				// Check for HMAC equality
				for(int i = 0; i < newHmacBytes.Length; i++) {
					if (newHmacBytes[i] != hmacBytes[i]) {
						throw new CryptographicException("Content has been tampered. HMAC don't match.");
					}
				}
			}

			using (MemoryStream outputStream = new MemoryStream()) {
				// Report the IV
				aes.IV = ivBytes;
				
				// Write the AES decrypted content
				using (ICryptoTransform transform = aes.CreateDecryptor ()) {
					using (CryptoStream cryptoStream = new CryptoStream(outputStream, transform, CryptoStreamMode.Write)) {
						cryptoStream.Write (dataBytes, 0, dataBytes.Length);
					}
				}
				
				// Collect cypher result
				outputStream.Flush ();
				outputStream.Close ();
				byte[] outputBytes = outputStream.ToArray ();

				return outputBytes;
			}
		}

		public static bool IsEncrypted (String file)
		{
			byte[] header = new byte[4];
			using (Stream sourceStream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
				sourceStream.Read (header, 0, 4);
				sourceStream.Close ();
			}
			return IsEncrypted (header);
		}

		public static bool IsEncrypted (byte[] header)
		{
			if (header.Length < 4) {
				throw new ArgumentException("Header should contain at least 4 bytes");
			}

			bool isEncrypted = true;
			isEncrypted &= (header [0] == MAGIC_NUMBER [0]);
			isEncrypted &= (header [1] == MAGIC_NUMBER [1]);
			isEncrypted &= (header [2] == MAGIC_NUMBER [2]);
			isEncrypted &= (header [3] == MAGIC_NUMBER [3]);
			return isEncrypted;
		}
	}
}

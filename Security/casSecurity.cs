using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace CommandAS.Tools.Security
{
	/// <summary>
	/// 
	/// </summary>
	public class CasSymmCrypto
	{
		public enum eSymmetricAlgorithm
		{
			DES				= 0,
			RC2				= 1,
			TripleDES = 2,
			Rijndael	= 3
		}

		#region PROPERTY:

		//protected MemoryStream								mMemStream;
		protected SymmetricAlgorithm					mSymmAlg;
		protected byte[]											mIV;

		protected byte[]											mBuff;
		protected byte[]											mBuffCipher;


		public byte[]													pBuff
		{
			set { mBuff = value; }
			get { return mBuff;  }
		}
		public string													pStringBuff
		{
			set { mBuff = Encoding.Default.GetBytes(value); }
			get { return Encoding.Default.GetString(mBuff); }
		}

		public byte[]													pBuffCipher
		{
			set { mBuffCipher = value; }
			get { return mBuffCipher;  }
		}
		public string													pStringBuffCipher
		{
			//set { mBuffCipher = Encoding.Unicode.GetBytes(value); }
			//get { return Encoding.Unicode.GetString(mBuffCipher); }
			set 
			{
				try
				{
					if (value.Trim().Length >0)
						mBuffCipher = Convert.FromBase64String(value); 
				}
				catch (Exception ex)
				{
					pErr.ex = ex;
				}
			}
			get 
			{ 
				if (mBuffCipher != null && mBuffCipher.Length > 0)
					return Convert.ToBase64String(mBuffCipher);
				else
					return string.Empty;
			}
		}

		public eSymmetricAlgorithm						pSymmAlg
		{
			set 
			{
				mSymmAlg = SymmetricAlgorithm.Create(GetSymmetricAlgorithmText(value));
			}
		}

		public int														pKeySize
		{
			get 
			{
				if (mSymmAlg != null)
					return mSymmAlg.KeySize;
				else
					return 0;
			}
		}

		public Error													pErr;

		#endregion property

		public CasSymmCrypto() : this (eSymmetricAlgorithm.TripleDES) {}
		public CasSymmCrypto(eSymmetricAlgorithm aSymmAlg)
		{
			pErr = new Error();
			pSymmAlg = aSymmAlg;
			mIV = Encoding.Default.GetBytes("shgfvbnmw3cvakskf4dhfja3sf This is initial vector");
		}

		public bool EncryptingBuf(string aPassphrase)
		{

			if (aPassphrase.Length < 4)
			{
				pErr.text = "Не допустимый размер пароля (ключевой фразы)";
				return false;
			}

			if(GetKeyFromPassphrase(aPassphrase))
				return EncryptingBuf();
			else
				return false;
		}

		public bool EncryptingBuf()
		{
			if (mBuff == null || mBuff.Length == 0)
			{
				pErr.text = "Входной буфер пуст";
				return false;
			}

			bool ret = true;
			try
			{
				ICryptoTransform encryptor = mSymmAlg.CreateEncryptor();
				MemoryStream mMemStream = new MemoryStream();
				CryptoStream crStream = new CryptoStream(mMemStream, encryptor, CryptoStreamMode.Write);
				crStream.Write(mBuff,0, mBuff.Length);
				crStream.FlushFinalBlock();
				crStream.Close();
				mBuffCipher = mMemStream.ToArray();
			}
			catch (Exception ex)
			{
				pErr.ex = ex;
				ret = false;
			}

			return ret;
		}

		public bool DecryptingBuf(string aPassphrase)
		{
			if (aPassphrase.Length < 4)
			{
				pErr.text = "Не допустимый размер пароля (ключевой фразы)";
				return false;
			}

			if(GetKeyFromPassphrase(aPassphrase))
				return DecryptingBuf();
			else
				return false;
		}

		public bool DecryptingBuf()
		{
			if (mBuffCipher == null || mBuffCipher.Length == 0)
			{
				pErr.text = "Входной буфер пуст";
				return false;
			}

			bool ret = true;
			try
			{
				ICryptoTransform decryptor = mSymmAlg.CreateDecryptor();
				MemoryStream mMemStream = new MemoryStream();
				CryptoStream crStream = new CryptoStream(mMemStream, decryptor, CryptoStreamMode.Write);
				crStream.Write(mBuffCipher,0, mBuffCipher.Length);
				crStream.FlushFinalBlock();
				crStream.Close();
				mBuff = mMemStream.ToArray();
			}
			catch (Exception ex)
			{
				pErr.ex = ex;
				ret = false;
			}

			return ret;
		}

		public bool GetKeyFromPassphrase (string aPassphrase)
		{
			try
			{
				//класс, позволяющий генерировать ключи на базе паролей
				PasswordDeriveBytes pdb=new PasswordDeriveBytes(aPassphrase,null); 
				pdb.HashName="SHA512"; //будем использовать SHA512
				int maxKeySize = 0;
				foreach (KeySizes ks in mSymmAlg.LegalKeySizes)
					if (ks.MaxSize > maxKeySize)
						maxKeySize = ks.MaxSize;
				mSymmAlg.KeySize=maxKeySize; //устанавливаем размер ключа
				mSymmAlg.Key=pdb.GetBytes(maxKeySize>>3); //получаем ключ из пароля
				//mSymmAlg.BlockSize = maxKeySize;
				mSymmAlg.Mode=CipherMode.CBC; //используем режим CBC
				//mSymmAlg.Padding=PaddingMode.Zeros;
				mSymmAlg.IV=new Byte[mSymmAlg.BlockSize>>3]; //и пустой инициализационный вектор
				for (int ii = 0; ii<mSymmAlg.IV.Length && ii<mIV.Length; ii++)
					mSymmAlg.IV[ii]=mIV[ii];
				return true;
			}
			catch (Exception ex)
			{
				pErr.ex = ex;
				return false;
			}
		}

		private string GetSymmetricAlgorithmText (eSymmetricAlgorithm aSA)
		{
			string ret = string.Empty;
			switch (aSA)
			{
				case eSymmetricAlgorithm.DES:
					ret = "DES";
					break;
				case eSymmetricAlgorithm.RC2:
					ret = "RC2";
					break;
				case eSymmetricAlgorithm.Rijndael:
					ret = "Rijndael";
					break;
				case eSymmetricAlgorithm.TripleDES:
					ret = "TripleDES";
					break;
			}
			return ret;
		}


	}
}

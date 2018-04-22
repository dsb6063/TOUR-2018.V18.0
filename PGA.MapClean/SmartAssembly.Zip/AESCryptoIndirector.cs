using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace SmartAssembly.Zip
{
	public sealed class AESCryptoIndirector : IDisposable
	{
		private readonly Type m_AcspType;

		private readonly object m_AESCryptoServiceProvider;

		public AESCryptoIndirector()
		{
			try
			{
				this.m_AcspType = Assembly.Load("System.Core, Version=2.0.5.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e").GetType("System.Security.Cryptography.AesManaged");
			}
			catch (FileNotFoundException fileNotFoundException)
			{
				this.m_AcspType = Assembly.Load("mscorlib").GetType("System.Security.Cryptography.RijndaelManaged");
			}
			this.m_AESCryptoServiceProvider = Activator.CreateInstance(this.m_AcspType);
		}

		public void Clear()
		{
			this.m_AcspType.GetMethod("Clear").Invoke(this.m_AESCryptoServiceProvider, new object[0]);
		}

		public void Dispose()
		{
			this.Clear();
		}

		public ICryptoTransform GetAESCryptoTransform(byte[] key, byte[] iv, bool decrypt)
		{
			MethodInfo setMethod = this.m_AcspType.GetProperty("Key").GetSetMethod();
			object mAESCryptoServiceProvider = this.m_AESCryptoServiceProvider;
			object[] objArray = new object[] { key };
			setMethod.Invoke(mAESCryptoServiceProvider, objArray);
			MethodInfo methodInfo = this.m_AcspType.GetProperty("IV").GetSetMethod();
			object obj = this.m_AESCryptoServiceProvider;
			objArray = new object[] { iv };
			methodInfo.Invoke(obj, objArray);
			MethodInfo method = this.m_AcspType.GetMethod((decrypt ? "CreateDecryptor" : "CreateEncryptor"), new Type[0]);
			return (ICryptoTransform)method.Invoke(this.m_AESCryptoServiceProvider, new object[0]);
		}
	}
}
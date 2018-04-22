using System;
using System.Reflection;
using System.Security.Cryptography;

namespace SmartAssembly.Zip
{
	public sealed class DESCryptoIndirector : IDisposable
	{
		private readonly Type m_DcspType;

		private readonly object m_DESCryptoServiceProvider;

		public DESCryptoIndirector()
		{
			this.m_DcspType = Assembly.Load("mscorlib").GetType("System.Security.Cryptography.DESCryptoServiceProvider");
			this.m_DESCryptoServiceProvider = Activator.CreateInstance(this.m_DcspType);
		}

		public void Clear()
		{
			this.m_DcspType.GetMethod("Clear").Invoke(this.m_DESCryptoServiceProvider, new object[0]);
		}

		public void Dispose()
		{
			this.Clear();
		}

		public ICryptoTransform GetDESCryptoTransform(byte[] key, byte[] iv, bool decrypt)
		{
			MethodInfo setMethod = this.m_DcspType.GetProperty("Key").GetSetMethod();
			object mDESCryptoServiceProvider = this.m_DESCryptoServiceProvider;
			object[] objArray = new object[] { key };
			setMethod.Invoke(mDESCryptoServiceProvider, objArray);
			MethodInfo methodInfo = this.m_DcspType.GetProperty("IV").GetSetMethod();
			object obj = this.m_DESCryptoServiceProvider;
			objArray = new object[] { iv };
			methodInfo.Invoke(obj, objArray);
			MethodInfo method = this.m_DcspType.GetMethod((decrypt ? "CreateDecryptor" : "CreateEncryptor"), new Type[0]);
			return (ICryptoTransform)method.Invoke(this.m_DESCryptoServiceProvider, new object[0]);
		}
	}
}
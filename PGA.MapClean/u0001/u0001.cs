using SmartAssembly.Zip;
using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace 
{
	internal class 
	{
		private static Hashtable ;

		[DllImport("kernel32", CharSet=CharSet.None, EntryPoint="MoveFileEx", ExactSpelling=false)]
		private static extern bool (string , string , int );

		internal static void ()
		{
			try
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(..);
			}
			catch
			{
			}
		}

		internal static Assembly (object obj, ResolveEventArgs resolveEventArg)
		{
			Assembly item;
			.. _u0001 = new ..(resolveEventArg.Name);
			string str = _u0001.(false);
			string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(str));
			string[] strArrays = "QXV0b2Rlc2suQXV0b0NBRC5JbnRlcm9wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGw=,[z]{9c149476-e79e-4a3b-9b89-a70b80b0b3c3},QXV0b2Rlc2suQXV0b0NBRC5JbnRlcm9wLkNvbW1vbiwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1udWxs,[z]{65457a50-6bf7-4d09-a63a-dbfae1c62950},SW50ZXJvcC5BY1N0TWdyLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGw=,[z]{b4f3a309-21ad-45ce-a339-0ede9f44a96b},SW50ZXJvcC5BWERCTGliLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGw=,[z]{d304c6a0-3481-4b3c-ad88-bc59a5017636},TWljcm9zb2Z0Lk9mZmljZS5JbnRlcm9wLkV4Y2VsLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPTcxZTliY2UxMTFlOTQyOWM=,[z]{660c805b-a607-45f1-afaa-1c24766768e3},TWljcm9zb2Z0Lk9mZmljZS5JbnRlcm9wLkV4Y2Vs,[z]{660c805b-a607-45f1-afaa-1c24766768e3},TWljcm9zb2Z0LlZiZS5JbnRlcm9wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPTcxZTliY2UxMTFlOTQyOWM=,[z]{a9038fb1-5e10-41e6-8f31-34d556a6a211},TWljcm9zb2Z0LlZiZS5JbnRlcm9w,[z]{a9038fb1-5e10-41e6-8f31-34d556a6a211},b2ZmaWNlLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPTcxZTliY2UxMTFlOTQyOWM=,[z]{0ce928d2-0401-4ba2-a177-83037ae1c353},b2ZmaWNl,[z]{0ce928d2-0401-4ba2-a177-83037ae1c353},UEdBLkF1dG9kZXNrLlNldHRpbmdzLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGw=,[z]{97f07e53-114b-44a4-9a14-90b7962bdaf2},UEdBLkNvbW1vbi5BdXRvQ0FELCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGw=,[z]{18d95c1c-90ae-4f6f-b33a-6fe2bbaffbe8},UEdBLkNvbW1vbi5Mb2dnaW5nLk1hcENsZWFuLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGw=,[z]{8653f348-3e46-4594-b25b-acc1599077d1}".Split(new char[] { ',' });
			string empty = string.Empty;
			bool flag = false;
			bool flag1 = false;
			int num = 0;
			while (num < (int)strArrays.Length - 1)
			{
				if (strArrays[num] != base64String)
				{
					num = num + 2;
				}
				else
				{
					empty = strArrays[num + 1];
					break;
				}
			}
			if (empty.Length == 0 && _u0001..Length == 0)
			{
				base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(_u0001.));
				int num1 = 0;
				while (num1 < (int)strArrays.Length - 1)
				{
					if (strArrays[num1] != base64String)
					{
						num1 = num1 + 2;
					}
					else
					{
						empty = strArrays[num1 + 1];
						break;
					}
				}
			}
			if (empty.Length > 0)
			{
				if (empty[0] == '[')
				{
					int num2 = empty.IndexOf(']');
					string str1 = empty.Substring(1, num2 - 1);
					flag = str1.IndexOf('z') >= 0;
					flag1 = str1.IndexOf('t') >= 0;
					empty = empty.Substring(num2 + 1);
				}
				lock (..)
				{
					if (!...ContainsKey(empty))
					{
						Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(empty);
						if (manifestResourceStream == null)
						{
							return null;
						}
						else
						{
							int length = (int)manifestResourceStream.Length;
							byte[] numArray = new byte[length];
							manifestResourceStream.Read(numArray, 0, length);
							if (flag)
							{
								numArray = SimpleZip.Unzip(numArray);
							}
							Assembly assembly = null;
							if (!flag1)
							{
								try
								{
									assembly = Assembly.Load(numArray);
								}
								catch (FileLoadException fileLoadException)
								{
									flag1 = true;
								}
								catch (BadImageFormatException badImageFormatException)
								{
									flag1 = true;
								}
							}
							if (flag1)
							{
								try
								{
									string str2 = string.Format("{0}{1}\\", Path.GetTempPath(), empty);
									Directory.CreateDirectory(str2);
									string str3 = string.Concat(str2, _u0001., ".dll");
									if (!File.Exists(str3))
									{
										FileStream fileStream = File.OpenWrite(str3);
										fileStream.Write(numArray, 0, (int)numArray.Length);
										fileStream.Close();
										..(str3, null, 4);
										..(str2, null, 4);
									}
									assembly = Assembly.LoadFile(str3);
								}
								catch
								{
								}
							}
							..[empty] = assembly;
							item = assembly;
						}
					}
					else
					{
						item = (Assembly)..[empty];
					}
				}
				return item;
			}
			return null;
		}

		static ()
		{
			.. = new Hashtable();
		}

		public ()
		{
		}

		internal struct 
		{
			public string ;

			public Version ;

			public string ;

			public string ;

			public string (bool flag)
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.);
				if (flag && this. != null)
				{
					stringBuilder.Append(", Version=");
					stringBuilder.Append(this.);
				}
				stringBuilder.Append(", Culture=");
				stringBuilder.Append((this..Length == 0 ? "neutral" : this.));
				stringBuilder.Append(", PublicKeyToken=");
				stringBuilder.Append((this..Length == 0 ? "null" : this.));
				return stringBuilder.ToString();
			}

			public (string str)
			{
				this. = null;
				this. = string.Empty;
				this. = string.Empty;
				this. = string.Empty;
				string[] strArrays = str.Split(new char[] { ',' });
				for (int i = 0; i < (int)strArrays.Length; i++)
				{
					string str1 = strArrays[i].Trim();
					if (str1.StartsWith("Version="))
					{
						this. = new Version(str1.Substring(8));
					}
					else if (str1.StartsWith("Culture="))
					{
						this. = str1.Substring(8);
						if (this. == "neutral")
						{
							this. = string.Empty;
						}
					}
					else if (!str1.StartsWith("PublicKeyToken="))
					{
						this. = str1;
					}
					else
					{
						this. = str1.Substring(15);
						if (this. == "null")
						{
							this. = string.Empty;
						}
					}
				}
			}
		}
	}
}
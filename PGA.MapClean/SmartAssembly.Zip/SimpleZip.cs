using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace SmartAssembly.Zip
{
	public class SimpleZip
	{
		public static string ExceptionMessage;

		public SimpleZip()
		{
		}

		private static bool PublicKeysMatch(Assembly executingAssembly, Assembly callingAssembly)
		{
			byte[] publicKey = executingAssembly.GetName().GetPublicKey();
			byte[] numArray = callingAssembly.GetName().GetPublicKey();
			if (numArray == null != (publicKey == null))
			{
				return false;
			}
			if (numArray != null)
			{
				for (int i = 0; i < (int)numArray.Length; i++)
				{
					if (numArray[i] != publicKey[i])
					{
						return false;
					}
				}
			}
			return true;
		}

		public static byte[] Unzip(byte[] buffer)
		{
			int num = 0;
			Assembly callingAssembly = Assembly.GetCallingAssembly();
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			if ((object)callingAssembly != (object)executingAssembly && !SimpleZip.PublicKeysMatch(executingAssembly, callingAssembly))
			{
				return null;
			}
			SimpleZip.ZipStream zipStream = new SimpleZip.ZipStream(buffer);
			byte[] numArray = new byte[0];
			int num1 = zipStream.ReadInt();
			if (num1 != 67324752)
			{
				int num2 = num1 >> 24;
				num1 = num1 - (num2 << 24);
				if (num1 != 8223355)
				{
					throw new FormatException("Unknown Header");
				}
				if (num2 == 1)
				{
					int num3 = zipStream.ReadInt();
					numArray = new byte[checked((uint)num3)];
					for (int i = 0; i < num3; i = i + num)
					{
						int num4 = zipStream.ReadInt();
						num = zipStream.ReadInt();
						byte[] numArray1 = new byte[checked((uint)num4)];
						zipStream.Read(numArray1, 0, (int)numArray1.Length);
						(new SimpleZip.Inflater(numArray1)).Inflate(numArray, i, num);
					}
				}
				if (num2 == 2)
				{
					byte[] numArray2 = new byte[] { 167, 76, 30, 205, 241, 254, 68, 18 };
					byte[] numArray3 = new byte[] { 148, 121, 54, 39, 64, 20, 42, 219 };
					using (DESCryptoIndirector dESCryptoIndirector = new DESCryptoIndirector())
					{
						using (ICryptoTransform dESCryptoTransform = dESCryptoIndirector.GetDESCryptoTransform(numArray2, numArray3, true))
						{
							byte[] numArray4 = dESCryptoTransform.TransformFinalBlock(buffer, 4, (int)buffer.Length - 4);
							numArray = SimpleZip.Unzip(numArray4);
						}
					}
				}
				if (num2 == 3)
				{
					byte[] numArray5 = new byte[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
					byte[] numArray6 = new byte[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
					using (AESCryptoIndirector aESCryptoIndirector = new AESCryptoIndirector())
					{
						using (ICryptoTransform aESCryptoTransform = aESCryptoIndirector.GetAESCryptoTransform(numArray5, numArray6, true))
						{
							byte[] numArray7 = aESCryptoTransform.TransformFinalBlock(buffer, 4, (int)buffer.Length - 4);
							numArray = SimpleZip.Unzip(numArray7);
						}
					}
				}
			}
			else
			{
				short num5 = (short)zipStream.ReadShort();
				int num6 = zipStream.ReadShort();
				int num7 = zipStream.ReadShort();
				if (num1 != 67324752 || num5 != 20 || num6 != 0 || num7 != 8)
				{
					throw new FormatException("Wrong Header Signature");
				}
				zipStream.ReadInt();
				zipStream.ReadInt();
				zipStream.ReadInt();
				int num8 = zipStream.ReadInt();
				int num9 = zipStream.ReadShort();
				int num10 = zipStream.ReadShort();
				if (num9 > 0)
				{
					byte[] numArray8 = new byte[checked((uint)num9)];
					zipStream.Read(numArray8, 0, num9);
				}
				if (num10 > 0)
				{
					byte[] numArray9 = new byte[checked((uint)num10)];
					zipStream.Read(numArray9, 0, num10);
				}
				byte[] numArray10 = new byte[checked((uint)(zipStream.Length - zipStream.Position))];
				zipStream.Read(numArray10, 0, (int)numArray10.Length);
				SimpleZip.Inflater inflater = new SimpleZip.Inflater(numArray10);
				numArray = new byte[checked((uint)num8)];
				inflater.Inflate(numArray, 0, (int)numArray.Length);
				numArray10 = null;
			}
			zipStream.Close();
			zipStream = null;
			return numArray;
		}

		public static byte[] Zip(byte[] buffer)
		{
			return SimpleZip.Zip(buffer, 1, null, null);
		}

		private static byte[] Zip(byte[] buffer, int version, byte[] key, byte[] iv)
		{
			unsafe
			{
				byte[] numArray = null;
				byte[] array;
				try
				{
					SimpleZip.ZipStream zipStream = new SimpleZip.ZipStream();
					if (version == 0)
					{
						SimpleZip.Deflater deflater = new SimpleZip.Deflater();
						DateTime now = DateTime.Now;
						long year = (long)((now.Year - 1980 & 127) << 25 | now.Month << 21 | now.Day << 16 | now.Hour << 11 | now.Minute << 5 | now.Second >> 1);
						uint[] numArray1 = new uint[] { 0, 1996959894, 3993919788, 2567524794, 124634137, 1886057615, 3915621685, 2657392035, 249268274, 2044508324, 3772115230, 2547177864, 162941995, 2125561021, 3887607047, 2428444049, 498536548, 1789927666, 4089016648, 2227061214, 450548861, 1843258603, 4107580753, 2211677639, 325883990, 1684777152, 4251122042, 2321926636, 335633487, 1661365465, 4195302755, 2366115317, 997073096, 1281953886, 3579855332, 2724688242, 1006888145, 1258607687, 3524101629, 2768942443, 901097722, 1119000684, 3686517206, 2898065728, 853044451, 1172266101, 3705015759, 2882616665, 651767980, 1373503546, 3369554304, 3218104598, 565507253, 1454621731, 3485111705, 3099436303, 671266974, 1594198024, 3322730930, 2970347812, 795835527, 1483230225, 3244367275, 3060149565, 1994146192, 31158534, 2563907772, 4023717930, 1907459465, 112637215, 2680153253, 3904427059, 2013776290, 251722036, 2517215374, 3775830040, 2137656763, 141376813, 2439277719, 3865271297, 1802195444, 476864866, 2238001368, 4066508878, 1812370925, 453092731, 2181625025, 4111451223, 1706088902, 314042704, 2344532202, 4240017532, 1658658271, 366619977, 2362670323, 4224994405, 1303535960, 984961486, 2747007092, 3569037538, 1256170817, 1037604311, 2765210733, 3554079995, 1131014506, 879679996, 2909243462, 3663771856, 1141124467, 855842277, 2852801631, 3708648649, 1342533948, 654459306, 3188396048, 3373015174, 1466479909, 544179635, 3110523913, 3462522015, 1591671054, 702138776, 2966460450, 3352799412, 1504918807, 783551873, 3082640443, 3233442989, 3988292384, 2596254646, 62317068, 1957810842, 3939845945, 2647816111, 81470997, 1943803523, 3814918930, 2489596804, 225274430, 2053790376, 3826175755, 2466906013, 167816743, 2097651377, 4027552580, 2265490386, 503444072, 1762050814, 4150417245, 2154129355, 426522225, 1852507879, 4275313526, 2312317920, 282753626, 1742555852, 4189708143, 2394877945, 397917763, 1622183637, 3604390888, 2714866558, 953729732, 1340076626, 3518719985, 2797360999, 1068828381, 1219638859, 3624741850, 2936675148, 906185462, 1090812512, 3747672003, 2825379669, 829329135, 1181335161, 3412177804, 3160834842, 628085408, 1382605366, 3423369109, 3138078467, 570562233, 1426400815, 3317316542, 2998733608, 733239954, 1555261956, 3268935591, 3050360625, 752459403, 1541320221, 2607071920, 3965973030, 1969922972, 40735498, 2617837225, 3943577151, 1913087877, 83908371, 2512341634, 3803740692, 2075208622, 213261112, 2463272603, 3855990285, 2094854071, 198958881, 2262029012, 4057260610, 1759359992, 534414190, 2176718541, 4139329115, 1873836001, 414664567, 2282248934, 4279200368, 1711684554, 285281116, 2405801727, 4167216745, 1634467795, 376229701, 2685067896, 3608007406, 1308918612, 956543938, 2808555105, 3495958263, 1231636301, 1047427035, 2932959818, 3654703836, 1088359270, 936918000, 2847714899, 3736837829, 1202900863, 817233897, 3183342108, 3401237130, 1404277552, 615818150, 3134207493, 3453421203, 1423857449, 601450431, 3009837614, 3294710456, 1567103746, 711928724, 3020668471, 3272380065, 1510334235, 755167117 };
						uint num = -1;
						uint num1 = num;
						int num2 = 0;
						int length = (int)buffer.Length;
						while (true)
						{
							int num3 = length - 1;
							length = num3;
							if (num3 < 0)
							{
								break;
							}
							int num4 = num2;
							num2 = num4 + 1;
							num1 = numArray1[(num1 ^ buffer[num4]) & 255] ^ num1 >> 8;
						}
						num1 = num1 ^ num;
						zipStream.WriteInt(67324752);
						zipStream.WriteShort(20);
						zipStream.WriteShort(0);
						zipStream.WriteShort(8);
						zipStream.WriteInt((int)year);
						zipStream.WriteInt((int)num1);
						long position = zipStream.Position;
						zipStream.WriteInt(0);
						zipStream.WriteInt((int)buffer.Length);
						byte[] bytes = Encoding.UTF8.GetBytes("{data}");
						zipStream.WriteShort((int)bytes.Length);
						zipStream.WriteShort(0);
						zipStream.Write(bytes, 0, (int)bytes.Length);
						deflater.SetInput(buffer);
						while (!deflater.IsNeedingInput)
						{
							byte[] numArray2 = new byte[512];
							int num5 = deflater.Deflate(numArray2);
							if (num5 <= 0)
							{
								break;
							}
							zipStream.Write(numArray2, 0, num5);
						}
						deflater.Finish();
						while (!deflater.IsFinished)
						{
							byte[] numArray3 = new byte[512];
							int num6 = deflater.Deflate(numArray3);
							if (num6 <= 0)
							{
								break;
							}
							zipStream.Write(numArray3, 0, num6);
						}
						long totalOut = deflater.TotalOut;
						zipStream.WriteInt(33639248);
						zipStream.WriteShort(20);
						zipStream.WriteShort(20);
						zipStream.WriteShort(0);
						zipStream.WriteShort(8);
						zipStream.WriteInt((int)year);
						zipStream.WriteInt((int)num1);
						zipStream.WriteInt((int)totalOut);
						zipStream.WriteInt((int)buffer.Length);
						zipStream.WriteShort((int)bytes.Length);
						zipStream.WriteShort(0);
						zipStream.WriteShort(0);
						zipStream.WriteShort(0);
						zipStream.WriteShort(0);
						zipStream.WriteInt(0);
						zipStream.WriteInt(0);
						zipStream.Write(bytes, 0, (int)bytes.Length);
						zipStream.WriteInt(101010256);
						zipStream.WriteShort(0);
						zipStream.WriteShort(0);
						zipStream.WriteShort(1);
						zipStream.WriteShort(1);
						zipStream.WriteInt(46 + (int)bytes.Length);
						zipStream.WriteInt((int)((long)(30 + (int)bytes.Length) + totalOut));
						zipStream.WriteShort(0);
						zipStream.Seek(position, SeekOrigin.Begin);
						zipStream.WriteInt((int)totalOut);
					}
					else if (version == 1)
					{
						zipStream.WriteInt(25000571);
						zipStream.WriteInt((int)buffer.Length);
						for (int i = 0; i < (int)buffer.Length; i = i + (int)numArray.Length)
						{
							numArray = new byte[checked((uint)Math.Min(2097151, (int)buffer.Length - i))];
							Buffer.BlockCopy(buffer, i, numArray, 0, (int)numArray.Length);
							long position1 = zipStream.Position;
							zipStream.WriteInt(0);
							zipStream.WriteInt((int)numArray.Length);
							SimpleZip.Deflater deflater1 = new SimpleZip.Deflater();
							deflater1.SetInput(numArray);
							while (!deflater1.IsNeedingInput)
							{
								byte[] numArray4 = new byte[512];
								int num7 = deflater1.Deflate(numArray4);
								if (num7 <= 0)
								{
									break;
								}
								zipStream.Write(numArray4, 0, num7);
							}
							deflater1.Finish();
							while (!deflater1.IsFinished)
							{
								byte[] numArray5 = new byte[512];
								int num8 = deflater1.Deflate(numArray5);
								if (num8 <= 0)
								{
									break;
								}
								zipStream.Write(numArray5, 0, num8);
							}
							long position2 = zipStream.Position;
							zipStream.Position = position1;
							zipStream.WriteInt((int)deflater1.TotalOut);
							zipStream.Position = position2;
						}
					}
					else if (version == 2)
					{
						zipStream.WriteInt(41777787);
						byte[] numArray6 = SimpleZip.Zip(buffer, 1, null, null);
						using (DESCryptoIndirector dESCryptoIndirector = new DESCryptoIndirector())
						{
							using (ICryptoTransform dESCryptoTransform = dESCryptoIndirector.GetDESCryptoTransform(key, iv, false))
							{
								byte[] numArray7 = dESCryptoTransform.TransformFinalBlock(numArray6, 0, (int)numArray6.Length);
								zipStream.Write(numArray7, 0, (int)numArray7.Length);
							}
						}
					}
					else if (version == 3)
					{
						zipStream.WriteInt(58555003);
						byte[] numArray8 = SimpleZip.Zip(buffer, 1, null, null);
						using (AESCryptoIndirector aESCryptoIndirector = new AESCryptoIndirector())
						{
							using (ICryptoTransform aESCryptoTransform = aESCryptoIndirector.GetAESCryptoTransform(key, iv, false))
							{
								byte[] numArray9 = aESCryptoTransform.TransformFinalBlock(numArray8, 0, (int)numArray8.Length);
								zipStream.Write(numArray9, 0, (int)numArray9.Length);
							}
						}
					}
					zipStream.Flush();
					zipStream.Close();
					array = zipStream.ToArray();
				}
				catch (Exception exception)
				{
					SimpleZip.ExceptionMessage = string.Concat("ERR 2003: ", exception.Message);
					throw;
				}
				return array;
			}
		}

		public static byte[] ZipAndAES(byte[] buffer, byte[] key, byte[] iv)
		{
			return SimpleZip.Zip(buffer, 3, key, iv);
		}

		public static byte[] ZipAndEncrypt(byte[] buffer, byte[] key, byte[] iv)
		{
			return SimpleZip.Zip(buffer, 2, key, iv);
		}

		internal sealed class Deflater
		{
			private const int IS_FLUSHING = 4;

			private const int IS_FINISHING = 8;

			private const int BUSY_STATE = 16;

			private const int FLUSHING_STATE = 20;

			private const int FINISHING_STATE = 28;

			private const int FINISHED_STATE = 30;

			private int state;

			private long totalOut;

			private SimpleZip.DeflaterPending pending;

			private SimpleZip.DeflaterEngine engine;

			public bool IsFinished
			{
				get
				{
					if (this.state != 30)
					{
						return false;
					}
					return this.pending.IsFlushed;
				}
			}

			public bool IsNeedingInput
			{
				get
				{
					return this.engine.NeedsInput();
				}
			}

			public long TotalOut
			{
				get
				{
					return this.totalOut;
				}
			}

			public Deflater()
			{
				this.pending = new SimpleZip.DeflaterPending();
				this.engine = new SimpleZip.DeflaterEngine(this.pending);
			}

			public int Deflate(byte[] output)
			{
				int num = 0;
				int length = (int)output.Length;
				int num1 = length;
				while (true)
				{
					int num2 = this.pending.Flush(output, num, length);
					num = num + num2;
					SimpleZip.Deflater deflater = this;
					deflater.totalOut = deflater.totalOut + (long)num2;
					length = length - num2;
					if (length == 0 || this.state == 30)
					{
						break;
					}
					if (!this.engine.Deflate((this.state & 4) != 0, (this.state & 8) != 0))
					{
						if (this.state == 16)
						{
							return num1 - length;
						}
						if (this.state == 20)
						{
							for (int i = 8 + (-this.pending.BitCount & 7); i > 0; i = i - 10)
							{
								this.pending.WriteBits(2, 10);
							}
							this.state = 16;
						}
						else if (this.state == 28)
						{
							this.pending.AlignToByte();
							this.state = 30;
						}
					}
				}
				return num1 - length;
			}

			public void Finish()
			{
				SimpleZip.Deflater deflater = this;
				deflater.state = deflater.state | 12;
			}

			public void SetInput(byte[] buffer)
			{
				this.engine.SetInput(buffer);
			}
		}

		internal sealed class DeflaterEngine
		{
			private const int MAX_MATCH = 258;

			private const int MIN_MATCH = 3;

			private const int WSIZE = 32768;

			private const int WMASK = 32767;

			private const int HASH_SIZE = 32768;

			private const int HASH_MASK = 32767;

			private const int HASH_SHIFT = 5;

			private const int MIN_LOOKAHEAD = 262;

			private const int MAX_DIST = 32506;

			private const int TOO_FAR = 4096;

			private int ins_h;

			private short[] head;

			private short[] prev;

			private int matchStart;

			private int matchLen;

			private bool prevAvailable;

			private int blockStart;

			private int strstart;

			private int lookahead;

			private byte[] window;

			private byte[] inputBuf;

			private int totalIn;

			private int inputOff;

			private int inputEnd;

			private SimpleZip.DeflaterPending pending;

			private SimpleZip.DeflaterHuffman huffman;

			public DeflaterEngine(SimpleZip.DeflaterPending pending)
			{
				this.pending = pending;
				this.huffman = new SimpleZip.DeflaterHuffman(pending);
				this.window = new byte[65536];
				this.head = new short[32768];
				this.prev = new short[32768];
				int num = 1;
				int num1 = num;
				this.strstart = num;
				this.blockStart = num1;
			}

			public bool Deflate(bool flush, bool finish)
			{
				bool flag;
				do
				{
					this.FillWindow();
					flag = this.DeflateSlow((!flush ? false : this.inputOff == this.inputEnd), finish);
				}
				while (this.pending.IsFlushed && flag);
				return flag;
			}

			private bool DeflateSlow(bool flush, bool finish)
			{
				int num;
				if (this.lookahead < 262 && !flush)
				{
					return false;
				}
				while (this.lookahead >= 262 || flush)
				{
					if (this.lookahead == 0)
					{
						if (this.prevAvailable)
						{
							this.huffman.TallyLit(this.window[this.strstart - 1] & 255);
						}
						this.prevAvailable = false;
						this.huffman.FlushBlock(this.window, this.blockStart, this.strstart - this.blockStart, finish);
						this.blockStart = this.strstart;
						return false;
					}
					if (this.strstart >= 65274)
					{
						this.SlideWindow();
					}
					int num1 = this.matchStart;
					int num2 = this.matchLen;
					if (this.lookahead >= 3)
					{
						int num3 = this.InsertString();
						if (num3 != 0 && this.strstart - num3 <= 32506 && this.FindLongestMatch(num3) && this.matchLen <= 5 && this.matchLen == 3 && this.strstart - this.matchStart > 4096)
						{
							this.matchLen = 2;
						}
					}
					if (num2 < 3 || this.matchLen > num2)
					{
						if (this.prevAvailable)
						{
							this.huffman.TallyLit(this.window[this.strstart - 1] & 255);
						}
						this.prevAvailable = true;
						SimpleZip.DeflaterEngine deflaterEngine = this;
						deflaterEngine.strstart = deflaterEngine.strstart + 1;
						SimpleZip.DeflaterEngine deflaterEngine1 = this;
						deflaterEngine1.lookahead = deflaterEngine1.lookahead - 1;
					}
					else
					{
						this.huffman.TallyDist(this.strstart - 1 - num1, num2);
						num2 = num2 - 2;
						do
						{
							SimpleZip.DeflaterEngine deflaterEngine2 = this;
							deflaterEngine2.strstart = deflaterEngine2.strstart + 1;
							SimpleZip.DeflaterEngine deflaterEngine3 = this;
							deflaterEngine3.lookahead = deflaterEngine3.lookahead - 1;
							if (this.lookahead >= 3)
							{
								this.InsertString();
							}
							num = num2 - 1;
							num2 = num;
						}
						while (num > 0);
						SimpleZip.DeflaterEngine deflaterEngine4 = this;
						deflaterEngine4.strstart = deflaterEngine4.strstart + 1;
						SimpleZip.DeflaterEngine deflaterEngine5 = this;
						deflaterEngine5.lookahead = deflaterEngine5.lookahead - 1;
						this.prevAvailable = false;
						this.matchLen = 2;
					}
					if (!this.huffman.IsFull())
					{
						continue;
					}
					int num4 = this.strstart - this.blockStart;
					if (this.prevAvailable)
					{
						num4--;
					}
					bool flag = (!finish || this.lookahead != 0 ? false : !this.prevAvailable);
					this.huffman.FlushBlock(this.window, this.blockStart, num4, flag);
					SimpleZip.DeflaterEngine deflaterEngine6 = this;
					deflaterEngine6.blockStart = deflaterEngine6.blockStart + num4;
					return !flag;
				}
				return true;
			}

			public void FillWindow()
			{
				if (this.strstart >= 65274)
				{
					this.SlideWindow();
				}
				while (this.lookahead < 262 && this.inputOff < this.inputEnd)
				{
					int num = 65536 - this.lookahead - this.strstart;
					if (num > this.inputEnd - this.inputOff)
					{
						num = this.inputEnd - this.inputOff;
					}
					Array.Copy(this.inputBuf, this.inputOff, this.window, this.strstart + this.lookahead, num);
					SimpleZip.DeflaterEngine deflaterEngine = this;
					deflaterEngine.inputOff = deflaterEngine.inputOff + num;
					SimpleZip.DeflaterEngine deflaterEngine1 = this;
					deflaterEngine1.totalIn = deflaterEngine1.totalIn + num;
					SimpleZip.DeflaterEngine deflaterEngine2 = this;
					deflaterEngine2.lookahead = deflaterEngine2.lookahead + num;
				}
				if (this.lookahead >= 3)
				{
					this.UpdateHash();
				}
			}

			private bool FindLongestMatch(int curMatch)
			{
				int num;
				int num1;
				int num2;
				int num3 = 128;
				int num4 = 128;
				short[] numArray = this.prev;
				int num5 = this.strstart;
				int num6 = this.strstart + this.matchLen;
				int num7 = Math.Max(this.matchLen, 2);
				int num8 = Math.Max(this.strstart - 32506, 0);
				int num9 = this.strstart + 258 - 1;
				byte num10 = this.window[num6 - 1];
				byte num11 = this.window[num6];
				if (num7 >= 8)
				{
					num3 = num3 >> 2;
				}
				if (num4 > this.lookahead)
				{
					num4 = this.lookahead;
				}
				do
				{
					if (this.window[curMatch + num7] == num11 && this.window[curMatch + num7 - 1] == num10 && this.window[curMatch] == this.window[num5] && this.window[curMatch + 1] == this.window[num5 + 1])
					{
						int num12 = curMatch + 2;
						num5 = num5 + 2;
						do
						{
							int num13 = num5 + 1;
							num5 = num13;
							int num14 = num12 + 1;
							num12 = num14;
							if (this.window[num13] != this.window[num14])
							{
								break;
							}
							int num15 = num5 + 1;
							num5 = num15;
							int num16 = num12 + 1;
							num12 = num16;
							if (this.window[num15] != this.window[num16])
							{
								break;
							}
							int num17 = num5 + 1;
							num5 = num17;
							int num18 = num12 + 1;
							num12 = num18;
							if (this.window[num17] != this.window[num18])
							{
								break;
							}
							int num19 = num5 + 1;
							num5 = num19;
							int num20 = num12 + 1;
							num12 = num20;
							if (this.window[num19] != this.window[num20])
							{
								break;
							}
							int num21 = num5 + 1;
							num5 = num21;
							int num22 = num12 + 1;
							num12 = num22;
							if (this.window[num21] != this.window[num22])
							{
								break;
							}
							int num23 = num5 + 1;
							num5 = num23;
							int num24 = num12 + 1;
							num12 = num24;
							if (this.window[num23] != this.window[num24])
							{
								break;
							}
							int num25 = num5 + 1;
							num5 = num25;
							int num26 = num12 + 1;
							num12 = num26;
							if (this.window[num25] != this.window[num26])
							{
								break;
							}
							num1 = num5 + 1;
							num5 = num1;
							num2 = num12 + 1;
							num12 = num2;
						}
						while (this.window[num1] == this.window[num2] && num5 < num9);
						if (num5 > num6)
						{
							this.matchStart = curMatch;
							num6 = num5;
							num7 = num5 - this.strstart;
							if (num7 >= num4)
							{
								break;
							}
							num10 = this.window[num6 - 1];
							num11 = this.window[num6];
						}
						num5 = this.strstart;
					}
					int num27 = numArray[curMatch & 32767] & 65535;
					curMatch = num27;
					if (num27 <= num8)
					{
						break;
					}
					num = num3 - 1;
					num3 = num;
				}
				while (num != 0);
				this.matchLen = Math.Min(num7, this.lookahead);
				return this.matchLen >= 3;
			}

			private int InsertString()
			{
				int insH = (this.ins_h << 5 ^ this.window[this.strstart + 2]) & 32767;
				short[] numArray = this.prev;
				short num = this.head[insH];
				short num1 = num;
				numArray[this.strstart & 32767] = num;
				this.head[insH] = (short)this.strstart;
				this.ins_h = insH;
				return num1 & 65535;
			}

			public bool NeedsInput()
			{
				return this.inputEnd == this.inputOff;
			}

			public void SetInput(byte[] buffer)
			{
				this.inputBuf = buffer;
				this.inputOff = 0;
				this.inputEnd = (int)buffer.Length;
			}

			private void SlideWindow()
			{
				object obj;
				object obj1;
				Array.Copy(this.window, 32768, this.window, 0, 32768);
				SimpleZip.DeflaterEngine deflaterEngine = this;
				deflaterEngine.matchStart = deflaterEngine.matchStart - 32768;
				SimpleZip.DeflaterEngine deflaterEngine1 = this;
				deflaterEngine1.strstart = deflaterEngine1.strstart - 32768;
				SimpleZip.DeflaterEngine deflaterEngine2 = this;
				deflaterEngine2.blockStart = deflaterEngine2.blockStart - 32768;
				for (int i = 0; i < 32768; i++)
				{
					int num = this.head[i] & 65535;
					short[] numArray = this.head;
					int num1 = i;
					if (num >= 32768)
					{
						obj = num - 32768;
					}
					else
					{
						obj = null;
					}
					numArray[num1] = (short)obj;
				}
				for (int j = 0; j < 32768; j++)
				{
					int num2 = this.prev[j] & 65535;
					short[] numArray1 = this.prev;
					int num3 = j;
					if (num2 >= 32768)
					{
						obj1 = num2 - 32768;
					}
					else
					{
						obj1 = null;
					}
					numArray1[num3] = (short)obj1;
				}
			}

			private void UpdateHash()
			{
				this.ins_h = this.window[this.strstart] << 5 ^ this.window[this.strstart + 1];
			}
		}

		internal sealed class DeflaterHuffman
		{
			private const int BUFSIZE = 16384;

			private const int LITERAL_NUM = 286;

			private const int DIST_NUM = 30;

			private const int BITLEN_NUM = 19;

			private const int REP_3_6 = 16;

			private const int REP_3_10 = 17;

			private const int REP_11_138 = 18;

			private const int EOF_SYMBOL = 256;

			private readonly static int[] BL_ORDER;

			private readonly static byte[] bit4Reverse;

			private SimpleZip.DeflaterPending pending;

			private SimpleZip.DeflaterHuffman.Tree literalTree;

			private SimpleZip.DeflaterHuffman.Tree distTree;

			private SimpleZip.DeflaterHuffman.Tree blTree;

			private short[] d_buf;

			private byte[] l_buf;

			private int last_lit;

			private int extra_bits;

			private readonly static short[] staticLCodes;

			private readonly static byte[] staticLLength;

			private readonly static short[] staticDCodes;

			private readonly static byte[] staticDLength;

			static DeflaterHuffman()
			{
				SimpleZip.DeflaterHuffman.BL_ORDER = new int[] { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };
				SimpleZip.DeflaterHuffman.bit4Reverse = new byte[] { 0, 8, 4, 12, 2, 10, 6, 14, 1, 9, 5, 13, 3, 11, 7, 15 };
				SimpleZip.DeflaterHuffman.staticLCodes = new short[286];
				SimpleZip.DeflaterHuffman.staticLLength = new byte[286];
				int i = 0;
				while (i < 144)
				{
					SimpleZip.DeflaterHuffman.staticLCodes[i] = SimpleZip.DeflaterHuffman.BitReverse(48 + i << 8);
					int num = i;
					i = num + 1;
					SimpleZip.DeflaterHuffman.staticLLength[num] = 8;
				}
				while (i < 256)
				{
					SimpleZip.DeflaterHuffman.staticLCodes[i] = SimpleZip.DeflaterHuffman.BitReverse(256 + i << 7);
					int num1 = i;
					i = num1 + 1;
					SimpleZip.DeflaterHuffman.staticLLength[num1] = 9;
				}
				while (i < 280)
				{
					SimpleZip.DeflaterHuffman.staticLCodes[i] = SimpleZip.DeflaterHuffman.BitReverse(-256 + i << 9);
					int num2 = i;
					i = num2 + 1;
					SimpleZip.DeflaterHuffman.staticLLength[num2] = 7;
				}
				while (i < 286)
				{
					SimpleZip.DeflaterHuffman.staticLCodes[i] = SimpleZip.DeflaterHuffman.BitReverse(-88 + i << 8);
					int num3 = i;
					i = num3 + 1;
					SimpleZip.DeflaterHuffman.staticLLength[num3] = 8;
				}
				SimpleZip.DeflaterHuffman.staticDCodes = new short[30];
				SimpleZip.DeflaterHuffman.staticDLength = new byte[30];
				for (i = 0; i < 30; i++)
				{
					SimpleZip.DeflaterHuffman.staticDCodes[i] = SimpleZip.DeflaterHuffman.BitReverse(i << 11);
					SimpleZip.DeflaterHuffman.staticDLength[i] = 5;
				}
			}

			public DeflaterHuffman(SimpleZip.DeflaterPending pending)
			{
				this.pending = pending;
				this.literalTree = new SimpleZip.DeflaterHuffman.Tree(this, 286, 257, 15);
				this.distTree = new SimpleZip.DeflaterHuffman.Tree(this, 30, 1, 15);
				this.blTree = new SimpleZip.DeflaterHuffman.Tree(this, 19, 4, 7);
				this.d_buf = new short[16384];
				this.l_buf = new byte[16384];
			}

			public static short BitReverse(int toReverse)
			{
				return (short)(SimpleZip.DeflaterHuffman.bit4Reverse[toReverse & 15] << 12 | SimpleZip.DeflaterHuffman.bit4Reverse[toReverse >> 4 & 15] << 8 | SimpleZip.DeflaterHuffman.bit4Reverse[toReverse >> 8 & 15] << 4 | SimpleZip.DeflaterHuffman.bit4Reverse[toReverse >> 12]);
			}

			public void CompressBlock()
			{
				for (int i = 0; i < this.last_lit; i++)
				{
					int lBuf = this.l_buf[i] & 255;
					int dBuf = this.d_buf[i];
					int num = dBuf;
					dBuf = num - 1;
					if (num == 0)
					{
						this.literalTree.WriteSymbol(lBuf);
					}
					else
					{
						int num1 = this.Lcode(lBuf);
						this.literalTree.WriteSymbol(num1);
						int num2 = (num1 - 261) / 4;
						if (num2 > 0 && num2 <= 5)
						{
							this.pending.WriteBits(lBuf & (1 << (num2 & 31)) - 1, num2);
						}
						int num3 = this.Dcode(dBuf);
						this.distTree.WriteSymbol(num3);
						num2 = num3 / 2 - 1;
						if (num2 > 0)
						{
							this.pending.WriteBits(dBuf & (1 << (num2 & 31)) - 1, num2);
						}
					}
				}
				this.literalTree.WriteSymbol(256);
			}

			private int Dcode(int distance)
			{
				int num = 0;
				while (distance >= 4)
				{
					num = num + 2;
					distance = distance >> 1;
				}
				return num + distance;
			}

			public void FlushBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
			{
				short[] numArray = this.literalTree.freqs;
				numArray[256] = (short)(numArray[256] + 1);
				this.literalTree.BuildTree();
				this.distTree.BuildTree();
				this.literalTree.CalcBLFreq(this.blTree);
				this.distTree.CalcBLFreq(this.blTree);
				this.blTree.BuildTree();
				int num = 4;
				for (int i = 18; i > num; i--)
				{
					if (this.blTree.length[SimpleZip.DeflaterHuffman.BL_ORDER[i]] > 0)
					{
						num = i + 1;
					}
				}
				int encodedLength = 14 + num * 3 + this.blTree.GetEncodedLength() + this.literalTree.GetEncodedLength() + this.distTree.GetEncodedLength() + this.extra_bits;
				int extraBits = this.extra_bits;
				for (int j = 0; j < 286; j++)
				{
					extraBits = extraBits + this.literalTree.freqs[j] * SimpleZip.DeflaterHuffman.staticLLength[j];
				}
				for (int k = 0; k < 30; k++)
				{
					extraBits = extraBits + this.distTree.freqs[k] * SimpleZip.DeflaterHuffman.staticDLength[k];
				}
				if (encodedLength >= extraBits)
				{
					encodedLength = extraBits;
				}
				if (storedOffset >= 0 && storedLength + 4 < encodedLength >> 3)
				{
					this.FlushStoredBlock(stored, storedOffset, storedLength, lastBlock);
					return;
				}
				if (encodedLength != extraBits)
				{
					this.pending.WriteBits(4 + (lastBlock ? 1 : 0), 3);
					this.SendAllTrees(num);
					this.CompressBlock();
					this.Init();
					return;
				}
				this.pending.WriteBits(2 + (lastBlock ? 1 : 0), 3);
				this.literalTree.SetStaticCodes(SimpleZip.DeflaterHuffman.staticLCodes, SimpleZip.DeflaterHuffman.staticLLength);
				this.distTree.SetStaticCodes(SimpleZip.DeflaterHuffman.staticDCodes, SimpleZip.DeflaterHuffman.staticDLength);
				this.CompressBlock();
				this.Init();
			}

			public void FlushStoredBlock(byte[] stored, int storedOffset, int storedLength, bool lastBlock)
			{
				this.pending.WriteBits((lastBlock ? 1 : 0), 3);
				this.pending.AlignToByte();
				this.pending.WriteShort(storedLength);
				this.pending.WriteShort(~storedLength);
				this.pending.WriteBlock(stored, storedOffset, storedLength);
				this.Init();
			}

			public void Init()
			{
				this.last_lit = 0;
				this.extra_bits = 0;
			}

			public bool IsFull()
			{
				return this.last_lit >= 16384;
			}

			private int Lcode(int len)
			{
				if (len == 255)
				{
					return 285;
				}
				int num = 257;
				while (len >= 8)
				{
					num = num + 4;
					len = len >> 1;
				}
				return num + len;
			}

			public void SendAllTrees(int blTreeCodes)
			{
				this.blTree.BuildCodes();
				this.literalTree.BuildCodes();
				this.distTree.BuildCodes();
				this.pending.WriteBits(this.literalTree.numCodes - 257, 5);
				this.pending.WriteBits(this.distTree.numCodes - 1, 5);
				this.pending.WriteBits(blTreeCodes - 4, 4);
				for (int i = 0; i < blTreeCodes; i++)
				{
					this.pending.WriteBits((int)this.blTree.length[SimpleZip.DeflaterHuffman.BL_ORDER[i]], 3);
				}
				this.literalTree.WriteTree(this.blTree);
				this.distTree.WriteTree(this.blTree);
			}

			public bool TallyDist(int dist, int len)
			{
				this.d_buf[this.last_lit] = (short)dist;
				byte[] lBuf = this.l_buf;
				SimpleZip.DeflaterHuffman deflaterHuffman = this;
				int lastLit = deflaterHuffman.last_lit;
				int num = lastLit;
				deflaterHuffman.last_lit = lastLit + 1;
				lBuf[num] = (byte)(len - 3);
				int num1 = this.Lcode(len - 3);
				short[] numArray = this.literalTree.freqs;
				short[] numArray1 = numArray;
				int num2 = num1;
				IntPtr intPtr = (IntPtr)num2;
				numArray[num2] = (short)(numArray1[intPtr] + 1);
				if (num1 >= 265 && num1 < 285)
				{
					SimpleZip.DeflaterHuffman extraBits = this;
					extraBits.extra_bits = extraBits.extra_bits + (num1 - 261) / 4;
				}
				int num3 = this.Dcode(dist - 1);
				short[] numArray2 = this.distTree.freqs;
				numArray1 = numArray2;
				int num4 = num3;
				intPtr = (IntPtr)num4;
				numArray2[num4] = (short)(numArray1[intPtr] + 1);
				if (num3 >= 4)
				{
					SimpleZip.DeflaterHuffman extraBits1 = this;
					extraBits1.extra_bits = extraBits1.extra_bits + (num3 / 2 - 1);
				}
				return this.IsFull();
			}

			public bool TallyLit(int lit)
			{
				this.d_buf[this.last_lit] = 0;
				byte[] lBuf = this.l_buf;
				SimpleZip.DeflaterHuffman deflaterHuffman = this;
				int lastLit = deflaterHuffman.last_lit;
				int num = lastLit;
				deflaterHuffman.last_lit = lastLit + 1;
				lBuf[num] = (byte)lit;
				short[] numArray = this.literalTree.freqs;
				short[] numArray1 = numArray;
				int num1 = lit;
				IntPtr intPtr = (IntPtr)num1;
				numArray[num1] = (short)(numArray1[intPtr] + 1);
				return this.IsFull();
			}

			public sealed class Tree
			{
				public short[] freqs;

				public byte[] length;

				public int minNumCodes;

				public int numCodes;

				private short[] codes;

				private int[] bl_counts;

				private int maxLength;

				private SimpleZip.DeflaterHuffman dh;

				public Tree(SimpleZip.DeflaterHuffman dh, int elems, int minCodes, int maxLength)
				{
					this.dh = dh;
					this.minNumCodes = minCodes;
					this.maxLength = maxLength;
					this.freqs = new short[checked((uint)elems)];
					this.bl_counts = new int[checked((uint)maxLength)];
				}

				public void BuildCodes()
				{
					int length = (int)this.freqs.Length;
					int[] numArray = new int[checked((uint)this.maxLength)];
					int blCounts = 0;
					this.codes = new short[checked((uint)((int)this.freqs.Length))];
					for (int i = 0; i < this.maxLength; i++)
					{
						numArray[i] = blCounts;
						blCounts = blCounts + (this.bl_counts[i] << (15 - i & 31));
					}
					for (int j = 0; j < this.numCodes; j++)
					{
						int num = this.length[j];
						if (num > 0)
						{
							this.codes[j] = SimpleZip.DeflaterHuffman.BitReverse(numArray[num - 1]);
							int[] numArray1 = numArray;
							int[] numArray2 = numArray1;
							int num1 = num - 1;
							IntPtr intPtr = (IntPtr)num1;
							numArray1[num1] = numArray2[intPtr] + (1 << (16 - num & 31));
						}
					}
				}

				private void BuildLength(int[] childs)
				{
					int[] numArray;
					this.length = new byte[checked((uint)((int)this.freqs.Length))];
					int length = (int)childs.Length / 2;
					int num = (length + 1) / 2;
					int num1 = 0;
					for (int i = 0; i < this.maxLength; i++)
					{
						this.bl_counts[i] = 0;
					}
					int[] numArray1 = new int[checked((uint)length)];
					numArray1[length - 1] = 0;
					for (int j = length - 1; j >= 0; j--)
					{
						if (childs[2 * j + 1] == -1)
						{
							int num2 = numArray1[j];
							int[] blCounts = this.bl_counts;
							numArray = blCounts;
							int num3 = num2 - 1;
							blCounts[num3] = numArray[(IntPtr)num3] + 1;
							this.length[childs[2 * j]] = (byte)numArray1[j];
						}
						else
						{
							int num4 = numArray1[j] + 1;
							if (num4 > this.maxLength)
							{
								num4 = this.maxLength;
								num1++;
							}
							int num5 = num4;
							int num6 = num5;
							numArray1[childs[2 * j + 1]] = num5;
							numArray1[childs[2 * j]] = num6;
						}
					}
					if (num1 == 0)
					{
						return;
					}
					int num7 = this.maxLength - 1;
					do
					{
					Label0:
						int num8 = num7 - 1;
						num7 = num8;
						if (this.bl_counts[num8] != 0)
						{
							do
							{
								int[] blCounts1 = this.bl_counts;
								numArray = blCounts1;
								int num9 = num7;
								blCounts1[num9] = numArray[(IntPtr)num9] - 1;
								int[] blCounts2 = this.bl_counts;
								numArray = blCounts2;
								int num10 = num7 + 1;
								num7 = num10;
								blCounts2[num10] = numArray[(IntPtr)num10] + 1;
								num1 = num1 - (1 << (this.maxLength - 1 - num7 & 31));
							}
							while (num1 > 0 && num7 < this.maxLength - 1);
						}
						else
						{
							goto Label0;
						}
					}
					while (num1 > 0);
					int[] numArray2 = this.bl_counts;
					numArray = numArray2;
					int num11 = this.maxLength - 1;
					numArray2[num11] = numArray[(IntPtr)num11] + num1;
					int[] blCounts3 = this.bl_counts;
					numArray = blCounts3;
					int num12 = this.maxLength - 2;
					blCounts3[num12] = numArray[(IntPtr)num12] - num1;
					int num13 = 2 * num;
					for (int k = this.maxLength; k != 0; k--)
					{
						int blCounts4 = this.bl_counts[k - 1];
						while (blCounts4 > 0)
						{
							int num14 = num13;
							num13 = num14 + 1;
							int num15 = 2 * childs[num14];
							if (childs[num15 + 1] != -1)
							{
								continue;
							}
							this.length[childs[num15]] = (byte)k;
							blCounts4--;
						}
					}
				}

				public void BuildTree()
				{
					int j;
					int num = 0;
					int l;
					int num1;
					int length = (int)this.freqs.Length;
					int[] numArray = new int[checked((uint)length)];
					int num2 = 0;
					int num3 = 0;
					for (int i = 0; i < length; i++)
					{
						int num4 = this.freqs[i];
						if (num4 != 0)
						{
							int num5 = num2;
							num2 = num5 + 1;
							for (j = num5; j > 0; j = num)
							{
								short[] numArray1 = this.freqs;
								int num6 = (j - 1) / 2;
								num = num6;
								if (numArray1[numArray[num6]] <= num4)
								{
									break;
								}
								numArray[j] = numArray[num];
							}
							numArray[j] = i;
							num3 = i;
						}
					}
					while (num2 < 2)
					{
						if (num3 < 2)
						{
							num1 = num3 + 1;
							num3 = num1;
						}
						else
						{
							num1 = 0;
						}
						int num7 = num1;
						int num8 = num2;
						num2 = num8 + 1;
						numArray[num8] = num7;
					}
					this.numCodes = Math.Max(num3 + 1, this.minNumCodes);
					int num9 = num2;
					int[] numArray2 = new int[checked((uint)(4 * num2 - 2))];
					int[] numArray3 = new int[checked((uint)(2 * num2 - 1))];
					int num10 = num9;
					for (int k = 0; k < num2; k++)
					{
						int num11 = numArray[k];
						numArray2[2 * k] = num11;
						numArray2[2 * k + 1] = -1;
						numArray3[k] = this.freqs[num11] << 8;
						numArray[k] = k;
					}
					do
					{
						int num12 = numArray[0];
						int num13 = num2 - 1;
						num2 = num13;
						int num14 = numArray[num13];
						int num15 = 0;
						for (l = 1; l < num2; l = l * 2 + 1)
						{
							if (l + 1 < num2 && numArray3[numArray[l]] > numArray3[numArray[l + 1]])
							{
								l++;
							}
							numArray[num15] = numArray[l];
							num15 = l;
						}
						int num16 = numArray3[num14];
						while (true)
						{
							int num17 = num15;
							l = num17;
							if (num17 <= 0)
							{
								break;
							}
							int num18 = (l - 1) / 2;
							num15 = num18;
							if (numArray3[numArray[num18]] <= num16)
							{
								break;
							}
							numArray[l] = numArray[num15];
						}
						numArray[l] = num14;
						int num19 = numArray[0];
						int num20 = num10;
						num10 = num20 + 1;
						num14 = num20;
						numArray2[2 * num14] = num12;
						numArray2[2 * num14 + 1] = num19;
						int num21 = Math.Min(numArray3[num12] & 255, numArray3[num19] & 255);
						int num22 = numArray3[num12] + numArray3[num19] - num21 + 1;
						num16 = num22;
						numArray3[num14] = num22;
						num15 = 0;
						for (l = 1; l < num2; l = num15 * 2 + 1)
						{
							if (l + 1 < num2 && numArray3[numArray[l]] > numArray3[numArray[l + 1]])
							{
								l++;
							}
							numArray[num15] = numArray[l];
							num15 = l;
						}
						while (true)
						{
							int num23 = num15;
							l = num23;
							if (num23 <= 0)
							{
								break;
							}
							int num24 = (l - 1) / 2;
							num15 = num24;
							if (numArray3[numArray[num24]] <= num16)
							{
								break;
							}
							numArray[l] = numArray[num15];
						}
						numArray[l] = num14;
					}
					while (num2 > 1);
					this.BuildLength(numArray2);
				}

				public void CalcBLFreq(SimpleZip.DeflaterHuffman.Tree blTree)
				{
					int num;
					int num1;
					short[] numArray;
					IntPtr intPtr;
					int num2;
					int num3 = -1;
					int num4 = 0;
					while (num4 < this.numCodes)
					{
						int num5 = 1;
						int num6 = this.length[num4];
						if (num6 != 0)
						{
							num = 6;
							num1 = 3;
							if (num3 != num6)
							{
								short[] numArray1 = blTree.freqs;
								numArray = numArray1;
								int num7 = num6;
								intPtr = (IntPtr)num7;
								numArray1[num7] = (short)(numArray[intPtr] + 1);
								num5 = 0;
							}
						}
						else
						{
							num = 138;
							num1 = 3;
						}
						num3 = num6;
						num4++;
						do
						{
							if (num4 >= this.numCodes || num3 != this.length[num4])
							{
								break;
							}
							num4++;
							num2 = num5 + 1;
							num5 = num2;
						}
						while (num2 < num);
						if (num5 < num1)
						{
							short[] numArray2 = blTree.freqs;
							numArray = numArray2;
							int num8 = num3;
							intPtr = (IntPtr)num8;
							numArray2[num8] = (short)(numArray[intPtr] + (short)num5);
						}
						else if (num3 != 0)
						{
							short[] numArray3 = blTree.freqs;
							numArray3[16] = (short)(numArray3[16] + 1);
						}
						else if (num5 > 10)
						{
							short[] numArray4 = blTree.freqs;
							numArray4[18] = (short)(numArray4[18] + 1);
						}
						else
						{
							short[] numArray5 = blTree.freqs;
							numArray5[17] = (short)(numArray5[17] + 1);
						}
					}
				}

				public int GetEncodedLength()
				{
					int num = 0;
					for (int i = 0; i < (int)this.freqs.Length; i++)
					{
						num = num + this.freqs[i] * this.length[i];
					}
					return num;
				}

				public void SetStaticCodes(short[] stCodes, byte[] stLength)
				{
					this.codes = stCodes;
					this.length = stLength;
				}

				public void WriteSymbol(int code)
				{
					this.dh.pending.WriteBits(this.codes[code] & 65535, (int)this.length[code]);
				}

				public void WriteTree(SimpleZip.DeflaterHuffman.Tree blTree)
				{
					int num;
					int num1;
					int num2;
					int num3 = -1;
					int num4 = 0;
					while (num4 < this.numCodes)
					{
						int num5 = 1;
						int num6 = this.length[num4];
						if (num6 != 0)
						{
							num = 6;
							num1 = 3;
							if (num3 != num6)
							{
								blTree.WriteSymbol(num6);
								num5 = 0;
							}
						}
						else
						{
							num = 138;
							num1 = 3;
						}
						num3 = num6;
						num4++;
						do
						{
							if (num4 >= this.numCodes || num3 != this.length[num4])
							{
								break;
							}
							num4++;
							num2 = num5 + 1;
							num5 = num2;
						}
						while (num2 < num);
						if (num5 < num1)
						{
							while (true)
							{
								int num7 = num5;
								num5 = num7 - 1;
								if (num7 <= 0)
								{
									break;
								}
								blTree.WriteSymbol(num3);
							}
						}
						else if (num3 != 0)
						{
							blTree.WriteSymbol(16);
							this.dh.pending.WriteBits(num5 - 3, 2);
						}
						else if (num5 > 10)
						{
							blTree.WriteSymbol(18);
							this.dh.pending.WriteBits(num5 - 11, 7);
						}
						else
						{
							blTree.WriteSymbol(17);
							this.dh.pending.WriteBits(num5 - 3, 3);
						}
					}
				}
			}
		}

		internal sealed class DeflaterPending
		{
			protected byte[] buf;

			private int start;

			private int end;

			private uint bits;

			private int bitCount;

			public int BitCount
			{
				get
				{
					return this.bitCount;
				}
			}

			public bool IsFlushed
			{
				get
				{
					return this.end == 0;
				}
			}

			public DeflaterPending()
			{
			}

			public void AlignToByte()
			{
				if (this.bitCount > 0)
				{
					byte[] numArray = this.buf;
					SimpleZip.DeflaterPending deflaterPending = this;
					int num = deflaterPending.end;
					int num1 = num;
					deflaterPending.end = num + 1;
					numArray[num1] = (byte)this.bits;
					if (this.bitCount > 8)
					{
						byte[] numArray1 = this.buf;
						SimpleZip.DeflaterPending deflaterPending1 = this;
						int num2 = deflaterPending1.end;
						num1 = num2;
						deflaterPending1.end = num2 + 1;
						numArray1[num1] = (byte)(this.bits >> 8);
					}
				}
				this.bits = 0;
				this.bitCount = 0;
			}

			public int Flush(byte[] output, int offset, int length)
			{
				if (this.bitCount >= 8)
				{
					byte[] numArray = this.buf;
					SimpleZip.DeflaterPending deflaterPending = this;
					int num = deflaterPending.end;
					int num1 = num;
					deflaterPending.end = num + 1;
					numArray[num1] = (byte)this.bits;
					SimpleZip.DeflaterPending deflaterPending1 = this;
					deflaterPending1.bits = deflaterPending1.bits >> 8;
					SimpleZip.DeflaterPending deflaterPending2 = this;
					deflaterPending2.bitCount = deflaterPending2.bitCount - 8;
				}
				if (length <= this.end - this.start)
				{
					Array.Copy(this.buf, this.start, output, offset, length);
					SimpleZip.DeflaterPending deflaterPending3 = this;
					deflaterPending3.start = deflaterPending3.start + length;
				}
				else
				{
					length = this.end - this.start;
					Array.Copy(this.buf, this.start, output, offset, length);
					this.start = 0;
					this.end = 0;
				}
				return length;
			}

			public void WriteBits(int b, int count)
			{
				SimpleZip.DeflaterPending deflaterPending = this;
				deflaterPending.bits = deflaterPending.bits | b << (this.bitCount & 31);
				SimpleZip.DeflaterPending deflaterPending1 = this;
				deflaterPending1.bitCount = deflaterPending1.bitCount + count;
				if (this.bitCount >= 16)
				{
					byte[] numArray = this.buf;
					SimpleZip.DeflaterPending deflaterPending2 = this;
					int num = deflaterPending2.end;
					int num1 = num;
					deflaterPending2.end = num + 1;
					numArray[num1] = (byte)this.bits;
					byte[] numArray1 = this.buf;
					SimpleZip.DeflaterPending deflaterPending3 = this;
					int num2 = deflaterPending3.end;
					num1 = num2;
					deflaterPending3.end = num2 + 1;
					numArray1[num1] = (byte)(this.bits >> 8);
					SimpleZip.DeflaterPending deflaterPending4 = this;
					deflaterPending4.bits = deflaterPending4.bits >> 16;
					SimpleZip.DeflaterPending deflaterPending5 = this;
					deflaterPending5.bitCount = deflaterPending5.bitCount - 16;
				}
			}

			public void WriteBlock(byte[] block, int offset, int len)
			{
				Array.Copy(block, offset, this.buf, this.end, len);
				SimpleZip.DeflaterPending deflaterPending = this;
				deflaterPending.end = deflaterPending.end + len;
			}

			public void WriteShort(int s)
			{
				byte[] numArray = this.buf;
				SimpleZip.DeflaterPending deflaterPending = this;
				int num = deflaterPending.end;
				int num1 = num;
				deflaterPending.end = num + 1;
				numArray[num1] = (byte)s;
				byte[] numArray1 = this.buf;
				SimpleZip.DeflaterPending deflaterPending1 = this;
				int num2 = deflaterPending1.end;
				num1 = num2;
				deflaterPending1.end = num2 + 1;
				numArray1[num1] = (byte)(s >> 8);
			}
		}

		internal sealed class Inflater
		{
			private const int DECODE_HEADER = 0;

			private const int DECODE_DICT = 1;

			private const int DECODE_BLOCKS = 2;

			private const int DECODE_STORED_LEN1 = 3;

			private const int DECODE_STORED_LEN2 = 4;

			private const int DECODE_STORED = 5;

			private const int DECODE_DYN_HEADER = 6;

			private const int DECODE_HUFFMAN = 7;

			private const int DECODE_HUFFMAN_LENBITS = 8;

			private const int DECODE_HUFFMAN_DIST = 9;

			private const int DECODE_HUFFMAN_DISTBITS = 10;

			private const int DECODE_CHKSUM = 11;

			private const int FINISHED = 12;

			private readonly static int[] CPLENS;

			private readonly static int[] CPLEXT;

			private readonly static int[] CPDIST;

			private readonly static int[] CPDEXT;

			private int mode;

			private int neededBits;

			private int repLength;

			private int repDist;

			private int uncomprLen;

			private bool isLastBlock;

			private SimpleZip.StreamManipulator input;

			private SimpleZip.OutputWindow outputWindow;

			private SimpleZip.InflaterDynHeader dynHeader;

			private SimpleZip.InflaterHuffmanTree litlenTree;

			private SimpleZip.InflaterHuffmanTree distTree;

			static Inflater()
			{
				SimpleZip.Inflater.CPLENS = new int[] { 3, 4, 5, 6, 7, 8, 9, 10, 11, 13, 15, 17, 19, 23, 27, 31, 35, 43, 51, 59, 67, 83, 99, 115, 131, 163, 195, 227, 258 };
				SimpleZip.Inflater.CPLEXT = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 4, 5, 5, 5, 5, 0 };
				SimpleZip.Inflater.CPDIST = new int[] { 1, 2, 3, 4, 5, 7, 9, 13, 17, 25, 33, 49, 65, 97, 129, 193, 257, 385, 513, 769, 1025, 1537, 2049, 3073, 4097, 6145, 8193, 12289, 16385, 24577 };
				SimpleZip.Inflater.CPDEXT = new int[] { 0, 0, 0, 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7, 7, 8, 8, 9, 9, 10, 10, 11, 11, 12, 12, 13, 13 };
			}

			public Inflater(byte[] bytes)
			{
				this.input = new SimpleZip.StreamManipulator();
				this.outputWindow = new SimpleZip.OutputWindow();
				this.mode = 2;
				this.input.SetInput(bytes, 0, (int)bytes.Length);
			}

			private bool Decode()
			{
				int num;
				SimpleZip.Inflater inflater;
				switch (this.mode)
				{
					case 2:
					{
						if (this.isLastBlock)
						{
							this.mode = 12;
							return false;
						}
						int num1 = this.input.PeekBits(3);
						if (num1 < 0)
						{
							return false;
						}
						this.input.DropBits(3);
						if ((num1 & 1) != 0)
						{
							this.isLastBlock = true;
						}
						switch (num1 >> 1)
						{
							case 0:
							{
								this.input.SkipToByteBoundary();
								this.mode = 3;
								break;
							}
							case 1:
							{
								this.litlenTree = SimpleZip.InflaterHuffmanTree.defLitLenTree;
								this.distTree = SimpleZip.InflaterHuffmanTree.defDistTree;
								this.mode = 7;
								break;
							}
							case 2:
							{
								this.dynHeader = new SimpleZip.InflaterDynHeader();
								this.mode = 6;
								break;
							}
						}
						return true;
					}
					case 3:
					{
						int num2 = this.input.PeekBits(16);
						int num3 = num2;
						this.uncomprLen = num2;
						if (num3 < 0)
						{
							return false;
						}
						this.input.DropBits(16);
						this.mode = 4;
						if (this.input.PeekBits(16) < 0)
						{
							return false;
						}
						this.input.DropBits(16);
						this.mode = 5;
						num = this.outputWindow.CopyStored(this.input, this.uncomprLen);
						inflater = this;
						inflater.uncomprLen = inflater.uncomprLen - num;
						if (this.uncomprLen == 0)
						{
							this.mode = 2;
							return true;
						}
						return !this.input.IsNeedingInput;
					}
					case 4:
					{
						if (this.input.PeekBits(16) < 0)
						{
							return false;
						}
						this.input.DropBits(16);
						this.mode = 5;
						num = this.outputWindow.CopyStored(this.input, this.uncomprLen);
						inflater = this;
						inflater.uncomprLen = inflater.uncomprLen - num;
						if (this.uncomprLen == 0)
						{
							this.mode = 2;
							return true;
						}
						return !this.input.IsNeedingInput;
					}
					case 5:
					{
						num = this.outputWindow.CopyStored(this.input, this.uncomprLen);
						inflater = this;
						inflater.uncomprLen = inflater.uncomprLen - num;
						if (this.uncomprLen == 0)
						{
							this.mode = 2;
							return true;
						}
						return !this.input.IsNeedingInput;
					}
					case 6:
					{
						if (!this.dynHeader.Decode(this.input))
						{
							return false;
						}
						this.litlenTree = this.dynHeader.BuildLitLenTree();
						this.distTree = this.dynHeader.BuildDistTree();
						this.mode = 7;
						return this.DecodeHuffman();
					}
					case 7:
					case 8:
					case 9:
					case 10:
					{
						return this.DecodeHuffman();
					}
					case 11:
					{
						return false;
					}
					case 12:
					{
						return false;
					}
					default:
					{
						return false;
					}
				}
			}

			private bool DecodeHuffman()
			{
				int symbol;
				int num;
				int freeSpace = this.outputWindow.GetFreeSpace();
				while (freeSpace >= 258)
				{
					switch (this.mode)
					{
						case 7:
						{
							do
							{
								int symbol1 = this.litlenTree.GetSymbol(this.input);
								symbol = symbol1;
								if ((symbol1 & -256) == 0)
								{
									this.outputWindow.Write(symbol);
									num = freeSpace - 1;
									freeSpace = num;
								}
								else
								{
									if (symbol < 257)
									{
										if (symbol < 0)
										{
											return false;
										}
										this.distTree = null;
										this.litlenTree = null;
										this.mode = 2;
										return true;
									}
									this.repLength = SimpleZip.Inflater.CPLENS[symbol - 257];
									this.neededBits = SimpleZip.Inflater.CPLEXT[symbol - 257];
									goto case 8;
								}
							}
							while (num >= 258);
							return true;
						}
						case 8:
						{
							if (this.neededBits > 0)
							{
								this.mode = 8;
								int num1 = this.input.PeekBits(this.neededBits);
								if (num1 < 0)
								{
									return false;
								}
								this.input.DropBits(this.neededBits);
								SimpleZip.Inflater inflater = this;
								inflater.repLength = inflater.repLength + num1;
							}
							this.mode = 9;
							goto case 9;
						}
						case 9:
						{
							symbol = this.distTree.GetSymbol(this.input);
							if (symbol < 0)
							{
								return false;
							}
							this.repDist = SimpleZip.Inflater.CPDIST[symbol];
							this.neededBits = SimpleZip.Inflater.CPDEXT[symbol];
							goto case 10;
						}
						case 10:
						{
							if (this.neededBits > 0)
							{
								this.mode = 10;
								int num2 = this.input.PeekBits(this.neededBits);
								if (num2 < 0)
								{
									return false;
								}
								this.input.DropBits(this.neededBits);
								SimpleZip.Inflater inflater1 = this;
								inflater1.repDist = inflater1.repDist + num2;
							}
							this.outputWindow.Repeat(this.repLength, this.repDist);
							freeSpace = freeSpace - this.repLength;
							this.mode = 7;
							continue;
						}
						default:
						{
							continue;
						}
					}
				}
				return true;
			}

			public int Inflate(byte[] buf, int offset, int len)
			{
				int num = 0;
				do
				{
					if (this.mode == 11)
					{
						continue;
					}
					int num1 = this.outputWindow.CopyOutput(buf, offset, len);
					offset = offset + num1;
					num = num + num1;
					len = len - num1;
					if (len != 0)
					{
						continue;
					}
					return num;
				}
				while (this.Decode() || this.outputWindow.GetAvailable() > 0 && this.mode != 11);
				return num;
			}
		}

		internal sealed class InflaterDynHeader
		{
			private const int LNUM = 0;

			private const int DNUM = 1;

			private const int BLNUM = 2;

			private const int BLLENS = 3;

			private const int LENS = 4;

			private const int REPS = 5;

			private readonly static int[] repMin;

			private readonly static int[] repBits;

			private byte[] blLens;

			private byte[] litdistLens;

			private SimpleZip.InflaterHuffmanTree blTree;

			private int mode;

			private int lnum;

			private int dnum;

			private int blnum;

			private int num;

			private int repSymbol;

			private byte lastLen;

			private int ptr;

			private readonly static int[] BL_ORDER;

			static InflaterDynHeader()
			{
				SimpleZip.InflaterDynHeader.repMin = new int[] { 3, 3, 11 };
				SimpleZip.InflaterDynHeader.repBits = new int[] { 2, 3, 7 };
				SimpleZip.InflaterDynHeader.BL_ORDER = new int[] { 16, 17, 18, 0, 8, 7, 9, 6, 10, 5, 11, 4, 12, 3, 13, 2, 14, 1, 15 };
			}

			public InflaterDynHeader()
			{
			}

			public SimpleZip.InflaterHuffmanTree BuildDistTree()
			{
				byte[] numArray = new byte[checked((uint)this.dnum)];
				Array.Copy(this.litdistLens, this.lnum, numArray, 0, this.dnum);
				return new SimpleZip.InflaterHuffmanTree(numArray);
			}

			public SimpleZip.InflaterHuffmanTree BuildLitLenTree()
			{
				byte[] numArray = new byte[checked((uint)this.lnum)];
				Array.Copy(this.litdistLens, 0, numArray, 0, this.lnum);
				return new SimpleZip.InflaterHuffmanTree(numArray);
			}

			public bool Decode(SimpleZip.StreamManipulator input)
			{
				int num;
				while (true)
				{
					switch (this.mode)
					{
						case 0:
						{
							this.lnum = input.PeekBits(5);
							if (this.lnum < 0)
							{
								return false;
							}
							SimpleZip.InflaterDynHeader inflaterDynHeader = this;
							inflaterDynHeader.lnum = inflaterDynHeader.lnum + 257;
							input.DropBits(5);
							this.mode = 1;
							goto case 1;
						}
						case 1:
						{
							this.dnum = input.PeekBits(5);
							if (this.dnum < 0)
							{
								return false;
							}
							SimpleZip.InflaterDynHeader inflaterDynHeader1 = this;
							inflaterDynHeader1.dnum = inflaterDynHeader1.dnum + 1;
							input.DropBits(5);
							this.num = this.lnum + this.dnum;
							this.litdistLens = new byte[checked((uint)this.num)];
							this.mode = 2;
							goto case 2;
						}
						case 2:
						{
							this.blnum = input.PeekBits(4);
							if (this.blnum < 0)
							{
								return false;
							}
							SimpleZip.InflaterDynHeader inflaterDynHeader2 = this;
							inflaterDynHeader2.blnum = inflaterDynHeader2.blnum + 4;
							input.DropBits(4);
							this.blLens = new byte[19];
							this.ptr = 0;
							this.mode = 3;
							goto case 3;
						}
						case 3:
						{
							while (this.ptr < this.blnum)
							{
								int num1 = input.PeekBits(3);
								if (num1 < 0)
								{
									return false;
								}
								input.DropBits(3);
								this.blLens[SimpleZip.InflaterDynHeader.BL_ORDER[this.ptr]] = (byte)num1;
								SimpleZip.InflaterDynHeader inflaterDynHeader3 = this;
								inflaterDynHeader3.ptr = inflaterDynHeader3.ptr + 1;
							}
							this.blTree = new SimpleZip.InflaterHuffmanTree(this.blLens);
							this.blLens = null;
							this.ptr = 0;
							this.mode = 4;
							goto case 4;
						}
						case 4:
						{
							do
							{
								int symbol = this.blTree.GetSymbol(input);
								int num2 = symbol;
								if ((symbol & -16) == 0)
								{
									byte[] numArray = this.litdistLens;
									SimpleZip.InflaterDynHeader inflaterDynHeader4 = this;
									int num3 = inflaterDynHeader4.ptr;
									num = num3;
									inflaterDynHeader4.ptr = num3 + 1;
									byte num4 = (byte)num2;
									byte num5 = num4;
									this.lastLen = num4;
									numArray[num] = num5;
								}
								else
								{
									if (num2 < 0)
									{
										return false;
									}
									if (num2 >= 17)
									{
										this.lastLen = 0;
									}
									this.repSymbol = num2 - 16;
									this.mode = 5;
									goto case 5;
								}
							}
							while (this.ptr != this.num);
							return true;
						}
						case 5:
						{
							int num6 = SimpleZip.InflaterDynHeader.repBits[this.repSymbol];
							int num7 = input.PeekBits(num6);
							if (num7 < 0)
							{
								return false;
							}
							input.DropBits(num6);
							num7 = num7 + SimpleZip.InflaterDynHeader.repMin[this.repSymbol];
							while (true)
							{
								int num8 = num7;
								num7 = num8 - 1;
								if (num8 <= 0)
								{
									break;
								}
								byte[] numArray1 = this.litdistLens;
								SimpleZip.InflaterDynHeader inflaterDynHeader5 = this;
								int num9 = inflaterDynHeader5.ptr;
								num = num9;
								inflaterDynHeader5.ptr = num9 + 1;
								numArray1[num] = this.lastLen;
							}
							if (this.ptr == this.num)
							{
								return true;
							}
							this.mode = 4;
							continue;
						}
						default:
						{
							continue;
						}
					}
				}
				return true;
			}
		}

		internal sealed class InflaterHuffmanTree
		{
			private const int MAX_BITLEN = 15;

			private short[] tree;

			public readonly static SimpleZip.InflaterHuffmanTree defLitLenTree;

			public readonly static SimpleZip.InflaterHuffmanTree defDistTree;

			static InflaterHuffmanTree()
			{
				byte[] numArray = new byte[288];
				int num = 0;
				while (num < 144)
				{
					int num1 = num;
					num = num1 + 1;
					numArray[num1] = 8;
				}
				while (num < 256)
				{
					int num2 = num;
					num = num2 + 1;
					numArray[num2] = 9;
				}
				while (num < 280)
				{
					int num3 = num;
					num = num3 + 1;
					numArray[num3] = 7;
				}
				while (num < 288)
				{
					int num4 = num;
					num = num4 + 1;
					numArray[num4] = 8;
				}
				SimpleZip.InflaterHuffmanTree.defLitLenTree = new SimpleZip.InflaterHuffmanTree(numArray);
				numArray = new byte[32];
				num = 0;
				while (num < 32)
				{
					int num5 = num;
					num = num5 + 1;
					numArray[num5] = 5;
				}
				SimpleZip.InflaterHuffmanTree.defDistTree = new SimpleZip.InflaterHuffmanTree(numArray);
			}

			public InflaterHuffmanTree(byte[] codeLengths)
			{
				this.BuildTree(codeLengths);
			}

			private void BuildTree(byte[] codeLengths)
			{
				int[] numArray = new int[16];
				int[] numArray1 = new int[16];
				for (int i = 0; i < (int)codeLengths.Length; i++)
				{
					int num = codeLengths[i];
					if (num > 0)
					{
						int[] numArray2 = numArray;
						int[] numArray3 = numArray2;
						int num1 = num;
						numArray2[num1] = numArray3[(IntPtr)num1] + 1;
					}
				}
				int num2 = 0;
				int num3 = 512;
				for (int j = 1; j <= 15; j++)
				{
					numArray1[j] = num2;
					num2 = num2 + (numArray[j] << (16 - j & 31));
					if (j >= 10)
					{
						int num4 = numArray1[j] & 130944;
						int num5 = num2 & 130944;
						num3 = num3 + (num5 - num4 >> (16 - j & 31));
					}
				}
				this.tree = new short[checked((uint)num3)];
				int num6 = 512;
				for (int k = 15; k >= 10; k--)
				{
					int num7 = num2 & 130944;
					num2 = num2 - (numArray[k] << (16 - k & 31));
					for (int l = num2 & 130944; l < num7; l = l + 128)
					{
						this.tree[SimpleZip.DeflaterHuffman.BitReverse(l)] = (short)(-num6 << 4 | k);
						num6 = num6 + (1 << (k - 9 & 31));
					}
				}
				for (int m = 0; m < (int)codeLengths.Length; m++)
				{
					int num8 = codeLengths[m];
					if (num8 != 0)
					{
						num2 = numArray1[num8];
						int num9 = SimpleZip.DeflaterHuffman.BitReverse(num2);
						if (num8 > 9)
						{
							int num10 = this.tree[num9 & 511];
							int num11 = 1 << (num10 & 15 & 31);
							num10 = -(num10 >> 4);
							do
							{
								this.tree[num10 | num9 >> 9] = (short)(m << 4 | num8);
								num9 = num9 + (1 << (num8 & 31));
							}
							while (num9 < num11);
						}
						else
						{
							do
							{
								this.tree[num9] = (short)(m << 4 | num8);
								num9 = num9 + (1 << (num8 & 31));
							}
							while (num9 < 512);
						}
						numArray1[num8] = num2 + (1 << (16 - num8 & 31));
					}
				}
			}

			public int GetSymbol(SimpleZip.StreamManipulator input)
			{
				int num;
				int num1 = input.PeekBits(9);
				int num2 = num1;
				if (num1 < 0)
				{
					int availableBits = input.AvailableBits;
					num2 = input.PeekBits(availableBits);
					num = this.tree[num2];
					if (num < 0 || (num & 15) > availableBits)
					{
						return -1;
					}
					input.DropBits(num & 15);
					return num >> 4;
				}
				short num3 = this.tree[num2];
				num = (int)num3;
				if (num3 >= 0)
				{
					input.DropBits(num & 15);
					return num >> 4;
				}
				int num4 = -(num >> 4);
				int num5 = input.PeekBits(num & 15);
				num2 = num5;
				if (num5 >= 0)
				{
					num = this.tree[num4 | num2 >> 9];
					input.DropBits(num & 15);
					return num >> 4;
				}
				int availableBits1 = input.AvailableBits;
				num2 = input.PeekBits(availableBits1);
				num = this.tree[num4 | num2 >> 9];
				if ((num & 15) > availableBits1)
				{
					return -1;
				}
				input.DropBits(num & 15);
				return num >> 4;
			}
		}

		internal sealed class OutputWindow
		{
			private const int WINDOW_SIZE = 32768;

			private const int WINDOW_MASK = 32767;

			private byte[] window;

			private int windowEnd;

			private int windowFilled;

			public OutputWindow()
			{
			}

			public void CopyDict(byte[] dict, int offset, int len)
			{
				if (this.windowFilled > 0)
				{
					throw new InvalidOperationException();
				}
				if (len > 32768)
				{
					offset = offset + (len - 32768);
					len = 32768;
				}
				Array.Copy(dict, offset, this.window, 0, len);
				this.windowEnd = len & 32767;
			}

			public int CopyOutput(byte[] output, int offset, int len)
			{
				int num = this.windowEnd;
				if (len <= this.windowFilled)
				{
					num = this.windowEnd - this.windowFilled + len & 32767;
				}
				else
				{
					len = this.windowFilled;
				}
				int num1 = len;
				int num2 = len - num;
				if (num2 > 0)
				{
					Array.Copy(this.window, 32768 - num2, output, offset, num2);
					offset = offset + num2;
					len = num;
				}
				Array.Copy(this.window, num - len, output, offset, len);
				SimpleZip.OutputWindow outputWindow = this;
				outputWindow.windowFilled = outputWindow.windowFilled - num1;
				if (this.windowFilled < 0)
				{
					throw new InvalidOperationException();
				}
				return num1;
			}

			public int CopyStored(SimpleZip.StreamManipulator input, int len)
			{
				int num;
				len = Math.Min(Math.Min(len, 32768 - this.windowFilled), input.AvailableBytes);
				int num1 = 32768 - this.windowEnd;
				if (len <= num1)
				{
					num = input.CopyBytes(this.window, this.windowEnd, len);
				}
				else
				{
					num = input.CopyBytes(this.window, this.windowEnd, num1);
					if (num == num1)
					{
						num = num + input.CopyBytes(this.window, 0, len - num1);
					}
				}
				this.windowEnd = this.windowEnd + num & 32767;
				SimpleZip.OutputWindow outputWindow = this;
				outputWindow.windowFilled = outputWindow.windowFilled + num;
				return num;
			}

			public int GetAvailable()
			{
				return this.windowFilled;
			}

			public int GetFreeSpace()
			{
				return 32768 - this.windowFilled;
			}

			public void Repeat(int len, int dist)
			{
				SimpleZip.OutputWindow outputWindow = this;
				int num = outputWindow.windowFilled + len;
				int num1 = num;
				outputWindow.windowFilled = num;
				if (num1 > 32768)
				{
					throw new InvalidOperationException();
				}
				int num2 = this.windowEnd - dist & 32767;
				int num3 = 32768 - len;
				if (num2 > num3 || this.windowEnd >= num3)
				{
					this.SlowRepeat(num2, len, dist);
					return;
				}
				if (len <= dist)
				{
					Array.Copy(this.window, num2, this.window, this.windowEnd, len);
					SimpleZip.OutputWindow outputWindow1 = this;
					outputWindow1.windowEnd = outputWindow1.windowEnd + len;
					return;
				}
				while (true)
				{
					int num4 = len;
					len = num4 - 1;
					if (num4 <= 0)
					{
						break;
					}
					byte[] numArray = this.window;
					SimpleZip.OutputWindow outputWindow2 = this;
					int num5 = outputWindow2.windowEnd;
					num1 = num5;
					outputWindow2.windowEnd = num5 + 1;
					int num6 = num2;
					num2 = num6 + 1;
					numArray[num1] = this.window[num6];
				}
			}

			public void Reset()
			{
				int num = 0;
				int num1 = num;
				this.windowEnd = num;
				this.windowFilled = num1;
			}

			private void SlowRepeat(int repStart, int len, int dist)
			{
				while (true)
				{
					int num = len;
					len = num - 1;
					if (num <= 0)
					{
						break;
					}
					byte[] numArray = this.window;
					SimpleZip.OutputWindow outputWindow = this;
					int num1 = outputWindow.windowEnd;
					int num2 = num1;
					outputWindow.windowEnd = num1 + 1;
					int num3 = repStart;
					repStart = num3 + 1;
					numArray[num2] = this.window[num3];
					SimpleZip.OutputWindow outputWindow1 = this;
					outputWindow1.windowEnd = outputWindow1.windowEnd & 32767;
					repStart = repStart & 32767;
				}
			}

			public void Write(int abyte)
			{
				SimpleZip.OutputWindow outputWindow = this;
				int num = outputWindow.windowFilled;
				int num1 = num;
				outputWindow.windowFilled = num + 1;
				if (num1 == 32768)
				{
					throw new InvalidOperationException();
				}
				byte[] numArray = this.window;
				SimpleZip.OutputWindow outputWindow1 = this;
				int num2 = outputWindow1.windowEnd;
				num1 = num2;
				outputWindow1.windowEnd = num2 + 1;
				numArray[num1] = (byte)abyte;
				SimpleZip.OutputWindow outputWindow2 = this;
				outputWindow2.windowEnd = outputWindow2.windowEnd & 32767;
			}
		}

		internal sealed class StreamManipulator
		{
			private byte[] window;

			private int window_start;

			private int window_end;

			private uint buffer;

			private int bits_in_buffer;

			public int AvailableBits
			{
				get
				{
					return this.bits_in_buffer;
				}
			}

			public int AvailableBytes
			{
				get
				{
					return this.window_end - this.window_start + (this.bits_in_buffer >> 3);
				}
			}

			public bool IsNeedingInput
			{
				get
				{
					return this.window_start == this.window_end;
				}
			}

			public StreamManipulator()
			{
			}

			public int CopyBytes(byte[] output, int offset, int length)
			{
				int num = 0;
				while (this.bits_in_buffer > 0 && length > 0)
				{
					int num1 = offset;
					offset = num1 + 1;
					output[num1] = (byte)this.buffer;
					SimpleZip.StreamManipulator streamManipulator = this;
					streamManipulator.buffer = streamManipulator.buffer >> 8;
					SimpleZip.StreamManipulator bitsInBuffer = this;
					bitsInBuffer.bits_in_buffer = bitsInBuffer.bits_in_buffer - 8;
					length--;
					num++;
				}
				if (length == 0)
				{
					return num;
				}
				int windowEnd = this.window_end - this.window_start;
				if (length > windowEnd)
				{
					length = windowEnd;
				}
				Array.Copy(this.window, this.window_start, output, offset, length);
				SimpleZip.StreamManipulator windowStart = this;
				windowStart.window_start = windowStart.window_start + length;
				if ((this.window_start - this.window_end & 1) != 0)
				{
					byte[] numArray = this.window;
					SimpleZip.StreamManipulator streamManipulator1 = this;
					int windowStart1 = streamManipulator1.window_start;
					int num2 = windowStart1;
					streamManipulator1.window_start = windowStart1 + 1;
					this.buffer = (uint)(numArray[num2] & 255);
					this.bits_in_buffer = 8;
				}
				return num + length;
			}

			public void DropBits(int n)
			{
				SimpleZip.StreamManipulator streamManipulator = this;
				streamManipulator.buffer = streamManipulator.buffer >> (n & 31);
				SimpleZip.StreamManipulator bitsInBuffer = this;
				bitsInBuffer.bits_in_buffer = bitsInBuffer.bits_in_buffer - n;
			}

			public int PeekBits(int n)
			{
				if (this.bits_in_buffer < n)
				{
					if (this.window_start == this.window_end)
					{
						return -1;
					}
					SimpleZip.StreamManipulator bitsInBuffer = this;
					uint num = bitsInBuffer.buffer;
					byte[] numArray = this.window;
					SimpleZip.StreamManipulator streamManipulator = this;
					int windowStart = streamManipulator.window_start;
					int num1 = windowStart;
					streamManipulator.window_start = windowStart + 1;
					int num2 = numArray[num1] & 255;
					byte[] numArray1 = this.window;
					SimpleZip.StreamManipulator streamManipulator1 = this;
					int windowStart1 = streamManipulator1.window_start;
					num1 = windowStart1;
					streamManipulator1.window_start = windowStart1 + 1;
					bitsInBuffer.buffer = num | (num2 | (numArray1[num1] & 255) << 8) << (this.bits_in_buffer & 31);
					SimpleZip.StreamManipulator bitsInBuffer1 = this;
					bitsInBuffer1.bits_in_buffer = bitsInBuffer1.bits_in_buffer + 16;
				}
				return (int)((ulong)this.buffer & (long)((1 << (n & 31)) - 1));
			}

			public void Reset()
			{
				int num = 0;
				int num1 = num;
				this.bits_in_buffer = num;
				int num2 = num1;
				num1 = num2;
				this.window_end = num2;
				int num3 = num1;
				num1 = num3;
				this.window_start = num3;
				this.buffer = (uint)num1;
			}

			public void SetInput(byte[] buf, int off, int len)
			{
				if (this.window_start < this.window_end)
				{
					throw new InvalidOperationException();
				}
				int num = off + len;
				if (0 > off || off > num || num > (int)buf.Length)
				{
					throw new ArgumentOutOfRangeException();
				}
				if ((len & 1) != 0)
				{
					SimpleZip.StreamManipulator streamManipulator = this;
					int num1 = off;
					off = num1 + 1;
					streamManipulator.buffer = streamManipulator.buffer | (buf[num1] & 255) << (this.bits_in_buffer & 31);
					SimpleZip.StreamManipulator bitsInBuffer = this;
					bitsInBuffer.bits_in_buffer = bitsInBuffer.bits_in_buffer + 8;
				}
				this.window = buf;
				this.window_start = off;
				this.window_end = num;
			}

			public void SkipToByteBoundary()
			{
				SimpleZip.StreamManipulator bitsInBuffer = this;
				bitsInBuffer.buffer = bitsInBuffer.buffer >> (this.bits_in_buffer & 7 & 31);
				SimpleZip.StreamManipulator streamManipulator = this;
				streamManipulator.bits_in_buffer = streamManipulator.bits_in_buffer & -8;
			}
		}

		internal sealed class ZipStream : MemoryStream
		{
			public ZipStream()
			{
			}

			public ZipStream(byte[] buffer) : base(buffer, false)
			{
			}

			public int ReadInt()
			{
				return this.ReadShort() | this.ReadShort() << 16;
			}

			public int ReadShort()
			{
				return this.ReadByte() | this.ReadByte() << 8;
			}

			public void WriteInt(int value)
			{
				this.WriteShort(value);
				this.WriteShort(value >> 16);
			}

			public void WriteShort(int value)
			{
				this.WriteByte((byte)(value & 255));
				this.WriteByte((byte)(value >> 8 & 255));
			}
		}
	}
}
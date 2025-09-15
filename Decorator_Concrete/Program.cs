using System.Diagnostics;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Decorator_Concrete
{
	public interface IDataSourse
	{
		void WriteData(string data);
		string ReadData();
	}

	public class FileDataSource : IDataSourse
	{
		private string _filename;

		public FileDataSource(string filename)
		{
			_filename = filename;
		}

		public void WriteData(string data)
		{
			try
			{
				File.WriteAllText(_filename, data);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString(), ex.InnerException, ex.Message);
			}
		}

		public string ReadData()
		{
			string resp = string.Empty;

			try
			{
				resp = File.ReadAllText(_filename);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.ToString(), ex.InnerException, ex.Message);
			}
			return resp;
		}
	}

	public abstract class DataSourceDecorator : IDataSourse
	{
		protected IDataSourse _wrappee;

		public DataSourceDecorator(IDataSourse sourse)
		{
			_wrappee = sourse;
		}

		public virtual void WriteData(string data)
		{
			_wrappee.WriteData(data);
		}

		public virtual string ReadData()
		{
			return _wrappee.ReadData();
		}
	}

	public class EncryptionDecorator : DataSourceDecorator
	{
		private byte[] _key = Encoding.UTF8.GetBytes("qwertyuiop[]asdf");

		public EncryptionDecorator(IDataSourse sourse) : base(sourse) { }

		public override void WriteData(string data)
		{
			string encryptedData = Encrypt(data);
			base.WriteData(encryptedData);
		}

		public override string ReadData()
		{
			string encryptedData = base.ReadData();
			return Decrypt(encryptedData);
		}

		private string Encrypt(string plainText)
		{

			using (Aes aes = Aes.Create())
			{
				aes.Key = _key;
				aes.IV = new byte[16];
				ICryptoTransform en = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream ms = new MemoryStream())
				using (CryptoStream cs = new CryptoStream(ms, en, CryptoStreamMode.Write))
				{

					using (StreamWriter sw = new StreamWriter(cs))
					{
						sw.Write(plainText);
					}
					return Convert.ToBase64String(ms.ToArray());
				}
			}
		}



		private string Decrypt(string cipherText)
		{
			using (Aes aes = Aes.Create())
			{
				aes.Key = _key;
				aes.IV = new byte[16];
				ICryptoTransform dec = aes.CreateDecryptor(aes.Key, aes.IV);
				byte[] buff = Convert.FromBase64String(cipherText);
				using (MemoryStream ms = new MemoryStream(buff))
				using (CryptoStream cs = new CryptoStream(ms, dec, CryptoStreamMode.Read))
				using (StreamReader sr = new StreamReader(cs))
				{
					return sr.ReadToEnd();
				}
			}
		}
	}

	public class CompressionDecorator : DataSourceDecorator
	{
		public CompressionDecorator(IDataSourse sourse) : base(sourse) { }

		public override void WriteData(string data)
		{
			string compressData = Compress(data);
			base.WriteData(compressData);
		}

		public override string ReadData()
		{
			string compressData = base.ReadData();
			return Decompress(compressData);
		}

		private string Compress(string data)
		{
			byte[] buffer = Encoding.UTF8.GetBytes(data);
			using (MemoryStream ms = new MemoryStream())
			{
				using (GZipStream gzip = new GZipStream(ms, CompressionMode.Compress, true))
				{
					gzip.Write(buffer, 0, buffer.Length); 
				}

				return Convert.ToBase64String(ms.ToArray());
			}
		}


		private string Decompress(string compressedData)
		{
			byte[] buff = Convert.FromBase64String(compressedData);
			
			using (MemoryStream ms = new MemoryStream(buff))
			{
				using (GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress))
				{
					using (MemoryStream res = new MemoryStream())
					{
						gzip.CopyTo(res);
						return Encoding.UTF8.GetString(res.ToArray());
					}
				}
			}
		}
	}


	// Клиентский код
	public class SalaryManager
	{
		private IDataSourse _dataSourse;
		private string _salaryRecords = "Salary data examle!";
		public SalaryManager(IDataSourse sourse)
		{
			_dataSourse = sourse;
		}

		public string Load()
		{
			return _dataSourse.ReadData();
		}

		public void Save()
		{
			_dataSourse.WriteData(_salaryRecords);
		}

		public class Application
		{
			public void DumpUsageExample()
			{
				IDataSourse source = new FileDataSource("somefile.dat");
				string salaryRecords = "Records Salary DATA";

				source.WriteData(salaryRecords);

				source = new CompressionDecorator(source);
				source.WriteData(salaryRecords);

				source = new EncryptionDecorator(source);
				source.WriteData(salaryRecords);

			}
		}
	}


	public class ApplicationConfigurator
	{
		public void ConfigurationExample(bool enableEncryption, bool enableCompression)
		{
			IDataSourse source = new FileDataSource("slary.dat");
			if (enableEncryption)
			{
				source = new EncryptionDecorator(source);
			}

			if (enableCompression)
			{
				source = new CompressionDecorator(source);
			}

			SalaryManager logger = new SalaryManager(source);
			string salary = logger.Load();
		}
	}

	internal class Program
	{
		static void Main(string[] args)
		{
			IDataSourse fileSource = new FileDataSource("data222.txt");

			IDataSourse encrDecor = new EncryptionDecorator(fileSource);
			IDataSourse comressedSource = new CompressionDecorator(encrDecor);
			comressedSource.WriteData("Привет");

			Console.WriteLine($"{fileSource.ReadData()}");

		}
	}
}

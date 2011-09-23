namespace Roberocity.Storage
{
	using System;
	using System.IO;
	
	public class DiskFileStore : IFileStore
	{
		readonly string _basePath;

		public DiskFileStore(string basePath)
		{
			if(string.IsNullOrEmpty(basePath)) 
				throw new ArgumentNullException("basePath", "base path cannot be null when creating a DiskFileStore");
			_basePath = basePath;
		}

		protected string StorageLocation
		{
			get { return _basePath; }
		}

		public Guid Save(Stream file)
		{
			EnsurePath(StorageLocation);
			var identifier = Guid.NewGuid();
			var path = Path.Combine(StorageLocation, identifier.ToString());
			Write(path, file);
			return identifier;
		}

		static void Write(string path, Stream contents)
		{
			using(var writer = new FileStream(path, FileMode.Create, FileAccess.Write)) {
				int count = Convert.ToInt32(contents.Length);
				var buffer = new byte[count];
				if(contents.Read(buffer, 0, count) == count) {
					writer.Write(buffer, 0, count);
				}
			}

			contents.Close();
			contents.Dispose();
		}


		public void Delete(Guid identifier)
		{
			string path = GetPath(identifier);
			if (File.Exists(path)) File.Delete(path);
		}

		public void Replace(Guid identifier, Stream file)
		{
			string path = Path.Combine(StorageLocation, identifier.ToString());
			Write(path, file);
		}

		public Stream Get(Guid identifier)
		{
			string path = GetPath(identifier);
			return new FileStream(path, FileMode.Open, FileAccess.Read);
		}

		string GetPath(Guid identifier)
		{
			string path = Path.Combine(StorageLocation, identifier.ToString());
			if (!File.Exists(path)) throw new FileNotFoundException("Could not find the file.", identifier.ToString());
			return path;
		}

		private static void EnsurePath(string path)
		{
			if (!Directory.Exists(path)) Directory.CreateDirectory(path);
		}

		private string GetFileLocation(Guid identifier)
		{
			return Path.Combine(StorageLocation, identifier.ToString());
		}
	}
}
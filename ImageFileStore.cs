namespace Roberocity.Storage
{
	using System;
	using System.IO;

	public class ImageFileStore : IImageStore
	{

		public ImageFileStore(string rawPath, string cachePath)
		{
			if(string.IsNullOrEmpty(rawPath)) throw new ArgumentNullException("rawPath", "The raw image storage path cannot be null or empty");
			if(string.IsNullOrEmpty(cachePath)) throw new ArgumentNullException("cachePath", "The cached image storage path cannot be null or empty");

			RawStorageLocation = rawPath;
			CacheStorageLocation = cachePath;
		}

		protected string RawStorageLocation { get; set; }
		protected string CacheStorageLocation {get; set; }

		static void EnsurePath(string path)
		{
			if(!Directory.Exists(path)) Directory.CreateDirectory(path);
		}

		string GetRawLocation(Guid identifier)
		{
			return Path.Combine(RawStorageLocation, identifier.ToString());
		}

		private static void Write(string path, Stream contents)
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

		public Guid Save(Stream file)
		{
			EnsurePath(RawStorageLocation);
			var identifier = Guid.NewGuid();
			var path = Path.Combine(RawStorageLocation, identifier.ToString());
			Write(path, file);
			return identifier;
		}

		public void SaveInCache(Stream file, Guid guid, int width, int height)
		{
			EnsurePath(CacheStorageLocation);
			var filename = GetCacheFileName(guid, width, height);
			var path = Path.Combine(CacheStorageLocation, filename);
			Write(path, file);
		}

		public Stream Get(Guid identifier)
		{
			EnsurePath(RawStorageLocation);
			string filepath = GetRawLocation(identifier);
			if (!File.Exists(filepath)) throw new FileNotFoundException("Image was not found");
			return new FileStream(filepath, FileMode.Open, FileAccess.Read);
		}

		public void Delete(Guid identifier)
		{
			DeleteFromRaw(identifier);
			DeleteFromCache(identifier);
		}

		public void Replace(Guid identifier, Stream file)
		{
			DeleteFromCache(identifier);
			var path = Path.Combine(RawStorageLocation, identifier.ToString());
			Write(path, file);
		}

		void DeleteFromRaw(Guid identifier)
		{
			var path = GetRawLocation(identifier);
			if(File.Exists(path)) File.Delete(path);
		}

		public Stream GetFromCache(Guid identifier, int width, int height)
		{
			EnsurePath(CacheStorageLocation);
			if (!IsInCache(identifier, width, height)) return Stream.Null;
			string filepath = GetCacheFileName(identifier, width, height);
			if (!File.Exists(filepath)) return Stream.Null;
			return new FileStream(filepath, FileMode.Open, FileAccess.Read);
		}

		string GetCacheFileName(Guid identifier, int width, int height)
		{
			string filename = string.Format("{0}!{1}!{2}", identifier, width, height);
			return Path.Combine(CacheStorageLocation, filename);
		}

		public void DeleteFromCache(Guid identifier)
		{
			EnsurePath(CacheStorageLocation);
			var filter = identifier + "*";
			var files = Directory.GetFiles(CacheStorageLocation, filter);
			foreach(var file in files)
				File.Delete(file);
		}

		public bool IsInCache(Guid identifier, int width, int height)
		{
			EnsurePath(CacheStorageLocation);
			string filename = GetCacheFileName(identifier, width, height);
			return File.Exists(Path.Combine(CacheStorageLocation, filename));
		}
	}
}
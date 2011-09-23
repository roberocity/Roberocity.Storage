namespace Roberocity.Storage
{
	using System;
	using System.IO;

	public interface IImageStore : IFileStore
	{
		Stream GetFromCache(Guid identifier, int width, int height);
		void DeleteFromCache(Guid identifier);
		bool IsInCache(Guid identifier, int width, int height);
		void SaveInCache(Stream file, Guid guid, int width, int height);
	}
}
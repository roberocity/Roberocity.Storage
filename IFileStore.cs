
namespace Roberocity.Storage
{
	using System;
	using System.IO;

	public interface IFileStore
	{
		Guid Save(Stream file);
		Stream Get(Guid identifier);
		void Delete(Guid identifier);
		void Replace(Guid identifier, Stream file);
	}
}
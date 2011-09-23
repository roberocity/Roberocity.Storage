# Simple Storage Library for C#

This is a library that I've used in several projects to save uploaded files as attachments to posts or other content.

Saves files to an IFileStore and returns a GUID. The GUID can be used to identify the file and retrieve it later. 

Current implementations of the IFileStore include a simple DiskFileStore for storing on a local system and the ImageFileStore that provides an additional storage option for a cache of resized images.

The idea of the IFileStore interface was to be a generic interface that could implement storage in other systems as well. I implemented an Amazon S3 version for a client, but am not able to release that to the public. 

# License

It's yours. Use it freely. Modify at will. Make it better.
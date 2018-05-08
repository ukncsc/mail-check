using System;
using System.IO;
using System.IO.Compression;

namespace Dmarc.AggregateReport.Parser.Lambda.Compression
{
    public interface IZipDecompressor : IDecompressor { }

    public class ZipDecompressor : IZipDecompressor
    {
        public string StreamType => "Zip";

        public Stream Decompress(Stream stream)
        {
            using (ZipArchive archive = new ZipArchive(stream))
            {
                if (archive.Entries.Count != 1)
                {
                    throw new ArgumentException($"Expected only 1 zip entires found {archive.Entries.Count}");
                }

                if (archive.Entries[0].FullName.Contains("/"))
                {
                    throw new ArgumentException("Expected file not directory.");
                }

                MemoryStream memoryStream = new MemoryStream();

                Stream compressedStream = archive.Entries[0].Open();

                compressedStream.CopyTo(memoryStream);

                return memoryStream;
            }
        }
    }
}
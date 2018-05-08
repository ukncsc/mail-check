using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dmarc.MxSecurityTester.Util
{
    public interface IStreamReader : IDisposable
    {
        void DiscardBufferedData();
        int Peek();
        int Read();
        int Read(char[] buffer, int index, int count);
        Task<int> ReadAsync(char[] buffer, int index, int count);
        int ReadBlock(char[] buffer, int index, int count);
        Task<int> ReadBlockAsync(char[] buffer, int index, int count);
        string ReadLine();
        Task<string> ReadLineAsync();
        string ReadToEnd();
        Task<string> ReadToEndAsync();
        Stream BaseStream { get; }
        Encoding CurrentEncoding { get; }
        bool EndOfStream { get; }
    }

    public class StreamReaderWrapper : StreamReader, IStreamReader
    {
        public StreamReaderWrapper(Stream stream) : base(stream)
        {
        }

        public StreamReaderWrapper(Stream stream, bool detectEncodingFromByteOrderMarks) 
            : base(stream, detectEncodingFromByteOrderMarks)
        {
        }

        public StreamReaderWrapper(Stream stream, Encoding encoding) 
            : base(stream, encoding)
        {
        }

        public StreamReaderWrapper(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) 
            : base(stream, encoding, detectEncodingFromByteOrderMarks)
        {
        }

        public StreamReaderWrapper(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) 
            : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        {
        }

        public StreamReaderWrapper(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen) 
            : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen)
        {
        }
    }
}
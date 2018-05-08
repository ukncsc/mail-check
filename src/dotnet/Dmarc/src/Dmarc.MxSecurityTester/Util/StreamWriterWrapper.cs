using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Dmarc.MxSecurityTester.Util
{
    public interface IStreamWriter : IDisposable
    {
        void Flush();
        Task FlushAsync();
        void Write(char value);
        void Write(char[] buffer);
        void Write(char[] buffer, int index, int count);
        void Write(string value);
        Task WriteAsync(char value);
        Task WriteAsync(char[] buffer, int index, int count);
        Task WriteAsync(string value);
        Task WriteLineAsync();
        Task WriteLineAsync(char value);
        Task WriteLineAsync(char[] buffer, int index, int count);
        Task WriteLineAsync(string value);
        bool AutoFlush { get; set; }
        Stream BaseStream { get; }
        Encoding Encoding { get; }
        IFormatProvider FormatProvider { get; }
        string NewLine { get; set; }
        void Write(bool value);
        void Write(decimal value);
        void Write(double value);
        void Write(int value);
        void Write(long value);
        void Write(object value);
        void Write(float value);
        void Write(string format, object arg0);
        void Write(string format, object arg0, object arg1);
        void Write(string format, object arg0, object arg1, object arg2);
        void Write(string format, params object[] arg);
        void Write(uint value);
        void Write(ulong value);
        Task WriteAsync(char[] buffer);
        void WriteLine();
        void WriteLine(bool value);
        void WriteLine(char value);
        void WriteLine(char[] buffer);
        void WriteLine(char[] buffer, int index, int count);
        void WriteLine(decimal value);
        void WriteLine(double value);
        void WriteLine(int value);
        void WriteLine(long value);
        void WriteLine(object value);
        void WriteLine(float value);
        void WriteLine(string value);
        void WriteLine(string format, object arg0);
        void WriteLine(string format, object arg0, object arg1);
        void WriteLine(string format, object arg0, object arg1, object arg2);
        void WriteLine(string format, params object[] arg);
        void WriteLine(uint value);
        void WriteLine(ulong value);
        Task WriteLineAsync(char[] buffer);
    }

    public class StreamWriterWrapper : StreamWriter, IStreamWriter
    {
        public StreamWriterWrapper(Stream stream) 
            : base(stream)
        {
        }

        public StreamWriterWrapper(Stream stream, Encoding encoding) 
            : base(stream, encoding)
        {
        }

        public StreamWriterWrapper(Stream stream, Encoding encoding, int bufferSize) 
            : base(stream, encoding, bufferSize)
        {
        }

        public StreamWriterWrapper(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen) 
            : base(stream, encoding, bufferSize, leaveOpen)
        {
        }
    }
}
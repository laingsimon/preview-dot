using System.IO;

namespace PreviewDot
{
    internal class StreamHelper
    {
        public Stream ExcludeBomFromStream(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.Seek(0, SeekOrigin.Begin);
            stream.CopyTo(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            var buffer = new byte[3];
            var bytesRead = memoryStream.Read(buffer, 0, 3);

            if (bytesRead != 3 || buffer[0] != 0xef || buffer[1] != 0xbb || buffer[2] != 0xbf)
            {
                memoryStream.Seek(0, SeekOrigin.Begin); //there is no BOM, reset to the start so the start data isn't excluded
                return memoryStream;
            }

            var cleanStream = new MemoryStream();
            memoryStream.CopyTo(cleanStream); //we've read the BOM so it'll not be copied

            cleanStream.Seek(0, SeekOrigin.Begin); //reset the new stream back to the start, no BOM in this one.
            return cleanStream;
        }
    }
}

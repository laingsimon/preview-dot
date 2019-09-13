using System.IO;

namespace PreviewDot
{
    public class GeneratePreviewResult
    {
        public GeneratePreviewResult(Stream imageData)
        {
            ImageData = imageData;
            Success = true;
        }

        public GeneratePreviewResult(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Success = false;
        }

        public string ErrorMessage { get; }
        public Stream ImageData { get; }
        public bool Success { get; }
    }
}

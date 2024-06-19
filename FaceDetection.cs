using System.Drawing;
using System.Linq;
using FaceRecognitionDotNet;

namespace MVCTest1
{
    public class FaceDetection
    {
        private readonly FaceRecognition _faceRecognition;

        public FaceDetection(string modelDirectory)
        {
            _faceRecognition = FaceRecognition.Create(modelDirectory);
        }

        public Rectangle? DetectFace(Bitmap bitmap)
        {
            using (var image = FaceRecognition.LoadImage(bitmap))
            {
                var faceLocations = _faceRecognition.FaceLocations(image).ToArray();
                if (faceLocations.Any())
                {
                    var faceLocation = faceLocations.First();
                    return new Rectangle(faceLocation.Left, faceLocation.Top, faceLocation.Right - faceLocation.Left, faceLocation.Bottom - faceLocation.Top);
                }
            }
            return null;
        }

    }
}

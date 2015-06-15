using System;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace PeekABoo
{
    class PeekTrigger : StateTriggerBase
    {
        private static MediaCapture _mediaCapture;
        private static FaceDetectionEffect _faceDetectionEffect;

        private string mode;
        public string Mode
        {
            set
            {
                mode = value;
                SetupFaceDetection();
            }
        }

        // initialize DaceDetectionEffect
        private void SetupFaceDetection()
        {
            if (_mediaCapture == null || _faceDetectionEffect == null)
            {

                _mediaCapture = new MediaCapture();
                CaptureElement element = new CaptureElement();
                var settings = new MediaCaptureInitializationSettings()
                {
                    StreamingCaptureMode = StreamingCaptureMode.Video,
                };
                _mediaCapture.InitializeAsync(settings).AsTask().Wait();
                element.Source = _mediaCapture;
                _mediaCapture.StartPreviewAsync().AsTask().Wait();
                

                var definition = new FaceDetectionEffectDefinition();
                definition.SynchronousDetectionEnabled = false;
                definition.DetectionMode = FaceDetectionMode.HighPerformance;

                var t = _mediaCapture.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview).AsTask();
                t.Wait();
                _faceDetectionEffect = (FaceDetectionEffect)t.Result;

                _faceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
                _faceDetectionEffect.Enabled = true;
            }

            _faceDetectionEffect.FaceDetected += FaceDetectionEffect_FaceDetected;
        }

        private async void FaceDetectionEffect_FaceDetected(FaceDetectionEffect sender, FaceDetectedEventArgs args)
        {
            if (args.ResultFrame.DetectedFaces.Count > 0 && mode.ToLower() == "facedetected" ||
                args.ResultFrame.DetectedFaces.Count == 0 && mode.ToLower() == "facenotdetected")
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    SetActive(true);
                });
            }
            else
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    SetActive(false);
                });
            }
        }

        

    }
}

# Visual State Triggers in Windows 10

Developers writing UWP apps in XAML have the capability to create VisualState triggers that can automatically trigger a VisualState change. One example is the [AdaptiveTrigger](https://msdn.microsoft.com/library/windows/apps/windows.ui.xaml.AdaptiveTrigger.aspx) that can be triggered by changing the size of the window

```XAML
<VisualState>
  <VisualState.StateTriggers>
  <!--VisualState to be triggered when window width is >=720 effective pixels.-->
      <AdaptiveTrigger MinWindowWidth="720" />
  </VisualState.StateTriggers>

  <VisualState.Setters>
      <Setter Target="myPanel.Orientation" Value="Horizontal" />
  </VisualState.Setters>
</VisualState>
```

## PeekABoo

PeekABoo is an app that uses the [StateTriggerBase](https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.xaml.statetriggerbase.aspx) class to create a *PeekTrigger* for detecting when a face is present in the view of the camera

```XAML
<VisualState.StateTriggers>
    <local:PeekTrigger Mode="FaceDetected"></local:PeekTrigger>
</VisualState.StateTriggers>
```

By creating a property *Mode* on the *PeekTrigger* class, we can allow the developer to choose if the trigger is active when a face is detected (FaceDetected) or when a face is not detected (FaceNotDetected).

FaceDetection is done by using the new [FaceDetectionEffect](https://msdn.microsoft.com/en-us/library/windows/apps/windows.media.core.facedetectioneffect.aspx) class in Windows 10 to detect faces in a video stream, in this case the camera.

```c#
var definition = new FaceDetectionEffectDefinition();
definition.SynchronousDetectionEnabled = false;
definition.DetectionMode = FaceDetectionMode.HighPerformance;

_faceDetectionEffect = (FaceDetectionEffect) await _mediaCapture.AddVideoEffectAsync(definition, MediaStreamType.VideoPreview);

// detect 30 frames/second
_faceDetectionEffect.DesiredDetectionInterval = TimeSpan.FromMilliseconds(33);
_faceDetectionEffect.Enabled = true;
_faceDetectionEffect.FaceDetected += FaceDetectionEffect_FaceDetected;
```

We set the trigger to active by using the SetActive(true) method of the StateTriggerBase when the condition is satisfied

```C#
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
}
```


###Images
[Sad dog](https://www.flickr.com/photos/latteda/7201215532/)  
[Happy dog](https://www.flickr.com/photos/edanley/3246241416)

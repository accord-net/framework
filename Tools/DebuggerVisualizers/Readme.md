# DebugVisualizers

#### ImageVisualizers
  - Bitmap Visualizer
  - BitmapData Visualizer
  - UnmanagedImage Visualizer

#### Features
- Ability to view System.Drawing.Image, System.Drawing.Bitmap, Accord.Imaging.UnmanagedImage
- Ability to Copy visualized image to the clipboard
- Ability to Save visualized image to a file on your hard drive in a variety of dotnet formats
- When your cursor is over the image, scrolling the mousewheel will zoom in and out.
- Keypad + or - will zoom in and out
- Number row - will zoom out and = will zoom in
- For *nix afficianados and touch typists, j and k will zoom in and out
- When image needs scrollbars and cursor is not over image, mousewheel will scroll image
- Escape key will dismiss dialog
- Clicking the Github logo will take you to the Accord.Net framework github site
- The following information is available in the status strip
  - Image Width
  - Image Height
  - Image PixelFormat
  - Image Type
  - Image X DPI and Y DPI
  - Curosr X and Y Image coordiate

#### Install Instructions
- Copy the files below to "C:\Users\<username>\Documents\Visual Studio 2017\Visualizers"
    - Accord.DebuggerVisualizers.dll
    - Accord.dll
    - Accord.Imaging.dll
    - Accord.Math.Core.dll
    - Accord.Math.dll
    - Accord.Statistics.dll
    - Accord.Controls.dll
    - Accord.Controls.Imaging.dll
- Once the files are in the Visualizers folder you will see an magnifying glass while debugging on the supported Object types. Click that icon to see the visualizer.
  
###### TODO
- BitmapSource Visualizer
- Integrate all Accord Controls with Test App
# Cyotek ImageBox Change Log

## 1.2
### Changed
* Reworked events to have less overhead
* Switched to semantic versioning

## 1.1.5.1
* **FIX:** Fixes the `FitRectangle` method from increasing the width or height of the source rectangle if `X` or `Y` were negative (generally seen when the `SelectionMode` is set to `Rectangle` and you draw outside the bounds of the image area)
* **FIX:** Resizing the selection in the `ImageBoxEx` control now works correctly even if the mouse passes outside the bounds of the image.

## 1.1.5.0
* **NEW:** Added `AllowUnfocusedMouseWheel` property. If set to `true` the control will support mouse wheel zooming even when it does not have focus
* **FIX:** The `fitToBounds` parameter of the `PointToImage` wasn't being used correctly

## 1.1.4.6
* Zooming in or out using the default keybinds now preserves the center point relative to the new zoom
* Added new overloads to `ZoomIn` and `ZoomOut` to determine if the center point should be preserved
* Added new `MaximumSelectionSize` property to the demonstration `ImageBoxEx` control that allows for more control over default selection
* Fixed the **About** dialog in the demonstration program not loading the default tab correctly
* Refactored zoom handling for mouse and keyboard to avoid duplicate calculations
* Updated copyright year

## 1.1.4.5
* `ImageBoxEx` example control now correctly allows you to move and resize selection regions even when the control is zoomed
* If the `InterpolationMode` property is set to `Default`, the `ImageBox` control will now choose an appropriate mode based on the current zoom levels

## 1.1.4.4
### Changes and new features
* Added new `DrawBackground` virtual method. This allows you to override how the `ImageBox` draws the image background without having to override the entire `OnPaint` method.

## 1.1.4.3
### Changes and new features
* Added a new `TextPadding` property and corresponding `DrawLabel` overload. When this property is set, rendered text from the `Text` and `TextAlign` properties is drawn with the appropriate padding. If the `TextBackColor` property is set to something not fully transparent, then the background will be filled in the original render spot, excluding the padding, allowing for labels with wider background borders.

## 1.1.4.2
### Changes and new features
* `DrawImage` now also ignores `OutOfMemoryException` exceptions
* Removed requirement for .NET 3.5 thanks to a commit from [dahmage](https://github.com/dahmage)
* Added `GetScaledRectangle` overloads using `Point` and `Size` (and the float variant) parameters

### Bug Fixes
* Calling `SelectAll` caused a crash if a backing image wasn't present (even if `VirtualMode` was set)

### Demonstration changes
* Added a new **Resizable Selection** demo. This demo makes use of a subclass of the `ImageBox` to add native dragging and resizing of the region defined by the `SelectionRegion` property.

## 1.1.4.1
### Bug Fixes
* Removed unnecessary `UpdateStyles` calls.
* Changed the `ViewSize` property and `DrawImage` methods to handle disposed images rather than bringing down an entire application.
* Added `TextFormatFlags.NoPadding` to the flags used by `DrawLabel` to avoid a slight gap on left aligned text.
* Fixed a potential crash calling `GetSelectedImage`. Thanks to MutStarburst for finding this bug.

## 1.1.4.0
### Changes and new features
* Added NuGet package
* Added a new `SizeMode` property. This allows you to switch between `Normal`, `Fit` and `Stretch` modes. Stretch is a new mode for the `ImageBox`, and acts similar to existing `Fit` functionality except the aspect ratio is not preserved.
* The `SizeToFit` property has been marked as deprecated and should no longer be used. The `SizeMode` property has a `Fit` value that should be used instead. Setting the `SizeToFit` property will now manipulate `SizeMode` instead.
* Added a license file to hopefully cut down on questions about usage. The `ImageBox` control is licensed under the MIT license, allowing you free reign to use it in your projects, commercial or otherwise. See `license.txt` for the full text.
* Added a new `CenterPoint` property. This property returns the pixel at the center of the current image viewport.
* Added a bunch of missing XML comments documentation.
* Added new overloads for most methods that accepted a source `Rectangle`, `Point` or `Size` to also accept `float` and `int` arguments.
* Added a new `Zoomed` method that uses new `ImageBoxZoomEventArgs` arguments. This new event allows you to tell if the zoom was in or out, how it was raised, and current and previous zoom values. Not hugely thrilled with how aspects of this change has been internally implemented, so implementation methods are private rather than virtual so I can change them without affecting the signature.
* Added new `CenterToImage` method which resets the viewport to be centered of the image, in the same way as zooming via the keyboard used to work.
* Added support for animated GIF's, thanks to a contribution from [Eggy](https://github.com/teamalpha5441). Note animations only play at runtime, not design time.
* The `Text` and `Font` properties are now available and, if set, will be displayed in the control. You can use the `ForeColor`, `TextBackColor`, `TextAlign`, `TextDisplayMode` and `ScaleText` properties to determine how the text will be rendered.
* A new `DrawLabel` method that performs text drawing is available for use by custom implementations or virtual modes.

### Demonstration Changes
* Added a new *Scaled Adornments* demonstration, showing how easy it is to add custom drawing that is scaled and positioned appropriately.
* Added a new *Switch Image During Zoom* demonstration, a demo with an unwieldy name that shows how to switch out a low resolution image with a higher detailed one as you zoom into an `ImageBox`.
* Added new *Text* and *Size Mode* demonstrations.

### Bug Fixes
* Zooming in and out with the keyboard now keeps the view centered to the same pixel that was centered prior to the zoom
* Zooming in and out with the keyboard is now correctly disabled if the `AllowZoom` property is `False`, or the `SizeMode` property is a value other than `Normal`. This means keyboard behaviour now matches mouse behaviour.
* If the mouse wheel was rapidly spun (thus having a multiple of the base delta), the `Zoom` property was only adjusted once
* Setting the `GridScale` property to `None` rendered the default `Small` grid. Using a scale of `None` now correctly just fills the grid area with a solid brush from the `GridColor` property.
* The `MouseWheel` event is now available
* Layout changes no longer occur if the `AllowPainting` property is `false` through use of the `BeginUpdate` method.
* Fixed various documentation errors

## 1.1.3.0
### Changes and new features
* The `Selecting` event now uses `ImageBoxCancelEventArgs` in order to provide further information.
* Added new *DragTestForm* demo
* Added new `Tiny` setting for `ImageBoxGridScale` which is half the size of `Small`.

### Bug Fixes
* If the `Selecting` event was cancelled, it would continue to be re-raised with every movement of the mouse while the button was pressed. Now the event is only raised once, and if cancelled will not be raised again until the button is released and a new drag initiated.

## 1.1.2.2
### Changes and new features
* Changed `PixelGridThreshold` into an instance property, and changed default value to `5`.

## 1.1.2.1
### Changes and new features
* Added missing `GetOffsetRectangle` overload which supports `Rectangle` structs.

### Bug Fixes
* The `ZoomToFit` method didn't support virtual mode and crashed if called.

## 1.1.2.0
### Changes and new features
* Added `IsPointInImage` method. This function returns if a given point is within the image viewport, and is useful for combining with `PointToImage`.
* Added `ImageBorderColor` property, allowing you to customize the color of the image border
* Added a new `ImageBoxBorderStyle`, `FixedSingleGlowShadow`. This style allows for a more smoother outer glow shadow instead of the existing clunky drop shadow.
* Added  `ShowPixelGrid` and `PixelGridColor` properties. When set, a dotted grid is displayed around pixels when zooming in on an image.
* Added new overload to `PointToImage` which allows you to specify if the function should map the given point to the nearest available edge(s) if the point is outside the image boundaries
* Added `AllowDoubleClick` property. When set, the normal double click events and overrides work as expected.
* Additional documentation added via XML comments

### Bug Fixes
* If the `GridDisplayMode` property is set to `Image` an explicit image border is no longer drawn, instead the `ImageBorder` property is correctly honoured.
* Fixes a problem where half the pixels of the first row/column were lost when zooming. Thanks to Rotem for the fix.
* The `GetImageViewport` method now correctly returns a width and height that accounts for control size, padding and zoom levels.
* Fixed incorrect attributes on `AutoSize` property
* Fixes "see also" documentation errors for events

## 1.1.1.0
### Changes and new features
* Added `VirtualMode` and `VirtualSize` properties. These new properties allow you to use all functionality of the ImageBox control without having to set the `Image` property. You can also use the new `VirtualDraw` event to provide custom drawing without having to override existing drawing functionality.

### Bug Fixes
* Fixed the image viewport sometimes being the incorrect size when zoomed in. Thanks to WMJ for the fix.

## 1.1.0.0
### Changes and new features
* Zooming with the mouse is now smoother, and the control attempts to keep the area under the mouse before the zoom in the same area after the zoon.
* Added a `ZoomLevels` property which allows you to configure the different zoom levels supported by the control. Now instead of the control trying to guess the next zoom level, it cycles appropriately through the defined levels. *Currently ZoomLevels (apart from the default series) can only be set at runtime.*
* The `ZoomIncrement` property has been removed due to the introduction of the new zoom levels.
* New `CenterAt` and `ScrollTo` methods allow you to scroll to a given location in the source image.
* Split shortcut handling into two methods `ProcessScrollingShortcuts` for handling arrow keys and `ProcessImageShortcuts` for handling pretty much anything else.
* Added `EnableShortcuts` property, allowing the built in keyboard support to be disabled. When this property is true, `ProcessImageShortcuts` is not called, allowing the control to still be scrolled via the keyboard, but not zoomed etc.
* Zooming can now be performed by the -/+ keys (`OemMinus` and `Oemplus`).
* When zooming (except via mouse action), if the `AutoCenter` property is set, the control will always center the image even when scrollbars are present.
* Nestable `BeginUpdate` and `EndUpdate` methods allow you to disable and enable painting of the control, for example when changing multiple properties at once.
* Added a new `GetSelectedImage` method which creates a new `Bitmap` based on the current selection.
* Added new `FitRectangle` method which takes a given rectangle and ensure it fits within the image boundaries
* The `AllowClickZoom` property now defaults to `false`.
* The `PointToImage` function no longer adds +1 to the result of the function.
* Added a new `ZoomToRegion` method. This will caculate and appropriate zoom level and scrollbar positions to fit a given rectangle.
* Added new `SelectionMode.Zoom`. When this mode is selected, drawing a region will automatically zoom and position the control to fit the region, after which the region is automatically cleared.

### Bug fixes
* Panning no longer tries to activate if no scrollbars are visible
* A new base class, `VirtualScrollableControl` is now used instead of `ScrollableControl`. This removes completely the flicker issues present in previous versions of the control.
* The BorderStyle property has been moved to the `ScrollControl` class, so that borders now correctly surround the control (including scrollbars) rather than just the client area.
* If the `AllowZoomClick` property is `true`, the control no longer magically zooms after panning or selecting a region. Code previously in the `OnMouseClick` override is now in `OnMouseUp`.
* If both `AutoPan` and a valid `SelectionMode` are set, only selections are processed, instead of the control tying to do both. As a result of this fix, setting the `SelectionMode` property no longer resets `AutoPan`
* With the introduction of the `VirtualScrollableControl`, the `MouseWheel` event is now raised as expected.

### Known issues
* The `ScrollProperties` class hasn't been fully integrated with the `ScrollControl`, setting properties on this class won't update the owner control.

## 1.0.0.5
* Initial GitHub release

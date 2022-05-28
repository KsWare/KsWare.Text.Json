# KsWare.Windows.Display

This library provides information for displays (screens, monitors). 
The name is derived von [System.Windows.Forms.Screen](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.screen?view=windowsdesktop-6.0). This is my implementation targeting WPF.

This is a really early stage of development. But is it usable.

- You can set the process or thread DPI behavior.
- You can check if a window fits on the monitor or the whole virtual screen.
- You can get/set window bounds independed of individual monitor scaling using physical pixal. 
- and of course you can get information about a display (size, position, color depth, scaling factor, DPI)

## API
- [DisplayInfo-class](#DisplayInfo-class)
- [WindowExtensions-class](#WindowExtensions-class)
- [VirtualScreen-class](#VirtualScreen-class)  
- [DpiBehavior-class](#DpiBehavior-class)  

Some API methods do not participate in DPI virtualization. 
Then the values are always in terms of physical pixels, and is not related to the calling context. 
In this cases always classes from [System.Drawing namespace](https://docs.microsoft.com/en-us/dotnet/api/system.drawing?view=net-6.0) are used to specify physical pixels.
[System.Drawing.Rectangle](https://docs.microsoft.com/en-us/dotnet/api/system.drawing.rectangle?view=net-6.0),
[System.Drawing.Point](https://docs.microsoft.com/en-us/dotnet/api/system.drawing.point?view=net-6.0),
[System.Drawing.Size](https://docs.microsoft.com/en-us/dotnet/api/system.drawing.size?view=net-6.0).  
In all other casses classes from [System.Window namespace](https://docs.microsoft.com/en-us/dotnet/api/system.windows?view=windowsdesktop-6.0) are used.
[System.Window.Rect](https://docs.microsoft.com/en-us/dotnet/api/system.windows.rect?view=windowsdesktop-6.0),
[System.Window.Point](https://docs.microsoft.com/en-us/dotnet/api/system.windows.point?view=windowsdesktop-6.0),
[System.Window.Size](https://docs.microsoft.com/en-us/dotnet/api/system.windows.size?view=windowsdesktop-6.0)

### DisplayInfo-class

Contains static methods to get instances of DisplayInfo.

The DisplayInfo instance contains information about a specific display/monitor.

### WindowExtensions-class

contains "display" specifig extention methods for the [Window-class](https://docs.microsoft.com/en-us/dotnet/api/system.windows.window?view=windowsdesktop-6.0)

### VirtualScreen-class

| Method | Description |
| :--- | :--- |  
| Bounds | Gets the bounds of the virtual screen.  
| IntersectsWith | Determines if the virtual screen bounds intersects with the rectangular region represented by parameter `rect`. |
| Contains | Determines if the rectangular region represented by parameter `rect` is entirely contained within the rectangular region represented by the virtual screens bounds

### DpiBehavior-class

Gets/Sets DPI awareness context and Dpi hosting behavior.

## Naming mismatch

**KsWare.Screen** assembly and **DisplayInfo** class  
The name "KsWare.Screen" is a working title and subject to change. Microsoft API uses "display". So I named all classes also Display.

**KsWare.Windows.Screen** assembly and **Screen** class  
One could assume that it is a 1:1 implementation of Winfowms [System.Windows.Forms.Screen](https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.screen?view=windowsdesktop-6.0) is, but this is not intended.

**KsWare.Windows.Display** assembly and **DisplayInfo** class  
This seems to be a good name.

**Screen**: a flat surface in a cinema, on a television, or as part of a computer, on which pictures or words are shown

**Screen** In a computer display , the screen is the physical surface on which visual information is presented. This surface is usually made of glass. The screen size is measured from one corner to the opposite corner diagonally.

**Display** A display is a computer output surface and projecting mechanism that shows text and often graphic images to the computer user, using a cathode ray ...
Simple Titler
By Paul Hindt, 4/22/2015

Developed in Visual Studio 2013. Will compile in 2012 though.
Requires Extended WPF Toolkit to build - http://wpftoolkit.codeplex.com/
nuget> Install-Package Extended.Wpf.Toolkit

-

This project was designed with Telestream Wirecast usage in mind.
However it is a generic application with no direct ties to Wirecast.

Telestream Wirecast 6.0.3 and later support the ability to dynamically reload image files from disk.
I thought a titling/lower-third app would be a fun project to help me learn C# and .NET programming.
This program essentially allows creating a simple lower third and exporting it as a PNG image.
Wirecast will dynamically re-load the image each time it is overwritten.

-

Current functionality:
- fixed image size for the title
- Add multiple layers of text with the ability to re-order (like Photoshop layers)
- change font styles and color
- export the current canvas to a PNG image for import to Wirecast 6.0.3 and higher

Goals:
- Ability to import image files as additional layers
- Ability to re-size the title image size/viewport
- Automatic export (i.e. write to disk at user-defined intervals)
- Move text/image layers in the viewport with mouse drag
- Ability to save the current layer configuration as a "document" or config file

Wish List:
- Convert text functionality to DirectWrite instead of relying on WPF font capabilities
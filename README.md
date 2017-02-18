Realistic Grass Rendering - DirectX 11
====
Realistic grass simulation using the geometry shader to render millions of grass blades simultaneously.

<p align="center">
  <img src="https://raw.githubusercontent.com/mreinfurt/Grass-DX11/master/Paper/images/preview2.png">
</p>

The project is using SharpDX with DirectX 11 settings. A video is available on YouTube: https://youtu.be/jxnacUBaG7c

## Installation
- Visual Studio 2013
- Building the project will automatically download all dependencies thanks to NuGet.

## Resources
I mainly used and combined ideas from the following papers (sorted by importance):
* [Edward Lee: Real-Time Grass (Master Thesis)](http://illogictree.com/blog/projects/)
* [Kevin Boulanger: Rendering Grass in Real Time with Dynamic Lighting (PhD Thesis)](http://kevinboulanger.net/grass.html)
* [Markus R. Tillmann: Procedural Rendering of Geometry-Based Grass in Real-Time (Bachelor Thesis)](http://www.bth.se/fou/cuppsats.nsf/all/9b18626fa27d52c9c1257bae002ca00d/$file/BTH2013Tillman.pdf)

However, there are a few more notable articles, mostly describing alternative ways to render grass:
* [GPU Gems: Rendering Countless Blades of Waving Grass](http://http.developer.nvidia.com/GPUGems/gpugems_ch07.html)
* [Outerra: Procedural Grass Rendering](http://outerra.blogspot.cz/2012/05/procedural-grass-rendering.html)

## References
* [Direct3D Reference](http://msdn.microsoft.com/en-us/library/windows/desktop/ff476147(v=vs.85).aspx)
* [HLSL Reference](http://msdn.microsoft.com/en-us/library/windows/desktop/ff471376(v=vs.85).aspx)
* [SharpDX Reference](http://sharpdx.org/documentation/api)

Realistic Grass Rendering - DirectX 11
====
Realistic grass simulation using the geometry shader to render millions of grass blades simultaneously.

<p align="center">
  <img src="https://raw.githubusercontent.com/mreinfurt/Grass-DX11/master/Paper/images/preview2.png">
</p>

The project is using SharpDX with DirectX 11 settings. A video of an older state (early 2015) is available on YouTube: https://youtu.be/Y96cWqCfFH8

## Installation
- Visual Studio 2013
- Building the project will automatically download all dependencies thanks to NuGet.

## Resources
I mainly used and combined ideas from the following papers:
* [Edward Lee: Master’s Thesis: Real-Time Grass](http://illogictree.com/blog/projects/)
* [Kevin Boulanger](http://kevinboulanger.net/grass.html)
* [Tillmann](http://www.bth.se/fou/cuppsats.nsf/all/9b18626fa27d52c9c1257bae002ca00d/$file/BTH2013Tillman.pdf)

However, there are a few more notable articles, mostly describing alternative ways to render grass:
* [GPU Gems](http://http.developer.nvidia.com/GPUGems/gpugems_ch07.html)
* [Grass your world](http://grassyourworld.blogspot.de/)
* [Outerra: Procedural Grass Rendering](http://outerra.blogspot.cz/2012/05/procedural-grass-rendering.html)

## References
* [Direct3D Reference](http://msdn.microsoft.com/en-us/library/windows/desktop/ff476147(v=vs.85).aspx)
* [HLSL Reference](http://msdn.microsoft.com/en-us/library/windows/desktop/ff471376(v=vs.85).aspx)
* [SharpDX Reference](http://sharpdx.org/documentation/api)

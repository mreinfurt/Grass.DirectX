Realistic Grass Rendering - DirectX 11
====
Realistic grass simulation using the geometry shader to render millions of grass blades simultaneously. This was created as part of a masters course on the Hochschule Darmstadt.

![Render example](https://raw.githubusercontent.com/mreinfurt/Grass-DX11/master/Paper/images/final_result_small.png)

The project is using SharpDX with DirectX 11 settings. A video of the current implementation is available on YouTube: https://youtu.be/Y96cWqCfFH8

## Installation
- Visual Studio 2013
- Building the project will automatically download all dependencies thanks to NuGet.

## Resources
I'm mainly using ideas from the following papers:
* [Kevin Boulanger](http://kevinboulanger.net/grass.html)
* [Edward Lee: Master’s Thesis: Real-Time Grass](http://illogictree.com/blog/projects/)
* [Outerra: Procedural Grass Rendering](http://outerra.blogspot.cz/2012/05/procedural-grass-rendering.html)
* [Tillmann](http://www.bth.se/fou/cuppsats.nsf/all/9b18626fa27d52c9c1257bae002ca00d/$file/BTH2013Tillman.pdf)
* [Grass your world](http://grassyourworld.blogspot.de/)

However, there are a few more notable articles, mostly describing different ways to render grass:
* [Blog Article: Summary](http://users.csc.calpoly.edu/~zwood/teaching/csc471/finalprojw12/rsteiger/)
* [GPU Gems](http://http.developer.nvidia.com/GPUGems/gpugems_ch07.html)
* [TU Wien: Instant Animated Grass](http://www.cg.tuwien.ac.at/research/publications/2007/Habel_2007_IAG/)
* [Siggraph](http://www.siggraph.org/s2006/main.php?f=conference&p=sketches&s=6)

## Handy References
* [Direct3D Reference](http://msdn.microsoft.com/en-us/library/windows/desktop/ff476147(v=vs.85).aspx)
* [HLSL Reference](http://msdn.microsoft.com/en-us/library/windows/desktop/ff471376(v=vs.85).aspx)
* [SharpDX Reference](http://sharpdx.org/documentation/api)

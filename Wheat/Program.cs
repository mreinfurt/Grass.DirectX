using System;

namespace GrassRendering
{
    /// <summary>
    /// Simple GrassRenderingApplication application using SharpDX.Toolkit.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
#if NETFX_CORE
        [MTAThread]
#else
        [STAThread]
#endif
        static void Main()
        {
            using (var program = new GrassRenderingApplication())
                program.Run();
        }
    }
}
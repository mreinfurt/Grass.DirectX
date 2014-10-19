using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using Wheat.Components;
using Buffer = SharpDX.Direct3D11.Buffer;
using Device = SharpDX.Direct3D11.Device;

namespace Wheat
{
    class Game
    {
        #region Fields

        // DirectX
        private RenderForm _renderForm;
        private SwapChain _swapChain;
        private SwapChainDescription _swapChainDescription;
        private Device _device;
        private DeviceContext _context;
        private Factory _factory;

        // Shader
        private ShaderBytecode _vertexShaderByteCode;
        private VertexShader _vertexShader;
        private ShaderBytecode _pixelShaderByteCode;
        private PixelShader _pixelShader;
        private ShaderSignature _signature;
        private InputLayout _inputLayout;

        // Object
        private Buffer _vertices;
        private Buffer _constantBuffer;

        // Components
        private Camera _camera;

        #endregion

        #region Public Methods        
        /// <summary>
        /// Initializes a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            _renderForm = new RenderForm("COGR - Wheat");

            // SwapChain description
            _swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription =
                    new ModeDescription(_renderForm.ClientSize.Width, _renderForm.ClientSize.Height,
                                        new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = _renderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            // Create Device and SwapChain
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, _swapChainDescription, out _device, out _swapChain);
            _context = _device.ImmediateContext;

            // Ignore all windows events
            _factory = _swapChain.GetParent<Factory>();
            _factory.MakeWindowAssociation(_renderForm.Handle, WindowAssociationFlags.IgnoreAll);
        
            _camera = new Camera(_renderForm.ClientSize.Width, _renderForm.ClientSize.Height);
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            this.SetUpShader();
            this.SetUpObject();
            this.EnableDebug();
        }

        public void Run()
        {

            // Use clock
            var clock = new Stopwatch();
            clock.Start();

            // Declare texture for rendering
            bool userResized = true;
            Texture2D backBuffer = null;
            RenderTargetView renderView = null;
            Texture2D depthBuffer = null;
            DepthStencilView depthView = null;

            // Setup handler on resize form
            _renderForm.UserResized += (sender, args) => userResized = true;

            // Setup full screen mode change F5 (Full) F4 (Window)
            _renderForm.KeyUp += (sender, args) =>
            {
                if (args.KeyCode == Keys.F5)
                    _swapChain.SetFullscreenState(true, null);
                else if (args.KeyCode == Keys.F4)
                    _swapChain.SetFullscreenState(false, null);
                else if (args.KeyCode == Keys.Escape)
                    _renderForm.Close();
            };

            // Prepare All the stages
            _context.InputAssembler.InputLayout = _inputLayout;
            _context.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            _context.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertices, Utilities.SizeOf<Vector4>() * 2, 0));
            _context.VertexShader.SetConstantBuffer(0, _constantBuffer);
            _context.VertexShader.Set(_vertexShader);
            _context.PixelShader.Set(_pixelShader);

            // Main loop
            RenderLoop.Run(_renderForm, () =>
            {
                // If Form resized
                if (userResized)
                {
                    // Dispose all previous allocated resources
                    Utilities.Dispose(ref backBuffer);
                    Utilities.Dispose(ref renderView);
                    Utilities.Dispose(ref depthBuffer);
                    Utilities.Dispose(ref depthView);

                    // Resize the backbuffer
                    _swapChain.ResizeBuffers(_swapChainDescription.BufferCount, _renderForm.ClientSize.Width, _renderForm.ClientSize.Height, Format.Unknown, SwapChainFlags.None);

                    // Get the backbuffer from the swapchain
                    backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);

                    // Renderview on the backbuffer
                    renderView = new RenderTargetView(_device, backBuffer);

                    // Create the depth buffer
                    depthBuffer = new Texture2D(_device, new Texture2DDescription()
                    {
                        Format = Format.D32_Float_S8X24_UInt,
                        ArraySize = 1,
                        MipLevels = 1,
                        Width = _renderForm.ClientSize.Width,
                        Height = _renderForm.ClientSize.Height,
                        SampleDescription = new SampleDescription(1, 0),
                        Usage = ResourceUsage.Default,
                        BindFlags = BindFlags.DepthStencil,
                        CpuAccessFlags = CpuAccessFlags.None,
                        OptionFlags = ResourceOptionFlags.None
                    });

                    // Create the depth buffer view
                    depthView = new DepthStencilView(_device, depthBuffer);

                    // Setup targets and viewport for rendering
                    _context.Rasterizer.SetViewport(new Viewport(0, 0, _renderForm.ClientSize.Width, _renderForm.ClientSize.Height, 0.0f, 1.0f));
                    _context.OutputMerger.SetTargets(depthView, renderView);

                    // Setup new projection matrix with correct aspect ratio
                    _camera.Resize(_renderForm.ClientSize.Width, _renderForm.ClientSize.Height);

                    // We are done resizing
                    userResized = false;
                }

                var time = clock.ElapsedMilliseconds / 1000.0f;

                var viewProj = Matrix.Multiply(_camera.ViewMatrix, _camera.ProjectionMatrix);

                // Clear views
                _context.ClearDepthStencilView(depthView, DepthStencilClearFlags.Depth, 1.0f, 0);
                _context.ClearRenderTargetView(renderView, Color.Black);

                _camera.Update(time);

                // Update WorldViewProj Matrix
                //var worldViewProj = Matrix.RotationX(time) * Matrix.RotationY(time * 2) * Matrix.RotationZ(time * .7f) * viewProj;
                var worldViewProj = Matrix.Identity*viewProj;
                worldViewProj.Transpose();
                _context.UpdateSubresource(ref worldViewProj, _constantBuffer);

                // Draw the cube
                _context.Draw(36, 0);

                // Present!
                _swapChain.Present(0, PresentFlags.None);
            });

            depthBuffer.Dispose();
            depthView.Dispose();
            renderView.Dispose();
            backBuffer.Dispose();

            this.Dispose();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            // Release all resources
            _signature.Dispose();
            _vertexShaderByteCode.Dispose();
            _vertexShader.Dispose();
            _pixelShaderByteCode.Dispose();
            _pixelShader.Dispose();
            _vertices.Dispose();
            _inputLayout.Dispose();
            _constantBuffer.Dispose();
            _context.ClearState();
            _context.Flush();
            _device.Dispose();
            _context.Dispose();
            _swapChain.Dispose();
            _factory.Dispose();
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Enables debug options.
        /// </summary>
        private void EnableDebug()
        {
            // Used for debugging dispose object references
            Configuration.EnableObjectTracking = true;

            // Disable throws on shader compilation errors
            Configuration.ThrowOnShaderCompileError = true;
        }

        /// <summary>
        /// Set up shaders.
        /// </summary>
        private void SetUpShader()
        {
            // Compile Vertex and Pixel shaders
            _vertexShaderByteCode = ShaderBytecode.CompileFromFile("Content/Effects/MiniCube.fx", "VS", "vs_4_0");
            _vertexShader = new VertexShader(_device, _vertexShaderByteCode);

            _pixelShaderByteCode = ShaderBytecode.CompileFromFile("Content/Effects/MiniCube.fx", "PS", "ps_4_0");
            _pixelShader = new PixelShader(_device, _pixelShaderByteCode);

            _signature = ShaderSignature.GetInputSignature(_vertexShaderByteCode);

            // Layout from VertexShader input signature
            _inputLayout = new InputLayout(_device, _signature, new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
                    });

            // Create Constant Buffer
            _constantBuffer = new Buffer(_device, Utilities.SizeOf<Matrix>(), ResourceUsage.Default, BindFlags.ConstantBuffer, CpuAccessFlags.None, ResourceOptionFlags.None, 0);

        }

        /// <summary>
        /// Set up the object.
        /// </summary>
        private void SetUpObject()
        {
            // Instantiate Vertex buiffer from vertex data
            _vertices = Buffer.Create(_device, BindFlags.VertexBuffer, new[]
                                  {
                                      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f), // Front
                                      new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f),

                                      new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f), // BACK
                                      new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f),

                                      new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f), // Top
                                      new Vector4(-1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, 1.0f,  1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, 1.0f, -1.0f,  1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f),

                                      new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f), // Bottom
                                      new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4(-1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,-1.0f, -1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),
                                      new Vector4( 1.0f,-1.0f,  1.0f,  1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f),

                                      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f), // Left
                                      new Vector4(-1.0f, -1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f, -1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f,  1.0f,  1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),
                                      new Vector4(-1.0f,  1.0f, -1.0f, 1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f),

                                      new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f), // Right
                                      new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, -1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f, -1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f, -1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                                      new Vector4( 1.0f,  1.0f,  1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f),
                            });
        }
        #endregion
    }
}

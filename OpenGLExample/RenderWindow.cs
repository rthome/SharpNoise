using OpenGLExample.Properties;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OpenGLExample
{
    class RenderWindow : GameWindow
    {
        const int PrimitiveRestart = 12345678;

        int ProgramHandle;
        Matrix4 ModelviewMatrix, ProjectionMatrix;

        #region Shader Loading

        int LoadShaderFromResource(ShaderType shaderType, string resourceName)
        {
            // Load shader from WinForms resource manager thing
            var shaderBytes = (byte[])Resources.ResourceManager.GetObject(resourceName);
            var shaderSource = Encoding.UTF8.GetString(shaderBytes);

            // Create and compile shader
            var shaderHandle = GL.CreateShader(shaderType);
            GL.ShaderSource(shaderHandle, shaderSource);
            GL.CompileShader(shaderHandle);

            // Check for compilation errors
            int status;
            GL.GetShader(shaderHandle, ShaderParameter.CompileStatus, out status);
            if (status == 0)
            {
                var infoLog = GL.GetShaderInfoLog(shaderHandle);
                Debug.Print("Compile failed for shader {0}: {1}", resourceName, infoLog);
            }

            return shaderHandle;
        }

        int CreateAndLinkProgram(params int[] shaders)
        {
            // Create program, attach shaders, link
            var programHandle = GL.CreateProgram();
            foreach (var shader in shaders)
                GL.AttachShader(programHandle, shader);
            GL.LinkProgram(programHandle);

            // Check for link errors
            int status;
            GL.GetProgram(programHandle, GetProgramParameterName.LinkStatus, out status);
            if (status == 0)
            {
                var infoLog = GL.GetProgramInfoLog(programHandle);
                Debug.Print("Link for shader program failed: {0}", infoLog);
            }

            // Delete shaders
            foreach (var shader in shaders)
                GL.DeleteShader(shader);
            return programHandle;
        }

        #endregion

        #region Buffer setup

        int CreatePlaneDataBuffer()
        {
            // # VBOs: positions, indices
            var buffers = new int[2];
            GL.GenBuffers(2, buffers);

            // Position data
            var positions = new Vector3[100];
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    positions[y * 10 + x] = new Vector3(x, y, 0);
                }
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, buffers[0]);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Vector3.SizeInBytes * positions.Length), positions, BufferUsageHint.StaticDraw);

            // Indices
            // Triangulate like this:
            // 
            // x   x   x   x
            // |1 /|3 /|5 /|
            // | / | / | / |
            // |/ 2|/ 4|/ 6|
            // x   x   x   x
            // |7 /|9 /|  /|
            // | / | / | / |
            // |/ 8|/  |/  |
            // x   x   x   x
            //
            var indices = new List<int>();
            for(int row = 0; row < 9; row++)
            {
                indices.Add(row * 10);
                indices.Add((row + 1) * 10);
                for (int x = 0; x < 9; x++)
                {
                    indices.Add(row * 10 + x + 1);
                    indices.Add((row + 1) * 10 + x + 1);
                }
                indices.Add(PrimitiveRestart);
            }

            // Set up VAO
            var vertexArrayObject = GL.GenVertexArray();

            return vertexArrayObject;
        }

        #endregion

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(ClientRectangle);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, ClientSize.Width / (float)ClientSize.Height, 0.1f, 10);
        }

        protected override void OnLoad(EventArgs e)
        {
            ModelviewMatrix = Matrix4.Identity;
            GL.ClearColor(Color4.Gray);
            GL.PrimitiveRestartIndex(PrimitiveRestart);

            // Load shaders
            var vertexHandle = LoadShaderFromResource(ShaderType.VertexShader, "vertexShader");
            var fragmentHandle = LoadShaderFromResource(ShaderType.FragmentShader, "fragmentShader");
            ProgramHandle = CreateAndLinkProgram(vertexHandle, fragmentHandle);
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {

        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(ProgramHandle);
            GL.UseProgram(0);

            SwapBuffers();
        }

        public RenderWindow()
            : base(800, 600, GraphicsMode.Default, "SharpNoise OpenGL Example",
            GameWindowFlags.Default, DisplayDevice.Default, 3, 3,
            GraphicsContextFlags.ForwardCompatible)
        {
            VSync = VSyncMode.Adaptive;
        }
    }
}

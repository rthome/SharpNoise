using OpenGLExample.Properties;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SharpNoise;
using SharpNoise.Builders;
using SharpNoise.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace OpenGLExample
{
    class RenderWindow : GameWindow
    {
        const int LatitudeBands = 50;
        const int LongitudeBands = 100;
        const float SphereRadius = 1.6f;

        int ProgramHandle;
        Matrix4 ModelMatrix, ViewMatrix, ProjectionMatrix, MvpMatrix;
        int MvpUniformLocation;

        int VertexArrayObject;
        int VertexBuffer, NormalBuffer, ElevationBuffer, IndexBuffer;
        int ElementCount;

        Vector3[] Positions, Normals;
        int[] Indices;

        TranslatePoint TimeTranslator;
        NoiseMap NoiseMap;
        PlaneNoiseMapBuilder NoiseMapBuilder;

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

        void CreateVertexData()
        {
            var positions = new List<Vector3>();
            var normals = new List<Vector3>();
            var indices = new List<int>();

            for (double latitudeNum = 0; latitudeNum <= LatitudeBands; latitudeNum++)
            {
                var theta = latitudeNum * MathHelper.Pi / LatitudeBands;
                var sinTheta = Math.Sin(theta);
                var cosTheta = Math.Cos(theta);

                for (double longitudeNum = 0; longitudeNum <= LongitudeBands; longitudeNum++)
                {
                    var phi = longitudeNum * MathHelper.TwoPi / LongitudeBands;
                    var sinPhi = Math.Sin(phi);
                    var cosPhi = Math.Cos(phi);

                    var x = cosPhi * sinTheta;
                    var y = sinPhi * sinTheta;
                    var z = cosTheta;

                    var normal = new Vector3((float)x, (float)y, (float)z);
                    var position = normal * SphereRadius;
                    normals.Add(normal);
                    positions.Add(position);
                }
            }

            for (int latitudeNum = 0; latitudeNum < LatitudeBands; latitudeNum++)
            {
                for (int longitudeNum = 0; longitudeNum <= LongitudeBands; longitudeNum++)
                {
                    var i0 = (latitudeNum * (LongitudeBands + 1)) + longitudeNum;
                    var i1 = i0 + LongitudeBands + 1;

                    indices.Add(i0);
                    indices.Add(i1);
                    indices.Add(i0 + 1);

                    indices.Add(i1);
                    indices.Add(i1 + 1);
                    indices.Add(i0 + 1);
                }
            }

            Positions = positions.ToArray();
            Normals = normals.ToArray();
            Indices = indices.ToArray();
            ElementCount = Indices.Length;
        }

        void SetupBuffers()
        {
            // Generate VBOs
            VertexBuffer = GL.GenBuffer();
            NormalBuffer = GL.GenBuffer();
            ElevationBuffer = GL.GenBuffer();
            IndexBuffer = GL.GenBuffer();

            // Create positions, normals, and indices
            CreateVertexData();

            // positions
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Positions.Length * Vector3.SizeInBytes), Positions, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // normals, no data for now
            GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Normals.Length * Vector3.SizeInBytes), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // elevation, no data for now
            GL.BindBuffer(BufferTarget.ArrayBuffer, ElevationBuffer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Positions.Length * sizeof(float)), IntPtr.Zero, BufferUsageHint.StreamDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // indices
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(int)), Indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Create and set up VAO
            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);
            {
                // positions, located at attribute index 0
                GL.EnableVertexAttribArray(0);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

                // normals, located at atteibute location 1
                GL.EnableVertexAttribArray(1);
                GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBuffer);
                GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);

                // elevation, located at attribute index 2
                GL.EnableVertexAttribArray(2);
                GL.BindBuffer(BufferTarget.ArrayBuffer, ElevationBuffer);
                GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 0, 0);
            }
            GL.BindVertexArray(0);
        }

        #endregion

        void UpdateTriangleNormals(Vector3[] positions, int[] indices, float[] elevation)
        {
            // Do nothing
        }

        void GenerateElevationNoise(double timeDelta)
        {
            TimeTranslator.YTranslation += timeDelta;
            NoiseMapBuilder.Build();
        }

        void SetupNoiseMapBuilder()
        {
            // Set up noise module tree
            TimeTranslator = new TranslatePoint
            {
                Source0 = new ScaleBias
                {
                    Scale = 0.8,
                    Bias = 0,
                    Source0 = new ScalePoint
                    {
                        XScale = 0.0375,
                        ZScale = 0.0375,
                        YScale = 0.625,
                        Source0 = new Billow(),
                    },
                },
            };

            // Set up target noise map and noise map builder
            NoiseMap = new NoiseMap();
            NoiseMapBuilder = new PlaneNoiseMapBuilder
            {
                DestNoiseMap = NoiseMap,
                SourceModule = TimeTranslator,
            };
            NoiseMapBuilder.SetBounds(0, LongitudeBands, 0, LatitudeBands);
            NoiseMapBuilder.SetDestSize(LongitudeBands, LatitudeBands);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(ClientRectangle);
            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, ClientSize.Width / (float)ClientSize.Height, 0.1f, 1000);
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color4.Gray);

            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.CullFace(CullFaceMode.Back);

            // Load shaders
            var vertexHandle = LoadShaderFromResource(ShaderType.VertexShader, "vertexShader");
            var fragmentHandle = LoadShaderFromResource(ShaderType.FragmentShader, "fragmentShader");
            ProgramHandle = CreateAndLinkProgram(vertexHandle, fragmentHandle);
            MvpUniformLocation = GL.GetUniformLocation(ProgramHandle, "MVP");

            // Create plane data and set up buffers
            SetupBuffers();

            // Initialize model and view matrices once
            ViewMatrix = Matrix4.LookAt(new Vector3(7, 0, 0), Vector3.Zero, Vector3.UnitZ);
            ModelMatrix = Matrix4.CreateScale(1.0f);

            // Set up noise module
            SetupNoiseMapBuilder();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            // Update elevation data
            GenerateElevationNoise(e.Time);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ElevationBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(NoiseMap.Data.Length * sizeof(float)), NoiseMap.Data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            // Update normals
            UpdateTriangleNormals(Positions, Indices, NoiseMap.Data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBuffer);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, (IntPtr)(Normals.Length * Vector3.SizeInBytes), Normals);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            var error = GL.GetError();
            if (error != ErrorCode.NoError)
                Debug.Print("OpenGL error: " + error.ToString());

            // Rotate the plane
            var modelRotation = Matrix4.CreateRotationZ((float)(e.Time));
            Matrix4.Mult(ref ModelMatrix, ref modelRotation, out ModelMatrix);

            // Update MVP matrix
            Matrix4 mvMatrix;
            Matrix4.Mult(ref ModelMatrix, ref ViewMatrix, out mvMatrix);
            Matrix4.Mult(ref mvMatrix, ref ProjectionMatrix, out MvpMatrix);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(ProgramHandle);
            GL.BindVertexArray(VertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);

            GL.UniformMatrix4(MvpUniformLocation, false, ref MvpMatrix);
            GL.DrawElements(PrimitiveType.Triangles, ElementCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            SwapBuffers();
        }

        public RenderWindow()
            : base(800, 600, GraphicsMode.Default, "SharpNoise OpenGL Example",
            GameWindowFlags.Default, DisplayDevice.Default, 3, 3, GraphicsContextFlags.ForwardCompatible)
        {
            VSync = VSyncMode.Off;
        }
    }
}

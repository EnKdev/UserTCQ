using UserTCQ.Engine.Types;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.ComponentModel;
using System.Diagnostics;

namespace UserTCQ.Engine.Rendering
{
    public static class Time
    {
        public static float deltaTime = 0.16f;
        public static float deltaTimeUnscaled = 0.16f;
        public static float timeScale = 1f;
    }

    public enum VSyncMode
    {
        None = 0,
        VSync = 1
    }

    public class MainWindow : GameWindow
    {
        public static MainWindow instance = null;

        public readonly Matrix4 projectMat = Matrix4.CreateOrthographicOffCenter(-16, 16, -9, 9, -1, 200);
        public readonly Matrix4 guiProjectMat = Matrix4.CreateOrthographicOffCenter(-1280, 1280, -720, 720, -1, 200);

        public PostProcessor postProcessor;

        public event Action earlyUpdateEvent;
        public event Action updateEvent;
        public event Action lateUpdateEvent;
        public event Action renderEvent;
        public event Action guiEvent;
        public event Action closingEvent;

        public VSyncMode vSyncMode
        {
            set
            {
                if (value == VSyncMode.None)
                    GLFW.SwapInterval(0);
                else
                    GLFW.SwapInterval(1);

                vSyncMode = value;
            }

            get
            {
                return vSyncMode;
            }
        }

        private Action initializeCallback;

        public MainWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, Action initializeCallback) : base(gameWindowSettings, nativeWindowSettings)
        {
            if (instance != null)
            {
                Debug.WriteLine("Another instance is running!");
                return;
            }

            instance = this;

            this.initializeCallback = initializeCallback;
        }

        public override void Run()
        {
            base.Run();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            postProcessor.Dispose();
            closingEvent?.Invoke();

            base.OnClosing(e);
        }

        private const float ratio = 16 / 9f;
        private int viewportWidth = 1600, viewportHeight = 900, viewportX = 0, viewportY = 0;

        protected override void OnResize(ResizeEventArgs e)
        {
            viewportWidth = e.Width / (float)e.Height < ratio ? e.Width : (int)(e.Height * ratio);
            viewportHeight = e.Width / (float)e.Height < ratio ? (int)(e.Width / ratio) : e.Height;
            viewportX = (int)((e.Width - viewportWidth) / 2f);
            viewportY = (int)((e.Height - viewportHeight) / 2f);

            GL.Viewport(viewportX, viewportY, viewportWidth, viewportHeight);

            postProcessor.Update(viewportX, viewportY, e.Width, e.Height);

            base.OnResize(e);
        }

        protected override void OnLoad()
        {
            GL.Enable(EnableCap.Texture2D);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            Shader.InitDefaults();

            postProcessor = new PostProcessor();
            postProcessor.Init(viewportWidth, viewportHeight);

            initializeCallback?.Invoke();

            base.OnLoad();
        }

        private float lastTime = 0;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            Update();

            base.OnUpdateFrame(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            Draw();

            base.OnRenderFrame(e);
        }

        private void Update()
        {
            lastTime = (float)GLFW.GetTime();

            earlyUpdateEvent?.Invoke();
            updateEvent?.Invoke();

            lateUpdateEvent?.Invoke();

            GameObject.instances.Sort(new ByZPosition());
        }

        private void Draw()
        {
            postProcessor.Begin();

            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            RenderObjects();

            postProcessor.End();
            postProcessor.Render();

            RenderGUI();

            SwapBuffers();

            Time.deltaTimeUnscaled = (float)GLFW.GetTime() - lastTime;
            Time.deltaTime = Time.deltaTimeUnscaled * Time.timeScale;
        }

        private void RenderObjects()
        {
            foreach (var rendering in GameObject.instances)
            {
                rendering.Render();
            }

            renderEvent?.Invoke();
        }
        private void RenderGUI()
        {
            foreach (var rendering in GameObject.instances)
            {
                rendering.RenderGUI();
            }

            guiEvent?.Invoke();
        }
    }
}

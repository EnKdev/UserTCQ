using UserTCQ.Engine.Rendering;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System.Collections.Generic;

namespace UserTCQ.Engine.Types
{
    public class ByZPosition : IComparer<GameObject>
    {
        public int Compare(GameObject x, GameObject y)
        {
            return x.position.Z.CompareTo(y.position.Z);
        }
    }

    public enum RenderLayer
    {
        WorldLayer,
        GUILayer
    }

    public class GameObject : Behaviour
    {
        public static List<GameObject> instances = new List<GameObject>();

        public string name;

        public Vector3 position = new Vector3(0, 0, 0);
        public Vector3 scale = new Vector3(1, 1, 1);
        public Vector3 rotationEuler = new Vector3(0, 0, 0);

        public Color4 color = Color4.White;

        public RenderLayer renderLayer = RenderLayer.WorldLayer;

        public VertexArray vertexArray;
        public Shader shader;
        public Texture texture;

        public bool enabled;

        public float width = 1f, height = 1f;

        protected float[] vertBuffer = new float[20] {
            -1, -1, 0, 0, 1,
            1, -1, 0, 1, 1,
            1, 1, 0,  1, 0,
            -1, 1, 0, 0, 0
        };

        protected uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private bool initialized = false;

        private List<Component> components = new List<Component>();

        public GameObject(string name)
        {
            this.name = name;
            instances.Add(this);

            shader = Shader.defaultShader;
        }

        protected override void Start()
        {
            MainWindow.instance.updateEvent += Update;
            MainWindow.instance.lateUpdateEvent += LateUpdate;
            MainWindow.instance.closingEvent += OnClosing;
        }

        public void SetActive(bool active)
        {
            enabled = active;
        }

        public T AddComponent<T>(T component) where T : Component
        {
            components.Add(component);
            component.gameObject = this;

            if (initialized)
                component.Start();

            return component;
        }

        public T GetComponent<T>() where T : Component
        {
            return (T)components.Find(x => x.GetType().Equals(typeof(T)));
        }

        public void RemoveComponent<T>() where T : Component
        {
            var component = components.Find(x => x.GetType().Equals(typeof(T)));
            component.OnDestroy();
            components.Remove(component);
        }

        protected override void Update()
        {
            if (!enabled)
                return;

            foreach (var component in components)
            {
                if (component.active)
                    component.Update();
            }
        }

        protected override void LateUpdate()
        {
            if (!enabled)
                return;

            foreach (var component in components)
            {
                if (component.active)
                    component.LateUpdate();
            }
        }

        public void Init(float width, float height)
        {
            this.width = width;
            this.height = height;

            vertexArray = new VertexArray(vertBuffer, indices);
            vertexArray.Init();
            
            foreach (Component component in components)
            {
                component.Start();
            }
            initialized = true;
        }

        public override void Dispose()
        {
            MainWindow.instance.updateEvent -= Update;
            MainWindow.instance.lateUpdateEvent -= LateUpdate;
            MainWindow.instance.closingEvent -= OnClosing;

            enabled = false;
            foreach (var component in components)
            {
                component.OnDestroy();
            }
            components.Clear();
            instances.Remove(this);
        }

        public virtual void Render()
        {
            if (!enabled)
                return;

            if (renderLayer != RenderLayer.WorldLayer)
                return;

            GL.BindVertexArray(vertexArray.VAO);

            Matrix4 model = Matrix4.Identity *
                Matrix4.CreateScale(scale * new Vector3(width / 2f, height / 2f, 1.0f)) *
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotationEuler)) *
                Matrix4.CreateTranslation(position);

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Use();

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("projection", MainWindow.instance.projectMat);
            shader.SetVector4("color", new Vector4(color.R, color.G, color.B, color.A));
            shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public virtual void RenderGUI()
        {
            if (!enabled)
                return;

            if (renderLayer != RenderLayer.GUILayer)
                return;

            GL.BindVertexArray(vertexArray.VAO);

            Matrix4 model = Matrix4.Identity *
                Matrix4.CreateScale(scale) *
                Matrix4.CreateFromQuaternion(Quaternion.FromEulerAngles(rotationEuler)) *
                Matrix4.CreateTranslation(position);

            GL.ActiveTexture(TextureUnit.Texture0);
            texture.Use();

            shader.SetMatrix4("model", model);
            shader.SetMatrix4("projection", MainWindow.instance.guiProjectMat);
            shader.SetVector4("color", new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f));
            shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }
    }
}

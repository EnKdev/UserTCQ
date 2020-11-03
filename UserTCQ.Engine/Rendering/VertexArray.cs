using UserTCQ.Engine.Types;
using OpenTK.Graphics.OpenGL;

namespace UserTCQ.Engine.Rendering
{
    public class VertexArray : Behaviour
    {
        public float[] vertexBuffer;
        public uint[] indices;

        public Shader shader = Shader.defaultShader;

        public int VBO, EBO, VAO;

        public VertexArray(float[] vertexBuffer, uint[] indices)
        {
            this.vertexBuffer = vertexBuffer;
            this.indices = indices;
        }

        protected override void Start()
        {
            MainWindow.instance.closingEvent += Dispose;
        }

        public VertexArray Init()
        {
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBuffer.Length * sizeof(float), vertexBuffer, BufferUsageHint.DynamicDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            int vertLoc = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertLoc);
            GL.VertexAttribPointer(vertLoc, 3, VertexAttribPointerType.Float, false, sizeof(float) * 5, 0);

            int texCoordLoc = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLoc);
            GL.VertexAttribPointer(texCoordLoc, 2, VertexAttribPointerType.Float, false, sizeof(float) * 5, sizeof(float) * 3);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);

            return this;
        }

        public override void Dispose()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteVertexArray(VAO);
        }
    }
}

using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;

namespace UserTCQ.Engine.Rendering
{
    public class PostProcessor : IDisposable
    {
        static string vertDefault = "#version 330 core\nlayout(location = 0) in vec3 aPosition;layout(location = 1) in vec2 aTexCoord;out vec2 texCoord;uniform mat4 model;uniform mat4 projection;\nvoid main(void){texCoord = aTexCoord;gl_Position = vec4(aPosition, 1.0) * model * projection;}";
        static string fragDefault = "#version 330\nuniform vec4 color;out vec4 outputColor;in vec2 texCoord;uniform sampler2D texture0;void main(){outputColor = texture(texture0, texCoord) * color;}";

        public Shader shader = new Shader(vertDefault, fragDefault);

        private int VBO, EBO, VAO, FBO, Texture;

        private uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private float[] vertBuffer = new float[20] {
                -1280, -720, 0, 0, 0,
                 1280, -720, 0, 1, 0,
                 1280,  720, 0, 1, 1,
                -1280,  720, 0, 0, 1
        };

        private float[] borderColor = { 0.0f, 0.0f, 0.0f, 1.0f };

        public void Init(int width, int height)
        {
            FBO = GL.GenFramebuffer();
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);

            Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, new byte[width * height * 4]);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, Texture, 0);
            GL.DrawBuffer(DrawBufferMode.ColorAttachment0);

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);

            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertBuffer.Length * sizeof(float), vertBuffer, BufferUsageHint.DynamicDraw);

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
        }

        public void Begin()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, FBO);
        }

        public void End()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Render()
        {
            GL.BindVertexArray(VAO);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Texture);

            shader.SetMatrix4("model", Matrix4.Identity);
            shader.SetMatrix4("projection", MainWindow.instance.guiProjectMat);
            shader.SetVector4("color", new Vector4(1, 1, 1, 1));
            shader.Use();

            GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Update(int x, int y, int width, int height)
        {
            GL.BindTexture(TextureTarget.Texture2D, Texture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, new byte[width * height * 4]);
            GL.BindTexture(TextureTarget.Texture2D, 0);

            float xLeftTexCoord = (float)x / width;
            float yLeftTexCoord = (float)y / height;
            float xRightTexCoord = 1.0f - xLeftTexCoord;
            float yRightTexCoord = 1.0f - yLeftTexCoord;

            vertBuffer[3] = xLeftTexCoord; vertBuffer[4] = yLeftTexCoord;
            vertBuffer[8] = xRightTexCoord; vertBuffer[9] = yLeftTexCoord;
            vertBuffer[13] = xRightTexCoord; vertBuffer[14] = yRightTexCoord;
            vertBuffer[18] = xLeftTexCoord; vertBuffer[19] = yRightTexCoord;

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertBuffer.Length * sizeof(float), vertBuffer, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.DynamicDraw);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Texture);
            GL.DeleteFramebuffer(FBO);

            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
            GL.DeleteVertexArray(VAO);
        }
    }
}

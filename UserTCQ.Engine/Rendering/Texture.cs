using UserTCQ.Engine.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace UserTCQ.Engine.Rendering
{
    public class Texture : Behaviour
    {
        public int Handle;

        public Texture FromFile(string path)
        {
            Bitmap bitmap = new Bitmap(path);
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            FromIntPtr(bitmap.Width, bitmap.Height, data.Scan0);
            bitmap.UnlockBits(data);
            return this;
        }

        public Texture FromBitmap(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            FromIntPtr(bitmap.Width, bitmap.Height, data.Scan0);
            bitmap.UnlockBits(data);
            return this;
        }

        public Texture FromByteArray(int width, int height, byte[] data, PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra, PixelType pixelType = PixelType.UnsignedByte)
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, pixelInternalFormat, width, height, 0, format, pixelType, data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return this;
        }

        public Texture FromIntPtr(int width, int height, IntPtr data, PixelInternalFormat pixelInternalFormat = PixelInternalFormat.Rgba, PixelFormat format = PixelFormat.Bgra, PixelType pixelType = PixelType.UnsignedByte)
        {
            Handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, Handle);
            GL.TexImage2D(TextureTarget.Texture2D, 0, pixelInternalFormat, width, height, 0, format, pixelType, data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.BindTexture(TextureTarget.Texture2D, 0);
            return this;
        }

        public void Use()
        {
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public override void Dispose()
        {
            base.Dispose();
            GL.DeleteTexture(Handle);
        }
    }
}

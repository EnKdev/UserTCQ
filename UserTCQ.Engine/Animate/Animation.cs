using UserTCQ.Engine.Rendering;
using UserTCQ.Engine.Types;
using System.Drawing;

namespace UserTCQ.Engine.Animate
{
    public class Animation : Behaviour
    {
        public Texture[] frames;
        public int frameRate = 60;
        public int frameCount;

        public bool looping = true;

        public Animation(Bitmap[] frames)
        {
            this.frames = new Texture[frames.Length];
            for (int i = 0; i < this.frames.Length; i++)
            {
                this.frames[i] = new Texture().FromBitmap(frames[i]);
            }
            frameCount = frames.Length;
        }

        public override void Dispose()
        {
            foreach (var frame in frames)
            {
                frame.Dispose();
            }
        }
    }
}

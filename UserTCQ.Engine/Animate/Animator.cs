using UserTCQ.Engine.Rendering;
using UserTCQ.Engine.Types;
using System;

namespace UserTCQ.Engine.Animate
{
    public class Animator : Component
    {
        private float time;
        private Animation animation;
        private int frameIndex;

        Action animationComplete;

        public void SetAnimation(Animation animation)
        {
            this.animation = animation;
            gameObject.texture = animation.frames[0];
        }

        public void AnimationCompleteCallback(Action callback)
        {
            animationComplete = callback;
        }

        public void Seek(int frame)
        {
            time = (float)frame / animation.frameRate;
        }

        public override void Update()
        {
            time += Time.deltaTime;

            if (animation.looping)
            {
                frameIndex = (int)(time * animation.frameRate) % animation.frameCount;
            }
            else
            {
                frameIndex = Math.Clamp((int)(time * animation.frameRate), 0, animation.frameCount);
                if (frameIndex == animation.frameCount - 1)
                {
                    if (animationComplete != null)
                        animationComplete?.Invoke();
                    active = false;
                }
            }

            gameObject.texture = animation.frames[frameIndex];
        }
    }
}

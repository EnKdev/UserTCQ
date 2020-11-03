using UserTCQ.Engine.Rendering;
using OpenTK.Mathematics;
using System;

namespace UserTCQ.Engine.Types
{
    public struct ParticleProps
    {
        public float speed;
        public float life;
        public Color4 colorBegin;
        public Color4 colorEnd;
        public Vector2 movementVector;
        public Vector3 startPosition;
    }

    public class Particle : GameObject
    {
        static new uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        static new float[] vertBuffer = new float[20] {
            -1, -1, 0, 0, 1,
            1, -1, 0, 1, 1,
            1, 1, 0, 1, 0,
            -1, 1, 0, 0, 0
        };

        static new VertexArray vertexArray = new VertexArray(vertBuffer, indices).Init();

        ParticleProps props;

        public Particle(float width, float height, Texture texture, Shader shader, ParticleProps props) : base("Particle")
        {
            base.vertexArray = vertexArray;
            base.texture = texture;
            base.shader = shader;
            scale.X = width; scale.Y = height; scale.Z = 1.0f;
            position = props.startPosition;
            color = props.colorBegin;
            this.props = props;
            SetActive(true);
        }

        float t;

        protected override void Update()
        {
            base.Update();
            t += Time.deltaTime;

            if (t > props.life)
            {
                Dispose();
                return;
            }

            position += props.movementVector.ToVector3() * props.speed * Time.deltaTime;
            color = Helper.LerpColor(props.colorBegin, props.colorEnd, t / props.life);
        }
    }

    public class ParticleSystem : Component
    {
        public bool enabled;

        public Color4 colorBegin;
        public Color4 colorEnd;

        public float particleRate = 30f;
        public float particleScale = 0.2f;
        public float particleSpeed = 2f;
        public float particleLife = 0.25f;

        public float spreadAngle;

        public float offsetAngle;
        public float positionZ = 0f;

        public float scatter;
        public Vector2 offset;

        public Texture texture;
        public Shader shader;

        float particleCount;
        int particlesSpawned;

        Random rnd = new Random();

        public override void Update()
        {
            if (!enabled)
                return;

            particleCount += Time.deltaTime * particleRate;

            if ((int)particleCount > particlesSpawned)
            {
                for (int i = 0; i < (int)particleCount - particlesSpawned; i++)
                {
                    float randomPos = (float)rnd.NextDouble() * 2 - 1;
                    Vector2 posRaw = new Vector2(offset.X, offset.Y + randomPos * scatter);
                    Vector3 newPos = Helper.RotatePoint(Vector2.Zero, posRaw, gameObject.rotationEuler.Z + offsetAngle).ToVector3();
                    newPos.Z = positionZ;
                    new Particle(particleScale, particleScale, texture, shader, new ParticleProps()
                    {
                        speed = particleSpeed,
                        life = particleLife,
                        colorBegin = colorBegin,
                        colorEnd = colorEnd,
                        movementVector = new Vector2(MathF.Cos(gameObject.rotationEuler.Z + offsetAngle + randomPos * spreadAngle), MathF.Sin(gameObject.rotationEuler.Z + offsetAngle + randomPos * spreadAngle)),
                        startPosition = newPos + gameObject.position
                    });
                }
                particlesSpawned = (int)particleCount;
            }
        }
    }
}

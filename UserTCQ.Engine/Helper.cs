using OpenTK.Mathematics;
using System;

namespace UserTCQ.Engine
{
    public static class Helper
    {
        public static float Lerp(float a, float b, float t)
        {
            t = Math.Clamp(t, 0f, 1f);
            float result = ((1 - t) * a) + (t * b);
            return result;
        }

        public static Color4 LerpColor(Color4 c0, Color4 c1, float t)
        {
            return new Color4(
                Lerp(c0.R, c1.R, t),
                Lerp(c0.G, c1.G, t),
                Lerp(c0.B, c1.B, t),
                Lerp(c0.A, c1.A, t)
            );
        }

        private const float PI_TIMES_TWO = MathF.PI * 2f;

        public static float LerpRadians(float a, float b, float t)
        {
            float difference = MathF.Abs(b - a);
            if (difference > MathF.PI)
            {
                if (b > a)
                {
                    a += PI_TIMES_TWO;
                }
                else
                {
                    b += PI_TIMES_TWO;
                }
            }
            float value = a + ((b - a) * t);

            float rangeZero = PI_TIMES_TWO;

            if (value >= 0 && value <= PI_TIMES_TWO)
                return value;

            return value % rangeZero;
        }

        public static Vector2 RotatePoint(Vector2 pivot, Vector2 point, float angle)
        {
            float s = MathF.Sin(angle);
            float c = MathF.Cos(angle);

            point.X -= pivot.X;
            point.Y -= pivot.Y;

            float xnew = point.X * c - point.Y * s;
            float ynew = point.X * s + point.Y * c;

            point.X = xnew + pivot.X;
            point.Y = ynew + pivot.Y;
            return point;
        }

        public static Vector3 ToVector3(this Vector2 vector2)
        {
            return new Vector3(vector2.X, vector2.Y, 0f);
        }

        public static Vector2 ToVector2(this Vector3 vector3)
        {
            return new Vector2(vector3.X, vector3.Y);
        }

        public static bool IsWithin(this byte value, byte minimum, byte maximum)
        {
            return value >= minimum && value <= maximum;
        }
    }
}

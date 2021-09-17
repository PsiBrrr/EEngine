using System.Drawing;

namespace EEngine.EEngine
{
    public class Vector2
    {
        public float X { get; set; }
        public float Y { get; set; }

        public Vector2()
        {
            X = Zero().X;
            Y = Zero().Y;
        }

        public Vector2(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Returns X and Y as 1
        /// </summary>
        public static Vector2 One()
        {
            return new Vector2(1, 1);
        }

        /// <summary>
        /// Returns X and Y as 0
        /// </summary>
        public static Vector2 Zero()
        {
            return new Vector2(0, 0);
        }

        public static Point AsPoint(Vector2 A)
        {
            return new Point((int)A.X, (int)A.Y);
        }
        public static Size AsSize(Vector2 A)
        {
            return new Size((int)A.X, (int)A.Y);
        }

        public static Vector2 operator +(Vector2 A, Vector2 B)
        {
            return new Vector2(A.X + B.X, A.Y + B.Y);
        }
        public static Vector2 operator +(Vector2 A, decimal B)
        {
            return new Vector2(A.X + ((float)B), A.Y + ((float)B));
        }
        public static Vector2 operator -(Vector2 A, Vector2 B)
        {
            return new Vector2(A.X - B.X, A.Y - B.Y);
        }
        public static Vector2 operator *(Vector2 A, Vector2 B)
        {
            return new Vector2(A.X * B.X, A.Y * B.Y);
        }
        public static Vector2 operator *(Vector2 A, int B)
        {
            return new Vector2(A.X * B, A.Y * B);
        }
        public static Vector2 operator *(Vector2 A, decimal B)
        {
            return new Vector2(A.X * ((float)B), A.Y * ((float)B));
        }
        public static Vector2 operator *(Vector2 A, float B)
        {
            return new Vector2(A.X * B, A.Y * B);
        }
        public static Vector2 operator /(Vector2 A, Vector2 B)
        {
            return new Vector2(A.X / B.X, A.Y / B.Y);
        }
        public static Vector2 operator /(Vector2 A, int B)
        {
            return new Vector2(A.X / B, A.Y / B);
        }

        public static bool operator ==(Vector2 A, Vector2 B)
        {
            if (A.X == B.X && A.Y == B.Y) { return true; }
            else { return false; }
        }
        public static bool operator !=(Vector2 A, Vector2 B)
        {
            if (A.X == B.X && A.Y == B.Y) { return false; }
            else { return true; }
        }
    }
}

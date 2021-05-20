namespace EEngine.EEngine
{
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vector3()
        {
            X = Zero().X;
            Y = Zero().Y;
            Z = Zero().Z;
        }

        public Vector3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        /// <summary>
        /// Returns X, Y, and Z as 1
        /// </summary>
        public static Vector3 One()
        {
            return new Vector3(1, 1, 1);
        }


        /// <summary>
        /// Returns X, Y, and Z as 0
        /// </summary>
        public static Vector3 Zero()
        {
            return new Vector3(0, 0, 0);
        }
    }
}

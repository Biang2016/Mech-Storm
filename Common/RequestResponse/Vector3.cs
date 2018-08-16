using System;

namespace MyCardGameCommon
{
    public class Vector3
    {
        public float x;
        public float y;
        public float z;

        public Vector3()
        {
        }

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override string ToString()
        {
            return String.Format("({0},{1},{2})", x, y, z);
        }

        public void Serialize(DataStream writer)
        {
            writer.WriteSInt32((int) (x * 1000));
            writer.WriteSInt32((int) (y * 1000));
            writer.WriteSInt32((int) (z * 1000));
        }

        public static Vector3 Deserialize(DataStream reader)
        {
            Vector3 vector = new Vector3();
            vector.x = (float) reader.ReadSInt32() / 1000;
            vector.y = (float) reader.ReadSInt32() / 1000;
            vector.z = (float) reader.ReadSInt32() / 1000;
            return vector;
        }
    }
}
using DarkRift;

namespace PlanetGorgon_Server
{
    class Player : IDarkRiftSerializable
    {
        public Vec3 Position { get; set; }
        public Vec3 Rotation { get; set; }
        public ushort ID { get; set; }

        public Player()
        {

        }

        public Player(Vec3 position, Vec3 rotation, ushort ID)
        {
            this.Position = position;
            this.Rotation = rotation;
            this.ID = ID;
        }

        public void Deserialize(DeserializeEvent e)
        {
            this.Position = e.Reader.ReadSerializable<Vec3>();
            this.Rotation = e.Reader.ReadSerializable<Vec3>();
            this.ID = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Position);
            e.Writer.Write(Rotation);
            e.Writer.Write(ID);
        }
    }

    class PlayerAnimationState : IDarkRiftSerializable
    {
        public int Speed { get; set; }
        public byte Jump { get; set; }
        public byte Grounded { get; set; }
        public ushort ID { get; set; }

        public PlayerAnimationState()
        {

        }

        public PlayerAnimationState(int Speed, byte Jump, byte Grounded, ushort ID)
        {
            this.Speed = Speed;
            this.Jump = Jump;
            this.Grounded = Grounded;
            this.ID = ID;
        }

        public void Deserialize(DeserializeEvent e)
        {
            this.Speed = e.Reader.ReadInt32();
            this.Jump = e.Reader.ReadByte();
            this.Grounded = e.Reader.ReadByte();
            this.ID = e.Reader.ReadUInt16();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(Speed);
            e.Writer.Write(Jump);
            e.Writer.Write(Grounded);
            e.Writer.Write(ID);
        }
    }

    class Vec3 : IDarkRiftSerializable
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public Vec3()
        {

        }

        public Vec3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public void Deserialize(DeserializeEvent e)
        {
            this.X = e.Reader.ReadSingle();
            this.Y = e.Reader.ReadSingle();
            this.Z = e.Reader.ReadSingle();
        }

        public void Serialize(SerializeEvent e)
        {
            e.Writer.Write(X);
            e.Writer.Write(Y);
            e.Writer.Write(Z);
        }
    }

}

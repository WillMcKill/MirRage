using System;
using System.Drawing;
using System.IO;

namespace Server.MirDatabase
{
    public class RespawnInfo
    {
        public int MonsterIndex;
        public Point Location;
        public ushort Count, Spread, Delay;
        public byte Direction;

        public RespawnInfo()
        {

        }
        public RespawnInfo(BinaryReader reader)
        {
            MonsterIndex = reader.ReadInt32();

            Location = new Point(reader.ReadInt32(), reader.ReadInt32());

            Count = reader.ReadUInt16();
            Spread = reader.ReadUInt16();

            Delay = reader.ReadUInt16();
            Direction = reader.ReadByte();
        }

        public static RespawnInfo FromText(string text)
        {
            string[] data = text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);

            if (data.Length != 7) return null;

            RespawnInfo info = new RespawnInfo();

            int x,y ;

            if (!int.TryParse(data[0], out info.MonsterIndex)) return null;
            if (!int.TryParse(data[1], out x)) return null;
            if (!int.TryParse(data[2], out y)) return null;

            info.Location = new Point(x, y);

            if (!ushort.TryParse(data[3], out info.Count)) return null;
            if (!ushort.TryParse(data[4], out info.Spread)) return null;
            if (!ushort.TryParse(data[5], out info.Delay)) return null;
            if (!byte.TryParse(data[6], out info.Direction)) return null;

            return info;
        }

        public void Save(BinaryWriter writer)
        {
            writer.Write(MonsterIndex);

            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(Count);
            writer.Write(Spread);

            writer.Write(Delay);
            writer.Write(Direction);
        }

        public override string ToString()
        {
            return string.Format("Monster: {0} - {1} - {2} - {3}", MonsterIndex, Functions.PointToString(Location), Count, Delay);
        }
    }
}
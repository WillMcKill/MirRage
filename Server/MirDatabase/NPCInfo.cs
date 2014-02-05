using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace Server.MirDatabase
{
    public class NPCInfo
    {
        public string FileName = string.Empty, Name = string.Empty;

        public Point Location;
        public ushort Rate = 100;
        public byte Image;

        public float PriceRate
        {
            get { return Rate / 100F; }
        }

        
        public NPCInfo()
        { }
        public NPCInfo(BinaryReader reader)
        {
            FileName = reader.ReadString();
            Name = reader.ReadString();

            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Image = reader.ReadByte();
            
            Rate = reader.ReadUInt16();
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(FileName);
            writer.Write(Name);

            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(Image);
            writer.Write(Rate);
        }


        public override string ToString()
        {
            return string.Format("{0}: - {1}", Name, Functions.PointToString(Location));
        }
    }
}

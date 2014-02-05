using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;


namespace ServerPackets
{
    public sealed class Connected : Packet
    {
        public override short Index
        {
            get { return 0; }
        }

        protected override void ReadPacket(BinaryReader reader)
        {
        }

        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class ClientVersion : Packet
    {
        public override short Index
        {
            get { return 1; }
        }

        public byte Result;
        /*
         * 0: Wrong Version
         * 1: Correct Version
         */

        protected override void ReadPacket(BinaryReader reader)
        {
            Result = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Result);
        }
    }
    public sealed class Disconnect : Packet
    {
        public override short Index
        {
            get { return 2; }
        }

        public byte Reason;

        /*
         * 0: Server Closing.
         * 1: Another User.
         * 2: Packet Error.
         * 3: Server Crashed.
         */

        protected override void ReadPacket(BinaryReader reader)
        {
            Reason = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Reason);
        }
    }
    public sealed class NewAccount : Packet
    {
        public override short Index
        {
            get { return 3; }
        }

        public byte Result;
        /*
         * 0: Disabled
         * 1: Bad AccountID
         * 2: Bad Password
         * 3: Bad Email
         * 4: Bad Name
         * 5: Bad Question
         * 6: Bad Answer
         * 7: Account Exists.
         * 8: Success
         */

        protected override void ReadPacket(BinaryReader reader)
        {
            Result = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Result);
        }

    }
    public sealed class ChangePassword : Packet
    {
        public override short Index
        {
            get { return 4; }
        }

        public byte Result;
        /*
         * 0: Disabled
         * 1: Bad AccountID
         * 2: Bad Current Password
         * 3: Bad New Password
         * 4: Account Not Exist
         * 5: Wrong Password
         * 6: Success
         */

        protected override void ReadPacket(BinaryReader reader)
        {
            Result = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Result);
        }
    }
    public sealed class ChangePasswordBanned : Packet
    {
        public override short Index
        {
            get { return 5; }
        }

        public string Reason = string.Empty;
        public DateTime ExpiryDate;

        protected override void ReadPacket(BinaryReader reader)
        {
            Reason = reader.ReadString();
            ExpiryDate = DateTime.FromBinary(reader.ReadInt64());
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Reason);
            writer.Write(ExpiryDate.ToBinary());
        }
    }
    public sealed class Login : Packet
    {
        public override short Index
        {
            get { return 6; }
        }

        public byte Result;
        /*
         * 0: Disabled
         * 1: Bad AccountID
         * 2: Bad Password
         * 3: Account Not Exist
         * 4: Wrong Password
         */

        protected override void ReadPacket(BinaryReader reader)
        {
            Result = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Result);
        }
    }
    public sealed class LoginBanned : Packet
    {
        public override short Index
        {
            get { return 7; }
        }

        public string Reason = string.Empty;
        public DateTime ExpiryDate;

        protected override void ReadPacket(BinaryReader reader)
        {
            Reason = reader.ReadString();
            ExpiryDate = DateTime.FromBinary(reader.ReadInt64());
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Reason);
            writer.Write(ExpiryDate.ToBinary());
        }
    }
    public sealed class LoginSuccess : Packet
    {
        public override short Index
        {
            get { return 8; }
        }

        public List<SelectInfo> Characters = new List<SelectInfo>();

        protected override void ReadPacket(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Characters.Add(new SelectInfo(reader));
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Characters.Count);

            for (int i = 0; i < Characters.Count; i++)
                Characters[i].Save(writer);
        }
    }
    public sealed class NewCharacter : Packet
    {
        public override short Index
        {
            get { return 9; }
        }

        /*
         * 0: Disabled.
         * 1: Bad Character Name
         * 2: Bad Gender
         * 3: Bad Class
         * 4: Max Characters
         * 5: Character Exists.
         * */
        public byte Result;

        protected override void ReadPacket(BinaryReader reader)
        {
            Result = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Result);
        }
    }
    public sealed class NewCharacterSuccess : Packet
    {
        public override short Index
        {
            get { return 10; }
        }

        public SelectInfo CharInfo;

        protected override void ReadPacket(BinaryReader reader)
        {
            CharInfo = new SelectInfo(reader);
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            CharInfo.Save(writer);
        }
    }
    public sealed class DeleteCharacter : Packet
    {
        public override short Index
        {
            get { return 11; }
        }

        public byte Result;

        /*
         * 0: Disabled.
         * 1: Character Not Found
         * */

        protected override void ReadPacket(BinaryReader reader)
        {
            Result = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Result);
        }
    }
    public sealed class DeleteCharacterSuccess : Packet
    {
        public override short Index
        {
            get { return 12; }
        }

        public int CharacterIndex;

        protected override void ReadPacket(BinaryReader reader)
        {
            CharacterIndex = reader.ReadInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(CharacterIndex);
        }
    }
    public sealed class StartGame : Packet
    {
        public override short Index
        {
            get { return 13; }
        }

        public byte Result;

        /*
         * 0: Disabled.
         * 1: Not logged in
         * 2: Character not found.
         * 3: Start Game Error
         * */

        protected override void ReadPacket(BinaryReader reader)
        {
            Result = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Result);
        }
    }
    public sealed class StartGameBanned : Packet
    {
        public override short Index
        {
            get { return 14; }
        }

        public string Reason = string.Empty;
        public DateTime ExpiryDate;

        protected override void ReadPacket(BinaryReader reader)
        {
            Reason = reader.ReadString();
            ExpiryDate = DateTime.FromBinary(reader.ReadInt64());
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Reason);
            writer.Write(ExpiryDate.ToBinary());
        }
    }
    public sealed class StartGameDelay : Packet
    {
        public override short Index
        {
            get { return 15; }
        }

        public long Milliseconds;

        protected override void ReadPacket(BinaryReader reader)
        {
            Milliseconds = reader.ReadInt64();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Milliseconds);
        }

    }
    public sealed class MapInformation : Packet
    {
        public override short Index
        {
            get { return 16; }
        }

        public string FileName = string.Empty;
        public string Title = string.Empty;
        public ushort MiniMap, BigMap;
        public LightSetting Lights;
        public bool Lightning, Fire;

        protected override void ReadPacket(BinaryReader reader)
        {
            FileName = reader.ReadString();
            Title = reader.ReadString();
            MiniMap = reader.ReadUInt16();
            BigMap = reader.ReadUInt16();
            Lights = (LightSetting) reader.ReadByte();
            Lightning = reader.ReadBoolean();
            Fire = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(FileName);
            writer.Write(Title);
            writer.Write(MiniMap);
            writer.Write(BigMap);
            writer.Write((byte) Lights);
            writer.Write(Lightning);
            writer.Write(Fire);
        }
    }
    public sealed class UserInformation : Packet
    {
        public override short Index
        {
            get { return 17; }
        }

        public uint ObjectID;
        public string Name = string.Empty;
        public Color NameColour;
        public MirClass Class;
        public MirGender Gender;
        public byte Level;
        public Point Location;
        public MirDirection Direction;
        public byte Hair;
        public ushort HP, MP;
        public long Experience, MaxExperience;

        public UserItem[] Inventory, Equipment;
        public uint Gold;

        public List<ClientMagic> Magics = new List<ClientMagic>();

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Name = reader.ReadString();
            NameColour = Color.FromArgb(reader.ReadInt32());
            Class = (MirClass) reader.ReadByte();
            Gender = (MirGender) reader.ReadByte();
            Level = reader.ReadByte();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
            Hair = reader.ReadByte();
            HP = reader.ReadUInt16();
            MP = reader.ReadUInt16();

            Experience = reader.ReadInt64();
            MaxExperience = reader.ReadInt64();

            if (reader.ReadBoolean())
            {
                Inventory = new UserItem[reader.ReadInt32()];
                for (int i = 0; i < Inventory.Length; i++)
                {
                    if (!reader.ReadBoolean()) continue;
                    Inventory[i] = new UserItem(reader);
                }
            }

            if (reader.ReadBoolean())
            {
                Equipment = new UserItem[reader.ReadInt32()];
                for (int i = 0; i < Equipment.Length; i++)
                {
                    if (!reader.ReadBoolean()) continue;
                    Equipment[i] = new UserItem(reader);
                }
            }
            Gold = reader.ReadUInt32();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Magics.Add(new ClientMagic(reader));
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Name);
            writer.Write(NameColour.ToArgb());
            writer.Write((byte) Class);
            writer.Write((byte) Gender);
            writer.Write(Level);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
            writer.Write(Hair);
            writer.Write(HP);
            writer.Write(MP);

            writer.Write(Experience);
            writer.Write(MaxExperience);

            writer.Write(Inventory != null);
            if (Inventory != null)
            {
                writer.Write(Inventory.Length);
                for (int i = 0; i < Inventory.Length; i++)
                {
                    writer.Write(Inventory[i] != null);
                    if (Inventory[i] == null) continue;

                    Inventory[i].Save(writer);
                }

            }

            writer.Write(Equipment != null);
            if (Equipment != null)
            {
                writer.Write(Equipment.Length);
                for (int i = 0; i < Equipment.Length; i++)
                {
                    writer.Write(Equipment[i] != null);
                    if (Equipment[i] == null) continue;

                    Equipment[i].Save(writer);
                }
            }
            writer.Write(Gold);

            writer.Write(Magics.Count);
            for (int i = 0; i < Magics.Count; i++)
                Magics[i].Save(writer);
        }
    }
    public sealed class UserLocation : Packet
    {
        public override short Index
        {
            get { return 18; }
        }

        public Point Location;
        public MirDirection Direction;


        protected override void ReadPacket(BinaryReader reader)
        {
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
        }
    }
    public sealed class ObjectPlayer : Packet
    {

        public override short Index
        {
            get { return 19; }
        }

        public uint ObjectID;
        public string Name = string.Empty;
        public Color NameColour;
        public MirClass Class;
        public MirGender Gender;
        public Point Location;
        public MirDirection Direction;
        public byte Hair;
        public byte Light;
        public sbyte Weapon, Armour;
        public PoisonType Poison;
        public bool Dead, Hidden;
        public SpellEffect Effect;
        public byte WingEffect;
        public bool Extra;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Name = reader.ReadString();
            NameColour = Color.FromArgb(reader.ReadInt32());
            Class = (MirClass) reader.ReadByte();
            Gender = (MirGender) reader.ReadByte();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
            Hair = reader.ReadByte();
            Light = reader.ReadByte();
            Weapon = reader.ReadSByte();
            Armour = reader.ReadSByte();
            Poison = (PoisonType) reader.ReadByte();
            Dead = reader.ReadBoolean();
            Hidden = reader.ReadBoolean();
            Effect = (SpellEffect) reader.ReadByte();
            WingEffect = reader.ReadByte();
            Extra = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Name);
            writer.Write(NameColour.ToArgb());
            writer.Write((byte) Class);
            writer.Write((byte) Gender);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
            writer.Write(Hair);
            writer.Write(Light);
            writer.Write(Weapon);
            writer.Write(Armour);
            writer.Write((byte) Poison);
            writer.Write(Dead);
            writer.Write(Hidden);
            writer.Write((byte) Effect);
            writer.Write(WingEffect);
            writer.Write(Extra);
        }
    }
    public sealed class ObjectRemove : Packet
    {
        public override short Index
        {
            get { return 20; }
        }

        public uint ObjectID;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
        }

    }
    public sealed class ObjectTurn : Packet
    {
        public override short Index
        {
            get { return 21; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
        }
    }
    public sealed class ObjectWalk : Packet
    {
        public override short Index
        {
            get { return 22; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
        }
    }
    public sealed class ObjectRun : Packet
    {
        public override short Index
        {
            get { return 23; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
        }
    }
    public sealed class Chat : Packet
    {
        public override short Index
        {
            get { return 24; }
        }

        public string Message = string.Empty;
        public ChatType Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            Message = reader.ReadString();
            Type = (ChatType) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Message);
            writer.Write((byte) Type);
        }
    }
    public sealed class ObjectChat : Packet
    {
        public override short Index
        {
            get { return 25; }
        }

        public uint ObjectID;
        public string Text = string.Empty;
        public ChatType Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Text = reader.ReadString();
            Type = (ChatType) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Text);
            writer.Write((byte) Type);
        }
    }
    public sealed class NewItemInfo : Packet
    {
        public override short Index
        {
            get { return 26; }
        }

        public ItemInfo Info;

        protected override void ReadPacket(BinaryReader reader)
        {
            Info = new ItemInfo(reader);
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            Info.Save(writer);
        }
    }
    public sealed class MoveItem : Packet
    {
        public override short Index
        {
            get { return 27; }
        }

        public MirGridType Grid;
        public int From, To;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType) reader.ReadByte();
            From = reader.ReadInt32();
            To = reader.ReadInt32();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Grid);
            writer.Write(From);
            writer.Write(To);
            writer.Write(Success);
        }
    }
    public sealed class EquipItem : Packet
    {
        public override short Index
        {
            get { return 28; }
        }

        public MirGridType Grid;
        public ulong UniqueID;
        public int To;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType) reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            To = reader.ReadInt32();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Grid);
            writer.Write(UniqueID);
            writer.Write(To);
            writer.Write(Success);
        }
    }
    public sealed class MergeItem : Packet
    {
        public override short Index
        {
            get { return 29; }
        }

        public MirGridType GridFrom, GridTo;
        public ulong IDFrom, IDTo;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            GridFrom = (MirGridType) reader.ReadByte();
            GridTo = (MirGridType) reader.ReadByte();
            IDFrom = reader.ReadUInt64();
            IDTo = reader.ReadUInt64();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) GridFrom);
            writer.Write((byte) GridTo);
            writer.Write(IDFrom);
            writer.Write(IDTo);
            writer.Write(Success);
        }
    }
    public sealed class RemoveItem : Packet
    {
        public override short Index
        {
            get { return 30; }
        }

        public MirGridType Grid;
        public ulong UniqueID;
        public int To;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType) reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            To = reader.ReadInt32();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Grid);
            writer.Write(UniqueID);
            writer.Write(To);
            writer.Write(Success);
        }
    }
    public sealed class TakeBackItem : Packet
    {
        public override short Index
        {
            get { return 31; }
        }

        public int From, To;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
            writer.Write(Success);
        }
    }
    public sealed class StoreItem : Packet
    {
        public override short Index
        {
            get { return 32; }
        }

        public int From, To;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
            writer.Write(Success);
        }
    }
    public sealed class SplitItem : Packet
    {
        public override short Index
        {
            get { return 33; }
        }

        public UserItem Item;
        public MirGridType Grid;

        protected override void ReadPacket(BinaryReader reader)
        {
            if (reader.ReadBoolean())
                Item = new UserItem(reader);

            Grid = (MirGridType) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Item != null);
            if (Item != null) Item.Save(writer);
            writer.Write((byte) Grid);
        }
    }
    public sealed class SplitItem1 : Packet
    {
        public override short Index
        {
            get { return 34; }
        }

        public MirGridType Grid;
        public ulong UniqueID;
        public uint Count;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType) reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt32();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Grid);
            writer.Write(UniqueID);
            writer.Write(Count);
            writer.Write(Success);
        }
    }
    public sealed class UseItem : Packet
    {
        public override short Index
        {
            get { return 35; }
        }

        public ulong UniqueID;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Success);
        }
    }
    public sealed class DropItem : Packet
    {
        public override short Index
        {
            get { return 36; }
        }

        public ulong UniqueID;
        public uint Count;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt32();
            Success = reader.ReadBoolean();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Count);
            writer.Write(Success);
        }
    }
    public sealed class PlayerUpdate : Packet
    {
        public override short Index
        {
            get { return 37; }
        }

        public uint ObjectID;
        public byte Light;
        public sbyte Weapon, Armour;
        public byte WingEffect;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();

            Light = reader.ReadByte();
            Weapon = reader.ReadSByte();
            Armour = reader.ReadSByte();
            WingEffect = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);

            writer.Write(Light);
            writer.Write(Weapon);
            writer.Write(Armour);
            writer.Write(WingEffect);
        }
    }
    public sealed class PlayerInspect : Packet
    {
        public override short Index
        {
            get { return 38; }
        }

        public string Name = string.Empty;
        public UserItem[] Equipment;
        public MirClass Class;
        public MirGender Gender;
        public byte Hair;

        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();

            Equipment = new UserItem[reader.ReadInt32()];
            for (int i = 0; i < Equipment.Length; i++)
            {
                if (reader.ReadBoolean())
                    Equipment[i] = new UserItem(reader);
            }

            Class = (MirClass) reader.ReadByte();
            Gender = (MirGender) reader.ReadByte();
            Hair = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);

            writer.Write(Equipment.Length);
            for (int i = 0; i < Equipment.Length; i++)
            {
                UserItem T = Equipment[i];
                writer.Write(T != null);
                if (T != null) T.Save(writer);
            }

            writer.Write((byte) Class);
            writer.Write((byte) Gender);
            writer.Write(Hair);

        }
    }
    public sealed class LogOutSuccess : Packet
    {
        public override short Index
        {
            get { return 39; }
        }

        public List<SelectInfo> Characters = new List<SelectInfo>();

        protected override void ReadPacket(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Characters.Add(new SelectInfo(reader));
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Characters.Count);

            for (int i = 0; i < Characters.Count; i++)
                Characters[i].Save(writer);
        }
    }
    public sealed class TimeOfDay : Packet
    {
        public override short Index
        {
            get { return 40; }
        }

        public LightSetting Lights;

        protected override void ReadPacket(BinaryReader reader)
        {
            Lights = (LightSetting) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Lights);
        }
    }
    public sealed class ChangeAMode : Packet
    {
        public override short Index
        {
            get { return 41; }
        }

        public AttackMode Mode;

        protected override void ReadPacket(BinaryReader reader)
        {
            Mode = (AttackMode) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Mode);
        }
    }
    public sealed class ChangePMode : Packet
    {
        public override short Index
        {
            get { return 42; }
        }

        public PetMode Mode;

        protected override void ReadPacket(BinaryReader reader)
        {
            Mode = (PetMode) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Mode);
        }
    }
    public sealed class ObjectItem : Packet
    {
        public override short Index
        {
            get { return 43; }
        }

        public uint ObjectID;
        public string Name = string.Empty;
        public Color NameColour;
        public Point Location;
        public ushort Image;


        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Name = reader.ReadString();
            NameColour = Color.FromArgb(reader.ReadInt32());
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Image = reader.ReadUInt16();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Name);
            writer.Write(NameColour.ToArgb());
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write(Image);
        }
    }
    public sealed class ObjectGold : Packet
    {
        public override short Index
        {
            get { return 44; }
        }

        public uint ObjectID;
        public uint Gold;
        public Point Location;


        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Gold = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Gold);
            writer.Write(Location.X);
            writer.Write(Location.Y);
        }
    }
    public sealed class GainedItem : Packet
    {
        public override short Index
        {
            get { return 45; }
        }

        public UserItem Item;

        protected override void ReadPacket(BinaryReader reader)
        {
            Item = new UserItem(reader);
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            Item.Save(writer);
        }
    }
    public sealed class GainedGold : Packet
    {
        public override short Index
        {
            get { return 46; }
        }

        public uint Gold;

        protected override void ReadPacket(BinaryReader reader)
        {
            Gold = reader.ReadUInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Gold);
        }
    }
    public sealed class LoseGold : Packet
    {
        public override short Index
        {
            get { return 47; }
        }

        public uint Gold;

        protected override void ReadPacket(BinaryReader reader)
        {
            Gold = reader.ReadUInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Gold);
        }
    }
    public sealed class ObjectMonster : Packet
    {
        public override short Index
        {
            get { return 48; }
        }

        public uint ObjectID;
        public string Name = string.Empty;
        public Color NameColour;
        public Point Location;
        public Monster Image;
        public MirDirection Direction;
        public byte Effect, AI, Light;
        public bool Dead, Skeleton;
        public PoisonType Poison;
        public bool Hidden, Extra;
        public byte ExtraByte;


        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Name = reader.ReadString();
            NameColour = Color.FromArgb(reader.ReadInt32());
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Image = (Monster) reader.ReadUInt16();
            Direction = (MirDirection) reader.ReadByte();
            Effect = reader.ReadByte();
            AI = reader.ReadByte();
            Light = reader.ReadByte();
            Dead = reader.ReadBoolean();
            Skeleton = reader.ReadBoolean();
            Poison = (PoisonType)reader.ReadByte();
            Hidden = reader.ReadBoolean();
            Extra = reader.ReadBoolean();
            ExtraByte = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Name);
            writer.Write(NameColour.ToArgb());
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((ushort) Image);
            writer.Write((byte) Direction);
            writer.Write(Effect);
            writer.Write(AI);
            writer.Write(Light);
            writer.Write(Dead);
            writer.Write(Skeleton);
            writer.Write((byte)Poison);
            writer.Write(Hidden);
            writer.Write(Extra);
            writer.Write((byte)ExtraByte);
        }

    }
    public sealed class ObjectAttack : Packet
    {
        public override short Index
        {
            get { return 49; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;
        public Spell Spell;
        public byte Level;
        public byte Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
            Spell = (Spell)reader.ReadByte();
            Level = reader.ReadByte();
            Type = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
            writer.Write((byte)Spell);
            writer.Write(Level);
            writer.Write(Type);
        }
    }
    public sealed class Struck : Packet
    {
        public override short Index
        {
            get { return 50; }
        }

        public uint AttackerID;

        protected override void ReadPacket(BinaryReader reader)
        {
            AttackerID = reader.ReadUInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AttackerID);
        }
    }
    public sealed class ObjectStruck : Packet
    {
        public override short Index
        {
            get { return 51; }
        }

        public uint ObjectID;
        public uint AttackerID;
        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            AttackerID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(AttackerID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
        }
    }
    public sealed class DuraChanged : Packet
    {
        public override short Index
        {
            get { return 52; }
        }

        public ulong UniqueID;
        public ushort CurrentDura;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            CurrentDura = reader.ReadUInt16();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(CurrentDura);
        }
    }
    public sealed class HealthChanged : Packet
    {
        public override short Index
        {
            get { return 53; }
        }

        public ushort HP, MP;

        protected override void ReadPacket(BinaryReader reader)
        {
            HP = reader.ReadUInt16();
            MP = reader.ReadUInt16();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(HP);
            writer.Write(MP);
        }
    }
    public sealed class DeleteItem : Packet
    {
        public override short Index
        {
            get { return 54; }
        }

        public ulong UniqueID;
        public uint Count;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Count);
        }
    }
    public sealed class Death : Packet
    {
        public override short Index
        {
            get { return 55; }
        }

        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class ObjectDied : Packet
    {
        public override short Index
        {
            get { return 56; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;
        public byte Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
            Type = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
            writer.Write(Type);
        }
    }
    public sealed class ColourChanged : Packet
    {
        public override short Index
        {
            get { return 57; }
        }

        public Color NameColour;

        protected override void ReadPacket(BinaryReader reader)
        {
            NameColour = Color.FromArgb(reader.ReadInt32());
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(NameColour.ToArgb());
        }
    }
    public sealed class ObjectColourChanged : Packet
    {
        public override short Index
        {
            get { return 58; }
        }

        public uint ObjectID;
        public Color NameColour;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            NameColour = Color.FromArgb(reader.ReadInt32());
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(NameColour.ToArgb());
        }
    }
    public sealed class GainExperience : Packet
    {
        public override short Index
        {
            get { return 59; }
        }

        public uint Amount;

        protected override void ReadPacket(BinaryReader reader)
        {
            Amount = reader.ReadUInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Amount);
        }
    }
    public sealed class LevelChanged : Packet
    {
        public override short Index
        {
            get { return 60; }
        }

        public byte Level;
        public long Experience, MaxExperience;

        protected override void ReadPacket(BinaryReader reader)
        {
            Level = reader.ReadByte();
            Experience = reader.ReadInt64();
            MaxExperience = reader.ReadInt64();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Level);
            writer.Write(Experience);
            writer.Write(MaxExperience);
        }
    }
    public sealed class ObjectLeveled : Packet
    {
        public override short Index
        {
            get { return 61; }
        }

        public uint ObjectID;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
        }
    }
    public sealed class ObjectHarvest : Packet
    {
        public override short Index
        {
            get { return 62; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
        }
    }
    public sealed class ObjectHarvested : Packet
    {
        public override short Index
        {
            get { return 63; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Direction);
        }

    }
    public sealed class ObjectNPC : Packet
    {
        public override short Index
        {
            get { return 64; }
        }

        public uint ObjectID;
        public string Name = string.Empty;
        public Color NameColour;
        public byte Image;
        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Name = reader.ReadString();
            NameColour = Color.FromArgb(reader.ReadInt32());
            Image = reader.ReadByte();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Name);
            writer.Write(NameColour.ToArgb());
            writer.Write(Image);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class NPCResponse : Packet
    {
        public override short Index { get { return 65; } }

        public List<string> Page;

        protected override void ReadPacket(BinaryReader reader)
        {
            Page = new List<string>();

            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Page.Add(reader.ReadString());
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Page.Count);

            for (int i = 0; i < Page.Count; i++)
                writer.Write(Page[i]);
        }
    }
    public sealed class ObjectHide : Packet
    {
        public override short Index { get { return 66; } }

        public uint ObjectID;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
        }
    }
    public sealed class ObjectShow : Packet
    {
        public override short Index { get { return 67; } }

        public uint ObjectID;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
        }
    }
    public sealed class Poisoned : Packet
    {
        public override short Index { get { return 68; } }

        public PoisonType Poison;

        protected override void ReadPacket(BinaryReader reader)
        {
            Poison = (PoisonType)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Poison);
        }
    }
    public sealed class ObjectPoisoned : Packet
    {
        public override short Index { get { return 69; } }

        public uint ObjectID;
        public PoisonType Poison;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Poison = (PoisonType)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write((byte)Poison);
        }
    }
    public sealed class MapChanged : Packet
    {
        public override short Index
        {
            get { return 70; }
        }

        public string FileName = string.Empty;
        public string Title = string.Empty;
        public ushort MiniMap, BigMap;
        public LightSetting Lights;
        public Point Location;
        public MirDirection Direction;


        protected override void ReadPacket(BinaryReader reader)
        {
            FileName = reader.ReadString();
            Title = reader.ReadString();
            MiniMap = reader.ReadUInt16();
            BigMap = reader.ReadUInt16();
            Lights = (LightSetting)reader.ReadByte();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(FileName);
            writer.Write(Title);
            writer.Write(MiniMap);
            writer.Write(BigMap);
            writer.Write((byte)Lights);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class ObjectTeleportOut : Packet
    {
        public override short Index { get { return 71; } }

        public uint ObjectID;
        public byte Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Type = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Type);
        }
    }
    public sealed class ObjectTeleportIn : Packet
    {
        public override short Index { get { return 72; } }

        public uint ObjectID;
        public byte Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Type = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Type);
        }
    }
    public sealed class TeleportIn : Packet
    {
        public override short Index { get { return 73; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class NPCGoods : Packet
    {
        public override short Index { get { return 74; } }

        public List<int> List = new List<int>();
        public float Rate;

        protected override void ReadPacket(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                List.Add(reader.ReadInt32());

            Rate = reader.ReadSingle();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(List.Count);

            for (int i = 0; i < List.Count; i++)
                writer.Write(List[i]);

            writer.Write(Rate);
        }
    }
    public sealed class NPCSell : Packet
    {
        public override short Index { get { return 75; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class NPCRepair : Packet
    {
        public override short Index { get { return 76; } }
        public float Rate;

        protected override void ReadPacket(BinaryReader reader)
        {
            Rate = reader.ReadSingle();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Rate);
        }
    }
    public sealed class NPCSRepair : Packet
    {
        public override short Index { get { return 77; } }

        public float Rate;

        protected override void ReadPacket(BinaryReader reader)
        {
            Rate = reader.ReadSingle();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Rate);
        }
    }
    public sealed class NPCStorage : Packet
    {
        public override short Index { get { return 78; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class SellItem : Packet
    {
        public override short Index { get { return 79; } }

        public ulong UniqueID;
        public uint Count;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt32();
            Success = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Count);
            writer.Write(Success);
        }
    }
    public sealed class RepairItem : Packet
    {
        public override short Index { get { return 80; } }

        public ulong UniqueID;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
        }
    }
    public sealed class ItemRepaired : Packet
    {
        public override short Index { get { return 81; } }

        public ulong UniqueID;
        public ushort MaxDura, CurrentDura;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            MaxDura = reader.ReadUInt16();
            CurrentDura = reader.ReadUInt16();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(MaxDura);
            writer.Write(CurrentDura);
        }
    }
    public sealed class NewMagic : Packet
    {
        public override short Index
        {
            get { return 82; }
        }

        public ClientMagic Magic;
        protected override void ReadPacket(BinaryReader reader)
        {
            Magic = new ClientMagic(reader);
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            Magic.Save(writer);
        }

    }
    public sealed class RemoveMagic : Packet
    {
        public override short Index
        {
            get { return 119; }
        }

        public int PlaceId;
        protected override void ReadPacket(BinaryReader reader)
        {
            PlaceId = reader.ReadInt32();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(PlaceId);
        }

    }
    public sealed class MagicLeveled : Packet
    {
        public override short Index
        {
            get { return 83; }
        }

        public Spell Spell;
        public byte Level;
        public ushort Experience;

        protected override void ReadPacket(BinaryReader reader)
        {
            Spell = (Spell) reader.ReadByte();
            Level = reader.ReadByte();
            Experience = reader.ReadUInt16();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Spell);
            writer.Write(Level);
            writer.Write(Experience);
        }
    }
    public sealed class Magic : Packet
    {
        public override short Index { get { return 84; } }

        public Spell Spell;
        public uint TargetID;
        public Point Target;
        public bool Cast;
        public byte Level;

        protected override void ReadPacket(BinaryReader reader)
        {
            Spell = (Spell)reader.ReadByte();
            TargetID = reader.ReadUInt32();
            Target = new Point(reader.ReadInt32(), reader.ReadInt32());
            Cast = reader.ReadBoolean();
            Level = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Spell);
            writer.Write(TargetID);
            writer.Write(Target.X);
            writer.Write(Target.Y);
            writer.Write(Cast);
            writer.Write(Level);
        }
    }
    public sealed class ObjectMagic : Packet
    {
        public override short Index { get { return 85; } }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;

        public Spell Spell;
        public uint TargetID;
        public Point Target;
        public bool Cast;
        public byte Level;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();

            Spell = (Spell)reader.ReadByte();
            TargetID = reader.ReadUInt32();
            Target = new Point(reader.ReadInt32(), reader.ReadInt32());
            Cast = reader.ReadBoolean();
            Level = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);

            writer.Write((byte)Spell);
            writer.Write(TargetID);
            writer.Write(Target.X);
            writer.Write(Target.Y);
            writer.Write(Cast);
            writer.Write(Level);
        }
    }
    public sealed class ObjectEffect : Packet
    {
        public override short Index { get { return 86; } }

        public uint ObjectID;
        public SpellEffect Effect;
        
        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Effect = (SpellEffect) reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write((byte) Effect);
        }
    }
    public sealed class Pushed : Packet
    {
        public override short Index
        {
            get { return 87; }
        }

        public Point Location;
        public MirDirection Direction;


        protected override void ReadPacket(BinaryReader reader)
        {
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class ObjectPushed : Packet
    {
        public override short Index
        {
            get { return 88; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class ObjectName : Packet
    {
        public override short Index { get { return 89; } }

        public uint ObjectID;
        public string Name = string.Empty;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Name);
        }
    }
    public sealed class UserStorage : Packet
    {
        public override short Index { get { return 90; } }

        public UserItem[] Storage;

        protected override void ReadPacket(BinaryReader reader)
        {
            if (!reader.ReadBoolean()) return;

            Storage = new UserItem[reader.ReadInt32()];
            for (int i = 0; i < Storage.Length; i++)
            {
                if (!reader.ReadBoolean()) continue;
                Storage[i] = new UserItem(reader);
            }
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Storage != null);
            if (Storage == null) return;

            writer.Write(Storage.Length);
            for (int i = 0; i < Storage.Length; i++)
            {
                writer.Write(Storage[i] != null);
                if (Storage[i] == null) continue;

                Storage[i].Save(writer);
            }
        }
    }
    public sealed class SwitchGroup : Packet
    {
        public override short Index { get { return 91; } }

        public bool AllowGroup;
        protected override void ReadPacket(BinaryReader reader)
        {
            AllowGroup = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AllowGroup);
        }
    }
    public sealed class DeleteGroup : Packet
    {
        public override short Index { get { return 92; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class DeleteMember : Packet
    {
        public override short Index { get { return 93; } }

        public string Name = string.Empty;
        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
    public sealed class GroupInvite : Packet
    {
        public override short Index { get { return 94; } }

        public string Name = string.Empty;
        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
    public sealed class AddMember : Packet
    {
        public override short Index { get { return 95; } }

        public string Name = string.Empty;
        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
        }
    }
    public sealed class Revived : Packet
    {
        public override short Index { get { return 96; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class ObjectRevived : Packet
    {
        public override short Index { get { return 97; } }
        public uint ObjectID;
        public bool Effect;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Effect = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Effect);
        }
    }
    public sealed class SpellToggle : Packet
    {
        public override short Index { get { return 98; } }
        public Spell Spell;
        public bool CanUse;

        protected override void ReadPacket(BinaryReader reader)
        {
            Spell = (Spell) reader.ReadByte();
            CanUse = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Spell);
            writer.Write(CanUse);
        }
    }
    public sealed class ObjectHealth : Packet
    {
        public override short Index { get { return 99; } }
        public uint ObjectID;
        public byte Percent, Expire;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Percent = reader.ReadByte();
            Expire = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Percent);
            writer.Write(Expire);
        }
    }
    public sealed class MapEffect : Packet
    {
        public override short Index { get { return 100; } }

        public Point Location;
        public SpellEffect Effect;

        protected override void ReadPacket(BinaryReader reader)
        {
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Effect = (SpellEffect)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Effect);
        }
    }
    public sealed class ObjectRangeAttack : Packet
    {
        public override short Index
        {
            get { return 101; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;
        public uint TargetID;
        public byte Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
            TargetID = reader.ReadUInt32();
            Type = reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
            writer.Write(TargetID);
            writer.Write(Type);
        }
    }
    public sealed class AddBuff : Packet
    {
        public override short Index { get { return 102; } }

        public BuffType Type;
        public string Caster = string.Empty;
        public long Expire;
        public int Value;
        public bool Infinite;

        protected override void ReadPacket(BinaryReader reader)
        {
            Type = (BuffType)reader.ReadByte();
            Caster = reader.ReadString();
            Expire = reader.ReadInt64();
            Value = reader.ReadInt32();
            Infinite = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Type);
            writer.Write(Caster);
            writer.Write(Expire);
            writer.Write(Value);
            writer.Write(Infinite);
        }
    }
    public sealed class RemoveBuff : Packet
    {
        public override short Index { get { return 103; } }

        public BuffType Type;

        protected override void ReadPacket(BinaryReader reader)
        {
            Type = (BuffType)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Type);
        }
    }
    public sealed class ObjectHidden :Packet
    {
        public override short Index { get { return 104; } }
        public uint ObjectID;
        public bool Hidden;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Hidden = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Hidden);
        }
    }
    public sealed class RefreshItem : Packet
    {
        public override short Index { get { return 105; } }
        public UserItem Item;
        protected override void ReadPacket(BinaryReader reader)
        {
            Item = new UserItem(reader);
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            Item.Save(writer);
        }
    }
    public sealed class ObjectSpell :Packet
    {
        public override short Index
        {
            get { return 106; }
        }

        public uint ObjectID;
        public Point Location;
        public Spell Spell;
        public MirDirection Direction;


        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Spell = (Spell) reader.ReadByte();
            Direction = (MirDirection) reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte) Spell);
            writer.Write((byte)Direction);
        }
    }
    public sealed class UserDash : Packet
    {
        public override short Index
        {
            get { return 107; }
        }

        public Point Location;
        public MirDirection Direction;


        protected override void ReadPacket(BinaryReader reader)
        {
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class ObjectDash : Packet
    {
        public override short Index
        {
            get { return 108; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;


        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class UserDashFail : Packet
    {
        public override short Index
        {
            get { return 109; }
        }

        public Point Location;
        public MirDirection Direction;


        protected override void ReadPacket(BinaryReader reader)
        {
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class ObjectDashFail : Packet
    {
        public override short Index
        {
            get { return 110; }
        }

        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;


        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
        }

        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
        }
    }
    public sealed class NPCConsign : Packet
    {
        public override short Index { get { return 111; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class NPCMarket : Packet
    {
        public override short Index { get { return 112; } }

        public List<ClientAuction> Listings = new List<ClientAuction>();
        public int Pages;
        public bool UserMode;

        protected override void ReadPacket(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Listings.Add(new ClientAuction(reader));

            Pages = reader.ReadInt32();
            UserMode = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Listings.Count);

            for (int i = 0; i < Listings.Count; i++)
                Listings[i].Save(writer);

            writer.Write(Pages);
            writer.Write(UserMode);
        }
    }
    public sealed class NPCMarketPage : Packet
    {
        public override short Index { get { return 113; } }

        public List<ClientAuction> Listings = new List<ClientAuction>();

        protected override void ReadPacket(BinaryReader reader)
        {
            int count = reader.ReadInt32();

            for (int i = 0; i < count; i++)
                Listings.Add(new ClientAuction(reader));
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Listings.Count);

            for (int i = 0; i < Listings.Count; i++)
                Listings[i].Save(writer);
        }
    }
    public sealed class ConsignItem : Packet
    {
        public override short Index { get { return 114; } }

        public ulong UniqueID;
        public bool Success;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Success = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Success);
        }
    }
    public sealed class MarketFail : Packet
    {
        public override short Index { get { return 115; } }

        public byte Reason;

        /*
         * 0: Dead.
         * 1: Not talking to TrustMerchant.
         * 2: Already Sold.
         * 3: Expired.
         * 4: Not enough Gold.
         * 5: Too heavy or not enough bag space.
         * 6: You cannot buy your own items.
         * 7: Trust Merchant is too far.
         * 8: Too much Gold.
         */

        protected override void ReadPacket(BinaryReader reader)
        {
            Reason = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Reason);
        }
    }
    public sealed class MarketSuccess : Packet
    {
        public override short Index { get { return 116; } }

        public string Message = string.Empty;
        
        protected override void ReadPacket(BinaryReader reader)
        {
            Message = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Message);
        }
    }
    public sealed class ObjectSitDown : Packet
    {
        public override short Index { get { return 117; } }
        public uint ObjectID;
        public Point Location;
        public MirDirection Direction;
        public bool Sitting;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
            Direction = (MirDirection)reader.ReadByte();
            Sitting = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
            writer.Write((byte)Direction);
            writer.Write(Sitting);
        }
    }
    public sealed class InTrapRock : Packet
    {
        public override short Index { get { return 118; } }
        public bool Trapped;

        protected override void ReadPacket(BinaryReader reader)
        {
            Trapped = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Trapped);
        }
    }
}
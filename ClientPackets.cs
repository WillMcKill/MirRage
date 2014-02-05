using System;
using System.Drawing;
using System.IO;

namespace ClientPackets
{
    public sealed class ClientVersion : Packet
    {
        public override short Index
        {
            get { return 0; }
        }

        public byte[] VersionHash;

        protected override void ReadPacket(BinaryReader reader)
        {
            VersionHash = reader.ReadBytes(reader.ReadInt32());
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(VersionHash.Length);
            writer.Write(VersionHash);
        }
    }
    public sealed class Disconnect : Packet
    {
        public override short Index
        {
            get { return 1; }
        }
        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class KeepAlive : Packet
    {
        public override short Index
        {
            get { return 2; }
        }
        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class NewAccount: Packet
    {
        public override short Index
        {
            get { return 3; }
        }

        public string AccountID = string.Empty;
        public string Password = string.Empty;
        public DateTime BirthDate;
        public string UserName = string.Empty;
        public string SecretQuestion = string.Empty;
        public string SecretAnswer = string.Empty;
        public string EMailAddress = string.Empty;

        protected override void ReadPacket(BinaryReader reader)
        {
            AccountID = reader.ReadString();
            Password = reader.ReadString();
            BirthDate = DateTime.FromBinary(reader.ReadInt64());
            UserName = reader.ReadString();
            SecretQuestion = reader.ReadString();
            SecretAnswer = reader.ReadString();
            EMailAddress = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AccountID);
            writer.Write(Password);
            writer.Write(BirthDate.ToBinary());
            writer.Write(UserName);
            writer.Write(SecretQuestion);
            writer.Write(SecretAnswer);
            writer.Write(EMailAddress);
        }
    }
    public sealed class ChangePassword: Packet
    {
        public override short Index
        {
            get { return 4; }
        }

        public string AccountID = string.Empty;
        public string CurrentPassword = string.Empty;
        public string NewPassword = string.Empty;

        protected override void ReadPacket(BinaryReader reader)
        {
            AccountID = reader.ReadString();
            CurrentPassword = reader.ReadString();
            NewPassword = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AccountID);
            writer.Write(CurrentPassword);
            writer.Write(NewPassword);
        }
    }
    public sealed class Login : Packet
    {
        public override short Index
        {
            get { return 5; }
        }

        public string AccountID = string.Empty;
        public string Password = string.Empty;

        protected override void ReadPacket(BinaryReader reader)
        {
            AccountID = reader.ReadString();
            Password = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AccountID);
            writer.Write(Password);
        }
    }
    public sealed class NewCharacter : Packet
    {
        public override short Index { get { return 6; } }

        public string Name = string.Empty;
        public MirGender Gender;
        public MirClass Class;
        protected override void ReadPacket(BinaryReader reader)
        {
            Name = reader.ReadString();
            Gender = (MirGender)reader.ReadByte();
            Class = (MirClass)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write((byte)Gender);
            writer.Write((byte)Class);
        }
    }
    public sealed class DeleteCharacter : Packet
    {
        public override short Index { get { return 7; } }

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
        public override short Index { get { return 8; } }

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
    public sealed class LogOut : Packet
    {
        public override short Index { get { return 9; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class Turn : Packet
    {
        public override short Index { get { return 10; } }

        public MirDirection Direction;

        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (MirDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
        }
    }
    public sealed class Walk : Packet
    {
        public override short Index { get { return 11; } }

        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (MirDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
        }
    }
    public sealed class Run : Packet
    {
        public override short Index { get { return 12; } }

        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (MirDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
        }
    }
    public sealed class Chat : Packet
    {
        public override short Index { get { return 13; } }

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
    public sealed class MoveItem : Packet
    {
        public override short Index { get { return 14; } }

        public MirGridType Grid;
        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(From);
            writer.Write(To);
        }
    }
    public sealed class StoreItem : Packet
    {
        public override short Index { get { return 15; } }

        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
        }
    }
    public sealed class TakeBackItem : Packet
    {
        public override short Index { get { return 16; } }

        public int From, To;
        protected override void ReadPacket(BinaryReader reader)
        {
            From = reader.ReadInt32();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(From);
            writer.Write(To);
        }
    }
    public sealed class MergeItem : Packet
    {
        public override short Index { get { return 17; } }

        public MirGridType GridFrom, GridTo;
        public ulong IDFrom, IDTo;
        protected override void ReadPacket(BinaryReader reader)
        {
            GridFrom = (MirGridType)reader.ReadByte();
            GridTo = (MirGridType)reader.ReadByte();
            IDFrom = reader.ReadUInt64();
            IDTo = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)GridFrom);
            writer.Write((byte)GridTo);
            writer.Write(IDFrom);
            writer.Write(IDTo);
        }
    }
    public sealed class EquipItem : Packet
    {
        public override short Index { get { return 18; } }

        public MirGridType Grid;
        public ulong UniqueID;
        public int To;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(UniqueID);
            writer.Write(To);
        }
    }
    public sealed class RemoveItem : Packet
    {
        public override short Index { get { return 19; } }

        public MirGridType Grid;
        public ulong UniqueID;
        public int To;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            To = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(UniqueID);
            writer.Write(To);
        }
    }
    public sealed class SplitItem : Packet
    {
        public override short Index { get { return 20; } }

        public MirGridType Grid;
        public ulong UniqueID;
        public uint Count;

        protected override void ReadPacket(BinaryReader reader)
        {
            Grid = (MirGridType)reader.ReadByte();
            UniqueID = reader.ReadUInt64();
            Count = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Grid);
            writer.Write(UniqueID);
            writer.Write(Count);
        }
    }
    public sealed class UseItem : Packet
    {
        public override short Index { get { return 21; } }

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
    public sealed class DropItem : Packet
    {
        public override short Index { get { return 22; } }

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
    public sealed class DropGold : Packet
    {
        public override short Index { get { return 23; } }

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
    public sealed class PickUp : Packet
    {
        public override short Index { get { return 24; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class Inspect : Packet
    {
        public override short Index
        {
            get { return 25; }
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
    public sealed class ChangeAMode : Packet
    {
        public override short Index { get { return 26; } }

        public AttackMode Mode;

        protected override void ReadPacket(BinaryReader reader)
        {
            Mode = (AttackMode)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Mode);
        }
    }
    public sealed class ChangePMode : Packet
    {
        public override short Index { get { return 27; } }

        public PetMode Mode;

        protected override void ReadPacket(BinaryReader reader)
        {
            Mode = (PetMode)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Mode);
        }
    }
    public sealed class Attack : Packet
    {
        public override short Index { get { return 28; } }

        public MirDirection Direction;
        public Spell Spell;

        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (MirDirection) reader.ReadByte();
            Spell = (Spell) reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
            writer.Write((byte)Spell);
        }
    }
    public sealed class Harvest : Packet
    {
        public override short Index { get { return 29; } }

        public MirDirection Direction;
        protected override void ReadPacket(BinaryReader reader)
        {
            Direction = (MirDirection)reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Direction);
        }
    }
    public sealed class CallNPC : Packet
    {
        public override short Index { get { return 30; } }

        public uint ObjectID;
        public string Key = string.Empty;

        protected override void ReadPacket(BinaryReader reader)
        {
            ObjectID = reader.ReadUInt32();
            Key = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ObjectID);
            writer.Write(Key);
        }
    }
    public sealed class BuyItem : Packet
    {
        public override short Index { get { return 31; } }

        public int ItemIndex;
        public uint Count;

        protected override void ReadPacket(BinaryReader reader)
        {
            ItemIndex = reader.ReadInt32();
            Count = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(ItemIndex);
            writer.Write(Count);
        }
    }
    public sealed class SellItem : Packet
    {
        public override short Index { get { return 32; } }

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
    public sealed class RepairItem : Packet
    {
        public override short Index { get { return 33; } }

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
    public sealed class BuyItemBack : Packet
    {
        public override short Index { get { return 34; } }

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
    public sealed class SRepairItem : Packet
    {
        public override short Index { get { return 35; } }

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
    public sealed class MagicKey : Packet
    {
        public override short Index { get { return 36; } }

        public Spell Spell;
        public byte Key;

        protected override void ReadPacket(BinaryReader reader)
        {
            Spell = (Spell) reader.ReadByte();
            Key = reader.ReadByte();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Spell);
            writer.Write(Key);
        }
    }
    public sealed class Magic : Packet
    {
        public override short Index { get { return 37; } }

        public Spell Spell;
        public MirDirection Direction;
        public uint TargetID;
        public Point Location;

        protected override void ReadPacket(BinaryReader reader)
        {
            Spell = (Spell) reader.ReadByte();
            Direction = (MirDirection)reader.ReadByte();
            TargetID = reader.ReadUInt32();
            Location = new Point(reader.ReadInt32(), reader.ReadInt32());
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte) Spell);
            writer.Write((byte)Direction);
            writer.Write(TargetID);
            writer.Write(Location.X);
            writer.Write(Location.Y);
        }
    }
    public sealed class SwitchGroup : Packet
    {
        public override short Index { get { return 38; } }

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
    public sealed class AddMember : Packet
    {
        public override short Index { get { return 39; } }

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
    public sealed class DelMember : Packet
    {
        public override short Index { get { return 40; } }

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
        public override short Index { get { return 41; } }

        public bool AcceptInvite;
        protected override void ReadPacket(BinaryReader reader)
        {
            AcceptInvite = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AcceptInvite);
        }
    }
    public sealed class TownRevive : Packet
    {
        public override short Index { get { return 42; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class SpellToggle : Packet
    {
        public override short Index { get { return 43; } }
        public Spell Spell;
        public bool CanUse;

        protected override void ReadPacket(BinaryReader reader)
        {
            Spell = (Spell)reader.ReadByte();
            CanUse = reader.ReadBoolean();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write((byte)Spell);
            writer.Write(CanUse);
        }
    }
    public sealed class ConsignItem : Packet
    {
        public override short Index { get { return 44; } }

        public ulong UniqueID;
        public uint Price;

        protected override void ReadPacket(BinaryReader reader)
        {
            UniqueID = reader.ReadUInt64();
            Price = reader.ReadUInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(UniqueID);
            writer.Write(Price);
        }
    }
    public sealed class MarketSearch : Packet
    {
        public override short Index { get { return 45; } }

        public string Match = string.Empty;
        protected override void ReadPacket(BinaryReader reader)
        {
            Match = reader.ReadString();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Match);
        }
    }
    public sealed class MarketRefresh : Packet
    {
        public override short Index { get { return 46; } }

        protected override void ReadPacket(BinaryReader reader)
        {
        }
        protected override void WritePacket(BinaryWriter writer)
        {
        }
    }
    public sealed class MarketPage : Packet
    {
        public override short Index { get { return 47; } }
        public int Page;

        protected override void ReadPacket(BinaryReader reader)
        {
            Page = reader.ReadInt32();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(Page);
        }
    }
    public sealed class MarketBuy : Packet
    {
        public override short Index { get { return 48; } }

        public ulong AuctionID;

        protected override void ReadPacket(BinaryReader reader)
        {
            AuctionID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AuctionID);
        }
    }
    public sealed class MarketGetBack : Packet
    {
        public override short Index { get { return 49; } }

        public ulong AuctionID;

        protected override void ReadPacket(BinaryReader reader)
        {
            AuctionID = reader.ReadUInt64();
        }
        protected override void WritePacket(BinaryWriter writer)
        {
            writer.Write(AuctionID);
        }
    }
}

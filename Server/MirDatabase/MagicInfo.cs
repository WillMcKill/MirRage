using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Server.MirEnvir;
using S = ServerPackets;

namespace Server.MirDatabase
{
    public class MagicInfo
    {
        public static List<MagicInfo> Magics = new List<MagicInfo>();

        //Warrior
        public static MagicInfo Fencing,
                                Slaying,
                                Thrusting,
                                HalfMoon,
                                ShoulderDash,
                                TwinDrakeBlade,
                                Entrapment,
                                FlamingSword,
                                LionRoar,
                                CrossHalfMoon,
                                BladeAvalanche,
                                ProtectionField,
                                Rage,
                                CounterAttack,
                                SlashingBurst;

        //Wizard
        public static MagicInfo FireBall,
                                Repulsion,
                                ElectricShock,
                                GreatFireBall,
                                HellFire,
                                ThunderBolt,
                                Teleport,
                                FireBang,
                                FireWall,
                                Lightning,
                                FrostCrunch,
                                ThunderStorm,
                                MagicShield,
                                TurnUndead,
                                Vampirism,
                                IceStorm,
                                FlameDisruptor,
                                Mirroring,
                                FlameField,
                                Blizzard,
                                MagicBooster,
                                MeteorStrike,
                                IceThrust;

        //Taoist
        public static MagicInfo Healing,
                                SpiriSword,
                                Poisoning,
                                SoulFireBall,
                                SummonSkeleton,
                                Hiding,
                                MassHiding,
                                SoulShield,
                                Revelation,
                                BlessedArmour,
                                EnergyRepulsor,
                                TrapHexagon,
                                Purification,
                                MassHealing,
                                Hallucination,
                                UltimateEnchancer,
                                SummonShinsu,
                                Reincarnation,
                                SummonHolyDeva,
                                Curse,
                                Plague,
                                PoisonField,
                                EnergyShield;

        //Assassin
        public static MagicInfo FatalSword,
                                DoubleSlash,
                                Haste,
                                FlashDash,
                                LightBody,
                                HeavenlySword,
                                FireBurst,
                                Trap,
                                PoisonSword,
                                MoonLight,
                                MPEater,
                                SwiftFeet,
                                DarkBody,
                                Hemorrhage,
                                CresentSlash;


        public Spell Spell;
        public byte BaseCost, LevelCost, Icon;
        public byte Level1, Level2, Level3;
        public ushort Need1, Need2, Need3;

        static MagicInfo()
        {
            //Warrior
            Fencing = new MagicInfo {Spell = Spell.Fencing, Icon = 2, Level1 = 7, Level2 = 9, Level3 = 11, Need1 = 100, Need2 = 200, Need3 = 300};
            Slaying = new MagicInfo { Spell = Spell.Slaying, Icon = 6, Level1 = 15, Level2 = 17, Level3 = 20, Need1 = 100, Need2 = 200, Need3 = 300 };
            Thrusting = new MagicInfo { Spell = Spell.Thrusting, Icon = 11, Level1 = 22, Level2 = 24, Level3 = 27, Need1 = 100, Need2 = 200, Need3 = 300 };
            HalfMoon = new MagicInfo { Spell = Spell.HalfMoon, Icon = 24, Level1 = 26, Level2 = 28, Level3 = 31, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 3 };
            ShoulderDash = new MagicInfo { Spell = Spell.ShoulderDash, Icon = 26, Level1 = 30, Level2 = 32, Level3 = 34, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 4, LevelCost = 4 };
            TwinDrakeBlade = new MagicInfo { Spell = Spell.TwinDrakeBlade, Icon = 37, Level1 = 32, Level2 = 34, Level3 = 36, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 10 };
            Entrapment = new MagicInfo { Spell = Spell.Entrapment, Icon = 46, Level1 = 32, Level2 = 35, Level3 = 37, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 15, LevelCost = 3 };
            FlamingSword = new MagicInfo { Spell = Spell.FlamingSword, Icon = 25, Level1 = 35, Level2 = 37, Level3 = 10, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 7 };
            LionRoar = new MagicInfo { Spell = Spell.LionRoar, Icon = 42, Level1 = 36, Level2 = 39, Level3 = 41, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 14, LevelCost = 4 };
            CrossHalfMoon = new MagicInfo { Spell = Spell.CrossHalfMoon, Icon = 33, Level1 = 38, Level2 = 40, Level3 = 42, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 6 };
            BladeAvalanche = new MagicInfo { Spell = Spell.BladeAvalanche, Icon = 43, Level1 = 38, Level2 = 41, Level3 = 43, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 14, LevelCost = 4 };
            ProtectionField = new MagicInfo { Spell = Spell.ProtectionField, Icon = 50, Level1 = 39, Level2 = 42, Level3 = 45, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 23, LevelCost = 6 };
            Rage = new MagicInfo { Spell = Spell.Rage, Icon = 49, Level1 = 44, Level2 = 47, Level3 = 50, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 20, LevelCost = 5 };
            CounterAttack = new MagicInfo { Spell = Spell.CounterAttack, Icon = 72, Level1 = 49, Level2 = 52, Level3 = 55, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 12, LevelCost = 4 };
            SlashingBurst = new MagicInfo { Spell = Spell.SlashingBurst, Icon = 55, Level1 = 50, Level2 = 53, Level3 = 56, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 25, LevelCost = 4 };


            //Wizard
            FireBall = new MagicInfo {Spell = Spell.FireBall, Icon = 0, Level1 = 7, Level2 = 9, Level3 = 11, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 3, LevelCost = 2};
            Repulsion = new MagicInfo {Spell = Spell.Repulsion, Icon = 7, Level1 = 12, Level2 = 15, Level3 = 19, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 2, LevelCost = 2};
            ElectricShock = new MagicInfo {Spell = Spell.ElectricShock, Icon = 19, Level1 = 13, Level2 = 18, Level3 = 24, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 3, LevelCost = 1};
            GreatFireBall = new MagicInfo {Spell = Spell.GreatFireBall, Icon = 4, Level1 = 15, Level2 = 18, Level3 = 21, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 5, LevelCost = 1};
            HellFire = new MagicInfo {Spell = Spell.HellFire, Icon = 8, Level1 = 16, Level2 = 20, Level3 = 24, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 10, LevelCost = 3};
            ThunderBolt = new MagicInfo {Spell = Spell.ThunderBolt, Icon = 10, Level1 = 17, Level2 = 20, Level3 = 23, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 9, LevelCost = 2};
            Teleport = new MagicInfo { Spell = Spell.Teleport, Icon = 20, Level1 = 19, Level2 = 22, Level3 = 25, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 10, LevelCost = 3 };
            FireBang = new MagicInfo { Spell = Spell.FireBang, Icon = 22, Level1 = 22, Level2 = 25, Level3 = 28, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 14, LevelCost = 4 };
            FireWall = new MagicInfo {Spell = Spell.FireWall, Icon = 21, Level1 = 24, Level2 = 28, Level3 = 33, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 30, LevelCost = 5};
            Lightning = new MagicInfo { Spell = Spell.Lightning, Icon = 9, Level1 = 26, Level2 = 29, Level3 = 32, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 38, LevelCost = 7 };
            FrostCrunch = new MagicInfo { Spell = Spell.FrostCrunch, Icon = 38, Level1 = 28, Level2 = 30, Level3 = 33, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 15, LevelCost = 3 };
            ThunderStorm = new MagicInfo { Spell = Spell.ThunderStorm, Icon = 23, Level1 = 30, Level2 = 32, Level3 = 34, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 29, LevelCost = 9 };
            MagicShield = new MagicInfo { Spell = Spell.MagicShield, Icon = 30, Level1 = 31, Level2 = 34, Level3 = 38, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 35, LevelCost = 5 };
            TurnUndead = new MagicInfo { Spell = Spell.TurnUndead, Icon = 31, Level1 = 32, Level2 = 35, Level3 = 39, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 52, LevelCost = 13 };
            Vampirism = new MagicInfo { Spell = Spell.Vampirism, Icon = 47, Level1 = 33, Level2 = 36, Level3 = 40, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 26, LevelCost = 13 };
            IceStorm = new MagicInfo { Spell = Spell.IceStorm, Icon = 32, Level1 = 35, Level2 = 37, Level3 = 40, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 33, LevelCost = 3 };
            FlameDisruptor = new MagicInfo { Spell = Spell.FlameDisruptor, Icon = 34, Level1 = 38, Level2 = 40, Level3 = 42, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 28, LevelCost = 3 };
            Mirroring = new MagicInfo { Spell = Spell.Mirroring, Icon = 41, Level1 = 41, Level2 = 43, Level3 = 45, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 21 };
            FlameField = new MagicInfo { Spell = Spell.FlameField, Icon = 44, Level1 = 42, Level2 = 43, Level3 = 45, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 45, LevelCost = 8 };
            Blizzard = new MagicInfo { Spell = Spell.Blizzard, Icon = 51, Level1 = 44, Level2 = 47, Level3 = 50, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 65, LevelCost = 10 };
            MagicBooster = new MagicInfo { Spell = Spell.MagicBooster, Icon = 73, Level1 = 47, Level2 = 49, Level3 = 52, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 150, LevelCost = 15 };
            MeteorStrike = new MagicInfo { Spell = Spell.MeteorStrike, Icon = 52, Level1 = 49, Level2 = 52, Level3 = 55, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 115, LevelCost = 17 };
            IceThrust = new MagicInfo { Spell = Spell.IceThrust, Icon = 56, Level1 = 53, Level2 = 56, Level3 = 59, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 100, LevelCost = 20 };



            //Taoist
            Healing = new MagicInfo { Spell = Spell.Healing, Icon = 1, Level1 = 7, Level2 = 9, Level3 = 11, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 3, LevelCost = 2 };
            SpiriSword = new MagicInfo { Spell = Spell.SpiritSword, Icon = 3, Level1 = 9, Level2 = 12, Level3 = 15, Need1 = 100, Need2 = 200, Need3 = 300 };
            Poisoning = new MagicInfo { Spell = Spell.Poisoning, Icon = 5, Level1 = 14, Level2 = 17, Level3 = 20, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 2, LevelCost = 1 };
            SoulFireBall = new MagicInfo { Spell = Spell.SoulFireBall, Icon = 12, Level1 = 18, Level2 = 21, Level3 = 23, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 3, LevelCost = 1 };
            SummonSkeleton = new MagicInfo { Spell = Spell.SummonSkeleton, Icon = 16, Level1 = 19, Level2 = 22, Level3 = 24, Need1 = 10, Need2 = 20, Need3 = 30, BaseCost = 12, LevelCost = 4 };
            Hiding = new MagicInfo { Spell = Spell.Hiding, Icon = 17, Level1 = 20, Level2 = 23, Level3 = 26, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 1, LevelCost = 1 };
            MassHiding = new MagicInfo { Spell = Spell.MassHiding, Icon = 18, Level1 = 21, Level2 = 25, Level3 = 29, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 2, LevelCost = 2 };
            SoulShield = new MagicInfo { Spell = Spell.SoulShield, Icon = 13, Level1 = 22, Level2 = 24, Level3 = 26, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 2, LevelCost = 2 };
            Revelation = new MagicInfo { Spell = Spell.Revelation, Icon = 27, Level1 = 23, Level2 = 25, Level3 = 28, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 4, LevelCost = 4 };
            BlessedArmour = new MagicInfo { Spell = Spell.BlessedArmour, Icon = 14, Level1 = 25, Level2 = 27, Level3 = 29, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 2, LevelCost = 2 };
            EnergyRepulsor = new MagicInfo { Spell = Spell.EnergyRepulsor, Icon = 36, Level1 = 27, Level2 = 29, Level3 = 31, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 2, LevelCost = 2 };
            TrapHexagon = new MagicInfo { Spell = Spell.TrapHexagon, Icon = 15, Level1 = 28, Level2 = 30, Level3 = 32, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 7, LevelCost = 3 };
            Purification = new MagicInfo { Spell = Spell.Purification, Icon = 39, Level1 = 30, Level2 = 32, Level3 = 35, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 14, LevelCost = 2 };
            MassHealing = new MagicInfo { Spell = Spell.MassHealing, Icon = 28, Level1 = 31, Level2 = 33, Level3 = 36, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 28, LevelCost = 3 };
            Hallucination = new MagicInfo { Spell = Spell.Hallucination, Icon = 48, Level1 = 32, Level2 = 34, Level3 = 36, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 22, LevelCost = 10 };
            UltimateEnchancer = new MagicInfo { Spell = Spell.UltimateEnhancer, Icon = 35, Level1 = 33, Level2 = 35, Level3 = 38, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 28, LevelCost = 4 };
            SummonShinsu = new MagicInfo { Spell = Spell.SummonShinsu, Icon = 29, Level1 = 35, Level2 = 37, Level3 = 40, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 28, LevelCost = 4 };
            Reincarnation = new MagicInfo { Spell = Spell.Reincarnation, Icon = 53, Level1 = 37, Level2 = 39, Level3 = 41, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 125, LevelCost = 17 };
            SummonHolyDeva = new MagicInfo { Spell = Spell.SummonHolyDeva, Icon = 40, Level1 = 38, Level2 = 41, Level3 = 43, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 28, LevelCost = 4 };
            Curse = new MagicInfo { Spell = Spell.Curse, Icon = 45, Level1 = 40, Level2 = 42, Level3 = 44, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 17, LevelCost = 3 };
            Plague = new MagicInfo { Spell = Spell.Plague, Icon = 74, Level1 = 42, Level2 = 44, Level3 = 47, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 20, LevelCost = 5 };
            PoisonField = new MagicInfo { Spell = Spell.PoisonField, Icon = 54, Level1 = 43, Level2 = 45, Level3 = 48, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 30, LevelCost = 5 };
            EnergyShield = new MagicInfo { Spell = Spell.EnergyShield, Icon = 47, Level1 = 48, Level2 = 51, Level3 = 54, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 50, LevelCost = 20 };




            //Assassin
            FatalSword = new MagicInfo { Spell = Spell.FatalSword, Icon = 58, Level1 = 7, Level2 = 9, Level3 = 11, Need1 = 100, Need2 = 200, Need3 = 300 };
            DoubleSlash = new MagicInfo { Spell = Spell.DoubleSlash, Icon = 59, Level1 = 15, Level2 = 17, Level3 = 19, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 2, LevelCost = 1 };
            Haste = new MagicInfo { Spell = Spell.Haste, Icon = 60, Level1 = 20, Level2 = 22, Level3 = 25, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 3, LevelCost = 2 };
            FlashDash = new MagicInfo { Spell = Spell.FlashDash, Icon = 61, Level1 = 25, Level2 = 27, Level3 = 30, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 12, LevelCost = 2 };
            LightBody = new MagicInfo { Spell = Spell.LightBody, Icon = 68, Level1 = 27, Level2 = 29, Level3 = 32, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 11, LevelCost = 2 };
            HeavenlySword = new MagicInfo { Spell = Spell.HeavenlySword, Icon = 62, Level1 = 30, Level2 = 32, Level3 = 35, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 13, LevelCost = 2 };
            FireBurst = new MagicInfo { Spell = Spell.FireBurst, Icon = 63, Level1 = 33, Level2 = 35, Level3 = 38, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 10, LevelCost = 1 };
            Trap = new MagicInfo { Spell = Spell.Trap, Icon = 64, Level1 = 33, Level2 = 35, Level3 = 38, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 14, LevelCost = 2 };
            PoisonSword = new MagicInfo { Spell = Spell.PoisonSword, Icon = 69, Level1 = 34, Level2 = 36, Level3 = 39, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 14, LevelCost = 3 };
            MoonLight = new MagicInfo { Spell = Spell.MoonLight, Icon = 65, Level1 = 36, Level2 = 39, Level3 = 42, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 36, LevelCost = 3 };
            MPEater = new MagicInfo { Spell = Spell.MPEater, Icon = 66, Level1 = 38, Level2 = 41, Level3 = 44, Need1 = 100, Need2 = 200, Need3 = 300 };
            SwiftFeet = new MagicInfo { Spell = Spell.SwiftFeet, Icon = 67, Level1 = 40, Level2 = 43, Level3 = 46, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 17, LevelCost = 5 };
            DarkBody = new MagicInfo { Spell = Spell.DarkBody, Icon = 70, Level1 = 46, Level2 = 49, Level3 = 52, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 40, LevelCost = 7 };
            Hemorrhage = new MagicInfo { Spell = Spell.Hemorrhage, Icon = 75, Level1 = 47, Level2 = 50, Level3 = 53, Need1 = 100, Need2 = 200, Need3 = 300 };
            CresentSlash = new MagicInfo { Spell = Spell.CrescentSlash, Icon = 71, Level1 = 50, Level2 = 53, Level3 = 56, Need1 = 100, Need2 = 200, Need3 = 300, BaseCost = 19, LevelCost = 5 };

        }

        public MagicInfo()
        {
            Magics.Add(this);
        }

        public static MagicInfo GetMagicInfo(Spell spell)
        {
            for (int i = 0; i < Magics.Count; i++)
            {
                MagicInfo info = Magics[i];
                if (info.Spell != spell) continue;
                return info;
            }

            return null;
        }
    }

    public class UserMagic
    {
        public Spell Spell;
        public MagicInfo Info;

        public byte Level, Key;
        public ushort Experience;
        public bool IsTempSpell;

        public UserMagic(Spell spell)
        {
            Spell = spell;
            Info = MagicInfo.GetMagicInfo(Spell);
        }
        public UserMagic(BinaryReader reader)
        {
            Spell = (Spell) reader.ReadByte();
            Info = MagicInfo.GetMagicInfo(Spell);

            Level = reader.ReadByte();
            Key = reader.ReadByte();
            Experience = reader.ReadUInt16();

            if (Envir.LoadVersion < 15) return;
            IsTempSpell = reader.ReadBoolean();
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write((byte) Spell);

            writer.Write(Level);
            writer.Write(Key);
            writer.Write(Experience);
            writer.Write(IsTempSpell);
        }

        public Packet GetInfo()
        {
            return new S.NewMagic
                {
                    Magic = CreateClientMagic()
                };
        }

        public ClientMagic CreateClientMagic()
        {
            return new ClientMagic
                {
                    Spell = Spell,
                    BaseCost = Info.BaseCost,
                    LevelCost = Info.LevelCost,
                    Icon = Info.Icon,
                    Level1 = Info.Level1,
                    Level2 = Info.Level2,
                    Level3 = Info.Level3,
                    Need1 = Info.Need1,
                    Need2 = Info.Need2,
                    Need3 = Info.Need3,
                    Level = Level,
                    Key = Key,
                    Experience = Experience,
                    IsTempSpell = IsTempSpell,
                };
        }


        public int GetPower()
        {
            return (int)Math.Round((MPower() / 4F) * (Level + 1) + DefPower());
        }

        public int MPower()
        {
            switch (Spell)
            {
                case Spell.FireBall:
                    return 8;
                case Spell.GreatFireBall:
                    return 6;
                case Spell.HellFire:
                    return 14;
                case Spell.ThunderBolt:
                    return SMain.Envir.Random.Next(8, 28);
                case Spell.FireBang:
                    return 8;
                case Spell.FireWall:
                    return 3;
                case Spell.Lightning:
                    return 12;
                case Spell.ThunderStorm:
                    return SMain.Envir.Random.Next(10, 30);
                case Spell.IceStorm:
                    return 12;
                case Spell.FlameDisruptor:
                    return SMain.Envir.Random.Next(15, 35);
                case Spell.FrostCrunch:
                    return 12;
                case Spell.Vampirism:
                    return 12;
                case Spell.FlameField:
                    return 100;


                case Spell.Healing:
                    return 14;
                case Spell.Poisoning:
                    return 6;
                case Spell.SoulFireBall:
                    return 8;
                case Spell.MassHealing:
                    return 10;
                case Spell.PoisonField:
                    return 40;



                default:
                    return 0;
            }
        }
        public int DefPower()
        {
            switch (Spell)
            {
                case Spell.FireBall:
                    return 2;
                case Spell.GreatFireBall:
                    return 10;
                case Spell.HellFire:
                    return 6;
                case Spell.ThunderBolt:
                    return 9;
                case Spell.FireBang:
                    return 8;
                case Spell.FireWall:
                    return 3;
                case Spell.Lightning:
                    return 12;
                case Spell.ThunderStorm:
                    return SMain.Envir.Random.Next(10, 30);
                case Spell.IceStorm:
                    return 14;
                case Spell.FlameDisruptor:
                    return 9;
                case Spell.FrostCrunch:
                    return 12;
                case Spell.Vampirism:
                    return 12;
                case Spell.FlameField:
                    return 25;


                case Spell.SoulFireBall:
                    return 3;
                case Spell.MassHealing:
                    return 4;
                case Spell.PoisonField:
                    return 20;


                default:
                    return 0;
            }
        }

        public int GetPower(int power)
        {
            return (int)Math.Round(power / 4F * (Level + 1) + DefPower());
        }
    }
}

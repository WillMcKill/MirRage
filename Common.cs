using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using C = ClientPackets;
using S = ServerPackets;


public enum Monster : ushort
{
    Guard = 0,
    Guard1 = 1,
    Guard2 = 2,
    Hen = 3,
    Deer = 4,
    Scarecrow = 5,
    HookingCat = 6,
    RakingCat = 7,
    Yob = 8,
    Oma = 9,
    CannibalPlant = 10,
    ForestYeti = 11,
    SpittingSpider = 12,
    ChestnutTree = 13,
    EbonyTree = 14,
    LargeMushroom = 15,
    CherryTree = 16,
    OmaFighter = 17,
    OmaWarrior = 18,
    CaveBat = 19,
    CaveMaggot = 20,
    Scorpion = 21,
    Skeleton = 22,
    BoneFighter = 23,
    AxeSkeleton = 24,
    BoneWarrior = 25,
    BoneElite = 26,
    Dung = 27,
    Dark = 28,
    WoomaSoldier = 29,
    WoomaFighter = 30,
    WoomaWarrior = 31,
    FlamingWooma = 32,
    WoomaGuardian = 33,
    WoomaTaurus = 34,
    WhimperingBee = 35,
    GiantWorm = 36,
    Centipede = 37,
    BlackMaggot = 38,
    Tongs = 39,
    EvilTongs = 40,
    EvilCentipede = 41,
    BugBat = 42,
    BugBatMaggot = 43,
    WedgeMoth = 44,
    RedBoar = 45,
    BlackBoar = 46,
    SnakeScorpion = 47,
    WhiteBoar = 48,
    EvilSnake = 49,
    BombSpider = 50,
    RootSpider = 51,
    SpiderBat = 52,
    VenomSpider = 53,
    GangSpider = 54,
    GreatSpider = 55,
    LureSpider = 56,
    BigApe = 57,
    EvilApe = 58,
    GrayEvilApe = 59,
    RedEvilApe = 60,
    CrystalSpider = 61,
    RedMoonEvil = 62,
    BigRat = 63,
    ZumaArcher = 64,
    ZumaStatue = 65,
    ZumaGuardian = 66,
    RedThunderZuma = 67,
    ZumaTaurus = 68,
    DigOutZombie = 69,
    ClZombie = 70,
    NdZombie = 71,
    CrawlerZombie = 72,
    ShamanZombie = 73,
    Ghoul = 74,
    KingScorpion = 75,
    KingHog = 76,
    DarkDevil = 77,
    BoneFamiliar = 78,
    Shinsu = 79,
    Shinsu1 = 80,
    SpiderFrog = 81,
    HoroBlaster = 82,
    BlueHoroBlaster = 83,
    KekTal = 84,
    VioletKekTal = 85,
    Khazard = 86,
    RoninGhoul = 87,
    ToxicGhoul = 88,
    BoneCaptain = 89,
    BoneSpearman = 90,
    BoneBlademan = 91,
    BoneArcher = 92,
    BoneLord = 93,
    Minotaur = 94,
    IceMinotaur = 95,
    ElectricMinotaur = 96,
    WindMinotaur = 97,
    FireMinotaur = 98,
    RightGuard = 99,
    LeftGuard = 100,
    MinotaurKing = 101,
    FrostTiger = 102,
    Sheep = 103,
    Wolf = 104,
    ShellNipper = 105,
    Keratoid = 106,
    GiantKeratoid = 107,
    SkyStinger = 108,
    SandWorm = 109,
    VisceralWorm = 110,
    RedSnake = 111,
    TigerSnake = 112,
    Yimoogi = 113,
    GiantWhiteSnake = 114,
    BlueSnake = 115,
    YellowSnake = 116,
    HolyDeva = 117,
    AxeOma = 118,
    SwordOma = 119,
    CrossbowOma = 120,
    WingedOma = 121,
    FlailOma = 122,
    OmaGuard = 123,
    YinDevilNode = 124,
    YangDevilNode = 125,
    OmaKing = 126,
    BlackFoxman = 127,
    RedFoxman = 128,
    WhiteFoxman = 129,
    TrapRock = 130,
    GuardianRock = 131,
    ThunderElement = 132,
    CloudElement = 133,
    GreatFoxSpirit = 134,
    HedgeKekTal = 135,
    BigHedgeKekTal = 136,
    EvilMir = 137,
    EvilMirBody = 138,
    DragonStatue = 139,
    RedFrogSpider = 140,
    BrownFrogSpider = 141,
}

public enum MirAction : byte
{
    Standing,
    Walking,
    Running,
    Pushed,
    DashL,
    DashR,
    DashFail,
    Stance,
    Stance2,
    Attack1,
    Attack2,
    Attack3,
    Attack4,
    AttackRange,
    AttackRange2,
    Special,
    Struck,
    Harvest,
    Spell,
    Die,
    Dead,
    Skeleton,
    Show,
    Hide,
    Stoned,
    Appear,
    Revive,
    SitDown,
}

public enum CellAttribute : byte
{
    Walk = 0,
    HighWall = 1,
    LowWall = 2,
}
public enum LightSetting : byte
{
    Normal = 0,
    Dawn = 1,
    Day = 2,
    Evening = 3,
    Night = 4
}
[Obfuscation(Feature = "renaming", Exclude = true)]
public enum MirGender : byte
{
    Male = 0,
    Female = 1
}
[Obfuscation(Feature = "renaming", Exclude = true)]
public enum MirClass : byte
{
    Warrior = 0,
    Wizard = 1,
    Taoist = 2,
    Assassin = 3
}
public enum MirDirection : byte
{
    Up = 0,
    UpRight = 1,
    Right = 2,
    DownRight = 3,
    Down = 4,
    DownLeft = 5,
    Left = 6,
    UpLeft = 7
}
public enum ObjectType : byte
{
    None= 0,
    Player = 1,
    Item = 2,
    Merchant = 3,
    Spell = 4,
    Monster = 5
}
public enum ChatType : byte
{
    Normal = 0,
    Shout = 1,
    System = 2,
    Hint = 3,
    Announcement = 4,
    Group = 5,
    WhisperIn = 6,
    WhisperOut = 7,
    Guild = 8,
    Experience = 9,
}
public enum ItemType : byte
{
    Nothing = 0,
    Weapon = 1,
    Armour = 2,
    Helmet = 4,
    Necklace = 5,
    Bracelet = 6,
    Ring = 7,
    Amulet = 8,
    Belt = 9,
    Boots = 10,
    Stone = 11,
    Torch = 12,
    Potion = 13,
    Ore = 14,
    Meat = 15,
    CraftingMaterial = 16,
    Scroll = 17,
    Gem = 18,
    Tiger = 19,
    Book = 20,
}
public enum MirGridType : byte
{
    None = 0,
    Inventory = 1,
    Equipment = 2,
    Trade = 3,
    Storage = 4,
    BuyBack = 5,
    DropPanel = 6,
    Inspect = 7,
    TrustMerchant = 8
}
public enum EquipmentSlot : byte
{
    Weapon = 0,
    Armour = 1,
    Helmet = 2,
    Torch = 3,
    Necklace = 4,
    BraceletL = 5,
    BraceletR = 6,
    RingL = 7,
    RingR = 8,
    Amulet = 9,
    Belt = 10,
    Boots = 11,
    Stone = 12,
}
[Obfuscation(Feature = "renaming", Exclude = true)]
public enum AttackMode : byte
{
    Peace = 0,
    Group = 1,
    Guild = 2,
    RedBrown = 3,
    All = 4
}
[Obfuscation(Feature = "renaming", Exclude = true)]
public enum PetMode : byte
{
    Both = 0,
    MoveOnly = 1,
    AttackOnly = 2,
    None = 3,
}
public enum PoisonType : byte
{
    None,
    Green,
    Red,
    Slow,
    Frozen,
    Stun,
    Paralysis
}

[Flags]
[Obfuscation(Feature = "renaming", Exclude = true)]
public enum RequiredClass : byte
{
    Warrior = 1,
    Wizard = 2,
    Taoist = 4,
    Assassin = 8,
    WarWizTao = Warrior | Wizard | Taoist,
    None = WarWizTao | Assassin
}
[Flags]
[Obfuscation(Feature = "renaming", Exclude = true)]
public enum RequiredGender : byte
{
    Male = 1,
    Female = 2,
    None = Male | Female
}
[Obfuscation(Feature = "renaming", Exclude = true)]
public enum RequiredType : byte
{
    Level = 0,
    AC = 1,
    MAC = 2,
    DC = 3,
    MC = 4,
    SC = 5,
}

[Obfuscation(Feature = "renaming", Exclude = true)]
public enum ItemSet : byte
{
    None = 0,
    Spirit = 1,
    Recall = 2,
    RedOrchid = 3,
    RedFlower = 4,
    Smash = 5,
    HwanDevil = 6,
    Purity = 7,
    FiveString = 8,
    Mundane = 9,
    NokChi = 10,
    TaoProtect = 11,
    Mir = 12,
}

[Obfuscation(Feature = "renaming", Exclude = true)]

public enum Spell : byte
{
    None = 0,

    //Warrior
    Fencing = 1,
    Slaying = 2,
    Thrusting = 3,
    HalfMoon = 4,
    ShoulderDash = 5,
    TwinDrakeBlade = 6,
    Entrapment = 7,
    FlamingSword = 8,
    LionRoar = 9,
    CrossHalfMoon = 10,
    BladeAvalanche = 11,
    ProtectionField = 12,
    Rage = 13,
    CounterAttack = 14,
    SlashingBurst = 15,

    //Wizard
    FireBall = 31,
    Repulsion = 32,
    ElectricShock = 33,
    GreatFireBall = 34,
    HellFire = 35,
    ThunderBolt = 36,
    Teleport = 37,
    FireBang = 38,
    FireWall = 39,
    Lightning = 40,
    FrostCrunch = 41,
    ThunderStorm = 42,
    MagicShield = 43,
    TurnUndead = 44,
    Vampirism = 45,
    IceStorm = 46,
    FlameDisruptor = 47,
    Mirroring = 48,
    FlameField = 49,
    Blizzard = 50,
    MagicBooster = 51,
    MeteorStrike = 52,
    IceThrust = 53,

    //Taoist
    Healing = 61,
    SpiritSword = 62,
    Poisoning = 63,
    SoulFireBall = 64,
    SummonSkeleton = 65,
    Hiding = 67,
    MassHiding = 68,
    SoulShield = 69,
    Revelation = 70,
    BlessedArmour = 71,
    EnergyRepulsor = 72,
    TrapHexagon = 73,
    Purification = 74,
    MassHealing = 75,
    Hallucination = 76,
    UltimateEnhancer = 77,
    SummonShinsu = 78,
    Reincarnation = 79,
    SummonHolyDeva = 80,
    Curse = 81,
    Plague = 82,
    PoisonField = 83,
    EnergyShield = 84,
    
    //Assassin
    FatalSword = 91,
    DoubleSlash = 92,
    Haste = 93,
    FlashDash = 94,
    LightBody = 95,
    HeavenlySword = 96,
    FireBurst = 97,
    Trap = 98,
    PoisonSword = 99,
    MoonLight = 100,
    MPEater = 101,
    SwiftFeet = 102,
    DarkBody = 103,
    Hemorrhage = 104,
    CrescentSlash = 105,

    //Map Events
    DigOutZombie = 200
}

public enum SpellEffect : byte
{
    None,
    FatalSword,
    SummonSkeleton,
    Teleport,
    Healing,
    RedMoonEvil,
    TwinDrakeBlade,
    MagicShieldUp,
    MagicShieldDown,
    FlameSwordCharge,
    GreatFoxSpirit,
    MapLightning,
    MapFire,
    Entrapment,
}

public enum BuffType : byte
{
    None,
    Teleport,
    Hiding,
    Haste,
    SoulShield,
    BlessedArmour,
    LightBody,
    UltimateEnhancer,
    ProtectionField,
    Rage,
}

public enum DefenceType : byte
{
    ACAgility,
    AC,
    MACAgility,
    MAC,
    Agility,
    Repulsion,
    None
}


public class InIReader
{
    #region Fields
    private readonly List<string> _contents;
    private readonly string _fileName;
    #endregion

    #region Constructor
    public InIReader(string fileName)
    {
        _fileName = fileName;

        _contents = new List<string>();
        try
        {
            if (File.Exists(_fileName))
                _contents.AddRange(File.ReadAllLines(_fileName));
        }
        catch
        {
        }
    }
    #endregion

    #region Functions
    private string FindValue(string section, string key)
    {
        for (int a = 0; a < _contents.Count; a++)
            if (String.CompareOrdinal(_contents[a], "[" + section + "]") == 0)
                for (int b = a + 1; b < _contents.Count; b++)
                    if (String.CompareOrdinal(_contents[b].Split('=')[0], key) == 0)
                        return _contents[b].Split('=')[1];
                    else if (_contents[b].StartsWith("[") && _contents[b].EndsWith("]"))
                        return null;
        return null;
    }

    private int FindIndex(string section, string key)
    {
        for (int a = 0; a < _contents.Count; a++)
            if (String.CompareOrdinal(_contents[a], "[" + section + "]") == 0)
                for (int b = a + 1; b < _contents.Count; b++)
                    if (String.CompareOrdinal(_contents[b].Split('=')[0], key) == 0)
                        return b;
                    else if (_contents[b].StartsWith("[") && _contents[b].EndsWith("]"))
                    {
                        _contents.Insert(b - 1, key + "=");
                        return b - 1;
                    }
                    else if (_contents.Count - 1 == b)
                    {
                        _contents.Add(key + "=");
                        return _contents.Count - 1;
                    }
        if (_contents.Count > 0)
            _contents.Add("");

        _contents.Add("[" + section + "]");
        _contents.Add(key + "=");
        return _contents.Count - 1;
    }

    public void Save()
    {
        try
        {
            File.WriteAllLines(_fileName, _contents);
        }
        catch
        {
        }
    }
    #endregion

    #region Read
    public bool ReadBoolean(string section, string key, bool Default)
    {
        bool result;

        if (!bool.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public byte ReadByte(string section, string key, byte Default)
    {
        byte result;

        if (!byte.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }


        return result;
    }

    public sbyte ReadSByte(string section, string key, sbyte Default)
    {
        sbyte result;

        if (!sbyte.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }


        return result;
    }

    public ushort ReadUInt16(string section, string key, ushort Default)
    {
        ushort result;

        if (!ushort.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }


        return result;
    }

    public short ReadInt16(string section, string key, short Default)
    {
        short result;

        if (!short.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }


        return result;
    }

    public uint ReadUInt32(string section, string key, uint Default)
    {
        uint result;

        if (!uint.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public int ReadInt32(string section, string key, int Default)
    {
        int result;

        if (!int.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public ulong ReadUInt64(string section, string key, ulong Default)
    {
        ulong result;

        if (!ulong.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public long ReadInt64(string section, string key, long Default)
    {
        long result;

        if (!long.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }


        return result;
    }

    public float ReadSingle(string section, string key, float Default)
    {
        float result;

        if (!float.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public double ReadDouble(string section, string key, double Default)
    {
        double result;

        if (!double.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public decimal ReadDecimal(string section, string key, decimal Default)
    {
        decimal result;

        if (!decimal.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public string ReadString(string section, string key, string Default)
    {
        string result = FindValue(section, key);

        if (string.IsNullOrEmpty(result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public char ReadChar(string section, string key, char Default)
    {
        char result;

        if (!char.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }

        return result;
    }

    public Point ReadPoint(string section, string key, Point Default)
    {
        string temp = FindValue(section, key);
        int tempX, tempY;
        if (temp == null || !int.TryParse(temp.Split(',')[0], out tempX))
        {
            Write(section, key, Default);
            return Default;
        }
        if (!int.TryParse(temp.Split(',')[1], out tempY))
        {
            Write(section, key, Default);
            return Default;
        }

        return new Point(tempX, tempY);
    }

    public Size ReadSize(string section, string key, Size Default)
    {
        string temp = FindValue(section, key);
        int tempX, tempY;
        if (!int.TryParse(temp.Split(',')[0], out tempX))
        {
            Write(section, key, Default);
            return Default;
        }
        if (!int.TryParse(temp.Split(',')[1], out tempY))
        {
            Write(section, key, Default);
            return Default;
        }

        return new Size(tempX, tempY);
    }

    public TimeSpan ReadTimeSpan(string section, string key, TimeSpan Default)
    {
        TimeSpan result;

        if (!TimeSpan.TryParse(FindValue(section, key), out result))
        {
            result = Default;
            Write(section, key, Default);
        }


        return result;
    }
    #endregion

    #region Write
    public void Write(string section, string key, bool value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, byte value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, sbyte value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, ushort value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, short value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, uint value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, int value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, ulong value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, long value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, float value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, double value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, decimal value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, string value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, char value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }

    public void Write(string section, string key, Point value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value.X + "," + value.Y;
        Save();
    }

    public void Write(string section, string key, Size value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value.Width + "," + value.Height;
        Save();
    }

    public void Write(string section, string key, TimeSpan value)
    {
        _contents[FindIndex(section, key)] = key + "=" + value;
        Save();
    }
    #endregion
}

public static class Globals
{
    public const int
        MinAccountIDLength = 3,
        MaxAccountIDLength = 15,

        MinPasswordLength = 5,
        MaxPasswordLength = 15,

        MinCharacterNameLength = 3,
        MaxCharacterNameLength = 15,
        MaxCharacterCount = 4,

        MaxChatLength = 80,

        MaxGroup = 15,

        MaxDragonLevel = 13,

        FlagIndexCount = 999,

        DataRange = 14;

    public static float Commission = 0.05F;

    public const uint SearchDelay = 500,
                      ConsignmentLength = 7,
                      ConsignmentCost = 5000,
                      MinConsignment = 5000,
                      MaxConsignment = 50000000;

}

public static class Functions
{
    public static bool CompareBytes(byte[] a, byte[] b)
    {
        if (a == b) return true;

        if (a == null || b == null || a.Length != b.Length) return false;

        for (int i = 0; i < a.Length; i++) if (a[i] != b[i]) return false;

        return  true;
    }

    public static bool TryParse(string s, out Point temp)
    {
        temp = Point.Empty;
        int tempX, tempY;
        if (String.IsNullOrWhiteSpace(s)) return false;

        string[] data = s.Split(',');
        if (data.Length <= 1) return false;

        if (!Int32.TryParse(data[0], out tempX))
            return false;

        if (!Int32.TryParse(data[1], out tempY))
            return false;

        temp = new Point(tempX, tempY);
        return true;
    }
    public static Point Subtract(this Point p1, Point p2)
    {
        return new Point(p1.X - p2.X, p1.Y - p2.Y);
    }
    public static Point Subtract(this Point p1, int x, int y)
    {
        return new Point(p1.X - x, p1.Y - y);
    }
    public static Point Add(this Point p1, Point p2)
    {
        return new Point(p1.X + p2.X, p1.Y + p2.Y);
    }
    public static Point Add(this Point p1, int x, int y)
    {
        return new Point(p1.X + x, p1.Y + y);
    }
    public static string PointToString(Point p)
    {
        return String.Format("{0}, {1}", p.X, p.Y);
    }
    public static bool InRange(Point a, Point b, int i)
    {
        return Math.Abs(a.X - b.X) <= i && Math.Abs(a.Y - b.Y) <= i;
    }


    public static MirDirection PreviousDir(MirDirection d)
    {
        switch (d)
        {
            case MirDirection.Up:
                return MirDirection.UpLeft;
            case MirDirection.UpRight:
                return MirDirection.Up;
            case MirDirection.Right:
                return MirDirection.UpRight;
            case MirDirection.DownRight:
                return MirDirection.Right;
            case MirDirection.Down:
                return MirDirection.DownRight;
            case MirDirection.DownLeft:
                return MirDirection.Down;
            case MirDirection.Left:
                return MirDirection.DownLeft;
            case MirDirection.UpLeft:
                return MirDirection.Left;
            default: return d;
        }
    }
    public static MirDirection NextDir(MirDirection d)
    {
        switch (d)
        {
            case MirDirection.Up:
                return MirDirection.UpRight;
            case MirDirection.UpRight:
                return MirDirection.Right;
            case MirDirection.Right:
                return MirDirection.DownRight;
            case MirDirection.DownRight:
                return MirDirection.Down;
            case MirDirection.Down:
                return MirDirection.DownLeft;
            case MirDirection.DownLeft:
                return MirDirection.Left;
            case MirDirection.Left:
                return MirDirection.UpLeft;
            case MirDirection.UpLeft:
                return MirDirection.Up;
            default: return d;
        }
    }
    public static MirDirection DirectionFromPoint(Point source, Point dest)
    {
        if (source.X < dest.X)
        {
            if (source.Y < dest.Y)
                return MirDirection.DownRight;
            if (source.Y > dest.Y)
                return MirDirection.UpRight;
            return MirDirection.Right;
        }

        if (source.X > dest.X)
        {
            if (source.Y < dest.Y)
                return MirDirection.DownLeft;
            if (source.Y > dest.Y)
                return MirDirection.UpLeft;
            return MirDirection.Left;
        }

        return source.Y < dest.Y ? MirDirection.Down : MirDirection.Up;
    }

    public static Size Add(this Size p1, Size p2)
    {
        return new Size(p1.Width + p2.Width, p1.Height + p2.Height);
    }
    public static Size Add(this Size p1, int width, int height)
    {
        return new Size(p1.Width + width, p1.Height + height);
    }

    public static Point PointMove(Point p, MirDirection d, int i)
    {
        switch (d)
        {
            case MirDirection.Up:
                p.Offset(0, -i);
                break;
            case MirDirection.UpRight:
                p.Offset(i, -i);
                break;
            case MirDirection.Right:
                p.Offset(i, 0);
                break;
            case MirDirection.DownRight:
                p.Offset(i, i);
                break;
            case MirDirection.Down:
                p.Offset(0, i);
                break;
            case MirDirection.DownLeft:
                p.Offset(-i, i);
                break;
            case MirDirection.Left:
                p.Offset(-i, 0);
                break;
            case MirDirection.UpLeft:
                p.Offset(-i, -i);
                break;
        }
        return p;
    }

    public static int MaxDistance(Point p1, Point p2)
    {
        return Math.Max(Math.Abs(p1.X - p2.X), Math.Abs(p1.Y - p2.Y));

    }

    public static MirDirection ReverseDirection(MirDirection dir)
    {
        switch (dir)
        {
            case MirDirection.Up:
                return MirDirection.Down;
            case MirDirection.UpRight:
                return MirDirection.DownLeft;
            case MirDirection.Right:
                return MirDirection.Left;
            case MirDirection.DownRight:
                return MirDirection.UpLeft;
            case MirDirection.Down:
                return MirDirection.Up;
            case MirDirection.DownLeft:
                return MirDirection.UpRight;
            case MirDirection.Left:
                return MirDirection.Right;
            case MirDirection.UpLeft:
                return MirDirection.DownRight;
            default:
                return dir;
        }
    }
}

public class SelectInfo
{
    public int Index;
    public string Name = string.Empty;
    public byte Level;
    public MirClass Class;
    public MirGender Gender;
    public DateTime LastAccess;
    
        public SelectInfo()
        { }
        public SelectInfo(BinaryReader reader)
        {
            Index = reader.ReadInt32();
            Name = reader.ReadString();
            Level = reader.ReadByte();
            Class = (MirClass)reader.ReadByte();
            Gender = (MirGender)reader.ReadByte();
            LastAccess = DateTime.FromBinary(reader.ReadInt64());
        }
        public void Save(BinaryWriter writer)
        {
            writer.Write(Index);
            writer.Write(Name);
            writer.Write(Level);
            writer.Write((byte)Class);
            writer.Write((byte)Gender);
            writer.Write(LastAccess.ToBinary());
        }
}

public class ItemInfo
{
    public int Index;
    public string Name = string.Empty;
    public ItemType Type;
    public RequiredType RequiredType = RequiredType.Level;
    public RequiredClass RequiredClass = RequiredClass.None;
    public RequiredGender RequiredGender = RequiredGender.None;
    public ItemSet Set;

    public sbyte Shape;
    public byte Weight, Light, RequiredAmount;

    public ushort Image, Durability;

    public uint Price, StackSize = 1;

    public byte MinAC, MaxAC, MinMAC, MaxMAC, MinDC, MaxDC, MinMC, MaxMC, MinSC, MaxSC, Accuracy, Agility, HP, MP;
    public sbyte AttackSpeed, Luck;
    public byte BagWeight, HandWeight, WearWeight;

    public bool StartItem;
    public byte Effect;


    public bool IsConsumable
    {
        get { return Type == ItemType.Potion || Type == ItemType.Scroll; }
    }
    
    public ItemInfo()
    {
    }
    public ItemInfo(BinaryReader reader, int version = int.MaxValue)
    {
        Index = reader.ReadInt32();
        Name = reader.ReadString();
        Type = (ItemType) reader.ReadByte();
        RequiredType = (RequiredType) reader.ReadByte();
        RequiredClass = (RequiredClass) reader.ReadByte();
        RequiredGender = (RequiredGender) reader.ReadByte();
        if(version >= 17) Set = (ItemSet)reader.ReadByte();

        Shape = reader.ReadSByte();
        Weight = reader.ReadByte();
        Light = reader.ReadByte();
        RequiredAmount = reader.ReadByte();

        Image = reader.ReadUInt16();
        Durability = reader.ReadUInt16();

        StackSize = reader.ReadUInt32();
        Price = reader.ReadUInt32();

        MinAC = reader.ReadByte();
        MaxAC = reader.ReadByte();
        MinMAC = reader.ReadByte();
        MaxMAC = reader.ReadByte();
        MinDC = reader.ReadByte();
        MaxDC = reader.ReadByte();
        MinMC = reader.ReadByte();
        MaxMC = reader.ReadByte();
        MinSC = reader.ReadByte();
        MaxSC = reader.ReadByte();
        HP = reader.ReadByte();
        MP = reader.ReadByte();
        Accuracy = reader.ReadByte();
        Agility = reader.ReadByte();

        Luck = reader.ReadSByte();
        AttackSpeed = reader.ReadSByte();

        StartItem = reader.ReadBoolean();

        BagWeight = reader.ReadByte();
        HandWeight = reader.ReadByte();
        WearWeight = reader.ReadByte();

        if (version >= 9) Effect = reader.ReadByte();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write(Index);
        writer.Write(Name);
        writer.Write((byte) Type);
        writer.Write((byte) RequiredType);
        writer.Write((byte) RequiredClass);
        writer.Write((byte) RequiredGender);
        writer.Write((byte) Set);

        writer.Write(Shape);
        writer.Write(Weight);
        writer.Write(Light);
        writer.Write(RequiredAmount);     

        writer.Write(Image);
        writer.Write(Durability);

        writer.Write(StackSize);
        writer.Write(Price);

        writer.Write(MinAC);
        writer.Write(MaxAC);
        writer.Write(MinMAC);
        writer.Write(MaxMAC);
        writer.Write(MinDC);
        writer.Write(MaxDC);
        writer.Write(MinMC);
        writer.Write(MaxMC);
        writer.Write(MinSC);
        writer.Write(MaxSC);
        writer.Write(HP);
        writer.Write(MP);
        writer.Write(Accuracy);
        writer.Write(Agility);

        writer.Write(Luck);
        writer.Write(AttackSpeed);

        writer.Write(StartItem);

        writer.Write(BagWeight);
        writer.Write(HandWeight);
        writer.Write(WearWeight);

        writer.Write(Effect);
    }

    public static ItemInfo FromText(string text)
    {
        string[] data = text.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (data.Length < 33) return null;

        ItemInfo info = new ItemInfo { Name = data[0] };


        if (!Enum.TryParse(data[1], out info.Type)) return null;
        if (!Enum.TryParse(data[2], out info.RequiredType)) return null;
        if (!Enum.TryParse(data[3], out info.RequiredClass)) return null;
        if (!Enum.TryParse(data[4], out info.RequiredGender)) return null;
        if (!sbyte.TryParse(data[5], out info.Shape)) return null;

        if (!byte.TryParse(data[6], out info.Weight)) return null;
        if (!byte.TryParse(data[7], out info.Light)) return null;
        if (!byte.TryParse(data[8], out info.RequiredAmount)) return null;

        if (!byte.TryParse(data[9], out info.MinAC)) return null;
        if (!byte.TryParse(data[10], out info.MaxAC)) return null;
        if (!byte.TryParse(data[11], out info.MinMAC)) return null;
        if (!byte.TryParse(data[12], out info.MaxMAC)) return null;
        if (!byte.TryParse(data[13], out info.MinDC)) return null;
        if (!byte.TryParse(data[14], out info.MaxDC)) return null;
        if (!byte.TryParse(data[15], out info.MinMC)) return null;
        if (!byte.TryParse(data[16], out info.MaxMC)) return null;
        if (!byte.TryParse(data[17], out info.MinSC)) return null;
        if (!byte.TryParse(data[18], out info.MaxSC)) return null;
        if (!byte.TryParse(data[19], out info.Accuracy)) return null;
        if (!byte.TryParse(data[20], out info.Agility)) return null;
        if (!byte.TryParse(data[21], out info.HP)) return null;
        if (!byte.TryParse(data[22], out info.MP)) return null;

        if (!sbyte.TryParse(data[23], out info.AttackSpeed)) return null;
        if (!sbyte.TryParse(data[24], out info.Luck)) return null;

        if (!byte.TryParse(data[25], out info.BagWeight)) return null;

        if (!byte.TryParse(data[26], out info.HandWeight)) return null;
        if (!byte.TryParse(data[27], out info.WearWeight)) return null;

        if (!bool.TryParse(data[28], out info.StartItem)) return null;

        if (!ushort.TryParse(data[29], out info.Image)) return null;
        if (!ushort.TryParse(data[30], out info.Durability)) return null;
        if (!uint.TryParse(data[31], out info.Price)) return null;
        if (!uint.TryParse(data[32], out info.StackSize)) return null;
        if (!byte.TryParse(data[33], out info.Effect)) return null;

        return info;

    }

    public string ToText()
    {
        return string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26}," +
                             "{27},{28},{29},{30},{31},{32},{33}",
            Name, (byte)Type, (byte)RequiredType, (byte)RequiredClass, (byte)RequiredGender, Shape, Weight, Light, RequiredAmount, MinAC, MaxAC, MinMAC, MaxMAC, MinDC, MaxDC,
            MinMC, MaxMC, MinSC, MaxSC, Accuracy, Agility, HP, MP, AttackSpeed, Luck, BagWeight, HandWeight, WearWeight, StartItem, Image, Durability, Price, 
            StackSize, Effect);
    }

    

    public override string ToString()
    {
        return string.Format("{0}: {1}", Index, Name);
    }

}
public class UserItem
{
    public ulong UniqueID;
    public int ItemIndex;

    public ItemInfo Info;
    public ushort CurrentDura, MaxDura;
    public uint Count = 1;

    public byte AC, MAC, DC, MC, SC, Accuracy, Agility, HP, MP;
    public sbyte AttackSpeed, Luck;

    public bool DuraChanged;

    public bool IsAdded
    {
        get
        {
            return AC != 0 || MAC != 0 || DC != 0 || MC != 0 || SC != 0 || Accuracy != 0 || Agility != 0 || HP != 0 || MP != 0 || AttackSpeed != 0 || Luck != 0;
        }
    }

    public uint Weight
    {
        get { return Info.Type == ItemType.Amulet ? Info.Weight : Info.Weight*Count; }
    }

    public string Name
    {
        get { return Count > 1 ? string.Format("{0} ({1})", Info.Name, Count) : Info.Name; }
        
    }

    public UserItem(ItemInfo info)
    {
        ItemIndex = info.Index;
        Info = info;
    }
    public UserItem(BinaryReader reader, int version = int.MaxValue)
    {
        UniqueID = reader.ReadUInt64();
        ItemIndex = reader.ReadInt32();

        CurrentDura = reader.ReadUInt16();
        MaxDura = reader.ReadUInt16();

        Count = reader.ReadUInt32();

        AC = reader.ReadByte();
        MAC = reader.ReadByte();
        DC = reader.ReadByte();
        MC = reader.ReadByte();
        SC = reader.ReadByte();

        Accuracy = reader.ReadByte();
        Agility = reader.ReadByte();
        HP = reader.ReadByte();
        MP = reader.ReadByte();

        AttackSpeed = reader.ReadSByte();
        Luck = reader.ReadSByte();
       
    }
    public void Save(BinaryWriter writer)
    {
        writer.Write(UniqueID);
        writer.Write(ItemIndex);

        writer.Write(CurrentDura);
        writer.Write(MaxDura);

        writer.Write(Count);

        writer.Write(AC);
        writer.Write(MAC);
        writer.Write(DC);
        writer.Write(MC);
        writer.Write(SC);

        writer.Write(Accuracy);
        writer.Write(Agility);
        writer.Write(HP);
        writer.Write(MP);

        writer.Write(AttackSpeed);
        writer.Write(Luck);

    }


    public uint Price()
    {
        if (Info == null) return 0;

        uint p = Info.Price;


        if (Info.Durability > 0)
        {
            float r = ((Info.Price / 2F) / Info.Durability);

            p = (uint)(MaxDura * r);

            if (MaxDura > 0)
                r = CurrentDura / (float)MaxDura;
            else
                r = 0;

            p = (uint)Math.Floor(p / 2F + ((p / 2F) * r) + Info.Price / 2F);
        }


        p = (uint)(p * ((AC + MAC + DC + MC + SC + Accuracy + Agility + HP + MP + AttackSpeed + Luck) * 0.1F + 1F));
        

        return p * Count;
    }
    public uint RepairPrice()
    {
        if (Info == null || Info.Durability == 0) return 0;

        uint p = Info.Price;

        if (Info.Durability > 0)
        {
            p = (uint)Math.Floor(MaxDura * ((Info.Price / 2F) / Info.Durability) + Info.Price / 2F);
            p = (uint)(p * ((AC + MAC + DC + MC + SC + Accuracy + Agility + HP + MP + AttackSpeed + Luck) * 0.1F + 1F));

        }

        return (p * Count) - Price();
    }

    public UserItem Clone()
    {
        UserItem item = new UserItem(Info)
            {
                UniqueID =  UniqueID,
                CurrentDura = CurrentDura,
                MaxDura = MaxDura,
                Count = Count,

                AC = AC,
                MAC = MAC,
                DC = DC,
                MC = MC,
                SC = SC,
                Accuracy = Accuracy,
                Agility = Agility,
                HP = HP,
                MP = MP,

                AttackSpeed = AttackSpeed,
                Luck = Luck,

                DuraChanged = DuraChanged,
            };

        return item;
    }
}
public class ClientMagic
{

    public Spell Spell;
    public byte BaseCost, LevelCost, Icon;
    public byte Level1, Level2, Level3;
    public ushort Need1, Need2, Need3;

    public byte Level, Key;
    public ushort Experience;

    public bool IsTempSpell;

    public ClientMagic()
    {
    }

    public ClientMagic(BinaryReader reader)
    {
        Spell = (Spell)reader.ReadByte();

        BaseCost = reader.ReadByte();
        LevelCost = reader.ReadByte();
        Icon = reader.ReadByte();
        Level1 = reader.ReadByte();
        Level2 = reader.ReadByte();
        Level3 = reader.ReadByte();
        Need1 = reader.ReadUInt16();
        Need2 = reader.ReadUInt16();
        Need3 = reader.ReadUInt16();

        Level = reader.ReadByte();
        Key = reader.ReadByte();
        Experience = reader.ReadUInt16();
    }

    public void Save(BinaryWriter writer)
    {
        writer.Write((byte)Spell);

        writer.Write(BaseCost);
        writer.Write(LevelCost);
        writer.Write(Icon);
        writer.Write(Level1);
        writer.Write(Level2);
        writer.Write(Level3);
        writer.Write(Need1);
        writer.Write(Need2);
        writer.Write(Need3);

        writer.Write(Level);
        writer.Write(Key);
        writer.Write(Experience);
    }
   
}
public class ClientAuction
{
    public ulong AuctionID;
    public UserItem Item;
    public string Seller = string.Empty;
    public uint Price;
    public DateTime ConsignmentDate;

    public ClientAuction()
    {
        
    }
    public ClientAuction(BinaryReader reader)
    {
        AuctionID = reader.ReadUInt64();
        Item = new UserItem(reader);
        Seller = reader.ReadString();
        Price = reader.ReadUInt32();
        ConsignmentDate = DateTime.FromBinary(reader.ReadInt64());
    }
    public void Save(BinaryWriter writer)
    {
        writer.Write(AuctionID);
        Item.Save(writer);
        writer.Write(Seller);
        writer.Write(Price);
        writer.Write(ConsignmentDate.ToBinary());
    }
}


public abstract class Packet
{
    public static bool IsServer;

    public abstract short Index { get; }

    public static Packet ReceivePacket(byte[] rawBytes, out byte[] extra)
    {
        extra = rawBytes;

        Packet p;

        if (rawBytes.Length < 4) return null; //| 2Bytes: Packet Size | 2Bytes: Packet ID |

        int length = (rawBytes[1] << 8) + rawBytes[0];

        if (length > rawBytes.Length) return null;

        using (MemoryStream stream = new MemoryStream(rawBytes, 2, length - 2))
        using (BinaryReader reader = new BinaryReader(stream))
        {
            short id = reader.ReadInt16();

            p = IsServer ? GetClientPacket(id) : GetServerPacket(id);

            p.ReadPacket(reader);
        }

        extra = new byte[rawBytes.Length - length];
        Buffer.BlockCopy(rawBytes, length, extra, 0, rawBytes.Length - length);

        return p;
    }

    public IEnumerable<byte> GetPacketBytes()
    {
        if (Index < 0) return new byte[0];

        byte[] data;

        using (MemoryStream stream = new MemoryStream())
        {
            stream.SetLength(2);
            stream.Seek(2, SeekOrigin.Begin);
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Index);
                WritePacket(writer);
                stream.Seek(0, SeekOrigin.Begin);
                writer.Write((short)stream.Length);
                stream.Seek(0, SeekOrigin.Begin);

                data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
            }
        }

        return data;
    }

    protected abstract void ReadPacket(BinaryReader reader);
    protected abstract void WritePacket(BinaryWriter writer);

    private static Packet GetClientPacket(short index)
    {
        switch (index)
        {
            case 0:
                return new C.ClientVersion();
            case 1:
                return new C.Disconnect();
            case 2:
                return new C.KeepAlive();
            case 3:
                return new C.NewAccount();
            case 4:
                return new C.ChangePassword();
            case 5:
                return new C.Login();
            case 6:
                return new C.NewCharacter();
            case 7:
                return new C.DeleteCharacter();
            case 8:
                return new C.StartGame();
            case 9:
                return new C.LogOut();
            case 10:
                return new C.Turn();
            case 11:
                return new C.Walk();
            case 12:
                return new C.Run();
            case 13:
                return new C.Chat();
            case 14:
                return new C.MoveItem();
            case 15:
                return new C.StoreItem();
            case 16:
                return new C.TakeBackItem();
            case 17:
                return new C.MergeItem();
            case 18:
                return new C.EquipItem();
            case 19:
                return new C.RemoveItem();
            case 20:
                return new C.SplitItem();
            case 21:
                return new C.UseItem();
            case 22:
                return new C.DropItem();
            case 23:
                return new C.DropGold();
            case 24:
                return new C.PickUp();
            case 25:
                return new C.Inspect();
            case 26:
                return new C.ChangeAMode();
            case 27:
                return new C.ChangePMode();
            case 28:
                return new C.Attack();
            case 29:
                return new C.Harvest();
            case 30:
                return new C.CallNPC();
            case 31:
                return new C.BuyItem();
            case 32:
                return new C.SellItem();
            case 33:
                return new C.RepairItem();
            case 34:
                return new C.BuyItemBack();
            case 35:
                return new C.SRepairItem();
            case 36:
                return new C.MagicKey();
            case 37:
                return new C.Magic();
            case 38:
                return new C.SwitchGroup();
            case 39:
                return new C.AddMember();
            case 40:
                return new C.DelMember();
            case 41:
                return new C.GroupInvite();
            case 42:
                return new C.TownRevive();
            case 43:
                return new C.SpellToggle();
            case 44:
                return new C.ConsignItem();
            case 45:
                return new C.MarketSearch();
            case 46:
                return new C.MarketRefresh();
            case 47:
                return new C.MarketPage();
            case 48:
                return new C.MarketBuy();
            case 49:
                return new C.MarketGetBack();
            default:
                throw new NotImplementedException();
        }

    }
    public static Packet GetServerPacket(short index)
    {
        switch (index)
        {
            case 0:
                return new S.Connected();
            case 1:
                return new S.ClientVersion();
            case 2:
                return new S.Disconnect();
            case 3:
                return new S.NewAccount();
            case 4:
                return new S.ChangePassword();
            case 5:
                return new S.ChangePasswordBanned();
            case 6:
                return new S.Login();
            case 7:
                return new S.LoginBanned();
            case 8:
                return new S.LoginSuccess();
            case 9:
                return new S.NewCharacter();
            case 10:
                return new S.NewCharacterSuccess();
            case 11:
                return new S.DeleteCharacter();
            case 12:
                return new S.DeleteCharacterSuccess();
            case 13:
                return new S.StartGame();
            case 14:
                return new S.StartGameBanned();
            case 15:
                return new S.StartGameDelay();
            case 16:
                return new S.MapInformation();
            case 17:
                return new S.UserInformation();
            case 18:
                return new S.UserLocation();
            case 19:
                return new S.ObjectPlayer();
            case 20:
                return new S.ObjectRemove();
            case 21:
                return new S.ObjectTurn();
            case 22:
                return new S.ObjectWalk();
            case 23:
                return new S.ObjectRun();
            case 24:
                return new S.Chat();
            case 25:
                return new S.ObjectChat();
            case 26:
                return new S.NewItemInfo();
            case 27:
                return new S.MoveItem();
            case 28:
                return new S.EquipItem();
            case 29:
                return new S.MergeItem();
            case 30:
                return new S.RemoveItem();
            case 31:
                return new S.TakeBackItem();
            case 32:
                return new S.StoreItem();
            case 33:
                return new S.SplitItem();
            case 34:
                return new S.SplitItem1();
            case 35:
                return new S.UseItem();
            case 36:
                return new S.DropItem();
            case 37:
                return new S.PlayerUpdate();
            case 38:
                return new S.PlayerInspect();
            case 39:
                return new S.LogOutSuccess();
            case 40:
                return new S.TimeOfDay();
            case 41:
                return new S.ChangeAMode();
            case 42:
                return new S.ChangePMode();
            case 43:
                return new S.ObjectItem();
            case 44:
                return new S.ObjectGold();
            case 45:
                return new S.GainedItem();
            case 46:
                return new S.GainedGold();
            case 47:
                return new S.LoseGold();
            case 48:
                return new S.ObjectMonster();
            case 49:
                return new S.ObjectAttack();
            case 50:
                return new S.Struck();
            case 51:
                return new S.ObjectStruck();
            case 52:
                return new S.DuraChanged();
            case 53:
                return new S.HealthChanged();
            case 54:
                return new S.DeleteItem();
            case 55:
                return new S.Death();
            case 56:
                return new S.ObjectDied();
            case 57:
                return new S.ColourChanged();
            case 58:
                return new S.ObjectColourChanged();
            case 59:
                return new S.GainExperience();
            case 60:
                return new S.LevelChanged();
            case 61:
                return new S.ObjectLeveled();
            case 62:
                return new S.ObjectHarvest();
            case 63:
                return new S.ObjectHarvested();
            case 64:
                return new S.ObjectNPC();
            case 65:
                return new S.NPCResponse();
            case 66:
                return new S.ObjectHide();
            case 67:
                return new S.ObjectShow();
            case 68:
                return new S.Poisoned();
            case 69:
                return new S.ObjectPoisoned();
            case 70:
                return new S.MapChanged();
            case 71:
                return new S.ObjectTeleportOut();
            case 72:
                return new S.ObjectTeleportIn();
            case 73:
                return new S.TeleportIn();
            case 74:
                return new S.NPCGoods();
            case 75:
                return new S.NPCSell();
            case 76:
                return new S.NPCRepair();
            case 77:
                return new S.NPCSRepair();
            case 78:
                return new S.NPCStorage();
            case 79:
                return new S.SellItem();
            case 80:
                return new S.RepairItem();
            case 81:
                return new S.ItemRepaired();
            case 82:
                return new S.NewMagic();
            case 83:
                return new S.MagicLeveled();
            case 84:
                return new S.Magic();
            case 85:
                return new S.ObjectMagic();
            case 86:
                return new S.ObjectEffect();
            case 87:
                return new S.Pushed();
            case 88:
                return new S.ObjectPushed();
            case 89:
                return new S.ObjectName();
            case 90:
                return new S.UserStorage();
            case 91:
                return new S.SwitchGroup();
            case 92:
                return new S.DeleteGroup();
            case 93:
                return new S.DeleteMember();
            case 94:
                return new S.GroupInvite();
            case 95:
                return new S.AddMember();
            case 96:
                return new S.Revived();
            case 97:
                return new S.ObjectRevived();
            case 98:
                return new S.SpellToggle();
            case 99:
                return new S.ObjectHealth();
            case 100:
                return new S.MapEffect();
            case 101:
                return new S.ObjectRangeAttack();
            case 102:
                return new S.AddBuff();
            case 103:
                return new S.RemoveBuff();
            case 104:
                return new S.ObjectHidden();
            case 105:
                return new S.RefreshItem();
            case 106:
                return new S.ObjectSpell();
            case 107:
                return new S.UserDash();
            case 108:
                return new S.ObjectDash();
            case 109:
                return new S.UserDashFail();
            case 110:
                return new S.ObjectDashFail();
            case 111:
                return new S.NPCConsign();
            case 112:
                return new S.NPCMarket();
            case 113:
                return new S.NPCMarketPage();
            case 114:
                return new S.ConsignItem();
            case 115:
                return new S.MarketFail();
            case 116:
                return new S.MarketSuccess();
            case 117:
                return new S.ObjectSitDown();
            case 118:
                return new S.InTrapRock();
            case 119:
                return new S.RemoveMagic();
            default:
                throw new NotImplementedException();
        }
    }

}



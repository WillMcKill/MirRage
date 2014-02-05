using System.IO;
using Client.MirSounds;

namespace Client
{
    class Settings
    {
        public const long CleanDelay = 600000;
        public static int ScreenWidth = 800, ScreenHeight = 600;
        private static readonly InIReader Reader = new InIReader(@".\Mir2Config.ini");

        public const string DataPath = @".\Data\",
                            MapPath = @".\Map\",
                            SoundPath = @".\Sound\",
                            MonsterPath = @".\Data\Monster\",
                            NPCPath = @".\Data\NPC\",
                            CArmourPath = @".\Data\CArmour\",
                            CWeaponPath = @".\Data\CWeapon\",
                            CHairPath = @".\Data\CHair\",
                            AArmourPath = @".\Data\AArmour\",
                            AWeaponPath = @".\Data\AWeapon\",
                            AHairPath = @".\Data\AHair\",
                            CHumEffectPath = @".\Data\CHumEffect\",
                            AHumEffectPath = @".\Data\AHumEffect\";
        //Logs
        public static bool LogErrors = true;
        public static bool LogChat = true;
        public static int RemainingErrorLogs = 100;

        //Graphics
        public static bool FullScreen = true, TopMost = true;
        public static string FontName = "MS Sans Serif";
        public static bool FPSCap = false;
        public static int MaxFPS = 100;
        public static bool HighResolution = false;

        //Network
        public static bool UseConfig = true;
        public static string IPAddress = "82.7.209.103";
        public static int Port = 7000;
        public const int TimeOut = 5000;

        //Sound
        public static int SoundOverLap = 3;
        private static byte _volume = 100;
        public static byte Volume
        {
            get { return _volume; }
            set
            {
                if (_volume == value) return;

                _volume = (byte) (value > 100 ? 100 : value);

                if (_volume == 0)
                    SoundManager.Vol = -10000;
                else 
                    SoundManager.Vol = (int)(-3000 + (3000 * (_volume / 100M)));
            }
        }

        //Game

        public static string AccountID = "",
                             Password = "";
        public static bool
            SkillMode,
            SkillBar,
            Effect = true,
            DropView = true,
            NameView = true,
            HPView = true;

        public static void Load()
        {
            if (!Directory.Exists(DataPath)) Directory.CreateDirectory(DataPath);
            if (!Directory.Exists(MapPath)) Directory.CreateDirectory(MapPath);
            if (!Directory.Exists(SoundPath)) Directory.CreateDirectory(SoundPath);

            //Graphics
            FullScreen = Reader.ReadBoolean("Graphics", "FullScreen", FullScreen);
            TopMost = Reader.ReadBoolean("Graphics", "AlwaysOnTop", TopMost);
            FontName = Reader.ReadString("Graphics", "FontName", FontName);
            FPSCap = Reader.ReadBoolean("Graphics", "FPSCap", FPSCap);
            HighResolution = Reader.ReadBoolean("Graphics", "HighResolution", HighResolution);

            //Network
            UseConfig = Reader.ReadBoolean("Network", "UseConfig", UseConfig);
            if (UseConfig)
            {
                IPAddress = Reader.ReadString("Network", "IPAddress", IPAddress);
                Port = Reader.ReadInt32("Network", "Port", Port);
            }

            //Logs
            LogErrors = Reader.ReadBoolean("Logs", "LogErrors", LogErrors);
            LogChat = Reader.ReadBoolean("Logs", "LogChat", LogChat);

            //Sound
            Volume = Reader.ReadByte("Sound", "Volume", Volume);
            SoundOverLap = Reader.ReadInt32("Sound", "SoundOverLap", SoundOverLap);

            //Game
            AccountID = Reader.ReadString("Game", "AccountID", AccountID);
            Password = Reader.ReadString("Game", "Password", Password);

            SkillMode = Reader.ReadBoolean("Game", "SkillMode", SkillMode);
            SkillBar = Reader.ReadBoolean("Game", "SkillBar", SkillBar);
            Effect = Reader.ReadBoolean("Game", "Effect", Effect);
            DropView = Reader.ReadBoolean("Game", "DropView", DropView);
            NameView = Reader.ReadBoolean("Game", "NameView", NameView);
            HPView = Reader.ReadBoolean("Game", "HPMPView", HPView);
        }

        public static void Save()
        {
            //Graphics
            Reader.Write("Graphics", "FullScreen", FullScreen);
            Reader.Write("Graphics", "AlwaysOnTop", TopMost);
            Reader.Write("Graphics", "FontName", FontName);
            Reader.Write("Graphics", "FPSCap", FPSCap);
            Reader.Write("Graphics", "HighResolution", HighResolution);

            //Sound
            Reader.Write("Sound", "Volume", Volume);

            //Game
            Reader.Write("Game", "SkillMode", SkillMode);
            Reader.Write("Game", "SkillBar", SkillBar);
            Reader.Write("Game", "Effect", Effect);
            Reader.Write("Game", "DropView", DropView);
            Reader.Write("Game", "NameView", NameView);
            Reader.Write("Game", "HPMPView", HPView);
            Reader.Write("Game", "FontName", FontName);
            
        }
    }
}

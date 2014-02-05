using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirScenes;
using Client.MirSounds;
using S = ServerPackets;
using C = ClientPackets;

namespace Client.MirObjects
{
    public class PlayerObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Player; }
        }

        public override bool Blocking
        {
            get { return !Dead; }
        }

        public MirGender Gender;
        public MirClass Class;
        public byte Hair;

        public MLibrary WeaponLibrary1, WeaponLibrary2, HairLibrary, WingLibrary;
        public int Armour, Weapon, ArmourOffSet, HairOffSet, WeaponOffSet, WingOffset;

        public int DieSound, FlinchSound, AttackSound;


        public FrameSet Frames;
        public Frame Frame, WingFrame;
        public int FrameIndex, FrameInterval, EffectFrameIndex, EffectFrameInterval;


        public Spell Spell;
        public byte SpellLevel;
        public bool Cast;
        public uint TargetID;
        public Point TargetPoint;

        public bool MagicShield;
        public Effect ShieldEffect;
        public byte WingEffect;
        private short StanceDelay = 2500;
        public long StanceTime, BlizzardFreezeTime;
        
        public PlayerObject(uint objectID) : base(objectID)
        {
            Frames = FrameSet.Players;
        }

        public void Load(S.ObjectPlayer info)
        {
            Name = info.Name;
            NameColour = info.NameColour;
            Class = info.Class;
            Gender = info.Gender;

            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);

            Direction = info.Direction;
            Hair = info.Hair;

            Weapon = info.Weapon;
            Armour = info.Armour;
            Light = info.Light;

            Poison = info.Poison;

            Dead = info.Dead;
            Hidden = info.Hidden;

            WingEffect = info.WingEffect;
            
            SetLibraries();

            if (Dead) ActionFeed.Add(new QueuedAction { Action = MirAction.Dead, Direction = Direction, Location = CurrentLocation });
            if (info.Extra) Effects.Add(new Effect(Libraries.Magic2, 670, 10, 800, this));

            SetAction();
        }
        public void Update(S.PlayerUpdate info)
        {
            Weapon = info.Weapon;
            Armour = info.Armour;
            Light = info.Light;
            WingEffect = info.WingEffect;
            SetLibraries();
        }


        public virtual void SetLibraries()
        {
            if (Class == MirClass.Assassin)
            {
                switch (Armour)
                {
                    case 12:
                    case 14:
                        BodyLibrary = Armour - 5 < Libraries.AArmours.Length ? Libraries.AArmours[Armour - 5] : Libraries.AArmours[0];
                        break;
                    case 17:
                    case 18:
                        BodyLibrary = Armour - 7 < Libraries.AArmours.Length ? Libraries.AArmours[Armour - 7] : Libraries.AArmours[0];
                        break;
                    default:
                        BodyLibrary = Armour < Libraries.AArmours.Length ? Libraries.AArmours[Armour] : Libraries.AArmours[0];
                        break;
                }

                    HairLibrary = Hair < Libraries.AHair.Length ? Libraries.AHair[Hair] : null;

                if (Weapon >= 0)
                {
                    int index = Weapon == 41 ? 10 : Weapon;
                    WeaponLibrary1 = index < Libraries.AWeaponsL.Length ? Libraries.AWeaponsL[index] : null;
                    WeaponLibrary2 = index < Libraries.AWeaponsR.Length ? Libraries.AWeaponsR[index] : null;
                }
                else
                {
                    WeaponLibrary1 = null;
                    WeaponLibrary2 = null;
                }

                /*if (WingEffect > 0)
                {
                    WingLibrary = WingEffect < Libraries.AHumEffect.Length ? Libraries.AHumEffect[WingEffect] : null;
                }*/

                ArmourOffSet = Gender == MirGender.Male ? 0 : 512;
                HairOffSet = Gender == MirGender.Male ? 0 : 512;
                WeaponOffSet = Gender == MirGender.Male ? 0 : 512;
                WingOffset = Gender == MirGender.Male ? 0 : 840;
            }
            else
            {
                BodyLibrary = Armour < Libraries.CArmours.Length ? Libraries.CArmours[Armour] : Libraries.CArmours[0];
                HairLibrary = Hair < Libraries.CHair.Length ? Libraries.CHair[Hair] : null;
                if (Weapon >= 0)
                WeaponLibrary1 = Weapon < Libraries.CWeapons.Length ? Libraries.CWeapons[Weapon] : null;
                else
                    WeaponLibrary1 = null;
                WeaponLibrary2 = null;

                if (WingEffect > 0)
                {
                    WingLibrary = (WingEffect - 1) < Libraries.CHumEffect.Length ? Libraries.CHumEffect[WingEffect - 1] : null;
                }

                ArmourOffSet = Gender == MirGender.Male ? 0 : 808;
                HairOffSet = Gender == MirGender.Male ? 0 : 808;
                WeaponOffSet = Gender == MirGender.Male ? 0 : 416;
                WingOffset = Gender == MirGender.Male ? 0 : 840;
            }

            DieSound = Gender == MirGender.Male ? SoundList.MaleDie : SoundList.FemaleDie;
            FlinchSound = Gender == MirGender.Male ? SoundList.MaleFlinch : SoundList.FemaleFlinch;
        }

        public override void Process()
        {
            bool update = CMain.Time >= NextMotion || GameScene.CanMove;

            SkipFrames = this != User && ActionFeed.Count > 1;

            ProcessFrames();

            if (Frame == null)
            {
                DrawFrame = 0;
                DrawWingFrame = 0;
            }
            else
            {
                DrawFrame = Frame.Start + (Frame.OffSet * (byte)Direction) + FrameIndex;
                DrawWingFrame = Frame.EffectStart + (Frame.EffectOffSet * (byte)Direction) + EffectFrameIndex;
            }


            #region Moving OffSet

            switch (CurrentAction)
            {
                case MirAction.Walking:
                case MirAction.Running:
                case MirAction.Pushed:
                case MirAction.DashL:
                case MirAction.DashR:
                    if (Frame == null)
                    {
                        OffSetMove = Point.Empty;
                        Movement = CurrentLocation;
                        break;
                    }
                    int i = CurrentAction == MirAction.Running ? 2 : 1;

                    Movement = Functions.PointMove(CurrentLocation, Direction, CurrentAction == MirAction.Pushed ? 0 : -i);

                    int count = Frame.Count;
                    int index = FrameIndex;

                    if (CurrentAction == MirAction.DashR || CurrentAction == MirAction.DashL)
                    {
                        count = 3;
                        index %= 3;
                    }

                    switch (Direction)
                    {
                        case MirDirection.Up:
                            OffSetMove = new Point(0, (int)((MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.UpRight:
                            OffSetMove = new Point((int)((-MapControl.CellWidth * i / (float)(count)) * (index + 1)), (int)((MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.Right:
                            OffSetMove = new Point((int)((-MapControl.CellWidth * i / (float)(count)) * (index + 1)), 0);
                            break;
                        case MirDirection.DownRight:
                            OffSetMove = new Point((int)((-MapControl.CellWidth * i / (float)(count)) * (index + 1)), (int)((-MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.Down:
                            OffSetMove = new Point(0, (int)((-MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.DownLeft:
                            OffSetMove = new Point((int)((MapControl.CellWidth * i / (float)(count)) * (index + 1)), (int)((-MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                        case MirDirection.Left:
                            OffSetMove = new Point((int)((MapControl.CellWidth * i / (float)(count)) * (index + 1)), 0);
                            break;
                        case MirDirection.UpLeft:
                            OffSetMove = new Point((int)((MapControl.CellWidth * i / (float)(count)) * (index + 1)), (int)((MapControl.CellHeight * i / (float)(count)) * (index + 1)));
                            break;
                    }

                    OffSetMove = new Point(OffSetMove.X % 2 + OffSetMove.X, OffSetMove.Y % 2 + OffSetMove.Y);
                    break;
                default:
                    OffSetMove = Point.Empty;
                    Movement = CurrentLocation;
                    break;
            }

            #endregion


            DrawY = Movement.Y > CurrentLocation.Y ? Movement.Y : CurrentLocation.Y;

            DrawLocation = new Point((Movement.X - User.Movement.X + MapControl.OffSetX) * MapControl.CellWidth, (Movement.Y - User.Movement.Y + MapControl.OffSetY) * MapControl.CellHeight);
  
            if (this != User)
            {
                DrawLocation.Offset(User.OffSetMove);
                DrawLocation.Offset(-OffSetMove.X, -OffSetMove.Y);
            }

            if (BodyLibrary != null && update)
            {
                FinalDrawLocation = DrawLocation.Add(BodyLibrary.GetOffSet(DrawFrame));
                DisplayRectangle = new Rectangle(DrawLocation, BodyLibrary.GetTrueSize(DrawFrame));
            }

            for (int i = 0; i < Effects.Count; i++)
                Effects[i].Process();

            Color colour = DrawColour;

            switch (Poison)
            {
                case PoisonType.None:
                    DrawColour = Color.White;
                    break;
                case PoisonType.Green:
                    DrawColour = Color.Green;
                    break;
                case PoisonType.Red:
                    DrawColour = Color.Red;
                    break;
                case PoisonType.Slow:
                    DrawColour = Color.Purple;
                    break;
                case PoisonType.Stun:
                    DrawColour = Color.Yellow;
                    break;
                case PoisonType.Frozen:
                    DrawColour = Color.Blue;
                    break;
                case PoisonType.Paralysis:
                    DrawColour = Color.Gray;
                    break;
            }


            if (colour != DrawColour) GameScene.Scene.MapControl.TextureValid = false;
        }
        public virtual void SetAction()
        {
            if (NextAction != null &&   !GameScene.CanMove)
            {
                switch (NextAction.Action)
                {
                    case MirAction.Walking:
                    case MirAction.Running:
                    case MirAction.Pushed:
                    case MirAction.DashL:
                    case MirAction.DashR:
                        return;
                }
            }

            if (User == this && CMain.Time < MapControl.NextAction)// && CanSetAction)
            {
                //NextMagic = null;
                return;
            }


            if (ActionFeed.Count == 0)
            {
                CurrentAction = CMain.Time > BlizzardFreezeTime ? MirAction.Standing : MirAction.Stance2;
                if (CurrentAction == MirAction.Standing) CurrentAction = CMain.Time > StanceTime ? MirAction.Standing : MirAction.Stance;

                Frames.Frames.TryGetValue(CurrentAction, out Frame);
                FrameIndex = 0;
                EffectFrameIndex = 0;

                if (MapLocation != CurrentLocation)
                {
                    GameScene.Scene.MapControl.RemoveObject(this);
                    MapLocation = CurrentLocation;
                    GameScene.Scene.MapControl.AddObject(this);
                }

                if (Frame == null) return;

                FrameInterval = Frame.Interval;
                EffectFrameInterval = Frame.EffectInterval;
            }
            else
            {
                QueuedAction action = ActionFeed[0];
                ActionFeed.RemoveAt(0);

                CurrentAction = action.Action;
                CurrentLocation = action.Location;
                MirDirection olddirection = Direction;
                Direction = action.Direction;

                Point temp;
                switch (CurrentAction)
                {
                    case MirAction.Walking:
                    case MirAction.Running:
                    case MirAction.Pushed:
                    case MirAction.DashL:
                    case MirAction.DashR:
                        int i = CurrentAction == MirAction.Running ? 2 : 1;
                        temp = Functions.PointMove(CurrentLocation, Direction, CurrentAction == MirAction.Pushed ? 0 : -i);
                     
                        break;
                    default:
                        temp = CurrentLocation;
                        break;
                }

                temp = new Point(action.Location.X, temp.Y > CurrentLocation.Y ? temp.Y : CurrentLocation.Y);

                if (MapLocation != temp)
                {
                    GameScene.Scene.MapControl.RemoveObject(this);
                    MapLocation = temp;
                    GameScene.Scene.MapControl.AddObject(this);
                }


                switch (CurrentAction)
                {
                    case MirAction.Pushed:
                        if (this == User)
                            MapControl.InputDelay = CMain.Time + 500;
                        Frames.Frames.TryGetValue(MirAction.Walking, out Frame);
                        break;
                    case MirAction.DashL:
                    case MirAction.DashR:
                        Frames.Frames.TryGetValue(MirAction.Running, out Frame);
                        break;
                    case MirAction.DashFail:
                        Frames.Frames.TryGetValue(MirAction.Standing, out Frame);
                        //CanSetAction = false;
                        break;
                    case MirAction.Attack4:
                        Spell = (Spell)action.Params[0];
                        Frames.Frames.TryGetValue(Spell == Spell.TwinDrakeBlade || Spell == Spell.FlamingSword ? MirAction.Attack1 : CurrentAction, out Frame);
                        break;
                    case MirAction.Spell:
                        Spell = (Spell)action.Params[0];
                        switch (Spell)
                        {
                            case Spell.ShoulderDash:
                                Frames.Frames.TryGetValue(MirAction.Running, out Frame);
                                CurrentAction = MirAction.DashL;
                                Direction = olddirection;
                                CurrentLocation = Functions.PointMove(CurrentLocation, Direction, 1);
                                if (this == User)
                                {
                                    MapControl.NextAction = CMain.Time + 2500;
                                    GameScene.SpellTime = CMain.Time + 2500; //Spell Delay

                                    Network.Enqueue(new C.Magic { Spell = Spell, Direction = Direction, });
                                }
                                break;
                            case Spell.BladeAvalanche:
                                Frames.Frames.TryGetValue(MirAction.Attack3, out Frame);
                                if (this == User)
                                {
                                    MapControl.NextAction = CMain.Time + 2500;
                                    GameScene.SpellTime = CMain.Time + 1500; //Spell Delay
                                }
                                break;
                            default:
                                Frames.Frames.TryGetValue(CurrentAction, out Frame);
                                break;
                        }
                        break;
                    default:
                        Frames.Frames.TryGetValue(CurrentAction, out Frame);
                        break;

                }

                FrameIndex = 0;
                EffectFrameIndex = 0;
                Spell = Spell.None;
                SpellLevel = 0;
                //NextMagic = null;

                if (Frame == null) return;

                FrameInterval = Frame.Interval;
                EffectFrameInterval = Frame.EffectInterval;

                if (this == User)
                {
                    switch (CurrentAction)
                    {
                        case MirAction.DashFail:
                            //CanSetAction = false;
                            break;
                        case MirAction.Standing:
                            Network.Enqueue(new C.Turn {Direction = Direction});
                            MapControl.NextAction = CMain.Time + 2500;
                            break;
                        case MirAction.Walking:
                            Network.Enqueue(new C.Walk {Direction = Direction});
                            GameScene.Scene.MapControl.FloorValid = false;
                            GameScene.CanRun = true;
                            MapControl.NextAction = CMain.Time + 2500;
                            break;
                        case MirAction.Running:
                            Network.Enqueue(new C.Run {Direction = Direction});
                            GameScene.Scene.MapControl.FloorValid = false;
                            MapControl.NextAction = CMain.Time + 2500;
                            break;
                        case MirAction.Pushed:
                            GameScene.Scene.MapControl.FloorValid = false;
                            MapControl.InputDelay = CMain.Time + 500;
                            break;
                        case MirAction.DashL:
                        case MirAction.DashR:
                            GameScene.Scene.MapControl.FloorValid = false;
                            //CanSetAction = false;
                            break;
                        case MirAction.Attack1:
                            ClientMagic magic;
                            if (GameScene.Slaying && TargetObject != null)
                                Spell = Spell.Slaying;

                            if (GameScene.Thrusting && GameScene.Scene.MapControl.HasTarget(Functions.PointMove(CurrentLocation, Direction, 2)))
                                Spell = Spell.Thrusting;

                            if (GameScene.HalfMoon)
                            {
                                if (TargetObject != null || GameScene.Scene.MapControl.CanHalfMoon(CurrentLocation, Direction))
                                {
                                    magic = User.GetMagic(Spell.HalfMoon);
                                    if (magic != null && magic.BaseCost + magic.LevelCost * magic.Level <= User.MP)
                                        Spell = Spell.HalfMoon;
                                }
                            }

                            if (GameScene.CrossHalfMoon)
                            {
                                if (TargetObject != null || GameScene.Scene.MapControl.CanCrossHalfMoon(CurrentLocation))
                                {
                                    magic = User.GetMagic(Spell.CrossHalfMoon);
                                    if (magic != null && magic.BaseCost + magic.LevelCost * magic.Level <= User.MP)
                                        Spell = Spell.CrossHalfMoon;
                                }
                            }

                            if (GameScene.DoubleSlash)
                            {
                                magic = User.GetMagic(Spell.DoubleSlash);
                                if (magic != null && magic.BaseCost + magic.LevelCost * magic.Level <= User.MP)
                                    Spell = Spell.DoubleSlash;
                            }


                            if (GameScene.TwinDrakeBlade)
                            {
                                magic = User.GetMagic(Spell.TwinDrakeBlade);
                                if (magic != null && magic.BaseCost + magic.LevelCost * magic.Level <= User.MP)
                                    Spell = Spell.TwinDrakeBlade;
                            }

                            if (GameScene.FlamingSword)
                            {
                                if (TargetObject != null)
                                {
                                    magic = User.GetMagic(Spell.FlamingSword);
                                    if (magic != null)
                                        Spell = Spell.FlamingSword;
                                }
                            }

                            Network.Enqueue(new C.Attack { Direction = Direction, Spell = Spell });

                            if (Spell == Spell.Slaying)
                                GameScene.Slaying = false;


                            if (Spell == Spell.TwinDrakeBlade)
                                GameScene.TwinDrakeBlade = false;
                            if (Spell == Spell.FlamingSword)
                                GameScene.FlamingSword = false;

                            magic = User.GetMagic(Spell);
                            if (magic != null) SpellLevel = magic.Level;

                            GameScene.AttackTime = CMain.Time + User.AttackSpeed;
                            MapControl.NextAction = CMain.Time + 2500;
                            break;
                        case MirAction.Attack2:
                            //Network.Enqueue(new C.Attack2 { Direction = Direction });
                            break;
                        case MirAction.Attack3:
                            //Network.Enqueue(new C.Attack3 { Direction = Direction });
                            break;
                        case MirAction.Spell:
                            Spell = (Spell) action.Params[0];
                            uint targetID = (uint) action.Params[1];
                            Point location = (Point) action.Params[2];
                            Network.Enqueue(new C.Magic {Spell = Spell, Direction = Direction, TargetID = targetID, Location = location});

                            GameScene.SpellTime = Spell == Spell.FlameField ? CMain.Time + 2500 : CMain.Time + 1800;

                            MapControl.NextAction = CMain.Time + 2500;
                            break;
                        case MirAction.Harvest:
                            Network.Enqueue(new C.Harvest {Direction = Direction});
                            MapControl.NextAction = CMain.Time + 2500;
                            break;
                        
                    }
                }


                switch (CurrentAction)
                {
                    case MirAction.Pushed:
                        FrameIndex = Frame.Count - 1;
                        EffectFrameIndex = Frame.EffectCount - 1;
                        GameScene.Scene.Redraw();
                        break;
                    case MirAction.DashL:
                        FrameIndex = 0;
                        EffectFrameIndex = 0;
                        GameScene.Scene.Redraw();
                        break;
                    case MirAction.DashR:
                        FrameIndex = 3;
                        EffectFrameIndex = 3;
                        GameScene.Scene.Redraw();
                        break;
                    case MirAction.Walking:
                    case MirAction.Running:
                        GameScene.Scene.Redraw();
                        break;
                    case MirAction.Attack1:
                        if (this != User)
                        {
                            Spell = (Spell)action.Params[0];
                            SpellLevel = (byte)action.Params[1];
                        }

                        switch (Spell)
                        {
                            case Spell.Slaying:
                                SoundManager.PlaySound(20000 + (ushort) Spell*10 + (Gender == MirGender.Male ? 0 : 1));
                                break;
                            case Spell.DoubleSlash:
                                FrameInterval = FrameInterval*5/10; //50% Faster Animation
                                EffectFrameInterval = EffectFrameInterval * 5 / 10;
                                action = new QueuedAction {Action = MirAction.Attack4, Direction = Direction, Location = CurrentLocation, Params = new List<object>()};
                                action.Params.Add(Spell);
                                ActionFeed.Insert(0, action);
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;
                            case Spell.Thrusting:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;
                            case Spell.HalfMoon:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;
                                
                            case Spell.TwinDrakeBlade:
                                FrameInterval = FrameInterval*7/10; //70% Faster Animation
                                EffectFrameInterval = EffectFrameInterval * 7 / 10;
                                action = new QueuedAction {Action = MirAction.Attack4, Direction = Direction, Location = CurrentLocation, Params = new List<object>()};
                                action.Params.Add(Spell);
                                ActionFeed.Insert(0, action);
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            case Spell.CrossHalfMoon:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            case Spell.FlamingSword:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                break;
                        }
                        break;
                    case MirAction.Attack4:
                        Spell = (Spell) action.Params[0];
                        switch (Spell)
                        {
                            case Spell.DoubleSlash:
                                FrameInterval = FrameInterval * 5 / 10; //50% Animation Speed
                                EffectFrameInterval = EffectFrameInterval * 5 / 10;
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                break;
                            case Spell.TwinDrakeBlade:
                                FrameInterval = FrameInterval * 8 / 10; //80% Animation Speed
                                EffectFrameInterval = EffectFrameInterval * 8 / 10;
                                break;
                        }
                        break;
                    case MirAction.Struck:
                        uint attackerID = (uint)action.Params[0];
                        StruckWeapon = -2;
                        for (int i = 0; i < MapControl.Objects.Count; i++)
                        {
                            MapObject ob = MapControl.Objects[i];
                            if (ob.ObjectID != attackerID) continue;
                            if (ob.Race != ObjectType.Player) break;
                            PlayerObject player = ((PlayerObject) ob);
                            StruckWeapon = player.Weapon;
                            if (player.Class != MirClass.Assassin || StruckWeapon == -1) break;
                            StruckWeapon = 1;
                            break;
                        }
                        PlayStruckSound();
                        PlayFlinchSound();
                        break;
                    case MirAction.Spell:
                        if (this != User)
                        {
                            Spell = (Spell)action.Params[0];
                            TargetID = (uint)action.Params[1];
                            TargetPoint = (Point)action.Params[2];
                            Cast = (bool)action.Params[3];
                            SpellLevel = (byte)action.Params[4];
                        }

                        switch (Spell)
                        {
                            #region FireBall

                            case Spell.FireBall:
                                Effects.Add(new Effect(Libraries.Magic, 0, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Healing

                            case Spell.Healing:
                                Effects.Add(new Effect(Libraries.Magic, 200, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Repulsion

                            case Spell.Repulsion:
                                Effects.Add(new Effect(Libraries.Magic, 900, 6, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region ElectricShock

                            case Spell.ElectricShock:
                                Effects.Add(new Effect(Libraries.Magic, 1560, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Poisoning

                            case Spell.Poisoning:
                                Effects.Add(new Effect(Libraries.Magic, 600, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort) Spell*10 );
                                break;

                            #endregion

                            #region GreatFireBall

                            case Spell.GreatFireBall:
                                Effects.Add(new Effect(Libraries.Magic, 400, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region HellFire

                            case Spell.HellFire:
                                Effects.Add(new Effect(Libraries.Magic, 920, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region ThunderBolt

                            case Spell.ThunderBolt:
                                Effects.Add(new Effect(Libraries.Magic2, 20, 3, 300, this));
                                break;

                            #endregion

                            #region SoulFireBall

                            case Spell.SoulFireBall:
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region SummonSkeleton

                            case Spell.SummonSkeleton:
                                Effects.Add(new Effect(Libraries.Magic, 1500, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region Teleport

                            case Spell.Teleport:
                                Effects.Add(new Effect(Libraries.Magic, 1590, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Hiding

                            case Spell.Hiding:
                                Effects.Add(new Effect(Libraries.Magic, 1520, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region Haste

                            case Spell.Haste:
                                Effects.Add(new Effect(Libraries.Magic2, 2140 + (int)Direction * 10, 6, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region FireBang

                            case Spell.FireBang:
                                Effects.Add(new Effect(Libraries.Magic, 1650, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region FireWall

                            case Spell.FireWall:
                                Effects.Add(new Effect(Libraries.Magic, 1620, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region TrapHexagon

                            case Spell.TrapHexagon:
                                Effects.Add(new Effect(Libraries.Magic, 1380, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region EnergyRepulsor

                            case Spell.EnergyRepulsor:
                                Effects.Add(new Effect(Libraries.Magic2, 190, 6, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region FireBurst

                            case Spell.FireBurst:
                                Effects.Add(new Effect(Libraries.Magic2, 2320, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region FlameDisruptor

                            case Spell.FlameDisruptor:
                                Effects.Add(new Effect(Libraries.Magic2, 130, 6, Frame.Count * FrameInterval, this));
                                break;

                            #endregion

                            #region SummonShinsu

                            case Spell.SummonShinsu:
                                Effects.Add(new Effect(Libraries.Magic2, 0, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region UltimateEnchancer

                            case Spell.UltimateEnhancer:
                                Effects.Add(new Effect(Libraries.Magic2, 160, 15, 1000, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region FrostCrunch

                            case Spell.FrostCrunch:
                                Effects.Add(new Effect(Libraries.Magic2, 400, 6, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region Purification

                            case Spell.Purification:
                                Effects.Add(new Effect(Libraries.Magic2, 600, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region FlameField

                            case Spell.FlameField:
                                MapControl.Effects.Add(new Effect(Libraries.Magic2, 910, 20, 1800, CurrentLocation));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region HeavenlySword

                            case Spell.HeavenlySword:
                                Effects.Add(new Effect(Libraries.Magic2, 2230 + ((int)Direction * 10), 8, 800, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Trap

                            case Spell.Trap:
                                Effects.Add(new Effect(Libraries.Magic2, 2230 + ((int)Direction * 10), 6, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region MoonLight

                            case Spell.MoonLight:
                                Effects.Add(new Effect(Libraries.Magic2, 2380, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region SwiftFeet

                            case Spell.SwiftFeet:
                                Effects.Add(new Effect(Libraries.Magic2, 2230, 15, 1000, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region LightBody

                            case Spell.LightBody:
                                Effects.Add(new Effect(Libraries.Magic2, 2470, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region PoisonSword

                            case Spell.PoisonSword:
                                Effects.Add(new Effect(Libraries.Magic2, 2490 + ((int)Direction * 10), 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region ThunderStorm

                            case Spell.ThunderStorm:
                                MapControl.Effects.Add(new Effect(Libraries.Magic, 1680, 10, Frame.Count * FrameInterval, CurrentLocation));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region MassHealing

                            case Spell.MassHealing:
                                Effects.Add(new Effect(Libraries.Magic, 1790, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region IceStorm

                            case Spell.IceStorm:
                                Effects.Add(new Effect(Libraries.Magic, 3840, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region MagicShield

                            case Spell.MagicShield:
                                Effects.Add(new Effect(Libraries.Magic, 3880, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region TurnUndead

                            case Spell.TurnUndead:
                                Effects.Add(new Effect(Libraries.Magic, 3920, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region Revelation

                            case Spell.Revelation:
                                Effects.Add(new Effect(Libraries.Magic, 3960, 20, 1200, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region ProtectionField

                            case Spell.ProtectionField:
                                Effects.Add(new Effect(Libraries.Magic2, 1520, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region Rage

                            case Spell.Rage:
                                Effects.Add(new Effect(Libraries.Magic2, 1510, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region Vampirism

                            case Spell.Vampirism:
                                Effects.Add(new Effect(Libraries.Magic2, 1040, 7, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                break;

                            #endregion

                            #region LionRoar

                            case Spell.LionRoar:
                                Effects.Add(new Effect(Libraries.Magic2, 710, 20, 1200, this));
                                SoundManager.PlaySound(20000 + (ushort) Spell*10 + (Gender == MirGender.Male ? 0 : 1));
                                break;

                            #endregion

                            #region Entrapment

                            case Spell.Entrapment:
                                Effects.Add(new Effect(Libraries.Magic2, 990, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region BladeAvalanche

                            case Spell.BladeAvalanche:
                                Effects.Add(new Effect(Libraries.Magic2, 740 + (int)Direction * 20, 15, 15 * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Mirroring

                            case Spell.Mirroring:
                                Effects.Add(new Effect(Libraries.Magic2, 650, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region Blizzard

                            case Spell.Blizzard:
                                Effects.Add(new Effect(Libraries.Magic2, 1540, 8, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion

                            #region MeteorStrike

                            case Spell.MeteorStrike:
                                Effects.Add(new Effect(Libraries.Magic2, 1590, 10, Frame.Count * FrameInterval, this));
                                SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                break;

                            #endregion
                        }


                        break;
                    case MirAction.Dead:
                        GameScene.Scene.Redraw();
                        GameScene.Scene.MapControl.SortObject(this);
                        if (MouseObject == this) MouseObject = null;
                        if (TargetObject == this) TargetObject = null;
                        if (MagicObject == this) MagicObject = null;
                        DeadTime = CMain.Time;
                        break;

                }

            }

            GameScene.Scene.MapControl.TextureValid = false;
            
            NextMotion = CMain.Time + FrameInterval;
            NextMotion2 = CMain.Time + EffectFrameInterval;

            if (!MagicShield) return;

            switch (CurrentAction)
            {
                case MirAction.Struck:
                    if (ShieldEffect != null)
                    {
                        ShieldEffect.Clear();
                        ShieldEffect.Remove();
                    }

                    Effects.Add(ShieldEffect = new Effect(Libraries.Magic, 3900, 3, 600, this));
                    ShieldEffect.Complete += (o, e) => Effects.Add(ShieldEffect = new Effect(Libraries.Magic, 3890, 3, 600, this) { Repeat = true });
                    break;
                default:
                    if (ShieldEffect == null)
                        Effects.Add(ShieldEffect = new Effect(Libraries.Magic, 3890, 3, 600, this) { Repeat = true });
                    break;
            }
        }
        public virtual void ProcessFrames()
        {
            if (Frame == null) return;

            switch (CurrentAction)
            {
                case MirAction.Walking:
                case MirAction.Running:
                    if (!GameScene.CanMove) return;

                    GameScene.Scene.MapControl.TextureValid = false;

                    if (this == User) GameScene.Scene.MapControl.FloorValid = false;

                    if (SkipFrames) UpdateFrame();

                    if (UpdateFrame() >= Frame.Count)
                    {
                        FrameIndex = Frame.Count - 1;
                        SetAction();
                    }
                    else
                    {
                        if (this == User)
                        {
                            if (FrameIndex == 1 || FrameIndex == 4)
                                PlayStepSound();
                        }
                    }

                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        if (this == User) GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.DashL:
                    if (!GameScene.CanMove) return;

                    GameScene.Scene.MapControl.TextureValid = false;

                    if (this == User) GameScene.Scene.MapControl.FloorValid = false;
                    if (UpdateFrame() >= 3)
                    {
                        FrameIndex = 2;
                        SetAction();
                    }

                    if (UpdateFrame2() >= 3) EffectFrameIndex = 2;
                    break;
                case MirAction.DashR:
                    if (!GameScene.CanMove) return;

                    GameScene.Scene.MapControl.TextureValid = false;

                    if (this == User) GameScene.Scene.MapControl.FloorValid = false;

                    if (UpdateFrame() >= 6)
                    {
                        FrameIndex = 5;
                        SetAction();
                    }

                    if (UpdateFrame2() >= 6) EffectFrameIndex = 5;
                    break;
                case MirAction.Pushed:
                    if (!GameScene.CanMove) return;

                    GameScene.Scene.MapControl.TextureValid = false;

                    if (this == User) GameScene.Scene.MapControl.FloorValid = false;

                    FrameIndex -= 2;
                    EffectFrameIndex -= 2;

                    if (FrameIndex < 0)
                    {
                        FrameIndex = 0;
                        SetAction();
                    }

                    if (FrameIndex < 0) EffectFrameIndex = 0;            
                    break;
                case MirAction.Standing:
                case MirAction.DashFail:
                case MirAction.Harvest:
                case MirAction.Stance:
                case MirAction.Stance2:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            FrameIndex = Frame.Count - 1;
                            SetAction();
                        }
                        else
                        {
                            NextMotion += FrameInterval;
                        }
                    }

                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Attack1:
                case MirAction.Attack2:
                case MirAction.Attack3:
                case MirAction.Attack4:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();
                        
                        if (UpdateFrame() >= Frame.Count)
                        {
                            //if (ActionFeed.Count == 0)
                            //    ActionFeed.Add(new QueuedAction { Action = MirAction.Stance, Direction = Direction, Location = CurrentLocation });

                            StanceTime = CMain.Time + StanceDelay;
                            FrameIndex = Frame.Count - 1;
                            SetAction();
                        }
                        else
                        {
                            if (FrameIndex == 1) PlayAttackSound();
                            NextMotion += FrameInterval;
                        }
                    }

                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Struck:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            FrameIndex = Frame.Count - 1;
                            SetAction();
                        }
                        else
                        {
                            NextMotion += FrameInterval;
                        }
                    }
                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Spell:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            if (Cast)
                            {

                                MapObject ob = MapControl.GetObject(TargetID);

                                Missile missile;
                                Effect effect;
                                switch (Spell)
                                {
                                    #region FireBall

                                    case Spell.FireBall:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        missile = CreateProjectile(10, Libraries.Magic, true, 6, 30, 4);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic, 170, 10, 600, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort)Spell.FireBall * 10 + 2);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region GreatFireBall

                                    case Spell.GreatFireBall:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        missile = CreateProjectile(410, Libraries.Magic, true, 6, 30, 4);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic, 570, 10, 600, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort) Spell.GreatFireBall*10 + 2);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region Healing

                                    case Spell.Healing:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 370, 10, 800, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic, 370, 10, 800, ob));
                                        break;

                                    #endregion

                                    #region ElectricShock

                                    case Spell.ElectricShock:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 1570, 10, 1000, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic, 1570, 10, 1000, ob));
                                        break;
                                    #endregion

                                    #region Poisoning

                                    case Spell.Poisoning:
                                        SoundManager.PlaySound(20000 + (ushort) Spell*10 + 1);
                                        if (ob != null)
                                            ob.Effects.Add(new Effect(Libraries.Magic, 770, 10, 1000, ob));
                                        break;
                                    #endregion

                                    #region HellFire

                                    case Spell.HellFire:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);

                                        Point dest = CurrentLocation;
                                        for (int i = 0; i < 4; i++)
                                        {
                                            dest = Functions.PointMove(dest, Direction, 1);
                                            if (!GameScene.Scene.MapControl.ValidPoint(dest)) break;
                                            effect = new Effect(Libraries.Magic, 930, 6, 500, dest) { Rate = 0.7F };

                                            effect.SetStart(CMain.Time + i * 50);
                                            MapControl.Effects.Add(effect);
                                        }


                                        if (SpellLevel == 3)
                                        {
                                            MirDirection dir = (MirDirection)(((int)Direction + 1) % 8);
                                            dest = CurrentLocation;
                                            for (int r = 0; r < 4; r++)
                                            {
                                                dest = Functions.PointMove(dest, dir, 1);
                                                if (!GameScene.Scene.MapControl.ValidPoint(dest)) break;
                                                effect = new Effect(Libraries.Magic, 930, 6, 500, dest) { Rate = 0.7F };

                                                effect.SetStart(CMain.Time + r * 50);
                                                MapControl.Effects.Add(effect);
                                            }

                                            dir = (MirDirection)(((int)Direction - 1 + 8) % 8);
                                            dest = CurrentLocation;
                                            for (int r = 0; r < 4; r++)
                                            {
                                                dest = Functions.PointMove(dest, dir, 1);
                                                if (!GameScene.Scene.MapControl.ValidPoint(dest)) break;
                                                effect = new Effect(Libraries.Magic, 930, 6, 500, dest) { Rate = 0.7F };

                                                effect.SetStart(CMain.Time + r * 50);
                                                MapControl.Effects.Add(effect);
                                            }
                                        }
                                        break;

                                    #endregion

                                    #region ThunderBolt

                                    case Spell.ThunderBolt:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10);

                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic2, 10, 5, 400, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic2, 10, 5, 400, ob));
                                        break;

                                    #endregion

                                    #region SoulFireBall

                                    case Spell.SoulFireBall:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 30, 7);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic, 1360, 10, 600, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort)Spell.SoulFireBall * 10 + 2);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region FireBang

                                    case Spell.FireBang:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        MapControl.Effects.Add(new Effect(Libraries.Magic, 1660, 10, 1000, TargetPoint));
                                        break;

                                    #endregion

                                    #region MassHiding

                                    case Spell.MassHiding:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 30, 7);
                                        missile.Explode = true;

                                        missile.Complete += (o, e) =>
                                        {
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 1540, 10, 800, TargetPoint));
                                            SoundManager.PlaySound(20000 + (ushort)Spell.MassHiding * 10 + 1);
                                        };
                                        break;

                                    #endregion

                                    #region SoulShield

                                    case Spell.SoulShield:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 30, 7);
                                        missile.Explode = true;

                                        missile.Complete += (o, e) =>
                                        {
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 1320, 15, 1200, TargetPoint));
                                            SoundManager.PlaySound(20000 + (ushort)Spell.SoulShield * 10 + 1);
                                        };
                                        break;

                                    #endregion

                                    #region BlessedArmour

                                    case Spell.BlessedArmour:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                        missile = CreateProjectile(1160, Libraries.Magic, true, 3, 30, 7);
                                        missile.Explode = true;

                                        missile.Complete += (o, e) =>
                                        {
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 1340, 15, 1200, TargetPoint));
                                            SoundManager.PlaySound(20000 + (ushort) Spell.BlessedArmour*10 + 1);
                                        };
                                        break;

                                    #endregion

                                    #region FireWall

                                    case Spell.FireWall:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        break;

                                    #endregion

                                    #region MassHealing

                                    case Spell.MassHealing:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        MapControl.Effects.Add(new Effect(Libraries.Magic, 1800, 10, 1000, TargetPoint));
                                        break;

                                    #endregion

                                    #region IceStorm

                                    case Spell.IceStorm:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        MapControl.Effects.Add(new Effect(Libraries.Magic, 3850, 20, 1300, TargetPoint));
                                        break;

                                    #endregion

                                    #region TurnUndead

                                    case Spell.TurnUndead:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 3930, 15, 1000, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic, 3930, 15, 1000, ob));
                                        break;
                                    #endregion

                                    #region Revelation

                                    case Spell.Revelation:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic, 3990, 10, 1000, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic, 3990, 10, 1000, ob));
                                        break;
                                    #endregion

                                    #region FlameDisruptor

                                    case Spell.FlameDisruptor:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 );

                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic2, 140, 10, 600, TargetPoint));
                                        else
                                            ob.Effects.Add(new Effect(Libraries.Magic2, 140, 10, 600, ob));
                                        break;

                                    #endregion

                                    #region FrostCrunch

                                    case Spell.FrostCrunch:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        missile = CreateProjectile(410, Libraries.Magic2, true, 4, 30, 6);

                                        if (missile.Target != null)
                                        {
                                            missile.Complete += (o, e) =>
                                            {
                                                if (missile.Target.CurrentAction == MirAction.Dead) return;
                                                missile.Target.Effects.Add(new Effect(Libraries.Magic2, 570, 8, 600, missile.Target));
                                                SoundManager.PlaySound(20000 + (ushort)Spell.FrostCrunch * 10 + 2);
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region Purification

                                    case Spell.Purification:
                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic2, 620, 10, 800, TargetPoint));
                                        break;

                                    #endregion

                                    #region Curse

                                    case Spell.Curse:
                                        missile = CreateProjectile(1160, Libraries.Magic2, true, 3, 30, 7);
                                        missile.Explode = true;

                                        missile.Complete += (o, e) =>
                                        {
                                            MapControl.Effects.Add(new Effect(Libraries.Magic2, 1320, 24, 2000, TargetPoint));
                                            SoundManager.PlaySound(20000 + (ushort)Spell.Curse * 10);
                                        };
                                        break;

                                    #endregion

                                    #region Hallucination

                                    case Spell.Hallucination:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10);
                                        if (ob != null)
                                            ob.Effects.Add(new Effect(Libraries.Magic2, 1110, 10, 1000, ob));
                                        break;
                                    #endregion

                                    #region Lightning

                                    case Spell.Lightning:
                                        Effects.Add(new Effect(Libraries.Magic, 970 + (int)Direction * 20, 6, Frame.Count * FrameInterval, this));
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                        break;

                                    #endregion

                                    #region Vampirism

                                    case Spell.Vampirism:

                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);

                                        if (ob == null)
                                            MapControl.Effects.Add(new Effect(Libraries.Magic2, 1060, 20, 1000, TargetPoint));
                                        else
                                        {
                                            ob.Effects.Add(effect = new Effect(Libraries.Magic2, 1060, 20, 1000, ob));
                                            effect.Complete += (o, e) =>
                                            {
                                                SoundManager.PlaySound(20000 + (ushort)Spell.Vampirism * 10 + 2);
                                                Effects.Add(new Effect(Libraries.Magic2, 1090, 10, 500, this));
                                            };
                                        }
                                        break;

                                    #endregion

                                    #region PoisonField

                                    case Spell.PoisonField:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 );
                                        break;

                                    #endregion

                                    #region Blizzard

                                    case Spell.Blizzard:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        BlizzardFreezeTime = CMain.Time + 3000;
                                        break;

                                    #endregion

                                    #region MeteorStrike

                                    case Spell.MeteorStrike:
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 1);
                                        SoundManager.PlaySound(20000 + (ushort)Spell * 10 + 2);
                                        BlizzardFreezeTime = CMain.Time + 3000;
                                        break;

                                    #endregion

                                }


                                Cast = false;
                            }
                            //if (ActionFeed.Count == 0)
                            //    ActionFeed.Add(new QueuedAction { Action = MirAction.Stance, Direction = Direction, Location = CurrentLocation });

                            StanceTime = CMain.Time + StanceDelay;
                            FrameIndex = Frame.Count - 1;
                            SetAction();

                        }
                        else
                        {
                            NextMotion += FrameInterval;
                            
                        }
                    }
                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Die:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;
                        
                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            FrameIndex = Frame.Count - 1;
                            ActionFeed.Clear();
                            ActionFeed.Add(new QueuedAction { Action = MirAction.Dead, Direction = Direction, Location = CurrentLocation });
                            SetAction();
                        }
                        else
                        {
                            if (FrameIndex == 1)
                                PlayDieSound();

                            NextMotion += FrameInterval;
                        }
                    }
                    if (WingEffect > 0 && CMain.Time >= NextMotion2)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame2();

                        if (UpdateFrame2() >= Frame.EffectCount)
                            EffectFrameIndex = Frame.EffectCount - 1;
                        else
                            NextMotion2 += EffectFrameInterval;
                    }
                    break;
                case MirAction.Dead:
                    break;
                case MirAction.Revive:
                    if (CMain.Time >= NextMotion)
                    {
                        GameScene.Scene.MapControl.TextureValid = false;

                        if (SkipFrames) UpdateFrame();

                        if (UpdateFrame() >= Frame.Count)
                        {
                            FrameIndex = Frame.Count - 1;
                            ActionFeed.Clear();
                            ActionFeed.Add(new QueuedAction { Action = MirAction.Standing, Direction = Direction, Location = CurrentLocation });
                            SetAction();
                        }
                        else
                        {
                            NextMotion += FrameInterval;
                        }
                    }
                    break;

            }
            
            if (this == User) return;

            if ((CurrentAction == MirAction.Standing || CurrentAction == MirAction.Stance || CurrentAction == MirAction.Stance2 || CurrentAction == MirAction.DashFail) && NextAction != null)
                SetAction();
            //if Revive and dead set action

        }
        public int UpdateFrame()
        {
            if (Frame == null) return 0;

            if (Frame.Reverse) return Math.Abs(--FrameIndex);

            return ++FrameIndex;
        }

        public int UpdateFrame2()
        {
            if (Frame == null) return 0;

            if (Frame.Reverse) return Math.Abs(--EffectFrameIndex);

            return ++EffectFrameIndex;
        }


        private Missile CreateProjectile(int baseIndex, MLibrary library, bool blend, int count, int interval, int skip)
        {
            MapObject ob = MapControl.GetObject(TargetID);

            if (ob != null) TargetPoint = ob.CurrentLocation;

            int duration = Functions.MaxDistance(CurrentLocation, TargetPoint) * 50;


            Missile missile = new Missile(library, baseIndex, duration / interval, duration, this, TargetPoint)
            {
                Target = ob,
                Interval = interval,
                FrameCount = count,
                Blend = blend,
                Skip = skip
            };

            Effects.Add(missile);

            return missile;
        }

        //Rebuild
        public void PlayStepSound()
        {
            int x = CurrentLocation.X - CurrentLocation.X % 2;
            int y = CurrentLocation.Y - CurrentLocation.Y % 2;

            int index = (GameScene.Scene.MapControl.M2CellInfo[x, y].BackImage & 0x1FFFF) - 1;
            index = GameScene.Scene.MapControl.M2CellInfo[x, y].FileIndex * 10000 + index;
            int moveSound;

            if ((index >= 330 && index <= 349) || (index >= 450 && index <= 454) || (index >= 550 && index <= 554) ||
                (index >= 750 &&
                index <= 754) || (index >= 950 && index <= 954) || (index >= 1250 && index <= 1254) ||
                (index >= 1400 && index <= 1424) || (index >= 1455 && index <= 1474) || (index >= 1500 && index <= 1524) ||
                (index >= 1550 && index <= 1574))
                moveSound = SoundList.WalkLawnL;
            else if ((index >= 250 && index <= 254) || (index >= 1005 && index <= 1009) || (index >= 1050 && index <= 1054) ||
                (index >= 1060 && index <= 1064) || (index >= 1450 && index <= 1454) || (index >= 1650 && index <= 1654))
                moveSound = SoundList.WalkRoughL;
            else if ((index >= 605 && index <= 609) || (index >= 650 && index <= 654) || (index >= 660 && index <= 664) ||
                (index >= 2000 && index <= 2049) || (index >= 3025 && index <= 3049) || (index >= 2400 && index <= 2424) ||
                (index >= 4625 && index <= 4649) || (index >= 4675 && index <= 4678))
                moveSound = SoundList.WalkStoneL;
            else if ((index >= 1825 && index <= 1924) || (index >= 2150 && index <= 2174) || (index >= 3075 && index <= 3099) ||
                (index >= 3325 && index <= 3349) || (index >= 3375 && index <= 3399))
                moveSound = SoundList.WalkCaveL;
            else if (index == 3230 || index == 3231 || index == 3246 || index == 3277 || (index >= 3780 && index <= 3799))
                moveSound = SoundList.WalkWoodL;
            else if (index >= 3825 && index <= 4434)
                switch (index % 25)
                {
                    case 0:
                        moveSound = SoundList.WalkWoodL;
                        break;
                    default:
                        moveSound = SoundList.WalkGroundL;
                        break;
                }
            else if ((index >= 2075 && index <= 2099) || (index >= 2125 && index <= 2149))
                moveSound = SoundList.WalkRoomL;
            else if (index >= 1800 && index <= 1824)
                moveSound = SoundList.WalkWaterL;
            else moveSound = SoundList.WalkGroundL;

            if ((index >= 825 && index <= 1349) && (index - 825) / 25 % 2 == 0) moveSound = SoundList.WalkStoneL;
            if ((index >= 1375 && index <= 1799) && (index - 1375) / 25 % 2 == 0) moveSound = SoundList.WalkCaveL;
            if (index == 1385 || index == 1386 || index == 1391 || index == 1392) moveSound = SoundList.WalkWoodL;

            index = (GameScene.Scene.MapControl.M2CellInfo[x, y].MiddleImage & 0x7FFF) - 1;
            if (index >= 0 && index <= 115)
                moveSound = SoundList.WalkGroundL;
            else if (index >= 120 && index <= 124)
                moveSound = SoundList.WalkLawnL;

            index = (GameScene.Scene.MapControl.M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
            if ((index >= 221 && index <= 289) || (index >= 583 && index <= 658) || (index >= 1183 && index <= 1206) ||
                (index >= 7163 && index <= 7295) || (index >= 7404 && index <= 7414))
                moveSound = SoundList.WalkStoneL;
            else if ((index >= 3125 && index <= 3267) || (index >= 3757 && index <= 3948) || (index >= 6030 && index <= 6999))
                moveSound = SoundList.WalkWoodL;
            if (index >= 3316 && index <= 3589)
                moveSound = SoundList.WalkRoomL;

            if (CurrentAction == MirAction.Running) moveSound += 2;
            if (FrameIndex == 4) moveSound++;

            SoundManager.PlaySound(moveSound);
        }
        public void PlayStruckSound()
        {
            int add = 0;
            if (Class != MirClass.Assassin)
            switch (Armour)
            {
                case 3:
                case 6:
                case 9:
                    add = 10;
                    break;
            }

            switch (StruckWeapon)
            {
                case 0:
                case 23:
                case 1:
                case 12:
                case 28:
                case 40:
                    SoundManager.PlaySound(SoundList.StruckBodySword + add);
                    break;
                case 2:
                case 8:
                case 11:
                case 15:
                case 18:
                case 20:
                case 25:
                case 31:
                case 33:
                case 34:
                case 37:
                case 41:
                    SoundManager.PlaySound(SoundList.StruckBodySword + add);
                    break;
                case 3:
                case 5:
                case 7:
                case 9:
                case 13:
                case 19:
                case 24:
                case 26:
                case 29:
                case 32:
                case 35:
                    SoundManager.PlaySound(SoundList.StruckBodySword + add);
                    break;
                case 4:
                case 14:
                case 16:
                case 38:
                    SoundManager.PlaySound(SoundList.StruckBodyAxe + add);
                    break;
                case 6:
                case 10:
                case 17:
                case 22:
                case 27:
                case 30:
                case 36:
                case 39:
                    SoundManager.PlaySound(SoundList.StruckBodyLongStick + add);
                    break;
                case 21:
                    SoundManager.PlaySound(SoundList.StruckBodyLongStick + add);
                    break;
                case -1:
                    SoundManager.PlaySound(SoundList.StruckBodyFist + add);
                    break;
            }
        }
        public void PlayFlinchSound()
        {
            SoundManager.PlaySound(FlinchSound);
        }
        public void PlayAttackSound()
        {
            if (Weapon >= 0 && Class == MirClass.Assassin)
            {
                SoundManager.PlaySound(SoundList.SwingShort);
                return;
            }

            switch (Weapon)
            {
                case 0:
                case 23:
                case 28:
                case 40:
                    SoundManager.PlaySound(SoundList.SwingWood);
                    break;
                case 1:
                case 12:
                    SoundManager.PlaySound(SoundList.SwingShort);
                    break;
                case 2:
                case 8:
                case 11:
                case 15:
                case 18:
                case 20:
                case 25:
                case 31:
                case 33:
                case 34:
                case 37:
                case 41:
                    SoundManager.PlaySound(SoundList.SwingSword);
                    break;
                case 3:
                case 5:
                case 7:
                case 9:
                case 13:
                case 19:
                case 24:
                case 26:
                case 29:
                case 32:
                case 35:
                    SoundManager.PlaySound(SoundList.SwingSword2);
                    break;
                case 4:
                case 14:
                case 16:
                case 38:
                    SoundManager.PlaySound(SoundList.SwingAxe);
                    break;
                case 6:
                case 10:
                case 17:
                case 22:
                case 27:
                case 30:
                case 36:
                case 39:
                    SoundManager.PlaySound(SoundList.SwingLong);
                    break;
                case 21:
                    SoundManager.PlaySound(SoundList.SwingClub);
                    break;
                default:
                    SoundManager.PlaySound(SoundList.SwingFist);
                    break;
            }
        }

        public void PlayDieSound()
        {

        }


        public override void Draw()
        {
            float oldOpacity = DXManager.Opacity;
            if (Hidden && !DXManager.Blending) DXManager.SetOpacity(0.5F);

            if (Direction == MirDirection.Left || Direction == MirDirection.Up || Direction == MirDirection.UpLeft || Direction == MirDirection.DownLeft)
                DrawWeapon();
            else
                DrawWeapon2();

            DrawBody();

            DrawHead();

            if (this != User) DrawWings();

            if (Direction == MirDirection.UpRight || Direction == MirDirection.Right || Direction == MirDirection.DownRight || Direction == MirDirection.Down)
                DrawWeapon();
            else
                DrawWeapon2();

            DXManager.SetOpacity(oldOpacity);
            
        }
        public override void DrawEffects()
        {
            for (int i = 0; i < Effects.Count; i++)
                Effects[i].Draw();

            switch (CurrentAction)
            {
                case MirAction.Attack1:
                    switch (Spell)
                    {
                        case Spell.Slaying:
                            Libraries.Magic.DrawBlend(1820 + ((int)Direction * 10) + SpellLevel * 90 + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.DoubleSlash:
                            Libraries.Magic2.DrawBlend(1960 + ((int)Direction * 10) + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.Thrusting:
                            Libraries.Magic.DrawBlend(2190 + ((int)Direction * 10) + SpellLevel * 90 + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.HalfMoon:
                            Libraries.Magic.DrawBlend(2560 + ((int)Direction * 10) + SpellLevel * 90 + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.TwinDrakeBlade:
                            Libraries.Magic2.DrawBlend(220 + ((int)Direction * 20) + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.CrossHalfMoon:
                            Libraries.Magic2.DrawBlend(40 + ((int)Direction * 10) + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.FlamingSword:
                            Libraries.Magic.DrawBlend(3480 + ((int)Direction * 10) + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                    }
                    break;
                case MirAction.Attack4:

                    switch (Spell)
                    {
                        case Spell.DoubleSlash:
                            Libraries.Magic2.DrawBlend(2050 + ((int)Direction * 10) + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.TwinDrakeBlade:
                            Libraries.Magic2.DrawBlend(226 + ((int)Direction * 20) + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                        case Spell.FlamingSword:
                            Libraries.Magic.DrawBlend(3480 + ((int)Direction * 10) + FrameIndex, DrawLocation, Color.White, true, 0.7F);
                            break;
                    }
                    break;
            }


        }
        public override void DrawBlend()
        {
            DXManager.SetBlend(true, 0.3F);
            Draw();
            DXManager.SetBlend(false);
        }
        public void DrawBody()
        {
            if (BodyLibrary != null)
            {
                BodyLibrary.Draw(DrawFrame + ArmourOffSet, DrawLocation, DrawColour, true);
                //BodyLibrary.DrawTinted(DrawFrame + ArmourOffSet, DrawLocation, DrawColour, Color.DarkSeaGreen);
            }
        }
        public void DrawHead()
        {
            if (HairLibrary != null)
                HairLibrary.Draw(DrawFrame + HairOffSet, DrawLocation, DrawColour, true);
        }
        public void DrawWeapon()
        {
            if (Weapon < 0) return;

            if (WeaponLibrary1 != null)
                WeaponLibrary1.Draw(DrawFrame + WeaponOffSet, DrawLocation, DrawColour, true);
        }
        public void DrawWeapon2()
        {
            if (Weapon == -1) return;

            if (WeaponLibrary2 != null)
                WeaponLibrary2.Draw(DrawFrame + WeaponOffSet, DrawLocation, DrawColour, true);
        }
        public void DrawWings()
        {
            if (WingEffect <= 0) return;

            if (WingLibrary != null)
                WingLibrary.DrawBlend(DrawWingFrame + WingOffset, DrawLocation, DrawColour, true);
        }

        public override bool MouseOver(Point p)
        {
            return MapControl.MapLocation == CurrentLocation || BodyLibrary != null && BodyLibrary.VisiblePixel(DrawFrame + ArmourOffSet, p.Subtract(FinalDrawLocation), false);
        }

    }


    public class QueuedAction
    {
        public MirAction Action;
        public Point Location;
        public MirDirection Direction;
        public List<object> Params;
    }
}

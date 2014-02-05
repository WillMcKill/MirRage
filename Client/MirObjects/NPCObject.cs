using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Client.MirGraphics;
using Client.MirScenes;
using S = ServerPackets;

namespace Client.MirObjects
{
    class NPCObject :MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Merchant; }
        }
        public override bool Blocking
        {
            get { return true; }
        }

        public FrameSet Frames;
        public Frame Frame;
        public int BaseIndex, FrameIndex, FrameInterval;

        public NPCObject(uint objectID) : base(objectID)
        {
        }

        public void Load(S.ObjectNPC info)
        {
            Name = info.Name;
            NameColour = info.NameColour;

            CurrentLocation = info.Location;
            Movement = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);
            Direction = info.Direction;
            if (info.Image < Libraries.NPCs.Length)
                BodyLibrary = Libraries.NPCs[info.Image];

            switch (info.Image)
            {
                case 23:
                    Frames = FrameSet.NPCs[1];
                    break;
                default:
                    Frames = FrameSet.NPCs[0];
                    break;
            }

            Light = 10;

            SetAction();
        }


        public override void Process()
        {
            bool update = CMain.Time >= NextMotion || GameScene.CanMove;

            ProcessFrames();

            if (Frame == null)
                DrawFrame = 0;
            else
                DrawFrame = Frame.Start + (Frame.OffSet * (byte)Direction) + FrameIndex;

            DrawY = CurrentLocation.Y;

            DrawLocation = new Point((Movement.X - User.Movement.X + MapControl.OffSetX) * MapControl.CellWidth, (Movement.Y - User.Movement.Y + MapControl.OffSetY) * MapControl.CellHeight);
            DrawLocation.Offset(User.OffSetMove);
            if (BodyLibrary != null)
                FinalDrawLocation = DrawLocation.Add(BodyLibrary.GetOffSet(DrawFrame));

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
        public virtual void ProcessFrames()
        {
            if (Frame == null) return;

            switch (CurrentAction)
            {
                case MirAction.Standing:
                case MirAction.Harvest:
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
                    break;

            }

        }
        public int UpdateFrame()
        {
            if (Frame == null) return 0;

            if (Frame.Reverse) return Math.Abs(--FrameIndex);

            return ++FrameIndex;
        }

        public virtual void SetAction()
        {
            if (ActionFeed.Count == 0)
            {
                CurrentAction = MirAction.Standing;

                Frames.Frames.TryGetValue(CurrentAction, out Frame);
                FrameIndex = 0;

                if (MapLocation != CurrentLocation)
                {
                    GameScene.Scene.MapControl.RemoveObject(this);
                    MapLocation = CurrentLocation;
                    GameScene.Scene.MapControl.AddObject(this);
                }

                if (Frame == null) return;

                FrameInterval = Frame.Interval;
            }
            else
            {
                QueuedAction action = ActionFeed[0];
                ActionFeed.RemoveAt(0);

                CurrentAction = action.Action;
                CurrentLocation = action.Location;
                Direction = action.Direction;
                
                FrameIndex = 0;

                if (Frame == null) return;

                FrameInterval = Frame.Interval;
            }

            NextMotion = CMain.Time + FrameInterval;
            GameScene.Scene.MapControl.TextureValid = false;

        }
        public override void Draw()
        {
            if (BodyLibrary == null) return;

            BodyLibrary.Draw(DrawFrame, DrawLocation, DrawColour, true);
        }

        public override bool MouseOver(Point p)
        {
            return MapControl.MapLocation == CurrentLocation || BodyLibrary != null && BodyLibrary.VisiblePixel(DrawFrame, p.Subtract(FinalDrawLocation), false);
        }

        public override void DrawEffects()
        {
            //Time Stone
        }
    }
}

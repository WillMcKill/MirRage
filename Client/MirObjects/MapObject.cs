using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirScenes;

namespace Client.MirObjects
{
    public abstract class MapObject
    {
        public static Font ChatFont = new Font(Settings.FontName, 10F);
        public static List<MirLabel> LabelList = new List<MirLabel>();

        public static UserObject User;
        public static MapObject MouseObject, TargetObject, MagicObject;
        public abstract ObjectType Race { get; }
        public abstract bool Blocking { get; }


        public uint ObjectID;
        public string Name = string.Empty;
        public Point CurrentLocation, MapLocation;
        public MirDirection Direction;
        public bool Dead, Hidden, SitDown;
        public PoisonType Poison;
        public long DeadTime;
        public byte AI;
        public bool InTrapRock;

        public byte PercentHealth;
        public long HealthTime;

        public List<QueuedAction> ActionFeed = new List<QueuedAction>();
        public QueuedAction NextAction
        {
            get { return ActionFeed.Count > 0 ? ActionFeed[0] : null; }
        }
        public List<Effect> Effects = new List<Effect>();
        public MLibrary BodyLibrary;
        public Color DrawColour = Color.White, NameColour = Color.White;
        public MirLabel NameLabel, ChatLabel;
        public long ChatTime;
        public int DrawFrame, DrawWingFrame;
        public Point DrawLocation, Movement, FinalDrawLocation, OffSetMove;
        public Rectangle DisplayRectangle;
        public int Light, DrawY;
        public long NextMotion, NextMotion2;
        public MirAction CurrentAction;
        public bool SkipFrames;
        
        //Sound
        public int StruckWeapon;



        protected MapObject(uint objectID)
        {
            ObjectID = objectID;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != ObjectID) continue;
                ob.Remove();
            }

            MapControl.Objects.Add(this);
        }
        public void Remove()
        {
            if (MouseObject == this) MouseObject = null;
            if (TargetObject == this) TargetObject = null;
            if (MagicObject == this) MagicObject = null;

            if (this == User.NextMagicObject)
                User.ClearMagic();

            MapControl.Objects.Remove(this);
            GameScene.Scene.MapControl.RemoveObject(this);

            if (ObjectID != GameScene.NPCID) return;

            GameScene.NPCID = 0;
            GameScene.Scene.NPCDialog.Hide();
        }

        public abstract void Process();
        public abstract void Draw();
        public abstract bool MouseOver(Point p);


        public void Chat(string text)
        {
            if (ChatLabel != null && !ChatLabel.IsDisposed)
            {
                ChatLabel.Dispose();
                ChatLabel = null;
            }

            const int chatWidth = 200;
            List<string> chat = new List<string>();

            int index = 0;
            for (int i = 1; i < text.Length; i++)
                if (TextRenderer.MeasureText(CMain.Graphics, text.Substring(index, i - index), ChatFont).Width > chatWidth)
                {
                    chat.Add(text.Substring(index, i - index - 1));
                    index = i - 1;
                }
            chat.Add(text.Substring(index, text.Length - index));

            text = chat[0];
            for (int i = 1; i < chat.Count; i++)
                text += string.Format("\n{0}", chat[i]);

            ChatLabel = new MirLabel
            {
                AutoSize = true,
                BackColour = Color.Transparent,
                ForeColour = Color.White,
                OutLine = true,
                OutLineColour = Color.Black,
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Text = text,
            };
            ChatTime = CMain.Time + 5000;
        }
        public void DrawChat()
        {
            if (ChatLabel == null || ChatLabel.IsDisposed) return;

            if (CMain.Time > ChatTime)
            {
                ChatLabel.Dispose();
                ChatLabel = null;
                return;
            }

            ChatLabel.ForeColour = Dead ? Color.Gray : Color.White;
            ChatLabel.Location = new Point(DisplayRectangle.X + (48 - ChatLabel.Size.Width) / 2, DisplayRectangle.Y - (60 + ChatLabel.Size.Height) - (Dead ? 35 : 0));
            ChatLabel.Draw();
        }

        private void CreateLabel()
        {
            NameLabel = null;

            for (int i = 0; i < LabelList.Count; i++)
            {
                if (LabelList[i].Text != Name || LabelList[i].ForeColour != NameColour) continue;
                NameLabel = LabelList[i];
                break;
            }


            if (NameLabel != null && !NameLabel.IsDisposed) return;

            NameLabel = new MirLabel
            {
                AutoSize = true,
                BackColour = Color.Transparent,
                ForeColour = NameColour,
                OutLine = true,
                OutLineColour = Color.Black,
                Text = Name,
            };
            NameLabel.Disposing += (o, e) => LabelList.Remove(NameLabel);
            LabelList.Add(NameLabel);

        }
        public virtual void DrawName()
        {
            CreateLabel();

            if (NameLabel == null) return;

            NameLabel.Text = Name;
            NameLabel.Location = new Point(DisplayRectangle.X + (48 - NameLabel.Size.Width) / 2, DisplayRectangle.Y - (32 - NameLabel.Size.Height / 2) + (Dead ? 35 : 8));
            NameLabel.Draw();
        }
        public virtual void DrawBlend()
        {
            DXManager.SetBlend(true, 0.8F);
            Draw();
            DXManager.SetBlend(false);
        }
        public void DrawHealth()
        {
            string name = Name;

            if (Name.Contains("(")) name = Name.Substring(Name.IndexOf("(") + 1, Name.Length - Name.IndexOf("(") - 2);


            if (PercentHealth == 0 || Dead) return;

            if (CMain.Time >= HealthTime)
            {
                if (Race == ObjectType.Monster && !Name.EndsWith(string.Format("({0})", User.Name)) && !GroupDialog.GroupList.Contains(name)) return;
                if (Race == ObjectType.Player && this != User && !GroupDialog.GroupList.Contains(Name)) return;
                if (this == User && GroupDialog.GroupList.Count == 0) return;
            }



            Libraries.Prguse2.Draw(0, DisplayRectangle.X + 8, DisplayRectangle.Y - 64);
            int index = 1;

            switch (Race)
            {
                case ObjectType.Player:
                    index = 10;
                    break;
                case ObjectType.Monster:
                    if (GroupDialog.GroupList.Contains(name) || name == User.Name) index = 11;
                    break;
            }

            Libraries.Prguse2.Draw(index, new Rectangle(0, 0, (int)(32 * PercentHealth / 100F), 4), new Point(DisplayRectangle.X + 8, DisplayRectangle.Y - 64), Color.White, false);
        }

        public abstract void DrawEffects();
    }
}

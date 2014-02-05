using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Client.MirControls;
using Client.MirGraphics;
using Client.MirNetwork;
using Client.MirObjects;
using Client.MirSounds;
using Microsoft.DirectX.Direct3D;
using Font = System.Drawing.Font;
using S = ServerPackets;
using C = ClientPackets;
using Effect = Client.MirObjects.Effect;

namespace Client.MirScenes
{
    public enum PanelType { Sell, Repair, SpecialRepair,
        Consign
    }

    public sealed class GameScene : MirScene
    {
        public static GameScene Scene;

        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public static long MoveTime, AttackTime, NextRunTime;
        public static bool CanMove, CanRun;

        public MapControl MapControl;
        public MainDialog MainDialog;
        public ChatDialog ChatDialog;
        public ChatControlBar ChatControl;
        public InventoryDialog InventoryDialog;
        public CharacterDialog CharacterDialog;
        public StorageDialog StorageDialog;
        public BeltDialog BeltDialog;
        public MiniMapDialog MiniMapDialog;
        public InspectDialog InspectDialog;
        public OptionDialog OptionDialog;
        public MenuDialog MenuDialog;
        public NPCDialog NPCDialog;
        public NPCGoodsDialog NPCGoodsDialog;
        public NPCDropDialog NPCDropDialog;
        public GroupDialog GroupDialog;
        public BigMapDialog BigMapDialog;
        public TrustMerchantDialog TrustMerchantDialog;
        
        public static List<ItemInfo> ItemInfoList = new List<ItemInfo>();

        public List<Buff> Buffs = new List<Buff>();

        public static UserItem[] Storage = new UserItem[80];
        public static UserItem HoverItem;
        public static MirItemCell SelectedCell;
        public static bool PickedUpGold;
        public MirControl ItemLabel;
        public static long UseItemTime, PickUpTime;
        public static uint Gold;
        public static long InspectTime;
        public bool ShowReviveMessage;

        public AttackMode AMode;
        public PetMode PMode;
        public LightSetting Lights;

        public static long NPCTime;
        public static uint NPCID;
        public static float NPCRate;

        public long ToggleTime;
        public static bool Slaying, Thrusting, HalfMoon, CrossHalfMoon, DoubleSlash, TwinDrakeBlade, FlamingSword;
        public static long SpellTime;


        public MirLabel[] OutputLines = new MirLabel[10];
        public List<OutPutMessage> OutputMessages = new List<OutPutMessage>();

        public List<MirImageControl> BuffList = new List<MirImageControl>();
        public static long PoisonFieldTime;

        public GameScene()
        {
            MapControl.AutoRun = false;
            Slaying = false;
            Thrusting = false;
            HalfMoon = false;
            CrossHalfMoon = false;
            DoubleSlash = false;
            TwinDrakeBlade = false;
            FlamingSword = false;

            GroupDialog.GroupList.Clear();

            Scene = this;
            BackColour = Color.Transparent;
            MoveTime = CMain.Time;

            KeyDown += GameScene_KeyDown;

            MainDialog = new MainDialog {Parent = this};
            ChatDialog = new ChatDialog {Parent = this};
            ChatControl = new ChatControlBar {Parent = this};
            InventoryDialog = new InventoryDialog {Parent = this};
            CharacterDialog = new CharacterDialog {Parent = this, Visible = false};
            BeltDialog = new BeltDialog {Parent = this};
            StorageDialog = new StorageDialog {Parent = this, Visible = false};
            MiniMapDialog = new MiniMapDialog {Parent = this};
            InspectDialog = new InspectDialog {Parent = this, Visible = false};
            OptionDialog = new OptionDialog {Parent = this, Visible = false};
            MenuDialog = new MenuDialog {Parent = this, Visible = false};
            NPCDialog = new NPCDialog {Parent = this, Visible = false};
            NPCGoodsDialog = new NPCGoodsDialog { Parent = this, Visible = false };
            NPCDropDialog = new NPCDropDialog { Parent = this, Visible = false };
            GroupDialog = new GroupDialog { Parent = this, Visible = false };
            BigMapDialog = new BigMapDialog {Parent = this, Visible = false};
            TrustMerchantDialog = new TrustMerchantDialog {Parent = this, Visible = false};

            for (int i = 0; i < OutputLines.Length; i++)
                OutputLines[i] = new MirLabel
                    {
                        AutoSize = true,
                        BackColour = Color.Transparent,
                        Font = new Font(Settings.FontName, 10F),
                        ForeColour = Color.LimeGreen,
                        Location = new Point(20, 20 + i*13),
                        OutLine = true,
                    };

        }

        public void OutputMessage(string message)
        {
            OutputMessages.Add(new OutPutMessage {Message = message, ExpireTime = CMain.Time + 5000});
            if (OutputMessages.Count > 10)
                OutputMessages.RemoveAt(0);
        }

        private void ProcessOuput()
        {
            for (int i = 0; i < OutputMessages.Count; i++)
            {
                if (CMain.Time >= OutputMessages[i].ExpireTime)
                    OutputMessages.RemoveAt(i);
            }

            for (int i = 0; i < OutputLines.Length; i++)
            {
                if (OutputMessages.Count > i)
                {
                    OutputLines[i].Text = OutputMessages[i].Message;
                    OutputLines[i].Visible = true;
                }
                else
                {
                    OutputLines[i].Text = string.Empty;
                    OutputLines[i].Visible = false;
                }
            }
        }
        private void GameScene_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                     case Keys.F1:
                     UseSpell(CMain.Ctrl ? 9 : 1);
                     break;
                 case Keys.F2:
                     UseSpell(CMain.Ctrl ? 10 : 2);
                     break;
                 case Keys.F3:
                     UseSpell(CMain.Ctrl ? 11 : 3);
                     break;
                 case Keys.F4:
                     UseSpell(CMain.Ctrl ? 12 : 4);
                     break;
                 case Keys.F5:
                     UseSpell(CMain.Ctrl ? 13 : 5);
                     break;
                 case Keys.F6:
                     UseSpell(CMain.Ctrl ? 14 : 6);
                     break;
                 case Keys.F7:
                     UseSpell(CMain.Ctrl ? 15 : 7);
                     break;
                 case Keys.F8:
                     UseSpell(CMain.Ctrl ? 16 : 8);
                     break;
                case Keys.I:
                case Keys.F9:
                    if (!InventoryDialog.Visible) InventoryDialog.Show();
                    else InventoryDialog.Hide();
                    break;
                case Keys.C:
                case Keys.F10:
                    if (!CharacterDialog.Visible || !CharacterDialog.CharacterPage.Visible)
                    {
                        CharacterDialog.Show();
                        CharacterDialog.ShowCharacterPage();
                    }
                    else CharacterDialog.Hide();
                    break;
                case Keys.S:
                case Keys.F11:
                    if (!CharacterDialog.Visible || !CharacterDialog.SkillPage.Visible)
                    {
                        CharacterDialog.Show();
                        CharacterDialog.ShowSkillPage();
                    }
                    else CharacterDialog.Hide();
                    break;
                case Keys.Escape:
                    InventoryDialog.Hide();
                    CharacterDialog.Hide();
                    OptionDialog.Hide();
                    MenuDialog.Hide();
                    NPCDialog.Hide();
                     GroupDialog.Hide();
                    InspectDialog.Hide();
                    StorageDialog.Hide();
                    TrustMerchantDialog.Hide();
                    BigMapDialog.Visible = false;
                    break;
                case Keys.O:
                case Keys.F12:
                    if (!OptionDialog.Visible) OptionDialog.Show();
                    else OptionDialog.Hide();
                    break;
                case Keys.P:
                     if (!GroupDialog.Visible) GroupDialog.Show();
                     else GroupDialog.Hide();
                    break;
                case Keys.Z:
                    if (CMain.Ctrl) BeltDialog.Flip();
                    else
                    {
                        if (!BeltDialog.Visible) BeltDialog.Show();
                        else BeltDialog.Hide();
                    }
                    break;
                case Keys.Tab:
                    if (CMain.Time > PickUpTime)
                    {
                        PickUpTime = CMain.Time + 200;
                        Network.Enqueue(new C.PickUp());
                    }
                    break;
                case Keys.NumPad1:
                case Keys.D1:
                    if (CMain.Shift) return;
                    BeltDialog.Grid[0].UseItem();
                    break;
                case Keys.NumPad2:
                case Keys.D2:
                    BeltDialog.Grid[1].UseItem();
                    break;
                case Keys.NumPad3:
                case Keys.D3:
                    BeltDialog.Grid[2].UseItem();
                    break;
                case Keys.NumPad4:
                case Keys.D4:
                    BeltDialog.Grid[3].UseItem();
                    break;
                case Keys.NumPad5:
                case Keys.D5:
                    BeltDialog.Grid[4].UseItem();
                    break;
                case Keys.NumPad6:
                case Keys.D6:
                    BeltDialog.Grid[5].UseItem();
                    break;
                case Keys.X:
                    if (!CMain.Alt) break;
                    LogOut();
                    break;
                case Keys.Q:
                    if (!CMain.Alt) break;
                    QuitGame();
                    break;
                case Keys.V:
                    MiniMapDialog.Toggle();
                    break;
                case Keys.B:
                    BigMapDialog.Toggle();
                    break;
                case Keys.A:
                    switch (PMode)
                    {
                        case PetMode.Both:
                            Network.Enqueue(new C.ChangePMode {Mode = PetMode.MoveOnly});
                            return;
                        case PetMode.MoveOnly:
                            Network.Enqueue(new C.ChangePMode {Mode = PetMode.AttackOnly});
                            return;
                        case PetMode.AttackOnly:
                            Network.Enqueue(new C.ChangePMode {Mode = PetMode.None});
                            return;
                        case PetMode.None:
                            Network.Enqueue(new C.ChangePMode {Mode = PetMode.Both});
                            return;
                    }
                    break;
                case Keys.H:
                    switch (AMode)
                    {
                        case AttackMode.Peace:
                            Network.Enqueue(new C.ChangeAMode {Mode = AttackMode.Group});
                            return;
                        case AttackMode.Group:
                            Network.Enqueue(new C.ChangeAMode {Mode = AttackMode.Guild});
                            return;
                        case AttackMode.Guild:
                            Network.Enqueue(new C.ChangeAMode {Mode = AttackMode.RedBrown});
                            return;
                        case AttackMode.RedBrown:
                            Network.Enqueue(new C.ChangeAMode {Mode = AttackMode.All});
                            return;
                        case AttackMode.All:
                            Network.Enqueue(new C.ChangeAMode {Mode = AttackMode.Peace});
                            return;
                    }
                    break;
                case Keys.D:
                    MapControl.AutoRun = !MapControl.AutoRun;
                    break;

            }
        }

        private void UseSpell(int key)
        {

            ClientMagic magic = null;

            for (int i = 0; i < User.Magics.Count; i++)
            {
                if (User.Magics[i].Key != key) continue;
                magic = User.Magics[i];
                break;
            }

            if (magic == null) return;

            int cost;
            switch (magic.Spell)
            {
                case Spell.Fencing:
                case Spell.FatalSword:
                case Spell.SpiritSword:
                case Spell.Slaying:
                    return;
                case Spell.Thrusting:
                    if (CMain.Time < ToggleTime) return;
                    Thrusting = !Thrusting;
                    ChatDialog.ReceiveChat(Thrusting ? "Use Thrusting." : "Do not use Thrusting.", ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    Network.Enqueue(new C.SpellToggle {Spell = magic.Spell, CanUse = Thrusting});
                    break;
                case Spell.HalfMoon:
                    if (CMain.Time < ToggleTime) return;
                    HalfMoon = !HalfMoon;
                    ChatDialog.ReceiveChat(HalfMoon ? "Use Half Moon." : "Do not use Half Moon.", ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = HalfMoon });
                    break;
                case Spell.CrossHalfMoon:
                    if (CMain.Time < ToggleTime) return;
                    CrossHalfMoon = !CrossHalfMoon;
                    ChatDialog.ReceiveChat(CrossHalfMoon ? "Use Cross Half Moon." : "Do not use Cross Half Moon.", ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = CrossHalfMoon });
                    break;
                case Spell.DoubleSlash:
                    if (CMain.Time < ToggleTime) return;
                    DoubleSlash = !DoubleSlash;
                    ChatDialog.ReceiveChat(DoubleSlash ? "Use Double Slash." : "Do not use Double Slash.", ChatType.Hint);
                    ToggleTime = CMain.Time + 1000;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = DoubleSlash });
                    break;
                case Spell.TwinDrakeBlade:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;
                    
                    cost = magic.Level*magic.LevelCost + magic.BaseCost;
                    if (cost > MapObject.User.MP)
                    {
                        Scene.OutputMessage("Not Enough Mana to cast.");
                        return;
                    }
                    TwinDrakeBlade = true;
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = true });
                    User.Effects.Add(new Effect(Libraries.Magic2, 210, 6, 500, User));
                    break;
                case Spell.FlamingSword:
                    if (CMain.Time < ToggleTime) return;
                    ToggleTime = CMain.Time + 500;

                    cost = magic.Level * magic.LevelCost + magic.BaseCost;
                    if (cost > MapObject.User.MP)
                    {
                        Scene.OutputMessage("Not Enough Mana to cast.");
                        return;
                    }
                    Network.Enqueue(new C.SpellToggle { Spell = magic.Spell, CanUse = true });
                    break;
                default:
                    User.NextMagic = magic;
                    User.NextMagicLocation = MapControl.MapLocation;
                    User.NextMagicObject = MapObject.MouseObject;
                    User.NextMagicDirection = MapControl.MouseDirection();
                    break;
            }
        }

        public void QuitGame()
        {
            //If Last Combat < 10 CANCEL
            MirMessageBox messageBox = new MirMessageBox("Do you want to quit Legend of Mir?", MirMessageBoxButtons.YesNo);
            messageBox.YesButton.Click += (o, e) => Program.Form.Close();
            messageBox.Show();
        }
        public void LogOut()
        {
            //If Last Combat < 10 CANCEL
            MirMessageBox messageBox = new MirMessageBox("Do you want to log out of Legend of Mir?", MirMessageBoxButtons.YesNo);
            messageBox.YesButton.Click += (o, e) =>
                {
                    Network.Enqueue(new C.LogOut());
                    Enabled = false;
                };
            messageBox.Show();
        }

        protected internal override void DrawControl()
        {
            if (MapControl != null && !MapControl.IsDisposed)
                MapControl.DrawControl();

            base.DrawControl();


            if (PickedUpGold || (SelectedCell != null && SelectedCell.Item != null))
            {
                int image = PickedUpGold ? 116 : SelectedCell.Item.Info.Image;
                Size imgSize = Libraries.Items.GetTrueSize(image);
                Point p = CMain.MPoint.Add(-imgSize.Width/2, -imgSize.Height/2);

                if (p.X + imgSize.Width >= Settings.ScreenWidth)
                    p.X = Settings.ScreenWidth - imgSize.Width;

                if (p.Y + imgSize.Height >= Settings.ScreenHeight)
                    p.Y = Settings.ScreenHeight - imgSize.Height;

                Libraries.Items.Draw(image, p.X, p.Y);
            }

            for (int i = 0; i < OutputLines.Length; i++)
                OutputLines[i].Draw();
        }
        public override void Process()
        {
            if (MapControl == null || User == null)
                return;

            if (CMain.Time >= MoveTime)
            {
                MoveTime += 100; //Move Speed
                CanMove = true;
                MapControl.AnimationCount++;
                MapControl.TextureValid = false;
            }
            else
                CanMove = false;

            MirItemCell cell = MouseControl as MirItemCell;

            if (cell != null && HoverItem != cell.Item)
            {
                DisposeItemLabel();
                HoverItem = null;
                CreateItemLabel(cell.Item);
            }

            if (ItemLabel != null && !ItemLabel.IsDisposed)
            {
                ItemLabel.BringToFront();

                int x = CMain.MPoint.X + 15, y = CMain.MPoint.Y;
                if (x + ItemLabel.Size.Width > Settings.ScreenWidth)
                    x = Settings.ScreenWidth - ItemLabel.Size.Width;

                if (y + ItemLabel.Size.Height > Settings.ScreenHeight)
                    y = Settings.ScreenHeight - ItemLabel.Size.Height;
                ItemLabel.Location = new Point(x, y);
            }

            if (!User.Dead) ShowReviveMessage = false;

            if (ShowReviveMessage && CMain.Time > User.DeadTime && User.CurrentAction == MirAction.Dead)
            {
                ShowReviveMessage = false;
                MirMessageBox messageBox = new MirMessageBox("You're dead, thats it. Bye!");

                messageBox.Show();
            }

            UpdateBuffs();
            MapControl.Process();
            MainDialog.Process();
            InventoryDialog.Process();
            MiniMapDialog.Process();
            ProcessOuput();
        }
        public override void ProcessPacket(Packet p)
        {
            switch (p.Index)
            {
                case 16: //MapInfo
                    MapInformation((S.MapInformation) p);
                    break;
                case 17:
                    UserInformation((S.UserInformation) p);
                    break;
                case 18:
                    UserLocation((S.UserLocation) p);
                    break;
                case 19:
                    ObjectPlayer((S.ObjectPlayer) p);
                    break;
                case 20:
                    ObjectRemove((S.ObjectRemove) p);
                    break;
                case 21:
                    ObjectTurn((S.ObjectTurn) p);
                    break;
                case 22:
                    ObjectWalk((S.ObjectWalk) p);
                    break;
                case 23:
                    ObjectRun((S.ObjectRun) p);
                    break;
                case 24:
                    ReceiveChat((S.Chat) p);
                    break;
                case 25:
                    ObjectChat((S.ObjectChat) p);
                    break;
                case 27:
                    MoveItem((S.MoveItem) p);
                    break;
                case 28:
                    EquipItem((S.EquipItem) p);
                    break;
                case 29:
                    MergeItem((S.MergeItem) p);
                    break;
                case 30:
                    RemoveItem((S.RemoveItem) p);
                    break;
                case 31:
                    TakeBackItem((S.TakeBackItem) p);
                    break;
                case 32:
                    StoreItem((S.StoreItem) p);
                    break;
                case 33:
                    SplitItem((S.SplitItem) p);
                    break;
                case 34:
                    SplitItem1((S.SplitItem1) p);
                    break;
                case 35:
                    UseItem((S.UseItem) p);
                    break;
                case 36:
                    DropItem((S.DropItem) p);
                    break;
                case 37:
                    PlayerUpdate((S.PlayerUpdate) p);
                    break;
                case 38:
                    PlayerInspect((S.PlayerInspect) p);
                    break;
                case 39:
                    LogOutSuccess((S.LogOutSuccess) p);
                    break;
                case 40:
                    TimeOfDay((S.TimeOfDay) p);
                    break;
                case 41:
                    ChangeAMode((S.ChangeAMode) p);
                    break;
                case 42:
                    ChangePMode((S.ChangePMode) p);
                    break;
                case 43:
                    ObjectItem((S.ObjectItem) p);
                    break;
                case 44:
                    ObjectGold((S.ObjectGold) p);
                    break;
                case 45:
                    GainedItem((S.GainedItem) p);
                    break;
                case 46:
                    GainedGold((S.GainedGold) p);
                    break;
                case 47:
                    LoseGold((S.LoseGold) p);
                    break;
                case 48:
                    ObjectMonster((S.ObjectMonster) p);
                    break;
                case 49:
                    ObjectAttack((S.ObjectAttack) p);
                    break;
                case 50:
                    Struck((S.Struck) p);
                    break;
                case 51:
                    ObjectStruck((S.ObjectStruck) p);
                    break;
                case 52:
                    DuraChanged((S.DuraChanged) p);
                    break;
                case 53:
                    HealthChanged((S.HealthChanged) p);
                    break;
                case 54:
                    DeleteItem((S.DeleteItem) p);
                    break;
                case 55:
                    Death((S.Death) p);
                    break;
                case 56:
                    ObjectDied((S.ObjectDied) p);
                    break;
                case 57:
                    ColourChanged((S.ColourChanged) p);
                    break;
                case 58:
                    ObjectColourChanged((S.ObjectColourChanged) p);
                    break;
                case 59:
                    GainExperience((S.GainExperience) p);
                    break;
                case 60:
                    LevelChanged((S.LevelChanged) p);
                    break;
                case 61:
                    ObjectLeveled((S.ObjectLeveled) p);
                    break;
                case 62:
                    ObjectHarvest((S.ObjectHarvest) p);
                    break;
                case 63:
                    ObjectHarvested((S.ObjectHarvested) p);
                    break;
                case 64:
                    ObjectNPC((S.ObjectNPC) p);
                    break;
                case 65:
                    NPCResponse((S.NPCResponse) p);
                    break;
                case 66:
                    ObjectHide((S.ObjectHide) p);
                    break;
                case 67:
                    ObjectShow((S.ObjectShow) p);
                    break;
                case 68:
                    Poisoned((S.Poisoned) p);
                    break;
                case 69:
                    ObjectPoisoned((S.ObjectPoisoned) p);
                    break;
                case 70:
                    MapChanged((S.MapChanged) p);
                    break;
                case 71:
                    ObjectTeleportOut((S.ObjectTeleportOut) p);
                    break;
                case 72:
                    ObjectTeleportIn((S.ObjectTeleportIn) p);
                    break;
                case 73:
                    TeleportIn();
                    break;
                case 74:
                    NPCGoods((S.NPCGoods) p);
                    break;
                case 75:
                    NPCSell();
                    break;
                case 76:
                    NPCRepair((S.NPCRepair) p);
                    break;
                case 77:
                    NPCSRepair((S.NPCSRepair) p);
                    break;
                case 78:
                    NPCStorage();
                    break;
                case 79:
                    SellItem((S.SellItem)p);
                    break;
                case 80:
                    RepairItem((S.RepairItem)p);
                    break;
                case 81:
                    ItemRepaired((S.ItemRepaired)p);
                    break;
                case 82:
                    NewMagic((S.NewMagic)p);
                    break;
                case 83:
                    MagicLeveled((S.MagicLeveled)p);
                    break;
                case 84:
                    Magic((S.Magic)p);
                    break;
                case 85:
                    ObjectMagic((S.ObjectMagic)p);
                    break;
                case 86:
                    ObjectEffect((S.ObjectEffect)p);
                    break;
                case 87:
                    Pushed((S.Pushed)p);
                    break;
                case 88:
                    ObjectPushed((S.ObjectPushed)p);
                    break;
                case 89:
                    ObjectName((S.ObjectName)p);
                    break;
                case 90:
                    UserStorage((S.UserStorage)p);
                    break;
                case 91:
                    SwitchGroup((S.SwitchGroup) p);
                    break;
                case 92:
                    DeleteGroup();
                    break;
                case 93:
                    DeleteMember((S.DeleteMember)p);
                    break;
                case 94:
                    GroupInvite((S.GroupInvite)p);
                    break;
                case 95:
                    AddMember((S.AddMember)p);
                    break;
                case 96:
                    Revived();
                    break;
                case 97:
                    ObjectRevived((S.ObjectRevived)p);
                    break;
                case 98:
                    SpellToggle((S.SpellToggle)p);
                    break;
                case 99:
                    ObjectHealth((S.ObjectHealth)p);
                    break;
                case 100:
                    MapEffect((S.MapEffect) p);
                    break;
                case 101:
                    ObjectRangeAttack((S.ObjectRangeAttack)p);
                    break;
                case 102:
                    AddBuff((S.AddBuff)p);
                    break;
                case 103:
                    RemoveBuff((S.RemoveBuff)p);
                    break;
                case 104:
                    ObjectHidden((S.ObjectHidden) p);
                    break;
                case 105:
                    RefreshItem((S.RefreshItem) p);
                    break;
                case 106:
                    ObjectSpell((S.ObjectSpell)p);
                    break;
                case 107:
                    UserDash((S.UserDash)p);
                    break;
                case 108:
                    ObjectDash((S.ObjectDash)p);
                    break;
                case 109:
                    UserDashFail((S.UserDashFail)p);
                    break;
                case 110:
                    ObjectDashFail((S.ObjectDashFail)p);
                    break;
                case 111:
                    NPCConsign();
                    break;
                case 112:
                    NPCMarket((S.NPCMarket)p);
                    break;
                case 113:
                    NPCMarketPage((S.NPCMarketPage)p);
                    break;
                case 114:
                    ConsignItem((S.ConsignItem)p);
                    break;
                case 115:
                    MarketFail((S.MarketFail)p);
                    break;
                case 116:
                    MarketSuccess((S.MarketSuccess)p);
                    break;
                case 117:
                    ObjectSitDown((S.ObjectSitDown)p);
                    break;
                case 118:
                    S.InTrapRock packetdata = (S.InTrapRock)p;
                    User.InTrapRock = packetdata.Trapped;
                    break;
                case 119:
                    RemoveMagic((S.RemoveMagic)p);
                    break;
                default:
                    base.ProcessPacket(p);
                    break;
            }
        }

        public void CreateBuff(Buff buff)
        {
            MirImageControl image =  new MirImageControl
                {
                    Library = Libraries.Prguse,
                    Parent = this,
                    Visible = true,
                };

            new MirLabel
                {
                    DrawFormat = TextFormatFlags.Right,
                    NotControl = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(-7, 10),
                    Size = new Size(30, 20),
                    Parent = image
                };

            switch (buff.Type)
            {
                case BuffType.Teleport:
                    image.Index = 885;
                    break;
                case BuffType.Hiding:
                    image.Index = 884;
                    break;
                case BuffType.Haste:
                    image.Index = 880;
                    break;
                case BuffType.LightBody:
                    image.Index = 882;
                    break;
                case BuffType.SoulShield:
                    image.Index = 870;
                    break;
                case BuffType.BlessedArmour:
                    image.Index = 871;
                    break;
                case BuffType.ProtectionField:
                    image.Index = 861;
                    break;
                case BuffType.Rage:
                    image.Index = 860;
                    break;
            }

            BuffList.Insert(0, image);
        }
        public void UpdateBuffs()
        {
            for (int i = 0; i < BuffList.Count; i++)
            {
                MirImageControl image = BuffList[i];
                Buff buff = Buffs[i];
                image.Location = new Point((Settings.ScreenWidth - 160) - i * 30, 15);
                image.Hint = buff.ToString();
                ((MirLabel)image.Controls[0]).Text = buff.Infinite ? "" : Math.Round((buff.Expire - CMain.Time) / 1000D).ToString();

                switch (buff.Type)
                {
                    case BuffType.Teleport:
                        image.Index = 885;
                        break;
                    case BuffType.Hiding:
                        image.Index = 884;
                        break;
                    case BuffType.Haste:
                        image.Index = 880;
                        break;
                    case BuffType.LightBody:
                        image.Index = 882;
                        break;
                    case BuffType.SoulShield:
                        image.Index = 870;
                        break;
                    case BuffType.BlessedArmour:
                        image.Index = 871;
                        break;
                }

            }
        }
        
        private void MapInformation(S.MapInformation p)
        {
            if (MapControl != null && !MapControl.IsDisposed)
                MapControl.Dispose();

            MapControl = new MapControl { FileName = Path.Combine(Settings.MapPath, p.FileName + ".map"), Title = p.Title, MiniMap = p.MiniMap, BigMap = p.BigMap, Lights = p.Lights, Lightning = p.Lightning, Fire = p.Fire };
            MapControl.LoadMap();
            InsertControl(0, MapControl);
        }
        private void UserInformation(S.UserInformation p)
        {
            User = new UserObject(p.ObjectID);
            User.Load(p);
            MainDialog.PModeLabel.Visible = User.Class == MirClass.Wizard || User.Class == MirClass.Taoist;
            Gold = p.Gold;
        }
        private void UserLocation(S.UserLocation p)
        {
            MapControl.NextAction = 0;
            if (User.CurrentLocation == p.Location && User.Direction == p.Direction) return;


            ReceiveChat(new S.Chat {Message = "Displacement", Type = ChatType.System});
            
            MapControl.RemoveObject(User);
            User.CurrentLocation = p.Location;
            User.MapLocation = p.Location;
            MapControl.AddObject(User);

            MapControl.FloorValid = false;
            MapControl.InputDelay = CMain.Time + 400;

            if (User.Dead) return;

            User.ClearMagic();
            User.QueuedAction = null;

            for (int i = User.ActionFeed.Count - 1; i >= 0; i--)
            {
                if (User.ActionFeed[i].Action == MirAction.Pushed) continue;
                User.ActionFeed.RemoveAt(i);
            }

            User.SetAction();
        }
        private void ReceiveChat(S.Chat p)
        {
            ChatDialog.ReceiveChat(p.Message, p.Type);
        }
        private void ObjectPlayer(S.ObjectPlayer p)
        {
            PlayerObject player = new PlayerObject(p.ObjectID);
            player.Load(p);
        }
        private void ObjectRemove(S.ObjectRemove p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Remove();
            }

        }
        private void ObjectTurn(S.ObjectTurn p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction {Action = MirAction.Standing, Direction = p.Direction, Location = p.Location});
                return;
            }
        }
        private void ObjectWalk(S.ObjectWalk p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction {Action = MirAction.Walking, Direction = p.Direction, Location = p.Location});
                return;
            }
        }
        private void ObjectRun(S.ObjectRun p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction {Action = MirAction.Running, Direction = p.Direction, Location = p.Location});
                return;
            }
        }
        private void ObjectChat(S.ObjectChat p)
        {
            ChatDialog.ReceiveChat(p.Text, p.Type);

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Chat(p.Text);
                return;
            }

        }
        private void MoveItem(S.MoveItem p)
        {
            MirItemCell toCell, fromCell;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = p.From < 40 ? InventoryDialog.Grid[p.From] : BeltDialog.Grid[p.From - 40];
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.Grid[p.From];
                    break;
                default:
                    return;
            }

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    toCell = p.To < 40 ? InventoryDialog.Grid[p.To] : BeltDialog.Grid[p.To - 40];
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.Grid[p.To];
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            UserItem i = fromCell.Item;
            fromCell.Item = toCell.Item;
            toCell.Item = i;
            User.RefreshStats();
        }
        private void EquipItem(S.EquipItem p)
        {
            MirItemCell fromCell;

            MirItemCell toCell = CharacterDialog.Grid[p.To];

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    fromCell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;

            UserItem i = fromCell.Item;
            fromCell.Item = toCell.Item;
            toCell.Item = i;
            User.RefreshStats();
        }
        private void MergeItem(S.MergeItem p)
        {
            MirItemCell toCell, fromCell;

            switch (p.GridFrom)
            {
                case MirGridType.Inventory:
                    fromCell = InventoryDialog.GetCell(p.IDFrom) ?? BeltDialog.GetCell(p.IDFrom);
                    break;
                case MirGridType.Storage:
                    fromCell = StorageDialog.GetCell(p.IDFrom);
                    break;
                case MirGridType.Equipment:
                    fromCell = CharacterDialog.GetCell(p.IDFrom);
                    break;
                default:
                    return;
            }

            switch (p.GridTo)
            {
                case MirGridType.Inventory:
                    toCell = InventoryDialog.GetCell(p.IDTo) ?? BeltDialog.GetCell(p.IDTo);
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.GetCell(p.IDTo);
                    break;
                case MirGridType.Equipment:
                    toCell = CharacterDialog.GetCell(p.IDTo);
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;
            if (!p.Success) return;
            if (fromCell.Item.Count <= toCell.Item.Info.StackSize - toCell.Item.Count)
            {
                toCell.Item.Count += fromCell.Item.Count;
                fromCell.Item = null;
            }
            else
            {
                fromCell.Item.Count -= toCell.Item.Info.StackSize - toCell.Item.Count;
                toCell.Item.Count = toCell.Item.Info.StackSize;
            }

            User.RefreshStats();
        }
        private void RemoveItem(S.RemoveItem p)
        {
            MirItemCell toCell;

            int index = -1;

            for (int i = 0; i < MapObject.User.Equipment.Length; i++)
            {
                if (MapObject.User.Equipment[i] == null || MapObject.User.Equipment[i].UniqueID != p.UniqueID) continue;
                index = i;
                break;
            }

            MirItemCell fromCell = CharacterDialog.Grid[index];


            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    toCell = p.To < 40 ? InventoryDialog.Grid[p.To] : BeltDialog.Grid[p.To - 40];
                    break;
                case MirGridType.Storage:
                    toCell = StorageDialog.Grid[p.To];
                    break;
                default:
                    return;
            }

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
        private void TakeBackItem(S.TakeBackItem p)
        {
            MirItemCell fromCell = StorageDialog.Grid[p.From];

            MirItemCell toCell = p.To < 40 ? InventoryDialog.Grid[p.To] : BeltDialog.Grid[p.To - 40];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
        private void StoreItem(S.StoreItem p)
        {
            MirItemCell fromCell = p.From < 40 ? InventoryDialog.Grid[p.From] : BeltDialog.Grid[p.From - 40];

            MirItemCell toCell = StorageDialog.Grid[p.To];

            if (toCell == null || fromCell == null) return;

            toCell.Locked = false;
            fromCell.Locked = false;

            if (!p.Success) return;
            toCell.Item = fromCell.Item;
            fromCell.Item = null;
            User.RefreshStats();
        }
        private void SplitItem(S.SplitItem p)
        {
            Bind(p.Item);

            UserItem[] array;
            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    array = MapObject.User.Inventory;
                    break;
                case MirGridType.Storage:
                    array = Storage;
                    break;
                default:
                    return;
            }

            if (p.Grid == MirGridType.Inventory && (p.Item.Info.Type == ItemType.Potion || p.Item.Info.Type == ItemType.Scroll))
            {
                for (int i = 40; i < array.Length; i++)
                {
                    if (array[i] != null) continue;
                    array[i] = p.Item;
                    User.RefreshStats();
                    return;
                }
            }

            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != null) continue;
                array[i] = p.Item;
                User.RefreshStats();
                return;
            }
        }
        private void SplitItem1(S.SplitItem1 p)
        {
            MirItemCell cell;

            switch (p.Grid)
            {
                case MirGridType.Inventory:
                    cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);
                    break;
                case MirGridType.Storage:
                    cell = StorageDialog.GetCell(p.UniqueID);
                    break;
                default:
                    return;
            }

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;
            cell.Item.Count -= p.Count;
            User.RefreshStats();
        }
        private void UseItem(S.UseItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;
            if (cell.Item.Count > 1) cell.Item.Count--;
            else cell.Item = null;
            User.RefreshStats();
        }
        private void DropItem(S.DropItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;

            if (p.Count == cell.Item.Count)
                cell.Item = null;
            else
                cell.Item.Count -= p.Count;

            User.RefreshStats();
        }
        private void PlayerUpdate(S.PlayerUpdate p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                if (MapControl.Objects[i].ObjectID != p.ObjectID) continue;

                PlayerObject player = MapControl.Objects[i] as PlayerObject;
                if (player != null) player.Update(p);
                return;
            }
        }
        private void PlayerInspect(S.PlayerInspect p)
        {
            InspectDialog.Items = p.Equipment;

            InspectDialog.Name = p.Name;
            InspectDialog.Class = p.Class;
            InspectDialog.Gender = p.Gender;
            InspectDialog.Hair = p.Hair;

            InspectDialog.RefreshInferface();
            InspectDialog.Show();
        }
        private void LogOutSuccess(S.LogOutSuccess p)
        {
            User = null;
            if (Settings.HighResolution)
                CMain.SetResolution(800, 600);
            ActiveScene = new SelectScene(p.Characters);

            Dispose();
        }
        private void TimeOfDay(S.TimeOfDay p)
        {
            Lights = p.Lights;
            switch (Lights)
            {
                case LightSetting.Day:
                case LightSetting.Normal:
                    MiniMapDialog.LightSetting.Index = 2093;
                    break;
                case LightSetting.Dawn:
                    MiniMapDialog.LightSetting.Index = 2095;
                    break;
                case LightSetting.Evening:
                    MiniMapDialog.LightSetting.Index = 2094;
                    break;
                case LightSetting.Night:
                    MiniMapDialog.LightSetting.Index = 2092;
                    break;
            }
        }
        private void ChangeAMode(S.ChangeAMode p)
        {
            AMode = p.Mode;

            switch (p.Mode)
            {
                case AttackMode.Peace:
                    ChatDialog.ReceiveChat("[Attack Mode: Peaceful]", ChatType.Hint);
                    break;
                case AttackMode.Group:
                    ChatDialog.ReceiveChat("[Attack Mode: Group]", ChatType.Hint);
                    break;
                case AttackMode.Guild:
                    ChatDialog.ReceiveChat("[Attack Mode: Guild]", ChatType.Hint);
                    break;
                case AttackMode.RedBrown:
                    ChatDialog.ReceiveChat("[Attack Mode: Red+Brown]", ChatType.Hint);
                    break;
                case AttackMode.All:
                    ChatDialog.ReceiveChat("[Attack Mode: All]", ChatType.Hint);
                    break;
            }
        }
        private void ChangePMode(S.ChangePMode p)
        {
            
            PMode = p.Mode;
            switch (p.Mode)
            {
                case PetMode.Both:
                    ChatDialog.ReceiveChat("[Pet Mode: Attack and Move]", ChatType.Hint);
                    break;
                case PetMode.MoveOnly:
                    ChatDialog.ReceiveChat("[Pet Mode: Do Not Attack]", ChatType.Hint);
                    break;
                case PetMode.AttackOnly:
                    ChatDialog.ReceiveChat("[Pet Mode: Do Not Move]", ChatType.Hint);
                    break;
                case PetMode.None:
                    ChatDialog.ReceiveChat("[Pet Mode: Do Not Attack or Move]", ChatType.Hint);
                    break;
            }

            MainDialog.PModeLabel.Visible = true;
        }
        private void ObjectItem(S.ObjectItem p)
        {
            ItemObject ob = new ItemObject(p.ObjectID);
            ob.Load(p);
        }
        private void ObjectGold(S.ObjectGold p)
        {
            ItemObject ob = new ItemObject(p.ObjectID);
            ob.Load(p);
        }
        private void GainedItem(S.GainedItem p)
        {
            Bind(p.Item);
            AddItem(p.Item);
            User.RefreshStats();

            OutputMessage(string.Format("You gained {0}.", p.Item.Name));
        }
        private void GainedGold(S.GainedGold p)
        {
            Gold += p.Gold;
            SoundManager.PlaySound(SoundList.Gold);
            OutputMessage(string.Format("You gained {0:###,###,###} Gold.", p.Gold));
        }
        private void LoseGold(S.LoseGold p)
        {
            Gold -= p.Gold;
            SoundManager.PlaySound(SoundList.Gold);
        }
        private void ObjectMonster(S.ObjectMonster p)
        {
            MonsterObject mob;
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID == p.ObjectID)
                {
                    mob = (MonsterObject)ob;
                    mob.Load(p, true);
                    return;
                }
            }
            mob = new MonsterObject(p.ObjectID);
            mob.Load(p);
        }
        private void ObjectAttack(S.ObjectAttack p)
        {
            if (p.ObjectID == User.ObjectID) return;
            QueuedAction action = null;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (ob.Race == ObjectType.Player)
                {
                    action = new QueuedAction { Action = MirAction.Attack1, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                }
                else
                {
                    switch(p.Type)
                    {
                        case 1: 
                            {
                                action = new QueuedAction { Action = MirAction.Attack2, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        case 2:
                            {
                                action = new QueuedAction { Action = MirAction.Attack3, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        default:
                            {
                                action = new QueuedAction { Action = MirAction.Attack1, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                    }
                }
                action.Params.Add(p.Spell);
                action.Params.Add(p.Level);
                ob.ActionFeed.Add(action);
                return;
            }
        }
        private void Struck(S.Struck p)
        {
            NextRunTime = CMain.Time + 2500;
            User.BlizzardFreezeTime = 0;
            User.ClearMagic();

            if (User.CurrentAction == MirAction.Struck) return; 
            
            MirDirection dir = User.Direction;
            Point location = User.CurrentLocation;

            for (int i = 0; i < User.ActionFeed.Count; i++)
                if (User.ActionFeed[i].Action == MirAction.Struck) return;
            

            if (User.ActionFeed.Count > 0)
            {
                dir = User.ActionFeed[User.ActionFeed.Count - 1].Direction;
                location = User.ActionFeed[User.ActionFeed.Count - 1].Location;
            }

            QueuedAction action = new QueuedAction { Action = MirAction.Struck, Direction = dir, Location = location, Params = new List<object>() };
            action.Params.Add(p.AttackerID);
            User.ActionFeed.Add(action);

        }
        private void ObjectStruck(S.ObjectStruck p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                if (ob.SkipFrames) return;
                if (ob.CurrentAction == MirAction.Struck) return;
                if (ob.ActionFeed.Count > 0 && ob.ActionFeed[ob.ActionFeed.Count - 1].Action == MirAction.Struck) return;

                if (ob.Race == ObjectType.Player) 
                    ((PlayerObject)ob).BlizzardFreezeTime = 0;
                QueuedAction action = new QueuedAction { Action = MirAction.Struck, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                action.Params.Add(p.AttackerID);
                ob.ActionFeed.Add(action);
                return;
            }
        }
        private void DuraChanged(S.DuraChanged p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }


            if (item == null)
                for (int i = 0; i < User.Equipment.Length; i++)
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }

            if (item == null) return;

            item.CurrentDura = p.CurrentDura;

            if (item.CurrentDura == 0)
            {
                User.RefreshStats();
                ChatDialog.ReceiveChat(string.Format("{0}'s dura has dropped to 0.", item.Info.Name), ChatType.System);
            }

            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }
        private void HealthChanged(S.HealthChanged p)
        {
            User.HP = p.HP;
            User.MP = p.MP;
        }
        private void DeleteItem(S.DeleteItem p)
        {
            for (int i = 0; i < User.Inventory.Length; i++)
            {
                UserItem item = User.Inventory[i];
                if (item == null || item.UniqueID != p.UniqueID) continue;

                if (item.Count == p.Count)
                    User.Inventory[i] = null;
                else 
                    item.Count -= p.Count;
                break;
            }

            for (int i = 0; i < User.Equipment.Length; i++)
            {
                UserItem item = User.Equipment[i];
                if (item == null || item.UniqueID != p.UniqueID) continue;

                if (item.Count == p.Count)
                    User.Equipment[i] = null;
                else
                    item.Count -= p.Count;
                break;
            }

            User.RefreshStats();
        }
        private void Death(S.Death p)
        {
            User.Dead = true;

            User.ActionFeed.Add(new QueuedAction {Action = MirAction.Die, Direction = p.Direction, Location = p.Location});
            ShowReviveMessage = true;
        }
        private void ObjectDied(S.ObjectDied p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                if (p.Type == 0)
                {
                    ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Die, Direction = p.Direction, Location = p.Location });
                    ob.Dead = true;
                }
                else
                {
                    MapControl.Effects.Add(new Effect(Libraries.Magic2, 690, 10, 1000, ob.CurrentLocation));
                    ob.Remove();
                }
                return;
            }
        }
        private void ColourChanged(S.ColourChanged p)
        {
            User.NameColour = p.NameColour;
        }
        private void ObjectColourChanged(S.ObjectColourChanged p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.NameColour = p.NameColour;
                return;
            }
        }
        private void GainExperience(S.GainExperience p)
        {
            OutputMessage(string.Format("Experience Gained {0}.", p.Amount));
            MapObject.User.Experience += p.Amount;
        }
        private void LevelChanged(S.LevelChanged p)
        {
            User.Level = p.Level;
            User.Experience = p.Experience;
            User.MaxExperience = p.MaxExperience;
            User.RefreshStats();
            OutputMessage("Level Increased!");
            User.Effects.Add(new Effect(Libraries.Magic2, 1200, 20, 2000, User));
            SoundManager.PlaySound(SoundList.LevelUp);
        }
        private void ObjectLeveled(S.ObjectLeveled p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Effects.Add(new Effect(Libraries.Magic2, 1200, 20, 2000, ob));
                SoundManager.PlaySound(SoundList.LevelUp);
                return;
            }
        }
        private void ObjectHarvest(S.ObjectHarvest p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Harvest, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void ObjectHarvested(S.ObjectHarvested p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Skeleton, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void ObjectNPC(S.ObjectNPC p)
        {
            NPCObject ob = new NPCObject(p.ObjectID);
            ob.Load(p);
        }
        private void NPCResponse(S.NPCResponse p)
        {
            NPCTime = 0;
            NPCDialog.NewText(p.Page);

            if (p.Page.Count > 0)
                NPCDialog.Show();
            else
                NPCDialog.Hide();

            NPCGoodsDialog.Hide();
            // BuyBackDialog.Hide();
            //   NPCDropDialog.Hide();
            StorageDialog.Hide();
        }
        private void ObjectHide(S.ObjectHide p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Hide, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void ObjectShow(S.ObjectShow p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Show, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void Poisoned(S.Poisoned p)
        {
            User.Poison = p.Poison;
            switch (p.Poison)
            {
                case PoisonType.Frozen:
                case PoisonType.Paralysis:
                case PoisonType.Stun:
                    User.ClearMagic();
                    break;
            }
        }
        private void ObjectPoisoned(S.ObjectPoisoned p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Poison = p.Poison;
                return;
            }
        }
        private void MapChanged(S.MapChanged p)
        {
            MapControl.FileName = Path.Combine(Settings.MapPath, p.FileName + ".map");
            MapControl.Title = p.Title;
            MapControl.MiniMap = p.MiniMap;
            MapControl.BigMap = p.BigMap;
            MapControl.Lights = p.Lights;
            MapControl.LoadMap();

            MapControl.NextAction = 0;

            User.CurrentLocation = p.Location;
            User.MapLocation = p.Location;
            MapControl.AddObject(User);

            User.Direction = p.Direction;

            User.QueuedAction = null;
            User.ActionFeed.Clear(); 
            User.ClearMagic();
            User.SetAction();

            MapControl.FloorValid = false;
            MapControl.InputDelay = CMain.Time + 400;
        }
        private void ObjectTeleportOut(S.ObjectTeleportOut p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                Effect effect = null;
                switch (p.Type)
                {
                    case 1: //Yimoogi
                        {
                            effect = new Effect(Libraries.Magic2, 1300, 10, 500, ob);
                            break;
                        }
                    case 2: //RedFoxman
                        {
                            effect = new Effect(Libraries.Monsters[(ushort)Monster.RedFoxman], 243, 10, 500, ob);
                            break;
                        }
                    default:
                        {
                            effect = new Effect(Libraries.Magic, 250, 10, 500, ob);
                            break;
                        }
                }

                if (effect != null)
                {
                    effect.Complete += (o, e) => ob.Remove();
                    ob.Effects.Add(effect);
                }

                SoundManager.PlaySound(SoundList.Teleport);
                return;
            }
        }
        private void ObjectTeleportIn(S.ObjectTeleportIn p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                switch (p.Type)
                {
                    case 1: //Yimoogi
                        {
                            ob.Effects.Add(new Effect(Libraries.Magic2, 1310, 10, 500, ob));
                            break;
                        }
                    case 2: //RedFoxman
                        {
                            ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.RedFoxman], 253, 10, 500, ob));
                            break;
                        }
                    default:
                        {
                            ob.Effects.Add(new Effect(Libraries.Magic, 260, 10, 500, ob));
                            break;
                        }
                }
                SoundManager.PlaySound(SoundList.Teleport);
                return;
            }
        }
        private void TeleportIn()
        {
            User.Effects.Add(new Effect(Libraries.Magic, 260, 10, 500, User));
            SoundManager.PlaySound(SoundList.Teleport);
        }
        private void NPCGoods(S.NPCGoods p)
        {
            NPCRate = p.Rate;
            if (!NPCDialog.Visible) return;
            NPCGoodsDialog.NewGoods(p.List);
            NPCGoodsDialog.Show();
        }
        private void NPCSell()
        {
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Sell;
            NPCDropDialog.Show();
        }
        private void NPCRepair(S.NPCRepair p)
        {
            NPCRate = p.Rate;
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Repair;
            NPCDropDialog.Show();
        }
        private void NPCStorage()
        {
            if (NPCDialog.Visible)
                StorageDialog.Show();
        }
        private void NPCSRepair(S.NPCSRepair p)
        {
            NPCRate = p.Rate;
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.SpecialRepair;
            NPCDropDialog.Show();
        }
        private void SellItem(S.SellItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;

            if (p.Count == cell.Item.Count)
                cell.Item = null;
            else
                cell.Item.Count -= p.Count;

            User.RefreshStats();
        }
        private void RepairItem(S.RepairItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;
        }
        private void ItemRepaired(S.ItemRepaired p)
        {
            UserItem item = null;
            for (int i = 0; i < User.Inventory.Length; i++)
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.UniqueID)
                {
                    item = User.Inventory[i];
                    break;
                }


            if (item == null)
                for (int i = 0; i < User.Equipment.Length; i++)
                    if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.UniqueID)
                    {
                        item = User.Equipment[i];
                        break;
                    }

            if (item == null) return;

            item.MaxDura = p.MaxDura;
            item.CurrentDura = p.CurrentDura;
            
            if (HoverItem == item)
            {
                DisposeItemLabel();
                CreateItemLabel(item);
            }
        }
        private void NewMagic(S.NewMagic p)
        {
            User.Magics.Add(p.Magic);
            User.RefreshStats();
        }

        private void RemoveMagic(S.RemoveMagic p)
        {
            User.Magics.RemoveAt(p.PlaceId);
            User.RefreshStats();
        }

        private void MagicLeveled(S.MagicLeveled p)
        {
            for (int i = 0; i < User.Magics.Count; i++)
            {
                ClientMagic magic = User.Magics[i];
                if (magic.Spell != p.Spell) continue;

                if (magic.Level != p.Level)
                {
                    magic.Level = p.Level;
                    User.RefreshStats();
                }

                magic.Experience = p.Experience;
                break;
            }


        }
        private void Magic(S.Magic p)
        {
            User.Spell = p.Spell;
            User.Cast = p.Cast;
            User.TargetID = p.TargetID;
            User.TargetPoint = p.Target;
            User.SpellLevel = p.Level;

            if (!p.Cast) return;

            switch (p.Spell)
            {
                case Spell.PoisonField:
                    PoisonFieldTime = CMain.Time + (18 - p.Level * 2) * 1000;
                    break;
            }
        }
        private void ObjectMagic(S.ObjectMagic p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;

                QueuedAction action = new QueuedAction { Action = MirAction.Spell, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                action.Params.Add(p.Spell);
                action.Params.Add(p.TargetID);
                action.Params.Add(p.Target);
                action.Params.Add(p.Cast);
                action.Params.Add(p.Level);
                

                ob.ActionFeed.Add(action);
                return;
            }

        }
        private void ObjectEffect(S.ObjectEffect p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                PlayerObject player;
                switch (p.Effect)
                {
                    case SpellEffect.FatalSword:
                        ob.Effects.Add(new Effect(Libraries.Magic2, 1940, 4, 400, ob));
                        SoundManager.PlaySound(20000 + (ushort)Spell.FatalSword * 10);
                        break;
                    case SpellEffect.Teleport:
                        ob.Effects.Add(new Effect(Libraries.Magic, 1600, 10, 600, ob));
                        SoundManager.PlaySound(SoundList.Teleport);
                        break;
                    case SpellEffect.Healing:
                        SoundManager.PlaySound(20000 + (ushort)Spell.Healing * 10 + 1);
                        ob.Effects.Add(new Effect(Libraries.Magic, 370, 10, 800, ob));
                        break;
                    case SpellEffect.RedMoonEvil:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.RedMoonEvil], 32, 6, 400, ob) { Blend = false });
                        break;
                    case SpellEffect.TwinDrakeBlade:
                        ob.Effects.Add(new Effect(Libraries.Magic2, 380, 6, 800, ob));
                        break;
                    case SpellEffect.MagicShieldUp:
                        if (ob.Race != ObjectType.Player) return;
                        player = (PlayerObject) ob;
                        if (player.ShieldEffect != null)
                        {
                            player.ShieldEffect.Clear();
                            player.ShieldEffect.Remove();
                        }

                        player.MagicShield = true;
                        player.Effects.Add(player.ShieldEffect = new Effect(Libraries.Magic, 3890, 3, 600, ob) { Repeat = true });
                        break;
                    case SpellEffect.MagicShieldDown:
                        if (ob.Race != ObjectType.Player) return;
                        player = (PlayerObject) ob;
                        if (player.ShieldEffect != null)
                        {
                            player.ShieldEffect.Clear();
                            player.ShieldEffect.Remove();
                        }
                        player.ShieldEffect = null;
                        player.MagicShield = false;
                        break;
                    case SpellEffect.GreatFoxSpirit:
                        ob.Effects.Add(new Effect(Libraries.Monsters[(ushort)Monster.GreatFoxSpirit], 375 + (CMain.Random.Next(3) * 20), 20, 1400, ob));
                        break;
                    case SpellEffect.MapLightning:
                        ob.Effects.Add(new Effect(Libraries.Dragon, 400 + (CMain.Random.Next(3) * 10), 5, 600, ob));
                        SoundManager.PlaySound(20000 + (ushort)Spell.ThunderBolt * 10);
                        break;
                    case SpellEffect.MapFire:
                        ob.Effects.Add(new Effect(Libraries.Dragon, 440, 20, 1600, ob) { Blend = false });
                        ob.Effects.Add(new Effect(Libraries.Dragon, 470, 10, 800, ob));
                        SoundManager.PlaySound(20000 + (ushort)Spell.ThunderBolt * 10);
                        break;
                    case SpellEffect.Entrapment:
                        ob.Effects.Add(new Effect(Libraries.Magic2, 1010, 10, 1500, ob));
                        ob.Effects.Add(new Effect(Libraries.Magic2, 1020, 8, 1200, ob));
                        break;
                }
                return;
            }

        }
        private void Pushed(S.Pushed p)
        {
            User.ActionFeed.Add(new QueuedAction { Action = MirAction.Pushed, Direction = p.Direction, Location = p.Location });
        }
        private void ObjectPushed(S.ObjectPushed p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Pushed, Direction = p.Direction, Location = p.Location });

                return;
            }
        }
        private void ObjectName(S.ObjectName p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Name = p.Name;
                return;
            }
        }
        private void UserStorage(S.UserStorage p)
        {
            Storage = p.Storage;

            for (int i = 0; i < Storage.Length; i++)
            {
                if (Storage[i] == null) continue;
                Bind(Storage[i]);
            }
        }
        private void SwitchGroup(S.SwitchGroup p)
        {
            GroupDialog.AllowGroup = p.AllowGroup;
        }
        private void DeleteGroup()
        {
            GroupDialog.GroupList.Clear();
            ChatDialog.ReceiveChat("You have left the group.", ChatType.Group);
        }
        private void DeleteMember(S.DeleteMember p)
        {
            GroupDialog.GroupList.Remove(p.Name);
            ChatDialog.ReceiveChat(string.Format("-{0} has left the group.", p.Name), ChatType.Group);
        }
        private void GroupInvite(S.GroupInvite p)
        {
            MirMessageBox messageBox = new MirMessageBox(string.Format("Do you want to group with {0}?", p.Name), MirMessageBoxButtons.YesNo);

            messageBox.YesButton.Click += (o, e) => Network.Enqueue(new C.GroupInvite { AcceptInvite = true });
            messageBox.NoButton.Click += (o, e) => Network.Enqueue(new C.GroupInvite { AcceptInvite = false });

            messageBox.Show();
        }
        private void AddMember(S.AddMember p)
        {
            GroupDialog.GroupList.Add(p.Name);
            ChatDialog.ReceiveChat(string.Format("-{0} has joined the group.", p.Name), ChatType.Group);
        }
        private void Revived()
        {
            User.SetAction();
            User.Dead = false;
            User.Effects.Add(new Effect(Libraries.Magic2, 1220, 20, 2000, User));
            SoundManager.PlaySound(SoundList.Revive);
        }
        private void ObjectRevived(S.ObjectRevived p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (p.Effect)
                {
                    ob.Effects.Add(new Effect(Libraries.Magic2, 1220, 20, 2000, ob));
                    SoundManager.PlaySound(SoundList.Revive);
                }
                ob.Dead = false;
                ob.ActionFeed.Clear();
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.Revive, Direction = ob.Direction, Location = ob.CurrentLocation });
                return;
            }
        }
        private void SpellToggle(S.SpellToggle p)
        {
            switch (p.Spell)
            {
                case Spell.Slaying:
                    Slaying = p.CanUse;
                    break;
                case Spell.Thrusting:
                    Thrusting = p.CanUse;
                    ChatDialog.ReceiveChat(Thrusting ? "Use Thrusting." : "Do not use Thrusting.", ChatType.Hint);
                    break;
                case Spell.HalfMoon:
                    HalfMoon = p.CanUse;
                    ChatDialog.ReceiveChat(HalfMoon ? "Use HalfMoon." : "Do not use HalfMoon.", ChatType.Hint);
                    break;
                case Spell.CrossHalfMoon:
                    CrossHalfMoon = p.CanUse;
                    ChatDialog.ReceiveChat(CrossHalfMoon ? "Use CrossHalfMoon." : "Do not use CrossHalfMoon.", ChatType.Hint);
                    break;
                case Spell.DoubleSlash:
                    DoubleSlash = p.CanUse;
                    ChatDialog.ReceiveChat(DoubleSlash ? "Use DoubleSlash." : "Do not use DoubleSlash.", ChatType.Hint);
                    break;
                case Spell.FlamingSword:
                    FlamingSword = p.CanUse;
                    if (FlamingSword) 
                        ChatDialog.ReceiveChat("Your weapon is glowed by spirit of fire.", ChatType.Hint);
                    else
                        ChatDialog.ReceiveChat("The spirits of fire disappeared.", ChatType.System);
                    break;
            }
        }
        private void ObjectHealth(S.ObjectHealth p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.PercentHealth = p.Percent;
                ob.HealthTime = CMain.Time + p.Expire * 1000;
                return;
            }
        }
        private void MapEffect(S.MapEffect p)
        {

        }
        private void ObjectRangeAttack(S.ObjectRangeAttack p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                QueuedAction action = null;
                if (ob.Race == ObjectType.Player)
                {
                    action = new QueuedAction { Action = MirAction.AttackRange, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                }
                else
                {
                    switch (p.Type)
                    {
                        case 1:
                            {
                                action = new QueuedAction { Action = MirAction.AttackRange2, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                        default:
                            {
                                action = new QueuedAction { Action = MirAction.AttackRange, Direction = p.Direction, Location = p.Location, Params = new List<object>() };
                                break;
                            }
                    }
                }
                action.Params.Add(p.TargetID);
                ob.ActionFeed.Add(action);
                return;
            }
        }
        private void AddBuff(S.AddBuff p)
        {
            Buff buff = new Buff {Type = p.Type, Caster = p.Caster, Expire = CMain.Time + p.Expire, Value = p.Value, Infinite = p.Infinite};
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Type != buff.Type) continue;

                Buffs[i] = buff;
                User.RefreshStats();
                return;
            }
            Buffs.Add(buff);
            CreateBuff(buff);
            User.RefreshStats();
        }
        private void RemoveBuff(S.RemoveBuff p)
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].Type != p.Type) continue;

                Buffs.RemoveAt(i);
                BuffList[i].Dispose();
                BuffList.RemoveAt(i);
            }
            User.RefreshStats();
        }
        private void ObjectHidden(S.ObjectHidden p)
        {
            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                ob.Hidden = p.Hidden;
                return;
            }
        }
        private void RefreshItem(S.RefreshItem p)
        {
            Bind(p.Item);

            if (SelectedCell != null && SelectedCell.Item.UniqueID == p.Item.UniqueID)
                SelectedCell = null;

            if (HoverItem != null && HoverItem.UniqueID == p.Item.UniqueID)
            {
                DisposeItemLabel();
                CreateItemLabel(p.Item);
            }

            for (int i = 0; i < User.Inventory.Length; i++)
                if (User.Inventory[i] != null && User.Inventory[i].UniqueID == p.Item.UniqueID)
                {
                    User.Inventory[i] = p.Item;
                    User.RefreshStats();
                    return;
                }

            for (int i = 0; i < User.Equipment.Length; i++)
                if (User.Equipment[i] != null && User.Equipment[i].UniqueID == p.Item.UniqueID)
                {
                    User.Equipment[i] = p.Item;
                    User.RefreshStats();
                    return;
                }


        }
        private void ObjectSpell(S.ObjectSpell p)
        {
            SpellObject ob = new SpellObject(p.ObjectID);
            ob.Load(p);
        }
        private void UserDash(S.UserDash p)
        {
            if (User.Direction == p.Direction && User.CurrentLocation == p.Location)
            {
                MapControl.NextAction = 0; 
                return;
            }
            MirAction action = User.CurrentAction == MirAction.DashL ? MirAction.DashR :  MirAction.DashL;
            for (int i = User.ActionFeed.Count - 1; i >= 0; i--)
            {
                if (User.ActionFeed[i].Action == MirAction.DashR)
                {
                    action = MirAction.DashL;
                    break;
                }
                if (User.ActionFeed[i].Action == MirAction.DashL)
                {
                    action = MirAction.DashR;
                    break;
                }
            }

            User.ActionFeed.Add(new QueuedAction { Action = action, Direction = p.Direction, Location = p.Location });

        }
        private void UserDashFail(S.UserDashFail p)
        {
            MapControl.NextAction = 0;
            User.ActionFeed.Add(new QueuedAction { Action = MirAction.DashFail, Direction = p.Direction, Location = p.Location });
        }
        private void ObjectDash(S.ObjectDash p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                
                MirAction action = MirAction.DashL;

                if (ob.ActionFeed.Count > 0 && ob.ActionFeed[ob.ActionFeed.Count - 1].Action == action)
                    action = MirAction.DashR;

                ob.ActionFeed.Add(new QueuedAction { Action = action, Direction = p.Direction, Location = p.Location });

                return;
            }
        }
        private void ObjectDashFail(S.ObjectDashFail p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.DashFail, Direction = p.Direction, Location = p.Location });

                return;
            }
        }
        private void NPCConsign()
        {
            if (!NPCDialog.Visible) return;
            NPCDropDialog.PType = PanelType.Consign;
            NPCDropDialog.Show();
        }
        private void NPCMarket(S.NPCMarket p)
        {
            for (int i = 0; i < p.Listings.Count; i++)
                Bind(p.Listings[i].Item);

            TrustMerchantDialog.Show();
            TrustMerchantDialog.UserMode = p.UserMode;
            TrustMerchantDialog.Listings = p.Listings;
            TrustMerchantDialog.Page = 0;
            TrustMerchantDialog.PageCount = p.Pages;
            TrustMerchantDialog.UpdateInterface();
        }
        private void NPCMarketPage(S.NPCMarketPage p)
        {
            if (!TrustMerchantDialog.Visible) return;

            for (int i = 0; i < p.Listings.Count; i++)
                Bind(p.Listings[i].Item);

            TrustMerchantDialog.Listings.AddRange(p.Listings);
            TrustMerchantDialog.Page = (TrustMerchantDialog.Listings.Count - 1)/10;
            TrustMerchantDialog.UpdateInterface();
        }
        private void ConsignItem(S.ConsignItem p)
        {
            MirItemCell cell = InventoryDialog.GetCell(p.UniqueID) ?? BeltDialog.GetCell(p.UniqueID);

            if (cell == null) return;

            cell.Locked = false;

            if (!p.Success) return;
            
            cell.Item = null;

            User.RefreshStats();
        }
        private void MarketFail(S.MarketFail p)
        {
            TrustMerchantDialog.MarketTime = 0;
            switch (p.Reason)
            {
                case 0:
                    MirMessageBox.Show("You cannot use the TrustMerchant when dead.");
                    break;
                case 1:
                    MirMessageBox.Show("You cannot buy from the TrustMerchant without using.");
                    break;
                case 2:
                    MirMessageBox.Show("This item has already been sold.");
                    break;
                case 3:
                    MirMessageBox.Show("This item has Expired and cannot be brought.");
                    break;
                case 4:
                    MirMessageBox.Show("You do not have enough gold to buy this item.");
                    break;
                case 5:
                    MirMessageBox.Show("You do not have enough weight or space spare to buy this item.");
                    break;
                case 6:
                    MirMessageBox.Show("You cannot buy your own items.");
                    break;
                case 7:
                    MirMessageBox.Show("You are too far away from the Trust Merchant.");
                    break;
                case 8:
                    MirMessageBox.Show("You cannot hold enough gold to get your sale");
                    break;
            }

        }
        private void MarketSuccess(S.MarketSuccess p)
        {
            TrustMerchantDialog.MarketTime = 0;
            MirMessageBox.Show(p.Message);
        }
        private void ObjectSitDown(S.ObjectSitDown p)
        {
            if (p.ObjectID == User.ObjectID) return;

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];
                if (ob.ObjectID != p.ObjectID) continue;
                if (ob.Race != ObjectType.Monster) continue;
                ob.SitDown = p.Sitting;
                ob.ActionFeed.Add(new QueuedAction { Action = MirAction.SitDown, Direction = p.Direction, Location = p.Location });
                return;
            }
        }


        public void AddItem(UserItem item)
        {
            Redraw();

            if (item.Info.StackSize > 1) //Stackable
            {
                for (int i = 0; i < User.Inventory.Length; i++)
                {
                    UserItem temp = User.Inventory[i];
                    if (temp == null || item.Info != temp.Info || temp.Count >= temp.Info.StackSize) continue;

                    if (item.Count + temp.Count <= temp.Info.StackSize)
                    {
                        temp.Count += item.Count;
                        return;
                    }
                    item.Count -= temp.Info.StackSize - temp.Count;
                    temp.Count = temp.Info.StackSize;
                }
            }

            if (item.Info.Type == ItemType.Potion || item.Info.Type == ItemType.Scroll)
            {
                for (int i = 40; i < User.Inventory.Length; i++)
                {
                    if (User.Inventory[i] != null) continue;
                    User.Inventory[i] = item;
                    return;
                }
            }

            for (int i = 0; i < User.Inventory.Length; i++)
            {
                if (User.Inventory[i] != null) continue;
                User.Inventory[i] = item;
                return;
            }
        }
        public static void Bind(UserItem item)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                if (ItemInfoList[i].Index != item.ItemIndex) continue;

                item.Info = ItemInfoList[i];
                return;
            }
        }

        public void DisposeItemLabel()
        {
            if (ItemLabel != null && !ItemLabel.IsDisposed)
                ItemLabel.Dispose();
            ItemLabel = null;
        }

        public void CreateItemLabel(UserItem item)
        {
            if (item == null)
            {
                DisposeItemLabel();
                HoverItem = null;
                return;
            }

            if (item == HoverItem && ItemLabel != null && !ItemLabel.IsDisposed) return;

            HoverItem = item;

            ItemLabel = new MirControl
                {
                    BackColour = Color.FromArgb(255, 0, 24, 48),
                    Border = true,
                    BorderColour = Color.FromArgb(144, 148, 48),
                    DrawControlTexture = true,
                    NotControl = true,
                    Parent = this,
                    Opacity = 0.7F,
                    Visible = false
                };


            MirLabel nameLabel = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.Yellow,
                    Location = new Point(4, 4),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = HoverItem.Info.Name
                };
            ItemLabel.Size = new Size(nameLabel.DisplayRectangle.Right + 4, nameLabel.DisplayRectangle.Bottom);

            if (HoverItem.Weight > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(ItemLabel.DisplayRectangle.Right, 4),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format("W: {0}", HoverItem.Weight)
                    };
                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4, ItemLabel.Size.Height);
            }

            if (HoverItem.Info.StackSize > 1)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(ItemLabel.DisplayRectangle.Right, 4),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format("Count: {0}/{1}", HoverItem.Count, HoverItem.Info.StackSize)
                    };
                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4, ItemLabel.Size.Height);
            }

            switch (HoverItem.Info.Type)
            {
                case ItemType.Potion:
                case ItemType.Scroll:
                    PotionItemInfo();
                    break;
                default:
                    EquipmentItemInfo();
                    break;
            }



            if (HoverItem.Info.RequiredGender != RequiredGender.None)
            {
                Color colour = Color.White;
                switch (MapObject.User.Gender)
                {
                    case MirGender.Male:
                        if (!HoverItem.Info.RequiredGender.HasFlag(RequiredGender.Male))
                            colour = Color.Red;
                        break;
                    case MirGender.Female:
                        if (!HoverItem.Info.RequiredGender.HasFlag(RequiredGender.Female))
                            colour = Color.Red;
                        break;
                }

                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = colour,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format("Gender Required: {0}", HoverItem.Info.RequiredGender),
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }
            if (HoverItem.Info.RequiredClass != RequiredClass.None)
            {
                Color colour = Color.White;
                switch (MapObject.User.Class)
                {
                    case MirClass.Warrior:
                        if (!HoverItem.Info.RequiredClass.HasFlag(RequiredClass.Warrior))
                            colour = Color.Red;
                        break;
                    case MirClass.Wizard:
                        if (!HoverItem.Info.RequiredClass.HasFlag(RequiredClass.Wizard))
                            colour = Color.Red;
                        break;
                    case MirClass.Taoist:
                        if (!HoverItem.Info.RequiredClass.HasFlag(RequiredClass.Taoist))
                            colour = Color.Red;
                        break;
                    case MirClass.Assassin:
                        if (!HoverItem.Info.RequiredClass.HasFlag(RequiredClass.Assassin))
                            colour = Color.Red;
                        break;
                }

                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = colour,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format("Class Required: {0}", HoverItem.Info.RequiredClass),
                    };

                if (HoverItem.Info.RequiredClass == RequiredClass.WarWizTao)
                    label.Text = string.Format("Class Required: {0}, {1}, {2}", RequiredClass.Warrior, RequiredClass.Wizard, RequiredClass.Taoist);

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }


            if (HoverItem.Info.RequiredAmount > 0)
            {
                string text;
                Color colour = Color.White;
                switch (HoverItem.Info.RequiredType)
                {
                    case RequiredType.Level:
                        text = string.Format("Required Level: {0}", HoverItem.Info.RequiredAmount);
                        if (MapObject.User.Level < HoverItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.AC:
                        text = string.Format("Required AC: {0}", HoverItem.Info.RequiredAmount);
                        if (MapObject.User.MaxAC < HoverItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MAC:
                        text = string.Format("Required MAC: {0}", HoverItem.Info.RequiredAmount);
                        if (MapObject.User.MaxMAC < HoverItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.DC:
                        text = string.Format("Required DC: {0}", HoverItem.Info.RequiredAmount);
                        if (MapObject.User.MaxDC < HoverItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.MC:
                        text = string.Format("Required MC: {0}", HoverItem.Info.RequiredAmount);
                        if (MapObject.User.MaxMC < HoverItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    case RequiredType.SC:
                        text = string.Format("Required SC: {0}", HoverItem.Info.RequiredAmount);
                        if (MapObject.User.MaxSC < HoverItem.Info.RequiredAmount)
                            colour = Color.Red;
                        break;
                    default:
                        text = "Unknown Type Required";
                        break;
                }
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = colour,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = text
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            ItemLabel.Size = ItemLabel.Size.Add(0, 4);

            ItemLabel.Visible = true;
        }

        private void PotionItemInfo()
        {
            if (HoverItem.Info.Durability > 0)
            {
                MirLabel label = new MirLabel
                {
                    AutoSize = true,
                    ForeColour = Color.White,
                    Location = new Point(ItemLabel.DisplayRectangle.Right, 4),
                    OutLine = false,
                    Parent = ItemLabel,
                    Text = string.Format("Range: {0}", HoverItem.Info.Durability)
                };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4, ItemLabel.Size.Height);
            }
            int value = HoverItem.Info.HP;
            int addedValue = HoverItem.HP;

            //  int x = 4;
            //   int y = ItemLabel.DisplayRectangle.Bottom;

            if (value > 0 || addedValue > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue > 0 ? "Health Restoration: +{0}HP (+{1})" : "Health Restoration: +{0}HP", value + addedValue, addedValue)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value = HoverItem.Info.MP;
            addedValue = HoverItem.MP;


            if (value > 0 || addedValue > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue > 0 ? "Mana Restoration: +{0}MP (+{1})" : "Mana Restoration: +{0}MP", value + addedValue, addedValue)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

        }

        private void EquipmentItemInfo()
        {
            if (HoverItem.Info.Durability > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = HoverItem.CurrentDura == 0 ? Color.Red : Color.White,
                        Location = new Point(ItemLabel.DisplayRectangle.Right, 4),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = HoverItem.Info.Type == ItemType.Amulet
                                   ? string.Format("Useage: {0}/{1}", HoverItem.CurrentDura, HoverItem.MaxDura)
                                   : string.Format("Durability: {0}/{1}", Math.Round(HoverItem.CurrentDura/1000M),
                                                   Math.Round(HoverItem.MaxDura/1000M)),
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4, ItemLabel.Size.Height);
            }

            int value1 = HoverItem.Info.MinAC;
            int value2 = HoverItem.Info.MaxAC;
            int addedValue1 = HoverItem.AC;


            if (value1 > 0 || addedValue1 > 0 || value2 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "AC: {0}-{1} (+{2})" : "AC: {0}-{1}", value1, value2 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.MinMAC;
            value2 = HoverItem.Info.MaxMAC;
            addedValue1 = HoverItem.MAC;


            if (value1 > 0 || addedValue1 > 0 || value2 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "MAC: {0}-{1} (+{2})" : "MAC: {0}-{1}", value1, value2 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.MinDC;
            value2 = HoverItem.Info.MaxDC;
            addedValue1 = HoverItem.DC;


            if (value1 > 0 || addedValue1 > 0 || value2 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "DC: {0}-{1} (+{2})" : "DC: {0}-{1}", value1, value2 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.MinMC;
            value2 = HoverItem.Info.MaxMC;
            addedValue1 = HoverItem.MC;


            if (value1 > 0 || addedValue1 > 0 || value2 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "MC: {0}-{1} (+{2})" : "MC: {0}-{1}", value1, value2 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.MinSC;
            value2 = HoverItem.Info.MaxSC;
            addedValue1 = HoverItem.SC;


            if (value1 > 0 || addedValue1 > 0 || value2 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "SC: {0}-{1} (+{2})" : "SC: {0}-{1}", value1, value2 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.HP;
            addedValue1 = HoverItem.HP;


            if (value1 > 0 || addedValue1 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "Health: +{0} (+{1})" : "Health: +{0}", value1 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.MP;
            addedValue1 = HoverItem.MP;


            if (value1 > 0 || addedValue1 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "Mana: +{0} (+{1})" : "Mana: +{0}", value1 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.Accuracy;
            addedValue1 = HoverItem.Accuracy;


            if (value1 > 0 || addedValue1 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "Accuracy: +{0} (+{1})" : "Accuracy: +{0}", value1 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.Agility;
            addedValue1 = HoverItem.Agility;


            if (value1 > 0 || addedValue1 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "Agility: +{0} (+{1})" : "Agility: +{0}", value1 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.AttackSpeed;
            addedValue1 = HoverItem.AttackSpeed;


            if (value1 > 0 || addedValue1 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = addedValue1 > 0 ? Color.Cyan : Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(addedValue1 > 0 ? "A.Speed: +{0} (+{1})" : "A.Speed: +{0}", value1 + addedValue1, addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.BagWeight;

            if (value1 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format("Bag Weight: +{0}", value1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.HandWeight;

            if (value1 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format("Hand Weight: +{0}", value1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.WearWeight;

            if (value1 > 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = Color.White,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format("Wear Weight: +{0}", value1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

            value1 = HoverItem.Info.Luck;
            addedValue1 = HoverItem.Luck;


            if (value1 + addedValue1 != 0)
            {
                MirLabel label = new MirLabel
                    {
                        AutoSize = true,
                        ForeColour = value1 + addedValue1 > 0 ? Color.Yellow : Color.Red,
                        Location = new Point(4, ItemLabel.DisplayRectangle.Bottom),
                        OutLine = false,
                        Parent = ItemLabel,
                        Text = string.Format(value1 + addedValue1 > 0 ? "Luck: +{0} " : "Curse: +{0}", value1 + addedValue1)
                    };

                ItemLabel.Size = new Size(label.DisplayRectangle.Right + 4 > ItemLabel.Size.Width ? label.DisplayRectangle.Right + 4 : ItemLabel.Size.Width,
                                          label.DisplayRectangle.Bottom > ItemLabel.Size.Height ? label.DisplayRectangle.Bottom : ItemLabel.Size.Height);
            }

        }

        public class OutPutMessage
        {
            public string Message;
            public long ExpireTime;
        }

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Scene = null;
                User = null;

                MoveTime = 0;
                AttackTime = 0;
                NextRunTime = 0;
                CanMove = false;
                CanRun = false;

                MapControl = null;
                MainDialog = null;
                ChatDialog = null;
                ChatControl = null;
                InventoryDialog = null;
                CharacterDialog = null;
                StorageDialog = null;
                BeltDialog = null;
                MiniMapDialog = null;
                InspectDialog = null;
                OptionDialog = null;
                MenuDialog = null;
                NPCDialog = null;

                HoverItem = null;
                SelectedCell = null;
                PickedUpGold = false;

                UseItemTime = 0;
                PickUpTime = 0;
                InspectTime = 0;

                DisposeItemLabel();

                AMode = 0;
                PMode = 0;
                Lights = 0;

                NPCTime = 0;
                NPCID = 0;


                for (int i = 0; i < OutputLines.Length; i++)
                    if (OutputLines[i] != null && OutputLines[i].IsDisposed)
                        OutputLines[i].Dispose();

                OutputMessages.Clear();
                OutputMessages = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        public static ItemInfo GetInfo(int index)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                ItemInfo info = ItemInfoList[i];
                if (info.Index != index) continue;
                return info;
            }

            return null;
        }
    }



    public sealed class MapControl : MirControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public static List<MapObject> Objects = new List<MapObject>();

        public const int CellWidth = 48;
        public const int CellHeight = 32;

        public static int OffSetX;
        public static int OffSetY;

        public static int ViewRangeX;
        public static int ViewRangeY;



        public static Point MapLocation
        {
            get { return GameScene.User == null ? Point.Empty : new Point(MouseLocation.X / CellWidth - OffSetX, MouseLocation.Y / CellHeight - OffSetY).Add(GameScene.User.CurrentLocation); }
        }

        public static MouseButtons MapButtons;
        public static Point MouseLocation;
        public static long InputDelay;
        public static long NextAction;

        public CellInfo[,] M2CellInfo;
        public int Width, Height;

        public string FileName = String.Empty;
        public string Title = String.Empty;
        public ushort MiniMap, BigMap;
        public LightSetting Lights;
        public bool Lightning, Fire;
        public long LightningTime, FireTime;

        public bool FloorValid, LightsValid;

        private Texture _floorTexture, _lightTexture;
        private Surface _floorSurface, _lightSurface;

        public long OutputDelay;

        private static bool _autoRun; 
        public static bool AutoRun
        {
            get { return _autoRun; }
            set
            {
                if (_autoRun == value) return;
                _autoRun = value;
                if (GameScene.Scene != null)
                    GameScene.Scene.ChatDialog.ReceiveChat(value ? "[AutoRun: On]" : "[AutoRun: Off]", ChatType.Hint);
            }

        }

        public int AnimationCount;
        public static List<Effect> Effects = new List<Effect>();

        public MapControl()
        {
            MapButtons = MouseButtons.None;

            OffSetX = Settings.ScreenWidth / 2 / CellWidth;
            OffSetY = Settings.ScreenHeight / 2 / CellHeight - 1;

            ViewRangeX = OffSetX + 4;
            ViewRangeY = OffSetY + 4;

            Size = new Size(Settings.ScreenWidth, Settings.ScreenHeight);
            DrawControlTexture = true;
            BackColour = Color.Black;

            MouseDown += OnMouseDown;
            MouseMove += (o, e) => MouseLocation = e.Location;
            Click += OnMouseClick;
        }

        public void LoadMap()
        {
            GameScene.Scene.NPCDialog.Hide();
            Objects.Clear();
            Effects.Clear();

            if (User != null)
                Objects.Add(User);


            MapObject.MouseObject = null;
            MapObject.TargetObject = null;
            MapObject.MagicObject = null;

            try
            {
                if (File.Exists(FileName))
                {
                    int offSet = 21;
                    byte[] fileBytes = File.ReadAllBytes(FileName);
                    int w = BitConverter.ToInt16(fileBytes, offSet);
                    offSet += 2;
                    int xor = BitConverter.ToInt16(fileBytes, offSet);
                    offSet += 2;
                    int h = BitConverter.ToInt16(fileBytes, offSet);
                    Width = w ^ xor;
                    Height = h ^ xor;
                    M2CellInfo = new CellInfo[Width,Height];

                    offSet = 54;

                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                        {
                            M2CellInfo[x, y] = new CellInfo
                                {
                                    BackImage = (int) (BitConverter.ToInt32(fileBytes, offSet) ^ 0xAA38AA38),
                                    MiddleImage = (short) (BitConverter.ToInt16(fileBytes, offSet += 4) ^ xor),
                                    FrontImage = (short) (BitConverter.ToInt16(fileBytes, offSet += 2) ^ xor),
                                    DoorIndex = fileBytes[offSet += 2],
                                    DoorOffset = fileBytes[++offSet],
                                    AnimationFrame = fileBytes[++offSet],
                                    AnimationTick = fileBytes[++offSet],
                                    FileIndex = fileBytes[++offSet],
                                    Light = fileBytes[++offSet],
                                    Unknown = fileBytes[++offSet],
                                };
                            offSet++;
                        }
                }
                else
                {
                    Width = 1000;
                    Height = 1000;
                    M2CellInfo = new CellInfo[Width,Height];

                    for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                        {
                            M2CellInfo[x, y] = new CellInfo();
                        }
                }
            }
            catch (Exception ex)
            {
                if (Settings.LogErrors) CMain.SaveError(ex.ToString());
            }

        }

        public void Process()
        {
            User.Process();

            for (int i = Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = Objects[i];
                if (ob == User) continue;
              //  if (ob.ActionFeed.Count > 0 || ob.Effects.Count > 0 || GameScene.CanMove || CMain.Time >= ob.NextMotion)
                    ob.Process();
            }

            for (int i = Effects.Count - 1; i >= 0; i--)
                Effects[i].Process();

            if (Lightning && CMain.Time > LightningTime)
            {
                LightningTime = CMain.Time + CMain.Random.Next(2000, 5000);
                Point source = new Point(User.CurrentLocation.X + CMain.Random.Next(-7, 7), User.CurrentLocation.Y + CMain.Random.Next(-7, 7));
                MapControl.Effects.Add(new Effect(Libraries.Dragon, 400 + (CMain.Random.Next(3) * 10), 5, 400, source));
            }
            if (Fire && CMain.Time > FireTime)
            {
                FireTime = CMain.Time + CMain.Random.Next(2000, 5000);
                Point source = new Point(User.CurrentLocation.X + CMain.Random.Next(-7, 7), User.CurrentLocation.Y + CMain.Random.Next(-7, 7));
                MapControl.Effects.Add(new Effect(Libraries.Dragon, 440, 20, 1600, source) { Blend = false });
                MapControl.Effects.Add(new Effect(Libraries.Dragon, 470, 10, 800, source));
            }

            CheckInput();

            MapObject bestmouseobject = null;
            for (int y = MapLocation.Y + 2; y >= MapLocation.Y - 2; y--)
            {
                if (y >= Height) continue;
                if (y < 0) break;
                for (int x = MapLocation.X + 2; x >= MapLocation.X - 2; x--)
                {
                    if (x >= Width) continue;
                    if (x < 0) break;
                    CellInfo cell = M2CellInfo[x, y];
                    if (cell.CellObjects == null) continue;

                    for (int i = cell.CellObjects.Count - 1; i >= 0; i--)
                    {
                        MapObject ob = cell.CellObjects[i];
                        if (ob == MapObject.User || !ob.MouseOver(CMain.MPoint)) continue;

                        if (MapObject.MouseObject != ob)
                        {
                            if (ob.Dead)
                            {
                                bestmouseobject = ob;
                                continue;
                            }
                            MapObject.MouseObject = ob;
                            Redraw();
                        }
                        if (bestmouseobject != null && MapObject.MouseObject == null)
                        {
                            MapObject.MouseObject = bestmouseobject;
                            Redraw();
                        }
                        return;
                    }
                }
            }


            if (MapObject.MouseObject != null)
            {
                MapObject.MouseObject = null;
                Redraw();
            }
        }

        public static MapObject GetObject(uint targetID)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob.ObjectID != targetID) continue;
                return ob;
            }
            return null;
        }

        public override void Draw()
        {
            //Do nothing.
        }

        protected override void CreateTexture()
        {
            if (User == null) return;

            if (!FloorValid)
                DrawFloor();

            if (ControlTexture != null && !ControlTexture.Disposed && Size != TextureSize)
                ControlTexture.Dispose();

            if (ControlTexture == null || ControlTexture.Disposed)
            {
                DXManager.ControlList.Add(this);
                ControlTexture = new Texture(DXManager.Device, Size.Width, Size.Height, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                ControlTexture.Disposing += ControlTexture_Disposing;
                TextureSize = Size;
            }

            Surface oldSurface = DXManager.CurrentSurface;
            Surface surface = ControlTexture.GetSurfaceLevel(0);
            DXManager.SetSurface(surface);
            DXManager.Device.Clear(ClearFlags.Target, BackColour, 0, 0);

            if (FloorValid)
                DXManager.Sprite.Draw2D(_floorTexture, Point.Empty, 0F, Point.Empty, Color.White);

            DrawObjects();

            //Render Death, 

            LightSetting setting = Lights == LightSetting.Normal ? GameScene.Scene.Lights : Lights;
            if (setting != LightSetting.Day)
                 DrawLights(setting);

            if (Settings.DropView)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    ItemObject ob = Objects[i] as ItemObject;
                    if (ob == null) continue;

                    if (!ob.MouseOver(MouseLocation))
                        ob.DrawName();
                }
            }

            if (MapObject.MouseObject != null && !(MapObject.MouseObject is ItemObject))
                MapObject.MouseObject.DrawName();

            int offSet = 0;
            for (int i = 0; i < Objects.Count; i++)
            {
                ItemObject ob = Objects[i] as ItemObject;
                if (ob == null) continue;

                if (!ob.MouseOver(MouseLocation)) continue;
                ob.DrawName(offSet);
                offSet -= ob.NameLabel.Size.Height + (ob.NameLabel.Border ? 1 : 0);
            }

            if (MapObject.User.MouseOver(MouseLocation))
                MapObject.User.DrawName();




            DXManager.SetSurface(oldSurface);
            surface.Dispose();
            TextureValid = true;
        }
        protected internal override void DrawControl()
        {
            if (!DrawControlTexture)
                return;

            if (!TextureValid)
                CreateTexture();

            if (ControlTexture == null || ControlTexture.Disposed)
                return;

            float oldOpacity = DXManager.Opacity;

            DXManager.SetOpacity(Opacity);
            DXManager.Sprite.Draw2D(ControlTexture, Point.Empty, 0F, Point.Empty, Color.White);
            DXManager.SetOpacity(oldOpacity);

            CleanTime = CMain.Time + Settings.CleanDelay;
        }

        private void DrawFloor()
        {

            if (_floorTexture == null || _floorTexture.Disposed)
            {
                _floorTexture = new Texture(DXManager.Device, Settings.ScreenWidth, Settings.ScreenHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                _floorTexture.Disposing += FloorTexture_Disposing;
                _floorSurface = _floorTexture.GetSurfaceLevel(0);
            }


            Surface oldSurface = DXManager.CurrentSurface;

            DXManager.SetSurface(_floorSurface);
            DXManager.Device.Clear(ClearFlags.Target, Color.Black, 0, 0);

            int index;
            int drawY, drawX;

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY; y++)
            {
                if (y <= 0 || y%2 == 1) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight + User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x <= 0 || x%2 == 1) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX+ User.OffSetMove.X; //Moving OffSet

                    if (M2CellInfo[x, y].BackImage == 0) continue;
                    index = (M2CellInfo[x, y].BackImage & 0x1FFFF) - 1;
                    Libraries.Tiles.Draw(index, drawX, drawY);
                }
            }

            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight+ User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x <= 0) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX+ User.OffSetMove.X; //Moving OffSet

                    index = M2CellInfo[x, y].MiddleImage - 1;

                    if (index < 0) continue;

                    Libraries.SmallTiles.Draw(index, drawX, drawY);
                }
            }
            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                drawY = (y - User.Movement.Y + OffSetY) * CellHeight+ User.OffSetMove.Y; //Moving OffSet

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x <= 0) continue;
                    if (x >= Width) break;
                    drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX+ User.OffSetMove.X; //Moving OffSet

                    index = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
                    int fileIndex = M2CellInfo[x, y].FileIndex;
                    Size s = Libraries.Objects[fileIndex].GetSize(index);

                    if (index < 0 || s.Width != CellWidth || s.Height != CellHeight) continue;

                    Libraries.Objects[fileIndex].Draw(index, drawX, drawY);
                }
            }

            DXManager.SetSurface(oldSurface);

            FloorValid = true;
        }

        private void DrawObjects()
        {
            for (int y = User.Movement.Y - ViewRangeY; y <= User.Movement.Y + ViewRangeY + 25; y++)
            {
                if (y <= 0) continue;
                if (y >= Height) break;
                int drawY = (y - User.Movement.Y + OffSetY + 1) * CellHeight + User.OffSetMove.Y;

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x <= 0) continue;
                    if (x >= Width) break;
                    int drawX = (x - User.Movement.X + OffSetX) * CellWidth - OffSetX + User.OffSetMove.X;

                    int index = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;

                    if (index < 0) continue;

                    int fileIndex = M2CellInfo[x, y].FileIndex;
                    byte animation = M2CellInfo[x, y].AnimationFrame;


                    bool blend;
                    if ((animation & 0x80) > 0)
                    {
                        blend = true;
                        animation &= 0x7F;
                    }
                    else
                        blend = false;

                    if (animation > 0)
                    {
                        byte animationTick = M2CellInfo[x, y].AnimationTick;
                        index += (AnimationCount%(animation + (animation*animationTick)))/(1 + animationTick);
                    }

                    Size s = Libraries.Objects[fileIndex].GetSize(index);

                    if (s.Width == CellWidth && s.Height == CellHeight && animation == 0) continue;

                    if (blend)
                        Libraries.Objects[fileIndex].DrawBlend(index, new Point(drawX, drawY - s.Height), Color.White, index >= 2723 && index <= 2732);
                    else
                        Libraries.Objects[fileIndex].Draw(index, drawX, drawY - s.Height);
                }

                for (int x = User.Movement.X - ViewRangeX; x <= User.Movement.X + ViewRangeX; x++)
                {
                    if (x <= 0) continue;
                    if (x >= Width) break;
                    M2CellInfo[x, y].DrawObjects();
                }
            }

            DXManager.Sprite.Flush();
            float oldOpacity = DXManager.Opacity;
            DXManager.SetOpacity(0.5F);

            MapObject.User.DrawBody();
            MapObject.User.DrawHead();
            MapObject.User.DrawWings();

            DXManager.SetOpacity(oldOpacity);


            if (MapObject.MouseObject != null && !MapObject.MouseObject.Dead && MapObject.MouseObject != MapObject.TargetObject)
                MapObject.MouseObject.DrawBlend();

            if (MapObject.TargetObject != null)
                MapObject.TargetObject.DrawBlend();

            if (Settings.NameView)
            {
                for (int i = 0; i < Objects.Count; i++)
                {
                    if (Objects[i] is ItemObject || Objects[i].Dead) continue;
                    Objects[i].DrawName();
                }
            }

            for (int i = 0; i < Objects.Count; i++)
            {
                Objects[i].DrawChat();
                if (Settings.Effect) Objects[i].DrawEffects();
                Objects[i].DrawHealth();
            }

            if (!Settings.Effect) return;
            
            for (int i = Effects.Count - 1; i >= 0; i--)
                Effects[i].Draw();

        }
        
        private void DrawLights(LightSetting setting)
        {
            if (DXManager.Lights == null || DXManager.Lights.Count == 0) return;

            if (_lightTexture == null || _lightTexture.Disposed)
            {
                _lightTexture = new Texture(DXManager.Device, Settings.ScreenWidth, Settings.ScreenHeight, 1, Usage.RenderTarget, Format.A8R8G8B8, Pool.Default);
                _lightTexture.Disposing += FloorTexture_Disposing;
                _lightSurface = _lightTexture.GetSurfaceLevel(0);
            }


            Surface oldSurface = DXManager.CurrentSurface;
            DXManager.SetSurface(_lightSurface);

            DXManager.Device.Clear(ClearFlags.Target, setting == LightSetting.Night ? Color.Black : Color.FromArgb(255, 50, 50, 50), 0, 0);


            int light;
            Point p;
            DXManager.SetBlend(true);
            DXManager.Device.RenderState.SourceBlend = Blend.SourceAlpha;


            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];
                if (ob.Light > 0 && (!ob.Dead || ob == MapObject.User || ob.Race == ObjectType.Spell))
                {

                    light = ob.Light;

                    if (light >= DXManager.Lights.Count)
                        light = DXManager.Lights.Count - 1;

                    p = ob.DrawLocation;
                    p.Offset(((light + 1)*-65 + CellWidth)/2, ((light + 1)*-50 + CellHeight)/2);


                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                        DXManager.Sprite.Draw2D(DXManager.Lights[light], PointF.Empty, 0, p, ob is MonsterObject && ob.AI != 6 ? Color.PaleVioletRed : Color.White);
                }
                
                if (!Settings.Effect) continue;
                for (int e = 0; e < ob.Effects.Count; e++)
                {
                    Effect effect = ob.Effects[e];
                    if (!effect.Blend || CMain.Time < effect.Start || (!(effect is Missile) && effect.Light < ob.Light)) continue;

                    light = effect.Light;

                    p = effect.DrawLocation;
                    p.Offset(((light + 1)*-65 + CellWidth)/2, ((light + 1)*-50 + CellHeight)/2);


                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                        DXManager.Sprite.Draw2D(DXManager.Lights[light], PointF.Empty, 0, p, Color.White);

                }
            }


            if (Settings.Effect)
                for (int e = 0; e < Effects.Count; e++)
                {
                    Effect effect = Effects[e];
                    if (!effect.Blend || CMain.Time < effect.Start) continue;

                    light = effect.Light;

                    p = effect.DrawLocation;
                    p.Offset(((light + 1)*-65 + CellWidth)/2, ((light + 1)*-50 + CellHeight)/2);


                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                        DXManager.Sprite.Draw2D(DXManager.Lights[light], PointF.Empty, 0, p, Color.White);
                }


            for (int y = MapObject.User.Movement.Y - ViewRangeY - 10; y <= MapObject.User.Movement.Y + ViewRangeY + 10; y++)
            {
                if (y < 0) continue;
                if (y >= Height) break;
                for (int x = MapObject.User.Movement.X - ViewRangeX - 10; x < MapObject.User.Movement.X + ViewRangeX + 10; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    int imageIndex = (M2CellInfo[x, y].FrontImage & 0x7FFF) - 1;
                    if (M2CellInfo[x, y].Light <= 0 || M2CellInfo[x, y].Light >= 10) continue;
                    light = M2CellInfo[x, y].Light*3;
                    int fileIndex = M2CellInfo[x, y].FileIndex;

                    p = new Point(
                        (x + OffSetX - MapObject.User.Movement.X) * CellWidth + MapObject.User.OffSetMove.X,
                        (y + OffSetY - MapObject.User.Movement.Y) * CellHeight + MapObject.User.OffSetMove.Y + 32);
                    p.Offset(((light + 1)*-65 + CellWidth)/2, ((light + 1)*-50 + CellHeight)/2);

                    if (M2CellInfo[x, y].AnimationFrame > 0)
                        p.Offset(Libraries.Objects[fileIndex].GetOffSet(imageIndex));

                    if (light > DXManager.Lights.Count)
                        light = DXManager.Lights.Count - 1;

                    if (DXManager.Lights[light] != null && !DXManager.Lights[light].Disposed)
                        DXManager.Sprite.Draw2D(DXManager.Lights[light], PointF.Empty, 0, p, Color.White);
                }
            }
            DXManager.SetBlend(false);
            DXManager.SetSurface(oldSurface);

            DXManager.Device.RenderState.SourceBlend = Blend.DestinationColor;
            DXManager.Device.RenderState.DestinationBlend = Blend.BothInvSourceAlpha;
            
            DXManager.Sprite.Draw2D(_lightTexture, PointF.Empty, 0, PointF.Empty, Color.White);
            DXManager.Sprite.End();
            DXManager.Sprite.Begin(SpriteFlags.AlphaBlend);



        }
        
        private static void OnMouseClick(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;
            if (me == null) return;

            switch (me.Button)
            {
                case MouseButtons.Left:
                    {
                        AutoRun = false;
                        if (MapObject.MouseObject == null) return;
                        NPCObject npc = MapObject.MouseObject as NPCObject;
                        if (npc != null)
                        {
                            if (CMain.Time <= GameScene.NPCTime && npc.ObjectID == GameScene.NPCID) return;

                            GameScene.NPCTime = CMain.Time + 5000;
                            GameScene.NPCID = npc.ObjectID;
                            Network.Enqueue(new C.CallNPC {ObjectID = npc.ObjectID, Key = "[Main]"});
                        }
                    }
                    break;
                case MouseButtons.Right:
                    {
                        AutoRun = false;
                        if (MapObject.MouseObject == null) return;
                        PlayerObject player = MapObject.MouseObject as PlayerObject;
                        if (player == null || player == User || !CMain.Ctrl) return;
                        if (CMain.Time <= GameScene.InspectTime && player.ObjectID == InspectDialog.InspectID) return;

                        GameScene.InspectTime = CMain.Time + 500;
                        InspectDialog.InspectID = player.ObjectID;
                        Network.Enqueue(new C.Inspect {ObjectID = player.ObjectID});
                    }
                    break;
                case MouseButtons.Middle:
                    AutoRun = !AutoRun;
                    break;
            }
        }

        private static void OnMouseDown(object sender, MouseEventArgs e)
        {
            MapButtons |= e.Button;
            GameScene.CanRun = false;


            if (e.Button != MouseButtons.Left) return;

            if (GameScene.SelectedCell != null)
            {
                if (GameScene.SelectedCell.GridType != MirGridType.Inventory)
                {
                    GameScene.SelectedCell = null;
                    return;
                }

                MirItemCell cell = GameScene.SelectedCell;

                if (cell.Item.Count == 1)
                {
                    MirMessageBox messageBox = new MirMessageBox(string.Format("Are you sure you want to drop {0}?", cell.Item.Name), MirMessageBoxButtons.YesNo);

                    messageBox.YesButton.Click += (o, a) =>
                        {
                            Network.Enqueue(new C.DropItem {UniqueID = cell.Item.UniqueID, Count = 1});

                            cell.Locked = true;
                        };
                    messageBox.Show();
                }
                else
                {
                    MirAmountBox amountBox = new MirAmountBox("Drop Amount:", cell.Item.Info.Image, cell.Item.Count);

                    amountBox.OKButton.Click += (o, a) =>
                        {
                            if (amountBox.Amount <= 0) return;
                            Network.Enqueue(new C.DropItem
                                {
                                    UniqueID = cell.Item.UniqueID,
                                    Count = amountBox.Amount
                                });

                            cell.Locked = true;
                        };

                    amountBox.Show();
                }
                GameScene.SelectedCell = null;

                return;
            }
            if (GameScene.PickedUpGold)
            {
                MirAmountBox amountBox = new MirAmountBox("Drop Amount:", 116, GameScene.Gold);

                amountBox.OKButton.Click += (o, a) =>
                    {
                        if (amountBox.Amount > 0)
                        {
                            Network.Enqueue(new C.DropGold {Amount = amountBox.Amount});
                        }
                    };

                amountBox.Show();
                GameScene.PickedUpGold = false;
            }



            if (MapObject.MouseObject != null && !MapObject.MouseObject.Dead && !(MapObject.MouseObject is ItemObject) &&
                !(MapObject.MouseObject is NPCObject))
            {
                MapObject.TargetObject = MapObject.MouseObject;
                if (MapObject.MouseObject is MonsterObject && MapObject.MouseObject.AI != 6)
                    MapObject.MagicObject = MapObject.TargetObject;
            }
            else
                MapObject.TargetObject = null;
        }

        private void CheckInput()
        {
            if (CMain.Time < InputDelay || CMain.Time < User.BlizzardFreezeTime || User.Poison == PoisonType.Paralysis || User.Poison == PoisonType.Frozen) return;

            if (User.NextMagic != null)
            {
                UseMagic(User.NextMagic);
                return;
            }

            if (MapObject.TargetObject != null && !MapObject.TargetObject.Dead)
            {
                if (((MapObject.TargetObject.Name.EndsWith(")") || MapObject.TargetObject is PlayerObject) && CMain.Shift) ||
                    (!MapObject.TargetObject.Name.EndsWith(")") && MapObject.TargetObject is MonsterObject))
                {
                    if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, 1))
                    {
                        if (CMain.Time > GameScene.AttackTime)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Attack1, Direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation), Location = User.CurrentLocation };
                            return;
                        }
                    }
                }
            }

            MirDirection direction;
            if (MouseControl == this)
            {
                direction = MouseDirection();
                if (AutoRun)
                {
                    if (GameScene.CanRun && CanRun(direction) && CMain.Time > GameScene.NextRunTime && User.HP >= 10)
                    {
                        User.QueuedAction = new QueuedAction { Action = MirAction.Running, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 2) };
                        return;
                    }
                    if (CanWalk(direction))
                    {
                        User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                        return;
                    }
                    if (direction != User.Direction)
                    {
                        User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                        return;
                    }
                    return;
                }

                switch (MapButtons)
                {
                    case MouseButtons.Left:
                        if (MapObject.MouseObject is NPCObject || (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != User)) break;

                        if (CMain.Alt)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Harvest, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }
                        if (CMain.Shift)
                        {
                            if (CMain.Time > GameScene.AttackTime)
                            {
                                User.QueuedAction = new QueuedAction { Action = MirAction.Attack1, Direction = direction, Location = User.CurrentLocation };
                            }
                            return;
                        }
                        if (MapLocation == User.CurrentLocation)
                        {
                            if (CMain.Time > GameScene.PickUpTime)
                            {
                                GameScene.PickUpTime = CMain.Time + 200;
                                Network.Enqueue(new C.PickUp());
                            }
                            return;
                        }
                        if (CanWalk(direction))
                        {
                            //if (MapObject.MouseObject != null) return;
                            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                            return;
                        }
                        if (direction != User.Direction)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }
                        break;
                    case MouseButtons.Right:
                        if (MapObject.MouseObject is PlayerObject && MapObject.MouseObject != User && CMain.Ctrl) break;

                        if (Functions.InRange(MapLocation, User.CurrentLocation, 2))
                        {
                            if (direction != User.Direction)
                            {
                                User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            }
                            return;
                        }
                        if (GameScene.CanRun && CanRun(direction) && CMain.Time > GameScene.NextRunTime && User.HP >= 10)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Running, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 2) };
                            return;
                        }
                        if (CanWalk(direction))
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
                            return;
                        }
                        if (direction != User.Direction)
                        {
                            User.QueuedAction = new QueuedAction { Action = MirAction.Standing, Direction = direction, Location = User.CurrentLocation };
                            return;
                        }
                        break;
                }
            }

            if (MapObject.TargetObject == null || MapObject.TargetObject.Dead) return;
            if (((!MapObject.TargetObject.Name.EndsWith(")") && !(MapObject.TargetObject is PlayerObject)) || !CMain.Shift) &&
                (MapObject.TargetObject.Name.EndsWith(")") || !(MapObject.TargetObject is MonsterObject))) return;
            if (Functions.InRange(MapObject.TargetObject.CurrentLocation, User.CurrentLocation, 1)) return;

            direction = Functions.DirectionFromPoint(User.CurrentLocation, MapObject.TargetObject.CurrentLocation);

            if (!CanWalk(direction)) return;

            User.QueuedAction = new QueuedAction { Action = MirAction.Walking, Direction = direction, Location = Functions.PointMove(User.CurrentLocation, direction, 1) };
        }

        private void UseMagic(ClientMagic magic)
        {
            if (CMain.Time < GameScene.SpellTime || User.Poison == PoisonType.Stun)
            {
                User.ClearMagic();
                return;
            }

            int cost = magic.Level*magic.LevelCost + magic.BaseCost;

            if (magic.Spell == Spell.Teleport)
            {
                for (int i = 0; i < GameScene.Scene.Buffs.Count; i++)
                {
                    if (GameScene.Scene.Buffs[i].Type != BuffType.Teleport) continue;
                    cost += (int) (User.MaxMP*0.3F);
                }
            }

            if (cost > MapObject.User.MP)
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    GameScene.Scene.OutputMessage("Not Enough Mana to cast.");
                }
                User.ClearMagic();
                return;
            }

            MapObject target = null;


            //Targeting
            switch (magic.Spell)
            {
                case Spell.FireBall:
                case Spell.GreatFireBall:
                case Spell.ElectricShock:
                case Spell.Poisoning:
                case Spell.ThunderBolt:
                case Spell.FlameDisruptor:
                case Spell.SoulFireBall:
                case Spell.TurnUndead:
                case Spell.FrostCrunch:
                case Spell.Vampirism:
                case Spell.Revelation:
                case Spell.Entrapment:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }

                    if (target == null) target = MapObject.MagicObject;

                    if (target != null && target.Race == ObjectType.Monster) MapObject.MagicObject = target;
                    break;
                case Spell.Purification:
                case Spell.Healing:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }

                    if (target == null) target = User;
                    break;
                case Spell.FireBang:
                case Spell.MassHiding:
                case Spell.FireWall:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }
                    break;
                case Spell.PoisonField:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }
                    if (CMain.Time < GameScene.PoisonFieldTime)
                    {
                        if (CMain.Time >= OutputDelay)
                        {
                            OutputDelay = CMain.Time + 1000;
                            GameScene.Scene.OutputMessage(string.Format("You cannot cast Poison Field for another {0} seconds.", (GameScene.PoisonFieldTime - CMain.Time - 1)/1000 + 1));
                        }
                        
                        User.ClearMagic();
                        return;
                    }
                    break;
                case Spell.Blizzard:
                case Spell.MeteorStrike:
                    if (User.NextMagicObject != null)
                    {
                        if (!User.NextMagicObject.Dead && User.NextMagicObject.Race != ObjectType.Item && User.NextMagicObject.Race != ObjectType.Merchant)
                            target = User.NextMagicObject;
                    }
                    break;
            }

            MirDirection dir = (target == null || target == User) ? User.NextMagicDirection : Functions.DirectionFromPoint(User.CurrentLocation, target.CurrentLocation);

            Point location = target != null ? target.CurrentLocation : User.NextMagicLocation;

            if (!Functions.InRange(User.CurrentLocation, location, 9))
            {
                if (CMain.Time >= OutputDelay)
                {
                    OutputDelay = CMain.Time + 1000;
                    GameScene.Scene.OutputMessage("Target is too far.");
                }
                User.ClearMagic();
                return;
            }

            User.QueuedAction = new QueuedAction { Action = MirAction.Spell, Direction = dir, Location = User.CurrentLocation, Params = new List<object>() };
            User.QueuedAction.Params.Add(magic.Spell);
            User.QueuedAction.Params.Add(target != null ? target.ObjectID : 0);
            User.QueuedAction.Params.Add(location);
        }

        public static MirDirection MouseDirection(float ratio = 45F) //22.5 = 16
        {
            Point p = new Point(MouseLocation.X / CellWidth, MouseLocation.Y / CellHeight);
            if (Functions.InRange(new Point(OffSetX, OffSetY), p, 2))
                return Functions.DirectionFromPoint(new Point(OffSetX, OffSetY), p);

            PointF c = new PointF(OffSetX * CellWidth + CellWidth / 2F, OffSetY * CellHeight + CellHeight / 2F);
            PointF a = new PointF(c.X, 0);
            PointF b = MouseLocation;
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (MouseLocation.X < c.X) angle = 360 - angle;
            angle += ratio/2;
            if (angle > 360) angle -= 360;

            return (MirDirection)(angle / ratio);
        }

        public static int Direction16(Point source, Point destination)
        {
            PointF c = new PointF(source.X, source.Y);
            PointF a = new PointF(c.X, 0);
            PointF b = new PointF(destination.X, destination.Y);
            float bc = (float)Distance(c, b);
            float ac = bc;
            b.Y -= c.Y;
            c.Y += bc;
            b.Y += bc;
            float ab = (float)Distance(b, a);
            double x = (ac * ac + bc * bc - ab * ab) / (2 * ac * bc);
            double angle = Math.Acos(x);

            angle *= 180 / Math.PI;

            if (destination.X < c.X) angle = 360 - angle;
            angle += 11.25F;
            if (angle > 360) angle -= 360;

            return (int)(angle / 22.5F);
        }

        public static double Distance(PointF p1, PointF p2)
        {
            double x = p2.X - p1.X;
            double y = p2.Y - p1.Y;
            return Math.Sqrt(x*x + y*y);
        }

        private bool EmptyCell(Point p)
        {
            if ((M2CellInfo[p.X, p.Y].BackImage & 0x20000000) != 0 || (M2CellInfo[p.X, p.Y].FrontImage & 0x8000) != 0) // + (M2CellInfo[P.X, P.Y].FrontImage & 0x7FFF) != 0)
                return false;

            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];

                if (ob.CurrentLocation == p && ob.Blocking)
                    return false;
            }

            return true;
        }

        private bool CanWalk(MirDirection dir)
        {
            return EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 1)) && !User.InTrapRock;
        }

        private bool CanRun(MirDirection dir)
        {
            return CanWalk(dir) && EmptyCell(Functions.PointMove(User.CurrentLocation, dir, 2)) && !User.InTrapRock;
        }

        public bool ValidPoint(Point p)
        {
            //GameScene.Scene.ChatDialog.ReceiveChat(string.Format("cell: {0}", (M2CellInfo[p.X, p.Y].BackImage & 0x20000000)), ChatType.Hint);
            return (M2CellInfo[p.X, p.Y].BackImage & 0x20000000) == 0;
        }
        public bool HasTarget(Point p)
        {
            for (int i = 0; i < Objects.Count; i++)
            {
                MapObject ob = Objects[i];

                if (ob.CurrentLocation == p && ob.Blocking)
                    return true;
            }
            return false;
        }
        public bool CanHalfMoon(Point p, MirDirection d)
        {
            d = Functions.PreviousDir(d);
            for (int i = 0; i < 4; i++)
            {
                if (HasTarget(Functions.PointMove(p, d, 1))) return true;
                d = Functions.NextDir(d);
            }
            return false;
        }
        public bool CanCrossHalfMoon(Point p)
        {
            MirDirection dir = MirDirection.Up;
            for (int i = 0; i < 8; i++)
            {
                if (HasTarget(Functions.PointMove(p, dir, 1))) return true;
                dir = Functions.NextDir(dir);
            }
            return false;
        }

        private void FloorTexture_Disposing(object sender, EventArgs e)
        {
            FloorValid = false;
            _floorTexture = null;

            if (_floorSurface != null && !_floorSurface.Disposed)
                _floorSurface.Dispose();
            _floorSurface = null;
        }
        #region Disposable

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Objects.Clear();

                MapButtons = 0;
                MouseLocation = Point.Empty;
                InputDelay = 0;
                NextAction = 0;

                M2CellInfo = null;
                Width = 0;
                Height = 0;

                FileName = String.Empty;
                Title = String.Empty;
                MiniMap = 0;
                BigMap = 0;
                Lights = 0;
                FloorValid = false;
                LightsValid = false;

                if (_floorSurface != null && !_floorSurface.Disposed)
                    _floorSurface.Dispose();


                if (_lightSurface != null && !_lightSurface.Disposed)
                    _lightSurface.Dispose();

                AnimationCount = 0;
                Effects.Clear();
            }

            base.Dispose(disposing);
        }

        #endregion

        public class CellInfo
        {
            public int BackImage;
            public short MiddleImage;
            public short FrontImage;

            public byte DoorIndex;
            public byte DoorOffset;
            public byte AnimationFrame;
            public byte AnimationTick;
            public byte FileIndex;
            public byte Light;
            public byte Unknown;

            public List<MapObject> CellObjects;

            public void AddObject(MapObject ob)
            {
                if (CellObjects == null) CellObjects = new List<MapObject>();

                CellObjects.Insert(0, ob);
                Sort();
            }
            public void RemoveObject(MapObject ob)
            {
                CellObjects.Remove(ob);

                if (CellObjects.Count == 0) CellObjects = null;
                else Sort();
            }
            public void DrawObjects()
            {
                if (CellObjects == null) return;
                for (int i = 0; i < CellObjects.Count; i++)
                    CellObjects[i].Draw();
            }

            public void Sort()
            {
                CellObjects.Sort(delegate(MapObject ob1, MapObject ob2)
                {
                    if (ob1.Race == ObjectType.Item && ob2.Race != ObjectType.Item)
                        return -1;
                    if (ob2.Race == ObjectType.Item && ob1.Race != ObjectType.Item)
                        return 1;
                    if (ob1.Race == ObjectType.Spell && ob2.Race != ObjectType.Spell)
                        return -1;
                    if (ob2.Race == ObjectType.Spell && ob1.Race != ObjectType.Spell)
                        return 1;

                        int i = ob2.Dead.CompareTo(ob1.Dead);
                        return i == 0 ? ob1.ObjectID.CompareTo(ob2.ObjectID) : i;
                    });
            }
        }

        public void RemoveObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].RemoveObject(ob);
        }
        public void AddObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].AddObject(ob);
        }
        public void SortObject(MapObject ob)
        {
            M2CellInfo[ob.MapLocation.X, ob.MapLocation.Y].Sort();
        }
    }
    public sealed class MainDialog : MirImageControl
    {
        public static UserObject User
        {
            get { return MapObject.User; }
            set { MapObject.User = value; }
        }

        public MirImageControl ExperienceBar, WeightBar;
        public MirButton MenuButton, InventoryButton, CharacterButton, SkillButton, QuestButton, OptionButton;
        public MirControl HealthOrb;

        public MirLabel HealthLabel,
                        ManaLabel,
                        LevelLabel,
                        CharacterName,
                        ExperienceLabel,
                        GoldLabel,
                        WeightLabel,
                        AModeLabel,
                        PModeLabel,
                        TerritoryLabel;

        public MainDialog()
        {
            Index = Settings.HighResolution ? 2 : 1;
            Library = Libraries.Prguse;
            Location = new Point(0, Settings.ScreenHeight - Size.Height);
            PixelDetect = true;


            InventoryButton = new MirButton
            {
                HoverIndex = 1904,
                Index = 1903,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth - 96, 76),
                Parent = this,
                PressedIndex = 1905,
                Sound = SoundList.ButtonA,
            };
            InventoryButton.Click += (o, e) =>
            {
                if (GameScene.Scene.InventoryDialog.Visible)
                    GameScene.Scene.InventoryDialog.Hide();
                else
                    GameScene.Scene.InventoryDialog.Show();
            };

            CharacterButton = new MirButton
            {
                HoverIndex = 1901,
                Index = 1900,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth - 119, 76),
                Parent = this,
                PressedIndex = 1902,
                Sound = SoundList.ButtonA,
            };
            CharacterButton.Click += (o, e) =>
            {
                if (GameScene.Scene.CharacterDialog.Visible && GameScene.Scene.CharacterDialog.CharacterPage.Visible)
                    GameScene.Scene.CharacterDialog.Hide();
                else
                {
                    GameScene.Scene.CharacterDialog.Show();
                    GameScene.Scene.CharacterDialog.ShowCharacterPage();
                }
            };

            SkillButton = new MirButton
            {
                HoverIndex = 1907,
                Index = 1906,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth - 73, 76),
                Parent = this,
                PressedIndex = 1908,
                Sound = SoundList.ButtonA,
            };
            SkillButton.Click += (o, e) =>
            {
                if (GameScene.Scene.CharacterDialog.Visible && GameScene.Scene.CharacterDialog.SkillPage.Visible)
                    GameScene.Scene.CharacterDialog.Hide();
                else
                {
                    GameScene.Scene.CharacterDialog.Show();
                    GameScene.Scene.CharacterDialog.ShowSkillPage();
                }
            };

            QuestButton = new MirButton
            {
                HoverIndex = 1910,
                Index = 1909,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth - 50, 76),
                Parent = this,
                PressedIndex = 1911,
                Sound = SoundList.ButtonA,
            };

            OptionButton = new MirButton
            {
                HoverIndex = 1913,
                Index = 1912,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth - 27, 76),
                Parent = this,
                PressedIndex = 1914,
                Sound = SoundList.ButtonA,
            };
            OptionButton.Click += (o, e) =>
            {
                if (!GameScene.Scene.OptionDialog.Visible)
                    GameScene.Scene.OptionDialog.Show();
                else GameScene.Scene.OptionDialog.Hide();
            };

            MenuButton = new MirButton
            {
                HoverIndex = 1961,
                Index = 1960,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth - 55, 35),
                Parent = this,
                PressedIndex = 1962,
                Sound = SoundList.ButtonC,
            };
            MenuButton.Click += (o, e) =>
            {
                if (!GameScene.Scene.MenuDialog.Visible) GameScene.Scene.MenuDialog.Show();
                else GameScene.Scene.MenuDialog.Hide();
            };

            HealthOrb = new MirControl
            {
                Parent = this,
                Location = new Point(0, 30),
                NotControl = true,
            };
            HealthOrb.BeforeDraw += HealthOrb_BeforeDraw;

            HealthLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(0, 32),
                Parent = HealthOrb,
            };
            HealthLabel.SizeChanged += Label_SizeChanged;

            ManaLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(0, 50),
                Parent = HealthOrb,
            };
            ManaLabel.SizeChanged += Label_SizeChanged;

            LevelLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(5, 108),
            };
            CharacterName = new MirLabel
            {
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                Location = new Point(6, 122),
                Size = new Size(90, 16)
            };


            ExperienceBar = new MirImageControl
            {
                Index = Settings.HighResolution ? 8 : 7,
                Library = Libraries.Prguse,
                Location = new Point(9, 143),
                Parent = this,
                DrawImage = false,
                NotControl = true,
            };
            ExperienceBar.BeforeDraw += ExperienceBar_BeforeDraw;

            ExperienceLabel = new MirLabel
            {
                AutoSize = true,
                Parent = ExperienceBar,
                NotControl = true,
            };

            GoldLabel = new MirLabel
            {
                DrawFormat = TextFormatFlags.VerticalCenter,
                Font = new Font(Settings.FontName, 8F),
                Location = new Point(Settings.ScreenWidth - 105, 119),
                Parent = this,
                Size = new Size(99, 13),
                Sound = SoundList.Gold,
            };
            GoldLabel.Click += (o, e) =>
            {
                if (GameScene.SelectedCell == null)
                    GameScene.PickedUpGold = !GameScene.PickedUpGold && GameScene.Gold > 0;
            };



            WeightBar = new MirImageControl
            {
                Index = 76,
                Library = Libraries.Prguse,
                Location = new Point(Settings.ScreenWidth - 105, 103),
                Parent = this,
                DrawImage = false,
                NotControl = true,
            };
            WeightBar.BeforeDraw += WeightBar_BeforeDraw;

            WeightLabel = new MirLabel
            {
                Parent = this,
                Location = new Point(Settings.ScreenWidth - 30, 101),
                Size = new Size(26, 14),
            };

            AModeLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(115, 125)
            };

            PModeLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(230, 125),
                Visible = false
            };

            TerritoryLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                OutLineColour = Color.Black,
                Parent = this,
                Location = new Point(400, 125)
            };
        }

        public void Process()
        {
            switch (GameScene.Scene.AMode)
            {
                case AttackMode.Peace:
                    AModeLabel.Text = "[Mode: Peacefull]";
                    break;
                case AttackMode.Group:
                    AModeLabel.Text = "[Mode: Group]";
                    break;
                case AttackMode.Guild:
                    AModeLabel.Text = "[Mode: Guild]";
                    break;
                case AttackMode.RedBrown:
                    AModeLabel.Text = "[Mode: Red/Brown]";
                    break;
                case AttackMode.All:
                    AModeLabel.Text = "[Mode: Attack All]";
                    break;
            }

            switch (GameScene.Scene.PMode)
            {
                case PetMode.Both:
                    PModeLabel.Text = "[Pet: Attack and Move]";
                    break;
                case PetMode.MoveOnly:
                    PModeLabel.Text = "[Pet: Do Not Attack]";
                    break;
                case PetMode.AttackOnly:
                    PModeLabel.Text = "[Pet: Do Not Move]";
                    break;
                case PetMode.None:
                    PModeLabel.Text = "[Pet: Do Not Attack or Move]";
                    break;
            }

            TerritoryLabel.Text = "[WILLS TERRITORY, MOTHERFUCKER!]";


            HealthLabel.Text = string.Format("HP {0}/{1}", User.HP, User.MaxHP);
            ManaLabel.Text = string.Format("MP {0}/{1} ", User.MP, User.MaxMP);
            LevelLabel.Text = User.Level.ToString();
            ExperienceLabel.Text = string.Format("{0:#0.##%}", User.Experience/(double) User.MaxExperience);
            ExperienceLabel.Location = new Point((ExperienceBar.Size.Width/2) - 20, -10);
            GoldLabel.Text = GameScene.Gold.ToString("###,###,##0");
            CharacterName.Text = User.Name;
            WeightLabel.Text = (User.MaxBagWeight - User.CurrentBagWeight).ToString();
        }

        private void Label_SizeChanged(object sender, EventArgs e)
        {
            MirLabel l = sender as MirLabel;

            if (l == null) return;

            l.Location = new Point(50 - (l.Size.Width/2), l.Location.Y);
        }

        private void HealthOrb_BeforeDraw(object sender, EventArgs e)
        {
            if (Libraries.Prguse == null) return;

            int height;
            if (User.HP != User.MaxHP)
                height = (int) (80*User.HP/(float) User.MaxHP);
            else
                height = 80;

            if (height < 0) height = 0;
            if (height > 80) height = 80;
            Rectangle r = new Rectangle(0, 80 - height, 50, height);
            Libraries.Prguse.Draw(4, r, new Point(0, HealthOrb.DisplayLocation.Y + 80 - height), Color.White, false);

            if (User.MP != User.MaxMP)
                height = (int) (80*User.MP/(float) User.MaxMP);
            else
                height = 80;

            if (height < 0) height = 0;
            if (height > 80) height = 80;
            r = new Rectangle(51, 80 - height, 50, height);

            Libraries.Prguse.Draw(4, r, new Point(51, HealthOrb.DisplayLocation.Y + 80 - height), Color.White, false);
        }

        private void ExperienceBar_BeforeDraw(object sender, EventArgs e)
        {
            if (ExperienceBar.Library == null) return;

            double percent = MapObject.User.Experience/(double) MapObject.User.MaxExperience;
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
                {
                    Size = new Size((int) ((ExperienceBar.Size.Width - 3)*percent), ExperienceBar.Size.Height)
                };

            ExperienceBar.Library.Draw(ExperienceBar.Index, section, ExperienceBar.DisplayLocation, Color.White, false);
        }

        private void WeightBar_BeforeDraw(object sender, EventArgs e)
        {
            if (WeightBar.Library == null) return;
            double percent = MapObject.User.CurrentBagWeight/(double) MapObject.User.MaxBagWeight;
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
                {
                    Size = new Size((int) ((WeightBar.Size.Width - 2)*percent), WeightBar.Size.Height)
                };

            WeightBar.Library.Draw(WeightBar.Index, section, WeightBar.DisplayLocation, Color.White, false);
        }
    }
    public sealed class ChatDialog : MirImageControl
    {
        public List<ChatHistory> History = new List<ChatHistory>();
        public List<MirLabel> ChatLines = new List<MirLabel>();

        public MirButton HomeButton, UpButton, EndButton, DownButton, PositionBar;
        public MirImageControl CountBar;
        public MirTextBox ChatTextBox;
        public Font ChatFont = new Font(Settings.FontName, 8F);
        public string LastPM = string.Empty;

        public int StartIndex, LineCount = 4, WindowSize;

        public ChatDialog()
        {
            Index = Settings.HighResolution ? 2221 : 2201;
            Library = Libraries.Prguse;
            Location = new Point(230, Settings.ScreenHeight - 97);
            PixelDetect = true;

            KeyPress += ChatPanel_KeyPress;
            KeyDown += ChatPanel_KeyDown;
            MouseWheel += ChatPanel_MouseWheel;


            ChatTextBox = new MirTextBox
                {
                    BackColour = Color.DarkGray,
                    ForeColour = Color.Black,
                    Parent = this,
                    Size = new Size(Settings.HighResolution ? 627 : 403, 13),
                    Location = new Point(1, 54),
                    MaxLength = Globals.MaxChatLength,
                    Visible = false,
                    Font = ChatFont,
                };
            ChatTextBox.TextBox.KeyPress += ChatTextBox_KeyPress;
            ChatTextBox.TextBox.KeyDown += ChatTextBox_KeyDown;
            ChatTextBox.TextBox.KeyUp += ChatTextBox_KeyUp;

            HomeButton = new MirButton
                {
                    Index = 2018,
                    HoverIndex = 2019,
                    Library = Libraries.Prguse,
                    Location = new Point(Settings.HighResolution ? 618 : 394, 1),
                    Parent = this,
                    PressedIndex = 2020,
                    Sound = SoundList.ButtonA
                };
            HomeButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex = 0;
                Update();
            };


            UpButton = new MirButton
                {
                    Index = 2021,
                    HoverIndex = 2022,
                    Library = Libraries.Prguse,
                    Location = new Point(Settings.HighResolution ? 618 : 394, 9),
                    Parent = this,
                    PressedIndex = 2023,
                    Sound = SoundList.ButtonA
                };
            UpButton.Click += (o, e) =>
            {
                if (StartIndex == 0) return;
                StartIndex--;
                Update();
            };


            EndButton = new MirButton
                {
                    Index = 2027,
                    HoverIndex = 2028,
                    Library = Libraries.Prguse,
                    Location = new Point(Settings.HighResolution ? 618 : 394, 45),
                    Parent = this,
                    PressedIndex = 2029,
                    Sound = SoundList.ButtonA
                };
            EndButton.Click += (o, e) =>
            {
                if (StartIndex == History.Count - 1) return;
                StartIndex = History.Count - 1;
                Update();
            };

            DownButton = new MirButton
                {
                    Index = 2024,
                    HoverIndex = 2025,
                    Library = Libraries.Prguse,
                    Location = new Point(Settings.HighResolution ? 618 : 394, 39),
                    Parent = this,
                    PressedIndex = 2026,
                    Sound = SoundList.ButtonA
                };
            DownButton.Click += (o, e) =>
            {
                if (StartIndex == History.Count - 1) return;
                StartIndex++;
                Update();
            };



            CountBar = new MirImageControl
                {
                    Index = 2012,
                    Library = Libraries.Prguse,
                    Location = new Point(Settings.HighResolution ? 622 : 398, 16),
                    Parent = this,
                };

            PositionBar = new MirButton
                {
                    Index = 2015,
                    HoverIndex = 2016,
                    Library = Libraries.Prguse,
                    Location = new Point(Settings.HighResolution ? 619 : 395, 16),
                    Parent = this,
                    PressedIndex = 2017,
                    Movable = true,
                    Sound = SoundList.None,
                };
            PositionBar.OnMoving += PositionBar_OnMoving;
        }

        private void ChatTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case (char) Keys.Enter:
                    e.Handled = true;
                    if (!string.IsNullOrEmpty(ChatTextBox.Text))
                    {
                        Network.Enqueue(new C.Chat
                            {
                                Message = ChatTextBox.Text,
                            });

                        if (ChatTextBox.Text[0] == '/')
                        {
                            string[] parts = ChatTextBox.Text.Split(' ');
                            if (parts.Length > 0)
                                LastPM = parts[0];
                        }
                    }
                    ChatTextBox.Visible = false;
                    ChatTextBox.Text = string.Empty;
                    break;
                case (char) Keys.Escape:
                    e.Handled = true;
                    ChatTextBox.Visible = false;
                    ChatTextBox.Text = string.Empty;
                    break;
            }
        }

        void PositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            int x = Settings.HighResolution ? 619 : 395;
            int y = PositionBar.Location.Y;
            if (y >= 16 + CountBar.Size.Height - PositionBar.Size.Height) y = 16 + CountBar.Size.Height - PositionBar.Size.Height;
            if (y < 16) y = 16;

            int h = CountBar.Size.Height - PositionBar.Size.Height;
            h = (int)((y - 16) / (h / (float)(History.Count - 1)));

            if (h != StartIndex)
            {
                StartIndex = h;
                Update();
            }

            PositionBar.Location = new Point(x, y);
        }

        public void ReceiveChat(string text, ChatType type)
        {
            int chatWidth = Settings.HighResolution ? 614 : 390;
            List<string> chat = new List<string>();

            int index = 0;
            for (int i = 1; i < text.Length; i++)
                if (TextRenderer.MeasureText(CMain.Graphics, text.Substring(index, i - index), ChatFont).Width > chatWidth)
                {
                    chat.Add(text.Substring(index, i - index - 1));
                    index = i - 1;
                }
            chat.Add(text.Substring(index, text.Length - index));

            Color foreColour, backColour;

            switch (type)
            {
                case ChatType.Hint:
                    backColour = Color.White;
                    foreColour = Color.DarkGreen;
                    break;
                case ChatType.Announcement:
                    backColour = Color.Blue;
                    foreColour = Color.White;
                    break;
                case ChatType.Shout:
                    backColour = Color.Yellow;
                    foreColour = Color.Black;
                    break;
                case ChatType.System:
                    backColour = Color.Red;
                    foreColour = Color.White;
                    break;
                case ChatType.Group:
                    backColour = Color.White;
                    foreColour = Color.Brown;
                    break;
                case ChatType.WhisperOut:
                    foreColour = Color.CornflowerBlue;
                    backColour = Color.White;
                    break;
                case ChatType.WhisperIn:
                    foreColour = Color.DarkBlue;
                    backColour = Color.White;
                    break;
                default:
                    backColour = Color.White;
                    foreColour = Color.Black;
                    break;
            }

            if (StartIndex == History.Count - LineCount)
                StartIndex += chat.Count;

            for (int i = 0; i < chat.Count; i++)
                History.Add(new ChatHistory {Text = chat[i], BackColour = backColour, ForeColour = foreColour});

            Update();
        }

        private void Update()
        {
            for (int i = 0; i < ChatLines.Count; i++)
                ChatLines[i].Dispose();

            ChatLines.Clear();

            if (StartIndex >= History.Count) StartIndex = History.Count - 1;
            if (StartIndex < 0) StartIndex = 0;

            if (History.Count > 1)
            {
                int h = CountBar.Size.Height - PositionBar.Size.Height;
                h = (int) ((h/(float) (History.Count - 1))*StartIndex);
                PositionBar.Location = new Point(Settings.HighResolution ? 619 : 395, 16 + h);
            }

            int y = 1;
            for (int i = StartIndex; i < History.Count; i++)
            {
                MirLabel temp = new MirLabel
                    {
                        AutoSize = true,
                        BackColour = History[i].BackColour,
                        ForeColour = History[i].ForeColour,
                        Location = new Point(1, y),
                        OutLine = false,
                        Parent = this,
                        Text = History[i].Text,
                        Font = ChatFont,
                    };
                temp.MouseWheel += ChatPanel_MouseWheel;
                ChatLines.Add(temp);

                temp.Click += (o, e) =>
                    {
                        MirLabel l = o as MirLabel;
                        if (l == null) return;

                        string[] parts = l.Text.Split(':', ' ');
                        if (parts.Length == 0) return;

                        string name = Regex.Replace(parts[0], "[^A-Za-z0-9]", "");

                        ChatTextBox.SetFocus();
                        ChatTextBox.Text = string.Format("/{0} ", name);
                        ChatTextBox.Visible = true;
                        ChatTextBox.TextBox.SelectionLength = 0;
                        ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    };


                y += 13;
                if (i - StartIndex == LineCount - 1) break;
            }

        }

        private void ChatPanel_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (StartIndex == 0) return;
                    StartIndex--;
                    break;
                case Keys.Home:
                    if (StartIndex == 0) return;
                    StartIndex = 0;
                    break;
                case Keys.Down:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex++;
                    break;
                case Keys.End:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex = History.Count - 1;
                    break;
                case Keys.PageUp:
                    if (StartIndex == 0) return;
                    StartIndex -= LineCount;
                    break;
                case Keys.PageDown:
                    if (StartIndex == History.Count - 1) return;
                    StartIndex += LineCount;
                    break;
                default:
                    return;
            }
            Update();
            e.Handled = true;
        }
        private void ChatPanel_KeyPress(object sender, KeyPressEventArgs e)
        {
            switch (e.KeyChar)
            {
                case '@':
                case '!':
                case ' ':
                case (char) Keys.Enter:
                    ChatTextBox.SetFocus();
                    if (e.KeyChar == '!') ChatTextBox.Text = "!";
                    if (e.KeyChar == '@') ChatTextBox.Text = "@";
                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
                case '/':
                    ChatTextBox.SetFocus();
                    ChatTextBox.Text = LastPM + " ";
                    ChatTextBox.Visible = true;
                    ChatTextBox.TextBox.SelectionLength = 0;
                    ChatTextBox.TextBox.SelectionStart = ChatTextBox.Text.Length;
                    e.Handled = true;
                    break;
            }
        }
        private void ChatPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta/SystemInformation.MouseWheelScrollDelta;

            if (StartIndex == 0 && count >= 0) return;
            if (StartIndex == History.Count - 1 && count <= 0) return;

            StartIndex -= count;
            Update();
        }

        private void ChatTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyUp(sender, e);
                    break;

            }
        }

        private void ChatTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                    CMain.CMain_KeyDown(sender, e);
                    break;

            }
        }


        public void ChangeSize()
        {
            if (++WindowSize >= 3) WindowSize = 0;

            int y = DisplayRectangle.Bottom;
            switch (WindowSize)
            {
                case 0:
                    LineCount = 4;
                    Index = Settings.HighResolution ? 2221 : 2201;
                    CountBar.Index = 2012;
                    DownButton.Location = new Point(Settings.HighResolution ? 618 : 394, 39);
                    EndButton.Location = new Point(Settings.HighResolution ? 618 : 394, 45);
                    ChatTextBox.Location = new Point(1, 54);
                    break;
                case 1:
                    LineCount = 7;
                    Index = Settings.HighResolution ? 2224 : 2204;
                    CountBar.Index = 2013;
                    DownButton.Location = new Point(Settings.HighResolution ? 618 : 394, 39 + 48);
                    EndButton.Location = new Point(Settings.HighResolution ? 618 : 394, 45 + 48);
                    ChatTextBox.Location = new Point(1, 54 + 48);
                    break;
                case 2:
                    LineCount = 11;
                    Index = Settings.HighResolution ? 2227 : 2207;
                    CountBar.Index = 2014;
                    DownButton.Location = new Point(Settings.HighResolution ? 618 : 394, 39 + 96);
                    EndButton.Location = new Point(Settings.HighResolution ? 618 : 394, 45 + 96);
                    ChatTextBox.Location = new Point(1, 54 + 96); 
                    break;
            }
            Location = new Point(Location.X, y - Size.Height);
            Update();
        }

        public class ChatHistory
        {
            public string Text;
            public Color ForeColour, BackColour;
        }
    }
    public sealed class ChatControlBar : MirImageControl
    {
        public MirButton SizeButton, SettingsButton, NormalButton, ShoutButton, WhisperButton, LoverButton, MentorButton, GroupButton, GuildButton;

        public ChatControlBar()
        {
            Index = Settings.HighResolution ? 2034 : 2035;
            Library = Libraries.Prguse;
            Location = new Point(230, Settings.ScreenHeight - 112);

            SizeButton = new MirButton
                {
                    Index = 2057,
                    HoverIndex = 2058,
                    PressedIndex = 2059,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(Settings.HighResolution ? 574 : 350, 1),
                    Visible = true,
                    Sound = SoundList.ButtonA
                };
            SizeButton.Click += (o, e) =>
            {
                GameScene.Scene.ChatDialog.ChangeSize();
                Location = new Point(Location.X, GameScene.Scene.ChatDialog.DisplayRectangle.Top - Size.Height);
                if (GameScene.Scene.BeltDialog.Index == 1932)
                    GameScene.Scene.BeltDialog.Location = new Point(230, Location.Y - GameScene.Scene.BeltDialog.Size.Height);
            };


            SettingsButton = new MirButton
                {
                    Index = 2060,
                    HoverIndex = 2061,
                    PressedIndex = 2062,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(Settings.HighResolution ? 596 : 372, 1),
                    Sound = SoundList.ButtonA
                };

            NormalButton = new MirButton
                {
                    Index = 2037,
                    HoverIndex = 2037,
                    PressedIndex = 2038,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(12, 1),
                    Sound = SoundList.ButtonA
                };



            ShoutButton = new MirButton
                {
                    Index = 2039,
                    HoverIndex = 2040,
                    PressedIndex = 2041,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(34, 1),
                    Sound = SoundList.ButtonA
                };


            WhisperButton = new MirButton
                {
                    Index = 2042,
                    HoverIndex = 2043,
                    PressedIndex = 2044,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(56, 1),
                    Sound = SoundList.ButtonA
                };


            LoverButton = new MirButton
                {
                    Index = 2045,
                    HoverIndex = 2046,
                    PressedIndex = 2047,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(78, 1),
                    Sound = SoundList.ButtonA
                };

            MentorButton = new MirButton
                {
                    Index = 2048,
                    HoverIndex = 2049,
                    PressedIndex = 2050,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(100, 1),
                    Sound = SoundList.ButtonA
                };


            GroupButton = new MirButton
                {
                    Index = 2051,
                    HoverIndex = 2052,
                    PressedIndex = 2053,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(122, 1),
                    Sound = SoundList.ButtonA
                };


            GuildButton = new MirButton
                {
                    Index = 2054,
                    HoverIndex = 2055,
                    PressedIndex = 2056,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(144, 1),
                    Sound = SoundList.ButtonA
                };

        }

    }
    public sealed class InventoryDialog : MirImageControl
    {
        public MirImageControl WeightBar;
        public MirItemCell[] Grid;
        public MirButton CloseButton;
        public MirLabel GoldLabel, WeightLabel;

        public InventoryDialog()
        {
            Index = 196;
            Library = Libraries.Title;
            Movable = true;
            Sort = true;
            Visible = false;

            WeightBar = new MirImageControl
                {
                    Index = 24,
                    Library = Libraries.Prguse,
                    Location = new Point(182, 217),
                    Parent = this,
                    DrawImage = false,
                    NotControl = true,
                };

            CloseButton = new MirButton
                {
                    HoverIndex = 361,
                    Index = 360,
                    Location = new Point(289, 3),
                    Library = Libraries.Prguse2,
                    Parent = this,
                    PressedIndex = 362,
                    Sound = SoundList.ButtonA,
                };
            CloseButton.Click += (o, e) => Hide();

            GoldLabel = new MirLabel
                {
                    Parent = this,
                    Location = new Point(40, 212),
                    Size = new Size(111, 14),
                    Sound = SoundList.Gold,
                };
            GoldLabel.Click += (o, e) =>
                {
                    if (GameScene.SelectedCell == null)
                        GameScene.PickedUpGold = !GameScene.PickedUpGold && GameScene.Gold > 0;
                };


            Grid = new MirItemCell[8*5];

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    Grid[8*y + x] = new MirItemCell
                        {
                            ItemSlot = 8*y + x,
                            GridType = MirGridType.Inventory,
                            Library = Libraries.Items,
                            Parent = this,
                            Location = new Point(x*36 + 9 + x, y*32 + 37 + y),
                        };
                }
            }

            WeightLabel = new MirLabel
                {
                    Parent = this,
                    Location = new Point(268, 212),
                    Size = new Size(26, 14)
                };
            WeightBar.BeforeDraw += WeightBar_BeforeDraw;
        }

        public void Process()
        {
            WeightLabel.Text = (MapObject.User.MaxBagWeight - MapObject.User.CurrentBagWeight).ToString();
            GoldLabel.Text = GameScene.Gold.ToString("###,###,##0");
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }

        private void WeightBar_BeforeDraw(object sender, EventArgs e)
        {
            if (WeightBar.Library == null) return;

            double percent = MapObject.User.CurrentBagWeight/(double) MapObject.User.MaxBagWeight;
            if (percent > 1) percent = 1;
            if (percent <= 0) return;

            Rectangle section = new Rectangle
                {
                    Size = new Size((int) ((WeightBar.Size.Width - 3)*percent), WeightBar.Size.Height)
                };

            WeightBar.Library.Draw(WeightBar.Index, section, WeightBar.DisplayLocation, Color.White, false);
        }


        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }
    }
    public sealed class BeltDialog : MirImageControl
    {
        public MirButton CloseButton, RotateButton;
        public MirItemCell[] Grid;

        public BeltDialog()
        {
            Index = 1932;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Visible = true;
            Location = new Point(230, Settings.ScreenHeight - 150);

            BeforeDraw += BeltPanel_BeforeDraw;
            RotateButton = new MirButton
                {
                    HoverIndex = 1927,
                    Index = 1926,
                    Location = new Point(222, 3),
                    Library = Libraries.Prguse,
                    Parent = this,
                    PressedIndex = 1928,
                    Sound = SoundList.ButtonA,
                };
            RotateButton.Click += (o, e) => Flip();

            CloseButton = new MirButton
                {
                    HoverIndex = 1924,
                    Index = 1923,
                    Location = new Point(222, 19),
                    Library = Libraries.Prguse,
                    Parent = this,
                    PressedIndex = 1925,
                    Sound = SoundList.ButtonA,
                };
            CloseButton.Click += (o, e) => Hide();

            Grid = new MirItemCell[6];

            for (int x = 0; x < 6; x++)
            {
                Grid[x] = new MirItemCell
                    {
                        ItemSlot = 40 + x,
                        Size = new Size(32, 32),
                        GridType = MirGridType.Inventory,
                        Library = Libraries.Items,
                        Parent = this,
                        Location = new Point(x*35 + 12, 3),
                    };
            }

        }

        private void BeltPanel_BeforeDraw(object sender, EventArgs e)
        {
            //if Transparent return

            if (Libraries.Prguse != null)
                Libraries.Prguse.Draw(Index + 1, DisplayLocation, Color.White, false, 0.5F);
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            Visible = true;
        }

        public void Flip()
        {
            //0,70 LOCATION
            if (Index == 1932)
            {
                Index = 1944;
                Location = new Point(0, 224);

                for (int x = 0; x < 6; x++)
                    Grid[x].Location = new Point(3, x*35 + 12);

                CloseButton.Index = 1935;
                CloseButton.HoverIndex = 1936;
                CloseButton.Location = new Point(3, 222);
                CloseButton.PressedIndex = 1937;

                RotateButton.Index = 1938;
                RotateButton.HoverIndex = 1939;
                RotateButton.Location = new Point(19, 222);
                RotateButton.PressedIndex = 1940;

            }
            else
            {
                Index = 1932;
                Location = new Point(230, Settings.ScreenHeight - 150);

                for (int x = 0; x < 6; x++)
                    Grid[x].Location = new Point(x*35 + 12, 3);

                CloseButton.Index = 1923;
                CloseButton.HoverIndex = 1924;
                CloseButton.Location = new Point(222, 19);
                CloseButton.PressedIndex = 1925;

                RotateButton.Index = 1926;
                RotateButton.HoverIndex = 1927;
                RotateButton.Location = new Point(222, 3);
                RotateButton.PressedIndex = 1928;

            }
        }


        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }
    }
    public sealed class CharacterDialog : MirImageControl
    {
        public MirButton CloseButton, CharacterButton, StatusButton, StateButton, SkillButton;
        public MirImageControl CharacterPage, StatusPage, StatePage, SkillPage, ClassImage;

        public MirLabel NameLabel, GuildLabel, LoverLabel;
        public MirLabel ACLabel, MACLabel, DCLabel, MCLabel, SCLabel, HealthLabel, ManaLabel;
        public MirLabel HeadingLabel, StatLabel;
        public MirButton NextButton, BackButton;

        public MirItemCell[] Grid;
        public MagicButton[] Magics;

        public int StartIndex;

        public CharacterDialog()
        {
            Index = 504;
            Library = Libraries.Title;
            Location = new Point(Settings.ScreenWidth - 264, 0);
            Movable = true;
            Sort = true;

            BeforeDraw += (o, e) => RefreshInferface();

            CharacterPage = new MirImageControl
                {
                    Index = 345,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(8, 90),
                };
            CharacterPage.AfterDraw += (o, e) =>
                {
                    if (Libraries.StateItems == null) return;

                    if (Grid[(int)EquipmentSlot.Armour].Item != null)
                        Libraries.StateItems.Draw(Grid[(int)EquipmentSlot.Armour].Item.Info.Image, DisplayLocation, Color.White, true, 1F);

                    if (Grid[(int)EquipmentSlot.Weapon].Item != null)
                        Libraries.StateItems.Draw(Grid[(int)EquipmentSlot.Weapon].Item.Info.Image, DisplayLocation, Color.White, true, 1F);

                    if (Grid[(int)EquipmentSlot.Helmet].Item != null)
                        Libraries.StateItems.Draw(Grid[(int)EquipmentSlot.Helmet].Item.Info.Image, DisplayLocation, Color.White, true, 1F);
                    else
                        Libraries.Prguse.Draw(440 + MapObject.User.Hair + (MapObject.User.Gender == MirGender.Male ? 0 : 40), DisplayLocation, Color.White, true, 1F);
                };

            StatusPage = new MirImageControl
                {
                    Index = 506,
                    Parent = this,
                    Library = Libraries.Title,
                    Location = new Point(8, 90),
                    Visible = false,
                };
            StatusPage.BeforeDraw += (o, e) =>
                {
                    ACLabel.Text = string.Format("{0}-{1}", MapObject.User.MinAC, MapObject.User.MaxAC);
                    MACLabel.Text = string.Format("{0}-{1}", MapObject.User.MinMAC, MapObject.User.MaxMAC);
                    DCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinDC, MapObject.User.MaxDC);
                    MCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinMC, MapObject.User.MaxMC);
                    SCLabel.Text = string.Format("{0}-{1}", MapObject.User.MinSC, MapObject.User.MaxSC);
                    HealthLabel.Text = string.Format("{0}/{1}", MapObject.User.HP, MapObject.User.MaxHP);
                    ManaLabel.Text = string.Format("{0}/{1}", MapObject.User.MP, MapObject.User.MaxMP);
                };

            StatePage = new MirImageControl
                {
                    Index = 507,
                    Parent = this,
                    Library = Libraries.Title,
                    Location = new Point(8, 90),
                    Visible = false
                };
            StatePage.BeforeDraw += (o, e) =>
                {
                    StatLabel.Text = string.Format("{0:#0.##%}\n{1}/{2}\n{3}/{4}\n{5}/{6}\n{7}\n{8}\n+{9}\n+{10}",
                                                   MapObject.User.Experience/(double) MapObject.User.MaxExperience,
                                                   MapObject.User.CurrentBagWeight, MapObject.User.MaxBagWeight,
                                                   MapObject.User.CurrentWearWeight, MapObject.User.MaxWearWeight,
                                                   MapObject.User.CurrentHandWeight, MapObject.User.MaxHandWeight,
                                                   MapObject.User.Accuracy, MapObject.User.Agility,
                                                   MapObject.User.Luck, MapObject.User.ASpeed);
                };


            SkillPage = new MirImageControl
                {
                    Index = 508,
                    Parent = this,
                    Library = Libraries.Title,
                    Location = new Point(8, 90),
                    Visible = false
                };


            CharacterButton = new MirButton
                {
                    Index = 500,
                    Library = Libraries.Title,
                    Location = new Point(8, 70),
                    Parent = this,
                    PressedIndex = 500,
                    Size = new Size(64, 20),
                    Sound = SoundList.ButtonA,
                };
            CharacterButton.Click += (o, e) => ShowCharacterPage();
            StatusButton = new MirButton
                {
                    Library = Libraries.Title,
                    Location = new Point(70, 70),
                    Parent = this,
                    PressedIndex = 501,
                    Size = new Size(64, 20),
                    Sound = SoundList.ButtonA
                };
            StatusButton.Click += (o, e) => ShowStatusPage();

            StateButton = new MirButton
                {
                    Library = Libraries.Title,
                    Location = new Point(132, 70),
                    Parent = this,
                    PressedIndex = 502,
                    Size = new Size(64, 20),
                    Sound = SoundList.ButtonA
                };
            StateButton.Click += (o, e) => ShowStatePage();

            SkillButton = new MirButton
                {
                    Library = Libraries.Title,
                    Location = new Point(194, 70),
                    Parent = this,
                    PressedIndex = 503,
                    Size = new Size(64, 20),
                    Sound = SoundList.ButtonA
                };
            SkillButton.Click += (o, e) => ShowSkillPage();

            CloseButton = new MirButton
                {
                    HoverIndex = 361,
                    Index = 360,
                    Location = new Point(241, 3),
                    Library = Libraries.Prguse2,
                    Parent = this,
                    PressedIndex = 362,
                    Sound = SoundList.ButtonA,
                };
            CloseButton.Click += (o, e) => Hide();

            NameLabel = new MirLabel
                {
                    DrawFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter,
                    Parent = this,
                    Location = new Point(50, 12),
                    Size = new Size(190, 20),
                    NotControl = true,
                };
            ClassImage = new MirImageControl
                {
                    Index = 100,
                    Library = Libraries.Prguse,
                    Location = new Point(15, 33),
                    Parent = this,
                    NotControl = true,
                };

            Grid = new MirItemCell[Enum.GetNames(typeof (EquipmentSlot)).Length];

            Grid[(int)EquipmentSlot.Weapon] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Weapon,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(125, 7),
                };


            Grid[(int)EquipmentSlot.Armour] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Armour,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(164, 7),
                };


            Grid[(int)EquipmentSlot.Helmet] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Helmet,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(203, 7),
                };



            Grid[(int)EquipmentSlot.Torch] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Torch,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(203, 134),
                };


            Grid[(int)EquipmentSlot.Necklace] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Necklace,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(203, 98),
                };


            Grid[(int)EquipmentSlot.BraceletL] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.BraceletL,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(8, 170),
                };

            Grid[(int)EquipmentSlot.BraceletR] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.BraceletR,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(203, 170),
                };

            Grid[(int)EquipmentSlot.RingL] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.RingL,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(8, 206),
                };

            Grid[(int)EquipmentSlot.RingR] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.RingR,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(203, 206),
                };


            Grid[(int)EquipmentSlot.Amulet] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Amulet,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(8, 241),
                };


            Grid[(int)EquipmentSlot.Boots] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Boots,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(47, 241),
                };

            Grid[(int)EquipmentSlot.Belt] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Belt,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(86, 241),
                };


            Grid[(int)EquipmentSlot.Stone] = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Stone,
                    GridType = MirGridType.Equipment,
                    Parent = CharacterPage,
                    Location = new Point(125, 241),
                };


            ACLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatusPage,
                    Location = new Point(105, 62),
                    NotControl = true,
                    Text = "0-0"
                };

            MACLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatusPage,
                    Location = new Point(105, 89),
                    NotControl = true,
                    Text = "0-0"
                };

            DCLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatusPage,
                    Location = new Point(105, 115),
                    NotControl = true,
                    Text = "0-0"
                };

            MCLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatusPage,
                    Location = new Point(105, 143),
                    NotControl = true,
                    Text = "0-0"
                };
            SCLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatusPage,
                    Location = new Point(105, 170),
                    NotControl = true,
                    Text = "0-0"
                };
            HealthLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatusPage,
                    Location = new Point(105, 196),
                    NotControl = true,
                    Text = "0/0"
                };
            ManaLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatusPage,
                    Location = new Point(105, 222),
                    NotControl = true,
                    Text = "0/0"
                };

            HeadingLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatePage,
                    Location = new Point(10, 59),
                    NotControl = true,
                    Text = "Experience\nBag Weight\nWear Weight\nHand Weight\nAccuracy\nAgility\nLuck\nAttack Speed"
                };
            StatLabel = new MirLabel
                {
                    AutoSize = true,
                    Parent = StatePage,
                    Location = new Point(120, 59),
                    NotControl = true,
                };

            Magics = new MagicButton[6];

            for (int i = 0; i < Magics.Length; i++)
                Magics[i] = new MagicButton {Parent = SkillPage, Visible = false, Location = new Point(8, 40 + i*33)};

            NextButton = new MirButton
            {
                Index = 396,
                Location = new Point(140, 250),
                Library = Libraries.Prguse,
                Parent = SkillPage,
                PressedIndex = 397,
                Sound = SoundList.ButtonA,
            };
            NextButton.Click += (o, e) =>
                {
                    if (StartIndex + 6 >= MapObject.User.Magics.Count) return;

                    StartIndex += 6;
                    RefreshInferface();
                };

            BackButton = new MirButton
            {
                Index = 398,
                Location = new Point(90, 250),
                Library = Libraries.Prguse,
                Parent = SkillPage,
                PressedIndex = 399,
                Sound = SoundList.ButtonA,
            };
            BackButton.Click += (o, e) =>
            {
                if (StartIndex - 6 < 0) return;

                StartIndex -= 6; 
                RefreshInferface();
            };
        }

        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }

        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }

        public void ShowCharacterPage()
        {
            CharacterPage.Visible = true;
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = 500;
            StatusButton.Index = -1;
            StateButton.Index = -1;
            SkillButton.Index = -1;
        }

        private void ShowStatusPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = true;
            StatePage.Visible = false;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = 501;
            StateButton.Index = -1;
            SkillButton.Index = -1;
        }

        private void ShowStatePage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = true;
            SkillPage.Visible = false;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = 502;
            SkillButton.Index = -1;
        }

        public void ShowSkillPage()
        {
            CharacterPage.Visible = false;
            StatusPage.Visible = false;
            StatePage.Visible = false;
            SkillPage.Visible = true;
            CharacterButton.Index = -1;
            StatusButton.Index = -1;
            StateButton.Index = -1;
            SkillButton.Index = 503;
            StartIndex = 0;
        }

        private void RefreshInferface()
        {
            int offSet = MapObject.User.Gender == MirGender.Male ? 0 : 1;

            Index = 504 + offSet;
            CharacterPage.Index = 345 + offSet;

            switch (MapObject.User.Class)
            {
                case MirClass.Warrior:
                    ClassImage.Index = 100 + offSet * 5;
                    break;
                case MirClass.Wizard:
                    ClassImage.Index = 101 + offSet * 5;
                    break;
                case MirClass.Taoist:
                    ClassImage.Index = 102 + offSet * 5;
                    break;
                case MirClass.Assassin:
                    ClassImage.Index = 103 + offSet * 5;
                    break;
            }

            NameLabel.Text = MapObject.User.Name;

            for (int i = 0; i < Magics.Length; i++)
            {
                if (i + StartIndex >= MapObject.User.Magics.Count)
                {
                    Magics[i].Visible = false;
                    continue;
                }

                Magics[i].Visible = true;
                Magics[i].Update(MapObject.User.Magics[i + StartIndex]);
            }
        }
        
        public MirItemCell GetCell(ulong id)
        {

            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }
        
    }
    public sealed class StorageDialog : MirImageControl
    {
        public MirItemCell[] Grid;
        public MirButton CloseButton;

        public StorageDialog()
        {
            Index = 586;
            Library = Libraries.Prguse;
            Location = new Point(0, 224);
            Sort = true;

            CloseButton = new MirButton
                {
                    HoverIndex = 361,
                    Index = 360,
                    Location = new Point(363, 3),
                    Library = Libraries.Prguse2,
                    Parent = this,
                    PressedIndex = 362,
                    Sound = SoundList.ButtonA,
                };
            CloseButton.Click += (o, e) => Hide();



            Grid = new MirItemCell[10*8];

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Grid[10*y + x] = new MirItemCell
                        {
                            ItemSlot = 10*y + x,
                            GridType = MirGridType.Storage,
                            Library = Libraries.Items,
                            Parent = this,
                            Location = new Point(x*36 + 9 + x, y*32 + 60 + y),
                        };
                }
            }
        }

        public void Hide()
        {
            Visible = false;
        }

        public void Show()
        {
            GameScene.Scene.InventoryDialog.Show();
            Visible = true;
        }


        public MirItemCell GetCell(ulong id)
        {
            for (int i = 0; i < Grid.Length; i++)
            {
                if (Grid[i].Item == null || Grid[i].Item.UniqueID != id) continue;
                return Grid[i];
            }
            return null;
        }
    }
    public sealed class MiniMapDialog : MirImageControl
    {
        public MirImageControl LightSetting;
        public MirButton ToggleButton, BigMapButton, MailButton;
        public MirLabel LocationLabel, MapNameLabel;
        private float _fade = 1F;
        private bool _bigMode = true;

        public MiniMapDialog()
        {
            Index = 2090;
            Library = Libraries.Prguse;
            Location = new Point(Settings.ScreenWidth - 126, 0);
            PixelDetect = true;
            Movable = true;

            BeforeDraw += MiniMap_BeforeDraw;


            MapNameLabel = new MirLabel
                {
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    Parent = this,
                    Size = new Size(120, 18),
                    Location = new Point(2, 2),
                    NotControl = true,
                };

            LocationLabel = new MirLabel
                {
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    Parent = this,
                    Size = new Size(56, 18),
                    Location = new Point(46, 131),
                    NotControl = true,
                };

            MailButton = new MirButton
                {
                    Index = 2099,
                    HoverIndex = 2100,
                    PressedIndex = 2101,
                    Parent = this,
                    Location = new Point(4, 131),
                    Library = Libraries.Prguse,
                    Sound = SoundList.ButtonA,
                };

            BigMapButton = new MirButton
                {
                    Index = 2096,
                    HoverIndex = 2097,
                    PressedIndex = 2098,
                    Parent = this,
                    Location = new Point(25, 131),
                    Library = Libraries.Prguse,
                    Sound = SoundList.ButtonA,
                };
            BigMapButton.Click += (o, e) => GameScene.Scene.BigMapDialog.Toggle(); 

            ToggleButton = new MirButton
                {
                    Index = 2102,
                    HoverIndex = 2103,
                    PressedIndex = 2104,
                    Parent = this,
                    Location = new Point(109, 3),
                    Library = Libraries.Prguse,
                    Sound = SoundList.ButtonA,
                };
            ToggleButton.Click += (o, e) => Toggle();

            LightSetting = new MirImageControl
                {
                    Index = 2093,
                    Library = Libraries.Prguse,
                    Parent = this,
                    Location = new Point(102, 131),
                };
        }

        private void MiniMap_BeforeDraw(object sender, EventArgs e)
        {
            MapControl map = GameScene.Scene.MapControl;
            if (map == null) return;

            if (map.MiniMap == 0 && Index != 2091)
                SetSmallMode();
            else if (map.MiniMap > 0 && _bigMode && Index == 2091) SetBigMode();

            if (map.MiniMap <= 0 || Index != 2090 || Libraries.MiniMap == null) return;
            Rectangle viewRect = new Rectangle(0, 0, 120, 108);
            Point drawLocation = Location;
            drawLocation.Offset(3, 22);

            Size miniMapSize = Libraries.MiniMap.GetTrueSize(map.MiniMap);
            float scaleX = miniMapSize.Width/(float) map.Width;
            float scaleY = miniMapSize.Height/(float) map.Height;

            viewRect.Location = new Point(
                (int)(scaleX * MapObject.User.CurrentLocation.X) - viewRect.Width / 2,
                (int)(scaleY * MapObject.User.CurrentLocation.Y) - viewRect.Height / 2);

            //   viewRect.Location = viewRect.Location.Subtract(1, 1);
            if (viewRect.Right >= miniMapSize.Width)
                viewRect.X = miniMapSize.Width - viewRect.Width;
            if (viewRect.Bottom >= miniMapSize.Height)
                viewRect.Y = miniMapSize.Height - viewRect.Height;

            if (viewRect.X < 0) viewRect.X = 0;
            if (viewRect.Y < 0) viewRect.Y = 0;

            Libraries.MiniMap.Draw(map.MiniMap, viewRect, drawLocation, Color.FromArgb(255,255,255), false);


            int startPointX = (int) (viewRect.X/scaleX);
            int startPointY = (int) (viewRect.Y/scaleY);

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];


                if (ob.Race == ObjectType.Item || ob.Dead || ob.Race == ObjectType.Spell) continue;
                float x = ((ob.CurrentLocation.X - startPointX) * scaleX) + drawLocation.X;
                float y = ((ob.CurrentLocation.Y - startPointY) * scaleY) + drawLocation.Y;

                Color colour;

                if ((GroupDialog.GroupList.Contains(ob.Name) && MapObject.User != ob) || ob.Name.EndsWith(string.Format("({0})", MapObject.User.Name)))
                    colour = Color.FromArgb(0, 0, 255);
                else 
                if (ob is PlayerObject)
                    colour = Color.FromArgb(255, 255, 255);
                else if (ob is NPCObject || ob.AI == 6)
                    colour = Color.FromArgb(0, 255, 50);
                else
                    colour = Color.FromArgb(255, 0, 0);

                DXManager.Sprite.Draw2D(DXManager.RadarTexture, Point.Empty, 0, new PointF((int) (x - 0.5F), (int) (y - 0.5F)), colour);
            }
        }

        public void Toggle()
        {
            if (_fade == 0F)
            {
                _bigMode = true;
                SetBigMode();
                _fade = 1F;
            }
            else
            {
                _bigMode = false;
                SetSmallMode();
                _fade = 0;
            }

            Redraw();
        }

        private void SetSmallMode()
        {
            Index = 2091;
            int y = Size.Height - 23;
            MailButton.Location = new Point(4, y);
            BigMapButton.Location = new Point(25, y);
            LocationLabel.Location = new Point(46, y);
            LightSetting.Location = new Point(102, y);
        }

        private void SetBigMode()
        {
            Index = 2090;
            int y = Size.Height - 23;
            MailButton.Location = new Point(4, y);
            BigMapButton.Location = new Point(25, y);
            LocationLabel.Location = new Point(46, y);
            LightSetting.Location = new Point(102, y);
        }

        public void Process()
        {
            MapControl map = GameScene.Scene.MapControl;
            if (map == null) return;
            MapNameLabel.Text = map.Title;
            LocationLabel.Text = Functions.PointToString(MapObject.User.CurrentLocation);
        }
    }
    public sealed class InspectDialog : MirImageControl
    {
        public static UserItem[] Items = new UserItem[14];
        public static uint InspectID;

        public string Name;
        public MirClass Class;
        public MirGender Gender;
        public byte Hair;

        public MirButton CloseButton, GroupButton, FriendButton, MailButton;
        public MirImageControl CharacterPage, ClassImage;
        public MirLabel NameLabel;
        public MirLabel GuildLabel, LoverLabel;



        public MirItemCell
            WeaponCell,
            ArmorCell,
            HelmetCell,
            TorchCell,
            NecklaceCell,
            BraceletLCell,
            BraceletRCell,
            RingLCell,
            RingRCell,
            AmuletCell,
            BeltCell,
            BootsCell,
            StoneCell;

        public InspectDialog()
        {
            Index = 430;
            Library = Libraries.Prguse;
            Location = new Point(536, 0);
            Movable = true;
            Sort = true;

            CharacterPage = new MirImageControl
                {
                    Index = 345,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(8, 90),
                };
            CharacterPage.AfterDraw += (o, e) =>
                {
                    if (Libraries.StateItems == null) return;

                    if (ArmorCell.Item != null)
                        Libraries.StateItems.Draw(ArmorCell.Item.Info.Image, DisplayLocation, Color.White, true, 1F);

                    if (WeaponCell.Item != null)
                        Libraries.StateItems.Draw(WeaponCell.Item.Info.Image, DisplayLocation, Color.White, true, 1F);

                    if (HelmetCell.Item != null)
                        Libraries.StateItems.Draw(HelmetCell.Item.Info.Image, DisplayLocation, Color.White, true, 1F);
                    else
                        Libraries.Prguse.Draw(440 + Hair + (Gender == MirGender.Male ? 0 : 40), DisplayLocation, Color.White, true, 1F);
                };


            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(241, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();



            GroupButton = new MirButton
            {
                HoverIndex = 432,
                Index = 431,
                Location = new Point(85, 379),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 433,
                Sound = SoundList.ButtonA,
            };
            GroupButton.Click += (o, e) =>
                {

                    if (GroupDialog.GroupList.Count >= Globals.MaxGroup)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Your group already has the maximum number of members.", ChatType.System);
                        return;
                    }
                    if (GroupDialog.GroupList.Count > 0 && GroupDialog.GroupList[0] != MapObject.User.Name)
                    {

                        GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                        return;
                    }

                    Network.Enqueue(new C.AddMember { Name = Name });
                };

            FriendButton = new MirButton
            {
                HoverIndex = 435,
                Index = 434,
                Location = new Point(115, 379),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 436,
                Sound = SoundList.ButtonA,
            };

            MailButton = new MirButton
            {
                HoverIndex = 438,
                Index = 437,
                Location = new Point(145, 379),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 439,
                Sound = SoundList.ButtonA,
            };

            NameLabel = new MirLabel
                {
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                    Parent = this,
                    Location = new Point(50, 12),
                    Size = new Size(190, 20),
                };
            NameLabel.Click += (o, e) =>
                {
                    GameScene.Scene.ChatDialog.ChatTextBox.SetFocus();
                    GameScene.Scene.ChatDialog.ChatTextBox.Text = string.Format("/{0} ", Name);
                    GameScene.Scene.ChatDialog.ChatTextBox.Visible = true;
                    GameScene.Scene.ChatDialog.ChatTextBox.TextBox.SelectionLength = 0;
                    GameScene.Scene.ChatDialog.ChatTextBox.TextBox.SelectionStart = Name.Length + 2;

                };
            ClassImage = new MirImageControl
                {
                    Index = 100,
                    Library = Libraries.Prguse,
                    Location = new Point(15, 33),
                    Parent = this,
                    NotControl = true,
                };


            WeaponCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Weapon,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(125, 7),
                };

            ArmorCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Armour,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(164, 7),
                };

            HelmetCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Helmet,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(203, 7),
                };


            TorchCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Torch,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(203, 134),
                };

            NecklaceCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Necklace,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(203, 98),
                };

            BraceletLCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.BraceletL,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(8, 170),
                };
            BraceletRCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.BraceletR,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(203, 170),
                };
            RingLCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.RingL,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(8, 206),
                };
            RingRCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.RingR,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(203, 206),
                };

            AmuletCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Amulet,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(8, 241),
                };

            BootsCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Boots,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(47, 241),
                };
            BeltCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Belt,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(86, 241),
                };

            StoneCell = new MirItemCell
                {
                    ItemSlot = (int) EquipmentSlot.Stone,
                    GridType = MirGridType.Inspect,
                    Parent = CharacterPage,
                    Location = new Point(125, 241),
                };
        }

        public void RefreshInferface()
        {
            int offSet = Gender == MirGender.Male ? 0 : 1;

            CharacterPage.Index = 345 + offSet;

            switch (Class)
            {
                case MirClass.Warrior:
                    ClassImage.Index = 100 + offSet*5;
                    break;
                case MirClass.Wizard:
                    ClassImage.Index = 101 + offSet*5;
                    break;
                case MirClass.Taoist:
                    ClassImage.Index = 102 + offSet*5;
                    break;
                case MirClass.Assassin:
                    ClassImage.Index = 103 + offSet*5;
                    break;
            }

            NameLabel.Text = Name;

            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i] == null) continue;
                GameScene.Bind(Items[i]);
            }
        }

        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }

        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }

    }
    public sealed class OptionDialog : MirImageControl
    {
        public MirButton SkillModeOn, SkillModeOff;
        public MirButton SkillBarOn, SkillBarOff;
        public MirButton EffectOn, EffectOff;
        public MirButton DropViewOn, DropViewOff;
        public MirButton NameViewOn, NameViewOff;
        public MirButton HPViewOn, HPViewOff;
        public MirImageControl SoundBar;
        public MirImageControl VolumeBar;

        public MirButton CloseButton;


        public OptionDialog()
        {
            Index = 411;
            Library = Libraries.Title;
            Movable = true;
            Sort = true;

            Location = new Point((Settings.ScreenWidth - Size.Width)/2, (Settings.ScreenHeight - Size.Height)/2);

            BeforeDraw += OptionPanel_BeforeDraw;

            CloseButton = new MirButton
                {
                    Index = 360,
                    HoverIndex = 361,
                    Library = Libraries.Prguse2,
                    Location = new Point(Size.Width - 26, 5),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    PressedIndex = 362,
                };
            CloseButton.Click += (o, e) => Hide();

            SkillModeOn = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(159, 68),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 451,
                };
            SkillModeOn.Click += (o, e) => Settings.SkillMode = true;

            SkillModeOff = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(201, 68),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 454
                };
            SkillModeOff.Click += (o, e) => Settings.SkillMode = false;

            SkillBarOn = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(159, 93),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 457,
                };
            SkillBarOn.Click += (o, e) => Settings.SkillBar = true;

            SkillBarOff = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(201, 93),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 460
                };
            SkillBarOff.Click += (o, e) => Settings.SkillBar = false;

            EffectOn = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(159, 118),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 457,
                };
            EffectOn.Click += (o, e) => Settings.Effect = true;

            EffectOff = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(201, 118),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 460
                };
            EffectOff.Click += (o, e) => Settings.Effect = false;

            DropViewOn = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(159, 168),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 457,
                };
            DropViewOn.Click += (o, e) => Settings.DropView = true;

            DropViewOff = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(201, 168),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 460
                };
            DropViewOff.Click += (o, e) => Settings.DropView = false;

            NameViewOn = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(159, 193),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 457,
                };
            NameViewOn.Click += (o, e) => Settings.NameView = true;

            NameViewOff = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(201, 193),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 460
                };
            NameViewOff.Click += (o, e) => Settings.NameView = false;

            HPViewOn = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(159, 218),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 463,
                };
            HPViewOn.Click += (o, e) => Settings.HPView = true;

            HPViewOff = new MirButton
                {
                    Library = Libraries.Prguse2,
                    Location = new Point(201, 218),
                    Parent = this,
                    Sound = SoundList.ButtonA,
                    Size = new Size(36, 17),
                    PressedIndex = 466
                };
            HPViewOff.Click += (o, e) => Settings.HPView = false;

            SoundBar = new MirImageControl
                {
                    Index = 468,
                    Library = Libraries.Prguse2,
                    Location = new Point(159, 142),
                    Parent = this,
                    DrawImage = false,
                };
            SoundBar.MouseDown += SoundBar_MouseMove;
            SoundBar.MouseMove += SoundBar_MouseMove;
            SoundBar.BeforeDraw += SoundBar_BeforeDraw;

            VolumeBar = new MirImageControl
                {
                    Index = 20,
                    Library = Libraries.Prguse,
                    Location = new Point(155, 141),
                    Parent = this,
                    NotControl = true,
                };

        }


        private void SoundBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || SoundBar != ActiveControl) return;

            Point p = e.Location.Subtract(SoundBar.DisplayLocation);

            byte volume = (byte) (p.X/(double) SoundBar.Size.Width*100);
            Settings.Volume = volume;


            double percent = Settings.Volume/100D;
            if (percent > 1) percent = 1;

            VolumeBar.Location = percent > 0 ? new Point(159 + (int) ((SoundBar.Size.Width - 2)*percent), 142) : new Point(159, 142);
        }


        private void SoundBar_BeforeDraw(object sender, EventArgs e)
        {
            if (SoundBar.Library == null) return;

            double percent = Settings.Volume/100D;
            if (percent > 1) percent = 1;
            if (percent > 0)
            {
                Rectangle section = new Rectangle
                    {
                        Size = new Size((int) ((SoundBar.Size.Width - 2)*percent), SoundBar.Size.Height)
                    };

                SoundBar.Library.Draw(SoundBar.Index, section, SoundBar.DisplayLocation, Color.White, false);
                VolumeBar.Location = new Point(159 + section.Size.Width, 142);
            }
            else
                VolumeBar.Location = new Point(159, 142);
        }

        private void OptionPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (Settings.SkillMode)
            {
                SkillModeOn.Index = 452;
                SkillModeOff.Index = 453;
            }
            else
            {
                SkillModeOn.Index = 450;
                SkillModeOff.Index = 455;
            }

            if (Settings.SkillBar)
            {
                SkillBarOn.Index = 458;
                SkillBarOff.Index = 459;
            }
            else
            {
                SkillBarOn.Index = 456;
                SkillBarOff.Index = 461;
            }

            if (Settings.Effect)
            {
                EffectOn.Index = 458;
                EffectOff.Index = 459;
            }
            else
            {
                EffectOn.Index = 456;
                EffectOff.Index = 461;
            }

            if (Settings.DropView)
            {
                DropViewOn.Index = 458;
                DropViewOff.Index = 459;
            }
            else
            {
                DropViewOn.Index = 456;
                DropViewOff.Index = 461;
            }

            if (Settings.NameView)
            {
                NameViewOn.Index = 458;
                NameViewOff.Index = 459;
            }
            else
            {
                NameViewOn.Index = 456;
                NameViewOff.Index = 461;
            }

            if (Settings.HPView)
            {
                HPViewOn.Index = 464;
                HPViewOff.Index = 465;
            }
            else
            {
                HPViewOn.Index = 462;
                HPViewOff.Index = 467;
            }


        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

    }
    public sealed class MenuDialog : MirImageControl
    {
        public MirButton ExitButton,
                         LogOutButton,
                         HelpButton,
                         UnkownButton1,
                         UnkownButton2,
                         UnkownButton3,
                         RideButton,
                         FishingButton,
                         FriendButton,
                         MentorButton,
                         RelationshipButton,
                         GroupButton,
                         GuildButton;

        public MenuDialog()
        {
            Index = 1963;
            Parent = GameScene.Scene;
            Library = Libraries.Prguse;
            Location = new Point(Settings.ScreenWidth - Size.Width, 224);
            Sort = true;
            Visible = false;

            ExitButton = new MirButton
                {
                    HoverIndex = 1965,
                    Index = 1964,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 12),
                    PressedIndex = 1966
                };
            ExitButton.Click += (o, e) => GameScene.Scene.QuitGame();

            LogOutButton = new MirButton
                {
                    HoverIndex = 1968,
                    Index = 1967,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 31),
                    PressedIndex = 1969
                };
            LogOutButton.Click += (o, e) => GameScene.Scene.LogOut();


            HelpButton = new MirButton
                {
                    Index = 1970,
                    HoverIndex = 1971,
                    PressedIndex = 1972,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 50),
                };
            UnkownButton1 = new MirButton
                {
                    Index = 2000,
                    HoverIndex = 2001,
                    PressedIndex = 2002,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 69),
                };
            UnkownButton2 = new MirButton
                {
                    Index = 1997,
                    HoverIndex = 1998,
                    PressedIndex = 1999,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 88),
                };
            UnkownButton3 = new MirButton
                {
                    Index = 1973,
                    HoverIndex = 1974,
                    PressedIndex = 1975,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 107),
                };
            RideButton = new MirButton
                {
                    Index = 1976,
                    HoverIndex = 1977,
                    PressedIndex = 1978,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 126),
                };
            FishingButton = new MirButton
                {
                    Index = 1979,
                    HoverIndex = 1980,
                    PressedIndex = 1981,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 145),
                };
            FriendButton = new MirButton
                {
                    Index = 1982,
                    HoverIndex = 1983,
                    PressedIndex = 1984,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 164),
                };
            MentorButton = new MirButton
                {
                    Index = 1985,
                    HoverIndex = 1986,
                    PressedIndex = 1987,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 183),
                };

            RelationshipButton = new MirButton
                {
                    Index = 1988,
                    HoverIndex = 1989,
                    PressedIndex = 1990,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 202),
                };

            GroupButton = new MirButton
                {
                    Index = 1991,
                    HoverIndex = 1992,
                    PressedIndex = 1993,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 221),
                };
              GroupButton.Click += (o, e) =>
              {
                  if (GameScene.Scene.GroupDialog.Visible)
                      GameScene.Scene.GroupDialog.Hide();
                  else GameScene.Scene.GroupDialog.Show();
              };
            GuildButton = new MirButton
                {
                    Index = 1994,
                    HoverIndex = 1995,
                    PressedIndex = 1996,
                    Parent = this,
                    Library = Libraries.Prguse,
                    Location = new Point(3, 240),
                };

        }

        public void Show()
        {
            Visible = true;
        }

        public void Hide()
        {
            Visible = false;
        }

    }
    public sealed class NPCDialog : MirImageControl
    {
        public static Regex R = new Regex(@"<(.*?/.*?)>");

        public MirButton CloseButton;
        public MirLabel[] TextLabel;
        public List<MirLabel> TextButtons;

        public NPCDialog()
        {
            Index = 995;
            Library = Libraries.Prguse;

            TextLabel = new MirLabel[12];
            TextButtons = new List<MirLabel>();

            Sort = true;

            for (int i = 0; i < TextLabel.Length; i++)
            {
                TextLabel[i] = new MirLabel
                    {
                        DrawFormat = TextFormatFlags.WordBreak,
                        Visible = true,
                        Parent = this,
                        Size = new Size(420, 18),
                        Location = new Point(10, 34 + i*18),
                    };
            }

            CloseButton = new MirButton
                {
                    HoverIndex = 361,
                    Index = 360,
                    Location = new Point(413, 3),
                    Library = Libraries.Prguse2,
                    Parent = this,
                    PressedIndex = 362,
                    Sound = SoundList.ButtonA,
                };
            CloseButton.Click += (o, e) => Hide();
        }

        public void NewText(List<string> lines)
        {
            for (int i = 0; i < TextButtons.Count; i++)
                TextButtons[i].Dispose();

            TextButtons.Clear();

            for (int i = 0; i < TextLabel.Length; i++)
            {
                if (i >= lines.Count)
                {
                    TextLabel[i].Text = string.Empty;
                    continue;
                }

                Match match = R.Match(lines[i]);

                while (match.Success)
                {
                    Capture capture = match.Groups[1].Captures[0];
                    string[] values = capture.Value.Split('/');

                    lines[i] = lines[i].Remove(capture.Index - 1, capture.Length + 2).Insert(capture.Index - 1, values[0]);
                    string text = lines[i].Substring(0, capture.Index - 1) + " ";
                    Size size = TextRenderer.MeasureText(CMain.Graphics, text, TextLabel[i].Font, TextLabel[i].Size, TextFormatFlags.TextBoxControl);

                    NewButton(values[0], values[1], TextLabel[i].Location.Add(new Point(size.Width - 10, 0)));

                    match = R.Match(lines[i]);
                }

                TextLabel[i].Text = lines[i];
            }
        }

        private void NewButton(string text, string key, Point p)
        {
            key = string.Format("[{0}]", key);

            MirLabel temp = new MirLabel
                {
                    AutoSize = true,
                    Visible = true,
                    Parent = this,
                    Location = p,
                    Text = text,
                    ForeColour = Color.Yellow,
                    Sound = SoundList.ButtonC,
                    Font = new Font(Settings.FontName, 8F, FontStyle.Underline)
                };


            temp.MouseEnter += (o, e) => temp.ForeColour = Color.Red;
            temp.MouseLeave += (o, e) => temp.ForeColour = Color.Yellow;
            temp.MouseDown += (o, e) => temp.ForeColour = Color.Yellow;
            temp.MouseUp += (o, e) => temp.ForeColour = Color.Red;

            temp.Click += (o, e) =>
                {
                    if (key == "[Exit]")
                    {
                        Hide();
                        return;
                    }

                    if (CMain.Time <= GameScene.NPCTime) return;

                    GameScene.NPCTime = CMain.Time + 5000;
                    Network.Enqueue(new C.CallNPC {ObjectID = GameScene.NPCID, Key = key});
                };
            TextButtons.Add(temp);
        }

        public void Hide()
        {
            Visible = false;
            GameScene.Scene.NPCGoodsDialog.Hide();
            GameScene.Scene.NPCDropDialog.Hide();
            /*
            GameScene.Scene.BuyBackDialog.Hide();*/
            GameScene.Scene.InventoryDialog.Location = new Point(0, 0);
            GameScene.Scene.StorageDialog.Hide();
            GameScene.Scene.TrustMerchantDialog.Hide();
        }

        public void Show()
        {
            GameScene.Scene.InventoryDialog.Location = new Point(Settings.ScreenWidth - GameScene.Scene.InventoryDialog.Size.Width, 0);
            Visible = true;
        }
    }
    public sealed class NPCGoodsDialog : MirImageControl
    {
        public int StartIndex;
        public ItemInfo SelectedItem;

        public List<ItemInfo> Goods = new List<ItemInfo>();
        public MirGoodsCell[] Cells;
        public MirButton BuyButton, CloseButton;
        public MirImageControl BuyLabel;

        public MirButton UpButton, DownButton, PositionBar;

        public NPCGoodsDialog()
        {
            Index = 1000;
            Library = Libraries.Prguse;
            Location = new Point(0, 224);
            Cells = new MirGoodsCell[8];
            Sort = true;

            for (int i = 0; i < Cells.Length; i++)
            {
                Cells[i] = new MirGoodsCell
                    {
                        Parent = this,
                        Location = new Point(10, 34 + i*33),
                        Sound = SoundList.ButtonC,
                    };
                Cells[i].Click += (o, e) =>
                    {
                        SelectedItem = ((MirGoodsCell) o).Item;
                        Update();
                    };
                Cells[i].MouseWheel += NPCGoodsPanel_MouseWheel;
                Cells[i].DoubleClick += (o, e) => BuyItem();
            }

            CloseButton = new MirButton
                {
                    HoverIndex = 361,
                    Index = 360,
                    Location = new Point(216, 3),
                    Library = Libraries.Prguse2,
                    Parent = this,
                    PressedIndex = 362,
                    Sound = SoundList.ButtonA,
                };
            CloseButton.Click += (o, e) => Hide();

            BuyButton = new MirButton
                {
                    HoverIndex = 313,
                    Index = 312,
                    Location = new Point(77, 304),
                    Library = Libraries.Title,
                    Parent = this,
                    PressedIndex = 314,
                    Sound = SoundList.ButtonA,
                };
            BuyButton.Click += (o, e) => BuyItem();

            BuyLabel = new MirImageControl
                {
                    Index = 459,
                    Library = Libraries.Title,
                    Parent = this,
                    Location = new Point(20, 6),
                };


            UpButton = new MirButton
                {
                    Index = 197,
                    HoverIndex = 198,
                    Library = Libraries.Prguse2,
                    Location = new Point(218, 35),
                    Parent = this,
                    PressedIndex = 199,
                    Sound = SoundList.ButtonA
                };
            UpButton.Click += (o, e) =>
                {
                    if (StartIndex == 0) return;
                    StartIndex--;
                    Update();
                };

            DownButton = new MirButton
                {
                    Index = 207,
                    HoverIndex = 208,
                    Library = Libraries.Prguse2,
                    Location = new Point(218, 284),
                    Parent = this,
                    PressedIndex = 209,
                    Sound = SoundList.ButtonA
                };
            DownButton.Click += (o, e) =>
                {
                    if (Goods.Count <= 8) return;

                    if (StartIndex == Goods.Count - 8) return;
                    StartIndex++;
                    Update();
                };

            PositionBar = new MirButton
                {
                    Index = 205,
                    HoverIndex = 206,
                    Library = Libraries.Prguse2,
                    Location = new Point(218, 49),
                    Parent = this,
                    PressedIndex = 206,
                    Movable = true,
                    Sound = SoundList.None
                };
            PositionBar.OnMoving += PositionBar_OnMoving;
            PositionBar.MouseUp += (o, e) => Update();
        }

        private void BuyItem()
        {
            if (SelectedItem == null) return;
            if (SelectedItem.StackSize > 1)
            {
                UserItem temp = new UserItem(SelectedItem) {Count = SelectedItem.StackSize};

                if (temp.Price() > GameScene.Gold)
                {
                    temp.Count = GameScene.Gold / SelectedItem.Price;
                    if (temp.Count == 0)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("You do no have enough Gold.", ChatType.System);
                        return;
                    }
                }

                MapObject.User.GetMaxGain(temp);

                if (temp.Count == 0) return;

                MirAmountBox amountBox = new MirAmountBox("Purchase Amount:", SelectedItem.Image, temp.Count);

                amountBox.OKButton.Click += (o, e) =>
                    {
                        if (amountBox.Amount > 0)
                        {
                            Network.Enqueue(new C.BuyItem {ItemIndex = SelectedItem.Index, Count = amountBox.Amount});
                        }
                    };

                amountBox.Show();
            }
            else
            {
                if (SelectedItem.Price > GameScene.Gold)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do no have enough Gold.", ChatType.System);
                    return;
                }

                if (SelectedItem.Weight > (MapObject.User.MaxBagWeight - MapObject.User.CurrentBagWeight))
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do no have enough weight.", ChatType.System);
                    return;
                }

                for (int i = 0; i < MapObject.User.Inventory.Length; i++)
                {
                    if (MapObject.User.Inventory[i] == null) break;
                    if (i == MapObject.User.Inventory.Length - 1)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat("Cannot purchase any more items.", ChatType.System);
                        return;
                    }
                }


                Network.Enqueue(new C.BuyItem { ItemIndex = SelectedItem.Index, Count = 1 });
            }
        }
        private void NPCGoodsPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            int count = e.Delta/SystemInformation.MouseWheelScrollDelta;

            if (StartIndex == 0 && count >= 0) return;
            if (StartIndex == Goods.Count - 1 && count <= 0) return;

            StartIndex -= count;
            Update();
        }
        private void Update()
        {
            if (StartIndex > Goods.Count - 8) StartIndex = Goods.Count - 8;
            if (StartIndex <= 0) StartIndex = 0;

            if (Goods.Count > 8)
            {
                PositionBar.Visible = true;
                int h = 233 - PositionBar.Size.Height;
                h = (int) ((h/(float) (Goods.Count - 8))*StartIndex);
                PositionBar.Location = new Point(218, 49 + h);
            }
            else
                PositionBar.Visible = false;


            for (int i = 0; i < 8; i++)
            {
                if (i + StartIndex >= Goods.Count)
                {
                    Cells[i].Visible = false;
                    continue;
                }
                Cells[i].Visible = true;

                Cells[i].Item = Goods[i + StartIndex];
                Cells[i].Border = SelectedItem != null && Cells[i].Item == SelectedItem;
            }

        }
        private void PositionBar_OnMoving(object sender, MouseEventArgs e)
        {
            const int x = 218;
            int y = PositionBar.Location.Y;
            if (y >= 282 - PositionBar.Size.Height) y = 282 - PositionBar.Size.Height;
            if (y < 49) y = 49;

            int h = 233 - PositionBar.Size.Height;
            h = (int) Math.Round(((y - 49)/(h/(float) (Goods.Count - 8))));

            PositionBar.Location = new Point(x, y);

            if (h == StartIndex) return;
            StartIndex = h;
            Update();
        }


        public void NewGoods(List<int> list)
        {
            Goods.Clear();
            StartIndex = 0;
            SelectedItem = null;

            for (int i = 0; i < list.Count; i++)
            {
                ItemInfo info = GameScene.GetInfo(list[i]);
                if (info == null) continue;
                Goods.Add(info);
            }

            Update();
        }



        public void Hide()
        {
            Visible = false;
        }
        public void Show()
        {
            GameScene.Scene.InventoryDialog.Show();
            Visible = true;
        }
    }
    public sealed class NPCDropDialog : MirImageControl
    {

        public readonly MirButton ConfirmButton, HoldButton;
        public readonly MirItemCell ItemCell;
        public MirItemCell OldCell;
        public readonly MirLabel InfoLabel;
        public PanelType PType;

        public static UserItem TargetItem;
        public bool Hold;


        public NPCDropDialog()
        {
            Index = 392;
            Library = Libraries.Prguse;
            Location = new Point(264, 224);
            Sort = true;

            Click += NPCDropPanel_Click;

            HoldButton = new MirButton
            {
                HoverIndex = 294,
                Index = 293,
                Location = new Point(114, 36),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 295,
                Sound = SoundList.ButtonA,
            };
            HoldButton.Click += (o, e) => Hold = !Hold;

            ConfirmButton = new MirButton
            {
                HoverIndex = 291,
                Index = 290,
                Location = new Point(114, 62),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 292,
                Sound = SoundList.ButtonA,
            };
            ConfirmButton.Click += (o, e) => Confirm();

            InfoLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(30, 10),
                Parent = this,
                NotControl = true,
            };

            ItemCell = new MirItemCell
            {
                BorderColour = Color.Lime,
                GridType = MirGridType.DropPanel,
                Library = Libraries.Items,
                Parent = this,
                Location = new Point(38, 72),
            };
            ItemCell.Click += (o, e) => ItemCell_Click();

            BeforeDraw += NPCDropPanel_BeforeDraw;
            AfterDraw += NPCDropPanel_AfterDraw;
        }

        private void NPCDropPanel_AfterDraw(object sender, EventArgs e)
        {
            if (Hold)
                Libraries.Prguse.Draw(403, 91 + DisplayLocation.X, 39 + DisplayLocation.Y);
        }

        private void NPCDropPanel_Click(object sender, EventArgs e)
        {
            MouseEventArgs me = e as MouseEventArgs;

            if (me == null) return;
            int x = me.X - DisplayLocation.X;
            int y = me.Y - DisplayLocation.Y;

            if (new Rectangle(20, 55, 75, 75).Contains(x, y))
                ItemCell_Click();
        }

        private void Confirm()
        {
            switch (PType)
            {
                case PanelType.Sell:
                    if (GameScene.Gold + TargetItem.Price() / 2 <= uint.MaxValue)
                    {
                        Network.Enqueue(new C.SellItem { UniqueID = TargetItem.UniqueID, Count = TargetItem.Count });
                        TargetItem = null;
                        return;
                    }
                    GameScene.Scene.ChatDialog.ReceiveChat("Cannot carry anymore gold.", ChatType.System);
                    break;
                case PanelType.Repair:
                    if (GameScene.Gold >= TargetItem.RepairPrice()*GameScene.NPCRate)
                    {
                        Network.Enqueue(new C.RepairItem { UniqueID = TargetItem.UniqueID });
                        TargetItem = null;
                        return;
                    }
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough gold.", ChatType.System);
                    break;
                case PanelType.SpecialRepair:
                    if (GameScene.Gold >= (TargetItem.RepairPrice() * 3) * GameScene.NPCRate)
                    {
                        Network.Enqueue(new C.SRepairItem { UniqueID = TargetItem.UniqueID });
                        TargetItem = null;
                        return;
                    }
                        GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough gold.", ChatType.System);
                    break;
                case PanelType.Consign:
                    MirAmountBox box = new MirAmountBox("Consignment Price:", TargetItem.Info.Image, Globals.MaxConsignment, Globals.MinConsignment)
                        {
                            InputTextBox = {Text = string.Empty},
                            Amount = 0
                        };

                    box.Show();
                    box.OKButton.Click += (o, e) =>
                        {
                            Network.Enqueue(new C.ConsignItem {UniqueID = TargetItem.UniqueID, Price = box.Amount});
                            TargetItem = null;
                        };
                    return;
            }


            TargetItem = null;
            OldCell.Locked = false;
            OldCell = null;
        }

        private void ItemCell_Click()
        {
            if (OldCell != null)
            {
                OldCell.Locked = false;
                TargetItem = null;
                OldCell = null;
            }

            if (GameScene.SelectedCell == null || GameScene.SelectedCell.GridType != MirGridType.Inventory ||
                (PType != PanelType.Sell && PType != PanelType.Consign && GameScene.SelectedCell.Item != null && GameScene.SelectedCell.Item.Info.Durability == 0))
                return;

            if (GameScene.SelectedCell.Item != null && (GameScene.SelectedCell.Item.Info.StackSize > 1 && GameScene.SelectedCell.Item.Count > 1))
            {
                MirAmountBox amountBox = new MirAmountBox("Sell Amount:", GameScene.SelectedCell.Item.Info.Image, GameScene.SelectedCell.Item.Count);

                amountBox.OKButton.Click += (o, a) =>
                {
                    TargetItem = GameScene.SelectedCell.Item.Clone();
                    TargetItem.Count = amountBox.Amount;

                    OldCell = GameScene.SelectedCell;
                    OldCell.Locked = true;
                    GameScene.SelectedCell = null;
                    if (Hold) Confirm();
                };

                amountBox.Show();
            }
            else
            {
                TargetItem = GameScene.SelectedCell.Item;
                OldCell = GameScene.SelectedCell;
                OldCell.Locked = true;
                GameScene.SelectedCell = null;
                if (Hold) Confirm();
            }

        }

        private void NPCDropPanel_BeforeDraw(object sender, EventArgs e)
        {
            string text;

            switch (PType)
            {
                case PanelType.Sell:

                    text = "Sale: ";
                    break;
                case PanelType.Repair:
                    text = "Repair: ";
                    break;
                case PanelType.SpecialRepair:
                    text = "S. Repair: ";
                    break;
                case PanelType.Consign:
                    InfoLabel.Text = "Consignment: ";
                    return;
                default: return;
                    
            }

            if (TargetItem != null)
            {

                switch (PType)
                {
                    case PanelType.Sell:
                        text += (TargetItem.Price() / 2).ToString();
                        break;
                    case PanelType.Repair:
                        text += (TargetItem.RepairPrice() * GameScene.NPCRate).ToString();
                        break;
                    case PanelType.SpecialRepair:
                        text += ((TargetItem.RepairPrice() * 3) * GameScene.NPCRate).ToString();
                        break;
                    default: return;
                }

                text += " Gold";
            }

            InfoLabel.Text = text;
        }

        public void Hide()
        {
            if (OldCell != null)
            {
                OldCell.Locked = false;
                TargetItem = null;
                OldCell = null;
            }
            Visible = false;
        }
        public void Show()
        {
            Hold = false;
            GameScene.Scene.InventoryDialog.Show();
            Visible = true;
        }
    }
    public sealed class MagicButton : MirControl
    {
        public MirImageControl LevelImage, ExpImage;
        public MirButton SkillButton;
        public MirLabel LevelLabel, NameLabel, ExpLabel, KeyLabel;
        public ClientMagic Magic;

        public MagicButton()
        {
            Size = new Size(231, 33);

            SkillButton = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.MagIcon2,
                Parent = this,
                Location = new Point(36, 0),
                Sound = SoundList.ButtonA,
            };
            SkillButton.Click += (o, e) => new AssignKeyPanel(Magic);

            LevelImage = new MirImageControl
            {
                Index = 516,
                Library = Libraries.Title,
                Location = new Point(73, 7),
                Parent = this,
                NotControl = true,
            };

            ExpImage = new MirImageControl
            {
                Index = 517,
                Library = Libraries.Title,
                Location = new Point(73, 19),
                Parent = this,
                NotControl = true,
            };

            LevelLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(88, 2),
                NotControl = true,
            };

            NameLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 2),
                NotControl = true,
            };

            ExpLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(109, 15),
                NotControl = true,
            };

            KeyLabel = new MirLabel
            {
                AutoSize = true,
                Parent = this,
                Location = new Point(2, 2),
                NotControl = true,
            };

        }
        public void Update(ClientMagic magic)
        {
            Magic = magic;

            NameLabel.Text = Magic.Spell.ToString();

            LevelLabel.Text = Magic.Level.ToString();
            switch (Magic.Level)
            {
                case 0:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need1);
                    break;
                case 1:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need2);
                    break;
                case 2:
                    ExpLabel.Text = string.Format("{0}/{1}", Magic.Experience, Magic.Need3);
                    break;
                case 3:
                    ExpLabel.Text = "-";
                    break;
            }

            if (Magic.Key > 8)
                KeyLabel.Text = string.Format("CTRL" + Environment.NewLine + "F{0}", Magic.Key % 8);
            else if (Magic.Key > 0)
                KeyLabel.Text = string.Format("F{0}", Magic.Key);
            else
                KeyLabel.Text = string.Empty;


            SkillButton.Index = Magic.Icon * 2;
            SkillButton.PressedIndex = Magic.Icon * 2 + 1;
        }
    }
    public sealed class AssignKeyPanel : MirImageControl
    {
        public MirButton SaveButton, NoneButton;

        public MirLabel TitleLabel;
        public MirImageControl MagicImage;
        public MirButton[] FKeys;

        public ClientMagic Magic;
        public byte Key;

        public AssignKeyPanel(ClientMagic magic)
        {
            Magic = magic;
            Key = magic.Key;

            Modal = true;
            Index = 710;
            Library = Libraries.Prguse;
            Location = new Point((800 - Size.Width) / 2, (600 - Size.Height) / 2);
            Parent = GameScene.Scene;
            Visible = true;

            MagicImage = new MirImageControl
            {
                Location = new Point(16, 16),
                Index = magic.Icon * 2,
                Library = Libraries.MagIcon2,
                Parent = this,
            };

            TitleLabel = new MirLabel
                {
                    Location = new Point(49, 17),
                    Parent = this,
                    Size = new Size(230, 32),
                    DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak,
                    Text = string.Format("Select the Key for: {0}", magic.Spell)
                };

            NoneButton = new MirButton
            {
                Library = Libraries.Title,
                Parent = this,
                Location = new Point(284, 64),
                Index = 153,
                HoverIndex = 154,
                PressedIndex = 155,
            };
            NoneButton.Click += (o, e) => Key = 0;

            SaveButton = new MirButton
            {
                Library = Libraries.Title,
                Parent = this,
                Location = new Point(284, 101),
                Index = 156,
                HoverIndex = 157,
                PressedIndex = 158,
            };
            SaveButton.Click += (o, e) =>
            {
                for (int i = 0; i < MapObject.User.Magics.Count; i++)
                {
                    if (MapObject.User.Magics[i].Key == Key)
                        MapObject.User.Magics[i].Key = 0;
                }

                Network.Enqueue(new C.MagicKey { Spell = Magic.Spell, Key = Key });
                Magic.Key = Key;
                Dispose();
            };


            FKeys = new MirButton[16];

            FKeys[0] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(17, 58),
                Sound = SoundList.ButtonA,
                Text = "F1"
            };
            FKeys[0].Click += (o, e) => Key = 1;

            FKeys[1] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(49, 58),
                Sound = SoundList.ButtonA,
                Text = "F2"
            };
            FKeys[1].Click += (o, e) => Key = 2;

            FKeys[2] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(81, 58),
                Sound = SoundList.ButtonA,
                Text = "F3"
            };
            FKeys[2].Click += (o, e) => Key = 3;

            FKeys[3] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(113, 58),
                Sound = SoundList.ButtonA,
                Text = "F4"
            };
            FKeys[3].Click += (o, e) => Key = 4;

            FKeys[4] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(150, 58),
                Sound = SoundList.ButtonA,
                Text = "F5"
            };
            FKeys[4].Click += (o, e) => Key = 5;

            FKeys[5] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(182, 58),
                Sound = SoundList.ButtonA,
                Text = "F6",
            };
            FKeys[5].Click += (o, e) => Key = 6;

            FKeys[6] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(214, 58),
                Sound = SoundList.ButtonA,
                Text = "F7"
            };
            FKeys[6].Click += (o, e) => Key = 7;

            FKeys[7] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(246, 58),
                Sound = SoundList.ButtonA,
                Text = "F8"
            };
            FKeys[7].Click += (o, e) => Key = 8;


            FKeys[8] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(17, 95),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F1"
            };
            FKeys[8].Click += (o, e) => Key = 9;

            FKeys[9] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(49, 95),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F2"
            };
            FKeys[9].Click += (o, e) => Key = 10;

            FKeys[10] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(81, 95),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F3"
            };
            FKeys[10].Click += (o, e) => Key = 11;

            FKeys[11] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(113, 95),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F4"
            };
            FKeys[11].Click += (o, e) => Key = 12;

            FKeys[12] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(150, 95),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F5"
            };
            FKeys[12].Click += (o, e) => Key = 13;

            FKeys[13] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(182, 95),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F6"
            };
            FKeys[13].Click += (o, e) => Key = 14;

            FKeys[14] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(214, 95),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F7"
            };
            FKeys[14].Click += (o, e) => Key = 15;

            FKeys[15] = new MirButton
            {
                Index = 0,
                PressedIndex = 1,
                Library = Libraries.Prguse,
                Parent = this,
                Location = new Point(246, 95),
                Sound = SoundList.ButtonA,
                Text = "Ctrl" + Environment.NewLine + "F8"
            };
            FKeys[15].Click += (o, e) => Key = 16;

            BeforeDraw += AssignKeyPanel_BeforeDraw;
        }

        private void AssignKeyPanel_BeforeDraw(object sender, EventArgs e)
        {
            for (int i = 0; i < FKeys.Length; i++)
            {
                FKeys[i].Index = 1656;
                FKeys[i].HoverIndex = 1657;
                FKeys[i].PressedIndex = 1658;
                FKeys[i].Visible = true;
            }

            if (Key == 0 || Key > FKeys.Length) return;

            FKeys[Key - 1].Index = 1658;
            FKeys[Key - 1].HoverIndex = 1658;
            FKeys[Key - 1].PressedIndex = 1658;
        }
    }
    public sealed class GroupDialog : MirImageControl
    {
        public static bool AllowGroup;
        public static List<string> GroupList = new List<string>();

        public MirImageControl TitleLabel;
        public MirButton SwitchButton, CloseButton, AddButton, DelButton;
        public MirLabel[] GroupMembers;

        public GroupDialog()
        {
            Index = 120;
            Library = Libraries.Prguse;
            Movable = true;
            Sort = true;
            Location = new Point((800 - Size.Width) / 2, (600 - Size.Height) / 2);

            GroupMembers = new MirLabel[Globals.MaxGroup];

            GroupMembers[0] = new MirLabel
            {
                AutoSize = true,
                Location = new Point(16, 33),
                Parent = this,
                NotControl = true,
            };

            for (int i = 1; i < GroupMembers.Length; i++)
            {
                GroupMembers[i] = new MirLabel
                {
                    AutoSize = true,
                    Location = new Point(((i + 1) % 2) * 100 + 16, 55 + ((i - 1) / 2) * 20),
                    Parent = this,
                    NotControl = true,
                };
            }



            TitleLabel = new MirImageControl
            {
                Index = 5,
                Library = Libraries.Title,
                Location = new Point(18, 4),
                Parent = this
            };

            CloseButton = new MirButton
            {
                HoverIndex = 361,
                Index = 360,
                Location = new Point(206, 3),
                Library = Libraries.Prguse2,
                Parent = this,
                PressedIndex = 362,
                Sound = SoundList.ButtonA,
            };
            CloseButton.Click += (o, e) => Hide();


            SwitchButton = new MirButton
            {
                HoverIndex = 115,
                Index = 114,
                Location = new Point(25, 219),
                Library = Libraries.Prguse,
                Parent = this,
                PressedIndex = 116,
                Sound = SoundList.ButtonA,
            };
            SwitchButton.Click += (o, e) => Network.Enqueue(new C.SwitchGroup { AllowGroup = !AllowGroup });

            AddButton = new MirButton
            {
                HoverIndex = 134,
                Index = 133,
                Location = new Point(70, 219),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 135,
                Sound = SoundList.ButtonA,
            };
            AddButton.Click += (o, e) => AddMember();

            DelButton = new MirButton
            {
                HoverIndex = 137,
                Index = 136,
                Location = new Point(140, 219),
                Library = Libraries.Title,
                Parent = this,
                PressedIndex = 138,
                Sound = SoundList.ButtonA,
            };
            DelButton.Click += (o, e) => DelMember();

            BeforeDraw += GroupPanel_BeforeDraw;
        }

        private void GroupPanel_BeforeDraw(object sender, EventArgs e)
        {
            if (GroupList.Count == 0)
            {
                AddButton.Index = 130;
                AddButton.HoverIndex = 131;
                AddButton.PressedIndex = 132;
            }
            else
            {
                AddButton.Index = 133;
                AddButton.HoverIndex = 134;
                AddButton.PressedIndex = 135;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {
                AddButton.Visible = false;
                DelButton.Visible = false;
            }
            else
            {
                AddButton.Visible = true;
                DelButton.Visible = true;
            }

            if (AllowGroup)
            {
                SwitchButton.Index = 117;
                SwitchButton.HoverIndex = 118;
                SwitchButton.PressedIndex = 119;
            }
            else
            {
                SwitchButton.Index = 114;
                SwitchButton.HoverIndex = 115;
                SwitchButton.PressedIndex = 116;
            }

            for (int i = 0; i < GroupMembers.Length; i++)
                GroupMembers[i].Text = i >= GroupList.Count ? string.Empty : GroupList[i];
        }

        private void AddMember()
        {
            if (GroupList.Count >= Globals.MaxGroup)
            {
                GameScene.Scene.ChatDialog.ReceiveChat("Your group already has the maximum number of members.", ChatType.System);
                return;
            }
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to group.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.AddMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }
        private void DelMember()
        {
            if (GroupList.Count > 0 && GroupList[0] != MapObject.User.Name)
            {

                GameScene.Scene.ChatDialog.ReceiveChat("You are not the leader of your group.", ChatType.System);
                return;
            }

            MirInputBox inputBox = new MirInputBox("Please enter the name of the person you wish to group.");

            inputBox.OKButton.Click += (o, e) =>
            {
                Network.Enqueue(new C.DelMember { Name = inputBox.InputTextBox.Text });
                inputBox.Dispose();
            };
            inputBox.Show();
        }


        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }
    }
    public sealed class BigMapDialog : MirControl
    {
        public BigMapDialog()
        {
            NotControl = true;
            Location = new Point(130, 100);
            Border = true;
            BorderColour = Color.Lime;
            BeforeDraw += (o, e) => OnBeforeDraw();
            Sort = true;
        }

        private void OnBeforeDraw()
        {
            MapControl map = GameScene.Scene.MapControl;
            if (map == null) return;

            int index = map.BigMap <= 0 ? map.MiniMap : map.BigMap;
            if (index <= 0) return;
            TrySort();

            Rectangle viewRect = new Rectangle(0, 0, 600, 400);

            Size = Libraries.MiniMap.GetSize(index);

            if (Size.Width < 600)
                viewRect.Width = Size.Width;

            if (Size.Height < 400)
                viewRect.Height = Size.Height;

            viewRect.X = (Settings.ScreenWidth - viewRect.Width) / 2;
            viewRect.Y = (Settings.ScreenHeight - viewRect.Height) / 2;

            Location = viewRect.Location;
            Size = viewRect.Size;
            
            float scaleX = Size.Width / (float)map.Width;
            float scaleY = Size.Height / (float)map.Height;

            viewRect.Location = new Point(
                (int)(scaleX * MapObject.User.CurrentLocation.X) - viewRect.Width / 2,
                (int)(scaleY * MapObject.User.CurrentLocation.Y) - viewRect.Height / 2);

            if (viewRect.Right >= Size.Width)
                viewRect.X = Size.Width - viewRect.Width;
            if (viewRect.Bottom >= Size.Height)
                viewRect.Y = Size.Height - viewRect.Height;

            if (viewRect.X < 0) viewRect.X = 0;
            if (viewRect.Y < 0) viewRect.Y = 0;

            Libraries.MiniMap.Draw(index, Location, Size, Color.FromArgb(255, 255, 255));

            int startPointX = (int)(viewRect.X / scaleX);
            int startPointY = (int)(viewRect.Y / scaleY);

            for (int i = MapControl.Objects.Count - 1; i >= 0; i--)
            {
                MapObject ob = MapControl.Objects[i];


                if (ob.Race == ObjectType.Item || ob.Dead || ob.Race == ObjectType.Spell) continue;
                float x = ((ob.CurrentLocation.X - startPointX) * scaleX) + Location.X;
                float y = ((ob.CurrentLocation.Y - startPointY) * scaleY) + Location.Y;

                Color colour;

                if ((GroupDialog.GroupList.Contains(ob.Name) && MapObject.User != ob) || ob.Name.EndsWith(string.Format("({0})", MapObject.User.Name)))
                    colour = Color.FromArgb(0, 0, 255);
                else
                    if (ob is PlayerObject)
                        colour = Color.FromArgb(255, 255, 255);
                    else if (ob is NPCObject || ob.AI == 6)
                        colour = Color.FromArgb(0, 255, 50);
                    else
                        colour = Color.FromArgb(255, 0, 0);

                DXManager.Sprite.Draw2D(DXManager.RadarTexture, Point.Empty, 0, new PointF((int)(x - 0.5F), (int)(y - 0.5F)), colour);
            }
        }


        public void Toggle()
        {
            Visible = !Visible;

            Redraw();
        }
    }
    public sealed class TrustMerchantDialog : MirImageControl
    {
        public static bool UserMode = false;

        public static long SearchTime, MarketTime; 

        public MirTextBox SearchTextBox;
        public MirButton FindButton, RefreshButton, MailButton, BuyButton, CloseButton, NextButton, BackButton;
        public MirLabel ItemLabel, PriceLabel, SellerLabel, PageLabel;
        public MirLabel DateLabel, ExpireLabel;
        public MirLabel NameLabel, TotalPriceLabel, SplitPriceLabel;

        public MirItemCell ItemCell;

        public List<ClientAuction> Listings = new List<ClientAuction>();


        public int Page, PageCount;
        public static AuctionRow Selected;
        public AuctionRow[] Rows = new AuctionRow[10]; 

        public TrustMerchantDialog()
        {
            Index = 670;
            Library = Libraries.Prguse;
            Sort = true;

            SearchTextBox = new MirTextBox
                {
                    Location = new Point(19, 329),
                    Parent = this,
                    Size = new Size(104, 15),
                    MaxLength = 20,
                    CanLoseFocus = true
                };
            SearchTextBox.TextBox.KeyPress += SearchTextBox_KeyPress;
            SearchTextBox.TextBox.KeyUp += SearchTextBox_KeyUp;
            SearchTextBox.TextBox.KeyDown += SearchTextBox_KeyDown;

            FindButton = new MirButton
                {
                    HoverIndex = 481,
                    Index = 480,
                    Location = new Point(130, 325),
                    Library = Libraries.Title,
                    Parent = this,
                    PressedIndex = 482,
                    Sound = SoundList.ButtonA,
                };
            FindButton.Click += (o, e) =>
                {
                    if (string.IsNullOrEmpty(SearchTextBox.Text)) return;
                    if (CMain.Time < SearchTime)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat(string.Format("You can search again after {0} seconds.", Math.Ceiling((SearchTime - CMain.Time) / 1000D)), ChatType.System);
                        return;
                    }

                    SearchTime = CMain.Time + Globals.SearchDelay;
                    Network.Enqueue(new C.MarketSearch
                    {
                        Match = SearchTextBox.Text,
                    });
                };

            RefreshButton = new MirButton
                {
                    HoverIndex = 664,
                    Index = 663,
                    Location = new Point(190, 325),
                    Library = Libraries.Prguse,
                    Parent = this,
                    PressedIndex = 665,
                    Sound = SoundList.ButtonA,
                };
            RefreshButton.Click += (o, e) =>
                {
                    if (CMain.Time < SearchTime)
                    {
                        GameScene.Scene.ChatDialog.ReceiveChat(string.Format("You can search again after {0} seconds.", Math.Ceiling((SearchTime - CMain.Time)/1000D)), ChatType.System);
                        return;
                    }
                    SearchTime = CMain.Time + Globals.SearchDelay;
                    SearchTextBox.Text = string.Empty;
                    Network.Enqueue(new C.MarketRefresh());
                };


            MailButton = new MirButton
                {
                    HoverIndex = 667,
                    Index = 666,
                    Location = new Point(225, 325),
                    Library = Libraries.Prguse,
                    Parent = this,
                    PressedIndex = 668,
                    Sound = SoundList.ButtonA,
                    Visible = false
                };

            BuyButton = new MirButton
                {
                    HoverIndex = 484,
                    Index = 483,
                    Location = new Point(400, 325),
                    Library = Libraries.Title,
                    Parent = this,
                    PressedIndex = 485,
                    Sound = SoundList.ButtonA,
                };
            BuyButton.Click += (o, e) =>
                {
                    if (Selected == null || CMain.Time < MarketTime) return;

                    if (UserMode)
                    {
                        if (Selected.Listing.Seller == "For Sale")
                        {
                            MirMessageBox box = new MirMessageBox(string.Format("{0} has not sold, Are you sure you want to get it back?", Selected.Listing.Item.Name), MirMessageBoxButtons.YesNo);
                            box.YesButton.Click += (o1, e2) =>
                                {
                                    MarketTime = CMain.Time + 3000;
                                    Network.Enqueue(new C.MarketGetBack {AuctionID = Selected.Listing.AuctionID});
                                };
                            box.Show();
                        }
                        else
                        {
                            MarketTime = CMain.Time + 3000;
                            Network.Enqueue(new C.MarketGetBack { AuctionID = Selected.Listing.AuctionID });
                        }

                    }
                    else
                    {
                        MirMessageBox box = new MirMessageBox(string.Format("Are you sure you want to buy {0} for {1}?", Selected.Listing.Item.Name, Selected.Listing.Price));
                        box.OKButton.Click += (o1, e2) =>
                        {
                            MarketTime = CMain.Time + 3000;
                            Network.Enqueue(new C.MarketBuy { AuctionID = Selected.Listing.AuctionID });
                        };
                        box.Show();
                    }
                };


            BackButton = new MirButton
                {
                    Index = 398,
                    Location = new Point(189, 298),
                    Library = Libraries.Prguse,
                    Parent = this,
                    PressedIndex = 399,
                    Sound = SoundList.ButtonA,
                };
            BackButton.Click += (o, e) =>
                {
                    if (Page <= 0) return;

                    Page--;
                    UpdateInterface();
                };

            NextButton = new MirButton
                {
                    Index = 396,
                    Location = new Point(283, 298),
                    Library = Libraries.Prguse,
                    Parent = this,
                    PressedIndex = 397,
                    Sound = SoundList.ButtonA,
                };
            NextButton.Click += (o, e) =>
            {
                if (Page >= PageCount - 1) return;
                if (Page < (Listings.Count - 1)/10)
                {
                    Page++;
                    UpdateInterface();
                    return;
                }

                Network.Enqueue(new C.MarketPage {Page = Page + 1});

            };

            CloseButton = new MirButton
                {
                    HoverIndex = 361,
                    Index = 360,
                    Location = new Point(462, 3),
                    Library = Libraries.Prguse2,
                    Parent = this,
                    PressedIndex = 362,
                    Sound = SoundList.ButtonA,
                };
            CloseButton.Click += (o, e) => Hide();

            PageLabel = new MirLabel
            {
                Location = new Point(207, 298),
                Size = new Size(70, 18),
                DrawFormat = TextFormatFlags.HorizontalCenter,
                Parent = this,
                NotControl = true,
                Text = "0/0",
            };


            NameLabel = new MirLabel
            {
                AutoSize = true,
                ForeColour = Color.Yellow,
                Location = new Point(20, 240),
                Parent = this,
                NotControl = true,
            };
            TotalPriceLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(20, 256),
                Parent = this,
                NotControl = true,
            };
            SplitPriceLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(20, 272),
                Parent = this,
                NotControl = true,
            };

            DateLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(250, 245),
                Parent = this,
                NotControl = true,
                Text = "Start Date:"
            };

            ExpireLabel = new MirLabel
            {
                AutoSize = true,
                Location = new Point(250, 265),
                Parent = this,
                NotControl = true,
                Text = "Expire Date:"
            };

            ItemLabel = new MirLabel
            {
                Location = new Point(7, 32),
                Size = new Size(142, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Text = "Item",
            };

            PriceLabel = new MirLabel
            {
                Location = new Point(148, 32),
                Size = new Size(180, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Text = "Price",
            };

            SellerLabel = new MirLabel
            {
                Location = new Point(327, 32),
                Size = new Size(150, 22),
                DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                Parent = this,
                NotControl = true,
                Text = "Seller",
            };

            for (int i = 0; i < Rows.Length; i++)
            {
                Rows[i]= new AuctionRow
                    {
                        Location = new Point(8, 54 + i*18),
                        Parent = this
                    };
                Rows[i].Click += (o, e) =>
                    {
                        Selected = (AuctionRow) o;
                        UpdateInterface();
                    };
            }


            ItemCell = new MirItemCell
                {
                    ItemSlot = 0,
                    GridType = MirGridType.TrustMerchant,
                    Library = Libraries.Items,
                    Parent = this,
                    Location = new Point(195, 248),
                };
        }

        public void UpdateInterface()
        {
            SellerLabel.Text = UserMode ? "Status" : "Seller";
            BuyButton.Index = UserMode ? 400 : 483;
            BuyButton.HoverIndex = UserMode ? 401 : 484;
            BuyButton.PressedIndex = UserMode ? 402 : 485;

            PageLabel.Text = string.Format("{0}/{1}", Page + 1, PageCount);

            for (int i = 0; i < 10; i++)
                if (i + Page*10 >= Listings.Count)
                {
                    Rows[i].Clear();
                    if (Rows[i] == Selected) Selected = null;
                }
                else
                {
                    if (Rows[i] == Selected && Selected.Listing != Listings[i + Page*10])
                    {
                        Selected.Border = false;
                        Selected = null;
                    }

                    Rows[i].Update(Listings[i + Page*10]);
                }

            for (int i = 0; i < Rows.Length; i++)
                Rows[i].Border = Rows[i] == Selected;



            NameLabel.Visible = Selected != null;
            TotalPriceLabel.Visible = Selected != null;
            SplitPriceLabel.Visible = Selected != null && Selected.Listing.Item.Count > 1;

            DateLabel.Visible = Selected != null;
            ExpireLabel.Visible = Selected != null;

            if (Selected == null) return;

            NameLabel.Text = Selected.Listing.Item.Name;

            TotalPriceLabel.Text = string.Format("Price: {0:#,##0}", Selected.Listing.Price);
            SplitPriceLabel.Text = string.Format("Each: {0:#,##0.#}", Selected.Listing.Price/(float) Selected.Listing.Item.Count);

            DateLabel.Text = string.Format("Start Date: {0}", Selected.Listing.ConsignmentDate);
            ExpireLabel.Text = string.Format("Finish Date: {0}", Selected.Listing.ConsignmentDate.AddDays(Globals.ConsignmentLength));

        }
        private void SearchTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (CMain.Time < SearchTime)
            {
                GameScene.Scene.ChatDialog.ReceiveChat(string.Format("You can search again after {0} seconds.", Math.Ceiling((SearchTime - CMain.Time)/1000D)), ChatType.System);
                return;
            }

            switch (e.KeyChar)
            {
                case (char) Keys.Enter:
                    e.Handled = true;
                    if (string.IsNullOrEmpty(SearchTextBox.Text)) return;
                    SearchTime = CMain.Time + Globals.SearchDelay;
                    Network.Enqueue(new C.MarketSearch
                        {
                            Match = SearchTextBox.Text,
                        });
                    Program.Form.ActiveControl = null;
                    break;
                case (char) Keys.Escape:
                    e.Handled = true;
                    break;
            }
        }


        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                case Keys.Escape:
                    CMain.CMain_KeyUp(sender, e);
                    break;

            }
        }

        private void SearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            CMain.Shift = e.Shift;
            CMain.Alt = e.Alt;
            CMain.Ctrl = e.Control;

            switch (e.KeyCode)
            {
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.Tab:
                case Keys.Escape:
                    CMain.CMain_KeyDown(sender, e);
                    break;

            }
        }

        
        public void Hide()
        {
            if (!Visible) return;
            Visible = false;
        }
        public void Show()
        {
            if (Visible) return;
            Visible = true;
        }

        public sealed class AuctionRow : MirControl
        {
            public ClientAuction Listing;
            public MirLabel NameLabel, PriceLabel, SellerLabel;

            public AuctionRow()
            {
                Sound = SoundList.ButtonA;

                Size = new Size(468, 17);
                BorderColour = Color.Lime;

                NameLabel = new MirLabel
                    {
                        Size = new Size(140, 17),
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                        Parent = this,
                        NotControl = true,
                    };
                PriceLabel = new MirLabel
                    {
                        Location = new Point(141, 0),
                        Size = new Size(178, 17),
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                        Parent = this,
                        NotControl = true,
                    };

                SellerLabel = new MirLabel
                    {
                        Location = new Point(320),
                        Size = new Size(148, 17),
                        DrawFormat = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter,
                        Parent = this,
                        NotControl = true,
                    };

            }

            public void Clear()
            {
                Visible = false;
                NameLabel.Text = string.Empty;
                PriceLabel.Text = string.Empty;
                SellerLabel.Text = string.Empty;
            }
            public void Update(ClientAuction listing)
            {
                Listing = listing;
                NameLabel.Text = Listing.Item.Name;
                PriceLabel.Text = Listing.Price.ToString("###,###,##0");

                NameLabel.ForeColour = Listing.Item.IsAdded ? Color.Cyan : Color.White;

                if (Listing.Price > 10000000) //10Mil
                    PriceLabel.ForeColour = Color.Red;
                else if (listing.Price > 1000000) //1Million
                    PriceLabel.ForeColour = Color.Orange;
                else if (listing.Price > 100000) //1Million
                    PriceLabel.ForeColour = Color.Green;
                else if (listing.Price > 10000) //1Million
                    PriceLabel.ForeColour = Color.DeepSkyBlue;
                else
                    PriceLabel.ForeColour = Color.White;


                SellerLabel.Text = Listing.Seller;

                if (UserMode)
                {
                    switch (Listing.Seller)
                    {
                        case "Sold":
                            SellerLabel.ForeColour = Color.Gold;
                            break;
                        case "Expired":
                            SellerLabel.ForeColour = Color.Red;
                            break;
                        default:
                            SellerLabel.ForeColour = Color.White;
                            break;
                    }
                }
                Visible = true;
            }

            protected override void OnMouseEnter()
            {
                if (Listing == null) return;
                
                base.OnMouseEnter();
                GameScene.Scene.CreateItemLabel(Listing.Item);
            }
            protected override void OnMouseLeave()
            {
                if (Listing == null) return;

                base.OnMouseLeave();
                GameScene.Scene.DisposeItemLabel();
                GameScene.HoverItem = null;
            }
        }
    }

    public class Buff
    {
        public BuffType Type;
        public string Caster;
        public long Expire;
        public int Value;
        public bool Infinite;

        public override string ToString()
        {
            string text = string.Empty;

            switch (Type)
            {
                case BuffType.Teleport:
                    text = string.Format("Temporal Flux\nIncreases cost of next Teleport by: {0} MP.\n", (int) (MapObject.User.MaxMP*0.3F));
                    break;
                case BuffType.Hiding:
                    text = "Hiding\nInvisible to many monsters.\n";
                    break;
                case BuffType.Haste:
                    text = string.Format("Haste\nIncreases Attack Speed by: {0}.\n", Value);
                    break;
                case BuffType.LightBody:
                    text = string.Format("Light Body\nIncreases Agility by: {0}.\n", Value);
                    break;
                case BuffType.SoulShield:
                    text = string.Format("Soul Shield\nIncreases MAC by: 0-{0}.\n", Value);
                    break;
                case BuffType.BlessedArmour:
                    text = string.Format("Blessed Armour\nIncreases AC by: 0-{0}.\n", Value);
                    break;
                case BuffType.ProtectionField:
                    text = string.Format("Protection Field\nIncreases AC by: 0-{0}.\n", Value);
                    break;
                case BuffType.Rage:
                    text = string.Format("Rage\nIncreases DC by: 0-{0}.\n", Value);
                    break;
            }

            if(Infinite)
                text += string.Format("Expire: Never\nCaster: {0}", Caster);
            else
                text += string.Format("Expire: {0} (s)\nCaster: {1}", Math.Round((Expire - CMain.Time) / 1000D), Caster);

            return text;
        }
    }
}


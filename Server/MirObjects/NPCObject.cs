using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Server.MirDatabase;
using Server.MirEnvir;
using S = ServerPackets;

namespace Server.MirObjects
{
    public sealed class NPCObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Merchant; }
        }

        public const string
            MainKey = "[Main]",
            BuyKey = "[Buy]",
            SellKey = "[Sell]",
            RepairKey = "[Repair]",
            SRepairKey = "[SRepair]",
            TradeKey = "[Trade]",
            BuyBackKey = "[BuyBack]",
            StorageKey = "[Storage]",
            TypeKey = "[Types]",
            ConsignKey = "[Consign]",
            MarketKey = "[Market]",
            ConsignmentsKey = "[Consignments]";

        public static Regex Regex = new Regex(@"<.*?/(.*?)>");

        public NPCInfo Info;
        private const long TurnDelay = 10000;
        public long TurnTime;

        public List<ItemInfo> Goods = new List<ItemInfo>();
        public List<int> GoodsIndex = new List<int>();
        public List<ItemType> Types = new List<ItemType>();
        public List<NPCPage> NPCSections = new List<NPCPage>();

        public override string Name
        {
            get { return Info.Name; }
            set { throw new NotSupportedException(); }
        }

        public override int CurrentMapIndex { get; set; }

        public override Point CurrentLocation
        {
            get { return Info.Location; }
            set { throw new NotSupportedException(); }
        }

        public override MirDirection Direction { get; set; }

        public override byte Level
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override uint Health
        {
            get { throw new NotSupportedException(); }
        }

        public override uint MaxHealth
        {
            get { throw new NotSupportedException(); }
        }


        public NPCObject(NPCInfo info)
        {
            Info = info;
            NameColour = Color.Lime;

            Direction = (MirDirection) Envir.Random.Next(3);
            TurnTime = Envir.Time + Envir.Random.Next(10000);
            
            if (!Directory.Exists(Settings.NPCPath)) return;


            string fileName = Path.Combine(Settings.NPCPath, info.FileName + ".txt");
            if (File.Exists(fileName))
                ParseScript(File.ReadAllLines(fileName));
            else
                SMain.Enqueue(string.Format("File Not Found: {0}, NPC: {1}", info.FileName, info.Name));
        }

        private void ParseScript(IList<string> lines)
        {
            List<string> buttons = ParseSection(lines, MainKey);

            for (int i = 0; i < buttons.Count; i++)
            {
                string section = buttons[i];

                bool match = false;
                for (int a = 0; a < NPCSections.Count; a++)
                {
                    if (NPCSections[a].Key != section) continue;
                    match = true;
                    break;

                }

                if (match) continue;

                buttons.AddRange(ParseSection(lines, section));
            }

            ParseGoods(lines);
            ParseTypes(lines);

            for (int i = 0; i < Goods.Count; i++)
                GoodsIndex.Add(Goods[i].Index);
        }

        private List<string> ParseSection(IList<string> lines, string sectionName)
        {
            List<string>
                checks = new List<string>(),
                acts = new List<string>(),
                say = new List<string>(),
                buttons = new List<string>(),
                elseSay = new List<string>(),
                elseActs = new List<string>(),
                elseButtons = new List<string>(),
                gotoButtons = new List<string>();

            List<string> currentSay = say, currentButtons = buttons;

            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].StartsWith(sectionName)) continue;

                for (int x = i + 1; x < lines.Count; x++)
                {
                    if (string.IsNullOrEmpty(lines[x])) continue;

                    if (lines[x].StartsWith("#"))
                    {
                        switch (lines[x].Remove(0, 1).ToUpper())
                        {
                            case "IF":
                                currentSay = checks;
                                currentButtons = null;
                                continue;
                            case "SAY":
                                currentSay = say;
                                currentButtons = buttons;
                                continue;
                            case "ACT":
                                currentSay = acts;
                                currentButtons = gotoButtons;
                                continue;
                            case "ELSESAY":
                                currentSay = elseSay;
                                currentButtons = elseButtons;
                                continue;
                            case "ELSEACT":
                                currentSay = elseActs;
                                currentButtons = gotoButtons;
                                continue;
                            default:
                                throw new NotImplementedException();
                        }
                    }

                    if (lines[x].StartsWith("[") && lines[x].EndsWith("]")) break;

                    if (currentButtons != null)
                    {
                        Match match = Regex.Match(lines[x]);
                        while (match.Success)
                        {
                            currentButtons.Add(string.Format("[{0}]", match.Groups[1].Captures[0].Value));
                            match = match.NextMatch();
                        }

                        //Check if line has a goto command
                        var parts = lines[x].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Count() > 1 && parts[0].ToLower() == "goto")
                            gotoButtons.Add(string.Format("[{0}]", parts[1]));
                    }

                    currentSay.Add(lines[x].TrimEnd());
                }

                break;
            }


            NPCPage page = new NPCPage(sectionName, say, buttons, elseSay, elseButtons, gotoButtons);

            for (int i = 0; i < checks.Count; i++)
                page.ParseCheck(checks[i]);

            for (int i = 0; i < acts.Count; i++)
                page.ParseAct(page.ActList, acts[i]);

            for (int i = 0; i < elseActs.Count; i++)
                page.ParseAct(page.ElseActList, elseActs[i]);

            NPCSections.Add(page);
            currentButtons = new List<string>();
            currentButtons.AddRange(buttons);
            currentButtons.AddRange(elseButtons);
            currentButtons.AddRange(gotoButtons);

            return currentButtons;
        }

        private void ParseTypes(IList<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].StartsWith(TypeKey)) continue;

                while (++i < lines.Count)
                {
                    if (String.IsNullOrEmpty(lines[i])) continue;

                    int index;
                    if (!int.TryParse(lines[i], out index)) return;
                    Types.Add((ItemType) index);
                }
            }
        }

        private void ParseGoods(IList<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (!lines[i].StartsWith(TradeKey)) continue;

                while (++i < lines.Count)
                {
                    if (String.IsNullOrEmpty(lines[i])) continue;

                    ItemInfo info = SMain.Envir.GetItemInfo(lines[i]);
                    if (info == null || Goods.Contains(info))
                    {
                        SMain.Enqueue(string.Format("Could not find Item: {0}, File: {1}", lines[i], Info.FileName));
                        continue;
                    }

                    Goods.Add(info);
                }
            }
        }

        public override void Process(DelayedAction action)
        {
            throw new NotSupportedException();
        }

        public override bool IsAttackTarget(PlayerObject attacker)
        {
            throw new NotSupportedException();
        }
        public override bool IsFriendlyTarget(PlayerObject ally)
        {
            throw new NotSupportedException();
        }
        public override bool IsFriendlyTarget(MonsterObject ally)
        {
            throw new NotSupportedException();
        }
        public override bool IsAttackTarget(MonsterObject attacker)
        {
            throw new NotSupportedException();
        }

        public override int Attacked(PlayerObject attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            throw new NotSupportedException();
        }

        public override int Attacked(MonsterObject attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            throw new NotSupportedException();
        }

        public override void SendHealth(PlayerObject player)
        {
            throw new NotSupportedException();
        }

        public override void Die()
        {
            throw new NotSupportedException();
        }

        public override int Pushed(MapObject pusher, MirDirection dir, int distance)
        {
            throw new NotSupportedException();
        }

        public override void ReceiveChat(string text, ChatType type)
        {
            throw new NotSupportedException();
        }


        public override void Process()
        {
            base.Process();

            if (Envir.Time < TurnTime) return;

            TurnTime = Envir.Time + TurnDelay;
            Turn((MirDirection) Envir.Random.Next(3));
        }

        public override void SetOperateTime()
        {
            long time = Envir.Time + 2000;

            if (TurnTime < time && TurnTime > Envir.Time)
                time = TurnTime;

            if (OwnerTime < time && OwnerTime > Envir.Time)
                time = OwnerTime;

            if (ExpireTime < time && ExpireTime > Envir.Time)
                time = ExpireTime;

            if (PKPointTime < time && PKPointTime > Envir.Time)
                time = PKPointTime;

            if (LastHitTime < time && LastHitTime > Envir.Time)
                time = LastHitTime;

            if (EXPOwnerTime < time && EXPOwnerTime > Envir.Time)
                time = EXPOwnerTime;

            if (BrownTime < time && BrownTime > Envir.Time)
                time = BrownTime;

            for (int i = 0; i < ActionList.Count; i++)
            {
                if (ActionList[i].Time >= time && ActionList[i].Time > Envir.Time) continue;
                time = ActionList[i].Time;
            }

            for (int i = 0; i < PoisonList.Count; i++)
            {
                if (PoisonList[i].TickTime >= time && PoisonList[i].TickTime > Envir.Time) continue;
                time = PoisonList[i].TickTime;
            }

            for (int i = 0; i < Buffs.Count; i++)
            {
                if (Buffs[i].ExpireTime >= time && Buffs[i].ExpireTime > Envir.Time) continue;
                time = Buffs[i].ExpireTime;
            }


            if (OperateTime <= Envir.Time || time < OperateTime)
                OperateTime = time;
        }

        public void Turn(MirDirection dir)
        {
            Direction = dir;

            Broadcast(new S.ObjectTurn {ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation});
        }

        public override Packet GetInfo()
        {
            return new S.ObjectNPC
                {
                    ObjectID = ObjectID,
                    Name = Name,
                    NameColour = NameColour,
                    Image = Info.Image,
                    Location = CurrentLocation,
                    Direction = Direction,
                };
        }

        public override void ApplyPoison(Poison p)
        {
            throw new NotSupportedException();
        }

        public override void AddBuff(Buff b)
        {
            throw new NotSupportedException();
        }

        public void Call(PlayerObject player, string key)
        {
            if (key != MainKey)
            {
                if (player.NPCID != ObjectID) return;

                if (!player.NPCGoto)
                {
                    if (player.NPCSuccess)
                    {
                        if (!player.NPCPage.Buttons.Contains(key)) return;
                    }
                    else
                    {
                        if (!player.NPCPage.ElseButtons.Contains(key)) return;
                    }
                }

                player.NPCGoto = false;
            }

            for (int i = 0; i < NPCSections.Count; i++)
            {

                NPCPage page = NPCSections[i];
                if (page.Key != key) continue;

                ProcessPage(player, page);
            }
        }

        public void Buy(PlayerObject player, int index, uint count)
        {
            ItemInfo info = null;

            for (int i = 0; i < Goods.Count; i++)
            {
                if (Goods[i].Index != index) continue;
                info = Goods[i];
                break;
            }

            if (count == 0 || info == null || count > info.StackSize) return;

            uint cost = info.Price*count;
            cost = (uint) (cost*Info.PriceRate);

            if (cost > player.Account.Gold) return;

            UserItem item = Envir.CreateFreshItem(info);
            item.Count = count;

            if (!player.CanGainItem(item)) return;

            player.Account.Gold -= cost;
            player.Enqueue(new S.LoseGold {Gold = cost});
            player.GainItem(item);
        }
        public void Sell(UserItem item)
        {
            /* Handle Item Sale */


        }

        private void ProcessPage(PlayerObject player, NPCPage page)
        {
            player.NPCID = ObjectID;
            player.NPCSuccess = page.Check(player);
            player.NPCPage = page;

            switch (page.Key)
            {
                case BuyKey:
                    for (int i = 0; i < Goods.Count; i++)
                        player.CheckItemInfo(Goods[i]);

                    player.Enqueue(new S.NPCGoods {List = GoodsIndex, Rate = Info.PriceRate});
                    break;
                case SellKey:
                    player.Enqueue(new S.NPCSell());
                    break;
                case RepairKey:
                    player.Enqueue(new S.NPCRepair { Rate = Info.PriceRate });
                    break;
                case SRepairKey:
                    player.Enqueue(new S.NPCSRepair { Rate = Info.PriceRate });
                    break;
                case StorageKey:
                    player.SendStorage();
                    player.Enqueue(new S.NPCStorage());
                    break;
                case BuyBackKey:
                    break;
                case ConsignKey:
                    player.Enqueue(new S.NPCConsign());
                    break;
                case MarketKey:
                    player.UserMatch = false;
                    player.GetMarket(string.Empty, ItemType.Nothing);
                    break;
                case ConsignmentsKey:
                    player.UserMatch = true;
                    player.GetMarket(string.Empty, ItemType.Nothing);
                    break;
            }

        }
    }

    public class NPCPage
    {
        public readonly string Key;
        public List<NPCChecks> CheckList = new List<NPCChecks>();

        public List<NPCActions> ActList = new List<NPCActions>(), ElseActList = new List<NPCActions>();
        public List<string> Say, ElseSay, Buttons, ElseButtons, GotoButtons;

        public NPCPage(string key, List<string> say, List<string> buttons, List<string> elseSay, List<string> elseButtons, List<string> gotoButtons)
        {
            Key = key;

            Say = say;
            Buttons = buttons;

            ElseSay = elseSay;
            ElseButtons = elseButtons;

            GotoButtons = gotoButtons;
        }

        private bool _sayCommandFound;
        private string _sayCommandValue;
        public string SayCommandCheck
        {
            get { return _sayCommandValue; }
            set { _sayCommandValue = value; _sayCommandFound = true; }
        }

        public void ParseCheck(string line)
        {
            var parts = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) return;

            int temp;
            uint temp2;
            byte temp3;
            var regexFlag = new Regex(@"\[(.*?)\]");

            switch (parts[0].ToUpper())
            {
                case "MINLEVEL":
                    if (parts.Length < 2) return;
                    if (!byte.TryParse(parts[1], out temp3)) return;

                    CheckList.Add(new NPCChecks(CheckType.MinLevel, temp3));
                    break;

                case "MAXLEVEL":
                    if (parts.Length < 2) return;
                    if (!byte.TryParse(parts[1], out temp3)) return;

                    CheckList.Add(new NPCChecks(CheckType.MaxLevel, temp3));
                    break;

                case "CHECKGOLD":
                    if (parts.Length < 2) return;
                    if (!uint.TryParse(parts[1], out temp2)) return;

                    CheckList.Add(new NPCChecks(CheckType.CheckGold, temp2));
                    break;

                case "CHECKITEM":
                    if (parts.Length < 2) return;
                    var info = SMain.Envir.GetItemInfo(parts[1]);

                    if (info == null)
                    {
                        SMain.Enqueue(string.Format("Failed to get ItemInfo: {0}, Page: {1}", parts[1], Key));
                        return;
                    }

                    if (parts.Length < 3 || !uint.TryParse(parts[2], out temp2)) temp2 = 1;
                    CheckList.Add(new NPCChecks(CheckType.CheckItem, parts[1], temp2));
                    break;

                case "CHECKGENDER":
                    if (parts.Length < 2) return;
                    if (!Enum.IsDefined(typeof(MirGender), parts[1])) return;

                    temp3 = (byte)Enum.Parse(typeof(MirGender), parts[1]);
                    CheckList.Add(new NPCChecks(CheckType.CheckGender, temp3));
                    break;

                case "CHECKCLASS":
                    if (parts.Length < 2) return;
                    if (!Enum.IsDefined(typeof(MirClass), parts[1])) return;

                    temp3 = (byte)Enum.Parse(typeof(MirClass), parts[1]);
                    CheckList.Add(new NPCChecks(CheckType.CheckClass, temp3));
                    break;

                case "DAYOFWEEK":
                    if (parts.Length < 2) return;
                    CheckList.Add(new NPCChecks(CheckType.CheckDay, parts[1]));
                    break;

                case "HOUR":
                    if (parts.Length < 2 || !uint.TryParse(parts[1], out temp2)) return;

                    CheckList.Add(new NPCChecks(CheckType.CheckHour, temp2));
                    break;

                case "MIN":
                    if (parts.Length < 2 || !uint.TryParse(parts[1], out temp2)) return;

                    CheckList.Add(new NPCChecks(CheckType.CheckMinute, temp2));
                    break;

                case "CHECKNAMELIST":
                    if (parts.Length < 2) return;

                    var fileName = Path.Combine(Settings.NameListPath, parts[1] + ".txt");
                    if (File.Exists(fileName))
                        CheckList.Add(new NPCChecks(CheckType.CheckNameList, fileName));
                    break;

                case "ISADMIN":
                    CheckList.Add(new NPCChecks(CheckType.IsAdmin));
                    break;

                case "CHECKPKPOINT":
                    if (parts.Length < 2) return;
                    if (!uint.TryParse(parts[1], out temp2)) return;

                    CheckList.Add(new NPCChecks(CheckType.CheckPkPoint));
                    break;

                case "CHECKRANGE":
                    if (parts.Length < 4) return;

                    int x, y, distance;
                    if (!int.TryParse(parts[1], out x) || !int.TryParse(parts[2], out y) || !int.TryParse(parts[3], out distance)) return;
                    CheckList.Add(new NPCChecks(CheckType.CheckRange, x, y, distance));
                    break;

                case "CHECK":
                    if (parts.Length < 3) return;
                    var match = regexFlag.Match(parts[1]);
                    if (match.Success)
                    {
                        uint flagIndex;
                        uint onCheck;
                        if (!uint.TryParse(match.Groups[1].Captures[0].Value, out flagIndex)) return;
                        if (!uint.TryParse(parts[2], out onCheck)) return;
                        if (flagIndex > Globals.FlagIndexCount) return;
                        var flagIsOn = Convert.ToBoolean(onCheck);
                        CheckList.Add(new NPCChecks(CheckType.Check, flagIndex, flagIsOn));
                    }   
                    break;

            }

        }
        public void ParseAct(List<NPCActions> acts, string line)
        {
            var parts = line.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0) return;

            ItemInfo info;
            uint temp;
            int temp1;
            string fileName;
            var regexMessage = new Regex("\"([^\"]*)\"");
            var regexFlag = new Regex(@"\[(.*?)\]");

            switch (parts[0].ToUpper())
            {
                case "MOVE":
                    int x = 0, y = 0;
                    if (parts.Length < 2) return;

                    if (parts.Length > 3)
                    {
                        if (!int.TryParse(parts[2], out x)) return;
                        if (!int.TryParse(parts[3], out y)) return;
                    }

                    acts.Add(new NPCActions(ActionType.Teleport, parts[1], new Point(x, y)));
                    break;

                case "INSTANCEMOVE":
                    int instanceId;
                    if (parts.Length < 5) return;
                    if (!int.TryParse(parts[2], out instanceId)) return;
                    if (!int.TryParse(parts[3], out x)) return;
                    if (!int.TryParse(parts[4], out y)) return;

                    acts.Add(new NPCActions(ActionType.InstanceTeleport, parts[1], instanceId, new Point(x, y)));
                    break;

                case "GIVEGOLD":
                    if (parts.Length < 2) return;
                    if (!uint.TryParse(parts[1], out temp)) return;

                    acts.Add(new NPCActions(ActionType.GiveGold, temp));
                    break;

                case "TAKEGOLD":
                    if (parts.Length < 2) return;
                    if (!uint.TryParse(parts[1], out temp)) return;

                    acts.Add(new NPCActions(ActionType.TakeGold, temp));
                    break;

                case "GIVEITEM":
                    if (parts.Length < 2) return;

                    info = SMain.Envir.GetItemInfo(parts[1]);

                    if (info == null)
                    {
                        SMain.Enqueue(string.Format("Failed to get ItemInfo: {0}, Page: {1}", parts[1], Key));
                        break;
                    }

                    if (parts.Length < 3 || !uint.TryParse(parts[2], out temp)) temp = 1;
                    acts.Add(new NPCActions(ActionType.GiveItem, info, temp));
                    break;

                case "TAKEITEM":
                    if (parts.Length < 3) return;

                    info = SMain.Envir.GetItemInfo(parts[1]);

                    if (info == null)
                    {
                        SMain.Enqueue(string.Format("Failed to get ItemInfo: {0}, Page: {1}", parts[1], Key));
                        break;
                    }

                    if (parts.Length < 3 || !uint.TryParse(parts[2], out temp)) temp = 1;
                    acts.Add(new NPCActions(ActionType.TakeItem, info, temp));
                    break;

                case "GIVEEXP":
                    if (parts.Length < 2) return;
                    if (!uint.TryParse(parts[1], out temp)) return;

                    acts.Add(new NPCActions(ActionType.GiveExp, temp));
                    break;

                case "GIVEPET":
                    if (parts.Length < 2) return;

                    byte petcount = 0;
                    byte petlevel = 0;

                    MonsterInfo mInfo2 = SMain.Envir.GetMonsterInfo(parts[1]);
                    if (mInfo2 == null) return;

                    if (parts.Length > 2)
                       petcount = byte.TryParse(parts[2], out petcount) ? Math.Min((byte)5, petcount) : (byte)1;

                    if(parts.Length > 3)
                       petlevel = byte.TryParse(parts[3], out petlevel) ? Math.Min((byte)7, petlevel) : (byte)0;

                    acts.Add(new NPCActions(ActionType.GivePet, mInfo2, petcount, petlevel));
                    break;

                case "GOTO":
                    if (parts.Length < 2) return;

                    acts.Add(new NPCActions(ActionType.Goto, parts[1]));
                    break;

                case "ADDNAMELIST":
                    if (parts.Length < 2) return;

                    fileName = Path.Combine(Settings.NameListPath, parts[1] + ".txt");
                    if (!File.Exists(fileName))
                        File.Create(fileName);

                        acts.Add(new NPCActions(ActionType.AddNameList, fileName));
                    break;

                case "DELNAMELIST":
                    if (parts.Length < 2) return;

                    fileName = Path.Combine(Settings.NameListPath, parts[1] + ".txt");
                    if (File.Exists(fileName))
                        acts.Add(new NPCActions(ActionType.DelNameList, fileName));
                    break;

                case "CLEARNAMELIST":
                    if (parts.Length < 2) return;

                    fileName = Path.Combine(Settings.NameListPath, parts[1] + ".txt");
                    if (File.Exists(fileName))
                        acts.Add(new NPCActions(ActionType.ClearNameList, fileName));
                    break;

                case "GIVEHP":
                    if (parts.Length < 2) return;
                    if (!int.TryParse(parts[1], out temp1)) return;
                    acts.Add(new NPCActions(ActionType.GiveHP, temp1));
                    break;

                case "GIVEMP":
                    if (parts.Length < 2) return;
                    if (!int.TryParse(parts[1], out temp1)) return;
                    acts.Add(new NPCActions(ActionType.GiveMP, temp1));
                    break;

                case "CHANGELEVEL":
                    if (parts.Length < 2) return;
                    byte temp2;
                    if (!byte.TryParse(parts[1], out temp2)) return;

                    temp2 = Math.Min(byte.MaxValue, temp2);
                    acts.Add(new NPCActions(ActionType.ChangeLevel, temp2));
                    break;

                case "SETPKPOINT":
                    if (parts.Length < 2) return;
                    if (!int.TryParse(parts[1], out temp1)) return;
                    acts.Add(new NPCActions(ActionType.SetPkPoint, temp1));
                    break;

                case "CHANGEGENDER":
                    acts.Add(new NPCActions(ActionType.ChangeGender));
                    break;

                case "CHANGECLASS":
                    if (!Enum.IsDefined(typeof(MirClass), parts[1])) return;

                    var type = (byte)Enum.Parse(typeof(MirClass), parts[1]);
                    acts.Add(new NPCActions(ActionType.ChangeClass, type));
                    break;

                case "LINEMESSAGE":
                    var match = regexMessage.Match(line);
                    if (match.Success)
                    {
                        var message = match.Groups[1].Captures[0].Value;

                        var last = parts.Count() - 1;
                        if (!Enum.IsDefined(typeof(ChatType), parts[last])) return;

                        var chatType = (byte)Enum.Parse(typeof(ChatType), parts[last]);
                        acts.Add(new NPCActions(ActionType.LineMessage, message, chatType));
                    }
                    break;

                case "GIVESKILL":
                    if (parts.Length < 3) return;
                    if (!Enum.IsDefined(typeof(Spell), parts[1])) return;
                    if (!byte.TryParse(parts[2], out temp2)) return;

                    var spell = (byte)Enum.Parse(typeof(Spell), parts[1]);
                    acts.Add(new NPCActions(ActionType.GiveSkill, spell, Math.Min(temp2, (byte)3)));
                    break;

                case "SET":
                    if (parts.Length < 3) return;
                    match = regexFlag.Match(parts[1]);
                    if (match.Success)
                    {
                        uint flagIndex;
                        uint onCheck;
                        if (!uint.TryParse(match.Groups[1].Captures[0].Value, out flagIndex)) return;
                        if (!uint.TryParse(parts[2], out onCheck)) return;
                        if (flagIndex > Globals.FlagIndexCount) return;
                        var flagIsOn = Convert.ToBoolean(onCheck);
                        acts.Add(new NPCActions(ActionType.Set, flagIndex, flagIsOn));
                    }   
                    break;
            }

        }
        public List<string> ParseSay(PlayerObject player, List<string> speech)
        {
            for (var i = 0; i < speech.Count; i++)
            {
                var parts = speech[i].Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 0) continue;

                var regex = new Regex(@"<\$(.*?)>");

                foreach (var part in parts)
                {
                    var match = regex.Match(part);

                    if (!match.Success) continue;

                    switch (match.Groups[1].Captures[0].Value.ToUpper())
                    {
                        case "USERNAME":
                            SayCommandCheck = player.Name;
                            break;
                        case "LEVEL":
                            SayCommandCheck = player.Level.ToString(CultureInfo.InvariantCulture);
                            break;
                        case "HP":
                            SayCommandCheck = player.HP.ToString(CultureInfo.InvariantCulture);
                            break;
                        case "MAXHP":
                            SayCommandCheck = player.MaxHP.ToString(CultureInfo.InvariantCulture);
                            break;
                        case "MP":
                            SayCommandCheck = player.MP.ToString(CultureInfo.InvariantCulture);
                            break;
                        case "MAXMP":
                            SayCommandCheck = player.MaxMP.ToString(CultureInfo.InvariantCulture);
                            break;
                        case "GAMEGOLD":
                            SayCommandCheck = player.Account.Gold.ToString(CultureInfo.InvariantCulture);
                            break;
                        case "ARMOUR":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Armour] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Armour].Info.Name : "No Armour";
                            break;
                        case "WEAPON":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Weapon] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Weapon].Info.Name : "No Weapon";
                            break;
                        case "RING_L":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.RingL] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.RingL].Info.Name : "No Ring";
                            break;
                        case "RING_R":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.RingR] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.RingR].Info.Name : "No Ring";
                            break;
                        case "BRACELET_L":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.BraceletL] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.BraceletL].Info.Name : "No Bracelet";
                            break;
                        case "BRACELET_R":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.BraceletR] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.BraceletR].Info.Name : "No Bracelet";
                            break;
                        case "NECKLACE":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Necklace] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Necklace].Info.Name : "No Necklace";
                            break;
                        case "BELT":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Belt] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Belt].Info.Name : "No Belt";
                            break;
                        case "BOOTS":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Boots] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Boots].Info.Name : "No Boots";
                            break;
                        case "HELMET":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Helmet] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Helmet].Info.Name : "No Helmet";
                            break;
                        case "AMULET":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Amulet] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Amulet].Info.Name : "No Amulet";
                            break;
                        case "STONE":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Stone] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Stone].Info.Name : "No Stone";
                            break;
                        case "TORCH":
                            SayCommandCheck = player.Info.Equipment[(int)EquipmentSlot.Torch] != null ?
                                player.Info.Equipment[(int)EquipmentSlot.Torch].Info.Name : "No Torch";
                            break;

                        case "DATE":
                            SayCommandCheck = DateTime.Now.ToShortDateString();
                            break;
                        case "USERCOUNT":
                            //SayCommandCheck = Envir.PlayerCount.ToString(CultureInfo.InvariantCulture);
                            break;
                        case "PKPOINT":
                            SayCommandCheck = player.PKPoints.ToString();
                            break;

                        default:
                            SayCommandCheck = string.Empty;
                            break;
                    }

                    if (!_sayCommandFound) continue;

                    _sayCommandFound = false;
                    speech[i] = speech[i].Replace(part, _sayCommandValue);
                }
            }
            return speech;
        }

        public bool Check(PlayerObject player)
        {
            var failed = false;

            foreach (NPCChecks check in CheckList)
            {
                switch (check.Type)
                {
                    case CheckType.MaxLevel:
                        if (player.Level > (byte) check.Params[0])
                            failed = true;
                        break;

                    case CheckType.MinLevel:
                        if (player.Level < (byte) check.Params[0])
                            failed = true;
                        break;

                    case CheckType.CheckGold:
                        if (player.Account.Gold < (uint) check.Params[0])
                            failed = true;
                        break;

                    case CheckType.CheckItem:
                        var info = SMain.Envir.GetItemInfo((string) check.Params[0]);

                        var count = (uint) check.Params[1];

                        foreach (var item in player.Info.Inventory.Where(item => item != null && item.Info == info))
                        {
                            if (count > item.Count)
                            {
                                count -= item.Count;
                                continue;
                            }

                            if (count > item.Count) continue;
                            count = 0;
                            break;
                        }
                        if (count > 0)
                            failed = true;
                        break;

                    case CheckType.CheckGender:
                        failed = (byte)player.Gender != (byte)check.Params[0];
                        break;

                    case CheckType.CheckClass:
                        failed = (byte)player.Class != (byte)check.Params[0];
                        break;

                    case CheckType.CheckDay:
                        var day = DateTime.Now.DayOfWeek.ToString().ToUpper();
                        var dayToCheck = check.Params[0].ToString().ToUpper();

                        failed = day != dayToCheck;
                        break;

                    case CheckType.CheckHour:
                        var hour = DateTime.Now.Hour;
                        var hourToCheck = (uint)check.Params[0];

                        failed = hour != hourToCheck;
                        break;

                    case CheckType.CheckMinute:
                        var minute = DateTime.Now.Minute;
                        var minuteToCheck = (uint)check.Params[0];

                        failed = minute != minuteToCheck;
                        break;

                    case CheckType.CheckNameList:
                        var read = File.ReadAllLines((string) check.Params[0]);
                        failed = !read.Contains(player.Name);
                        break;

                    case CheckType.IsAdmin:
                        failed = !player.IsGM;
                        break;

                    case CheckType.CheckPkPoint:
                        failed = player.PKPoints < (int) check.Params[0];      
                        break;

                    case CheckType.CheckRange:
                        var target = new Point {X = (int) check.Params[0], Y = (int) check.Params[1]};

                        failed = !Functions.InRange(player.CurrentLocation, target, (int)check.Params[2]);
                        break;

                    case CheckType.Check:
                        var flag = player.Info.Flags[(uint)check.Params[0]];

                        failed = flag != (bool) check.Params[1];
                        break;
                }

                if (!failed) continue;

                Failed(player);
                return false;
            }

            Success(player);
            return true;

        }
        private void Act(IList<NPCActions> acts, PlayerObject player)
        {
            for (var i = 0; i < acts.Count; i++)
            {
                NPCActions act = acts[i];
                uint gold;
                uint count;
                string path;

                switch (act.Type)
                {
                    case ActionType.Teleport:
                        var map = SMain.Envir.GetMapByNameAndInstance((string)act.Params[0]);
                        if (map == null) return;

                        var coords = (Point)act.Params[1];

                        if (coords.X > 0 && coords.Y > 0) player.Teleport(map, coords);
                        else player.TeleportRandom(200, 0, map);
                        break;

                    case ActionType.InstanceTeleport:
                        map = SMain.Envir.GetMapByNameAndInstance((string)act.Params[0], (int)act.Params[1]);
                        if (map == null) return;
                        player.Teleport(map, (Point)act.Params[2]);
                        break;

                    case ActionType.GiveGold:
                        gold = (uint)act.Params[0];

                        if (gold + player.Account.Gold >= uint.MaxValue)
                            gold = uint.MaxValue - player.Account.Gold;

                        player.GainGold(gold);
                        break;

                    case ActionType.TakeGold:
                        gold = (uint)act.Params[0];

                        if (gold >= player.Account.Gold) gold = player.Account.Gold;

                        player.Account.Gold -= gold;
                        player.Enqueue(new S.LoseGold { Gold = gold });
                        break;

                    case ActionType.GiveItem:
                        count = (uint)act.Params[1];

                        while (count > 0)
                        {
                            UserItem item = SMain.Envir.CreateFreshItem((ItemInfo)act.Params[0]);

                            if (item == null)
                            {
                                SMain.Enqueue(string.Format("Failed to create UserItem: {0}, Page: {1}", act.Params[0], Key));
                                return;
                            }

                            if (item.Info.StackSize > count)
                            {
                                item.Count = count;
                                count = 0;
                            }
                            else
                            {
                                count -= item.Info.StackSize;
                                item.Count = item.Info.StackSize;
                            }

                            if (player.CanGainItem(item, false))
                                player.GainItem(item);
                        }
                        break;

                    case ActionType.TakeItem:
                        ItemInfo info = (ItemInfo)act.Params[0];

                        count = (uint)act.Params[1];

                        for (int o = 0; o < player.Info.Inventory.Length; o++)
                        {
                            UserItem item = player.Info.Inventory[o];
                            if (item == null) continue;
                            if (item.Info != info) continue;

                            if (count > item.Count)
                            {
                                player.Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = item.Count });
                                player.Info.Inventory[o] = null;

                                count -= item.Count;
                                continue;
                            }

                            player.Enqueue(new S.DeleteItem { UniqueID = item.UniqueID, Count = count });
                            if (count == item.Count)
                                player.Info.Inventory[o] = null;
                            else
                                item.Count -= count;
                            break;
                        }
                        player.RefreshStats();
                        break;

                    case ActionType.GiveExp:
                        player.GainExp((uint)act.Params[0]);
                        break;

                    case ActionType.GivePet:
                        for (var c = 0; c < (byte)act.Params[1]; c++)
                        {
                            MonsterObject monster = MonsterObject.GetMonster((MonsterInfo)act.Params[0]);
                            if (monster == null) return;
                            monster.PetLevel = (byte)act.Params[2];
                            monster.Master = player;
                            monster.MaxPetLevel = 7;
                            monster.Direction = player.Direction;
                            monster.ActionTime = SMain.Envir.Time + 1000;
                            monster.Spawn(player.CurrentMap, player.CurrentLocation);
                            player.Pets.Add(monster);
                        }
                        break;

                    case ActionType.AddNameList:
                        path = (string)act.Params[0];
                        if (File.ReadAllLines(path).All(t => player.Name != t))
                            {
                                using (var line = File.AppendText(path))
                                {
                                    line.WriteLine(player.Name);
                                }
                            }
                        break;

                    case ActionType.DelNameList:
                        path = (string)act.Params[0];
                        File.WriteAllLines(path, File.ReadLines(path).Where(l => l != player.Name).ToList());
                        break;

                    case ActionType.ClearNameList:
                        path = (string)act.Params[0];
                        File.WriteAllLines(path, new string[] { });
                        break;

                    case ActionType.GiveHP:
                        player.ChangeHP((int)act.Params[0]);
                        break;

                    case ActionType.GiveMP:
                        player.ChangeMP((int)act.Params[0]);
                        break;

                    case ActionType.ChangeLevel:
                        player.Level = (byte) act.Params[0];
                        player.LevelUp();
                        break;

                    case ActionType.SetPkPoint:
                        player.PKPoints = (int) act.Params[0];
                        break;

                    case ActionType.ChangeGender:
                        switch (player.Info.Gender)
                        {
                            case MirGender.Male:
                                player.Info.Gender = MirGender.Female;
                                break;
                            case MirGender.Female:
                                player.Info.Gender = MirGender.Male;
                                break;
                        }
                        break;

                    case ActionType.ChangeClass:
                        var data = (MirClass)act.Params[0];
                        switch (data)
                        {
                            case MirClass.Warrior:
                                player.Info.Class = MirClass.Warrior;
                                break;
                            case MirClass.Taoist:
                                player.Info.Class = MirClass.Taoist;
                                break;
                            case MirClass.Wizard:
                                player.Info.Class = MirClass.Wizard;
                                break;
                            case MirClass.Assassin:
                                player.Info.Class = MirClass.Assassin;
                                break;
                        }
                        break;

                    case ActionType.LineMessage:
                        player.ReceiveChat((string)act.Params[0], (ChatType)act.Params[1]);
                        break;

                    case ActionType.GiveSkill:
                        var magic = new UserMagic((Spell)act.Params[0]) { Level = (byte)act.Params[1] };

                        player.Info.Magics.Add(magic);
                        player.Enqueue(magic.GetInfo());
                        break;

                    case ActionType.Goto:
                        player.NPCGoto = true;
                        player.NPCGotoPage = "[" + act.Params[0] + "]";
                        break;

                    case ActionType.Set:
                        player.Info.Flags[(uint) act.Params[0]] = (bool) act.Params[1];
                        break;
                }
            }
        }

        private void Success(PlayerObject player)
        {
            Act(ActList, player);

            var parseSay = new List<String>(Say);
            parseSay = ParseSay(player, parseSay);

            player.Enqueue(new S.NPCResponse { Page = parseSay });
        }
        private void Failed(PlayerObject player)
        {
            Act(ElseActList, player);

            var parseElseSay = new List<String>(ElseSay);
            parseElseSay = ParseSay(player, parseElseSay);

            player.Enqueue(new S.NPCResponse { Page = parseElseSay });
        }
        
    }

    public class NPCChecks
    {
        public CheckType Type;
        public List<object> Params = new List<object>();

        public NPCChecks(CheckType check, params object[] p)
        {
            Type = check;

            for (int i = 0; i < p.Length; i++)
                Params.Add(p[i]);
        }
    }
    public class NPCActions
    {
        public ActionType Type;
        public List<object> Params = new List<object>();

        public NPCActions(ActionType action, params object[] p)
        {
            Type = action;

            Params.AddRange(p);
        }
    }

    public enum ActionType
    {
        Teleport,
        InstanceTeleport,
        GiveGold,
        TakeGold,
        GiveItem,
        TakeItem,
        GiveExp,
        GivePet,
        AddNameList,
        DelNameList,
        ClearNameList,
        GiveHP,
        GiveMP,
        ChangeLevel,
        SetPkPoint,
        ChangeGender,
        ChangeClass,
        LineMessage,
        Goto,
        GiveSkill,
        Set,
    }
    public enum CheckType
    {
        IsAdmin,
        MinLevel,
        MaxLevel,
        CheckItem,
        CheckGold,
        CheckGender,
        CheckClass,
        CheckDay,
        CheckHour,
        CheckMinute,
        CheckNameList,
        CheckPkPoint,
        CheckRange,
        Check,
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using Server.MirDatabase;
using Server.MirNetwork;
using Server.MirObjects;
using S = ServerPackets;

namespace Server.MirEnvir
{
    public class Envir
    {
        public static object AccountLock = new object();
        public static object LoadLock = new object();

        public const int Version = 18;
        public const string DatabasePath = @".\Server.MirDB";
        public const string AccountPath = @".\Server.MirADB";
        public const string BackUpPath = @".\Back Up\";
        private static readonly Regex AccountIDReg, PasswordReg, EMailReg, CharacterReg;

        public static int LoadVersion;

        private readonly DateTime _startTime = DateTime.Now;
        public readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        public long Time { get; private set; }

        public DateTime Now
        {
            get { return _startTime.AddMilliseconds(Time); }
        }

        public bool Running { get; private set; }


        private static uint _objectID;
        public uint ObjectID
        {
            get { return ++_objectID; }
        }

        public static int _playerCount;
        public int PlayerCount
        {
            get { return Players.Count; }
        }


        public Random Random = new Random();
        private Thread _thread;
        private TcpListener _listener;
        private int _sessionID;
        public List<MirConnection> Connections = new List<MirConnection>();


        //Server DB
        public int MapIndex, ItemIndex,  MonsterIndex;
        public List<MapInfo> MapInfoList = new List<MapInfo>();
        public List<ItemInfo> ItemInfoList = new List<ItemInfo>();
        public List<MonsterInfo> MonsterInfoList = new List<MonsterInfo>();
        public DragonInfo DragonInfo;

        //User DB
        public int NextAccountID, NextCharacterID;
        public ulong NextUserItemID, NextAuctionID;
        public List<AccountInfo> AccountList = new List<AccountInfo>();
        public List<CharacterInfo> CharacterList = new List<CharacterInfo>(); 
        public LinkedList<AuctionInfo> Auctions = new LinkedList<AuctionInfo>();

        //Live Info
        public List<Map> MapList = new List<Map>();
        public List<SafeZoneInfo> StartPoints = new List<SafeZoneInfo>(); 
        public List<ItemInfo> StartItems = new List<ItemInfo>(); 
        public List<PlayerObject> Players = new List<PlayerObject>();//farril
        public bool Saving = false;
        public LightSetting Lights;
        public LinkedList<MapObject> Objects = new LinkedList<MapObject>();
        public Dragon DragonSystem;


        static Envir()
        {
            AccountIDReg =
                new Regex(@"^[A-Za-z0-9]{" + Globals.MinAccountIDLength + "," + Globals.MaxAccountIDLength + "}$");
            PasswordReg =
                new Regex(@"^[A-Za-z0-9]{" + Globals.MinPasswordLength + "," + Globals.MaxPasswordLength + "}$");
            EMailReg = new Regex(@"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            CharacterReg =
                new Regex(@"^[A-Za-z0-9]{" + Globals.MinCharacterNameLength + "," + Globals.MaxCharacterNameLength +
                          "}$");
        }

        public static int LastCount = 0, LastRealCount = 0;
        public int MonsterCount;

        private void WorkLoop()
        {
            Time = _stopwatch.ElapsedMilliseconds;
            long conTime = Time;
            long saveTime = Time + Settings.SaveDelay * Settings.Minute;
            long userTime = Time + Settings.Minute * 5;
            long proceesTime = Time + 1000;
            int processCount = 0;
            int processRealCount = 0;

            LinkedListNode<MapObject> current = null;

            StartEnvir();
            StartNetwork();

            try
            {

                while (Running)
                {
                    Time = _stopwatch.ElapsedMilliseconds;

                    if (Time >= proceesTime)
                    {
                        LastCount = processCount;
                        LastRealCount = processRealCount;
                        processCount = 0;
                        processRealCount = 0;
                        proceesTime = Time + 1000;
                    }

                    if (conTime != Time)
                    {
                        conTime = Time;

                        AdjustLights();

                        lock (Connections)
                        {
                            for (int i = Connections.Count - 1; i >= 0; i--)
                                Connections[i].Process();
                        }
                    }

                    if (current == null)
                        current = Objects.First;

                    for (int i = 0; i < 100; i++)
                    {
                        if (current == null) break;

                        LinkedListNode<MapObject> next = current.Next;


                        if (Time > current.Value.OperateTime)
                        {
                            processRealCount++;
                            current.Value.Process();
                            current.Value.SetOperateTime();
                        }
                        processCount++;
                        current = next;
                    }


                    for (int i = 0; i < MapList.Count; i++)
                        MapList[i].Process();

                    if (DragonSystem != null) DragonSystem.Process();

                    if (Time >= saveTime)
                    {
                        saveTime = Time + Settings.SaveDelay*Settings.Minute;
                        BeginSaveAccounts();
                    }

                    if (Time >= userTime)
                    {
                        userTime = Time + Settings.Minute*5;
                        Broadcast(new S.Chat
                            {
                                Message = string.Format("Online Players: {0}", Players.Count),
                                Type = ChatType.Hint
                            });
                    }

                    //   if (Players.Count == 0) Thread.Sleep(1);
                    //   GC.Collect();
                }
            }
            catch (Exception ex)
            {
                SMain.Enqueue(ex);

                lock (Connections)
                {
                    for (int i = Connections.Count - 1; i >= 0; i--)
                        Connections[i].SendDisconnect(3);
                }
            }

            StopNetwork();
            StopEnvir(); 
            SaveAccounts();

            _thread = null;
        }


        private void AdjustLights()
        {
            LightSetting oldLights = Lights;

            int hours = (Now.Hour * 2) % 24;
            if (hours == 6 || hours == 7)
                Lights = LightSetting.Dawn;
            else if (hours >= 8 && hours <= 15)
                Lights = LightSetting.Day;
            else if (hours == 16 || hours == 17)
                Lights = LightSetting.Evening;
            else
                Lights = LightSetting.Night;

            if (oldLights == Lights) return;

            Broadcast(new S.TimeOfDay { Lights = Lights });
        }

        public void Broadcast(Packet p)
        {
            for (int i = 0; i < Players.Count; i++) Players[i].Enqueue(p);
        }


        public void SaveDB()
        {
            using (FileStream stream = File.Create(DatabasePath))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Version);
                writer.Write(MapIndex);
                writer.Write(ItemIndex);
                writer.Write(MonsterIndex);

                writer.Write(MapInfoList.Count);
                for (int i = 0; i < MapInfoList.Count; i++)
                    MapInfoList[i].Save(writer);

                writer.Write(ItemInfoList.Count);
                for (int i = 0; i < ItemInfoList.Count; i++)
                    ItemInfoList[i].Save(writer);

                writer.Write(MonsterInfoList.Count);
                for (int i = 0; i < MonsterInfoList.Count; i++)
                    MonsterInfoList[i].Save(writer);

                //DragonInfo.Save(writer);
            }
        }
        public void SaveAccounts()
        {
            while (Saving)
                Thread.Sleep(1);

            try
            {
                using (FileStream stream = File.Create(AccountPath))
                    SaveAccounts(stream);
            }
            catch (Exception ex)
            {
                SMain.Enqueue(ex);
            }
        }

        private void SaveAccounts(Stream stream)
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(Version);
                writer.Write(NextAccountID);
                writer.Write(NextCharacterID);
                writer.Write(NextUserItemID);

                writer.Write(AccountList.Count);
                for (int i = 0; i < AccountList.Count; i++)
                    AccountList[i].Save(writer);

                writer.Write(NextAuctionID);
                writer.Write(Auctions.Count);
                foreach (AuctionInfo auction in Auctions)
                    auction.Save(writer);
            }
        }
        private void BeginSaveAccounts()
        {
            if (Saving) return;

            Saving = true;
            

            using (MemoryStream mStream = new MemoryStream())
            {
                if (File.Exists(AccountPath))
                {
                    if (!Directory.Exists(BackUpPath)) Directory.CreateDirectory(BackUpPath);
                    string fileName = string.Format("Accounts {0:0000}-{1:00}-{2:00} {3:00}-{4:00}-{5:00}.bak", Now.Year, Now.Month, Now.Day, Now.Hour, Now.Minute, Now.Second);
                    if (File.Exists(Path.Combine(BackUpPath, fileName))) File.Delete(Path.Combine(BackUpPath, fileName));
                    File.Move(AccountPath, Path.Combine(BackUpPath, fileName));
                }

                SaveAccounts(mStream);
                FileStream fStream = new FileStream(AccountPath, FileMode.Create);

                byte[] data = mStream.ToArray();
                fStream.BeginWrite(data, 0, data.Length, EndSaveAccounts, fStream);
            }

        }
        private void EndSaveAccounts(IAsyncResult result)
        {
            FileStream fStream = result.AsyncState as FileStream;

            if (fStream != null)
            {
                fStream.EndWrite(result);
                fStream.Dispose();
            }

            Saving = false;
        }

        public void LoadDB()
        {
            lock (LoadLock)
            {
                if (!File.Exists(DatabasePath))
                    SaveDB();

                using (FileStream stream = File.OpenRead(DatabasePath))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    LoadVersion = reader.ReadInt32();
                    MapIndex = reader.ReadInt32();
                    ItemIndex = reader.ReadInt32();
                    MonsterIndex = reader.ReadInt32();

                    int count = reader.ReadInt32();
                    MapInfoList.Clear();
                    for (int i = 0; i < count; i++)
                        MapInfoList.Add(new MapInfo(reader));

                    count = reader.ReadInt32();
                    ItemInfoList.Clear();
                    for (int i = 0; i < count; i++)
                        ItemInfoList.Add(new ItemInfo(reader, LoadVersion));

                    count = reader.ReadInt32();
                    MonsterInfoList.Clear();
                    for (int i = 0; i < count; i++)
                        MonsterInfoList.Add(new MonsterInfo(reader));

                    //if (LoadVersion >= 11) DragonInfo = new DragonInfo(reader);
                    //else DragonInfo = new DragonInfo();
                }

            }

        }

        public void LoadAccounts()
        {
            lock (LoadLock)
            {
                if (!File.Exists(AccountPath))
                    SaveAccounts();

                using (FileStream stream = File.OpenRead(AccountPath))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    LoadVersion = reader.ReadInt32();
                    NextAccountID = reader.ReadInt32();
                    NextCharacterID = reader.ReadInt32();
                    NextUserItemID = reader.ReadUInt64();


                    int count = reader.ReadInt32();
                    AccountList.Clear();
                    CharacterList.Clear();
                    for (int i = 0; i < count; i++)
                    {
                        AccountList.Add(new AccountInfo(reader));
                        CharacterList.AddRange(AccountList[i].Characters);
                    }

                    if (LoadVersion < 7) return;

                    foreach (AuctionInfo auction in Auctions)
                        auction.CharacterInfo.AccountInfo.Auctions.Remove(auction);
                    Auctions.Clear();

                    if (LoadVersion >= 8)
                        NextAuctionID = reader.ReadUInt64();

                    count = reader.ReadInt32();
                    for (int i = 0; i < count; i++)
                    {
                        AuctionInfo auction = new AuctionInfo(reader);

                        if (!BindItem(auction.Item) || !BindCharacter(auction)) continue;

                        Auctions.AddLast(auction);
                        auction.CharacterInfo.AccountInfo.Auctions.AddLast(auction);
                    }

                    if (LoadVersion == 7)
                    {
                        foreach (AuctionInfo auction in Auctions)
                        {
                            if (auction.Sold && auction.Expired) auction.Expired = false;

                            auction.AuctionID = ++NextAuctionID;
                        }
                    }
                }
            }
        }

        private bool BindCharacter(AuctionInfo auction)
        {
            for (int i = 0; i < CharacterList.Count; i++)
            {
                if (CharacterList[i].Index != auction.CharacterIndex) continue;

                auction.CharacterInfo = CharacterList[i];
                return true;
            }
            return false;

        }

        public void Start()
        {
            if (Running || _thread != null) return;

            Running = true;

            _thread = new Thread(WorkLoop) {IsBackground = true};
            _thread.Start();
        }
        public void Stop()
        {
            Running = false;

            while (_thread != null)
                Thread.Sleep(1);
        }
        
        private void StartEnvir()
        {
            Players.Clear();
            StartPoints.Clear();
            StartItems.Clear();
            MapList.Clear();

            LoadDB();


            for (int i = 0; i < MapInfoList.Count; i++)
                MapInfoList[i].CreateMap();

            for (int i = 0; i < ItemInfoList.Count; i++)
                if (ItemInfoList[i].StartItem)
                    StartItems.Add(ItemInfoList[i]);

            for (int i = 0; i < MonsterInfoList.Count; i++)
                MonsterInfoList[i].LoadDrops();

            //if (DragonInfo.Enabled)
            //{
            //    DragonSystem = new Dragon(DragonInfo);
            //    if (DragonSystem != null)
            //    {
            //        if (DragonSystem.Load()) DragonSystem.Info.LoadDrops();
            //    }
            //}

            SMain.Enqueue("Envir Started.");
        }
        private void StartNetwork()
        {
            Connections.Clear();
            LoadAccounts();

            _listener = new TcpListener(IPAddress.Parse(Settings.IPAddress), Settings.Port);
            _listener.Start();
            _listener.BeginAcceptTcpClient(Connection, null);
            SMain.Enqueue("Network Started.");
        }

        private void StopEnvir()
        {
            MapList.Clear();
            StartPoints.Clear();
            StartItems.Clear();
            Objects.Clear();
            Players.Clear();
            GC.Collect();

            SMain.Enqueue("Envir Stopped.");
        }
        private void StopNetwork()
        {
            _listener.Stop();

            lock (Connections)
            {
                for (int i = Connections.Count - 1; i >= 0; i--)
                    Connections[i].SendDisconnect(0);
            }

            long expire = Time + 5000;

            while (Connections.Count != 0 && _stopwatch.ElapsedMilliseconds < expire)
            {
                Time = _stopwatch.ElapsedMilliseconds;

                for (int i = Connections.Count - 1; i >= 0; i--)
                    Connections[i].Process();

                Thread.Sleep(1);
            }
            

            Connections.Clear();
            SMain.Enqueue("Network Stopped.");
        }

        private void Connection(IAsyncResult result)
        {
            if (!Running || !_listener.Server.IsBound) return;

            try
            {
                TcpClient tempTcpClient = _listener.EndAcceptTcpClient(result);
                lock (Connections)
                    Connections.Add(new MirConnection(++_sessionID, tempTcpClient));
            }
            catch (Exception ex)
            {
                SMain.Enqueue(ex);
            }
            finally
            {
                while (Connections.Count >= Settings.MaxUser)
                    Thread.Sleep(1);

                if (Running && _listener.Server.IsBound)
                    _listener.BeginAcceptTcpClient(Connection, null);
            }
        }

        
        public void NewAccount(ClientPackets.NewAccount p, MirConnection c)
        {
            if (!Settings.AllowNewAccount)
            {
                c.Enqueue(new ServerPackets.NewAccount {Result = 0});
                return;
            }

            if (!AccountIDReg.IsMatch(p.AccountID))
            {
                c.Enqueue(new ServerPackets.NewAccount {Result = 1});
                return;
            }

            if (!PasswordReg.IsMatch(p.Password))
            {
                c.Enqueue(new ServerPackets.NewAccount {Result = 2});
                return;
            }
            if (!string.IsNullOrWhiteSpace(p.EMailAddress) && !EMailReg.IsMatch(p.EMailAddress) ||
                p.EMailAddress.Length > 50)
            {
                c.Enqueue(new ServerPackets.NewAccount {Result = 3});
                return;
            }

            if (!string.IsNullOrWhiteSpace(p.UserName) && p.UserName.Length > 20)
            {
                c.Enqueue(new ServerPackets.NewAccount {Result = 4});
                return;
            }

            if (!string.IsNullOrWhiteSpace(p.SecretQuestion) && p.SecretQuestion.Length > 30)
            {
                c.Enqueue(new ServerPackets.NewAccount {Result = 5});
                return;
            }

            if (!string.IsNullOrWhiteSpace(p.SecretAnswer) && p.SecretAnswer.Length > 30)
            {
                c.Enqueue(new ServerPackets.NewAccount {Result = 6});
                return;
            }

            lock (AccountLock)
            {
                if (AccountExists(p.AccountID))
                {
                    c.Enqueue(new ServerPackets.NewAccount {Result = 7});
                    return;
                }

                AccountList.Add(new AccountInfo(p) {Index = ++NextAccountID, CreationIP = c.IPAddress});


                c.Enqueue(new ServerPackets.NewAccount {Result = 8});
            }
        }
        public void ChangePassword(ClientPackets.ChangePassword p, MirConnection c)
        {
            if (!Settings.AllowChangePassword)
            {
                c.Enqueue(new ServerPackets.ChangePassword {Result = 0});
                return;
            }

            if (!AccountIDReg.IsMatch(p.AccountID))
            {
                c.Enqueue(new ServerPackets.ChangePassword {Result = 1});
                return;
            }

            if (!PasswordReg.IsMatch(p.CurrentPassword))
            {
                c.Enqueue(new ServerPackets.ChangePassword {Result = 2});
                return;
            }

            if (!PasswordReg.IsMatch(p.NewPassword))
            {
                c.Enqueue(new ServerPackets.ChangePassword {Result = 3});
                return;
            }

            AccountInfo account = GetAccount(p.AccountID);

            if (account == null)
            {
                c.Enqueue(new ServerPackets.ChangePassword {Result = 4});
                return;
            }

            if (account.Banned)
            {
                if (account.ExpiryDate > Now)
                {
                    c.Enqueue(new ServerPackets.ChangePasswordBanned {Reason = account.BanReason, ExpiryDate = account.ExpiryDate});
                    return;
                }
                account.Banned = false;
            }
            account.BanReason = string.Empty;
            account.ExpiryDate = DateTime.MinValue;

            if (String.CompareOrdinal(account.Password, p.CurrentPassword) != 0)
            {
                c.Enqueue(new ServerPackets.ChangePassword {Result = 5});
                return;
            }

            account.Password = p.NewPassword;
            c.Enqueue(new ServerPackets.ChangePassword {Result = 6});
        }
        public void Login(ClientPackets.Login p, MirConnection c)
        {
            if (!Settings.AllowLogin)
            {
                c.Enqueue(new ServerPackets.Login { Result = 0 });
                return;
            }

            if (!AccountIDReg.IsMatch(p.AccountID))
            {
                c.Enqueue(new ServerPackets.Login { Result = 1 });
                return;
            }

            if (!PasswordReg.IsMatch(p.Password))
            {
                c.Enqueue(new ServerPackets.Login { Result = 2 });
                return;
            }
            AccountInfo account = GetAccount(p.AccountID);

            if (account == null)
            {
                c.Enqueue(new ServerPackets.Login { Result = 3 });
                return;
            }

            if (account.Banned)
            {
                if (account.ExpiryDate > DateTime.Now)
                {
                    c.Enqueue(new ServerPackets.LoginBanned
                    {
                        Reason = account.BanReason,
                        ExpiryDate = account.ExpiryDate
                    });
                    return;
                }
                account.Banned = false;
            }
                account.BanReason = string.Empty;
                account.ExpiryDate = DateTime.MinValue;


            if (String.CompareOrdinal(account.Password, p.Password) != 0)
            {
                c.Enqueue(new ServerPackets.Login { Result = 4 });
                return;
            }

            lock (AccountLock)
            {
                if (account.Connection != null)
                    account.Connection.SendDisconnect(1);

                account.Connection = c;
            }

            c.Account = account;
            c.Stage = GameStage.Select;

            account.LastDate = Now;
            account.LastIP = c.IPAddress;
            
            c.Enqueue(new ServerPackets.LoginSuccess { Characters = account.GetSelectInfo() });
        }
        public void NewCharacter(ClientPackets.NewCharacter p, MirConnection c)
        {
            if (!Settings.AllowNewCharacter)
            {
                c.Enqueue(new ServerPackets.NewCharacter {Result = 0});
                return;
            }

            if (!CharacterReg.IsMatch(p.Name))
            {
                c.Enqueue(new ServerPackets.NewCharacter {Result = 1});
                return;
            }

            if (p.Gender != MirGender.Male && p.Gender != MirGender.Female)
            {
                c.Enqueue(new ServerPackets.NewCharacter {Result = 2});
                return;
            }

            if (p.Class != MirClass.Warrior && p.Class != MirClass.Wizard && p.Class != MirClass.Taoist &&
                p.Class != MirClass.Assassin)
            {
                c.Enqueue(new ServerPackets.NewCharacter {Result = 3});
                return;
            }

            int count = 0;

            for (int i = 0; i < c.Account.Characters.Count; i++)
            {
                if (c.Account.Characters[i].Deleted || c.Account.Characters[i].Dead) continue;

                if (++count >= Globals.MaxCharacterCount)
                {
                    c.Enqueue(new ServerPackets.NewCharacter {Result = 4});
                    return;
                }
            }

            lock (AccountLock)
            {
                if (CharacterExists(p.Name))
                {
                    c.Enqueue(new ServerPackets.NewCharacter {Result = 5});
                    return;
                }

                CharacterInfo info = new CharacterInfo(p, c) {Index = ++NextCharacterID};

                c.Account.Characters.Add(info);
                CharacterList.Add(info);

                c.Enqueue(new ServerPackets.NewCharacterSuccess {CharInfo = info.ToSelectInfo()});
            }
        }

        public bool AccountExists(string accountID)
        {
                for (int i = 0; i < AccountList.Count; i++)
                    if (String.Compare(AccountList[i].AccountID, accountID, StringComparison.OrdinalIgnoreCase) == 0)
                        return true;

                return false;
        }
        public bool CharacterExists(string name)
        {
            for (int i = 0; i < CharacterList.Count; i++)
                if (String.Compare(CharacterList[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;

            return false;
        }

        private AccountInfo GetAccount(string accountID)
        {
                for (int i = 0; i < AccountList.Count; i++)
                    if (String.Compare(AccountList[i].AccountID, accountID, StringComparison.OrdinalIgnoreCase) == 0)
                        return AccountList[i];

                return null;
        }
        public List<AccountInfo> MatchAccounts(string accountID)
        {
                if (string.IsNullOrEmpty(accountID)) return new List<AccountInfo>(AccountList);

                List<AccountInfo> list = new List<AccountInfo>();

                for (int i = 0; i < AccountList.Count; i++)
                    if (AccountList[i].AccountID.IndexOf(accountID, StringComparison.OrdinalIgnoreCase) >= 0)
                        list.Add(AccountList[i]);

            return list;
        }

        public void CreateAccountInfo()
        {
            AccountList.Add(new AccountInfo {Index = ++NextAccountID});
        }

        public void CreateMapInfo()
        {
            MapInfoList.Add(new MapInfo {Index = ++MapIndex});
        }

        public void CreateItemInfo(ItemType type = ItemType.Nothing)
        {
            ItemInfoList.Add(new ItemInfo { Index = ++ItemIndex, Type = type });
        }

        public void CreateMonsterInfo()
        {
            MonsterInfoList.Add(new MonsterInfo {Index = ++MonsterIndex});
        }

        public void Remove(MapInfo info)
        {
            MapInfoList.Remove(info);
            //Desync all objects\
        }
        public void Remove(ItemInfo info)
        {
            ItemInfoList.Remove(info);
        }
        public void Remove(MonsterInfo info)
        {
            MonsterInfoList.Remove(info);
            //Desync all objects\
        }


        public UserItem CreateFreshItem(ItemInfo info)
        {
            return new UserItem(info)
                {
                    UniqueID = ++NextUserItemID,
                    CurrentDura = info.Durability,
                    MaxDura = info.Durability,
                };
        }
        public UserItem CreateDropItem(int index)
        {
            return CreateDropItem(GetItemInfo(index));
        }
        public UserItem CreateDropItem(ItemInfo info)
        {
            if (info == null) return null;

            UserItem item = new UserItem(info)
                {
                    UniqueID = ++NextUserItemID,
                    MaxDura = info.Durability,
                    CurrentDura = (ushort) Math.Min(info.Durability, Random.Next(info.Durability) + 1000)
                };

            switch (info.Type)
            {
                case ItemType.Weapon:
                    UpgradeWeapon(item);
                    break;
                case ItemType.Armour:
                    UpgradeArmour(item);
                    break;
                case ItemType.Helmet:
                    UpgradeHelmet(item);
                    break;
                case ItemType.Belt:
                case ItemType.Boots:
                    UpgradeBeltBoot(item);
                    break;
                case ItemType.Necklace:
                    UpgradeNecklace(item);
                    break;
                case ItemType.Bracelet:
                    UpgradeBracelet(item);
                    break;
                case ItemType.Ring:
                    UpgradeRing(item);
                    break;
            }

            return item;
        }

        public void UpgradeArmour(UserItem item)
        {
            if (Random.Next(15) == 0) item.AC = (byte)(RandomomRange(6, 15) + 1);
            if (Random.Next(15) == 0) item.MAC = (byte)(RandomomRange(6, 15) + 1);

            if (Random.Next(20) == 0) item.DC = (byte)(RandomomRange(6, 20) + 1);
            if (Random.Next(20) == 0) item.MC = (byte)(RandomomRange(6, 20) + 1);
            if (Random.Next(20) == 0) item.SC = (byte)(RandomomRange(6, 20) + 1);

            if (Random.Next(60) == 0) item.AttackSpeed = (sbyte)(RandomomRange(3, 30) + 1);

            if (Random.Next(2) == 0)
            {
                int dura = RandomomRange(6, 10);

                item.MaxDura = (ushort)Math.Min(ushort.MaxValue, item.MaxDura + dura * 1000);
                item.CurrentDura = (ushort)Math.Min(ushort.MaxValue, item.CurrentDura + dura * 1000);
            }
        }
        public void UpgradeBeltBoot(UserItem item)
        {
            if (Random.Next(15) == 0) item.AC = (byte)(RandomomRange(6, 15) + 1);
            if (Random.Next(15) == 0) item.MAC = (byte)(RandomomRange(6, 15) + 1);

            if (Random.Next(20) == 0) item.DC = (byte)(RandomomRange(6, 20) + 1);
            if (Random.Next(20) == 0) item.MC = (byte)(RandomomRange(6, 20) + 1);
            if (Random.Next(20) == 0) item.SC = (byte)(RandomomRange(6, 20) + 1);

            if (Random.Next(60) == 0) item.Agility = (byte)(RandomomRange(3, 30) + 1);

            if (Random.Next(2) == 0)
            {
                int dura = RandomomRange(6, 10);

                item.MaxDura = (ushort)Math.Min(ushort.MaxValue, item.MaxDura + dura * 1000);
                item.CurrentDura = (ushort)Math.Min(ushort.MaxValue, item.CurrentDura + dura * 1000);
            }
        }
        public void UpgradeHelmet(UserItem item)
        {
            if (Random.Next(15) == 0) item.AC = (byte)(RandomomRange(6, 15) + 1);
            if (Random.Next(15) == 0) item.MAC = (byte)(RandomomRange(6, 15) + 1);

            if (Random.Next(20) == 0) item.DC = (byte)(RandomomRange(6, 20) + 1);
            if (Random.Next(20) == 0) item.MC = (byte)(RandomomRange(6, 20) + 1);
            if (Random.Next(20) == 0) item.SC = (byte)(RandomomRange(6, 20) + 1);

            if (Random.Next(60) == 0) item.Accuracy = (byte)(RandomomRange(3, 30) + 1);

            if (Random.Next(2) == 0)
            {
                int dura = RandomomRange(6, 10);

                item.MaxDura = (ushort)Math.Min(ushort.MaxValue, item.MaxDura + dura * 1000);
                item.CurrentDura = (ushort)Math.Min(ushort.MaxValue, item.CurrentDura + dura * 1000);
            }
        }
        public void UpgradeWeapon(UserItem item)
        {
            if (Random.Next(15) == 0) item.DC = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.MC = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.SC = (byte)(RandomomRange(12, 15) + 1);

            if (Random.Next(30) == 0) item.Accuracy = (byte)(RandomomRange(3, 20) + 1);
            if (Random.Next(60) == 0) item.AttackSpeed = (sbyte)(RandomomRange(3, 30) + 1);

            if (Random.Next(2) == 0)
            {
                int dura = RandomomRange(6, 10);

                item.MaxDura = (ushort)Math.Min(ushort.MaxValue, item.MaxDura + dura * 2000);
                item.CurrentDura = (ushort)Math.Min(ushort.MaxValue, item.CurrentDura + dura * 2000);
            }
        }
        public void UpgradeRing(UserItem item)
        {
            if (Random.Next(25) == 0) item.AC = (byte)(RandomomRange(6, 20) + 1);
            if (Random.Next(25) == 0) item.MAC = (byte)(RandomomRange(6, 20) + 1);

            if (Random.Next(15) == 0) item.DC = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.MC = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.SC = (byte)(RandomomRange(12, 15) + 1);

            if (Random.Next(60) == 0) item.Accuracy = (byte)(RandomomRange(3, 30) + 1);

            if (Random.Next(2) == 0)
            {
                int dura = RandomomRange(3, 10);

                item.MaxDura = (ushort)Math.Min(ushort.MaxValue, item.MaxDura + dura * 1000);
                item.CurrentDura = (ushort)Math.Min(ushort.MaxValue, item.CurrentDura + dura * 1000);
            }
        }
        public void UpgradeBracelet(UserItem item)
        {
            if (Random.Next(25) == 0) item.AC = (byte)(RandomomRange(6, 20) + 1);
            if (Random.Next(25) == 0) item.MAC = (byte)(RandomomRange(6, 20) + 1);

            if (Random.Next(15) == 0) item.DC = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.MC = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.SC = (byte)(RandomomRange(12, 15) + 1);

            if (Random.Next(15) == 0) item.Accuracy = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.Agility = (byte)(RandomomRange(12, 15) + 1);

            if (Random.Next(60) == 0) item.HP = (byte)(RandomomRange(100, 5) + 5);

            if (Random.Next(2) == 0)
            {
                int dura = RandomomRange(3, 10);

                item.MaxDura = (ushort)Math.Min(ushort.MaxValue, item.MaxDura + dura * 1000);
                item.CurrentDura = (ushort)Math.Min(ushort.MaxValue, item.CurrentDura + dura * 1000);
            }
        }
        public void UpgradeNecklace(UserItem item)
        {
            if (Random.Next(15) == 0) item.DC = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.MC = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.SC = (byte)(RandomomRange(12, 15) + 1);

            if (Random.Next(15) == 0) item.Accuracy = (byte)(RandomomRange(12, 15) + 1);
            if (Random.Next(15) == 0) item.Agility = (byte)(RandomomRange(12, 15) + 1);

            if (Random.Next(60) == 0) item.Luck = (sbyte)(RandomomRange(3, 45) + 1);

            if (Random.Next(2) == 0)
            {
                int dura = RandomomRange(3, 10);

                item.MaxDura = (ushort)Math.Min(ushort.MaxValue, item.MaxDura + dura * 1000);
                item.CurrentDura = (ushort)Math.Min(ushort.MaxValue, item.CurrentDura + dura * 1000);
            }
        }

        public int RandomomRange(int count, int rate)
        {
            int x = 0;
            for (int i = 0; i < count; i++) if (Random.Next(rate) == 0) x++;
            return x;
        }
        public bool BindItem(UserItem item)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                ItemInfo info = ItemInfoList[i];
                if (info.Index != item.ItemIndex) continue;
                item.Info = info;
                return true;
            }
            return false;
        }


        public Map GetMap(int index)
        {
            for (int i = 0; i < MapList.Count; i++)
                if (MapList[i].Info.Index == index) return MapList[i];

            return null;
        }

        //public Map GetMapByName(string name)
        //{
        //    return MapList.FirstOrDefault(t => String.Equals(t.Info.FileName, name, StringComparison.CurrentCultureIgnoreCase));
        //}

        public Map GetMapByNameAndInstance(string name, int instanceValue = 0)
        {
            if (instanceValue < 0) instanceValue = 0;
            if (instanceValue > 0) instanceValue--;

            var instanceMapList = MapList.Where(t => String.Equals(t.Info.FileName, name, StringComparison.CurrentCultureIgnoreCase)).ToList();
            return instanceValue < instanceMapList.Count() ? instanceMapList[instanceValue] : null;
        }

        public MonsterInfo GetMonsterInfo(int index)
        {
            for (int i = 0; i < MonsterInfoList.Count; i++)
                if (MonsterInfoList[i].Index == index) return MonsterInfoList[i];

            return null;
        }
        public MonsterInfo GetMonsterInfo(string name)
        {
            for (int i = 0; i < MonsterInfoList.Count; i++)
            {
                MonsterInfo info = MonsterInfoList[i];
                if (info.Name != name && !info.Name.Replace(" ", "").StartsWith(name, StringComparison.OrdinalIgnoreCase)) continue;
                return info;
            }
            return null;
        }
        public PlayerObject GetPlayer(string name)
        {
            for (int i = 0; i < Players.Count; i++)
                if (String.Compare(Players[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return Players[i];

            return null;
        }
        public CharacterInfo GetCharacterInfo(string name)
        {
            for (int i = 0; i < CharacterList.Count; i++)
                if (String.Compare(CharacterList[i].Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                    return CharacterList[i];

            return null;
        }
        public ItemInfo GetItemInfo(int index)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                ItemInfo info = ItemInfoList[i];
                if (info.Index != index) continue;
                return info;
            }
            return null;
        }
        public ItemInfo GetItemInfo(string name)
        {
            for (int i = 0; i < ItemInfoList.Count; i++)
            {
                ItemInfo info = ItemInfoList[i];
                if (String.Compare(info.Name.Replace(" ", ""), name, StringComparison.OrdinalIgnoreCase) != 0) continue;
                return info;
            }
            return null;
        }

        public void MessageAccount(AccountInfo account, string message, ChatType type)
        {
            if (account == null) return;
            if (account.Characters == null) return;

            for (int i = 0; i < account.Characters.Count; i++)
            {
                if (account.Characters[i].Player == null) continue;
                account.Characters[i].Player.ReceiveChat(message, type);
                return;
            }
        }
    }
}


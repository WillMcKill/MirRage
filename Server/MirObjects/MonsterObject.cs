﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Server.MirDatabase;
using Server.MirEnvir;
using Server.MirObjects.Monsters;
using S = ServerPackets;

namespace Server.MirObjects
{
    public class MonsterObject : MapObject
    {
        public static MonsterObject GetMonster(MonsterInfo info)
        {
            if (info == null) return null;

            switch (info.AI)
            {
                case 1:
                case 2:
                    return new Deer(info);
                case 3:
                    return new Tree(info);
                case 4:
                    return new SpittingSpider(info); //Spider
                case 5:
                    return new CannibalPlant(info);
                case 6:
                    return new Guard(info);
                case 7:
                    return new CaveMaggot(info);
                case 8:
                    return new AxeSkeleton(info);
                case 9:
                    return new HarvestMonster(info);
                case 10:
                    return new FlamingWooma(info);
                case 11:
                    return new WoomaTaurus(info);
                case 12:
                    return new BugBagMaggot(info);
                case 13:
                    return new RedMoonEvil(info);
                case 14:
                    return new EvilCentipede(info);
                case 15:
                    return new ZumaMonster(info);
                case 16:
                    return new RedThunderZuma(info);
                case 17:
                    return new ZumaTaurus(info);
                case 18:
                    return new Shinsu(info);
                case 19:
                    return new KingScorpion(info);
                case 20:
                    return new DarkDevil(info);
                case 21:
                    return new IncarnatedGhoul(info);
                case 22:
                    return new IncarnatedZT(info);
                case 23:
                    return new BoneFamiliar(info);
                case 24:
                    return new DigOutZombie(info);
                case 25:
                    return new RevivingZombie(info);
                case 26:
                    return new ShamanZombie(info);
                case 27:
                    return new Khazard(info);
                case 28:
                    return new ToxicGhoul(info);
                case 29:
                    return new BoneSpearman(info);
                case 30:
                    return new BoneLord(info);
                case 31:
                    return new RightGuard(info);
                case 32:
                    return new LeftGuard(info);
                case 33:
                    return new MinotaurKing(info);
                case 34:
                    return new FrostTiger(info);
                case 35:
                    return new SandWorm(info);
                case 36:
                    return new Yimoogi(info);
                case 37:
                    return new CrystalSpider(info);
                case 38:
                    return new HolyDeva(info);
                case 39:
                    return new RootSpider(info);
                case 40:
                    return new BombSpider(info);
                case 41:
                case 42:
                    return new YinDevilNode(info);
                case 43:
                    return new OmaKing(info);
                case 44:
                    return new BlackFoxman(info);
                case 45:
                    return new RedFoxman(info);
                case 46:
                    return new WhiteFoxman(info);
                case 47:
                    return new TrapRock(info);
                case 48:
                    return new GuardianRock(info);
                case 49:
                    return new ThunderElement(info);
                case 50:
                    return new GreatFoxSpirit(info);
                case 51:
                    return new HedgeKekTal(info);
                case 52:
                    return new EvilMir(info);
                case 53:
                    return new EvilMirBody(info);
                case 54:
                    return new DragonStatue(info);
                case 55:
                    return new HumanWizard(info);
                default:
                    return new MonsterObject(info);
            }
        }

        public override ObjectType Race
        {
            get { return ObjectType.Monster; }
        }

        public MonsterInfo Info;
        public MapRespawn Respawn;
        
        public override string Name
        {
            get { return Master == null ? Info.GameName : string.Format("{0}({1})", Info.GameName, Master.Name); }
            set { throw new NotSupportedException(); }
        }

        public override int CurrentMapIndex { get; set; }
        public override Point CurrentLocation { get; set; }
        public override sealed MirDirection Direction { get; set; }
        public override byte Level
        {
            get { return Info.Level; }
            set { throw new NotSupportedException(); }
        }

        public override sealed AttackMode AMode
        {
            get
            {
                return base.AMode;
            }
            set
            {
                base.AMode = value;
            }
        }
        public override sealed PetMode PMode
        {
            get
            {
                return base.PMode;
            }
            set
            {
                base.PMode = value;
            }
        }

        public override uint Health
        {
            get { return HP; }
        }

        public override uint MaxHealth
        {
            get { return MaxHP; }
        }

        public uint HP, MaxHP;
        public ushort MoveSpeed;

        public const int RegenDelay = 10000, EXPOwnerDelay = 5000, DeadDelay = 180000, SearchDelay = 3000, RoamDelay = 1000, HealDelay = 600, RevivalDelay = 2000;
        public long ActionTime, MoveTime, AttackTime, RegenTime, DeadTime, SearchTime, RoamTime, HealTime;
        public long ShockTime, RageTime;

        public byte PetLevel;
        public uint PetExperience;
        public byte MaxPetLevel;


        public List<MonsterObject> SlaveList = new List<MonsterObject>();
        

        public override bool Blocking
        {
            get
            {
                return !Dead;
            }
        }
        protected virtual bool CanRegen
        {
            get { return Envir.Time >= RegenTime; }
        }
        protected virtual bool CanMove
        {
            get
            {
                return !Dead && Envir.Time > MoveTime && Envir.Time > ActionTime && Envir.Time > ShockTime &&
                       (Master == null || Master.PMode == PetMode.MoveOnly || Master.PMode == PetMode.Both) && CurrentPoison != PoisonType.Paralysis
                      && CurrentPoison != PoisonType.Stun && CurrentPoison != PoisonType.Frozen;
            }
        }
        protected virtual bool CanAttack
        {
            get
            {
                return !Dead && Envir.Time > AttackTime && Envir.Time > ActionTime &&
                     (Master == null || Master.PMode == PetMode.AttackOnly || Master.PMode == PetMode.Both) && CurrentPoison != PoisonType.Paralysis
                      && CurrentPoison != PoisonType.Stun && CurrentPoison != PoisonType.Frozen;
            }
        }


        protected internal MonsterObject(MonsterInfo info)
        {
            Info = info;

            Undead = Info.Undead;
            AutoRev = info.AutoRev;
            CoolEye = info.CoolEye > Envir.Random.Next(100);
            Direction = (MirDirection)Envir.Random.Next(8);

            AMode = AttackMode.All;
            PMode = PetMode.Both;

            RegenTime = Envir.Random.Next(RegenDelay) + Envir.Time;
            SearchTime = Envir.Random.Next(SearchDelay) + Envir.Time;
            RoamTime = Envir.Random.Next(RoamDelay) + Envir.Time;
        }
        public bool Spawn(Map temp, Point location)
        {
            if (!temp.ValidPoint(location)) return false;

            CurrentMap = temp;
            CurrentLocation = location;

            CurrentMap.AddObject(this);

            RefreshAll();
            SetHP(MaxHP);

            Spawned();
            Envir.MonsterCount++;

            return true;
        }
        public void Spawn(MapRespawn respawn)
        {
            Respawn = respawn;

            if (Respawn.Map == null) return;

            for (int i = 0; i < 10; i++)
            {
                CurrentLocation = new Point(Respawn.Info.Location.X + Envir.Random.Next(-Respawn.Info.Spread, Respawn.Info.Spread + 1),
                                            Respawn.Info.Location.Y + Envir.Random.Next(-Respawn.Info.Spread, Respawn.Info.Spread + 1));
                
                if (!respawn.Map.ValidPoint(CurrentLocation)) continue;

                respawn.Map.AddObject(this);

                CurrentMap = respawn.Map;

                RefreshAll();
                SetHP(MaxHP);

                Spawned();
                Respawn.Count++;
                Envir.MonsterCount++;
                return;
            }
        }

        public override void Spawned()
        {
            base.Spawned();
            ActionTime = Envir.Time + 2000;
        }

        private void RefreshBase()
        {
            MaxHP = Info.HP;
            MinAC = Info.MinAC;
            MaxAC = Info.MaxAC;
            MinMAC = Info.MinMAC;
            MaxMAC = Info.MaxMAC;
            MinDC = Info.MinDC;
            MaxDC = Info.MaxDC;
            MinMC = Info.MinMC;
            MaxMC = Info.MaxMC;
            MinSC = Info.MinSC;
            MaxSC = Info.MaxSC;
            Accuracy = Info.Accuracy;
            Agility = Info.Agility;

            MoveSpeed = Info.MoveSpeed;
            AttackSpeed = Info.AttackSpeed;
        }
        public virtual void RefreshAll()
        {
            RefreshBase();
            
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + PetLevel * 20);
                MinAC = (byte)Math.Min(byte.MaxValue, MinAC + PetLevel * 2);
                MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + PetLevel * 2);
                MinMAC = (byte)Math.Min(byte.MaxValue, MinMAC + PetLevel * 2);
                MaxMAC = (byte)Math.Min(byte.MaxValue, MaxMAC + PetLevel * 2);
                MinDC = (byte)Math.Min(byte.MaxValue, MinDC + PetLevel);
                MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + PetLevel);

            if (Info.Name == Settings.SkeletonName ||Info.Name == Settings.ShinsuName ||Info.Name == Settings.AngelName) 
            {
                MoveSpeed = (ushort)(MoveSpeed - MaxPetLevel * 130);
                AttackSpeed = (ushort)( AttackSpeed - MaxPetLevel * 70);
            }

            if (MoveSpeed < 400) MoveSpeed = 400;
            if (AttackSpeed < 400) AttackSpeed = 400;

            RefreshBuffs();
        }
        private void RefreshBuffs()
        {
            for (int i = 0; i < Buffs.Count; i++)
            {
                Buff buff = Buffs[i];

                switch (buff.Type)
                {
                    case BuffType.Haste:
                        ASpeed = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, ASpeed + buff.Value)));
                        break;
                    case BuffType.LightBody:
                        Agility = (byte)Math.Min(byte.MaxValue, Agility + buff.Value);
                        break;
                    case BuffType.SoulShield:
                        MaxMAC = (byte)Math.Min(byte.MaxValue, MaxMAC + buff.Value);
                        break;
                    case BuffType.BlessedArmour:
                        MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + buff.Value);
                        break;
                    case BuffType.UltimateEnhancer:
                        MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + buff.Value);
                        break;
                }

            }
        }
        public void RefreshNameColour(bool send = true)
        {

            Color colour = Color.White;
            
            switch (PetLevel)
            {
                case 1:
                    colour = Color.Aqua;
                    break;
                case 2:
                    colour = Color.Aquamarine;
                    break;
                case 3:
                    colour = Color.LightSeaGreen;
                    break;
                case 4:
                    colour = Color.SlateBlue;
                    break;
                case 5:
                    colour = Color.SteelBlue;
                    break;
                case 6:
                    colour = Color.Blue;
                    break;
                case 7:
                    colour = Color.Navy;
                    break;
            }

            if (Envir.Time < ShockTime)
                colour = Color.Peru;
            else if (Envir.Time < RageTime)
                colour = Color.Red;

            if (colour == NameColour || !send) return;

            NameColour = colour;

            Broadcast(new S.ObjectColourChanged { ObjectID = ObjectID, NameColour = NameColour });
        }

        public void SetHP(uint amount)
        {
            if (HP == amount) return;

            HP = amount <= MaxHP ? amount : MaxHP;

            if (!Dead && HP == 0) Die();

            //  HealthChanged = true;
            BroadcastHealthChange();
        }
        public virtual void ChangeHP(int amount)
        {
            uint value = (uint)Math.Max(uint.MinValue, Math.Min(MaxHP, HP + amount));

            if (value == HP) return;

            HP = value;

            if (!Dead && HP == 0) Die();

           // HealthChanged = true;
            BroadcastHealthChange();
        }
        public override void Die()
        {
            if (Dead) return;

            HP = 0;
            Dead = true;

            DeadTime = Envir.Time + DeadDelay;

            Broadcast(new S.ObjectDied { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

            if (EXPOwner != null && Master == null && EXPOwner.Race == ObjectType.Player) EXPOwner.WinExp(Info.Experience);

            if (Respawn != null)
                Respawn.Count--;

            if (Master == null)
                 Drop();

            PoisonList.Clear();
            Envir.MonsterCount--;

        }

        public void Revive(uint hp)
        {
            if (!Dead) return;

            SetHP(hp);

            Dead = false;
            ActionTime = Envir.Time + RevivalDelay;

            Broadcast(new S.ObjectRevived { ObjectID = ObjectID, Effect = false });

            if (Respawn != null)
                Respawn.Count++;

            Envir.MonsterCount++;

        }

        public override int Pushed(MapObject pusher, MirDirection dir, int distance)
        {
            if (!Info.CanPush) return 0;

            int result = 0;
            MirDirection reverse = Functions.ReverseDirection(dir);
            for (int i = 0; i < distance; i++)
            {
                Point location = Functions.PointMove(CurrentLocation, dir, 1);

                if (!CurrentMap.ValidPoint(location)) return result;

                Cell cell = CurrentMap.GetCell(location);

                bool stop = false;
                if (cell.Objects != null)
                    for (int c = 0; c < cell.Objects.Count; c++)
                    {
                        MapObject ob = cell.Objects[c];
                        if (!ob.Blocking) continue;
                        stop = true;
                    }
                if (stop) break;

                CurrentMap.GetCell(CurrentLocation).Remove(this);

                Direction = reverse;
                RemoveObjects(dir, 1);
                CurrentLocation = location;
                CurrentMap.GetCell(CurrentLocation).Add(this);
                AddObjects(dir, 1);

                Broadcast(new S.ObjectPushed { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });

                result++;
            }

            ActionTime = Envir.Time + 300 * result;
            MoveTime = Envir.Time + 500 * result;

            if (result > 0)
            {
                Cell cell = CurrentMap.GetCell(CurrentLocation);

                for (int i = 0; i < cell.Objects.Count; i++)
                {
                    if (cell.Objects[i].Race != ObjectType.Spell) continue;
                    SpellObject ob = (SpellObject)cell.Objects[i];

                    ob.ProcessSpell(this);
                    break;
                }
            }

            return result;
        }

        protected virtual void Drop()
        {
            for (int i = 0; i < Info.Drops.Count; i++)
            {
                DropInfo drop = Info.Drops[i];

                int rate = (int)(drop.Chance / Settings.DropRate); if (rate < 1) rate = 1;
                if (Envir.Random.Next(rate) != 0) continue;

                if (drop.Gold > 0)
                {
                    int gold = Envir.Random.Next((int)(drop.Gold / 2), (int)(drop.Gold + drop.Gold / 2)); //Messy

                    if (gold <= 0) continue;

                    if (!DropGold((uint)gold)) return;
                }
                else
                {
                    UserItem item = Envir.CreateDropItem(drop.Item);
                    if (item == null) continue;

                    if (!DropItem(item)) return;
                }
            }

        }
        protected virtual bool DropItem(UserItem item)
        {
            if (CurrentMap.Info.NoDropMonster) return false;

            ItemObject ob = new ItemObject(this, item)
            {
                Owner = EXPOwner,
                OwnerTime = Envir.Time + Settings.Minute,
            };

            return ob.Drop(Settings.DropRange);
        }

        protected virtual bool DropGold(uint gold)
        {
            if (EXPOwner != null && EXPOwner.CanGainGold(gold))
            {
                EXPOwner.WinGold(gold);
                return true;
            }

            ItemObject ob = new ItemObject(this, gold)
            {
                Owner = EXPOwner,
                OwnerTime = Envir.Time + Settings.Minute,
            };

            return ob.Drop(Settings.DropRange);
        }

        public override void Process()
        {
            base.Process();

            RefreshNameColour();

            if (Target != null && (Target.CurrentMap != CurrentMap || !Target.IsAttackTarget(this) || !Functions.InRange(CurrentLocation, Target.CurrentLocation, Globals.DataRange)))
                Target = null;

            for (int i = SlaveList.Count - 1; i >= 0; i--)
                if (SlaveList[i].Dead || SlaveList[i].Node == null)
                    SlaveList.RemoveAt(i);

            if (Dead && Envir.Time >= DeadTime)
            {
                CurrentMap.RemoveObject(this);
                if (Master != null)
                {
                    Master.Pets.Remove(this);
                    Master = null;
                }

                Despawn();
                return;
            }

            ProcessAI();

            ProcessBuffs();
            ProcessRegen();
            ProcessPoison();


         /*   if (!HealthChanged) return;

            HealthChanged = false;
            
            BroadcastHealthChange();*/
        }

        public override void SetOperateTime()
        {
            long time = Envir.Time + 2000;

            if (DeadTime < time && DeadTime > Envir.Time)
                time = DeadTime;

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

            if (SearchTime < time && SearchTime > Envir.Time)
                time = SearchTime;

            if (RoamTime < time && RoamTime > Envir.Time)
                time = RoamTime;


            if (ShockTime < time && ShockTime > Envir.Time)
                time = ShockTime;

            if (RegenTime < time && RegenTime > Envir.Time && Health < MaxHealth)
                time = RegenTime;

            if (RageTime < time && RageTime > Envir.Time)
                time = RageTime;

            if (ActionTime < time && ActionTime > Envir.Time)
                time = ActionTime;

            if (MoveTime < time && MoveTime > Envir.Time)
                time = MoveTime;

            if (AttackTime < time && AttackTime > Envir.Time)
                time = AttackTime;

            if (HealTime < time && HealTime > Envir.Time && HealAmount > 0)
                time = HealTime;

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

        public override void Process(DelayedAction action)
        {
            switch (action.Type)
            {
                case DelayedType.Damage:
                    CompleteAttack(action.Params);
                    break;
                case DelayedType.Recall:
                    PetRecall();
                    break;

            }
        }

        public void PetRecall()
        {
            if (!Teleport(Master.CurrentMap, Master.Back))
                Teleport(Master.CurrentMap, Master.CurrentLocation);
        }
        protected virtual void CompleteAttack(IList<object> data)
        {
            MapObject target = (MapObject)data[0];
            int damage = (int)data[1];
            DefenceType defence = (DefenceType)data[2];

            if (target == null || !target.IsAttackTarget(this) || target.CurrentMap != CurrentMap || target.Node == null) return;

            target.Attacked(this, damage, defence);
        }

        protected virtual void ProcessRegen()
        {
            if (Dead) return;

            int healthRegen = 0;

            if (CanRegen)
            {
                RegenTime = Envir.Time + RegenDelay;


                if (HP < MaxHP)
                    healthRegen += (int)(MaxHP * 0.05F) + 1;
            }


            if (Envir.Time > HealTime)
            {
                HealTime = Envir.Time + HealDelay;

                if (HealAmount > 5)
                {
                    healthRegen += 5;
                    HealAmount -= 5;
                }
                else
                {
                    healthRegen += HealAmount;
                    HealAmount = 0;
                }
            }

            if (healthRegen > 0) ChangeHP(healthRegen);
            if (HP == MaxHP) HealAmount = 0;
        }
        protected virtual void ProcessPoison()
        {
            PoisonType type = PoisonType.None;
            PoisonRate = 1F;

            for (int i = PoisonList.Count - 1; i >= 0; i--)
            {
                Poison poison = PoisonList[i];
                if (poison.Owner != null && poison.Owner.Node == null)
                {
                    PoisonList.RemoveAt(i);
                    continue;
                }

                if (Envir.Time > poison.TickTime)
                {
                    poison.Time++;
                    poison.TickTime = Envir.Time + poison.TickSpeed;

                    if (poison.Time >= poison.Duration)
                        PoisonList.RemoveAt(i);

                    if (poison.PType == PoisonType.Green)
                    {
                        if (EXPOwner == null || EXPOwner.Dead)
                        {
                            EXPOwner = poison.Owner;
                            EXPOwnerTime = Envir.Time + EXPOwnerDelay;
                        }
                        else if (EXPOwner == poison.Owner)
                            EXPOwnerTime = Envir.Time + EXPOwnerDelay;

                        ChangeHP(-poison.Value);
                        if (Dead) break;
                        RegenTime = Envir.Time + RegenDelay;
                    }
                }

                switch (poison.PType)
                {
                    case PoisonType.Red:
                        PoisonRate -= 0.5F;
                        break;
                    case PoisonType.Stun:
                        PoisonRate -= 0.5F;
                        break;
                }

                if ((int)type < (int)poison.PType)
                    type = poison.PType;
            }

            
            if (type == CurrentPoison) return;

            CurrentPoison = type;
            Broadcast(new S.ObjectPoisoned { ObjectID = ObjectID, Poison = type });
        }

        private void ProcessBuffs()
        {
            bool refresh = false;
            for (int i = Buffs.Count - 1; i >= 0; i--)
            {

                Buff buff = Buffs[i];

                if (Envir.Time <= buff.ExpireTime) continue;

                Buffs.RemoveAt(i);

                switch (buff.Type)
                {
                    case BuffType.Hiding:
                        Hidden = false;
                        break;
                }

                refresh = true;
            }

            if (refresh) RefreshAll();
        }
        protected virtual void ProcessAI()
        {
            if (Dead) return;

            if (Master != null)
            {
                if ((Master.PMode == PetMode.Both || Master.PMode == PetMode.MoveOnly))
                {
                    if (!Functions.InRange(CurrentLocation, Master.CurrentLocation, Globals.DataRange) || CurrentMap != Master.CurrentMap)
                        PetRecall();
                }

                if (Master.PMode == PetMode.MoveOnly || Master.PMode == PetMode.None)
                    Target = null;
            }

            ProcessSearch();
            ProcessRoam();
            ProcessTarget();
        }
        protected virtual void ProcessSearch()
        {
            if (Envir.Time < SearchTime) return;
            if (Master != null && (Master.PMode == PetMode.MoveOnly || Master.PMode == PetMode.None)) return;

            SearchTime = Envir.Time + SearchDelay;

            //Stacking or Infront of master - Move
            bool stacking = false;

            Cell cell = CurrentMap.GetCell(CurrentLocation);

            if (cell.Objects != null)
                for (int i = 0; i < cell.Objects.Count; i++)
                {
                    MapObject ob = cell.Objects[i];
                    if (ob == this || !ob.Blocking) continue;
                    stacking = true;
                    break;
                }

            if (CanMove && ((Master != null && Master.Front == CurrentLocation) || stacking))
            {
                //Walk Randomly
                if (!Walk(Direction))
                {
                    MirDirection dir = Direction;

                    switch (Envir.Random.Next(3)) // favour Clockwise
                    {
                        case 0:
                            for (int i = 0; i < 7; i++)
                            {
                                dir = Functions.NextDir(dir);

                                if (Walk(dir))
                                    break;
                            }
                            break;
                        default:
                            for (int i = 0; i < 7; i++)
                            {
                                dir = Functions.PreviousDir(dir);

                                if (Walk(dir))
                                    break;
                            }
                            break;
                    }
                }
            }

            if (Target == null || Envir.Random.Next(3) == 0)
                FindTarget();
        }
        protected virtual void ProcessRoam()
        {
            if (Target != null || Envir.Time < RoamTime) return;

            if (Master != null)
            {
                MoveTo(Master.Back);
                return;
            }

            RoamTime = Envir.Time + RoamDelay;
            if (Envir.Random.Next(10) != 0) return;

            switch (Envir.Random.Next(3)) //Face Walk
            {
                case 0:
                    Turn((MirDirection)Envir.Random.Next(8));
                    break;
                default:
                    Walk(Direction);
                    break;
            }
        }
        protected virtual void ProcessTarget()
        {
            if (Target == null || !CanAttack) return;

            if (InAttackRange())
            {
                Attack();
                if (Target.Dead)
                    FindTarget();

                return;
            }

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }
            
            MoveTo(Target.CurrentLocation);
        }
        protected virtual bool InAttackRange()
        {
            if (Target.CurrentMap != CurrentMap) return false;

            return Target.CurrentLocation != CurrentLocation && Functions.InRange(CurrentLocation, Target.CurrentLocation, 1);
        }
        protected virtual void FindTarget()
        {
            for (int d = 0; d <= Info.ViewRange; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d*2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.GetCell(x, y);
                        if (!cell.Valid || cell.Objects == null) continue;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObject ob = cell.Objects[i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (ob.Hidden && (!CoolEye || Level < ob.Level)) continue;
                                    if (this is TrapRock && ob.InTrapRock) continue;
                                    Target = ob;
                                    return;
                                case ObjectType.Player:
                                    PlayerObject playerob = (PlayerObject)ob;
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (playerob.GMGameMaster || ob.Hidden && (!CoolEye || Level < ob.Level)) continue;
                                    Target = ob;
                                    return;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
        }

        protected virtual void MoveTo(Point location)
        {
            if (CurrentLocation == location) return;

            bool inRange = Functions.InRange(location, CurrentLocation, 1);

            if (inRange)
            {
                if (!CurrentMap.ValidPoint(location)) return;
                Cell cell = CurrentMap.GetCell(location);
                if (cell.Objects != null)
                    for (int i = 0; i < cell.Objects.Count; i++)
                    {
                        MapObject ob = cell.Objects[i];
                        if (!ob.Blocking) continue;
                        return;
                    }
            }

            MirDirection dir = Functions.DirectionFromPoint(CurrentLocation, location);

            if (Walk(dir)) return;

            switch (Envir.Random.Next(2)) //No favour
            {
                case 0:
                    for (int i = 0; i < 7; i++)
                    {
                        dir = Functions.NextDir(dir);

                        if (Walk(dir))
                            return;
                    }
                    break;
                default:
                    for (int i = 0; i < 7; i++)
                    {
                        dir = Functions.PreviousDir(dir);

                        if (Walk(dir))
                            return;
                    }
                    break;
            }
        }

        public virtual void Turn(MirDirection dir)
        {
            if (!CanMove) return;

            Direction = dir;
                
            InSafeZone = CurrentMap.GetSafeZone(CurrentLocation) != null;


            Cell cell = CurrentMap.GetCell(CurrentLocation);

            for (int i = 0; i < cell.Objects.Count; i++)
            {
                if (cell.Objects[i].Race != ObjectType.Spell) continue;
                SpellObject ob = (SpellObject)cell.Objects[i];

                ob.ProcessSpell(this);
                break;
            }


            Broadcast(new S.ObjectTurn { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });
        }


        public virtual bool Walk(MirDirection dir) 
        {
            if (!CanMove) return false;

            Point location = Functions.PointMove(CurrentLocation, dir, 1);

            if (!CurrentMap.ValidPoint(location)) return false;

            Cell cell = CurrentMap.GetCell(location);

            if (cell.Objects != null)
            for (int i = 0; i < cell.Objects.Count; i++)
            {
                MapObject ob = cell.Objects[i];
                if (!ob.Blocking) continue;

                return false;
            }

            CurrentMap.GetCell(CurrentLocation).Remove(this);

            Direction = dir;
            RemoveObjects(dir, 1);
            CurrentLocation = location;
            CurrentMap.GetCell(CurrentLocation).Add(this);
            AddObjects(dir, 1);

            if (Hidden)
            {
                Hidden = false;

                for (int i = 0; i < Buffs.Count; i++)
                {
                    if (Buffs[i].Type != BuffType.Hiding) continue;

                    Buffs[i].ExpireTime = 0;
                    break;
                }
            }


            CellTime = Envir.Time + 500;
            ActionTime = Envir.Time + 300;
            MoveTime = Envir.Time + MoveSpeed;
            AttackTime = MoveTime;

            InSafeZone = CurrentMap.GetSafeZone(CurrentLocation) != null;

            Broadcast(new S.ObjectWalk { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });


            cell = CurrentMap.GetCell(CurrentLocation);

            for (int i = 0; i < cell.Objects.Count; i++)
            {
                if (cell.Objects[i].Race != ObjectType.Spell) continue;
                SpellObject ob = (SpellObject)cell.Objects[i];

                ob.ProcessSpell(this);
                break;
            }

            return true;
        }
        protected virtual void Attack()
        {
            ShockTime = 0;

            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }


            Direction = Functions.DirectionFromPoint(CurrentLocation, Target.CurrentLocation);
            Broadcast(new S.ObjectAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation });


            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;

            int damage = GetAttackPower(MinDC, MaxDC);
            if (damage == 0) return;

            Target.Attacked(this, damage);
        }


        public bool FindNearby(int distance)
        {
            for (int d = 0; d <= distance; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;
                        if (!CurrentMap.ValidPoint(x, y)) continue;
                        Cell cell = CurrentMap.GetCell(x, y);
                        if (cell.Objects == null) continue;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObject ob = cell.Objects[i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                case ObjectType.Player:
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (ob.Race == ObjectType.Player)
                                    {
                                        PlayerObject player = ((PlayerObject)ob);
                                        if (player.GMGameMaster) continue;
                                    }
                                    return true;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }

            return false;
        }
        public bool FindFriendsNearby(int distance)
        {
            for (int d = 0; d <= distance; d++)
            {
                for (int y = CurrentLocation.Y - d; y <= CurrentLocation.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = CurrentLocation.X - d; x <= CurrentLocation.X + d; x += Math.Abs(y - CurrentLocation.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;
                        if (!CurrentMap.ValidPoint(x, y)) continue;
                        Cell cell = CurrentMap.GetCell(x, y);
                        if (cell.Objects == null) continue;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObject ob = cell.Objects[i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                case ObjectType.Player:
                                    if (ob == this || ob.Dead) continue;
                                    if (ob.IsAttackTarget(this)) continue;
                                    if (ob.Race == ObjectType.Player)
                                    {
                                        PlayerObject player = ((PlayerObject)ob);
                                        if (player.GMGameMaster) continue;
                                    }
                                    return true;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }

            return false;
        }
        protected List<MapObject> FindAllTargets(int dist, Point location)
        {
            List<MapObject> targets = new List<MapObject>();
            for (int d = 0; d <= dist; d++)
            {
                for (int y = location.Y - d; y <= location.Y + d; y++)
                {
                    if (y < 0) continue;
                    if (y >= CurrentMap.Height) break;

                    for (int x = location.X - d; x <= location.X + d; x += Math.Abs(y - location.Y) == d ? 1 : d * 2)
                    {
                        if (x < 0) continue;
                        if (x >= CurrentMap.Width) break;

                        Cell cell = CurrentMap.GetCell(x, y);
                        if (!cell.Valid || cell.Objects == null) continue;

                        for (int i = 0; i < cell.Objects.Count; i++)
                        {
                            MapObject ob = cell.Objects[i];
                            switch (ob.Race)
                            {
                                case ObjectType.Monster:
                                case ObjectType.Player:
                                    if (!ob.IsAttackTarget(this)) continue;
                                    if (ob.Hidden && (!CoolEye || Level < ob.Level)) continue;
                                    if (ob.Race == ObjectType.Player)
                                    {
                                        PlayerObject player = ((PlayerObject)ob);
                                        if (player.GMGameMaster) continue;
                                    }
                                    targets.Add(ob);
                                    continue;
                                default:
                                    continue;
                            }
                        }
                    }
                }
            }
            return targets;
        }

        public override bool IsAttackTarget(PlayerObject attacker)
        {
            if (attacker == null || attacker.Node == null) return false;
            if (Dead) return false;
            if (Master == null) return true;
            if (attacker.AMode == AttackMode.Peace) return false;
            if (Master == attacker) return attacker.AMode == AttackMode.All;
            if (Master.Race == ObjectType.Player && (attacker.InSafeZone || InSafeZone)) return false;

            switch (attacker.AMode)
            {
                case AttackMode.Group:
                    return Master.GroupMembers == null || !Master.GroupMembers.Contains(attacker);
                case AttackMode.Guild:
                    //TODO GuildWars
                    return true;
                case AttackMode.RedBrown:
                    return Master.PKPoints >= 200 || Envir.Time < Master.BrownTime;
                default:
                    return true;
            }
        }
        public override bool IsAttackTarget(MonsterObject attacker)
        {
            if (attacker == null || attacker.Node == null) return false;
            if (Dead || attacker == this) return false;

            if (attacker.Info.AI == 6) // Guard
            {
                if (Info.AI != 1 && Info.AI != 2 && (Master == null || Master.PKPoints >= 200)) //Not Dear/Hen or Red Master 
                    return true;
            }
            else if (Master != null) //Pet Attacked
            {
                if (attacker.Master == null) //Wild Monster
                    return true;

                //Pet Vs Pet
                if (Master == attacker.Master)
                    return false;

                if (Envir.Time < ShockTime) //Shocked
                    return false;

                if (Master.Race == ObjectType.Player && attacker.Master.Race == ObjectType.Player && (Master.InSafeZone || attacker.Master.InSafeZone)) return false;

                switch (attacker.Master.AMode)
                {
                    case AttackMode.Group:
                        if (Master.GroupMembers != null && Master.GroupMembers.Contains((PlayerObject)attacker.Master)) return false;
                        break;
                    case AttackMode.Guild:
                        break;
                    case AttackMode.RedBrown:
                        if (attacker.Master.PKPoints < 200 || Envir.Time > attacker.Master.BrownTime) return false;
                        break;
                    case AttackMode.Peace:
                        return false;
                }

                for (int i = 0; i < Master.Pets.Count; i++)
                    if (Master.Pets[i].EXPOwner == attacker.Master) return true;

                return Master.LastHitter == attacker.Master;
            }
            else if (attacker.Master != null) //Pet Attacking Wild Monster
            {
                if (Envir.Time < ShockTime) //Shocked
                    return false;

                for (int i = 0; i < attacker.Master.Pets.Count; i++)
                {
                    MonsterObject ob = attacker.Master.Pets[i];
                    if (ob == Target || ob.Target == this) return true;
                }

                if (Target == attacker.Master )
                    return true;
            }

            return Envir.Time < attacker.RageTime;
        }
        public override bool IsFriendlyTarget(PlayerObject ally)
        {
            if (Master == null) return false;
            if (Master == ally) return true;

            switch (ally.AMode)
            {
                case AttackMode.Group:
                    return Master.GroupMembers != null && Master.GroupMembers.Contains(ally);
                case AttackMode.Guild:
                    return false;
                case AttackMode.RedBrown:
                    return Master.PKPoints < 200 & Envir.Time > Master.BrownTime;
            }
            return true;
        }

        public override bool IsFriendlyTarget(MonsterObject ally)
        {
            if (Master != null) return false;
            if (ally.Race != ObjectType.Monster) return false;
            if (ally.Master != null) return false;

            return true;
        }

        public override int Attacked(PlayerObject attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            if (Target == null && attacker.IsAttackTarget(this))
                Target = attacker;

            int armour = 0;

            switch (type)
            {
                case DefenceType.ACAgility:
                    if (Envir.Random.Next(Agility + 1) > attacker.Accuracy) return 0;
                    armour = GetAttackPower(MinAC, MaxAC);
                    break;
                case DefenceType.AC:
                    armour = GetAttackPower(MinAC, MaxAC);
                    break;
                case DefenceType.MACAgility:
                    if (Envir.Random.Next(Agility + 1) > attacker.Accuracy) return 0;
                    armour = GetAttackPower(MinMAC, MaxMAC);
                    break;
                case DefenceType.MAC:
                    armour = GetAttackPower(MinAC, MaxAC);
                    break;
                case DefenceType.Agility:
                    if (Envir.Random.Next(Agility + 1) > attacker.Accuracy) return 0;
                    break;
            }

            armour = (int) (armour*PoisonRate);

            if (damageWeapon)
                attacker.DamageWeapon();

            if (armour >= damage) return 0;

            if (attacker.LifeOnHit > 0)
                attacker.ChangeHP(attacker.LifeOnHit);

            if (Target != this && attacker.IsAttackTarget(this))
                Target = attacker;


            ShockTime = 0;

            if (Master != null && Master != attacker)
                if (Envir.Time > Master.BrownTime && Master.PKPoints < 200)
                    attacker.BrownTime = Envir.Time + Settings.Minute;

            if (EXPOwner == null || EXPOwner.Dead)
                EXPOwner = attacker;

            if (EXPOwner == attacker)
                EXPOwnerTime = Envir.Time + EXPOwnerDelay;

            if (attacker.HasParalysisRing && 1 == Envir.Random.Next(1, 15))
                ApplyPoison(new Poison { PType = PoisonType.Paralysis, Duration = 5, TickSpeed = 1000 });

            Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = attacker.ObjectID, Direction = Direction, Location = CurrentLocation });

            int additionalDamage = 0;

            if (attacker.ItemSets.Any(s => s.Set == ItemSet.RedOrchid && s.SetComplete))
            {
                additionalDamage = (damage * 10) / 100;
                attacker.ChangeHP(additionalDamage);
            }

            ChangeHP(armour - (damage + additionalDamage));

            return damage - armour;
        }
        public override int Attacked(MonsterObject attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            if (Target == null && attacker.IsAttackTarget(this))
                Target = attacker;

            int armour = 0;

            switch (type)
            {
                case DefenceType.ACAgility:
                    if (Envir.Random.Next(Agility + 1) > attacker.Accuracy) return 0;
                    armour = GetAttackPower(MinAC, MaxAC);
                    break;
                case DefenceType.AC:
                    armour = GetAttackPower(MinAC, MaxAC);
                    break;
                case DefenceType.MACAgility:
                    if (Envir.Random.Next(Agility + 1) > attacker.Accuracy) return 0;
                    armour = GetAttackPower(MinMAC, MaxMAC);
                    break;
                case DefenceType.MAC:
                    armour = GetAttackPower(MinAC, MaxAC);
                    break;
                case DefenceType.Agility:
                    if (Envir.Random.Next(Agility + 1) > attacker.Accuracy) return 0;
                    break;
            }

            armour = (int)(armour * PoisonRate);

            if (armour >= damage) return 0;

            if (Target != this && attacker.IsAttackTarget(this))
                Target = attacker;
            
            ShockTime = 0;

            if (attacker.Info.AI == 6)
                EXPOwner = null;

            else if (attacker.Master != null)
            {
                if (!Functions.InRange(attacker.CurrentLocation, attacker.Master.CurrentLocation, Globals.DataRange))
                    EXPOwner = null;
                else
                {

                    if (EXPOwner == null || EXPOwner.Dead)
                        EXPOwner = attacker.Master;

                    if (EXPOwner == attacker.Master)
                        EXPOwnerTime = Envir.Time + EXPOwnerDelay;
                }

            }

            Broadcast(new S.ObjectStruck { ObjectID = ObjectID, AttackerID = attacker.ObjectID, Direction = Direction, Location = CurrentLocation });

            ChangeHP(armour - damage);
            return damage - armour;
        }

        public override void ApplyPoison(Poison p)
        {
            if (p.Owner != null && p.Owner.IsAttackTarget(this))
                Target = p.Owner;

            if (Master != null && p.Owner != null && p.Owner.Race == ObjectType.Player && p.Owner != Master)
            {
                if (Envir.Time > Master.BrownTime && Master.PKPoints < 200)
                    p.Owner.BrownTime = Envir.Time + Settings.Minute;
            }

            for (int i = 0; i < PoisonList.Count; i++)
            {
                if (PoisonList[i].PType != p.PType) continue;

                PoisonList[i] = p;
                return;
            }

            PoisonList.Add(p);
        }
        public override void AddBuff(Buff b)
        {
            base.AddBuff(b);
            RefreshAll();
        }
        
        public override Packet GetInfo()
        {
            return new S.ObjectMonster
                {
                    ObjectID = ObjectID,
                    Name = Name,
                    NameColour = NameColour,
                    Location = CurrentLocation,
                    Image = Info.Image,
                    Direction = Direction,
                    Effect = Info.Effect,
                    AI = Info.AI,
                    Light = Info.Light,
                    Dead = Dead,
                    Skeleton = Harvested,
                    Poison = CurrentPoison,
                    Hidden = Hidden,
                };
        }

        public override void ReceiveChat(string text, ChatType type)
        {
            throw new NotSupportedException();
        }

        public void RemoveObjects(MirDirection dir, int count)
        {
            switch (dir)
            {
                case MirDirection.Up:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpRight:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Right:
                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownRight:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Down:
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownLeft:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Left:
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpLeft:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Remove(this);
                            }
                        }
                    }
                    break;
            }
        }
        public void AddObjects(MirDirection dir, int count)
        {
            switch (dir)
            {
                case MirDirection.Up:
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpRight:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Right:
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownRight:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }

                    //Right Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X + Globals.DataRange - b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Down:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.DownLeft:
                    //Bottom Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y + Globals.DataRange - a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange - count; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.Left:
                    //Left Block
                    for (int a = -Globals.DataRange; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
                case MirDirection.UpLeft:
                    //Top Block
                    for (int a = 0; a < count; a++)
                    {
                        int y = CurrentLocation.Y - Globals.DataRange + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = -Globals.DataRange; b <= Globals.DataRange; b++)
                        {
                            int x = CurrentLocation.X + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }

                    //Left Block
                    for (int a = -Globals.DataRange + count; a <= Globals.DataRange; a++)
                    {
                        int y = CurrentLocation.Y + a;
                        if (y < 0 || y >= CurrentMap.Height) continue;

                        for (int b = 0; b < count; b++)
                        {
                            int x = CurrentLocation.X - Globals.DataRange + b;
                            if (x < 0 || x >= CurrentMap.Width) continue;

                            Cell cell = CurrentMap.GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject ob = cell.Objects[i];
                                if (ob.Race != ObjectType.Player) continue;
                                ob.Add(this);
                            }
                        }
                    }
                    break;
            }
        }

        public override void SendHealth(PlayerObject player)
        {
            if (!player.IsMember(Master) && !player.IsMember(EXPOwner) && Envir.Time > RevTime) return;

            byte time = Math.Min(byte.MaxValue, (byte) Math.Max(5, (RevTime - Envir.Time)/1000));
            player.Enqueue(new S.ObjectHealth { ObjectID = ObjectID, Percent = PercentHealth, Expire = time });
        }

        public void PetExp(uint amount)
        {
            if (PetLevel >= MaxPetLevel) return;

            if (Info.Name == Settings.SkeletonName || Info.Name == Settings.ShinsuName || Info.Name == Settings.AngelName)
                amount *= 3;

            PetExperience += amount;

            if (PetExperience < (PetLevel + 1)*20000) return;

            PetExperience = (uint) (PetExperience - ((PetLevel + 1)*20000));
            PetLevel++;
            RefreshAll();
            OperateTime = 0;
            BroadcastHealthChange();
        }
        public override void Despawn()
        {
            SlaveList.Clear();
            base.Despawn();
        }

        public void AddNeedHole()
        {
            
        }
    }
}

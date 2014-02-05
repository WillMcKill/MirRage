using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Client.MirScenes;
using S = ServerPackets;

namespace Client.MirObjects
{
    public class UserObject : PlayerObject
    {
        public byte Level;

        public ushort HP, MaxHP, MP, MaxMP;

        public byte MinAC, MaxAC,
                   MinMAC, MaxMAC,
                   MinDC, MaxDC,
                   MinMC, MaxMC,
                   MinSC, MaxSC;

        public byte Accuracy, Agility;
        public sbyte ASpeed, Luck;
        public int AttackSpeed;

        public byte CurrentHandWeight, MaxHandWeight,
                      CurrentWearWeight, MaxWearWeight;
        public ushort CurrentBagWeight, MaxBagWeight;
        public long Experience, MaxExperience;
        public byte LifeOnHit;

        public bool HasTeleportRing, HasProtectionRing, HasRevivalRing, HasClearRing,
            HasMuscleRing, HasParalysisRing, HasFireRing, HasHealRing;

        public UserItem[] Inventory = new UserItem[46], Equipment = new UserItem[14];
        public List<ClientMagic> Magics = new List<ClientMagic>();
        public List<ItemSets> ItemSets = new List<ItemSets>();
        public List<EquipmentSlot> MirSet = new List<EquipmentSlot>();

        public ClientMagic NextMagic;
        public Point NextMagicLocation;
        public MapObject NextMagicObject;
        public MirDirection NextMagicDirection;
        public QueuedAction QueuedAction;


        public UserObject(uint objectID) : base(objectID)
        {
        }

        public void Load(S.UserInformation info)
        {
            Name = info.Name;
            NameColour = info.NameColour;
            Class = info.Class;
            Gender = info.Gender;
            Level = info.Level;

            CurrentLocation = info.Location;
            MapLocation = info.Location;
            GameScene.Scene.MapControl.AddObject(this);

            Direction = info.Direction;
            Hair = info.Hair;

            HP = info.HP;
            MP = info.MP;

            Experience = info.Experience;
            MaxExperience = info.MaxExperience;

            Inventory = info.Inventory;
            Equipment = info.Equipment;
            Magics = info.Magics;
            

            BindAllItems();

            RefreshStats();

            SetAction();
        }

        public override void SetLibraries()
        {
            Weapon = -1;
            Armour = 0;
            WingEffect = 0;

            if (Equipment != null)
            {
                if (Equipment[(int) EquipmentSlot.Weapon] != null)
                    Weapon = Equipment[(int) EquipmentSlot.Weapon].Info.Shape;
                if (Equipment[(int)EquipmentSlot.Armour] != null)
                {
                    Armour = Equipment[(int)EquipmentSlot.Armour].Info.Shape;
                    WingEffect = Equipment[(int)EquipmentSlot.Armour].Info.Effect;
                }
            }
            base.SetLibraries();
        }

        public void RefreshStats()
        {
            RefreshLevelStats();
            RefreshBagWeight();
            RefreshEquipmentStats();
            RefreshItemSetStats();
            RefreshMirSetStats();
            RefreshSkills();
            RefreshBuffs();
            SetLibraries();
            
            if (this == User && Light < 3) Light = 3;
            AttackSpeed = 1500 - ASpeed * 50 - Level * 5;

            if (AttackSpeed < 600) AttackSpeed = 600;

            GameScene.Scene.Redraw();
        }
        private void RefreshLevelStats()
        {
            MaxHP = 0; MaxMP = 0;
            MinAC = 0; MaxAC = 0;
            MinMAC = 0; MaxMAC = 0;
            MinDC = 0; MaxDC = 0;
            MinMC = 0; MaxMC = 0;
            MinSC = 0; MaxSC = 0;

            Accuracy = 12; Agility = 15;

            //Other Stats;
            MaxBagWeight = 0;
            MaxWearWeight = 0;
            MaxHandWeight = 0;
            ASpeed = 0;
            Luck = 0;
            Light = 0;
            LifeOnHit = 0;

            switch (Class)
            {
                case MirClass.Warrior:
                    MaxHP = (ushort)Math.Min(ushort.MaxValue, 14 + (Level / 4F + 4.5F + Level / 20F) * Level);
                    MaxMP = (ushort)Math.Min(ushort.MaxValue, 11 + Level * 3.5F);

                    MaxAC = (byte)Math.Min(byte.MaxValue, Level / 7);
                    MinDC = (byte)Math.Min(byte.MaxValue, Level / 5);
                    MaxDC = (byte)Math.Min(byte.MaxValue, Level / 5 + 1);


                    MaxBagWeight = (ushort)(50 + Level / 3F * Level);
                    MaxWearWeight = (byte)Math.Min(byte.MaxValue, 15 + Level / 20F * Level);
                    MaxHandWeight = (byte)Math.Min(byte.MaxValue, 12 + Level / 13F * Level);
                    break;
                case MirClass.Wizard:
                    MaxHP = (ushort)Math.Min(ushort.MaxValue, 14 + (Level / 15F + 1.8F) * Level);
                    MaxMP = (ushort)Math.Min(ushort.MaxValue, 13 + (Level / 5F + 2F) * 2.2F * Level);

                    MinDC = (byte)Math.Min(byte.MaxValue, Level / 7);
                    MaxDC = (byte)Math.Min(byte.MaxValue, Level / 7 + 1);
                    MinMC = (byte)Math.Min(byte.MaxValue, Level / 7);
                    MaxMC = (byte)Math.Min(byte.MaxValue, Level / 7 + 1);

                    MaxBagWeight = (ushort)(50 + Level / 5F * Level);
                    MaxWearWeight = (byte)Math.Min(byte.MaxValue, 15 + Level / 100F * Level);
                    MaxHandWeight = (byte)Math.Min(byte.MaxValue, 12 + Level / 90F * Level);
                    break;
                case MirClass.Taoist:
                    MaxHP = (ushort)Math.Min(ushort.MaxValue, 14 + (Level / 6F + 2.5F) * Level);
                    MaxMP = (ushort)Math.Min(ushort.MaxValue, 13 + Level / 8F * 2.2F * Level);

                    MinMAC = (byte)Math.Min(byte.MaxValue, Level / 12);
                    MaxMAC = (byte)Math.Min(byte.MaxValue, Level / 6 + 1);
                    MinDC = (byte)Math.Min(byte.MaxValue, Level / 7);
                    MaxDC = (byte)Math.Min(byte.MaxValue, Level / 7 + 1);
                    MinSC = (byte)Math.Min(byte.MaxValue, Level / 7);
                    MaxSC = (byte)Math.Min(byte.MaxValue, Level / 7 + 1);

                    MaxBagWeight = (ushort)(50 + Level / 4F * Level);
                    MaxWearWeight = (byte)Math.Min(byte.MaxValue, 15 + Level / 50F * Level);
                    MaxHandWeight = (byte)Math.Min(byte.MaxValue, 12 + Level / 42F * Level);

                    Agility += 3;
                    break;
                case MirClass.Assassin:
                    MaxHP = (ushort)Math.Min(ushort.MaxValue, 14 + (Level / 4F + 3.25F) * Level);
                    MaxMP = (ushort)Math.Min(ushort.MaxValue, 11 + Level * 5F);

                    MinDC = (byte)Math.Min(byte.MaxValue, Level / 8);
                    MaxDC = (byte)Math.Min(byte.MaxValue, Level / 6 + 1);

                    MaxBagWeight = (ushort)(50 + Level / 3.5F * Level);
                    MaxWearWeight = (byte)Math.Min(byte.MaxValue, 15 + Level / 33F * Level);
                    MaxHandWeight = (byte)Math.Min(byte.MaxValue, 12 + Level / 30F * Level);

                    Agility += 5;
                    // LifeOnHit = (byte)Math.Min(10, Level / 5);
                    break;
            }
        }
        private void RefreshBagWeight()
        {
            CurrentBagWeight = 0;

            for (int i = 0; i < Inventory.Length; i++)
            {
                UserItem item = Inventory[i];
                if (item != null)
                    CurrentBagWeight = (ushort)Math.Min(ushort.MaxValue, CurrentBagWeight + item.Weight);
            }
        }
        private void RefreshEquipmentStats()
        {
            CurrentWearWeight = 0;
            CurrentHandWeight = 0;

            HasTeleportRing = false;
            HasProtectionRing = false;
            HasMuscleRing = false;
            HasParalysisRing = false;

            ItemSets.Clear();
            MirSet.Clear();

            for (int i = 0; i < Equipment.Length; i++)
            {
                UserItem temp = Equipment[i];

                if (temp == null) continue;

                if (temp.Info.Type == ItemType.Weapon || temp.Info.Type == ItemType.Torch)
                    CurrentHandWeight = (byte)Math.Min(byte.MaxValue, CurrentHandWeight + temp.Weight);
                else
                    CurrentWearWeight = (byte)Math.Min(byte.MaxValue, CurrentWearWeight + temp.Weight);

                if (temp.CurrentDura == 0 && temp.Info.Durability > 0) continue;


                MinAC = (byte)Math.Min(byte.MaxValue, MinAC + temp.Info.MinAC);
                MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + temp.Info.MaxAC + temp.AC);
                MinMAC = (byte)Math.Min(byte.MaxValue, MinMAC + temp.Info.MinMAC);
                MaxMAC = (byte)Math.Min(byte.MaxValue, MaxMAC + temp.Info.MaxMAC + temp.MAC);

                MinDC = (byte)Math.Min(byte.MaxValue, MinDC + temp.Info.MinDC);
                MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + temp.Info.MaxDC + temp.DC);
                MinMC = (byte)Math.Min(byte.MaxValue, MinMC + temp.Info.MinMC);
                MaxMC = (byte)Math.Min(byte.MaxValue, MaxMC + temp.Info.MaxMC + temp.MC);
                MinSC = (byte)Math.Min(byte.MaxValue, MinSC + temp.Info.MinSC);
                MaxSC = (byte)Math.Min(byte.MaxValue, MaxSC + temp.Info.MaxSC + temp.SC);

                Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + temp.Info.Accuracy + temp.Accuracy);
                Agility = (byte)Math.Min(byte.MaxValue, Agility + temp.Info.Agility + temp.Agility);

                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + temp.Info.HP + temp.HP);
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + temp.Info.MP + temp.MP);

                ASpeed = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, ASpeed + temp.AttackSpeed + temp.Info.AttackSpeed)));
                Luck = (sbyte)Math.Max(sbyte.MinValue, (Math.Min(sbyte.MaxValue, Luck + temp.Luck + temp.Info.Luck)));

                MaxBagWeight = (ushort)Math.Max(ushort.MinValue, (Math.Min(ushort.MaxValue, MaxBagWeight + temp.Info.BagWeight)));
                MaxWearWeight = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, MaxWearWeight + temp.Info.WearWeight)));
                MaxHandWeight = (byte)Math.Max(byte.MinValue, (Math.Min(byte.MaxValue, MaxHandWeight + temp.Info.HandWeight)));

                if (temp.Info.Light > Light) Light = temp.Info.Light;

                switch (temp.Info.Type)
                {
                    case ItemType.Ring:
                        switch (temp.Info.Shape)
                        {
                            case 1:
                                HasParalysisRing = true;
                                break;
                            case 2:
                                HasTeleportRing = true;
                                break;
                            case 3:
                                //HasClearRing = true;
                                break;
                            case 4:
                                HasProtectionRing = true;
                                break;
                            case 6:
                                HasMuscleRing = true;
                                break;
                        }
                        break;
                }

                if (temp.Info.Set == ItemSet.None) continue;

                bool sameSetFound = false;
                foreach (var set in ItemSets.Where(set => set.Set == temp.Info.Set && !set.Type.Contains(temp.Info.Type)).TakeWhile(set => !set.SetComplete))
                {
                    set.Type.Add(temp.Info.Type);
                    set.Count++;
                    sameSetFound = true;
                }

                if (!ItemSets.Any() || !sameSetFound)
                    ItemSets.Add(new ItemSets { Count = 1, Set = temp.Info.Set, Type = new List<ItemType> { temp.Info.Type } });

                //Mir Set
                if (temp.Info.Set == ItemSet.Mir)
                {
                    if (!MirSet.Contains((EquipmentSlot)i))
                        MirSet.Add((EquipmentSlot)i);
                }
            }

            if (HasMuscleRing)
            {
                MaxBagWeight = (ushort)(MaxBagWeight * 2);
                MaxWearWeight = Math.Min(byte.MaxValue, (byte)(MaxWearWeight * 2));
                MaxHandWeight = Math.Min(byte.MaxValue, (byte)(MaxHandWeight * 2));
            }
        }

        private void RefreshItemSetStats()
        {
            foreach (var s in ItemSets.Where(s => s.SetComplete))
            {
                switch (s.Set)
                {
                    case ItemSet.Mundane:
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 50);
                        break;
                    case ItemSet.NokChi:
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 50);
                        break;
                    case ItemSet.TaoProtect:
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 30);
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 30);
                        break;
                    case ItemSet.RedOrchid:
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + 2);
                        break;
                    case ItemSet.RedFlower:
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 50);
                        MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP - 50);
                        break;
                    case ItemSet.Smash:
                        AttackSpeed = Math.Min(int.MaxValue, AttackSpeed + 2);
                        MinDC = (byte)Math.Min(byte.MaxValue, MinDC + 1);
                        MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + 3);
                        break;
                    case ItemSet.HwanDevil:
                        MaxWearWeight = (byte)Math.Min(byte.MaxValue, MaxWearWeight + 5);
                        MaxBagWeight = (byte)Math.Min(byte.MaxValue, MaxBagWeight + 20);
                        MinMC = (byte)Math.Min(byte.MaxValue, MinMC + 1);
                        MaxMC = (byte)Math.Min(byte.MaxValue, MaxMC + 2);
                        break;
                    case ItemSet.Purity:
                        MinSC = (byte)Math.Min(byte.MaxValue, MinSC + 1);
                        MaxSC = (byte)Math.Min(byte.MaxValue, MaxSC + 2);
                        //holy +2;
                        break;
                    case ItemSet.FiveString:
                        MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + ((MaxHP / 100) * 30));
                        MinAC = (byte)Math.Min(byte.MaxValue, MinAC + 2);
                        MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + 2);
                        break;
                    case ItemSet.Spirit:
                        MinDC = (byte)Math.Min(byte.MaxValue, MinDC + 2);
                        MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + 5);
                        AttackSpeed = Math.Min(int.MaxValue, AttackSpeed + 2);
                        break;
                }
            }
        }

        private void RefreshMirSetStats()
        {
            if (MirSet.Count() == 10)
            {
                MinAC = (byte)Math.Min(byte.MaxValue, MinAC + 1);
                MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + 4);
                MinMAC = (byte)Math.Min(byte.MaxValue, MinMAC + 1);
                MaxMAC = (byte)Math.Min(byte.MaxValue, MaxMAC + 4);
                MaxWearWeight = (byte)Math.Min(byte.MaxValue, MaxWearWeight + 27);
                MaxBagWeight = (byte)Math.Min(byte.MaxValue, MaxBagWeight + 120);
                MaxHandWeight = (byte)Math.Min(byte.MaxValue, MaxHandWeight + 34);
                Luck = (sbyte)Math.Min(sbyte.MaxValue, Luck + 2);
                AttackSpeed = Math.Min(int.MaxValue, AttackSpeed + 2);
                MaxHP = (ushort)Math.Min(ushort.MaxValue, MaxHP + 70);
                MaxMP = (ushort)Math.Min(ushort.MaxValue, MaxMP + 80);
                //MR+6 //PR+6
            }

            else if (MirSet.Contains(EquipmentSlot.Armour) && MirSet.Contains(EquipmentSlot.Boots) && MirSet.Contains(EquipmentSlot.Belt) && MirSet.Contains(EquipmentSlot.Helmet) && MirSet.Contains(EquipmentSlot.Weapon))
            {
                MinDC = (byte)Math.Min(byte.MaxValue, MinDC + 1);
                MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + 4);
                MinMC = (byte)Math.Min(byte.MaxValue, MinMC + 1);
                MaxMC = (byte)Math.Min(byte.MaxValue, MaxMC + 3);
                MinSC = (byte)Math.Min(byte.MaxValue, MinSC + 1);
                MaxSC = (byte)Math.Min(byte.MaxValue, MaxSC + 3);
                MaxHandWeight = (byte)Math.Min(byte.MaxValue, MaxHandWeight + 34);
                Agility = (byte)Math.Min(byte.MaxValue, Agility + 1);
            }

            else if (MirSet.Contains(EquipmentSlot.RingL) && MirSet.Contains(EquipmentSlot.RingR) && MirSet.Contains(EquipmentSlot.BraceletL) && MirSet.Contains(EquipmentSlot.BraceletR) && MirSet.Contains(EquipmentSlot.Necklace))
            {
                MinAC = (byte)Math.Min(byte.MaxValue, MinAC + 1);
                MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + 3);
                MinMAC = (byte)Math.Min(byte.MaxValue, MinMAC + 1);
                MaxMAC = (byte)Math.Min(byte.MaxValue, MaxMAC + 1);
                MaxWearWeight = (byte)Math.Min(byte.MaxValue, MaxWearWeight + 27);
                MaxBagWeight = (byte)Math.Min(byte.MaxValue, MaxBagWeight + 50);
            }

            else if (MirSet.Contains(EquipmentSlot.Armour) && MirSet.Contains(EquipmentSlot.Boots) &&
                     MirSet.Contains(EquipmentSlot.Belt))
            {
                MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + 1);
                MaxMC = (byte)Math.Min(byte.MaxValue, MaxMC + 1);
                MaxSC = (byte)Math.Min(byte.MaxValue, MaxSC + 1);
                MaxHandWeight = (byte)Math.Min(byte.MaxValue, MaxHandWeight + 17);
            }

            else if (MirSet.Contains(EquipmentSlot.Armour) && MirSet.Contains(EquipmentSlot.Helmet) && MirSet.Contains(EquipmentSlot.Weapon))
            {
                MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + 2);
                MaxMC = (byte)Math.Min(byte.MaxValue, MaxMC + 1);
                MaxSC = (byte)Math.Min(byte.MaxValue, MaxSC + 1);
                Agility = (byte)Math.Min(byte.MaxValue, Agility + 1);
            }

            else if ((MirSet.Contains(EquipmentSlot.RingL) || MirSet.Contains(EquipmentSlot.RingR)) && (MirSet.Contains(EquipmentSlot.BraceletL) || MirSet.Contains(EquipmentSlot.BraceletR)) && MirSet.Contains(EquipmentSlot.Necklace))
            {
                MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + 1);
                MaxMAC = (byte)Math.Min(byte.MaxValue, MaxMAC + 1);
                MaxWearWeight = (byte)Math.Min(byte.MaxValue, MaxWearWeight + 17);
                MaxBagWeight = (byte)Math.Min(byte.MaxValue, MaxBagWeight + 30);
            }

            else if (MirSet.Contains(EquipmentSlot.RingL) && MirSet.Contains(EquipmentSlot.RingR))
            {
                MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + 1);
                MaxMAC = (byte)Math.Min(byte.MaxValue, MaxMAC + 1);
            }

            else if (MirSet.Contains(EquipmentSlot.BraceletL) && MirSet.Contains(EquipmentSlot.BraceletR))
            {
                MinAC = (byte)Math.Min(byte.MaxValue, MinAC + 1);
                MinMAC = (byte)Math.Min(byte.MaxValue, MinMAC + 1);
            }
        }

        private void RefreshSkills()
        {
            for (int i = 0; i < Magics.Count; i++)
            {
                ClientMagic magic = Magics[i];
                switch (magic.Spell)
                {
                    case Spell.Fencing:
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + magic.Level * 3);
                        MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + (magic.Level + 1) * 3);
                        break;
                    case Spell.FatalSword:
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + magic.Level);
                        break;
                    case Spell.SpiritSword:
                        Accuracy = (byte)Math.Min(byte.MaxValue, Accuracy + magic.Level);
                        MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + MaxSC * (magic.Level + 1) * 0.1F);
                        break;
                }
            }
        }
        private void RefreshBuffs()
        {
            for (int i = 0; i < GameScene.Scene.Buffs.Count; i++)
            {
                Buff buff = GameScene.Scene.Buffs[i];

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
                    case BuffType.ProtectionField:
                        MaxAC = (byte)Math.Min(byte.MaxValue, MaxAC + buff.Value);
                        break;
                    case BuffType.Rage:
                        MaxDC = (byte)Math.Min(byte.MaxValue, MaxDC + buff.Value);
                        break;
                }

            }
        }

        public void BindAllItems()
        {
            for (int i = 0; i < Inventory.Length; i++)
            {
                if (Inventory[i] == null) continue;
                GameScene.Bind(Inventory[i]);
            }

            for (int i = 0; i < Equipment.Length; i++)
            {
                if (Equipment[i] == null) continue;
                GameScene.Bind(Equipment[i]);
            }
        }


        public ClientMagic GetMagic(Spell spell)
        {
            for (int i = 0; i < Magics.Count; i++)
            {
                ClientMagic magic = Magics[i];
                if (magic.Spell != spell) continue;
                return magic;
            }

            return null;
        }


        public void GetMaxGain(UserItem item)
        {
            if (CurrentBagWeight + item.Weight <= MaxBagWeight && FreeSpace(Inventory) > 0) return;

            uint min = 0;
            uint max = item.Count;

            if (item.Info.Type == ItemType.Amulet)
            {
                for (int i = 0; i < Inventory.Length; i++)
                {
                    UserItem bagItem = Inventory[i];

                    if (bagItem == null || bagItem.Info != item.Info) continue;

                    if (bagItem.Count + item.Count <= bagItem.Info.StackSize)
                    {
                        item.Count = max;
                        return;
                    }
                    item.Count = bagItem.Info.StackSize - bagItem.Count;
                    min += item.Count;
                    if (min >= max)
                    {
                        item.Count = max;
                        return;
                    }
                }

                if (min == 0)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat(FreeSpace(Inventory) == 0 ? "You do not have enough space." : "You do not have enough weight.", ChatType.System);

                    item.Count = 0;
                    return;
                }

                item.Count = min;
                return;
            }

            if (CurrentBagWeight + item.Weight > MaxBagWeight)
            {
                item.Count = (uint)((MaxBagWeight - CurrentBagWeight) / item.Info.Weight);
                max = item.Count;
                if (item.Count == 0)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough weight.", ChatType.System);
                    return;
                }
            }

            if (item.Info.StackSize > 1)
            {
                for (int i = 0; i < Inventory.Length; i++)
                {
                    UserItem bagItem = Inventory[i];

                    if (bagItem == null) return;
                    if (bagItem.Info != item.Info) continue;

                    if (bagItem.Count + item.Count <= bagItem.Info.StackSize)
                    {
                        item.Count = max;
                        return;
                    }

                    item.Count = bagItem.Info.StackSize - bagItem.Count;
                    min += item.Count;
                    if (min >= max)
                    {
                        item.Count = max;
                        return;
                    }
                }

                if (min == 0)
                {
                    GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough space.", ChatType.System);
                    item.Count = 0;
                }
            }
            else
            {
                GameScene.Scene.ChatDialog.ReceiveChat("You do not have enough space.", ChatType.System);
                item.Count = 0;
            }

        }
        private int FreeSpace(UserItem[] array)
        {
            int count = 0;
            for (int i = 0; i < array.Length; i++)
                count++;
            return count;
        }

        public override void SetAction()
        {
            if (QueuedAction != null )
            {
                if ((ActionFeed.Count == 0) || (ActionFeed.Count == 1 && NextAction.Action == MirAction.Stance))
                {
                    ActionFeed.Clear();
                    ActionFeed.Add(QueuedAction);
                    QueuedAction = null;
                }
            }

            base.SetAction();
        }
        public override void ProcessFrames()
        {
            bool clear = CMain.Time >= NextMotion;

            base.ProcessFrames();

            if (clear) QueuedAction = null;
            if ((CurrentAction == MirAction.Standing || CurrentAction == MirAction.Stance || CurrentAction == MirAction.Stance2 || CurrentAction == MirAction.DashFail) && (QueuedAction != null || NextAction != null))
                SetAction();
        }

        public void ClearMagic()
        {
            NextMagic = null;
            NextMagicDirection = 0;
            NextMagicLocation = Point.Empty;
            NextMagicObject = null;
        }
    }
}

public class ItemSets
{
    public ItemSet Set;
    public List<ItemType> Type;
    private byte Amount
    {
        get
        {
            switch (Set)
            {
                case ItemSet.Mundane:
                case ItemSet.NokChi:
                case ItemSet.TaoProtect:
                    return 2;
                case ItemSet.RedOrchid:
                case ItemSet.RedFlower:
                case ItemSet.Smash:
                case ItemSet.HwanDevil:
                case ItemSet.Purity:
                case ItemSet.FiveString:
                    return 3;
                case ItemSet.Recall:
                    return 4;
                case ItemSet.Spirit:
                    return 5;
                default:
                    return 0;
            }
        }
    }
    public byte Count;
    public bool SetComplete
    {
        get
        {
            return Count == Amount;
        }
    }
}


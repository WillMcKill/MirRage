using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Server.MirDatabase;
using Server.MirObjects;
using S = ServerPackets;

namespace Server.MirEnvir
{
    public class Map
    {
        private static Envir Envir
        {
            get { return SMain.Envir; }
        }
        
        public MapInfo Info;

        public int Width, Height;
        public Cell[,] Cells;
        public long LightningTime, FireTime;

        public List<NPCObject> NPCs = new List<NPCObject>();
        public List<PlayerObject> Players = new List<PlayerObject>();
        public List<MapRespawn> Respawns = new List<MapRespawn>();
        public List<DelayedAction> ActionList = new List<DelayedAction>();

        public Map(MapInfo info)
        {
            Info = info;
        }
        public bool Load()
        {
            try
            {
                string fileName = Path.Combine(Settings.MapPath, Info.FileName + ".map");
                if (File.Exists(fileName))
                {
                    byte[] fileBytes = File.ReadAllBytes(fileName);

                    int offSet = 21;

                    int w = BitConverter.ToInt16(fileBytes, offSet);
                    offSet += 2;
                    int xor = BitConverter.ToInt16(fileBytes, offSet);
                    offSet += 2;
                    int h = BitConverter.ToInt16(fileBytes, offSet);
                    Width = w ^ xor;
                    Height = h ^ xor;
                    Cells = new Cell[Width, Height];

                    offSet = 54;

                            for (int x = 0; x < Width; x++)
                        for (int y = 0; y < Height; y++)
                            {
                                if (((BitConverter.ToInt32(fileBytes, offSet) ^ 0xAA38AA38) & 0x20000000) != 0)
                                    Cells[x, y] = Cell.HighWall; //Can Fire Over.

                                offSet += 6;
                                if (((BitConverter.ToInt16(fileBytes, offSet) ^ xor) & 0x8000) != 0)
                                    Cells[x, y] = Cell.LowWall; //Can't Fire Over.

                                if (Cells[x, y] == null) Cells[x, y] = new Cell {Attribute = CellAttribute.Walk};

                                offSet += 9;
                            }

                    for (int i = 0; i < Info.Respawns.Count; i++)
                    {
                        MapRespawn info = new MapRespawn(Info.Respawns[i]);
                        if (info.Monster == null) continue;
                        info.Map = this;
                        Respawns.Add(info);
                    }


                    for (int i = 0; i < Info.NPCs.Count; i++)
                    {
                        NPCInfo info = Info.NPCs[i];
                        if (!ValidPoint(info.Location)) continue;

                        AddObject(new NPCObject(info) {CurrentMap = this});
                    }

                    for (int i = 0; i < Info.SafeZones.Count; i++)
                        CreateSafeZone(Info.SafeZones[i]);

                    return true;
                }
            }
            catch (Exception ex)
            {
                SMain.Enqueue(ex);
            }

            SMain.Enqueue("Failed to Load Map: " + Info.FileName);
            return false;
        }

        private void CreateSafeZone(SafeZoneInfo info)
        {

            for (int y = info.Location.Y - info.Size; y <= info.Location.Y + info.Size; y++)
            {
                if (y < 0) continue;
                if (y >= Height) break;
                for (int x = info.Location.X - info.Size; x <= info.Location.X + info.Size  ; x += Math.Abs(y - info.Location.Y) == info.Size ? 1 : info.Size * 2)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    if (!Cells[x, y].Valid) continue;

                    SpellObject spell = new SpellObject
                    {
                        ExpireTime = long.MaxValue,
                        Spell = Spell.TrapHexagon,
                        TickSpeed = int.MaxValue,
                        CurrentLocation = new Point(x, y),
                        CurrentMap = this
                    };

                    Cells[x, y].Add(spell);

                    spell.Spawned();
                }
            }


            for (int y = info.Location.Y - info.Size; y <= info.Location.Y + info.Size; y++)
            {
                if (y < 0) continue;
                if (y >= Height) break;
                for (int x = info.Location.X - info.Size; x <= info.Location.X + info.Size; x++)
                {
                    if (x < 0) continue;
                    if (x >= Width) break;
                    if (!Cells[x,y].Valid) continue;

                    SpellObject spell = new SpellObject
                        {
                            ExpireTime = long.MaxValue,
                            Value = 25,
                            TickSpeed = 2000,
                            Spell = Spell.Healing,
                            CurrentLocation = new Point(x, y),
                            CurrentMap = this
                        };

                    Cells[x, y].Add(spell);

                    spell.Spawned();
                }
            }


        }

        public Cell GetCell(Point location)
        {
            return Cells[location.X, location.Y];
        }
        public Cell GetCell(int x, int y)
        {
            return Cells[x, y];
        }

        public bool ValidPoint(Point location)
        {
            return location.X >= 0 && location.X < Width && location.Y >= 0 && location.Y < Height && GetCell(location).Valid;
        }
        public bool ValidPoint(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height && GetCell(x, y).Valid;
        }

        public void Process()
        {
            ProcessRespawns();

            if ((Info.Lightning) && Envir.Time > LightningTime)
            {
                LightningTime = Envir.Time + Envir.Random.Next(5000, 60000);
                for (int i = Players.Count - 1; i >= 0; i--)
                {
                    PlayerObject player = Players[i];

                    Broadcast(new S.ObjectEffect { ObjectID = player.ObjectID, Effect = SpellEffect.MapLightning }, player.CurrentLocation);
                }
            }
            if ((Info.Fire) && Envir.Time > FireTime)
            {
                FireTime = Envir.Time + Envir.Random.Next(5000, 60000);
                for (int i = Players.Count - 1; i >= 0; i--)
                {
                    PlayerObject player = Players[i];

                    Broadcast(new S.ObjectEffect { ObjectID = player.ObjectID, Effect = SpellEffect.MapFire }, player.CurrentLocation);
                }
            }

            for (int i = 0; i < ActionList.Count; i++)
            {
                if (Envir.Time < ActionList[i].Time) continue;
                Process(ActionList[i]);
                ActionList.RemoveAt(i);
            }

        }

        private void ProcessRespawns()
        {
            for (int i = 0; i < Respawns.Count; i++)
            {
                MapRespawn respawn = Respawns[i];

                if (Envir.Time < respawn.RespawnTime || respawn.Count >= respawn.Info.Count) continue;

                int count = respawn.Info.Count - respawn.Count;

                for (int c = 0; c < count; c++)
                    respawn.Spawn();

                respawn.RespawnTime = Envir.Time + (respawn.Info.Delay * Settings.Minute);
            }
        }

        public void Process(DelayedAction action)
        {
            switch (action.Type)
            {
                case DelayedType.Magic:
                    CompleteMagic(action.Params);
                    break;
                    case DelayedType.Spawn:
                    MonsterObject mob = (MonsterObject) action.Params[0];
                    mob.Spawn(this, (Point) action.Params[1]);
                    if (action.Params.Length > 2) ((MonsterObject) action.Params[2]).SlaveList.Add(mob);
                    break;
            }
        }
        private void CompleteMagic(IList<object> data)
        {
            bool train = false;
            PlayerObject player = (PlayerObject)data[0];
            UserMagic magic = (UserMagic)data[1];

            int value;
            Point location;
            Cell cell;
            MirDirection dir;
            MonsterObject monster;
            Point front;
            switch (magic.Spell)
            {
                    #region HellFire

                case Spell.HellFire:
                    value = (int) data[2];
                    dir = (MirDirection) data[4];
                    location = Functions.PointMove((Point) data[3], dir, 1);
                    int count = (int) data[5] - 1;

                    if (!ValidPoint(location)) return;

                    if (count > 0)
                    {
                        DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 100, player, magic, value, location, dir, count);
                        ActionList.Add(action);
                    }

                    cell = GetCell(location);

                    if (cell.Objects == null) return;


                    for (int i = 0; i < cell.Objects.Count; i++)
                    {
                        MapObject target = cell.Objects[i];
                        switch (target.Race)
                        {
                            case ObjectType.Monster:
                            case ObjectType.Player:
                                //Only targets
                                if (target.IsAttackTarget(player))
                                {
                                    if (target.Attacked(player, value, DefenceType.MAC, false) > 0)
                                        player.LevelMagic(magic);
                                    return;
                                }
                                break;
                        }
                    }
                    break;

                    #endregion

                    #region SummonSkeleton

                case Spell.SummonSkeleton:
                    monster = (MonsterObject)data[2];
                    front = (Point)data[3];
                    
                    if (ValidPoint(front))
                        monster.Spawn(this, front);
                    else
                        monster.Spawn(player.CurrentMap, player.CurrentLocation);
                    break;

                #endregion

                    #region FireBang, IceStorm

                case Spell.IceStorm:
                case Spell.FireBang:
                    value = (int) data[2];
                    location = (Point) data[3];

                    for (int y = location.Y - 1; y <= location.Y + 1; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 1; x <= location.X + 1; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (target.IsAttackTarget(player))
                                        {
                                            if (target.Attacked(player, value, DefenceType.MAC, false) > 0)
                                                train = true;
                                        }
                                        break;
                                }
                            }

                        }

                    }

                    break;

                    #endregion

                    #region MassHiding

                case Spell.MassHiding:
                    value = (int) data[2];
                    location = (Point) data[3];

                    for (int y = location.Y - 1; y <= location.Y + 1; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 1; x <= location.X + 1; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (target.IsFriendlyTarget(player))
                                        {
                                            for (int b = 0; b < target.Buffs.Count; b++)
                                                if (target.Buffs[b].Type == BuffType.Hiding) return;

                                            target.AddBuff(new Buff { Type = BuffType.Hiding, Caster = player, ExpireTime = Envir.Time + value * 1000 });
                                            target.OperateTime = 0;
                                            train = true;
                                        }
                                        break;
                                }
                            }

                        }

                    }

                    break;

                    #endregion

                    #region SoulShield, BlessedArmour

                case Spell.SoulShield:
                case Spell.BlessedArmour:
                    value = (int) data[2];
                    location = (Point) data[3];
                    BuffType type = magic.Spell == Spell.SoulShield ? BuffType.SoulShield : BuffType.BlessedArmour;

                    for (int y = location.Y - 3; y <= location.Y + 3; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 3; x <= location.X + 3; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (target.IsFriendlyTarget(player))
                                        {
                                            target.AddBuff(new Buff { Type = type, Caster = player, ExpireTime = Envir.Time + value * 1000, Value = target.Level / 7 + 4 });
                                            target.OperateTime = 0;
                                            train = true;
                                        }
                                        break;
                                }
                            }

                        }

                    }

                    break;

                    #endregion

                    #region FireWall

                case Spell.FireWall:
                    value = (int)data[2];
                    location = (Point)data[3];

                    player.LevelMagic(magic);

                    if (ValidPoint(location))
                    {
                        cell = GetCell(location);
                        
                        bool cast = true;
                        if (cell.Objects != null)
                            for (int o = 0; o < cell.Objects.Count; o++)
                            {
                                MapObject target = cell.Objects[o];
                                if (target.Race != ObjectType.Spell || ((SpellObject) target).Spell != Spell.FireWall) continue;

                                cast = false;
                                break;
                            }

                        if (cast)
                        {
                            SpellObject ob = new SpellObject
                                {
                                    Spell = Spell.FireWall,
                                    Value = value,
                                    ExpireTime = Envir.Time + (10 + value/2)*1000,
                                    TickSpeed = 2000,
                                    Caster = player,
                                    CurrentLocation = location,
                                    CurrentMap = this,
                                };
                            AddObject(ob);
                            ob.Spawned();
                        }
                    }

                    dir = MirDirection.Up;
                    for (int i = 0; i < 4; i++)
                    {
                        location = Functions.PointMove((Point) data[3], dir, 1);
                        dir += 2;

                        if (!ValidPoint(location)) continue;

                        cell = GetCell(location);
                        bool cast = true;

                        if (cell.Objects != null)
                            for (int o = 0; o < cell.Objects.Count; o++)
                            {
                                MapObject target = cell.Objects[o];
                                if (target.Race != ObjectType.Spell || ((SpellObject) target).Spell != Spell.FireWall) continue;

                                cast = false;
                                break;
                            }

                        if (!cast) continue;

                        SpellObject ob = new SpellObject
                        {
                            Spell = Spell.FireWall,
                            Value = value,
                            ExpireTime = Envir.Time + (10 + value / 2) * 1000,
                            TickSpeed = 2000,
                            Caster = player,
                            CurrentLocation = location,
                            CurrentMap = this,
                        };
                        AddObject(ob);
                        ob.Spawned();
                    }

                    break;

                #endregion

                    #region Lightning

                case Spell.Lightning:
                    value = (int)data[2];
                    location = (Point)data[3];
                    dir = (MirDirection)data[4];

                    for (int i = 0; i < 6; i++)
                    {
                        location = Functions.PointMove(location, dir, 1);

                        if (!ValidPoint(location)) continue;

                        cell = GetCell(location);

                        if (cell.Objects == null) continue;

                        for (int o = 0; o < cell.Objects.Count; o++)
                        {
                            MapObject target = cell.Objects[o];
                            if (target.Race != ObjectType.Player && target.Race != ObjectType.Monster) continue;

                            if (!target.IsAttackTarget(player)) continue;
                            if (target.Attacked(player, value, DefenceType.MAC, false) > 0)
                                train = true;
                            break;
                        }
                    }

                    break;

                #endregion

                    #region HeavenlySword

                case Spell.HeavenlySword:
                    value = (int)data[2];
                    location = (Point)data[3];
                    dir = (MirDirection)data[4];

                    for (int i = 0; i < 3; i++)
                    {
                        location = Functions.PointMove(location, dir, 1);

                        if (!ValidPoint(location)) continue;

                        cell = GetCell(location);

                        if (cell.Objects == null) continue;

                        for (int o = 0; o < cell.Objects.Count; o++)
                        {
                            MapObject target = cell.Objects[o];
                            if (target.Race != ObjectType.Player && target.Race != ObjectType.Monster) continue;

                            if (!target.IsAttackTarget(player)) continue;
                            if (target.Attacked(player, value, DefenceType.MAC, false) > 0)
                                train = true;
                            break;
                        }
                    }

                    break;

                #endregion

                    #region MassHealing

                case Spell.MassHealing:
                    value = (int)data[2];
                    location = (Point)data[3];

                    for (int y = location.Y - 1; y <= location.Y + 1; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 1; x <= location.X + 1; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (target.IsFriendlyTarget(player))
                                        {
                                            if (target.Health >= target.MaxHealth) continue;
                                            target.HealAmount = (ushort)Math.Min(ushort.MaxValue, target.HealAmount + value);
                                            target.OperateTime = 0;
                                            train = true;
                                        }
                                        break;
                                }
                            }

                        }

                    }

                    break;

                #endregion

                    #region ThunderStorm

                case Spell.ThunderStorm:
                case Spell.FlameField:
                    value = (int)data[2];
                    location = (Point)data[3];

                    for (int y = location.Y - 2; y <= location.Y + 2; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 2; x <= location.X + 2; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                switch (target.Race)
                                {
                                    case ObjectType.Monster:
                                    case ObjectType.Player:
                                        //Only targets
                                        if (!target.IsAttackTarget(player)) break;

                                        if (target.Attacked(player, magic.Spell == Spell.FlameField || target.Undead ? value : value/10, DefenceType.MAC, false) <= 0) break;

                                        train = true;
                                        break;
                                }
                            }

                        }
                    }

                    break;

                #endregion

                    #region SummonShinsu

                case Spell.SummonShinsu:
                    monster = (MonsterObject)data[2];
                    front = (Point)data[3];

                    if (ValidPoint(front))
                        monster.Spawn(this, front);
                    else
                        monster.Spawn(player.CurrentMap, player.CurrentLocation);
                    break;

                #endregion

                    #region LionRoar

                case Spell.LionRoar:
                    location = (Point)data[2];

                    for (int y = location.Y - 2; y <= location.Y + 2; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 2; x <= location.X + 2; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid || cell.Objects == null) continue;

                            for (int i = 0; i < cell.Objects.Count; i++)
                            {
                                MapObject target = cell.Objects[i];
                                if (target.Race != ObjectType.Monster) continue;
                                //Only targets
                                if (!target.IsAttackTarget(player) || player.Level + 10 < target.Level) continue;
                                target.ApplyPoison(new Poison { PType = PoisonType.Paralysis, Duration = magic.Level + 2, TickSpeed = 1000 });
                                target.OperateTime = 0;
                                train = true;
                            }

                        }

                    }

                    break;

                #endregion

                #region PoisonField

                case Spell.PoisonField:
                    value = (int)data[2];
                    location = (Point)data[3];

                    train = true;
                    bool show = true;

                    for (int y = location.Y - 1; y <= location.Y + 1; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 1; x <= location.X + 1; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid) continue;

                            bool cast = true;
                            if (cell.Objects != null)
                                for (int o = 0; o < cell.Objects.Count; o++)
                                {
                                    MapObject target = cell.Objects[o];
                                    if (target.Race != ObjectType.Spell || ((SpellObject) target).Spell != Spell.PoisonField) continue;

                                    cast = false;
                                    break;
                                }

                            if (!cast) continue;

                            SpellObject ob = new SpellObject
                                {
                                    Spell = Spell.PoisonField,
                                    Value = value,
                                    ExpireTime = Envir.Time + 6000,
                                    TickSpeed = 1000,
                                    Caster = player,
                                    CurrentLocation = new Point(x, y),
                                    CastLocation = location,
                                    Show = show,
                                    CurrentMap = this,
                                };

                            show = false;

                            AddObject(ob);
                            ob.Spawned();
                        }
                    } 

                    break;

                #endregion

                #region BladeAvalanche

                case Spell.BladeAvalanche:
                    value = (int)data[2];
                    dir = (MirDirection)data[4];
                    location = Functions.PointMove((Point)data[3], dir, 1);
                    count = (int)data[5] - 1;

                    if (!ValidPoint(location)) return;

                    if (count > 0)
                    {
                        DelayedAction action = new DelayedAction(DelayedType.Magic, Envir.Time + 100, player, magic, value, location, dir, count);
                        ActionList.Add(action);
                    }

                    cell = GetCell(location);

                    if (cell.Objects == null) return;


                    for (int i = 0; i < cell.Objects.Count; i++)
                    {
                        MapObject target = cell.Objects[i];
                        switch (target.Race)
                        {
                            case ObjectType.Monster:
                            case ObjectType.Player:
                                //Only targets
                                if (target.IsAttackTarget(player))
                                {
                                    if (target.Attacked(player, value, DefenceType.MAC, false) > 0)
                                        player.LevelMagic(magic);
                                    return;
                                }
                                break;
                        }
                    }
                    break;

                #endregion

                #region Mirroring

                case Spell.Mirroring:
                    monster = (MonsterObject)data[2];
                    front = (Point)data[3];
                    bool finish = (bool)data[4];

                    if (finish)
                    {
                        monster.Die();
                        return;
                    };

                    if (ValidPoint(front))
                        monster.Spawn(this, front);
                    else
                        monster.Spawn(player.CurrentMap, player.CurrentLocation);
                    break;

                #endregion

                #region Blizzard

                case Spell.Blizzard:
                    value = (int)data[2];
                    location = (Point)data[3];

                    train = true;
                    show = true;

                    for (int y = location.Y - 2; y <= location.Y + 2; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 2; x <= location.X + 2; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid) continue;

                            bool cast = true;
                            if (cell.Objects != null)
                                for (int o = 0; o < cell.Objects.Count; o++)
                                {
                                    MapObject target = cell.Objects[o];
                                    if (target.Race != ObjectType.Spell || ((SpellObject) target).Spell != Spell.Blizzard) continue;

                                    cast = false;
                                    break;
                                }

                            if (!cast) continue;

                            SpellObject ob = new SpellObject
                                {
                                    Spell = Spell.Blizzard,
                                    Value = value,
                                    ExpireTime = Envir.Time + 3000,
                                    TickSpeed = 440,
                                    Caster = player,
                                    CurrentLocation = new Point(x, y),
                                    CastLocation = location,
                                    Show = show,
                                    CurrentMap = this,
                                    StartTime = Envir.Time + 800,
                                };

                            show = false;

                            AddObject(ob);
                            ob.Spawned();
                        }
                    } 

                    break;

                #endregion

                #region MeteorStrike

                case Spell.MeteorStrike:
                    value = (int)data[2];
                    location = (Point)data[3];

                    train = true;
                    show = true;

                    for (int y = location.Y - 2; y <= location.Y + 2; y++)
                    {
                        if (y < 0) continue;
                        if (y >= Height) break;

                        for (int x = location.X - 2; x <= location.X + 2; x++)
                        {
                            if (x < 0) continue;
                            if (x >= Width) break;

                            cell = GetCell(x, y);

                            if (!cell.Valid) continue;

                            bool cast = true;
                            if (cell.Objects != null)
                                for (int o = 0; o < cell.Objects.Count; o++)
                                {
                                    MapObject target = cell.Objects[o];
                                    if (target.Race != ObjectType.Spell || ((SpellObject)target).Spell != Spell.MeteorStrike) continue;

                                    cast = false;
                                    break;
                                }

                            if (!cast) continue;

                            SpellObject ob = new SpellObject
                            {
                                Spell = Spell.MeteorStrike,
                                Value = value,
                                ExpireTime = Envir.Time + 3000,
                                TickSpeed = 440,
                                Caster = player,
                                CurrentLocation = new Point(x, y),
                                CastLocation = location,
                                Show = show,
                                CurrentMap = this,
                                StartTime = Envir.Time + 800,
                            };

                            show = false;

                            AddObject(ob);
                            ob.Spawned();
                        }
                    }

                    break;

                #endregion
            }

            if (train)
                player.LevelMagic(magic);


        }

        public void AddObject(MapObject ob)
        {
            //  Objects.Add(ob);
            if (ob.Race == ObjectType.Player) Players.Add((PlayerObject)ob);
            if (ob.Race == ObjectType.Merchant) NPCs.Add((NPCObject)ob);

            GetCell(ob.CurrentLocation).Add(ob);
        }

        public void RemoveObject(MapObject ob)
        {
            if (ob.Race == ObjectType.Player) Players.Remove((PlayerObject)ob);
            if (ob.Race == ObjectType.Merchant) NPCs.Remove((NPCObject)ob);

            GetCell(ob.CurrentLocation).Remove(ob);
        }


        public SafeZoneInfo GetSafeZone(Point location)
        {
            for (int i = 0; i < Info.SafeZones.Count; i++)
            {
                SafeZoneInfo szi = Info.SafeZones[i];
                if (Functions.InRange(szi.Location, location, szi.Size))
                    return szi;
            }
            return null;
        }

        public void Broadcast(Packet p, Point location)
        {
            if (p == null) return;

            for (int i = Players.Count - 1; i >= 0; i--)
            {
                PlayerObject player = Players[i];

                if (Functions.InRange(location, player.CurrentLocation, Globals.DataRange))
                    player.Enqueue(p);
            }
        }
    }
    public class Cell
    {
        public static readonly Cell HighWall = new Cell { Attribute = CellAttribute.HighWall };
        public static readonly Cell LowWall = new Cell { Attribute = CellAttribute.LowWall };

        public bool Valid
        {
            get { return Attribute == CellAttribute.Walk; }
        }

        public List<MapObject> Objects;
        public CellAttribute Attribute;

        public void Add(MapObject mapObject)
        {
            if (Objects == null) Objects = new List<MapObject>();

            Objects.Add(mapObject);
        }
        public void Remove(MapObject mapObject)
        {
            Objects.Remove(mapObject);
            if (Objects.Count == 0) Objects = null;
        }
    }
    public class MapRespawn
    {
        public RespawnInfo Info;
        public MonsterInfo Monster;
        public Map Map;
        public int Count;
        public long RespawnTime;

        public MapRespawn(RespawnInfo info)
        {
            Info = info;
            Monster = SMain.Envir.GetMonsterInfo(info.MonsterIndex);
        }
        public void Spawn()
        {
            MonsterObject ob = MonsterObject.GetMonster(Monster);
            if (ob == null) return;
            ob.Spawn(this);
        }

    }
}

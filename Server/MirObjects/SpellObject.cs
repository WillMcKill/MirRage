using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Server.MirEnvir;
using S = ServerPackets;

namespace Server.MirObjects
{
    class SpellObject : MapObject
    {
        public override ObjectType Race
        {
            get { return ObjectType.Spell; }
        }

        public override string Name { get; set; }
        public override int CurrentMapIndex { get; set; }
        public override Point CurrentLocation { get; set; }
        public override MirDirection Direction { get; set; }
        public override byte Level { get; set; }
        public override bool Blocking
        {
            get
            {
                return false;
            }
        }

        public long TickTime, StartTime;
        public PlayerObject Caster;
        public int Value, TickSpeed;
        public Spell Spell;
        public Point CastLocation;
        public bool Show;

        public override uint Health
        {
            get { throw new NotSupportedException(); }
        }
        public override uint MaxHealth
        {
            get { throw new NotSupportedException(); }
        }


        public override void Process()
        {
            if (Caster != null && Caster.Node == null) Caster = null;

            if (Envir.Time > ExpireTime || (Spell == Spell.FireWall && Caster == null))
            {
                CurrentMap.RemoveObject(this);
                Despawn();
                return;
            }

            if (Envir.Time < TickTime) return;
            TickTime = Envir.Time + TickSpeed;

            Cell cell = CurrentMap.GetCell(CurrentLocation);
            for (int i = 0; i < cell.Objects.Count; i++)
                ProcessSpell(cell.Objects[i]);
        }
        public void ProcessSpell(MapObject ob)
        {
            if (Envir.Time < StartTime) return;
            switch (Spell)
            {
                case Spell.FireWall:
                    if (ob.Race != ObjectType.Player && ob.Race != ObjectType.Monster) return;
                    if (ob.Dead) return;

                    if (!ob.IsAttackTarget(Caster)) return;
                    ob.Attacked(Caster, Value, DefenceType.MAC, false);
                    break;
                case Spell.Healing: //SafeZone
                    if (ob.Race != ObjectType.Player && (ob.Race != ObjectType.Monster || ob.Master == null || ob.Master.Race != ObjectType.Player)) return;
                    if (ob.Dead || ob.HealAmount != 0 || ob.PercentHealth == 100) return;

                    ob.HealAmount += 25;
                    Broadcast(new S.ObjectEffect {ObjectID = ob.ObjectID, Effect = SpellEffect.Healing});
                    break;
                case Spell.PoisonField:
                    if (ob.Race != ObjectType.Player && ob.Race != ObjectType.Monster) return;
                    if (ob.Dead) return;

                    if (!ob.IsAttackTarget(Caster)) return;
                    ob.Attacked(Caster, Value, DefenceType.None, false);
                    if (!ob.Dead)
                    ob.ApplyPoison(new Poison
                        {
                            Duration = 15,
                            Owner = Caster,
                            PType = PoisonType.Green,
                            TickSpeed = 2000,
                            Value = Value/20
                        });
                    break;
                case Spell.Blizzard:
                    if (ob.Race != ObjectType.Player && ob.Race != ObjectType.Monster) return;
                    if (ob.Dead) return;
                    if (Caster.ActiveBlizzard == false) return;
                    if (!ob.IsAttackTarget(Caster)) return;
                    ob.Attacked(Caster, Value, DefenceType.MACAgility, false);
                    if (!ob.Dead && Envir.Random.Next(8) == 0)
                        ob.ApplyPoison(new Poison
                        {
                            Duration = 5,
                            Owner = Caster,
                            PType = PoisonType.Slow,
                            TickSpeed = 2000,
                        });
                    break;
                case Spell.MeteorStrike:
                    if (ob.Race != ObjectType.Player && ob.Race != ObjectType.Monster) return;
                    if (ob.Dead) return;
                    if (Caster.ActiveBlizzard == false) return;
                    if (!ob.IsAttackTarget(Caster)) return;
                    ob.Attacked(Caster, Value, DefenceType.MACAgility, false);
                    break;
            }
        }

        public override void SetOperateTime()
        {
            long time = Envir.Time + 2000;

            if (TickTime < time && TickTime > Envir.Time)
                time = TickTime;

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

        public override void Process(DelayedAction action)
        {
            throw new NotSupportedException();
        }
        public override bool IsAttackTarget(PlayerObject attacker)
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
        public override bool IsFriendlyTarget(PlayerObject ally)
        {
            throw new NotSupportedException();
        }
        public override bool IsFriendlyTarget(MonsterObject ally)
        {
            throw new NotSupportedException();
        }
        public override void ReceiveChat(string text, ChatType type)
        {
            throw new NotSupportedException();
        }

        public override Packet GetInfo()
        {
            switch (Spell)
            {
                case Spell.Healing:
                    return null;
                case Spell.PoisonField:
                case Spell.Blizzard:
                case Spell.MeteorStrike:
                    if (!Show)
                        return null;

                    return new S.ObjectSpell
                    {
                        ObjectID = ObjectID,
                        Location = CastLocation,
                        Spell = Spell,
                        Direction = Direction
                    };

                default:
                    return new S.ObjectSpell
                    {
                        ObjectID = ObjectID,
                        Location = CurrentLocation,
                        Spell = Spell,
                        Direction = Direction
                    };
            }

        }

        public override void ApplyPoison(Poison p)
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
        public override void SendHealth(PlayerObject player)
        {
            throw new NotSupportedException();
        }
    }
}

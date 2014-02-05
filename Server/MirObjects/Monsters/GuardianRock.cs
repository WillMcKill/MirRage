﻿using System.Collections.Generic;
using System.Drawing;
using Server.MirDatabase;
using S = ServerPackets;

namespace Server.MirObjects.Monsters
{
    class GuardianRock : MonsterObject
    {
        protected override bool CanMove { get { return false; } }

        protected internal GuardianRock(MonsterInfo info)
            : base(info)
        {
            Direction = MirDirection.Up;
        }

        public override void Turn(MirDirection dir)
        {
        }

        public override bool Walk(MirDirection dir) { return false; }

        protected override void ProcessRoam() { }

        protected override bool InAttackRange()
        {
            return CurrentMap == Target.CurrentMap && Functions.InRange(CurrentLocation, Target.CurrentLocation, Info.ViewRange);
        }
        protected override void CompleteAttack(IList<object> data)
        {
            if (!Target.IsAttackTarget(this))
            {
                Target = null;
                return;
            }

            Broadcast(new S.ObjectRangeAttack { ObjectID = ObjectID, Direction = Direction, Location = CurrentLocation, TargetID = Target.ObjectID });
            PullAttack();
            ActionTime = Envir.Time + 300;
            AttackTime = Envir.Time + AttackSpeed;
        }

        private void PullAttack()
        {
            MirDirection pushdir = Functions.DirectionFromPoint(Target.CurrentLocation, CurrentLocation);
            Target.Pushed(this, pushdir, 4);
        }

        protected override void ProcessTarget()
        {
            if (Target == null) return;

            if (InAttackRange() && CanAttack)
            {
                ActionList.Add(new DelayedAction(DelayedType.Damage, Envir.Time + 500));
                return;
            }

            if (Envir.Time < ShockTime)
            {
                Target = null;
                return;
            }
        }

        public override int Attacked(MonsterObject attacker, int damage, DefenceType type = DefenceType.ACAgility)
        {
            return 0;
        }

        public override int Attacked(PlayerObject attacker, int damage, DefenceType type = DefenceType.ACAgility, bool damageWeapon = true)
        {
            return 0;
        }
    }
}

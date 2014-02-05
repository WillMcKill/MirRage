using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client.MirObjects
{
    public class FrameSet
    {
        public static FrameSet Players;
        public static List<FrameSet> NPCs; //Make Array
        public static List<FrameSet> Monsters;

        public Dictionary<MirAction, Frame> Frames = new Dictionary<MirAction, Frame>();


        static FrameSet()
        {
            FrameSet frame;
            NPCs = new List<FrameSet>();

            Monsters = new List<FrameSet>();

            Players = new FrameSet();
            Players.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500, 0, 8, 0, 250));
            Players.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100, 64, 6, 0, 100));
            Players.Frames.Add(MirAction.Running, new Frame(80, 6, 0, 100, 112, 6, 0, 100));
            Players.Frames.Add(MirAction.Stance, new Frame(128, 1, 0, 1000, 160, 1, 0, 1000));
            Players.Frames.Add(MirAction.Stance2, new Frame(300, 1, 5, 1000, 300, 1, 5, 1000));
            Players.Frames.Add(MirAction.Attack1, new Frame(136, 6, 0, 100, 168, 6, 0, 100));
            Players.Frames.Add(MirAction.Attack2, new Frame(184, 6, 0, 100, 216, 6, 0, 100));
            Players.Frames.Add(MirAction.Attack3, new Frame(232, 8, 0, 100, 264, 8, 0, 100));
            Players.Frames.Add(MirAction.Spell, new Frame(296, 6, 0, 100, 328, 6, 0, 100));
            Players.Frames.Add(MirAction.Harvest, new Frame(344, 2, 0, 300, 376, 2, 0, 300));
            Players.Frames.Add(MirAction.Struck, new Frame(360, 3, 0, 100, 392, 3, 0, 100));
            Players.Frames.Add(MirAction.Die, new Frame(384, 4, 0, 100, 416, 4, 0, 100));
            Players.Frames.Add(MirAction.Dead, new Frame(387, 1, 3, 1000, 419, 1, 3, 1000));
            Players.Frames.Add(MirAction.Attack4, new Frame(416, 6, 0, 100, 448, 6, 0, 100));
            Players.Frames.Add(MirAction.Revive, new Frame(384, 4, 0, 100, 416, 4, 0, 100) { Reverse = true });           

            /*
             * NPCS
             */
            NPCs.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 450));
            frame.Frames.Add(MirAction.Harvest, new Frame(12, 10, 0, 200));

            NPCs.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 450));
            frame.Frames.Add(MirAction.Harvest, new Frame(12, 10, 0, 200));

            NPCs.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 450));
            frame.Frames.Add(MirAction.Harvest, new Frame(12, 20, 0, 200));

            NPCs.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 450));


            /*
             * MONSTERS             
             */

            //0
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));


            //1
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.Skeleton, new Frame(224, 1, 0, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //2 - Regular
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //3
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Frames.Add(MirAction.Show, new Frame(4, 8, -8, 200));
            frame.Frames.Add(MirAction.Hide, new Frame(11, 8, -8, 200) { Reverse = true });
            frame.Frames.Add(MirAction.Attack1, new Frame(12, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(60, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(76, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(85, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(76, 10, 0, 100) { Reverse = true });

            //4
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 4, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(147, 1, 3, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 4, 0, 100) { Reverse = true });
            
            //5
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack2, new Frame(128, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(176, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(192, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(201, 1, 9, 1000));

            //6
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 10, -10, 500));
            frame.Frames.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(12, 10, -10, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(12, 10, -10, 100) { Reverse = true });

            //7
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, -4, 1000));
            frame.Frames.Add(MirAction.Show, new Frame(22, 10, -10, 150));
            frame.Frames.Add(MirAction.Hide, new Frame(31, 10, -10, 150) { Reverse = true });
            frame.Frames.Add(MirAction.Attack1, new Frame(4, 6, -6, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(12, 10, -10, 150));
            frame.Frames.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(12, 10, -10, 150) { Reverse = true });

            //8
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, -4, 1000));
            frame.Frames.Add(MirAction.Attack1, new Frame(4, 6, -6, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(12, 10, -10, 150));
            frame.Frames.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(12, 10, -10, 150) { Reverse = true });

            //9
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.Attack2, new Frame(224, 6, 0, 100));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //10
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, -4, 1000));
            frame.Frames.Add(MirAction.Attack1, new Frame(4, 6, -6, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(12, 20, -20, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(31, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(12, 20, -20, 100) { Reverse = true });

            //11
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Stoned, new Frame(0, 1, 5, 100));
            frame.Frames.Add(MirAction.Show, new Frame(0, 6, 0, 100));
            frame.Frames.Add(MirAction.Hide, new Frame(5, 6, 0, 100) { Reverse = true });
            frame.Frames.Add(MirAction.Standing, new Frame(48, 4, 0, 1000));
            frame.Frames.Add(MirAction.Walking, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(128, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(176, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(192, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(201, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(192, 10, 0, 100) { Reverse = true });

            //12
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Stoned, new Frame(0, 1, -1, 100));
            frame.Frames.Add(MirAction.Show, new Frame(0, 20, -20, 100));
            frame.Frames.Add(MirAction.Standing, new Frame(20, 4, 0, 1000));
            frame.Frames.Add(MirAction.Walking, new Frame(52, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(100, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(148, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(164, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(173, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(164, 10, 0, 100) { Reverse = true });

            //13
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(224, 6, 0, 100));
            frame.Frames.Add(MirAction.Stoned, new Frame(272, 1, 5, 100));
            frame.Frames.Add(MirAction.Show, new Frame(272, 6, 0, 100));
            frame.Frames.Add(MirAction.Hide, new Frame(277, 6, 0, 100) { Reverse = true });
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });
            
            //14
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(224, 6, 0, 100));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //15
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.Appear, new Frame(224, 10, -10, 100) { Blend = true });
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //16
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Appear, new Frame(0, 10, 0, 100));
            frame.Frames.Add(MirAction.Standing, new Frame(80, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(112, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(160, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(176, 10, 0, 100));
            frame.Frames.Add(MirAction.Show, new Frame(256, 10, 0, 100));
            frame.Frames.Add(MirAction.Hide, new Frame(265, 10, 0, 100) { Reverse = true });
            frame.Frames.Add(MirAction.Revive, new Frame(176, 10, 0, 100) { Reverse = true });

            //17
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.Show, new Frame(224, 10, 0, 200));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //18
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(224, 10, 0, 100));

            //19
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.AttackRange, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //20
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(224, 6, 0, 100));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //21
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack2, new Frame(128, 6, 0, 200));
            frame.Frames.Add(MirAction.AttackRange, new Frame(176, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(224, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(240, 20, 0, 150));
            frame.Frames.Add(MirAction.Dead, new Frame(259, 1, 19, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(240, 20, 0, 150) { Reverse = true });

            //22
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(224, 6, 0, 100));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });
            frame.Frames.Add(MirAction.SitDown, new Frame(272, 4, 0, 500));

            //23
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack2, new Frame(224, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack3, new Frame(224, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(224, 6, 0, 100));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //24
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Appear, new Frame(216, 10, -10, 100) { Blend = true });
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500) { Blend = true });
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100) { Blend = true });
            frame.Frames.Add(MirAction.AttackRange, new Frame(80, 6, 0, 100) { Blend = true });
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200) { Blend = true });
            frame.Frames.Add(MirAction.Die, new Frame(144, 9, 0, 100) { Blend = true });
            frame.Frames.Add(MirAction.Revive, new Frame(144, 9, 0, 100) { Blend = true, Reverse = true });

            //25
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 18, 1000));
            frame.Frames.Add(MirAction.Attack1, new Frame(4, 6, 16, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(10, 2, 20, 200));
            frame.Frames.Add(MirAction.Die, new Frame(12, 10, 12, 150));
            frame.Frames.Add(MirAction.Dead, new Frame(21, 1, 21, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(12, 10, 12, 150) { Reverse = true });

            //26
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 1, 5, 1000));
            frame.Frames.Add(MirAction.Walking, new Frame(0, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(0, 1, 5, 200));
            frame.Frames.Add(MirAction.Die, new Frame(48, 10, 0, 150));
            frame.Frames.Add(MirAction.Dead, new Frame(57, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(48, 10, 0, 150) { Reverse = true });

            //27
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 1000));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 1, 100));
            frame.Frames.Add(MirAction.AttackRange, new Frame(88, 6, 1, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(144, 1, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(160, 10, 0, 150));
            frame.Frames.Add(MirAction.Dead, new Frame(169, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(160, 10, 0, 150) { Reverse = true });

            //28
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, -4, 1000));
            frame.Frames.Add(MirAction.Attack1, new Frame(4, 6, -6, 180));
            frame.Frames.Add(MirAction.Struck, new Frame(10, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(12, 10, -10, 150));
            frame.Frames.Add(MirAction.Dead, new Frame(21, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(12, 10, -10, 150) { Reverse = true });

            //29
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 1000));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack2, new Frame(464, 20, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 20, 0, 150));
            frame.Frames.Add(MirAction.Dead, new Frame(163, 1, 19, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 20, 0, 150) { Reverse = true });

            //30
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Attack2, new Frame(80, 6, 0, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(128, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(144, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(153, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(144, 10, 0, 100) { Reverse = true });

            //31
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Frames.Add(MirAction.Show, new Frame(4, 5, -5, 200));
            frame.Frames.Add(MirAction.Attack1, new Frame(9, 5, -5, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(14, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(16, 10, -10, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(25, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(16, 10, -10, 100) { Reverse = true });

            //32
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, -4, 500));
            frame.Frames.Add(MirAction.Attack1, new Frame(4, 4, -4, 200));

            //33
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 10, -10, 800));
            frame.Frames.Add(MirAction.Walking, new Frame(10, 10, -10, 80));
            frame.Frames.Add(MirAction.Attack1, new Frame(20, 10, -10, 80));
            frame.Frames.Add(MirAction.Struck, new Frame(30, 4, -4, 200));
            frame.Frames.Add(MirAction.Die, new Frame(34, 10, -10, 120));
            frame.Frames.Add(MirAction.Dead, new Frame(43, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(34, 10, -10, 150) { Reverse = true });

            //34 - GreatFoxSpirit level 0
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 20, -20, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(22, 8, -8, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(20, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Frames.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //35 - GreatFoxSpirit level 1
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(60, 20, -20, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(82, 8, -8, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(80, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Frames.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //36 - GreatFoxSpirit level 2
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(120, 20, -20, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(142, 8, -8, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(140, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Frames.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //37 - GreatFoxSpirit level 3
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(180, 20, -20, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(202, 8, -8, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(200, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Frames.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //38 - GreatFoxSpirit level 4
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(240, 20, -20, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(262, 8, -8, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(260, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(300, 18, -18, 120));
            frame.Frames.Add(MirAction.Dead, new Frame(317, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(300, 18, -18, 150) { Reverse = true });

            //39
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 4, 0, 500));
            frame.Frames.Add(MirAction.Walking, new Frame(32, 6, 4, 100));
            frame.Frames.Add(MirAction.Attack1, new Frame(112, 6, 4, 100));
            frame.Frames.Add(MirAction.Struck, new Frame(192, 2, 0, 200));
            frame.Frames.Add(MirAction.Die, new Frame(208, 10, 0, 100));
            frame.Frames.Add(MirAction.Dead, new Frame(217, 1, 9, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(208, 10, 0, 100) { Reverse = true });
            frame.Frames.Add(MirAction.AttackRange, new Frame(288, 6, 0, 100));

            //40
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(0, 10, -10, 1000));
            frame.Frames.Add(MirAction.Attack1, new Frame(42, 8, -8, 120));
            frame.Frames.Add(MirAction.AttackRange, new Frame(10, 6, 4, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(40, 2, -2, 200));
            frame.Frames.Add(MirAction.Die, new Frame(42, 7, -7, 120));
            frame.Frames.Add(MirAction.Dead, new Frame(48, 1, -1, 1000));
            frame.Frames.Add(MirAction.Revive, new Frame(42, 7, -7, 120) { Reverse = true });

            //41 - Dragon Statue 1
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(300, 1, -1, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(300, 1, -1, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(300, 1, -1, 200));

            //42 - Dragon Statue 2
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(301, 1, -1, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(301, 1, -1, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(301, 1, -1, 200));

            //43 - Dragon Statue 3
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(302, 1, -1, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(302, 1, -1, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(302, 1, -1, 200));

            //44 - Dragon Statue 4
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(320, 1, -1, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(320, 1, -1, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(320, 1, -1, 200));

            //45 - Dragon Statue 5
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(321, 1, -1, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(321, 1, -1, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(321, 1, -1, 200));

            //46 - Dragon Statue 6
            Monsters.Add(frame = new FrameSet());
            frame.Frames.Add(MirAction.Standing, new Frame(322, 1, -1, 1000));
            frame.Frames.Add(MirAction.AttackRange, new Frame(322, 1, -1, 120));
            frame.Frames.Add(MirAction.Struck, new Frame(322, 1, -1, 200));
        }
    }

    

    public class Frame
    {
        public int Start, Count, Skip, EffectStart, EffectCount, EffectSkip;
        public int Interval, EffectInterval;
        public bool Reverse, Blend;

        public int OffSet
        {
            get { return Count + Skip; }
        }

        public int EffectOffSet
        {
            get { return EffectCount + EffectSkip; }
        }

        public Frame(int start, int count, int skip, int interval, int effectstart = 0, int effectcount = 0, int effectskip = 0, int effectinterval = 0)
        {
            Start = start;
            Count = count;
            Skip = skip;
            Interval = interval;
            EffectStart = effectstart;
            EffectCount = effectcount;
            EffectSkip = effectskip;
            EffectInterval = effectinterval;
        }
    }

}

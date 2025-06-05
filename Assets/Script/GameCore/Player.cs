using System;
using System.Collections.Generic;

namespace CardGame
{
    public class Player
    {


        private int _hp = 10;
        private int _curse = 0;

        public virtual int Hp
        {
            get => _hp;
            set => _hp = Math.Max(0, value);
        }

        public virtual int Curse
        {
            get => _curse;
            set => _curse = Math.Max(0, value);
        }

        public int NextDrawNum { get; set; } = 3;
        public int NextPickNum { get; set; } = 1;

        public bool SkipNextTurn { get; set; } = false;
        public int RerollAvailable { get; set; } = 0;
        public int NonCurseDamageTurn { get; set; } = 0;
        public int NonCurseIncreaseTurn { get; set; } = 0;
        public int NonHpIncreaseTurn { get; set; } = 0;
        public int NonCurseDecreaseTurn { get; set; } = 0;
        public int NonHpDecreaseTurn { get; set; } = 0;
        public int NotAddDeath { get; set; } = 0;

        public bool Ember { get; set; } = false;
        public bool Archangel { get; set; } = false;
        public bool RandomChoice { get; set; } = false;

        public bool Chariot { get; set; } = false;

        public Card? LastCard { get; set; } = null;

        public List<(int Delay, Action Effect)> DelayedEffects { get; set; } = new();
<<<<<<< HEAD
        
=======

>>>>>>> 66ec636d998dd922d37a220cff5467f3a4ee2654
        public bool HpChangedThisCard { get; set; }
        public bool CurseChangedThisCard { get; set; }
        public bool DeathCardAddedThisCard { get; set; }
    }
}

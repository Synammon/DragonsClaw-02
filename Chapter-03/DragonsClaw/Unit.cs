using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DragonsClaw
{
    public class Unit
    {
        public string Name { get; set; }
        public int CreditCost { get; set; }
        public int CadetCost { get; set; }
        public int OreCost { get; set; }
        public int DilithiumCost { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }

        internal Unit()
        {

        }

        public Unit(string name, int credits, int cadets, int ore, int dilithium, int attack, int defense)
        {
            Name = name;
            CreditCost = credits;
            CadetCost = cadets;
            OreCost = ore;
            DilithiumCost = dilithium;
            Attack = attack;
            Defense = defense;
        }
    }
}
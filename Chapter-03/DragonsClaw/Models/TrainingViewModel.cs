using DragonsClaw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DragonsClaw.Models
{
    public class TrainingViewModel
    {
        public PlayerViewModel Player { get; set; }
        // Troop names
        public string OffensiveTroop { get; set; }
        public string DefensiveTroop { get; set; }
        public string SpecialtyTroop { get; set; }

        public Unit OffUnit { get; set; }
        public Unit DefUnit { get; set; }
        public Unit SpecUnit { get; set; }
        public Unit Fighter { get; set; }
        public Unit Bomber { get; set; }
        public Unit Cruiser { get; set; }
        public Unit Destroyer { get; set; }
        public Unit Dreadnaught { get; set; }
        public Unit Raider { get; set; }
        public Unit Spy { get; set; }

        // Troops owned
        public int OffensiveTroops { get; set; }
        public int DefensiveTroops { get; set; }
        public int SpecialtyTroops { get; set; }
        public int FighterTroops { get; set; }
        public int SpyTroops { get; set; }
        public int RaiderTroops { get; set; }

        // Troops to be trained
        public int OffensiveUnits { get; set; }
        public int DefensiveUnits { get; set; }
        public int SpecialtyUnits { get; set; }
        public int FighterUnits { get; set; }
        public int BomberUnits { get; set; }
        public int CruiserUnits { get; set; }
        public int DestroyerUnits { get; set; }
        public int DreadnaughtUnits { get; set; }
        public int SpyUnits { get; set; }

        // Maximum troops
        public int MaxOffensive { get; set; }
        public int MaxDefensive { get; set; }
        public int MaxSpecialty { get; set; }
        public int MaxFighters { get; set; }
        public int MaxBombers { get; set; }
        public int MaxCruisers { get; set; }
        public int MaxDestroyers { get; set; }
        public int MaxDreadnaughts { get; set; }
        public int MaxSpies { get; set; }

        public int OffensiveValue { get; set; }
        public int DefensiveValue { get; set; }
        public int SpecialtyOffensiveValue { get; set; }
        public int SpecialtyDefensiveValue { get; set; }

        // Cadets that can be trained
        public int Cadets { get; set; }

        // Credits
        public int Credits { get; set; }
        public int Fighters { get; set; }
        public int Bombers { get; set; }
        public int Cruisers { get; set; }
        public int Destroyers { get; set; }
        public int Dreadnaughts { get; set; }
        public string OffensiveName { get; set; }
        public string DefensiveName { get; set; }
        public string SpecialtyName { get; set; }
        public int MaxRaiders { get; set; }
        public int RaiderUnits { get; set; }
        public sbyte Spies { get; internal set; }
        public string Message { get; internal set; }
        public TrainingViewModel InTraining { get; internal set; }
    }
}
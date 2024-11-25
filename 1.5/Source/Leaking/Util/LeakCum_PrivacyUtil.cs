using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;
using Verse.Sound;
using Verse.Noise;
using Verse.Grammar;
using RimWorld;
using RimWorld.Planet;

using System.Reflection;
using HarmonyLib;

namespace Cumpilation.Leaking
{
    public class LeakCum_PrivacyUtil
    {   
        public static bool CanSeeDeflateSpot(List<Pawn> exclude, IntVec3 pos, Map map, out Pawn pawn)
        {
            foreach (Thing item in GenRadial.RadialDistinctThingsAround(pos, map, 7f, useCenter: true))
            {
                if (item is Pawn p && !p.RaceProps.Animal && !p.RaceProps.IsMechanoid && !exclude.Contains(p) && GenSight.LineOfSight(pos, p.Position, map))
                {
                    pawn = p;
                    return true;
                }
            }
            pawn = null;
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;
using Verse.Noise;
using RimWorld.QuestGen;
using rjw;
using Cumpilation.Cumflation;
using Cumpilation;
using Cumpilation.Gathering;

namespace Cumpilation.Leaking
{
    [StaticConstructorOnStartup]
    public static class AddComp
    {
        static AddComp()
        {
            AddLeakCumComp();
        }
        public static void AddLeakCumComp()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where(x => x.race != null && x.race.Humanlike && !x.IsCorpse))
            {
                thingDef.comps.Add(new CompProperties_SealCum());
            }
        }
    }
}

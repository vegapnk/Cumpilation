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
    public static class ThingDefsDeflate
    {
        public static List<ThingDef> bucketDefs = new List<ThingDef>();
        public static List<ThingDef> cleanDefs = new List<ThingDef>();

        static ThingDefsDeflate()
        {
            GetThingDefsBucket();
            GetThingDefsClean();
        }
        public static void GetThingDefsBucket()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where(x => x.HasComp<Comp_DeflateBucket>()))
            {
                bucketDefs.Add(thingDef);
            }
        }

        public static void GetThingDefsClean()
        {
            foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefs.Where(x => x.HasComp<Comp_DeflateClean>()))
            {
                cleanDefs.Add(thingDef);
            }
        }
    }
}

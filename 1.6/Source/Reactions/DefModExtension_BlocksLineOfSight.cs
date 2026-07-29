using System.Collections.Generic;
using Verse;

namespace Cumpilation.Reactions
{

    [StaticConstructorOnStartup]
    public static class SightBlockerCache
    {
        public static readonly HashSet<ThingDef> LOSBlockingDefs = new();

        static SightBlockerCache()
        {
            foreach (ThingDef def in DefDatabase<ThingDef>.AllDefs)
            {
                if (def.HasModExtension<DefModExtension_BlocksLineOfSight>())
                {
                    LOSBlockingDefs.Add(def);
                }
            }
        }
    }

    public class DefModExtension_BlocksLineOfSight : DefModExtension
    {
        // Mod extension used in ThingDefs XML:
        // <modExtensions>
        //   <li Class="Cumpilation.Reactions.DefModExtension_BlocksLineOfSight"/>
        // </modExtensions>
    }
}

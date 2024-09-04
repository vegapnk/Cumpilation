using Cumpilation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Fluids.Slug
{
    public class HediffCompProperties_SlugExplosionOnDeath : HediffCompProperties
    {

        public float baseRadius = 4;
        public bool radiusMultipliedByBodySize = false;
        public bool radiusMultipliedBySeverity = true;

        public bool doToxicCloud = true;
        public bool doFireExplosion = true;

        public HediffCompProperties_SlugExplosionOnDeath() => this.compClass = typeof(HediffComp_SlugExplosionOnDeath);

    }
}

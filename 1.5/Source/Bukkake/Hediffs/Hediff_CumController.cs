using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Cumpilation.Bukkake
{
    public class Hediff_CumController : HediffWithComps
    {
        
        public override bool TryMergeWith(Hediff other)
        {
            if (other == null || other.def != this.def)
            {
                return false;
            }
            return true;
        }


    }
}

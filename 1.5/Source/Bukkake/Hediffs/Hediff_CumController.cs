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
        
        private static readonly float cumWeight = 0.2f;//how much individual cum_hediffs contribute to the overall bukkake severity
        List<Hediff> hediffs_cum;
        

        //new Hediff_CumController added to pawn -> just combine the two
        public override bool TryMergeWith(Hediff other)
        {
            if (other == null || other.def != this.def)
            {
                return false;
            }
            return true;
        }

        private float CalculateLevel()
        {
            float num = 0f;
            for (int i = 0; i < hediffs_cum.Count; i++)
            {
                num += hediffs_cum[i].Severity * cumWeight;
            }
            return num;
        }


    }
}

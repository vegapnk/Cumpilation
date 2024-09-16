using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Oscillation
{
    public class OscillationHelper
    {
        /// <summary>
        /// Gets (or creates) a BlueBall Hediff for the pawn, 
        /// located in the Genitals BodyPart. 
        /// The severity is set later in case of a new creation.
        /// </summary>
        /// <param name="pawn"></param>
        /// <returns>A Blueball Hediff with Severity 0 of the pawn.</returns>
        public static Hediff GetOrCreateBlueBallsHediff(Pawn pawn)
        {
            BodyPartRecord bodyPartRecord = Genital_Helper.get_genitalsBPR(pawn);

            Hediff blueballHediff = pawn.health.hediffSet.GetFirstHediffOfDef(DefOfs.Cumpilation_BlueBalls);
            if (blueballHediff == null)
            {
                blueballHediff = HediffMaker.MakeHediff(DefOfs.Cumpilation_BlueBalls, pawn, bodyPartRecord);
                blueballHediff.Severity = 0;
                pawn.health.AddHediff(blueballHediff, bodyPartRecord);
            }
            return blueballHediff;
        }

    }
}

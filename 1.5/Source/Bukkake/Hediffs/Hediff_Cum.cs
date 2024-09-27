using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Cumpilation.Bukkake
{
    public class Hediff_Cum : HediffWithComps
    {

        public override string LabelInBrackets
        {
            get
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append(base.LabelInBrackets);
                if (this.sourceHediffDef != null)
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(this.sourceHediffDef.label);
                }
                else if (this.sourceDef != null)
                {
                    if (stringBuilder.Length != 0)
                    {
                        stringBuilder.Append(", ");
                    }
                    stringBuilder.Append(this.sourceLabel);
                    if (this.sourceBodyPartGroup != null)
                    {
                        stringBuilder.Append(" ");
                        stringBuilder.Append(this.sourceBodyPartGroup.LabelShort);
                    }
                }
                return stringBuilder.ToString();
            }
        }


        public override string SeverityLabel
        {
            get
            {
                if (this.Severity == 0f)
                {
                    return null;
                }
                return this.Severity.ToString("F1");
            }
        }

        //[SyncMethod]
        public override bool TryMergeWith(Hediff other)
        {
            //if a new cum hediff is added to the same body part, they are combined. if severity reaches more than 1, spillover to other body parts occurs

            /*
            Hediff_Cum hediff_cum = other as Hediff_Cum;
            if (hediff_cum != null && hediff_cum.def == this.def && hediff_cum.Part == base.Part && this.def.injuryProps.canMerge)
            {
                cumType = hediff_cum.cumType;//take over new creature color

                float totalAmount = hediff_cum.Severity + this.Severity;
                if (totalAmount > 1.0f)
                {
                    BodyPartDef spillOverTo = BukkakeUtility.spillover(this.Part.def);//cumHelper saves valid other body parts for spillover
                    if (spillOverTo != null)
                    {
                        //Rand.PopState();
                        //Rand.PushState(RJW_Multiplayer.PredictableSeed());
                        IEnumerable<BodyPartRecord> availableParts = BukkakeUtility.getAvailableBodyParts(pawn);//gets all non missing, valid body parts
                        IEnumerable<BodyPartRecord> filteredParts = availableParts.Where(x => x.def == spillOverTo);//filters again for valid spill target
                        if (!filteredParts.EnumerableNullOrEmpty())
                        {
                            BodyPartRecord spillPart = null;
                            spillPart = filteredParts.RandomElement<BodyPartRecord>();//then pick one
                            if (spillPart != null)
                            {
                                BukkakeUtility.cumOn(pawn, spillPart, totalAmount - this.Severity, null, cumType);
                            }
                        }
                    }
                }

                return (base.TryMergeWith(other));

            }
            */
            return (false);
        }



    }
}

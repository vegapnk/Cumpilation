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

    }
}

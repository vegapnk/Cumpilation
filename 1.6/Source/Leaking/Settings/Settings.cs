using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Cumpilation.Leaking
{
    public class Settings : ModSettings
    {
        /// <summary>
        /// When enabled, pawns with an inflation hediff will leak cum(or other relevent fluids) over the floor as they slowly deflate.
        /// </summary>
        public static bool EnableFilthGeneration = true;
        public static bool EnableAutoDeflateBucket = false;
        public static bool EnableAutoDeflateClean = false;
        public static bool EnableAutoDeflateDirty = false;
        public static bool EnablePrivacy = true;
        public static float AutoDeflateMinSeverity = 0.4f;
        public static float AutoDeflateMaxDistance = 100f;
        public static float LeakMult = 5.0f;
        public static float LeakRate = 1.0f;
        public static float DeflateMult = 5.0f;
        public static float DeflateRate = 1.0f;

        public override void ExposeData()
        {

            base.ExposeData();
            Scribe_Values.Look<bool>(ref EnableFilthGeneration, "EnableFilthGeneration", true);
            Scribe_Values.Look<bool>(ref EnableAutoDeflateBucket, "EnableAutoDeflateBucket", true);
            Scribe_Values.Look<bool>(ref EnableAutoDeflateClean, "EnableAutoDeflateClean", false);
            Scribe_Values.Look<bool>(ref EnableAutoDeflateDirty, "EnableAutoDeflateDirty", false);
            Scribe_Values.Look<bool>(ref EnablePrivacy, "EnablePrivacy", true);

            Scribe_Values.Look<float>(ref AutoDeflateMinSeverity, "AutoDeflateMinSeverity", 0.4f);
            Scribe_Values.Look<float>(ref AutoDeflateMaxDistance, "AutoDeflateMaxDistance", 100f);

            Scribe_Values.Look<float>(ref LeakMult, "LeakMult", 5.0f);
            Scribe_Values.Look<float>(ref LeakRate, "LeakRate", 1.0f);
            Scribe_Values.Look<float>(ref DeflateMult, "DeflateMult", 5.0f);
            Scribe_Values.Look<float>(ref DeflateRate, "DeflateRate", 1.0f);
        }

        private static readonly float height_modifier = 100f;

        public static void DoWindowContents(Rect inRect)
        {
            Rect viewRect = new Rect(0f, 0f, inRect.width - 16f, inRect.height + height_modifier);

            Listing_Standard listingStandard = new Listing_Standard
            {
                maxOneColumn = true,
                ColumnWidth = viewRect.width / 2.05f
            };

            listingStandard.Begin(inRect);
            listingStandard.Gap(4f);
            listingStandard.CheckboxLabeled("cumpilation_cumsettings_enable_filth_key".Translate(), ref EnableFilthGeneration, "cumpilation_cumsettings_enable_filth_desc".Translate());
            listingStandard.Gap(4f);

            listingStandard.CheckboxLabeled("cumpilation_cumsettings_enable_autodeflatebucket_key".Translate(), ref EnableAutoDeflateBucket, "cumpilation_cumsettings_enable_autodeflatebucket_desc".Translate());
            listingStandard.Gap(4f);

            listingStandard.CheckboxLabeled("cumpilation_cumsettings_enable_autodeflatedubs_key".Translate(), ref EnableAutoDeflateClean, "cumpilation_cumsettings_enable_autodeflatedubs_desc".Translate());
            listingStandard.Gap(4f);

            listingStandard.CheckboxLabeled("cumpilation_cumsettings_enable_autodeflatedirty_key".Translate(), ref EnableAutoDeflateDirty, "cumpilation_cumsettings_enable_autodeflatedirty_desc".Translate());
            listingStandard.Gap(4f);

            listingStandard.CheckboxLabeled("cumpilation_cumsettings_enable_privacy_key".Translate(), ref EnablePrivacy, "cumpilation_cumsettings_enable_privacy_desc".Translate());
            listingStandard.Gap(4f);

            listingStandard.Label("cumpilation_cumsettings_min_autodeflate_key".Translate() + Math.Round(AutoDeflateMinSeverity, 2).ToString(), tooltip: "cumpilation_cumsettings_min_autodeflate_desc".Translate());
            AutoDeflateMinSeverity = listingStandard.Slider(AutoDeflateMinSeverity, 0f, 3f);
            listingStandard.Gap(4f);

            listingStandard.Label("cumpilation_cumsettings_max_deflatedistance_key".Translate() + Math.Round(AutoDeflateMaxDistance, 1).ToString(), tooltip: "cumpilation_cumsettings_max_deflatedistance_desc".Translate());
            AutoDeflateMaxDistance = listingStandard.Slider(AutoDeflateMaxDistance, 0f, 1000f);
            listingStandard.Gap(4f);

            listingStandard.Label("cumpilation_cumsettings_leak_amount_multi_key".Translate() + Math.Round(LeakMult, 1).ToString(), tooltip: "cumpilation_cumsettings_leak_amount_multi_desc".Translate());
            LeakMult = listingStandard.Slider(LeakMult, 0.1f, 10f);
            listingStandard.Gap(4f);

            listingStandard.Label("cumpilation_cumsettings_leak_speed_multi_key".Translate() + Math.Round(LeakRate, 1).ToString(), tooltip: "cumpilation_cumsettings_leak_speed_multi_desc".Translate());
            LeakRate = listingStandard.Slider(LeakRate, 0.1f, 10f);
            listingStandard.Gap(4f);

            listingStandard.Label("cumpilation_cumsettings_deflate_amount_multi_key".Translate() + Math.Round(DeflateMult, 1).ToString(), tooltip: "cumpilation_cumsettings_deflate_amount_multi_desc".Translate());
            DeflateMult = listingStandard.Slider(DeflateMult, 0.1f, 10f);
            listingStandard.Gap(4f);

            listingStandard.Label("cumpilation_cumsettings_deflate_speed_multi_key".Translate() + Math.Round(DeflateRate, 1).ToString(), tooltip: "cumpilation_cumsettings_deflate_speed_multi_desc".Translate());
            DeflateRate = listingStandard.Slider(DeflateRate, 0.1f, 10f);
            listingStandard.Gap(4f);

            listingStandard.End();
        }

    }
}
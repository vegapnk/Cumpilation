using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Cumpilation.Settings
{
    public class Settings : ModSettings
    {
        public static bool EnableCumflation = true;
        public static float GlobalCumflationModifier = 1.0f;

        public static bool EnableStuffing = true;
        public static float GlobalStuffingModifier = 1.0f;

        public static bool EnableBukkake = true;
        public static float GlobaleBukkakeModifier = 1.0f;


        public static float MaxGatheringCheckDistance = 15.0f;
        public static bool EnableProgressingConsumptionThoughts = true;

        public static bool EnableCumpilationDebugLogging = false;


        public override void ExposeData()
        {

            base.ExposeData(); 
            Scribe_Values.Look<bool>(ref EnableCumflation, "EnableCumflation", true);
            Scribe_Values.Look<float>(ref GlobalCumflationModifier, "GlobalCumflationModifier", 1.0f);
            Scribe_Values.Look<bool>(ref EnableStuffing, "EnableStuffing", true);
            Scribe_Values.Look<float>(ref GlobalStuffingModifier, "GlobalStuffingModifier", 1.0f);
            Scribe_Values.Look<bool>(ref EnableBukkake, "EnableBukkake", true);
            Scribe_Values.Look<float>(ref GlobaleBukkakeModifier, "GlobaleBukkakeModifier", 1.0f);

            Scribe_Values.Look<bool>(ref EnableProgressingConsumptionThoughts, "EnableProgressingConsumptionThoughts", true);
            Scribe_Values.Look<float>(ref MaxGatheringCheckDistance, "MaxGatheringCheckDistance", 15.0f);
            Scribe_Values.Look<bool>(ref EnableCumpilationDebugLogging, "EnableCumpilationDebugLogging", true);
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

            listingStandard.CheckboxLabeled("cumpilation_settings_enable_cumflation_key".Translate()+": ", ref EnableCumflation, "cumpilation_settings_enable_cumflation_desc".Translate());
            if (EnableCumflation)
            {
                listingStandard.Gap(4f);
                listingStandard.Label("cumpilation_settings_cumflation_modifier_key".Translate()+": " + Math.Round((double)(GlobalCumflationModifier), 1).ToString(), tooltip: "cumpilation_settings_cumflation_modifier_desc".Translate());
                GlobalCumflationModifier = listingStandard.Slider(GlobalCumflationModifier, 0.1f, 5f);
            }
            listingStandard.Gap(5f);


            listingStandard.CheckboxLabeled("cumpilation_settings_enable_stuffing_key".Translate() + ": ", ref EnableStuffing, "cumpilation_settings_enable_stuffing_desc".Translate());
            if (EnableStuffing)
            {
                listingStandard.Gap(4f);
                listingStandard.Label("cumpilation_settings_stuffing_modifier_key".Translate() + ": " + Math.Round((double)(GlobalStuffingModifier), 1).ToString(), tooltip: "cumpilation_settings_stuffing_modifier_desc".Translate());
                GlobalStuffingModifier = listingStandard.Slider(GlobalStuffingModifier, 0.1f, 5f);
            }
            listingStandard.Gap(5f);


            listingStandard.CheckboxLabeled("cumpilation_settings_enable_bukkake_key".Translate() + ": ", ref EnableBukkake, "cumpilation_settings_enable_bukkake_desc".Translate());
            if (EnableBukkake)
            {
                listingStandard.Gap(4f);
                listingStandard.Label("cumpilation_settings_bukkake_modifier_key".Translate() + ": " + Math.Round((double)(GlobaleBukkakeModifier), 1).ToString(), tooltip: "cumpilation_settings_bukkake_modifier_desc".Translate());
                GlobaleBukkakeModifier = listingStandard.Slider(GlobaleBukkakeModifier, 0.1f, 5f);
            }
            listingStandard.Gap(5f);


            listingStandard.Label("cumpilation_settings_max_gathering_check_distance_key".Translate() + ": " + Math.Round((double)(MaxGatheringCheckDistance), 1).ToString() , tooltip: "cumpilation_settings_max_gathering_check_distance_desc".Translate());
            MaxGatheringCheckDistance = listingStandard.Slider(MaxGatheringCheckDistance, 3.0f, 50f);
            listingStandard.Gap(4f);
            listingStandard.CheckboxLabeled("cumpilation_settings_enable_progressing_consumption_thoughts_key".Translate() + ": ", ref EnableProgressingConsumptionThoughts, "cumpilation_settings_enable_progressing_consumption_thoughts_desc".Translate());

            listingStandard.Gap(8f);
            listingStandard.CheckboxLabeled("cumpilation_settings_enable_debug_logging_key".Translate() + ": ", ref EnableProgressingConsumptionThoughts, "cumpilation_settings_enable_debug_logging_desc".Translate());

            listingStandard.End();
        }

    }
}

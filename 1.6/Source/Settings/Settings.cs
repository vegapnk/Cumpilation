using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Cumpilation
{
    public class Settings : ModSettings
    {
        /// <summary>
        /// Whether or not Cumflation-Code is run at all
        /// </summary>
        public static bool EnableCumflation = true;
        /// <summary>
        /// A global multiplier how much fluid is needed to cumflate all pawns. 
        /// Their size will also still be taken into account. 
        /// E.g. if set to 2, you will need twice as much fluid to inflate any given pawn.
        /// </summary>
        public static float GlobalCumflationModifier = 1.0f;
        /// <summary>
        /// Whether or not Stuffing-Code is run at all
        /// </summary>
        public static bool EnableStuffing = true;
        /// <summary>
        /// A global multiplier how much fluid is needed to stuff all pawns. 
        /// Their size will also still be taken into account. 
        /// E.g. if set to 2, you will need twice as much fluid to stuff any given pawn.
        /// </summary>
        public static float GlobalStuffingModifier = 1.0f;
        /// <summary>
        /// Whether or not Covering-Code is run at all
        /// </summary>
        public static bool EnableBukkake = true;
        /// <summary>
        /// A global multiplier how much fluid is needed to cover all pawns. 
        /// Their size will also still be taken into account. 
        /// E.g. if set to 2, you will need twice as much fluid to cover any given pawn.
        /// </summary>
        public static float GlobaleBukkakeModifier = 1.0f;


        /// <summary>
        /// When enabled, there is a chance to gather fluids while doing the normal cleaning Task. 
        /// The chances are based on the individual FluidGatheringDefs and if there is none, nothing will happen. 
        /// </summary>
        public static bool EnableFluidGatheringWhileCleaning = true;

        /// <summary>
        /// When the Gathering Code runs, it checks all items in the room in a certain distance. 
        /// This variable keeps track as high numbers can produce performance issues in big rooms (such as outdoors). 
        /// The buildings themselves still have a check for their in-built distance, this is mostly about finding the right possible candidates. 
        /// Note: if the buildings distance is higher than the Max-Gathering distance, the Max-Gathering-Check Distance is an upper bound.
        /// </summary>
        public static float MaxGatheringCheckDistance = 15.0f;
        /// <summary>
        /// Whether or not the pawns gain thoughts on fluid-consumption. 
        /// The records are updated regardless. 
        /// </summary>
        public static bool EnableProgressingConsumptionThoughts = true;
        /// <summary>
        /// Whether or not Drained, Wetness and Blueballs are done. 
        /// </summary>
        public static bool EnableOscillationMechanics = true;
        /// <summary>
        /// Whether or not to have debug-prints on the Mod-Log. 
        /// Passively used in `ModLog.Debug(str)`
        /// </summary>
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
            

            Scribe_Values.Look<bool>(ref EnableFluidGatheringWhileCleaning, "EnableFluidGatheringWhileCleaning", true);
            Scribe_Values.Look<bool>(ref EnableProgressingConsumptionThoughts, "EnableProgressingConsumptionThoughts", true);
            Scribe_Values.Look<float>(ref MaxGatheringCheckDistance, "MaxGatheringCheckDistance", 15.0f);
            Scribe_Values.Look<bool>(ref EnableOscillationMechanics, "EnableOscillationMechanics", true);
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
            listingStandard.CheckboxLabeled("cumpilation_settings_enable_fluid_gathering_while_cleaning_key".Translate() + ": ", ref EnableFluidGatheringWhileCleaning, "cumpilation_settings_enable_fluid_gathering_while_cleaning_desc".Translate());
            listingStandard.Gap(4f);
            listingStandard.Label("cumpilation_settings_max_gathering_check_distance_key".Translate() + ": " + Math.Round((double)(MaxGatheringCheckDistance), 1).ToString() , tooltip: "cumpilation_settings_max_gathering_check_distance_desc".Translate());
            MaxGatheringCheckDistance = listingStandard.Slider(MaxGatheringCheckDistance, 3.0f, 50f);
            listingStandard.Gap(4f);
            listingStandard.CheckboxLabeled("cumpilation_settings_enable_progressing_consumption_thoughts_key".Translate() + ": ", ref EnableProgressingConsumptionThoughts, "cumpilation_settings_enable_progressing_consumption_thoughts_desc".Translate());
            listingStandard.Gap(4f);
            listingStandard.CheckboxLabeled("cumpilation_settings_enable_oscillation_mechanics_key".Translate() + ": ", ref EnableOscillationMechanics, "cumpilation_settings_enable_oscillation_mechanics_desc".Translate());

            listingStandard.Gap(8f);
            listingStandard.CheckboxLabeled("cumpilation_settings_enable_debug_logging_key".Translate() + ": ", ref EnableCumpilationDebugLogging, "cumpilation_settings_enable_debug_logging_desc".Translate());

            listingStandard.End();
        }

    }
}

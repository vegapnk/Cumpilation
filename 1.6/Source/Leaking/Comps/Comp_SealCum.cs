using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;
using RimWorld;
using Verse.Noise;
using RimWorld.QuestGen;
using rjw;
using Cumpilation.Cumflation;
using Cumpilation;
using Cumpilation.Gathering;

namespace Cumpilation.Leaking
{
    public class Comp_SealCum : ThingComp
    {
        private CompProperties_SealCum Props => (CompProperties_SealCum)props;
        private bool cumSealed = false;
        private bool canDeflate = true;
        private static readonly CachedTexture Icon1 = new CachedTexture("UI/Plug");
        private static readonly CachedTexture Icon2 = new CachedTexture("UI/DeflateAllowed");
        private static readonly CachedTexture Icon3 = new CachedTexture("UI/DeflateForbidden");

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref cumSealed, "cumSealed", false);
            Scribe_Values.Look(ref canDeflate, "canDeflate", true);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo gizmo in base.CompGetGizmosExtra())
            {
                yield return gizmo;
            }
            Pawn pawn = (Pawn)parent;
            if (pawn.DevelopmentalStage != DevelopmentalStage.Adult || !Genital_Helper.has_vagina(pawn))
                yield break;
            if (Settings.EnableCumSealToggle && canSeal())
            {
                Command_Toggle seal_Toggle = new Command_Toggle();
                seal_Toggle.isActive = () => cumSealed;
                seal_Toggle.toggleAction = delegate
                {
                    cumSealed = !cumSealed;
                };
                seal_Toggle.defaultDesc = "cumpilation_button_cumseal_desc".Translate();
                seal_Toggle.icon = Icon1.Texture;
                seal_Toggle.defaultLabel = cumSealed ? "cumpilation_button_cumseal_sealed".Translate() : "cumpilation_button_cumseal_unsealed".Translate();
                seal_Toggle.activateSound = SoundDefOf.Tick_Tiny;
                yield return seal_Toggle;
            }
            if (Settings.EnableAutoDeflateToggle && CumflationUtility.CanBeCumflated(pawn) && PlayerControlled)
            {
                Command_Toggle deflate_Toggle = new Command_Toggle();
                deflate_Toggle.isActive = () => canDeflate;
                deflate_Toggle.toggleAction = delegate
                {
                    canDeflate = !canDeflate;
                };
                deflate_Toggle.defaultDesc = "cumpilation_button_cumdeflate_desc".Translate();
                deflate_Toggle.icon = canDeflate ? Icon2.Texture : Icon3.Texture;
                deflate_Toggle.defaultLabel = canDeflate ? "cumpilation_button_cumdeflate_deflate".Translate() : "cumpilation_button_cumdeflate_nodeflate".Translate();
                deflate_Toggle.activateSound = SoundDefOf.Tick_Tiny;
                yield return deflate_Toggle;
            }
            else
            {
                canDeflate = true;
            }
        }

        public bool CanDeflate()
        {
            return canDeflate;
        }

        public bool PlayerControlled
        {
            get
            {
                Pawn pawn = (Pawn)parent;
                if (pawn.IsColonist)
                {
                    if (pawn.HostFaction != null)
                    {
                        return pawn.IsSlave;
                    }
                    return true;
                }
                return false;
            }
        }

        public bool IsSealed()
        {
            return canSeal() && cumSealed;
        }

        public bool canSeal()
        {
            Pawn pawn = (Pawn)parent;

            if (pawn.Dead || !PlayerControlled)
            {
                return false;
            }

            var GenitalBPR = Genital_Helper.get_genitalsBPR(pawn);
            IEnumerable<ISexPartHediff> vaginas = Genital_Helper.get_PartsHediffList(pawn, GenitalBPR).Where(x => Genital_Helper.is_vagina(x)).Cast<ISexPartHediff>();
            if (!vaginas.Any())
            {
                return false;
            }

            if (!vaginas.Any(v => !v.Def.partTags.Contains("Resizable")))
            {
                return true;
            }

            if (pawn.health.hediffSet.HasHediff(DefOfs.Cumpilation_Sealed))
            {
                return true;
            }

            return false;
        }

    }
}

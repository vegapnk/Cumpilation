﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Reactions
{
    /// <summary>
    /// Thought class that uses record to select active stage 
    /// Initially taken from Ameravashis Sexperience, extended to cover multiple fluids, optional quirks and traits that hardwire results.
    /// </summary>
    public class Thought_Recordbased : Thought_Memory
    {
        private ThoughtDefExtension_StageFromConsumption extension;
        protected ThoughtDefExtension_StageFromConsumption Extension => extension ?? (extension = def.GetModExtension<ThoughtDefExtension_StageFromConsumption>());

        /// <summary>
        /// This method is called for every thought right after the pawn is assigned
        /// </summary>
        public override bool TryMergeWithExistingMemory(out bool showBubble)
        {
            UpdateCurStage();
            return base.TryMergeWithExistingMemory(out showBubble);
        }

        protected virtual void UpdateCurStage()
        {
            ModLog.Debug($"Trying to add / update thought {this.def.defName} for {pawn}");
            SetForcedStage(Extension.GetStageIndex(pawn));
        }

    }

}

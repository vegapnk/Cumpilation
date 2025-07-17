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

namespace Cumpilation.Leaking
{
	public class CompProperties_DeflateClean : CompProperties
	{
		public float deflateRate = 1f;
		public CompProperties_DeflateClean()
		{
			compClass = typeof(Comp_DeflateClean);
		}
	}
}

using rjw;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation.Common
{
    public interface IPartTargetter : IPawnTargetter
    {
        public IEnumerable<ISexPartHediff> GetSexPartHediffs(Pawn pawn);

    }
}

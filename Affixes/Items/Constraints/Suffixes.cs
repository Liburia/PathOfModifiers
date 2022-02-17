using System.Collections.Generic;
using System.Linq;

namespace PathOfModifiers.Affixes.Items.Constraints
{
    public class Suffixes : Constraint
    {
        protected override IEnumerable<Affix> ProcessInner(IEnumerable<Affix> input)
        {
            return input.Where((affix) => affix.IsSuffix);
        }
    }
}

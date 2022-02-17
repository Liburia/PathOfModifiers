using System.Collections.Generic;
using System.Linq;

namespace PathOfModifiers.Affixes.Items.Constraints
{
    public class Prefixes : Constraint
    {
        protected override IEnumerable<Affix> ProcessInner(IEnumerable<Affix> input)
        {
            return input.Where((affix) => affix.IsPrefix);
        }
    }
}

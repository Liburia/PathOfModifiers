using System.Collections.Generic;

namespace PathOfModifiers.Affixes.Items.Constraints
{
    public abstract class Constraint
    {
        List<Constraint> nextConstraints = new List<Constraint>();
        public virtual Constraint Then(Constraint nextConstraint)
        {
            nextConstraints.Add(nextConstraint);
            return this;
        }

        public IEnumerable<Affix> Process(IEnumerable<Affix> input)
        {
            var output = ProcessInner(input);

            foreach (var c in nextConstraints)
            {
                output = c.Process(output);
            }

            return output;
        }

        protected virtual IEnumerable<Affix> ProcessInner(IEnumerable<Affix> input)
        {
            return input;
        }
    }
}

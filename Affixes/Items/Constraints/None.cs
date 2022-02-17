namespace PathOfModifiers.Affixes.Items.Constraints
{
    public class None : Constraint
    {
        public override Constraint Then(Constraint nextConstraint)
        {
            return nextConstraint;
        }
    }
}

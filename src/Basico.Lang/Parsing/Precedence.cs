namespace Basico.Parsing
{
    public enum Precedence
    {
        Lowest,
        Statement,
        Assign,
        AndOr,
        LessThanGreaterThan,
        Equals,
        Sum,
        Product,
        Pow,
        Prefix,
        Call,
    }
}

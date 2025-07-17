public static class ValueCapper
{
    private static readonly (int minValue, int maxValue, int cappedValue)[] cappingRules =
    {
        (0, 149, 1),
        (150, 299, 2),
        (300, 449, 3),
        (450, 599, 4),
        (600, 749, 5),
        (750, 899, 6),
        (900, 1099, 7),
        (1100, 1299, 8),
        (1300, 1599, 9),
        (1600, int.MaxValue, 10)
    };

    public static int CapValue(int rawValue)
    {
        foreach (var rule in cappingRules)
        {
            if(rawValue >= rule.minValue && rawValue <= rule.maxValue)
            {
                return rule.cappedValue;
            }
        }
        return 10;
    }
}

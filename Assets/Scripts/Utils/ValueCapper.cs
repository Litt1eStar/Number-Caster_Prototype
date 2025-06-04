public static class ValueCapper
{
    private static readonly (int minValue, int maxValue, int cappedValue)[] cappingRules =
    {
        (0, 99, 1),
        (100, 300, 2),
        (301, 500, 3),
        (501, 800, 4),
        (801, 1200, 5),
        (1201, 1700, 6),
        (1701, 2500, 7),
        (2501, 3500, 8),
        (3501, 5000, 9),
        (5001, int.MaxValue, 10)
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

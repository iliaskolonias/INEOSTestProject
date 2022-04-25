namespace INEOSTestProject
{
    internal class PowerPositionPeriod
    {
        internal int Period { get; set; }

        internal double Volume { get; set;}

    }
    internal class PowerPosition
    {
        internal DateTime Date { get; set; }

        //internal PowerPositionPeriod[] Periods { get; set; }
        internal Dictionary<int, PowerPositionPeriod> Periods { get; set; }

        internal PowerPosition(DateTime date, int numberOfPeriods = 24)
        {
            Date = date;
            Periods = (from period in Enumerable.Range(1, numberOfPeriods)
                       select new PowerPositionPeriod
                       {
                           Period = period,
                           Volume = 0
                       }).ToDictionary(p => p.Period, p => p);
        }
    }
}

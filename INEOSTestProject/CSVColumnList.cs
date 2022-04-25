namespace INEOSTestProject
{
    internal class CSVColumnList : Dictionary<int, string>
    {
        private ILogger<INEOSTestService> _logger;
        internal CSVColumnList(ILogger<INEOSTestService> logger, string columnList, char columnDelimiter = ',')
        {
            _logger = logger;

            try
            {
                string[] columns = columnList.Split(columnDelimiter, StringSplitOptions.TrimEntries);
                int order = 1;
                foreach (string column in columns)
                {
                    if (!string.IsNullOrEmpty(column)) AddColumn(order++, column);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error assembling CSV column list - error was: {e.Message}");
                throw new INEOSTestProjectException(e.Message, e);
            }
        }

        internal void AddColumn(int columnOrder, string columnName)
        {
            if (ContainsValue(columnName)) throw new INEOSTestProjectException("CSV column list cannot be generated; duplicate in column list");
            if (!TryAdd(columnOrder, columnName)) throw new INEOSTestProjectException("CSV column list cannot be generated; cannot compile column list");
        }
    }
}

using System.Text;

namespace INEOSTestProject
{
    internal class CSVWriter
    {
        private ILogger<INEOSTestService> _logger;
        internal CSVColumnList ColumnList { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        internal string FileHeader { get; set; }
        internal char FileDelimiter { get; set; }

        internal CSVWriter(IConfiguration configuration, ILogger<INEOSTestService> logger, DateTime timeToRun)
        {
            _logger = logger;
            try
            {
                FileHeader = configuration.GetValue<string>("Configuration:OutputFileHeader");
                FileDelimiter = configuration.GetValue<char>("Configuration:OutputFileDelimiter");
                ColumnList = new CSVColumnList(_logger, FileHeader, FileDelimiter);

                FilePath = configuration.GetValue<string>("Configuration:OutputFileDirectory");
                FileName = configuration.GetValue<string>("Configuration:OutputFileNameTemplate");
                FileName = FileName.Replace("YYYYMMDD", String.Format("{0:0000}{1:00}{2:00}", timeToRun.Year, timeToRun.Month, timeToRun.Day), true, null);
                FileName = FileName.Replace("HHMM", String.Format("{0:00}{1:00}", timeToRun.Hour, timeToRun.Minute), true, null);
            }
            catch (Exception e)
            {
                throw new INEOSTestProjectException($"Failed to assemble output file name, error is: {e.Message}", e);
            }
        }
        internal async Task doWrite(PowerPositionCalculator powerPositionCalculator, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                token.ThrowIfCancellationRequested();
            }

            if (String.IsNullOrEmpty(FileName)) throw new INEOSTestProjectException("CSV file cannot be generated; file name not defined");
            if (null == ColumnList) throw new INEOSTestProjectException("CSV file cannot be generated; column list not defined");
            StringBuilder allData = new StringBuilder();
            allData.AppendLine(FileHeader);
            foreach (var pp in powerPositionCalculator.powerPosition.Periods)
            {
                allData.AppendLine(String.Format("{0:00}:00,{1}", (pp.Key + 22) % 24, pp.Value.Volume));
            }
            try
            {
                using (StreamWriter file = new StreamWriter(Path.Combine(FilePath, FileName), false, Encoding.Default))
                {
                    await file.WriteAsync(allData, token);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"Error writing out to CSV file - error was: {e.Message}");
                throw new INEOSTestProjectException(e.Message, e);
            }
            return;
        }
    }
}

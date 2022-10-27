namespace JiraTesterProService.FileHandler
{
    public class DelimettedFileService : FileServiceBase
    {
        private ILogger<DelimettedFileService> logger;
        public DelimettedFileService(ILogger<DelimettedFileService> logger)
        {
            this.logger = logger;
        }




        public override DataTable GetDataTableFromFile(MemoryStream stream)
        {
            var dt = new DataTable();
            //Get the delimeter
            using (var reader = new StreamReader(stream))
            {
                var delimiter = DetectDelimiter(reader);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {

                    Delimiter = delimiter,
                    BadDataFound = context =>
                    {
                        logger.LogError($"Bad data found on row '{context.RawRecord}'");

                    },
                    HasHeaderRecord = true,
                    MissingFieldFound = null

                };


                using (var csv = new CsvReader(reader, config))
                {

                    using (var dr = new CsvDataReader(csv))
                    {
                        try
                        {
                            dt.Load(dr);
                        }
                        catch (Exception ex)
                        {
                            logger.LogError(ex.Message);

                        }



                    }
                }
            }
            foreach (var column in dt.Columns)
            {
                dt.Columns[column.ToString()].ColumnName = column.ToString().Replace("\"", "").StandardiseColumnTableName();
            }

            return dt;

        }

        private string DetectDelimiter(StreamReader reader)
        {
            // assume one of following delimiters
            var possibleDelimiters = new List<string> { ",", ";", "\t", "|" };

            var headerLine = reader.ReadLine();

            // reset the reader to initial position for outside reuse
            // Eg. Csv helper won't find header line, because it has been read in the Reader
            reader.BaseStream.Position = 0;
            reader.DiscardBufferedData();

            foreach (var possibleDelimiter in possibleDelimiters)
            {
                if (headerLine.Contains(possibleDelimiter))
                {
                    return possibleDelimiter;
                }
            }

            return possibleDelimiters[0];
        }
    }
}

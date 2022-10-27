

namespace JiraTesterProService.FileHandler
{
    public class FileGenerator<T> : IFileGenerator<T>
    {
        public MemoryStream GenerateCsvFile(IList<T> lst)
        {
            var stream = new MemoryStream();
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                ShouldQuote = context => true
            };

            using (var writeFile = new StreamWriter(stream, leaveOpen: true))
            {

                var csv = new CsvWriter(writeFile, config);
                //   csv.Context.RegisterClassMap<SeconiqueShopifyProductMap>();
                csv.WriteRecords(lst);
                csv.Flush();
            }
            stream.Position = 0; //reset stream
            return stream;
            // return File(stream, "application/octet-stream", "Reports.csv");
        }
    }
}

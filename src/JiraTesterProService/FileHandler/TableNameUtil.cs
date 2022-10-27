namespace JiraTesterProService.FileHandler
{
    internal static class TableNameUtil
    {
        public static string GetDataTableName(ReconConfig reconConfig, string fileName, string comparisionSide = "")
        {
            string standardTableName;
            var tableName = fileName;
            int fileExtPos = fileName.LastIndexOf(".", StringComparison.Ordinal);
            if (fileExtPos >= 0)
                tableName = fileName.Substring(0, fileExtPos);

            standardTableName = !string.IsNullOrEmpty(comparisionSide) ? comparisionSide + "_" : "";

            if (reconConfig.ReconConfigId > -1)
            {
                standardTableName = standardTableName + reconConfig.ReconGroup + "_";
            }
            return standardTableName + tableName.StandardiseColumnTableName();
        }
    }


}
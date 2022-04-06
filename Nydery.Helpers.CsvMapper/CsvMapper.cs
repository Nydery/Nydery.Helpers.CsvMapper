
namespace Nydery.Helpers.CsvMapper
{
    public sealed class CsvMapper
    {
        public static string SeperatorValue { get; set; } = ";";

        public static IEnumerable<T> Map<T>(string headLine, IEnumerable<string> csvLines)
            where T : class, new()
        {
            IDictionary<string, int> columnIdx = CreateColumnIdxDictionary(headLine);
            return Map<T>(columnIdx, csvLines);
        }

        public static IEnumerable<T> Map<T>(IDictionary<string, int> columnIdx, IEnumerable<string> csvLines)
            where T : class, new()
        {
            var result = new List<T>();

            foreach (var csvLine in csvLines)
            {
                result.Add(Map<T>(columnIdx: columnIdx, csvLine: csvLine));
            }

            return result.AsEnumerable();
        }

        public static T Map<T>(string headLine, string csvLine)
            where T : class, new()
        {
            IDictionary<string, int> columnIdx = CreateColumnIdxDictionary(headLine);

            return Map<T>(columnIdx: columnIdx, csvLine: csvLine);
        }

        public static T Map<T>(IDictionary<string, int> columnIdx, string csvLine)
            where T : class, new()
        {
            T result = new();
            var csvData = csvLine.Split(SeperatorValue);
            var paramsOk = CheckParameters(columnIdx, csvData);

            if (!paramsOk)
            {
                //Parameters are not correct
                string errorMessage = $"Data and specification columns need to be the same amout";
                throw new Exception(errorMessage);
            }

            var typeProperties = typeof(T).GetProperties();
            typeProperties.Where(p => p.CanWrite).ToArray();

            //Map all csvColumns to type properties
            foreach (var col in columnIdx)
            {
                var prop = typeProperties.FirstOrDefault(p => p.Name == col.Key);
                if (prop == null)
                    continue;

                var propType = prop.PropertyType;
                var csvValue = csvData[col.Value];

                prop.SetValue(result, Convert.ChangeType(csvValue, propType));
            }

            return result;
        }

        private static bool CheckParameters(IDictionary<string, int> columnIdx, IEnumerable<string> csvData)
        {
            if (columnIdx == null || csvData == null)
                return false;

            if (columnIdx.Count == 0 || csvData.Count() == 0)
                return false;

            if (columnIdx.Count != csvData.Count())
                return false;

            return true;
        }

        private static IDictionary<string, int> CreateColumnIdxDictionary(string spec)
        {
            IDictionary<string, int> result = new Dictionary<string, int>();

            var columns = spec.Split(SeperatorValue);
            for (int i = 0; i < columns.Length; i++)
            {
                result.Add(new KeyValuePair<string, int>(columns[i], i));
            }

            return result;
        }
    }

}
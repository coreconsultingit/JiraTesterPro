using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection;
using System.Text;

namespace JiraTesterProService.ExcelHandler
{
    public class DataTableValueProvider<TEntity> : IValueProvider

    {

        private readonly DataTable source;

        public DataTableValueProvider(DataTable source)

        {

            this.source = source;

        }
        public IEnumerable<IDictionary<string, object>> GetValues()

        {

            var properties = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);



            var missingProperties = new List<string>();

            var foundProperties = new List<PropertyValueMetaData>();

            foreach (var p in properties)

            {

                bool found = false;

                var friendlyNameInfo = GetFriendlyName(p);



                for (int i = 0; i < source.Columns.Count; i++)

                {

                    var columnName = Encoding.UTF8.GetString(Encoding.ASCII.GetBytes(source.Columns[i].ColumnName)).Replace("?", "").Replace("\"", "");
                    if (p.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase) || friendlyNameInfo.FriendlyName.Equals(columnName, StringComparison.OrdinalIgnoreCase))

                    {

                        friendlyNameInfo.UseFriendlyName = p.Name != source.Columns[i].ColumnName;

                        found = true;

                        foundProperties.Add(friendlyNameInfo);

                        break;

                    }

                }

                if (found == false)

                {

                    missingProperties.Add(p.Name);

                }

            }



            foreach (DataRow row in source.Rows)

            {

                yield return GetDataForRow(row, foundProperties, missingProperties);

            }

        }



        private IDictionary<string, object> GetDataForRow(DataRow row, IEnumerable<PropertyValueMetaData> foundProperties, IEnumerable<string> missingProperties)

        {

            var values = new Dictionary<string, object>();

            foreach (var p in foundProperties)

            {

                string column = p.UseFriendlyName ? p.FriendlyName : p.PropertyName;

                object value = row.IsNull(column) ? null : row[column];

                values.Add(p.PropertyName, value);

            }



            foreach (var p in missingProperties)

            {

                values.Add(p, null);

            }

            return values;

        }



        private PropertyValueMetaData GetFriendlyName(PropertyInfo p)

        {

            var attributes = p.GetCustomAttributes(typeof(DisplayAttribute), false);

            DisplayAttribute attr = attributes.Length > 0 ? (DisplayAttribute)attributes[0] : null;

            var friendlyName = attr != null && !string.IsNullOrEmpty(attr.Name) ? attr.Name : p.Name;

            return new PropertyValueMetaData { PropertyName = p.Name, FriendlyName = friendlyName };

        }



        class PropertyValueMetaData

        {

            public string PropertyName { get; set; }

            public string FriendlyName { get; set; }

            public bool UseFriendlyName { get; set; }

        }

    }

}

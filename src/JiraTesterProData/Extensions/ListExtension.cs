using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace JiraTesterProData.Extensions;

public static class ListExtension
{
   
    public static IEnumerable<List<T>> SplitList<T>(List<T> locations, int nSize = 30)
        {
            for (int i = 0; i < locations.Count; i += nSize)
            {
                yield return locations.GetRange(i, Math.Min(nSize, locations.Count - i));
            }
        }
        public static IEnumerable<IEnumerable<T>> SplitIntoSets<T>
            (this IEnumerable<T> source, int itemsPerSet)
        {
            var sourceList = source as List<T> ?? source.ToList();
            for (var index = 0; index < sourceList.Count; index += itemsPerSet)
            {
                yield return sourceList.Skip(index).Take(itemsPerSet);
            }
        }
        public static bool ContainsWithIgnoreCase(this IList<string> lst, string val)
        {
            foreach (var item in lst)
            {


                if (item.EqualsWithIgnoreCase(val))
                {
                    return true;

                }
            }
            return false;
        }
        public static string ConvertToCorrectCase(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            // start by converting entire string to lower case
            var lowerCase = str.ToLower();
            // matches the first sentence of a string, as well as subsequent sentences
            var r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);
            // MatchEvaluator delegate defines replacement of setence starts to uppercase
            return r.Replace(lowerCase, s => s.Value.ToUpper());


        }
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name.Replace("\"", ""), Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name.Replace("\"", "")] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            table.TableName = typeof(T).Name;
            return table;
        }
        public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
        {
            if (list == null)
            {
                throw new ArgumentNullException("list");
            }

            if (collection == null)
            {
                throw new ArgumentNullException("collection");
            }

            List<T> list2 = list as List<T>;
            if (list2 != null)
            {
                list2.AddRange(collection);
                return;
            }

            foreach (T item in collection)
            {
                list.Add(item);
            }
        }
        public static int ConvertToInt(this string val)
        {
            int.TryParse(val, out int convertedInt);

            return convertedInt;
        }

        //public static DataTable ToPivotTable<T, TColumn, TRow, TData>(
        //    this IEnumerable<T> source,
        //    Func<T, TColumn> columnSelector,
        //    Expression<Func<T, TRow>> rowSelector,
        //    Func<IEnumerable<T>, TData> dataSelector)
        //{
        //    DataTable table = new DataTable();
        //    var rowName = ((MemberExpression)rowSelector.Body).Member.Name;
        //    table.Columns.Add(new DataColumn(rowName));
        //    var columns = source.Select(columnSelector).Distinct();

        //    foreach (var column in columns)
        //        table.Columns.Add(new DataColumn(column.ToString()));

        //    var rows = source.GroupBy(rowSelector.Compile())
        //        .Select(rowGroup => new
        //        {
        //            Key = rowGroup.Key,
        //            Values = columns.GroupJoin(
        //                rowGroup,
        //                c => c,
        //                r => columnSelector(r),
        //                (c, columnGroup) => dataSelector(columnGroup))
        //        });

        //    foreach (var row in rows)
        //    {
        //        var dataRow = table.NewRow();
        //        var items = row.Values.Cast<object>().ToList();
        //        items.Insert(0, row.Key);
        //        dataRow.ItemArray = items.ToArray();
        //        table.Rows.Add(dataRow);
        //    }

        //    return table;
        //}

        public static dynamic[] ToPivotArray<T, TColumn, TRow, TData>(
            this IEnumerable<T> source,
            Func<T, TColumn> columnSelector,
            Expression<Func<T, TRow>> rowSelector,
            Func<IEnumerable<T>, TData> dataSelector)
        {

            var arr = new List<object>();
            var cols = new List<string>();
            String rowName = ((MemberExpression)rowSelector.Body).Member.Name;
            var columns = source.Select(columnSelector).Distinct();

            cols = (new[] { rowName }).Concat(columns.Select(x => x.ToString())).ToList();


            var rows = source.GroupBy(rowSelector.Compile())
                .Select(rowGroup => new
                {
                    Key = rowGroup.Key,
                    Values = columns.GroupJoin(
                        rowGroup,
                        c => c,
                        r => columnSelector(r),
                        (c, columnGroup) => dataSelector(columnGroup))
                }).ToArray();


            foreach (var row in rows)
            {
                var items = row.Values.Cast<object>().ToList();
                items.Insert(0, row.Key);
                var obj = GetAnonymousObject(cols, items);
                arr.Add(obj);
            }
            return arr.ToArray();
        }
        private static dynamic GetAnonymousObject(IEnumerable<string> columns, IEnumerable<object> values)
        {
            IDictionary<string, object> eo = new ExpandoObject() as IDictionary<string, object>;
            int i;
            for (i = 0; i < columns.Count(); i++)
            {
                eo.Add(columns.ElementAt<string>(i), values.ElementAt<object>(i));
            }
            return eo;
        }

        public static bool IsListEqual<T>(this IList<T> lst1, IList<T> lst2)
        {
            if (lst1.Count != lst2.Count)
            {
                return false;
            }

            var set1 = new HashSet<T>(lst1);
            var set2 = new HashSet<T>(lst2);
            return set1.SetEquals(set2);
        }
        public static (bool isEqual,IList<string> sourceOnly, IList<string> destinationOnly) IsListEqual(this IList<string> lst1, IList<string> lst2)
        {
            if (lst1.Count != lst2.Count)
            {
                return (false, lst1.Except(lst2).ToList(),lst2.Except(lst1).ToList());
            }

            var set1 = new HashSet<string>(lst1);
            var set2 = new HashSet<string>(lst2);
            return (set1.SetEquals(set2), lst1.Except(lst2).ToList(), lst2.Except(lst1).ToList());
        }
        public static (bool isEqual, IList<string> missing) IsListEqualWithDestinationCheck(this IList<string> lst1, IList<string> lst2)
        {

            var set1 = new HashSet<string>(lst1);
            var set2 = new HashSet<string>(lst2);
            var anymissing = set2.Except(set1).ToList();

            return (!anymissing.Any(), anymissing);
        }

}

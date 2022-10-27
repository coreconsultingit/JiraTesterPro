using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace JiraTesterProService.ExcelHandler
{
    public class PropertyBinder : IPropertyBinder

    {

        public BindingResult<TEntity> Bind<TEntity>(TEntity entity, IDictionary<string, object> values)

        {

            var type = typeof(TEntity);

            var valid = true;

            var errorMessages = new List<string>();

            foreach (var v in values)

            {

                bool propertyValid = true;

                var property = type.GetProperty(v.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (property == null)

                    continue;

                var attributes = property.GetCustomAttributes(typeof(ValidationAttribute), true);

                foreach (var attr in attributes)

                {

                    var validationAttribute = (ValidationAttribute)attr;

                    if (!validationAttribute.IsValid(v.Value))

                    {

                        propertyValid = false;

                        errorMessages.Add(validationAttribute.FormatErrorMessage(v.Key));

                    }

                }

                if (propertyValid)

                {

                    object convertedValue;

                    var propertyType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;

                    try

                    {

                        if (v.Value != null && v.Value.GetType() == propertyType)

                        {

                            convertedValue = v.Value;

                        }

                        else

                        {

                            convertedValue = v.Value == null || v.Value.ToString().Trim().Length == 0

                                ? null

                                : Convert.ChangeType(v.Value, propertyType);

                        }

                    }

                    catch (Exception)

                    {

                        convertedValue = null;

                        bool parsed = propertyType.Name == "Boolean" && TryParseBool(v.Value, out convertedValue) || propertyType.IsEnum && TryParseEnum(v.Value, property, out convertedValue);



                        if (!parsed)

                        {

                            if (propertyType == typeof(decimal) || propertyType == typeof(double))

                            {

                                double convValue;

                                parsed = double.TryParse(v.Value.ToString(), out convValue);

                                if (parsed)

                                {

                                    convertedValue = Convert.ChangeType(convValue, propertyType);

                                }

                            }

                            if (propertyType == typeof(int) || propertyType == typeof(int) || propertyType == typeof(long))
                            {
                                decimal convValue;

                                parsed = decimal.TryParse(v.Value.ToString(), out convValue);

                                if (parsed)

                                {
                                    convertedValue = decimal.ToInt32(convValue);
                                }




                            }

                        }

                        if (!parsed)

                        {

                            valid = false;

                            errorMessages.Add(v.Key + " must be a valid " + propertyType.Name);

                            continue;

                        }

                    }

                    if (property.GetSetMethod() != null)

                        property.SetValue(entity, convertedValue, new object[0]);

                }

                else

                {

                    valid = false;

                }

            }

            var result = new BindingResult<TEntity>(entity, valid, errorMessages);

            return result;

        }



        private bool TryParseEnum(object value, PropertyInfo propertyInfo, out object parsedValue)

        {

            var method = typeof(PropertyBinder).GetMethod("ParseEnum", new[] { typeof(object) });

            var genericMethod = method.MakeGenericMethod(propertyInfo.PropertyType);

            parsedValue = genericMethod.Invoke(this, new[] { value });

            return parsedValue != null;

        }



        private bool TryParseBool(object value, out object parsedValue)

        {

            if (value != null)

            {

                var text = value.ToString().ToLower();

                if (text == "yes" || text == "y" || text == "1")

                {

                    parsedValue = true;

                    return true;

                }

                if (text == "no" || text == "n" || text == "0")

                {

                    parsedValue = false;

                    return true;

                }

            }



            parsedValue = null;

            return false;

        }







        public object ParseEnum<T>(object value) where T : struct

        {

            object parsedValue = null;

            if (value == null)

            {

                return null;

            }



            T result;

            if (!Enum.TryParse(value.ToString(), out result))

            {

                try

                {
                    //TODO: fix thisone

                    parsedValue = value.ToString();//EnumHelper<T>.StringToEnum(value.ToString());

                }

                catch (Exception)

                {

                    parsedValue = null;

                }

            }

            else

            {

                parsedValue = result;

            }



            return parsedValue;

        }

    }
}
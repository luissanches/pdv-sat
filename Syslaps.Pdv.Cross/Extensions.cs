using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Syslaps.Pdv.Cross
{
    public static class Extensions
    {
        public static string RecuperarChaveDoConfig(this string value)
        {
            return ConfigurationManager.AppSettings[value];
        }

        public static bool SimNaoToBool(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return value.Trim().ToLower() == "sim";
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static long? ToLong(this object value)
        {
            return value == null ? (long?)null : Convert.ToInt64(value);
        }

        public static decimal ToDecimal(this object value)
        {
            return Convert.ToDecimal(value);
        }

        public static ICollection<ValidationResult> TryValidateAnnotation(this object @value)
        {
            var validation = new Collection<ValidationResult>();
            var validationContext = new ValidationContext(@value, null, null);
            Validator.TryValidateObject(@value, validationContext, validation, true);
            return validation;
        }


        public static string RemoveBlankSpace(this String self)
        {
            return self.Replace(" ", String.Empty);
        }

        public static string ToComparableString(this string self)
        {
            return self.ToLower().RemoveBlankSpace().RemoveSpecialCharacters();
        }

        public static T DeepClone<T>(this T source) where T : class
        {
            using (Stream cloneStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();

                formatter.Serialize(cloneStream, source);
                cloneStream.Position = 0;
                T clone = (T)formatter.Deserialize(cloneStream);

                return clone;
            }
        }

        public static DateTime GetFirstDateTimeOfMonth(this DateTime value)
        {
            var primeiroDia = value.AddDays((value.Day - 1) * -1);
            return new DateTime(primeiroDia.Year, primeiroDia.Month, primeiroDia.Day, 0, 0, 0);
        }

        public static string GetMonthName(this DateTime value)
        {
            var cultureInfo = CultureInfo.GetCultureInfo("pt-BR");
            return cultureInfo.TextInfo.ToTitleCase(cultureInfo.DateTimeFormat.GetMonthName(value.Month));
        }

        public static DateTime GetLastDateTimeOfMonth(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, DateTime.DaysInMonth(value.Year, value.Month), 23, 59, 59);
        }

        public static DateTime GetLastDateTimeOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
        }

        public static DateTime GeFirstDateTimeOfDay(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
        }

        public static void Add(this List<KeyValuePair<string, string>> lista, string key, string value)
        {
            lista.Add(new KeyValuePair<string, string>(key, value));
        }

        public static bool PublicInstancePropertiesEqual<T>(this T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                Type type = self.GetType();
                List<string> ignoreList = new List<string>(ignore);
                foreach (PropertyInfo pi in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return self == to;
        }

        public static string RemoveSpecialCharacters(this string self)
        {
            var normalizedString = self;

            // Prepara a tabela de símbolos.
            var symbolTable = new Dictionary<char, char[]>();
            symbolTable.Add('a', new[] { 'à', 'á', 'ä', 'â', 'ã' });
            symbolTable.Add('A', new[] { 'Á', 'À', 'Ä', 'Â', 'Ã' });
            symbolTable.Add('c', new[] { 'ç' });
            symbolTable.Add('C', new[] { 'Ç' });
            symbolTable.Add('e', new[] { 'è', 'é', 'ë', 'ê' });
            symbolTable.Add('E', new[] { 'È', 'É', 'Ë', 'Ê' });
            symbolTable.Add('i', new[] { 'ì', 'í', 'ï', 'î' });
            symbolTable.Add('I', new[] { 'Ì', 'Í', 'Ï', 'Î' });
            symbolTable.Add('o', new[] { 'ò', 'ó', 'ö', 'ô', 'õ' });
            symbolTable.Add('O', new[] { 'Ò', 'Ó', 'Ö', 'Ô', 'Õ' });
            symbolTable.Add('u', new[] { 'ù', 'ú', 'ü', 'û' });
            symbolTable.Add('U', new[] { 'Ù', 'Ú', 'Ü', 'Û' });
            symbolTable.Add(Convert.ToChar(" "), new[] { '´', ';', '^', '~', '-', Convert.ToChar("'") });

            // Substitui os símbolos.
            foreach (var key in symbolTable.Keys)
            {
                foreach (var symbol in symbolTable[key])
                {
                    normalizedString = normalizedString.Replace(symbol, key);
                }
            }

            return normalizedString;
        }

        public static string LimparCaractersDocumento(this string str)
        {
            return str.Replace(".", "").Replace("/", "").Replace("-", "");
        }

        public static string ToMessage(this IList<string> lista)
        {
            var message = new StringBuilder();
            foreach (var item in lista)
            {
                message.AppendLine(item);
            }

            return message.ToString();
        }

        public static string NomePropriedade<T>(this object o, Expression<Func<T>> property)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException(" A Expressão Lambda deve ser uma propriedade valida");
            }
            var propertyName = propertyInfo.Name;

            return propertyName;
        }


        public static int ToInt(this string value)
        {
            return Convert.ToInt32(value);
        }
        public static int ToInt(this object value)
        {
            return Convert.ToInt32(value);
        }

        public static int ToInt(this bool value)
        {
            return Convert.ToInt32(value);
        }

        public static int ToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static long? ToLong(this string value)
        {
            return string.IsNullOrEmpty(value) ? (long?)null : Convert.ToInt64(value);
        }

        public static double ToDouble(this string value)
        {
            return Convert.ToDouble(value);
        }

        public static decimal ToDecimal(this string value)
        {
            if (string.IsNullOrEmpty(value)) return 0;
            return decimal.Parse(value, NumberStyles.Any, CultureInfo.GetCultureInfo("pt-BR"));
        }

        public static bool IsNumeric(this string s)
        {
            float output;
            return float.TryParse(s, out output);
        }

        public static DateTime ToDateTime(this string value)
        {
            return Convert.ToDateTime(value);
        }

        public static bool ToBool(this string value)
        {
            return Convert.ToBoolean(value);
        }

        public static decimal ToDecimal(this double value)
        {
            return Convert.ToDecimal(value);
        }
        public static decimal ToPositive(this decimal value)
        {
            return value * -1;
        }


        public static string RemoveLetters(this string value)
        {
            String textWithoutLetters = String.Empty;
            foreach (var character in value.ToCharArray())
                if (!Char.IsLetter(character))
                    textWithoutLetters += character;
            return textWithoutLetters;
        }

        public static int Eval(this string value)
        {
            return Int32.Parse(Regex.Match(value, @"-?\d+").Value);
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }

        public static T ToEnum<T>(this int value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.ToObject(typeof(T), value);
        }

        public static int EnumToInt(this Enum value)
        {
            return Convert.ToInt32(value);
        }

        public static T ToConvertType<T>(this object value, CultureInfo cultureInfo = null)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString()))
            {
                return default(T);
            }

            if (cultureInfo == null)
            {

                cultureInfo = new CultureInfo("pt-BR");
            }

            return (T)Convert.ChangeType(value, typeof(T), cultureInfo);
        }

        public static string GetHash(this string value)
        {

            var buffer = Encoding.UTF8.GetBytes(value);
            var hash = MD5.Create().ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            foreach (var bit in hash)
            {
                sb.Append(bit.ToString("x2"));
            }

            return sb.ToString();
        }

        public static bool CompareHash(this string value, string openedValue)
        {
            return value.GetHashCode().Equals(openedValue.GetHashCode());
        }

        public static string GetConfigValue(this string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static DataSet ToDataSet<T>(this IList<T> list, string tableName = "Table1")
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable(tableName);
            ds.Tables.Add(t);

            foreach (var propInfo in elementType.GetProperties())
            {
                if (propInfo.PropertyType.IsGenericType && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {

                    t.Columns.Add(propInfo.Name, Nullable.GetUnderlyingType(propInfo.PropertyType));
                }
                else
                {
                    t.Columns.Add(propInfo.Name, propInfo.PropertyType);
                }
            }

            foreach (T item in list)
            {
                DataRow row = t.NewRow();
                foreach (var propInfo in elementType.GetProperties())
                {
                    var value = propInfo.GetValue(item, null);
                    row[propInfo.Name] = value ?? DBNull.Value;
                }

                t.Rows.Add(row);
            }

            return ds;
        }

        public static string GetDescription(this Enum enumerator)
        {
            var type = enumerator.GetType();
            var memberInfo = type.GetMember(enumerator.ToString());

            if (memberInfo.Length <= 0) return enumerator.ToString();
            var attributes = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            return attributes.Length > 0 ? ((DescriptionAttribute)attributes[0]).Description : enumerator.ToString();
        }

        public static T ToEnumFromDescription<T>(this string description)
        {
            var memberInfos = typeof(T).GetMembers();
            if (memberInfos.Length > 0)
            {
                foreach (var memberInfo in memberInfos)
                {
                    var attributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                    if (attributes.Length > 0)
                        if (((DescriptionAttribute)attributes[0]).Description == description)
                            return (T)Enum.Parse(typeof(T), memberInfo.Name);
                }
            }

            return default(T);
        }

        public static string ToDescriptionEnumFromValue<T>(this string value)
        {
            var memberInfos = typeof(T).GetMembers();

            if (memberInfos.Length > 0)
            {
                foreach (var memberInfo in memberInfos)
                {
                    var attributesValue = memberInfo.GetCustomAttributes(typeof(DefaultValueAttribute), false);

                    var attributesDescription = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);

                    if (attributesValue.Length > 0)
                    {
                        if (((DefaultValueAttribute)attributesValue[0]).Value.ToString() == value)
                        {
                            return ((DescriptionAttribute)attributesDescription[0]).Description;
                        }
                    }
                }
            }

            return string.Empty;
        }

        public static T ToEnumFromStringValue<T>(this string value)
        {
            var parse = Enum.Parse(typeof(T), value);
            var result = (T)parse;
            return result;
        }

        public static bool IsNull(this object objectToValidate)
        {
            return objectToValidate == null;
        }

        public static List<ValidationResult> Validate(this object objectToValidate)
        {
            var results = new List<ValidationResult>();

            if (!objectToValidate.IsNull())
            {
                var context = new ValidationContext(objectToValidate, null, null);
                Validator.TryValidateObject(objectToValidate, context, results, true);
            }

            return results;
        }

        public static string CreateXmlElement(this string value, string element, string attrib = "")
        {
            return value.IsNullOrEmpty() ? string.Empty : $"<{element}{attrib}>{value}</{element}>";
        }

        public static bool IsCpfOrCnpj(this string value)
        {
            return value.IsCpf() || value.IsCnpj();
        }

        public static bool IsCpf(this string cpf)
        {
            int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempCpf;
            string digito;
            int soma;
            int resto;
            cpf = cpf.Trim();
            cpf = cpf.Replace(".", "").Replace("-", "");
            if (cpf.Length != 11)
                return false;
            tempCpf = cpf.Substring(0, 9);
            soma = 0;

            for (int i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCpf = tempCpf + digito;
            soma = 0;
            for (int i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];
            resto = soma % 11;
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cpf.EndsWith(digito);
        }

        public static bool IsCnpj(this string cnpj)
        {
            int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma;
            int resto;
            string digito;
            string tempCnpj;
            cnpj = cnpj.Trim();
            cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
            if (cnpj.Length != 14)
                return false;
            tempCnpj = cnpj.Substring(0, 12);
            soma = 0;
            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = resto.ToString();
            tempCnpj = tempCnpj + digito;
            soma = 0;
            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];
            resto = (soma % 11);
            if (resto < 2)
                resto = 0;
            else
                resto = 11 - resto;
            digito = digito + resto.ToString();
            return cnpj.EndsWith(digito);
        }
    }
}
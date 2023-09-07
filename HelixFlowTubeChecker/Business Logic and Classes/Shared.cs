using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace OncoSeek.Core
{
    public static class Shared
    {

        public static string GetSHAHash(string Data)
        {
            SHA512Managed HashTool = new SHA512Managed();
            Byte[] PhraseAsByte = System.Text.Encoding.UTF8.GetBytes(string.Concat(Data));
            Byte[] EncryptedBytes = HashTool.ComputeHash(PhraseAsByte);
            HashTool.Clear();
            return Convert.ToBase64String(EncryptedBytes);
        }


        public static string GetMD5HashFromFile(string filename)
        {

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    string raw = BitConverter.ToString(md5.ComputeHash(stream));
                    return raw.Replace("-", "").ToLower();
                }
            }

        }


        public static DataTable GetTableFromIList<T>(IList<T> list)
        {

            var table = new DataTable();
            var props = TypeDescriptor.GetProperties(typeof(T))
                                         .Cast<PropertyDescriptor>()
                                       .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                       .ToArray();
            foreach (var propertyInfo in props)
                table.Columns.Add(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType);
            var values = new object[props.Length];
            foreach (var item in list)
            {
                for (var i = 0; i < values.Length; i++)
                    values[i] = props[i].GetValue(item);
                table.Rows.Add(values);
            }


            return table;
        }


        public static DataTable ImportExceltoDatatable(string filePath)
        {

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var result = reader.AsDataSet(new ExcelDataSetConfiguration() { ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration() { UseHeaderRow = true } });
                return result.Tables[0];
            }


        }


    }
}

using System;
using System.Data;
using System.Reflection;
using System.Web;
using System.Collections.Generic;
using System.Security;
using System.Linq;

namespace FrameWork.Platform
{
    public class Utility
	{
		public static string AppendFolderChar(string psPath)
		{
			int ln_Length = psPath.Length;
			string ls_NewPath = psPath;
			string ls_LastChar = psPath.Substring(ln_Length - 1);
			if (ls_LastChar != "\\" && ls_LastChar != "/") ls_NewPath += "\\";
			return ls_NewPath;
		}

		public static string Escape(string inString)
		{ return HttpUtility.UrlDecode(inString); }

        public static SecureString ToSecureString(string Source)
        {
            if (string.IsNullOrWhiteSpace(Source))
                return null;
            else
            {
                SecureString Result = new SecureString();
                foreach (char c in Source.ToCharArray())
                    Result.AppendChar(c);
                return Result;
            }
        }
        /// <summary>
        /// 取得目前網頁名稱(已去掉.aspx,等同FunctionID)
        /// </summary>
        /// <returns></returns>
        //public static string GetPageID()
        //{
        //    string FileName = VirtualPathUtility.GetFileName(HttpContext.Current.Request.FilePath);
        //    string Extension = VirtualPathUtility.GetExtension(HttpContext.Current.Request.FilePath);
        //    string PageID = FileName.Replace(Extension, "");
        //    return PageID;
        //}

        public static List<T> ConvertTo<T>(DataTable datatable) where T : new()
        {
            List<T> Temp = new List<T>();
            try
            {
                List<string> columnsNames = new List<string>();
                foreach (DataColumn DataColumn in datatable.Columns)
                    columnsNames.Add(DataColumn.ColumnName);
                Temp = datatable.AsEnumerable().ToList().ConvertAll<T>(row => getObject<T>(row, columnsNames));
                return Temp;
            }
            catch
            {
                return Temp;
            }

        }
        private static T getObject<T>(DataRow row, List<string> columnsName) where T : new()
        {
            T obj = new T();
            try
            {
                string columnname = "";
                string value = "";
                PropertyInfo[] Properties;
                Properties = typeof(T).GetProperties();
                foreach (PropertyInfo objProperty in Properties)
                {
                    columnname = columnsName.Find(name => name.ToLower() == objProperty.Name.ToLower());
                    if (!string.IsNullOrEmpty(columnname))
                    {
                        value = row[columnname].ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            if (Nullable.GetUnderlyingType(objProperty.PropertyType) != null)
                            {
                                value = row[columnname].ToString().Replace("$", "").Replace(",", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(Nullable.GetUnderlyingType(objProperty.PropertyType).ToString())), null);
                            }
                            else
                            {
                                value = row[columnname].ToString().Replace("%", "");
                                objProperty.SetValue(obj, Convert.ChangeType(value, Type.GetType(objProperty.PropertyType.ToString())), null);
                            }
                        }
                    }
                }
                return obj;
            }
            catch
            {
                return obj;
            }
        }
    }
}
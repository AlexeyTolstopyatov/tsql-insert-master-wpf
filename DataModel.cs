using System.Collections.Generic;


namespace sqx
{
    public class DataModel
    {
        public static IEnumerable<IEnumerable<string>> Table;
        public static string Query;
        public static string File;
        public static string Name = "table_name";
        public static string Delimeter = ",";
        public static bool isSure = false;
        public static string OpenedFile;
        
        public static bool IsEnumEnabled;
        public static bool IsCommentEnabled;
    }
}

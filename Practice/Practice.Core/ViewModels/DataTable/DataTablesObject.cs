namespace Practice.Core.ViewModels
{
    using System.Collections.Generic;

    public class DataTablesObject<T>
    {
        public DataTablesObject()
        {
            iTotalRecords = 0;
            iTotalDisplayRecords = 0;
            aaData = new List<T>();
        }

        public List<T> aaData { get; set; }

        public int iTotalDisplayRecords { get; set; }

        public int iTotalRecords { get; set; }
    }
}

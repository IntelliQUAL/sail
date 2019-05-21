using System;
using System.Text;
using System.Collections.Generic;

namespace SAIL.ViewModel
{
    /// <summary>
    /// Created:        05/16/2017
    /// Author:         David J. McKee
    /// Purpose:        Common response for Search Requests.
    /// </summary>
    public class SearchResponse<T>
    {
        public List<T> EntityList = new List<T>();

        public int Page = 0;
        public int RowsPerPage = 0;
        public int TotalPages = 0;
        public int TotalRows = 0;

        public void Add(T entity)
        {
            EntityList.Add(entity);
        }
    }
}

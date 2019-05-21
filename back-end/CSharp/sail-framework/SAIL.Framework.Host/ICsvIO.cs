using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    public interface ICsvIO
    {
        // Ordinal Position
        //
        // string[] csvLineOrdinal = new string[3];
        //
        // csvLineOrdinal[0] = "Col Value 0";
        // csvLineOrdinal[1] = "Col Value 1";
        // csvLineOrdinal[2] = "Col Value 2";

        // Name / Value
        //
        // string[] columnPositionMap = new string[3];
        //
        // columnPositionMap[0] = "Column Name 0";
        // columnPositionMap[1] = "Column Name 1";
        // columnPositionMap[2] = "Column Name 2";
        //
        // Hashtable csvLineNameValue = new Hashtable();
        //
        // csvLineNameValue["Column Name 0"] = "Col Value 0";
        // csvLineNameValue["Column Name 1"] = "Col Value 1";
        // csvLineNameValue["Column Name 2"] = "Col Value 2";

        // Row Level Operations.

        // Create
        void CreateCsvRowOrdinal(System.IO.StreamWriter csvStreamWriter, string[] csvLineOrdinal);
        void CreateCsvRow(System.IO.StreamWriter csvStreamWriter, string[] columnPositionMap, Dictionary<string, string> csvLineNameValue);

        // Search Criteria
        // 
        // List<KeyValuePair<string, string>> searchCriteria = new List<KeyValuePair<string, string>>();
        //
        // searchCriteria.Add(new KeyValuePair<string,string>("Column Name 0", "Col Value 0"));
        // searchCriteria.Add(new KeyValuePair<string,string>("Column Name 1", "Col Value 1"));
        // searchCriteria.Add(new KeyValuePair<string,string>("Column Name 2", "Col Value 2"));

        // Read
        Dictionary<string, string> ReadCsvLine(string filePathname, List<KeyValuePair<string, string>> searchCriteria);
        Dictionary<string, string> ReadCsvLineStream(System.IO.StreamReader reader, string[] columnPositionMap);
        Dictionary<string, string> ParseCsvLineSingle(string singleLine, string[] columnPositionMap);

        // Update
        int UpdateCsvRowOrdinal(string filePathname, List<KeyValuePair<string, string>> searchCriteria, string[] csvLineOrdinal);
        int UpdateCsvRow(string filePathname, List<KeyValuePair<string, string>> searchCriteria, string[] columnPositionMap, Dictionary<string, string> csvLineNameValue);

        // Delete
        int DeleteCsvRow(string filePathname, List<KeyValuePair<string, string>> searchCriteria);

        // Search
        List<Dictionary<string, string>> SearchCsvLineList(string filePathname, List<KeyValuePair<string, string>> searchCriteria, out string[] headerRow);
        List<Dictionary<string, string>> SearchCsvLineListPaged(string filePathname,
                                                            int page,
                                                            int rowsPerPage,
                                                            List<KeyValuePair<string, string>> searchCriteria,
                                                            out string[] headerRow,
                                                            out int totalRows,
                                                            out int totalPages);

        // Supporting Operations
        bool IsMatch(string[] csvLine, List<KeyValuePair<string, string>> searchCriteria, string[] columnPositionMap, out Dictionary<string, string> csvLineNameValue);
        string[] ParseCsvLine(string singleLine);

        // Used when the line may contain more than one physical row.
        //string[] ParseStreamReaderCsvLine(StreamReader streamReader, string singleLine);
        Dictionary<string, string> LineOrdinalToLineNameValue(string[] csvLineOrdinal, string[] columnPositionMap);
        string[] ReadColumnPositionMap(string filePathname);

        /// <summary>
        /// Created:        03/03/2014
        /// Author:         David J. McKee
        /// Purpose:        Creates the row headers for the given file.
        /// Important:
        ///                 Each column must be stored within the hashtable by its value.
        ///                 rowData["First Column Value"] = 0;
        ///                 rowData["Second Column Value"] = 1 ;
        ///                 rowData["Last Column Value"] = n;
        /// </summary>        
        string BuildCsvLineByOrdinalPosition(Hashtable rowData);

        /// <summary>
        /// Created:        03/03/2014
        /// Author:         David J. McKee
        /// Purpose:        Creates the row headers for the given file.
        /// Important:
        ///                 Each column must be stored within the hashtable by its ordinal position.
        ///                 rowData[0] = "First Column Value";
        ///                 rowData[1] = "Second Column Value";
        ///                 rowData[n] = "Last Column Value";
        /// </summary>        
        string BuildCsvLine(Hashtable rowData);
        string BuildCsvLine(Dictionary<string, string> rowData);

        string BuildCsvLineParams(params string[] cols);

        string ReadTextValue(Dictionary<string, string> offerDictionary, string columnName);
        bool ReadBoolValue(Dictionary<string, string> offerDictionary, string columnName, bool defaultValue);
        int ReadIntValue(Dictionary<string, string> offerDictionary, string columnName, int defaultValue);
    }
}

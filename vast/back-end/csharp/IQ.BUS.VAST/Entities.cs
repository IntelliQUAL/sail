using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.Entities.VastDB;
using IQ.BUS.Vast.Helpers;
using IQ.Entities.VastDB.Const;
using IQ.BUS.Vast.Common.Helpers;

namespace IQ.BUS.Vast
{
    /// <summary>
    /// Created:        02/26/2015
    /// Author:         David J. McKee
    /// Purpose:
    /// Important:      All QueryStrings will contain 'AuthToken', 'InstanceGUID', 'DatabaseName', and 'TableID'
    /// Examples:
    ///                 XML Request:    
    ///                 XML Response:   
    ///                 JSON Request:   
    ///                     new                     http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/new/                                                      
    ///                     Create                  http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/?cors-method=POST&e=ExampleRequest            
    ///                     Read                    http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/87eaeee8-d641-4c05-800c-790b4561c286/                     // Read based on Primary Key
    ///                     Update                  http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/?cors-method=PUT&e=ExampleRequest
    ///                     Delete                  http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/87eaeee8-d641-4c05-800c-790b4561c286/?cors-method=DELETE  // This is the request example.                    
    ///                     Search All              http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/abc/                                                      // N/A no example needed
    ///                     Search w/ Criterial     http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/?e=ExampleRequest                                         // Search Criteria on QueryString as json
    /// 
    ///                 JSON Response:  
    ///                     new                     http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/new/                                                      
    ///                     Create                  http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/?cors-method=POST&e=ExampleResponse
    ///                     Read                    http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/87eaeee8-d641-4c05-800c-790b4561c286/?e=ExampleResponse   // Read based on Primary Key
    ///                     Update                  http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/?cors-method=PUT&e=ExampleResponse
    ///                     Delete                  http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/87eaeee8-d641-4c05-800c-790b4561c286/?cors-method=DELETE&e=ExampleResponse
    ///                     Search All and Search   http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.Entities/?e=ExampleResponse
    ///                 SOAP Request:   
    ///                 SOAP Response:  
    ///                 SOAP WSDL:      
    ///                 XSD:            
    ///                 
    /// Error Trace:                                http://localhost/PIPE.WebApiHost/v7/html/PIPE.BusinessProcesses.Tracing.TraceQueueDump/Error
    /// </summary>
    /// 
    public class Entities : IQ.BUS.Vast.BaseClasses.REST
    {
        public override ViewModel.Vast.Entity ExampleCreateRequest
        {
            get
            {
                return new ViewModel.Vast.Entity();
            }
        }

        public override ViewModel.Vast.Entity ExampleCreateResponse
        {
            get
            {
                return new ViewModel.Vast.Entity();
            }
        }

        public override ViewModel.Vast.Entity ExampleDeleteResponse
        {
            get
            {
                return new ViewModel.Vast.Entity();
            }
        }

        public override ViewModel.Vast.Entity ExampleReadResponse
        {
            get
            {
                return new ViewModel.Vast.Entity();
            }
        }

        public override ViewModel.Vast.Grid ExampleSearchAllResponse
        {
            get
            {
                return new ViewModel.Vast.Grid();
            }
        }

        public override ViewModel.Vast.EntitySearch ExampleSearchRequest
        {
            get
            {
                return new ViewModel.Vast.EntitySearch();
            }
        }

        public override ViewModel.Vast.Entity ExampleUpdateRequest
        {
            get
            {
                return new ViewModel.Vast.Entity();
            }
        }

        public override ViewModel.Vast.Entity ExampleUpdateResponse
        {
            get
            {
                return new ViewModel.Vast.Entity();
            }
        }
    }
}

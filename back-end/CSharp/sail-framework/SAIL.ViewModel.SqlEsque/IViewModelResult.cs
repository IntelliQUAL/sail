using System;
namespace SAIL.ViewModel
{
    /// <summary>
    /// Created:        10/01/2014
    /// Author:         David J. McKee
    /// Purpose:        Provides access to common results for all API responses.
    /// 10/25/2014 - DJM:   Updates based on framework meetings.
    /// </summary>
    public interface IViewModelResult
    {
        bool Success { get; set; }          // success or failure
        string ErrorCode { get; set; }      // 401
        string ErrorText { get; set; }      // Authentication Required
        string UserMessage { get; set; }    // Your error has been logged. Please include error code 'ABC23-434234-ASGSG' when reporting this error.
    }
}

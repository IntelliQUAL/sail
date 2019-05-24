using System;
using System.Collections.Generic;
using System.Text;

namespace SAIL.Framework.Host
{
    // Equiv to HttpPostedFile
    public interface IFile
    {
        void SaveAs(string filename);
        byte[] Contents { get; }

        System.IO.Stream Stream { get; }

        string Extension { get; }   // The extension of the specified path (including the period ".". Examples: .pdf, .jpeg, .png

        string FormFieldName { get; }   // for example 'thumbImage' where <input type="file" name="thumbImage" />
    }
}

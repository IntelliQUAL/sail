using System;
using System.Collections.Generic;
using System.Text;

namespace SAIL.Framework.Host
{
    public interface IFileIO
    {
        string ReadTextFileContents(IContext context, string sourceFilePathname);
        void WriteTextFileContents(IContext context, string destinationFilePathname, string contents);

        byte[] ReadBinaryFileContents(IContext context, string sourceFilePathname);
        void WriteBinaryFileContents(IContext context, string destinationFilePathname, byte[] contents);

        void SafeDelete(IContext context, string filename);
        void Move(IContext context, string sourceFileName, string destPath);

        void WriteBinaryFileSteam(IContext context, string destinationFilePathname, System.IO.Stream contents);
        System.IO.Stream ReadBinaryFileSteam(IContext context, string sourceFilePathname);
        string FileNameOnly(IContext context, string filePathname);                     // Retruns the filename without the path
        string FileNameOnlyWithoutExtension(IContext context, string filePathname);     // Retruns the filename without the path or Extension
        string FilePathOnly(IContext context, string filePathname);                     // Returns the path without the filename

        bool FileExists(IContext context, string filePathname);
        long FileLength(IContext context, string filePathname);       // File Length in bytes
    }
}

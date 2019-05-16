using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Host.API.Helpers
{
    internal class FileIO : IFileIO
    {
        bool IFileIO.FileExists(IContext context, string filePathname)
        {
            return System.IO.File.Exists(filePathname);
        }

        long IFileIO.FileLength(IContext context, string filePathname)
        {
            throw new NotImplementedException();
        }

        string IFileIO.FileNameOnly(IContext context, string filePathname)
        {
            return new System.IO.FileInfo(filePathname).Name;
        }

        string IFileIO.FileNameOnlyWithoutExtension(IContext context, string filePathname)
        {
            return System.IO.Path.GetFileNameWithoutExtension(filePathname);
        }

        string IFileIO.FilePathOnly(IContext context, string filePathname)
        {
            throw new NotImplementedException();
        }

        void IFileIO.Move(IContext context, string sourceFileName, string destPath)
        {
            throw new NotImplementedException();
        }

        byte[] IFileIO.ReadBinaryFileContents(IContext context, string sourceFilePathname)
        {
            throw new NotImplementedException();
        }

        Stream IFileIO.ReadBinaryFileSteam(IContext context, string sourceFilePathname)
        {
            throw new NotImplementedException();
        }

        string IFileIO.ReadTextFileContents(IContext context, string sourceFilePathname)
        {
            string contents = string.Empty;

            try
            {
                if (System.IO.File.Exists(sourceFilePathname))
                {
                    // Read the entire text file into a string.
                    using (System.IO.StreamReader streamReader = new System.IO.StreamReader(sourceFilePathname))
                    {
                        contents = streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                }
            }
            catch { }

            return contents;
        }

        void IFileIO.SafeDelete(IContext context, string filename)
        {
            throw new NotImplementedException();
        }

        void IFileIO.WriteBinaryFileContents(IContext context, string destinationFilePathname, byte[] contents)
        {
            throw new NotImplementedException();
        }

        void IFileIO.WriteBinaryFileSteam(IContext context, string destinationFilePathname, Stream contents)
        {
            throw new NotImplementedException();
        }

        void IFileIO.WriteTextFileContents(IContext context, string destinationFilePathname, string contents)
        {
            if (System.IO.File.Exists(destinationFilePathname))
            {
                System.IO.File.Delete(destinationFilePathname);
            }

            using (System.IO.StreamWriter sw = new System.IO.StreamWriter(destinationFilePathname, false))
            {
                sw.Write(contents);
                sw.Flush();
                sw.Close();
            }
        }
    }
}
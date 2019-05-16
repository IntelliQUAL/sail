using System;
using System.Collections.Generic;
using System.Text;

namespace SAIL.Framework.Host
{
    public interface IPathIO
    {
        void EnsurePath(IContext context, string path);
        void DeleteOldFiles(IContext context, string path, int daysToKeep);
        void DeleteOldChildPaths(IContext context, string basePath, int daysToKeep);                  // Assumes each child page name can be converted into a date.
        List<string> GetDirectoryNames(IContext context, string basePath);                            // Returns a list of sub-directory names only, NOT the fully qualified path.        

        // List all files in a given path sorted by date created.
        // Each filename is fully qualified
        List<string> GetFilesSortedByCreateDate(IContext context, string path, bool sortAscending);

        // fileNameStringReplaceList and pathStringReplaceList to replace the names of files and paths respectively
        void SynchFolders(IContext context, string sourcePath,
                            string targetPath,
                            Dictionary<string, string> fileNameStringReplaceList,
                            Dictionary<string, string> pathStringReplaceList,
                            IFileIO sourceFileIo,
                            IFileIO targetFileIo);
        string Combine(IContext context, string path1, string path2);
        string PathNameOnly(IContext context, string fullPathName);
        bool PathExists(IContext context, string fullPathName);
        void CreatePath(IContext context, string fullPathName);
    }
}

using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Host.API.Helpers
{
    public class PathIO : IPathIO
    {
        string IPathIO.Combine(IContext context, string path1, string path2)
        {
            return System.IO.Path.Combine(path1, path2);
        }

        void IPathIO.CreatePath(IContext context, string fullPathName)
        {
            throw new NotImplementedException();
        }

        void IPathIO.DeleteOldChildPaths(IContext context, string basePath, int daysToKeep)
        {
            try
            {
                DateTime maxDate = DateTime.Now.AddDays(-(daysToKeep));

                System.IO.DirectoryInfo di = new DirectoryInfo(basePath);

                foreach (System.IO.DirectoryInfo childPath in di.GetDirectories())
                {
                    try
                    {
                        DateTime pathDate;
                        if (DateTime.TryParse(childPath.Name, out pathDate))
                        {
                            if (maxDate > pathDate)
                            {
                                childPath.Delete(true);
                            }
                        }
                        else
                        {
                            // The directory is not formatted as a date.
                            DirectoryInfo childDirectory = new DirectoryInfo(childPath.FullName);

                            if (maxDate > childDirectory.LastWriteTime)
                            {
                                childPath.Delete(true);
                            }
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        void IPathIO.DeleteOldFiles(IContext context, string path, int daysToKeep)
        {
            throw new NotImplementedException();
        }

        private static bool IsAFileName(string pathFragment)
        {
            string last5 = pathFragment.Substring(pathFragment.Length - Math.Min(5, pathFragment.Length));
            bool suspectedFilename = (last5.IndexOf('.') > -1);
            return suspectedFilename;
        }

        void IPathIO.EnsurePath(IContext context, string folderPath)
        {
            if (Directory.Exists(folderPath) == false)
            {
                //setup
                string[] splitByColon = folderPath.Split(':');
                string letter = splitByColon[0];
                string folders = splitByColon[1];
                string[] pathParts = folders.Split('\\');
                string pathToEnsure = letter + @":";

                //strip the file name if present
                if (IsAFileName(pathParts[pathParts.Length - 1]))
                {
                    pathParts[pathParts.Length - 1] = string.Empty;
                }

                //build a clean path skipping double slashes
                foreach (string part in pathParts)
                {
                    if (part.Length > 0)
                    {
                        pathToEnsure += @"\" + part;
                    }
                }

                //build the path
                Directory.CreateDirectory(pathToEnsure);

                //should exist
                bool success = Directory.Exists(pathToEnsure);
                if (!success)
                {
                    throw new SystemException("Unable to create " + pathToEnsure + ".");
                }
            }
        }

        List<string> IPathIO.GetDirectoryNames(IContext context, string basePath)
        {
            throw new NotImplementedException();
        }

        List<string> IPathIO.GetFilesSortedByCreateDate(IContext context, string path, bool sortAscending)
        {
            List<string> filePathnameList = new List<string>();

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);

                List<FileInfo> fileInfoList = directoryInfo.GetFiles("*.*", SearchOption.TopDirectoryOnly).OrderBy(t => t.CreationTime).ToList();

                foreach (FileInfo fi in fileInfoList)
                {
                    filePathnameList.Add(fi.FullName);
                }
            }
            catch { }

            return filePathnameList;
        }

        bool IPathIO.PathExists(IContext context, string fullPathName)
        {
            throw new NotImplementedException();
        }

        string IPathIO.PathNameOnly(IContext context, string fullPathName)
        {
            throw new NotImplementedException();
        }

        void IPathIO.SynchFolders(IContext context, string sourcePath, string targetPath, Dictionary<string, string> fileNameStringReplaceList, Dictionary<string, string> pathStringReplaceList, IFileIO sourceFileIo, IFileIO targetFileIo)
        {
            throw new NotImplementedException();
        }
    }
}
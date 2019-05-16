using System;
using System.Web;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Host.API.Helpers
{
    public class LateBinding
    {
        private static Dictionary<string, object> _commandTypeRef = new Dictionary<string, object>();   // Cache of names to types
        private static string _standardFilePath = string.Empty;                                         // file path for loading assemblies
        private static object _loadClassSyncLock = new object();                                        // concurrency lock for loading a new class type
        private static object _commandTypeRefSyncLock = new object();
        private static string[] _businessProcessPathList = null;

        #region Public Methods

        /// <summary>
        /// Created:        05/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Loads a given class based on its' namespace and class
        /// </summary>        
        public static object LoadViaFullName(IContext context, string namespacePlusClassName)
        {
            StringBuilder issueList = new StringBuilder();
            return LoadViaFullName(context, namespacePlusClassName, issueList, null);
        }

        /// <summary>
        /// Created:        05/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Loads a given class based on its' namespace and class
        /// </summary>        
        public static object LoadViaFullName(IContext context, string namespacePlusClassName, StringBuilder issueList)
        {
            return LoadViaFullName(context, namespacePlusClassName, issueList, null);
        }

        /// <summary>
        /// Created:        05/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Loads a given class based on its' namespace and class with params
        /// </summary>        
        public static object LoadViaFullName(IContext context, string namespacePlusClassName, StringBuilder issueList, params object[] args)
        {
            object classInstance = null;

            try
            {
                Type classType = ReadTypeFromCache(context, namespacePlusClassName);

                if (classType != null)
                {
                    classInstance = Activator.CreateInstance(classType, args);
                }
                else
                {
                    // Not already in memory?  Hold other threads off until we load.
                    lock (_loadClassSyncLock)
                    {
                        // Check again for class type
                        classType = ReadTypeFromCache(context, namespacePlusClassName);

                        if (classType == null)
                        {
                            IFileIO fileIo = new FileIO();
                            IConfiguration configuration = new HostConfiguration();

                            // Load the class from the file system.
                            classInstance = LoadClassFromFileSystem(context, namespacePlusClassName, issueList, args);

                            // Cache the type
                            WriteTypeToCache(context, namespacePlusClassName, classInstance);
                        }
                        else
                        {
                            // Load the object from a known type.
                            classInstance = Activator.CreateInstance(classType, args);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex, issueList.ToString());
            }

            return classInstance;
        }

        public static string ExecutingAssemblyPath()
        {
            string executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring("file:///".Length);

            string defaultPath = new System.IO.FileInfo(executingAssembly).DirectoryName;
            return defaultPath;
        }

        /// <summary>
        /// Created:        09/17/2014
        /// Author:         David J. McKee
        /// Purpose:        Reads the Business Process Path list from the hostconfig.xml file      
        /// </summary>        
        public static string[] ReadBusinessProcessPathList(IContext context)
        {
            if (_businessProcessPathList == null)
            {
                const string BUSINESS_PROCESS_PATH_LIST = "ServicePathList";

                string defaultPath = ExecutingAssemblyPath();

                IConfiguration hostConfiguration = context.Get<IConfiguration>();

                // This contains a pipe delimited list of file system locations.
                const string PATH_SEP = "|";
                _businessProcessPathList = hostConfiguration.ReadSetting(BUSINESS_PROCESS_PATH_LIST, defaultPath).Split(new string[] { PATH_SEP }, StringSplitOptions.RemoveEmptyEntries);
            }

            return _businessProcessPathList;
        }

        /// <summary>
        /// Created:        05/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Loads a class from the file system.      
        /// </summary>        
        private static object LoadClassFromFileSystem(IContext context, string namespacePlusClassName, StringBuilder issueList, object[] args)
        {
            object classInstance = null;

            try
            {
                // Assume the command contains the assembly and class, and ignore the version.
                namespacePlusClassName = namespacePlusClassName.Replace("_", ".");
                string[] parts = namespacePlusClassName.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);

                List<System.IO.FileInfo> dllFileInfos = new List<System.IO.FileInfo>();

                if (parts.Length > 1)
                {
                    string assemblyClassName = parts[parts.Length - 1];
                    string assemblyNamespace = namespacePlusClassName.Substring(0, (namespacePlusClassName.Length - assemblyClassName.Length) - 1);
                    string assemblyName = assemblyNamespace + ".dll";
                    assemblyClassName = assemblyNamespace + "." + assemblyClassName;

                    foreach (string businessProcessPath in ReadBusinessProcessPathList(context))
                    {
                        string assemblyFilePathName = System.IO.Path.Combine(businessProcessPath, assemblyName);

                        System.IO.FileInfo dllFileInfo = new System.IO.FileInfo(assemblyFilePathName);

                        if (dllFileInfo.Exists)
                        {
                            dllFileInfos.Add(dllFileInfo);
                        }
                    }

                    if (dllFileInfos.Count == 1)
                    {
                        classInstance = LateBinding.LoadAssemblyClass(context, assemblyName, dllFileInfos[0].FullName, assemblyClassName, issueList, args);
                    }
                    else if (dllFileInfos.Count > 1)
                    {
                        //We need to start breaking ties between the dlls based on version number then on modified date
                        //LINQ makes this super easy.  I sort the list by the version number as an integer which works provided the version numbers don't do anything with leading zeros in the minor parts.
                        var dllVersionInfos = dllFileInfos.Select(d => FileVersionInfo.GetVersionInfo(d.FullName))
                                                        .OrderByDescending(fvi => fvi.FileVersion.Replace(".", string.Empty))
                                                        .ThenByDescending(fvi => new System.IO.FileInfo(fvi.FileName).LastWriteTime).ToList();


                        classInstance = LateBinding.LoadAssemblyClass(context, assemblyName, dllVersionInfos.First().FileName, assemblyClassName, issueList, args);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex, issueList.ToString());
            }

            return classInstance;
        }

        /// <summary>
        /// Created:        05/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Loads a specific assembly from the file system and then initializes a named 
        ///                     object for a class within that assebly. 
        /// </summary>        
        public static object LoadAssemblyClass(IContext context,
                                                string assemblyFullName,
                                                string assemblyFilePathName,
                                                string className,
                                                StringBuilder issueList,
                                                params object[] args)
        {
            object returnObject = null;

            try
            {
                className = className.Trim();

                System.Reflection.Assembly asmFormMain = GetAssembly(context, assemblyFullName, assemblyFilePathName, issueList);

                if (asmFormMain != null)
                {
                    Type classType = null;

                    try
                    {
                        classType = asmFormMain.GetType(className, true, true);
                    }
                    catch (System.Exception ex)
                    {
                        issueList.AppendLine(ex.ToString());
                    }

                    if (classType != null)
                    {
                        // Create an instance of the class
                        returnObject = Activator.CreateInstance(classType, args);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex, issueList.ToString());
            }

            return returnObject;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Created:        05/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Reads the type from cache if it exists.      
        /// </summary>        
        private static Type ReadTypeFromCache(IContext context, string namespacePlusClassName)
        {
            Type classType = null;

            try
            {
                lock (_commandTypeRefSyncLock)
                {
                    if (_commandTypeRef.ContainsKey(namespacePlusClassName))
                    {
                        classType = (Type)_commandTypeRef[namespacePlusClassName];
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return classType;
        }

        /// <summary>
        /// Created:        05/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Writes a good type to the cache for performance.
        /// </summary>        
        private static void WriteTypeToCache(IContext context, string namespacePlusClassName, object classInstance)
        {
            try
            {
                if (classInstance != null)
                {
                    lock (_commandTypeRefSyncLock)
                    {
                        _commandTypeRef[namespacePlusClassName] = classInstance.GetType();
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        5/3/2017
        /// Author:         David J. McKee
        /// Purpose:        Loads an assebly from the file system.
        /// </summary>        
        public static System.Reflection.Assembly GetAssembly(IContext context,
                                                                string assemblyFullName,
                                                                string assemblyFilePathName,
                                                                StringBuilder issueList)
        {
            System.Reflection.Assembly returnAssembly = null;

            try
            {
                // First, see if the assebly has already been loaded.
                assemblyFilePathName = assemblyFilePathName.ToLower().Trim();

                string assemblyFullNameTest = assemblyFullName.ToLower().Trim();
                assemblyFullNameTest = assemblyFullNameTest.Replace(".dll", string.Empty);

                foreach (System.Reflection.Assembly objAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (assemblyFilePathName.Length > 0)
                        {
                            try
                            {
                                if (objAssembly.Location.ToLower().Trim() == assemblyFilePathName)
                                {
                                    returnAssembly = objAssembly;
                                    break;
                                }
                            }
                            catch { }
                        }

                        if (assemblyFullName.Length > 0)
                        {
                            if (objAssembly.GetName().Name.ToLower().Trim() == assemblyFullNameTest)
                            {
                                returnAssembly = objAssembly;
                                break;
                            }
                        }
                    }
                    catch (System.NotSupportedException nse)
                    {
                        issueList.Append("assemblyFullName=" + assemblyFullName + ", assemblyFilePathName=" + assemblyFilePathName);

                        // Expected exception
                        System.Diagnostics.Debug.Write(nse.ToString() + ", assemblyFullName=" + assemblyFullName + ", assemblyFilePathName=" + assemblyFilePathName);
                    }
                    catch (Exception ex1)
                    {
                        issueList.Append("assemblyFullName=" + assemblyFullName + ", assemblyFilePathName=" + assemblyFilePathName);

                        Console.WriteLine(ex1.ToString() + ", assemblyFullName=" + assemblyFullName + ", assemblyFilePathName=" + assemblyFilePathName);
                    }
                }

                // Try to load the assembly by name explicitly
                try
                {
                    if (returnAssembly == null)
                    {
                        returnAssembly = System.Reflection.Assembly.Load(assemblyFullName);
                    }
                }
                catch { }         // Ignore error                    

                // Finally, try to load the assembly from the file system.
                if (System.IO.File.Exists(assemblyFilePathName) && returnAssembly == null)
                {
                    if (returnAssembly == null)
                    {
                        try
                        {
                            returnAssembly = System.Reflection.Assembly.LoadFrom(assemblyFilePathName);
                        }
                        catch (System.Exception ex)
                        {
                            issueList.Append("assemblyFullName=" + assemblyFullName + ", assemblyFilePathName=" + assemblyFilePathName);

                            System.Diagnostics.Debug.Write(ex.ToString() + ", assemblyFullName=" + assemblyFullName + ", assemblyFilePathName=" + assemblyFilePathName);
                        }
                    }
                }
                else
                {
                    if (!System.IO.File.Exists(assemblyFilePathName))
                    {
                        issueList.AppendLine("File Not Found '" + assemblyFilePathName + "'.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return returnAssembly;
        }

        #endregion
    }
}
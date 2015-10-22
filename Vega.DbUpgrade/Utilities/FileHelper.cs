using System.IO;

namespace Vega.DbUpgrade.Utilities
{
    /// <summary>
    /// Contains file management methods.
    /// </summary>
    public class FileHelper
    {
        /// <summary>
        /// Returns file from directory by name.
        /// </summary>
        /// <param name="dirInfo">Directory on file system where search will be performed.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="searchOption">Defines should search </param>
        /// <returns>The directory if exists; otherwise <c>null</c></returns>
        public static FileInfo GetFileByName(DirectoryInfo dirInfo, string fileName, SearchOption searchOption)
        {
            InputParametersValidator.ValidateObjectParameter(dirInfo, "dirInfo");
            InputParametersValidator.ValidateStringNotEmpty(fileName, "fileName");

            FileInfo retVal = null;
            var files = dirInfo.GetFiles(fileName, searchOption);
            if (files.Length > 0)
            {
                retVal = files[0];
            }

            return retVal;
        }

        /// <summary>
        /// Returns sub directory by name.
        /// </summary>
        /// <param name="dirInfo">Base directory on file system.</param>
        /// <param name="dirName">Name of the directory that should be returned.</param>
        /// <param name="searchOption">AllDirectories or TopDirectoryOnly</param>
        /// <returns>The directory if exists; otherwise <c>null</c></returns>
        public static DirectoryInfo GetSubDirectoryByName(DirectoryInfo dirInfo, string dirName, SearchOption searchOption)
        {
            InputParametersValidator.ValidateObjectParameter(dirInfo, "dirInfo");
            InputParametersValidator.ValidateStringNotEmpty(dirName, "dirName");

            DirectoryInfo retVal = null;
            var subDirs = dirInfo.GetDirectories(dirName, searchOption);
            if (subDirs.Length > 0)
            {
                retVal = subDirs[0];
            }

            return retVal;
        }

        /// <summary>
        /// Gets file content.
        /// </summary>
        /// <param name="fileName">File which content will be returned.</param>
        /// <returns>File's content.</returns>
        public static string GetFileContent(string fileName)
        {
            InputParametersValidator.ValidateStringNotEmpty(fileName, "fileName");

            string retVal;
            using (TextReader textReader = new StreamReader(fileName))
            {
                retVal = textReader.ReadToEnd();
            }

            return retVal;
        }

        /// <summary>
        /// Gets relative path based on full path and first path part
        /// </summary>
        /// <param name="fullPath">Full path to folder</param>
        /// <param name="scriptRelPath">Relative path to folder with scripts</param>
        /// <returns>Returns relative path</returns>
        public static string GetRelativePath(string fullPath, string scriptRelPath)
        {
            var index = fullPath.LastIndexOf(Path.DirectorySeparatorChar + scriptRelPath, System.StringComparison.Ordinal);

            if (index == -1)
            {
                index = fullPath.LastIndexOf(Path.DirectorySeparatorChar + "Scripts", System.StringComparison.Ordinal);
            }

            return fullPath.Substring(index);
        }
    }
}
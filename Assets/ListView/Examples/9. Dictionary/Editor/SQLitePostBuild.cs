using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.IO;

namespace Unity.Labs.ListView
{
    static class SQLitePostBuild
    {
        [PostProcessBuild(0)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuildProject)
        {
            string fileName;
            string dirName;
            switch (target)
            {
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    dirName = Path.GetDirectoryName(pathToBuildProject);
                    fileName = string.Format("{0}_Data", Path.GetFileNameWithoutExtension(pathToBuildProject));
                    pathToBuildProject = string.IsNullOrEmpty(dirName) ? fileName : Path.Combine(dirName, fileName);

                    Debug.Log(string.Format("Copying {0} to {1}",
                        Path.Combine(Application.dataPath, DictionaryResourceStrings.editorDatabasePath),
                        Path.Combine(pathToBuildProject, DictionaryResourceStrings.databasePath)));

                    File.Copy(Path.Combine(Application.dataPath, DictionaryResourceStrings.editorDatabasePath),
                        Path.Combine(pathToBuildProject, DictionaryResourceStrings.databasePath));
                    break;
#if UNITY_2017_3_OR_NEWER
                case BuildTarget.StandaloneOSX:
#else
                case BuildTarget.StandaloneOSXIntel:
                case BuildTarget.StandaloneOSXIntel64:
#endif

                    dirName = Path.GetDirectoryName(pathToBuildProject);
                    fileName = string.Format("{0}.app", Path.GetFileNameWithoutExtension(pathToBuildProject));

                    pathToBuildProject = Path.Combine(
                        string.IsNullOrEmpty(dirName) ? fileName : Path.Combine(dirName, fileName), "Contents");

                    Debug.Log(string.Format("Copying {0} to {1}",
                        Path.Combine(Application.dataPath, DictionaryResourceStrings.editorDatabasePath),
                        Path.Combine(pathToBuildProject, DictionaryResourceStrings.databasePath)));

                    File.Copy(Path.Combine(Application.dataPath, DictionaryResourceStrings.editorDatabasePath),
                        Path.Combine(pathToBuildProject, DictionaryResourceStrings.databasePath));
                    break;
            }
        }
    }
}

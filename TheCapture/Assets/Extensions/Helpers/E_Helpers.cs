#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;


    public class E_Helpers
    {
        
        public static string ReadTextFile()
        {
            string path = "Assets/Extensions/Packages/version.txt";
            //Read the text from directly from the test.txt file
            StreamReader reader = new StreamReader(path);
            return reader.ReadToEnd();
        }
        public static Texture2D LoadImage(Vector2 size, string filePath)
        {

            byte[] bytes = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;
            texture.LoadImage(bytes);

            return texture;
        }
        public enum FileAddtionType { NewLine, Replace, InsertLine };
        public class FileData
        {
            public bool exists = false;
            public string path = "";
        }
        public class Additions
        {
            public string target = "";
            public string add = "";
            public string nextline = "";
            public FileAddtionType type = FileAddtionType.NewLine;
            public Additions(string in_target, string in_add, string in_nextline, FileAddtionType in_type)
            {
                this.target = in_target;
                this.add = in_add;
                this.nextline = in_nextline;
                this.type = in_type;
            }
        }

        #region Inspector
        public static BindingFlags allBindings = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public static string[] EditorGetVariables(Type classType)
        {
            List<string> variables = new List<string>();
            BindingFlags bindingFlags = BindingFlags.DeclaredOnly | // This flag excludes inherited variables.
                                        BindingFlags.Public |
                                        BindingFlags.NonPublic |
                                        BindingFlags.Instance |
                                        BindingFlags.Static;
            foreach (FieldInfo field in classType.GetFields(bindingFlags))
            {
                variables.Add(field.Name);
            }
            variables.Add("m_Script");
            return variables.ToArray();
        }
        #endregion
        

        #region Loading/Saving Resources
        public static Texture2D LoadTexture(string path)
        {
            Texture2D texture = new Texture2D(10,10);
            texture.LoadImage(File.ReadAllBytes(path));
            texture.Apply();
            return texture;
        }

        public static GUISkin LoadSkin(string path)
        {
            if (path.StartsWith("Assets" + Path.DirectorySeparatorChar) == false)
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
            GUISkin skin = AssetDatabase.LoadAssetAtPath<GUISkin>(path);
            return skin;
        }

        public static GameObject CreatePrefabFromPath(string path)
        {
            if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
            UnityEngine.Object prefab = AssetDatabase.LoadMainAssetAtPath(path);
            GameObject target = (GameObject)prefab;
            return PrefabUtility.InstantiatePrefab(target) as GameObject;
        }

        public static GameObject GetPrefabReference(string path)
        {
            UnityEngine.Object prefab = AssetDatabase.LoadMainAssetAtPath(path);
            GameObject target = (GameObject)prefab;
            return target;
        }

        public static void SaveAsPrefab(GameObject obj, string path)
        {
            if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
        }

        #endregion

        #region File/Directory Actions
        public static string[] GetAllPrefabs(bool excludeResourcesFolder=false, bool onlyIncludeResourcesFolder=false)
        {
            string[] temp = AssetDatabase.GetAllAssetPaths();
            List<string> result = new List<string>();
            foreach (string s in temp)
            {
                if (s.Contains(".prefab"))
                {
                    if (onlyIncludeResourcesFolder == true)
                    {
                        if (s.Contains("Assets/Resources"))
                        {
                            result.Add(s);
                        }
                    }
                    else
                    {
                        if (excludeResourcesFolder == true && !s.Contains("Assets/Resources"))
                        {
                            result.Add(s);
                        }
                        else
                        {
                            result.Add(s);
                        }
                    }
                }
            }
            return result.ToArray();
        }

        public static FileData FileExists(string filename, string directory, string fileType = "*.cs")
        {
            FileData data = new FileData();
            DirectoryInfo dir = new DirectoryInfo(directory);
            foreach (FileInfo file in dir.GetFiles(fileType))
            {
                if (file.Name == filename)
                {
                    data.exists = true;
                    data.path = file.ToString();
                    return data;
                }
            }
            foreach (string subDir in Directory.GetDirectories(directory))
            {
                data = FileExists(filename, subDir);
                if (data.exists == true)
                {
                    return data;
                }
            }
            data.exists = false;
            data.path = "";
            return data;
        }

        public static void ModifyFile(string filepath, List<Additions> additions)
        {
            int _index = 0;
            List<string> lines = new List<string>(System.IO.File.ReadAllLines(filepath));
            List<string> modified = new List<string>();
            bool added = false;

            for (int i = 0; i < lines.Count; i++)
            {
                foreach (Additions item in additions)
                {
                    if (lines[i].Trim().Equals(item.target))
                    {
                        string space;
                        switch (item.type)
                        {
                            case FileAddtionType.NewLine:
                                added = true;
                                modified.Add(lines[i]);
                                if (!lines[i + 1].Trim().Equals(item.add))              //prevent this code from adding the same line in twice
                                {
                                    space = lines[i].Split(item.target[0])[0];          //Get spaces
                                    modified.Add(space + item.add);
                                }
                                break;
                            case FileAddtionType.Replace:
                                added = true;
                                if (!lines[i].Trim().Equals("//" + item.target))       //prevent this code from adding the same line in twice
                                {
                                    space = lines[i].Split(item.target[0])[0];         //Get spaces
                                    modified.Add(space + "//" + lines[i].Trim());      //Comment out the target line
                                }
                                else
                                {
                                    modified.Add(lines[i]);
                                }
                                if (!lines[i + 1].Trim().Equals(item.add))             //prevent this code from adding the same line in twice
                                {
                                    space = lines[i].Split(item.target[0])[0];         //Get spaces
                                    modified.Add(space + item.add);                    //Add new line
                                }
                                break;
                            case FileAddtionType.InsertLine:
                                _index = i + 1;
                                if (lines[_index].Trim() == "")
                                    _index += 1;
                                if (lines[_index].Trim().Equals(item.nextline))
                                {
                                    added = true;
                                    space = lines[i].Split(item.target[0])[0];         //Get spaces
                                    modified.Add(lines[i]);
                                    modified.Add(space + item.add);
                                }
                                break;
                        }
                    }
                }
                if (added == false)
                {
                    modified.Add(lines[i]);
                }
                else
                {
                    added = false;
                }

            }

            using (StreamWriter writer = new StreamWriter(filepath, false))
            {
                foreach (string line in modified)
                {
                    writer.WriteLine(line);
                }
            }
        }

        public static bool DirExists(string path)
        {
            if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
            return Directory.Exists(path);
        }

        public static bool FileExists(string path)
        {
            if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
            return File.Exists(path);
        }

        public static void DeleteFile(string path)
        {
            if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
            File.Delete(path);
        }

        public static bool CreateDirectory(string path)
        {
            if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
            if (DirExists(path))
            {
                return false;
            }
            else
            {
                Directory.CreateDirectory(path);
                return true;
            }
        }

        public static void DeleteDir(string path)
        {
            if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
            FileUtil.DeleteFileOrDirectory(path);
        }

        public static string GetDirectoryPath(string path)
        {
            if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                path = "Assets" + Path.DirectorySeparatorChar + path;
            }
            return Path.GetDirectoryName(path);
        }
        
        public static string CommentOutFile(string path, bool commentOut)
        {
            try
            {
                if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
                {
                    path = "Assets" + Path.DirectorySeparatorChar + path;
                }
                List<string> lines = new List<string>();
                lines.AddRange(File.ReadAllLines(@path));
                if (commentOut == true)
                {
                    if (!lines[0].Contains("/*"))
                    {
                        lines.Insert(0, "/*");
                        lines.Add("*/");
                    }
                }
                else
                {
                    if (lines[0] == "/*")
                    {
                        lines.RemoveAt(0);
                    }
                    if (lines[lines.Count - 1] == "*/")
                    {
                        lines.RemoveAt(lines.Count - 1);
                    }
                }
                File.WriteAllLines(@path, lines);
                return (commentOut == true) ? "Successfully commented out file at path: " + path : "Successfully un-commented file at path: " + path;
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }

        public static string CommentOutRegionInFile(string path, string region, bool commentOut)
        {
            try
            {
                if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
                {
                    path = "Assets" + Path.DirectorySeparatorChar + path;
                }
                List<string> lines = new List<string>();
                lines.AddRange(File.ReadAllLines(@path));
                List<string> modifiedLines = new List<string>();
                bool inregion = false;
                foreach(string line in lines)
                {
                    if (line.Trim().Contains("#region "+region))
                    {
                        inregion = true;
                        if (commentOut == true && !line.Contains("/*"))
                        {
                            modifiedLines.Add("/*"+line);
                        }
                        else
                        {
                            modifiedLines.Add(line.Replace("/*",""));
                        }
                    }
                    else if (inregion == true && line.Trim().Contains("#endregion"))
                    {
                        inregion = false;
                        if (commentOut == true && !line.Contains("*/"))
                        {
                            modifiedLines.Add(line+"*/");
                        }
                        else
                        {
                            modifiedLines.Add(line.Replace("*/", ""));
                        }
                    }
                    else
                    {
                        modifiedLines.Add(line);
                    }
                }
                File.WriteAllLines(@path, modifiedLines);
                return (commentOut == true) ? "Successfully commented out region: " + region : "Successfully uncommented region: " + region;
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }
        }

        public static bool FileContainsText(string path, string text)
        {
            try
            {
                if (!path.StartsWith("Assets" + Path.DirectorySeparatorChar))
                {
                    path = "Assets" + Path.DirectorySeparatorChar + path;
                }
                List<string> lines = new List<string>();
                lines.AddRange(File.ReadAllLines(@path));
                foreach (string line in lines)
                {
                    if (line.Trim().Contains(text.Trim()))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Editor Actions
        public static void SetObjectIcon(GameObject obj, string loadImageAtPath)
        {
            if (!loadImageAtPath.StartsWith("Assets" + Path.DirectorySeparatorChar))
            {
                loadImageAtPath = "Assets" + Path.DirectorySeparatorChar + loadImageAtPath;
            }
            GUIContent icon = EditorGUIUtility.IconContent(loadImageAtPath);
            var egu = typeof(EditorGUIUtility);
            var flags = BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic;
            var args = new object[] { obj, icon.image };
            var setIcon = egu.GetMethod("SetIconForObject", flags, null, new Type[] { typeof(UnityEngine.Object), typeof(Texture2D) }, null);
            setIcon.Invoke(null, args);
        }

        public static SerializedProperty GetLayerProperty()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            return layersProp;
        }

        public static bool InspectorTagExists(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        public static void AddInspectorTag(string tagName)
        {
            if (!InspectorTagExists(tagName))
            {
                SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty tagsProp = tagManager.FindProperty("tags");
                int index = tagsProp.arraySize;
                tagsProp.InsertArrayElementAtIndex(index);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(index);
                n.stringValue = tagName;
                tagManager.ApplyModifiedProperties();
                Debug.Log("Added a new tag called \"SpawnPoint\"");
            }
        }

        public static List<string> CopyComponentValues(Component source, Component dest, bool showDebug=false)
        {
            List<string> skips = new List<string>();
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            FieldInfo[] fields = source.GetType().GetFields(flags);
            foreach (FieldInfo field in fields)
            {
                try
                {
                    if (dest != null && dest.GetType().GetField(field.Name, flags) != null)
                    {
                        dest.GetType().GetField(field.Name, flags).SetValue(dest, field.GetValue(source));
                    }
                    else
                    {
                        Debug.LogWarning("(" + source.name + ") Unable to copy field: " + field.Name);
                        skips.Add(field.Name);
                    }
                }
                catch(Exception ex)
                {
                    if (showDebug)
                    {
                        Debug.Log("Failed to copy field \"" + field.Name + "\": "+ ex);
                    }
                }
            }
            return skips;
        }

        #region List GO Weapons
        public static GameObject[] ListPrefabs()
        {
            GameObject[] _GO = Resources.LoadAll<GameObject>("");
            List<GameObject> result = new List<GameObject>();
            foreach (GameObject go in _GO)
            {
                Debug.Log("<color=#e00003>" + go.name + "</color>");

            }

            return result.ToArray();
        }
        #endregion

        public static void AddInspectorLayer(string layerName)
        {
            if (!InspectorLayerExists(layerName))
            {
                SerializedObject layerManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
                SerializedProperty layersProp = layerManager.FindProperty("layers");
                int index = layersProp.arraySize;
                layersProp.InsertArrayElementAtIndex(index);
                SerializedProperty n = layersProp.GetArrayElementAtIndex(index);
                n.stringValue = layerName;
                layerManager.ApplyModifiedProperties();
                Debug.Log("Added a new layer called \"" + layerName + "\"");
            }
        }

        public static bool InspectorLayerExists(string layerName)
        {
            SerializedObject layerManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layerProps = layerManager.FindProperty("layers");

            bool found = false;
            for (int i = 0; i < layerProps.arraySize; i++)
            {
                SerializedProperty t = layerProps.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(layerName))
                {
                    found = true;
                    break;
                }
            }
            return found;
        }
        #endregion
        
    }

#endif

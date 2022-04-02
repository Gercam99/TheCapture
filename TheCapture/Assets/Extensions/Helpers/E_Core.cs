#if UNITY_EDITOR
using System.IO;
using UnityEngine;



    public class E_Core : MonoBehaviour
    {
        public static string e_guiSkinPath = string.Format("Assets{0}Extensions{0}ExtensionsSkin.guiskin", Path.DirectorySeparatorChar);

        public static string e_version = string.Format("Assets{0}Extensions{0}version.txt", Path.DirectorySeparatorChar);
        public static string e_importAllExtensions = string.Format("Assets{0}Extensions{0}Packages{0}AllExtensions.unitypackage", Path.DirectorySeparatorChar);
        public static string e_importSampleExtensions = string.Format("Assets{0}Extensions{0}Packages{0}SampleExtensions.unitypackage", Path.DirectorySeparatorChar);
        public static string e_importSystems = string.Format("Assets{0}Extensions{0}Packages{0}Systems.unitypackage", Path.DirectorySeparatorChar);
        public static string e_importUtilities = string.Format("Assets{0}Extensions{0}Packages{0}Utilities.unitypackage", Path.DirectorySeparatorChar);
        public static string e_importAI = string.Format("Assets{0}Extensions{0}Packages{0}AIAdvanced.unitypackage", Path.DirectorySeparatorChar);

        public static string e_guiBoxPath = string.Format("Extensions{0}Editor{0}Images{0}Box.png", Path.DirectorySeparatorChar);
    }

    public class E_Colors
    {
        public static string e_t_blue_1 = string.Format("Assets{0}Extensions{0}Editor{0}Images{0}Window_Colors{0}color-1.png", Path.DirectorySeparatorChar);
        public static string e_t_blue_2 = string.Format("Assets{0}Extensions{0}Editor{0}Images{0}Window_Colors{0}color-2.png", Path.DirectorySeparatorChar);
        public static string e_t_blue_3 = string.Format("Assets{0}Extensions{0}Editor{0}Images{0}Window_Colors{0}color-3.png", Path.DirectorySeparatorChar);
        public static string e_t_blue_4 = string.Format("Assets{0}Extensions{0}Editor{0}Images{0}Window_Colors{0}color-4.png", Path.DirectorySeparatorChar);
        public static string e_t_blue_5 = string.Format("Assets{0}Extensions{0}Editor{0}Images{0}Window_Colors{0}color-5.png", Path.DirectorySeparatorChar);
        public static string e_t_blue_6 = string.Format("Assets{0}Extensions{0}Editor{0}Images{0}Window_Colors{0}Window.png", Path.DirectorySeparatorChar);

        public static Color e_c_blue_1 = new Color(114 / 255f, 137 / 255f, 218 / 255f); //new Color(47.1f/255f, 52.9f / 255f, 67.1f / 255f);
        public static Color e_c_blue_2 = new Color(66 / 255f, 69 / 255f, 73 / 255f); //new Color(31f / 255f, 38.4f / 255f, 55.7f / 255f);
        public static Color e_c_blue_3 = new Color(54 / 255f, 57 / 255f, 62 / 255f); // new Color(18f / 255f, 25.9f / 255f, 44.7f / 255f);
        public static Color e_c_blue_4 = new Color(40 / 255f, 43 / 255f, 48 / 255f); //new Color(8.6f / 255f, 16.1f / 255f, 33.33f / 255f);
        public static Color e_c_blue_5 = new Color(30 / 255f, 33 / 255f, 36 / 255f); //new Color(2.4f / 255f, 8.2f / 255f, 22.4f / 255f);

        //public static Color e_c_text = new Color();
    }

#endif
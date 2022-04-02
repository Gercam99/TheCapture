using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class ExtensionsWindow : EditorWindow
{
    public static ExtensionsWindow current { get; private set; }
    [NonSerialized] private GUISkin _skin = null;
    private bool showDescription;

    private E_Versions _versions;

    [MenuItem("Extensions/Setup", false)]
    public static void Open()
    {
        ShowWindow();
    }

    private static ExtensionsWindow ShowWindow()
    {
        var window = EditorWindow.GetWindow<ExtensionsWindow>(true, "Extensions", true);
        current = window;
        ResizeWindow(current, 500,270);
        return window;
    }

    private void OnEnable()
    {
        if (!_skin) _skin = E_Helpers.LoadSkin(E_Core.e_guiSkinPath);
        _versions = new E_Versions();
        
        _versions.Initialized();

    }

    private static void ResizeWindow(ExtensionsWindow window, float x, float y)
    {
        window.minSize = new Vector2(x, y);
        window.maxSize = new Vector2(x, y);
    }

    private void OnGUI()
    {

        GUILayout.BeginVertical(_skin.box);
        EditorGUILayout.LabelField("EXTENSIONS", _skin.customStyles[3]);
        GUILayout.Space(20);

        

        ExtensionsPackages();

        
        GUILayout.Space(15);

        if (GUILayout.Button("SHOW DESCRIPTIONS",_skin.button))
        {
            showDescription = !showDescription;
            
            ResizeWindow(current, 500, showDescription? 510:270);
            

        }


        if (showDescription)
        {
            EditorGUILayout.LabelField("DESCRIPTIONS", _skin.customStyles[3]);
            GUILayout.Space(20);



            GUILayout.BeginVertical(_skin.customStyles[5]);
            GUILayout.Label("SAMPLE EXTENSIONS→ Package que contiene scripts para facilitar tu proyeto.");
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_skin.customStyles[5]);
            GUILayout.Label("SYSTEMS→ Package que contiene scripts para hacer sistems (audio, singleton).");
            EditorGUILayout.HelpBox("Se requiere instalar el Package Utilities", MessageType.Info);
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_skin.customStyles[5]);
            GUILayout.Label("UTILITIES→ Package que contiene scripts para la utilidad en el proyecto.");
            GUILayout.EndVertical();

            GUILayout.BeginVertical(_skin.customStyles[5]);
            GUILayout.Label("AI ADVANCED→ Package que contiene scripts para crear una IA avanzada.");
            EditorGUILayout.HelpBox("Se requiere instalar el Package Extensions", MessageType.Info);
            GUILayout.EndVertical();



        }


        // CHECK
        GUILayout.BeginHorizontal();
        CheckVersionsMethod();
        GUILayout.EndHorizontal();

        
        GUILayout.Label("v"+_versions.CurrentVersion, _skin.customStyles[4]);
        GUILayout.EndVertical();

    }

    private void ExtensionsPackages()
    {
        if (GUILayout.Button("SAMPLE EXTENSIONS",_skin.button))
        {
            if (EditorUtility.DisplayDialog("Add Sample Extensions", "Scripts sample extensions", "Continue", "Cancel"))
            {
                AssetDatabase.ImportPackage(E_Core.e_importSampleExtensions, true);
            }
        }

        if (GUILayout.Button("SYSTEMS",_skin.button))
        {
            if (EditorUtility.DisplayDialog("Add Systems", "Scripts systems", "Continue", "Cancel"))
            {
                AssetDatabase.ImportPackage(E_Core.e_importSystems, true);
            }
        }
        
        if (GUILayout.Button("UTILITIES",_skin.button))
        {
            if (EditorUtility.DisplayDialog("Add Utilities", "Scripts utilities", "Continue", "Cancel"))
            {
                AssetDatabase.ImportPackage(E_Core.e_importUtilities, true);
            }
        }

        if (GUILayout.Button("ALL EXTENSIONS",_skin.button))
        {
            if (EditorUtility.DisplayDialog("Add All Extensions", "Scripts of All extensions", "Continue", "Cancel"))
            {
                AssetDatabase.ImportPackage(E_Core.e_importAllExtensions, true);
            }
        }
        
        if (GUILayout.Button("AI ADVANCED",_skin.button))
        {
            if (EditorUtility.DisplayDialog("Add AI Advanced", "Scripts of AI's", "Continue", "Cancel"))
            {
                AssetDatabase.ImportPackage(E_Core.e_importAI, true);
            }
        }
    }

    private void CheckVersionsMethod()
    {
        if (GUILayout.Button("CHECK VERSION",_skin.button))
        {
            _versions.CheckVersions();
        }

        if (_versions.NeedUpdate)
        {
            if (GUILayout.Button("UPDATE",_skin.button))
            {
                _versions.UpdateAndCheckVersions();
            }
        }
    }
    
    private void OnInspectorUpdate()
    {
        Repaint();
    }
}

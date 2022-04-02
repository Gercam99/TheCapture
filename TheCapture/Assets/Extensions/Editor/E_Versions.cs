using System;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using  System.IO;
using System.Net;
using Debug = UnityEngine.Debug;

public enum StatusVersion{Late, Ready, Failed}

public class E_Versions
{
    
    public StatusVersion _status;

    internal StatusVersion StatusVersion
    {
        get => _status;
        set
        {
            _status = value;
            switch (_status)
            {
                case StatusVersion.Late:
                    Debug.Log("UPDATE");
                    break; 
                case  StatusVersion.Ready:
                    Debug.Log("UPDATED");
                    break;
                case StatusVersion.Failed:
                    Debug.Log("CANT CONNECT");
                    break;
                    
            }
        }
    }
    private string versionFile;
    private string currentVersion;
    private string path;
    private bool needUpdate;
    private string pathAllExtensions;



    public bool NeedUpdate => needUpdate;
    public string CurrentVersion => currentVersion;

    public void Initialized()
    {
        path = Application.dataPath + "/Extensions/Packages";
        versionFile = Path.Combine(path, "version.txt");
        currentVersion = E_Helpers.ReadTextFile();

        pathAllExtensions = Path.Combine(path, "AllExtensions.unitypackage");
        
    }

    public void CheckVersions()
    {
        if (File.Exists(versionFile))
        {

            Version localVersion = new Version(File.ReadAllText(versionFile));

            try
            {   
                Debug.Log(File.ReadAllText(versionFile));
                WebClient webClient = new WebClient();
                webClient.Credentials = System.Net.CredentialCache.DefaultCredentials;
                Version onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1g26ELaa8Mi6D7gd9VvubDhU_rTBaSafp"));
                if (onlineVersion.IsDifferentThan(localVersion))
                {
                    // Install New versions
                    needUpdate = true;
                    Debug.Log("There is a new version of the asset: "+onlineVersion.ToString());
                    StatusVersion = StatusVersion.Late;

                }
                else
                {
                    StatusVersion = StatusVersion.Ready;
                }

            }
            catch (Exception e)
            {
                StatusVersion = StatusVersion.Failed;
                Debug.LogError($"Error checking for asset update: {e}");
            }
        }
    }
    public void UpdateAndCheckVersions()
    {
        if (File.Exists(versionFile))
        {
            Version localVersion = new Version(File.ReadAllText(versionFile));


            try
            {
                WebClient webClient = new WebClient();
                Version onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1g26ELaa8Mi6D7gd9VvubDhU_rTBaSafp"));
                
                if (onlineVersion.IsDifferentThan(localVersion))
                {
                    // Install New versions
                    InstallPackages(true, onlineVersion);

                }
                else
                {
                    StatusVersion = StatusVersion.Ready;
                    Debug.Log("UPDATED");
                }
            }
            catch (Exception e)
            {
                StatusVersion = StatusVersion.Failed;
                Debug.LogError($"Error checking for asset update: {e}");
            }
        }
        else
        {
            // NO INSTALL
            InstallPackages(false, Version.zero);
            Debug.Log("NO INSTALLING");
        }
    }
    

    private void InstallPackages(bool _isUpdate, Version _onlineVersion)
    {
        try
        {
            WebClient client = new WebClient();
            if (_isUpdate)
            {
                StatusVersion = StatusVersion.Ready;
            }
            else
            {
                StatusVersion = StatusVersion.Late;
                _onlineVersion = new Version(client.DownloadString("https://drive.google.com/uc?export=download&id=1g26ELaa8Mi6D7gd9VvubDhU_rTBaSafp"));
            }

            Debug.Log("PREPARE INSTALLER");
            DownloadFiles(_onlineVersion, client);
        }
        catch (Exception e)
        {
            StatusVersion = StatusVersion.Failed;
            Debug.LogError($"Error install Packages: {e}");
        }
    }

    private void DownloadFiles(Version _onlineVersion, WebClient client)
    {
        client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadDataCompletedEventArgs);


        // Install Packages
        client.DownloadFileAsync(
            new Uri("https://drive.google.com/uc?export=download&id=1CCvewMSANmx9-r_oFGGCTNCnjM7MC9zq"), pathAllExtensions,
            _onlineVersion);
    }
    

    private void DownloadDataCompletedEventArgs(object sender, AsyncCompletedEventArgs e)
    {
        try
        {
            string onlineVersion = ((Version) e.UserState).ToString();

            if (EditorUtility.DisplayDialog("IMPORT ALL EXTENSIONS", "Package with all extensions", "Continue", "Cancel"))
            {
                AssetDatabase.ImportPackage(E_Core.e_importAllExtensions, true);
                currentVersion = onlineVersion;
                File.WriteAllText(versionFile, currentVersion);
                
                Debug.Log("DONE");
                needUpdate = false;

            }           


        }
        catch (Exception exception)
        {
            StatusVersion = StatusVersion.Failed;
            Debug.LogError($"Error to import Packages: {exception}");
        }
    }
    
    
    

}

public struct Version
{
    internal static Version zero = new Version( 0, 0,0);
    
    private short major;
    private short minor;
    private short subMinor;

    internal Version( short _major, short _minor, short _subMinor)
    {
        major = _major;
        minor = _minor;
        subMinor = _subMinor;
    }

    internal Version(string _version)
    {
        string[] _versionString = _version.Split('.');
        if (_versionString.Length != 3)
        {
            major = 0;
            minor = 0;
            subMinor = 0;
            return;
        }
        
        major = short.Parse(_versionString[0]);
        minor = short.Parse(_versionString[1]);
        subMinor = short.Parse(_versionString[2]);
    }

    internal bool IsDifferentThan(Version _otherVersion)
    {
        if (major != _otherVersion.major) return true;
        else
        {
            if (minor != _otherVersion.minor) return true;
            else
            {
                if (subMinor != _otherVersion.subMinor) return true;
            }
        }

        return false;
    }

    public override string ToString()
    {
        return $"{major}.{minor}.{subMinor}";
    }
}

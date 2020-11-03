using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class InstallBoltMenuItem
{
	private const string InstallFolder = "Install Bolt";
#if !UNITY_2019_3_OR_NEWER
	private const string NET3PackageRuntimeName = "NET3";
#endif
	private const string NET4PackageRuntimeName = "NET4";

	[MenuItem("Tools/Install Bolt")]
	private static void Install()
	{
		var packageFiles = Directory.GetFiles(Path.Combine(Application.dataPath, InstallFolder), "*.unitypackage");

		if (packageFiles.Length == 0)
		{
			EditorUtility.DisplayDialog("Bolt Install Error", "Could not find any Bolt package file under '" + InstallFolder + "'.", "OK");
			return;
		}

		string matchingPackageFile = null;

		foreach (var packageFile in packageFiles)
		{
#if UNITY_2019_3_OR_NEWER
			if (Path.GetFileNameWithoutExtension(packageFile).Contains(NET4PackageRuntimeName))
#else
			if (PlayerSettings.scriptingRuntimeVersion == InferRuntimeVersion(Path.GetFileNameWithoutExtension(packageFile)))
#endif
			{
				matchingPackageFile = packageFile;
				break;
			}
		}

		if (matchingPackageFile == null)
		{
#if UNITY_2019_3_OR_NEWER
			EditorUtility.DisplayDialog("Bolt Install Error", "Could not find any Bolt package file that matches the current scripting runtime version: 'NET4'.", "OK");
#else
			EditorUtility.DisplayDialog("Bolt Install Error", "Could not find any Bolt package file that matches the current scripting runtime version: '" + PlayerSettings.scriptingRuntimeVersion + "'.", "OK");
#endif
			return;
		}

#if UNITY_2019_3_OR_NEWER
		AssetDatabase.ImportPackage(matchingPackageFile, true);
#else
		if (EditorUtility.DisplayDialog("Install Bolt", "Import Bolt for " + GetRuntimeVersionStringPretty(PlayerSettings.scriptingRuntimeVersion) + "?", "Import", "Cancel"))
		{
			AssetDatabase.ImportPackage(matchingPackageFile, true);
		}
#endif
    }

#if !UNITY_2019_3_OR_NEWER
    private static string GetRuntimeVersionString(ScriptingRuntimeVersion version)
	{
		switch (version)
		{
			case ScriptingRuntimeVersion.Latest:
				return NET4PackageRuntimeName;

			case ScriptingRuntimeVersion.Legacy:
				return NET3PackageRuntimeName;

			default:
				return version.ToString();
		}
	}

	private static string GetRuntimeVersionStringPretty(ScriptingRuntimeVersion version)
	{
		switch (version)
		{
			case ScriptingRuntimeVersion.Latest:
				return ".NET 4.x";

			case ScriptingRuntimeVersion.Legacy:
				return ".NET 3.x";

			default:
				return version.ToString();
		}
	}

	private static ScriptingRuntimeVersion? InferRuntimeVersion(string packageName)
	{
		foreach (var version in Enum.GetValues(typeof(ScriptingRuntimeVersion)).Cast<ScriptingRuntimeVersion>())
		{
			if (packageName.Contains(GetRuntimeVersionString(version)))
			{
				return version;
			}
		}

		return null;
	}
#endif
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HarmonyLib;
using Assets.Scripts;
using Assets.Scripts.Serialization;
using Assets.Scripts.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Assets.Scripts.Util;

namespace StationeersBepInEx.FixModLoading
{
    [HarmonyPatch(typeof(WorldManager))]
    public static class WorldManager_Patch
    {
		[HarmonyPatch("LoadModDataFiles", new Type[] { typeof(string) })]
		[HarmonyPrefix]
		public static bool LoadModDataFiles_Patch(WorldManager __instance, string modPath)
        {
			string installDir = GameManager.SteamAppPath;
			string gameDataDirectory = Application.streamingAssetsPath + "/Data/";
			string gameWorkshopDirectory = installDir + "/../../workshop/content/544550/";
			string localModDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/My Games/Stationeers/mods/";
			XmlSaveLoad.UpdateLoadingScreen("", 0f, "Loading mods...");
			using (IEnumerator<ModData> enumerator = WorkshopMenu.ModsConfig.GetEnabledMods().GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ModData modData = enumerator.Current as ModData;
					if (modData.IsCore)
					{
						LoadDataFiles(gameDataDirectory);
						XmlSaveLoad.UpdateLoadingScreen("", 0f, "Loaded core game");
					}
					else
					{
						if (modData.IsWorkshop)
						{
							string dirPath = gameWorkshopDirectory + modData.Id.ToString() + "/GameData/";
							LoadDataFiles(dirPath);
							XmlSaveLoad.UpdateLoadingScreen("", 0f, "Loaded workshop mod " + modData.Id.ToString());
						}
						else if (modData.IsLocal)
						{
							//string dirPath2 = localModDirectory + modData.LocalPath + "/GameData/";
							string dirPath2 = modData.LocalPath + "/GameData/";
							LoadDataFiles(dirPath2);
							XmlSaveLoad.UpdateLoadingScreen("", 0f, "Loaded local mod " + Path.GetDirectoryName(modData.LocalPath));
						}
					}
				}
			}
			return false; // skip original method
        }

        private static void LoadDataFiles(string dirPath)
        {
			if (!Directory.Exists(dirPath))
			{
				return;
			}
			string[] files = Directory.GetFiles(dirPath, "*.xml", SearchOption.AllDirectories);
			foreach (string text in files)
			{
				if (!text.ToLower().Contains("gamedata/language"))
				{
					WorldManager.LoadXmlFileData(Serializers.GameData, text);
					UnityEngine.Debug.Log("Loaded mod file " + text);
				}
			}
			if (Directory.Exists(dirPath + "\\Language"))
			{
				Localization.GetLanguages(dirPath);
				Localization.ProcessNewPages(Settings.CurrentData.LanguageCode);
			}
		}
	}
}


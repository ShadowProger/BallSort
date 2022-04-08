using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

namespace Assets.SimpleLocalization
{
	[CreateAssetMenu]
    public class LocalizationSyncSettings : ScriptableObject
    {
		/// <summary>
		/// Table id on Google Spreadsheet.
		/// Let's say your table has the following url https://docs.google.com/spreadsheets/d/1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4/edit#gid=331980525
		/// So your table id will be "1RvKY3VE_y5FPhEECCa5dv4F7REJ7rBtGzQg9Z_B_DE4" and sheet id will be "331980525" (gid parameter)
		/// </summary>
		public string TableId;

		/// <summary>
		/// Table sheet contains sheet name and id. First sheet has always zero id. Sheet name is used when saving.
		/// </summary>
		public Sheet[] Sheets;

		/// <summary>
		/// Folder to save spreadsheets. Must be inside Resources folder.
		/// </summary>
		public UnityEngine.Object SaveFolder;

		private const string UrlPattern = "https://docs.google.com/spreadsheets/d/{0}/export?format=csv&gid={1}";

#if UNITY_EDITOR

		[Button("Sync")]
		/// <summary>
		/// Sync spreadsheets.
		/// </summary>
		public void Sync()
		{
			//UnityMainThreadDispatcher.Instance().StartCoroutine(SyncCoroutine());
		}

		private IEnumerator SyncCoroutine()
		{
			var folder = UnityEditor.AssetDatabase.GetAssetPath(SaveFolder);

			Debug.Log("<color=yellow>Sync started, please wait for confirmation message...</color>");

			var urls = new List<string>();

			foreach (var sheet in Sheets)
			{
				var url = string.Format(UrlPattern, TableId, sheet.Id);

				Debug.Log($"Downloading: {url}...");

				urls.Add(url);
			}

            for (int i = 0; i < urls.Count; i++)
			{
                string url = urls[i];
                using (UnityWebRequest www = UnityWebRequest.Get(url))
                {
					yield return www.SendWebRequest();

					if (!www.isNetworkError && !www.isHttpError)
					{
						var path = System.IO.Path.Combine(folder, Sheets[i].Name + ".csv");

						System.IO.File.WriteAllBytes(path, www.downloadHandler.data);
						UnityEditor.AssetDatabase.Refresh();
						Debug.LogFormat("Sheet {0} downloaded to {1}", Sheets[i].Id, path);
					}
					else
					{
						throw new Exception(www.error);
					}
				}
			}

			Debug.Log("<color=green>Localization successfully synced!</color>");
		}

#endif
	}
}

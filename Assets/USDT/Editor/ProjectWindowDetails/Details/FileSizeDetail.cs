using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace USDT.CustomEditor.ProjectWindowDetails {
	/// <summary>
	/// Draws the file size of assets into the project window.
	/// </summary>
	public class FileSizeDetail : ProjectWindowDetailBase
	{
		private readonly static Dictionary<string, string> _sizeMap = new Dictionary<string, string>();
		public FileSizeDetail()
		{
			Name = "File Size";
			Alignment = TextAlignment.Right;
			ColumnWidth = 80;
		}

		public override string GetLabel(string guid, string assetPath, Object asset)
		{
			return EditorUtility.FormatBytes(GetFileSize(assetPath));
		}

		private long GetFileSize(string assetPath)
		{

			string fullAssetPath =
				string.Concat(Application.dataPath.Substring(0, Application.dataPath.Length - 7), "/", assetPath);
			long size = new FileInfo(fullAssetPath).Length;
			return size;
		}
	}
}

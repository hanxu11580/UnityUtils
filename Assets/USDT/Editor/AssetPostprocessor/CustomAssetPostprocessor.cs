using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using USDT.Utils;

namespace USDT.CustomEditor {
    public class CustomAssetPostprocessor : AssetPostprocessor {
        private static StringBuilder _SB = new StringBuilder();
        //模型导入之前调用
        public void OnPreprocessModel() {
            //LogUtils.Log("Model导入前" + this.assetPath);
        }

        //模型导入之前调用
        public void OnPostprocessModel(GameObject go) {
            //LogUtils.Log("Model导入后" + go.name);
        }

        //纹理导入之前调用，针对入到的纹理进行设置
        public void OnPreprocessTexture() {
            //Debug.Log("Texture导入前=" + this.assetPath);
            //TextureImporter impor = this.assetImporter as TextureImporter;
            //impor.textureFormat = TextureImporterFormat.ARGB32;
            //impor.maxTextureSize = 512;
            //impor.textureType = TextureImporterType.Advanced;
            //impor.mipmapEnabled = false;

        }

        public void OnPostprocessTexture(Texture2D tex) {

        }

        public void OnPreprocessAudio() {
            //AudioImporter audio = this.assetImporter as AudioImporter;
            //audio.format = AudioImporterFormat.Compressed;
        }

        public void OnPostprocessAudio(AudioClip clip) {
        }
        
        /// <summary>
        /// 导入的都是Assets路径
        /// </summary>
        /// <param name="importedAsset"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            if(importedAsset.Length > 0) {
                bool showLog = false;
                _SB.Clear();
                foreach (string path in importedAsset) {
                    if (path.EndsWith("cs")) {
                        continue;
                    }
                    showLog = true;
                    _SB.Append(StringUtils.Format(ColorStringConst.GreenFormat, path));
                    _SB.Append("\n");
                    SetPlugnImporterByPath(path);
                }

                if (showLog) {
                    _SB.Append($"导入资源数量:{importedAsset.Length}\n\n");
                    lg.i(_SB.ToString());
                }
            }
        }

        /// <summary>
        /// 根据路径设置插件的平台选项
        /// </summary>
        /// <param name="path"></param>
        private static void SetPlugnImporterByPath(string path) {
            var pluginsImporter = PluginImporter.GetAtPath(path) as PluginImporter;
            if(pluginsImporter == null) {
                return;
            }
            if (path.Contains(CustomAssetPostprocessorConst.SubPathPluginsiOS)) {
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.iOS, true);
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.Android, false);
                lg.i($"设置 {path} only iOS platform");
            }
            else if (path.Contains(CustomAssetPostprocessorConst.SubPathPluginsAndroid)) {
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.Android, true);
                lg.i($"设置 {path} only Android platform");
            }
        }
    }
}
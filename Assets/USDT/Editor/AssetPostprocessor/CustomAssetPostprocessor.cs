using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using USDT.Utils;

namespace USDT.CustomEditor {
    public class AssetPostprocessorConst {
        public const string SubPathPluginsiOS = @"Plugins/iOS";
        public const string SubPathPluginsAndroid = @"Plugins/Android";
    }

    public class CustomAssetPostprocessor : AssetPostprocessor {
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
            LogUtils.Log($"导入资源数量:{importedAsset.Length}", true);
            foreach (string path in importedAsset) {
                SetPlugnImporterByPath(path);
            }
            //foreach (string path in deletedAssets) {

            //}
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
            if (path.Contains(AssetPostprocessorConst.SubPathPluginsiOS)) {
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.iOS, true);
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.Android, false);
                LogUtils.Log($"设置 {path} only iOS platform");
            }
            else if (path.Contains(AssetPostprocessorConst.SubPathPluginsAndroid)) {
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.Android, true);
                LogUtils.Log($"设置 {path} only Android platform");
            }
        }
    }
}
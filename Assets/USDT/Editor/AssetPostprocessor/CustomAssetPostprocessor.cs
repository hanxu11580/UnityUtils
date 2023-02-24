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
        //ģ�͵���֮ǰ����
        public void OnPreprocessModel() {
            //LogUtils.Log("Model����ǰ" + this.assetPath);
        }

        //ģ�͵���֮ǰ����
        public void OnPostprocessModel(GameObject go) {
            //LogUtils.Log("Model�����" + go.name);
        }

        //������֮ǰ���ã�����뵽�������������
        public void OnPreprocessTexture() {
            //Debug.Log("Texture����ǰ=" + this.assetPath);
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
        /// ����Ķ���Assets·��
        /// </summary>
        /// <param name="importedAsset"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        public static void OnPostprocessAllAssets(string[] importedAsset, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            LogUtils.Log($"������Դ����:{importedAsset.Length}", true);
            foreach (string path in importedAsset) {
                SetPlugnImporterByPath(path);
            }
            //foreach (string path in deletedAssets) {

            //}
        }

        /// <summary>
        /// ����·�����ò����ƽ̨ѡ��
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
                LogUtils.Log($"���� {path} only iOS platform");
            }
            else if (path.Contains(AssetPostprocessorConst.SubPathPluginsAndroid)) {
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.Android, true);
                LogUtils.Log($"���� {path} only Android platform");
            }
        }
    }
}
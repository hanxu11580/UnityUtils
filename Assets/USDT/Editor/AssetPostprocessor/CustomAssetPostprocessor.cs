using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using USDT.Utils;

namespace USDT.CustomEditor {
    public class CustomAssetPostprocessor : AssetPostprocessor {
        private static StringBuilder _SB = new StringBuilder();
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
            if(importedAsset.Length > 0) {
                _SB.Append($"������Դ����:{importedAsset.Length}\n\n");
                foreach (string path in importedAsset) {
                    _SB.Append(StringUtils.Format(ColorStringConst.GreenFormat, path));
                    _SB.Append("\n");
                    SetPlugnImporterByPath(path);
                }
                LogUtils.Log(_SB.ToString(), true);
            }
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
            if (path.Contains(CustomAssetPostprocessorConst.SubPathPluginsiOS)) {
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.iOS, true);
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.Android, false);
                LogUtils.Log($"���� {path} only iOS platform");
            }
            else if (path.Contains(CustomAssetPostprocessorConst.SubPathPluginsAndroid)) {
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.iOS, false);
                pluginsImporter.SetCompatibleWithPlatform(BuildTarget.Android, true);
                LogUtils.Log($"���� {path} only Android platform");
            }
        }
    }
}
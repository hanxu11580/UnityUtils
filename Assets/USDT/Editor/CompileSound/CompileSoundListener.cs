using UnityEditor.Compilation;

namespace USDT.CustomEditor.CompileSound {
    public class CompileSoundListener
    {
        public static void Init()
        {
            UpdateToggle();
            //#if UNITY_2020_2_OR_NEWER
            //            AssemblyReloadEvents.afterAssemblyReload += DidCompileFinished;
            //#else
            //            EditorApplication.didReloadScripts += DidCompileFinished;
            //#endif

            // 上面那个脚本有编译报错时候，不会执行，音乐会一直执行下去
            CompilationPipeline.compilationFinished += DidCompileFinished;
        }

        public static void DidCompileFinished(object obj)
        {
            if (CompileSoundSettings.UseWaitMusic)
                PlayerFactory.GetPlayer().CompileFinished();
        }


        #region CallBack Listeners

        private static void CompilationPipeline_assemblyCompilationStarted(object obj)
        {
            TriggerSoundPlay();
        }

        private static void Lightmapping_started()
        {
            TriggerSoundPlay();
        }

        #endregion

        private static void TriggerSoundPlay()
        {
            PlayerFactory.GetPlayer().Play();
        }

        /// <summary>
        /// 更新回调注册
        /// </summary>
        public static void UpdateToggle()
        {
            if (CompileSoundSettings.UseWaitMusic)
            {
                CompilationPipeline.compilationStarted += CompilationPipeline_assemblyCompilationStarted;
                //if (CompileSoundSettings.PlayOnLightMapping)
                //{
                //    Lightmapping.bakeStarted += Lightmapping_started;
                //    Lightmapping.bakeCompleted += DidCompileFinished;
                //}
            }
            else
            {
                CompilationPipeline.compilationStarted -= CompilationPipeline_assemblyCompilationStarted;
                //Lightmapping.bakeStarted -= Lightmapping_started;
                //Lightmapping.bakeCompleted -= DidCompileFinished;
            }

        }
    }

}
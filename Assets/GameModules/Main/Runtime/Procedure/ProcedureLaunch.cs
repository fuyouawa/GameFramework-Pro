using GameFramework.Localization;
using System;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

namespace GameMain.Runtime
{
    /// <summary>
    /// 流程 => 启动器。
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        protected override bool EnableAutoUpdateLoadingPhasesContext => false;
        protected override bool EnableAutoUpdateLoadingUISpinnerBox => false;

        protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
        {
            base.OnEnter(procedureOwner);

            // 语言配置：设置当前使用的语言，如果不设置，则默认使用操作系统语言
            InitLanguageSettings();

            // 声音配置：根据用户配置数据，设置即将使用的声音选项
            InitSoundSettings();
        }

        protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);

            ChangeState<ProcedureInitBuiltinPackage>(procedureOwner);
        }

        private void InitLanguageSettings()
        {
            bool isEditorMode = Application.platform == RuntimePlatform.WindowsEditor ||
                                Application.platform == RuntimePlatform.OSXEditor ||
                                Application.platform == RuntimePlatform.LinuxEditor;
            if (isEditorMode && GameEntry.Base.EditorLanguage != Language.Unspecified)
            {
                // 编辑器资源模式直接使用 Inspector 上设置的语言
                return;
            }

            Language language = GameEntry.Localization.Language;
            if (GameEntry.Setting.HasSetting(Constant.Setting.Language))
            {
                try
                {
                    string languageString = GameEntry.Setting.GetString(Constant.Setting.Language);
                    language = (Language)Enum.Parse(typeof(Language), languageString);
                }
                catch(Exception exception)
                {
                    Log.Error("Init language error, reason {0}",exception.ToString());
                }
            }

            if (language != Language.English
                && language != Language.ChineseSimplified
                && language != Language.ChineseTraditional
                && language != Language.Korean)
            {
                // 若是暂不支持的语言，则使用英语
                language = Language.English;

                GameEntry.Setting.SetString(Constant.Setting.Language, language.ToString());
                GameEntry.Setting.Save();
            }

            GameEntry.Localization.Language = language;
            Log.Debug("Init language settings complete, current language is '{0}'.", language.ToString());
        }

        private void InitSoundSettings()
        {
            GameEntry.Sound.Mute(Constant.SoundGroup.Music, GameEntry.Setting.GetBool(Constant.Setting.MusicMuted, false));
            GameEntry.Sound.SetVolume(Constant.SoundGroup.Music, GameEntry.Setting.GetFloat(Constant.Setting.MusicVolume, 0.5f));
            GameEntry.Sound.Mute(Constant.SoundGroup.Sound, GameEntry.Setting.GetBool(Constant.Setting.SoundMuted, false));
            GameEntry.Sound.SetVolume(Constant.SoundGroup.Sound, GameEntry.Setting.GetFloat(Constant.Setting.SoundVolume, 0.5f));
            GameEntry.Sound.Mute(Constant.SoundGroup.UISound, GameEntry.Setting.GetBool(Constant.Setting.UISoundMuted, false));
            GameEntry.Sound.SetVolume(Constant.SoundGroup.UISound, GameEntry.Setting.GetFloat(Constant.Setting.UISoundVolume, 0.5f));
            Log.Debug("Init sound settings complete.");
        }
    }
}

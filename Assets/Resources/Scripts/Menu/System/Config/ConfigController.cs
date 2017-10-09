using System;
using UnityEngine;

using UnityEngine.UI;
using ProjectWitch.Extention;

namespace ProjectWitch.Menu.System.Config
{
    public class ConfigController : MonoBehaviour {

        //各要素への参照
        [Header("GraphicsSettings")]

        [SerializeField]
        private Toggle mFullScreen = null;
        [SerializeField]
        private Dropdown mResolution = null;
        [SerializeField]
        private Dropdown mQuality = null;

        [Header("SoundSettings")]

        [SerializeField]
        private Slider mMasterVolume = null;
        [SerializeField]
        private Slider mBGMVolume = null;
        [SerializeField]
        private Slider mSEVolume = null;
        [SerializeField]
        private Slider mVoiceVolume = null;

        [Header("GamePlaySetting")]

        [SerializeField]
        private Slider mTalkSpeed = null;
        [SerializeField]
        private Slider mBattleSpeed = null;

        //内部
        private ConfigDataFormat mConfig = null;

        // Use this for initialization
        void Start() {
            mConfig = Game.GetInstance().SystemData.Config;

            mFullScreen.isOn = mConfig.IsFullScreen;
            mResolution.value = (int)mConfig.Resolution;
            mQuality.value = (int)mConfig.GraphicQuality;
            mMasterVolume.value = mConfig.MasterVolume;
            mBGMVolume.value = mConfig.BGMVolume;
            mSEVolume.value = mConfig.BGMVolume;
            mVoiceVolume.value = mConfig.VoiceVolume;
            mTalkSpeed.value = (int)mConfig.TextSpeed;
            mBattleSpeed.value = mConfig.BattleSpeed;
        }

        //各種コンポーネントのOnChangeイベントで呼び出すメソッド
        public void OnChangeFullScreen()
        {
            mConfig.IsFullScreen = mFullScreen.isOn;
        }

        public void OnChangeResolution()
        {
            mConfig.Resolution = 
                EnumConverter.ToEnum<ConfigDataFormat.ResolutionEnum>(mResolution.value);
        }

        public void OnChangeQuiality()
        {
            mConfig.GraphicQuality =
                EnumConverter.ToEnum<ConfigDataFormat.GraphicQualityEnum>(mQuality.value);
        }

        public void OnChangeMasterVolume()
        {
            mConfig.MasterVolume = mMasterVolume.value;
        }

        public void OnChangeBGMVolume()
        {
            mConfig.BGMVolume = mBGMVolume.value;
        }

        public void OnChangeSEVolume()
        {
            mConfig.SEVolume = mSEVolume.value;
        }

        public void OnChangeVoiceVolume()
        {
            mConfig.VoiceVolume = mVoiceVolume.value;
        }

        public void OnChangeTalkSpeed()
        {
            mConfig.TextSpeed = EnumConverter.ToEnum<ConfigDataFormat.TextSpeedEnum>
                ((int)mTalkSpeed.value);
        }

        public void OnChangeBattleSpeed()
        {
            mConfig.BattleSpeed = (int)mBattleSpeed.value;
        }
    }
}
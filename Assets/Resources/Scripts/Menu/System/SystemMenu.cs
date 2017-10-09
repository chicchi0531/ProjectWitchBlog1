using UnityEngine;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class SystemMenu : BaseMenu
    {
        //セーブパネルへの参照
        [SerializeField]
        private GameObject mSavePanel = null;

        //ロードパネルへの参照
        [SerializeField]
        private GameObject mLoadPanel = null;

        //コンフィグパネルへの参照
        [SerializeField]
        private GameObject mConfigPanel = null;
        
        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            mSavePanel.SetActive(false);
            mLoadPanel.SetActive(false);
            mConfigPanel.SetActive(false);
        }
        
        //セーブメニューを開く
        public void OpenSave()
        {
            mSavePanel.SetActive(true);
            mLoadPanel.SetActive(false);
            mConfigPanel.SetActive(false);
        }

        //ロードメニューを開く
        public void OpenLoad()
        {
            mSavePanel.SetActive(false);
            mLoadPanel.SetActive(true);
            mConfigPanel.SetActive(false);
        }

        //コンフィグメニューを開く
        public void OpenConfig()
        {
            mSavePanel.SetActive(false);
            mLoadPanel.SetActive(false);
            mConfigPanel.SetActive(true);
        }

    }
}
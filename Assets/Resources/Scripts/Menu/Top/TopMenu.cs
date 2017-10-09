using UnityEngine;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class TopMenu : MonoBehaviour
    {

        [SerializeField]
        private MenuController mMenuController = null;

        [Header("各メニューのキャンバス")]
        [SerializeField]
        private BaseMenu mArmy = null;
        [SerializeField]
        private BaseMenu mItem = null;
        [SerializeField]
        private BaseMenu mTips = null;
        [SerializeField]
        private BaseMenu mSystem = null;

        //内部変数
        private Animator mcAnim = null;

        // Use this for initialization
        void Start()
        {
            mcAnim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
        }

        //軍拡をクリック
        public void OnClickArmy()
        {
            mArmy.Open();
            Close();
        }

        //アイテムをクリック
        public void OnClickItem()
        {
            mItem.Open();
            Close();
        }

        //Tipsをクリック
        public void OnClickTips()
        {
            mTips.Open();
            Close();
        }

        //システムをクリック
        public void OnClickSystem()
        {
            mSystem.Open();
            Close();
        }

        public void Open()
        {
            mcAnim.SetBool("IsShow", true);
        }

        public void Close()
        {
            mcAnim.SetBool("IsShow", false);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class BasePurchaseMenu : BaseMenu
    {
        private Animator mAnim = null;

        //情報ウィンドウ
        [SerializeField]
        private BaseItemInfo mInfoWindow = null;

        //消費後マナウィンドウ
        [SerializeField]
        private NextManaWindow mNextMana = null;

        //リストへの参照
        [SerializeField]
        private BaseList mList = null;

        // Use this for initialization
        void Start()
        {
            mAnim = GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (mAnim.GetBool("IsShow") && Input.GetButtonDown("Cancel"))
            {
                Close();
            }
        }

        public void Reset()
        {
            mList.Reset();
        }

        public void Close()
        {

            mMesBox.SetText("", "");

            mInfoWindow.Close();
            mNextMana.Close();

            mTop.SetBool("IsShow", true);
            mAnim.SetBool("IsShow", false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class NextManaWindow : MonoBehaviour
    {
        //マイナス時の色
        [SerializeField]
        private Color mMinusColor = Color.red;

        [SerializeField]
        private Text mcText = null;

        private Animator mcAnimator = null;

        //デフォルトのカラー
        private Color mDefaultColor = Color.black;

        // Use this for initialization
        void Start()
        {
            mcAnimator = GetComponent<Animator>();
            mDefaultColor = mcText.color;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SetMana(int mana)
        {
            if (mana < 0) mcText.color = mMinusColor;
            else mcText.color = mDefaultColor;
            mcText.text = mana.ToString();
            Open();
        }

        public void Open()
        {
            mcAnimator.SetBool("IsShow", true);
        }

        public void Close()
        {
            mcAnimator.SetBool("IsShow", false);
        }
    }
}
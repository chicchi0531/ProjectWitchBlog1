using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu.System
{
    [RequireComponent(typeof(Text))]
    public class BattleSliderText : MonoBehaviour
    {
        //スライダーへの参照
        [SerializeField]
        private Slider mSlider = null;

        //テキストコンポーネントへの参照
        private Text mText = null;

        // Use this for initialization
        void Start()
        {
            mText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            int val = (int)mSlider.value;
            switch(val)
            {
                case 0:
                    mText.text = "x1";
                    break;
                case 1:
                    mText.text = "x2";
                    break;
                case 2:
                    mText.text = "x4";
                    break;
                default:
                    break;
            }
        }
    }
}
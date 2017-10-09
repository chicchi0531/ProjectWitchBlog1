using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu.System
{
    [RequireComponent(typeof(Text))]
    public class VolumeSliderText : MonoBehaviour
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
            int value = (int)(mSlider.value*100.0f);

            mText.text = value.ToString();
        }
    }
}
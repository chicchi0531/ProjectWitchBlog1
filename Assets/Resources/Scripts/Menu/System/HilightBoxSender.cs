using UnityEngine;
using UnityEngine.UI;
using ProjectWitch.Sys;

namespace ProjectWitch.Menu.System
{
    public class HilightBoxSender : MonoBehaviour
    {
        [SerializeField]
        private Image mHilightImage = null;

        [SerializeField]
        private SaveLoadContainerBase mContainer = null;

        [SerializeField]
        private HilightBoxReciever mReciever = null;

        [SerializeField]
        private int mIndex = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            //選択されているときはハイライトする
            if(mReciever.SelectedIndex==mIndex)
            {
                var color = mHilightImage.color;
                color.a = 1.0f;
                mHilightImage.color = color;
            }
            else
            {
                var color = mHilightImage.color;
                color.a = 0.0f;
                mHilightImage.color = color;
            }
        }

        public void SendIndex()
        {
            mReciever.SelectedIndex = mIndex;
            mReciever.Container = mContainer;
        }
    }
}
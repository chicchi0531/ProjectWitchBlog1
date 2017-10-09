using UnityEngine;
using UnityEngine.UI;
using ProjectWitch.Sys;

namespace ProjectWitch.Menu.System
{
    public class HilightBoxReciever : MonoBehaviour
    {
        //現在選択しているインデックス番号
        private int mSelectedIndex = -2;
        public int SelectedIndex { get { return mSelectedIndex; } set { mSelectedIndex = value; } }

        //インフォテキスト
        [SerializeField]
        protected Text mcText = null;

        //ＯＫボタン
        [SerializeField]
        protected Button mcButton = null;

        //コンテナへの参照
        private SaveLoadContainerBase mContainer = null;
        public SaveLoadContainerBase Container { get { return mContainer; } set { mContainer = value; } }
    }
}
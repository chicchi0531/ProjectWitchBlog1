using UnityEngine;

namespace ProjectWitch.Shop
{
    public class BaseList : MonoBehaviour
    {
        //リストのコンテンツへの参照
        [SerializeField]
        protected GameObject mListContent = null;

        //コンテンツの親
        [SerializeField]
        protected GameObject mListContentParent = null;

        //情報ウィンドウへの参照
        [SerializeField]
        protected BaseItemInfo mInfoWindow = null;

        //メッセージボックスへの参照
        [SerializeField]
        protected MessageBox mMessageBox = null;

        // Use this for initialization
        void Start()
        {
            Reset();
        }

        public virtual void Reset()
        {
            var game = Game.GetInstance();

            //子供を全削除
            foreach (Transform child in mListContentParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
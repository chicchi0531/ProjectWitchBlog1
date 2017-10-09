using System;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu.Tips
{

    //リストに格納するアイテムの情報
    [Serializable]
    public class ListItemInfo
    {
        //リストアイテムに表示するタイトル
        [SerializeField]
        private string mTitle = "";
        public string Title { get { return mTitle; } set { mTitle = value; } }

        //右の表示部分に表示するプレハブ
        [SerializeField]
        private GameObject mPrefab = null;
        public GameObject Prefab { get { return mPrefab; } set { mPrefab = value; } }

        //そのアイテムをリストに入れるかどうかの判定に使うフラグ
        [SerializeField]
        private int mFlag = -1;
        public int Flag { get { return mFlag; } set { mFlag = value; } }
    }

    //リストの制御クラス
    public class List : MonoBehaviour {

        //リストのデータを挿入する親
        [SerializeField]
        private GameObject mItemParent = null;

        //右側に表示するコンテンツの親
        [SerializeField]
        private GameObject mContentParent = null;

        //リストアイテムのプレハブ
        [SerializeField]
        private GameObject mItemPrefab = null;

        //スクロールバーへの参照
        [SerializeField]
        private Scrollbar mScrollbar = null;

        //リストデータ、このデータを元にリストへデータを入れていく
        [SerializeField]
        private ListItemInfo[] mListData = null;

        public void Init()
        {
            var game = Game.GetInstance();

            //現在あるデータを削除
            foreach(var item in mItemParent.GetComponentsInChildren<ListItem>())
            {
                Destroy(item.gameObject);
            }
            foreach(var item in mContentParent.GetComponentsInChildren<Content>())
            {
                Destroy(item.gameObject);
            }

            //リストにデータを格納
            foreach (var data in mListData)
            {
                if (data.Flag != -1)
                    if (game.GameData.Memory[data.Flag] == 0) continue;

                //生成
                var inst = Instantiate(mItemPrefab);
                inst.transform.SetParent(mItemParent.transform,false);

                //情報の格納
                var comp = inst.GetComponent<ListItem>();
                comp.ContentParent = mContentParent;
                comp.Title = data.Title;
                comp.Prefab = data.Prefab;
                comp.Init();
            }

            //スクロールバーを初期位置へ
            //mScrollbar.value = 1.0f;
        }
    }
}
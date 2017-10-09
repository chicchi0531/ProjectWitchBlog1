using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu.Tips
{
    public class ListItem : MonoBehaviour
    {
        //リストアイテムのタイトルを表示するテキストコンポーネント
        [SerializeField]
        private Text mcTitle = null;

        //コンテンツの親オブジェクト
        public GameObject ContentParent { get; set; }

        //リストアイテムのタイトル名
        private string mTitle = "タイトル名が入力されていません";
        public string Title { get { return mTitle; } set { mTitle = value; } }
        
        //生成するコンテンツのプレハブ
        public GameObject Prefab { get; set; }


        // Use this for initialization
        public void Init()
        {
            mcTitle.text = Title;
        }

        public void OnClicked()
        {
            //すでにあるコンテンツを削除
            foreach (var child in ContentParent.GetComponentsInChildren<Content>())
                Destroy(child.gameObject);

            //コンテンツを生成、表示
            var inst = Instantiate(Prefab);
            inst.transform.SetParent(ContentParent.transform, false);

        }
    }
}
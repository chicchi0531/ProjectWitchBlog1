using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace ProjectWitch.Menu.Item
{
    public class List : MonoBehaviour
    {
        [SerializeField]
        private bool mIsEquipment = true;
        public bool IsEquipment { get { return mIsEquipment; } set { mIsEquipment = value; } }

        [SerializeField]
        private GameObject mListItemPrefab = null;

        [SerializeField]
        private GameObject mListContents = null;

        [SerializeField]
        private EquipmentInfo mEquipmentInfo = null;
        [SerializeField]
        private CardInfo mCardInfo = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        public void Init()
        {
            //すでにある子どもを削除
            var children = mListContents.GetComponentsInChildren<ListItem>();
            foreach (var child in children) Destroy(child.gameObject);

            var game = Game.GetInstance();
            //装備の場合
            if(IsEquipment)
            {
                var count = game.GameData.Territory[0].EquipmentList.Count;
                for (int i=0; i< count;i++)
                {
                    //装備品は1以上所持している場合に続行
                    if (game.GameData.Territory[0].EquipmentList[i].Count == 0) continue;

                    //リストアイテムの生成
                    var inst = Instantiate(mListItemPrefab);
                    inst.transform.SetParent(mListContents.transform, false);
                    var comp = inst.GetComponent<ListItem>();
                    comp.IsEquipment = IsEquipment;
                    comp.ID = i;
                    comp.mInfo = (BaseInfo)mEquipmentInfo;
                    comp.Init();
                }
            }

            //カードの場合
            else
            {
                var groupID = game.GameData.Territory[0].GroupList[0];
                var list = game.GameData.Group[groupID].CardList.Distinct().ToList(); //重複を削除
                foreach(var id in list)
                {
                    //リストアイテムの生成
                    var inst = Instantiate(mListItemPrefab);
                    inst.transform.SetParent(mListContents.transform, false);
                    var comp = inst.GetComponent<ListItem>();
                    comp.IsEquipment = IsEquipment;
                    comp.ID = id;
                    comp.mInfo = (BaseInfo)mCardInfo;
                    comp.Init();
                }
            }

            
        }
    }
}
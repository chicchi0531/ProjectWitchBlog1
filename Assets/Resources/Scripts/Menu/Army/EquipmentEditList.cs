using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Menu
{
    public class EquipmentEditList : MonoBehaviour
    {

        private List<EquipmentEditListItem> mList = null;

        //コンテンツ
        [SerializeField]
        private GameObject mItemPrefab = null;

        //コンテンツの親オブジェクト
        [SerializeField]
        private Transform mContentParent = null;

        //ウィンドウ本体への参照
        [SerializeField]
        private EquipmentEditWindow mEquipmentEditWindow = null;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        public void ItemUpdate()
        {
            foreach(var item in mList)
            {
                item.Reset();
            }
        }

        public void Reset()
        {
            var game = Game.GetInstance();
            var list = game.GameData.Territory[0].EquipmentList;

            //作成済みの装備を削除
            if (mList != null)
            {
                foreach (var item in mList)
                    Destroy(item.gameObject);
            }

            mList = new List<EquipmentEditListItem>();

            //装備なしを追加する
            var inst = Instantiate(mItemPrefab);
            inst.transform.SetParent(mContentParent, false);
            var comp = inst.GetComponent<EquipmentEditListItem>();
            comp.ItemID = -1;
            comp.Window = mEquipmentEditWindow;
            comp.Reset();
            mList.Add(comp);

            //アイテムを追加
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Count == 0) continue;

                inst = Instantiate(mItemPrefab);
                inst.transform.SetParent(mContentParent,false);
                comp = inst.GetComponent<EquipmentEditListItem>();
                comp.ItemID = i;
                comp.Window = mEquipmentEditWindow;
                comp.Reset();

                mList.Add(comp);
            }
        }
    }
}
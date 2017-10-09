using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ProjectWitch.Menu
{
    [RequireComponent(typeof(Button))]
    public class EquipmentEditListItem : MonoBehaviour
    {

        //ID
        public int ItemID { get; set; }

        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mNum = null;

        public EquipmentEditWindow Window { get; set; }

        //内部コンポーネント
        private Button mcButton = null;

        //所持数の内部ストック
        private int mNumAsInteger = 0;

        // Use this for initialization
        void Start()
        {
            mcButton = GetComponent<Button>();
        }

        void Update()
        {
            if (mNumAsInteger == 0 && ItemID != -1)
                mcButton.interactable = false;
            else
                mcButton.interactable = true;
        }

        public void Reset()
        {
            if (ItemID == -1)
            {
                mName.text = "なし";
                mNum.text = "";
            }
            else
            {
                var game = Game.GetInstance();
                var item = game.GameData.Equipment[ItemID];
                var list = game.GameData.Territory[0].EquipmentList[ItemID];

                //装備者数を抜いた数を所持数とする
                var num = list.Count(x => x == -1);
                mNumAsInteger = num;

                mName.text = item.Name;
                mNum.text = num.ToString();
            }
        }

        public void OnClick()
        {
            Window.ItemID = ItemID;
            Window.Reset();
        }
    }
}
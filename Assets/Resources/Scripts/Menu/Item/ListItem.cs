using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ProjectWitch.Menu.Item
{
    public class ListItem : MonoBehaviour
    {
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mNum = null;
        
        public bool IsEquipment { get; set; }

        [SerializeField]
        private int mID = -1;
        public int ID { get { return mID; } set { mID = value; } }

        public BaseInfo mInfo { get; set; }

        // Update is called once per frame
        public void Init()
        {
            if (ID < 0) return;

            var game = Game.GetInstance();
            if(IsEquipment)
            {
                mName.text = game.GameData.Equipment[ID].Name;
                mNum.text = game.GameData.Territory[0].EquipmentList[ID].Count.ToString();
            }
            else
            {
                mName.text = game.GameData.Card[ID].Name;
                var group = game.GameData.Group[game.GameData.Territory[0].GroupList[0]];
                mNum.text = group.CardList.Where(x => x == ID).ToArray().Length.ToString();
            }
        }

        public void OnClicked()
        {
            mInfo.ID = ID;
            mInfo.Init();
        }
    }
}
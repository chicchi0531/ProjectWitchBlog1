using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop.Tool
{
    public class BuyItem : BaseShopItem
    {
        //所持数
        [SerializeField]
        private Text mNum = null;

        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];
            var itemList = game.GameData.Territory[0].EquipmentList;

            //名前をセット
            mName.text = item.Name;

            //個数をセット
            var num = itemList[ItemID].Count;
            mNum.text = num.ToString();

            //価格をセット
            var price = game.GameData.Equipment[ItemID].BuyingPrice;
            mPrice.text = price.ToString();
        }
    }
}
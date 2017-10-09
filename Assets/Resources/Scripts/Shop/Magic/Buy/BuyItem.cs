using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace ProjectWitch.Shop.Magic
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
            var item = game.GameData.Card[ItemID];
            var itemList = game.GameData.Group[game.GameData.Territory[0].GroupList[0]].CardList;

            //名前をセット
            mName.text = item.Name;

            //個数をセット
            var num = (itemList.Where(x => x == ItemID).ToList()).Count;
            mNum.text = num.ToString();

            //価格をセット
            var price = game.GameData.Card[ItemID].BuyingPrice;
            mPrice.text = price.ToString();
        }
    }
}
using UnityEngine.UI;
using UnityEngine;

namespace ProjectWitch.Shop.Tool
{
    public class SellItem : BaseShopItem
    {
        //プレイヤーが所持するアイテムの識別用ID(複数のアイテムIDを所持するための識別用)
        public int ItemUniID { get; set; }

        //所持数
        [SerializeField]
        private Text mIsEquipment = null;

        public override void Reset()
        {
            base.Reset();

            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];
            var itemList = game.GameData.Territory[0].EquipmentList;
  
            //誰かが装備していないかチェック
            var isEquipment = itemList[ItemID][ItemUniID] == -1 ? false : true;
            if (isEquipment) mIsEquipment.text = "E";
            else mIsEquipment.text = "";

            //名前をセット
            mName.text = item.Name;

            //価格をセット
            var price = game.GameData.Equipment[ItemID].SellingPrice;
            mPrice.text = price.ToString();
        }

        public override void OnClicked()
        {
            var game = Game.GetInstance();
            
            //売却商品情報ウィンドウに装備しているユニットのIDをセット
            var unitID = game.GameData.Territory[0].EquipmentList[ItemID][ItemUniID];
            var infoWindow = (SellItemInfo)InfoWindow;
            infoWindow.UnitID = unitID;

            base.OnClicked();
        }
    }
}
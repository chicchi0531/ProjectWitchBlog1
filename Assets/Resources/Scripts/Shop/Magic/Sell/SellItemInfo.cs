using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop.Magic
{
    [RequireComponent(typeof(Animator))]
    public class SellItemInfo : MagicBaseItemInfo
    {
        public override void Reset()
        {
            if (ItemID != -1)
            {
                var game = Game.GetInstance();
                var item = game.GameData.Card[ItemID];

                var mana = game.GameData.PlayerMana + item.SellingPrice;
                mNextManaWindow.SetMana(mana);

                //文字列にアイテム名を差し込む
                var messageA = mMessageA.Replace("[0]", item.Name);
                messageA = messageA.Replace("[1]", item.SellingPrice.ToString());

                mMessageBox.SetText(mMesNameA, messageA);
            }

            base.Reset();
        }

        public void ClickSellButton()
        {
            var game = Game.GetInstance();
            var item = game.GameData.Card[ItemID];

            //マナを増やす
            game.GameData.PlayerMana += item.SellingPrice;

            //所持カードリストから除外する
            game.GameData.Group[game.GameData.Territory[0].GroupList[0]].CardList.Remove(ItemID);

            //メッセージを表示
            mMessageBox.SetText(mMesNameB, mMessageB);

            //データをリセット
            mList.Reset();

            Close();
        }
    }
}
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop.Tool
{
    [RequireComponent(typeof(Animator))]
    public class SellItemInfo : ToolBaseItemInfo
    {
        //装備しているユニットID
        public int UnitID { get; set; }

        //強制的に売るボタンを含んだゲームオブジェクト
        [SerializeField]
        private GameObject mForceSellButtonPanel = null;

        public override void Reset()
        {
            if (ItemID != -1)
            {
                var game = Game.GetInstance();
                var item = game.GameData.Equipment[ItemID];

                var mana = game.GameData.PlayerMana + item.SellingPrice;
                mNextManaWindow.SetMana(mana);

                //文字列にアイテム名を差し込む
                var messageA = mMessageA.Replace("[0]", item.Name);
                messageA = messageA.Replace("[1]", item.SellingPrice.ToString());

                mMessageBox.SetText(mMesNameA, messageA);

                //強制売却ボタンを非表示にする
                mForceSellButtonPanel.SetActive(false);
            }

            base.Reset();
        }

        public override void Close()
        {
            mForceSellButtonPanel.SetActive(false);
            base.Close();
        }

        public void ClickSellButton()
        {
            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];

            //装備者がいるかいないかで挙動を変更
            if (UnitID != -1)
            {
                var unit = game.GameData.Unit[UnitID];

                //メッセージを変換
                var messageC = mMessageC.Replace("[0]", item.Name);
                messageC = messageC.Replace("[1]", unit.Name);

                //メッセージを表示
                mMessageBox.SetText(mMesNameC, messageC);

                //強制売却ボタンを表示
                mForceSellButtonPanel.SetActive(true);
            }
            else
            {
                SellItem();
            }
        }

        //装備済みの装備を売る際の最終確認
        public void ClickForceSellButton()
        {
            //売却するプレイヤーから装備を外す
            var game = Game.GetInstance();
            if (UnitID != -1)
            {
                game.GameData.Unit[UnitID].ChangeEquipment(-1);
            }

            //アイテムを売却
            SellItem();
        }

        //最終確認をキャンセルした場合
        public void ClickForceSellCancelButton()
        {
            mMessageBox.SetText(mMesNameA, mMessageA);
            mForceSellButtonPanel.SetActive(false);
        }


        //アイテムを売却したときの処理
        private void SellItem()
        {
            var game = Game.GetInstance();
            var item = game.GameData.Equipment[ItemID];

            //マナを増やす
            game.GameData.PlayerMana += item.SellingPrice;

            //装備品リストから除外する
            game.GameData.Territory[0].EquipmentList[ItemID].Remove(-1);

            //メッセージを表示
            mMessageBox.SetText(mMesNameB, mMessageB);

            //データをリセット
            mList.Reset();

            Close();
        }
    }
}
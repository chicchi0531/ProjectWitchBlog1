using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWitch.Shop.Tool
{
    public class TalkMenu_Tool : TalkMenu
    {
        //装備品のID
        private int mEquipmentID = -1;

        //トークシーンの開始
        public override void Begin()
        {
            //装備をランダムで選択
            var game = Game.GetInstance();
            var list = new List<int>();
            for (int i = 0; i < game.GameData.Equipment.Count; i++)
            {
                list.Add(i);
            }

            mEquipmentID = list[UnityEngine.Random.Range(0, list.Count-1)];

            StartCoroutine(_TalkProcess());
        }

        //OKクリック後の処理
        protected override IEnumerator _ClickOK()
        {
            mChoicePanel.SetActive(false);
            yield return null;

            //所持金が足りるか足りないかでメッセージ変更
            var gamedata = Game.GetInstance().GameData;
            var equipment = gamedata.Equipment[mEquipmentID];
            ShopMessage[] exeMessage = new ShopMessage[1];
            if (gamedata.PlayerMana >= (int)(equipment.BuyingPrice * mDiscountRateB))
            {
                exeMessage = mMes_OK;

                //所持金を減らす処理
                gamedata.PlayerMana -= (int)(equipment.BuyingPrice * mDiscountRateB);

                //購入処理
                gamedata.Territory[0].EquipmentList[mEquipmentID].Add(-1);
            }
            else
            {
                exeMessage = mMes_OK_NoMoney;
            }

            //メッセージを表示
            foreach (var mes in exeMessage)
            {
                yield return StartCoroutine(_ShowText(mes));
            }

            End();
            yield return null;
        }

        //テキストの中に含まれるタグを置換
        protected override string ReplaceMessage(string message)
        {
            //装備データを取得
            var equipment = Game.GetInstance().GameData.Equipment[mEquipmentID];

            //文字列を置換
            var outstr = message.Replace(mTag_ItemName, equipment.Name);
            outstr = outstr.Replace(mTag_ItemPriceA, equipment.BuyingPrice.ToString());
            outstr = outstr.Replace(mTag_ItemPriceB, ((int)(equipment.BuyingPrice * mDiscountRateA)).ToString());
            outstr = outstr.Replace(mTag_ItemPriceC, ((int)(equipment.BuyingPrice * mDiscountRateB)).ToString());

            return outstr;
        }
    }
}
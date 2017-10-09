using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWitch.Shop.Magic
{
    public class TalkMenu_Magic : TalkMenu
    {
        //装備品のID
        private int mCardID = -1;

        //トークシーンの開始
        public override void Begin()
        {
            //装備をランダムで選択
            var game = Game.GetInstance();
            var list = new List<int>();
            for (int i = 0; i < game.GameData.Card.Count; i++)
            {
                list.Add(i);
            }

            mCardID = list[UnityEngine.Random.Range(0, list.Count-1)];

            StartCoroutine(_TalkProcess());
        }

        //OKクリック後の処理
        protected override IEnumerator _ClickOK()
        {
            mChoicePanel.SetActive(false);
            yield return null;

            //所持金が足りるか足りないかでメッセージ変更
            var gamedata = Game.GetInstance().GameData;
            var card = gamedata.Card[mCardID];
            ShopMessage[] exeMessage = new ShopMessage[1];
            if (gamedata.PlayerMana >= (int)(card.BuyingPrice * mDiscountRateB))
            {
                exeMessage = mMes_OK;

                //所持金を減らす処理
                gamedata.PlayerMana -= (int)(card.BuyingPrice * mDiscountRateB);

                //購入処理
                gamedata.Group[gamedata.Territory[0].GroupList[0]].CardList.Add(mCardID);
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
            var card = Game.GetInstance().GameData.Card[mCardID];

            //文字列を置換
            var outstr = message.Replace(mTag_ItemName, card.Name);
            outstr = outstr.Replace(mTag_ItemPriceA, card.BuyingPrice.ToString());
            outstr = outstr.Replace(mTag_ItemPriceB, ((int)(card.BuyingPrice * mDiscountRateA)).ToString());
            outstr = outstr.Replace(mTag_ItemPriceC, ((int)(card.BuyingPrice * mDiscountRateB)).ToString());

            return outstr;
        }
    }
}
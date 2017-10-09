using UnityEngine;
using System.Collections;
using System;

namespace ProjectWitch.Shop
{
    public class TalkMenu : BaseMenu
    {
        //最初の値引き率
        [SerializeField]
        protected float mDiscountRateA = 0.9f;

        //最終的な値引き率
        [SerializeField]
        protected float mDiscountRateB = 0.5f;

        //選択肢ウィンドウへの参照
        [SerializeField]
        protected GameObject mChoicePanel = null;

        //選択肢までのセリフ
        [SerializeField]
        private ShopMessage[] mMessages = new ShopMessage[10];

        [Space(30)]

        //選択肢に買うと答えた場合
        [SerializeField]
        protected ShopMessage[] mMes_OK = new ShopMessage[10];

        //選択肢に買うと答えた場合で、お金が足りなかった場合
        [SerializeField]
        protected ShopMessage[] mMes_OK_NoMoney = new ShopMessage[10];

        [Space(20)]
        
        //選択肢に買わないと答えた場合
        [SerializeField]
        protected ShopMessage[] mMes_NO = new ShopMessage[10];

        //置換する文字列
        protected string mTag_ItemName = "[name]";
        protected string mTag_ItemPriceA = "[priceA]";
        protected string mTag_ItemPriceB = "[priceB]";
        protected string mTag_ItemPriceC = "[priceC]";
        
        //トークシーンの開始
        public virtual void Begin()
        {
        }

        public void End()
        {
            mMesBox.SetText("", "");
            mTop.SetBool("IsShow", true);
        }

        //選択肢前までのメッセージ表示
        protected virtual IEnumerator _TalkProcess()
        {
            //会話の開始

            //選択肢までの会話
            foreach (var mes in mMessages)
            {
                yield return StartCoroutine(_ShowText(mes));
            }
            mChoicePanel.SetActive(true);
            yield return null;
        }

        //OKクリック後の処理
        public void Click_OK()
        {
            StartCoroutine(_ClickOK());
        }
        protected virtual IEnumerator _ClickOK()
        {
            yield return null;
        }

        //NOクリック後の処理
        public void Click_NO()
        {
            StartCoroutine(_ClickNO());
        }
        private IEnumerator _ClickNO()
        {
            mChoicePanel.SetActive(false);
            yield return null;

            foreach (var mes in mMes_NO)
            {
                yield return StartCoroutine(_ShowText(mes));
            }

            End();
            yield return null;
        }
        

        //テキストの中に含まれるタグを置換
        protected virtual string ReplaceMessage(string message)
        {
            return "";
        }

        //テキストを終端まで読み出し
        protected IEnumerator _ShowText(ShopMessage mes)
        {
            //テキストを置換してセット
            var message = ReplaceMessage(mes.Message);
            mMesBox.SetText(mes.Name, message);

            //テキストの表示終了待ち
            while (mMesBox.TextState == MessageBox.State.Active) yield return null;
            
            //キー入力待ち
            while (Input.GetButtonDown("TalkNext")) yield return null; //押し直すまでウェイト
            while (!Input.GetButtonDown("TalkNext")) yield return null;
        }
    }

    //名前とメッセージを一つにしたクラス
    [Serializable]
    public class ShopMessage
    {
        [SerializeField]
        private string mName = "";
        [SerializeField,Multiline]
        private string mMessage = "";

        public string Name { get { return mName; } set { mName = value; } }
        public string Message { get { return mMessage; } set { mMessage = value; } }
    }
}
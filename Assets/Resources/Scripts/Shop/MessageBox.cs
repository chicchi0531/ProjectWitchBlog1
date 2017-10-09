using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    public class MessageBox : MonoBehaviour {

        //名前オブジェクト
        [SerializeField]
        private Text mName = null;

        //メッセージ表示オブジェクト
        [SerializeField]
        private Text mMessage = null;

        //文字送りアイコン
        [SerializeField]
        private GameObject mNextIcon = null;

        //すべてのテキスト
        private string mAllText = "";

        //表示開始からの時間
        private float mTime = 0.0f;

        //現在の状態
        public enum State
        {
            Active, //文字送り中
            Wait,   //クリック待ち
        }
        public State TextState { get; set; }

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            mTime += Time.deltaTime;

            //テキストスピードを取得
            var game = Game.GetInstance();
            var speed = ConfigDataFormat.TextSpeedValues[game.SystemData.Config.TextSpeed];

            //表示する文字数
            int numLetter = (int)Mathf.Min(speed * mTime, mAllText.Length);

            //テキストセット
            mMessage.text = mAllText.Substring(0, numLetter);

            if(mAllText.Length == numLetter && mAllText.Length > 0)
            {
                TextState = State.Wait;
                mNextIcon.SetActive(true);
            }
        }

        //テキストをセットしなおす
        public void SetText(string name, string mes)
        {
            mName.text = name;
            mAllText = mes;
            mTime = 0.0f;
            TextState = State.Active;
            mNextIcon.SetActive(false);
        }
    }
}
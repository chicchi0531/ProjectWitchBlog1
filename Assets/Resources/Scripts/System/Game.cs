using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.Analytics;


namespace ProjectWitch
{
    public class Game : MonoBehaviour
    {

        #region ゲームデータ関連

        //実行中のゲーム内データ
        [SerializeField]
        private GameData mGameData = null;
        public GameData GameData { get { return mGameData; } set { mGameData = value; } }
        
        //アプリケーション全体のシステムデータ
        public SystemData SystemData { get; set; }

        #endregion


        //Singleton
        private static Game mInst;
        public static Game GetInstance()
        {
            if (mInst == null)
            {
                GameObject gameObject = Resources.Load("Prefabs/System/Game") as GameObject;
                mInst = Instantiate(gameObject).GetComponent<Game>();
            }
            return mInst;
        }
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (mInst == null)
            {
                this.Setup();
                mInst = this;
            }
            else if (mInst != this)
            {
                Destroy(this.gameObject);
            }
        }

        //初期化処理
        public void Setup()
        {
            //ゲームデータ初期化
            GameData.Reset();

            //システムデータ初期化
            SystemData = new SystemData();
            SystemData.Reset();
            SystemData.Load();
            
        }
 


    }
}
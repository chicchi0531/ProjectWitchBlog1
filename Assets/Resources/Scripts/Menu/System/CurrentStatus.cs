using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu.System
{
    public class CurrentStatus : MonoBehaviour
    {
        [SerializeField]
        private Text mTimeStamp = null;

        [SerializeField]
        private Text mArea = null;

        [SerializeField]
        private Text mLevel = null;

        [SerializeField]
        private Text mTurn = null;

        private bool mIsRunning = false;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();


        }

        // Update is called once per frame
        void Update()
        {
            if(!mIsRunning)
            {
                //1秒に一回実行
                StartCoroutine(_Update());
            }
        }

        private IEnumerator _Update()
        {
            mIsRunning = true;

            var game = Game.GetInstance();
            game.GameData.Meta.Update();
            var meta = game.GameData.Meta;

            //time
            var y = meta.Year.ToString();
            var m = string.Format("{0:D2}", meta.Month);
            var d = string.Format("{0:D2}", meta.Day);
            var h = string.Format("{0:D2}", meta.Hour);
            var min = string.Format("{0:D2}", meta.Minute);
            mTimeStamp.text = y + "/" + m + "/" + d + "  " + h + ":" + min;

            //占領領地
            mArea.text = "占領済み領地 : " + meta.DominatedTerritory.ToString();

            //レベル
            mLevel.text = "レベル : " + meta.Level.ToString();

            //経過ターン数
            mTurn.text = meta.Turn.ToString() + "ターン目";

            yield return new WaitForSeconds(1.0f);
            mIsRunning = false;
            yield return null;
        }
    }
}
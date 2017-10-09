using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

namespace ProjectWitch.Menu
{

    public class InfoBoxText : MonoBehaviour
    {

        //各情報のテキストコンポーネント
        [SerializeField]
        private Text mcMana = null;
        [SerializeField]
        private Text mcSoldier = null;
        [SerializeField]
        private Text mcArea = null;
        [SerializeField]
        private Text mcUnit = null;

        private bool isRunning = false;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (!isRunning)
                StartCoroutine(_Update());
        }

        IEnumerator _Update()
        {
            var game = Game.GetInstance();

            isRunning = true;

            //所持マナ
            TextSetter(mcMana, game.GameData.PlayerMana.ToString());

            //ユニット
            var groupID = game.GameData.Territory[0].GroupList[0];
            var groupData = game.GameData.Group[groupID];
            TextSetter(mcUnit, groupData.UnitList.Count.ToString());

            //所持領地
            TextSetter(mcArea,
                game.GameData.Territory[0].AreaList.Count.ToString() + "/"
                + (game.GameData.Area.Count - 1).ToString());

            //総兵力
            var soldireNumList =
                from p in groupData.UnitList
                select game.GameData.Unit[p].SoldierNum;

            int sumSoldire = 0;
            foreach (var soldireNum in soldireNumList)
            {
                sumSoldire += soldireNum;
            }

            TextSetter(mcSoldier, sumSoldire.ToString());

            isRunning = false;
            yield return new WaitForSeconds(1.0f);
        }

        private void TextSetter(Text comp, string str)
        {
            if (comp)
                comp.text = str;
            else
                Debug.Log("テキストコンポーネントの接続がきれています");
        }
    }
}
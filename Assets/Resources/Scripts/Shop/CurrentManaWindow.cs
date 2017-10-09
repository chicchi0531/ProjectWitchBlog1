using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    public class CurrentManaWindow : MonoBehaviour
    {
        [SerializeField]
        private Text mcText = null;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();
            mcText.text = game.GameData.PlayerMana.ToString();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class ShopController : MonoBehaviour
    {
        //シーン名
        [SerializeField]
        private string mSceneName = "";

        private Animator mcAnimator = null;

        void Start()
        {
            var game = Game.GetInstance();
            mcAnimator = GetComponent<Animator>();
        }

    }
}
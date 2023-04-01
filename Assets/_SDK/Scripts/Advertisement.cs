using InstantGamesBridge;
using InstantGamesBridge.Modules.Advertisement;
using UnityEngine;

namespace SDK
{
    /// <summary>
    /// Реклама для ВК и Яндекс игр
    /// </summary>
    public class Advertisement: MonoBehaviour
    {
        [Header("All about interstitial video")]
        [SerializeField] private bool showInterstitial = false; // Показывать ли вообще interstitial видео
        [SerializeField] private int delaysSecondsVK = 0; // Задержка между рекламами в ВК (секунды)
        [SerializeField] private int delaySecondsYG = 0; // Задержка между рекламами в Яндекс Играх (секунды)
        [SerializeField] private bool ignoreDelayIntVK = false; // Игнорировать задержку между рекламами в ВК
        [SerializeField] private bool ignoreDelayIntYG = false; // Игнорировать задержку между рекламами в Яндекс играх

        [Header("All about rewarded video")]
        [SerializeField] private bool showRewarded = false; // Показывать ли вообще rewarded видео
        
        /// <summary>
        /// Вернет состояние rewarded видео
        /// </summary>
        /// <returns>По дефолту вернет Closed</returns>
        public RewardedState GetStateRewarded()
        {
            RewardedState stateRewardedVideo = RewardedState.Closed;
            Bridge.advertisement.rewardedStateChanged += state => { stateRewardedVideo = state; };
            return stateRewardedVideo;
        }
        
        /// <summary>
        /// Вернет состояние interstitial видео
        /// </summary>
        /// <returns>По дефолту вернет Closed</returns>
        public InterstitialState GetStateInterstitial()
        {
            InterstitialState stateInterstitialVideo = InterstitialState.Closed;
            Bridge.advertisement.interstitialStateChanged += state => { stateInterstitialVideo = state; };
            return stateInterstitialVideo;
        }

        /// <summary>
        /// Показывает interstitial видео
        /// </summary>
        public void ShowInterstitial()
        {
            if (showInterstitial)
            {
                Bridge.advertisement.SetMinimumDelayBetweenInterstitial(
                    new SetMinimumDelayBetweenInterstitialVkOptions(delaysSecondsVK),
                    new SetMinimumDelayBetweenInterstitialYandexOptions(delaySecondsYG));
                
                Bridge.advertisement.ShowInterstitial(
                    new ShowInterstitialVkOptions(ignoreDelayIntVK),
                    new ShowInterstitialYandexOptions(ignoreDelayIntYG));
            }
            
        }
        
        /// <summary>
        /// Показывает rewarded видео
        /// </summary>
        public void ShowRewarded()
        {
            if (showRewarded)
            {
                Bridge.advertisement.ShowRewarded();
            }
        }
    }
}

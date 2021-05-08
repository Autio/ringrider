using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAdScript : MonoBehaviour
{
    public string gameId = "1696406";
    public string placementId = "LevelBanner";
    public bool testMode = true;

    void Start()
    {
        Advertisement.Initialize (gameId, testMode);
        Advertisement.Banner.SetPosition (BannerPosition.TOP_CENTER);
        StartCoroutine (ShowBannerWhenReady ());
    }
    IEnumerator ShowBannerWhenReady () {
        while (!Advertisement.IsReady (placementId)) {
             yield return new WaitForSeconds (0.5f);
        }
        yield return new WaitForSeconds (0.1f);
        Advertisement.Banner.Show (placementId);
    }

}

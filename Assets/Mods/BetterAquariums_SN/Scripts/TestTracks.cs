using UnityEngine;

namespace DaftAppleGames.BetterAquariums_SN
{
    public class TestTracks : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Awake()
        {
            BetterAquariumHelper helper = GetComponent<BetterAquariumHelper>();
            foreach (BetterAquariumHelper.FishTrackContainer container in helper.ExistingContainer)
            {
                Debug.Log(container.ToString());
            }
        }
    }
}
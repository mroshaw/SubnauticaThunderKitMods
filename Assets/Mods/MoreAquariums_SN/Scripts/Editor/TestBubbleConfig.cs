using UnityEngine;

namespace DaftAppleGames.MoreAquariums.Editor
{
    public class TestBubbleConfig : MonoBehaviour
    {
        private void OnEnable()
        {
            DeskAquarium.PostConfigAction(gameObject);
        }
    }
}
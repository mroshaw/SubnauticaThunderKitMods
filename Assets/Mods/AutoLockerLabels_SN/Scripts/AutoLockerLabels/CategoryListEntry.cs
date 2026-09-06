using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DaftAppleGames.AutoLockerLabels_SN.AutoLockerLabels
{
    public sealed class CategoryListEntry : MonoBehaviour
    {
        [SerializeField] private Button selectButton;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text statusText;
        private int index;
        private Action<int> selected;

        internal void Bind(int categoryIndex, string categoryName, string status, Action<int> callback)
        {
            index = categoryIndex;
            selected = callback;
            nameText.text = categoryName;
            statusText.text = status;
            selectButton.onClick.RemoveListener(Select);
            selectButton.onClick.AddListener(Select);
        }

        private void Select()
        {
            if (selected != null)
            {
                selected(index);
            }
        }
    }
}

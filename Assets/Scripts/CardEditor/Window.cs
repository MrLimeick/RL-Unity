using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RL.CardEditor
{
    [AddComponentMenu("RL/GameEditor/Window")]
    [RequireComponent(typeof(UI.UI))]
    public class Window : MonoBehaviour
    {
        public Page.AdditionalWindows ParentPage;
        public UI.UI UI;
        public bool IsOpen { get ; private set; }

        public UnityEngine.UI.Button OpenCloseButton;

        public void Awake()
        {
            IsOpen = true;
            UI = gameObject.GetComponent<UI.UI>();
            if (OpenCloseButton != null) OpenCloseButton.onClick.AddListener(OpenCloseToggle);
        }
        public void Open()
        {
            if (ParentPage is not null) ParentPage.AllClose = false;
            IsOpen = true;
            UI.Show();
        }
        public void OpenAsync()
        {
            if (ParentPage is not null) ParentPage.AllClose = false;
            IsOpen = true;
            UI.ShowAsync();
        }
        public void Close()
        {
            IsOpen = false;
            UI.Hide();
        }
        public void CloseAsync()
        {
            IsOpen = false;
            UI.HideAsync();
        }

        public void OpenCloseToggle()
        {
            IsOpen = !IsOpen;
            if (IsOpen) Open();
            else Close();
        }
    }
}

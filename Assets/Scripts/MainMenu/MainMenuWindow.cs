using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RL.MainMenu
{
    public class MainMenuWindow : RLScript
    {
        /// <summary>
        /// Оригигальный цвет окна
        /// </summary>
        public Color OriginalColor;
        private Color m_Color;
        /// <summary>
        /// Цвет окна на данный момент
        /// </summary>
        public Color Color
        {
            get => m_Color;
            set  
            {
                /*GetComponent<Image>().ColorAnim(value,0.5f);*/
                m_Color = value;
            }
        }
        /// <summary>
        /// Открытое на данный момент
        /// </summary>
        /// <remarks>Равно NULL если все окна на данный момент закрыты</remarks>
        public static MainMenuWindow OpennedWindow;
        /// <summary>
        /// Дополнительное ли это окно
        /// </summary>
        public bool AdditionalWindow = false;
        /// <summary>
        /// Открыто ли это окно в данный момент
        /// </summary>
        public bool IsOpen;
        /// <summary>
        /// Открыть данное окно
        /// </summary>
        public async void Open() => await OpenAsync();
        /// <summary>
        /// Открыть данное окно
        /// </summary>
        /// <returns></returns>
        public async Task OpenAsync()
        {
            IsOpen = true;
            if (OpennedWindow == this && !AdditionalWindow) return;
            if (OpennedWindow != null && !AdditionalWindow) await OpennedWindow.CloseAsync();
            if (!AdditionalWindow) OpennedWindow = this;
            MainMenuController.Instance.windows.MainMenu.Color = OriginalColor;
            await OpenAnimation();
        }
        /// <summary>
        /// Закрыть данное окно
        /// </summary>
        public async void Close() => await CloseAsync();
        /// <summary>
        /// Закрыть данное окно
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            IsOpen = false;
            if (OpennedWindow != this && !AdditionalWindow) return;
            if (!AdditionalWindow) OpennedWindow = null;
            await CloseAnimation();
            MainMenuController.Instance.windows.MainMenu.GoToOriginalColor();
        }
        /// <summary>
        /// Закрыть/Открыть данное окно
        /// </summary>
        public void OpenCloseToggle()
        {
            IsOpen = !IsOpen;
            if (!IsOpen) Close();
            else Open();
        }
        protected virtual Task OpenAnimation()
        {
            return Task.CompletedTask;
//            throw new NotImplementedException();
        }
        protected virtual Task CloseAnimation()
        {
            return Task.CompletedTask;

            //            throw new NotImplementedException();
        }
        /// <summary>
        /// Вернуть окно к оригинальному цвету
        /// </summary>
        public void GoToOriginalColor() => Color = OriginalColor;
    }
}

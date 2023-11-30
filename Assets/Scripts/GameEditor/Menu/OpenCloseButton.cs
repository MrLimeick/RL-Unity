using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

namespace RL.GameEditor.Menu
{
    public class OpenCloseButton : UI.Button
    {
        private bool m_MenuIsOpen = false;
        public bool MenuIsOpen
        {
            get => m_MenuIsOpen;
            set
            {
                m_MenuIsOpen = value;
                /*if (value) OpenMenuAnimation();*/
                /*else CloseMenuAnimation();*/
            }
        }
        public RectTransform UpStick, MiddleStick, DownStick;
        public RectTransform Graphic;
        public Image Block;
        /*private CancellationTokenSource cts = new();*/
        [SerializeField]
        private AudioSource OpenCloseSound;
        public void Awake()
        {
            //OnClick += () => MenuIsOpen = !MenuIsOpen;
            if (Block == null) Debug.LogWarning(@"� ���� ��������� ���������� �������� ���. ������� ����� �������� �� ������ �� ���� �� � �� ���������� ��������");
            else
            {
                if (Block.TryGetComponent(out Button BlockButton)) BlockButton.onClick.AddListener(() => MenuIsOpen = false);
                else Debug.LogWarning(@"� ��������� ���� ���� ��������� ���������� ��������� ""Button"". ����� �������� �� ���� �� ����� ��������.");
            }
        }
        /// <summary>
        /// �������� �������� ����
        /// </summary>
        /*private async void OpenMenuAnimation()
        {
            *//*cts.Cancel();
            cts = new();
            CancellationToken ct = cts.Token;*//*

            if (Block.TryGetComponent(out GraphicRaycaster GraphicRaycaster)) { GraphicRaycaster.enabled = true; }

            *//*var time = 0.5f;*/

            /*await GetComponent<Shadow>().EffectDistanceAsync(new Vector2(0,0),time/5);*//*

            var UpStickImage = UpStick.GetComponent<Image>();
            var DownStickImage = DownStick.GetComponent<Image>();
            var MiddleStickImage = MiddleStick.GetComponent<Image>();

            OpenCloseSound.Stop();
            OpenCloseSound.time = 0.1f;
            OpenCloseSound.Play();

            *//*List<Task> tasks = new()
            {
                UpStickImage.ColorAnimAsync(new Color(1f, 1f, 1f), time,ct),
                DownStickImage.ColorAnimAsync(new Color(1f, 1f, 1f), time,ct),
                MiddleStickImage.ColorAnimAsync(new Color(1f, 1f, 1f), time,ct),
                GetComponent<Image>().ColorAnimAsync(new Color(0.5019608f, 0.764706f, 0.7686275f), time,ct),

                UpStick.PositionAnimAsync(new Vector2(0, 0), time,ct),
                DownStick.PositionAnimAsync(new Vector2(0, 0), time,ct),
                UpStick.RotationAnimAsync(new Vector3(0, 0, 45), time,ct),
                DownStick.RotationAnimAsync(new Vector3(0, 180, 45), time,ct),
                MiddleStick.SizeAnimAsync(new Vector2(5, 5), time,ct),
                Graphic.SizeAnimAsync(new Vector2(180, 185), time,ct),
                Block.ColorAnimAsync(new Color(0, 0, 0, 0.9f), time,ct),
            };
            await Task.WhenAll(tasks.ToArray());*//*
            if (GraphicRaycaster is not null) { GraphicRaycaster.enabled = true; } // ����� ��� �������������� ���� � ������� �������� �� ������ �������� ����
        }*/
        /// <summary>
        /// �������� �������� ����
        /// </summary>
        /*private async void CloseMenuAnimation()
        {
            cts.Cancel();
            cts = new();
            CancellationToken ct = cts.Token;

            if (Block.TryGetComponent(out GraphicRaycaster GraphicRaycaster)) { GraphicRaycaster.enabled = false; }

            var time = 0.5f;

            var UpStickImage = UpStick.GetComponent<Image>();
            var DownStickImage = DownStick.GetComponent<Image>();
            var MiddleStickImage = MiddleStick.GetComponent<Image>();

            OpenCloseSound.Stop();
            OpenCloseSound.time = 0.1f;
            OpenCloseSound.Play();

            List<Task> tasks = new()
            {
                UpStickImage.ColorAnimAsync(new Color(0.5019608f, 0.764706f, 0.7686275f), time,ct),
                DownStickImage.ColorAnimAsync(new Color(0.5019608f, 0.764706f, 0.7686275f), time,ct),
                MiddleStickImage.ColorAnimAsync(new Color(0.5019608f, 0.764706f, 0.7686275f), time,ct),
                GetComponent<Image>().ColorAnimAsync(new Color(1f, 1f, 1f), time,ct),


                UpStick.PositionAnimAsync(new Vector2(0, 10), time,ct),
                DownStick.PositionAnimAsync(new Vector2(0, -10), time,ct),
                UpStick.RotationAnimAsync(new Vector3(0, 0, 0), time,ct),
                DownStick.RotationAnimAsync(new Vector3(0, 180, 0), time,ct),
                MiddleStick.SizeAnimAsync(new Vector2(30, 5), time,ct),
                Graphic.SizeAnimAsync(new Vector2(0, 0), time,ct),
                Block.ColorAnimAsync(new Color(0, 0, 0, 0f), time,ct)
            };
            await Task.WhenAll(tasks.ToArray());
            if (Block.TryGetComponent(out GraphicRaycaster)) { GraphicRaycaster.enabled = false; }

            await GetComponent<Shadow>().EffectDistanceAsync(new Vector2(0,-5),time/5);
        }*/
    }
}
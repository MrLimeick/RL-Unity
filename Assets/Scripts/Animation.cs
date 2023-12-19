using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System;

public class Animation
{
	private Coroutine _coroutine = null;
	private readonly MonoBehaviour _monoBehaviour;
	public float Duration = 1;
	public UnityAction<float> Action;
	public UnityAction OnEnd;
	public UnityAction OnStart;
	public bool Invert = false;
	public bool UnscaledTime = true;

	public Animation(MonoBehaviour monoBehaviour)
	{
		_monoBehaviour = monoBehaviour;
	}

	public void Start()
	{
        if (_coroutine != null) _monoBehaviour.StopCoroutine(_coroutine);
        _coroutine = _monoBehaviour.StartCoroutine(AnimationCoroutine());
    }

	public void Stop()
	{
		if (_coroutine == null) return;

		_monoBehaviour.StopCoroutine(_coroutine);
		_coroutine = null;
	}

	float _startTime = 0;
	float _localTime = 0;

	float UpdateTime()
	{
		_localTime = ((UnscaledTime ? Time.unscaledTime : Time.time) - _startTime) / Duration;
		return _localTime;
	}

    IEnumerator AnimationCoroutine()
    {
		OnStart?.Invoke();

        _startTime = UnscaledTime ? Time.unscaledTime : Time.time;

        while (UpdateTime() < 1)
        {
            var t = Invert ? (1 - _localTime) : _localTime;
            Action?.Invoke(t);

            yield return 0;
        }

		Action?.Invoke(Invert ? 0 : 1);
		OnEnd?.Invoke();
    }
}
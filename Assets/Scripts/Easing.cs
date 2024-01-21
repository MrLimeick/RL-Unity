using System;
/// <summary>
/// <see href="https://easings.net/#">Нажми здесь что-бы увидеть все easing'и!</see>
/// </summary>
public static class Easing
{
    const float c1 = 1.70158f;
    const float c3 = c1 + 1;
    const float c2 = c1 * 1.525f;
    const float c4 = (2 * MathF.PI) / 3;
    const float c5 = (2 * MathF.PI) / 4.5f;
    const float n1 = 7.5625f;
    const float d1 = 2.75f;

    #region Sine
    /// <summary>
    /// <see href="https://easings.net/#easeInSine">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InSine(float val)
        => 1 - MathF.Cos((val * MathF.PI) / 2);

    /// <summary>
    /// <see href="https://easings.net/#easeOutSine">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
	public static float OutSine(float val)
        => MathF.Sin((val * MathF.PI) / 2);

    /// <summary>
    /// <see href="https://easings.net/#easeInOutSine">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutSine(float val)
        => -(MathF.Cos(MathF.PI * val) - 1) / 2;
    #endregion
    #region Quad
    /// <summary>
    /// <see href="https://easings.net/#easeInQuad">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InQuad(float x)
        => x * x;

    /// <summary>
    /// <see href="https://easings.net/#easeOutQuad">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutQuad(float x)
        => 1 - (1 - x) * (1 - x);

    /// <summary>
    /// <see href="https://easings.net/#easeInOutQuad">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutQuad(float x)
        => x < 0.5f
        ? 2 * x * x
        : 1 - MathF.Pow(-2 * x + 2, 2) / 2;
    #endregion
    #region Cubic
    /// <summary>
    /// <see href="https://easings.net/#easeInCubic">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InCubic(float x)
        => x * x * x;

    /// <summary>
    /// <see href="https://easings.net/#easeOutCubic">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutCubic(float x)
        => 1 - MathF.Pow(1 - x, 3);

    /// <summary>
    /// <see href="https://easings.net/#easeInOutCubic">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutCubic(float x)
        => x < 0.5
        ? 4 * x * x * x
        : 1 - MathF.Pow(-2 * x + 2, 3) / 2;
    #endregion
    #region Quart
    /// <summary>
    /// <see href="https://easings.net/#easeInQuart">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InQuart(float x)
        => x * x * x * x;

    /// <summary>
    /// <see href="https://easings.net/#easeOutQuart">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutQuart(float x)
        => 1 - MathF.Pow(1 - x, 4);

    /// <summary>
    /// <see href="https://easings.net/#easeInOutQuart">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutQuart(float x)
        => x < 0.5
        ? 8 * x * x * x * x
        : 1 - MathF.Pow(-2 * x + 2, 4) / 2;
    #endregion
    #region Quint
    /// <summary>
    /// <see href="https://easings.net/#easeInQuint">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InQuint(float x)
        => x * x * x * x * x;

    /// <summary>
    /// <see href="https://easings.net/#easeOutQuint">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutQuint(float x)
        => 1 - MathF.Pow(1 - x, 5);

    /// <summary>
    /// <see href="https://easings.net/#easeInOutQuint">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutQuint(float x)
        => x < 0.5 ?
        16 * x * x * x * x * x :
        1 - MathF.Pow(-2 * x + 2, 5) / 2;
    #endregion
    #region Expo
    /// <summary>
    /// <see href="https://easings.net/#easeInExpo">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InExpo(float x)
        => x == 0
        ? 0
        : MathF.Pow(2, 10 * x - 10);

    /// <summary>
    /// <see href="https://easings.net/#easeOutExpo">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutExpo(float x)
        => x == 1
        ? 1
        : 1 - MathF.Pow(2, -10 * x);

    /// <summary>
    /// <see href="https://easings.net/#easeInOutExpo">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutExpo(float x)
        => x == 0
        ? 0
        : x == 1
        ? 1
        : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2
        : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
    #endregion
    #region Circ
    /// <summary>
    /// <see href="https://easings.net/#easeInCirc">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InCirc(float x)
        => 1 - MathF.Sqrt(1 - MathF.Pow(x, 2));

    /// <summary>
    /// <see href="https://easings.net/#easeOutCirc">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutCirc(float x)
        => MathF.Sqrt(1 - MathF.Pow(x - 1, 2));

    /// <summary>
    /// <see href="https://easings.net/#easeInOutCirc">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutCirc(float x) =>
        x < 0.5f
        ? (1 - MathF.Sqrt(1 - MathF.Pow(2 * x, 2))) / 2
        : (MathF.Sqrt(1 - MathF.Pow(-2 * x + 2, 2)) + 1) / 2;
    #endregion
    #region Back
    /// <summary>
    /// <see href="https://easings.net/#easeInBack">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InBack(float x)
        => c3 * x * x* x - c1 * x * x;

    /// <summary>
    /// <see href="https://easings.net/#easeOutBack">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutBack(float x)
        => 1 + c3 * MathF.Pow(x - 1, 3) + c1 * MathF.Pow(x - 1, 2);

    /// <summary>
    /// <see href="https://easings.net/#easeInOutBack">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutBack(float x)
        => x < 0.5f
        ? (MathF.Pow(2 * x, 2) * ((c2 + 1) * 2 * x - c2)) / 2
        : (MathF.Pow(2 * x - 2, 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    #endregion
    #region Elastic
    /// <summary>
    /// <see href="https://easings.net/#easeInElastic">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InElastic(float x)
        => x == 0
        ? 0
        : x == 1
        ? 1
        : -MathF.Pow(2, 10 * x - 10) * MathF.Sin((x * 10 - 10.75f) * c4);

    /// <summary>
    /// <see href="https://easings.net/#easeOutElastic">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutElastic(float x)
        => x == 0
        ? 0
        : x == 1
        ? 1
        : MathF.Pow(2, -10 * x) * MathF.Sin((x * 10 - 0.75f) * c4) + 1;

    /// <summary>
    /// <see href="https://easings.net/#easeInOutElastic">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutElastic(float x)
        => x == 0
        ? 0
        : x == 1
        ? 1
        : x < 0.5f
        ? -(MathF.Pow(2, 20 * x - 10) * MathF.Sin((20 * x - 11.125f) * c5)) / 2
        : (MathF.Pow(2, -20 * x + 10) * MathF.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
    #endregion
    #region Bounce
    /// <summary>
    /// <see href="https://easings.net/#easeInBounce">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InBounce(float x)
        => 1 - OutBounce(1 - x);

    /// <summary>
    /// <see href="https://easings.net/#easeOutBounce">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float OutBounce(float x)
        => x < 1 / d1
        ? n1 * x * x
        : x < 2 / d1
        ? n1 * (x -= 1.5f / d1) * x + 0.75f
        : x < 2.5 / d1
        ? n1 * (x -= 2.25f / d1) * x + 0.9375f
        : n1 * (x -= 2.625f / d1) * x + 0.984375f;

    /// <summary>
    /// <see href="https://easings.net/#easeInOutBounce">Нажми здесь что-бы посмотреть.</see>
    /// </summary>
    public static float InOutBounce(float x)
        => x < 0.5
        ? (1 - OutBounce(1 - 2 * x)) / 2
        : (1 + OutBounce(2 * x - 1)) / 2;
    #endregion
}
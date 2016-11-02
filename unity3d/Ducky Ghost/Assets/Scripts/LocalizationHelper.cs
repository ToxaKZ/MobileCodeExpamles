using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public sealed class LocalizationHelper
{
    public const string RES_KEY_QUIT_FROM_TH_GAME = "1";
    public const string RES_KEY_PAUSE = "2";
    public const string RES_KEY_LEVEL_FAILED = "3";
    public const string RES_KEY_CONGRATULATIONS = "4";
    public const string RES_KEY_GO = "5";
    public const string RES_KEY_RATE = "6";
    public const string RES_KEY_REMOVE_ADS = Constants.TAG_NAME_REMOVE_ADS;
    public const string RES_KEY_INTERNET_NO = "8";
    public const string RES_KEY_RESTORE = Constants.TAG_NAME_RESTORE;
    public const string RES_KEY_FORMAT_POINTS = "10";
    public const string RES_KEY_TURN_OFF_TIME_LIMIT = Constants.TAG_NAME_TURN_OFF_TIME_LIMIT;
    public const string RES_KEY_FORMAT_TWEET = "11";
    private readonly Dictionary<string, string> Russian;
    private readonly Dictionary<string, string> English;
    private readonly IDictionary<string, string> current;
    private static LocalizationHelper helper = null;

    public static LocalizationHelper SharedInstance()
    {
        if (helper == null)
        {
            helper = new LocalizationHelper();
        }

        return helper;
    }

    private LocalizationHelper()
    {
        Russian = new Dictionary<string, string>();
        English = new Dictionary<string, string>();

        InitResources();

        current = SystemLanguage.Russian.Equals(Application.systemLanguage) ? Russian : English;
    }

    private void InitResources()
    {
        Russian.Add(RES_KEY_QUIT_FROM_TH_GAME, "Выйти из игры?");
        English.Add(RES_KEY_QUIT_FROM_TH_GAME, "Quit of the game?");

        Russian.Add(RES_KEY_PAUSE, "ПАУЗА");
        English.Add(RES_KEY_PAUSE, "PAUSE");

        Russian.Add(RES_KEY_LEVEL_FAILED, "ВЫ ПРОИГРАЛИ");
        English.Add(RES_KEY_LEVEL_FAILED, "LEVEL FAILED");

        Russian.Add(RES_KEY_CONGRATULATIONS, "ПОЗДРАВЛЯЕМ");
        English.Add(RES_KEY_CONGRATULATIONS, "CONGRATULATIONS");

        Russian.Add(RES_KEY_GO, "ПОЕХАЛИ");
        English.Add(RES_KEY_GO, "GO");

        Russian.Add(RES_KEY_RATE, "ОЦЕНИТЬ");
        English.Add(RES_KEY_RATE, "RATE");

        Russian.Add(RES_KEY_REMOVE_ADS, "УБРАТЬ РЕКЛАМУ ЗА $0.99");
        English.Add(RES_KEY_REMOVE_ADS, "REMOVE ADS FOR $0.99");

        Russian.Add(RES_KEY_INTERNET_NO, "Не удалось подключиться к интернету");
        English.Add(RES_KEY_INTERNET_NO, "Failed to connect to the Internet");

        Russian.Add(RES_KEY_RESTORE, "ВОССТАНОВИТЬ ПОКУПКИ");
        English.Add(RES_KEY_RESTORE, "RESTORE PURCHASES");

        Russian.Add(RES_KEY_FORMAT_POINTS, "ОЧКИ: {0}");
        English.Add(RES_KEY_FORMAT_POINTS, "POINTS: {0}");

        Russian.Add(RES_KEY_TURN_OFF_TIME_LIMIT, "ОТКЛЮЧИТЬ ЛИМИТ ВРЕМЕНИ ЗА $0.99");
        English.Add(RES_KEY_TURN_OFF_TIME_LIMIT, "TURN OFF THE TIME LIMIT FOR $0.99");

        Russian.Add(RES_KEY_FORMAT_TWEET, "Я набрал {0} очков в {1} уровне (игра Ducky Ghost доступно в App Store и Play Маркет)");
        English.Add(RES_KEY_FORMAT_TWEET, "I earned {0} points in level {1} (the game Ducky Ghost is available on the App Store and Play Market)");
    }

    public string GetValue(string key)
    {
        return current[key];
    }
}
using UnityEngine;

public static class PlayerStats
{
    public static int Wins
    {
        get => PlayerPrefs.GetInt("Wins", 0);
        set => PlayerPrefs.SetInt("Wins", value);
    }

    public static int Losses
    {
        get => PlayerPrefs.GetInt("Losses", 0);
        set => PlayerPrefs.SetInt("Losses", value);
    }

    public static float BestDistance
    {
        get => PlayerPrefs.GetFloat("BestDistance", 0);
        set => PlayerPrefs.SetFloat("BestDistance", value);
    }

    public static void AddWin()
    {
        Wins++;
        PlayerPrefs.Save();
    }

    public static void AddLoss()
    {
        Losses++;
        PlayerPrefs.Save();
    }

    public static void CheckBestDistance(float distance)
    {
        if (distance > BestDistance)
        {
            BestDistance = distance;
            PlayerPrefs.Save();
        }
    }
}
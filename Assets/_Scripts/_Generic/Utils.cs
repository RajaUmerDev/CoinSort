using System.Linq;
using UnityEngine;

public static class Utils
{
    public static T[] GetAllInstances<T>() where T : ScriptableObject
    {
        return Resources.LoadAll<T>("ScriptableObjects");
    }
    
   
    public static Coin[] LoadAllCoins(string Coins)
    {
        return Resources.LoadAll<Coin>(Coins);
    }
    
    
    public static Coin LoadCoinByColorType(string Coins,ColorType.Color colorType)
    {
        Coin[] coins = Resources.LoadAll<Coin>(Coins);

        Coin coin = coins.FirstOrDefault(c => c.ColorType == colorType);
        return coin;
    }
    
}

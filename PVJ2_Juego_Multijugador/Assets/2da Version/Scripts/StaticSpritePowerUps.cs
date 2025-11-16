using UnityEngine;

// Se encarga de asignarles a los sprites de los powerUps un id
// Este id luego es enviado a todos los jugadores para que puedan ver los powerUps de su rival
public static class StaticSpritePowerUps 
{
    private static Sprite[] _sprites;

    // Carga automática de todos los sprites en Resources/PowerUps
    public static void Load()
    {
        _sprites = Resources.LoadAll<Sprite>("PowerUps");
    }

    public static Sprite GetSprite(int id)
    {
        if (_sprites == null) Load();
        return _sprites[id];
    }

    public static int GetId(Sprite sprite)
    {
        if (_sprites == null) Load();

        for (int i = 0; i < _sprites.Length; i++)
        {
            if (_sprites[i] == sprite)
                return i;
        }

        return -1;
    }
}

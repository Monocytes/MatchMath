using System;

public static class GameEvents
{
    public static event Action<GameState> OnGameStateChange = null;

    public static void ReportGameStateChange(GameState gameState)
    {
        if (OnGameStateChange != null)
            OnGameStateChange(gameState);
    }
}

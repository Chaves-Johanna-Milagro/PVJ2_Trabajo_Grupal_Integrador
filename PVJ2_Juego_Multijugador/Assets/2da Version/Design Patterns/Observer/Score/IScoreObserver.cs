using UnityEngine;

public interface IScoreObserver
{
    void OnScoreChanged(int newScore);
}

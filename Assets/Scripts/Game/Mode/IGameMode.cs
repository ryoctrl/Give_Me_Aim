using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameMode {
    void GameProcess();
    void Play();
    void Pause();
    void SetAutoMode(bool auto);
    void Interrupt();
    void Initialize();
    void GameOver();
    void SongChange();
}

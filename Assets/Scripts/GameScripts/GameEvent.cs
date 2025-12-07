using System;

[Serializable]
public class GameEvent
{
    public string playerId;     // ID hráča (ak máš)
    public string sessionId;    // ID konkrétneho runu hry
    public string eventName;    // towerShot, zombieSpawn, gameOver, ...

    public string timestamp;    // ISO čas v UTC
    public float sessionTime;   // koľko sekúnd od začiatku session

    public string towerId;      // môže byť null / "" ak nedáva zmysel
    public string zombieId;     // to, čo chceš mať navyše

    public float posX;          // pozícia eventu (napr. miesto toweru/zombie)
    public float posY;
    public float posZ;

    // voliteľné: ak chceš extra payload, vieš pridať ďalšie polia:
    // public int hp;
    // public int waveNumber;
}
using UnityEngine;
using System.Collections;
using System;

public class EnergyReloader : MonoBehaviour
{
    public const int ENERGY_RELOADING_TIME = 360;
    public DateTime startTime;

    public static EnergyReloader Instance = null;

    public void Start()
    {
        Instance = this;
    }

    public void Awake()
    {
        Instance = this;
    }

    public static void RestoreEnergyByIdle() {
        PlayerSave.ActiveSave.energy.Value += (int)((DateTime.Now - PP.GetDateTime(PPKeys.LAST_ENERGY_RELOADING_TIME)).TotalSeconds / ENERGY_RELOADING_TIME);
        PlayerSave.Save();

        PP.SetDateTime(PPKeys.LAST_ENERGY_RELOADING_TIME, DateTime.Now);
    }

    public static void StartReloading() {
        RestoreEnergyByIdle();
        EnergyReloader.Instance.StartCoroutine(EnergyReloader.Instance.Reloader());
    }

    public IEnumerator Reloader()
    {
        startTime = DateTime.Now;
        yield return new WaitForSeconds(ENERGY_RELOADING_TIME);

        Reload();
    }

    public void Reload() {
        PlayerSave.ActiveSave.energy.Value++;
        PlayerSave.Save();
        EnergyReloader.onEnergyReload.Invoke();

        PP.SetDateTime(PPKeys.LAST_ENERGY_RELOADING_TIME, DateTime.Now);

        StartCoroutine(Reloader());
    }

    public static TimeSpan TimeToReload() {
        return new TimeSpan(
            0,
            0,
            ENERGY_RELOADING_TIME - (int)(DateTime.Now - EnergyReloader.Instance.startTime).TotalSeconds
        );
    }

    public static TimeSpan TimeToReload(int targetEnergy) {
        return TimeToReload().Add(new TimeSpan(0, 0, (int)(targetEnergy - PlayerSave.ActiveSave.energy.Value - 1) * ENERGY_RELOADING_TIME));
    }

    private static Action onEnergyReload = () => { };
    public static void OnEnergyReload(Action onEnergyReload)
    {
        EnergyReloader.onEnergyReload = onEnergyReload;
    }
}
﻿using Unity.Entities;
using UnityEngine;

public partial class PlayerHitFxSystem : SystemBase
{
    private static readonly int DieHash = Animator.StringToHash("Die");

    protected override void OnUpdate()
    {
        var gameUi = SurvivalShooterBootstrap.Settings.GameUi;
        int health = int.MaxValue;

        var playerTookDamage = false; 
        Entities.WithStructuralChanges().ForEach((Entity entity, ref HealthUpdatedEvent hp) =>
        {
            EntityManager.DestroyEntity(entity);

            gameUi.OnPlayerTookDamage(hp.Health);

            health = Mathf.Min(health, hp.Health);

            playerTookDamage = true;
        }).Run();

        if (playerTookDamage)        
        {
            Entities.WithoutBurst().WithAll<PlayerData>().ForEach((AudioSource audio, Animator animator) =>
            {
                if (health <= 0)
                {
                    var playerDeathClip = SurvivalShooterBootstrap.Settings.PlayerDeathClip;
                    audio.clip = playerDeathClip;

                    animator.SetTrigger(DieHash);
                }

                audio.Play();
            }).Run();
        }
    }
}

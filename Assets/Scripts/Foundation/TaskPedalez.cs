using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskPedalez : Task
{
    [Header("Pedalez pour gagner")]
    [SerializeField] float progress; // 100% = finished
    [SerializeField] protected float duration;
    [SerializeField] float delay;
    public override void Interact()
    {
        if (progress < duration)
        {
            HandleBeforePedale();
            GameManager.Instance.player.canMove = false;
            GameManager.Instance.player.characterController.enabled = false;
            Invoke("FunctionToStartListening", delay);
            HideInteractable();
        }
        else
        {
            base.Interact();
            GameManager.Instance.player.canMove = true;
            GameManager.Instance.player.characterController.enabled = true;
            HandleFinishPedale();
        }
    }

    protected virtual void HandleBeforePedale()
    {
        // do an animation for exemple
    }

    protected virtual void HandleFinishPedale()
    {
        // stop an animation for exemple
    }

    void FunctionToStartListening()
    {
        StartCoroutine("ListenToVelo");
    }

    IEnumerator ListenToVelo()
    {
        // Get required component to work
        DialogueVelo playerVelo = GameManager.Instance.velo;
        
        ResourceController resource =  playerVelo.GetComponent<ResourceController>();

        // Change resistance if the player needed items
        if (requireItem && !CheckNecessaryItem())
        {
            MyDebug.Log("YOU NEED SMTH AND YOU GET HURT by : "+ (int)SettingsManager.instance.settings.currentDifficulty.maxPente / 2);
            playerVelo.ChangePente((int)SettingsManager.instance.settings.currentDifficulty.maxPente / 2);
        }

        // increase progress when pedale
        while (progress < duration)
        {
            progress += Time.deltaTime * (resource.tempS * 2 / playerVelo.maxSpeed);
            OnProgessChanged(progress);
            yield return null;
        }

        // reset resistance to normal and player ability to walk
        progress = duration;
        Interact();
        GameManager.Instance.SetPenteScaledWithDmg();

    }

    protected virtual void OnProgessChanged(float p)
    {
        MyDebug.Log("Progress changed = "+p);
    }
}

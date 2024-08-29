using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Charactor
{
    [Serializable]
    public struct charactorData 
    {
        public float activeMaxSpeed;
        public float jumpForce;
    }
    [SerializeField]
    public charactorData charaData;

    public PlayerController playerController;

    public virtual void Jump()
    {
        playerController.velocity.y += charaData.jumpForce;
    }
}

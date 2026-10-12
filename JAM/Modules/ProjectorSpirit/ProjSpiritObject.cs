using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InnerNet;
using TownOfUs.Modules.MedSpirit;

namespace JAM.Modules.ProjectorSpirit
{
    public class ProjSpiritObject
    {
        public PlayerControl? Owner;
        public Rigidbody2D Rigidbody;
        public ProjSpiritNetTransform NetTransform;
        public SpriteRenderer Rend;
    }
}
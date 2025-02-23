using Godot;
using System;

public partial class CustomSignals : Node
{
    [Signal] public delegate void AmmoPickupEventHandler(int ammoAmount);
    [Signal] public delegate void EnemyDamageEventHandler(float damageAmount, CharacterBody3D enemy);
    [Signal] public delegate void PlayerDamageEventHandler(float damageAmount);
}

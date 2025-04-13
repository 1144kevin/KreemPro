using Fusion;
using UnityEngine;

public enum InputButton
{
  ATTACK
}
public struct NetworkInputData : INetworkInput
{
  public Vector3 direction;
  public NetworkButtons buttons;
  public bool damageTrigger;
  public bool respawnTrigger;

}

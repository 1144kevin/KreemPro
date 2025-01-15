using Fusion;
using UnityEngine;

public enum InputButton
{
  ATTACK
}
public struct NetworkInputData : INetworkInput
{
  public Vector3 direction;
  public NetworkButtons button;
  public Angle pitch;//鏡頭x軸轉向
  public Angle yaw;//鏡頭y軸轉向

}

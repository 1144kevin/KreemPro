using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System;
public class PlayerNetworkData : NetworkBehaviour
{
    [Networked, OnChangedRender(nameof(OnPlayerNameChanged))]
    public string PlayerName { get; set; }
    [Networked]
    public int SelectedCharacterIndex { get; set; }
    // [Networked]
    public override void Spawned()
    {
        if (Object.HasInputAuthority)
        {
            SetPlayerName_RPC(GameManager.Instance.PlayerName);
            SetSelectedCharacterIndex_RPC(GameManager.Instance.SelectedCharacterIndex);
        }
        GameManager.Instance.SetPlayerNetworkData(Object.InputAuthority, this);
    }

    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void SetPlayerName_RPC(string playerName)
    {
        PlayerName = playerName;
    }
    [Rpc(sources: RpcSources.InputAuthority, targets: RpcTargets.StateAuthority)]
    private void SetSelectedCharacterIndex_RPC(int characterIndex)
    {
        SelectedCharacterIndex = characterIndex;
    }
    private void OnPlayerNameChanged()
    {
        GameManager.Instance.UpdatePlayerList();
    }
    // private void OnSelectedCharacterNameChanged()
    // {
    //     GameManager.Instance.UpdatePlayerList();
    // }
}
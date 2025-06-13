using System;
using System.Threading.Tasks;
using UnityEngine;

public class GameInitiator : MonoBehaviour
{
    private async void Start()
    {
        await DoSomething();
    }

    private async Task DoSomething()
    {
        throw new NotImplementedException();
    }
}

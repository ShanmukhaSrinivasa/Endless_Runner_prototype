using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class UnityServicesManager : MonoBehaviour
{
    public static UnityServicesManager Instance;

    public async void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        await InitializeServices();
    }

    private async Task InitializeServices()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            Debug.Log("Unity Services Initialized");
            Debug.Log("Player ID: " + AuthenticationService.Instance.PlayerId);
        }
        catch(System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}

using UnityEngine;

public class Test : MonoBehaviour
{    
    
    public OpenAI_Config apiConfig;
    public OpenAIAPIManager apiManager;

    void Init()
    {
        apiManager.apiKey = apiConfig.apiKey;
        apiManager.apiUrl = apiConfig.apiUrl;
        apiManager.apiModel = apiConfig.apiModel;
    }

    void Start()
    {
        Init();

        if (apiManager != null)
        {
            apiManager.SendRequest(apiConfig.apiPrompt, apiConfig.apiMaxToken);
        }
    }
}


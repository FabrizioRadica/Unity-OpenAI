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
            string prompt = apiConfig.apiPrompt;
            int maxTokens = apiConfig.apiMaxToken; // Definisci il numero massimo di token
            apiManager.SendRequest(prompt, maxTokens);
        }
    }
}


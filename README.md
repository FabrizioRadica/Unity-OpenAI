# Unity-OpenAI
Simple implementation of OpenAI in Unity 3D

Hey everyone,

Here’s a simple, basic implementation of OpenAI’s API in Unity. The code isn't perfect, and it's meant for educational purposes to help anyone get started with the basics of OpenAI.

The code is provided as-is, with no guarantees that it will work in the future or that there will be any further updates.


*Implementation Test*
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

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

public class OpenAIAPIManager : MonoBehaviour
{

    [HideInInspector]
    public string apiKey;
    [HideInInspector]
    public string apiUrl;
        [HideInInspector]
    public string apiModel;

    /// <summary>
    /// Richiama il posto in una coroutine per leggere in modo asincrono i dati dal server OpenAI
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="maxTokens"></param>
    public void SendRequest(string prompt, int maxTokens)
    {
        StartCoroutine(PostRequest(prompt, maxTokens));
    }


    /// <summary>
    /// Post dei dati al server OpenAI. Se è tutto corretto, ritorna il risultato del prompt 
    /// </summary>
    /// <param name="prompt"></param>
    /// <param name="maxTokens"></param>
    /// <returns></returns>
    private IEnumerator PostRequest(string prompt, int maxTokens)
    {
        // Crea il payload JSON
        string jsonPayload = CreateJsonPayload(prompt, maxTokens);
        Debug.Log("Payload: " + jsonPayload); // Aggiunto per il debug

        // Converte il payload in un array di byte
        byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonPayload);

        // Crea una nuova richiesta Web
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // Invia la richiesta e aspetta la risposta da parte del server OpenAI
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {request.error}\nResponse Code: {request.responseCode}");
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Success: " + request.downloadHandler.text);
                
                OpenAIResponse jsonResponse = JsonUtility.FromJson<OpenAIResponse>(request.downloadHandler.text);

                // STAMPA IL RISULTATO
                string completion = jsonResponse.choices[0].message.content;
                Debug.Log(completion);
            }
            else
            {
                Debug.LogError("Unknown error occurred");
            }
        }
    }

    // Metodo per creare il payload JSON
    // Settorializza l'AI
    public string CreateJsonPayload(string prompt, int maxTokens)
    {
        var payload = new OpenAIPayload
        {
            model = apiModel,
            messages = new[]
            {
                new Message { role = "system", content = "You are a good archaeologist" },
                new Message { role = "user", content = prompt }
            },
            max_tokens = maxTokens
        };

        return JsonUtility.ToJson(payload);
    }
}

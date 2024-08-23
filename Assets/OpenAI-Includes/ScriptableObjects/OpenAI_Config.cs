using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class OpenAI_Config : ScriptableObject
{
    public string apiKey;  //your API Key. Needs an OpenAI Account!!!
    public string apiUrl;  //default: https://api.openai.com/v1/chat/completions
    public string apiModel;  // default: gpt-3.5-turbo 
    public string apiPrompt; //your prompt here :)
    public string apiSystemContent; //not yet implemented ;)
    public int apiMaxToken; //Max words
}

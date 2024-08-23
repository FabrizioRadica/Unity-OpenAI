[System.Serializable]
public class Message
{
    public string role;
    public string content;
}


[System.Serializable]
public class OpenAIPayload
{
    public string model;
    public Message[] messages;
    public int max_tokens;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class OpenAIResponse
{
    public Choice[] choices;
}


using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class Gemini : MonoBehaviour
{
    public static Gemini Instance;

    private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";

    private readonly string _apiKey = "AIzaSyBZQe7MjsaQT70LEJNKl_Et8RCQJ57E-04";

    private CurrentWeapon[] currentWeapons = new CurrentWeapon[]
    {
        new CurrentWeapon(1,"Cung thường",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757386421/Bow_qccbud.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757473587/Arrow_ydmjzs.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", false),
        new CurrentWeapon(2,"Cung lửa",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757386418/FireBow_sxs3jb.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757507666/Arrow_rsbtzz.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757481121/explose_fnf7rz.png", false),
        new CurrentWeapon(3,"Kiếm vàng",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757908601/Sword_xpn3er.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757507666/Arrow_rsbtzz.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png", true),
    };

    [Serializable]
    public class PartReq
    {
        public string text;
    }

    [Serializable]
    public class MessageReq
    {
        public string role;
        public PartReq[] parts;
    }

    [Serializable]
    public class PropertyDetail
    {
        public string type;
        public string description;
    }

    [Serializable]
    public class PropertyDef
    {
        public PropertyDetail weaponID;
        public PropertyDetail reason;
    }

    [Serializable]
    public class ResponseSchema
    {
        public string type;
        public PropertyDef properties;
        public string[] required;
    }

    [Serializable]
    public class GenerationConfig
    {
        public string response_mime_type;
        public ResponseSchema response_schema;
    }

    [Serializable]
    public class RequestBody
    {
        public MessageReq[] contents;
        public GenerationConfig generationConfig;
    }

    [Serializable] public class TextWrapper { public string text; }
    [Serializable] public class PartsWrapper { public TextWrapper[] parts; }
    [Serializable] public class ContentWrapper { public PartsWrapper content; }
    [Serializable] public class CandidateWrapper { public ContentWrapper[] candidates; }

    [Serializable]
    public class ContentValidationResponse
    {
        public int weaponID;
        public string reason;
    }

    public void ValidateContent(string prompt, Action<ContentValidationResponse> onResult, Action<string> onError)
    {
        StartCoroutine(ValidateCoroutine(prompt, onResult, onError));
    }

    private IEnumerator ValidateCoroutine(string content, Action<ContentValidationResponse> onResult, Action<string> onError)
    {
        var weaponsList = string.Join(", ", currentWeapons.Select(w => $"{w.id}. {w.name}"));

        var msg = new MessageReq
        {
            role = "user",
            parts = new PartReq[]
    {
        new PartReq
        {
            text = $"Hãy chọn id của weapon phù hợp nhất. Danh sách: {weaponsList}. Prompt: {content}"
        }
    }
        };

        var reqBody = new RequestBody
        {
            contents = new[] { msg },
            generationConfig = new GenerationConfig
            {
                response_mime_type = "application/json",
                response_schema = new ResponseSchema
                {
                    type = "object",
                    properties = new PropertyDef
                    {
                        weaponID = new PropertyDetail { type = "integer", description = "ID weapon phù hợp nhất" },
                        reason = new PropertyDetail { type = "string", description = "Lý do chọn" }
                    },
                    required = new[] { "weaponID", "reason" }
                }
            }
        };

        string json = JsonUtility.ToJson(reqBody);
        Debug.Log("📤 JSON gửi đi:\n" + json);

        using (UnityWebRequest www = new UnityWebRequest($"{_apiUrl}?key={_apiKey}", "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                string errorMsg = $"Network/API Error: {www.error}\nResponse: {www.downloadHandler.text}";
                Debug.LogError(errorMsg);
                onError?.Invoke(errorMsg);
            }
            else
            {
                try
                {
                    var responseString = www.downloadHandler.text;

                    CandidateWrapper wrapper = JsonUtility.FromJson<CandidateWrapper>(responseString);
                    if (wrapper == null || wrapper.candidates == null || wrapper.candidates.Length == 0)
                    {
                        throw new Exception("Response không có candidates hợp lệ!");
                    }

                    string textJson = wrapper.candidates[0].content.parts[0].text;
                    ContentValidationResponse result = JsonUtility.FromJson<ContentValidationResponse>(textJson);

                    if (result == null)
                    {
                        throw new Exception("Parse ContentValidationResponse thất bại!");
                    }

                    onResult?.Invoke(result);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Parse Error: " + ex.Message);
                    onError?.Invoke("Parse Error: " + ex.Message);
                }
            }
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
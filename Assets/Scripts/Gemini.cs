using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Gemini : MonoBehaviour
{
    public static Gemini Instance;

    private readonly HttpClient _httpClient = new HttpClient();
    private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent";
    private readonly string _apiKey = "AIzaSyBZQe7MjsaQT70LEJNKl_Et8RCQJ57E-04";

    private CurrentWeapon[] currentWeapons = new CurrentWeapon[]
    {
        new CurrentWeapon
        (
            1,
            "Cung thường",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757386421/Bow_qccbud.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757473587/Arrow_ydmjzs.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png",
            false
        ),
        new CurrentWeapon
        (
            2,
            "Cung lửa",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757386418/FireBow_sxs3jb.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757507666/Arrow_rsbtzz.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757481121/explose_fnf7rz.png",
            false
        ),
        new CurrentWeapon
        (
            3,
            "Kiếm vàng",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757908601/Sword_xpn3er.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757507666/Arrow_rsbtzz.png",
            "https://res.cloudinary.com/dl2rytqvu/image/upload/v1757480002/effect1_pnlbaf.png",
            true
        ),
    };

    public class MessageType
    {
        public string role { get; set; }
        public Part[] parts { get; set; }
    }

    public class Part
    {
        public string text { get; set; }
    }

    public class ContentValidationResponse
    {
        public int WeaponID { get; set; }
        public string Reason { get; set; }
    }

    public async Task<ContentValidationResponse> ValidateContentAsync(string content)
    {
        var weaponsList = string.Join(", ", currentWeapons.Select(w => $"{w.id}. {w.name}"));

        var messages = new[]
        {
                new MessageType
                {
                    role = "user",
                    parts = new[]
                        {
                            new Part
                            {
                                text = $"Hãy chọn id của weapon phù hợp nhất với prompt người dùng. Danh sách weapon: {weaponsList}. Prompt: {content}"
                            }
                        }
                }
            };

        var requestBody = new
        {
            contents = messages.Select(m => new
            {
                role = m.role,
                parts = m.parts.Select(p => new { text = p.text }).ToArray()
            }).ToArray(),
            generationConfig = new
            {
                response_mime_type = "application/json",
                response_schema = new
                {
                    type = "object",
                    properties = new
                    {
                        weaponID = new
                        {
                            type = "integer",
                            description = $"Số id (1..{currentWeapons.Length}) của weapon phù hợp nhất với prompt."
                        },
                        reason = new
                        {
                            type = "string",
                            description = "Giải thích ngắn gọn lý do vì sao chọn weapon đó."
                        }
                    },
                    required = new[] { "weaponID", "reason" }
                }
            }
        };

        var url = $"{_apiUrl}?key={_apiKey}";
        var json = JsonConvert.SerializeObject(requestBody);
        var contentData = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, contentData);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API error: {errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync();

        var root = JObject.Parse(responseString);

        var textJson = root["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

        var result = JsonConvert.DeserializeObject<ContentValidationResponse>(
            textJson,
            new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            }
        );

        return result;
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

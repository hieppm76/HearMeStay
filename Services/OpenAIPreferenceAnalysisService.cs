using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Services
{
    public class OpenAIPreferenceAnalysisService : IPreferenceAnalysisService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public OpenAIPreferenceAnalysisService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<GuestInsight> AnalyzePreferenceAsync(GuestPreference pref)
        {
            var apiKey = _configuration["AI:OpenAIKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("OpenAI API Key is missing. Vui lòng thêm AI:OpenAIKey vào appsettings.json.");
            }

            // Combine all user inputs
            var guestInput = $@"
Raw Text: {pref.RawText}
Food Allergy: {(pref.HasFoodAllergy ? pref.FoodAllergyDetail : "None")}
Diet: {pref.DietPreference}
Room: {pref.RoomPreference}
Health Note: {pref.HealthNote}
Special Occasion: {pref.SpecialOccasion}
Travel Purpose: {pref.TravelPurpose}
Activity: {pref.ActivityInterest}
Need Airport Pickup: {pref.NeedAirportPickup}
Need Early Check-In: {pref.NeedEarlyCheckIn}
Need Decoration: {pref.NeedDecoration}";

            var systemPrompt = @"Bạn là một trợ lý AI phân tích nhu cầu của khách lưu trú (Guest) để giúp khách sạn chuẩn bị dịch vụ cá nhân hóa.
Dựa vào thông tin khách cung cấp, hãy bóc tách và trả về kết quả dưới dạng JSON (KHÔNG bọc trong markdown block).
Cấu trúc JSON bắt buộc:
{
  ""summary"": ""Tóm tắt ngắn gọn nhu cầu chính của khách (tiếng Việt)"",
  ""priorityLevel"": ""Low, Medium, High, Critical"",
  ""hotelConfirmationMessage"": ""Thông báo xác nhận ngắn gọn sẽ gửi cho khách (tiếng Việt)"",
  ""tags"": [
    {
      ""category"": ""Allergy, FoodDiet, RoomPreference, HealthNote, SpecialOccasion, FamilyNeed, ActivityInterest, UpsellOpportunity, VIP"",
      ""tagName"": ""Tên tag ngắn gọn (tiếng Việt)"",
      ""severity"": ""Low, Medium, High"",
      ""description"": ""Mô tả chi tiết""
    }
  ],
  ""tasks"": [
    {
      ""department"": ""Reception, Housekeeping, Kitchen, CustomerService, Manager"",
      ""title"": ""Tiêu đề công việc ngắn (tiếng Việt)"",
      ""description"": ""Mô tả công việc cần chuẩn bị""
    }
  ],
  ""upsells"": [
    {
      ""title"": ""Tên dịch vụ bán chéo gợi ý (tiếng Việt)"",
      ""reason"": ""Lý do gợi ý"",
      ""estimatedPrice"": 500000
    }
  ]
}";

            var requestBody = new
            {
                model = "gpt-4o-mini", // Có thể thay bằng gpt-4o hoặc gpt-3.5-turbo tùy ý
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = guestInput }
                },
                response_format = new { type = "json_object" },
                temperature = 0.2
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", jsonContent);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Lỗi gọi OpenAI API: {response.StatusCode} - {error}");
            }

            var responseData = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseData);
            var aiMessage = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

            if (string.IsNullOrEmpty(aiMessage))
                throw new Exception("OpenAI API trả về rỗng.");

            // Parse the JSON output from AI
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var aiResult = JsonSerializer.Deserialize<AiAnalysisResult>(aiMessage, options);

            if (aiResult == null)
                throw new Exception("Không thể parse kết quả từ OpenAI.");

            // Map to Entity
            var insight = new GuestInsight
            {
                GuestPreferenceId = pref.Id,
                Summary = aiResult.Summary ?? "Không có tóm tắt.",
                PriorityLevel = Enum.TryParse<PriorityLevel>(aiResult.PriorityLevel, out var pri) ? pri : PriorityLevel.Low,
                HotelConfirmationMessage = aiResult.HotelConfirmationMessage,
                AiJsonResult = aiMessage,
                CreatedAt = DateTime.Now
            };

            _context.GuestInsights.Add(insight);
            await _context.SaveChangesAsync();

            // Tags
            if (aiResult.Tags != null)
            {
                foreach (var t in aiResult.Tags)
                {
                    var tag = new GuestInsightTag
                    {
                        GuestInsightId = insight.Id,
                        Category = Enum.TryParse<TagCategory>(t.Category, out var cat) ? cat : TagCategory.RoomPreference,
                        TagName = t.TagName ?? "Tag",
                        Severity = Enum.TryParse<SeverityLevel>(t.Severity, out var sev) ? sev : SeverityLevel.Low,
                        Description = t.Description
                    };
                    _context.GuestInsightTags.Add(tag);
                }
            }

            // Tasks
            if (aiResult.Tasks != null)
            {
                foreach (var t in aiResult.Tasks)
                {
                    var task = new GuestTask
                    {
                        GuestInsightId = insight.Id,
                        Department = Enum.TryParse<Department>(t.Department, out var dep) ? dep : Department.CustomerService,
                        Title = t.Title ?? "Công việc",
                        Description = t.Description,
                        TaskStatus = GuestTaskStatus.Pending,
                        CreatedAt = DateTime.Now
                    };
                    _context.GuestTasks.Add(task);
                }
            }

            // Upsells
            if (aiResult.Upsells != null)
            {
                foreach (var u in aiResult.Upsells)
                {
                    var upsell = new UpsellSuggestion
                    {
                        GuestInsightId = insight.Id,
                        Title = u.Title ?? "Dịch vụ",
                        Reason = u.Reason,
                        EstimatedPrice = u.EstimatedPrice,
                        Status = UpsellStatus.Suggested,
                        CreatedAt = DateTime.Now
                    };
                    _context.UpsellSuggestions.Add(upsell);
                }
            }

            await _context.SaveChangesAsync();
            pref.AiProcessedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return insight;
        }

        // Classes để hứng JSON từ OpenAI
        private class AiAnalysisResult
        {
            public string? Summary { get; set; }
            public string? PriorityLevel { get; set; }
            public string? HotelConfirmationMessage { get; set; }
            public List<AiTag>? Tags { get; set; }
            public List<AiTask>? Tasks { get; set; }
            public List<AiUpsell>? Upsells { get; set; }
        }

        private class AiTag
        {
            public string? Category { get; set; }
            public string? TagName { get; set; }
            public string? Severity { get; set; }
            public string? Description { get; set; }
        }

        private class AiTask
        {
            public string? Department { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
        }

        private class AiUpsell
        {
            public string? Title { get; set; }
            public string? Reason { get; set; }
            public decimal? EstimatedPrice { get; set; }
        }
    }
}

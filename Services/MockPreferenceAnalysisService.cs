using HearMeStay.Data;
using HearMeStay.Models;
using HearMeStay.Models.Enums;
using HearMeStay.Services.Interfaces;

namespace HearMeStay.Services
{
    /// <summary>
    /// Mock AI service using keyword detection rules.
    /// Replaces real OpenAI calls for MVP demo.
    /// </summary>
    public class MockPreferenceAnalysisService : IPreferenceAnalysisService
    {
        private readonly ApplicationDbContext _context;

        public MockPreferenceAnalysisService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GuestInsight> AnalyzePreferenceAsync(GuestPreference pref)
        {
            var tags = new List<GuestInsightTag>();
            var tasks = new List<GuestTask>();
            var upsells = new List<UpsellSuggestion>();
            var summaryParts = new List<string>();
            var priority = PriorityLevel.Low;

            // Combine all text for keyword search
            var allText = string.Join(" ",
                pref.RawText ?? "", pref.FoodAllergyDetail ?? "", pref.DietPreference ?? "",
                pref.RoomPreference ?? "", pref.HealthNote ?? "", pref.SpecialOccasion ?? "",
                pref.TravelPurpose ?? "", pref.ActivityInterest ?? "").ToLower();

            // Allergy detection
            if (pref.HasFoodAllergy || ContainsAny(allText, "dị ứng", "hải sản", "seafood", "allergy"))
            {
                tags.Add(new GuestInsightTag { Category = TagCategory.Allergy, TagName = "Dị ứng thực phẩm", Severity = SeverityLevel.High, Description = pref.FoodAllergyDetail ?? "Khách có dị ứng thực phẩm" });
                tasks.Add(new GuestTask { Department = Department.Kitchen, Title = "Chuẩn bị menu phù hợp dị ứng", Description = $"Khách có dị ứng: {pref.FoodAllergyDetail ?? "cần xác nhận"}. Loại bỏ thành phần gây dị ứng." });
                summaryParts.Add("dị ứng thực phẩm (ưu tiên cao)");
                priority = PriorityLevel.High;
            }

            // Vegan/vegetarian
            if (ContainsAny(allText, "ăn chay", "vegan", "vegetarian", "chay trường"))
            {
                tags.Add(new GuestInsightTag { Category = TagCategory.FoodDiet, TagName = "Ăn chay", Severity = SeverityLevel.High, Description = pref.DietPreference ?? "Khách ăn chay" });
                tasks.Add(new GuestTask { Department = Department.Kitchen, Title = "Chuẩn bị menu chay", Description = $"Khách ăn chay: {pref.DietPreference ?? "cần menu thực vật"}." });
                summaryParts.Add("ăn chay");
            }

            // Quiet room
            if (ContainsAny(allText, "yên tĩnh", "quiet", "im lặng"))
            {
                tags.Add(new GuestInsightTag { Category = TagCategory.RoomPreference, TagName = "Phòng yên tĩnh", Severity = SeverityLevel.Medium });
                tasks.Add(new GuestTask { Department = Department.Reception, Title = "Sắp xếp phòng yên tĩnh", Description = "Khách yêu cầu phòng yên tĩnh, ưu tiên xa thang máy và khu vực ồn." });
                summaryParts.Add("cần phòng yên tĩnh");
                if (priority < PriorityLevel.Medium) priority = PriorityLevel.Medium;
            }

            // Scent sensitivity
            if (ContainsAny(allText, "mùi", "tinh dầu", "scent", "nồng"))
            {
                tags.Add(new GuestInsightTag { Category = TagCategory.RoomPreference, TagName = "Nhạy cảm mùi", Severity = SeverityLevel.Medium });
                tasks.Add(new GuestTask { Department = Department.Housekeeping, Title = "Phòng không mùi nồng", Description = "Không sử dụng tinh dầu mạnh, nước xịt phòng. Khách nhạy cảm với mùi." });
                summaryParts.Add("nhạy cảm với mùi");
            }

            // Special occasion
            if (ContainsAny(allText, "kỷ niệm", "anniversary", "sinh nhật", "birthday", "honeymoon", "tuần trăng mật"))
            {
                tags.Add(new GuestInsightTag { Category = TagCategory.SpecialOccasion, TagName = pref.SpecialOccasion ?? "Dịp đặc biệt", Severity = SeverityLevel.Medium });
                tasks.Add(new GuestTask { Department = Department.CustomerService, Title = "Chuẩn bị cho dịp đặc biệt", Description = $"Dịp đặc biệt: {pref.SpecialOccasion ?? "cần xác nhận"}. Chuẩn bị trang trí phù hợp." });
                upsells.Add(new UpsellSuggestion { Title = "Trang trí phòng đặc biệt", Reason = $"Khách có dịp: {pref.SpecialOccasion}", EstimatedPrice = 800000 });
                upsells.Add(new UpsellSuggestion { Title = "Bữa tối riêng lãng mạn", Reason = "Gợi ý cho dịp đặc biệt", EstimatedPrice = 1500000 });
                summaryParts.Add($"dịp đặc biệt ({pref.SpecialOccasion ?? "chưa rõ"})");
                if (priority < PriorityLevel.Medium) priority = PriorityLevel.Medium;
            }

            // Family/children
            if (ContainsAny(allText, "trẻ nhỏ", "baby", "children", "em bé", "trẻ em"))
            {
                tags.Add(new GuestInsightTag { Category = TagCategory.FamilyNeed, TagName = "Có trẻ nhỏ", Severity = SeverityLevel.Medium });
                summaryParts.Add("có trẻ nhỏ đi cùng");
            }

            // Health note
            if (!string.IsNullOrEmpty(pref.HealthNote))
            {
                tags.Add(new GuestInsightTag { Category = TagCategory.HealthNote, TagName = "Ghi chú sức khỏe", Severity = SeverityLevel.High, Description = pref.HealthNote });
                priority = PriorityLevel.High;
                summaryParts.Add("có ghi chú sức khỏe (ưu tiên cao)");
            }

            // Airport pickup
            if (pref.NeedAirportPickup || ContainsAny(allText, "sân bay", "airport"))
            {
                upsells.Add(new UpsellSuggestion { Title = "Đưa đón sân bay", Reason = "Khách cần đưa đón sân bay", EstimatedPrice = 1200000 });
                summaryParts.Add("cần đưa đón sân bay");
            }

            // Early check-in
            if (pref.NeedEarlyCheckIn)
            {
                tasks.Add(new GuestTask { Department = Department.Reception, Title = "Nhận phòng sớm", Description = "Khách yêu cầu nhận phòng sớm hơn giờ quy định." });
                summaryParts.Add("cần nhận phòng sớm");
            }

            // Decoration
            if (pref.NeedDecoration)
            {
                tasks.Add(new GuestTask { Department = Department.CustomerService, Title = "Trang trí phòng", Description = "Khách yêu cầu trang trí phòng theo dịp đặc biệt." });
                upsells.Add(new UpsellSuggestion { Title = "Trang trí phòng", Reason = "Khách yêu cầu trang trí", EstimatedPrice = 800000 });
                summaryParts.Add("cần trang trí phòng");
            }

            // Build summary
            var summary = summaryParts.Count > 0
                ? $"Khách có nhu cầu: {string.Join(", ", summaryParts)}."
                : "Khách không có yêu cầu đặc biệt. Chuẩn bị dịch vụ tiêu chuẩn.";

            // Create GuestInsight
            var insight = new GuestInsight
            {
                GuestPreferenceId = pref.Id,
                Summary = summary,
                PriorityLevel = priority,
                HotelConfirmationMessage = $"Đã nhận thông tin từ khách. Có {tags.Count} nhu cầu cần lưu ý và {tasks.Count} việc cần chuẩn bị.",
                CreatedAt = DateTime.Now
            };

            _context.GuestInsights.Add(insight);
            await _context.SaveChangesAsync();

            // Save tags, tasks, upsells with insight ID
            foreach (var tag in tags) { tag.GuestInsightId = insight.Id; }
            foreach (var task in tasks) { task.GuestInsightId = insight.Id; }
            foreach (var upsell in upsells) { upsell.GuestInsightId = insight.Id; }

            _context.GuestInsightTags.AddRange(tags);
            _context.GuestTasks.AddRange(tasks);
            _context.UpsellSuggestions.AddRange(upsells);

            pref.AiProcessedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            insight.Tags = tags;
            insight.Tasks = tasks;
            insight.UpsellSuggestions = upsells;

            return insight;
        }

        private static bool ContainsAny(string text, params string[] keywords)
        {
            return keywords.Any(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
        }
    }
}

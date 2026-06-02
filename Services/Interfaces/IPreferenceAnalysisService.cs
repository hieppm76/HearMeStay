using HearMeStay.Models;

namespace HearMeStay.Services.Interfaces
{
    public interface IPreferenceAnalysisService
    {
        Task<GuestInsight> AnalyzePreferenceAsync(GuestPreference preference);
    }
}

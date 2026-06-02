namespace HearMeStay.Services.Interfaces
{
    public interface IReportService
    {
        Task<int> GetTotalBookingsAsync(int? accommodationId = null);
        Task<decimal> GetTotalRevenueAsync(int? accommodationId = null);
        Task<decimal> GetTotalCommissionAsync(int? accommodationId = null);
        Task<double> GetConfirmedRateAsync(int? accommodationId = null);
        Task<double> GetPreferenceFormRateAsync(int? accommodationId = null);
        Task<double> GetAverageRatingAsync(int? accommodationId = null);
        Task<double> GetAveragePersonalizationRatingAsync(int? accommodationId = null);
        Task<Dictionary<string, int>> GetCommonTagsAsync(int? accommodationId = null);
    }
}

namespace HearMeStay.Models.Enums
{
    /// <summary>
    /// Named GuestTaskStatus to avoid conflict with System.Threading.Tasks.TaskStatus
    /// </summary>
    public enum GuestTaskStatus
    {
        Pending,
        InProgress,
        Done,
        CannotSupport,
        NeedMoreInfo
    }
}

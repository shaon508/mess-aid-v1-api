namespace MessAidVOne.Application.DTOs
{
    public class ActivityOutboxPayload
    {
        public List<long> TargetUserIds { get; set; } = new();
        public Dictionary<string, string> Placeholders { get; set; } = new();
    }
}

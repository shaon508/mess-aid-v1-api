namespace MassAidVOne.Domain.Entities
{
    public enum EventDomain { Setting, Mess }

    public record ActivityEvent(string Key, string DescriptionTemplate, EventDomain Domain);

    public static class ActivityEvents
    {
        public static readonly ActivityEvent ChangedPassword = new(
            Key: "ChangedPassword",
            DescriptionTemplate: "You changed your password",
            Domain: EventDomain.Setting
        );

        public static readonly ActivityEvent AddedMember = new(
            Key: "AddedMember",
            DescriptionTemplate: "#ActionUser added #TargetUser to #MessName",
            Domain: EventDomain.Mess
        );

        public static readonly ActivityEvent CreatedMess = new(
            Key: "CreatedMess",
            DescriptionTemplate: "#ActionUser created #MessName",
            Domain: EventDomain.Mess
        );
    }
}

namespace MassAidVOne.Domain.Entities
{
    public enum EventDomain
    {
        Setting = 0,
        Mess = 1
    }

    public record ActivityEvent(string Key, string DescriptionTemplate, EventDomain Domain);

    public static class ActivityEvents
    {
        public static readonly ActivityEvent ChangedPassword = new(
            Key: "Changed Password",
            DescriptionTemplate: "You changed your password",
            Domain: EventDomain.Setting
        );

        public static readonly ActivityEvent AddedMember = new(
            Key: "Added Member",
            DescriptionTemplate: "#ActionUserName added #TargetUserId to #MessName",
            Domain: EventDomain.Mess
        );

        public static readonly ActivityEvent CreatedMess = new(
            Key: "Created Mess",
            DescriptionTemplate: "#ActionUserName created #MessName",
            Domain: EventDomain.Mess
        );


        private static readonly Dictionary<string, ActivityEvent> _map =
        new()
        {
            [ChangedPassword.Key] = ChangedPassword,
            [AddedMember.Key] = AddedMember,
            [CreatedMess.Key] = CreatedMess
        };

        public static ActivityEvent FromKey(string key)
            => _map.TryGetValue(key, out var ev)
                ? ev
                : throw new InvalidOperationException($"Unknown activity event: {key}");
    }
}

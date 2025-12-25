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

        public static readonly ActivityEvent CreatedMess = new(
            Key: "Created Mess",
            DescriptionTemplate: "{#ActionUserId} created #MessName",
            Domain: EventDomain.Mess
        );

        public static readonly ActivityEvent ModifyMess = new(
            Key: "Modified Mess",
            DescriptionTemplate: "#MessName Modified by {#ActionUserId}",
            Domain: EventDomain.Mess
        );

        public static readonly ActivityEvent AddedMember = new(
           Key: "Added Member",
           DescriptionTemplate: "{#ActionUserId} added {#TargetUserId} to #MessName",
           Domain: EventDomain.Mess
        );

        public static readonly ActivityEvent RemovedMessMember = new(
            Key: "Removed Mess Member",
            DescriptionTemplate: "{#ActionUserId} removed {#TargetUserId} from #MessName",
            Domain: EventDomain.Mess
        );

        public static readonly ActivityEvent LeavedMessMember = new(
            Key: "Leaved Mess Member",
            DescriptionTemplate: "{#ActionUserId} leave from #MessName",
            Domain: EventDomain.Mess
        );


        private static readonly Dictionary<string, ActivityEvent> _map =
        new()
        {
            [ChangedPassword.Key] = ChangedPassword,
            [CreatedMess.Key] = CreatedMess,
            [ModifyMess.Key] = ModifyMess,
            [AddedMember.Key] = AddedMember,
            [RemovedMessMember.Key] = RemovedMessMember,
            [LeavedMessMember.Key] = LeavedMessMember,
        };

        public static ActivityEvent FromKey(string key)
            => _map.TryGetValue(key, out var ev)
                ? ev
                : throw new InvalidOperationException($"Unknown activity event: {key}");

    }
}

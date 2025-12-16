namespace MessAidVOne.Application.Utilities
{
    public static class ActivityDescriptionBuilder
    {
        public static string Build(string template, Dictionary<string, string>? values)
        {
            if (values == null || values.Count == 0)
            {
                return template;
            }
            foreach (var item in values)
            {
                template = template.Replace(item.Key, item.Value);
            }
            return template;
        }
    }
}

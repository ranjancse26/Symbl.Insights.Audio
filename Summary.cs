using System.Collections.Generic;

namespace Symbl.Insights.Audio
{
    public class Summary
    {
        public string id { get; set; }
        public string text { get; set; }
        public List<MessageRef> messageRefs { get; set; }
    }

    public class MessageRef
    {
        public string id { get; set; }
    }

    public class SummaryRoot
    {
        public List<Summary> summary { get; set; }
    }
}

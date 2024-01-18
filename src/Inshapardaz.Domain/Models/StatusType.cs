using System.ComponentModel;

namespace Inshapardaz.Domain.Models
{
    public enum StatusType
    {
        [Description("Unknown")]
        Unknown = 0,

        [Description("Published")]
        Published = 1,

        [Description("AvailableForTyping")]
        AvailableForTyping,

        [Description("BeingTyped")]
        BeingTyped,

        [Description("ReadyForProofRead")]
        ReadyForProofRead,

        [Description("ProofRead")]
        ProofRead,
    }
}

using System.ComponentModel;

namespace Inshapardaz.Domain.Models
{
    public enum BookStatuses
    {
        [Description("Published")]
        Published = 0,

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

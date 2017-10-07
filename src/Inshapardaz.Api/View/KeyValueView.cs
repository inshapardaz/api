// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyValueModel.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Defines the KeyValueModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Inshapardaz.Api.View
{
    public class KeyValueView<TKey, TValue>
    {
        public TKey Key { get; set; }

        public TValue Value { get; set; }
    }
}
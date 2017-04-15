// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Languages.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   The language type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace Inshapardaz.Domain.Model
{
    /// <summary>
    ///     The language type.
    /// </summary>
    public enum Languages
    {
        /// <summary>
        ///     The none.
        /// </summary>
        [Description("نامعلوم")]
        None,

        /// <summary>
        ///     The unknown.
        /// </summary>
        [Description("نامعلوم")]
        Unknown,

        /// <summary>
        ///     The urdu.
        /// </summary>
        [Description("اردو")]
        Urdu,

        /// <summary>
        ///     The sansikrat.
        /// </summary>
        [Description("سنسکرت")]
        Sansikrat,

        /// <summary>
        ///     The hindi.
        /// </summary>
        [Description("ھندی")]
        Hindi,

        /// <summary>
        ///     The persian.
        /// </summary>
        [Description("فارسی")]
        Persian,

        /// <summary>
        ///     The arabic.
        /// </summary>
        [Description("عربی")]
        Arabic,

        /// <summary>
        ///     The turkish.
        /// </summary>
        [Description("ترک")]
        Turkish,

        /// <summary>
        ///     The english.
        /// </summary>
        [Description("انگریزی")]
        English,

        /// <summary>
        ///     The french.
        /// </summary>
        [Description("فرانسیسی")]
        French,

        /// <summary>
        ///     The prakrit.
        /// </summary>
        [Description("پراکرت")]
        Prakrit,

        /// <summary>
        ///     The latin.
        /// </summary>
        [Description("لاطینی")]
        Latin,

        /// <summary>
        ///     The punjabi.
        /// </summary>
        [Description("پنجابی")]
        Punjabi,

        /// <summary>
        ///     The greek.
        /// </summary>
        [Description("یونانی")]
        Greek,

        /// <summary>
        ///     The hebrew.
        /// </summary>
        [Description("عبرانی")]
        Hebrew,

        /// <summary>
        ///     The portugeese.
        /// </summary>
        [Description("پرتگالی")]
        Portugeese,

        /// <summary>
        ///     The italian.
        /// </summary>
        [Description("اطالوی")]
        Italian,

        /// <summary>
        ///     The spanish.
        /// </summary>
        [Description("ھسپانوی")]
        Spanish,

        /// <summary>
        ///     The chinese.
        /// </summary>
        [Description("چینی")]
        Chinese,

        /// <summary>
        ///     The japanese.
        /// </summary>
        [Description("جاپانی")]
        Japanese,

        /// <summary>
        ///     The sindhi.
        /// </summary>
        [Description("سندھی")]
        Sindhi,

        /// <summary>
        ///     The german.
        /// </summary>
        [Description("جرمن")]
        German,

        /// <summary>
        ///     The local.
        /// </summary>
        [Description("مقامی")]
        Local,

        /// <summary>
        ///     The syriac.
        /// </summary>
        [Description("سریانی")]
        Syriac,

        /// <summary>
        ///     The avestan.
        /// </summary>
        [Description("اویستا")]
        Avestan
    }
}
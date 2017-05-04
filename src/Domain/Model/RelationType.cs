// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelationType.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   The relation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.ComponentModel;

namespace Inshapardaz.Domain.Model
{
    /// <summary>
    ///     The relation type.
    /// </summary>
    public enum RelationType
    {
        /// <summary>
        ///     The synonym.
        /// </summary>
        [Description("مترادف")]
        Synonym,

        /// <summary>
        ///     The acronym.
        /// </summary>
        [Description("متضاد")]
        Acronym,

        /// <summary>
        ///     The compund.
        /// </summary>
        [Description("مرکب")]
        Compound,

        /// <summary>
        ///     The variation.
        /// </summary>
        [Description("تغیرات")]
        Variation,

        /// <summary>
        ///     The singular.
        /// </summary>
        [Description("واحد")]
        Singular,

        /// <summary>
        ///     The plural.
        /// </summary>
        [Description("جمع")]
        Plural,

        /// <summary>
        ///     The jama ghair nadai.
        /// </summary>
        [Description("جمع غیر ندائی")]
        JamaGhairNadai,

        /// <summary>
        ///     The wahid ghair nadai.
        /// </summary>
        [Description("واحد غیر ندائی")]
        WahidGhairNadai,

        /// <summary>
        ///     The jama istasnai.
        /// </summary>
        [Description("جمع اثتثنائی")]
        JamaIstasnai,

        /// <summary>
        ///     The opposite gender.
        /// </summary>
        [Description("جنس مخالف")]
        OppositeGender,

        /// <summary>
        ///     The form of feal.
        /// </summary>
        [Description("حالت‌‌‌ فعل")]
        FormOfFeal,

        /// <summary>
        ///     The halat.
        /// </summary>
        [Description("جالت")]
        Halat,

        /// <summary>
        ///     The halat mafooli.
        /// </summary>
        [Description("حالت مفعولی")]
        HalatMafooli,

        /// <summary>
        ///     The halat izafi.
        /// </summary>
        [Description("حالت اضافی")]
        HalatIzafi,

        /// <summary>
        ///     The halat tafseeli.
        /// </summary>
        [Description("حالت تفصیلی")]
        HalatTafseeli,

        /// <summary>
        ///     The jama nadai.
        /// </summary>
        [Description("جمع ندائی")]
        JamaNadai,

        /// <summary>
        ///     The halat faili.
        /// </summary>
        [Description("حالت فائلی")]
        HalatFaili,

        /// <summary>
        ///     The halat takhsis.
        /// </summary>
        [Description("حالت تخصیص")]
        HalatTakhsis,

        /// <summary>
        ///     The wahid nadai.
        /// </summary>
        [Description("واحد ندائی")]
        WahidNadai,

        /// <summary>
        ///     The takabuli.
        /// </summary>
        [Description("تقابلی")]
        Takabuli,

        [Description("استعمال")]
        Usage
    }
}
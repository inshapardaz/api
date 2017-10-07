// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GrammaticalType.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   The grammatical type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;

namespace Inshapardaz.Domain.Database.Entities
{
    /// <summary>
    ///     The grammatical type.
    /// </summary>
    [Flags]
    public enum GrammaticalType : long
    {
        /// <summary>
        ///     The none.
        /// </summary>
        [Description("نامعلوم")]
        None = 0x0000000000000000,

        /// <summary>
        ///     The male.
        /// </summary>
        [Description("مذکر")]
        Male = 0x0000000000000001,

        /// <summary>
        ///     The female.
        /// </summary>
        [Description("مؤنث")]
        Female = 0x0000000000000010, 

        /// <summary>
        ///     The singular.
        /// </summary>
        [Description("واحد")]
        Singular = 0x0000000000000100, 

        /// <summary>
        ///     The plural.
        /// </summary>
        [Description("جمع")]
        Plural = 0x0000000000001000, 

        /// <summary>
        ///     The ism.
        /// </summary>
        [Description("اسم")]
        Ism = 0x0000000000010000, 

        /// <summary>
        ///     The sift.
        /// </summary>
        [Description("صفت")]
        Sift = 0x0000000000020000, 

        /// <summary>
        ///     The feal.
        /// </summary>
        [Description("فعل")]
        Feal = 0x0000000000030000, 

        /// <summary>
        ///     The harf.
        /// </summary>
        [Description("حرف")]
        Harf = 0x0000000000040000, 

        /// <summary>
        ///     The ism nakra.
        /// </summary>
        [Description("اسم نکرہ")]
        IsmNakra = 0x0000000000110000, 

        /// <summary>
        ///     The ism ala.
        /// </summary>
        [Description("اسم آلہ")]
        IsmAla = 0x0000000001110000, 

        /// <summary>
        ///     The ism soot.
        /// </summary>
        [Description("اسم صوت")]
        IsmSoot = 0x0000000002110000, 

        /// <summary>
        ///     The ism tashgir.
        /// </summary>
        [Description("اسم تصغیر")]
        IsmTashgir = 0x0000000003110000, 

        /// <summary>
        ///     The ism masghar.
        /// </summary>
        [Description("اسم مصغّر")]
        IsmMasghar = 0x0000000004110000, 

        /// <summary>
        ///     The ism mukabbar.
        /// </summary>
        [Description("اسم مکبر")]
        IsmMukabbar = 0x0000000005110000, 

        /// <summary>
        ///     The ism zarf.
        /// </summary>
        [Description("اسم ظرف")]
        IsmZarf = 0x0000000006110000, 

        /// <summary>
        ///     The ism zarf makan.
        /// </summary>
        [Description("اسم ظرف مکاں")]
        IsmZarfMakan = 0x0000000016110000, 

        /// <summary>
        ///     The ism zarf zaman.
        /// </summary>
        [Description("اسم ظرف زماں")]
        IsmZarfZaman = 0x0000000026110000, 

        /// <summary>
        ///     The ism jama.
        /// </summary>
        [Description("اسم جمع")]
        IsmJama = 0x0000000036110000,

        /// <summary>
        ///     The ism muarfa.
        /// </summary>
        [Description("اسم معرفہ")]
        IsmMuarfa = 0x0000000000210000, 

        /// <summary>
        ///     The ism alam.
        /// </summary>
        [Description("اسم علم")]
        IsmAlam = 0x0000000001210000, 

        /// <summary>
        ///     The khitaab.
        /// </summary>
        [Description("خطاب")]
        Khitaab = 0x0000000011210000,

        /// <summary>
        ///     The lakab.
        /// </summary>
        [Description("لقب")]
        Lakab = 0x0000000021210000,

        /// <summary>
        ///     The takhallus.
        /// </summary>
        [Description("تخلّص")]
        Takhallus = 0x0000000031210000,

        /// <summary>
        ///     The kunniyat.
        /// </summary>
        [Description("کنّیت")]
        Kunniyat = 0x0000000041210000,

        /// <summary>
        ///     The urf.
        /// </summary>
        [Description("عرف")]
        Urf = 0x0000000051210000,

        /// <summary>
        ///     The ism zameer.
        /// </summary>
        [Description("اسم ضمیر")]
        IsmZameer = 0x0000000000310000, 

        /// <summary>
        ///     The zameer shakhsi.
        /// </summary>
        [Description("ضمیر شخصی")]
        ZameerShakhsi = 0x0000000001310000, 

        /// <summary>
        ///     The ghaib.
        /// </summary>
        [Description("ضمیر شخصی غائب")]
        Ghaib = 0x0000000011310000, 

        /// <summary>
        ///     The hazir.
        /// </summary>
        [Description("ضمیر شخصی حاضر")]
        Hazir = 0x0000000021310000, 

        /// <summary>
        ///     The mutakallam.
        /// </summary>
        [Description("ضمیر شخصی متکلم")]
        Mutakallam = 0x0000000031310000, 

        /// <summary>
        ///     The mukhatib.
        /// </summary>
        [Description("ضمیر شخصی مخاطب")]
        Mukhatib = 0x0000000041310000, 

        /// <summary>
        ///     The zameer ishara.
        /// </summary>
        [Description("ضمیر اشارہ")]
        ZameerIshara = 0x0000000002310000, 

        /// <summary>
        ///     The zameer ishara kareeb.
        /// </summary>
        [Description("ضمیر اشارہ قریب")]
        ZameerIsharaKareeb = 0x0000000012310000,

        /// <summary>
        ///     The zameer ishara baeed.
        /// </summary>
        [Description("ضمیر اشارہ بعید")]
        ZameerIsharaBaeed = 0x0000000022310000,

        /// <summary>
        ///     The zameer istafham.
        /// </summary>
        [Description("ضمیر استفہام")]
        ZameerIstafham = 0x0000000003310000, 

        /// <summary>
        ///     The zameer mosula.
        /// </summary>
        [Description("ضمیر موصولہ")]
        ZameerMosula = 0x0000000004310000, 

        /// <summary>
        ///     The zameer tankeer.
        /// </summary>
        [Description("ضمیر تنکیر")]
        ZameerTankeer = 0x0000000005310000,

        /// <summary>
        ///     The ism ishara.
        /// </summary>
        [Description("اسم اشارہ")]
        IsmIshara = 0x0000000000410000, 

        /// <summary>
        ///     The ism ishara kareeb.
        /// </summary>
        [Description("اسم اشارہ قریب")]
        IsmIsharaKareeb = 0x0000000001410000, 

        /// <summary>
        ///     The ism ishara baeed.
        /// </summary>
        [Description("ضمیر اشارہ بعید")]
        IsmIsharaBaeed = 0x0000000002410000,

        /// <summary>
        ///     The ism mosool.
        /// </summary>
        [Description("اسم موصول")]
        IsmMosool = 0x0000000000510000,

        /// <summary>
        ///     The ism mujarrad.
        /// </summary>
        [Description("اسم مجرد")]
        IsmMujarrad = 0x0000000000310000, 

        /// <summary>
        ///     The ism kaifiat.
        /// </summary>
        [Description("اسم کیفیت")]
        IsmKaifiat = 0x0000000000410000, 

        /// <summary>
        ///     The ism hasil masdar.
        /// </summary>
        [Description("اسم حاصل مصدر")]
        IsmHasilMasdar = 0x0000000000510000, 

        /// <summary>
        ///     The ism jamid.
        /// </summary>
        [Description("اسم جامد")]
        IsmJamid = 0x0000000000610000, 

        /// <summary>
        ///     The ism maada.
        /// </summary>
        [Description("اسم مادہ")]
        IsmMaada = 0x0000000000710000, 

        /// <summary>
        ///     The ism addad.
        /// </summary>
        [Description("اسم عدد")]
        IsmAddad = 0x0000000000810000, 

        /// <summary>
        ///     The ism muawqza.
        /// </summary>
        [Description("اسم معاوضہ")]
        IsmMuawqza = 0x0000000000910000,

        /// <summary>
        ///     The ism tashgeer.
        /// </summary>
        [Description("اسم تصغیر")]
        IsmTashgeer = 0x0000000000A10000,

        /// <summary>
        ///     The sift zati.
        /// </summary>
        [Description("صفت ذاتی")]
        SiftZati = 0x0000000000120000,

        /// <summary>
        ///     The sift nisbati.
        /// </summary>
        [Description("صفت نسبتی")]
        SiftNisbati = 0x0000000000220000,

        /// <summary>
        ///     The sift adadi.
        /// </summary>
        [Description("صفت عددی")]
        SiftAdadi = 0x0000000000320000, 

        /// <summary>
        ///     The sift miqdari.
        /// </summary>
        [Description("صفت مقداری")]
        SiftMiqdari = 0x0000000000420000, 

        /// <summary>
        ///     The sift ishara.
        /// </summary>
        [Description("صفت اشارہ ")]
        SiftIshara = 0x0000000000520000,

        /// <summary>
        ///     The sift mushba.
        /// </summary>
        [Description("صفت مشبّہ")]
        SiftMushba = 0x0000000000620000,

        /// <summary>
        ///     The feal mutaddi.
        /// </summary>
        [Description("فعل متعدی")]
        FealMutaddi = 0x0000000000130000,

        /// <summary>
        ///     The feal lazim.
        /// </summary>
        [Description("فعل لازم")]
        FealLazim = 0x0000000000230000,

        /// <summary>
        ///     The feal nakis.
        /// </summary>
        [Description("فعل ناقص")]
        FealNakis = 0x0000000000330000, 

        /// <summary>
        ///     The feal imdadi.
        /// </summary>
        [Description("فعل امدادی")]
        FealImdadi = 0x0000000000430000,

        /// <summary>
        ///     The feal taam.
        /// </summary>
        [Description("فعل تام")]
        FealTaam = 0x0000000000530000,

        /// <summary>
        ///     The mutaliq feal.
        /// </summary>
        [Description("متعلق فعل")]
        MutaliqFeal = 0x0000000000630000,

        /// <summary>
        ///     The harf fijaia.
        /// </summary>
        [Description("حرف فجائیہ")]
        HarfFijaia = 0x0000000000140000,

        /// <summary>
        ///     The harf jaar.
        /// </summary>
        [Description("حرف جار")]
        HarfJaar = 0x0000000000240000,

        /// <summary>
        ///     The harf nafi.
        /// </summary>
        [Description("حرف نفی")]
        HarfNafi = 0x0000000000340000,

        /// <summary>
        ///     The harf duaiya.
        /// </summary>
        [Description("حرف دعائیہ")]
        HarfDuaiya = 0x0000000000440000,

        /// <summary>
        ///     The harf tashbih.
        /// </summary>
        [Description("حرف تشبیہ")]
        HarfTashbih = 0x0000000000540000,

        /// <summary>
        ///     The harf tanbeeh.
        /// </summary>
        [Description("حرف تنبیہ")]
        HarfTanbeeh = 0x0000000000640000,

        /// <summary>
        ///     The harf tahseen.
        /// </summary>
        [Description("حرف تحسین")]
        HarfTahseen = 0x0000000000740000,

        /// <summary>
        ///     The harf istasna.
        /// </summary>
        [Description("حرف استثنا")]
        HarfIstasna = 0x0000000000840000,

        /// <summary>
        ///     The harf shart.
        /// </summary>
        [Description("حرف شرط")]
        HarfShart = 0x0000000000940000,

        /// <summary>
        ///     The harf taajub.
        /// </summary>
        [Description("حرف تعجب")]
        HarfTaajub = 0x0000000000A40000,

        /// <summary>
        ///     The harf nidaaiya.
        /// </summary>
        [Description("حرف ندائیہ")]
        HarfNidaaiya = 0x0000000000B40000,

        /// <summary>
        ///     The harf rabt.
        /// </summary>
        [Description("حرف ربط")]
        HarfRabt = 0x0000000000C40000,

        /// <summary>
        ///     The harf qasam.
        /// </summary>
        [Description("حرف قسم")]
        HarfQasam = 0x0000000000D40000,

        /// <summary>
        ///     The harf istasjab.
        /// </summary>
        [Description("حرف استعجاب")]
        HarfIstasjab = 0x0000000000E40000,

        /// <summary>
        ///     The harf istafham.
        /// </summary>
        [Description("حرف استفہام")]
        HarfIstafham = 0x0000000000F40000,

        /// <summary>
        ///     The harf tabeh.
        /// </summary>
        [Description("حرف تابع")]
        HarfTabeh = 0x0000000001040000,

        /// <summary>
        ///     The harf ejab.
        /// </summary>
        [Description("حرف ایجاب")]
        HarfEjab = 0x0000000001140000,

        /// <summary>
        ///     The harf jaza.
        /// </summary>
        [Description("حرف جزا")]
        HarfJaza = 0x0000000001240000,

        /// <summary>
        ///     The harf tahajji.
        /// </summary>
        [Description("حرف تہجی")]
        HarfTahajji = 0x0000000001340000,

        /// <summary>
        ///     The harf soot.
        /// </summary>
        [Description("حرف صوت")]
        HarfSoot = 0x0000000001440000,

        /// <summary>
        ///     The harf izafat.
        /// </summary>
        [Description("حرف اضافت")]
        HarfIzafat = 0x0000000001540000,

        /// <summary>
        ///     The harf nida.
        /// </summary>
        [Description("حرف ندا")]
        HarfNida = 0x0000000001640000,

        /// <summary>
        ///     The harf tardeed.
        /// </summary>
        [Description("حرف تردید")]
        HarfTardeed = 0x0000000001740000,

        /// <summary>
        ///     The harf takeed.
        /// </summary>
        [Description("حرف تاکید")]
        HarfTakeed = 0x0000000001840000,

        /// <summary>
        ///     The harf ataf.
        /// </summary>
        [Description("حرف عطف")]
        HarfAtaf = 0x0000000001940000,

        /// <summary>
        ///     The harf tanaffar.
        /// </summary>
        [Description("حرف تنفر")]
        HarfTanaffar = 0x0000000001A40000,

        /// <summary>
        ///     The harf biyaniya.
        /// </summary>
        [Description("حرف بیانیہ")]
        HarfBiyaniya = 0x0000000001B40000,

        /// <summary>
        ///     The harf istadraak.
        /// </summary>
        [Description("حرف استدراک")]
        HarfIstadraak = 0x0000000001C40000,

        /// <summary>
        ///     The harf illat.
        /// </summary>
        [Description("حرف علت")]
        HarfIllat = 0x0000000001D40000,

        /// <summary>
        ///     The harf takhsees.
        /// </summary>
        [Description("حرف تخصیص")]
        HarfTakhsees = 0x0000000001E40000,

        /// <summary>
        ///     The harf tamanna.
        /// </summary>
        [Description("حرف تمنا")]
        HarfTamanna = 0x0000000001F40000,
    }
}
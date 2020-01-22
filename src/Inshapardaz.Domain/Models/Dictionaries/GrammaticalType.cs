using System;
using System.ComponentModel;

namespace Inshapardaz.Domain.Models.Dictionaries
{
    [Flags]
    public enum GrammaticalType : long
    {
        [Description("")]
        None = 0x0000000000000000,
        [Description("مذکر")]
        Male = 0x0000000000000001,
        [Description("مؤنث")]
        Female = 0x0000000000000010,
        [Description("واحد")]
        Singular = 0x0000000000000100,
        [Description("جمع")]
        Plural = 0x0000000000001000,
        [Description("اسم")]
        Ism = 0x0000000000010000,
        [Description("صفت")]
        Sift = 0x0000000000020000,
        [Description("فعل")]
        Feal = 0x0000000000030000,
        [Description("حرف")]
        Harf = 0x0000000000040000,
        [Description("اسم نکرہ")]
        IsmNakra = 0x0000000000110000,
        [Description("اسم آلہ")]
        IsmAla = 0x0000000001110000,
        [Description("اسم صوت")]
        IsmSoot = 0x0000000002110000,
        [Description("اسم تصغیر")]
        IsmTashgir = 0x0000000003110000,
        [Description("اسم مصغّر")]
        IsmMasghar = 0x0000000004110000,
        [Description("اسم مکبر")]
        IsmMukabbar = 0x0000000005110000,
        [Description("اسم ظرف")]
        IsmZarf = 0x0000000006110000,
        [Description("اسم ظرف مکاں")]
        IsmZarfMakan = 0x0000000016110000,
        [Description("اسم ظرف زماں")]
        IsmZarfZaman = 0x0000000026110000,
        [Description("اسم جمع")]
        IsmJama = 0x0000000036110000,
        [Description("اسم معرفہ")]
        IsmMuarfa = 0x0000000000210000,
        [Description("اسم علم")]
        IsmAlam = 0x0000000001210000,
        [Description("خطاب")]
        Khitaab = 0x0000000011210000,
        [Description("لقب")]
        Lakab = 0x0000000021210000,
        [Description("تخلّص")]
        Takhallus = 0x0000000031210000,
        [Description("کنّیت")]
        Kunniyat = 0x0000000041210000,
        [Description("عرف")]
        Urf = 0x0000000051210000,
        [Description("اسم ضمیر")]
        IsmZameer = 0x0000000000310000,
        [Description("ضمیر شخصی")]
        ZameerShakhsi = 0x0000000001310000,
        [Description("ضمیر شخصی غائب")]
        Ghaib = 0x0000000011310000,
        [Description("ضمیر شخصی حاضر")]
        Hazir = 0x0000000021310000,
        [Description("ضمیر شخصی متکلم")]
        Mutakallam = 0x0000000031310000,
        [Description("ضمیر شخصی مخاطب")]
        Mukhatib = 0x0000000041310000,
        [Description("ضمیر اشارہ")]
        ZameerIshara = 0x0000000002310000,
        [Description("ضمیر اشارہ قریب")]
        ZameerIsharaKareeb = 0x0000000012310000,
        [Description("ضمیر اشارہ بعید")]
        ZameerIsharaBaeed = 0x0000000022310000,
        [Description("ضمیر استفہام")]
        ZameerIstafham = 0x0000000003310000,
        [Description("ضمیر موصولہ")]
        ZameerMosula = 0x0000000004310000,
        [Description("ضمیر تنکیر")]
        ZameerTankeer = 0x0000000005310000,
        [Description("اسم اشارہ")]
        IsmIshara = 0x0000000000410000,
        [Description("اسم اشارہ قریب")]
        IsmIsharaKareeb = 0x0000000001410000,
        [Description("ضمیر اشارہ بعید")]
        IsmIsharaBaeed = 0x0000000002410000,
        [Description("اسم موصول")]
        IsmMosool = 0x0000000000510000,
        [Description("اسم مجرد")]
        IsmMujarrad = 0x0000000000310000,
        [Description("اسم حاصل مصدر")]
        IsmHasilMasdar = 0x0000000000510000,
        [Description("اسم کیفیت")]
        IsmKaifiat = 0x0000000000610000,
        [Description("اسم جامد")]
        IsmJamid = 0x0000000000710000,
        [Description("اسم مادہ")]
        IsmMaada = 0x0000000000810000,
        [Description("اسم عدد")]
        IsmAddad = 0x0000000000910000,
        [Description("اسم معاوضہ")]
        IsmMuawqza = 0x0000000000A10000,
        [Description("اسم تصغیر")]
        IsmTashgeer = 0x0000000000B10000,
        [Description("صفت ذاتی")]
        SiftZati = 0x0000000000120000,
        [Description("صفت نسبتی")]
        SiftNisbati = 0x0000000000220000,
        [Description("صفت عددی")]
        SiftAdadi = 0x0000000000320000,
        [Description("صفت مقداری")]
        SiftMiqdari = 0x0000000000420000,
        [Description("صفت اشارہ ")]
        SiftIshara = 0x0000000000520000,
        [Description("صفت مشبّہ")]
        SiftMushba = 0x0000000000620000,
        [Description("فعل متعدی")]
        FealMutaddi = 0x0000000000130000,
        [Description("فعل لازم")]
        FealLazim = 0x0000000000230000,
        [Description("فعل ناقص")]
        FealNakis = 0x0000000000330000,
        [Description("فعل امدادی")]
        FealImdadi = 0x0000000000430000,
        [Description("فعل تام")]
        FealTaam = 0x0000000000530000,
        [Description("متعلق فعل")]
        MutaliqFeal = 0x0000000000630000,
        [Description("متعلق مرکب")]
        FealMurakkab = 0x0000000000730000,
        [Description("حرف فجائیہ")]
        HarfFijaia = 0x0000000000140000,
        [Description("حرف جار")]
        HarfJaar = 0x0000000000240000,
        [Description("حرف نفی")]
        HarfNafi = 0x0000000000340000,
        [Description("حرف دعائیہ")]
        HarfDuaiya = 0x0000000000440000,
        [Description("حرف تشبیہ")]
        HarfTashbih = 0x0000000000540000,
        [Description("حرف تنبیہ")]
        HarfTanbeeh = 0x0000000000640000,
        [Description("حرف تحسین")]
        HarfTahseen = 0x0000000000740000,
        [Description("حرف استثنا")]
        HarfIstasna = 0x0000000000840000,
        [Description("حرف شرط")]
        HarfShart = 0x0000000000940000,
        [Description("حرف تعجب")]
        HarfTaajub = 0x0000000000A40000,
        [Description("حرف ندائیہ")]
        HarfNidaaiya = 0x0000000000B40000,
        [Description("حرف ربط")]
        HarfRabt = 0x0000000000C40000,
        [Description("حرف قسم")]
        HarfQasam = 0x0000000000D40000,
        [Description("حرف استعجاب")]
        HarfIstasjab = 0x0000000000E40000,
        [Description("حرف استفہام")]
        HarfIstafham = 0x0000000000F40000,
        [Description("حرف تابع")]
        HarfTabeh = 0x0000000001040000,
        [Description("حرف ایجاب")]
        HarfEjab = 0x0000000001140000,
        [Description("حرف جزا")]
        HarfJaza = 0x0000000001240000,
        [Description("حرف تہجی")]
        HarfTahajji = 0x0000000001340000,
        [Description("حرف صوت")]
        HarfSoot = 0x0000000001440000,
        [Description("حرف اضافت")]
        HarfIzafat = 0x0000000001540000,
        [Description("حرف ندا")]
        HarfNida = 0x0000000001640000,
        [Description("حرف تردید")]
        HarfTardeed = 0x0000000001740000,
        [Description("حرف تاکید")]
        HarfTakeed = 0x0000000001840000,
        [Description("حرف عطف")]
        HarfAtaf = 0x0000000001940000,
        [Description("حرف تنفر")]
        HarfTanaffar = 0x0000000001A40000,
        [Description("حرف بیانیہ")]
        HarfBiyaniya = 0x0000000001B40000,
        [Description("حرف استدراک")]
        HarfIstadraak = 0x0000000001C40000,
        [Description("حرف علت")]
        HarfIllat = 0x0000000001D40000,
        [Description("حرف تخصیص")]
        HarfTakhsees = 0x0000000001E40000,
        [Description("حرف تمنا")]
        HarfTamanna = 0x0000000001F40000,
        [Description("جمع الجمع")]
        JamaUlJama = 0x0000000000050000,
        [Description("مبحول")]
        Majhool = 0x0000000000060000,
        [Description("معروف")]
        Maroof = 0x0000000000070000,
        [Description("محاورہ")]
        Proverb = 0x0000000000080000,
        [Description("کہاوت")]
        Saying = 0x0000000000090000,
        [Description("سابقہ")]
        PreFix = 0x00000000000A0000,
        [Description("لاحقہ")]
        PostFix = 0x00000000000B0000
    }
}

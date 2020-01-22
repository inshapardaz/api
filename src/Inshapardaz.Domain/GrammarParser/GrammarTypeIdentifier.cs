using System;
using Inshapardaz.Domain.Models.Dictionaries;

namespace Inshapardaz.Domain.GrammarParser
{
    public static class GrammarTypeIdentifier
    {
        public static bool IsIsmSift(this WordModel w)
        {
            return (w.Attributes & GrammaticalType.Sift ) == GrammaticalType.Sift;
        }

        public static bool IsIsm(this WordModel w)
        {
            if ((w.Attributes & GrammaticalType.Ism) == GrammaticalType.Ism)
            {
                if ((w.Attributes & GrammaticalType.Sift) != GrammaticalType.Sift &&
                        (w.Attributes & GrammaticalType.IsmIshara) != GrammaticalType.IsmIshara)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsIsmIshara(this WordModel w)
        {
            return (w.Attributes & GrammaticalType.IsmIshara) == GrammaticalType.IsmIshara;
        }

        public static bool IsHarf(this WordModel w)
        {
            if ((w.Attributes & GrammaticalType.Harf) == GrammaticalType.Harf)
            {
                if ((w.Attributes & GrammaticalType.HarfJaar) != GrammaticalType.HarfJaar &&
                        (w.Attributes & GrammaticalType.HarfNida) != GrammaticalType.HarfNida)
                {
                    return true;
                }
            }

            return false;
        }

        #region Haroof

        public static  bool IsHarfEJar(this WordModel w)
        {
            return (w.Attributes & GrammaticalType.HarfJaar) == GrammaticalType.HarfJaar;
        }

        public static bool IsHarfENida(this WordModel w)
        {
            return (w.Attributes & GrammaticalType.HarfNida) == GrammaticalType.HarfNida;
        }

        public static bool IsHarfETakeed(this WordModel w)
        {
            return (w.Attributes & GrammaticalType.HarfTakeed) == GrammaticalType.HarfTakeed;
        }

        #endregion

        #region Alamat

        public static bool IsAlamatNafi(this WordModel w)
        {
            return (w.Attributes & GrammaticalType.HarfNafi) == GrammaticalType.HarfNafi;
        }
        
        public static bool IsAlamatFail(this WordModel w)
        {
            return w.Title == "نے";
        }

        public static bool IsAlamatMafool(this WordModel w)
        {
            return w.Title == "کو";
        }

        #endregion
        public static bool IsAmr(this WordModel w)
        {
            return w.Title == "آ";
        }

        public static bool IsFealNaqis(this WordModel w)
        {
            return (w.Attributes & GrammaticalType.FealNakis) == GrammaticalType.FealNakis;
        }

        public static bool IsFealNaqisHall(this WordModel w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFealNaqisMazi(this WordModel w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFealNaqisMustaqbil(this WordModel w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFeal(this WordModel w)
        {
            return (w.Attributes & GrammaticalType.Feal) == GrammaticalType.Feal;
        }

        public static bool IsFealHaal(this WordModel w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFealMuzaray(this WordModel w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFealMaziMutlaq(this WordModel w)
        {
            throw new NotImplementedException();
        }

        public static bool IsMight(this WordModel w)
        {
            var word = w.Title;
            return word == "ہوگا" ||
                   word == "ہو گی" ||
                   word == "ہونگا" ||
                   word == "ہونگی" ||
                   word == "ہونگے" ||
                   word == "ہونگی";
        }

        public static bool IsWill(this WordModel w)
        {
            var word = w.Title;
            return word == "گا" ||
                   word == "گی" ||
                   word == "گے" ;
        }
        public static bool IsUsedTo(this WordModel w)
        {
            var word = w.Title;
            return word == "کرتا" ||
                   word == "کربی" ||
                   word == "کربتے" ||
                   word == "کرتیں";
        }
        public static bool IsWould(this WordModel w)
        {
            var word = w.Title;
            return word == "ہوتا" ||
                   word == "ہوتی" ||
                   word == "ہوتے" ||
                   word == "ہوتیں";
        }   

        public static bool IsHo(this WordModel w)
        {
            var word = w.Title;
            return word == "ہو";
        }
        public static bool IsDoing(this WordModel w)
        {
            var word = w.Title;
            return word == "رہا" ||
                   word == "رہی" ||
                   word == "رہے" ||
                   word == "رہیں";
        }
        public static bool IsHad(this WordModel w)
        {
            var word = w.Title;
            return word == "چکا" ||
                   word == "چکی" ||
                   word == "چکے" ||
                   word == "چکیں";
        }
    }
}

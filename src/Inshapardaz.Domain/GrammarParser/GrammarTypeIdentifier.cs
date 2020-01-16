using System;
using Inshapardaz.Domain.Entities.Dictionaries;

namespace Inshapardaz.Domain.GrammarParser
{
    public static class GrammarTypeIdentifier
    {
        public static bool IsIsmSift(this Word w)
        {
            return (w.Attributes & GrammaticalType.Sift ) == GrammaticalType.Sift;
        }

        public static bool IsIsm(this Word w)
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

        public static bool IsIsmIshara(this Word w)
        {
            return (w.Attributes & GrammaticalType.IsmIshara) == GrammaticalType.IsmIshara;
        }

        public static bool IsHarf(this Word w)
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

        public static  bool IsHarfEJar(this Word w)
        {
            return (w.Attributes & GrammaticalType.HarfJaar) == GrammaticalType.HarfJaar;
        }

        public static bool IsHarfENida(this Word w)
        {
            return (w.Attributes & GrammaticalType.HarfNida) == GrammaticalType.HarfNida;
        }

        public static bool IsHarfETakeed(this Word w)
        {
            return (w.Attributes & GrammaticalType.HarfTakeed) == GrammaticalType.HarfTakeed;
        }

        #endregion

        #region Alamat

        public static bool IsAlamatNafi(this Word w)
        {
            return (w.Attributes & GrammaticalType.HarfNafi) == GrammaticalType.HarfNafi;
        }
        
        public static bool IsAlamatFail(this Word w)
        {
            return w.Title == "نے";
        }

        public static bool IsAlamatMafool(this Word w)
        {
            return w.Title == "کو";
        }

        #endregion
        public static bool IsAmr(this Word w)
        {
            return w.Title == "آ";
        }

        public static bool IsFealNaqis(this Word w)
        {
            return (w.Attributes & GrammaticalType.FealNakis) == GrammaticalType.FealNakis;
        }

        public static bool IsFealNaqisHall(this Word w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFealNaqisMazi(this Word w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFealNaqisMustaqbil(this Word w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFeal(this Word w)
        {
            return (w.Attributes & GrammaticalType.Feal) == GrammaticalType.Feal;
        }

        public static bool IsFealHaal(this Word w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFealMuzaray(this Word w)
        {
            throw new NotImplementedException();
        }

        public static bool IsFealMaziMutlaq(this Word w)
        {
            throw new NotImplementedException();
        }

        public static bool IsMight(this Word w)
        {
            var word = w.Title;
            return word == "ہوگا" ||
                   word == "ہو گی" ||
                   word == "ہونگا" ||
                   word == "ہونگی" ||
                   word == "ہونگے" ||
                   word == "ہونگی";
        }

        public static bool IsWill(this Word w)
        {
            var word = w.Title;
            return word == "گا" ||
                   word == "گی" ||
                   word == "گے" ;
        }
        public static bool IsUsedTo(this Word w)
        {
            var word = w.Title;
            return word == "کرتا" ||
                   word == "کربی" ||
                   word == "کربتے" ||
                   word == "کرتیں";
        }
        public static bool IsWould(this Word w)
        {
            var word = w.Title;
            return word == "ہوتا" ||
                   word == "ہوتی" ||
                   word == "ہوتے" ||
                   word == "ہوتیں";
        }   

        public static bool IsHo(this Word w)
        {
            var word = w.Title;
            return word == "ہو";
        }
        public static bool IsDoing(this Word w)
        {
            var word = w.Title;
            return word == "رہا" ||
                   word == "رہی" ||
                   word == "رہے" ||
                   word == "رہیں";
        }
        public static bool IsHad(this Word w)
        {
            var word = w.Title;
            return word == "چکا" ||
                   word == "چکی" ||
                   word == "چکے" ||
                   word == "چکیں";
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


namespace HlidacStatu.Util
{
    public static class ParseTools
    {



        public static bool EnoughExtractedTextCheck(long words, long lengthChars, long uniqueWordsCount, decimal wordsVariance)
        {

            return !(lengthChars <= 20 || words <= 10 ||                
                (
                    uniqueWordsCount<7 || (uniqueWordsCount >=7 && wordsVariance > 0.7m  )
                )
                
            );
        }

        public static bool EnoughExtractedTextCheck(string plaintext)
        {
            var words = ParseTools.CountWords(plaintext);
            var length = plaintext?.Length ?? 0;
            var variance = Devmasters.TextUtil.WordsVarianceInText(plaintext);
            var uniqueWordsCount = variance.Item2;
            var wordsVariance = variance.Item1;

            return EnoughExtractedTextCheck(words, length, uniqueWordsCount, wordsVariance);
        }


        /// <summary>
        /// support simple LIKE syntaxt from T-SQL 
        /// %asdf 
        /// asdf%
        /// %asdf%
        /// </summary>
        public static bool FindInStringSqlLike(string wholetext, string stringToFind, StringComparison sc = StringComparison.OrdinalIgnoreCase)
        {
            if (stringToFind.StartsWith("%") && stringToFind.EndsWith("%"))
                return wholetext.IndexOf(stringToFind.Substring(1, stringToFind.Length - 2), sc) >= 0;
            else if (stringToFind.StartsWith("%"))
                return wholetext.EndsWith(stringToFind.Substring(1, stringToFind.Length - 1), sc);
            else if (stringToFind.EndsWith("%"))
                return wholetext.StartsWith(stringToFind.Substring(0, stringToFind.Length - 1), sc);
            else
                return string.Equals(wholetext, stringToFind, sc);


        }

        static Dictionary<string, string> FunkceNormalizaction = new Dictionary<string, string>()
        {
            {"poslanec%","poslanec" },
            {"poslankyně%","poslanec" },
            {"místopředsed%","místopředseda" },
            {"předsed%","předseda" },
            {"senátor%","senátor" },
            {"evropská komisařk%","komisař eu" },
            {"evropský komisař","komisař eu" },
            {"komisař%","komisař eu" },
            {"viceprezident%","viceprezident" },
            {"prezident%","prezident" },
            {"ministr%","ministr" },
            {"místopředsedkyn%","místopředseda" },
            {"starost%","starosta" },
            {"hejtman%","hejtman" },
            {"primátor%","primátor" },
            {"guvernér%","guvernér" },
            {"ředitel%","ředitel" },
            {"náměstkyn%","náměstek" },
            {"náměstek","náměstek" },
            {"%zastupitel%","zastupitel" },
        };

        public static string NormalizePolitikFunkce(string fce)
        {
            foreach (var fkv in FunkceNormalizaction)
            {
                if (FindInStringSqlLike(fce, fkv.Key))
                    return fkv.Value;
            }
            return fce;
        }

        public static string NormalizeIco(string ico)
        {
            if (ico == null) 
                return string.Empty;

            ico = ico.Trim();
            if (ico.Contains("cz-"))
                return MerkIcoToICO(ico);
            else
                return ico.PadLeft(8, '0');
        }


        public static string MerkIcoToICO(string merkIco)
        {
            if (merkIco.ToLower().Contains("cz-"))
                merkIco = merkIco.Trim().Replace("cz-", "");

            return merkIco.PadLeft(8, '0');
        }
        public static string IcoToMerkIco(string ico)
        {
            if (Devmasters.TextUtil.IsNumeric(ico))
            {
                long tmp;
                if (long.TryParse(ico.Trim(), out tmp))
                {
                    return tmp.ToString();
                }
            }

            return ico;
        }


        public static string NormalizePrilohaPlaintextText(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            //\u0000
            string ns = s.Replace("\0", "");

            //remove /n/r on the beginning
            ns = System.Text.RegularExpressions.Regex.Replace(ns, "^(\\s)*", "");
            ns = Devmasters.TextUtil.ReplaceDuplicates(ns, "\n\n");
            ns = Devmasters.TextUtil.ReplaceDuplicates(ns, "\r\r");
            ns = Devmasters.TextUtil.ReplaceDuplicates(ns, "\t\t");
            ns = ns.Trim();

            //remove /n/r on the beginning again
            ns = System.Text.RegularExpressions.Regex.Replace(ns, "^(\\s)*", "");
            return ns;
        }

        public static int CountWords(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            return collection.Count;
        }
        public static string[] GetWords(string s)
        {
            if (string.IsNullOrEmpty(s))
                return new string[] { };
            MatchCollection collection = Regex.Matches(s, @"[\S]+");
            List<string> words = new List<string>();
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Value.Length > 0)
                    words.Add(collection[i].Value);
            }

            return words.ToArray();
        }

        static Dictionary<string, string> StranyZkratky = new Dictionary<string, string>()
        {
            {"kdu","KDU-ČSL" },
            {"*ceskoslovenska strana lidova*","KDU-ČSL" },

            { "sz","Strana zelených" },
            { "zeleni","Strana zelených" },
            { "strana zelenych","Strana zelených" },

            {"starostove a nezavisli","STAN" },

            {"top09","TOP 09" },
            {"pirati","Česká pirátská strana" },
            {"ceska strana socialne demokraticka","ČSSD" },

            {"ano2011","ANO" },
            {"ano","ANO" },
            {"hnuti ano","ANO" },
            {"ano 2011, o.s.","ANO" },
            {"ano 2011","ANO" },

            {"obcanska demokraticka strana","ODS" },

            {"spo","Strana Práv Občanů ZEMANOVCI" },
            {"spoz","Strana Práv Občanů ZEMANOVCI" },
            {"strana prav obcanu","Strana Práv Občanů ZEMANOVCI" },
            {"strana prav obcanu zemanovci","Strana Práv Občanů ZEMANOVCI" },

            {"svoboda a prima demokracie","SPD" },
            {"spd","SPD" },
            {"svoboda a prima demokracie*","SPD" },

            {"strana svobodnych obcanu","Svobodní" },
            {"usvit - narodni koalice","Úsvit" },
            {"usvit-narodni koalice","Úsvit" },

            {"komunisticka strana cech a moravy","KSČM" },
            {"lev 21","Národní socialisté" },
            {"*rozumni*","ROZUMNÍ" },
            {"vv","Věci veřejné" },
            {"veci verejne","Věci veřejné" },
};

        public static string NormalizaceStranaShortName(string strana)
        {
            if (string.IsNullOrWhiteSpace(strana))
                return string.Empty;

            var s = strana.ToLower();
            s = Devmasters.TextUtil.ReplaceDuplicates(s, ' ');
            s = Devmasters.TextUtil.RemoveDiacritics(s);

            string[] vals = StranyZkratky.Keys
                    //.Union(StranyZkratky
                    //            .Values
                    //            .Select(m => Devmasters.TextUtil.RemoveDiacritics(m).ToLower())
                    //            .Distinct()
                    //            )
                    .ToArray();

            foreach (var val in vals)
            {
                if (val.StartsWith("*") && val.EndsWith("*"))
                {
                    if (s.Contains(val.Replace("*", "")))
                        return StranyZkratky[val];
                }
                if (val.StartsWith("*"))
                {
                    if (s.EndsWith(val.Replace("*", "")))
                        return StranyZkratky[val];
                }
                if (val.EndsWith("*"))
                {
                    if (s.StartsWith(val.Replace("*", "")))
                        return StranyZkratky[val];
                }
                else if (s == val)
                    return StranyZkratky[val];
            }
            return strana;

        }

        public static int? ToInt(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            int ret = 0;
            if (int.TryParse(value, out ret))
            {
                return ret;
            }
            else
                return null;
        }

        public static decimal? FromTextToDecimal(string value)
        {
            return ToDecimal(Devmasters.TextUtil.NormalizeToNumbersOnly(value));
        }

        public static decimal? ToDecimal(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            value = value.Replace(" ", "").Trim();
            if (value.StartsWith(",") || value.StartsWith("."))
                value = value.Substring(1);
            if (value.EndsWith(",") || value.EndsWith("."))
                value = value.Substring(0, value.Length - 1);
            if (value.EndsWith(",-") || value.EndsWith(".-"))
                value = value.Substring(0, value.Length - 2);
            if (value.EndsWith(",--") || value.EndsWith(".--"))
                value = value.Substring(0, value.Length - 3);
            if (value.EndsWith(",00") || value.EndsWith(".00"))
                value = value.Substring(0, value.Length - 3);

            System.Globalization.NumberStyles styles = System.Globalization.NumberStyles.AllowLeadingWhite
                | System.Globalization.NumberStyles.AllowTrailingWhite
                | System.Globalization.NumberStyles.AllowThousands
                | System.Globalization.NumberStyles.AllowCurrencySymbol
                ;
            decimal tmp;
            if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, Consts.czCulture, out tmp))
                return tmp;
            else if (decimal.TryParse(value, System.Globalization.NumberStyles.Any, Consts.enCulture, out tmp))
                return tmp;
            else
                return null;
        }



        public static string ValueFromTheBestRegex(this string txt, string[] regexs, string groupname)
        {
            return GetValueFromTheBestRegex(txt, regexs, groupname);
        }
        public static string GetValueFromTheBestRegex(string txt, string[] regexs, string groupname)
        {
            string[] rets = GetValuesFromTheBestRegex(txt, regexs, groupname);
            if (rets != null && rets.Length > 0)
                return rets[0];
            else
                return null;
        }


        public static string[] ValuesFromTheBestRegex(this string txt, string[] regexs, string groupname)
        {
            return GetValuesFromTheBestRegex(txt, regexs, groupname);
        }
        public static string[] GetValuesFromTheBestRegex(string txt, string[] regexs, string groupname)
        {
            string[] ret = null;
            foreach (var r in regexs)
            {
                ret = GetRegexGroupValues(txt, r, groupname);
                if (ret != null && ret.Length > 0)
                    return ret;
            }

            return new string[] { };
        }

        public static string RegexGroupValue(this string txt, string regex, string groupname)
        {
            return GetRegexGroupValue(txt, regex, groupname);
        }

        public static string GetRegexGroupValue(string txt, string regex, string groupname)
        {
            if (string.IsNullOrEmpty(txt))
                return null;
            Regex myRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            foreach (Match match in myRegex.Matches(txt))
            {
                if (match.Success)
                {
                    if (match.Groups[groupname].Captures.Count > 1)
                        return match.Groups[groupname].Captures[0].Value;
                    else
                        return match.Groups[groupname].Value;
                }
            }
            return string.Empty;
        }

        public static string[] RegexGroupValues(this string txt, string regex, string groupname)
        {
            return GetRegexGroupValues(txt, regex, groupname);
        }
        public static string[] GetRegexGroupValues(string txt, string regex, string groupname)
        {
            if (string.IsNullOrEmpty(txt))
                return null;
            Regex myRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            List<string> results = new List<string>();
            foreach (Match match in myRegex.Matches(txt))
            {
                if (match.Success)
                {
                    if (match.Groups[groupname].Captures.Count > 1)
                        results.Add(match.Groups[groupname].Captures[0].Value); //pokud nalezeno vice vyskytu, vem prvni
                    else
                        results.Add(match.Groups[groupname].Value);
                }
            }
            return results.ToArray();
        }

        public static string ReplaceGroupMatchNameWithRegex(this string txt, string regex, string groupname, string replacement)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            var dateIntervalM = Regex.Matches(txt, regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            foreach (Match m in dateIntervalM)
            {
                if (m.Success && m.Groups[groupname].Success)
                {
                    txt = txt.Substring(0, m.Groups[groupname].Index)
                        + replacement
                        + txt.Substring(m.Groups[groupname].Index + m.Groups[groupname].Length);

                }
            }
            return txt;
        }


        public static string ReplaceWithRegex(this string txt, string replacement, string regex)
        {
            return GetStringReplaceWithRegex(regex, txt, replacement);
        }
        public static string GetStringReplaceWithRegex(string regex, string txt, string replacement)
        {
            Regex myRegex = new Regex(regex, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);
            return myRegex.Replace(txt, replacement);
        }


        public static T ChangeType<T>(object value)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return default(T);
                }

                t = Nullable.GetUnderlyingType(t);
            }

            return (T)Convert.ChangeType(value, t);
        }

        public static object ChangeType(object value, Type conversion)
        {
            var t = conversion;

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                {
                    return null;
                }

                if (Nullable.GetUnderlyingType(t) == typeof(Util.Date))
                {
                    if (Util.Date.TryParseExact((string)value, "yyyy-MM-dd",
                        Consts.enCulture, System.Globalization.DateTimeStyles.AssumeLocal, out Util.Date date))
                    {
                        return date;
                    }
                    else
                        return null;
                }

                t = Nullable.GetUnderlyingType(t);
            }

            if (t == typeof(Util.Date))
            {
                return new Util.Date((string)value);
            }
            return Convert.ChangeType(value, t);
        }

    }
}
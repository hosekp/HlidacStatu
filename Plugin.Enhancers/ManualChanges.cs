﻿using Devmasters.Collections;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Enhancers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.Enhancers
{



    public class ManualChanges : IEnhancer
    {
        public const string NastaveniSmluvnichStran = "Ruční nastavení smluvních stran";
        public string Description
        {
            get
            {
                return "ManualChanges Enhancer";
            }
        }

        public string Name
        {
            get
            {
                return Description;
            }
        }
        public void SetInstanceData(object data)
        {
        }

        public bool Update(ref Lib.Data.Smlouva item)
        {
            return false;
        }

        public bool UpdateSmluvniStrany(ref Lib.Data.Smlouva item, Lib.Data.Smlouva.Subjekt platce, Lib.Data.Smlouva.Subjekt[] prijemce )
        {
            item.Enhancements = item.Enhancements.AddOrUpdate(
                new Enhancement(NastaveniSmluvnichStran, "Ručně nastaven plátce", "item.Platce", Ser(item.Platce), Ser(platce), this)
                );
            item.Platce = platce;

            item.Enhancements = item.Enhancements.AddOrUpdate(
                new Enhancement(NastaveniSmluvnichStran, "Ručně nastaven příjemce", "item.Prijemce", Ser(item.Prijemce), Ser(prijemce), this)
                );
            item.Prijemce = prijemce;

            return true;
        }


        private string Ser(Lib.Data.Smlouva.Subjekt subj)
        {
            if (subj == null)
                return null;
            return  Newtonsoft.Json.JsonConvert.SerializeObject(new { ico = subj.ico, nazev = subj.nazev });
        }
        private string Ser(IEnumerable<Lib.Data.Smlouva.Subjekt> subj)
        {
            if (subj == null)
                return null;
            return Newtonsoft.Json.JsonConvert.SerializeObject(
                    subj
                    .Select(m=>Ser(m))
                    .ToArray()
                );
        }


    }
}

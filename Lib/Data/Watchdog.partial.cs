﻿using Devmasters;
using Devmasters.Enums;

using HlidacStatu.Lib.Data.Insolvence;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace HlidacStatu.Lib.Data
{
    public partial class WatchDog
    {
        public static string APIID_Prefix = "APIID:";

        public static string AllDbDataType = "#ALL#";

        [Devmasters.Enums.ShowNiceDisplayName()]
        [Devmasters.Enums.Sortable(Devmasters.Enums.SortableAttribute.SortAlgorithm.BySortValue)]
        public enum PeriodTime
        {
            [Disabled()]
            [Devmasters.Enums.SortValue(0), Devmasters.Enums.NiceDisplayName("Ihned")]
            Immediatelly = 0,

            [Disabled()]
            [Devmasters.Enums.SortValue(0), Devmasters.Enums.NiceDisplayName("Každé 2 hodiny")]
            Hourly = 1,

            [Devmasters.Enums.SortValue(0), Devmasters.Enums.NiceDisplayName("Denně")]
            Daily = 2,

            [Devmasters.Enums.SortValue(0), Devmasters.Enums.NiceDisplayName("Týdně")]
            Weekly = 3,

            [Devmasters.Enums.SortValue(0), Devmasters.Enums.NiceDisplayName("Měsíčně")]
            Monthly = 4,
        }


        public enum FocusType
        {
            Search = 0,
            Issues = 1
        }


        public FocusType Focus
        {
            get { return (FocusType)this.FocusId; }
            set { this.FocusId = (int)value; }
        }

        public PeriodTime Period
        {
            get { return (PeriodTime)this.PeriodId; }
            set { this.PeriodId = (int)value; }
        }
        public void Save()
        {
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                if (this.Id == 0)
                {
                    db.WatchDogs.Add(this);
                }
                else
                {
                    db.WatchDogs.Attach(this);
                    db.Entry(this).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }
        public void Delete()
        {
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                if (this.Id == 0)
                {
                }
                else
                {
                    db.WatchDogs.Attach(this);
                    db.Entry(this).State = System.Data.Entity.EntityState.Deleted;
                    db.SaveChanges();
                }
            }
        }
        public AspNetUser User()
        {
            using (Lib.Data.DbEntities db = new DbEntities())
            {
                var user = db.AspNetUsers
                .Where(m => m.EmailConfirmed && m.Id == this.UserId)
                .FirstOrDefault();

                return user;
            }
        }

        public AspNetUser UnconfirmedUser()
        {
            using (Lib.Data.DbEntities db = new DbEntities())
            {
                var user = db.AspNetUsers
                .Where(m => m.Id == this.UserId)
                .FirstOrDefault();

                return user;
            }
        }

        public enum DisabledBySystemReasons
        {
            NoDataset,
            NoConfirmedEmail,
            InvalidQuery
        }

        public void DisableWDBySystem(DisabledBySystemReasons reason, bool repeated = false)
        {
            this.StatusId = 0;
            this.Save();
            var dataSetId = this.dataType.Replace("DataSet.", "");

            //send info about disabled wd
            var uuser = this.UnconfirmedUser();
            if (uuser != null && Devmasters.TextUtil.IsValidEmail(uuser.Email))
            {
                using (MailMessage msg = new MailMessage())
                {
                    if (reason == DisabledBySystemReasons.NoDataset)
                        msg.Body = "Dobrý den.\n"
                            + $"Váš hlídač '{this.Name}' hlídající nové záznamy byl deaktivován.\n"
                            + "\n"
                            + $"Důvodem je neexistující (nejspíše nedávno smazaný) dataset '{dataSetId}'.\n"
                            + "\n"
                            + "S pozdravem\n"
                            + "\n"
                            + "Hlídač státu";
                    else if (reason == DisabledBySystemReasons.NoConfirmedEmail)
                    {
                        msg.Body = "Dobrý den.\n"
                           + $"Váš hlídač '{this.Name}' hlídající nové záznamy byl deaktivován.\n"
                           + "\n"
                           + $"Důvodem je nepotvrzená emailová adresa z vaší registrace. Dnes jsme vám znovu poslali email s žádostí o potvrzení emailu z registrace. Stačí kliknout na odkaz v tomto mailu.\n"
                           + "\n"
                           + "S pozdravem\n"
                           + "\n"
                           + "Hlídač státu";
                        if (repeated == false) //send only once
                            new System.Net.WebClient()
                                .DownloadString($"http://www.hlidacstatu.cz/api/v1/ResendConfirmationMail?id={uuser.Id}&Authorization=" + uuser.GetAPIToken());
                    }
                    else if (reason == DisabledBySystemReasons.InvalidQuery)
                    {
                    }
                    msg.Subject = "Deaktivovaný hlídač nových záznamů na Hlídači státu";
                    msg.To.Add(uuser.Email);
                    msg.BodyEncoding = System.Text.Encoding.UTF8;
                    msg.SubjectEncoding = System.Text.Encoding.UTF8;
                    using (SmtpClient smtp = new SmtpClient())
                    {
                        HlidacStatu.Util.Consts.Logger.Info("Sending email to " + msg.To);
                        msg.Bcc.Add("michal@michalblaha.cz");
                        smtp.Send(msg);
                    }
                }
            }
        }
        public List<Lib.Watchdogs.IWatchdogProcessor> GetWatchDogProcessors()
        {

            var res = new List<Lib.Watchdogs.IWatchdogProcessor>();


            if (this.dataType == typeof(Smlouva).Name || this.dataType == WatchDog.AllDbDataType)
                res.Add(new Smlouva.WatchdogProcessor(this));
            if (this.dataType == typeof(VZ.VerejnaZakazka).Name || this.dataType == WatchDog.AllDbDataType)
                res.Add(new VZ.VerejnaZakazka.WatchdogProcessor(this));
            if (this.dataType == typeof(Rizeni).Name || this.dataType == WatchDog.AllDbDataType)
                res.Add(new Insolvence.Rizeni.WatchdogProcessor(this));
            if (this.dataType.StartsWith(typeof(Lib.Data.External.DataSets.DataSet).Name))
                res.Add(new Lib.Data.External.DataSets.DataSet.WatchdogProcessor(this));

            if (this.dataType == WatchDog.AllDbDataType)
            {                                                     //add all datasets
                foreach (var ds in External.DataSets.DataSetDB.ProductionDataSets.Get())
                {
                    res.Add(new Lib.Data.External.DataSets.DataSet.WatchdogProcessor(this, ds));
                }

            }
            //throw new System.NotImplementedException();
            return res;
        }


        public static WatchDog Load(int watchdogId)
        {
            using (HlidacStatu.Lib.Data.DbEntities db = new HlidacStatu.Lib.Data.DbEntities())
            {
                return db.WatchDogs.Where(m => m.Id == watchdogId).FirstOrDefault();
            }
        }

    }
}

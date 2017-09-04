﻿using System;
using System.Collections.Generic;
using System.Linq;
using KRSServerApplication.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KRSServerApplication.Controllers
{
    [Route("api/[controller]/[action]")]
    public class SortingController : Controller
    {
        KRSDataBaseContext db;

        public SortingController(KRSDataBaseContext context)
        {
            db = context;
        }

        //[HttpGet/*("{currentBatchNumber}")*/]
        //public IQueryable GetCurrentBatchesSkins(/*int currentBatchNumber*/)
        [HttpGet("{batchNumber}")]
        public IEnumerable<SortingSelect> GetCurrentBatchesSkins(int batchNumber)
        {
            var currentBatchesSkins = db.Skin
                .Join(db.Batch, skin => skin.IdBatch, batch => batch.IdBatch, (skin, batch) => new { skin, batch })
                .Join(db.SkinType, s => s.skin.IdTypeSkin, skinType => skinType.IdTypeSkin, (s, skinType) => new { s, skinType })
                .Join(db.Pallet, s => s.s.skin.IdPallet, pallet => pallet.IdPallet, (s, pallet) => new { s, pallet })
                .Where(idbatch => idbatch.s.s.batch.IdBatch == batchNumber)
                .Select(sortingSelect => new SortingSelect()
                {
                    Sort = sortingSelect.s.s.skin.IdSort,
                    Brutto = sortingSelect.s.s.skin.Brutto,
                    Netto = sortingSelect.s.s.skin.Netto,
                    Discount = sortingSelect.s.s.skin.Discount,
                    SkinTypeLabel = sortingSelect.s.skinType.TypeSkinLabel,
                    MaximumCountSkin = sortingSelect.s.skinType.MaximumCountSkin,
                    IdBatch = sortingSelect.s.s.batch.IdBatch,
                    IdSkin = sortingSelect.s.s.skin.IdSkin,
                    IdPallet = sortingSelect.pallet.IdPallet,
                    CurrentCount = sortingSelect.pallet.CurrentCountSkins
                });

            return currentBatchesSkins.ToList();
        }
        // CQRS - Command Query Responsibility Segrregation

        // GET: api/values
        [HttpGet]
        public IQueryable GetOpenBatches()
        {
            var openBatches = db.Batch.Where(b => b.BatchStatus == "Открыта")
                   .Select(s => s.IdBatch);
            return openBatches;
        }

        [HttpGet("{weight},{idconfiguration}")]
        public IQueryable GetConfigurationTypeSkin(float weight, int idconfiguration)
        {
            var typeSkin = db.SkinType.Where(st => st.MinimumWeight <= Convert.ToDecimal(weight) && st.MaximumWeight >= Convert.ToDecimal(weight) && st.IdConfiguration == idconfiguration)
                .Select(st => st.IdTypeSkin);
            return typeSkin;
        }

        [HttpGet("{idtypeskin}")]
        public IQueryable GetTypeSkinPallet(int idtypeskin)
        {
            var IDpallet = db.Pallet.Where(p => p.Status == "Открыт" && p.IdTypeSkin == idtypeskin)
                .Select(p => p.IdPallet);
            return IDpallet;
        }

        [HttpGet("{batchNumber}")]
        public IQueryable GetSchemeID(int batchNumber)
        {
            var IDScheme = db.Batch.Where(b => b.IdBatch == batchNumber)
                .Select(b => b.IdScheme);
            return IDScheme;
        }

        [HttpGet("{schemeID}")]
        public IQueryable GetEnabledTypeSkins(int schemeID)
        {
            var IDTypeSkin = db.SchemeType.Where(st => st.IdScheme == schemeID)
                .Select(st => st.IdTypeSkin);
            return IDTypeSkin;
        }

        [HttpGet("{palletID}")]
        public IQueryable GetPalletCountSkins(int palletID)
        {
            var CurrentCount = db.Pallet.Where(p => p.IdPallet == palletID)
                .Select(p => p.CurrentCountSkins);
            return CurrentCount;
        }

        [HttpGet("{palletID}")]
        public IQueryable GetPalletOpeningDate(int palletID)
        {
            var OpeningDate = db.Pallet.Where(p => p.IdPallet == palletID)
                .Select(p => p.OpeningDate);
            return OpeningDate;
        }

        [HttpGet("{typeskinID}")]
        public IQueryable GetMaxCountSkins(int typeskinID)
        {
            var MaxCount = db.SkinType.Where(st => st.IdTypeSkin == typeskinID)
                .Select(st => st.MaximumCountSkin);
            return MaxCount;
        }

        [HttpGet("{configName}")]
        public IQueryable GetConfigurationID(string configName)
        {
            var IDConfiguration = db.Configuration.Where(c => c.ConfigurationLabel == configName)
                .Select(c => c.IdConfiguration);
            return IDConfiguration;
        }

        [HttpGet("{batchID}")]
        public IEnumerable<Batch> GetBatch(int batchID)
        {
            var CurrentBatch = db.Batch.Where(b => b.IdBatch == batchID);
            return CurrentBatch;
        }
    }
}

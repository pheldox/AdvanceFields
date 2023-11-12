﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using static jQuery_Ajax_CRUD.Helper;
using AdvanceFields.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IO;
using AdvanceFields.Services;

namespace AdvanceFields.Controllers
{
    public class TranslationController : Controller
    {
    
        
        private readonly IConfiguration _configuration;
        private readonly ITranslation _translation;
        public TranslationController( IConfiguration configuration,ITranslation translation)
        {
            //_context = context;
            _translation = translation;
            _configuration = configuration;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {

            return View(_translation.LoadTranslation());
        
        }

        // GET: Transaction/AddOrEdit(Insert)
        // GET: Transaction/AddOrEdit/5(Update)
        [NoDirectAccess]
        public IActionResult AddOrEdit(int id = 0)
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> AddOrEdit(int id, [Bind("TransactionId,AccountNumber,BeneficiaryName,BankName,SWIFTCode,Amount,Date")] TransactionModel transactionModel)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        //Insert
        //        if (id == 0)
        //        {
        //            transactionModel.Date = DateTime.Now;
        //            _context.Add(transactionModel);
        //            await _context.SaveChangesAsync();

        //        }
        //        //Update
        //        else
        //        {
        //            try
        //            {
        //                _context.Update(transactionModel);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!TransactionModelExists(transactionModel.TransactionId))
        //                { return NotFound(); }
        //                else
        //                { throw; }
        //            }
        //        }
        //        return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Transactions.ToList()) });
        //    }
        //    return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", transactionModel) });
        //}

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Translate(string text)
        {
            var res = new Translation ();
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException("Input text cannot be empty.", nameof(text));
            }
            try
            {
              
                    res = RqTranslation(text);

                if (res != null)
                {
                    var translation = new RqTranslate
                    {
                        Text = res?.contents?.text ?? string.Empty,
                        Translated = res?.contents?.translation ?? string.Empty
                    };
                    //save to db
                    _translation.SaveTranslation(translation);
                }
            
                

            }catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }      
            
            return Json(res?.contents?.translated);
        }

        //// GET: Transaction/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var transactionModel = await _context.Transactions
        //        .FirstOrDefaultAsync(m => m.TransactionId == id);
        //    if (transactionModel == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(transactionModel);
        //}

        //// POST: Transaction/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var transactionModel = await _context.Transactions.FindAsync(id);
        //    _context.Transactions.Remove(transactionModel);
        //    await _context.SaveChangesAsync();
        //    return Json(new { html = Helper.RenderRazorViewToString(this, "_ViewAll", _context.Transactions.ToList()) });
        //}

        //private bool TransactionModelExists(int id)
        //{
        //    return _context.Transactions.Any(e => e.TransactionId == id);
        //}

        private  Translation RqTranslation(string text)
        {
            var request = JsonConvert.SerializeObject(text);
           // StringContent payload = new StringContent(request,Encoding.UTF8,"application/json");
            string endpoint = _configuration["TranslationSettings:PiglatinEndpoint"];
            var requestUri = _configuration["TranslationSettings:BaseApiURL"];
            //var reqString = endpoint+""
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(requestUri);
                    using (HttpResponseMessage response = httpClient.GetAsync(endpoint + $"?text={request}").Result)
                    using (HttpContent content = response.Content)
                    using (StringReader sr = new StringReader(content.ReadAsStringAsync().Result))
                    {
                        string result = sr.ReadToEnd();
                        var res = JsonConvert.DeserializeObject<Translation>(result);
                        return res;
                    }
                }
            }catch(Exception e)
            {
                throw e;
            }
        }
    }
}

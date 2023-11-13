using System;
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
using AdvanceFields.SqlRepository;
using Microsoft.Extensions.Logging;

namespace AdvanceFields.Controllers
{
    public class TranslationController : Controller
    {
    
        
        private readonly IConfiguration _configuration;
        private readonly ITranslation _translation;
        private readonly ILogger<TranslationController> _logger;
        public TranslationController( IConfiguration configuration,ITranslation translation, ILogger<TranslationController> logger)
        {
            //_context = context;
            _translation = translation;
            _configuration = configuration;
            _logger = logger;
        }

        // GET: Translation
        public async Task<IActionResult> Index()
        {
            try
            {

                var data = _translation.LoadTranslation();
            
                return View(data);
            }
            catch(Exception ex)
            {
                _logger.LogError($"Error Loading Translations", ex);
                return View("Error", ex.Message);
            }
        
        }

        [NoDirectAccess]
        public IActionResult AddOrEdit(int id = 0)
        {
            return View();
        }

     

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
                        Translated = res?.contents?.translated ?? string.Empty
                    };
                    //save to db
                    _translation.SaveTranslation(translation);
                }
            
                

            }catch(Exception ex)
            {
                _logger.LogError($"Error saving translation", ex);
                return View("Error", ex.Message);
            }      
            
            return Json(res?.contents?.translated);
        }


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
                    _logger.LogInformation($"Text to be translated {text}");
                    using (HttpResponseMessage response = httpClient.GetAsync(endpoint + $"?text={request}").Result)
                    using (HttpContent content = response.Content)
                    using (StringReader sr = new StringReader(content.ReadAsStringAsync().Result))
                    {
                        string result = sr.ReadToEnd();
                        var res = JsonConvert.DeserializeObject<Translation>(result);
                        _logger.LogInformation($"Translated Text {result}");
                        return res;
                    }
                }
            }catch(Exception ex)
            {
                _logger.LogError($"Error durring   translation", ex);
                throw ex;
            }
        }
    }
}

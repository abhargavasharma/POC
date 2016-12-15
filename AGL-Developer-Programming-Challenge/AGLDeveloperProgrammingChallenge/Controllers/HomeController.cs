using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AGLDeveloperProgrammingChallenge.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> AGLResponse()
        {
            //Can be taken from config file
            string AGLUrl = "http://agl-developer-test.azurewebsites.net/people.json";

            List<ResultData> resultFinal = new List<ResultData>();
            ResultData r = null;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(AGLUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                var myResponse = new JavaScriptSerializer().Deserialize<List<People>>(responseBody);

                var data = myResponse
                            .GroupBy(m => m.gender)
                            .Select(gm => new { gender = gm.Key, persons = gm });

                foreach (var x in data)
                {
                    r = new ResultData();
                    List<string> names = new List<string>();
                    r.gender = x.gender;
                    var cats = x.persons.Where(p => p.pets != null)
                        .Select(percat => new
                        {
                            percats = percat.pets.Where(p => p.type.ToLower() == "cat")
                        }).Select(p => p.percats);

                    foreach (var person in cats)
                    {
                        foreach (var cat in person)
                        {
                            names.Add(cat.name);
                        }
                    }
                    r.cats = names.OrderBy(c => c).ToList();
                    resultFinal.Add(r);
                }
            }

            return Json(resultFinal, JsonRequestBehavior.AllowGet);
        }

    }

    #region Model

    //For every class we can create aclass file but for sharing put it in single class file
    [Serializable]
    public class People
    {
        public string name { get; set; }
        public string gender { get; set; }
        public int age { get; set; }

        public List<Pet> pets = new List<Pet>();
    }

    [Serializable]
    public class Pet
    {
        public string name { get; set; }
        public string type { get; set; }
    }

    public class ResultData
    {
        public string gender { get; set; }
        public List<string> cats { get; set; }

    } 
    #endregion
}
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

public class HomeController : Controller
{
    private string response;
   private string AGLUrl = "http://agl-developer-test.azurewebsites.net/people.json";

    public async Task<ActionResult> Index()
    {
        List<ResultData> resultFinal = new List<ResultData>();

        Uri geturi = new Uri(AGLUrl); //replace your url  
        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
        System.Net.Http.HttpResponseMessage responseGet = await client.GetAsync(geturi);
        response = await responseGet.Content.ReadAsStringAsync();

        var myResponse = new JavaScriptSerializer().Deserialize<List<JsonSerialiser>>(response);

        var data = myResponse
                    .GroupBy(m => m.gender)
                    .Select(gm => new { gender = gm.Key, persons = gm });

        foreach (var x in data)
        {
            List<string> catNames = new List<string>();
            var cats = (x.persons.Where(per => per.pets != null).Select(PerPet => new { Percats = PerPet.pets.Where(p => p.type.ToLower() == "cat") })).Select(p => p.Percats);
            foreach (var person in cats)
            {
                foreach (var cat in person)
                {
                    catNames.Add(cat.name);
                }
            }

            ResultData r = new ResultData();
            r.gender = x.gender;
            r.cats = catNames.OrderBy(o => o).ToList();
            resultFinal.Add(r);
        }
        // }


        return View();
    }


    [HttpGet]
    public async Task<ActionResult> getPets()
    {
        List<ResultData> resultFinal = new List<ResultData>();

        Uri geturi = new Uri(AGLUrl); //replace your url  
        System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
        System.Net.Http.HttpResponseMessage responseGet = await client.GetAsync(geturi);
        response = await responseGet.Content.ReadAsStringAsync();

        var myResponse = new JavaScriptSerializer().Deserialize<List<JsonSerialiser>>(response);

        var data = myResponse
                    .GroupBy(m => m.gender)
                    .Select(gm => new { gender = gm.Key, persons = gm });

        foreach (var x in data)
        {
            List<string> catNames = new List<string>();
            var cats = (x.persons.Where(per => per.pets != null).Select(PerPet => new { Percats = PerPet.pets.Where(p => p.type.ToLower() == "cat") })).Select(p => p.Percats);
            foreach (var person in cats)
            {
                foreach (var cat in person)
                {
                    catNames.Add(cat.name);
                }
            }

            ResultData r = new ResultData();
            r.gender = x.gender;
            r.cats = catNames.OrderBy(o => o).ToList();
            resultFinal.Add(r);
        }

        return  Json(resultFinal, JsonRequestBehavior.AllowGet);


    }


    public ActionResult About()
    {
        ViewBag.Message = "Your application description page.";

        return View();
    }

    public ActionResult Contact()
    {
        ViewBag.Message = "Your contact page.";

        return View();
    }
}

[Serializable]
public class JsonSerialiser
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


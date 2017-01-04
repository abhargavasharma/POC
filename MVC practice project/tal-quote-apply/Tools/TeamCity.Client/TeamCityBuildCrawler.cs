using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Serialization;
using TeamCity.Client.Model;

namespace TeamCity.Client
{
    public interface ITeamCityBuildCrawler
    {
        List<buildsBuild> GetBuilds(string buildType);
        List<changesChange> GetChangesForBuild(int buildId);
        change GetChange(int changeId);
    }

    public class TeamCityBuildCrawler : ITeamCityBuildCrawler
    {
        private readonly string _baseUrl;
        public TeamCityBuildCrawler(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public List<buildsBuild> GetBuilds(string buildType)
        {
            var url = string.Format("{0}/httpAuth/app/rest/builds/?locator=buildType:{1},branch:default:any,running:any", _baseUrl, buildType);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "dG93ZXJcZXRoaWxhcHA6MU41dXJhbmNl");
                var result = client.GetAsync(url).Result;

                if (result.IsSuccessStatusCode)
                {
                    var resultXmlString = result.Content.ReadAsStringAsync().Result;
                    var xs = new XmlSerializer(typeof(builds));
                    var rez = (builds)xs.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(resultXmlString)));
                    return rez.build.ToList();
                }
            }
            return null;
        }

        public List<changesChange> GetChangesForBuild(int buildId)
        {
            var url = string.Format("{0}/httpAuth/app/rest/changes?locator=build:(id:{1})", _baseUrl, buildId);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "dG93ZXJcZXRoaWxhcHA6MU41dXJhbmNl");
                var result = client.GetAsync(url).Result;

                if (result.IsSuccessStatusCode)
                {
                    var resultXmlString = result.Content.ReadAsStringAsync().Result;
                    var xs = new XmlSerializer(typeof(changes));
                    var rez = (changes)xs.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(resultXmlString)));
                    return rez.change == null ? new List<changesChange>() : rez.change.ToList();
                }
            }
            return null;
        }

        public change GetChange(int changeId)
        {
            var url = string.Format("{0}/httpAuth/app/rest/changes/id:{1}", _baseUrl, changeId);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "dG93ZXJcZXRoaWxhcHA6MU41dXJhbmNl");
                var result = client.GetAsync(url).Result;

                if (result.IsSuccessStatusCode)
                {
                    var resultXmlString = result.Content.ReadAsStringAsync().Result;
                    var xs = new XmlSerializer(typeof(change));
                    var rez = (change)xs.Deserialize(new MemoryStream(Encoding.UTF8.GetBytes(resultXmlString)));
                    return rez;
                }
            }
            return null;
        }
        
    }
}
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace helpers.IDbManager {
    public abstract class IDbManager 
    {
        public HttpClient hc;

        public abstract Task<HttpResponseMessage> get(string url, string id);
        public abstract Task<HttpResponseMessage> create(string url, string jsonObj);
        public abstract Task<HttpResponseMessage> update(string url, string jsonObj);
        public abstract Task<HttpResponseMessage> delete(string url, string jsonObj);
    }


    public class genericDbManager : IDbManager 
    {
        public genericDbManager(){}

        public override async Task<HttpResponseMessage> get(string url, string id)
        {
            hc = new HttpClient();
            hc.BaseAddress= new Uri(url+"/"+id);
            hc.DefaultRequestHeaders.Clear();
            hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            HttpResponseMessage response = await hc.GetAsync("");
            return response;
        }
        public override async Task<HttpResponseMessage> create(string url, string jsonObj)
        {
            hc = new HttpClient();
            hc.BaseAddress= new Uri(url);
            hc.DefaultRequestHeaders.Clear();
            hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            HttpContent htc = new StringContent(jsonObj,System.Text.Encoding.UTF8,"application/json");
            HttpResponseMessage response = await hc.PostAsync("", htc);
            return response;
        }
        public override async Task<HttpResponseMessage> update(string url, string jsonObj)
        {
            hc = new HttpClient();
            hc.BaseAddress= new Uri(url);
            hc.DefaultRequestHeaders.Clear();
            hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            HttpContent htc = new StringContent(jsonObj,System.Text.Encoding.UTF8,"application/json");
            HttpResponseMessage response = await hc.PostAsync("", htc);
            return response;
        }
        public override async Task<HttpResponseMessage> delete(string url, string jsonObj)
        {
            hc = new HttpClient();
            hc.BaseAddress= new Uri(url);
            hc.DefaultRequestHeaders.Clear();
            hc.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); 
            HttpContent htc = new StringContent(jsonObj,System.Text.Encoding.UTF8,"application/json");
            HttpResponseMessage response = await hc.PostAsync("",htc);
            return response;
        }

    }

}
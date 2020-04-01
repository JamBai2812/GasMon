using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using Amazon.S3;
using Newtonsoft.Json;

namespace GasMon
{
    public class LocationsFetcher
    {
        private readonly IAmazonS3 client;


        public LocationsFetcher(IAmazonS3 client)
        {
            this.client = client;
        }

        public List<Location> GetLocations(string bucketName, string fileName)
        {
            var response = client.GetObjectAsync(bucketName, fileName).Result;
            
            using var streamReader = new StreamReader(response.ResponseStream);
            var content = streamReader.ReadToEnd();

            return JsonConvert.DeserializeObject<List<Location>>(content).ToList();
        }
    }
}
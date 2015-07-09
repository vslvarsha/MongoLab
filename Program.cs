using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MongoDB.Bson;
using MongoDB.Driver;

namespace ConsoleApplication4
{
    class Program
    {
        // Extra helper code
        static BsonDocument[] CreateSeedData()
        {
            BsonDocument seventies = new BsonDocument {
        { "Decade" , "1970s" },
        { "Artist" , "Debby Boone" },
        { "Title" , "You Light Up My Life" },
        { "WeeksAtOne" , 10 }
      };

            BsonDocument eighties = new BsonDocument {
        { "Decade" , "1980s" },
        { "Artist" , "Olivia Newton-John" },
        { "Title" , "Physical" },
        { "WeeksAtOne" , 10 }
      };

            BsonDocument nineties = new BsonDocument {
        { "Decade" , "1990s" },
        { "Artist" , "Mariah Carey" },
        { "Title" , "One Sweet Day" },
        { "WeeksAtOne" , 16 }
      };

            BsonDocument[] SeedData = { seventies, eighties, nineties };
            return SeedData;
        }

        async static Task AsyncCrud(BsonDocument[] seedData)
        {

            // Create seed data
            BsonDocument[] songData = seedData;

            // Standard URI format: mongodb://[dbuser:dbpassword@]host:port/dbname
            //String uri = "mongodb://:pass@host:port/db";

           // string uri = "mongodb://62f70ee8-5429-4cda-b947-a705c998f1a9:82148ec1-20ba-4f06-bcac-784267bfee38@159.8.128.196:10100/db";
            string uri = "mongodb://IbmCloud_7cmdvh0j_k24f66ka_h6amhrse:514lk3jqNAiTmfYuTcjWlOOTo7IfKIIs@ds037812.mongolab.com:37812/IbmCloud_7cmdvh0j_k24f66ka";




            
            var client = new MongoClient(uri);
           var db = client.GetDatabase("IbmCloud_7cmdvh0j_k24f66ka");

            /*
             * First we'll add a few songs. Nothing is required to create the
             * songs collection; it is created automatically when we insert.
             */
            
            var songs = db.GetCollection<BsonDocument>("songs");

            // Use InsertOneAsync for single BsonDocument insertion.
            await songs.InsertManyAsync(songData);

            /*
             * Then we need to give Boyz II Men credit for their contribution to
             * the hit "One Sweet Day".
             */

            var updateFilter = Builders<BsonDocument>.Filter.Eq("Title", "One Sweet Day");
            var update = Builders<BsonDocument>.Update.Set("Artist", "Mariah Carey ft. Boyz II Men");

            await songs.UpdateOneAsync(updateFilter, update);

            /*
             * Finally we run a query which returns all the hits that spent 10 
             * or more weeks at number 1.
             */

            var filter = Builders<BsonDocument>.Filter.Gte("WeeksAtOne", 10);
            var sort = Builders<BsonDocument>.Sort.Ascending("Decade");

            await songs.Find(filter).Sort(sort).ForEachAsync(song =>
              Console.WriteLine("In the {0}, {1} by {2} topped the charts for {3} straight weeks",
                song["Decade"], song["Title"], song["Artist"], song["WeeksAtOne"])
            );

            // Since this is an example, we'll clean up after ourselves.
            await db.DropCollectionAsync("songs");
        }

        static void Main(string[] args)
        {
            BsonDocument[] seedData = CreateSeedData();
            AsyncCrud(seedData).Wait();

            Console.Read();

        }
    }
}

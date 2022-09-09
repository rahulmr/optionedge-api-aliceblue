﻿///

// Development Test Class
// This is used to test the specific features as they are implemented

// If you are lookng for api samples, refer to FeaturesDemo.cs class

///
using Newtonsoft.Json;
using OptionEdge.API.AliceBlue.Records;

namespace OptionEdge.API.AliceBlue.Samples
{
    public class DevTest
    {
        // apiKey, userId, logging setting
        static Settings _settings;

        static IAliceBlue _aliceBlue;
        static Ticker _ticker;

        static string _cachedTokenFile = "cached_token.txt";

        public void Run()
        {
            try
            {
                // Read ApiKey, userId from Settings 
                _settings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText("settings.dev.json"));


                // ==========================
                // Initialize
                // ==========================

                // Create new instance of AliceBlue client library
                _aliceBlue = new AliceBlue(_settings.UserId, _settings.ApiKey, enableLogging: _settings.EnableLogging,
                    onAccessTokenGenerated: (accessToken) =>
                    {
                        // Store the generated access token to database or file store
                        // Token needs to be generated only once for the day

                        File.WriteAllText(_cachedTokenFile, accessToken);
                    }, cachedAccessTokenProvider: () =>
                    {
                        // Provide the saved token that will be used to making the REST calls.
                        // This method will be used when re-initializing the api client during the the day (eg after app restart etc)

                        // If token is invalid or not available, just return empty or null value
                        return File.Exists(_cachedTokenFile) ? File.ReadAllText(_cachedTokenFile) : null;
                    });


                // ==========================
                // Live Feeds Data Streaming
                // ==========================

                // Create Ticker instance
                // No need to provide the userId, apiKey, it will be automatically set
                _ticker = _aliceBlue.CreateTicker();

                // Setup event handlers
                _ticker.OnTick += _ticker_OnTick;
                _ticker.OnConnect += _ticker_OnConnect;
                _ticker.OnClose += _ticker_OnClose;
                _ticker.OnError += _ticker_OnError;
                _ticker.OnNoReconnect += _ticker_OnNoReconnect;
                _ticker.OnReconnect += _ticker_OnReconnect;

                // Connect the ticker to start receiving the live feeds
                // DO NOT FORGOT TO CONNECT else you will not receive any feed

                // _ticker.Connect();

                var openInterest = _aliceBlue.GetOpenInterest(Constants.EXCHANGE_NFO, new int[] { 35042, 37342 });



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }



        private static void _ticker_OnTick(Tick TickData)
        {
            Console.WriteLine(JsonConvert.SerializeObject(TickData));
        }

        private static void _ticker_OnReconnect()
        {
            Console.WriteLine("Ticker reconnecting.");
        }

        private static void _ticker_OnNoReconnect()
        {
            Console.WriteLine("Ticker not reconnected.");
        }

        private static void _ticker_OnError(string Message)
        {
            Console.WriteLine("Ticker error." + Message);
        }

        private static void _ticker_OnClose()
        {
            Console.WriteLine("Ticker closed.");
        }

        private static void _ticker_OnConnect()
        {
            Console.WriteLine("Ticker connected.");

            Thread.Sleep(2000);
            //_ticker.Subscribe(Constants.TICK_MODE_FULL,
            //    new SubscriptionToken[]
            //        {
            //           new SubscriptionToken
            //           {
            //               Exchange = Constants.EXCHANGE_NSE,
            //               Token = 26000
            //           },
            //           new SubscriptionToken
            //           {
            //               Exchange = Constants.EXCHANGE_NSE,
            //               Token = 26009
            //           },
            //           new SubscriptionToken
            //           {
            //               Exchange = Constants.EXCHANGE_NFO,
            //               Token = 35042
            //           },
            //        });

            //_ticker.Subscribe(Constants.EXCHANGE_NSE, Constants.TICK_MODE_FULL, new int[] { 26000, 26009 });
        }
    }
}

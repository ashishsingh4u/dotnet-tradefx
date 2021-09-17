//  ===================================================================================
//  <copyright file="OnadaMarketDataService.cs" company="TechieNotes">
//  ===================================================================================
//   TechieNotes Utilities & Best Practices
//   Samples and Guidelines for Winform & ASP.net development
//  ===================================================================================
//   Copyright (c) TechieNotes.  All rights reserved.
//   THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
//   OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
//   LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
//   FITNESS FOR A PARTICULAR PURPOSE.
//  ===================================================================================
//   The example companies, organizations, products, domain names,
//   e-mail addresses, logos, people, places, and events depicted
//   herein are fictitious.  No association with any real company,
//   organization, product, domain name, email address, logo, person,
//   places, or events is intended or should be inferred.
//  ===================================================================================
//  </copyright>
//  <author>ASHISHSINGH</author>
//  <email>mailto:ashishsingh4u@gmail.com</email>
//  <date>12-01-2013</date>
//  <summary>
//     The OnadaMarketDataService.cs file.
//  </summary>
//  ===================================================================================

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

using TradeFx.Common.Events;
using TradeFx.Common.MarketDataReader.FxContracts;
using TradeFx.Common.MarketDataReader.Interface;

namespace TradeFx.Common.MarketDataReader.Service
{
    /// <summary>The onada market data service.</summary>
    internal class OnadaMarketDataService : MarketDataService, IOnadaMarketDataService
    {
        #region Constants

        /// <summary>The api server.</summary>
        private const string ApiServer = "http://api-sandbox.oanda.com/";

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="OnadaMarketDataService"/> class.</summary>
        /// <param name="aggregator">The aggregator.</param>
        public OnadaMarketDataService(IEventAggregator aggregator)
            : base(aggregator)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>Retrieves H1 candles for the specified pair</summary>
        /// <param name="curPair">string name of the desired pair</param>
        /// <returns>list of candles up to the present</returns>
        public static List<Candle> GetCandles(string curPair)
        {
            var requestString = ApiServer + "v1/" + "instruments/" + curPair + "/candles?granularity=H1";

            var responseString = MakeRequest(requestString);

            var serializer = new JavaScriptSerializer();
            var candlesResponse = serializer.Deserialize<CandlesResponse>(responseString);
            var candles = new List<Candle>();
            candles.AddRange(candlesResponse.Candles);

            return candles;
        }

        /// <summary>
        ///     Gets the list of instruments that are available
        /// </summary>
        /// <returns>a list of the available instruments</returns>
        public static List<Instrument> GetInstruments()
        {
            const string requestString = ApiServer + "v1/instruments";

            var responseString = MakeRequest(requestString);

            var serializer = new JavaScriptSerializer();
            var instrumentResponse = serializer.Deserialize<InstrumentsResponse>(responseString);

            var instruments = new List<Instrument>();
            instruments.AddRange(instrumentResponse.Instruments);

            return instruments;
        }

        /// <summary>Gets the current rates for the given instruments
        ///     NOTE: For repeated requests, use PollRatesSession (and StartRatesSession)</summary>
        /// <param name="instruments">The list of instruments to request</param>
        /// <returns>The list of prices</returns>
        public static List<Price> GetRates(List<Instrument> instruments)
        {
            var requestBuilder = new StringBuilder(ApiServer + "v1/instruments/price?instruments=");

            foreach (var instrument in instruments)
            {
                requestBuilder.Append(instrument.instrument + ",");
            }

            // Grab the string and remove the trailing comma
            var requestString = requestBuilder.ToString().Trim(',');

            var responseString = MakeRequest(requestString);

            var serializer = new JavaScriptSerializer();
            var pricesResponse = serializer.Deserialize<PricesResponse>(responseString);
            var prices = new List<Price>();
            prices.AddRange(pricesResponse.Prices);

            return prices;
        }

        /// <summary>Poll the specified rates session</summary>
        /// <param name="sessionId">The ID for the rates session (obtained using StartRatesSession)</param>
        /// <returns>list of prices that have changed since the last poll request (empty list if nothing changed)</returns>
        public static List<Price> PollRatesSession(long sessionId)
        {
            var requestString = ApiServer + "v1/instruments/poll?sessionId=" + sessionId;
            var responseString = MakeRequest(requestString);

            var serializer = new JavaScriptSerializer();

            var pricesResponse = serializer.Deserialize<PricesResponse>(responseString);
            var prices = new List<Price>();
            if (pricesResponse.Prices != null)
            {
                prices.AddRange(pricesResponse.Prices);
            }

            return prices;
        }

        /// <summary>Initializes a rates session that can then be polled</summary>
        /// <param name="instruments">the list of instruments to be polled</param>
        /// <returns>the ID of the rates session</returns>
        public static long StartRatesSession(List<Instrument> instruments)
        {
            const string requestString = ApiServer + "v1/instruments/poll";

            var pollRequest = new PricePollRequest { prices = new List<string>() };
            foreach (var instrument in instruments)
            {
                pollRequest.prices.Add(instrument.instrument);
            }

            var serializer = new JavaScriptSerializer();

            var request = WebRequest.CreateHttp(requestString);
            request.Method = "POST";
            using (var writer = new StreamWriter(request.GetRequestStream()))
            {
                writer.Write(serializer.Serialize(pollRequest));
            }

            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseString = reader.ReadToEnd().Trim();

                    var sessionResponse = serializer.Deserialize<SessionResponse>(responseString);
                    return sessionResponse.SessionId;
                }
            }
        }

        /// <summary>The initialize.</summary>
        public override void Initialize()
        {
            base.Initialize();

            var instruments = GetInstruments();
            var session = StartRatesSession(instruments);
            var prices = PollRatesSession(session);
            var response = GetCandles("EUR_USD");
        }

        #endregion

        #region Methods

        /// <summary>send a request and retrieve the response</summary>
        /// <param name="requestString">the request to send</param>
        /// <returns>the response string</returns>
        private static string MakeRequest(string requestString)
        {
            var request = WebRequest.CreateHttp(requestString);

            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var responseString = reader.ReadToEnd().Trim();

                    return responseString;
                }
            }
        }

        #endregion
    }
}
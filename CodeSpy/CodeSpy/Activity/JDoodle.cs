﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.IO;

using Xamarin.Forms;

namespace CodeSpy
{
    class JDoodle
    {
        private const string endPoint =
            "https://api.jdoodle.com/v1/execute";
        private const string endPointCreditsCheck =
            "https://api.jdoodle.com/v1/credit-spent";

        internal MessageToCompile infoToCompileMessage;

        private  MessageToCheck creditsInfoMessage;

        private HttpWebRequest request;

        public CompilerOutputInfo compilerResult;

        private string json;
        private string result;

        public JDoodle()
        {
            infoToCompileMessage = new MessageToCompile
            {
                clientId = "<ID>",
                clientSecret =
                    "<SECRET>",
                stdin = "",
                language = "",
                versionIndex = ""
            };

            creditsInfoMessage = new MessageToCheck
            {
                clientId = "<ID>",
                clientSecret =
                    "<SECRET>"
            };

            compilerResult = new CompilerOutputInfo();
        }

        public async void SendMessage(string _code)
        {
            infoToCompileMessage.script = _code;

            json = JsonConvert.SerializeObject(infoToCompileMessage);

            request = (HttpWebRequest)WebRequest.Create(endPoint);
            request.ContentType = "application/json";
            request.Method = "POST";

            var stream = await request.GetRequestStreamAsync();

            using (var writer = new StreamWriter(stream))
            {
                writer.Write(json);
                writer.Flush();
                writer.Dispose();
            }
            
            var response = await request.GetResponseAsync();

            var respStream = response.GetResponseStream();

            using (StreamReader sr = new StreamReader(respStream))
            {
                result = sr.ReadToEnd();

                var resultExample = JsonConvert.DeserializeObject<CompilerOutputInfo>(result);   

                compilerResult.output = resultExample.output;
                compilerResult.statusCode = resultExample.statusCode;
                compilerResult.memory = resultExample.memory;
                compilerResult.cpuTime = resultExample.cpuTime;
            }
        }
    }
}

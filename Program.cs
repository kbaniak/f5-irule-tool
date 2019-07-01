using System;
using System.IO;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;

// local modules
using Getopt;
using Datadefs;
using CoreiRuleUtils;

namespace irule_tool
{
   
    class Program
    {
        const string headline = "+ irule rest client version 1.0.2, (C) 2017 Krystian Baniak";
        static void Main(string[] args)
        {
            Console.WriteLine(headline);
            Config Cfg = new Config();
            OptionSet Opts = new OptionSet("q:ht:P:db:u:p:a:");
            Opts.Parse(args);
            if (Opts.hasActiveOption('h')) {
                PrintHelp();
                Environment.Exit(0);
            }
            try {
                foreach (Option k in Opts.opts) 
                {
                    if (k.present) {
                        switch (k.opt) {
                          case 'u':
                            {
                                if (k.val != "") Cfg.User = k.val; 
                            }
                            break;
                          case 'p':
                            {
                                if (k.val != "") Cfg.Secret = k.val;
                            }
                            break;  
                          case 'q': 
                            {
                                if (k.val != "") Cfg.Query = k.val;
                            }
                            break;
                          case 'd':
                            Cfg.Debug = true;
                            break;
                          case 't':
                            Cfg.Host = k.val;
                            break;
                          case 'b':
                            {
                                if (File.Exists(k.val)) {
                                    Cfg.setBatchMode(k.val);
                                } else {
                                    throw new Exception("Batch file cannot be found: " + k.val);
                                }
                            }
                            break;
                        }
                    }
                }
                
                // AUDIT MODE
                if (Opts.hasActiveOption('a')) {
                    if (!Opts.hasActiveOption('t')) {
                        throw new Exception("Cannot proceed without specifying a target host");
                    }
                    var fname = Opts.opts.Find(x => x.opt == 'a');
                    var tgt = Opts.opts.Find(x => x.opt == 't');
                    Console.WriteLine("+ generating configuration audit to file: {0}", fname.val);
                    Utils.CreateDataBook(fname.val, Cfg, tgt.val);
                    Environment.Exit(0);
                }

                // BATCH MODE
                if (Cfg.bflag) {
                    if (!Opts.hasActiveOption('t')) {
                        throw new Exception("Cannot run batch without specifying a target host");
                    }
                    if (Cfg.IdentifyTarget(Opts.opts.Find(x => x.opt == 't'))) {
                      Batch hBtch = Cfg.OpenBatch();
                      hBtch.RunStepSet("default");
                    }
                    Console.WriteLine(". finished batch mode");
                    Environment.Exit(0);
                }
            } catch (Exception ex) {
                Console.WriteLine("! fatal exception: {0}", ex.Message);
                Environment.Exit(3);
            }
            ProcessiControlREST(Cfg).Wait();
        }
        private static void PrintHelp()
        {
            Console.Write(
                "++ usage guidelines:\n" +
                "  -h             : print help message\n" +
                "  -q path        : set query path, like: ltm/rule\n" +
                "  -t host        : set target F5 device IP address\n" +
                "  -P port        : set target host post\n" +
                "  -b batch       : run a batch set\n" +
                "  -u user        : default is admin\n" +
                "  -p password    : default is admin\n" +
                "  -a file.xlsx   : save running config in a databook file\n"
            );
        }
        private static async Task ProcessiControlREST(Config cfg)
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("UserAgent", "iCR Agent 1.0");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                        string.Format("{0}:{1}", cfg.User, cfg.Secret)
                    )
                )
            );
            try {
              var sTask = client.GetStreamAsync("https://" + cfg.Host + cfg.Query);
              Console.WriteLine("+ using target: {0}, query: {1}", cfg.Host, cfg.Query);
              var serializer = new DataContractJsonSerializer(typeof(iCRresponse<iCRitem>));
              var strm = Utils.CopyDebugStream(await sTask);
              var resp = serializer.ReadObject(strm) as iCRresponse<iCRitem>;              
              if (cfg.Debug) strm.Position = 0;
              Console.WriteLine("response: {0} --> {1}", resp.kind, (new StreamReader(strm)).ReadToEnd());
              if (resp.items != null) {
                foreach (var it in resp.items) {
                    if (it.reference != null) {
                            Console.WriteLine("  reference: {0}", it.reference.link);
                        } else {
                            Console.WriteLine("  item: {0} {1} gen: {2}", it.kind, it.name, it.generation);
                        }
                    }
                }
            } catch (HttpRequestException e) {
              Console.WriteLine("Network exception: {0} --> {1}", e.Message, e);
            }
        }
    }
}

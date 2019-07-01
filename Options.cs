
using System;
using System.Text;
using System.Collections.Generic;

namespace Getopt
{
    public class Option
    {
        public char opt;
        public bool ext;
        public string val;
        public bool present;
        public string description;
        public Option(char token, bool extended)
        {
            opt = token;
            ext = extended;
            val = "";
            present = false;
        }
        public Option(char token, bool extended, string vals)
        {
            opt = token;
            ext = extended;
            val = vals;
            present = false;
        }
    }
    public class OptionSet
    {
        public List<Option> opts;
        private string optset;
        private bool debug = false;
        private Option findOption(char opt)
        {
            foreach (Option k in opts)
            {
                if (k.opt == opt) return k;
            }
            return null;
        }
        public bool hasActiveOption(char opt)
        {
            foreach (Option k in opts)
            {
                if (k.opt == opt && k.present) return true;
            }
            return false;
        }
        public OptionSet(string optstring)
        {
            optset = optstring;
            opts = new List<Option>();
            if (debug) Console.WriteLine("+ options: {0}", optset);
            for (int ix=0;ix<optset.Length;ix++) {
               bool isExtended = false;
               if (optset[ix] == ':') continue;
               if (((ix+1) < optset.Length) && (optset[ix+1] == ':')) {
                  isExtended = true;
               }
               if (debug) Console.WriteLine("  opt: {0} ex:{1}", optset[ix], isExtended);
               opts.Add(new Option(optset[ix], isExtended));  
            }
        }
        public void Parse(string[] input)
        {
            if (debug) Console.WriteLine("+ parsing command line options");
            try {
              if (input != null) {
                bool getval = false;
                Option lastopt = null;
                foreach (string item in input) {
                    if (getval) {
                        lastopt.val = item;
                        lastopt.present = true;
                        getval = false;
                        if (debug) Console.WriteLine("  . option: {0} --> {1}", lastopt.opt, item);
                    } else {
                        if (item.StartsWith("-")) {
                            Option po = findOption(item[1]);
                            if (po != null) {
                                if (po.ext) { 
                                    getval = true;
                                    lastopt = po;
                                } else {
                                    if (debug) Console.WriteLine("  . option: {0}", item);
                                    po.present = true;
                                }
                            } else throw new Exception("unknown option: " + item);
                        } else throw new Exception("invalid option: " + item);
                    }
                }
              }
            } catch (Exception ex) {
              Console.WriteLine("! failed to parse command line: {0}", ex.Message);
              Environment.Exit(2);
            }
        }
	}
}
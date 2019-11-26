using Newtonsoft.Json.Linq;
using System;
using System.Globalization;

namespace PerformanceBiller
{
    public class Statement
    {
        public string Run(JObject invoice, JObject plays)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = $"Statement for {invoice.GetValue("customer")}\n";
            var cultureInfo = new CultureInfo("en-US");

            foreach (JObject perf in invoice.GetValue("performances")) {
                var play = (JObject) plays.GetValue(perf.GetValue("playID").ToString());
                var thisAmount = 0;
                //armazenar gets em variaveis 
                var valorAudience =  Convert.ToInt32(perf.GetValue("audience"));
                var valorType = play.GetValue("type").ToString();
                

                switch (valorType) {
                    case "tragedy":
                        if (valorAudience > 30) {
                            thisAmount += 40000 + (1000 * (valorAudience - 30));
                        }
                        break;
                    case "comedy":
                        if (valorAudience > 20) {
                            thisAmount += 40000 + (500 * (valorAudience - 20));
                        }
                        thisAmount += 300 * valorAudience;
                        break;
                    default:
                        throw new Exception($"unknown type: {valorType}");
                }
                // add volume credits
                volumeCredits += Math.Max(valorAudience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == valorType) volumeCredits += valorAudience / 5;
                // print line for this order
                result += $" {play.GetValue("name")}: {(thisAmount/100).ToString("C", cultureInfo)} ({perf.GetValue("audience")} seats)\n";
                totalAmount += thisAmount;
             }
             result += $"Amount owed is {(totalAmount/100).ToString("C", cultureInfo)}\n";
             result += $"You earned {volumeCredits} credits\n";

             return result;
        }
    }
}

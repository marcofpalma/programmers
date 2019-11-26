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

            foreach (JObject ListforPerformances in invoice.GetValue("performances")) {
                var thisAmount = 0;
                //store gets into variables
                var valorforPlay = (JObject)plays.GetValue(ListforPerformances.GetValue("playID").ToString());
                var valorforAudience =  Convert.ToInt32(ListforPerformances.GetValue("audience"));
                var valorforType = valorforPlay.GetValue("type").ToString();
                

                switch (valorforType) {
                    case "tragedy":
                        thisAmount = 40000;
                        if (valorforAudience > 30) {
                            thisAmount += 1000 * (valorforAudience - 30);
                        }
                        break;
                    case "comedy":
                        thisAmount = 30000;
                        if (valorforAudience > 20) {
                            thisAmount += 10000 + (500 * (valorforAudience - 20));
                        }
                        thisAmount += 300 * valorforAudience;
                        break;
                    default:
                        throw new Exception($"unknown type: {valorforType}");
                } // end switch (valorforType)


                // add volume credits
                volumeCredits += Math.Max(valorforAudience - 30, 0);
                
                
                // add extra credit for every ten comedy attendees
                if ("comedy" == valorforType) volumeCredits += valorforAudience / 5;
                
                
                // print line for this order
                result += $" {valorforPlay.GetValue("name")}: {(thisAmount/100).ToString("C", cultureInfo)} ({ListforPerformances.GetValue("audience")} seats)\n";
                totalAmount += thisAmount;
             }
             result += $"Amount owed is {(totalAmount/100).ToString("C", cultureInfo)}\n";
             result += $"You earned {volumeCredits} credits\n";

             return result;
        }
    }
  
}

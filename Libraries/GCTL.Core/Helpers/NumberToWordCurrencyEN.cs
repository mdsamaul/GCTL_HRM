
namespace GCTL.Core.Helpers
{
    public static class NumberToWordCurrencyEN
    {
        public static string NumberInWords(string AmountInNumber)
        {
            return ChangeToWords(AmountInNumber, true);
        }

        private static string ChangeToWords(string number, bool isCurrency)
        {
            string val = "", wholeNo = number, points = "", andStr = "", pointStr = "";
            string endStr = (isCurrency) ? ("Only") : ("");
            try
            {
                int decimalPlace = number.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = number.Substring(0, decimalPlace);
                    points = number.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) > 0)
                    {
                        // andStr = (isCurrency) ? ("and") : ("point");// just to separate whole numbers from points/cents
                        andStr = (isCurrency) ? ("Taka and ") : ("point");// just to separate whole numbers from points/cents
                                                                          // endStr = (isCurrency) ? ("Cents " + endStr) : ("");
                        endStr = (isCurrency) ? ("Paisa " + endStr) : ("");
                        pointStr = TranslateCents(points);
                    }
                }
                val = string.Format("BDT {0} {1}{2} {3}", TranslateWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch {; }
            return val;
        }
        private static string TranslateWholeNumber(string number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                // if (dblAmt > 0)
                if (dblAmt != 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = number.StartsWith("0");

                    int numDigits = number.Length;
                    int pos = 0;//store digit grouping
                    string place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range
                            word = Ones(number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = Tens(number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            //place = " Hundred "; commented on 18/11/2019
                            if (number.ToString().Substring(0, 1) == "0")
                            {
                                place = "";
                            }
                            else
                            {
                                place = " Hundred ";
                            }
                            break;
                        case 4://thousands' range
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 5:
                            pos = (numDigits % 4) + 1;
                            //place = " Thousands ";  commented on 18/11/2019
                            if (number.ToString().Substring(0, 1) == "0")
                            {
                                place = "";
                            }
                            else
                            {
                                place = " Thousands ";
                            }
                            break;
                        case 6:
                            pos = (numDigits % 6) + 1;
                            place = " Lac ";
                            break;
                        case 7://millions' range
                            pos = (numDigits % 6) + 1;
                            // place = " Lacs "; commented on 18/11/2019
                            if (number.ToString().Substring(0, 1) == "0")
                            {
                                place = "";
                            }
                            else
                            {
                                place = " Lacs ";
                            }
                            break;
                        case 8:
                        case 9:
                            // pos = (numDigits % 6) + 1;
                            // place = " Million ";
                            // break;
                            pos = (numDigits % 8) + 1;
                            place = " Crore ";
                            break;
                        case 10://Billions's range
                                //pos = (numDigits % 10) + 1;
                            pos = (numDigits % 8) + 1;
                            place = " Crore ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                     //if (beginsZero)
                     //{
                     //    place = "";
                     //}
                     //if (s == 0)
                     //{
                     //    place = "";
                     //}

                        word = TranslateWholeNumber(number.Substring(0, pos)) + place + TranslateWholeNumber(number.Substring(pos));
                        //check for trailing zeros
                        if (word.ToString().Trim().Substring(0, 3) != "and")
                        {
                            if (beginsZero) word = " and " + word.Trim();
                        }
                        //if (beginsZero) word = "  and " + word.Trim();

                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch {; }
            return word.Trim();
        }
        static int s = 0;
        private static string Tens(string digit)
        {
            int digt = Convert.ToInt32(digit);
            string name = null;
            switch (digt)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Forty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (digt > 0)
                    {
                        name = Tens(digit.Substring(0, 1) + "0") + " " + Ones(digit.Substring(1));
                    }
                    if (digt == 0)
                    {
                        s = 0;
                    }
                    break;
            }
            return name;
        }
        private static string Ones(string digit)
        {
            int digt = Convert.ToInt32(digit);
            string name = "";
            switch (digt)
            {
                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }
        private static string TranslateCents(string cents)
        {

            // string strReturn = tens(cents);

            string cts = "", digit = "", engOne = "";

            if (cents.Length == 1)
            {
                cents = cents.PadRight(2, '0');
            }

            //for (int i = 0; i < cents.Length; i++)
            //{
            //    digit = cents[i].ToString();
            //    if (digit.Equals("0"))
            //    {
            //        engOne = "Zero";
            //    }
            //    else
            //    {
            //        engOne =ones(digit);
            //    }
            //    cts += " " + engOne;
            //}

            if (digit.Equals("0"))
            {
                engOne = "Zero";
            }
            else
            {
                engOne = Tens(cents);
            }

            return engOne;
        }
    }
}
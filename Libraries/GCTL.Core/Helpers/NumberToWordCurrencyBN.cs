
namespace GCTL.Core.Helpers
{
    public class NumberToWordCurrencyBN
    {
        public NumberToWordCurrencyBN()
        {
        }

        public string Words_Money(decimal num)
        {
            string functionReturnValue = null;
            decimal dollars = 0;
            int cents = 0;
            string dollars_result = null;
            string cents_result = null;

            // Dollars.
            dollars = Convert.ToInt64(num);
            dollars_result = Words_1_all(dollars);
            if (dollars_result.Length == 0)
                dollars_result = "শূন্য";
            //    "zero"

            // "one" Then
            if (dollars_result == "এক")
            {
                dollars_result = dollars_result + "টাকা";
                //" dollar"
            }
            //else
            //{
            //    dollars_result = dollars_result + "টাকা";
            //}

            // Cents.
            cents = Convert.ToInt32((num - dollars) * 100);
            cents_result = Words_1_all(cents);
            if (dollars_result.Length == 0)
                cents_result = "শূন্য";
            // "zero"

            // "one" Then
            if (cents_result == "এক")
            {
                cents_result = cents_result + " পয়সা";
                // " cent"
            }
            else
            {
                cents_result = cents_result + " পয়সা";
            }

            // Combine the results.
            if (cents > 0)
            {
                functionReturnValue = dollars_result + " এবং " + cents_result;
            }
            else
            {
                functionReturnValue = dollars_result;
            }
            return functionReturnValue;
        }
        private string Words_1_99(int num)
        {
            string result = null;
            int tens = 0;
            int tens11 = 0;

            tens = num / 10;

            if (tens <= 1)
            {
                // 1 <= num <= 19
                result = result + " " + Words_1_19(num);
            }
            else
            {
                // 20 <= num
                // Get the tens digit word.
                switch (tens)
                {
                    case 2:
                        tens11 = num % 10;
                        if (tens11 == 0)
                        {
                            result = "বিশ";
                            // "twenty"
                        }
                        else
                        {
                            switch (tens11)
                            {
                                case 1:
                                    result = "একুশ";
                                    // "twenty one"
                                    break;
                                case 2:
                                    result = "বাইশ";
                                    // "twenty two"
                                    break;
                                case 3:
                                    result = "তেইশ";
                                    // "twenty three"
                                    break;
                                case 4:
                                    result = "চব্বিশ";
                                    // "twenty four"
                                    break;
                                case 5:
                                    result = "পঁচিশ";
                                    // "twenty five"
                                    break;
                                case 6:
                                    result = "ছাব্বিশ";
                                    // "twenty six"
                                    break;
                                case 7:
                                    result = "সাতাইশ";
                                    // "twenty seven"
                                    break;
                                case 8:
                                    result = "আটাইশ";
                                    // "twenty eight"
                                    break;
                                case 9:
                                    result = "ঊনত্রিশ";
                                    // "twenty nine"
                                    break;
                            }
                        }
                        break;
                    case 3:

                        tens11 = num % 10;
                        if (tens11 == 0)
                        {
                            result = "ত্রিশ";
                            // "thirty"
                        }
                        else
                        {
                            switch (tens11)
                            {
                                case 1:
                                    result = "একত্রিশ";
                                    // "thirty one"
                                    break;
                                case 2:
                                    result = "বত্রিশ";
                                    // "thirty two"
                                    break;
                                case 3:
                                    result = "তেত্রিশ";
                                    // "thirty three"
                                    break;
                                case 4:
                                    result = "চৌত্রিশ";
                                    // "thirty four"
                                    break;
                                case 5:
                                    result = "পঁয়ত্রিশ";
                                    // "thirty five"
                                    break;
                                case 6:
                                    result = "ছত্রিশ";
                                    // "thirty six"
                                    break;
                                case 7:
                                    result = "সাইত্রিশ";
                                    // "thirty seven"
                                    break;
                                case 8:
                                    result = "আটত্রিশ";
                                    // "thirty eight"
                                    break;
                                case 9:
                                    result = "ঊনত্রিশ";
                                    // "thirty nine"
                                    break;
                            }
                        }
                        break;
                    case 4:
                        //                result = "Pwj­k"  ' "forty"
                        tens11 = num % 10;
                        if (tens11 == 0)
                        {
                            result = "চল্লিশ";
                            // "forty"
                        }
                        else
                        {
                            switch (tens11)
                            {
                                case 1:
                                    result = "একচল্লিশ";
                                    // "forty one"
                                    break;
                                case 2:
                                    result = "বিয়াল্লিশ";
                                    // "forty two"
                                    break;
                                case 3:
                                    result = "তেতাল্লিশ";
                                    // "forty three"
                                    break;
                                case 4:
                                    result = "চুয়াল্লিশ";
                                    // "forty four"
                                    break;
                                case 5:
                                    result = "পঁয়তাল্লিশ";
                                    // "forty five"
                                    break;
                                case 6:
                                    result = "ছেচল্লিশ";
                                    // "forty six"
                                    break;
                                case 7:
                                    result = "সাতচল্লিশ";
                                    // "forty seven"
                                    break;
                                case 8:
                                    result = "আটচল্লিশ";
                                    // "forty eight"
                                    break;
                                case 9:
                                    result = "ঊনপঞ্চাশ";
                                    // "forty nine"
                                    break;
                            }
                        }
                        break;
                    case 5:

                        tens11 = num % 10;
                        if (tens11 == 0)
                        {
                            result = "পঞ্চাশ";
                            // "fifty"
                        }
                        else
                        {
                            switch (tens11)
                            {
                                case 1:
                                    result = "একান্ন";
                                    // "fifty one"
                                    break;
                                case 2:
                                    result = "বায়ান্ন";
                                    // "fifty two"
                                    break;
                                case 3:
                                    result = "তেপ্পান্ন";
                                    // "fifty three"
                                    break;
                                case 4:
                                    result = "চুয়ান্ন";
                                    // "fifty four"
                                    break;
                                case 5:
                                    result = "পঞ্চান্ন";
                                    // "fifty five"
                                    break;
                                case 6:
                                    result = "ছাপ্পান্ন";
                                    // "fifty six"
                                    break;
                                case 7:
                                    result = "সাতান্ন";
                                    // "fifty seven"
                                    break;
                                case 8:
                                    result = "আটান্ন";
                                    // "fifty eight"
                                    break;
                                case 9:
                                    result = "ঊনষাট";
                                    // "fifty nine"
                                    break;
                            }
                        }
                        break;
                    case 6:

                        tens11 = num % 10;
                        if (tens11 == 0)
                        {
                            result = "ষাট";
                            // "sixty"
                        }
                        else
                        {
                            switch (tens11)
                            {
                                case 1:
                                    result = "একষট্টি";
                                    // "sixty one"
                                    break;
                                case 2:
                                    result = "বাষট্টি";
                                    // "sixty two"
                                    break;
                                case 3:
                                    result = "তেষট্টি";
                                    // "sixty three"
                                    break;
                                case 4:
                                    result = "চৌষট্টি";
                                    // "sixty four"
                                    break;
                                case 5:
                                    result = "পঁয়ষট্টি";
                                    // "sixty five"
                                    break;
                                case 6:
                                    result = "ছেষট্টি";
                                    // "sixty six"
                                    break;
                                case 7:
                                    result = "সাতষট্টি";
                                    // "sixty seven"
                                    break;
                                case 8:
                                    result = "আটষট্টি";
                                    // "sixty eight"
                                    break;
                                case 9:
                                    result = "ঊনসত্তর";
                                    // "sixty nine"
                                    break;
                            }
                        }
                        break;
                    case 7:

                        tens11 = num % 10;
                        if (tens11 == 0)
                        {
                            result = "সত্তর";
                            // "seventy"
                        }
                        else
                        {
                            switch (tens11)
                            {
                                case 1:
                                    result = "একাত্তর";
                                    // "seventy one"
                                    break;
                                case 2:
                                    result = "বায়াত্তর";
                                    // "seventy two"
                                    break;
                                case 3:
                                    result = "তিয়াত্তর";
                                    // "seventy three"
                                    break;
                                case 4:
                                    result = "চুয়াত্তর";
                                    // "seventy four"
                                    break;
                                case 5:
                                    result = "পঁচাত্তর";
                                    // "seventy five"
                                    break;
                                case 6:
                                    result = "ছিয়াত্তর";
                                    // "seventy six"
                                    break;
                                case 7:
                                    result = "সাতাত্তর";
                                    // "seventy seven"
                                    break;
                                case 8:
                                    result = "আটাত্তর";
                                    // "seventy eight"
                                    break;
                                case 9:
                                    result = "ঊনআশি";
                                    // "seventy nine"
                                    break;
                            }
                        }
                        break;
                    case 8:

                        tens11 = num % 10;
                        if (tens11 == 0)
                        {
                            result = "আশি";
                            // "eighty"
                        }
                        else
                        {
                            switch (tens11)
                            {
                                case 1:
                                    result = "একাশি";
                                    // "eighty one"
                                    break;
                                case 2:
                                    result = "বিরাশি";
                                    // "eighty two"
                                    break;
                                case 3:
                                    result = "তিরাশি";
                                    // "eighty three"
                                    break;
                                case 4:
                                    result = "চুরাশি";
                                    // "eighty four"
                                    break;
                                case 5:
                                    result = "পঁচাশি";
                                    // "eighty five"
                                    break;
                                case 6:
                                    result = "ছিয়াশি";
                                    // "eighty six"
                                    break;
                                case 7:
                                    result = "সাতাশি";
                                    // "eighty seven"
                                    break;
                                case 8:
                                    result = "আটাশি";
                                    // "eighty eight"
                                    break;
                                case 9:
                                    result = "ঊননব্বই";
                                    // "eighty nine"
                                    break;
                            }
                        }
                        break;
                    case 9:

                        tens11 = num % 10;
                        if (tens11 == 0)
                        {
                            result = "নব্বই";
                            // "ninety"
                        }
                        else
                        {
                            switch (tens11)
                            {
                                case 1:
                                    result = "একানব্বই";
                                    // "ninety one"
                                    break;
                                case 2:
                                    result = "বিরানব্বই";
                                    // "ninety two"
                                    break;
                                case 3:
                                    result = "তিরানব্বই";
                                    // "ninety three"
                                    break;
                                case 4:
                                    result = "চুরানব্বই";
                                    // "ninety four"
                                    break;
                                case 5:
                                    result = "পঁচানব্বই";
                                    // "ninety five"
                                    break;
                                case 6:
                                    result = "ছিয়ানব্বই";
                                    // "ninety six"
                                    break;
                                case 7:
                                    result = "সাতানব্বই";
                                    // "ninety seven"
                                    break;
                                case 8:
                                    result = "আটানব্বই";
                                    // "ninety eight"
                                    break;
                                case 9:
                                    result = "নিরানব্বই";
                                    // "ninety nine"
                                    break;
                            }
                        }
                        break;
                }

                // Add the ones digit number.
                //result = result;
                // & " " & Words_1_19(num - tens * 10)
            }

            return result.Trim();
        }
        private string Words_1_999(int num)
        {
            int hundreds = 0;
            int remainder = 0;
            string result = null;

            hundreds = num / 100;
            remainder = num - hundreds * 100;

            if (hundreds > 0)
            {
                result = Words_1_99(hundreds) + " শত ";
                // " hundred "
            }

            if (remainder > 0)
            {
                result = result + Words_1_99(remainder);
            }

            return result.Trim();
        }
        private string Words_1_all(decimal num)
        {
            decimal[] power_value = new decimal[5];
            string[] power_name = new string[5];
            int digits = 0;
            string result = "";


            power_name[0] = "ট্রিলিয়ন";

            power_value[0] = 1000000000000;

            power_name[1] = "কোটি";

            power_value[1] = 10000000;

            power_name[2] = "লক্ষ";

            power_value[2] = 100000;

            power_name[3] = "হাজার";

            power_value[3] = 1000;

            power_name[4] = "";

            power_value[4] = 1;



            for (int i = 0; i < 5; i++)
            {
                if (num >= power_value[i])
                {
                    digits = (int)(num / power_value[i]);

                    if (result.Length > 0)
                    {
                        result = result + ", ";

                    }

                    result = result + Words_1_999(digits) + " " + power_name[i];


                    num = num - digits * power_value[i];

                }
            }
            return result.Trim();
        }
        private string Words_1_19(int num)
        {
            string functionReturnValue = null;
            switch (num)
            {
                case 1:
                    functionReturnValue = "এক";
                    // "one"
                    break;
                case 2:
                    functionReturnValue = "দুই";
                    // "two"
                    break;
                case 3:
                    functionReturnValue = "তিন";
                    // "three"
                    break;
                case 4:
                    functionReturnValue = "চার";
                    // "four"
                    break;
                case 5:
                    functionReturnValue = "পাঁচ";
                    // "five"
                    break;
                case 6:
                    functionReturnValue = "ছয়";
                    // "six"
                    break;
                case 7:
                    functionReturnValue = "সাত";
                    // "seven"
                    break;
                case 8:
                    functionReturnValue = "আট";
                    // "eight"
                    break;
                case 9:
                    functionReturnValue = "নয়";
                    // "nine"
                    break;
                case 10:
                    functionReturnValue = "দশ";
                    // "ten"
                    break;
                case 11:
                    functionReturnValue = "এগারো";
                    // "eleven"
                    break;
                case 12:
                    functionReturnValue = "বারো";
                    // "twelve"
                    break;
                case 13:
                    functionReturnValue = "তেরো";
                    // "thirteen"
                    break;
                case 14:
                    functionReturnValue = "চৌদ্দ";
                    // "fourteen"
                    break;
                case 15:
                    functionReturnValue = "পনেরো";
                    // "fifteen"
                    break;
                case 16:
                    functionReturnValue = "ষোলো";
                    // "sixteen"
                    break;
                case 17:
                    functionReturnValue = "সতেরো";
                    // "seventeen"
                    break;
                case 18:
                    functionReturnValue = "আঠারো";
                    // "eightteen"
                    break;
                case 19:
                    functionReturnValue = "ঊনিশ";
                    // "nineteen"
                    break;
            }
            return functionReturnValue;
        }
    }
}
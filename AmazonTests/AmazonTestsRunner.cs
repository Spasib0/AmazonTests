using System;
using System.Collections.Generic;
using System.Linq;

namespace AmazonTests
{
    class AmazonTestsRunner
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("\n\n\t\t Amazon Bucket test is: " + (new AmazonTagsData().Test() ? "PASSED" : "FAILED"));
            var _ = Console.ReadLine();
        }
    }
}
